<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">


        <!-- Body with Form -->
        <div class="card-body bg-primary-subtle-light scrollable p-3">
            <div class="card rounded-1 border-light">
                <div class="card-body p-3">
                    
                    <div class="mb-3">
                        <label class="fs-d8 text-muted ms-2 d-block mb-1" for="input_id">Workflow ID <span class="text-danger">*</span></label>
                        <input v-model="workflowForm.id" type="text" class="form-control form-control-sm w-100" 
                            id="input_id" placeholder="hello-world" pattern="^[a-z0-9-]+$" required :disabled="isEditMode">
                        <small class="text-muted d-block ms-2" style="font-size: 0.65rem;">lowercase, numbers, hyphens</small>
                    </div>

                    <div class="mb-3">
                        <label class="fs-d8 text-muted ms-2 d-block mb-1" for="input_name">Name <span class="text-danger">*</span></label>
                        <input v-model="workflowForm.name" type="text" class="form-control form-control-sm w-100" 
                            id="input_name" placeholder="Workflow Name" required>
                    </div>

                    <div class="mb-3">
                        <label class="fs-d8 text-muted ms-2 d-block mb-1" for="input_description">Description</label>
                        <textarea v-model="workflowForm.description" class="form-control form-control-sm w-100" 
                            id="input_description" rows="3" placeholder="Workflow description..."></textarea>
                    </div>

                    <div class="mb-0">
                        <label class="fs-d8 text-muted ms-2 d-block mb-1" for="input_version">Version</label>
                        <input v-model.number="workflowForm.version" type="number" 
                            class="form-control form-control-sm w-100" id="input_version" min="1">
                    </div>

                </div>
            </div>
        </div>

        <!-- Footer with Action Buttons -->
        <div class="card-footer p-0">
            <div class="container-fluid pt-2 pb-1">
                <div class="row p-0">
                    <div class="col-36 px-2">
                        <button class="btn btn-sm btn-primary w-100" @click="ok" data-ae-key="ok">
                            <i class="fa-solid fa-check me-1"></i>
                            <span>Save Workflow</span>
                        </button>
                    </div>
                    <div class="col-12 px-2">
                        <button class="btn btn-sm btn-secondary w-100" @click="cancel">
                            <i class="fa-solid fa-xmark me-1"></i>
                            <span>Cancel</span>
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    shared.setAppTitle("$auto$");
    shared.setAppSubTitle("Workflow Editor");

    let _this = { cid: "", c: null, inputs: {}, workflowForm: { id: '', name: '', description: '', version: 1 } };

    export default {
        methods: {
            ok() {
                if (!this.workflowForm.id || !this.workflowForm.name) {
                    showError('ID and Name are required');
                    return;
                }

                const rpcMethod = this.isEditMode ? 'UpdateWorkflow' : 'CreateWorkflow';
                const params = this.isEditMode 
                    ? {
                        WorkflowId: this.workflowForm.id,
                        Name: this.workflowForm.name,
                        Description: this.workflowForm.description,
                        Version: this.workflowForm.version
                    }
                    : {
                        WorkflowId: this.workflowForm.id,
                        Name: this.workflowForm.name,
                        Description: this.workflowForm.description,
                        Version: this.workflowForm.version,
                        Definition: {
                            activities: [],
                            variables: []
                        }
                    };

                console.log('🔍 Calling RPC:', rpcMethod, 'with params:', JSON.stringify(params, null, 2));

                rpcAEP(rpcMethod, params, (data) => {
                    console.log('📥 RPC Full Response:', data);
                    console.log('📥 RPC Response JSON:', JSON.stringify(data, null, 2));
                    
                    let payload = data;
                    
                    // Handle array wrapped response
                    if (Array.isArray(data)) {
                        console.log('Response is array, extracting first element');
                        payload = data[0] || {};
                    }
                    
                    console.log('📦 Final Payload:', JSON.stringify(payload, null, 2));
                    console.log('📦 Success field value:', payload?.Success);
                    console.log('📦 All fields:', Object.keys(payload || {}));
                    
                    // Check for success - WorkflowServices returns Success (uppercase)
                    const success = payload?.Success === true;
                    
                    if (success) {
                        const message = _this.c.isEditMode ? 'Workflow updated successfully' : 'Workflow created successfully';
                        console.log('✅ Success! Message:', message);
                        showSuccess(message);
                        shared.closeComponent(_this.cid, { success: true });
                    } else {
                        const errorMsg = payload?.ErrorMessage || payload?.Message || payload?.message || 'Check console for details';
                        console.error('❌ Error Message:', errorMsg);
                        console.error('❌ Full payload:', payload);
                        showError('Error: ' + errorMsg);
                    }
                }, (error) => {
                    console.error('❌ RPC Error:', error);
                    console.error('❌ RPC Error JSON:', JSON.stringify(error, null, 2));
                    showError('RPC Error: ' + (error?.message || error || 'Request failed'));
                });
            },

            cancel() {
                shared.closeComponent(_this.cid, { cancelled: true });
            }
        },
        computed: {
            isEditMode() {
                return !!this.inputs?.workflow;
            }
        },
        setup(props) {
            _this.cid = props["cid"];
            _this.inputs = shared["params_" + _this.cid];
        },
        data() {
            return _this;
        },
        created() { _this.c = this; },
        mounted() {
            if (this.inputs?.workflow) {
                const w = this.inputs.workflow;
                this.workflowForm = {
                    id: w.Id,
                    name: w.Name,
                    description: w.Description,
                    version: w.Version
                };
            }
        },
        props: { cid: String }
    }
</script>
