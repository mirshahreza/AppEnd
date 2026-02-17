<template>
    <div class="card h-100 bg-transparent rounded-0 border-0 d-flex flex-column">
        <!-- Header -->
        <div class="card-header p-2 bg-body-subtle rounded-0 border-0">
            <div class="hstack">
                <h6 class="m-0">
                    <i class="fa-solid fa-wand-magic-sparkles me-2"></i>Workflow Designer (JSON)
                </h6>
                <div class="vr"></div>
                <span v-if="workflowId" class="text-muted small">
                    <i class="fa-solid fa-fingerprint me-1"></i><code>{{ workflowId }}</code>
                </span>

                <div class="p-0 ms-auto"></div>

                <div class="vr"></div>
                <button class="btn btn-sm btn-outline-secondary" @click="viewJson" title="View JSON">
                    <i class="fa-solid fa-code me-1"></i> JSON
                </button>
                <div class="vr"></div>
                <button class="btn btn-sm btn-outline-success" @click="saveWorkflow" title="Save Workflow">
                    <i class="fa-solid fa-save me-1"></i> Save
                </button>
                <div class="vr"></div>
                <button class="btn btn-sm btn-outline-secondary" @click="closeDesigner" title="Close">
                    <i class="fa-solid fa-times me-1"></i> Close
                </button>
            </div>
        </div>

        <!-- Designer Container -->
        <div class="card-body p-0 flex-grow-1 position-relative" style="min-height: 0; overflow: hidden;">
            <div class="d-flex h-100">
                <!-- Left Panel: Activity Toolbox (Mifty Style) -->
                <div class="bg-white border-end d-flex flex-column" style="width: 280px;">
                    <!-- Sticky Header -->
                    <div class="p-3 border-bottom sticky-top bg-white">
                        <h6 class="m-0 mb-2 fw-bold text-dark" style="font-size: 0.9rem;">
                            <i class="fa-solid fa-shapes me-2 text-primary"></i>Activities Toolbox
                        </h6>
                        <input type="text" class="form-control form-control-sm border-0 bg-light" 
                            placeholder="Search activities..." 
                            v-model="searchQuery"
                            style="font-size: 0.85rem;">
                    </div>
                    
                    <!-- Activity Categories -->
                    <div class="flex-grow-1 overflow-auto p-2">
                        <div v-for="(category, catIndex) in activityCategories" :key="category.name" class="mb-1">
                            <!-- Category Header -->
                            <div class="d-flex align-items-center px-2 py-2 rounded cursor-pointer category-header"
                                @click="toggleCategory(catIndex)"
                                style="cursor: pointer;">
                                <i :class="category.icon" 
                                    :style="{ color: category.color, fontSize: '16px', width: '20px' }" 
                                    class="me-2 flex-shrink-0"></i>
                                <span class="fw-semibold flex-grow-1" style="font-size: 0.875rem;">
                                    {{ category.name }}
                                </span>
                                <span class="badge rounded-pill me-2" 
                                    :style="{ backgroundColor: category.color, fontSize: '0.7rem' }">
                                    {{ category.activities.length }}
                                </span>
                                <i class="fa-solid fa-chevron-down transition-transform" 
                                    :class="{ 'rotate-180': expandedCategories[catIndex] }"
                                    style="font-size: 0.7rem; color: #6c757d; transition: transform 0.2s;"></i>
                            </div>
                            
                            <!-- Category Items (Collapsible) -->
                            <div v-show="expandedCategories[catIndex]" class="category-items">
                                <div v-for="activity in category.activities" :key="activity.type"
                                    v-show="!searchQuery || activity.name.toLowerCase().includes(searchQuery.toLowerCase()) || activity.type.toLowerCase().includes(searchQuery.toLowerCase())"
                                    class="activity-item px-2 py-2 ms-3 rounded cursor-pointer"
                                    style="cursor: grab;"
                                    draggable="true"
                                    @dragstart="onActivityDragStart($event, activity)"
                                    @click="addActivity(activity)">
                                    <div class="d-flex align-items-start">
                                        <i :class="activity.icon" 
                                            :style="{ color: category.color, fontSize: '14px', width: '18px' }"
                                            class="me-2 mt-1 flex-shrink-0"></i>
                                        <div class="flex-grow-1 overflow-hidden">
                                            <div class="fw-medium text-truncate" style="font-size: 0.8rem;">
                                                {{ activity.name }}
                                            </div>
                                            <div class="text-muted text-truncate" style="font-size: 0.7rem;">
                                                {{ activity.type }}
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                
                                <!-- No results message -->
                                <div v-if="searchQuery && category.activities.filter(a => a.name.toLowerCase().includes(searchQuery.toLowerCase()) || a.type.toLowerCase().includes(searchQuery.toLowerCase())).length === 0"
                                    class="text-center text-muted py-2 small ms-3">
                                    No matches
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Center: Flowchart Canvas -->
                <div class="flex-grow-1 position-relative" 
                    style="overflow: hidden; background: #f8f9fa;"
                    @drop="onCanvasDrop"
                    @dragover="onCanvasDragOver">
                    
                    <!-- Zoom Controls -->
                    <div class="position-absolute top-0 end-0 m-3" style="z-index: 10;">
                        <div class="btn-group-vertical btn-group-sm shadow-sm" role="group">
                            <button class="btn btn-light" @click="zoomIn" title="Zoom In">
                                <i class="fa-solid fa-plus"></i>
                            </button>
                            <button class="btn btn-light" @click="zoomReset" title="Reset Zoom (100%)">
                                <span class="small">{{ Math.round(zoomLevel * 100) }}%</span>
                            </button>
                            <button class="btn btn-light" @click="zoomOut" title="Zoom Out">
                                <i class="fa-solid fa-minus"></i>
                            </button>
                            <button class="btn btn-light" @click="autoArrange" title="Auto Arrange">
                                <i class="fa-solid fa-diagram-project"></i>
                            </button>
                        </div>
                    </div>

                    <!-- SVG Canvas -->
                    <div ref="canvasContainer" class="w-100 h-100" 
                        @mousedown="onCanvasMouseDown"
                        @mousemove="onCanvasMouseMove"
                        @mouseup="onCanvasMouseUp"
                        @mouseleave="onCanvasMouseUp"
                        @wheel="onCanvasWheel"
                        style="cursor: grab;">
                        
                        <svg :width="canvasWidth" :height="canvasHeight" 
                            style="display: block; user-select: none;">
                            
                            <g :transform="`translate(${panX}, ${panY}) scale(${zoomLevel})`">
                                <!-- Empty state -->
                                <g v-if="workflow.activities.length === 0">
                                    <text x="400" y="200" text-anchor="middle" font-size="18" fill="#999" opacity="0.5">
                                        <tspan x="400" dy="0">Drag & drop activities from the left panel</tspan>
                                        <tspan x="400" dy="30">or click on them to add to your workflow</tspan>
                                    </text>
                                    <circle cx="400" cy="150" r="40" fill="none" stroke="#ddd" stroke-width="2" opacity="0.3" />
                                    <text x="400" y="160" text-anchor="middle" font-size="40" fill="#ddd" opacity="0.3">+</text>
                                </g>

                                <!-- Automatic Connections between sequential activities -->
                                <g v-for="(activity, index) in workflow.activities" :key="'auto-conn-' + index">
                                    <line v-if="index < workflow.activities.length - 1"
                                        :x1="activity.x + 200"
                                        :y1="activity.y + 40"
                                        :x2="workflow.activities[index + 1].x"
                                        :y2="workflow.activities[index + 1].y + 40"
                                        stroke="#6c757d" stroke-width="2" marker-end="url(#arrowhead)" />
                                </g>

                                <!-- Custom Connections (manual connections) -->
                                <g v-for="connection in workflow.connections" :key="connection.id">
                                    <g @click.stop="selectedConnection = connection" style="cursor: pointer;">
                                        <!-- Connection line with better routing -->
                                        <path 
                                            :d="getConnectionPath(connection)"
                                            :stroke="selectedConnection?.id === connection.id ? '#0d6efd' : '#6c757d'"
                                            :stroke-width="selectedConnection?.id === connection.id ? 3 : 2"
                                            fill="none"
                                            stroke-linecap="round"
                                            marker-end="url(#arrowhead)" />
                                        <!-- Delete button on connection -->
                                        <circle v-if="selectedConnection?.id === connection.id"
                                            :cx="getConnectionMidpoint(connection).x"
                                            :cy="getConnectionMidpoint(connection).y"
                                            r="8" fill="#dc3545" opacity="0.2" />
                                        <text v-if="selectedConnection?.id === connection.id"
                                            :x="getConnectionMidpoint(connection).x"
                                            :y="getConnectionMidpoint(connection).y + 2"
                                            text-anchor="middle" font-size="12" fill="#dc3545"
                                            @click.stop="removeConnection(connection.id)"
                                            style="cursor: pointer; font-weight: bold;">×</text>
                                    </g>
                                </g>

                                <!-- Arrow marker definition -->
                                <defs>
                                    <marker id="arrowhead" markerWidth="10" markerHeight="10" refX="9" refY="3" orient="auto">
                                        <polygon points="0 0, 10 3, 0 6" fill="#6c757d" />
                                    </marker>
                                </defs>

                                <!-- Activity Nodes with Input/Output Ports -->
                                <g v-for="(activity, index) in workflow.activities" :key="'node-' + index"
                                    :transform="`translate(${activity.x}, ${activity.y})`"
                                    @mousedown.stop="startDragActivity($event, activity)"
                                    style="cursor: move;">
                                    
                                    <!-- Node background - Diamond shape for If/Switch, Rectangle for others -->
                                    <template v-if="activity.type === 'If' || activity.type === 'Switch'">
                                        <!-- Diamond shape for decision nodes -->
                                        <polygon points="100,0 200,40 100,80 0,40"
                                            :fill="selectedActivityId === activity.id ? '#fff9e6' : '#fffbf0'"
                                            :stroke="getActivityColor(activity.type)"
                                            :stroke-width="selectedActivityId === activity.id ? 3 : 2"
                                            @click.stop="selectActivity(activity); selectActivityForProps(activity);" />
                                    </template>
                                    <template v-else>
                                        <!-- Rectangle for regular activities -->
                                        <rect x="0" y="0" width="200" height="80" rx="8"
                                            :fill="selectedActivityId === activity.id ? '#e7f3ff' : 'white'"
                                            :stroke="getActivityColor(activity.type)"
                                            :stroke-width="selectedActivityId === activity.id ? 3 : 2"
                                            @click.stop="selectActivity(activity); selectActivityForProps(activity);" />
                                    </template>
                                    
                                    <!-- Icon circle -->
                                    <circle :cx="20" cy="25" r="15"
                                        :fill="getActivityColor(activity.type)" opacity="0.2" />
                                    
                                    <!-- Icon -->
                                    <text x="20" y="30" text-anchor="middle" font-size="16" 
                                        :fill="getActivityColor(activity.type)"
                                        pointer-events="none">
                                        {{ getActivityEmoji(activity.type) }}
                                    </text>
                                    
                                    <!-- Activity name -->
                                    <text x="45" y="20" font-weight="bold" font-size="12" fill="#333" pointer-events="none">
                                        {{ truncate(activity.name || getActivityTypeName(activity.type), 20) }}
                                    </text>
                                    
                                    <!-- Activity type badge -->
                                    <text x="45" y="35" font-size="10" fill="#6c757d" pointer-events="none">
                                        {{ activity.type }}
                                    </text>
                                    
                                    <!-- Activity details -->
                                    <text v-if="activity.text" x="45" y="50" font-size="9" fill="#999" pointer-events="none">
                                        {{ truncate(activity.text, 25) }}
                                    </text>
                                    <text v-else-if="activity.variableName" x="45" y="50" font-size="9" fill="#999" pointer-events="none">
                                        {{ activity.variableName }}
                                    </text>
                                    
                                    <!-- Input Port (small circle on left) -->
                                    <circle cx="0" cy="40" r="4" fill="#198754" opacity="0.7" class="input-port"
                                        @click.stop="selectedConnection = null"
                                        style="cursor: crosshair;" title="Input port" />
                                    
                                    <!-- Output Ports -->
                                    <!-- Regular output port for non-decision nodes -->
                                    <circle v-if="activity.type !== 'If' && activity.type !== 'Switch'" 
                                        cx="200" cy="40" r="4" fill="#0d6efd" opacity="0.7" class="output-port"
                                        @click.stop="selectedConnection = null"
                                        style="cursor: crosshair;" title="Output port" />
                                    
                                    <!-- True/False output ports for If nodes -->
                                    <template v-if="activity.type === 'If'">
                                        <circle cx="100" cy="85" r="4" fill="#28a745" opacity="0.7" class="output-port"
                                            @click.stop="selectedConnection = null"
                                            style="cursor: crosshair;" title="True branch" />
                                        <text x="100" y="100" text-anchor="middle" font-size="8" fill="#28a745" font-weight="bold">True</text>
                                        
                                        <circle cx="100" cy="-5" r="4" fill="#dc3545" opacity="0.7" class="output-port"
                                            @click.stop="selectedConnection = null"
                                            style="cursor: crosshair;" title="False branch" />
                                        <text x="100" y="-10" text-anchor="middle" font-size="8" fill="#dc3545" font-weight="bold">False</text>
                                    </template>
                                    
                                    <!-- Delete button -->
                                    <g @click.stop="removeActivity(index)" style="cursor: pointer;">
                                        <circle cx="185" cy="15" r="10" fill="#dc3545" opacity="0.1" />
                                        <text x="185" y="19" text-anchor="middle" font-size="14" fill="#dc3545" font-weight="bold">×</text>
                                    </g>
                                </g>
                            </g>
                        </svg>
                    </div>
                </div>

                <!-- Right Panel: Property Panel -->
                <div v-if="showPropertyPanel && selectedActivityForProps" class="bg-white border-start d-flex flex-column" style="width: 320px; box-shadow: -2px 0 8px rgba(0,0,0,0.1);">
                    <div class="p-3 border-bottom">
                        <div class="d-flex justify-content-between align-items-center mb-2">
                            <h6 class="m-0 fw-bold">Properties</h6>
                            <button class="btn btn-sm btn-link" @click="showPropertyPanel = false" style="padding: 0;">
                                <i class="fa-solid fa-times"></i>
                            </button>
                        </div>
                        <small class="text-muted">{{ selectedActivityForProps.type }}</small>
                    </div>
                    
                    <div class="flex-grow-1 overflow-auto p-3">
                        <!-- Activity Name -->
                        <div class="mb-3">
                            <label class="form-label small fw-bold">Name</label>
                            <input type="text" class="form-control form-control-sm" 
                                :value="selectedActivityForProps.name"
                                @input="updateActivityProperty('name', $event.target.value)">
                        </div>
                        
                        <!-- Activity Description -->
                        <div class="mb-3">
                            <label class="form-label small fw-bold">Description</label>
                            <textarea class="form-control form-control-sm" rows="2"
                                :value="selectedActivityForProps.text"
                                @input="updateActivityProperty('text', $event.target.value)"></textarea>
                        </div>
                        
                        <!-- If Activity Condition (Priority) -->
                        <div v-if="selectedActivityForProps.type === 'If'" class="mb-3 p-2 bg-light rounded border border-warning">
                            <label class="form-label small fw-bold text-warning">
                                <i class="fa-solid fa-code-branch me-1"></i>Condition (For True Branch)
                            </label>
                            <input type="text" class="form-control form-control-sm"
                                :value="selectedActivityForProps.condition"
                                placeholder="e.g., x > 10 || status == 'active'"
                                @input="updateActivityProperty('condition', $event.target.value)">
                            <small class="text-muted d-block mt-2">
                                اگر این شرط صحیح باشد، جریان به شاخه True می‌رود<br/>
                                Otherwise، جریان به شاخه False می‌رود
                            </small>
                        </div>
                        
                        <!-- Common Properties -->
                        <div v-if="selectedActivityForProps.variableName !== undefined" class="mb-3">
                            <label class="form-label small fw-bold">Variable Name</label>
                            <input type="text" class="form-control form-control-sm"
                                :value="selectedActivityForProps.variableName"
                                @input="updateActivityProperty('variableName', $event.target.value)">
                        </div>
                        
                        <div v-if="selectedActivityForProps.value !== undefined" class="mb-3">
                            <label class="form-label small fw-bold">Value</label>
                            <input type="text" class="form-control form-control-sm"
                                :value="selectedActivityForProps.value"
                                @input="updateActivityProperty('value', $event.target.value)">
                        </div>
                        
                        <div v-if="selectedActivityForProps.condition !== undefined && selectedActivityForProps.type !== 'If'" class="mb-3">
                            <label class="form-label small fw-bold">Condition</label>
                            <input type="text" class="form-control form-control-sm"
                                :value="selectedActivityForProps.condition"
                                @input="updateActivityProperty('condition', $event.target.value)">
                        </div>
                        
                        <div v-if="selectedActivityForProps.url !== undefined" class="mb-3">
                            <label class="form-label small fw-bold">URL</label>
                            <input type="text" class="form-control form-control-sm"
                                :value="selectedActivityForProps.url"
                                @input="updateActivityProperty('url', $event.target.value)">
                        </div>
                        
                        <div v-if="selectedActivityForProps.method !== undefined" class="mb-3">
                            <label class="form-label small fw-bold">Method</label>
                            <select class="form-select form-select-sm"
                                :value="selectedActivityForProps.method"
                                @input="updateActivityProperty('method', $event.target.value)">
                                <option value="">Select...</option>
                                <option value="GET">GET</option>
                                <option value="POST">POST</option>
                                <option value="PUT">PUT</option>
                                <option value="DELETE">DELETE</option>
                                <option value="PATCH">PATCH</option>
                            </select>
                        </div>
                        
                        <div v-if="selectedActivityForProps.duration !== undefined" class="mb-3">
                            <label class="form-label small fw-bold">Duration</label>
                            <input type="text" class="form-control form-control-sm" placeholder="00:00:10"
                                :value="selectedActivityForProps.duration"
                                @input="updateActivityProperty('duration', $event.target.value)">
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    let _this = { 
        cid: "", 
        c: null, 
        workflowId: null, 
        workflow: {
            id: '',
            name: 'New Workflow',
            description: '',
            version: 1,
            isPublished: false,
            activities: [],
            connections: [] // Add connections array
        },
        searchQuery: '',
        selectedActivityId: null,
        canvasWidth: 1200,
        canvasHeight: 800,
        zoomLevel: 1,
        panX: 0,
        panY: 0,
        isPanning: false,
        panStartX: 0,
        panStartY: 0,
        draggedActivity: null,
        draggedActivityTemplate: null,
        dragStartX: 0,
        dragStartY: 0,
        dragActivityStartX: 0,
        dragActivityStartY: 0,
        expandedCategories: {},
        selectedConnection: null,
        expandedCategoryIndex: 0, // Track single expanded category
        autoSaveTimeout: null,
        showPropertyPanel: false,
        selectedActivityForProps: null
    };

    export default {
        methods: {
            notifySafe(message, type = 'info') {
                try {
                    if (typeof shared !== 'undefined' && shared && typeof shared.notify === 'function') {
                        shared.notify(message, type);
                    } else {
                        console.warn(`[${type.toUpperCase()}] ${message}`);
                    }
                } catch (e) {
                    console.error('Notification error:', e);
                }
            },

            loadWorkflow() {
                if (!_this.c.workflowId) return;

                rpcAEP('GetWorkflowDefinition', { WorkflowId: _this.c.workflowId }, (data) => {
                    const payload = Array.isArray(data) ? (data[0] || {}) : (data || {});
                    const wf = payload.Result || payload.result || payload;
                    
                    console.log('📥 Workflow loaded:', wf);
                    
                    if (wf && wf.Id) {
                        // Parse RawJson if exists
                        let fullDefinition = wf;
                        if (wf.RawJson) {
                            try {
                                fullDefinition = JSON.parse(wf.RawJson);
                                console.log('✅ Parsed RawJson:', fullDefinition);
                            } catch (e) {
                                console.error('❌ Failed to parse RawJson:', e);
                            }
                        }
                        
                        _this.c.workflow = {
                            id: wf.Id,
                            name: wf.Name || fullDefinition.name || 'Workflow',
                            description: wf.Description || fullDefinition.description || '',
                            version: wf.Version || fullDefinition.version || 1,
                            isPublished: wf.IsPublished || fullDefinition.isPublished || false,
                            activities: this.parseActivities(fullDefinition),
                            connections: fullDefinition.root?.connections || fullDefinition.Root?.connections || []
                        };
                        
                        console.log('✅ Parsed workflow:', _this.c.workflow);
                        console.log('📋 Activities count:', _this.c.workflow.activities.length);
                        console.log('🔗 Connections count:', _this.c.workflow.connections.length);
                    }
                }, (error) => {
                    console.error('❌ Error loading workflow:', error);
                    this.notifySafe('Error loading workflow: ' + error, 'error');
                });
            },

            parseActivities(workflowData) {
                console.log('🔄 Parsing workflow data:', workflowData);
                console.log('🔍 Available keys:', Object.keys(workflowData));
                console.log('🔍 Has root?', !!workflowData.root);
                console.log('🔍 Has Root?', !!workflowData.Root);
                console.log('🔍 Has activities?', !!workflowData.activities);
                console.log('🔍 Has Activities?', !!workflowData.Activities);
                
                // Check if workflow has Elsa V3 format (root.activities) - lowercase
                if (workflowData.root && workflowData.root.activities) {
                    console.log('📦 Found Elsa V3 format (root.activities)');
                    return this.flattenActivities(workflowData.root.activities);
                }
                
                // Check if workflow has Elsa V3 format (Root.Activities) - uppercase
                if (workflowData.Root && workflowData.Root.Activities) {
                    console.log('📦 Found Elsa V3 format (Root.Activities)');
                    return this.flattenActivities(workflowData.Root.Activities);
                }
                
                // Check if workflow has Elsa V3 format (Root.activities) - mixed
                if (workflowData.Root && workflowData.Root.activities) {
                    console.log('📦 Found Elsa V3 format (Root.activities)');
                    return this.flattenActivities(workflowData.Root.activities);
                }
                
                // Check if workflow has direct activities array
                if (workflowData.Activities && Array.isArray(workflowData.Activities)) {
                    console.log('📦 Found direct Activities array');
                    return this.flattenActivities(workflowData.Activities);
                }
                
                // Check if workflow itself is root container
                if (workflowData.activities && Array.isArray(workflowData.activities)) {
                    console.log('📦 Found activities in workflow root');
                    return this.flattenActivities(workflowData.activities);
                }
                
                console.log('⚠️ No activities found in workflow data');
                console.log('⚠️ Full workflow data:', JSON.stringify(workflowData, null, 2));
                return [];
            },
            
            flattenActivities(activities, level = 0) {
                if (!activities || !Array.isArray(activities)) return [];
                
                const result = [];
                
                activities.forEach((act, idx) => {
                    // Add this activity
                    const activity = {
                        id: act.id || ('act_' + idx + '_' + Date.now()),
                        type: act.type || 'WriteLine',
                        name: act.displayName || act.name || '',
                        text: act.text || '',
                        path: act.path || '',
                        variableName: act.variableName || '',
                        value: act.value || '',
                        condition: act.condition || '',
                        url: act.url || '',
                        method: act.method || '',
                        duration: act.duration || '',
                        items: act.items || [],
                        level: level,
                        x: act.x || (50 + (level * 250)),
                        y: act.y || (50 + (idx * 120))
                    };
                    
                    result.push(activity);
                    
                    // If this activity has nested activities (like Sequence, If, etc.), flatten them too
                    if (act.activities && Array.isArray(act.activities)) {
                        const nested = this.flattenActivities(act.activities, level + 1);
                        result.push(...nested);
                    }
                });
                
                console.log('✅ Flattened activities:', result);
                return result;
            },
            
            parseConnections(workflowData) {
                const connections = [];
                
                // Example parsing logic - modify based on actual workflow data structure
                workflowData.activities.forEach((activity, index) => {
                    if (activity.nextActivityId) {
                        const targetActivity = workflowData.activities.find(a => a.id === activity.nextActivityId);
                        if (targetActivity) {
                            connections.push({
                                id: 'conn_' + activity.id + '_' + targetActivity.id,
                                source: activity,
                                target: targetActivity
                            });
                        }
                    }
                });
                
                console.log('✅ Parsed connections:', connections);
                return connections;
            },

            addActivity(activityTemplate) {
                const newActivity = {
                    id: 'act_' + Date.now() + '_' + Math.random().toString(36).substr(2, 9),
                    type: activityTemplate.type,
                    name: activityTemplate.name,
                    x: 50 + (_this.c.workflow.activities.length % 3) * 250,
                    y: 50 + Math.floor(_this.c.workflow.activities.length / 3) * 120,
                    level: 0,
                    ...activityTemplate.defaultProps
                };
                _this.c.workflow.activities.push(newActivity);
                _this.c.selectedActivityId = newActivity.id;
            },

            removeActivity(index) {
                _this.c.workflow.activities.splice(index, 1);
                _this.c.selectedActivityId = null;
            },

            selectActivity(activity) {
                _this.c.selectedActivityId = activity.id;
                this.showPropertyPanel = true;
                this.selectedActivityForProps = { ...activity }; // Clone for editing
            },

            selectConnection(connection) {
                this.selectedConnection = connection;
                this.showPropertyPanel = true;
            },

            getActivityTypeName(type) {
                const template = this.activityCategories
                    .flatMap(c => c.activities)
                    .find(a => a.type === type);
                return template?.name || type;
            },

            getActivityIcon(type) {
                const template = this.activityCategories
                    .flatMap(c => c.activities)
                    .find(a => a.type === type);
                return template?.icon || 'fa-solid fa-circle';
            },

            getActivityColor(type) {
                const template = this.activityCategories
                    .flatMap(c => c.activities)
                    .find(a => a.type === type);
                
                if (template?.color) return template.color;
                
                // Fallback colors by type prefix
                if (type.startsWith('Http')) return '#0d6efd';
                if (type.includes('Variable') || type.includes('Output')) return '#198754';
                if (type === 'If' || type === 'Switch' || type === 'ForEach' || type === 'While') return '#ffc107';
                if (type === 'WriteLine' || type === 'WriteObject') return '#6c757d';
                if (type.includes('JavaScript') || type.includes('CSharp')) return '#e83e8c';
                if (type.includes('Email')) return '#17a2b8';
                if (type.includes('File')) return '#fd7e14';
                if (type.includes('Timer') || type === 'Cron' || type === 'StartAt') return '#20c997';
                if (type === 'Sequence' || type === 'Flowchart' || type.includes('Workflow')) return '#6610f2';
                
                return '#6c757d';
            },

            closeDesigner() {
                closeComponent(_this.cid);
            },

            getActivityEmoji(type) {
                const emojiMap = {
                    // HTTP
                    'HttpEndpoint': '🌐',
                    'SendHttpRequest': '📤',
                    'WriteHttpResponse': '📨',
                    
                    // Control Flow
                    'If': '🔀',
                    'Switch': '🔀',
                    'ForEach': '🔁',
                    'While': '🔄',
                    'Break': '🛑',
                    'Delay': '⏱️',
                    'Finish': '🏁',
                    'Fault': '⚠️',
                    
                    // Variables & Data
                    'SetVariable': '📦',
                    'GetVariable': '📥',
                    'SetOutput': '📤',
                    
                    // Console & Logging
                    'WriteLine': '📝',
                    'WriteObject': '📋',
                    
                    // Scripting
                    'RunJavaScript': '📜',
                    'RunCSharp': '⚙️',
                    
                    // Email
                    'SendEmail': '✉️',
                    
                    // File System
                    'ReadFile': '📖',
                    'WriteFile': '📝',
                    'DeleteFile': '🗑️',
                    
                    // Timer
                    'Timer': '⏰',
                    'Cron': '📅',
                    'StartAt': '🕐',
                    
                    // Workflow
                    'Sequence': '📋',
                    'Flowchart': '🗺️',
                    'RunWorkflow': '▶️'
                };
                return emojiMap[type] || '⚙️';
            },

            truncate(text, maxLength) {
                if (!text) return '';
                return text.length > maxLength ? text.substring(0, maxLength) + '...' : text;
            },

            // Zoom controls
            zoomIn() {
                _this.c.zoomLevel = Math.min(2, _this.c.zoomLevel + 0.1);
            },

            zoomOut() {
                _this.c.zoomLevel = Math.max(0.3, _this.c.zoomLevel - 0.1);
            },

            zoomReset() {
                _this.c.zoomLevel = 1;
                _this.c.panX = 0;
                _this.c.panY = 0;
            },

            // Pan controls
            onCanvasMouseDown(e) {
                if (e.button === 0 && !e.target.closest('g[style*="cursor: move"]')) {
                    _this.c.isPanning = true;
                    _this.c.panStartX = e.clientX - _this.c.panX;
                    _this.c.panStartY = e.clientY - _this.c.panY;
                    e.currentTarget.style.cursor = 'grabbing';
                }
            },

            onCanvasMouseMove(e) {
                if (_this.c.isPanning) {
                    _this.c.panX = e.clientX - _this.c.panStartX;
                    _this.c.panY = e.clientY - _this.c.panStartY;
                } else if (_this.c.draggedActivity) {
                    const zoom = _this.c.zoomLevel;
                    const dx = (e.clientX - _this.c.dragStartX) / zoom;
                    const dy = (e.clientY - _this.c.dragStartY) / zoom;
                    
                    _this.c.draggedActivity.x = _this.c.dragActivityStartX + dx;
                    _this.c.draggedActivity.y = _this.c.dragActivityStartY + dy;
                }
            },

            onCanvasMouseUp(e) {
                _this.c.isPanning = false;
                if (_this.c.draggedActivity) {
                    _this.c.draggedActivity = null;
                    this.autoSave(); // Auto-save when activity is dropped
                }
                if (e.currentTarget) {
                    e.currentTarget.style.cursor = 'grab';
                }
            },

            onCanvasWheel(e) {
                e.preventDefault();
                const delta = e.deltaY > 0 ? -0.05 : 0.05;
                _this.c.zoomLevel = Math.max(0.3, Math.min(2, _this.c.zoomLevel + delta));
            },

            // Drag activity
            startDragActivity(e, activity) {
                _this.c.draggedActivity = activity;
                _this.c.dragStartX = e.clientX;
                _this.c.dragStartY = e.clientY;
                _this.c.dragActivityStartX = activity.x;
                _this.c.dragActivityStartY = activity.y;
            },

            // Drag & Drop from Activity Toolbox
            onActivityDragStart(e, activity) {
                _this.c.draggedActivityTemplate = activity;
                e.dataTransfer.effectAllowed = 'copy';
            },

            onCanvasDragOver(e) {
                e.preventDefault();
                e.dataTransfer.dropEffect = 'copy';
            },

            onCanvasDrop(e) {
                e.preventDefault();
                
                if (!_this.c.draggedActivityTemplate) return;
                
                // Get canvas container position
                const canvasRect = this.$refs.canvasContainer.getBoundingClientRect();
                
                // Calculate drop position relative to canvas
                let x = (e.clientX - canvasRect.left - _this.c.panX) / _this.c.zoomLevel;
                let y = (e.clientY - canvasRect.top - _this.c.panY) / _this.c.zoomLevel;
                
                // Ensure minimum values
                x = Math.max(0, x);
                y = Math.max(0, y);
                
                const newActivity = {
                    id: 'act_' + Date.now() + '_' + Math.random().toString(36).substr(2, 9),
                    type: _this.c.draggedActivityTemplate.type,
                    name: _this.c.draggedActivityTemplate.name,
                    x: x,
                    y: y,
                    level: 0,
                    ..._this.c.draggedActivityTemplate.defaultProps
                };
                
                _this.c.workflow.activities.push(newActivity);
                _this.c.draggedActivityTemplate = null;
                this.autoSave();
            },

            // Auto arrange activities
            autoArrange() {
                const activities = _this.c.workflow.activities;
                if (activities.length === 0) {
                    this.notifySafe('No activities to arrange', 'info');
                    return;
                }

                const horizontalSpacing = 250;
                const verticalSpacing = 120;
                const maxPerRow = 3;

                activities.forEach((activity, index) => {
                    const level = activity.level || 0;
                    const rowIndex = Math.floor(index / maxPerRow);
                    const colIndex = index % maxPerRow;

                    activity.x = 50 + (colIndex * horizontalSpacing) + (level * 50);
                    activity.y = 50 + (rowIndex * verticalSpacing);
                });

                this.notifySafe('Activities arranged automatically', 'success');
                this.autoSave();
            },

            autoSave() {
                // Auto-save with debounce
                if (_this.c.autoSaveTimeout) {
                    clearTimeout(_this.c.autoSaveTimeout);
                }
                _this.c.autoSaveTimeout = setTimeout(() => {
                    this.saveWorkflow();
                }, 3000); // Save after 3 seconds of inactivity
            },

            selectActivityForProps(activity) {
                _this.c.selectedActivityForProps = activity;
                _this.c.showPropertyPanel = true;
            },

            updateActivityProperty(propName, value) {
                if (_this.c.selectedActivityForProps) {
                    _this.c.selectedActivityForProps[propName] = value;
                    this.autoSave();
                }
            },

            createConnection(fromActivityId, toActivityId, fromPort = 'output', toPort = 'input') {
                const fromActivity = this.getActivityById(fromActivityId);
                const connection = {
                    id: 'conn_' + Date.now() + '_' + Math.random().toString(36).substr(2, 9),
                    from: fromActivityId,
                    to: toActivityId,
                    fromPort: fromPort,
                    toPort: toPort,
                    label: fromActivity && (fromActivity.type === 'If' && fromPort === 'true') ? '✓' : 
                           (fromActivity && (fromActivity.type === 'If' && fromPort === 'false') ? '✗' : '')
                };
                _this.c.workflow.connections.push(connection);
                this.autoSave();
                return connection;
            },

            removeConnection(connectionId) {
                const index = _this.c.workflow.connections.findIndex(c => c.id === connectionId);
                if (index > -1) {
                    _this.c.workflow.connections.splice(index, 1);
                    this.autoSave();
                }
            },

            getActivityById(activityId) {
                return _this.c.workflow.activities.find(a => a.id === activityId);
            },

            getConnectionPath(connection) {
                const fromActivity = this.getActivityById(connection.from);
                const toActivity = this.getActivityById(connection.to);
                
                if (!fromActivity || !toActivity) return '';
                
                // Special handling for If/Switch nodes
                let x1, y1;
                if (fromActivity.type === 'If') {
                    // For If nodes, determine which port (True or False)
                    if (connection.fromPort === 'true') {
                        x1 = fromActivity.x + 100;
                        y1 = fromActivity.y + 85; // Bottom True port
                    } else {
                        x1 = fromActivity.x + 100;
                        y1 = fromActivity.y - 5; // Top False port
                    }
                } else {
                    // Regular output port
                    x1 = fromActivity.x + 200;
                    y1 = fromActivity.y + 40;
                }
                
                // End at input port of to-activity (left side)
                const x2 = toActivity.x;
                const y2 = toActivity.y + 40;
                
                // Bezier curve for smart connections
                const dx = (x2 - x1) / 2;
                return `M ${x1} ${y1} C ${x1 + dx} ${y1}, ${x1 + dx} ${y2}, ${x2} ${y2}`;
            },

            getConnectionMidpoint(connection) {
                const fromActivity = this.getActivityById(connection.from);
                const toActivity = this.getActivityById(connection.to);
                
                if (!fromActivity || !toActivity) return { x: 0, y: 0 };
                
                let x1, y1;
                if (fromActivity.type === 'If') {
                    if (connection.fromPort === 'true') {
                        x1 = fromActivity.x + 100;
                        y1 = fromActivity.y + 85;
                    } else {
                        x1 = fromActivity.x + 100;
                        y1 = fromActivity.y - 5;
                    }
                } else {
                    x1 = fromActivity.x + 200;
                    y1 = fromActivity.y + 40;
                }
                
                const x2 = toActivity.x;
                const y2 = toActivity.y + 40;
                
                return {
                    x: (x1 + x2) / 2,
                    y: (y1 + y2) / 2
                };
            },

            saveWorkflow() {
                const workflowData = {
                    Id: _this.c.workflow.id,
                    Name: _this.c.workflow.name,
                    Description: _this.c.workflow.description,
                    Version: _this.c.workflow.version,
                    IsPublished: _this.c.workflow.isPublished,
                    RawJson: JSON.stringify({
                        id: _this.c.workflow.id,
                        name: _this.c.workflow.name,
                        description: _this.c.workflow.description,
                        version: _this.c.workflow.version,
                        isPublished: _this.c.workflow.isPublished,
                        root: {
                            type: 'Elsa.Flowchart',
                            activities: _this.c.workflow.activities.map(act => ({
                                id: act.id,
                                type: act.type,
                                displayName: act.name,
                                text: act.text,
                                path: act.path,
                                variableName: act.variableName,
                                value: act.value,
                                condition: act.condition,
                                url: act.url,
                                method: act.method,
                                duration: act.duration,
                                items: act.items,
                                x: act.x,
                                y: act.y
                            })),
                            connections: _this.c.workflow.connections // Save connections
                        }
                    })
                };

                rpcAEP('SaveWorkflowDefinition', workflowData, (data) => {
                    const payload = Array.isArray(data) ? (data[0] || {}) : (data || {});
                    const result = payload.Result || payload.result || payload;
                    
                    if (result.Success || result.success) {
                        this.notifySafe('Workflow saved successfully', 'success');
                        closeComponent(_this.cid, { success: true });
                    } else {
                        const error = result.ErrorMessage || result.errorMessage || 'Failed to save';
                        this.notifySafe('Error: ' + error, 'error');
                    }
                }, (error) => {
                    console.error('Save error:', error);
                    this.notifySafe('Error saving workflow: ' + error, 'error');
                });
            },

            toggleCategory(catIndex) {
                // Accordion behavior: close other categories when opening one
                if (this.expandedCategories[catIndex]) {
                    // If already expanded, just close it
                    this.expandedCategories[catIndex] = false;
                } else {
                    // Close all others first
                    Object.keys(this.expandedCategories).forEach(key => {
                        this.expandedCategories[key] = false;
                    });
                    // Then open this one
                    this.expandedCategories[catIndex] = true;
                }
            }
        },
        computed: {
            activityCategories() {
                return [
                    {
                        name: 'HTTP',
                        icon: 'fa-solid fa-globe',
                        color: '#0d6efd',
                        activities: [
                            { 
                                type: 'HttpEndpoint', 
                                name: 'HTTP Endpoint', 
                                icon: 'fa-solid fa-plug',
                                color: '#0d6efd',
                                description: 'Trigger workflow via HTTP request',
                                defaultProps: { path: '/api/workflow', method: 'POST' }
                            },
                            { 
                                type: 'SendHttpRequest', 
                                name: 'Send HTTP Request', 
                                icon: 'fa-solid fa-paper-plane',
                                color: '#0d6efd',
                                description: 'Send HTTP request to external API',
                                defaultProps: { url: '', method: 'GET' }
                            },
                            { 
                                type: 'WriteHttpResponse', 
                                name: 'Write HTTP Response', 
                                icon: 'fa-solid fa-reply',
                                color: '#0d6efd',
                                description: 'Send HTTP response back to caller',
                                defaultProps: { content: '', statusCode: 200 }
                            }
                        ]
                    },
                    {
                        name: 'Control Flow',
                        icon: 'fa-solid fa-code-branch',
                        color: '#ffc107',
                        activities: [
                            { 
                                type: 'If', 
                                name: 'If Condition', 
                                icon: 'fa-solid fa-code-branch',
                                color: '#ffc107',
                                description: 'Conditional branching based on expression',
                                defaultProps: { condition: '' }
                            },
                            { 
                                type: 'Switch', 
                                name: 'Switch', 
                                icon: 'fa-solid fa-shuffle',
                                color: '#ffc107',
                                description: 'Multi-way branching',
                                defaultProps: { cases: [] }
                            },
                            { 
                                type: 'ForEach', 
                                name: 'For Each', 
                                icon: 'fa-solid fa-repeat',
                                color: '#ffc107',
                                description: 'Loop through collection items',
                                defaultProps: { items: [] }
                            },
                            { 
                                type: 'While', 
                                name: 'While Loop', 
                                icon: 'fa-solid fa-rotate',
                                color: '#ffc107',
                                description: 'Loop while condition is true',
                                defaultProps: { condition: '' }
                            },
                            { 
                                type: 'Break', 
                                name: 'Break', 
                                icon: 'fa-solid fa-hand',
                                color: '#dc3545',
                                description: 'Break out of loop',
                                defaultProps: {}
                            },
                            { 
                                type: 'Delay', 
                                name: 'Delay', 
                                icon: 'fa-solid fa-clock',
                                color: '#6c757d',
                                description: 'Wait for specified duration',
                                defaultProps: { duration: '00:00:10' }
                            },
                            { 
                                type: 'Finish', 
                                name: 'Finish', 
                                icon: 'fa-solid fa-flag-checkered',
                                color: '#198754',
                                description: 'End workflow execution',
                                defaultProps: {}
                            },
                            { 
                                type: 'Fault', 
                                name: 'Fault', 
                                icon: 'fa-solid fa-circle-exclamation',
                                color: '#dc3545',
                                description: 'Throw fault/error',
                                defaultProps: { message: '' }
                            }
                        ]
                    },
                    {
                        name: 'Variables & Data',
                        icon: 'fa-solid fa-database',
                        color: '#198754',
                        activities: [
                            { 
                                type: 'SetVariable', 
                                name: 'Set Variable', 
                                icon: 'fa-solid fa-box',
                                color: '#198754',
                                description: 'Set or update variable value',
                                defaultProps: { variableName: '', value: '' }
                            },
                            { 
                                type: 'SetOutput', 
                                name: 'Set Output', 
                                icon: 'fa-solid fa-arrow-up-from-bracket',
                                color: '#198754',
                                description: 'Set workflow output value',
                                defaultProps: { outputName: '', value: '' }
                            },
                            { 
                                type: 'GetVariable', 
                                name: 'Get Variable', 
                                icon: 'fa-solid fa-download',
                                color: '#198754',
                                description: 'Get variable value',
                                defaultProps: { variableName: '' }
                            }
                        ]
                    },
                    {
                        name: 'Console & Logging',
                        icon: 'fa-solid fa-terminal',
                        color: '#6c757d',
                        activities: [
                            { 
                                type: 'WriteLine', 
                                name: 'Write Line', 
                                icon: 'fa-solid fa-terminal',
                                color: '#6c757d',
                                description: 'Write line to console/log',
                                defaultProps: { text: '' }
                            },
                            { 
                                type: 'WriteObject', 
                                name: 'Write Object', 
                                icon: 'fa-solid fa-file-code',
                                color: '#6c757d',
                                description: 'Write object to log',
                                defaultProps: { value: {} }
                            }
                        ]
                    },
                    {
                        name: 'Scripting',
                        icon: 'fa-solid fa-file-code',
                        color: '#e83e8c',
                        activities: [
                            { 
                                type: 'RunJavaScript', 
                                name: 'Run JavaScript', 
                                icon: 'fa-brands fa-js',
                                color: '#f7df1e',
                                description: 'Execute JavaScript code',
                                defaultProps: { script: 'return {};' }
                            },
                            { 
                                type: 'RunCSharp', 
                                name: 'Run C#', 
                                icon: 'fa-solid fa-code',
                                color: '#68217a',
                                description: 'Execute C# code',
                                defaultProps: { code: '' }
                            }
                        ]
                    },
                    {
                        name: 'Email',
                        icon: 'fa-solid fa-envelope',
                        color: '#17a2b8',
                        activities: [
                            { 
                                type: 'SendEmail', 
                                name: 'Send Email', 
                                icon: 'fa-solid fa-envelope',
                                color: '#17a2b8',
                                description: 'Send email message',
                                defaultProps: { to: '', subject: '', body: '' }
                            }
                        ]
                    },
                    {
                        name: 'File System',
                        icon: 'fa-solid fa-folder',
                        color: '#fd7e14',
                        activities: [
                            { 
                                type: 'ReadFile', 
                                name: 'Read File', 
                                icon: 'fa-solid fa-file-arrow-down',
                                color: '#fd7e14',
                                description: 'Read file from disk',
                                defaultProps: { path: '' }
                            },
                            { 
                                type: 'WriteFile', 
                                name: 'Write File', 
                                icon: 'fa-solid fa-file-arrow-up',
                                color: '#fd7e14',
                                description: 'Write file to disk',
                                defaultProps: { path: '', content: '' }
                            },
                            { 
                                type: 'DeleteFile', 
                                name: 'Delete File', 
                                icon: 'fa-solid fa-file-circle-xmark',
                                color: '#dc3545',
                                description: 'Delete file from disk',
                                defaultProps: { path: '' }
                            }
                        ]
                    },
                    {
                        name: 'Timer',
                        icon: 'fa-solid fa-clock',
                        color: '#20c997',
                        activities: [
                            { 
                                type: 'Timer', 
                                name: 'Timer', 
                                icon: 'fa-solid fa-stopwatch',
                                color: '#20c997',
                                description: 'Trigger at specific time',
                                defaultProps: { timeout: '00:01:00' }
                            },
                            { 
                                type: 'Cron', 
                                name: 'Cron Schedule', 
                                icon: 'fa-solid fa-calendar-days',
                                color: '#20c997',
                                description: 'Schedule using cron expression',
                                defaultProps: { cronExpression: '0 0 * * *' }
                            },
                            { 
                                type: 'StartAt', 
                                name: 'Start At', 
                                icon: 'fa-solid fa-calendar-check',
                                color: '#20c997',
                                description: 'Start at specific date/time',
                                defaultProps: { instant: '' }
                            }
                        ]
                    },
                    {
                        name: 'Workflow',
                        icon: 'fa-solid fa-diagram-project',
                        color: '#6610f2',
                        activities: [
                            { 
                                type: 'Sequence', 
                                name: 'Sequence', 
                                icon: 'fa-solid fa-list-ol',
                                color: '#6610f2',
                                description: 'Execute activities in sequence',
                                defaultProps: { activities: [] }
                            },
                            { 
                                type: 'Flowchart', 
                                name: 'Flowchart', 
                                icon: 'fa-solid fa-diagram-project',
                                color: '#6610f2',
                                description: 'Execute activities as flowchart',
                                defaultProps: { activities: [] }
                            },
                            { 
                                type: 'RunWorkflow', 
                                name: 'Run Workflow', 
                                icon: 'fa-solid fa-play',
                                color: '#6610f2',
                                description: 'Execute another workflow',
                                defaultProps: { workflowDefinitionId: '' }
                            }
                        ]
                    }
                ];
            }
        },
        setup(props) {
            _this.cid = props['cid'];

            const params = shared["params_" + _this.cid];
            if (params) {
                _this.workflowId = params.workflowId || null;
            }
        },
        data() {
            return {
                workflowId: _this.workflowId,
                workflow: _this.workflow,
                searchQuery: _this.searchQuery,
                selectedActivityId: _this.selectedActivityId,
                canvasWidth: _this.canvasWidth,
                canvasHeight: _this.canvasHeight,
                zoomLevel: _this.zoomLevel,
                panX: _this.panX,
                panY: _this.panY,
                expandedCategories: _this.expandedCategories,
                selectedConnection: _this.selectedConnection,
                autoSaveTimeout: _this.autoSaveTimeout,
                showPropertyPanel: _this.showPropertyPanel,
                selectedActivityForProps: _this.selectedActivityForProps
            };
        },
        created() { 
            _this.c = this;
            
            // Initialize all categories as collapsed (except first one)
            // This ensures Vue 3 reactivity works properly
            this.activityCategories.forEach((cat, index) => {
                this.expandedCategories[index] = index === 0; // Only first category expanded by default
            });
        },
        mounted() {
            if (_this.c.workflowId) {
                this.loadWorkflow();
            }

            // Initialize SVG canvas size
            const updateCanvasSize = () => {
                _this.c.canvasWidth = Math.max(1200, window.innerWidth - 350);
                _this.c.canvasHeight = Math.max(800, window.innerHeight - 100);
            };

            updateCanvasSize();
            window.addEventListener('resize', updateCanvasSize);
        },
        unmounted() {
            window.removeEventListener('resize', updateCanvasSize);
        },
        props: { cid: String }
    }
</script>

<style scoped>
/* Mifty-inspired Sidebar Styling */

/* Category Header Hover */
.category-header {
    transition: all 0.2s ease;
}

.category-header:hover {
    background-color: #f8f9fa;
}

/* Activity Item Styling */
.activity-item {
    transition: all 0.2s ease;
    border: 1px solid transparent;
}

.activity-item:hover {
    background-color: #f8f9fa;
    border-color: #e9ecef;
    transform: translateX(2px);
}

.activity-item:active {
    transform: translateX(0);
    background-color: #e9ecef;
}

/* Rotate chevron icon when expanded */
.rotate-180 {
    transform: rotate(180deg);
}

.transition-transform {
    transition: transform 0.2s ease;
}

/* Category Items Slide Animation */
.category-items {
    animation: slideDown 0.2s ease;
}

@keyframes slideDown {
    from {
        opacity: 0;
        transform: translateY(-10px);
    }
    to {
        opacity: 1;
        transform: translateY(0);
    }
}

/* Custom Scrollbar (Mifty Style) */
.overflow-auto::-webkit-scrollbar {
    width: 6px;
}

.overflow-auto::-webkit-scrollbar-track {
    background: transparent;
}

.overflow-auto::-webkit-scrollbar-thumb {
    background: #cbd5e0;
    border-radius: 10px;
}

.overflow-auto::-webkit-scrollbar-thumb:hover {
    background: #a0aec0;
}

/* Badge Styling */
.badge {
    font-weight: 600;
    padding: 2px 8px;
}

/* Canvas styling */
.hover-shadow:hover {
    box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.activity-detail {
    transition: opacity 0.3s;
}

/* Sticky header for activity toolbox */
.sticky-top {
    position: sticky;
    top: 0;
    z-index: 1020;
}

/* Remove default accordion borders */
.accordion-item {
    background-color: transparent;
}

.accordion-button {
    border: none;
}

/* Activity item hover effect */
.activity-item:hover {
    box-shadow: 0 2px 8px rgba(0,0,0,0.15);
    transform: translateY(-1px);
    border-color: #dee2e6 !important;
}

.activity-item:active {
    transform: translateY(0);
    box-shadow: 0 1px 3px rgba(0,0,0,0.1);
}

/* Input/Output Ports */
.input-port {
    cursor: crosshair;
    transition: r 0.2s ease;
}

.input-port:hover {
    r: 6;
}

.output-port {
    cursor: crosshair;
    transition: r 0.2s ease;
}

.output-port:hover {
    r: 6;
}

/* Property Panel Styling */
.form-control-sm, .form-select-sm {
    font-size: 0.85rem;
    height: calc(1.5em + 0.5rem + 2px);
}

.form-label {
    margin-bottom: 0.25rem;
    color: #495057;
}

/* Connection Line Styles */
path {
    stroke-dasharray: none;
    transition: stroke 0.2s ease;
}

path:hover {
    stroke-width: 3 !important;
    opacity: 0.8;
}

/* Sticky header for property panel */
.sticky-top {
    position: sticky;
    top: 0;
    z-index: 1020;
    background-color: #ffffff;
    padding: 0.5rem 1rem;
    border-bottom: 1px solid #e9ecef;
}

/* Auto-save notification style */
.notify-auto-save {
    position: fixed;
    top: 20px;
    right: 20px;
    z-index: 1050;
    background-color: #28a745;
    color: white;
    padding: 10px 20px;
    border-radius: 5px;
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
    transition: opacity 0.3s ease;
}

.notify-auto-save.hide {
    opacity: 0;
    pointer-events: none;
}
</style>
