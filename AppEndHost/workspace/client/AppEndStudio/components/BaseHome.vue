<template>
    <div class="card h-100 rounded-bottom-0 rounded-end-0 bg-transparent border-0">
        <div class="card-body p-2">
            <div class="card h-100 border-light bg-light bg-opacity-75 border-0">
                <div class="card-body rounded rounded-2 border border-3 border-light fs-d8 p-3 bg-transparent">

                    <div class="row h-100 bg-transparent">
                        <div class="col-48 col-md-10">
                            <div class="card" style="background-color:#fff7f7">
                                <div class="card-body">
                                    <component-loader src="/a.SharedComponents/DigitalClock" uid="digitalClock" />
                                    <component-loader src="/a.SharedComponents/MySummary" uid="mySummary" />
                                </div>
                            </div>
                            <div class="card mt-2">
                                <div class="card-body">
                                    Actions
                                    <hr class="my-1 border-3 border-secondary-subtle" />
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
                        <div class="col-48 col-md-2"></div>
                        <div class="col-48 col-md-36">
                            <component-loader src="/a.SharedComponents/MyShortcuts" uid="myShortcuts" />
                            <div class="p-2">&nbsp;</div>
                            <div class="row" style="height:190px;">
                                <div class="col-24 h-100">
                                    <component-loader src="components/BaseServerSummary" uid="baseServerSummary" />
                                </div>
                                <div class="col-24 h-100">

                                </div>
                            </div>
                            <div class="p-2">&nbsp;</div>
                            <component-loader src="/a.SharedComponents/BaseSubApps" uid="baseSubApps" />
                        </div>
                    </div>

                </div>
            </div>

        </div>
        <div class="card-footer p-2 bg-light-subtle rounded-bottom-0 fs-d8">
            <component-loader src="/a.SharedComponents/baseAcountActions" uid="baseAcountActions" />
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
                        rpcAEP("RebuildProject", { }, function () {
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
        mounted() {  },
        props: { cid: String }
    }
</script>