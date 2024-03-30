<template>
    <div class="col-48 h-100">

        <div class="card h-100 border-0 bg-transparent">
            <div class="card-body p-3 pb-4 bg-transparent fs-d8">

                <div class="fs-d8"><span>Select columns</span></div>
                <div class="card">
                    <div class="card-body p-1">
                        <div class="badge p-2" v-for="i in selectedColumns">
                            <span class="form-control form-control-sm">
                                <i class="fa-solid fa-times fa-fw text-muted-light hover-danger pointer" @click="removeColumnFromUpdateList"></i>
                                <span class="col-name px-2 text-dark" v-if="shared.fixNull(i.Name,'')!==''">{{i.Name}}</span>
                            </span>
                        </div>
                        <div class="dropdown badge bg-primary-subtle p-2">
                            <div class="text-primary hover-success pointer text-center bg-transparent" id="addSimpleFieldDD" data-bs-toggle="dropdown" aria-expanded="false">
                                <i class="fa-solid fa-plus"></i>
                            </div>
                            <ul class="dropdown-menu shadow-lg border-2" aria-labelledby="addSimpleFieldDD">
                                <li v-for="i in allColumns">
                                    <a href="#" class="dropdown-item fs-d8 text-primary hover-success pointer" @click="addColumnToUpdateList"
                                       v-if="!shared.toSimpleArrayOf(selectedColumns,'Name').includes(i.Name)">
                                        <i class="fa-solid fa-plus fa-fw"></i>
                                        <span class="col-name">{{i.Name}}</span>
                                        <span class="text-muted fs-d7"> ({{i.DbType}})</span>
                                    </a>
                                </li>
                            </ul>
                        </div>
                    </div>
                </div>


                <div>&nbsp;</div>

                <div class="fs-d8"><span>Method name</span></div>
                <input class="form-control form-control-sm" type="text" id="txtMethodName" v-model="newMethod.MethodName"
                       data-ae-validation-required="true" data-ae-validation-rule="^[^a-zA-Z0-9]?.{1,64}$" />

                <div>&nbsp;</div>

                <fieldset>
                    <div class="fs-d8"><span>UpdatedBy [Column name policy]</span></div>
                    <select class="form-select form-select-sm" v-model="newMethod.ByColumnName">
                        <optgroup label="AppEnd options">
                            <option value="_Auto_">AppEnd will create or use existing column</option>
                            <option value="_Ignore_">Ignore</option>
                        </optgroup>
                        <optgroup label="Existing Columns">
                            <option v-for="i in shared.ld().filter(allColumns,function(i){return i.DbType==='NVARCHAR'})" :value="i.Name">{{i.Name}}</option>
                        </optgroup>
                    </select>

                    <div>&nbsp;</div>

                    <div class="fs-d8"><span>UpdatedOn [Column name policy]</span></div>
                    <select class="form-select form-select-sm" v-model="newMethod.OnColumnName">
                        <optgroup label="AppEnd options">
                            <option value="_Auto_">AppEnd will create or use existing column</option>
                            <option value="_Ignore_">Ignore</option>
                        </optgroup>
                        <optgroup label="Existing Columns">
                            <option v-for="i in shared.ld().filter(allColumns,function(i){return i.DbType==='DATETIME'})" :value="i.Name">{{i.Name}}</option>
                        </optgroup>
                    </select>
                </fieldset>

                <div>&nbsp;</div>

                <div class="fs-d8"><span>Log History/Version Table</span></div>
                <input class="form-control form-control-sm" v-model="newMethod.LogTableName" type="text" />
                <div class="text-muted fs-d7"><span>- Leave it empty if you don't want to log changes.</span></div>
                <div class="text-muted fs-d7"><span>- Write a table name. AppEnd will create it if it is not exists.</span></div>
                <div class="text-muted fs-d7"><span>- Or write <a href="#" @click="setLogTableNameAuto">$auto$</a> to generate log table name by AppEnd.</span></div>

            </div>
            <div class="card-footer p-3 bg-secondary-subtle bg-gradient border-0 rounded-0">
                <div class="row">
                    <div class="col-24">
                        <button class="btn btn-sm btn-secondary w-100 py-2" @click="cancel" data-ae-key="ok">
                            <i class="fa-solid fa-cancel"></i>
                            &nbsp;
                            <span>Cancel</span>
                        </button>
                    </div>
                    <div class="col-24">
                        <button class="btn btn-sm btn-primary w-100 py-2" @click="ok" data-ae-key="ok">
                            <i class="fa-solid fa-check"></i>
                            &nbsp;
                            <span>Ok</span>
                        </button>
                    </div>
                </div>
            </div>
        </div>

    </div>
</template>

<script>
    let _this = { cid: "", c: null, inputs: {}, newMethod: {}, selectedColumns: [], allColumns: [], regulator: null };
    _this.newMethod = { "MethodName": "", "ByColumnName": "_Auto_", "OnColumnName": "_Auto_", "LogTableName": "" };

    export default {
        methods: {
            removeColumnFromUpdateList(event) {
                let colName = $(event.target).parent().find(".col-name").text().trim();
                _.remove(_this.c.selectedColumns, function (i) { return i.Name == colName });
                _this.c.setMethodName();
            },
            addColumnToUpdateList(event) {
                _this.c.selectedColumns.push({ "Name": $(event.target).parent().find(".col-name").text().trim() });
                _this.c.setMethodName();
            },
            setMethodName() {
                _this.c.newMethod.MethodName = "";
                if (_this.c.selectedColumns.length === 0) return;
                if (_this.c.selectedColumns.length === 1) {
                    _this.c.newMethod.MethodName = _this.c.selectedColumns[0].Name + "Update";
                } else {
                    _this.c.newMethod.MethodName = "$YorUpdateConceptName$Call";
                }
                setTimeout(function () { $('#txtMethodName').keyup(); }, 200);
            },
            setLogTableNameAuto() {
                _this.c.newMethod.LogTableName = '$auto$';
            },
            localValidateForm() {
                if (!_this.regulator.isValid()) return false;
                if (_this.c.selectedColumns.length === 0) {
                    showError("You must select atleast one column for UpdateByKey Methods/APIs.");
                    return false;
                }
                if (_this.c.newMethod.MethodName.toString().indexOf("$") > -1) {
                    showError("You can not use $ for Method/API name.");
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
            _this.allColumns = _this.inputs["AllColumns"];
        },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { initVueComponent(_this); },
        props: { cid: String }
    }

</script>