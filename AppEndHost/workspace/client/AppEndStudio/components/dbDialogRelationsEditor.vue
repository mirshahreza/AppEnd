<template>
    <div class="col-48 h-100">

        <div class="card h-100 border-0 bg-transparent">
            <div class="card-header">
                <button class="btn btn-sm btn-success" @click="addRelation">
                    <i class="fa-solid fa-plus fa-fw"></i>
                    &nbsp;
                    <span>Add Relation</span>
                </button>
            </div>
            <div class="card-body p-3 pb-4 bg-transparent fs-d8">
                <div v-if="shared.fixNull(inputs.Relations,'')!==''">
                    <div class="card shadow-sm mb-1" v-for="i,j in inputs.Relations">
                        <div class="card-header bg-primary-subtle p-2 py-1">
                            <table class="w-100">
                                <tr>
                                    <td style="width:100px;"><span class="text-primary fw-bold">Relation Props</span></td>
                                    <td style="width:135px;">
                                        <select class="form-select form-select-sm" v-model="i.RelationType">
                                            <option value="ManyToMany">ManyToMany</option>
                                            <option value="OneToMany">OneToMany</option>
                                        </select>
                                    </td>
                                    <td><input class="form-control form-control-sm" v-model="i.RelationName" /></td>
                                    <td style="width:140px;">
                                        <select class="form-select form-select-sm" v-model="i.RelationUiWidget">
                                            <option value="CheckboxList" v-if="i.RelationType==='ManyToMany'">CheckboxList</option>
                                            <option value="AddableList" v-if="i.RelationType==='ManyToMany'">AddableList</option>
                                            <option value="Cards" v-if="i.RelationType==='OneToMany'">Cards</option>
                                            <option value="Grid" v-if="i.RelationType==='OneToMany'">Grid</option>
                                        </select>
                                    </td>
                                    <td style="width:100px;" class="ps-2">
                                        <div class="input-group input-group-sm">
                                            <span class="input-group-text">Min</span>
                                            <input class="form-control form-control-sm" v-model="i.MinN" />
                                        </div>
                                    </td>
                                    <td style="width:100px;" class="ps-2">
                                        <div class="input-group input-group-sm">
                                            <span class="input-group-text">Max</span>
                                            <input class="form-control form-control-sm" v-model="i.MaxN" />
                                        </div>
                                    </td>
                                    <td style="width:125px;" class="ps-2 pt-2">
                                        <div class="form-check">
                                            <input class="form-check-input" type="checkbox" :id="'chk_'+j" v-model="i.IsFileCentric" />
                                            <label class="form-check-label" :for="'chk_'+j">IsFileCentric</label>
                                        </div>
                                    </td>
                                    <td style="width:28px;" class="text-end">
                                        <div class="btn btn-close btn-sm text-hover-danger" :ae-data-index="j" @click="removeRelation"></div>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div class="card-body bg-success-subtle p-1">
                            <div class="row my-1">
                                <div class="col">
                                    <span class="text-secondary mx-2">RelationTable</span>
                                    <div class="input-group input-group-sm">
                                        <input class="form-control form-control-sm" v-model="i.RelationTable" />
                                        <div class="input-group-text pointer text-hover-primary" @click="pickTable(j)"><i class="fa-solid fa-search"></i></div>
                                    </div>
                                </div>
                                <div class="col">
                                    <span class="text-secondary mx-2">Relation<span class="fw-bold">Pk</span>Column</span>
                                    <div class="input-group input-group-sm">
                                        <input class="form-control form-control-sm" v-model="i.RelationPkColumn" />
                                        <div class="input-group-text pointer text-hover-primary" @click="pickPkColumnForRelationTable(j)"><i class="fa-solid fa-search"></i></div>
                                    </div>
                                </div>
                                <div class="col">
                                    <span class="text-secondary mx-2">Relation<span class="fw-bold">Fk</span>Column</span>
                                    <div class="input-group input-group-sm">
                                        <input class="form-control form-control-sm" v-model="i.RelationFkColumn" />
                                        <div class="input-group-text pointer text-hover-primary" @click="pickFkColumnForRelationTable(j)"><i class="fa-solid fa-search"></i></div>
                                    </div>
                                </div>
                                <div class="col" v-if="i.RelationType==='ManyToMany'">
                                    <span class="text-secondary mx-2">LinkingColumn</span>
                                    <div class="input-group input-group-sm">
                                        <input class="form-control form-control-sm" v-model="i.LinkingColumnInManyToMany" />
                                        <div class="input-group-text pointer text-hover-primary" @click="pickLinkingColumnOnRelationTable(j)"><i class="fa-solid fa-search"></i></div>
                                    </div>
                                </div>
                                <div class="col" v-if="i.RelationType==='ManyToMany'">
                                    <span class="text-secondary mx-2">LinkingTargetTable</span>
                                    <div class="input-group input-group-sm">
                                        <input class="form-control form-control-sm" v-model="i.LinkingTargetTable" />
                                        <div class="input-group-text pointer text-hover-primary" @click="pickLinkingTargetTable(j)"><i class="fa-solid fa-search"></i></div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="card-footer bg-light-subtle p-1">
                            <div class="row">
                                <div class="col-10 p-0 px-1 m-0">
                                    <span class="text-secondary mx-2">CreateQuery</span>
                                    <input class="form-control form-control-sm" v-model="i.CreateQuery" />
                                </div>
                                <div class="col-10 p-0 px-1 m-0">
                                    <span class="text-secondary mx-2">ReadListQuery</span>
                                    <input class="form-control form-control-sm" v-model="i.ReadListQuery" />
                                </div>
                                <div class="col-10 p-0 px-1 m-0">
                                    <span class="text-secondary mx-2">ChangeStateByKeyQuery</span>
                                    <input class="form-control form-control-sm" v-model="i.ChangeStateByKeyQuery" />
                                </div>
                                <div class="col-10 p-0 px-1 m-0">
                                    <span class="text-secondary mx-2">DeleteByKeyQuery</span>
                                    <input class="form-control form-control-sm" v-model="i.DeleteByKeyQuery" />
                                </div>
                                <div class="col-8 p-0 px-1 m-0">
                                    <span class="text-secondary mx-2">DeleteQuery</span>
                                    <input class="form-control form-control-sm" v-model="i.DeleteQuery" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="card-footer p-3 bg-secondary-subtle bg-gradient border-0 rounded-0">
                <div class="row">
                    <div class="col-18">
                    </div>
                    <div class="col-12">
                        <button class="btn btn-sm btn-primary w-100 py-2" @click="ok">
                            <i class="fa-solid fa-check"></i>
                            &nbsp;
                            <span>Ok</span>
                        </button>
                    </div>
                    <div class="col-18">
                    </div>
                </div>
            </div>
        </div>

    </div>
</template>

<script>
    let _this = { cid: "", c: null, inputs: {}, tables: [] };
    export default {
        methods: {
            pickLinkingTargetTable(ind) {
                openComponent("components/dbTablePicker", {
                    title: "Pick a table", params: {
                        "DbConfName": _this.c.inputs.DbConfName,
                        callback: function (ret) {
                            _this.c.inputs.Relations[ind].LinkingTargetTable = ret;
                        }
                    }
                });
            },
            pickLinkingColumnOnRelationTable(ind) {
                if (fixNull(_this.c.inputs.Relations[ind].RelationTable, '') === '') return;
                openComponent("components/dbTableColumnPicker", {
                    title: "Pick a column", params: {
                        "DbConfName": _this.c.inputs.DbConfName,
                        "TableName": _this.c.inputs.Relations[ind].RelationTable,
                        callback: function (ret) {
                            _this.c.inputs.Relations[ind].LinkingColumnInManyToMany = ret;
                        }
                    }
                });
            },
            pickPkColumnForRelationTable(ind) {
                if (fixNull(_this.c.inputs.Relations[ind].RelationTable, '') === '') return;
                openComponent("components/dbTableColumnPicker", {
                    title: "Pick a column", params: {
                        "DbConfName": _this.c.inputs.DbConfName,
                        "TableName": _this.c.inputs.Relations[ind].RelationTable,
                        callback: function (ret) {
                            _this.c.inputs.Relations[ind].RelationPkColumn = ret;
                        }
                    }
                });
            },
            pickFkColumnForRelationTable(ind) {
                if (fixNull(_this.c.inputs.Relations[ind].RelationTable, '') === '') return;
                openComponent("components/dbTableColumnPicker", {
                    title: "Pick a column", params: {
                        "DbConfName": _this.c.inputs.DbConfName,
                        "TableName": _this.c.inputs.Relations[ind].RelationTable,
                        callback: function (ret) {
                            _this.c.inputs.Relations[ind].RelationFkColumn = ret;
                        }
                    }
                });
            },
            pickTable(ind) {
                openComponent("components/dbTablePicker", {
                    title: "Pick a table", params: {
                        "DbConfName": _this.c.inputs.DbConfName,
                        callback: function (ret) {
                            _this.c.inputs.Relations[ind].RelationTable = ret;
                            rpcAEP("ReadObjectSchema", { "DbConfName": _this.c.inputs.DbConfName, "ObjectName": ret }, function (res) {
                                let fldPk = _.find(R0R(res), function (i) { return i.IsPrimaryKey === true; });
                                _this.c.inputs.Relations[ind].RelationPkColumn = fldPk.Name;
                            });
                        }
                    }
                });
            },
            removeRelation(event) {
                _.remove(_this.c.inputs.Relations, function (i, j) {
                    return j.toString() === $(event.target).attr("ae-data-index");
                });
            },
            addRelation() {
                if (fixNull(_this.c.inputs.Relations, '') === '') _this.c.inputs.Relations = [];
                _this.c.inputs.Relations.push({
                    "RelationName": "",
                    "RelationTable": "",
                    "RelationPkColumn": "",
                    "RelationFkColumn": "",

                    "CreateQuery": "Create",
                    "ReadListQuery": "ReadList",
                    "ChangeStateByKeyQuery": "ChangeStateByKey",
                    "DeleteByKeyQuery": "DeleteByKey",
                    "DeleteQuery": "Delete",

                    "RelationType": "OneToMany",
                    "IsFileCentric": false,
                    "RelationUiWidget": "Grid"
                });
            },
            ok(e) {
                if (_this.c.inputs.callback) _this.c.inputs.callback(_this.c.inputs.Relations);
                _this.c.close();
            },
            cancel() { _this.c.close(); },
            close() { shared.closeComponent(_this.cid); }
        },
        setup(props) {
            _this.cid = props['cid'];
            _this.inputs = shared["params_" + _this.cid];
        },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { },
        props: { cid: String }
    }

</script>