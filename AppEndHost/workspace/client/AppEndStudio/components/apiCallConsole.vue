<template>
    <div class="card h-100 rounded rounded-2 rounded-bottom-0 rounded-end-0 bg-transparent border-0">
        <div class="card-body p-2">
            <div class="h-100 w-100" data-flex-splitter-horizontal style="flex: auto;">
                <div class="h-100" style="min-width:200px;width:20%;">
                    <div class="card h-100 shadow-sm">
                        <div class="card-header p-1">
                            <div class="input-group input-group-sm p-0">
                                <span class="input-group-text border-0">
                                    <i class="fa-solid fa-fw fa-file me-1"></i> <span>Saved JSON Calls</span>
                                </span>
                            </div>
                        </div>
                        <div class="card-body scrollable p-2">
                            <div v-for="f in storedCalls" class="fs-d9 text-hover-primary bg-hover-light pointer" @click="openFile(f)">{{f}}</div>
                        </div>
                    </div>
                </div>
                <div role="separator" tabindex="1" class="bg-light"></div>
                <div class="h-100" style="min-width:400px;width:80%;">

                    <div class="h-100 w-100" data-flex-splitter-vertical style="flex: auto;">
                        <div style="min-height:200px;height:49.5%;">
                            <div class="card h-100">
                                <div class="card-header p-1">
                                    <div class="input-group input-group-sm p-0">
                                        <div class="btn btn-sm btn-outline-primary border-0 pointer" @click="newApiCall">
                                            <i class="fa-solid fa-fw fa-file-alt"></i> New
                                        </div>
                                        <div class="input-group-text bg-transparent border-0">|</div>
                                        <div class="btn btn-sm btn-outline-primary border-0 pointer" @click="saveContent">
                                            <i class="fa-solid fa-fw fa-save"></i> Save
                                        </div>
                                        <div class="input-group-text bg-transparent border-0">|</div>
                                        <div class="btn btn-sm btn-outline-primary border-0" @click="execJsonCall">
                                            <i class="fa-solid fa-fw fa-play"></i> Execute
                                        </div>
                                        <input type="text" class="form-control bg-transparent border-0" disabled />
                                        <div class="btn btn-sm btn-outline-primary border-0" @click="renameApiCall" v-if="appSubTitle!==''">
                                            <i class="fa-solid fa-fw fa-times"></i> Rename
                                        </div>
                                        <div class="btn btn-sm btn-outline-danger border-0" @click="deleteApiCall" v-if="appSubTitle!==''">
                                            <i class="fa-solid fa-fw fa-times"></i> Delete
                                        </div>
                                    </div>
                                </div>
                                <div class="card-body p-0">
                                    <div class="code-editor-container h-100" id="jsonRequest"></div>
                                </div>
                            </div>
                        </div>
                        <div role="separator" tabindex="1" class="bg-transparent" style="height:1%;"></div>
                        <div style="min-height:200px;height:49.50%;">
                            <div class="card h-100">
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
    shared.setAppTitle("API Call Console");

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
