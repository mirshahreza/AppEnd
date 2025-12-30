<template>
    <div class="card h-100 rounded rounded-2 rounded-bottom-0 rounded-end-0 bg-transparent border-0">
        <div class="card-header p-2 bg-frame rounded-0 border-0">
            <div class="hstack gap-1">
                <button type="button" class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="addNewCategory">
                    <i class="fa-solid fa-file-alt fa-bounce" style="--fa-animation-iteration-count:1"></i> <span>New Category</span>
                </button>
                <button type="button" class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="addMenuItem">
                    <i class="fa-solid fa-file-alt fa-bounce" style="--fa-animation-iteration-count:1"></i> <span>New MenuItem</span>
                </button>

                <button type="button" class="btn btn-sm btn-link text-decoration-none text-hover-danger" @click="removeMenuItem" v-if="selectedNode!==null">
                    <i class="fa-solid fa-times fa-bounce" style="--fa-animation-iteration-count:1"></i> <span>Delete MenuItem</span>
                </button>

                <div class="p-0 ms-auto"></div>

                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="saveContent">
                    <i class="fa-solid fa-fw fa-save"></i> <span>Save Navigation</span>
                </button>
            </div>
        </div>
        <div class="card-body p-2">
            <div class="card h-100 border-light bg-light bg-opacity-75 border-0">
                <div class="card-body rounded rounded-2 border border-3 border-light fs-d8 p-0 bg-transparent">

                    <div class="container-fluid h-100">
                        <div class="row h-100">
                            <div class="col-14 h-100">
                                <div class="card h-100 rounded-2 shadow-sm border-0 bg-transparent">
                                    <div class="card-header p-2">
                                        Navigation Menu
                                    </div>
                                    <div class="card-body scrollable p-2" id="navMenu">
                                        <div id="navTree"></div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-34 h-100">
                                <div class="card h-100 rounded-2 shadow-sm border-0 bg-transparent" v-if="selectedNode!==null">
                                    <div class="card-header p-2">
                                        Item Properties
                                    </div>
                                    <div class="card-body scrollable p-2">
                                        <div class="container-fluid">
                                            <div class="row">
                                                <div class="col-48">
                                                    <table class="w-100">
                                                        <tbody>
                                                            <tr>
                                                                <td style="width:100px;">Title</td>
                                                                <td>
                                                                    <input class="form-control form-control-sm" v-model="selectedNode.text" @keyup.enter="applyChanges" />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="width:100px;">Icon</td>
                                                                <td>
                                                                    <input class="form-control form-control-sm" v-model="selectedNode.icon" @keyup.enter="applyChanges" />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="width:100px;">Component</td>
                                                                <td>
                                                                    <input class="form-control form-control-sm" v-model="selectedNode.data.component" @keyup.enter="applyChanges"
                                                                           :disabled="selectedNode.text.startsWith('---')" />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="width:100px;">&nbsp;</td>
                                                                <td class="text-secondary fs-d9 ps-1 pt-3">Comma separated items for <span class="fw-bold">Actions</span> and <span class="fw-bold">Roles</span></td>
                                                            </tr>
                                                            <tr>
                                                                <td style="width:100px;">Actions</td>
                                                                <td>
                                                                    <input class="form-control form-control-sm" v-model="selectedNode.data.actions" @keyup.enter="applyChanges" />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="width:100px;">Roles</td>
                                                                <td>
                                                                    <input class="form-control form-control-sm" v-model="selectedNode.data.roles" @keyup.enter="applyChanges" />
                                                                </td>
                                                            </tr>
                                                        </tbody>
                                                    </table>
                                                    <hr />
                                                    <button class="btn btn-outline-primary m-3" @click="applyChanges">Apply Changes</button>

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
        </div>
    </div>
</template>

<script>
    shared.setAppTitle("Navigation Management ");
    shared.setAppSubTitle(getQueryString("app"));

    let _this = { cid: "", c: null, inputs: {}, appName: getQueryString("app"), fullConf: {}, d: {}, theTree: null, selectedNode: null };
    
    export default {
        methods: {
            removeMenuItem() {
                if (_this.c.selectedNode !== null) {
                    _this.theTree.jstree(true).delete_node(_this.c.selectedNode);
                }
            },
            applyChanges() {
                let n = _this.theTree.jstree(true).get_node(_this.c.selectedNode.id);
                _this.theTree.jstree(true).set_text(_this.c.selectedNode.id, _this.c.selectedNode.text);
                _this.theTree.jstree(true).set_icon(_this.c.selectedNode.id, _this.c.selectedNode.icon);

                if (_this.c.selectedNode.text.startsWith('---')) _this.c.selectedNode.data.component = '';

                _.forEach(_this.c.d, function (i) {
                    if (i.id === _this.c.selectedNode.id) {
                        i.title = _this.c.selectedNode.text;
                        i.icon = _this.c.selectedNode.icon;
                        i.actions = _this.c.selectedNode.data.actions;
                        i.roles = _this.c.selectedNode.data.roles;
                        return;
                    }
                    _.forEach(i["items"], function (nItem) {
                        if (nItem.id === _this.c.selectedNode.id) {
                            nItem.title = _this.c.selectedNode.text;
                            nItem.icon = _this.c.selectedNode.icon;
                            nItem.component = _this.c.selectedNode.data.component;
                            nItem.actions = _this.c.selectedNode.data.actions;
                            nItem.roles = _this.c.selectedNode.data.roles;
                            return;
                        }
                    });
                });
                showSuccess("Changes applied.");
            },
            addNewCategory() {
                _this.theTree.jstree(true).create_node(null, { id: genUN("navCat"), text: "new category", icon: "fa-solid fa-folder", data: { actions: "", roles: "" }, type: "folder" }, "first");
            },
            addMenuItem() {
                if (_this.c.selectedNode !== null && fixNull(_this.c.selectedNode.parent, '') !== '') {
                    let n = null;
                    if (fixNull(_this.c.selectedNode.parent, '') === '#') {
                        n = _this.theTree.jstree(true).get_node(_this.c.selectedNode.id);
                    } else {
                        n = _this.theTree.jstree(true).get_node(_this.c.selectedNode.parent);
                    }
                    _this.theTree.jstree(true).create_node(n, { id: genUN("navItem"), text: "new menu item", icon: "fa-solid fa-file", data: { component: "", actions: "", roles: "" }, type: "file" }, "first");
                    _this.theTree.jstree(true).open_node(n);
                }
            },
            saveContent() {
                if (!_this.regulator.isValid()) return false;
                let newNav = [];
                var treeData = _this.theTree.jstree(true).get_json('#', { flat: false })
                _.forEach(treeData, function (c) {
                    let cat = {};
                    cat["title"] = c["text"];
                    cat["icon"] = c["icon"];
                    cat["actions"] = c["data"]["actions"];
                    cat["roles"] = c["data"]["roles"];
                    cat["items"] = [];
                    _.forEach(c["children"], function (i) {
                        let itm = {};
                        itm["title"] = i["text"];
                        itm["icon"] = i["icon"];
                        itm["component"] = i["data"]["component"];
                        itm["actions"] = i["data"]["actions"];
                        itm["roles"] = i["data"]["roles"];
                        cat["items"].push(itm);
                    })
                    newNav.push(cat);
                });
                _this.c.fullConf["navigation"] = newNav;
                rpcAEP("SaveFileContent", { "PathToWrite": 'workspace/client/' + _this.c.appName + '/app.json', "FileContent": JSON.stringify(_this.c.fullConf, null, 1) }, function (res) {
                    showSuccess("Theme navigation keys saved.");
                });
            },
            loadContent() {
                rpcAEP("GetFileContent", { "PathToRead": 'workspace/client/' + _this.c.appName + '/app.json' }, function (res) {
                    _this.c.fullConf = JSON.parse(R0R(res));
                    _this.c.d = _this.c.fullConf["navigation"];
                    _this.c.d = fixNull(_this.c.d, {});
                    _this.c.buildTree();
                });
            },
            buildTree() {
                $(document).ready(function () {
                    _this.theTree = $("#navTree:first");
                    _this.theTree.jstree(_this.c.getTreeConfig());
                    _.forEach(_this.c.d, function (i) {
                        i.id = fixNull(i.id, genUN("navCat"));
                        var par = _this.theTree.jstree(true).create_node(null, { id: i.id, text: i.title, icon: i.icon, data: i, type: "folder" }, "last");
                        let navItems = fixNull(i["items"], []);
                        _.forEach(navItems, function (nItem) {
                            nItem.id = fixNull(nItem.id, genUN("navItem"));
                            _this.theTree.jstree(true).create_node(par, { id: nItem.id, text: nItem.title, icon: nItem.icon, data: nItem, type: "file" }, "last");
                        });
                        _this.theTree.jstree(true).open_node(par);
                    });
                    _this.c.attachSelectNode();
                });
            },
            attachSelectNode() {
                _this.theTree.on("select_node.jstree", function (evt, data) {
                    _this.c.selectedNode = _this.theTree.jstree(true).get_selected(true)[0];
                    _this.c.showNodeProps(_this.c.selectedNode);
                });
            },
            showNodeProps(selectedNode) {
                //alert(selectedNode.id);
            },
            getTreeConfig() {
                return {
                    core: { check_callback: true, },
                    plugins: ["types", "dnd"],
                    types: {
                        folder: { icon: "fa-solid fa-folder", valid_children: ["file"] },
                        file: { icon: "fa-solid fa-file text-info", valid_children: [] }
                    }
                };
            }
        },
        setup(props) {
            _this.cid = props['cid'];
            _this.inputs = shared["params_" + _this.cid];
        },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { initVueComponent(_this); _this.c.loadContent(); },
        props: { cid: String }
    }
</script>
