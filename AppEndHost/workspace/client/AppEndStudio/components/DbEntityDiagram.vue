<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-body rounded rounded-2 border border-3 border-light fs-d8 p-0 bg-white scrollable position-relative">
            <div v-if="local.isLoading">
               
            </div>
            <div v-else-if="local.error" class="alert alert-danger m-3">
                {{ local.error }}
            </div>
            <div v-else ref="network" class="network-container"></div>
            
            <!-- Floating Control Panel -->
            <div v-if="!local.isLoading && !local.error" 
                 class="floating-panel shadow" 
                 :style="{ left: local.panelPosition.x + 'px', top: local.panelPosition.y + 'px' }"
                 @mousedown="startDrag"
                 ref="floatingPanel">
                <div class="panel-header bg-primary text-white px-2 py-1 d-flex justify-content-between align-items-center" style="cursor: move;">
                    <small><i class="fa-solid fa-cog"></i> Controls</small>
                    <button class="btn btn-sm btn-link text-white p-0" @click="local.panelCollapsed = !local.panelCollapsed" style="text-decoration: none;">
                        <i class="fa-solid" :class="local.panelCollapsed ? 'fa-chevron-down' : 'fa-chevron-up'"></i>
                    </button>
                </div>
                <div v-show="!local.panelCollapsed" class="panel-body bg-white p-2" style="min-width: 220px;">
                    <!-- Data Source -->
                    <div class="mb-3">
                        <label class="fw-bold small mb-1">Data Source</label>
                        <select class="form-select form-select-sm" v-model='local.selectedConnection' @change="loadDiagram">
                            <option value="DefaultRepo">DefaultRepo:MsSql</option>
                        </select>
                    </div>
                    
                    <hr class="my-2">
                    
                    <!-- Object Types -->
                    <div class="mb-3">
                        <div class="fw-bold small mb-2">Object Types</div>
                        <div class="form-check">
                            <input class="form-check-input" type="checkbox" id="chkTables" v-model="local.showTables" @change="loadDiagram">
                            <label class="form-check-label small d-flex align-items-center" for="chkTables">
                                <i class="fa-solid fa-table text-primary me-1"></i>
                                <span>Tables</span>
                            </label>
                        </div>
                        <div class="form-check">
                            <input class="form-check-input" type="checkbox" id="chkViews" v-model="local.showViews" @change="loadDiagram">
                            <label class="form-check-label small d-flex align-items-center" for="chkViews">
                                <i class="fa-solid fa-eye text-info me-1"></i>
                                <span>Views</span>
                            </label>
                        </div>
                        <div class="form-check">
                            <input class="form-check-input" type="checkbox" id="chkSPs" v-model="local.showStoredProcedures" @change="loadDiagram">
                            <label class="form-check-label small d-flex align-items-center" for="chkSPs">
                                <i class="fa-solid fa-cog text-success me-1"></i>
                                <span>Stored Procedures</span>
                            </label>
                        </div>
                        <div class="form-check">
                            <input class="form-check-input" type="checkbox" id="chkFuncs" v-model="local.showFunctions" @change="loadDiagram">
                            <label class="form-check-label small d-flex align-items-center" for="chkFuncs">
                                <i class="fa-solid fa-code text-warning me-1"></i>
                                <span>Functions</span>
                            </label>
                        </div>
                    </div>
                    
                    <hr class="my-2">
                    
                    <!-- Layout -->
                    <div class="mb-3">
                        <label class="fw-bold small mb-1">Layout</label>
                        <select class="form-select form-select-sm" v-model='local.layoutType' @change="applyLayout">
                            <option value="physics">Physics</option>
                            <option value="random">Random</option>
                            <option value="hierarchical-ud">Top to Down</option>
                            <option value="hierarchical-lr">Left to Right</option>
                            <option value="hierarchical-du">Down to Up</option>
                            <option value="hierarchical-rl">Right to Left</option>
                        </select>
                    </div>
                    
                    <hr class="my-2">
                    
                    <!-- View Controls -->
                    <div>
                        <div class="fw-bold small mb-1">View Controls</div>
                        <div class="d-grid gap-1">
                            <button class="btn btn-sm btn-outline-secondary" @click="fitView">
                                <i class="fa-solid fa-expand"></i> Fit to Screen
                            </button>
                            <div class="btn-group btn-group-sm" role="group">
                                <button class="btn btn-outline-secondary" @click="zoomOut">
                                    <i class="fa-solid fa-search-minus"></i> Out
                                </button>
                                <button class="btn btn-outline-secondary" @click="zoomIn">
                                    <i class="fa-solid fa-search-plus"></i> In
                                </button>
                            </div>
                            <button class="btn btn-sm btn-outline-primary" @click="loadDiagram">
                                <i class="fa-solid fa-sync"></i> Refresh
                            </button>
                        </div>
                    </div>
                </div>
            </div>
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
            views: [],
            storedProcedures: [],
            functions: [],
            network: null,
            layoutType: "physics",
            showTables: true,
            showViews: false,
            showStoredProcedures: false,
            showFunctions: false,
            panelPosition: { x: 20, y: 20 },
            panelCollapsed: false,
            isDragging: false,
            dragOffset: { x: 0, y: 0 }
        }
    };

    export default {
        data() {
            return _this;
        },
        methods: {
            startDrag(event) {
                if (event.target.closest('.panel-body')) {
                    return; // Don't drag if clicking on panel body content
                }
                
                this.local.isDragging = true;
                const panel = this.$refs.floatingPanel;
                const rect = panel.getBoundingClientRect();
                
                this.local.dragOffset = {
                    x: event.clientX - rect.left,
                    y: event.clientY - rect.top
                };
                
                document.addEventListener('mousemove', this.onDrag);
                document.addEventListener('mouseup', this.stopDrag);
            },
            
            onDrag(event) {
                if (!this.local.isDragging) return;
                
                const containerRect = this.$refs.network.getBoundingClientRect();
                
                let newX = event.clientX - containerRect.left - this.local.dragOffset.x;
                let newY = event.clientY - containerRect.top - this.local.dragOffset.y;
                
                // Keep panel within bounds
                const panelRect = this.$refs.floatingPanel.getBoundingClientRect();
                newX = Math.max(0, Math.min(newX, containerRect.width - panelRect.width));
                newY = Math.max(0, Math.min(newY, containerRect.height - panelRect.height));
                
                this.local.panelPosition = { x: newX, y: newY };
            },
            
            stopDrag() {
                this.local.isDragging = false;
                document.removeEventListener('mousemove', this.onDrag);
                document.removeEventListener('mouseup', this.stopDrag);
            },

            toggleObjectType(type) {
                this.local[type] = !this.local[type];
                this.loadDiagram();
            },

            async loadDiagram() {
                this.local.isLoading = true;
                this.local.error = null;

                try {
                    const objectTypes = [];
                    if (this.local.showTables) objectTypes.push('Table');
                    if (this.local.showViews) objectTypes.push('View');
                    if (this.local.showStoredProcedures) objectTypes.push('StoredProcedure');
                    if (this.local.showFunctions) objectTypes.push('Function');

                    console.log('Loading diagram with object types:', objectTypes);

                    await rpcAEP("GetDbObjectsForDiagram", 
                        { 
                            "DbConfName": this.local.selectedConnection,
                            "ObjectTypes": objectTypes.join(',')
                        },
                        (res) => {
                            res = R0R(res);
                            console.log('Received data:', res);
                            
                            // Group objects by type
                            this.local.tables = res.filter(obj => obj.ObjectType === 'Table' || !obj.ObjectType);
                            this.local.views = res.filter(obj => obj.ObjectType === 'View');
                            this.local.storedProcedures = res.filter(obj => obj.ObjectType === 'StoredProcedure');
                            this.local.functions = res.filter(obj => obj.ObjectType === 'Function');
                            
                            console.log('Grouped data:', {
                                tables: this.local.tables.length,
                                views: this.local.views.length,
                                storedProcedures: this.local.storedProcedures.length,
                                functions: this.local.functions.length
                            });
                            
                            this.local.isLoading = false;
                            
                            this.$nextTick(() => {
                                this.initNetwork();
                            });
                        }
                    );
                } catch (error) {
                    console.error('Error loading diagram:', error);
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
                
                // Helper function to create node style based on object type
                const getNodeStyle = (objectType) => {
                    const styles = {
                        'Table': {
                            background: '#f8f9fa',
                            border: '#333',
                            highlightBg: '#e3f2fd',
                            highlightBorder: '#0066cc',
                            shape: 'box'
                        },
                        'View': {
                            background: '#e3f2fd',
                            border: '#1976d2',
                            highlightBg: '#bbdefb',
                            highlightBorder: '#0d47a1',
                            shape: 'box'
                        },
                        'StoredProcedure': {
                            background: '#e8f5e9',
                            border: '#388e3c',
                            highlightBg: '#c8e6c9',
                            highlightBorder: '#1b5e20',
                            shape: 'box'
                        },
                        'Function': {
                            background: '#fff3e0',
                            border: '#f57c00',
                            highlightBg: '#ffe0b2',
                            highlightBorder: '#e65100',
                            shape: 'box'
                        }
                    };
                    return styles[objectType] || styles['Table'];
                };

                // Add table nodes
                const allObjects = [
                    ...this.local.tables.map(t => ({ ...t, ObjectType: 'Table' })),
                    ...this.local.views.map(v => ({ ...v, ObjectType: 'View' })),
                    ...this.local.storedProcedures.map(sp => ({ ...sp, ObjectType: 'StoredProcedure' })),
                    ...this.local.functions.map(f => ({ ...f, ObjectType: 'Function' }))
                ];

                allObjects.forEach(obj => {
                    const objectType = obj.ObjectType || 'Table';
                    const style = getNodeStyle(objectType);
                    
                    let label = `<b>${obj.Name}</b>`;
                    
                    if (objectType === 'Table' || objectType === 'View') {
                        label += '\n-------------\n';
                        
                        if (obj.Columns && obj.Columns.length > 0) {
                            const maxCols = 15; // Limit columns to avoid too large nodes
                            const displayColumns = obj.Columns.slice(0, maxCols);
                            
                            displayColumns.forEach(col => {
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
                            
                            if (obj.Columns.length > maxCols) {
                                label += `... and ${obj.Columns.length - maxCols} more columns\n`;
                            }
                        }
                    } else if (objectType === 'StoredProcedure' || objectType === 'Function') {
                        label += `\n[${objectType === 'StoredProcedure' ? 'SP' : 'Function'}]`;
                        if (obj.Parameters && obj.Parameters.length > 0) {
                            label += '\n-------------\n';
                            obj.Parameters.forEach(param => {
                                const paramName = param.Name || param.name || '';
                                const paramType = param.Type || param.type || param.DbType || '';
                                label += `${paramName}: ${paramType}\n`;
                            });
                        }
                    }

                    nodes.push({
                        id: obj.Name,
                        label: label,
                        shape: style.shape,
                        margin: 15,
                        font: {
                            face: 'Consolas, Monaco, monospace',
                            size: 13,
                            align: 'left',
                            multi: true
                        },
                        color: {
                            background: style.background,
                            border: style.border,
                            highlight: {
                                background: style.highlightBg,
                                border: style.highlightBorder
                            }
                        },
                        borderWidth: 2,
                        borderWidthSelected: 3,
                        objectType: objectType
                    });
                });

                // Add FK relationships for tables
                const addedRelations = new Set();
                this.local.tables.forEach(table => {
                    if (table.Columns) {
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
                                        },
                                        relationType: 'FK'
                                    });
                                    addedRelations.add(relKey);
                                }
                            }
                        });
                    }
                });

                // Add relationships for views to tables
                this.local.views.forEach(view => {
                    if (view.Dependencies && view.Dependencies.length > 0) {
                        view.Dependencies.forEach(dep => {
                            const relKey = `${dep}_${view.Name}_view_dependency`;
                            if (!addedRelations.has(relKey)) {
                                edges.push({
                                    from: dep,
                                    to: view.Name,
                                    arrows: 'to',
                                    color: {
                                        color: '#1976d2',
                                        highlight: '#0d47a1'
                                    },
                                    dashes: [5, 5],
                                    width: 2,
                                    font: {
                                        size: 9,
                                        align: 'middle'
                                    },
                                    relationType: 'ViewDependency'
                                });
                                addedRelations.add(relKey);
                            }
                        });
                    }
                });

                // Add relationships for stored procedures to tables
                this.local.storedProcedures.forEach(sp => {
                    if (sp.Dependencies && sp.Dependencies.length > 0) {
                        sp.Dependencies.forEach(dep => {
                            const relKey = `${sp.Name}_${dep}_sp_dependency`;
                            if (!addedRelations.has(relKey)) {
                                edges.push({
                                    from: sp.Name,
                                    to: dep,
                                    arrows: 'to',
                                    color: {
                                        color: '#388e3c',
                                        highlight: '#1b5e20'
                                    },
                                    dashes: [10, 5],
                                    width: 2,
                                    font: {
                                        size: 9,
                                        align: 'middle'
                                    },
                                    relationType: 'SPDependency'
                                });
                                addedRelations.add(relKey);
                            }
                        });
                    }
                });

                // Add relationships for functions to tables
                this.local.functions.forEach(func => {
                    if (func.Dependencies && func.Dependencies.length > 0) {
                        func.Dependencies.forEach(dep => {
                            const relKey = `${func.Name}_${dep}_func_dependency`;
                            if (!addedRelations.has(relKey)) {
                                edges.push({
                                    from: func.Name,
                                    to: dep,
                                    arrows: 'to',
                                    color: {
                                        color: '#f57c00',
                                        highlight: '#e65100'
                                    },
                                    dashes: [5, 10],
                                    width: 2,
                                    font: {
                                        size: 9,
                                        align: 'middle'
                                    },
                                    relationType: 'FunctionDependency'
                                });
                                addedRelations.add(relKey);
                            }
                        });
                    }
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
            // Cleanup drag event listeners
            document.removeEventListener('mousemove', this.onDrag);
            document.removeEventListener('mouseup', this.stopDrag);
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
    
    .floating-panel {
        position: absolute;
        z-index: 1000;
        border-radius: 6px;
        border: 1px solid #dee2e6;
        user-select: none;
    }
    
    .panel-header {
        border-radius: 6px 6px 0 0;
        font-size: 0.875rem;
    }
    
    .panel-body {
        border-radius: 0 0 6px 6px;
        font-size: 0.875rem;
    }
    
    .form-check {
        margin-bottom: 0.5rem;
        display: flex;
        align-items: center;
    }
    
    .form-check-label {
        cursor: pointer;
        user-select: none;
        display: flex;
        align-items: center;
        margin-bottom: 0;
    }
    
    .form-check-input {
        cursor: pointer;
        flex-shrink: 0;
        margin-top: 0;
        margin-bottom: 0;
    }
    
    .form-check-label i {
        width: 16px;
        text-align: center;
        font-size: 0.875rem;
        margin-left: 0.25rem;
    }
    
    .form-check-label span {
        line-height: 1.2;
    }
</style>
