<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <!-- Header Toolbar -->
        <div class="card-header p-2 bg-frame rounded-0 border-0">
            <div class="hstack gap-2">
                <button class="btn btn-sm btn-link text-decoration-none" @click="formatJson">
                    <i class="fa-solid fa-align-left"></i> <span>Format</span>
                </button>
                <button class="btn btn-sm btn-link text-decoration-none" @click="validateJson">
                    <i class="fa-solid fa-check-circle"></i> <span>Validate</span>
                </button>
                <div class="p-0 ms-auto"></div>
                <button class="btn btn-sm btn-link text-decoration-none text-secondary" @click="cancel">
                    <i class="fa-solid fa-times"></i> <span>Cancel</span>
                </button>
                <button class="btn btn-sm btn-link text-decoration-none text-primary" @click="saveWorkflow">
                    <i class="fa-solid fa-save"></i> <span>Save Workflow</span>
                </button>
            </div>
        </div>

        <!-- Body with Split Panels -->
        <div class="card-body p-0">
            <div class="h-100 w-100" data-flex-splitter-horizontal style="flex: auto;">
                
                <!-- Form Fields Panel -->
                <div class="h-100" style="min-width:250px;width:30%;">
                    <div class="card h-100 rounded-0 border-0">
                        <div class="card-body p-3 scrollable">
                            
                            <div class="row">
                                <label class="col-10 col-form-label fw-bold small">ID <span class="text-danger">*</span></label>
                                <div class="col-38">
                                    <input v-model="workflowForm.id" type="text" class="form-control form-control-sm" 
                                        placeholder="hello-world" pattern="^[a-z0-9-]+$" required :disabled="isEditMode">
                                    <small class="text-muted" style="font-size: 0.65rem;">lowercase, numbers, hyphens</small>
                                </div>
                            </div>

                            <div class="row">
                                <label class="col-10 col-form-label fw-bold small">Name <span class="text-danger">*</span></label>
                                <div class="col-38">
                                    <input v-model="workflowForm.name" type="text" class="form-control form-control-sm" 
                                        placeholder="Workflow Name" required>
                                </div>
                            </div>

                            <div class="row">
                                <label class="col-10 col-form-label fw-bold small">Description</label>
                                <div class="col-38">
                                    <textarea v-model="workflowForm.description" class="form-control form-control-sm" 
                                        rows="3" placeholder="Workflow description..."></textarea>
                                </div>
                            </div>

                            <div class="row">
                                <label class="col-10 col-form-label fw-bold small">Version</label>
                                <div class="col-38">
                                    <input v-model.number="workflowForm.version" type="number" 
                                        class="form-control form-control-sm" min="1">
                                </div>
                            </div>

                            <div class="row">
                                <label class="col-10 col-form-label fw-bold small">Published</label>
                                <div class="col-38">
                                    <div class="form-check form-switch mt-2">
                                        <input v-model="workflowForm.isPublished" type="checkbox" 
                                            class="form-check-input" id="publishCheck">
                                        <label class="form-check-label small" for="publishCheck">Yes</label>
                                    </div>
                                </div>
                            </div>

                        </div>
                    </div>
                </div>

                <!-- Splitter -->
                <div role="separator" tabindex="1" class="bg-light" 
                    style="width:.5%; min-width:.5%; cursor: col-resize; background: linear-gradient(90deg, transparent 0%, rgba(0,0,0,0.02) 45%, rgba(0,0,0,0.06) 50%, rgba(0,0,0,0.02) 55%, transparent 100%);"></div>

                <!-- JSON Editor Panel -->
                <div class="h-100" style="min-width:400px;width:69.5%;">
                    <div class="card h-100 rounded-0 border-0">
                        <div class="card-body p-0">
                            <div id="aceWorkflowEditor" class="h-100 w-100"></div>
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </div>
</template>

<script>
    shared.setAppTitle("$auto$");
    shared.setAppSubTitle("Workflow Editor");

    let _this = { cid: "", c: null };

    export default {
        setup(props) {
            _this.cid = props["cid"];
            _this.inputs = shared["params_" + _this.cid];
        },
        data() {
            return {
                ..._this,
                aceEditor: null,
                workflowForm: {
                    id: '',
                    name: '',
                    description: '',
                    version: 1,
                    isPublished: false
                }
            };
        },
        computed: {
            isEditMode() {
                return !!this.inputs?.workflow;
            }
        },
        created() { _this.c = this; },
        mounted() {
            if (this.inputs?.workflow) {
                const w = this.inputs.workflow;
                this.workflowForm = {
                    id: w.Id,
                    name: w.Name,
                    description: w.Description,
                    version: w.Version,
                    isPublished: w.IsPublished
                };
            }
            
            this.$nextTick(() => {
                this.initializeAceEditor();
            });
        },
        beforeUnmount() {
            if (this.aceEditor) {
                this.aceEditor.destroy();
                this.aceEditor = null;
            }
        },
        methods: {
            initializeAceEditor() {
                if (typeof ace === 'undefined') {
                    const aceScript = document.createElement('script');
                    aceScript.src = '/a..lib/ace/src-min/ace.js';
                    aceScript.onload = () => {
                        this.createAceEditor();
                    };
                    document.head.appendChild(aceScript);
                } else {
                    this.createAceEditor();
                }
            },

            createAceEditor() {
                const editorElement = document.getElementById('aceWorkflowEditor');
                if (!editorElement) return;

                this.aceEditor = ace.edit(editorElement);
                this.aceEditor.setTheme('ace/theme/monokai');
                this.aceEditor.session.setMode('ace/mode/json');
                this.aceEditor.session.setTabSize(2);
                this.aceEditor.session.setUseSoftTabs(true);
                this.aceEditor.setShowPrintMargin(false);
                this.aceEditor.setFontSize(13);
                
                if (this.inputs?.workflow) {
                    const w = this.inputs.workflow;
                    try {
                        this.aceEditor.setValue(JSON.stringify(w, null, 2));
                    } catch (e) {
                        this.aceEditor.setValue('{}');
                    }
                } else {
                    this.aceEditor.setValue(JSON.stringify({
                        "Id": "",
                        "Name": "",
                        "Description": "",
                        "Version": 1,
                        "IsPublished": false,
                        "Activities": []
                    }, null, 2));
                }

                this.aceEditor.clearSelection();
                this.aceEditor.gotoLine(1);
            },

            formatJson() {
                if (!this.aceEditor) return;
                try {
                    const currentJson = this.aceEditor.getValue();
                    const parsed = JSON.parse(currentJson);
                    const formatted = JSON.stringify(parsed, null, 2);
                    this.aceEditor.setValue(formatted);
                    this.aceEditor.clearSelection();
                    this.aceEditor.gotoLine(1);
                    showSuccess('JSON formatted successfully');
                } catch (e) {
                    showError('Invalid JSON: ' + e.message);
                }
            },

            validateJson() {
                if (!this.aceEditor) return;
                try {
                    const currentJson = this.aceEditor.getValue();
                    JSON.parse(currentJson);
                    showSuccess('✓ JSON is valid');
                } catch (e) {
                    showError('✗ Invalid JSON: ' + e.message);
                }
            },

            saveWorkflow() {
                if (!this.workflowForm.id || !this.workflowForm.name) {
                    showError('ID and Name are required');
                    return;
                }

                let jsonPayload;
                try {
                    const jsonText = this.aceEditor ? this.aceEditor.getValue() : '{}';
                    jsonPayload = JSON.parse(jsonText);
                } catch (e) {
                    showError('Invalid JSON: ' + e.message);
                    return;
                }

                const rpcMethod = this.isEditMode ? 'UpdateWorkflow' : 'CreateWorkflow';
                const params = {
                    WorkflowId: this.workflowForm.id,
                    Name: this.workflowForm.name,
                    Description: this.workflowForm.description,
                    Version: this.workflowForm.version,
                    IsPublished: this.workflowForm.isPublished,
                    Definition: jsonPayload
                };

                rpcAEP(rpcMethod, params, (data) => {
                    const payload = Array.isArray(data) ? (data[0] || {}) : (data || {});
                    const success = payload.Success || payload.success || payload.IsSucceeded;
                    
                    if (success) {
                        closeComponent(this.cid, { success: true });
                    } else {
                        showError('Error: ' + (payload.ErrorMessage || payload.errorMessage || 'Unknown error'));
                    }
                }, (error) => {
                    console.error('RPC Error:', error);
                    showError('Error: ' + error);
                });
            },

            cancel() {
                closeComponent(this.cid, { cancelled: true });
            }
        },
        props: { cid: String }
    }
</script>
