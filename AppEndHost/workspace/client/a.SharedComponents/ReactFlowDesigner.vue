<template>
    <div class="reactflow-designer h-100 d-flex flex-column">
        <!-- Iframe -->
        <iframe 
            ref="iframeRef"
            :src="iframeUrl"
            class="flex-grow-1 border-0"
            allow="same-origin"
            @load="onIframeLoad"
            @error="onIframeError">
        </iframe>

        <!-- Fallback if iframe fails -->
        <div v-if="iframeFailed" class="alert alert-danger m-3">
            <h5>خطا در بارگذاری ReactFlow Designer</h5>
            <p>مسیر: <code>{{ iframeUrl }}</code></p>
            <p class="mb-0 text-muted small">لطفاً مطمئن شوید که React app ساخته شده است: <code>npm run build</code></p>
        </div>
    </div>
</template>

<script>
    let _this = { cid: "", c: null };

    export default {
        data() {
            return {
                iframeFailed: false,
                iframeUrl: "",
                workflowId: "",
                iframeWindow: null,
                workflowData: null,
                workflowDefinition: null,
                workflowDefinitionJson: null,
                iframeReady: false,
                workflowDefinitionReady: false
            };
        },
        methods: {
            initializeIframe() {
                // Build the iframe URL - test with a simple debug URL first
                const baseUrl = "/a.ReactFlow/dist/index.html";
                this.iframeUrl = `${baseUrl}?workflowId=${encodeURIComponent(this.workflowId)}`;
                console.log('[ReactFlowDesigner] Loading iframe from:', this.iframeUrl);
            },

            loadWorkflowDefinition() {
                rpcAEP('GetWorkflowDefinition', { WorkflowId: this.workflowId }, (data) => {
                    const payload = Array.isArray(data) ? (data[0] || {}) : (data || {});
                    const result = payload.Result || payload.result || payload;
                    const rawJson = result.RawJson || result.rawJson;

                    this.workflowDefinitionJson = rawJson || null;
                    if (rawJson) {
                        try {
                            this.workflowDefinition = JSON.parse(rawJson);
                        } catch (error) {
                            console.error('[ReactFlowDesigner] Failed to parse workflow JSON:', error);
                            this.workflowDefinition = null;
                        }
                    }

                    this.workflowDefinitionReady = true;
                    this.trySendWorkflowDefinition();
                }, (error) => {
                    console.error('[ReactFlowDesigner] Failed to load workflow definition:', error);
                    this.workflowDefinitionReady = true;
                    this.trySendWorkflowDefinition();
                });
            },

            trySendWorkflowDefinition() {
                if (!this.iframeReady || !this.iframeWindow || !this.workflowDefinitionReady) {
                    return;
                }

                this.sendMessageToIframe({
                    type: 'LOAD_WORKFLOW',
                    data: {
                        workflowId: this.workflowId,
                        workflowDefinitionJson: this.workflowDefinitionJson
                    }
                });
            },

            onIframeLoad(event) {
                console.log('[ReactFlowDesigner] Iframe loaded');
                this.iframeFailed = false;
                const iframe = event?.target || this.$refs.iframeRef;

                if (!iframe || !iframe.contentWindow) {
                    console.error('[ReactFlowDesigner] Iframe element not available');
                    return;
                }
                
                try {
                    this.iframeWindow = iframe.contentWindow;
                    this.iframeReady = true;
                    console.log('[ReactFlowDesigner] Iframe window accessible');
                    this.trySendWorkflowDefinition();
                } catch (error) {
                    console.error('[ReactFlowDesigner] Error accessing iframe:', error);
                    showError('خطا در بارگذاری طراح ReactFlow: ' + error.message);
                }
            },

            onIframeError() {
                console.error('[ReactFlowDesigner] Iframe failed to load');
                this.iframeFailed = true;
                showError('خطا: نتوانستم ReactFlow Designer را بارگذاری کنم');
            },

            sendMessageToIframe(message) {
                if (this.iframeWindow) {
                    const iframeOrigin = window.location.origin;
                    console.log('[ReactFlowDesigner] Sending message:', message);
                    this.iframeWindow.postMessage(
                        { ...message, timestamp: Date.now() },
                        iframeOrigin
                    );
                }
            },

            handleIframeMessage(event) {
                // Security check: verify origin
                if (event.origin !== window.location.origin) {
                    console.warn('Message from untrusted origin:', event.origin);
                    return;
                }

                const { type, data, error } = event.data;
                console.log('[ReactFlowDesigner] Message from iframe:', { type, data });

                switch (type) {
                    case 'WORKFLOW_LOADED':
                        console.log('Workflow loaded in ReactFlow:', data);
                        this.workflowData = data;
                        break;
                    case 'WORKFLOW_CHANGED':
                        console.log('Workflow changed:', data);
                        this.workflowData = data;
                        break;
                    case 'REQUEST_SAVE':
                        this.saveWorkflow();
                        break;
                    case 'ERROR':
                        console.error('ReactFlow error:', error);
                        showError(`خطا: ${error}`);
                        break;
                    default:
                        console.log('Unknown message type:', type);
                }
            },

            saveWorkflow() {
                // Request workflow data from iframe
                this.sendMessageToIframe({
                    type: 'GET_WORKFLOW'
                });

                // Wait for response and save
                const checkForData = setInterval(() => {
                    if (this.workflowData) {
                        clearInterval(checkForData);
                        
                        // Save workflow via RPC
                        rpcAEP('SaveWorkflowDefinition', { 
                            WorkflowId: this.workflowId,
                            WorkflowDefinition: JSON.stringify(this.workflowData)
                        }, (result) => {
                            const payload = Array.isArray(result) ? (result[0] || {}) : (result || {});
                            const success = payload.Success || payload.success;
                            
                            if (success) {
                                closeComponent(_this.cid, { success: true });
                            } else {
                                showError('خطا در ذخیره workflow');
                            }
                        }, (error) => {
                            console.error('Save error:', error);
                            showError(`خطا: ${error}`);
                        });
                    }
                }, 100);

                // Timeout after 5 seconds
                setTimeout(() => clearInterval(checkForData), 5000);
            },

            closeDesigner() {
                closeComponent(_this.cid, { success: false });
            },

            cleanup() {
                window.removeEventListener('message', this.handleIframeMessage);
            }
        },
        setup(props) {
            _this.cid = props['cid'];
        },
        created() { 
            _this.c = this;
            const params = shared["params_" + _this.cid] || {};
            this.workflowId = params.workflowId || '';
        },
        mounted() {
            this.initializeIframe();
            this.loadWorkflowDefinition();
        },
        beforeUnmount() {
            this.cleanup();
        },
        props: { cid: String }
    }
</script>

<style scoped>
    .reactflow-designer {
        background: #f8f9fa;
    }

    .reactflow-designer iframe {
        background: white;
    }
</style>
