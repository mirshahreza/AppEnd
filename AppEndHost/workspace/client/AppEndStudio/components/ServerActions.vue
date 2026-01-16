<template>
    <div class="card mt-2 text-center shadow-sm fs-d8">
        <div class="card-header">
            <span class="fw-bold">Actions</span>
        </div>
        <div class="card-body p-1">
            <div class="vstack gap-1 align-items-start">

                <button class="btn btn-sm btn-outline-primary w-100 rounded-3 border-0 text-decoration-none text-start" @click="reBuild">
                    <i class="fa-solid fa-fw fa-chevron-right"></i>
                    <span>ReBuild Code Files</span>
                </button>

                <button class="btn btn-sm btn-outline-primary w-100 rounded-3 border-0 text-decoration-none text-start" @click="reloadTasks">
                    <i class="fa-solid fa-fw fa-chevron-right"></i>
                    <span>Reload Tasks</span>
                </button>

            </div>
        </div>
    </div>
</template>

<script>
    let _this = { cid: "", c: null };

    export default {
        methods: {
            reloadTasks() {
                showConfirm({
                    title: "Reload Tasks", message1: "By this action AppEnd will reload Scheduled Tasks.", message2: "This task is not dangerous; it only reloads scheduled tasks and prepares them for execution.",
                    callback: function () {
                        rpc({
                            requests: [{ Method: 'Zzz.AppEndProxy.SchedulerReloadTasks', Inputs: {} }],
                            onDone(res) {
                                let result = R0R(res);
                                if (result && typeof result === 'object' && 'Success' in result) {
                                    if (result.Success) {
                                        showSuccess(result.Message || 'Tasks reloaded successfully');
                                    } else {
                                        showError(result.Message || 'Failed to reload tasks');
                                    }
                                } else {
                                    showSuccess('Tasks reload completed');
                                }
                            },
                            onFail(err) {
                                showError('Error reloading tasks');
                                console.error(err);
                            }
                        });
                    }
                });
            },
            reBuild() {
                showConfirm({
                    title: "ReBuild", message1: "By this action AppEnd will rebuild a new assembly from c# codes.", message2: "This task is not dangerous; it only recompiles the files' code from scratch.",
                    callback: function () {
                        rpcAEP("RebuildProject", {}, function () {
                            showSuccess("ReBuild done");
                        });
                    }
                });
            }

        },
        setup(props) { _this.cid = props['cid']; },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { },
        props: { cid: String }
    }
</script>

