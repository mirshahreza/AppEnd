<template>
    <div class="card h-100 rounded-bottom-0 rounded-end-0 bg-transparent border-0">
        <div class="card-body p-2 p-md-5">
            <div class="container-fluid">
                <div class="row h-100 bg-transparent">
                    <div class="col-48 col-md-5">
                        
                        <div class="card bg-success-subtle">
                            <div class="card-body p-1">
                                <div class="font-monospace text-center fs-1d5">
                                    <component-loader src="/a.SharedComponents/DigitalClock" uid="digitalClock" />
                                </div>
                            </div>
                        </div>

                        <div class="card mt-2 bg-danger-subtle">
                            <div class="card-body">
                                <div class="font-monospace text-center fs-1d3">
                                    <component-loader src="/a.SharedComponents/MySummary" uid="mySummary" />
                                </div>
                            </div>
                        </div>

                        <div class="card mt-2">
                            <div class="card-body">
                                Actions
                                <hr class="my-1 border-3 border-primary" />
                                <div class="btn btn-sm btn-link text-decoration-none ps-1" @click="reBuild">
                                    <i class="fa-solid fa-fw fa-chevron-right"></i> <span>ReBuild Code Files</span>
                                </div>
                                <br />
                                <div class="btn btn-sm btn-link text-decoration-none ps-1" @click="refreshSession">
                                    <i class="fa-solid fa-fw fa-chevron-right"></i> <span>Refresh Session</span>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-48 col-md-10">
                        <component-loader src="/a.SharedComponents/MyShortcuts" uid="myShortcuts" />
                        <component-loader src="components/BaseServerSummary" uid="baseServerSummary" />
                        <component-loader src="/a.SharedComponents/BaseSubApps" uid="baseSubApps" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>


<script>
    shared.setAppTitle(`<i class="fa-solid fa-fw fa-home"></i> <span>Home</span>`);
    let _this = { cid: "", c: null };

    export default {
        methods: {
            reBuild() {
                showConfirm({
                    title: "ReBuild", message1: "By this action AppEnd will rebuild a new assembly from c# codes.", message2: "It is not danger, be relax and do it",
                    callback: function () {
                        rpcAEP("RebuildProject", {}, function () {
                            showSuccess("ReBuild done");
                        });
                    }
                });
            },
            refreshSession() {
                let t1 = getUserToken();
                refereshSession();
                let t2 = getUserToken();
                setTimeout(function () { refereshPage(); }, 200);
            }
        },
        setup(props) { _this.cid = props['cid']; },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { },
        props: { cid: String }
    }
</script>