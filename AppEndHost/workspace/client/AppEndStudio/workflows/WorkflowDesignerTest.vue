<template>
    <div class="container-fluid py-4">
        <div class="row">
            <div class="col-12">
                <div class="card">
                    <div class="card-header bg-primary text-white">
                        <h5 class="m-0">Workflow Designer - Test Panel</h5>
                    </div>
                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-6">
                                <h6>Quick Actions</h6>
                                <button class="btn btn-primary btn-sm me-2 mb-2" @click="openWorkflowEditor">
                                    <i class="fa-solid fa-edit me-1"></i>Open Editor
                                </button>
                                <button class="btn btn-info btn-sm me-2 mb-2" @click="testNodeTypes">
                                    <i class="fa-solid fa-cube me-1"></i>Test Node Types
                                </button>
                                <button class="btn btn-warning btn-sm me-2 mb-2" @click="showNodeCategories">
                                    <i class="fa-solid fa-layer-group me-1"></i>Show Categories
                                </button>
                                <button class="btn btn-success btn-sm me-2 mb-2" @click="testValidation">
                                    <i class="fa-solid fa-check me-1"></i>Test Validation
                                </button>
                            </div>
                            <div class="col-md-6">
                                <h6>Info</h6>
                                <div class="alert alert-info small mb-0">
                                    <strong>Status:</strong> Ready for testing
                                    <br>
                                    <strong>Build:</strong> ✅ Successful
                                    <br>
                                    <strong>Node Types Loaded:</strong> <span v-if="nodeTypesCount" class="badge bg-primary">{{ nodeTypesCount }}</span>
                                    <br>
                                    <strong>Categories:</strong> <span v-if="categoriesCount" class="badge bg-success">{{ categoriesCount }}</span>
                                </div>
                            </div>
                        </div>

                        <!-- Node Types List -->
                        <hr>
                        <h6>Available Node Types</h6>
                        <div class="row">
                            <div v-for="(nodeType, key) in nodeTypesList" :key="key" class="col-md-3 mb-3">
                                <div class="card card-sm" :style="{ borderLeftColor: nodeType.color, borderLeftWidth: '3px' }">
                                    <div class="card-body p-2">
                                        <div class="d-flex align-items-center gap-2 mb-1">
                                            <i :class="nodeType.icon + ' text-muted'"></i>
                                            <small class="fw-bold">{{ nodeType.label }}</small>
                                        </div>
                                        <small class="text-muted d-block">{{ nodeType.category }}</small>
                                        <small class="text-muted d-block">{{ nodeType.shape }}</small>
                                        <span class="badge bg-light text-dark mt-1">{{ nodeType.id }}</span>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- Categories Info -->
                        <hr>
                        <h6>Categories Structure</h6>
                        <div class="table-responsive">
                            <table class="table table-sm table-hover">
                                <thead class="table-light">
                                    <tr>
                                        <th>Category</th>
                                        <th>Node Count</th>
                                        <th>Nodes</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="(category, key) in categoriesList" :key="key">
                                        <td>
                                            <i :class="category.icon + ' me-2'"></i>
                                            {{ category.name }}
                                        </td>
                                        <td>
                                            <span class="badge bg-primary">{{ category.nodes.length }}</span>
                                        </td>
                                        <td>
                                            <small>{{ category.nodes.join(', ') }}</small>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>

                        <!-- Workflow Builder Test -->
                        <hr>
                        <h6>Workflow Builder Test</h6>
                        <div class="alert alert-light">
                            <p><strong>Test Workflow:</strong></p>
                            <pre class="mb-0">{{ testWorkflowData }}</pre>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    export default {
        data() {
            return {
                nodeTypesList: {},
                categoriesList: {},
                nodeTypesCount: 0,
                categoriesCount: 0,
                testWorkflowData: {}
            };
        },
        methods: {
            openWorkflowEditor() {
                openComponent('/AppEndStudio/workflows/WorkflowEditor.vue', {
                    title: 'Workflow Designer - Test',
                    modalSize: 'modal-fullscreen',
                    windowSizeSwitchable: true,
                    modal: true,
                    params: {
                        workflow: null
                    },
                    callback: (result) => {
                        if (result?.success) {
                            showSuccess('Workflow saved successfully!');
                            console.log('Workflow:', result.workflow);
                        }
                    }
                });
            },

            testNodeTypes() {
                if (window.NodeTypes) {
                    this.nodeTypesList = window.NodeTypes;
                    this.nodeTypesCount = Object.keys(window.NodeTypes).length;
                    showSuccess(`Loaded ${this.nodeTypesCount} node types`);
                } else {
                    showError('NodeTypes not loaded. Make sure to load nodeTypes.js');
                }
            },

            showNodeCategories() {
                if (window.NodeCategories) {
                    this.categoriesList = window.NodeCategories;
                    this.categoriesCount = Object.keys(window.NodeCategories).length;
                    showSuccess(`Loaded ${this.categoriesCount} categories`);
                    console.log('Categories:', window.NodeCategories);
                } else {
                    showError('NodeCategories not loaded');
                }
            },

            testValidation() {
                const script = document.createElement('script');
                script.src = '/AppEndStudio/workflows/lib/workflowBuilder.js';
                script.onload = () => {
                    const builder = new window.WorkflowBuilder();
                    
                    // Test validation
                    const validation = builder.validateWorkflow();
                    console.log('Validation result:', validation);
                    
                    if (!validation.valid) {
                        showWarning('Empty workflow validation:\n' + validation.errors.join('\n'));
                    }

                    // Add some nodes
                    const startNode = builder.addNode(window.NodeTypes.START, { x: 50, y: 50 });
                    const taskNode = builder.addNode(window.NodeTypes.TASK, { x: 150, y: 50 });
                    const endNode = builder.addNode(window.NodeTypes.END, { x: 250, y: 50 });

                    // Add connections
                    builder.addConnection(startNode.id, taskNode.id);
                    builder.addConnection(taskNode.id, endNode.id);

                    // Validate again
                    const validation2 = builder.validateWorkflow();
                    console.log('Validation result after adding nodes:', validation2);

                    this.testWorkflowData = JSON.stringify(builder.getWorkflow(), null, 2);

                    if (validation2.valid) {
                        showSuccess('Test workflow is valid!');
                    } else {
                        showWarning('Test workflow has errors:\n' + validation2.errors.join('\n'));
                    }
                };
                document.head.appendChild(script);
            }
        },
        mounted() {
            // Auto-load test data
            this.testNodeTypes();
            this.showNodeCategories();
        }
    };
</script>

<style scoped>
    .card-sm {
        transition: all 0.2s ease;
    }

    .card-sm:hover {
        transform: translateY(-2px);
        box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    }
</style>
