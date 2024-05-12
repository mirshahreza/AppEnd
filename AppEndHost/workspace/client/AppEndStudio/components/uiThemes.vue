<template>
    <div class="card h-100 rounded rounded-2 rounded-bottom-0 rounded-end-0 bg-transparent border-0">
        <div class="card-body p-2">
            <div class="card h-100 border-light bg-light bg-opacity-75 border-0">
                <div class="card-body rounded rounded-2 border border-3 border-light fs-d8 p-3 bg-transparent scrollable">

                    <div class="row">
                        <div class="col-48 col-md-12 mb-2" v-for="app in d">
                            <div class="card shadow-sm">
                                <div class="card-body">
                                    <div class="text-dark fs-d9">
                                        <a :href="'/'+app.Name+'/'" target="_blank" class="form-control bg-hover-primary p-1 border-light fs-1d1 text-decoration-none pointer">
                                            <i class="fa-solid fa-fw fa-play text-success"></i>
                                            <span>Folder : <span class="fw-bold">{{app.Name}}</span></span>
                                        </a>
                                    </div>
                                    <hr class="my-1" />
                                    <div class="fw-bold text-success">
                                        {{app.Value.title}} {{app.Value['sub-title']}}
                                    </div>
                                    <div>
                                        {{app.Value.lang}} / {{app.Value.dir}} / {{app.Value.calendar}}
                                    </div>
                                    <div>
                                        <a class="text-primary text-hover-success text-decoration-none pointer" :href="'?c=components/uiThemesTranslationManagement&app='+app.Name"><i class="fa-solid fa-fw fa-globe"></i> Translation</a>
                                        <span class="mx-2">|</span>
                                        <a class="text-primary text-hover-success text-decoration-none pointer" :href="'?c=components/uiThemesNavigationManagement&app='+app.Name"><i class="fa-solid fa-fw fa-bars"></i>Navigation</a>
                                    </div>
                                </div>
                                <div class="card-footer px-2">
                                    <table class="w-100">
                                        <tr>
                                            <td>
                                                <span class="text-primary text-hover-success text-decoration-none pointer" @click="edit(app.Name)"><i class="fa-solid fa-fw fa-edit"></i> Edit</span>
                                                <span class="mx-2">|</span>
                                                <span class="text-primary text-hover-success text-decoration-none pointer" @click="duplicate(app.Name)"><i class="fa-solid fa-fw fa-copy"></i> Duplicate</span>
                                            </td>
                                            <td class="text-end" style="width:32px;">
                                                <span class="text-primary text-secondary text-hover-danger text-decoration-none pointer" @click="deleteApp(app.Name)"><i class="fa-solid fa-fw fa-times"></i></span>
                                            </td>
                                        </tr>
                                    </table>
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
    shared.setAppTitle("$auto$");
    let _this = { cid: "", c: null, d: [] };
    export default {
        methods: {
            edit(key) {
                let r = getRow(_this.c.d, "Name", key);
                openComponent("components/uiThemesEdit", {
                    title: "Theme Properties",
                    params: {
                        FolderName: key,
                        Title: r.Value["title"],
                        SubTitle: r.Value["sub-title"],
                        callback: function (ret) {
                            ret["OrigFolderName"] = key;
                            rpcAEP("SetThemeProps", ret, function (res) {
                                _this.c.readList();
                            });
                        }
                    }
                });
            },
            duplicate(key) {
                showPrompt({
                    title: "Theme Duplication", message1: "Enter a name for new Theme", message2: "Spaces and Wildcards are not allowed",
                    callback: function (ret) {
                        rpcAEP("DuplicateTheme", { "OrigFolderName": key, "CopyFolderName": ret }, function (res) {
                            _this.c.readList();
                        });
                    }
                });
            },
            deleteApp(key) {
                showConfirm({
                    title: "Delete Theme", message1: "Are you sure you want to delete this theme?", message2: key,
                    callback: function () {
                        rpcAEP("RemoveTheme", { "FolderName": key }, function (res) {
                            _this.c.readList();
                        });
                    }
                });
            },
            readList() {
                rpcAEP("GetThemes", {}, function (res) {
                    let r = R0R(res);
                    _.each(r, function (i) {
                        i.Value = JSON.parse(i.Value);
                    });
                    _this.c.d = r;
                });
            }
        },
        setup(props) {
            _this.cid = props['cid'];
        },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { _this.c.readList(); },
        props: { cid: String },
    }

</script>
