<template>
    <div class="card border-0 bg-transparent rounded-0 h-100">
        <div class="card-body bg-transparent fs-d8 p-0">
            <div class="h-100 w-100" data-flex-splitter-horizontal style="flex: auto; overflow: hidden;">
                <div class="h-100" style="min-width:200px;width:20%;overflow:hidden;">
                    <div class="card h-100 shadow-sm rounded-0 border-0">
                        <div class="card-header px-2 bg-warning-subtle host-toolbar">
                            <div class="hstack">
                                <span class="fw-bold">
                                    <i class="fa-solid fa-fw fa-file me-1"></i> <span>Saved JSON Calls</span>
                                </span>
                            </div>
                        </div>
                        <div class="card-body scrollable p-2">
                            <div v-for="f in storedCalls" class="text-hover-primary bg-hover-light pointer" @click="openFile(f)">{{f}}</div>
                        </div>
                    </div>
                </div>
                <div role="separator" tabindex="1" class="bg-warning-subtle" style="width:8px; min-width:8px; cursor: col-resize; background: linear-gradient(90deg, transparent 0%, rgba(0,0,0,0.02) 45%, rgba(0,0,0,0.06) 50%, rgba(0,0,0,0.02) 55%, transparent 100%);"></div>
                <div class="h-100" style="min-width:400px;width:calc(80% - 8px);overflow:hidden;">

                    <div class="h-100 w-100" data-flex-splitter-vertical style="flex: auto; overflow: hidden;">
                        <div style="min-height:200px;height:calc(50% - 4px);overflow:hidden;">
                            <div class="card h-100 shadow-sm rounded-0 border-0">
                                <div class="card-header px-2 bg-warning-subtle host-toolbar">
                                    <div class="hstack">
                                        <button class="btn btn-link text-decoration-none bg-hover-light host-toolbar-btn" @click="newApiCall" title="New">
                                            <i class="fa-solid fa-fw fa-file-alt"></i> New
                                        </button>
                                        <div class="vr mx-1"></div>
                                        <button class="btn btn-link text-decoration-none bg-hover-light host-toolbar-btn" @click="saveContent" title="Save">
                                            <i class="fa-solid fa-fw fa-save"></i> Save
                                        </button>
                                        <div class="vr mx-1"></div>
                                        <button class="btn btn-link text-decoration-none bg-hover-light host-toolbar-btn" @click="execJsonCall" title="Execute">
                                            <i class="fa-solid fa-fw fa-play"></i> Execute
                                        </button>
                                        <div class="vr mx-1" v-if="appSubTitle!==''"></div>
                                        <button class="btn btn-link text-decoration-none bg-hover-light host-toolbar-btn" @click="renameApiCall" v-if="appSubTitle!==''" title="Rename">
                                            <i class="fa-solid fa-fw fa-i-cursor"></i> Rename
                                        </button>
                                        <div class="p-0 ms-auto"></div>
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
                                <div class="card-body p-0">
                                    <div class="code-editor-container h-100" id="jsonResult"></div>
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

    let _this = { cid: "", c: null, storedCalls: [], jsonRequestEditor: null, jsonResultEditor: null, appSubTitle: "" };

    export default {
        methods: {
            deleteApiCall() {
                showConfirm({
                    title: "Delete", message1: "Are you sure you want to remove this item?", message2: _this.c.appSubTitle,
                    callback: function () {
                        rpcAEP("DeleteFileItem", { "FilePath": _this.c.turnToFullPath(_this.c.appSubTitle) }, function () {
                            _this.c.newApiCall();
                            _this.c.readSavedApiCalls();
                        });
                    }
                });
            },
            renameApiCall() {
                showPrompt({
                    title: "Rename", message1: "Enter a valid name to rename the Api Call file", message2: "Spaces and Wildcards are not allowed",
                    "retVal": _this.c.appSubTitle,
                    callback: function (ret) {
                        let fileName = ret.replace('.json', '');
                        rpcAEP("RenameFileItem", { "FilePath": _this.c.turnToFullPath(_this.c.appSubTitle), "NewFilePath": _this.c.turnToFullPath(fileName) }, function () {
                            _this.c.appSubTitle = fileName;
                            shared.setAppSubTitle(_this.c.appSubTitle);
                            _this.c.readSavedApiCalls();
                        });
                    }
                });
            },
            newApiCall() {
                _this.c.appSubTitle = "";
                _this.c.jsonRequestEditor.setValue("{}");
                shared.setAppSubTitle(_this.c.appSubTitle);
            },
            saveContent() {
                let fileContent = _this.c.jsonRequestEditor.getValue();
                if (fixNull(_this.c.appSubTitle, '') === '') {
                    showPrompt({
                        title: "Api Call FileName", message1: "Enter a valid name to store the Api Call body", message2: "Spaces and Wildcards are not allowed",
                        callback: function (ret) {
                            let fileName = ret.replace('.json', '');
                            _this.c.appSubTitle = fileName;
                            shared.setAppSubTitle(_this.c.appSubTitle);
                            _this.c.finalSave(_this.c.appSubTitle, fileContent, function () { _this.c.readSavedApiCalls(); });
                        }
                    });
                } else {
                    _this.c.finalSave(_this.c.appSubTitle, fileContent)
                }
            },
            finalSave(filePath, fileContennt,after) {
                rpcAEP("SaveFileContent", { "PathToWrite": _this.c.turnToFullPath(filePath), "FileContent": fileContennt }, function (res) {
                    showSuccess("Saved");
                    if (after) after();
                });
            },
            execJsonCall() {
                try {
                    let r = JSON.parse(_this.c.jsonRequestEditor.getValue());
                    rpc({
                        requests: r,
                        onDone: function (res) {
                            _this.c.jsonResultEditor.setValue(JSON.stringify(res, null, 4));
                        },
                        onFail: function (res) {
                            _this.c.jsonResultEditor.setValue(JSON.stringify(res, null, 4));
                        }
                    });
                } catch (ex) {
                    let error = { error: ex.message };
                    _this.c.jsonResultEditor.setValue(JSON.stringify(error, null, 4));
                }
            },
            openFile(f) {
                rpcAEP("GetFileContent", { "PathToRead": _this.c.turnToFullPath(f) }, function (res) {
                    _this.c.appSubTitle = f;
                    _this.c.jsonRequestEditor.setValue(R0R(res));
                    shared.setAppSubTitle(_this.c.appSubTitle);
                });
            },
            setupUi() {
                _this.c.jsonRequestEditor = ace.edit("jsonRequest", { theme: "ace/theme/cloud9_day", mode: "ace/mode/json", value: "{}" });
                _this.c.jsonResultEditor = ace.edit("jsonResult", { theme: "ace/theme/cloud9_day", mode: "ace/mode/json", value: "{}" });
            },
            readSavedApiCalls(after) {
                rpcAEP("GetStoredApiCalls", {}, function (res) {
                    _this.c.storedCalls = R0R(res);
                    if (after) after();
                });
            },
            initialLoad() {
                _this.c.readSavedApiCalls(function () {
                    _this.c.setupUi();
                });
            },
            turnToFullPath(fileName) {
                return 'workspace/apicalls/' + fileName + '.json';
            }
        },
        setup(props) { _this.cid = props['cid']; },
        data() {return _this;},
        created() { _this.c = this; },
        mounted() { _this.c.initialLoad(); },
        props: { cid: String }
    }

</script>
