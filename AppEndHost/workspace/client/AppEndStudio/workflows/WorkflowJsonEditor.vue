<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-header p-2 bg-dark text-white">
            <div class="hstack gap-2">
                <h5 class="modal-title mb-0">
                    <i class="fas fa-code me-2"></i>
                    Workflow JSON Editor - {{ workflowId || 'New Workflow' }}
                </h5>
                <div class="ms-auto">
                    <button type="button" class="btn btn-sm btn-outline-light me-2" @click="formatJson" title="Format JSON">
                        <i class="fas fa-align-left me-1"></i>Format
                    </button>
                    <button type="button" class="btn btn-sm btn-outline-light" @click="validateJson" title="Validate JSON">
                        <i class="fas fa-check-circle me-1"></i>Validate
                    </button>
                </div>
            </div>
        </div>
        <div class="card-body p-0" style="background: #1e1e1e;">
            <div id="aceJsonEditor" style="width: 100%; height: calc(100vh - 150px);"></div>
        </div>
        <div class="card-footer bg-light">
            <div class="d-flex align-items-center">
                <small class="text-muted flex-grow-1">
                    <i class="fas fa-info-circle me-2"></i>
                    Use Ctrl+F to find, Ctrl+H to replace, Ctrl+/ to comment
                </small>
                <button type="button" class="btn btn-secondary me-2" @click="cancel">Close</button>
                <button type="button" class="btn btn-success" @click="save">
                    <i class="fas fa-save me-2"></i>Save & Close
                </button>
            </div>
        </div>
    </div>
</template>

<script>
    shared.setAppTitle("$auto$");
    shared.setAppSubTitle("JSON Editor");

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
                workflowId: ''
            };
        },
        created() { _this.c = this; },
        mounted() {
            this.workflowId = this.inputs?.workflowId || '';
            this.initializeAceEditor();
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
                const editorElement = document.getElementById('aceJsonEditor');
                if (!editorElement) return;

                this.aceEditor = ace.edit(editorElement);
                this.aceEditor.setTheme('ace/theme/monokai');
                this.aceEditor.session.setMode('ace/mode/json');
                this.aceEditor.session.setTabSize(2);
                this.aceEditor.session.setUseSoftTabs(true);
                this.aceEditor.setShowPrintMargin(false);
                this.aceEditor.setFontSize(13);
                
                const currentJson = this.inputs?.json || '';
                if (currentJson) {
                    try {
                        const parsed = JSON.parse(currentJson);
                        this.aceEditor.setValue(JSON.stringify(parsed, null, 2));
                    } catch (e) {
                        this.aceEditor.setValue(currentJson);
                    }
                }

                this.aceEditor.commands.addCommand({
                    name: 'saveEditor',
                    bindKey: { win: 'Ctrl-S', mac: 'Cmd-S' },
                    exec: () => this.save()
                });

                this.aceEditor.focus();
                this.aceEditor.clearSelection();
            },

            formatJson() {
                if (!this.aceEditor) return;
                try {
                    const currentJson = this.aceEditor.getValue();
                    const parsed = JSON.parse(currentJson);
                    const formatted = JSON.stringify(parsed, null, 2);
                    this.aceEditor.setValue(formatted);
                    this.aceEditor.clearSelection();
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

            save() {
                if (this.aceEditor) {
                    const json = this.aceEditor.getValue();
                    closeComponent(this.cid, { json: json });
                }
            },

            cancel() {
                closeComponent(this.cid, { cancelled: true });
            }
        },
        props: { cid: String }
    }
</script>
