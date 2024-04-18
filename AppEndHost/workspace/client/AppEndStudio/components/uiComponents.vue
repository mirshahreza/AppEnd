<template>
    <div class="card h-100 rounded rounded-2 rounded-bottom-0 rounded-end-0 bg-transparent border-0">
        <div class="card-header p-2 bg-light-subtle rounded-end-0 border-0">
            <div class="input-group input-group-sm border-0 bg-transparent">

                <button type="button" class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="create">
                    <i class="fa-solid fa-file-alt fa-bounce" style="--fa-animation-iteration-count:1"></i> <span>Create Empty Component</span>
                </button>

                <input type="text" class="form-control form-control-sm border-0 rounded-0 bg-transparent" disabled />

            </div>
        </div>
        <div class="card-body p-2">
            <div class="card h-100 border-light bg-light bg-opacity-75 border-0">
                <div class="card-body rounded rounded-2 border border-3 border-light p-0 bg-transparent scrollable">
                    <div v-if="d.length>0">
                        <table class="table table-sm table-hover w-100 bg-transparent">
                            <tbody>
                                <tr v-for="c in d">
                                    <td :data-ae-key="c" style="width:75px;">
                                        <a :href="'?c=components/controlDesigner&edt=/'+shared.getQueryString('path')+'/'+c"
                                           class="btn btn-link btn-sm text-dark text-hover-primary p-0 text-decoration-none fs-d6">
                                            <i class="fa-solid fa-edit me-1"></i><span>Design</span>
                                        </a>
                                    </td>
                                    <td :data-ae-key="c" style="width:75px;">
                                        <a :href="'?c=/a.PublicComponents/baseFileEditor&filePath=workspace/client/'+shared.getQueryString('path')+'/'+c"
                                           class="btn btn-link btn-sm text-dark text-hover-primary p-0 text-decoration-none fs-d6">
                                            <i class="fa-solid fa-edit me-1"></i>Code
                                        </a>
                                    </td>

                                    <td :data-ae-key="c">
                                        <span class="text-dark p-0 text-decoration-none fs-d8 fw-bold">
                                            {{c.replace('.vue','').replace('.cshtml','')}}
                                        </span>
                                    </td>
                                    <td class="text-end" style="width:100px;">
                                        <button class="btn btn-link btn-sm text-secondary text-hover-danger p-0 text-decoration-none fs-d8" @click="duplicate">
                                            <i class="fa-solid fa-copy me-1"></i>Duplicate
                                        </button>
                                    </td>
                                    <td class="text-end" style="width:85px;">
                                        <button class="btn btn-link btn-sm text-secondary text-hover-danger p-0 text-decoration-none fs-d8"
                                                v-if="c!=='Login.vue' && c!=='SideMenu.vue'"
                                                @click="rename">
                                            <i class="fa-solid fa-i-cursor me-1"></i>Rename
                                        </button>
                                    </td>
                                    <td class="text-end" style="width:85px;">
                                        <button class="btn btn-link btn-sm text-secondary text-hover-danger p-0 text-decoration-none fs-d8"
                                                v-if="c!=='Login.vue' && c!=='SideMenu.vue'"
                                                @click="delete">
                                            <i class="fa-solid fa-trash me-1"></i>
                                        </button>
                                    </td>
                                    <td style="width:8px;"></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                    
                    <div v-else class="container-fluid">
                        <div class="row row-cols-1 row-cols-md-3">
                            <div class="col" v-for="f in folders">
                                <div class="card h-100 shadow-sm">
                                    <div class="card-body">
                                        <div class="text-dark fs-d9">
                                            <a :href="'?c=components/uiComponents&path='+f.link" target="_blank"
                                               class="form-control bg-hover-primary p-1 border-light fs-1d1 text-decoration-none pointer">
                                                <i class="fa-solid fa-fw fa-folder-open text-success"></i><span class="fw-bold">{{f.title}}</span>
                                            </a>
                                        </div>
                                        <hr class="my-1" />
                                        <div class="fs-d8 text-secondary">
                                            {{f.note}}
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
    shared.setAppTitle("UiComponents");
    shared.setAppSubTitle(getQueryString("path"));

    let _this = { cid: "", c: null, d: [], path: null, folders: [] };
    _this.path = fixNull(getQueryString("path"), '');
    _this.folders = [
        { "link": "a.UserComponents", "title": "User Components", "note": "Components generated by developers" },
        { "link": "a.SharedComponents", "title": "Shared Components", "note": "Components generated by developers" },
        { "link": "a.DbComponents", "title": "Db Components", "note": "Components generated by template engine" },
        { "link": "a.Layouts", "title": "Layouts", "note": "Layout files used to call directly as a page" },
        { "link": ".DbTemplates", "title": "Db Templates", "note": "Used by template engine to generate database components" }
    ];
    export default {
        methods: {
            create() {
                showPrompt({
                    title: "Create Empty Component", message1: "Enter a valid name to create new empty component", message2: "Spaces and Wildcards are not allowed",
                    callback: function (ret) {
                        let newFilePath = 'workspace/client/' + _this.c.path + "/" + ret.replace('.vue', '') + '.vue';
                        rpcAEP("CreateEmptyComponent", { ComponentFullPath: newFilePath }, function (res) {
                            _this.c.readList();
                        });
                    }
                });
            },
            duplicate(e) {
                let fileName = getKey(e);
                rpcAEP("DuplicateFileItem", { FilePath: 'workspace/client/' + _this.c.path + "/" + fileName }, function () {
                    _this.c.readList();
                });
            },
            rename(e) {
                let fileName = getKey(e) ;
                let filePath = 'workspace/client/' + _this.c.path + "/" + fileName;
                showPrompt({
                    title: "Rename Component", message1: "Enter a valid name to rename the component", message2: "Spaces and Wildcards are not allowed",
                    callback: function (ret) {
                        let newFilePath = filePath.replace(fileName, ret.replace('.vue', '')) + '.vue';
                        rpcAEP("RenameFileItem", { FilePath: filePath, NewFilePath: newFilePath }, function (res) {
                            _this.c.readList();
                        });
                    }
                });
            },
            delete(e) {
                let fileName = getKey(e);
                shared.showConfirm({
                    title: "Remove Component", message1: "Are you sure you want to remove this item?", message2: fileName,
                    callback: function () {
                        rpcAEP("DeleteFileItem", { FilePath: 'workspace/client/' + _this.c.path + "/" + fileName }, function () {
                            _this.c.readList();
                        });
                    }
                });
            },
            readList() {
                if (_this.c.path === '') return;
                rpcAEP("GetUiComponents", { FolderName: _this.c.path }, function (res) {
                    _this.c.d = R0R(res);
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
