<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-header p-2 bg-light-subtle rounded-0 border-0">
            <div class="hstack gap-1">
                <input class="form-control form-control-sm text-center" placeholder="find here..." @keyup="highLight" id="findInput" />
                <div class="p-0 ms-auto"></div>
            </div>
        </div>
        <div class="card-body p-2 scrollable">
            <div class="card h-100 border-light bg-light bg-opacity-75 border-0">
                <div class="card-body rounded rounded-2 border border-3 border-light fs-d8 p-3 bg-transparent scrollable">

                    <div class="mb-4 ControllerBlock" v-for="c in d">
                        <div class="p-0">
                            <span class="NamespaceName p-2 fs-d9">{{c.Namespace}}</span>
                            <span>.</span>
                            <span class="ClassName p-2">{{c.Name}}</span>
                        </div>
                        <div class="card my-0 border-secondary-subtle">
                            <div class="card-body p-3">
                                <div class="badge text-bg-light" v-for="method in c.DynaMethods">
                                    <div class="hover-primary fs-d9 pointer"
                                         :title="method.Name" :data-ae-key="method.Name"
                                         @click="openMethodAttributesEditor(c.Namespace,c.Name,method.Name)">
                                        <i class="fa-solid fa-fw fa-dot-circle opacity-25"></i> <span class="MethodName">{{method.Name}}</span>
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
    shared.setAppTitle("All Methods");
    let _this = { cid: "", c: null, d: [] };
    export default {
        methods: {
            highLight() {
                let txt = $("#findInput").val().trim().toLowerCase();
                $(".NamespaceName").removeClass("text-bg-success").removeClass("fb");
                $(".ClassName").removeClass("text-bg-success").removeClass("fb");
                $(".MethodName").removeClass("text-bg-success").removeClass("fb");
                if (txt.length === 0) return;
                $(".NamespaceName").each(function () {
                    let n = $(this);
                    if (n.text().toLowerCase().indexOf(txt) > -1) n.addClass("text-bg-success").addClass("fb");
                });
                $(".ClassName").each(function () {
                    let n = $(this);
                    if (n.text().toLowerCase().indexOf(txt) > -1) n.addClass("text-bg-success").addClass("fb");
                });
                $(".MethodName").each(function () {
                    let n = $(this);
                    if (n.text().toLowerCase().indexOf(txt) > -1) n.addClass("text-bg-success").addClass("fb");
                });
            },
            readList() {
                rpcAEP("GetDynaClasses", {}, function (res) {
                    _this.c.d = R0R(res);
                });
            },
            openMethodAttributesEditor(ns, cs, mn) {
                openComponent("components/methodsSettings", { title: `MethodSettings Editor :: ${ns} . ${cs} . ${mn}`, params: { "ns": ns, "cs": cs, "mn": mn } });
            }
        },
        setup(props) {
            _this.cid = props['cid'];
        },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { _this.c.readList(); },
        props: { cid: String }
    }

</script>