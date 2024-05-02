<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-header p-2 bg-success-subtle rounded-0 border-0">
            <div class="hstack gap-1">
                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="buildUi" :disabled="shared.fixNull(oJson.PreventBuildUI,false)===true">
                    <i class="fa-solid fa-fw fa-building-shield"></i> <span>Build UI</span>
                </button>
                <div class="p-0 ms-auto"></div>
                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="openMoreInfoEditor">
                    <i class="fa-solid fa-fw fa-tags"></i> <span>MoreInfo</span>
                </button>
                <div class="vr"></div>
                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="syncDbDialog">
                    <i class="fa-solid fa-fw fa-sync"></i> <span>Sync Columns</span>
                </button>

            </div>
        </div>
        <div class="card-body p-2">
            <div class="h-100 w-100" data-flex-splitter-horizontal style="flex: auto;">
                <div class="h-100" style="min-width:300px;width:60%;">
                    <div class="card h-100 shadow-sm">
                        <div class="card-header p-1">
                            <div class="input-group input-group-sm p-0 mx-0">
                                <span class="input-group-text border-0 rounded-0 text-primary pointer" title="Columns Ordering" @click="openColumnsOrdering">
                                    <i class="fa-solid fa-fw fa-table-columns"></i>
                                    <span>Columns Order</span>
                                </span>
                                <span class="input-group-text  border-0 rounded-0 fw-bold fs-d8" v-for="col in shared.ld().filter(oJson.Columns,function(i){return i.IsPrimaryKey===true;})">
                                    <i class="fa-solid fa-fw fa-key fs-d9"></i> {{col.Name}}
                                </span>
                                <input type="text" class="form-control form-control-sm border-0 rounded-0 bg-light" disabled />

                                <button class="btn btn-sm btn-link text-decoration-none" @click="openRelationEditor">
                                    <i class="fa-solid fa-fw fa-sitemap"></i> <span>Details</span> ({{shared.fixNull(oJson.Relations,[]).length}})
                                </button>

                            </div>
                        </div>
                        <div class="card-header fb p-1 fs-d8 bg-body-secondary">
                            <div class="row">

                                <div class="col">
                                    <div>
                                        <span class="text-primary ltr text-start fs-d9 pointer hover-success" @click="openLogicalFkEditor">
                                            <i class="fa-solid fa-fw fa-hand-pointer"></i> <span>Reference Columns (+)</span>
                                        </span>
                                    </div>
                                    <div class="card border-0">
                                        <div class="card-body bg-body-tertiary p-1 pb-0">
                                            <span class="badge bg-success-subtle text-success-emphasis p-2 me-1 mb-1 pointer text-hover-primary" @click="openFkLookupEditor"
                                                  v-for="col in shared.ld().filter(oJson.Columns,function(i){return shared.fixNull(i.Fk,'')!=='';})">
                                                <i class="fa-solid fa-fw fa-check text-success" v-if="shared.fixNull(col.Fk.Lookup,'')!==''"></i>
                                                <i class="fa-solid fa-fw fa-minus text-danger" v-else></i>
                                                {{col.Name}}
                                                <i class="fa-solid fa-fw fa-times text-muted text-hover-danger pointer" @click="removeLogicalFk"></i>
                                            </span>
                                            <span class="badge p-2 me-1 mb-1 fst-italic text-muted"
                                                  v-if="shared.ld().filter(oJson.Columns,function(i){return shared.fixNull(i.Fk,'')!=='';}).length===0">
                                                nothing
                                            </span>
                                        </div>
                                    </div>
                                </div>
                                <div class="col">
                                    <div>
                                        <span class="text-primary ltr text-start fs-d9 pointer hover-success" @click="openHumanIdsEditor">
                                            <i class="fa-solid fa-fw fa-check-double text-danger"
                                               v-if="shared.ld().filter(oJson.Columns,function(i){return i.IsHumanId===true;}).length===0"></i>
                                            <i class="fa-solid fa-fw fa-check-double text-success"
                                               v-if="shared.ld().filter(oJson.Columns,function(i){return i.IsHumanId===true;}).length!==0"></i>
                                            <span>HumanId Columns (+/-)</span>
                                        </span>
                                    </div>
                                    <div class="card border-0">
                                        <div class="card-body bg-body-tertiary p-1 pb-0">
                                            <span class="badge bg-success-subtle text-success-emphasis p-2 me-1 mb-1"
                                                  v-for="col in shared.ld().filter(oJson.Columns,function(i){return i.IsHumanId===true;})">
                                                {{col.Name}}
                                            </span>
                                            <span class="badge p-2 me-1 mb-1 fst-italic text-muted"
                                                  v-if="shared.ld().filter(oJson.Columns,function(i){return i.IsHumanId===true;}).length===0">
                                                nothing
                                            </span>
                                        </div>
                                    </div>
                                </div>
                                <div class="col">
                                    <div>
                                        <span class="text-primary ltr text-start fs-d9 pointer hover-success" @click="openSortableEditor">
                                            <i class="fa-solid fa-fw fa-check-double text-danger"
                                               v-if="shared.ld().filter(oJson.Columns,function(i){return i.IsSortable===true;}).length===0"></i>
                                            <i class="fa-solid fa-fw fa-check-double text-success"
                                               v-if="shared.ld().filter(oJson.Columns,function(i){return i.IsSortable===true;}).length!==0"></i>
                                            <span>Sortable Columns (+/-)</span>
                                        </span>
                                    </div>
                                    <div class="card border-0">
                                        <div class="card-body bg-body-tertiary p-1 pb-0">
                                            <span class="badge bg-success-subtle text-success-emphasis p-2 me-1 mb-1"
                                                  v-for="col in shared.ld().filter(oJson.Columns,function(i){return i.IsSortable===true;})">
                                                {{col.Name}}
                                            </span>
                                            <span class="badge p-2 me-1 mb-1 fst-italic text-muted"
                                                  v-if="shared.ld().filter(oJson.Columns,function(i){return i.IsSortable===true;}).length===0">
                                                nothing
                                            </span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="card-body scrollable p-2">
                            <div v-for="upG in changeStateGroups">
                                <span class="fs-d6 text-muted">ChangeStateGroup : </span><span class="badge text-bg-light fs-d7 ms-2 mt-2">{{upG.Name}}</span>
                                <div class="card border-0">
                                    <div class="card-body bg-light p-2 pb-0 rounded rounded-3">
                                        <div v-for="uiGroup in upG['Groups']" class="mb-1">
                                            <span class="fs-d6 text-muted">UiGroup : </span><span class="fs-d9 fw-bold text-secondary ms-2">{{uiGroup}}</span><br />
                                            <span class="badge bg-primary-subtle text-primary me-1 mb-1 pointer" @click="openColumnUiProps"
                                                  v-for="col in shared.ld().filter(oJson.Columns,function(cf){return cf['ChangeStateGroup']===upG.Name && shared.fixNull(cf['UiProps'],'')!=='' && shared.fixNull(cf['UiProps']['Group'],'')===uiGroup && cf.IsPrimaryKey!==true && !cf.Name.endsWith('_xs') && !cf.Name.endsWith('_FileMime') && !cf.Name.endsWith('_FileName') && !cf.Name.endsWith('_FileSize');})">
                                                <span class="data-ae-key fw-bold">{{col.Name}}</span>
                                            </span>

                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div role="separator" tabindex="1" class="bg-light" style="width:.5%;"></div>
                <div class="h-100" style="min-width:200px;width:39.5%;">
                    <div class="card h-100 shadow-sm">

                        <div class="card-body p-2">
                            <div class="">
                                <span class="text-secondary ltr text-start fs-d7 p-2 fb">
                                    <i class="fa-solid fa-fw fa-right-left"></i>
                                    Mapped Methods
                                    [
                                    <a class="p-1 px-1 text-primary text-hover-success pointer text-decoration-none" href="#" @click="createMethod">
                                        <i class="fa-solid fa-fw fa-wand-magic-sparkles"></i> <span>Create From Scratch</span>
                                    </a>
                                    <a class="p-1 px-1 text-primary text-hover-success pointer text-decoration-none" href="#" @click="createChangeStateByKey">
                                        <i class="fa-solid fa-fw fa-wand-magic-sparkles"></i> <span>Create Partial ChangeState</span>
                                    </a>
                                    ]
                                </span>
                            </div>
                            <div class="card bg-body-tertiary border-0">
                                <div class="card-body p-2">
                                    <div class="btn-group btn-group-sm me-1 mb-1 data-ae-parent" v-for="col in oJson.DbQueries">
                                        <button class="btn btn-outline-secondary p-0 px-1" @click="openQueryEditor">
                                            <span class="data-ae-key">{{col.Name}}</span>
                                        </button>
                                        <button type="button" class="btn btn-sm btn-outline-success dropdown-toggle dropdown-toggle-split  p-0 px-1" data-bs-toggle="dropdown" aria-expanded="false">
                                        </button>
                                        <ul class="dropdown-menu bg-white shadow-lg border-2" :data-ae-item-key="m" v-if="m!=='Delete'">
                                            <li v-if="oJson.ObjectType=='Table' || oJson.ObjectType=='View'">
                                                <a class="dropdown-item p-1 px-3 fs-d7 text-secondary hover-primary pointer" href="#" @click="openQueryEditor">
                                                    <i class="fa-solid fa-fw fa-right-left"></i> <span>Method IO</span>
                                                    <div class="fs-d9">Decide for Method inputs/outputs</div>
                                                </a>
                                            </li>
                                            <li>
                                                <a class="dropdown-item p-1 px-3 fs-d7 text-secondary hover-primary pointer" href="#" @click="openMethodSettings">
                                                    <i class="fa-solid fa-fw fa-cog"></i> <span>Method Settings</span>
                                                    <div class="fs-d9">Setting for method how to work : Access Rules / Caching / LogModel</div>
                                                </a>
                                            </li>
                                            <li v-if="oJson.ObjectType=='Table' || oJson.ObjectType=='View'"><hr class="dropdown-divider"></li>
                                            <li v-if="oJson.ObjectType=='Table' || oJson.ObjectType=='View'">
                                                <a class="dropdown-item p-1 px-3 fs-d7 text-secondary hover-primary pointer" href="#" @click="duplicateMethod">
                                                    <i class="fa-solid fa-fw fa-copy"></i> <span>Duplicate</span>
                                                    <div class="fs-d9">Duplicate Method with a new name</div>
                                                </a>
                                            </li>
                                            <li><hr class="dropdown-divider"></li>
                                            <li>
                                                <a class="dropdown-item p-1 px-3 fs-d7 text-secondary hover-primary pointer" href="#" @click="removeMethod">
                                                    <i class="fa-solid fa-fw fa-times"></i> <span>Remove</span>
                                                    <div class="fs-d9">Remove a method/api from the DbDialog</div>
                                                </a>
                                            </li>
                                        </ul>
                                    </div>
                                </div>
                            </div>

                            <div class="fs-d5">&nbsp;</div>

                            <div class="">
                                <span class="text-secondary ltr text-start fs-d7 p-2 fb">
                                    <i class="fa-solid fa-fw fa-right-left"></i>
                                    Not Mapped Methods
                                    [
                                    <a class="p-1 px-1 text-primary text-hover-success pointer text-decoration-none" href="#" @click="createNotMappedMethod">
                                        <i class="fa-solid fa-fw fa-wand-magic-sparkles"></i> <span>Create</span>
                                    </a>
                                    ]
                                </span>
                            </div>
                            <div class="card bg-body-tertiary border-0">
                                <div class="card-body p-2">
                                    <div class="btn-group btn-group-sm me-1 mb-1 data-ae-parent" v-for="m in notMappedMethods">
                                        <button class="btn btn-outline-secondary p-0 px-1" type="button" title="More Actions ...">
                                            <span class="data-ae-key">{{m}}</span>
                                        </button>
                                        <button type="button" class="btn btn-sm btn-outline-success dropdown-toggle dropdown-toggle-split p-0 px-1" data-bs-toggle="dropdown" aria-expanded="false">
                                        </button>
                                        <ul class="dropdown-menu bg-white shadow-lg border-2" :data-ae-item-key="m" v-if="m!=='Delete'">
                                            <li>
                                                <a class="dropdown-item p-1 px-3 fs-d7 text-secondary hover-primary pointer" href="#" @click="openMethodSettings">
                                                    <i class="fa-solid fa-fw fa-cog"></i> <span>Method Settings</span>
                                                    <div class="fs-d9">Setting for method how to work : Access Rules / Caching / LogModel</div>
                                                </a>
                                            </li>
                                            <li><hr class="dropdown-divider"></li>
                                            <li>
                                                <a class="dropdown-item p-1 px-3 fs-d7 text-secondary hover-primary pointer" href="#" @click="removeNotMappedMethod">
                                                    <i class="fa-solid fa-fw fa-times"></i> <span>Remove</span>
                                                    <div class="fs-d9">Remove a method/api from the csharp file</div>
                                                </a>
                                            </li>
                                        </ul>
                                    </div>
                                </div>
                            </div>

                            <div class="fs-d5">&nbsp;</div>

                            <button class="btn btn-sm btn-link text-decoration-none bg-hover-light fs-d8 p-1 py-0" @click="openClientUIsEditor">
                                <i class="fa-brands fa-fw fa-uikit"></i> <span>ClientUIs</span>
                            </button>

                            <div class="card bg-body-tertiary border-0">
                                <div class="card-body p-2">
                                    <div class="badge" v-for="cui in oJson.ClientUIs">
                                        <span class="text-primary text-hover-success" 
                                              v-if="cui.FileName.indexOf('ReadList')>-1 || cui.FileName.indexOf('ReadTreeList')>-1">
                                            <i class="fa-solid fa-fw fa-play"></i>
                                            <a class="text-hover-success text-decoration-none" :href="'?c=/a.DbComponents/'+cui.FileName" target="_blank">{{cui.FileName.replace(oJson.DbConfName+'_'+oJson.ObjectName+'_','')}}</a>
                                        </span>
                                        <span v-else class="text-bg-light">{{cui.FileName.replace(oJson.DbConfName+'_'+oJson.ObjectName+'_','')}}</span>
                                    </div>
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="card-footer bg-light-subtle rounded-0 p-0">
            <div class="input-group input-group-sm border-0">


                <button class="btn btn-sm text-success text-hover-danger border-0"
                        v-if="shared.fixNull(oJson.PreventBuildUI,false)===false" @click="switchPreventBuildUI">
                    <i class="fa-solid fa-fw fa-lock-open fs-d8"></i> <span class="fb fs-d8">Disable Build UI</span>
                </button>
                <button class="btn btn-sm text-secondary text-hover-success border-0"
                        v-if="shared.fixNull(oJson.PreventBuildUI,false)===true" @click="switchPreventBuildUI">
                    <i class="fa-solid fa-fw fa-lock fs-d8"></i> <span class="fb fs-d8">Enable Build UI</span>
                </button>

                <button class="btn btn-sm text-success text-hover-danger border-0"
                        v-if="shared.fixNull(oJson.PreventAlterServerObjects,false)===false" @click="switchPreventAlterServerObjects">
                    <i class="fa-solid fa-fw fa-lock-open fs-d8"></i> <span class="fb fs-d8">Disable RemoveServerObjects</span>
                </button>
                <button class="btn btn-sm text-secondary text-hover-success border-0"
                        v-if="shared.fixNull(oJson.PreventAlterServerObjects,false)===true" @click="switchPreventAlterServerObjects">
                    <i class="fa-solid fa-fw fa-lock fs-d8"></i> <span class="fb fs-d8">Enable RemoveServerObjects</span>
                </button>

                <input type="text" class="form-control form-control-sm border-0 rounded-0 bg-transparent" disabled />

                <a v-if="oJson.ObjectType==='Table'" :href="'?c=/a.PublicComponents/baseFileEditor&filePath='+filePath.replace('.dbdialog.json','.cs')"
                   class="btn btn-sm text-secondary text-hover-primary border-0">
                    <i class="fa-solid fa-fw fa-up-right-from-square fs-d8"></i><span class="fb fs-d8">CSharp</span>
                </a>
                <a v-if="oJson.ObjectType==='Table'" :href="'?c=/a.PublicComponents/baseFileEditor&filePath='+filePath"
                   class="btn btn-sm text-secondary text-hover-primary border-0">
                    <i class="fa-solid fa-fw fa-up-right-from-square fs-d8"></i><span class="fb fs-d8">JSON</span>
                </a>
                <a v-if="oJson.ObjectType!=='Table'" :href="'?c=/a.PublicComponents/dbScriptEditor&cnn='+oJson.DbConfName+'&o='+oJson.ObjectName"
                   class="btn btn-sm text-secondary text-hover-primary border-0">
                    <i class="fa-solid fa-fw fa-up-right-from-square fs-d8"></i><span class="fb fs-d8">Modify Db</span>
                </a>
                <a v-if="oJson.ObjectType==='Table'" :href="'?c=components/dbTableDesigner&cnn='+oJson.DbConfName+'&o='+oJson.ObjectName"
                   class="btn btn-sm text-secondary text-hover-primary border-0">
                    <i class="fa-solid fa-fw fa-up-right-from-square fs-d8"></i><span class="fb fs-d8">Modify Db</span>
                </a>

                <div class=""> &nbsp; </div>

                <div class="btn btn-sm text-danger hover-danger px-2 border-0 pointer fs-d8"
                     v-if="oJson.ObjectType==='Table'" @click="truncateTable">
                    <i class="fa-solid fa-fw fa-eraser"></i> <span class="fb fs-d8">Truncate</span>
                </div>

            </div>


        </div>
    </div>
</template>

<script>
    shared.setAppTitle("DbDialog");
    shared.setAppSubTitle(getQueryString("o"));

    let _this = { cid: "", c: null, oName: "", dbConfName: "", filePath: "", fileContent: {} };

    _this.oName = getQueryString("o");
    _this.dbConfName = getQueryString("cnn");
    _this.filePath = "workspace/server/" + getQueryString("cnn") + "." + getQueryString("o") + ".dbdialog.json";

    export default {
        methods: {
            openColumnsOrdering(){
                openComponent("components/dbDialogColumnsOrdering", {
                    title: "Columns Ordering", params: {
                        columns: _.cloneDeep(_this.c.oJson.Columns),
                        callback: function (ret) {
                            _this.c.oJson.Columns = ret;
                            _this.c.saveDbDialogChanges(function () {
                                _this.c.readFileContent();
                            });
                        }
                    }
                });
            },
            createChangeStateByKey() {
                openComponent("components/dbDialogCreateMethodChangeStateByKey", {
                    title: "Create new partial ChangeState API", modalSize: "modal-xl", params: {
                        BaseTableName: _this.oName,
                        oJson: _.cloneDeep(_this.c.oJson),
                        callback: function (ret) {
                            let params = {
                                "DbConfName": _this.dbConfName,
                                "ObjectName": _this.oName,
                                "ReadByKeyApiName": ret["ReadApiNameFinal"],
                                "PartialChangeStateApiName": ret["MethodNameFinal"],
                                "ColumnsToChangeState": shared.toSimpleArrayOf(ret["SelectedColumns"], 'Name'),
                                "ByColumnName": ret["ByColumnNameFinal"],
                                "OnColumnName": ret["OnColumnNameFinal"],
                                "HistoryTableName": ret["HistoryTableNameFinal"]
                            }
                            rpcAEP("CreateNewChangeStateByKey", params, function (res) {
                                _this.c.readFileContent();
                            });
                        }
                    }
                });
            },
            createMethod() {
                openComponent("components/dbDialogCreateMethodFromScratch", {
                    title: "Create new method from scratch", params: {
                        callback: function (ret) {
                            rpcAEP("CreateNewMethodQuery", { "DbConfName": _this.dbConfName, "ObjectName": _this.oName, "MethodType": ret["MethodType"], "MethodName": ret["MethodName"] }, function (res) {
                                _this.c.readFileContent();
                            });
                        }
                    }
                });
            },
            openMoreInfoEditor() {
                openComponent("components/dbDialogMoreInfoEditor", {
                    title: "MoreInfo Editor", resizable: false, draggable: false, params: {
                        oJson: _this.c.oJson,
                        callback: function (ret) {
                            _this.c.oJson = ret;
                            _this.c.saveDbDialogChanges(function () {
                                _this.c.readFileContent();
                            });
                        }
                    }
                });
            },
            openClientUIsEditor() {
                openComponent("components/dbDialogClientUIsEditor", {
                    title: "ClientUIs Editor", "modalSize": "modal-fullscreen", params: {
                        "ClientUIs": _.cloneDeep(_this.c.oJson.ClientUIs),
                        "DbQueries": _this.c.oJson.DbQueries,
                        callback: function (ret) {
                            _this.c.oJson.ClientUIs = ret;
                            _this.c.saveDbDialogChanges(function () {
                                _this.c.readFileContent();
                            });
                        }
                    }
                });
            },
            openRelationEditor() {
                openComponent("components/dbDialogRelationsEditor", {
                    title: "Relations Editor", "modalSize": "modal-fullscreen", params: {
                        "Relations": _.cloneDeep(_this.c.oJson.Relations),
                        "DbConfName": _this.c.oJson.DbConfName,
                        callback: function (ret) {
                            _this.c.oJson.Relations = ret;
                            _this.c.saveDbDialogChanges(function () {
                                _this.c.readFileContent();
                            });
                        }
                    }
                });
            },
            openQueryEditor(event) {
                let dbQ = $(event.target).parents(".data-ae-parent:first").find(".data-ae-key").text();
                let m = _.cloneDeep(_.find(_this.c.oJson.DbQueries, function (i) { return i.Name === dbQ }));
                openComponent("components/dbDialogMethodEditor", {
                    title: `MethodIO Designer : ${dbQ}`, "modalSize": "modal-fullscreen", params: {
                        "MethodBody": m,
                        "AllColumns": _this.c.oJson.Columns,
                        "Relations": _this.c.oJson.Relations,
                        callback: function (ret) {
                            let mIndex = _.findIndex(_this.c.oJson.DbQueries, function (i) { return i.Name === dbQ });
                            _this.c.oJson.DbQueries[mIndex] = ret;
                            _this.c.saveDbDialogChanges(function () {
                                _this.c.readFileContent();
                            });
                        }
                    }
                });

            },
            duplicateMethod(event) {
                let methodName = $(event.target).parents(".data-ae-parent:first").find(".data-ae-key").text();
                showPrompt({
                    title: "Method Duplication", message1: "Enter a name for new method", message2: "Spaces and Wildcards are not allowed",
                    callback: function (ret) {
                        rpcAEP("DuplicateMethodQuery", { "DbConfName": _this.dbConfName, "ObjectName": _this.oName, "MethodName": methodName, "MethodCopyName": ret }, function (res) {
                            showSuccess("Method created");
                            _this.c.readFileContent();
                        });
                    }
                });
            },
            removeMethod(event) {
                let methodName = $(event.target).parents(".data-ae-parent:first").find(".data-ae-key").text();
                showConfirm({
                    title: "Remove Method", message1: "Are you sure you want to delete this method?", message2: methodName,
                    callback: function () {
                        rpcAEP("RemoveMethodQuery", { "DbConfName": _this.dbConfName, "ObjectName": _this.oName, "MethodName": methodName }, function (res) {
                            showSuccess("Method removed");
                            _this.c.readFileContent();
                        });
                    }
                });
            },
            reCreateMethodJson(event) {
                let methodName = $(event.target).parents(".data-ae-parent:first").find(".data-ae-key").text();
                rpcAEP("ReCreateMethodJson", { "DbConfName": _this.dbConfName, "ObjectName": _this.oName, "ObjectType": _this.c.oJson.ObjectType, "MethodName": methodName }, function (res) {
                    showSuccess("Method recreated");
                    _this.c.readFileContent();
                });
            },
            openMethodSettings(event) {
                let methodName = $(event.target).parents(".data-ae-parent:first").find(".data-ae-key").text();
                openComponent("components/serverApiSettings", { title: `Api Settings Editor :: ${_this.dbConfName} . ${_this.oName} . ${methodName}`, params: { "ns": _this.dbConfName, "cs": _this.oName, "mn": methodName } });
            },
            removeNotMappedMethod(event) {
                let methodName = $(event.target).parents(".data-ae-parent:first").find(".data-ae-key").text();
                showConfirm({
                    title: "Remove Method", message1: "Are you sure you want to delete this method?", message2: methodName,
                    callback: function () {
                        rpcAEP("RemoveNotMappedMethod", { "DbConfName": _this.dbConfName, "ObjectName": _this.oName, "MethodName": methodName }, function (res) {
                            showSuccess("Method removed");
                            _this.c.readFileContent();
                        });
                    }
                });
            },
            createNotMappedMethod() {
                showPrompt({
                    title: "Method Name", message1: "Enter a name for new method", message2: "Spaces and Wildcards are not allowed",
                    callback: function (ret) {
                        rpcAEP("CreateNewNotMappedMethod", { "DbConfName": _this.dbConfName, "ObjectName": _this.oName, "MethodName": ret }, function (res) {
                            showSuccess("Method created");
                            _this.c.readFileContent();
                        });
                    }
                });
            },
            openHumanIdsEditor() {
                openComponent("components/dbHumanIdsEditor", {
                    title: "HumanIds Editor", "modalSize": "modal-xs", params: {
                        "Cols": _.cloneDeep(_this.c.oJson.Columns),
                        callback: function (ret) {
                            _.forEach(_this.c.oJson.Columns, function (i) {
                                delete i.IsHumanId;
                            });
                            _.forEach(ret, function (i) {
                                let c = _this.c.getColByName(i);
                                c.IsHumanId = true;
                            });
                            _this.c.saveDbDialogChanges(function () {
                                _this.c.readFileContent();
                            });
                        }
                    }
                });
            },
            openSortableEditor() {
                openComponent("components/dbSortableEditor", {
                    title: "Choose Sotable Columns", "modalSize": "modal-xs", params: {
                        "Cols": _.cloneDeep(_this.c.oJson.Columns),
                        callback: function (ret) {
                            _.forEach(_this.c.oJson.Columns, function (i) {
                                delete i.IsSortable;
                            });
                            _.forEach(ret, function (i) {
                                let c = _this.c.getColByName(i);
                                c.IsSortable = true;
                            });
                            _this.c.saveDbDialogChanges(function () {
                                _this.c.readFileContent();
                            });
                        }
                    }
                });
            },
            openFkLookupEditor(event) {
                let fieldName = $(event.target).text().trim();
                let c = _this.c.getColByName(fieldName);
                openComponent("components/dbFkLookupEditor", {
                    title: `Fk Lookup Editor [${fieldName}]`, "modalSize": "modal-xl", params: {
                        "Lookup": c.Fk.Lookup,
                        "ColName": fieldName,
                        callback: function (ret) {
                            c.Fk.Lookup = JSON.parse(ret);
                            _this.c.saveDbDialogChanges(function () {
                                _this.c.readFileContent();
                            });
                        }
                    }
                });
            },
            openLogicalFkEditor(event) {
                openComponent("components/dbFkEditor", {
                    title: "Fk Editor", params: {
                        "FkName": "",
                        "BaseTable": _this.oName,
                        "BaseColumn": "",
                        "TargetTable": "",
                        "TargetColumn": "",
                        "EnforceRelation": false,
                        callback: function (ret) {
                            let c = _this.c.getColByName(ret["BaseColumn"]);
                            if (fixNull(c["UiProps"], '') === '') c["UiProps"] = {};
                            c["UiProps"]["UiWidget"] = "Combo";
                            c["Fk"] = {};
                            c["Fk"]["FkName"] = _this.oName + '_' + ret["BaseColumn"] + '_' + ret["TargetTable"] + '_' + ret["TargetColumn"];
                            c["Fk"]["TargetTable"] = ret["TargetTable"];
                            c["Fk"]["TargetColumn"] = ret["TargetColumn"];
                            c["Fk"]["EnforceRelation"] = ret["EnforceRelation"];
                            _this.c.saveDbDialogChanges(function () {
                                _this.c.readFileContent();
                            });
                        }
                    }
                });
            },
            removeLogicalFk(event) {
                let fieldName = $(event.target).parent().text().trim();
                shared.showConfirm({
                    title: "Remove logical Fk", message1: "Are you sure you want to remove the logical Fk?", message2: fieldName,
                    callback: function () {
                        rpcAEP("RemoveLogicalFk", { "DbConfName": _this.dbConfName, "BaseTable": _this.oName, "BaseColumn": fieldName }, function (res) {
                            showSuccess("Fk removed");
                            _this.c.readFileContent();
                        });
                    }
                });
            },
            openColumnUiProps(event) {
                let fieldName = $(event.target).text();
                let c = _this.c.getColByName(fieldName);
                openComponent("components/dbDialogColUiProps", {
                    title: "UiProps Editor", resizable: false, draggable: false, params: {
                        "colProps": _.cloneDeep(c),
                        "uiProps": _.cloneDeep(c["UiProps"]),
                        callback: function (ret) {
                            c["DevNote"] = ret["colProps"]["DevNote"];
                            c["UiProps"] = ret["uiProps"];
                            _this.c.saveDbDialogChanges(function () {
                                _this.c.readFileContent();
                            });
                        }
                    }
                });
            },
            switchPreventAlterServerObjects() {
                _this.c.oJson.PreventAlterServerObjects = (fixNull(_this.c.oJson.PreventAlterServerObjects, false) === false ? true : false);
                _this.c.saveDbDialogChanges();
            },
            switchPreventBuildUI() {
                _this.c.oJson.PreventBuildUI = (fixNull(_this.c.oJson.PreventBuildUI, false) === false ? true : false);
                _this.c.saveDbDialogChanges();
            },
            getBeautifiedDbDialog() {
                return JSON.stringify(_this.c.oJson, null, '\t');
            },
            buildUi() {
                shared.showConfirm({
                    title: "Build UI", message1: "Are you sure you want to build all components for this object? existing components will override!!!", message2: _this.c.oJson.ObjectName,
                    callback: function () {
                        rpcAEP("BuildUiForDbObject", { "DbConfName": _this.c.oJson.DbConfName, "ObjectName": _this.c.oJson.ObjectName }, function (res) {
                            let errors = [];
                            res = R0R(res);
                            for (var key in res) {
                                if (res.hasOwnProperty(key)) {
                                    errors.push({ "Key": key, "Error": res[key] });
                                }
                            }
                            if (errors.length > 0) showJson(errors);
                            else showSuccess("Ui components built.");
                        });
                    }
                });
            },
            syncDbDialog() {
                shared.showConfirm({
                    title: "Sync DbDialog", message1: 'Columns section for DbDialog will be synched with the Database based on "Database First Policy", and deleted fields will removed from DbQueries', message2: _this.c.oJson.ObjectName,
                    callback: function () {
                        rpcAEP("SyncDbDialog", { "DbConfName": _this.c.oJson.DbConfName, "ObjectName": _this.c.oJson.ObjectName }, function (res) {
                            showSuccess("DbDialog changed");
                            _this.c.readFileContent();
                        });
                    }
                });
            },
            truncateTable() {
                shared.showConfirm({
                    title: "Truncate Table", message1: "Are you sure you want to truncate this table", message2: _this.c.oJson.ObjectName,
                    callback: function () {
                        rpcAEP("TruncateTable", { "DbConfName": _this.c.oJson.DbConfName, "TableName": _this.c.oJson.ObjectName }, function (res) {
                            showSuccess(_this.c.oJson.ObjectName + " truncated");
                        });
                    }
                });
            },
            extractChangeStateGroups(cols) {
                let arrChangeStateGroups = [{ "Name": "", "Groups": [""] }];
                _.each(cols, function (c) {
                    let itm = _.filter(arrChangeStateGroups, function (i) { return i.Name === c.ChangeStateGroup; });
                    if (itm.length === 0) arrChangeStateGroups.push({ "Name": c["ChangeStateGroup"], "Groups": [""] });
                })
                _.each(arrChangeStateGroups, function (upG) {
                    let colsForUG = _.filter(cols, function (c) { return fixNull(c.ChangeStateGroup, '') === upG.Name });
                    _.each(colsForUG, function (c) {
                        if (fixNull(c['UiProps'], '') !== '') {
                            let groupName = fixNull(c['UiProps']["Group"], '');
                            if (!upG["Groups"].includes(groupName)) upG["Groups"].push(groupName);
                        }
                    });
                });
                return arrChangeStateGroups;
            },
            readFileContent() {
                rpcAEP("ReadDbObjectBody", { "DbConfName": _this.dbConfName, "ObjectName": _this.oName }, function (res) {
                    _this.c.oJson = JSON.parse(R0R(res));
                    _this.c.changeStateGroups = _this.c.extractChangeStateGroups(_this.c.oJson.Columns);
                });
                rpcAEP("GetDbObjectNotMappedMethods", { "DbConfName": _this.dbConfName, "ObjectName": _this.oName }, function (res) {
                    _this.c.notMappedMethods = R0R(res);
                });
            },
            saveDbDialogChanges(callback) {
                rpcAEP("SaveDbObjectBody", { "DbConfName": _this.dbConfName, "ObjectName": _this.oName, "ObjectBody": _this.c.getBeautifiedDbDialog() }, function (res) {
                    if (callback) callback();
                });
            },
            getColByName(colName) {
                return _.find(_this.c.oJson.Columns, function (i) { return i.Name === colName });
            }
        },
        setup(props) { _this.cid = props['cid']; },
        data() {
            return { oJson: _this.fileContent, notMappedMethods: [], filePath: _this.filePath, changeStateGroups: [] };
        },
        created() { _this.c = this; },
        mounted() { _this.c.readFileContent(); },
        props: { cid: String }
    }

</script>
