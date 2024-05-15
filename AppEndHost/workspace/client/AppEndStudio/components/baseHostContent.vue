<template>
    <div class="card border-0 shadow-lg bg-transparent rounded-0 h-100">
        <div class="card-body p-3 bg-transparent fs-d8">
            <div class="h-100 w-100" data-flex-splitter-horizontal style="flex: auto;">
                <div class="h-100" style="min-width:300px;width:30%;">

                    <div class="card h-100 shadow-sm">
                        <div class="card-header">
                            Host Content
                        </div>
                        <div class="card-body p-0 pt-2 scrollable">
                            <div id="hostTree"></div>
                        </div>
                    </div>

                </div>
                <div role="separator" tabindex="1" class="bg-light" style="width:.5%;"></div>
                <div class="h-100" style="min-width:200px;width:69.5%;">

                    <div class="card h-100 shadow-sm">
                        <div class="card-header">
                            <span class="fw-bold" v-if="selectedNode===null">Not selected</span>
                            <span class="fw-bold" v-if="selectedNode!==null">{{selectedNode.id.replaceAll('/',' / ')}}</span>
                        </div>
                        <div class="card-body p-0 scrollable">
                            <div class="container-fluid h-100">
                                <div class="row h-100" v-if="selectedNode!==null && contentType==='text' && preview===false">
                                    <div class="code-editor-container h-100" id="aceTextEditor"></div>
                                </div>

                                <div class="row h-100" v-if="selectedNode!==null && (contentType==='zip' || contentType==='aepkg') && preview===false">
                                    <div class="h-100" id="zipEditor"></div>
                                </div>

                                <div class="row h-100 align-items-center text-center" v-if="selectedNode!==null && contentType==='text' && preview===true">
                                    <div class="col-48">
                                        <div class="fst-italic fs-1d4 text-secondary">
                                            Double click to edit
                                        </div>
                                    </div>
                                </div>

                                <div class="row h-100 align-items-center text-center" v-if="selectedNode!==null && contentType==='folder' && preview===true">
                                    <div class="col-48">
                                        <div class="fst-italic fs-1d4 text-secondary">
                                            Folder
                                        </div>
                                    </div>
                                </div>

                                <div class="row h-100 align-items-center text-center" v-if="selectedNode!==null && contentType==='image' && preview===true">
                                    <div class="col-48">
                                        <img :src="selectedNode.id.replace('/workspace/client','')" style="max-width:90%;max-height:90%;" />
                                    </div>
                                </div>

                                <div class="row h-100 align-items-center text-center" v-if="selectedNode!==null && contentType==='zip' && preview===true">
                                    <div class="col-48">
                                        <i class="fa-solid fa-fw fa-4x fa-file-zipper"></i>
                                        <br />
                                        <div>Zip file</div>
                                        <div class="fst-italic fs-1d4 text-secondary">
                                            Double click to edit
                                        </div>
                                    </div>
                                </div>

                                <div class="row h-100 align-items-center text-center" v-if="selectedNode!==null && contentType==='aepkg' && preview===true">
                                    <div class="col-48">
                                        <img src="/a..lib/images/AppEnd-Logo-Only.png" style="max-width:100%;max-height:100%;" />
                                        <br />
                                        <div>AppEnd Package file</div>
                                        <div class="fst-italic fs-1d4 text-secondary">
                                            Double click to edit
                                        </div>
                                    </div>
                                </div>

                                <div class="row h-100 align-items-center text-center" v-if="selectedNode!==null && contentType==='other' && preview===true">
                                    <div class="col-48">
                                        Other
                                    </div>
                                </div>

                                <div class="row h-100 align-items-center text-center" v-if="selectedNode===null">
                                    <div class="col-48">
                                        <div class="fst-italic fs-1d4 text-secondary">
                                            Select a node on the host tree view
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
    let _this = { cid: "", c: null, inputs: {}, hostTree: null, selectedNode: null, regulator: null, preview: false, contentType: null, editView: false, textToEdit:"aaa" };
    
    export default {
        methods: {
            ok(e) {
                if (_this.c.inputs.callback) _this.c.inputs.callback();
                _this.c.close();
            },
            addContent(tree, par, content) {
                _.forEach(content["folders"], function (i) {
                    tree.jstree(true).create_node(par, { id: i.Value, text: i.Name, type: "folder", data: i }, "last");
                });
                _.forEach(content["files"], function (i) {
                    tree.jstree(true).create_node(par, { id: i.Value, text: i.Name, type: "file", data: i }, "last");
                });
                tree.jstree(true).open_node(par);
            },
            readFolderContent(tree, par, folderPath) {
                rpcAEP("GetFolderContent", { PathToRead: folderPath }, function (res) {
                    _this.c.addContent(tree, par, R0R(res));
                });
            },
            genId(s) {
                return "n" + s.replaceAll('/', '_');
            },
            setupHostTree(tree, treeSelector) {
                tree = $(treeSelector);
                tree.jstree(_this.c.getTreeConfig());
                _this.c.readFolderContent(tree, null, '/');
                tree.bind("dblclick.jstree", function (event) {
                    _this.c.preview = false;
                    let node = fixNull(tree.jstree(true).get_selected(true)[0], null);
                    if (node === null) return;
                    if (node.type === "folder") {
                        if (node.loaded !== true) {
                            _this.c.readFolderContent(tree, node, node.data.Value);
                            node.loaded = true;
                            _this.c.preview = true;
                        }
                    } else {
                        _this.c.goEditView(tree, node);
                    }
                });
                tree.bind("select_node.jstree", function (evt, data) {
                    _this.c.preview = true;
                    _this.c.selectedNode = data.node;
                    setTimeout(function () {
                        if (_this.c.preview === true) _this.c.makePreview(tree, _this.c.selectedNode);
                    }, 250);
                });
            },
            goEditView(tree, node) {
                _this.c.preview = false;
                _this.c.contentType = getContentType(node.id);
                $(document).ready(function () {
                    if (_this.c.contentType === "text") {
                        $(document).ready(function () {
                            rpcAEP("GetFileContent", { "PathToRead": node.id }, function (res) {
                                ace.edit("aceTextEditor", { mode: getEditorMode(node.id), value: R0R(res) });
                            });
                        });
                    } else if (_this.c.contentType === "zip" || _this.c.contentType === "aepkg") {
                        rpcAEP("GetZipFileContent", { "PathToRead": node.id }, function (res) {
                            showJson(res);
                        });
                    } else {
                        _this.c.preview = true;
                        this.makePreview(tree, node);
                    }
                });
            },
            makePreview(tree, node) {
                if (node.type === "file") {
                    _this.c.contentType = getContentType(node.id);
                } else {
                    _this.c.contentType = "folder";
                }
            },
            getTreeConfig() {
                return {
                    core: { check_callback: true, },
                    plugins: ["types"],
                    types: {
                        folder: { icon: "fa-solid fa-folder", valid_children: ["file", "folder"] },
                        file: { icon: "fa-solid fa-file text-info", valid_children: [] }
                    }
                };
            },
            cleanTree() {

            },
            cancel() { _this.c.close(); },
            close() { shared.closeComponent(_this.cid); }
        },
        setup(props) {
            _this.cid = props['cid'];
            _this.inputs = shared["params_" + _this.cid];
            _this.selectedPath = (fixNull(_this.inputs, '') !== '') ? _this.inputs["selectedPath"] : null;
        },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { initVueComponent(_this); _this.c.setupHostTree(_this.c.hostTree, "#hostTree:first"); },
        props: { cid: String }
    }
</script>
