<template>
    <div class="card border-0 bg-transparent rounded-0 h-100">
        <div class="card-body p-0 bg-transparent fs-d8">
            <div class="h-100 w-100" data-flex-splitter-horizontal style="flex: auto;" id="splitContainer">
                <div class="h-100" style="min-width:350px;width:30%;" v-if="shared.fixNull(lockToSelectedPath, '') === ''">
                    <div class="card h-100 shadow-sm">
                        <div class="card-header">
                            Host Content
                        </div>
                        <div class="card-header px-2 bg-warning-subtle">
                            
                            <div class="hstack">
                                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="refreshFolder(null)" title="Refresh Folder">
                                    <i class="fa-solid fa-fw fa-refresh"></i>
                                </button>
                                <div class="vr mx-1"></div>
                                <label for="fileToUpload" class="btn btn-sm btn-link text-decoration-none bg-hover-light" title="Upload">
                                    <i class="fa-solid fa-upload"></i>
                                </label>
                                <input class="form-control collapse" type="file" id="fileToUpload" @change="uploadFile">
                                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="duplicateItem" title="Download">
                                    <i class="fa-solid fa-fw fa-download"></i>
                                </button>
                                <div class="vr mx-1"></div>
                                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="duplicateItem" title="Duplicate">
                                    <i class="fa-solid fa-fw fa-copy"></i>
                                </button>
                                <div class="vr mx-1"></div>
                                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="newFolder" title="New Folder">
                                    <i class="fa-solid fa-fw fa-folder-blank"></i>
                                </button>
                                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="newFile" title="New File">
                                    <i class="fa-solid fa-fw fa-file-alt"></i>
                                </button>
                                <div class="vr mx-1"></div>
                                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="renameItem" title="Rename">
                                    <i class="fa-solid fa-fw fa-i-cursor"></i>
                                </button>
                                <div class="p-0 ms-auto"></div>
                                <button class="btn btn-sm btn-link text-secondary text-hover-danger text-decoration-none bg-hover-light" @click="deleteItem">
                                    <i class="fa-solid fa-fw fa-trash"></i>
                                </button>
                            </div>

                        </div>
                        <div class="card-body p-0 pt-2 scrollable">
                            <div id="hostTree"></div>
                        </div>
                    </div>
                </div>
                <div role="separator" tabindex="1" class="bg-light" style="width:.5%;" v-if="shared.fixNull(lockToSelectedPath, '') === ''"></div>
                <div class="h-100" :style="shared.fixNull(lockToSelectedPath, '') === '' ? 'min-width:600px;width:69.5%;overflow:hidden' : ''">

                    <div class="card h-100 shadow-sm">
                        <div class="card-header" id="selectedNodeHeader">
                            <span class="fw-bold" v-if="selectedNode===null">Not selected</span>
                            <span class="fw-bold" v-if="selectedNode!==null">{{selectedNode.id.replaceAll('/',' / ')}}</span>
                        </div>
                        <div class="card-body p-0">
                            <div class="container-fluid p-0 h-100">
                                
                                <div class="row h-100" v-if="selectedNode!==null && contentType==='text' && preview===false">
                                    <div class="code-editor-container h-100" id="aceTextEditor"></div>
                                </div>

                                <div class="row h-100" v-if="(contentType==='zip' || contentType==='aepkg') && preview===false">
                                    <div class="col pt-0 h-100">
                                        <div class="card border-0 rounded-0 h-100">
                                            <div class="card-header px-2 bg-warning-subtle">
                                                <div class="hstack">
                                                    <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="packTo">
                                                        <i class="fa-solid fa-fw fa-minimize"></i> Pack Selected Item 
                                                    </button>
                                                </div>
                                            </div>
                                            <div class="card-body border-0 rounded-0 p-0">
                                                <div class="row pt-2 h-100">
                                                    <div class="col-24 h-100 p-0 scrollable">
                                                        <div class="h-100" id="workspaceTree"></div>
                                                    </div>
                                                    <div class="col-24 h-100 p-0 scrollable">
                                                        <div class="h-100" id="zipTree"></div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
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

                                <div class="row h-100 align-items-center text-center" v-if="selectedNode===null && shared.fixNull(lockToSelectedPath, '') === ''">
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
    shared.setAppTitle("$auto$");
    let _this = { cid: "", c: null, inputs: {}, lockToSelectedPath: "",selectedNode: null, regulator: null, preview: false, contentType: null, editView: false, textToEdit:"aaa" };
    
    export default {
        methods: {
            packTo() {
                let packingNode = _this.c.getSelectedWorkspaceHostNode();
                packingNode = packingNode === "#" ? "" : packingNode.id;
                let zipFile = _this.c.selectedNode !== null ? _this.c.selectedNode.id : _this.c.lockToSelectedPath;
                rpcAEP("PackItemToZipFile", { ItemToPack: packingNode, ZipFile: zipFile }, function (res) {
                    rpcAEP("GetZipFileContent", { "PathToRead": zipFile }, function (res) {
                        _this.c.setupZipTree(R0R(res), false);
                    });
                });
            },
            renameItem() {
                let tree = $("#hostTree:first");
                let node = _this.c.getSelectedHostNode();
                if (node === "#") return;
                showPrompt({
                    title: "Rename", message1: `Enter a new name for ${node.id}`, message2: "Spaces and Wildcards are not allowed",
                    retVal: node.text,
                    callback: function (ret) {
                        let oldName = node.text;
                        let newName = ret;
                        if (node.type === "file") {
                            let ext = ret.split('.')[ret.split('.').length - 1];
                            newName = fixEndBy(newName, '.' + ext);
                        }
                        let fullNewName = node.id.replace(oldName, newName);
                        rpcAEP("RenameItem", { ItemPath: node.id, NewItemPath: fullNewName }, function (res) {
                            _this.c.refreshFolder(node.parent === "#" ? "#" : tree.jstree(true).get_node(node.parent));
                        });
                    }
                });
            },
            newFolder() {
                let node = _this.c.getSelectedHostNode();
                showPrompt({
                    title: "New Folder", message1: "Enter a name for new folder", message2: "Spaces and Wildcards are not allowed",
                    callback: function (ret) {
                        let folderName = node === "#" ? ret : node.id + "/" + ret;
                        rpcAEP("CreateNewFolder", { PathToCreate: folderName }, function (res) {
                            _this.c.refreshFolder(node);
                        });
                    }
                });
            },
            newFile() {
                let node = _this.c.getSelectedHostNode();
                showPrompt({
                    title: "New File", message1: "Enter a name for new file", message2: "Spaces and Wildcards are not allowed",
                    callback: function (ret) {
                        if (ret.indexOf('.') === -1) ret = ret + '.vue';
                        let fileName = node === "#" ? ret : node.id + "/" + ret;
                        rpcAEP("CreateNewFile", { PathToCreate: fileName }, function (res) {
                            _this.c.refreshFolder(node);
                        });
                    }
                });
            },
            refreshFolder(node) {
                let tree = $("#hostTree:first");
                node = fixNull(node, _this.c.getCurrentFolder());
                if (node === "#") {
                    _this.c.cleanTree(tree);
                    _this.c.setupHostTree("#hostTree:first");
                } else {
                    node.loaded = false;
                    tree.jstree(true).close_node(node);
                    tree.jstree(true).delete_node(node.children);
                    _this.c.readFolderContent(tree, node, node.id);
                }
            },
            getCurrentFolder() {
                let node = _this.c.getSelectedHostNode();
                if (node === "#") return node;
                if (node.type === "folder") return node
                return node.parent;
            },
            duplicateItem() {
                let tree = $("#hostTree:first");
                let node = _this.c.getSelectedHostNode();
                if (node === "#") return;
                rpcAEP("DuplicateItem", { PathToDuplicate: node.id, PathType: node.type }, function (res) {
                    _this.c.refreshFolder(node.parent === "#" ? "#" : tree.jstree(true).get_node(node.parent));
                });
            },
            deleteItem() {
                let tree = $("#hostTree:first");
                let node = _this.c.getSelectedHostNode();
                if (node === "#") return;
                let message2 = node.type === "folder" ? "Becareful, Folder will delete recursively" : "";
                showConfirm({
                    title: "Delete Item", message1: `Are you sure you want to remove [${node.id}]?`, message2: message2,
                    callback: function () {
                        rpcAEP("DeleteItem", { ItemPath: node.id, PathType: node.type }, function (res) {
                            _this.c.refreshFolder(node.parent === "#" ? "#" : tree.jstree(true).get_node(node.parent));
                        });
                    }
                });
            },
            getSelectedHostNode() {
                let tree = $("#hostTree:first");
                let selectedNodes = tree.jstree(true).get_selected(true);
                if (selectedNodes.length > 0) return selectedNodes[0];
                return "#";
            },
            getSelectedWorkspaceHostNode() {
                let tree = $("#workspaceTree:first");
                let selectedNodes = tree.jstree(true).get_selected(true);
                if (selectedNodes.length > 0) return selectedNodes[0];
                return "#";
            },
            readFolderContent(tree, par, folderPath, filter) {
                rpcAEP("GetFolderContent", { PathToRead: folderPath }, function (res) {
                    _this.c.addContent(tree, par, R0R(res), filter);
                });
            },
            addContent(tree, par, content, filter) {
                _.forEach(content["folders"], function (i) {
                    if (fixNull(filter, '') === '' || i.Value.indexOf(filter) === -1) {
                        tree.jstree(true).create_node(par, { id: i.Value, text: i.Name, type: "folder", data: i }, "last");
                    }
                });
                _.forEach(content["files"], function (i) {
                    tree.jstree(true).create_node(par, { id: i.Value, text: i.Name, type: "file", data: i }, "last");
                });
                tree.jstree(true).open_node(par);
            },
            setupZipTree(content, setupHostWorkspace) {
                $(`.scrollable`).overlayScrollbars({});
                let tree = $("#zipTree:first");
                _this.c.cleanTree(tree);
                tree.jstree(_this.c.getTreeConfig());

                let folders = _.map(content, function (i) { return i.substring(0, i.lastIndexOf('/')); });
                let tempFolders = folders;
                _.forEach(folders, function (i) {
                    let iParts = i.split("/");
                    let ff = "";
                    _.forEach(iParts, function (ip) {
                        if (ip !== "") {
                            ff = ff + "/" + ip;
                            tempFolders.push(ff);
                        }
                    });
                });
                folders = tempFolders;
                folders = _.uniq(folders, true);
                folders = _.filter(folders, function (i) { return i !== ""; });
                folders = _.sortBy(folders, [i => i.toLowerCase()]);

                _.forEach(folders, function (f) {
                    let folderName = f.split('/')[f.split('/').length - 1];
                    let d = { value: f, name: folderName };
                    let parentFolderId = f.substring(0, f.lastIndexOf('/'));
                    console.log(folderName + " : " + parentFolderId);
                    let par = tree.jstree(true).get_node(parentFolderId);
                    tree.jstree(true).create_node((par === false ? "#" : par), { id: d.value, text: d.name, type: "folder", data: d }, "last");
                });

                _.forEach(content, function (f) {
                    let fileName = f.split('/')[f.split('/').length - 1];
                    if (fileName !== '') {
                        let folderFullName = f.replace("/" + fileName, "");
                        let par = tree.jstree(true).get_node(folderFullName);
                        let d = { name: fileName, value: f };
                        tree.jstree(true).create_node((par === false ? "#" : par), { id: d.value, text: d.name, type: "file", data: d }, "last");
                        if (par !== false) tree.jstree(true).open_node(par);
                    }
                });
                if (setupHostWorkspace === true) _this.c.setupHostWorkspaceTree("#workspaceTree:first");
            },
            setupHostTree(treeSelector) {
                let tree = $(treeSelector);
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
            setupHostWorkspaceTree(treeSelector) {
                let tree = $(treeSelector);
                _this.c.cleanTree(tree);
                tree.jstree(_this.c.getTreeConfig());
                _this.c.readFolderContent(tree, null, '/workspace', 'appendpackages');
                tree.bind("dblclick.jstree", function (event) {
                    let node = fixNull(tree.jstree(true).get_selected(true)[0], null);
                    if (node === null) return;
                    if (node.type === "folder") {
                        if (node.loaded !== true) {
                            _this.c.readFolderContent(tree, node, node.data.Value);
                            node.loaded = true;
                        }
                    }
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
                            _this.c.setupZipTree(R0R(res), true);
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
            getTreeConfig(dndConf) {
                let conf = {
                    core: { check_callback: true, },
                    plugins: ["types"],
                    types: {
                        folder: { icon: "fa-solid fa-folder", valid_children: ["file", "folder"] },
                        file: { icon: "fa-solid fa-file text-info", valid_children: [] }
                    },
                };

                if (dndConf) {
                    conf["plugins"].push("dnd");
                    conf["dnd"] = dndConf;
                }
                return conf;
            },
            uploadFile() {
                let tree = $("#hostTree:first");
                let node = fixNull(tree.jstree(true).get_selected(true)[0], null);
                let uploadingFolder = (node === null ? "" : node.id);

                if (node !== null) {
                    if (node.type === "file") {
                        let lastPart = uploadingFolder.split('/')[uploadingFolder.split('/').length - 1];
                        uploadingFolder = uploadingFolder.replace(lastPart, "");
                        if (uploadingFolder === "/") uploadingFolder = "";
                    }
                }

                let thisInput = document.getElementById('fileToUpload');
                let fileReader = new FileReader();
                fileReader.onload = function () {
                    let fileBody = getB64Str(fileReader.result);
                    let fileName = thisInput.files[0].name;
                    let finalFileName = uploadingFolder === "" ? fileName : uploadingFolder + "/" + fileName;
                    if (finalFileName.startsWith("/")) finalFileName = finalFileName.replace("/", "");
                    rpcAEP("UploadFile", { FileName: finalFileName, FileBody: fileBody }, function (res) {
                        thisInput.value = '';
                        let par = node === null ? null : (node.type === "folder" ? node : node.parent);
                        if (par !== null) {
                            par.loaded = false;
                            tree.jstree(true).delete_node(par.children);
                            _this.c.readFolderContent(tree, par, par.id);
                        }
                    });
                }
                fileReader.readAsArrayBuffer(thisInput.files[0]);
            },
            cleanTree(tree) {
                try {
                    tree.html("");
                    tree.jstree(true).destroy();
                } catch { }
            },
            ok(e) {
                if (_this.c.inputs.callback) _this.c.inputs.callback();
                _this.c.close();
            },
            cancel() { _this.c.close(); },
            close() { shared.closeComponent(_this.cid); }
        },
        setup(props) {
            _this.cid = props['cid'];
            _this.inputs = shared["params_" + _this.cid];
            _this.lockToSelectedPath = (fixNull(_this.inputs, '') !== '') ? _this.inputs["lockToSelectedPath"] : null;
        },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() {
            initVueComponent(_this);
            if (fixNull(_this.lockToSelectedPath, "") === "") _this.c.setupHostTree("#hostTree:first");
            else {
                $("#selectedNodeHeader").remove();
                $("#splitContainer").removeAttr("data-flex-splitter-horizontal");
                _this.c.preview = false;
                _this.c.contentType = getContentType(_this.c.lockToSelectedPath);
                rpcAEP("GetZipFileContent", { "PathToRead": _this.c.lockToSelectedPath }, function (res) {
                    _this.c.setupZipTree(R0R(res), true);
                });
            }
        },
        props: { cid: String }
    }
</script>
