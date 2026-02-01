<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-body rounded rounded-2 border border-3 border-light fs-d8 p-0 bg-white scrollable position-relative">
            <div v-if="local.isLoading">
                <div class="d-flex justify-content-center align-items-center" style="height: 400px;">
                    <div class="text-center">
                        <div class="spinner-border text-primary mb-3" role="status" style="width: 3rem; height: 3rem;">
                            <span class="visually-hidden">Loading...</span>
                        </div>
                        <div class="text-muted">Loading database schema...</div>
                    </div>
                </div>
            </div>
            <div v-else-if="local.error" class="alert alert-danger m-3">
                {{ local.error }}
            </div>
            <div v-else ref="network" class="network-container"></div>
            
            <!-- Node Hover Toolbar -->
            <div v-if="local.nodeTooltip.visible" 
                 class="node-toolbar"
                 @mouseenter="local.nodeTooltip.hovering = true"
                 @mouseleave="checkTooltipHide"
                 :style="{ left: (local.nodeTooltip.x + 10) + 'px', top: (local.nodeTooltip.y - 70) + 'px' }">
                <div class="node-toolbar-label">{{ local.nodeTooltip.nodeName }}</div>
                <div class="node-toolbar-content">
                    <button class="node-toolbar-btn" @click="showObjectFields(local.nodeTooltip.nodeId)" title="Show All Fields">
                        <i class="fa-solid fa-list"></i>
                    </button>
                    <button class="node-toolbar-btn" @click="openObjectEditor(local.nodeTooltip.nodeId)" title="Edit Object">
                        <i class="fa-solid fa-pen-to-square"></i>
                    </button>
                    <button class="node-toolbar-btn" @click="toggleFocusMode(local.nodeTooltip.nodeId)" 
                            :title="local.isFocusMode ? 'Show All' : 'Focus on Related'">
                        <i class="fa-solid" :class="local.isFocusMode ? 'fa-eye' : 'fa-filter'"></i>
                    </button>
                </div>
            </div>
            
            <!-- Fixed Vertical Toolbar -->
            <div class="fixed-toolbar" :class="{ 'toolbar-disabled': local.isLoading }">
                <button class="toolbar-btn-vertical" @click="zoomIn" title="Zoom In" :disabled="local.isLoading">
                    <i class="fa-solid fa-search-plus"></i>
                </button>
                <button class="toolbar-btn-vertical" @click="fitView" title="Fit to Screen" :disabled="local.isLoading">
                    <i class="fa-solid fa-expand"></i>
                </button>
                <button class="toolbar-btn-vertical" @click="zoomOut" title="Zoom Out" :disabled="local.isLoading">
                    <i class="fa-solid fa-search-minus"></i>
                </button>
                
                <div class="toolbar-divider-horizontal"></div>
                
                <button class="toolbar-btn-vertical" @click="resetFocus" title="Show All Objects" :disabled="local.isLoading">
                    <i class="fa-solid fa-eye"></i>
                </button>
                <button class="toolbar-btn-vertical" @click="local.showLegendModal = true" v-if="local.showRelations && !local.isLoading" title="Show Legend">
                    <i class="fa-solid fa-info-circle"></i>
                </button>
                <button class="toolbar-btn-vertical" @click="exportAsImage" title="Export as PNG" :disabled="local.isLoading || local.error">
                    <i class="fa-solid fa-download"></i>
                </button>
                <button class="toolbar-btn-vertical" @click="loadDiagram" title="Refresh" :disabled="local.isLoading">
                    <i class="fa-solid fa-sync" :class="{ 'fa-spin': local.isLoading }"></i>
                </button>
            </div>
            
            <!-- Legend Modal -->
            <div v-if="local.showLegendModal" class="legend-modal-overlay" @click="local.showLegendModal = false">
                <div class="legend-modal" @click.stop>
                    <div class="legend-modal-header">
                        <h6 class="mb-0"><i class="fa-solid fa-diagram-project me-2"></i>Relationships Legend</h6>
                        <button class="btn btn-sm btn-link text-dark p-0" @click="local.showLegendModal = false">
                            <i class="fa-solid fa-times"></i>
                        </button>
                    </div>
                    <div class="legend-modal-body">
                        <div class="mb-2 text-muted small fst-italic">
                            Click on a node to highlight its connections
                        </div>
                        <div class="legend-item mb-2">
                            <svg width="50" height="2">
                                <line x1="0" y1="1" x2="50" y2="1" stroke="#0066cc" stroke-width="3"/>
                            </svg>
                            <span class="ms-2">FK (Enforced)</span>
                        </div>
                        <div class="legend-item mb-2">
                            <svg width="50" height="2">
                                <line x1="0" y1="1" x2="50" y2="1" stroke="#999" stroke-width="2" stroke-dasharray="5,5"/>
                            </svg>
                            <span class="ms-2">FK (Not Enforced)</span>
                        </div>
                        <div class="legend-item mb-2">
                            <svg width="50" height="2">
                                <line x1="0" y1="1" x2="50" y2="1" stroke="#1976d2" stroke-width="2" stroke-dasharray="5,5"/>
                            </svg>
                            <span class="ms-2">View → Table</span>
                        </div>
                        <div class="legend-item mb-2">
                            <svg width="50" height="2">
                                <line x1="0" y1="1" x2="50" y2="1" stroke="#64b5f6" stroke-width="1.5" stroke-dasharray="2,8"/>
                            </svg>
                            <span class="ms-2">View → View</span>
                        </div>
                        <div class="legend-item mb-2">
                            <svg width="50" height="2">
                                <line x1="0" y1="1" x2="50" y2="1" stroke="#42a5f5" stroke-width="2" stroke-dasharray="8,3,2,3"/>
                            </svg>
                            <span class="ms-2">View → Function</span>
                        </div>
                        <div class="legend-item mb-2">
                            <svg width="50" height="2">
                                <line x1="0" y1="1" x2="50" y2="1" stroke="#388e3c" stroke-width="2.5" stroke-dasharray="10,5"/>
                            </svg>
                            <span class="ms-2">SP → Table</span>
                        </div>
                        <div class="legend-item mb-2">
                            <svg width="50" height="2">
                                <line x1="0" y1="1" x2="50" y2="1" stroke="#66bb6a" stroke-width="2" stroke-dasharray="8,4,2,4"/>
                            </svg>
                            <span class="ms-2">SP → View</span>
                        </div>
                        <div class="legend-item mb-2">
                            <svg width="50" height="2">
                                <line x1="0" y1="1" x2="50" y2="1" stroke="#81c784" stroke-width="2" stroke-dasharray="3,6"/>
                            </svg>
                            <span class="ms-2">SP → Function</span>
                        </div>
                        <div class="legend-item mb-2">
                            <svg width="50" height="2">
                                <line x1="0" y1="1" x2="50" y2="1" stroke="#2e7d32" stroke-width="1.5" stroke-dasharray="2,10"/>
                            </svg>
                            <span class="ms-2">SP → SP</span>
                        </div>
                        <div class="legend-item mb-2">
                            <svg width="50" height="2">
                                <line x1="0" y1="1" x2="50" y2="1" stroke="#f57c00" stroke-width="2" stroke-dasharray="5,10"/>
                            </svg>
                            <span class="ms-2">Func → Table</span>
                        </div>
                        <div class="legend-item mb-2">
                            <svg width="50" height="2">
                                <line x1="0" y1="1" x2="50" y2="1" stroke="#ff9800" stroke-width="2" stroke-dasharray="6,3,2,3"/>
                            </svg>
                            <span class="ms-2">Func → View</span>
                        </div>
                        <div class="legend-item mb-2">
                            <svg width="50" height="2">
                                <line x1="0" y1="1" x2="50" y2="1" stroke="#ffb74d" stroke-width="1.5" stroke-dasharray="2,8"/>
                            </svg>
                            <span class="ms-2">Func → Func</span>
                        </div>
                        <div class="legend-item mb-2">
                            <svg width="50" height="2">
                                <line x1="0" y1="1" x2="50" y2="1" stroke="#fb8c00" stroke-width="2" stroke-dasharray="8,5,2,5"/>
                            </svg>
                            <span class="ms-2">Func → SP</span>
                        </div>
                    </div>
                </div>
            </div>
            
            <!-- Fields Modal -->
            <div v-if="local.showFieldsModal" class="legend-modal-overlay" @click="local.showFieldsModal = false">
                <div class="fields-modal" @click.stop>
                    <div class="legend-modal-header">
                        <h6 class="mb-0">
                            <i class="fa-solid me-2" :class="{
                                'fa-table': local.selectedObjectForFields?.ObjectType === 'Table',
                                'fa-eye': local.selectedObjectForFields?.ObjectType === 'View',
                                'fa-cog': local.selectedObjectForFields?.ObjectType === 'StoredProcedure',
                                'fa-code': local.selectedObjectForFields?.ObjectType === 'Function'
                            }"></i>
                            {{ local.selectedObjectForFields?.Name }}
                        </h6>
                        <button class="btn btn-sm btn-link text-dark p-0" @click="local.showFieldsModal = false">
                            <i class="fa-solid fa-times"></i>
                        </button>
                    </div>
                    <div class="legend-modal-body">
                        <!-- For Tables and Views: Show Columns -->
                        <div v-if="local.selectedObjectForFields?.ObjectType === 'Table' || local.selectedObjectForFields?.ObjectType === 'View'">
                            <div class="mb-2">
                                <strong class="text-muted small">Columns ({{ local.selectedObjectForFields?.Columns?.length || 0 }})</strong>
                            </div>
                            <div v-if="local.selectedObjectForFields?.Columns && local.selectedObjectForFields.Columns.length > 0" class="table-responsive">
                                <table class="table table-sm table-bordered table-hover mb-0" style="font-size: 0.8rem;">
                                    <thead class="table-light">
                                        <tr>
                                            <th style="width: 30px;">#</th>
                                            <th>Name</th>
                                            <th>Type</th>
                                            <th style="width: 60px;">Nullable</th>
                                            <th style="width: 50px;">Key</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr v-for="(col, index) in local.selectedObjectForFields.Columns" :key="index"
                                            :class="{
                                                'table-primary': col.IsPrimaryKey,
                                                'table-info': col.Fk
                                            }">
                                            <td class="text-center">{{ index + 1 }}</td>
                                            <td>
                                                <span v-if="col.IsPrimaryKey" class="badge bg-primary me-1" style="font-size: 0.65rem;">PK</span>
                                                <span v-if="col.Fk" class="badge bg-info me-1" style="font-size: 0.65rem;">FK</span>
                                                {{ col.Name }}
                                            </td>
                                            <td>
                                                <code style="font-size: 0.75rem;">{{ col.DbType }}{{ col.Size ? `(${col.Size})` : '' }}</code>
                                            </td>
                                            <td class="text-center">
                                                <i class="fa-solid" :class="col.Nullable ? 'fa-check text-success' : 'fa-times text-danger'"></i>
                                            </td>
                                            <td class="text-center">
                                                <span v-if="col.Fk && col.Fk.TargetTable" class="text-info small" :title="`References ${col.Fk.TargetTable}`">
                                                    → {{ col.Fk.TargetTable }}
                                                </span>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                            <div v-else class="text-muted small fst-italic">
                                No columns found
                            </div>
                        </div>
                        
                        <!-- For Stored Procedures and Functions: Show Parameters -->
                        <div v-if="local.selectedObjectForFields?.ObjectType === 'StoredProcedure' || local.selectedObjectForFields?.ObjectType === 'Function'">
                            <div class="mb-2">
                                <strong class="text-muted small">Parameters ({{ local.selectedObjectForFields?.Parameters?.length || 0 }})</strong>
                            </div>
                            <div v-if="local.selectedObjectForFields?.Parameters && local.selectedObjectForFields.Parameters.length > 0" class="table-responsive">
                                <table class="table table-sm table-bordered table-hover mb-0" style="font-size: 0.8rem;">
                                    <thead class="table-light">
                                        <tr>
                                            <th style="width: 30px;">#</th>
                                            <th>Name</th>
                                            <th>Type</th>
                                            <th style="width: 80px;">Direction</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr v-for="(param, index) in local.selectedObjectForFields.Parameters" :key="index">
                                            <td class="text-center">{{ index + 1 }}</td>
                                            <td>{{ param.Name || param.name || '' }}</td>
                                            <td>
                                                <code style="font-size: 0.75rem;">{{ param.Type || param.type || param.DbType || '' }}</code>
                                            </td>
                                            <td class="text-center">
                                                <span class="badge bg-secondary" style="font-size: 0.65rem;">
                                                    {{ param.Direction || param.direction || 'IN' }}
                                                </span>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                            <div v-else class="text-muted small fst-italic">
                                No parameters found
                            </div>
                            
                            <!-- Show Dependencies if available -->
                            <div v-if="local.selectedObjectForFields?.Dependencies && local.selectedObjectForFields.Dependencies.length > 0" class="mt-3">
                                <div class="mb-2">
                                    <strong class="text-muted small">Dependencies ({{ local.selectedObjectForFields.Dependencies.length }})</strong>
                                </div>
                                <div class="d-flex flex-wrap gap-1">
                                    <span v-for="dep in local.selectedObjectForFields.Dependencies" :key="dep" 
                                          class="badge bg-light text-dark border" 
                                          style="font-size: 0.7rem; cursor: pointer;"
                                          @click="focusNode(dep)">
                                        {{ dep }}
                                    </span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            
            <!-- Floating Control Panel -->
            <div class="floating-panel shadow" 
                 :class="{ 'panel-disabled': local.isLoading }"
                 :style="{ left: local.panelPosition.x + 'px', top: local.panelPosition.y + 'px' }"
                 @mousedown="startDrag"
                 ref="floatingPanel">
                <div class="panel-header bg-primary text-white px-2 py-1 d-flex justify-content-between align-items-center" style="cursor: move;">
                    <small><i class="fa-solid fa-cog"></i> Controls</small>
                    <button class="btn btn-sm btn-link text-white p-0" @click="local.panelCollapsed = !local.panelCollapsed" style="text-decoration: none;">
                        <i class="fa-solid" :class="local.panelCollapsed ? 'fa-chevron-down' : 'fa-chevron-up'"></i>
                    </button>
                </div>
                <div v-show="!local.panelCollapsed" class="panel-body bg-white p-2 pb-0" style="min-width: 220px;">

                    <!-- Data Source -->
                    <div class="mb-2">
                        <select class="form-select form-select-sm" v-model='local.selectedConnection' @change="loadDiagram">
                            <option value="DefaultRepo">DefaultRepo:MsSql</option>
                        </select>
                    </div>

                    <!-- Search -->
                    <div class="mb-1">
                        <div class="input-group input-group-sm">
                            <input type="text"
                                   class="form-control form-control-sm"
                                   v-model="local.searchQuery"
                                   @input="searchNode"
                                   placeholder="Type to search...">
                            <button class="btn btn-outline-secondary"
                                    type="button"
                                    @click="clearSearch"
                                    v-if="local.searchQuery">
                                <i class="fa-solid fa-times"></i>
                            </button>
                        </div>
                        <div v-if="local.searchResults.length > 0" class="mt-2" style="max-height: 150px; overflow-y: auto; font-size: 0.75rem;">
                            <div v-for="result in local.searchResults"
                                 :key="result"
                                 class="search-result-item p-1 mb-1 border rounded cursor-pointer"
                                 @click="focusNode(result)">
                                {{ result }}
                            </div>
                        </div>
                    </div>

                    <hr class="my-3">

                    <!-- Layout -->
                    <div class="mb-1">
                        <select class="form-select form-select-sm" v-model='local.layoutType' @change="applyLayout">
                            <option value="physics">Physics</option>
                            <option value="random">Random</option>
                            <option value="hierarchical-ud">Top to Down</option>
                            <option value="hierarchical-lr">Left to Right</option>
                            <option value="hierarchical-du">Down to Up</option>
                            <option value="hierarchical-rl">Right to Left</option>
                        </select>
                    </div>

                    <hr class="my-3">

                    <!-- Object Types -->
                    <div class="mb-1">
                        <div class="form-check">
                            <input class="form-check-input" type="checkbox" id="chkTables" v-model="local.showTables" @change="loadDiagram">
                            <label class="form-check-label small d-flex align-items-center justify-content-between w-100" for="chkTables">
                                <span class="d-flex align-items-center">
                                    <i class="fa-solid fa-table text-primary me-1"></i>
                                    <span>Tables</span>
                                </span>
                                <span class="badge bg-primary rounded-pill">{{ local.tables.length }}</span>
                            </label>
                        </div>
                        <div class="form-check">
                            <input class="form-check-input" type="checkbox" id="chkViews" v-model="local.showViews" @change="loadDiagram">
                            <label class="form-check-label small d-flex align-items-center justify-content-between w-100" for="chkViews">
                                <span class="d-flex align-items-center">
                                    <i class="fa-solid fa-eye text-info me-1"></i>
                                    <span>Views</span>
                                </span>
                                <span class="badge bg-info rounded-pill">{{ local.views.length }}</span>
                            </label>
                        </div>
                        <div class="form-check">
                            <input class="form-check-input" type="checkbox" id="chkSPs" v-model="local.showStoredProcedures" @change="loadDiagram">
                            <label class="form-check-label small d-flex align-items-center justify-content-between w-100" for="chkSPs">
                                <span class="d-flex align-items-center">
                                    <i class="fa-solid fa-cog text-success me-1"></i>
                                    <span>Stored Procedures</span>
                                </span>
                                <span class="badge bg-success rounded-pill">{{ local.storedProcedures.length }}</span>
                            </label>
                        </div>
                        <div class="form-check">
                            <input class="form-check-input" type="checkbox" id="chkFuncs" v-model="local.showFunctions" @change="loadDiagram">
                            <label class="form-check-label small d-flex align-items-center justify-content-between w-100" for="chkFuncs">
                                <span class="d-flex align-items-center">
                                    <i class="fa-solid fa-code text-warning me-1"></i>
                                    <span>Functions</span>
                                </span>
                                <span class="badge bg-warning rounded-pill">{{ local.functions.length }}</span>
                            </label>
                        </div>
                        <div class="form-check">
                            <input class="form-check-input" type="checkbox" id="chkRelations" v-model="local.showRelations" @change="applyRelationshipFilters">
                            <label class="form-check-label small d-flex align-items-center justify-content-between w-100" for="chkRelations">
                                <span class="d-flex align-items-center">
                                    <i class="fa-solid fa-diagram-project text-secondary me-1"></i>
                                    <span>Relations</span>
                                </span>
                                <span class="badge bg-secondary rounded-pill" v-if="local.network">{{ getEdgeCount() }}</span>
                            </label>
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
            // Store ALL loaded data
            allLoadedData: [],
            network: null,
            layoutType: "physics",
            showTables: true,
            showViews: false,
            showStoredProcedures: false,
            showFunctions: false,
            showLegend: false,
            showLegendModal: false,
            showFieldsModal: false,
            selectedObjectForFields: null,
            showRelations: true,
            searchQuery: '',
            searchResults: [],
            panelPosition: { x: 20, y: 20 },
            panelCollapsed: false,
            isDragging: false,
            dragOffset: { x: 0, y: 0 },
            hoveredNode: null,
            nodeTooltip: {
                visible: false,
                x: 0,
                y: 0,
                nodeId: null,
                nodeName: '',
                hovering: false,
                hoveringNode: false
            },
            isFocusMode: false
        }
    };

    export default {
        data() {
            return _this;
        },
        methods: {
            startDrag(event) {
                if (this.local.isLoading) return; // Don't drag when loading
                
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
                    // If data not loaded yet, load ALL data once
                    if (this.local.allLoadedData.length === 0) {
                        console.log('Loading ALL database objects with dependencies...');
                        
                        await rpcAEP("GetAllDbObjectsWithDependencies", 
                            { 
                                "DbConfName": this.local.selectedConnection
                            },
                            async (res) => {
                                res = R0R(res);
                                console.log('Received ALL data from server:', res);
                                
                                // Store all loaded data
                                this.local.allLoadedData = res;
                                
                                // Populate categorized arrays for display counts
                                this.updateCategorizedData();
                                
                                this.local.isLoading = false;
                                
                                this.$nextTick(() => {
                                    this.initNetwork();
                                });
                            }, 
                            (error) => {
                                console.error('Error loading diagram:', error);
                                this.local.error = error.message || "Failed to load database schema";
                                this.local.isLoading = false;
                            }, 
                            true
                        );
                    } else {
                        // Data already loaded, just update the display
                        console.log('Data already loaded, updating display based on filters...');
                        this.updateCategorizedData();
                        this.local.isLoading = false;
                        
                        this.$nextTick(() => {
                            this.initNetwork();
                        });
                    }
                } catch (error) {
                    console.error('Error loading diagram:', error);
                    this.local.error = error.message || "Failed to load database schema";
                    this.local.isLoading = false;
                }
            },

            updateCategorizedData() {
                // Filter data based on selected object types
                this.local.tables = this.local.allLoadedData.filter(obj => obj.ObjectType === 'Table');
                this.local.views = this.local.allLoadedData.filter(obj => obj.ObjectType === 'View');
                this.local.storedProcedures = this.local.allLoadedData.filter(obj => obj.ObjectType === 'StoredProcedure');
                this.local.functions = this.local.allLoadedData.filter(obj => obj.ObjectType === 'Function');
                
                console.log('Categorized data:', {
                    tables: this.local.tables.length,
                    views: this.local.views.length,
                    storedProcedures: this.local.storedProcedures.length,
                    functions: this.local.functions.length
                });
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

                // Filter objects based on checkboxes - only show what's selected
                const allObjects = [];
                
                if (this.local.showTables) {
                    allObjects.push(...this.local.tables.map(t => ({ ...t, ObjectType: 'Table' })));
                }
                if (this.local.showViews) {
                    allObjects.push(...this.local.views.map(v => ({ ...v, ObjectType: 'View' })));
                }
                if (this.local.showStoredProcedures) {
                    allObjects.push(...this.local.storedProcedures.map(sp => ({ ...sp, ObjectType: 'StoredProcedure' })));
                }
                if (this.local.showFunctions) {
                    allObjects.push(...this.local.functions.map(f => ({ ...f, ObjectType: 'Function' })));
                }

                console.log('Rendering objects based on filters:', {
                    showTables: this.local.showTables,
                    showViews: this.local.showViews,
                    showStoredProcedures: this.local.showStoredProcedures,
                    showFunctions: this.local.showFunctions,
                    totalObjectsToRender: allObjects.length
                });

                allObjects.forEach(obj => {
                    const objectType = obj.ObjectType || 'Table';
                    const style = getNodeStyle(objectType);
                    
                    // Get icon based on object type - matching the control panel icons
                    let icon = '';
                    if (objectType === 'Table') {
                        icon = '▦ '; // Table icon
                    } else if (objectType === 'View') {
                        icon = '◉ '; // Eye/View icon
                    } else if (objectType === 'StoredProcedure') {
                        icon = '⚙ '; // Cog/Settings icon
                    } else if (objectType === 'Function') {
                        icon = '〈/〉 '; // Code icon
                    }
                    
                    let label = `<b>${icon}${obj.Name}</b>`;
                    
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
                        //label += `\n[${objectType === 'StoredProcedure' ? 'SP' : 'Function'}]`;
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
                            size: 14,
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

                // Add FK relationships for tables (only if both source and target are displayed)
                const addedRelations = new Set();
                if (this.local.showRelations && this.local.showTables) {
                    const displayedNodeNames = new Set(allObjects.map(obj => obj.Name));
                    
                    this.local.tables.forEach(table => {
                        if (table.Columns) {
                            table.Columns.forEach(col => {
                                if (col.Fk && col.Fk.TargetTable) {
                                    // Only add edge if both source and target are in the displayed nodes
                                    if (displayedNodeNames.has(table.Name) && displayedNodeNames.has(col.Fk.TargetTable)) {
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
                                }
                            });
                        }
                    });
                }

                // Helper to get object type from name
                const getObjectType = (name) => {
                    // Search in ALL loaded data, not just displayed ones
                    const found = this.local.allLoadedData.find(obj => obj.Name === name);
                    return found ? found.ObjectType : null;
                };

                // Add relationships for views to tables/views/functions
                if (this.local.showRelations && this.local.showViews) {
                    const displayedNodeNames = new Set(allObjects.map(obj => obj.Name));
                    
                    // Only process views that are currently displayed
                    const displayedViews = this.local.views;
                    displayedViews.forEach(view => {
                        if (view.Dependencies && view.Dependencies.length > 0) {
                            view.Dependencies.forEach(dep => {
                                // Only add edge if both source and target are in the displayed nodes
                                if (displayedNodeNames.has(view.Name) && displayedNodeNames.has(dep)) {
                                    const depType = getObjectType(dep);
                                    if (!depType) return; // Skip if dependency not found
                                    
                                    const relKey = `${dep}_${view.Name}_view_dependency`;
                                    if (!addedRelations.has(relKey)) {
                                        // Different styles based on dependency type
                                        let edgeStyle = {
                                            arrows: 'to',
                                            width: 2,
                                            font: { size: 9, align: 'middle' }
                                        };
                                        
                                        if (depType === 'Table') {
                                            // View -> Table: Blue dashed
                                            edgeStyle.color = { color: '#1976d2', highlight: '#0d47a1' };
                                            edgeStyle.dashes = [5, 5];
                                            edgeStyle.title = 'View uses Table';
                                        } else if (depType === 'View') {
                                            // View -> View: Light blue dotted
                                            edgeStyle.color = { color: '#64b5f6', highlight: '#1976d2' };
                                            edgeStyle.dashes = [2, 8];
                                            edgeStyle.width = 1.5;
                                            edgeStyle.title = 'View uses View';
                                        } else if (depType === 'Function') {
                                            // View -> Function: Blue-Orange gradient
                                            edgeStyle.color = { color: '#42a5f5', highlight: '#1976d2' };
                                            edgeStyle.dashes = [8, 3, 2, 3];
                                            edgeStyle.title = 'View uses Function';
                                        }
                                        
                                        edges.push({
                                            from: dep,
                                            to: view.Name,
                                            ...edgeStyle,
                                            relationType: `View-${depType}`
                                        });
                                        addedRelations.add(relKey);
                                    }
                                }
                            });
                        }
                    });
                }

                // Add relationships for stored procedures to tables/views/functions
                if (this.local.showRelations && this.local.showStoredProcedures) {
                    const displayedNodeNames = new Set(allObjects.map(obj => obj.Name));
                    
                    // Only process stored procedures that are currently displayed
                    const displayedSPs = this.local.storedProcedures;
                    displayedSPs.forEach(sp => {
                        if (sp.Dependencies && sp.Dependencies.length > 0) {
                            sp.Dependencies.forEach(dep => {
                                // Only add edge if both source and target are in the displayed nodes
                                if (displayedNodeNames.has(sp.Name) && displayedNodeNames.has(dep)) {
                                    const depType = getObjectType(dep);
                                    if (!depType) return;
                                    
                                    const relKey = `${sp.Name}_${dep}_sp_dependency`;
                                    if (!addedRelations.has(relKey)) {
                                        let edgeStyle = {
                                            arrows: 'to',
                                            width: 2.5,
                                            font: { size: 9, align: 'middle' }
                                        };
                                        
                                        if (depType === 'Table') {
                                            // SP -> Table: Green dashed
                                            edgeStyle.color = { color: '#388e3c', highlight: '#1b5e20' };
                                            edgeStyle.dashes = [10, 5];
                                            edgeStyle.title = 'SP uses Table';
                                        } else if (depType === 'View') {
                                            // SP -> View: Green-Blue mix
                                            edgeStyle.color = { color: '#66bb6a', highlight: '#388e3c' };
                                            edgeStyle.dashes = [8, 4, 2, 4];
                                            edgeStyle.width = 2;
                                            edgeStyle.title = 'SP uses View';
                                        } else if (depType === 'Function') {
                                            // SP -> Function: Green dotted
                                            edgeStyle.color = { color: '#81c784', highlight: '#66bb6a' };
                                            edgeStyle.dashes = [3, 6];
                                            edgeStyle.width = 2;
                                            edgeStyle.title = 'SP uses Function';
                                        } else if (depType === 'StoredProcedure') {
                                            // SP -> SP: Dark green dots
                                            edgeStyle.color = { color: '#2e7d32', highlight: '#1b5e20' };
                                            edgeStyle.dashes = [2, 10];
                                            edgeStyle.width = 1.5;
                                            edgeStyle.title = 'SP calls SP';
                                        }
                                        
                                        edges.push({
                                            from: sp.Name,
                                            to: dep,
                                            ...edgeStyle,
                                            relationType: `SP-${depType}`
                                        });
                                        addedRelations.add(relKey);
                                    }
                                }
                            });
                        }
                    });
                }

                // Add relationships for functions to tables/views/SP/functions
                if (this.local.showRelations && this.local.showFunctions) {
                    const displayedNodeNames = new Set(allObjects.map(obj => obj.Name));
                    
                    // Only process functions that are currently displayed
                    const displayedFuncs = this.local.functions;
                    displayedFuncs.forEach(func => {
                        if (func.Dependencies && func.Dependencies.length > 0) {
                            func.Dependencies.forEach(dep => {
                                // Only add edge if both source and target are in the displayed nodes
                                if (displayedNodeNames.has(func.Name) && displayedNodeNames.has(dep)) {
                                    const depType = getObjectType(dep);
                                    if (!depType) return;
                                    
                                    const relKey = `${func.Name}_${dep}_func_dependency`;
                                    if (!addedRelations.has(relKey)) {
                                        let edgeStyle = {
                                            arrows: 'to',
                                            width: 2,
                                            font: { size: 9, align: 'middle' }
                                        };
                                        
                                        if (depType === 'Table') {
                                            // Function -> Table: Orange dashed
                                            edgeStyle.color = { color: '#f57c00', highlight: '#e65100' };
                                            edgeStyle.dashes = [5, 10];
                                            edgeStyle.title = 'Function uses Table';
                                        } else if (depType === 'View') {
                                            // Function -> View: Orange-Blue mix
                                            edgeStyle.color = { color: '#ff9800', highlight: '#f57c00' };
                                            edgeStyle.dashes = [6, 3, 2, 3];
                                            edgeStyle.width = 2;
                                            edgeStyle.title = 'Function uses View';
                                        } else if (depType === 'Function') {
                                            // Function -> Function: Light orange dots
                                            edgeStyle.color = { color: '#ffb74d', highlight: '#ff9800' };
                                            edgeStyle.dashes = [2, 8];
                                            edgeStyle.width = 1.5;
                                            edgeStyle.title = 'Function calls Function';
                                        } else if (depType === 'StoredProcedure') {
                                            // Function -> SP: Orange-Green
                                            edgeStyle.color = { color: '#fb8c00', highlight: '#ef6c00' };
                                            edgeStyle.dashes = [8, 5, 2, 5];
                                            edgeStyle.width = 2;
                                            edgeStyle.title = 'Function uses SP';
                                        }
                                        
                                        edges.push({
                                            from: func.Name,
                                            to: dep,
                                            ...edgeStyle,
                                            relationType: `Function-${depType}`
                                        });
                                        addedRelations.add(relKey);
                                    }
                                }
                            });
                        }
                    });
                }

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
                        },
                        hoverWidth: 1.5,
                        selectionWidth: 2
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
                
                // Add event listeners for node interactions
                this.local.network.on('click', (params) => {
                    if (params.nodes.length > 0) {
                        this.highlightNodeConnections(params.nodes[0]);
                    } else {
                        this.clearHighlight();
                    }
                });
                
                this.local.network.on('doubleClick', (params) => {
                    if (params.nodes.length > 0) {
                        this.toggleFocusMode(params.nodes[0]);
                    }
                });
                
                this.local.network.on('hoverNode', (params) => {
                    this.$refs.network.style.cursor = 'pointer';
                    this.local.nodeTooltip.hoveringNode = true;
                    this.showNodeTooltip(params.node, params.event);
                });
                
                this.local.network.on('blurNode', () => {
                    this.$refs.network.style.cursor = 'default';
                    this.local.nodeTooltip.hoveringNode = false;
                    this.hideNodeTooltip();
                });
                
                this.local.network.on('dragStart', () => {
                    this.hideNodeTooltip();
                });
                
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

            highlightNodeConnections(nodeId) {
                if (!this.local.network) return;
                
                const connectedNodes = this.local.network.getConnectedNodes(nodeId);
                const connectedEdges = this.local.network.getConnectedEdges(nodeId);
                
                const allNodes = this.local.network.body.data.nodes.getIds();
                const allEdges = this.local.network.body.data.edges.getIds();
                
                // Dim all nodes except selected and connected
                const updateNodes = allNodes.map(id => {
                    if (id === nodeId) {
                        return { id: id, opacity: 1, font: { size: 14 } };
                    } else if (connectedNodes.includes(id)) {
                        return { id: id, opacity: 1 };
                    } else {
                        return { id: id, opacity: 0.15 };
                    }
                });
                
                // Dim all edges except connected
                const updateEdges = allEdges.map(id => {
                    if (connectedEdges.includes(id)) {
                        return { id: id, opacity: 1 };
                    } else {
                        return { id: id, opacity: 0.1 };
                    }
                });
                
                this.local.network.body.data.nodes.update(updateNodes);
                this.local.network.body.data.edges.update(updateEdges);
            },

            clearHighlight() {
                if (!this.local.network) return;
                
                const allNodes = this.local.network.body.data.nodes.getIds();
                const allEdges = this.local.network.body.data.edges.getIds();
                
                const updateNodes = allNodes.map(id => ({ id: id, opacity: 1, font: { size: 13 } }));
                const updateEdges = allEdges.map(id => ({ id: id, opacity: 1 }));
                
                this.local.network.body.data.nodes.update(updateNodes);
                this.local.network.body.data.edges.update(updateEdges);
            },

            getEdgeCount() {
                if (!this.local.network || !this.local.network.body.data.edges) return 0;
                return this.local.network.body.data.edges.length;
            },

            exportAsImage() {
                if (!this.local.network) return;
                
                try {
                    // Get the canvas element
                    const canvas = this.$refs.network.querySelector('canvas');
                    if (!canvas) {
                        console.error('Canvas not found');
                        return;
                    }
                    
                    // Convert canvas to blob and download
                    canvas.toBlob((blob) => {
                        const url = URL.createObjectURL(blob);
                        const link = document.createElement('a');
                        link.download = `db-diagram-${new Date().getTime()}.png`;
                        link.href = url;
                        link.click();
                        URL.revokeObjectURL(url);
                    });
                } catch (error) {
                    console.error('Error exporting image:', error);
                    alert('Failed to export diagram as image');
                }
            },

            searchNode() {
                if (!this.local.network || !this.local.searchQuery) {
                    this.local.searchResults = [];
                    return;
                }
                
                const query = this.local.searchQuery.toLowerCase();
                const allNodes = this.local.network.body.data.nodes.get();
                
                this.local.searchResults = allNodes
                    .filter(node => node.id.toLowerCase().includes(query))
                    .map(node => node.id)
                    .slice(0, 10); // Limit to 10 results
            },

            clearSearch() {
                this.local.searchQuery = '';
                this.local.searchResults = [];
                this.clearHighlight();
            },

            focusNode(nodeId) {
                if (!this.local.network) return;
                
                this.local.network.focus(nodeId, {
                    scale: 1.5,
                    animation: {
                        duration: 500,
                        easingFunction: 'easeInOutQuad'
                    }
                });
                
                this.highlightNodeConnections(nodeId);
                this.local.searchQuery = '';
                this.local.searchResults = [];
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

            applyRelationshipFilters() {
                if (!this.local.network) return;
                
                // Recreate the network with new filters
                this.initNetwork();
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
            },

            handleEscapeKey(event) {
                if (event.key === 'Escape') {
                    if (this.local.showFieldsModal) {
                        this.local.showFieldsModal = false;
                    } else if (this.local.showLegendModal) {
                        this.local.showLegendModal = false;
                    }
                }
            },

            showNodeTooltip(nodeId, event) {
                if (!this.local.network) return;
                
                try {
                    // Get DOM position from event
                    let x, y;
                    
                    if (event && event.pointer && event.pointer.DOM) {
                        x = event.pointer.DOM.x;
                        y = event.pointer.DOM.y;
                    } else if (event && event.event) {
                        x = event.event.clientX;
                        y = event.event.clientY;
                    } else {
                        // Fallback: get node position and convert to DOM
                        const nodePosition = this.local.network.getPositions([nodeId])[nodeId];
                        if (nodePosition) {
                            const canvasPos = this.local.network.canvasToDOM(nodePosition);
                            x = canvasPos.x;
                            y = canvasPos.y;
                        } else {
                            return; // Can't determine position
                        }
                    }
                    
                    this.local.nodeTooltip = {
                        visible: true,
                        x: x,
                        y: y,
                        nodeId: nodeId,
                        nodeName: nodeId,
                        hovering: false,
                        hoveringNode: true
                    };
                } catch (error) {
                    console.error('Error showing node tooltip:', error);
                }
            },

            hideNodeTooltip() {
                setTimeout(() => {
                    // Only hide if not hovering over node or toolbar
                    if (!this.local.nodeTooltip.hovering && !this.local.nodeTooltip.hoveringNode) {
                        this.local.nodeTooltip.visible = false;
                    }
                }, 200);
            },

            checkTooltipHide() {
                this.local.nodeTooltip.hovering = false;
                setTimeout(() => {
                    // Only hide if not hovering over node or toolbar
                    if (!this.local.nodeTooltip.hovering && !this.local.nodeTooltip.hoveringNode) {
                        this.local.nodeTooltip.visible = false;
                    }
                }, 200);
            },

            toggleFocusMode(nodeId) {
                if (this.local.isFocusMode) {
                    // Currently in focus mode, switch to show all
                    this.resetFocus();
                    this.local.isFocusMode = false;
                } else {
                    // Currently showing all, switch to focus mode
                    this.focusOnNode(nodeId);
                    this.local.isFocusMode = true;
                }
            },

            focusOnNode(nodeId) {
                if (!this.local.network) return;
                
                // Get connected nodes (dependencies)
                const connectedNodes = this.local.network.getConnectedNodes(nodeId);
                const allNodes = this.local.network.body.data.nodes.getIds();
                
                // Hide nodes that are not connected
                const nodesToShow = new Set([nodeId, ...connectedNodes]);
                
                const updateNodes = allNodes.map(id => {
                    if (nodesToShow.has(id)) {
                        return { id: id, hidden: false, opacity: 1 };
                    } else {
                        return { id: id, hidden: true, opacity: 0 };
                    }
                });
                
                this.local.network.body.data.nodes.update(updateNodes);
                
                // Also update edges
                const connectedEdges = this.local.network.getConnectedEdges(nodeId);
                const allEdges = this.local.network.body.data.edges.getIds();
                
                const updateEdges = allEdges.map(id => {
                    if (connectedEdges.includes(id)) {
                        return { id: id, hidden: false, opacity: 1 };
                    } else {
                        return { id: id, hidden: true, opacity: 0 };
                    }
                });
                
                this.local.network.body.data.edges.update(updateEdges);
                
                // Focus on the node
                this.local.network.focus(nodeId, {
                    scale: 1.2,
                    animation: {
                        duration: 500,
                        easingFunction: 'easeInOutQuad'
                    }
                });
                
                this.hideNodeTooltip();
            },

            resetFocus() {
                if (!this.local.network) return;
                
                const allNodes = this.local.network.body.data.nodes.getIds();
                const allEdges = this.local.network.body.data.edges.getIds();
                
                const updateNodes = allNodes.map(id => ({ id: id, hidden: false, opacity: 1 }));
                const updateEdges = allEdges.map(id => ({ id: id, hidden: false, opacity: 1 }));
                
                this.local.network.body.data.nodes.update(updateNodes);
                this.local.network.body.data.edges.update(updateEdges);
                
                this.fitView();
                
                this.local.isFocusMode = false;
            },

            openObjectEditor(nodeId) {
                // Get object details
                const node = this.local.network.body.data.nodes.get(nodeId);
                if (!node) return;
                
                const objectType = node.objectType || 'Table';
                const connection = this.local.selectedConnection;
                
                let url = '';
                
                if (objectType === 'Table') {
                    // Tables go to DbTableDesigner
                    url = `?c=components/DbTableDesigner&cnn=${encodeURIComponent(connection)}&o=${encodeURIComponent(nodeId)}`;
                } else {
                    // Views, Stored Procedures, and Functions go to DbScriptEditor
                    url = `?c=components/DbScriptEditor&cnn=${encodeURIComponent(connection)}&o=${encodeURIComponent(nodeId)}`;
                }
                
                window.open(url, '_blank');
                
                this.hideNodeTooltip();
            },

            showObjectFields(nodeId) {
                // Find the object in allLoadedData
                const obj = this.local.allLoadedData.find(o => o.Name === nodeId);
                
                if (!obj) {
                    console.error('Object not found:', nodeId);
                    return;
                }
                
                this.local.selectedObjectForFields = obj;
                this.local.showFieldsModal = true;
                this.hideNodeTooltip();
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
            
            // Add escape key listener for modal
            document.addEventListener('keydown', this.handleEscapeKey);
            
            // Load vis.js library if not already loaded
            if (typeof vis === 'undefined') {
                // Load CSS first
                const link = document.createElement('link');
                link.rel = 'stylesheet';
                link.href = '/a..lib/vis-network/vis-network.min.css';
                document.head.appendChild(link);
                
                // Then load JS
                const script = document.createElement('script');
                script.src = '/a..lib/vis-network/vis-network.min.js';
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
            // Cleanup escape key listener
            document.removeEventListener('keydown', this.handleEscapeKey);
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
        transition: opacity 0.3s;
    }
    
    .panel-disabled {
        opacity: 0.6;
        pointer-events: none;
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
        width: 100%;
    }
    
    .form-check-label .badge {
        font-size: 0.7rem;
        min-width: 24px;
        padding: 0.25em 0.5em;
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
    
    .search-result-item {
        cursor: pointer;
        background-color: #f8f9fa;
        transition: background-color 0.2s;
    }
    
    .search-result-item:hover {
        background-color: #e9ecef;
    }
    
    .cursor-pointer {
        cursor: pointer;
    }
    
    .node-toolbar {
        position: absolute;
        z-index: 1500;
        pointer-events: all;
        animation: fadeIn 0.15s ease-out;
        display: flex;
        flex-direction: column;
        align-items: center;
    }
    
    @keyframes fadeIn {
        from {
            opacity: 0;
        }
        to {
            opacity: 1;
        }
    }
    
    .node-toolbar-label {
        margin-bottom: 0.5rem;
        padding: 0.35rem 0.75rem;
        background: rgba(0, 0, 0, 0.85);
        color: white;
        font-size: 0.75rem;
        border-radius: 6px;
        text-align: center;
        white-space: nowrap;
        max-width: 250px;
        overflow: hidden;
        text-overflow: ellipsis;
        box-shadow: 0 2px 8px rgba(0, 0, 0, 0.2);
    }
    
    .node-toolbar-content {
        display: flex;
        gap: 0.25rem;
        background: white;
        padding: 0.35rem;
        border-radius: 8px;
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
        border: 1px solid #dee2e6;
    }
    
    .node-toolbar-btn {
        display: flex;
        align-items: center;
        justify-content: center;
        width: 32px;
        height: 32px;
        border: none;
        background: white;
        border-radius: 6px;
        cursor: pointer;
        transition: all 0.2s;
        color: #495057;
        font-size: 0.875rem;
    }
    
    .node-toolbar-btn:hover {
        background: #e3f2fd;
        color: #1976d2;
        transform: scale(1.1);
    }
    
    .node-toolbar-btn:active {
        transform: scale(0.95);
    }
    
    .fixed-toolbar {
        position: absolute;
        top: 10px;
        right: 10px;
        z-index: 999;
        display: flex;
        flex-direction: column;
        gap: 0.25rem;
        padding: 0.5rem;
        background-color: white;
        border-radius: 6px;
        border: 1px solid #dee2e6;
        box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
        transition: opacity 0.3s;
    }
    
    .toolbar-disabled {
        opacity: 0.6;
        pointer-events: none;
    }
    
    .toolbar-btn-vertical {
        display: flex;
        align-items: center;
        justify-content: center;
        width: 36px;
        height: 36px;
        border: 1px solid #dee2e6;
        background-color: white;
        border-radius: 4px;
        cursor: pointer;
        transition: all 0.2s;
        color: #495057;
        font-size: 0.95rem;
    }
    
    .toolbar-btn-vertical:hover {
        background-color: #e9ecef;
        border-color: #adb5bd;
        color: #212529;
        transform: scale(1.05);
    }
    
    .toolbar-btn-vertical:active {
        background-color: #dee2e6;
        transform: scale(0.95);
    }
    
    .toolbar-btn-vertical:disabled {
        opacity: 0.5;
        cursor: not-allowed;
        pointer-events: none;
    }
    
    .toolbar-divider-horizontal {
        height: 1px;
        width: 100%;
        background-color: #dee2e6;
        margin: 0.25rem 0;
    }
    
    .legend-modal-overlay {
        position: fixed;
        top: 0;
        left: 0;
        right: 0;
        bottom: 0;
        background-color: rgba(0, 0, 0, 0.5);
        display: flex;
        align-items: center;
        justify-content: center;
        z-index: 2000;
    }
    
    .legend-modal {
        background: white;
        border-radius: 8px;
        box-shadow: 0 4px 20px rgba(0, 0, 0, 0.3);
        max-width: 400px;
        width: 90%;
        max-height: 80vh;
        overflow: hidden;
        display: flex;
        flex-direction: column;
    }
    
    .legend-modal-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 1rem;
        border-bottom: 1px solid #dee2e6;
        background-color: #f8f9fa;
    }
    
    .legend-modal-body {
        padding: 1rem;
        overflow-y: auto;
        font-size: 0.875rem;
    }
    
    .legend-modal .legend-item {
        display: flex;
        align-items: center;
        padding: 0.25rem 0;
    }
    
    .fields-modal {
        background: white;
        border-radius: 8px;
        box-shadow: 0 4px 20px rgba(0, 0, 0, 0.3);
        max-width: 700px;
        width: 90%;
        max-height: 80vh;
        overflow: hidden;
        display: flex;
        flex-direction: column;
    }
    
    .fields-modal .table {
        margin-bottom: 0;
    }
    
    .fields-modal .table th {
        font-weight: 600;
        font-size: 0.75rem;
        text-transform: uppercase;
        letter-spacing: 0.5px;
    }
    
    .fields-modal .table td {
        vertical-align: middle;
    }
    
    .fields-modal .table code {
        background-color: #f8f9fa;
        padding: 0.2rem 0.4rem;
        border-radius: 3px;
    }
</style>
