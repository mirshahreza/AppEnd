<template>
    <div class="card h-100 rounded rounded-2 rounded-bottom-0 rounded-end-0 bg-transparent border-0">
        <div class="card-header p-2 bg-light-subtle rounded-end-0 border-0">

            <div class="input-group input-group-sm border-0 bg-transparent">

                <select id="dataSources" class="form-select" style="max-width:200px;" v-model='rowsFilter.DbConfName' @change="readList">
                    <option value="DefaultRepo">DefaultRepo:MsSql</option>
                </select>

                <span class="input-group-text border-0 bg-transparent fs-d4"> </span>

                <select id="dataSources" class="form-select" style="max-width:150px;" v-model='rowsFilter.ObjectType' @change="readList">
                    <option value="Table">Table</option>
                    <option value="View">View</option>
                    <option value="Procedure">Procedure</option>
                    <option value="TableFunction">TableFunction</option>
                    <option value="ScalarFunction">ScalarFunction</option>
                </select>

                <span class="input-group-text border-0 bg-transparent fs-d4"> </span>

                <input type="text" class="form-control" style="max-width:150px;" @keyup.enter="readList" v-model='rowsFilter.Filter' />

                <span class="input-group-text border-0 bg-transparent fs-d4"> </span>

                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="readList"><i class="fa-solid fa-search"></i></button>

                <input type="text" class="form-control form-control-sm border-0 rounded-0 bg-transparent" disabled />

                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="buildUiForAll">Build UIs</button>
                <div class="input-group-text border-0 bg-transparent"> | </div>
                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" data-bs-toggle="dropdown" aria-expanded="false"><i class="fa-solid fa-plus"></i></button>
                <ul class="dropdown-menu fs-d8 shadow-sm border-2">
                    <li>
                        <a class="dropdown-item" :href="'?c=components/dbTableDesigner&cnn='+rowsFilter.DbConfName+'&o=__new__&template=Table'">
                            <i class="fa-solid fa-table"></i> <span class="fs-d9">Create Table</span>
                        </a>
                    </li>
                    <li><hr class="dropdown-divider"></li>
                    <li>
                        <a class="dropdown-item" :href="'?c=components/dbTableDesigner&cnn='+rowsFilter.DbConfName+'&o=__new__&template=TableFiles'">
                            <i class="fa-solid fa-table"></i> <span class="fs-d9">Create Files Table</span>
                        </a>
                    </li>
                    <li>
                        <a class="dropdown-item" :href="'?c=components/dbTableDesigner&cnn='+rowsFilter.DbConfName+'&o=__new__&template=TableFilesWithTitleAndNote'">
                            <i class="fa-solid fa-table"></i> <span class="fs-d9">Create Files Table With Titles and Notes</span>
                        </a>
                    </li>
                    <li>
                        <a class="dropdown-item" :href="'?c=components/dbTableDesigner&cnn='+rowsFilter.DbConfName+'&o=__new__&template=TreeTable'">
                            <i class="fa-solid fa-table"></i> <span class="fs-d9">Create Tree Table</span>
                        </a>
                    </li>
                    <li>
                        <a class="dropdown-item" :href="'?c=components/dbTableDesigner&cnn='+rowsFilter.DbConfName+'&o=__new__&template=TableJunction'">
                            <i class="fa-solid fa-table"></i> <span class="fs-d9">Create Junction Table</span>
                        </a>
                    </li>
                    <li><hr class="dropdown-divider"></li>
                    <li>
                        <a class="dropdown-item" :href="'?c=components/dbScriptEditor&cnn='+rowsFilter.DbConfName+'&o=__new__&template=View'">
                            <i class="fa-solid fa-file-code"></i> <span class="fs-d9">Create View</span>
                        </a>
                    </li>
                    <li>
                        <a class="dropdown-item" :href="'?c=components/dbScriptEditor&cnn='+rowsFilter.DbConfName+'&o=__new__&template=Procedure'">
                            <i class="fa-solid fa-file-code"></i> <span class="fs-d9">Create Procedure</span>
                        </a>
                    </li>
                    <li>
                        <a class="dropdown-item" :href="'?c=components/dbScriptEditor&cnn='+rowsFilter.DbConfName+'&o=__new__&template=ScalarFunction'">
                            <i class="fa-solid fa-file-code"></i> <span class="fs-d9">Create Scalar Function</span>
                        </a>
                    </li>
                    <li>
                        <a class="dropdown-item" :href="'?c=components/dbScriptEditor&cnn='+rowsFilter.DbConfName+'&o=__new__&template=TableFunction'">
                            <i class="fa-solid fa-file-code"></i> <span class="fs-d9">Create Table Function</span>
                        </a>
                    </li>
                </ul>

            </div>


        </div>
        <div class="card-body p-2">
            <div class="card h-100 border-light bg-light bg-opacity-75 border-0">
                <div class="card-body rounded rounded-2 border border-3 border-light fs-d8 p-0 bg-transparent">

                    <table class="table table-sm table-hover w-100 ae-table m-0 bg-transparent">
                        <tbody>
                            <tr v-for="i in d">
                                <td style="width:300px;white-space: nowrap; overflow: hidden;text-overflow: ellipsis;vertical-align:middle">
                                    <a v-if="rowsFilter.SelectedObjectType==='Table'" :href="'?c=components/dbTableDesigner&cnn='+rowsFilter.DbConfName+'&o='+i.ObjectName" class="p-1 text-secondary text-hover-success text-decoration-none" :data-ae-key="i.ObjectName">
                                        <i class="fa-solid fa-fw fa-edit me-1"></i>
                                        <span class="fb objectname">{{i.ObjectName}}</span>
                                    </a>
                                    <a v-if="rowsFilter.SelectedObjectType!=='Table'" :href="'?c=components/dbScriptEditor&cnn='+rowsFilter.DbConfName+'&o='+i.ObjectName" class="p-1 text-secondary text-hover-success text-decoration-none" :data-ae-key="i.ObjectName">
                                        <i class="fa-solid fa-fw fa-edit"></i> <span class="fb objectname">{{i.ObjectName}}</span>
                                    </a>
                                </td>
                                <td style="width:34px;vertical-align:middle;text-align:center">
                                    <i class="fa-solid fa-fw text-primary fa-spinner fa-spin me-1" v-if="shared.fixNull(i.proggressStatus,'')==='inproggress'"></i>
                                    <i class="fa-solid fa-fw text-success fa-check me-1" v-if="shared.fixNull(i.proggressStatus,'')==='ok'"></i>
                                    <i class="fa-solid fa-fw text-danger fa-bug pointer me-1" v-if="shared.fixNull(i.proggressStatus,'')==='error'" @click="showErrors(i.ObjectName)"></i>
                                </td>
                                <td style="vertical-align:middle">
                                    <a :href="'?c=components/dbDialogDesigner&cnn='+rowsFilter.DbConfName+'&o='+i.ObjectName"
                                       v-if="i.HasServerObjects===true" class="text-primary hover-success pointer me-4 text-decoration-none" :data-ae-key="i.ObjectName">
                                        <i class="fa-solid fa-fw fa-puzzle-piece"></i><span class="fb">DbDialog</span>
                                    </a>
                                    <a v-for="cc in i.ClientComponents" target="_blank" class="text-success hover-primary pointer text-decoration-none"
                                       :href="'?c=/.DbComponents/'+cc.replace('.vue','')">
                                        <i class="fa-solid fa-fw fa-circle"></i><span class="fb">{{cc.replace(rowsFilter.DbConfName+'_','').replace(i.ObjectName+'_','').replace('.vue','')}}</span>
                                    </a>
                                </td>
                                <td style="width:180px;vertical-align:middle" class="text-center" v-if="rowsFilter.SelectedObjectType==='Table' || rowsFilter.SelectedObjectType==='View'">
                                    <span class="text-danger hover-primary pointer" v-if="i.HasServerObjects===true"
                                          @click="removeServerObjects(i.ObjectName)">
                                        <i class="fa-solid fa-fw fa-eraser"></i><span class="fb">Remove ServerObjects</span>
                                    </span>
                                    <span v-else class="text-success hover-primary text-start pointer" @click="createServerObjects(i.ObjectName)">
                                        <i class="fa-solid fa-fw fa-magic pointer"></i><span class="fb">Create ServerObjects</span>
                                    </span>
                                </td>
                                <td style="width:130px;vertical-align:middle" class="text-end">
                                    <span class="text-secondary hover-primary pointer mx-1" @click="renameDbObject(i.ObjectName)">
                                        <i class="fa-solid fa-fw fa-file-signature"></i><span class="fb">Rename</span>
                                    </span>
                                    <span class="text-secondary hover-primary pointer mx-1" @click="dropDbObject(i.ObjectName)">
                                        <i class="fa-solid fa-fw fa-trash"></i><span class="fb">Drop</span>
                                    </span>
                                </td>
                                <td style="width:130px;vertical-align:middle" class="text-end text-secondary fs-d8">
                                    {{shared.formatDateTime(i.LastWriteTime)}}
                                </td>
                            </tr>
                        </tbody>
                    </table>

                </div>
            </div>

        </div>
    </div>
</template>

<script>
    shared.setAppTitle("Db Objects");
    let _this = { cid: "", c: null, d: [], rowsFilter: {} };
    _this.rowsFilter = { "DbConfName": "DefaultRepo", "ObjectType": "Table", "SelectedObjectType": "Table", "Filter": "" };
    export default {
        methods: {
            showErrors(k) {
                let ind = _.findIndex(_this.c.d, (e) => { return e.ObjectName === k; }, 0);
                showJson(_this.c.d[ind]['errors']);
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
                            if(R0R(res)===true){
                                _this.c.readList();
                            }else{
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
        setup(props) {
            _this.cid = props['cid'];
        },
        data() {
            return {
                d: _this.d,
                rowsFilter: _this.rowsFilter
            };
        },
        created() { _this.c = this; },
        mounted() { _this.c.readList(); },
        props: { cid: String }
    }

</script>
