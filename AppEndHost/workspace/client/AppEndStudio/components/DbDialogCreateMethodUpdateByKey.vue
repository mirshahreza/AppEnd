<template>
    <div class="col-48 h-100" id="formArea" data-ae-widget="inputsRegulator" data-ae-widget-options="{}">

        <div class="card h-100 border-0 bg-transparent">
            <div class="card-body p-3 pb-4 bg-primary-subtle-light fs-d8">

                <div class="mb-3">
                    <div class="fw-bold fst-italic">
                        Partial Updates
                    </div>
                    <div class="bg-success-subtle text-dark rounded-2 p-2 px-3">
                        Sometimes you want to change state of spesific columns of an entity via a separated form.
                        <br />
                        In a real scenarios, we may want these columns to be editable through a separate process with different access levels.
                    </div>
                </div>

                <div class="card my-2">
                    <div class="card-header p-1">
                        <div class="input-group input-group-sm">
                            <div class="input-group-text" style="width:200px;">Update API ...</div>
                            <input class="form-control form-control-sm" type="text" id="txtMethodName"
                                   @keyup="setNames" v-model="newMethod.MethodName"
                                   data-ae-validation-required="true" data-ae-validation-rule="^[^a-zA-Z0-9]?.{1,64}$"
                                   :disabled="selectedColumns.length<2" />
                            <div>&nbsp;&nbsp;</div>
                            <div class="btn btn-sm dropdown">
                                <div class="text-primary hover-success pointer text-center text-nowrap bg-transparent" id="addSimpleFieldDD" data-bs-toggle="dropdown" aria-expanded="false">
                                    <i class="fa-solid fa-plus"></i> <span>Add Column</span>
                                </div>
                                <ul class="dropdown-menu shadow-lg border-2" aria-labelledby="addSimpleFieldDD">
                                    <li v-for="i in allColumns">
                                        <a href="#" class="dropdown-item fs-d7 text-primary hover-success pointer" @click="addColumnToUpdateList"
                                           v-if="!shared.toSimpleArrayOf(selectedColumns,'Name').includes(i.Name)">
                                            <span class="col-name">{{i.Name}}</span>
                                            <span class="text-muted fs-d7"> ({{i.DbType}})</span>
                                        </a>
                                    </li>
                                </ul>
                            </div>
                        </div>
                    </div>
                    <div class="card-body p-2">
                        <div class="badge p-1" v-for="i in selectedColumns">
                            <span class="form-control form-control-sm">
                                <i class="fa-solid fa-times fa-fw text-muted-light text-hover-danger pointer" @click="removeColumnFromUpdateList"></i>
                                <span class="col-name ms-1 text-dark" v-if="shared.fixNull(i.Name,'')!==''">{{i.Name}}</span>
                            </span>
                        </div>
                        <div class="badge p-1" v-if="selectedColumns.length===0">
                            <span class="form-control form-control-sm border-white w-100">
                                <span class="col-name ms-1 text-dark">
                                    <span class="text-center fst-italic text-muted">Choose atleast one column...</span>
                                </span>
                            </span>
                        </div>
                    </div>
                    <div class="card-footer bg-white p-1">
                        <div class="input-group input-group-sm">
                            <div class="input-group-text" style="width:200px;">Final Update API Name</div>
                            <input class="form-control form-control-sm border-0" v-model="newMethod.MethodNameFinal" disabled />
                            <div class="input-group-text" style="width:240px;">
                                <div class="text-success" v-if="shared.ld().filter(inputs.oJson.DbQueries,function(i){return i.Name.toLowerCase()===newMethod.MethodNameFinal.toLowerCase();}).length>0">
                                    <i class="fa-solid fa-fw fa-check"></i> <span>Exist : Will be changed if needed</span>
                                </div>
                                <div class="text-danger" v-else>
                                    <i class="fa-solid fa-fw fa-times"></i> <span>Not Exist : Will be created</span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="card my-2">
                    <div class="card-header p-1">
                        <div class="input-group input-group-sm">
                            <div class="input-group-text" style="width:200px;">Read API ...</div>
                            <select class="form-select form-select-sm" v-model="newMethod.ReadApiName" @change="setNames" data-ae-validation-required="true">
                                <option value="_Auto_">Auto : AppEnd will create or use existing Read API based on internal namming policy</option>

                                <option v-for="i in shared.ld().filter(inputs.oJson.DbQueries,function(i){return i.Type.toLowerCase()==='ReadByKey'.toLowerCase();})"
                                        :value="i.Name">
                                    {{i.Name}}
                                </option>
                            </select>
                        </div>
                    </div>
                    <div class="card-body p-1">
                        <div class="input-group input-group-sm">
                            <div class="input-group-text" style="width:200px;">Final Read API Name</div>
                            <input class="form-control form-control-sm border-0" v-model="newMethod.ReadApiNameFinal" disabled />
                            <div class="input-group-text text-start" style="width:240px;">
                                <div class="text-success" v-if="shared.ld().filter(inputs.oJson.DbQueries,function(i){return i.Name.toLowerCase()===newMethod.ReadApiNameFinal.toLowerCase();}).length>0">
                                    <i class="fa-solid fa-fw fa-check"></i> <span>Exist : Will be used</span>
                                </div>
                                <div class="text-danger" v-else>
                                    <i class="fa-solid fa-fw fa-times"></i> <span>Not Exist : Will be created</span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="card my-2">
                    <div class="card-header p-1">
                        <div class="input-group input-group-sm">
                            <div class="input-group-text" style="width:200px;">UpdatedBy ...</div>
                            <select class="form-select form-select-sm" v-model="newMethod.ByColumnName" @change="setNames">
                                <optgroup label="AppEnd options">
                                    <option value="_Auto_">Auto : AppEnd will create or use existing column based on internal namming policy</option>
                                    <option value="_Ignore_">Ignore : Partial Update will not write ActorId in the record</option>
                                </optgroup>
                                <optgroup label="Existing Columns">
                                    <option v-for="i in shared.ld().filter(allColumns,function(i){return i.DbType.toLowerCase().indexOf('int')>-1})" :value="i.Name">{{i.Name}}</option>
                                </optgroup>
                            </select>
                        </div>
                    </div>
                    <div class="card-body p-1">
                        <div class="input-group input-group-sm">
                            <div class="input-group-text" style="width:200px;">Final UpdatedBy Name</div>
                            <input class="form-control form-control-sm border-0" v-model="newMethod.ByColumnNameFinal" disabled />
                            <div class="input-group-text text-start" style="width:240px;" v-if="newMethod.ByColumnName!=='_Ignore_'">
                                <div class="text-success" v-if="shared.ld().filter(inputs.oJson.Columns,function(i){return i.Name.toLowerCase()===newMethod.ByColumnNameFinal.toLowerCase();}).length>0">
                                    <i class="fa-solid fa-fw fa-check"></i> <span>Exist : Will be used</span>
                                </div>
                                <div class="text-danger" v-else>
                                    <i class="fa-solid fa-fw fa-times"></i> <span>Not Exist : Will be created</span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="card my-2">
                    <div class="card-header p-1">
                        <div class="input-group input-group-sm">
                            <div class="input-group-text" style="width:200px;">UpdatedOn ...</div>
                            <select class="form-select form-select-sm" v-model="newMethod.OnColumnName" @change="setNames">
                                <optgroup label="AppEnd options">
                                    <option value="_Auto_">Auto : AppEnd will create or use existing column based on internal namming policy</option>
                                    <option value="_Ignore_">Ignore : AppEnd will not write ActionDateTime in the record</option>
                                </optgroup>
                                <optgroup label="Existing Columns">
                                    <option v-for="i in shared.ld().filter(allColumns,function(i){return i.DbType==='DATETIME'})" :value="i.Name">{{i.Name}}</option>
                                </optgroup>
                            </select>
                        </div>
                    </div>
                    <div class="card-body p-1">
                        <div class="input-group input-group-sm">
                            <div class="input-group-text" style="width:200px;">Final UpdatedOn Name</div>
                            <input class="form-control form-control-sm border-0" v-model="newMethod.OnColumnNameFinal" disabled />
                            <div class="input-group-text text-start" style="width:240px;" v-if="newMethod.OnColumnName!=='_Ignore_'">
                                <div class="text-success" v-if="shared.ld().filter(inputs.oJson.Columns,function(i){return i.Name.toLowerCase()===newMethod.OnColumnNameFinal.toLowerCase();}).length>0">
                                    <i class="fa-solid fa-fw fa-check"></i> <span>Exist : Will be used</span>
                                </div>
                                <div class="text-danger" v-else>
                                    <i class="fa-solid fa-fw fa-times"></i> <span>Not Exist : Will be created</span>
                                </div>
                            </div>                            
                        </div>
                    </div>
                </div>

                <div class="card mt-2">
                    <div class="card-header p-1">
                        <div class="input-group input-group-sm">
                            <div class="input-group-text" style="width:200px;">History ...</div>
                            <select class="form-select form-select-sm" v-model="newMethod.HistoryTableName" @change="setNames">
                                <option value="_Ignore_">Ignore : AppEnd will not write history of changes</option>
                                <option value="_Auto_">Auto : AppEnd will create or use existing table based on internal namming policy</option>
                            </select>
                        </div>
                    </div>
                    <div class="card-body p-1">
                        <div class="input-group input-group-sm">
                            <div class="input-group-text" style="width:200px;">Final HistoryTable Name</div>
                            <input class="form-control form-control-sm border-0" v-model="newMethod.HistoryTableNameFinal" disabled />
                            <div class="input-group-text text-start" style="width:240px;" v-if="newMethod.HistoryTableName!=='_Ignore_'">
                                <div class="text-success" v-if="shared.ld().filter(dbObjects,function(i){return i.toLowerCase()===newMethod.HistoryTableNameFinal.toLowerCase();}).length>0">
                                    <i class="fa-solid fa-fw fa-check"></i> <span>Exist : Will be changed if needed</span>
                                </div>
                                <div class="text-danger" v-else>
                                    <i class="fa-solid fa-fw fa-times"></i> <span>Not Exist : Will be created</span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="card-footer p-0">
                <div class="container-fluid pt-2 pb-1">
                    <div class="row p-0">
                        <div class="col-36 px-2">
                            <button class="btn btn-sm btn-primary w-100" @click="ok" data-ae-key="ok">
                                <i class="fa-solid fa-check me-1"></i>
                                <span>Ok</span>
                            </button>
                        </div>
                        <div class="col-12 px-2">
                            <button class="btn btn-sm btn-secondary w-100" @click="cancel">
                                <i class="fa-solid fa-xmark me-1"></i>
                                <span>Cancel</span>
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>

    </div>
</template>

<script>
    let _this = { cid: "", c: null, inputs: {}, newMethod: {}, selectedColumns: [], allColumns: [], dbObjects:[] };
    _this.newMethod = {
        ReadApiName: "_Auto_",
        ReadApiNameFinal: "",
        "MethodName": "", "MethodNameFinal": "",
        "ByColumnName": "_Auto_", "ByColumnNameFinal": "",
        "OnColumnName": "_Auto_", "OnColumnNameFinal": "",
        "HistoryTableName": "_Ignore_", "HistoryTableNameFinal": ""
    };

    export default {
        methods: {
            removeColumnFromUpdateList(event) {
                let colName = $(event.target).parent().find(".col-name").text().trim();
                _.remove(_this.c.selectedColumns, function (i) { return i.Name == colName });
                _this.c.setNames();
            },
            addColumnToUpdateList(event) {
                _this.c.selectedColumns.push({ "Name": $(event.target).parent().find(".col-name").text().trim() });
                _this.c.setNames();
            },
            setNames() {

                if (_this.c.selectedColumns.length === 0) {
                    _this.c.newMethod.MethodName = "";
                    _this.c.newMethod.MethodNameFinal = "";
                    _this.c.newMethod.ByColumnNameFinal = "";
                    _this.c.newMethod.OnColumnNameFinal = "";
                    _this.c.newMethod.HistoryTableNameFinal = "";
                    return;
                }

                _this.c.newMethod.MethodName = _this.c.calcMethodName();
                _this.c.newMethod.MethodNameFinal = `${_this.c.newMethod.MethodName}Update`;
                _this.c.newMethod.ByColumnNameFinal = _this.c.calcByColumnNameFinal();
                _this.c.newMethod.OnColumnNameFinal = _this.c.calcOnColumnNameFinal();

                _this.c.newMethod.ReadApiNameFinal =(_this.c.newMethod.ReadApiName==='_Auto_' ? `${_this.c.newMethod.MethodName}ReadByKey` : _this.c.newMethod.ReadApiName);

                _this.c.newMethod.HistoryTableNameFinal = (_this.c.newMethod.HistoryTableName === '_Ignore_' ? "" : `${_this.c.inputs.BaseTableName}_${_this.c.newMethod.MethodName}_History`);
            },
            calcByColumnNameFinal() {
                if (_this.c.newMethod.ByColumnName === '_Ignore_') return "";
                if (_this.c.newMethod.ByColumnName === '_Auto_') return `${_this.c.newMethod.MethodName}UpdatedBy`;
                return _this.c.newMethod.ByColumnName;
            },
            calcOnColumnNameFinal() {
                if (_this.c.newMethod.OnColumnName === '_Ignore_') return "";
                if (_this.c.newMethod.OnColumnName === '_Auto_') return `${_this.c.newMethod.MethodName}UpdatedOn`;
                return _this.c.newMethod.OnColumnName;
            },
            calcMethodName() {
                if (_this.c.selectedColumns.length === 0) return "";
                if (_this.c.selectedColumns.length === 1) return _this.c.selectedColumns[0].Name;

                if (fixNull(_this.c.newMethod.MethodName, _this.c.selectedColumns[0].Name) === _this.c.selectedColumns[0].Name) return "$YorUpdateConceptName$";
                return _this.c.newMethod.MethodName;
            },
            localValidateForm() {
                if (isAreaValidById("formArea")) return false;
                if (_this.c.selectedColumns.length === 0) {
                    showError("You must select atleast one column to create new UpdateByKey API.");
                    return false;
                }
                if (_this.c.newMethod.MethodName.toString().indexOf("$") > -1) {
                    showError("You can not use $ for API name.\nUse a valid name for API.");
                    return false;
                }
                if (_this.c.newMethod.ReadApiNameFinal === '') {
                    showError("You must select a ReadByKey API to show current values.");
                    return false;
                }
                return true;
            },
            ok(e) {
                if (_this.c.localValidateForm()) {
                    _this.c.newMethod["SelectedColumns"] = _this.c.selectedColumns;
                    if (_this.c.inputs.callback) _this.c.inputs.callback(_this.c.newMethod);
                    _this.c.close();
                }
            },
            cancel() { _this.c.close(); },
            close() { shared.closeComponent(_this.cid); }
        },
        setup(props) {
            _this.cid = props['cid'];
            _this.inputs = shared["params_" + _this.cid];
            _this.allColumns = _this.inputs["oJson"]["Columns"];
            rpcAEP("GetDbObjects", { "DbConfName": _this.inputs["oJson"].DbConfName, "ObjectType": "Table", "Filter": null }, function (res) {
                _this.c.dbObjects = R0R(res);
            });
        },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { initVueComponent(_this); },
        props: { cid: String }
    }

</script>