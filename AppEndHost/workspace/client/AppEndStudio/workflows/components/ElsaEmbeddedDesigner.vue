<template>
    <div class="elsa-embedded-designer h-100 d-flex flex-column">
        <!-- Header -->
        <div class="card-header p-3 bg-body-subtle rounded-0 border-0 d-flex justify-content-between align-items-center">
            <div class="hstack gap-3">
                <h5 class="mb-0">
                    <i class="fa-solid fa-diagram-project me-2"></i>
                    Elsa Workflow Designer
                </h5>
                <div class="vr"></div>
                <div v-if="workflowId" class="text-muted small">
                    ID: <code>{{ workflowId }}</code>
                </div>
            </div>
            <div class="hstack gap-2">
                <button 
                    class="btn btn-sm btn-outline-secondary"
                    @click="testConnection"
                    title="Test Elsa Server Connection"
                    :disabled="isLoading">
                    <i class="fa-solid fa-plug me-1"></i>
                    Test Connection
                </button>
            </div>
        </div>

        <!-- Loading Indicator -->
        <div v-if="isLoading" class="d-flex align-items-center justify-content-center flex-grow-1">
            <div class="text-center">
                <div class="spinner-border mb-3" role="status">
                    <span class="visually-hidden">Loading...</span>
                </div>
                <p class="text-muted">Loading Elsa Studio Designer...</p>
                <small class="text-muted">Server: {{ serverUrl }}</small>
            </div>
        </div>

        <!-- Elsa Designer Container -->
        <div v-else class="flex-grow-1 position-relative designer-container">
            <div 
                ref="elsaContainer"
                id="elsa-studio-container"
                style="width: 100%; height: 100%;"></div>

            <!-- Error Message with troubleshooting -->
            <div v-if="error" class="alert alert-danger position-absolute top-50 start-50 translate-middle p-4" style="z-index: 1000; min-width: 500px; max-width: 90%;">
                <h5 class="alert-heading mb-3">
                    <i class="fa-solid fa-exclamation-triangle me-2"></i>
                    Connection Error
                </h5>
                <p class="mb-3">{{ error }}</p>
                <hr>
                <div class="small text-muted mb-3">
                    <p><strong>Elsa Server URL:</strong> <code>{{ serverUrl }}</code></p>
                    <p><strong>Workflow ID:</strong> <code>{{ workflowId || 'Not specified' }}</code></p>
                </div>
                <div class="troubleshooting">
                    <p class="mb-2"><strong>Troubleshooting Steps:</strong></p>
                    <ul class="mb-3">
                        <li>✓ Ensure Elsa Server v3+ is running at {{ serverUrl }}</li>
                        <li>✓ Check CORS is enabled on the Elsa Server</li>
                        <li>✓ Verify network connectivity to Elsa Server</li>
                        <li>✓ Check browser console (F12) for more details</li>
                        <li>✓ Try different server URL if needed</li>
                    </ul>
                </div>
                <button type="button" class="btn btn-sm btn-outline-danger me-2" @click="retryLoad">
                    <i class="fa-solid fa-rotate-right me-1"></i>
                    Retry
                </button>
                <button type="button" class="btn btn-sm btn-outline-secondary" @click="error = null">
                    <i class="fa-solid fa-times me-1"></i>
                    Dismiss
                </button>
            </div>
        </div>
    </div>
</template>

<script>
export default {
    name: 'ElsaEmbeddedDesigner',
    props: {
        workflowId: {
            type: String,
            default: null
        },
        workflowData: {
            type: Object,
            default: null
        },
        serverUrl: {
            type: String,
            default: () => window.location.origin
        },
        apiKey: {
            type: String,
            default: null
        }
    },
    emits: ['workflow-saved', 'workflow-loaded', 'error'],
    data() {
        return {
            isLoading: false,
            error: null,
            elsaElement: null,
            libLoaded: false
        };
    },
    watch: {
        workflowId(newVal) {
            if (newVal && this.libLoaded) {
                this.updateWorkflowId(newVal);
            }
        },
        serverUrl(newVal) {
            if (this.libLoaded) {
                this.reinitializeDesigner();
            }
        }
    },
    methods: {
        /**
         * Initialize the Elsa Embedded Designer
         */
        async initializeElsaDesigner() {
            try {
                this.isLoading = true;
                this.error = null;

                // Check if Elsa library is already loaded
                if (window.ElsaWorkflows) {
                    this.createEditorElement();
                } else {
                    // Load Elsa Workflows Studio library
                    const scriptUrl = `${this.serverUrl}/elsa/studio.js`;
                    const script = document.createElement('script');
                    script.src = scriptUrl;
                    script.crossOrigin = 'anonymous';
                    
                    script.onload = () => {
                        this.libLoaded = true;
                        this.createEditorElement();
                    };
                    
                    script.onerror = () => {
                        this.error = `Failed to load Elsa Studio library from: ${scriptUrl}. Make sure Elsa Server is running and CORS is enabled.`;
                        this.$emit('error', this.error);
                    };

                    document.head.appendChild(script);
                }
            } catch (err) {
                this.error = `Initialization error: ${err.message}`;
                this.$emit('error', this.error);
            } finally {
                this.isLoading = false;
            }
        },

        /**
         * Create the Elsa editor Web Component
         */
        createEditorElement() {
            try {
                const container = this.$refs.elsaContainer;
                if (!container) return;

                // Clear existing content
                container.innerHTML = '';

                // Create the Elsa workflow definition editor element
                const elsaElement = document.createElement('elsa-workflow-definition-editor-screen');
                
                // Set attributes
                elsaElement.setAttribute('server-url', this.serverUrl);
                
                if (this.workflowId) {
                    elsaElement.setAttribute('workflow-definition-id', this.workflowId);
                }
                
                if (this.apiKey) {
                    elsaElement.setAttribute('api-key', this.apiKey);
                }

                // Set styling
                elsaElement.style.width = '100%';
                elsaElement.style.height = '100%';
                elsaElement.style.display = 'block';

                // Listen for Elsa events
                elsaElement.addEventListener('workflow-saved', (e) => {
                    this.$emit('workflow-saved', e.detail);
                });

                elsaElement.addEventListener('workflow-loaded', (e) => {
                    this.$emit('workflow-loaded', e.detail);
                });

                // Append to container
                container.appendChild(elsaElement);
                this.elsaElement = elsaElement;

                this.$emit('workflow-loaded', { workflowId: this.workflowId });
            } catch (err) {
                this.error = `Failed to create editor: ${err.message}`;
                this.$emit('error', this.error);
            }
        },

        /**
         * Update workflow ID
         */
        updateWorkflowId(id) {
            if (this.elsaElement) {
                this.elsaElement.setAttribute('workflow-definition-id', id);
            }
        },

        /**
         * Reinitialize the designer
         */
        reinitializeDesigner() {
            this.libLoaded = false;
            this.elsaElement = null;
            this.initializeElsaDesigner();
        },

        /**
         * Test connection to Elsa Server
         */
        async testConnection() {
            try {
                this.isLoading = true;
                const response = await fetch(`${this.serverUrl}/elsa/api/health`, {
                    method: 'GET',
                    headers: { 'Accept': 'application/json' }
                });
                
                if (response.ok) {
                    const data = await response.json();
                    window.shared?.notify?.(`✓ Elsa Server is online at ${this.serverUrl}`);
                    console.log('Elsa Server Health:', data);
                } else {
                    this.error = `Elsa Server returned status ${response.status}`;
                }
            } catch (err) {
                this.error = `Cannot connect to Elsa Server: ${err.message}`;
            } finally {
                this.isLoading = false;
            }
        },

        /**
         * Retry loading the designer
         */
        retryLoad() {
            this.error = null;
            this.reinitializeDesigner();
        }
    },
    mounted() {
        this.initializeElsaDesigner();
    }
};
</script>

<style scoped>
.elsa-embedded-designer {
    display: flex;
    flex-direction: column;
    width: 100%;
    height: 100%;
    overflow: hidden;
    background-color: #f8f9fa;
}

.designer-container {
    flex: 1 1 auto;
    min-height: 0;
    overflow: hidden;
}
</style>
