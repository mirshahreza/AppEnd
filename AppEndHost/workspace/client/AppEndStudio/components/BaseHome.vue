<template>
    <div class="card h-100 rounded-bottom-0 rounded-end-0 border-0">
        <div class="card-body p-2 p-md-5 h-100 scrollable">
            <div class="container-fluid h-100">
                <div class="row h-100">
                    <div class="col-48 col-lg-12 col-xl-10 col-xxl-8">

                        <div class="card font-monospace text-center fs-1d5 shadow-sm">
                            <div class="card-body">
                                <component-loader src="/a.SharedComponents/DigitalClock" uid="digitalClock" />
                            </div>
                        </div>

                        <div class="card mt-2 font-monospace text-center fs-1d3 shadow-sm">
                            <div class="card-body">
                                <component-loader src="/a.SharedComponents/MySummary" uid="mySummary" />
                            </div>
                        </div>

                        <div class="card mt-2 shadow-sm">
                            <div class="card-header">
                                <div class="hstack">
                                    <span class="fw-bold">Actions</span>
                                </div>
                            </div>
                            <div class="card-body">
                                <div class="vstack gap-0 align-items-start">
                                    <button type="button" class="btn btn-sm btn-link text-decoration-none ps-1" @click="reBuild">
                                        <i class="fa-solid fa-fw fa-chevron-right"></i>
                                        <span>ReBuild Code Files</span>
                                    </button>
                                    <button type="button" class="btn btn-sm btn-link text-decoration-none ps-1" @click="refreshSession">
                                        <i class="fa-solid fa-fw fa-chevron-right"></i>
                                        <span>Refresh Session</span>
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-48 mt-3 mt-md-0 col-lg-18 col-xl-14 col-xxl-14">
                        <component-loader src="/a.SharedComponents/MyShortcuts" uid="myShortcuts" />
                        <component-loader src="components/BaseServerSummary" uid="baseServerSummary" />
                        <component-loader src="/a.SharedComponents/BaseSubApps" uid="baseSubApps" />
                    </div>
                    <div class="col-48 mt-3 mt-md-0 col-lg-18 col-xl-14 col-xxl-12">
                        <component-loader src="/a.SharedComponents/BaseAiChat" uid="aiChat" />
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