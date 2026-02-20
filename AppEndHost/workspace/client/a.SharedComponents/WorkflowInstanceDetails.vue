<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-body p-3">
            <div v-if="instance" class="container-fluid">
                <div class="row mb-3">
                    <div class="col-md-6">
                        <strong>Instance ID:</strong>
                        <div class="font-monospace small text-muted">{{ instance.InstanceId }}</div>
                    </div>
                    <div class="col-md-6">
                        <strong>Workflow:</strong>
                        <div>{{ instance.DefinitionName || instance.DefinitionId }}</div>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-md-4">
                        <strong>Status:</strong>
                        <div>
                            <span :class="statusBadgeClass(instance.Status)">
                                {{ instance.Status }}
                            </span>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <strong>Started:</strong>
                        <div class="small text-muted">{{ formatDate(instance.StartedAt) }}</div>
                    </div>
                    <div class="col-md-4">
                        <strong>Finished:</strong>
                        <div class="small text-muted">{{ formatDate(instance.FinishedAt) }}</div>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-md-6">
                        <strong>Incidents:</strong>
                        <div>{{ instance.IncidentCount }}</div>
                    </div>
                    <div class="col-md-6">
                        <strong>Duration:</strong>
                        <div>{{ calculateDuration(instance.StartedAt, instance.FinishedAt) }}</div>
                    </div>
                </div>

                <hr>

                <div class="row">
                    <div class="col-12">
                        <strong>Full Details:</strong>
                        <pre class="bg-light p-2 rounded mt-2" style="max-height: 400px; overflow-y: auto; font-size: 0.85rem;">{{ JSON.stringify(instance, null, 2) }}</pre>
                    </div>
                </div>
            </div>

            <div class="d-flex gap-2 justify-content-end mt-3">
                <button type="button" class="btn btn-secondary" @click="close">Close</button>
            </div>
        </div>
    </div>
</template>

<script>
    shared.setAppTitle("$auto$");
    shared.setAppSubTitle("Instance Details");

    let _this = { cid: "", c: null };

    export default {
        setup(props) {
            _this.cid = props["cid"];
            _this.inputs = shared["params_" + _this.cid];
        },
        data() {
            return {
                ..._this,
                instance: null
            };
        },
        created() { _this.c = this; },
        mounted() {
            this.instance = this.inputs?.instance;
        },
        methods: {
            statusBadgeClass(status) {
                const baseClass = 'badge';
                switch(status) {
                    case 'Running': return baseClass + ' bg-primary';
                    case 'Completed': return baseClass + ' bg-success';
                    case 'Failed': return baseClass + ' bg-danger';
                    case 'Paused': return baseClass + ' bg-warning text-dark';
                    default: return baseClass + ' bg-secondary';
                }
            },

            formatDate(dateStr) {
                if (!dateStr) return '-';
                return new Date(dateStr).toLocaleString();
            },

            calculateDuration(startDate, endDate) {
                if (!startDate) return '-';
                const start = new Date(startDate);
                const end = endDate ? new Date(endDate) : new Date();
                const diffMs = end - start;
                const diffSecs = Math.floor(diffMs / 1000);
                const hours = Math.floor(diffSecs / 3600);
                const minutes = Math.floor((diffSecs % 3600) / 60);
                const seconds = diffSecs % 60;
                if (hours > 0) return `${hours}h ${minutes}m`;
                if (minutes > 0) return `${minutes}m ${seconds}s`;
                return `${seconds}s`;
            },

            close() {
                closeComponent(this.cid);
            }
        },
        props: { cid: String }
    }
</script>
