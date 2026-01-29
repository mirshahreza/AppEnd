<template>
    <div class="card border-0 bg-transparent rounded-0 h-100">
        <div class="card-body bg-transparent fs-d8 p-0">
            <div class="h-100 w-100" data-flex-splitter-horizontal style="flex: auto; overflow: hidden;">
                <div class="h-100" style="min-width:275px;width:15%;overflow:hidden;">
                    <div class="card h-100 shadow-sm rounded-0 border-0">
                        <div class="card-header px-2 bg-warning-subtle host-toolbar">
                            <div class="hstack">
                                <span class="fw-bold">
                                    <i class="fa-solid fa-fw fa-exchange me-1"></i> <span>Saved Queries</span>
                                </span>
                                <div class="p-0 ms-auto"></div>
                                <button class="btn btn-link text-decoration-none bg-hover-light host-toolbar-btn" @click="newItem" title="New">
                                    <i class="fa-solid fa-fw fa-file-alt"></i> New
                                </button>
                            </div>
                        </div>
                        <div class="card-body scrollable p-2">
                            <div v-for="f in storedCalls" class="text-hover-primary bg-hover-light pointer" @click="openFile(f)">{{f}}</div>
                        </div>
                    </div>
                </div>
                <div role="separator" tabindex="1" class="bg-warning-subtle" style="width:8px; min-width:8px; cursor: col-resize; background: linear-gradient(90deg, transparent 0%, rgba(0,0,0,0.02) 45%, rgba(0,0,0,0.06) 50%, rgba(0,0,0,0.02) 55%, transparent 100%);"></div>
                <div class="h-100" style="min-width:400px;width:calc(85% - 8px);overflow:hidden;">

                    <div class="h-100 w-100" data-flex-splitter-vertical style="flex: auto; overflow: hidden;">
                        <div style="min-height:200px;height:calc(50% - 4px);overflow:hidden;">
                            <div class="card h-100 shadow-sm rounded-0 border-0">
                                <div class="card-header px-2 bg-warning-subtle host-toolbar">
                                    <div class="hstack">
                                        <button class="btn btn-link text-decoration-none bg-hover-light host-toolbar-btn" @click="execQuery" title="Execute">
                                            <i class="fa-solid fa-fw fa-play"></i> Execute
                                        </button>
                                        <div class="vr mx-1"></div>
                                        <div>Connection :</div>
                                        <select class="form-select form-select-sm pt-0" style="width:150px;" v-model="dbConfName">
                                            <option v-for="ds in dataSources" :value="ds.Name">{{ds.Name}}</option>
                                        </select>

                                        <div class="p-0 ms-auto"></div>
                                        <button class="btn btn-link text-decoration-none bg-hover-light host-toolbar-btn" @click="saveContent" title="Save">
                                            <i class="fa-solid fa-fw fa-save"></i> Save
                                        </button>
                                        <div class="vr mx-1"></div>
                                        <button class="btn btn-link text-decoration-none bg-hover-light host-toolbar-btn" @click="renameItem" v-if="fileName!==''" title="Rename">
                                            <i class="fa-solid fa-fw fa-i-cursor"></i> Rename
                                        </button>
                                        <div class="vr mx-1" v-if="fileName!==''"></div>
                                        <button class="btn btn-link text-secondary text-hover-danger text-decoration-none bg-hover-light host-toolbar-btn" @click="deleteApiCall" v-if="appSubTitle!==''" title="Delete">
                                            <i class="fa-solid fa-fw fa-trash"></i> Delete
                                        </button>
                                    </div>
                                </div>
                                <div class="card-body p-0">
                                    <div class="code-editor-container h-100" id="jsonRequest"></div>
                                </div>
                            </div>
                        </div>
                        <div role="separator" tabindex="1" class="bg-warning-subtle" style="height:8px; min-height:8px; cursor: row-resize; background: linear-gradient(180deg, transparent 0%, rgba(0,0,0,0.02) 45%, rgba(0,0,0,0.06) 50%, rgba(0,0,0,0.02) 55%, transparent 100%);"></div>
                        <div style="min-height:200px;height:calc(50% - 4px);overflow:hidden;">
                            <div class="card h-100 shadow-sm rounded-0 border-0">
                                <div class="card-body p-0 scrollable">
                                    <div v-html="execResult"></div>
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

    let _this = { cid: "", c: null, dataSources: [], storedCalls: [], requestEditor: null, fileName: "", execResult: "", dbConfName: "DefaultRepo" };

    export default {
        methods: {
            deleteApiCall() {
                showConfirm({
                    title: "Delete", message1: "Are you sure you want to remove this item?", message2: _this.c.fileName,
                    callback: function () {
                        rpcAEP("DeleteFileItem", { "FilePath": _this.c.turnToFullPath(_this.c.fileName) }, function () {
                            _this.c.newItem();
                            _this.c.readSavedItems();
                        });
                    }
                });
            },
            renameItem() {
                showPrompt({
                    title: "Rename", message1: "Enter a valid name to rename the Api Call file", message2: "Spaces and Wildcards are not allowed",
                    "retVal": _this.c.fileName,
                    callback: function (ret) {
                        let fileName = ret.replace('.json', '');
                        rpcAEP("RenameItem", { "ItemPath": _this.c.turnToFullPath(_this.c.fileName), "NewItemPath": _this.c.turnToFullPath(fileName) }, function () {
                            _this.c.fileName = fileName;
                            shared.setAppSubTitle(' / ' + _this.c.fileName);
                            _this.c.readSavedItems();
                        });
                    }
                });
            },
            newItem() {
                _this.c.fileName = "";
                _this.c.dbConfName = "DefaultRepo";
                _this.c.requestEditor.setValue("");
                shared.setAppSubTitle(' / ' + _this.c.fileName);
            },
            saveContent() {
                let query = _this.c.requestEditor.getValue();
                let fileContent = JSON.stringify({ "DbConfName": _this.c.dbConfName, "Query": query }, null, 4);
                if (fixNull(_this.c.fileName, '') === '') {
                    showPrompt({
                        title: "Query FileName", message1: "Enter a valid name to store the Query", message2: "Spaces and Wildcards are not allowed",
                        callback: function (ret) {
                            let fileName = ret.replace('.json', '');
                            _this.c.fileName = fileName;
                            shared.setAppSubTitle(' / ' + _this.c.fileName);
                            _this.c.finalSave(_this.c.fileName, fileContent, function () { _this.c.readSavedItems(); });
                        }
                    });
                } else {
                    _this.c.finalSave(_this.c.fileName, fileContent)
                }
            },
            finalSave(filePath, fileContennt,after) {
                rpcAEP("SaveFileContent", { "PathToWrite": _this.c.turnToFullPath(filePath), "FileContent": fileContennt }, function (res) {
                    showSuccess("Saved");
                    if (after) after();
                });
            },
            execQuery() {
                try {
                    // todo : q must be selected area or entier text if there is not selection
                    let q = _this.c.requestEditor.getValue();

                    rpcAEP("Exec", { "DbConfName": _this.c.dbConfName, "Query": q }, function (res) {
                        let finalResult = "";
                        let r = R0R(res);
                        let sep = "";

                        _.forEach(r, function (i) {
                            finalResult += sep + i;
                            sep = "<br />";
                        });

                        _this.c.execResult = finalResult.replaceAll('\r\n','<br />');
                    });
                } catch (ex) {
                    let error = { error: ex.message };
                    showJson(error);
                }
            },
            openFile(f) {
                rpcAEP("GetFileContent", { "PathToRead": _this.c.turnToFullPath(f) }, function (res) {
                    _this.c.fileName = f;
                    let jFile = JSON.parse(R0R(res));
                    _this.c.requestEditor.setValue(jFile["Query"]);
                    _this.c.dbConfName = jFile["DbConfName"];
                    shared.setAppSubTitle(' / ' + _this.c.fileName);
                });
            },
            setupUi() {
                _this.c.requestEditor = ace.edit("jsonRequest", { theme: "ace/theme/cloud9_day", mode: "ace/mode/sql", value: "" });
            },
            readSavedItems(after) {
                rpcAEP("GetStoredSqlQueries", {}, function (res) {
                    _this.c.storedCalls = R0R(res);
                    if (after) after();
                });
            },
            initialLoad() {
                rpcAEP("GetDataSources", {}, function (res) {
                    _this.c.dataSources = R0R(res);
                    _this.c.readSavedItems(function () {
                        _this.c.setupUi();
                    });
                });
            },
            turnToFullPath(fileName) {
                return 'workspace/sqlqueries/' + fileName + '.json';
            }
        },
        setup(props) { _this.cid = props['cid']; },
        data() {return _this;},
        created() { _this.c = this; },
        mounted() { _this.c.initialLoad(); },
        props: { cid: String }
    }

</script>
