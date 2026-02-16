<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <!-- Header with filters and actions -->
        <div class="card-header p-2 bg-body-subtle rounded-0 border-0">
            <div class="hstack">
                <input type="text" class="form-control form-control-sm" style="max-width:250px;" 
                    @keyup.enter="loadWorkflows" v-model='filter.search' placeholder="Search workflow names" />
                <div class="vr"></div>
                <select class="form-select form-select-sm" style="max-width:180px;" v-model="filter.status" @change="loadWorkflows">
                    <option value="">All Workflows</option>
                    <option value="published">Published Only</option>
                    <option value="draft">Draft Only</option>
                </select>
                <div class="vr"></div>
                <button class="btn btn-sm btn-outline-primary" @click="loadWorkflows" title="Refresh">
                    <i class="fa-solid fa-search"></i>
                </button>

                <div class="p-0 ms-auto"></div>

                <div class="vr"></div>
                <button class="btn btn-sm btn-primary" @click="createNewWorkflow" title="Create New Workflow">
                    <i class="fa-solid fa-plus"></i>
                </button>
            </div>
        </div>

        <!-- Body with table -->
        <div class="card-body rounded rounded-2 border border-3 border-light fs-d8 p-0 bg-transparent scrollable">
            <table class="table table-sm table-hover w-100 ae-table m-0 bg-transparent">
                <thead>
                    <tr>
                        <th class="sticky-top ae-thead-th text-dark fw-bold" style="width:250px;vertical-align:middle">Workflow ID</th>
                        <th class="sticky-top ae-thead-th text-dark fw-bold" style="width:200px;vertical-align:middle">Name</th>
                        <th class="sticky-top ae-thead-th text-dark fw-bold" style="vertical-align:middle">Description</th>
                        <th class="sticky-top ae-thead-th text-dark fw-bold text-center" style="width:80px;vertical-align:middle">Version</th>
                        <th class="sticky-top ae-thead-th text-dark fw-bold text-center" style="width:100px;vertical-align:middle">Status</th>
                        <th class="sticky-top ae-thead-th text-dark fw-bold text-center" style="width:350px;vertical-align:middle">Actions</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-if="filteredWorkflows.length === 0">
                        <td colspan="6" class="text-center text-muted py-4">
                            <i class="fas fa-info-circle me-2"></i>No workflows found
                        </td>
                    </tr>
                    <tr v-for="workflow in filteredWorkflows" :key="workflow.Id">
                        <td style="width:250px;vertical-align:middle">
                            <a href="#" @click.prevent="editWorkflow(workflow)" 
                                class="p-1 text-secondary text-hover-primary text-decoration-none" 
                                :data-ae-key="workflow.Id">
                                <i class="fa-solid fa-fw fa-edit me-1"></i>
                                <span class="fw-bold font-monospace">{{workflow.Id}}</span>
                            </a>
                        </td>
                        <td style="width:200px;vertical-align:middle">
                            <span class="fw-bold">{{workflow.Name}}</span>
                        </td>
                        <td style="vertical-align:middle">
                            <span class="text-muted fs-d8">{{workflow.Description || '-'}}</span>
                        </td>
                        <td style="width:80px;vertical-align:middle;text-align:center">
                            <span class="badge bg-secondary">v{{workflow.Version}}</span>
                        </td>
                        <td style="width:100px;vertical-align:middle;text-align:center">
                            <span v-if="workflow.IsPublished" class="badge bg-success">Published</span>
                            <span v-else class="badge bg-warning text-dark">Draft</span>
                        </td>
                        <td style="width:350px;vertical-align:middle;text-align:center;white-space:nowrap;">
                            <button class="btn btn-sm btn-outline-primary" @click="editWorkflow(workflow)" title="Edit">
                                <i class="fa-solid fa-fw fa-edit"></i> Edit
                            </button>
                            <button class="btn btn-sm btn-outline-info" @click="viewDetails(workflow)" title="View Details">
                                <i class="fa-solid fa-fw fa-eye"></i> View
                            </button>
                            <button v-if="!workflow.IsPublished" 
                                class="btn btn-sm btn-outline-success" 
                                @click="publishWorkflow(workflow)" 
                                title="Publish">
                                <i class="fa-solid fa-fw fa-check"></i> Publish
                            </button>
                            <button v-else 
                                class="btn btn-sm btn-outline-warning" 
                                @click="unpublishWorkflow(workflow)" 
                                title="Unpublish">
                                <i class="fa-solid fa-fw fa-ban"></i> Unpublish
                            </button>
                            <button class="btn btn-sm btn-outline-danger" 
                                @click="deleteWorkflow(workflow)" 
                                title="Delete">
                                <i class="fa-solid fa-fw fa-trash"></i> Delete
                            </button>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>

<script>
    shared.setAppTitle("$auto$");
    shared.setAppSubTitle("Workflow Management");

    let _this = { cid: "", c: null, workflows: [], filter: {} };
    _this.filter = { search: "", status: "" };

    export default {
        methods: {
            loadWorkflows() {
                rpcAEP('GetWorkflowDefinitions', {}, (data) => {
                    const payload = Array.isArray(data) ? (data[0] || {}) : (data || {});
                    const list = payload.Result || payload.result || payload.workflows || payload.Workflows || payload;
                    _this.c.workflows = Array.isArray(list) ? list : [];
                }, (error) => {
                    console.error('Error loading workflows:', error);
                });
            },

            createNewWorkflow() {
                openComponent("/AppEndStudio/workflows/WorkflowDefinitionEditor", {
                    title: "Create New Workflow",
                    modalSize: "modal-fullscreen",
                    params: {},
                    caller: this,
                    callback: function(result) {
                        if (result?.success) {
                            showSuccess('Workflow created successfully');
                            this.loadWorkflows();
                        }
                    }
                });
            },

            editWorkflow(workflow) {
                openComponent("/AppEndStudio/workflows/WorkflowDefinitionEditor", {
                    title: "Edit Workflow - " + workflow.Name,
                    modalSize: "modal-fullscreen",
                    params: {
                        workflow: workflow
                    },
                    caller: this,
                    callback: function(result) {
                        if (result?.success) {
                            showSuccess('Workflow updated successfully');
                            this.loadWorkflows();
                        }
                    }
                });
            },

            viewDetails(workflow) {
                openComponent("/a.SharedComponents/BaseContent", {
                    title: workflow.Name,
                    windowSizeSwitchable: true,
                    modalSize: "modal-lg",
                    params: {
                        content: {
                            Title: workflow.Name,
                            ContentBody: `
                                <div class="p-3">
                                    <div class="row mb-3">
                                        <div class="col-md-6">
                                            <strong>ID:</strong> ${workflow.Id}
                                        </div>
                                        <div class="col-md-6">
                                            <strong>Version:</strong> v${workflow.Version}
                                        </div>
                                    </div>
                                    <div class="row mb-3">
                                        <div class="col-12">
                                            <strong>Description:</strong>
                                            <p class="text-muted mt-1">${workflow.Description || 'N/A'}</p>
                                        </div>
                                    </div>
                                    <div class="row mb-3">
                                        <div class="col-md-6">
                                            <strong>Status:</strong>
                                            <span class="badge ${workflow.IsPublished ? 'bg-success' : 'bg-warning text-dark'} ms-2">
                                                ${workflow.IsPublished ? 'Published' : 'Draft'}
                                            </span>
                                        </div>
                                        <div class="col-md-6">
                                            <strong>Loaded At:</strong> ${this.formatDate(workflow.LoadedAt)}
                                        </div>
                                    </div>
                                    <hr>
                                    <div class="row">
                                        <div class="col-12">
                                            <strong>Full JSON Definition:</strong>
                                            <pre class="bg-light p-2 rounded mt-2" style="max-height: 400px; overflow-y: auto; font-size: 0.85rem;">${JSON.stringify(workflow, null, 2)}</pre>
                                        </div>
                                    </div>
                                </div>
                            `
                        }
                    }
                });
            },

            publishWorkflow(workflow) {
                shared.showConfirm({
                    title: "Publish Workflow",
                    message1: "Are you sure you want to publish this workflow?",
                    message2: workflow.Name,
                    callback: function() {
                        rpcAEP('PublishWorkflow', { WorkflowId: workflow.Id }, (data) => {
                            const payload = Array.isArray(data) ? (data[0] || {}) : (data || {});
                            const success = payload.Success || payload.success;
                            if (success) {
                                showSuccess('Workflow published');
                                _this.c.loadWorkflows();
                            } else {
                                showError('Error: ' + (payload.ErrorMessage || 'Unknown error'));
                            }
                        }, (error) => {
                            showError('Error: ' + error);
                        });
                    }
                });
            },

            unpublishWorkflow(workflow) {
                shared.showConfirm({
                    title: "Unpublish Workflow",
                    message1: "Are you sure you want to unpublish this workflow?",
                    message2: workflow.Name,
                    callback: function() {
                        rpcAEP('UnpublishWorkflow', { WorkflowId: workflow.Id }, (data) => {
                            const payload = Array.isArray(data) ? (data[0] || {}) : (data || {});
                            const success = payload.Success || payload.success;
                            if (success) {
                                showSuccess('Workflow unpublished');
                                _this.c.loadWorkflows();
                            } else {
                                showError('Error: ' + (payload.ErrorMessage || 'Unknown error'));
                            }
                        }, (error) => {
                            showError('Error: ' + error);
                        });
                    }
                });
            },

            deleteWorkflow(workflow) {
                shared.showConfirm({
                    title: "Delete Workflow",
                    message1: "Are you sure you want to delete this workflow? This cannot be undone.",
                    message2: workflow.Name,
                    callback: function() {
                        rpcAEP('DeleteWorkflow', { WorkflowId: workflow.Id }, (data) => {
                            const payload = Array.isArray(data) ? (data[0] || {}) : (data || {});
                            const success = payload.Success || payload.success;
                            if (success) {
                                showSuccess('Workflow deleted');
                                _this.c.loadWorkflows();
                            } else {
                                showError('Error: ' + (payload.ErrorMessage || 'Unknown error'));
                            }
                        }, (error) => {
                            showError('Error: ' + error);
                        });
                    }
                });
            },

            formatDate(dateStr) {
                if (!dateStr) return '-';
                return new Date(dateStr).toLocaleString();
            }
        },
        computed: {
            filteredWorkflows() {
                let list = this.workflows || [];
                
                if (shared.fixNull(this.filter.search, '') !== '') {
                    const search = this.filter.search.toLowerCase();
                    list = _.filter(list, (w) => 
                        w.Id.toLowerCase().includes(search) || 
                        w.Name.toLowerCase().includes(search) ||
                        (w.Description && w.Description.toLowerCase().includes(search))
                    );
                }
                
                if (this.filter.status === 'published') {
                    list = _.filter(list, (w) => w.IsPublished === true);
                } else if (this.filter.status === 'draft') {
                    list = _.filter(list, (w) => w.IsPublished !== true);
                }
                
                return list;
            }
        },
        setup(props) {
            _this.cid = props['cid'];
        },
        data() {
            return {
                workflows: _this.workflows,
                filter: _this.filter
            };
        },
        created() { _this.c = this; },
        mounted() {
            this.loadWorkflows();
        },
        props: { cid: String }
    }
</script>
