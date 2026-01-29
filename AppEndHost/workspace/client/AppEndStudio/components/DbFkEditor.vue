<template>
    <div class="card border-0 shadow-lg bg-transparent rounded-0">
        <div class="card-body p-3 pb-4 bg-transparent fs-d8">

            <div class="row my-2">
                <div class="col-16"><span>FkName</span></div>
                <div class="col"><span >{{fk.FkName}}</span></div>
            </div>

            <div class="row my-2 mt-3">
                <div class="col-16"><span>BaseTable</span></div>
                <div class="col"><span >{{fk.BaseTable}}</span></div>
            </div>
            <div class="row my-2">
                <div class="col-16 pt-2"><span>BaseColumn</span></div>
                <div class="col">
                    <select class="form-select form-select-sm fw-bold " v-model="fk.BaseColumn">
                        <option>-</option>
                        <option v-for="i in baseColumns">{{i.Name}}</option>
                    </select>
                </div>
            </div>
            <div class="row my-2 mt-3">
                <div class="col-16 pt-2"><span>TargetTable</span></div>
                <div class="col">
                    <select class="form-select form-select-sm fw-bold " v-model="fk.TargetTable" @change="targetTableChanged">
                        <option>-</option>
                        <optgroup label="Most Usable Items">
                            <option>BaseInfo</option>
                        </optgroup>
                        <optgroup label="All Items">
                            <option v-for="i in tables">{{i.ObjectName}}</option>
                        </optgroup>
                    </select>
                </div>
            </div>
            <div class="row my-2">
                <div class="col-16"><span>TargetColumn</span></div>
                <div class="col"><span >{{fk.TargetColumn}}</span></div>
            </div>
            <div class="row my-0 mt-3">
                <div class="col-16"><span>Enforce relation</span></div>
                <div class="col">
                    <div class="form-check">
                        <input class="form-check-input" type="checkbox" v-model="fk.EnforceRelation" id="chk_EnforceRelation" disabled />
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
</template>

<script>
    let _this = { cid: "", c: null, row: {}, baseTable: "", dbConfName: "", tables: [], baseColumns: [] };
    _this.dbConfName = getQueryString("cnn");
    _this.baseTable = getQueryString("o");

    export default {
        methods: {
            ok(e) {
                if (_this.row.callback) _this.row.callback(_this.c.fk);
                shared.closeComponent(_this.cid);
            },
            cancel(e) {
                shared.closeComponent(_this.cid);
            },
            targetTableChanged() {

                let targetTable = _this.c.fk.TargetTable;
                if (targetTable === "") {
                    _this.c.fk.TargetColumn = "";
                    return;
                }
                rpcAEP("ReadObjectSchema", { "DbConfName": _this.dbConfName, "ObjectName": targetTable }, function (res) {
                    let fldPk = _.find(R0R(res), function (i) { return i.IsPrimaryKey === true; });
                    _this.c.fk.TargetColumn = fldPk.Name;
                });
            },
            readTablesList() {
                rpcAEP("GetDbObjectsStack", { "DbConfName": _this.dbConfName, "ObjectType": "Table", "Filter": null }, function (res) {
                    _this.c.tables = R0R(res);
                });
                rpcAEP("GetFileContent", { "PathToRead": "workspace/server/" + _this.dbConfName + "." + _this.baseTable + ".dbdialog.json" }, function (res) {
                    let oJson = JSON.parse(R0R(res));
                    _this.c.baseColumns = oJson.Columns;
                });
            }
        },
        setup(props) {
            _this.cid = props['cid'];
            _this.row = shared["params_" + _this.cid];
        },
        data() {
            return { fk: _this.row, baseColumns: _this.baseColumns, tables: _this.tables };
        },
        created() { _this.c = this; },
        mounted() { _this.c.readTablesList(); },
        props: { cid: String }
    }

</script>
