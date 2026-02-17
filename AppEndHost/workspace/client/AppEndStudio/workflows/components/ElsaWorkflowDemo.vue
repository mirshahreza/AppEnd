<template>
    <div class="workflow-demo-container h-100 d-flex flex-column">
        <WorkflowEditor 
            ref="designer"
            :workflow-id="selectedWorkflowId"
            :workflow-data="currentWorkflow"
            @workflow-saved="handleWorkflowSaved"
            @error="handleError"
        />
    </div>
</template>

<script>
import WorkflowEditor from '../WorkflowEditor.vue';

export default {
    name: 'ElsaWorkflowDemo',
    components: { WorkflowEditor },
    data() {
        return {
            selectedWorkflowId: null,
            currentWorkflow: null
        };
    },
    methods: {
        handleWorkflowSaved(data) {
            console.log('Workflow saved:', data);
        },
        handleError(error) {
            console.error('Workflow error:', error);
        }
    }
};
</script>

<style scoped>
.workflow-demo-container {
    display: flex;
    flex-direction: column;
    width: 100%;
    height: 100%;
}
</style>
