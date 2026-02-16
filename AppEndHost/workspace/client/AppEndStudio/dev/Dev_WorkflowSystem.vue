<template>
    <div class="card h-100 bg-transparent rounded-0 border-0 dev-guide">
        <div class="card-header p-2 bg-body-subtle rounded-0 border-0">
            <div class="hstack gap-2">
                <div class="fw-bold text-success">
                    <i class="fa-solid fa-diagram-project me-1"></i>
                    <span>Workflow System</span>
                </div>
                <div class="ms-auto text-muted fs-d8">Dev Guide</div>
            </div>
        </div>

        <div class="card-body p-2">
            <div class="container-fluid h-100">
                <div class="row h-100">
                    <!-- Main Content -->
                    <div class="col-48 col-lg-36 order-2 order-lg-1 scrollable h-100">
                        <div class="dev-guide-content p-2" tabindex="0">
                            <!-- Overview Section -->
                            <section class="dev-guide-section mb-5" id="section-status">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-info-circle me-1 text-success"></i>
                                    <span class="fw-bold">Workflow Engine Status</span>
                                </div>
                                <p class="mb-2">Elsa 3.5.3 Workflow Engine Integration - Phase 4 RPC Testing</p>
                                <div class="alert alert-info mb-0">
                                    <strong>Architecture:</strong> JSON-based workflow definitions stored in <code>workspace/workflows/</code>, loaded into cache on startup, zero-downtime reload via RPC
                                </div>
                            </section>

                            <!-- RPC Endpoint Reference -->
                            <section class="dev-guide-section mb-5" id="section-rpc">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-book me-1 text-primary"></i>
                                    <span class="fw-bold">RPC Endpoint Reference</span>
                                </div>
                                
                                <div class="mt-3">
                                    <h6 class="fw-bold text-success mb-3">Available Methods:</h6>
                                    <div class="row g-3">
                                        <div class="col-md-6">
                                            <div class="card border-start border-5 border-primary h-100">
                                                <div class="card-body">
                                                    <h6 class="card-title">GetWorkflowDefinitions()</h6>
                                                    <p class="card-text text-muted small">Returns list of all loaded workflows with metadata</p>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-md-6">
                                            <div class="card border-start border-5 border-primary h-100">
                                                <div class="card-body">
                                                    <h6 class="card-title">GetWorkflowDefinition(id)</h6>
                                                    <p class="card-text text-muted small">Retrieves specific workflow definition with full JSON</p>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-md-6">
                                            <div class="card border-start border-5 border-success h-100">
                                                <div class="card-body">
                                                    <h6 class="card-title">ExecuteWorkflow(id, params)</h6>
                                                    <p class="card-text text-muted small">Executes a workflow with optional input parameters</p>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-md-6">
                                            <div class="card border-start border-5 border-warning h-100">
                                                <div class="card-body">
                                                    <h6 class="card-title">ReloadWorkflow(id)</h6>
                                                    <p class="card-text text-muted small">Reloads single workflow from disk (zero downtime)</p>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-md-6">
                                            <div class="card border-start border-5 border-warning h-100">
                                                <div class="card-body">
                                                    <h6 class="card-title">ReloadAllWorkflows()</h6>
                                                    <p class="card-text text-muted small">Reloads all workflows from disk (atomic cache swap)</p>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-md-6">
                                            <div class="card border-start border-5 border-info h-100">
                                                <div class="card-body">
                                                    <h6 class="card-title">GetWorkflowStats()</h6>
                                                    <p class="card-text text-muted small">Returns workflow statistics (count, published, unpublished)</p>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </section>

                            <!-- Sample Workflows -->
                            <section class="dev-guide-section mb-5" id="section-workflows">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-flask me-1 text-primary"></i>
                                    <span class="fw-bold">Sample Workflows</span>
                                </div>
                                <div class="row g-3 mt-2">
                                    <div class="col-md-6">
                                        <div class="card">
                                            <div class="card-header bg-light">
                                                <strong>hello-world</strong>
                                            </div>
                                            <div class="card-body">
                                                <p class="text-muted small mb-0">Basic workflow demo - Simple logging activity</p>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="card">
                                            <div class="card-header bg-light">
                                                <strong>scheduled-db-check</strong>
                                            </div>
                                            <div class="card-body">
                                                <p class="text-muted small mb-0">Timer-based workflow - Health check on schedule</p>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                    <div class="card">
                                        <div class="card-header bg-light">
                                            <strong>order-approval</strong>
                                        </div>
                                        <div class="card-body">
                                            <p class="text-muted small mb-0">Multi-step workflow - Order approval with notifications</p>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <div class="card">
                                        <div class="card-header bg-light">
                                            <strong>data-pipeline</strong>
                                        </div>
                                        <div class="card-body">
                                            <p class="text-muted small mb-0">ETL-like workflow - Extract, transform, validate, load</p>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </section>

                        <!-- File Structure -->
                        <section class="dev-guide-section mb-5" id="section-files">
                            <div class="dev-guide-title">
                                <i class="fa-solid fa-folder-tree me-1 text-primary"></i>
                                <span class="fw-bold">File Structure</span>
                            </div>
                            <div class="mt-3">
                                <pre class="bg-light p-3 rounded border">workspace/workflows/
├── schema.json                  (JSON Schema for validation)
├── hello-world.json
├── scheduled-db-check.json
├── order-approval.json
└── data-pipeline.json</pre>
                            </div>
                        </section>

                        <!-- Database Schema -->
                        <section class="dev-guide-section mb-5" id="section-database">
                            <div class="dev-guide-title">
                                <i class="fa-solid fa-database me-1 text-primary"></i>
                                <span class="fw-bold">Database Tables</span>
                            </div>
                            <p class="text-muted small mb-3">The following 8 tables are created in [dbo] schema with Elsa prefix:</p>
                            <div class="row g-2">
                                <div class="col-md-8">
                                    <div class="list-group list-group-sm">
                                        <div class="list-group-item">ElsaWorkflowDefinitions</div>
                                        <div class="list-group-item">ElsaWorkflowInstances</div>
                                        <div class="list-group-item">ElsaActivityInstances</div>
                                        <div class="list-group-item">ElsaBookmarks</div>
                                    </div>
                                </div>
                                <div class="col-md-10">
                                    <div class="list-group list-group-sm">
                                        <div class="list-group-item">ElsaWorkflowExecutionLogRecords</div>
                                        <div class="list-group-item">ElsaLabels</div>
                                        <div class="list-group-item">ElsaWorkflowDefinitionLabels</div>
                                        <div class="list-group-item">ElsaStoredBookmarks</div>
                                    </div>
                                </div>
                            </div>
                            <p class="text-muted small mt-3 mb-0">
                                <i class="fa-solid fa-exclamation-triangle"></i>
                                Run <code>AppEndWorkflow/ElsaSchema.sql</code> on your SQL Server database to create these tables.
                            </p>
                        </section>

                        <!-- Notes -->
                        <section class="dev-guide-section">
                            <div class="alert alert-warning">
                                <i class="fa-solid fa-lightbulb me-1"></i>
                                <strong>Note:</strong> ExecuteWorkflow is currently a placeholder implementation. 
                                Full Elsa SDK integration is planned for Phase 8. See WorkflowServices.cs for current implementation.
                            </div>
                        </section>

                        <!-- Interactive Demo -->
                        <section class="dev-guide-section mb-5" id="section-demo">
                            <div class="dev-guide-title">
                                <i class="fa-solid fa-flask-vial me-1 text-success"></i>
                                <span class="fw-bold">Interactive Demo & RPC Testing</span>
                            </div>
                            <p class="mb-3">Test the RPC endpoints directly in the browser. Click any button to execute a method and see the results below.</p>
                            
                            <div class="demo-section">
                                <div class="demo-buttons">
                                    <div class="button-group">
                                        <button class="btn btn-sm btn-primary" @click="testGetDefinitions" :disabled="testing" title="Get all loaded workflows">
                                            <i class="fas fa-list me-1"></i>List All Workflows
                                        </button>
                                        <button class="btn btn-sm btn-info" @click="testGetStats" :disabled="testing" title="Get workflow statistics">
                                            <i class="fas fa-chart-bar me-1"></i>Get Stats
                                        </button>
                                        <button class="btn btn-sm btn-outline-danger" @click="clearResults" title="Clear results display">
                                            <i class="fas fa-trash me-1"></i>Clear Results
                                        </button>
                                    </div>

                                    <div class="button-group">
                                        <input 
                                            type="text" 
                                            class="form-control form-control-sm" 
                                            v-model="testWorkflowId" 
                                            placeholder="Workflow ID (e.g., hello-world)"
                                            title="Enter a workflow ID to test"
                                        >
                                        <button class="btn btn-sm btn-outline-primary" @click="testGetDefinition" :disabled="testing" title="Retrieve specific workflow definition">
                                            <i class="fas fa-search me-1"></i>Get Definition
                                        </button>
                                    </div>

                                    <div class="button-group">
                                        <button class="btn btn-sm btn-success" @click="testExecuteWorkflow" :disabled="testing" title="Execute a workflow instance">
                                            <i class="fas fa-play-circle me-1"></i>Execute Workflow
                                        </button>
                                        <button class="btn btn-sm btn-warning" @click="testReloadWorkflow" :disabled="testing" title="Reload single workflow from disk">
                                            <i class="fas fa-sync me-1"></i>Reload One
                                        </button>
                                        <button class="btn btn-sm btn-warning" @click="testReloadAll" :disabled="testing" title="Reload all workflows (zero downtime)">
                                            <i class="fas fa-sync-alt me-1"></i>Reload All
                                        </button>
                                    </div>
                                </div>

                                <!-- Results Display -->
                                <div v-if="results" class="demo-results mt-3">
                                    <div class="result-header">
                                        <div>
                                            <strong>{{ lastTestName }}</strong>
                                            <span class="result-time">@ {{ formatTime(results.timestamp) }}</span>
                                        </div>
                                    </div>

                                    <div v-if="results.success" class="alert alert-success mb-2">
                                        ✅ {{ results.message || 'Success' }}
                                    </div>
                                    <div v-else class="alert alert-danger mb-2">
                                        ❌ {{ results.errorMessage || 'Error' }}
                                    </div>

                                    <div class="result-data">
                                        <pre class="bg-light p-2 rounded border" style="max-height: 700px; overflow-y: auto; font-size: 0.8rem;">{{ JSON.stringify(results, null, 2) }}</pre>
                                    </div>
                                </div>

                                <div v-else class="alert alert-info mt-3">
                                    ℹ️ Click a button above to test RPC endpoints and see results here
                                </div>
                            </div>
                        </section>
                    </div>
                </div>

                <!-- Table of Contents -->
                <div class="col-48 col-lg-12 order-1 order-lg-2 mb-2 mb-lg-0">
                    <nav id="devGuideNavWorkflow" class="dev-guide-nav nav flex-column p-2">
                        <div class="fw-bold text-uppercase text-muted fs-d9 mb-2">On this page</div>
                        <a class="nav-link dev-guide-link" href="#section-status" @click.prevent="scrollTo('section-status')">Engine Status</a>
                        <a class="nav-link dev-guide-link" href="#section-rpc" @click.prevent="scrollTo('section-rpc')">RPC Endpoints</a>
                        <a class="nav-link dev-guide-link" href="#section-workflows" @click.prevent="scrollTo('section-workflows')">Sample Workflows</a>
                        <a class="nav-link dev-guide-link" href="#section-files" @click.prevent="scrollTo('section-files')">File Structure</a>
                        <a class="nav-link dev-guide-link" href="#section-database" @click.prevent="scrollTo('section-database')">Database Tables</a>
                        <a class="nav-link dev-guide-link" href="#section-progress" @click.prevent="scrollTo('section-progress')">Progress</a>
                        <a class="nav-link dev-guide-link" href="#section-demo" @click.prevent="scrollTo('section-demo')">Live Demo</a>
                    </nav>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    shared.setAppTitle("$auto$");
    shared.setAppSubTitle("Dev Guide");

    let _this = { cid: "", c: null };

    export default {
        setup(props) {
            _this.cid = props["cid"];
        },
        data() { 
            return {
                ..._this,
                testing: false,
                testWorkflowId: 'hello-world',
                results: null,
                lastTestName: ''
            }; 
        },
        created() { _this.c = this; },
        mounted() {
            let scrollEl = this.$el.querySelector(".dev-guide-content");
            if (window.bootstrap?.ScrollSpy) {
                new window.bootstrap.ScrollSpy(scrollEl, {
                    target: "#devGuideNavWorkflow",
                    rootMargin: "0px 0px -60%"
                });
            }
            initDevCodeBlocks(this.$el);
        },
        methods: {
            scrollTo(id) {
                let el = this.$el.querySelector('#' + id);
                if (el) el.scrollIntoView({ behavior: 'smooth', block: 'start' });
            },

            formatTime(timestamp) {
                if (!timestamp) return '';
                return new Date(timestamp).toLocaleTimeString();
            },

            normalizeRpcResult(data) {
                const payload = Array.isArray(data) ? (data[0] || {}) : (data || {});
                const result = payload.Result || payload.result || {};
                const rawSuccess = result.Success ?? result.success ?? payload.Success ?? payload.success ?? payload.IsSucceeded ?? payload.isSucceeded ?? payload.IsSuccess ?? payload.isSuccess;
                const success = rawSuccess === true || rawSuccess === 'true' || (rawSuccess === undefined && (result || payload) && !(result.ErrorMessage || result.errorMessage || payload.ErrorMessage || payload.errorMessage));
                const errorMessage = result.ErrorMessage ?? result.errorMessage ?? payload.ErrorMessage ?? payload.errorMessage;
                const message = result.Output?.message ?? result.output?.message ?? result.message ?? result.Message ?? payload.Message ?? payload.message;
                return { 
                    success,
                    message: message || (success ? 'Success' : undefined),
                    errorMessage: success ? undefined : errorMessage,
                    data,
                    timestamp: new Date().toISOString()
                };
            },

            async testGetDefinitions() {
                this.testing = true;
                this.lastTestName = 'Get All Workflows';
                try {
                    await new Promise((resolve, reject) => {
                        rpcAEP('GetWorkflowDefinitions', {}, (data) => {
                            this.results = { 
                                success: true, 
                                message: `Retrieved ${data.length || 0} workflows`,
                                workflows: data,
                                timestamp: new Date().toISOString()
                            };
                            this.testing = false;
                            resolve();
                        }, (error) => {
                            console.error('RPC Error:', error);
                            this.results = { 
                                success: false, 
                                errorMessage: error, 
                                timestamp: new Date().toISOString() 
                            };
                            this.testing = false;
                            reject();
                        });
                    });
                } catch (error) {
                    console.error('Test Error:', error);
                    this.testing = false;
                }
            },

            async testGetDefinition() {
                if (!this.testWorkflowId) {
                    this.results = { 
                        success: false, 
                        errorMessage: 'Please enter a Workflow ID',
                        timestamp: new Date().toISOString()
                    };
                    return;
                }
                this.testing = true;
                this.lastTestName = `Get Workflow: ${this.testWorkflowId}`;
                try {
                    await new Promise((resolve, reject) => {
                        rpcAEP('GetWorkflowDefinition', { WorkflowId: this.testWorkflowId }, (data) => {
                            this.results = { 
                                success: data !== null, 
                                message: data ? `Retrieved workflow: ${data.Id}` : 'Workflow not found',
                                workflow: data,
                                timestamp: new Date().toISOString()
                            };
                            this.testing = false;
                            resolve();
                        }, (error) => {
                            console.error('RPC Error:', error);
                            this.results = { 
                                success: false, 
                                errorMessage: error, 
                                timestamp: new Date().toISOString() 
                            };
                            this.testing = false;
                            reject();
                        });
                    });
                } catch (error) {
                    console.error('Test Error:', error);
                    this.testing = false;
                }
            },

            async testExecuteWorkflow() {
                if (!this.testWorkflowId) {
                    this.results = { 
                        success: false, 
                        errorMessage: 'Please enter a Workflow ID',
                        timestamp: new Date().toISOString()
                    };
                    return;
                }
                this.testing = true;
                this.lastTestName = `Execute Workflow: ${this.testWorkflowId}`;
                try {
                    await new Promise((resolve, reject) => {
                        rpcAEP('ExecuteWorkflow', { 
                            WorkflowId: this.testWorkflowId,
                            InputParams: {}
                        }, (data) => {
                            this.results = this.normalizeRpcResult(data);
                            this.testing = false;
                            resolve();
                        }, (error) => {
                            console.error('RPC Error:', error);
                            this.results = { 
                                success: false, 
                                errorMessage: error, 
                                timestamp: new Date().toISOString() 
                            };
                            this.testing = false;
                            reject();
                        });
                    });
                } catch (error) {
                    console.error('Test Error:', error);
                    this.testing = false;
                }
            },

            async testReloadWorkflow() {
                if (!this.testWorkflowId) {
                    this.results = { 
                        success: false, 
                        errorMessage: 'Please enter a Workflow ID',
                        timestamp: new Date().toISOString()
                    };
                    return;
                }
                this.testing = true;
                this.lastTestName = `Reload Workflow: ${this.testWorkflowId}`;
                try {
                    await new Promise((resolve, reject) => {
                        rpcAEP('ReloadWorkflow', { WorkflowId: this.testWorkflowId }, (data) => {
                            this.results = this.normalizeRpcResult(data);
                            this.testing = false;
                            resolve();
                        }, (error) => {
                            console.error('RPC Error:', error);
                            this.results = { 
                                success: false, 
                                errorMessage: error, 
                                timestamp: new Date().toISOString() 
                            };
                            this.testing = false;
                            reject();
                        });
                    });
                } catch (error) {
                    console.error('Test Error:', error);
                    this.testing = false;
                }
            },

            async testReloadAll() {
                this.testing = true;
                this.lastTestName = 'Reload All Workflows';
                try {
                    await new Promise((resolve, reject) => {
                        rpcAEP('ReloadAllWorkflows', {}, (data) => {
                            this.results = this.normalizeRpcResult(data);
                            this.testing = false;
                            resolve();
                        }, (error) => {
                            console.error('RPC Error:', error);
                            this.results = { 
                                success: false, 
                                errorMessage: error, 
                                timestamp: new Date().toISOString() 
                            };
                            this.testing = false;
                            reject();
                        });
                    });
                } catch (error) {
                    console.error('Test Error:', error);
                    this.testing = false;
                }
            },

            async testGetStats() {
                this.testing = true;
                this.lastTestName = 'Get Workflow Stats';
                try {
                    await new Promise((resolve, reject) => {
                        rpcAEP('GetWorkflowStats', {}, (data) => {
                            this.results = { 
                                success: true, 
                                message: 'Stats retrieved',
                                stats: data,
                                timestamp: new Date().toISOString()
                            };
                            this.testing = false;
                            resolve();
                        }, (error) => {
                            console.error('RPC Error:', error);
                            this.results = { 
                                success: false, 
                                errorMessage: error, 
                                timestamp: new Date().toISOString() 
                            };
                            this.testing = false;
                            reject();
                        });
                    });
                } catch (error) {
                    console.error('Test Error:', error);
                    this.testing = false;
                }
            },

            clearResults() {
                this.results = null;
                this.lastTestName = '';
            }
        },
        props: { cid: String }
    }
</script>

<style scoped>
.button-group {
    display: flex;
    gap: 0.5rem;
    flex-wrap: nowrap;
    align-items: stretch;
    overflow-x: auto;
}

.button-group .btn {
    flex-shrink: 0;
    white-space: nowrap;
    font-size: 0.85rem;
    padding: 0.375rem 0.75rem;
}

.button-group .form-control {
    flex: 1;
    min-width: 120px;
    font-size: 0.85rem;
}

.demo-buttons {
    display: flex;
    flex-direction: column;
    gap: 0.75rem;
}

.demo-section {
    background: #f8f9fa;
    padding: 1rem;
    border-radius: 0.375rem;
    border: 1px solid #dee2e6;
}

.demo-results {
    background: white;
    padding: 1rem;
    border-radius: 0.375rem;
    border: 1px solid #e9ecef;
}

.result-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 0.75rem;
    padding-bottom: 0.75rem;
    border-bottom: 1px solid #e9ecef;
    font-size: 0.9rem;
}

.result-header div {
    display: flex;
    gap: 1rem;
    align-items: center;
}

.result-time {
    color: #6c757d;
    font-size: 0.85rem;
}

.result-data {
    background: #f8f9fa;
    border-radius: 0.25rem;
}

.result-data pre {
    margin: 0;
    font-size: 0.75rem;
    color: #495057;
    line-height: 1.4;
}

@media (max-width: 992px) {
    .button-group {
        flex-wrap: wrap;
    }
    
    .button-group .btn {
        flex: 1 1 auto;
        min-width: 90px;
    }
}

@media (max-width: 768px) {
    .button-group {
        flex-direction: column;
        align-items: stretch;
    }

    .button-group .btn,
    .button-group .form-control {
        width: 100%;
        font-size: 0.9rem;
        padding: 0.5rem 0.75rem;
    }
}
</style>
