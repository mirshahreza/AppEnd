<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-header p-2 bg-body-subtle rounded-0 border-0">
            <div class="hstack">
                <select id="dataSources" class="form-select form-select-sm" style="max-width:200px;" v-model='local.selectedConnection' @change="loadDiagram">
                    <option value="DefaultRepo">DefaultRepo:MsSql</option>
                </select>
                <div class="vr"></div>
                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="loadDiagram">
                    <i class="fa-solid fa-sync"></i> Refresh
                </button>
                <div class="vr"></div>
                <select class="form-select form-select-sm" style="max-width:225px;" v-model='local.layoutType' @change="applyLayout">
                    <option value="hierarchical-ud">Hierarchical (Top-Down)</option>
                    <option value="hierarchical-lr">Hierarchical (Left-Right)</option>
                    <option value="hierarchical-du">Hierarchical (Bottom-Up)</option>
                    <option value="hierarchical-rl">Hierarchical (Right-Left)</option>
                    <option value="physics">Physics (Force-Directed)</option>
                    <option value="random">Random Spread</option>
                </select>
                <div class="vr"></div>
                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="fitView">
                    <i class="fa-solid fa-expand"></i> Fit to Screen
                </button>
                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="zoomIn">
                    <i class="fa-solid fa-search-plus"></i>
                </button>
                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="zoomOut">
                    <i class="fa-solid fa-search-minus"></i>
                </button>
            </div>
        </div>
        <div class="card-body rounded rounded-2 border border-3 border-light fs-d8 p-0 bg-white scrollable">
            <div v-if="local.isLoading">
            </div>
            <div v-else-if="local.error" class="alert alert-danger m-3">
                {{ local.error }}
            </div>
            <div v-else ref="network" class="network-container"></div>
        </div>
    </div>
</template>

<script>
    shared.setAppTitle("$auto$");
    
    let _this = {
        cid: "",
        c: null,
        local: {
            selectedConnection: "DefaultRepo",
            isLoading: false,
            error: null,
            tables: [],
            network: null,
            layoutType: "hierarchical-ud"
        }
    };

    export default {
        data() {
            return _this;
        },
        methods: {
            async loadDiagram() {
                this.local.isLoading = true;
                this.local.error = null;

                try {
                    await rpcAEP("GetDbTables", 
                        { "DbConfName": this.local.selectedConnection },
                        (res) => {
                            res = R0R(res);
                            this.local.tables = res;
                            this.local.isLoading = false;
                            
                            this.$nextTick(() => {
                                this.initNetwork();
                            });
                        }
                    );
                } catch (error) {
                    this.local.error = error.message || "Failed to load database schema";
                    this.local.isLoading = false;
                }
            },

            initNetwork() {
                if (!window.vis) {
                    this.local.error = "vis.js library not loaded";
                    return;
                }
                
                const nodes = [];
                const edges = [];
                
                // Add table nodes
                this.local.tables.forEach(table => {
                    let label = `<b>${table.Name}</b>\n-------------\n`;
                    
                    table.Columns.forEach(col => {
                        let icon = '';
                        if (col.IsPrimaryKey) {
                            icon = '[PK] ';
                        } else if (col.Fk) {
                            icon = '[FK] ';
                        } else {
                            icon = '     ';
                        }
                        let type = col.DbType + (col.Size ? `(${col.Size})` : '');
                        label += `${icon}${col.Name}: ${type}\n`;
                    });

                    nodes.push({
                        id: table.Name,
                        label: label,
                        shape: 'box',
                        margin: 15,
                        font: {
                            face: 'Consolas, Monaco, monospace',
                            size: 13,
                            align: 'left',
                            multi: true
                        },
                        color: {
                            background: '#f8f9fa',
                            border: '#333',
                            highlight: {
                                background: '#e3f2fd',
                                border: '#0066cc'
                            }
                        },
                        borderWidth: 2,
                        borderWidthSelected: 3
                    });
                });

                // Add relationship edges
                const addedRelations = new Set();
                this.local.tables.forEach(table => {
                    table.Columns.forEach(col => {
                        if (col.Fk && col.Fk.TargetTable) {
                            const relKey = `${col.Fk.TargetTable}_${table.Name}_${col.Name}`;
                            
                            if (!addedRelations.has(relKey)) {
                                edges.push({
                                    from: col.Fk.TargetTable,
                                    to: table.Name,
                                    label: col.Name,
                                    arrows: 'to',
                                    color: {
                                        color: col.Fk.EnforceRelation ? '#0066cc' : '#999',
                                        highlight: '#0066cc'
                                    },
                                    dashes: !col.Fk.EnforceRelation,
                                    width: col.Fk.EnforceRelation ? 3 : 2,
                                    font: {
                                        size: 10,
                                        align: 'middle'
                                    }
                                });
                                addedRelations.add(relKey);
                            }
                        }
                    });
                });

                const data = {
                    nodes: new vis.DataSet(nodes),
                    edges: new vis.DataSet(edges)
                };

                const options = {
                    layout: this.getLayoutOptions(),
                    physics: {
                        enabled: this.local.layoutType === 'physics',
                        stabilization: {
                            enabled: true,
                            iterations: 200
                        },
                        barnesHut: {
                            gravitationalConstant: -3000,
                            centralGravity: 0.3,
                            springLength: 250,
                            springConstant: 0.04,
                            damping: 0.09,
                            avoidOverlap: 0.5
                        }
                    },
                    interaction: {
                        dragNodes: true,
                        dragView: true,
                        zoomView: true,
                        hover: true
                    },
                    edges: {
                        smooth: {
                            type: 'cubicBezier',
                            forceDirection: this.local.layoutType.startsWith('hierarchical-') ? 'vertical' : 'none',
                            roundness: 0.4
                        }
                    },
                    nodes: {
                        widthConstraint: {
                            minimum: 200,
                            maximum: 400
                        },
                        heightConstraint: {
                            minimum: 100
                        }
                    }
                };

                if (this.local.network) {
                    this.local.network.destroy();
                }

                this.local.network = new vis.Network(this.$refs.network, data, options);
                
                // Fit to view after layout stabilization
                this.local.network.once('stabilizationIterationsDone', () => {
                    this.local.network.fit({
                        animation: {
                            duration: 500,
                            easingFunction: 'easeInOutQuad'
                        }
                    });
                });
            },

            getLayoutOptions() {
                const layoutMap = {
                    'hierarchical-ud': { direction: 'UD', enabled: true },
                    'hierarchical-lr': { direction: 'LR', enabled: true },
                    'hierarchical-du': { direction: 'DU', enabled: true },
                    'hierarchical-rl': { direction: 'RL', enabled: true },
                    'physics': { enabled: false },
                    'random': { enabled: false }
                };

                const layoutConfig = layoutMap[this.local.layoutType];
                
                if (layoutConfig && layoutConfig.enabled) {
                    return {
                        hierarchical: {
                            enabled: true,
                            direction: layoutConfig.direction,
                            sortMethod: 'directed',
                            nodeSpacing: 250,
                            levelSeparation: 300,
                            treeSpacing: 300,
                            blockShifting: true,
                            edgeMinimization: true,
                            parentCentralization: true
                        }
                    };
                } else if (this.local.layoutType === 'physics') {
                    return {
                        hierarchical: false,
                        randomSeed: undefined
                    };
                } else {
                    return {
                        hierarchical: false,
                        randomSeed: Math.floor(Math.random() * 1000000)
                    };
                }
            },

            applyLayout() {
                if (!this.local.network) return;
                
                const isPhysics = this.local.layoutType === 'physics';
                
                this.local.network.setOptions({
                    layout: this.getLayoutOptions(),
                    physics: {
                        enabled: isPhysics,
                        stabilization: {
                            enabled: true,
                            iterations: 200
                        }
                    }
                });
                
                if (isPhysics) {
                    this.local.network.stabilize();
                }
                
                setTimeout(() => {
                    this.fitView();
                }, isPhysics ? 1500 : 600);
            },

            fitView() {
                if (this.local.network) {
                    this.local.network.fit({
                        animation: {
                            duration: 500,
                            easingFunction: 'easeInOutQuad'
                        }
                    });
                }
            },

            zoomIn() {
                if (this.local.network) {
                    const scale = this.local.network.getScale();
                    this.local.network.moveTo({
                        scale: scale * 1.2,
                        animation: {
                            duration: 200,
                            easingFunction: 'easeInOutQuad'
                        }
                    });
                }
            },

            zoomOut() {
                if (this.local.network) {
                    const scale = this.local.network.getScale();
                    this.local.network.moveTo({
                        scale: scale * 0.8,
                        animation: {
                            duration: 200,
                            easingFunction: 'easeInOutQuad'
                        }
                    });
                }
            }
        },
        setup(props) {
            _this.cid = props['cid'];
        },
        created() {
            _this.c = this;
        },
        mounted() {
            initVueComponent(_this);
            
            // Load vis.js library if not already loaded
            if (typeof vis === 'undefined') {
                // Load CSS first
                const link = document.createElement('link');
                link.rel = 'stylesheet';
                link.href = 'https://cdn.jsdelivr.net/npm/vis-network@9.1.6/dist/dist/vis-network.min.css';
                document.head.appendChild(link);
                
                // Then load JS
                const script = document.createElement('script');
                script.src = 'https://cdn.jsdelivr.net/npm/vis-network@9.1.6/dist/vis-network.min.js';
                script.onload = () => {
                    this.loadDiagram();
                };
                script.onerror = () => {
                    this.local.error = "Failed to load vis.js library";
                    this.local.isLoading = false;
                };
                document.head.appendChild(script);
            } else {
                this.loadDiagram();
            }
        },
        beforeUnmount() {
            if (this.local.network) {
                this.local.network.destroy();
            }
        },
        props: {
            cid: String
        }
    }
</script>

<style scoped>
    .network-container {
        width: 100%;
        height: 100%;
        min-height: 600px;
        background-color: #ffffff;
    }
</style>
