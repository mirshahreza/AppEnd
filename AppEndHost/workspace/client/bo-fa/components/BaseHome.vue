<template>
    <div class="card h-100 rounded-bottom-0 rounded-end-0 border-0">
        <div class="card-body p-0 h-100 position-relative">
            <div class="h-100 w-100" style="overflow-x: hidden;">
                <div class="h-100" style="width:100%;">
                    <div class="card h-100 rounded-0 border-0">
                        <div class="card-body p-2 p-md-5 scrollable">
                            <div class="container-fluid">
                                <div class="row">
                                    <div class="col-48 col-md-12 col-lg-10 col-xl-8 col-xxl-6">

                                        <div class="card text-center shadow-sm">
                                            <div class="card-body p-2">
                                                <component-loader src="/a.SharedComponents/MySummary" uid="mySummary" />
                                            </div>
                                        </div>

                                        <div class="card mt-2 font-monospace text-center fs-1d5 shadow-sm">
                                            <div class="card-body">
                                                <component-loader src="/a.SharedComponents/DigitalClock" uid="digitalClock" />
                                            </div>
                                        </div>

                                    </div>
                                    <div class="col-48 mt-3 mt-md-0 col-lg-24">
                                        <component-loader src="/a.SharedComponents/MyShortcuts" uid="myShortcuts" />
                                        <component-loader src="/a.SharedComponents/BaseSubApps" uid="baseSubApps" />
                                        <div class="d-none d-md-block">
                                            <component-loader src="components/BaseServerSummary" uid="baseServerSummary" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>


<script>
    shared.setAppTitle(`<i class="fa-solid fa-fw fa-home"></i> <span>${shared.translate('Home')}</span>`);

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

