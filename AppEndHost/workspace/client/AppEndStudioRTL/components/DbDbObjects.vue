<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-header p-2 bg-primary-subtle-light rounded-0 border-0">
            <div class="hstack gap-1">
                <select id="dataSources" class="form-select form-select-sm" style="max-width:200px;" v-model='rowsFilter.DbConfName' @change="readList">
                    <option value="DefaultRepo">DefaultRepo:MsSql</option>
                </select>
                <select id="dataSources" class="form-select form-select-sm" style="max-width:150px;" v-model='rowsFilter.ObjectType' @change="readList">
                    <option value="Table">Table</option>
                    <option value="View">View</option>
                    <option value="Procedure">Procedure</option>
                    <option value="TableFunction">TableFunction</option>
                    <option value="ScalarFunction">ScalarFunction</option>
                </select>
                <input type="text" class="form-control form-control-sm" style="max-width:150px;" @keyup.enter="readList" v-model='rowsFilter.Filter' />
                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="readList"><i class="fa-solid fa-search"></i></button>

                <div class="p-0 ms-auto"></div>

                <div class="vr" v-if="rowsFilter.SelectedObjectType==='ScalarFunction' || rowsFilter.SelectedObjectType==='TableFunction' || rowsFilter.SelectedObjectType==='Procedure'"></div>
                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="synchDbDirectMethods" v-if="rowsFilter.SelectedObjectType==='ScalarFunction' || rowsFilter.SelectedObjectType==='TableFunction' || rowsFilter.SelectedObjectType==='Procedure'">Synch DbDirect Methods</button>

                <div class="vr" v-if="rowsFilter.SelectedObjectType==='Table' || rowsFilter.SelectedObjectType==='View'"></div>
                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="generateHints" v-if="rowsFilter.SelectedObjectType==='Table' || rowsFilter.SelectedObjectType==='View'">Generate Hints</button>

                <div class="vr" v-if="rowsFilter.SelectedObjectType==='Table' || rowsFilter.SelectedObjectType==='View'"></div>
                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="buildUiForAll" v-if="rowsFilter.SelectedObjectType==='Table' || rowsFilter.SelectedObjectType==='View'">Build UIs</button>

                <div class="vr"></div>
                <div>
                    <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" data-bs-toggle="dropdown" aria-expanded="false"><i class="fa-solid fa-plus"></i></button>
                    <ul class="dropdown-menu fs-d8 shadow-sm border-2">
                        <li>
                            <a class="dropdown-item" :href="'?c=components/DbTableDesigner&cnn='+rowsFilter.DbConfName+'&o=__new__&template=Table'">
                                <i class="fa-solid fa-table"></i> <span class="fs-d9">Create Table</span>
                            </a>
                        </li>
                        <li><hr class="dropdown-divider"></li>
                        <li>
                            <a class="dropdown-item" :href="'?c=components/DbTableDesigner&cnn='+rowsFilter.DbConfName+'&o=__new__&template=TableFiles'">
                                <i class="fa-solid fa-table"></i> <span class="fs-d9">Create Files Table</span>
                            </a>
                        </li>
                        <li>
                            <a class="dropdown-item" :href="'?c=components/DbTableDesigner&cnn='+rowsFilter.DbConfName+'&o=__new__&template=TableFilesWithTitleAndNote'">
                                <i class="fa-solid fa-table"></i> <span class="fs-d9">Create Files Table With Titles and Notes</span>
                            </a>
                        </li>
                        <li>
                            <a class="dropdown-item" :href="'?c=components/DbTableDesigner&cnn='+rowsFilter.DbConfName+'&o=__new__&template=TreeTable'">
                                <i class="fa-solid fa-table"></i> <span class="fs-d9">Create Tree Table</span>
                            </a>
                        </li>
                        <li>
                            <a class="dropdown-item" :href="'?c=components/DbTableDesigner&cnn='+rowsFilter.DbConfName+'&o=__new__&template=TableJunction'">
                                <i class="fa-solid fa-table"></i> <span class="fs-d9">Create Junction Table</span>
                            </a>
                        </li>
                        <li><hr class="dropdown-divider"></li>
                        <li>
                            <a class="dropdown-item" :href="'?c=components/DbScriptEditor&cnn='+rowsFilter.DbConfName+'&o=__new__&template=View'">
                                <i class="fa-solid fa-file-code"></i> <span class="fs-d9">Create View</span>
                            </a>
                        </li>
                        <li>
                            <a class="dropdown-item" :href="'?c=components/DbScriptEditor&cnn='+rowsFilter.DbConfName+'&o=__new__&template=Procedure'">
                                <i class="fa-solid fa-file-code"></i> <span class="fs-d9">Create Procedure</span>
                            </a>
                        </li>
                        <li>
                            <a class="dropdown-item" :href="'?c=components/DbScriptEditor&cnn='+rowsFilter.DbConfName+'&o=__new__&template=ScalarFunction'">
                                <i class="fa-solid fa-file-code"></i> <span class="fs-d9">Create Scalar Function</span>
                            </a>
                        </li>
                        <li>
                            <a class="dropdown-item" :href="'?c=components/DbScriptEditor&cnn='+rowsFilter.DbConfName+'&o=__new__&template=TableFunction'">
                                <i class="fa-solid fa-file-code"></i> <span class="fs-d9">Create Table Function</span>
                            </a>
                        </li>
                    </ul>
                </div>
            </div>
        </div>
        <div class="card-body rounded rounded-2 border border-3 border-light fs-d8 p-0 bg-transparent scrollable">
            <table class="table table-sm table-hover w-100 ae-table m-0 bg-transparent">
                <thead>
                    <tr>
                        <th class="sticky-top ae-thead-th text-dark fw-bold" style="width:300px;white-space: nowrap; overflow: hidden;text-overflow: ellipsis;vertical-align:middle">DbObject</th>
                        <th class="sticky-top ae-thead-th text-dark fw-bold" style="width:34px;vertical-align:middle;text-align:center"></th>
                        <th class="sticky-top ae-thead-th text-dark fw-bold" style="width:75px;vertical-align:middle;text-align:center">DbDialog</th>
                        <th class="sticky-top ae-thead-th text-dark fw-bold"></th>
                        <th class="sticky-top ae-thead-th text-dark fw-bold text-center" style="width:180px;vertical-align:middle" v-if="rowsFilter.SelectedObjectType==='Table' || rowsFilter.SelectedObjectType==='View'">Server Objects</th>
                        <th class="sticky-top ae-thead-th text-dark fw-bold text-center" style="width:130px;vertical-align:middle"></th>
                        <th class="sticky-top ae-thead-th text-dark fw-bold text-center text-secondary" style="width:130px;vertical-align:middle">Changed On</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="i in d">
                        <td style="width:300px;white-space: nowrap; overflow: hidden;text-overflow: ellipsis;vertical-align:middle">
                            <a v-if="rowsFilter.SelectedObjectType==='Table'" :href="'?c=components/DbTableDesigner&cnn='+rowsFilter.DbConfName+'&o='+i.ObjectName" class="p-1 text-secondary text-hover-success text-decoration-none" :data-ae-key="i.ObjectName">
                                <i class="fa-solid fa-fw fa-edit me-1"></i>
                                <span class="fb objectname">{{i.ObjectName}}</span>
                            </a>
                            <a v-if="rowsFilter.SelectedObjectType!=='Table'" :href="'?c=components/DbScriptEditor&cnn='+rowsFilter.DbConfName+'&o='+i.ObjectName" class="p-1 text-secondary text-hover-success text-decoration-none" :data-ae-key="i.ObjectName">
                                <i class="fa-solid fa-fw fa-edit"></i> <span class="fb objectname">{{i.ObjectName}}</span>
                            </a>
                        </td>
                        <td style="width:34px;vertical-align:middle;text-align:center">
                            <i class="fa-solid fa-fw text-primary fa-spinner fa-spin me-1" v-if="shared.fixNull(i.proggressStatus,'')==='inproggress'"></i>
                            <i class="fa-solid fa-fw text-success fa-check me-1" v-if="shared.fixNull(i.proggressStatus,'')==='ok'"></i>
                            <i class="fa-solid fa-fw text-danger fa-bug pointer me-1" v-if="shared.fixNull(i.proggressStatus,'')==='error'" @click="showErrors(i.ObjectName)"></i>
                        </td>
                        <td style="vertical-align:middle;text-align:center">
                            <a :href="'?c=components/DbDialogDesigner&cnn='+rowsFilter.DbConfName+'&o='+i.ObjectName"
                               v-if="i.HasServerObjects===true" class="text-primary hover-success pointer text-decoration-none" :data-ae-key="i.ObjectName">
                                <i class="fa-solid fa-fw fa-puzzle-piece"></i> Change
                            </a>
                            <!--<a v-for="cc in i.ClientComponents" target="_blank" class="text-hover-success text-decoration-none"
                               :href="'?c=/a.Components/'+cc.replace('.vue','')">
                                <i class="fa-solid fa-up-right-from-square me-1"></i><span>{{cc.replace(rowsFilter.DbConfName+'_','').replace(i.ObjectName+'_','').replace('.vue','')}}</span>
                            </a>-->
                        </td>
                        <td></td>
                        <td style="width:180px;vertical-align:middle" class="text-center" v-if="rowsFilter.SelectedObjectType==='Table' || rowsFilter.SelectedObjectType==='View'">
                            <span class="text-danger hover-primary pointer" v-if="i.HasServerObjects===true"
                                  @click="removeServerObjects(i.ObjectName)">
                                <i class="fa-solid fa-fw fa-eraser"></i><span>Remove</span>
                            </span>
                            <span v-else class="text-success hover-primary text-start pointer" @click="createServerObjects(i.ObjectName)">
                                <i class="fa-solid fa-fw fa-magic pointer"></i><span>Create</span>
                            </span>
                        </td>
                        <td style="width:130px;vertical-align:middle" class="text-center">
                            <span class="text-secondary hover-primary pointer mx-1" @click="renameDbObject(i.ObjectName)">
                                <i class="fa-solid fa-fw fa-file-signature"></i><span>Rename</span>
                            </span>
                            <span class="text-secondary hover-primary pointer mx-1" @click="dropDbObject(i.ObjectName)">
                                <i class="fa-solid fa-fw fa-trash"></i><span>Drop</span>
                            </span>
                        </td>
                        <td style="width:130px;vertical-align:middle" class="text-center text-secondary">
                            <div class="fs-d8">
                                {{shared.formatDateTime(i.LastWriteTime)}}
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>

        </div>
    </div>
</template>

<script>
    shared.setAppTitle("$auto$");
    let _this = { cid: "", c: null, d: [], rowsFilter: {} };
    _this.rowsFilter = { "DbConfName": "DefaultRepo", "ObjectType": "Table", "SelectedObjectType": "Table", "Filter": "" };
    export default {
        methods: {
            showErrors(k) {
                let ind = _.findIndex(_this.c.d, (e) => { return e.ObjectName === k; }, 0);
                showJson(_this.c.d[ind]['errors']);
            },
            synchDbDirectMethods() {
                rpcAEP("SynchDbDirectMethods", { "DbConfName": _this.c.rowsFilter.DbConfName }, function (res) {
                    res = R0R(res);
                });
            },
            buildUiForAll() {
                shared.showConfirm({
                    title: "Build UI", message1: "Are you sure you want to build UI components for all objects? existing components will override!!!", message2: "",
                    callback: function () {

                        _.forEach(_this.c.d, function (dbd) {
                            if (dbd.HasServerObjects === true) {

                                dbd.proggressStatus = "inproggress";

                                rpcAEP("BuildUiForDbObject", { "DbConfName": _this.c.rowsFilter.DbConfName, "ObjectName": dbd.ObjectName }, function (res) {
                                    let errors = [];
                                    res = R0R(res);
                                    for (var key in res) {
                                        if (res.hasOwnProperty(key)) {
                                            errors.push({ "Key": key, "Error": res[key] });
                                        }
                                    }
                                    if (errors.length > 0) {
                                        dbd.proggressStatus = "error";
                                        dbd.errors = errors;
                                    }
                                    else dbd.proggressStatus = "ok";
                                });

                            }
                        });

                    }
                });
            },
            generateHints() {
                _.forEach(_this.c.d, function (dbd) {
                    dbd.proggressStatus = "inproggress";
                    rpcAEP("GenerateHintsForDbObject", { "DbConfName": _this.c.rowsFilter.DbConfName, "ObjectName": dbd.ObjectName }, function (res) {
                        let hints = [];
                        res = R0R(res);
                        for (var key in res) {
                            if (res.hasOwnProperty(key)) {
                                hints.push({ "Key": key, "Hint": res[key] });
                            }
                        }
                        if (hints.length > 0) {
                            dbd.proggressStatus = "error";
                            dbd.errors = hints;
                        }
                        else dbd.proggressStatus = "ok";
                    });
                });
            },
            createServerObjects(k) {
                rpcAEP("CreateServerObjects", { "DbConfName": _this.c.getSelectedDbCNN(), "ObjectType": _this.rowsFilter.ObjectType, "ObjectName": k }, function (res) {
                    showSuccess("ServerObjects created");
                    _this.c.readList();
                });
            },
            removeServerObjects(k) {
                shared.showConfirm({
                    title: "Remove ServerObjects", message1: "Are you sure you want to delete the ServerObjects for", message2: k,
                    callback: function () {
                        rpcAEP("RemoveServerObjects", { "DbConfName": _this.c.getSelectedDbCNN(), "ObjectType": _this.rowsFilter.ObjectType, "ObjectName": k }, function (res) {
                            if (R0R(res) === true) {
                                _this.c.readList();
                            } else {
                                showError("This entity prevented for removing or updating server objects!!!");
                            }
                        });
                    }
                });
            },
            renameDbObject(k) {
                shared.showPrompt({
                    title: "Rename DbObject", message1: "You are renaming an object", message2: k,
                    callback: function (retVal) {
                        if (k.toLowerCase() === retVal.toLowerCase()) return;
                        rpcAEP("RenameObject", { "DbConfName": _this.c.getSelectedDbCNN(), "ObjectType": _this.rowsFilter.ObjectType, "ObjectName_Old": k, "ObjectName_New": retVal }, function (res) {
                            setTimeout(function () { _this.c.readList(); }, 300);
                        });
                    }
                });
            },
            dropDbObject(k) {
                shared.showConfirm({
                    title: "Remove DbObject", message1: "Are you sure you want to remove this item?", message2: k,
                    callback: function () {
                        rpcAEP("DeleteObject", { "DbConfName": _this.c.getSelectedDbCNN(), "ObjectType": _this.rowsFilter.ObjectType, "ObjectName": k }, function (res) {
                            _this.c.readList();
                        });
                    }
                });
            },
            readList() {
                _this.c.rowsFilter.SelectedObjectType = _this.c.rowsFilter.ObjectType;
                rpcAEP("GetDbObjectsStack", { "DbConfName": _this.c.rowsFilter.DbConfName, "ObjectType": _this.c.rowsFilter.ObjectType, "Filter": _this.c.rowsFilter.Filter }, function (res) {
                    _this.c.d = R0R(res);
                });
            },
            getSelectedDbCNN() {
                return _this.rowsFilter.DbConfName.split(':')[0];
            }
        },
        setup(props) { _this.cid = props['cid']; },
        data() {
            return {
                d: _this.d,
                rowsFilter: _this.rowsFilter
            };
        },
        created() { _this.c = this; },
        mounted() { initVueComponent(_this); _this.c.readList(); },
        props: { cid: String }
    }

</script>
