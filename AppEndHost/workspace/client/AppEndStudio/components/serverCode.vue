<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-header p-2 bg-success-subtle rounded-0 border-0">
            <div class="hstack gap-1">
                <input class="form-control form-control-sm" style="max-width:175px;" @keyup="highLight" id="findInput" />
                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="readList">
                    <i class="fa-solid fa-search"></i>
                </button>
                <div class="p-0 ms-auto"></div>
                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="createController">
                    <i class="fa-solid fa-plus"></i> <span>Create Controller</span>
                </button>
            </div>
        </div>
        <div class="card-body p-2 scrollable">
            <div class="card h-100 border-light bg-light bg-opacity-75 border-0">
                <div class="card-body rounded rounded-2 border border-3 border-light fs-d8 p-2 bg-transparent scrollable">

                    <div class="mb-2 ControllerBlock" v-for="c in d">
                        <i class="fa-solid fa-times text-secondary text-hover-danger ms-2 pointer"
                           v-if="c.Namespace!=='Zzz' && c.Namespace!=='DefaultRepo' && c.Name!=='AppEndProxy'"
                           @click="removeClass(c.Namespace,c.Name)"></i>
                        <a class="btn btn-sm btn-link text-decoration-none text-nowrap p-0 px-1 fs-d9" :href="'?c=/a.PublicComponents/baseFileEditor&filePath=workspace/server/'+c.Namespace+'.'+c.Name+'.cs'">
                            <i class="fa-solid fa-fw fa-edit"></i>
                            <span class="NamespaceName">{{c.Namespace}}</span><span class="px-1">.</span><span class="ClassName">{{c.Name}}</span>
                        </a>
                        <div class="btn btn-sm btn-link text-decoration-none text-dark text-nowrap bg-hover-light p-0 px-1 fs-d8" @click="createAPI(c.Namespace,c.Name)">
                            <i class="fa-solid fa-fw fa-plus"></i> Create API
                        </div>
                        <div class="card my-1 border-secondary-subtle">
                            <div class="card-body p-2">
                                <div class="btn btn-sm btn-link text-decoration-none text-dark text-nowrap bg-hover-light p-0 px-1 fs-d8" v-for="method in c.DynaMethods">
                                    <div class="hover-primary pointer"
                                         :title="method.Name" :data-ae-key="method.Name"
                                         @click="openMethodAttributesEditor(c.Namespace,c.Name,method.Name)">
                                        <span class="MethodName">{{method.Name}}</span>
                                        <i class="fa-solid fa-times text-secondary text-hover-danger ms-2" 
                                           v-if="c.Namespace!=='Zzz' && c.Namespace!=='DefaultRepo' && c.Name!=='AppEndProxy'"
                                           @click="removeMethod(c.Namespace,c.Name,method.Name)"></i>
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
    shared.setAppTitle("Server Code");
    let _this = { cid: "", c: null, d: [] };
    export default {
        methods: {
            removeClass(ns, cs) {
                showConfirm({
                    title: "Remove Controller", message1: "Are you sure you want to delete this controller?", message2: cs,
                    callback: function () {
                        rpcAEP("RemoveClass", { NamespaceName: ns, ClassName: cs }, function (res) {
                            showSuccess("Method removed");
                            _this.c.readList();
                        });
                    }
                });
                event.stopPropagation();
            },
            createAPI(ns, cs) {
                showPrompt({
                    title: "Create API", message1: "Enter a name for new API", message2: "Spaces and Wildcards are not allowed",
                    callback: function (ret) {
                        rpcAEP("CreateMethod", { NamespaceName: ns, ClassName: cs, MethodName: ret }, function (res) {
                            showSuccess("New API created");
                            _this.c.readList();
                        });
                    }
                });
            },
            removeMethod(ns, cs, mn) {
                showConfirm({
                    title: "Remove Method", message1: "Are you sure you want to delete this method?", message2: mn,
                    callback: function () {
                        rpcAEP("RemoveMethod", { NamespaceName: ns, ClassName: cs, MethodName: mn }, function (res) {
                            showSuccess("Method removed");
                            _this.c.readList();
                        });
                    }
                });
                event.stopPropagation();
            },
            createController() {
                openComponent("components/serverCodeCreateController", {
                    title: "Create Controller",
                    params: {
                        callback: function (ret) {
                            rpcAEP("CreateController", { NamespaceName: ret.NamespaceName, ClassName: ret.ClassName, AddSampleMthod: ret.AddSampleMthod }, function (res) {
                                _this.c.readList();
                            });
                        }
                    }
                });
            },
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
                openComponent("components/serverApiSettings", { title: `MethodSettings Editor :: ${ns} . ${cs} . ${mn}`, params: { "ns": ns, "cs": cs, "mn": mn } });
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