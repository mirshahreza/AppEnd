<template>
    <div class="card h-100 rounded-bottom-0 rounded-end-0 bg-transparent border-0">
        <div class="card-body p-2">
            <div class="card h-100 border-light bg-light bg-opacity-75 border-0">
                <div class="card-body rounded rounded-2 border border-3 border-light fs-d8 p-3 bg-transparent">

                    <div class="row h-100 bg-transparent">
                        <div class="col-48 col-md-10">
                            <component-loader src="/a.PublicComponents/digitalClock" uid="digitalClock" />
                            <component-loader src="/a.PublicComponents/mySummary" uid="mySummary" />

                            <hr />

                            <div class="btn btn-sm btn-link text-decoration-none" @click="reBuild">
                                <i class="fa-solid fa-fw fa-chevron-right"></i> <span>ReBuild Code Files</span>
                            </div>

                        </div>
                        <div class="col-48 col-md-2"></div>
                        <div class="col-48 col-md-36">
                            <component-loader src="/a.PublicComponents/myShortcuts" uid="myShortcuts" />
                            <div class="p-2">&nbsp;</div>
                            <component-loader src="components/baseNodesInfo" uid="baseNodesInfo" />
                            <div class="p-2">&nbsp;</div>
                            <component-loader src="components/baseServerSummary" uid="baseServerSummary" />
                        </div>
                    </div>

                </div>
            </div>

        </div>
        <div class="card-footer p-2 bg-light-subtle rounded-bottom-0 fs-d8">
            <component-loader src="/a.PublicComponents/baseAcountActions" uid="baseAcountActions" />
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

            }
        },
        setup(props) { _this.cid = props['cid']; },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() {  },
        props: { cid: String }
    }
</script>