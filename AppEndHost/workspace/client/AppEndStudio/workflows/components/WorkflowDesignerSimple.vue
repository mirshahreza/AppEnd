<template>
    <div class="workflow-designer-simple p-4">
        <h3>Workflow Designer - Minimal Version</h3>

        <div class="alert alert-info">
            <strong>Workflow ID:</strong> {{ currentWorkflowId }}
        </div>

        <div class="card mt-3">
            <div class="card-header">
                <h5>Workflow Information</h5>
            </div>
            <div class="card-body">
                <div class="mb-3">
                    <label>Name:</label>
                    <input v-model="workflowName" class="form-control" />
                </div>

                <div class="mb-3">
                    <label>Description:</label>
                    <textarea v-model="workflowDescription" class="form-control" rows="3"></textarea>
                </div>

                <div class="mb-3">
                    <label>Activities Count:</label>
                    <p class="form-control-plaintext">{{ activitiesCount }}</p>
                </div>
            </div>
        </div>

        <div class="mt-3">
            <button class="btn btn-primary" @click="saveWorkflow">
                <i class="fas fa-save"></i> Save
            </button>
            <button class="btn btn-secondary ms-2" @click="closeDesigner">
                <i class="fas fa-times"></i> Close
            </button>
        </div>

        <!-- Debug Info -->
        <div class="card mt-3">
            <div class="card-header bg-dark text-white">
                <h6 class="mb-0">Debug Information</h6>
            </div>
            <div class="card-body bg-light">
                <pre>{{ debugInfo }}</pre>
            </div>
        </div>
    </div>
</template>

<script>
    export default {
        name: "WorkflowDesignerSimple",
        props: {
            cid: String,
        },
        data() {
            return {
                currentWorkflowId: "",
                workflowName: "",
                workflowDescription: "",
                activitiesCount: 0,
            };
        },
        computed: {
            debugInfo() {
                return {
                    cid: this.cid,
                    workflowId: this.currentWorkflowId,
                    params: shared["params_" + this.cid],
                    timestamp: new Date().toISOString(),
                };
            },
        },
        methods: {
            loadWorkflow() {
                // Get params from shared
                const params = shared["params_" + this.cid];
                this.currentWorkflowId = params?.workflowId || "unknown";

                // Mock data
                this.workflowName = "Test Workflow - " + this.currentWorkflowId;
                this.workflowDescription = "This is a test workflow loaded at " + new Date().toLocaleString();
                this.activitiesCount = 4;

                showSuccess("Workflow loaded successfully!");
            },
            saveWorkflow() {
                showSuccess("Workflow saved!");

                // Close with success
                if (window.closeComponent) {
                    closeComponent(this.cid, { success: true });
                }
            },
            closeDesigner() {
                if (window.closeComponent) {
                    closeComponent(this.cid, { success: false });
                }
            },
        },
        mounted() {
            console.log("WorkflowDesignerSimple mounted");
            console.log("Props:", { cid: this.cid });
            console.log("Params:", shared["params_" + this.cid]);

            this.loadWorkflow();
        },
    };
</script>

<style scoped>
    .workflow-designer-simple {
        max-width: 800px;
        margin: 0 auto;
    }

    pre {
        font-size: 12px;
        max-height: 200px;
        overflow: auto;
    }
</style>
