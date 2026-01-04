<template>
    <div class="card border-0 shadow-lg bg-transparent rounded-0 h-100">
        <div class="card-body p-3 pb-4 bg-transparent fs-d8 scrollable">

            <div v-if="['Create','ReadList','ReadByKey','UpdateByKey'].includes(mObj['Type'])" class="my-1">
                <div class="card">
                    <div class="card-header p-1 py-0 bg-success-subtle">
                        <div class="input-group input-group-sm border-0 bg-transparent">
                            <div class="input-group-text bg-transparent p-1 my-1 me-1 fs-d8 fw-bold" style="width:90px;">Columns</div>

                            <div class="btn btn-sm bg-white p-1 text-primary my-1 me-1 fs-d8 fw-bold" id="addColumnBtn" style="z-index:100000">
                                <div class="dropdown">
                                    <div class="pointer text-center bg-transparent" id="addSimpleFieldDD" data-bs-toggle="dropdown" aria-expanded="false">
                                        Add Column <i class="fa-solid fa-plus"></i>
                                    </div>
                                    <ul class="dropdown-menu shadow-lg border-2" aria-labelledby="addSimpleFieldDD">
                                        <li v-for="i in allColumns">
                                            <span class="dropdown-item fs-d7 text-nowrap text-primary text-hover-success pointer p-1"
                                                  @click="addColumn" v-if="!shared.toSimpleArrayOf(mObj['Columns'],'Name').includes(i.Name)">
                                                <i class="fa-solid fa-plus fa-fw"></i>
                                                <span class="data-ae-key">{{i.Name}}</span>
                                                <span class="text-muted fs-d7"> ({{i.DbType}})</span>
                                            </span>
                                        </li>
                                    </ul>
                                </div>
                            </div>

                            <span>&nbsp;</span>

                            <div class="btn btn-sm bg-white p-1 text-primary my-1 me-1 fs-d8 fw-bold" @click="addPhraseColumn">
                                Add Phrase Column
                            </div>

                            <input type="text" class="form-control form-control-sm border-0 rounded-0 bg-transparent" disabled />

                            <div class="btn btn-sm bg-white p-1 text-primary my-1 me-1 fs-d8 fw-bold" @click="makeColumnsSortable" id="sortColumnsBtn">
                                Start Sorting Columns
                            </div>
                            <div class="btn btn-sm bg-white p-1 text-primary my-1 me-1 fs-d8 fw-bold collapse" @click="finishColumnsSortable" id="finishSortColumnsBtn">
                                Finish Sorting Columns
                            </div>

                        </div>
                    </div>
                    <div class="card-body p-1">
                        <ul class="list-group list-group-horizontal d-flex flex-wrap sortable-columns">
                            <li class="list-group-item p-1 border-0 data-ae-parent" v-for="i in mObj['Columns']">
                                <span class="form-control form-control-sm p-1 text-nowrap">
                                    <i class="fa-solid fa-times fa-fw text-secondary text-hover-danger pointer" @click="removeColumn"></i>
                                    <i class="fa-solid fa-eye-slash fs-d8 text-warning" v-if="i.Hidden===true"></i>
                                    <span class="data-ae-key me-1 text-dark" v-if="shared.fixNull(i.Name,'')!==''">{{i.Name}}</span>
                                    <span class="data-ae-as me-1 text-dark" v-if="shared.fixNull(i.As,'')!==''" :data-ae-name="i.As">{{i.As}} <span class="fs-d7 text-secondary">phrase</span></span>

                                    <span class="fs-d7 text-success" v-if="shared.fixNull(i.RefTo,'')!=='' && shared.fixNull(i.RefTo.Columns,'')!==''">
                                        <span class="mx-2" v-for="c in i.RefTo.Columns">{{c.As}}</span>
                                    </span>

                                    <i class="fa-solid fa-edit fa-fw text-primary text-hover-success pointer" @click="openDbQueryColumnEditor(i)"></i>
                                </span>
                            </li>
                        </ul>
                    </div>
                </div>
            </div>
            
            <div v-if="['Create','ReadList','ReadByKey','UpdateByKey'].includes(mObj['Type'])" class="my-1">
                <div class="card">
                    <div class="card-header p-1 py-0 bg-success-subtle">
                        <div class="input-group input-group-sm border-0 bg-transparent">
                            <div class="input-group-text bg-transparent p-1 my-1 me-1 fs-d8 fw-bold" style="width:90px;">Params</div>
                            <div class="btn btn-sm bg-white p-1 text-primary my-1 me-1 fs-d8 fw-bold" @click="addParam">
                                Add Param <i class="fa-solid fa-plus fa-fw"></i>
                            </div>
                            <input type="text" class="form-control form-control-sm border-0 rounded-0 bg-transparent" disabled />
                        </div>
                    </div>
                    <div class="card-body p-1">
                        <div v-for="i,j in mObj['Params']">
                            <div class="row" :ae-data-index="j">
                                <div class="col-2">
                                    <div class="form-control form-control-sm text-center text-muted text-hover-danger pointer" @click="removeParam">
                                        <i class="fa-solid fa-times fa-fw"></i>
                                    </div>
                                </div>
                                <div class="col">
                                    <input class="form-control form-control-sm" v-model="i.Name" />
                                </div>
                                <div class="col-10">
                                    <select class="form-select form-select-sm" v-model="i.DbType">
                                        <option value="BIGINT">BIGINT</option>
                                        <option value="INT">INT</option>
                                        <option value="TINYINT">TINYINT</option>
                                        <option value="smallint">SMALLINT</option>
                                        <option value="DECIMAL">DECIMAL</option>
                                        <option value="FLOAT">FLOAT</option>
                                        <option value="NUMERIC">NUMERIC</option>
                                        <option value="REAL">REAL</option>

                                        <option value="BIT" v-if="!i.IsPrimaryKey">BIT</option>
                                        <option value="UNIQUEIDENTIFIER">UNIQUEIDENTIFIER</option>

                                        <option value="DATE" v-if="!i.IsPrimaryKey">DATE</option>
                                        <option value="TIME" v-if="!i.IsPrimaryKey">TIME</option>
                                        <option value="DATETIME" v-if="!i.IsPrimaryKey">DATETIME</option>
                                        <option value="DATETIME2" v-if="!i.IsPrimaryKey">DATETIME2</option>
                                        <option value="SMALLDATETIME" v-if="!i.IsPrimaryKey">SMALLDATETIME</option>
                                        <option value="DATETIMEOFFSET" v-if="!i.IsPrimaryKey">DATETIMEOFFSET</option>
                                        <option value="TIMESTAMP">TIMESTAMP</option>

                                        <option value="TEXT" v-if="!i.IsPrimaryKey">TEXT</option>
                                        <option value="NTEXT" v-if="!i.IsPrimaryKey">NTEXT</option>
                                        <option value="VARCHAR">VARCHAR</option>
                                        <option value="NVARCHAR">NVARCHAR</option>
                                        <option value="CHAR">CHAR</option>
                                        <option value="NCHAR">NCHAR</option>

                                        <option value="IMAGE" v-if="!i.IsPrimaryKey">IMAGE</option>

                                        <option value="XML" v-if="!i.IsPrimaryKey">XML</option>
                                    </select>
                                </div>
                                <div class="col-4">
                                    <input class="form-control form-control-sm text-center" v-model="i.Size" />
                                </div>
                                <div class="col-14">
                                    <input class="form-control form-control-sm" v-model="i.ValueSharp" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div v-if="['Create','ReadList','ReadByKey','UpdateByKey','DeleteByKey'].includes(mObj['Type']) && shared.fixNull(relations,'')!=='' && relations.length>0" class="my-1">
                <div class="card">
                    <div class="card-header p-1 py-0 bg-success-subtle">
                        <div class="input-group input-group-sm border-0 bg-transparent">

                            <div class="input-group-text bg-transparent p-1 my-1 me-1 fs-d8 fw-bold" style="width:90px;">Relations</div>

                            <div class="btn btn-sm bg-white p-1 text-primary my-1 me-1 fs-d8 fw-bold">
                                <div class="dropdown">
                                    <div class="text-primary text-hover-success pointer text-center bg-transparent" id="addSimpleFieldDD" data-bs-toggle="dropdown" aria-expanded="false">
                                        Add Relation <i class="fa-solid fa-plus"></i>
                                    </div>
                                    <ul class="dropdown-menu shadow-lg border-2" aria-labelledby="addSimpleFieldDD">
                                        <li v-for="i in relations">
                                            <a href="#" class="dropdown-item fs-d8 text-primary text-hover-success pointer p-1" @click="addRelation"
                                               v-if="!shared.fixNull(mObj['Relations'],[]).includes(i.RelationName)">
                                                <i class="fa-solid fa-plus fa-fw"></i>
                                                <span class="relation-name">{{i.RelationName}}</span>
                                            </a>
                                        </li>
                                    </ul>
                                </div>
                            </div>
                            <input type="text" class="form-control form-control-sm border-0 rounded-0 bg-transparent" disabled />
                        </div>
                    </div>

                    <div class="card-body p-1">
                        <div class="badge p-2" v-if="shared.fixNull(mObj['Relations'],'')!==''">
                            <span class="form-control form-control-sm d-inline-block me-1" style="width:auto;" v-for="i in mObj['Relations']">
                                <i class="fa-solid fa-times fa-fw text-muted text-hover-danger pointer" @click="removeRelation"></i>
                                <span class="relation-name px-2 text-dark">{{i}}</span>
                            </span>
                        </div>
                    </div>
                </div>
            </div>

            <div v-if="['ReadList','AggregatedReadList'].includes(mObj['Type'])" class="my-1">
                <div class="card">
                    <div class="card-header p-1 py-0 bg-success-subtle">
                        <div class="input-group input-group-sm border-0 bg-transparent">
                            <div class="input-group-text bg-transparent p-1 my-1 me-1 fs-d8 fw-bold" style="width:90px;">Where</div>
                            <input type="text" class="form-control form-control-sm border-0 rounded-0 bg-transparent" disabled />
                        </div>
                    </div>
                    <div class="card-body p-1">

                        <div v-if="shared.fixNull(mObj['Where'],'')!==''">
                            <select class="form-select form-select-sm" v-model="mObj['Where']['ConjunctiveOperator']">
                                <option value="And">And</option>
                                <option value="Or">Or</option>
                            </select>
                        </div>

                        <div class="btn btn-sm btn-link text-decoration-none fs-d9 pointer" @click="addCompareClause">
                            <i class="fa-solid fa-plus fa-fw"></i> CompareClause
                        </div>
                        <div v-if="shared.fixNull(mObj['Where'],'')!=='' && shared.fixNull(mObj['Where']['CompareClauses'],'')!==''">
                            <div class="p-0" v-for="i,j in mObj['Where']['CompareClauses']">
                                <div class="row m-0 p-0" :ae-data-index="j">
                                    <div class="col-2 p-0 m-0">
                                        <div class="form-control form-control-sm text-center text-muted text-hover-danger pointer" @click="removeCompareClause">
                                            <i class="fa-solid fa-times fa-fw"></i>
                                        </div>
                                    </div>
                                    <div class="col-8 p-0 m-0">
                                        <select class="form-select form-select-sm" v-model="i.Name">
                                            <option v-for="c in allColumns" :value="c.Name">{{c.Name}}</option>
                                        </select>
                                    </div>
                                    <div class="col-8 p-0 m-0">
                                        <select class="form-select form-select-sm" v-model="i.CompareOperator">
                                            <option value="Equal">Equal</option>
                                            <option value="Contains">Contains</option>
                                            <option value="StartsWith">StartsWith</option>
                                            <option value="EndsWith">EndsWith</option>
                                            <option value="MoreThan">MoreThan</option>
                                            <option value="MoreThanOrEqual">MoreThanOrEqual</option>
                                            <option value="LessThan">LessThan</option>
                                            <option value="LessThanOrEqual">LessThanOrEqual</option>
                                            <option value="In">In</option>
                                            <option value="NotIn">NotIn</option>
                                            <option value="IsNull">IsNull</option>
                                            <option value="IsNotNull">IsNotNull</option>
                                        </select>
                                    </div>
                                    <div class="col p-0 m-0">
                                        <input class="form-control form-control-sm" v-model="i.Value" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div>&nbsp;</div>

                        <div class="btn btn-sm btn-link text-decoration-none fs-d9 pointer" @click="addSimpleClause">
                            <i class="fa-solid fa-plus fa-fw"></i> SimpleClause
                        </div>
                        <div v-if="shared.fixNull(mObj['Where'],'')!=='' && shared.fixNull(mObj['Where']['SimpleClauses'],'')!==''">
                            <div class="p-0" v-for="i,j in mObj['Where']['SimpleClauses']">
                                <div class="row m-0 p-0" :ae-data-index="j">
                                    <div class="col-2 p-0 m-0">
                                        <div class="form-control form-control-sm text-center text-muted text-hover-danger pointer" @click="removeSimpleClause">
                                            <i class="fa-solid fa-times fa-fw"></i>
                                        </div>
                                    </div>
                                    <div class="col p-0 m-0">
                                        <textarea class="form-control form-control-sm" v-model="i.Phrase" rows="2"></textarea>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div>&nbsp;</div>

                        <div class="btn btn-sm btn-link text-decoration-none fs-d9 pointer" @click="addComplexClause">
                            <i class="fa-solid fa-plus fa-fw"></i> ComplexClause
                        </div>
                        <div v-if="shared.fixNull(mObj['Where'],'')!=='' && shared.fixNull(mObj['Where']['ComplexClauses'],'')!==''">
                            <div class="p-0" v-for="i,j in mObj['Where']['ComplexClauses']">
                                <div class="row m-0 p-0" :ae-data-index="j">
                                    <div class="col-2 p-0 m-0">
                                        <div class="form-control form-control-sm text-center text-muted text-hover-danger pointer" @click="removeComplexClause">
                                            <i class="fa-solid fa-times fa-fw"></i>
                                        </div>
                                    </div>
                                    <div class="col p-0 m-0">

                                        <select class="form-select form-select-sm" v-model="i['ConjunctiveOperator']">
                                            <option value="And">And</option>
                                            <option value="Or">Or</option>
                                        </select>

                                        <div class="btn btn-sm btn-link text-decoration-none fs-d9 pointer" :ae-data-index-parent="j" @click="addCompareClause2">
                                            <i class="fa-solid fa-plus fa-fw"></i> CompareClause
                                        </div>
                                        <div class="p-0" v-for="i2,j2 in i['CompareClauses']">
                                            <div class="row m-0 p-0" :ae-data-index="j2" :ae-data-index-parent="j">
                                                <div class="col-2 p-0 m-0">
                                                    <div class="form-control form-control-sm text-center text-muted text-hover-danger pointer" @click="removeCompareClause2">
                                                        <i class="fa-solid fa-times fa-fw"></i>
                                                    </div>
                                                </div>
                                                <div class="col-8 p-0 m-0">
                                                    <select class="form-select form-select-sm" v-model="i2.Name">
                                                        <option v-for="c in allColumns" :value="c.Name">{{c.Name}}</option>
                                                    </select>
                                                </div>
                                                <div class="col-8 p-0 m-0">
                                                    <select class="form-select form-select-sm" v-model="i2.CompareOperator">
                                                        <option value="Equal">Equal</option>
                                                        <option value="Contains">Contains</option>
                                                        <option value="StartsWith">StartsWith</option>
                                                        <option value="EndsWith">EndsWith</option>
                                                        <option value="MoreThan">MoreThan</option>
                                                        <option value="MoreThanOrEqual">MoreThanOrEqual</option>
                                                        <option value="LessThan">LessThan</option>
                                                        <option value="LessThanOrEqual">LessThanOrEqual</option>
                                                        <option value="In">In</option>
                                                        <option value="NotIn">NotIn</option>
                                                        <option value="IsNull">IsNull</option>
                                                        <option value="IsNotNull">IsNotNull</option>
                                                    </select>

                                                </div>
                                                <div class="col p-0 m-0">
                                                    <input class="form-control form-control-sm" v-model="i2.Value" />
                                                </div>
                                            </div>
                                        </div>

                                        <div>&nbsp;</div>

                                        <div class="btn btn-sm btn-link text-decoration-none fs-d9 pointer" :ae-data-index-parent="j" @click="addSimpleClause2">
                                            <i class="fa-solid fa-plus fa-fw"></i> SimpleClause
                                        </div>
                                        <div class="p-0" v-for="i2,j2 in i['SimpleClauses']">
                                            <div class="row m-0 p-0" :ae-data-index="j2" :ae-data-index-parent="j">
                                                <div class="col-2 p-0 m-0">
                                                    <div class="form-control form-control-sm text-center text-muted text-hover-danger pointer" @click="removeSimpleClause2">
                                                        <i class="fa-solid fa-times fa-fw"></i>
                                                    </div>
                                                </div>
                                                <div class="col p-0 m-0">
                                                    <textarea class="form-control form-control-sm" v-model="i2.Phrase" rows="2"></textarea>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>


                    </div>
                </div>
            </div>

            <div v-if="['ReadList'].includes(mObj['Type'])" class="my-1">
                <div class="card">
                    <div class="card-header p-1 py-0 bg-success-subtle">
                        <div class="input-group input-group-sm border-0 bg-transparent">
                            <div class="input-group-text bg-transparent p-1 my-1 me-1 fs-d8 fw-bold" style="width:90px;">Aggregations</div>
                            <div class="btn btn-sm bg-white p-1 text-primary my-1 me-1 fs-d8 fw-bold" @click="addAggregation">
                                Add Aggregation <i class="fa-solid fa-plus fa-fw"></i>
                            </div>
                            <input type="text" class="form-control form-control-sm border-0 rounded-0 bg-transparent" disabled />
                        </div>
                    </div>
                    <div class="card-body p-1">
                        <div class="p-0" v-for="i,j in mObj['Aggregations']">
                            <div class="row m-0 p-0" :ae-data-index="j">
                                <div class="col-2 p-0 m-0">
                                    <div class="form-control form-control-sm text-center text-muted text-hover-danger pointer" @click="removeAggregation">
                                        <i class="fa-solid fa-times fa-fw"></i>
                                    </div>
                                </div>
                                <div class="col-6 p-0 m-0">
                                    <input class="form-control form-control-sm" v-model="i.Name" />
                                </div>
                                <div class="col p-0 m-0">
                                    <input class="form-control form-control-sm" v-model="i.Phrase" />
                                </div>
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
</template>

<script>
    let _this = { cid: "", row: {}, dbConfName: getQueryString("cnn"), oName: getQueryString("o"), c: null };

    export default {
        props: {
            cid: String
        },
        data() {
            return { mObj: _this.row["MethodBody"], allColumns: _this.row["AllColumns"], relations: _this.row["Relations"] };
        },
        setup(props) {
            _this.cid = props['cid'];
            _this.row = shared["params_" + _this.cid];
        },
        methods: {
            addAggregation() {
                if (fixNull(_this.c.mObj["Aggregations"], '') === '') _this.c.mObj["Aggregations"] = [];
                _this.c.mObj["Aggregations"].push({ "Name": "", "Phrase": "" });
            },
            removeAggregation(event) {
                _.remove(_this.c.mObj["Aggregations"], function (i, j) {
                    return j.toString() == $(event.target).parents(".row").attr("ae-data-index");
                });
            },
            addCompareClause() {
                if (fixNull(_this.c.mObj["Where"], '') === '') _this.c.mObj["Where"] = { "ConjunctiveOperator": "And" };
                if (fixNull(_this.c.mObj["Where"]['CompareClauses'], '') === '') _this.c.mObj["Where"]['CompareClauses'] = [];
                _this.c.mObj["Where"]['CompareClauses'].push({ "Name": "", "CompareOperator": "Equal", "Value": "" });
            },
            removeCompareClause(event) {
                _.remove(_this.c.mObj["Where"]['CompareClauses'], function (i, j) {
                    return j.toString() === $(event.target).parents(".row").attr("ae-data-index");
                });
            },

            addCompareClause2(event) {
                let ind = parseInt($(event.target).attr("ae-data-index-parent"));
                _this.c.mObj["Where"]['ComplexClauses'][ind]["CompareClauses"].push({ "Name": "", "CompareOperator": "", "Value": "" });
            },
            removeCompareClause2(event) {
                let ind = parseInt($(event.target).parents(".row").attr("ae-data-index-parent"));
                _.remove(_this.c.mObj["Where"]['ComplexClauses'][ind]['CompareClauses'], function (i, j) {
                    return j.toString() === $(event.target).parents(".row").attr("ae-data-index");
                });
            },

            addSimpleClause() {
                if (fixNull(_this.c.mObj["Where"], '') === '') _this.c.mObj["Where"] = { "ConjunctiveOperator": "And" };
                if (fixNull(_this.c.mObj["Where"]['SimpleClauses'], '') === '') _this.c.mObj["Where"]['SimpleClauses'] = [];
                _this.c.mObj["Where"]['SimpleClauses'].push({ "Phrase": "" });
            },
            removeSimpleClause(event) {
                _.remove(_this.c.mObj["Where"]['SimpleClauses'], function (i, j) {
                    return j.toString() === $(event.target).parents(".row").attr("ae-data-index");
                });
            },

            addSimpleClause2(event) {
                let ind = parseInt($(event.target).attr("ae-data-index-parent"));
                _this.c.mObj["Where"]['ComplexClauses'][ind]['SimpleClauses'].push({ "Phrase": "" });
            },
            removeSimpleClause2(event) {
                let ind = parseInt($(event.target).parents(".row").attr("ae-data-index-parent"));
                _.remove(_this.c.mObj["Where"]['ComplexClauses'][ind]['SimpleClauses'], function (i, j) {
                    return j.toString() == $(event.target).parents(".row").attr("ae-data-index");
                });
            },

            addComplexClause() {
                if (fixNull(_this.c.mObj["Where"], '') === '') _this.c.mObj["Where"] = { "ConjunctiveOperator": "And" };
                if (fixNull(_this.c.mObj["Where"]['ComplexClauses'], '') === '') _this.c.mObj["Where"]['ComplexClauses'] = [];
                _this.c.mObj["Where"]['ComplexClauses'].push({ "ConjunctiveOperator": "And", "CompareClauses": [], "SimpleClauses": [] });
            },
            removeComplexClause(event) {
                _.remove(_this.c.mObj["Where"]['ComplexClauses'], function (i, j) {
                    return j.toString() == $(event.target).parents(".row").attr("ae-data-index");
                });
            },

            addRelation(event) {
                if (fixNull(_this.c.mObj["Relations"], '') === '') _this.c.mObj["Relations"] = [];
                _this.c.mObj["Relations"].push($(event.target).parent().find(".relation-name").text().trim());
            },
            removeRelation(event) {
                _.remove(_this.c.mObj["Relations"], function (i) { return i == $(event.target).parent().find(".relation-name").text().trim(); });
            },
            removeParam(event) {
                _.remove(_this.c.mObj["Params"], function (i, j) {
                    return j.toString() === $(event.target).parents(".row").attr("ae-data-index");
                });
            },
            addParam() {
                if (fixNull(_this.c.mObj["Params"], '') === '') _this.c.mObj["Params"] = [];
                _this.c.mObj["Params"].push({ "Name": "", "DbType": "", "Size": "", "ValueSharp": "" });
            },
            openDbQueryColumnEditor(methodCol) {
                let modelCol = _.cloneDeep(_.find(_this.c.allColumns, function (i) { return i.Name === methodCol.Name; }));
                openComponent("components/DbDialogApiColEditor", {
                    title: `DbQueryColumn Editor`, modalSize:'modal-lg', params: {
                        "modelCol": modelCol,
                        "methodCol": methodCol,
                        callback: function (ret) {
                            let cIndex = _.findIndex(_this.c.mObj['Columns'], function (i) { return i.Name === methodCol.Name || i.As === methodCol.As; });
                            _this.c.mObj['Columns'][cIndex] = ret;
                        }
                    }
                });
            },
            addPhraseColumn() {
                _this.c.mObj['Columns'].push({ "As": genUN('As'), "Phrase": "SELECT 1" });
            },
            removeColumn(event) {
                let colName = $(event.target).parents(".data-ae-parent:first").find(".data-ae-key").text();
                if (colName === "") colName = $(event.target).parents(".data-ae-parent:first").find(".data-ae-as").text();
                _.remove(_this.c.mObj['Columns'], function (i) { return i.Name === colName || i.As === colName; });
            },
            addColumn(event) {
                let colName = $(event.target).parent().find(".data-ae-key").text();
                if (fixNull(_this.c.mObj['Columns'], '') === '') _this.c.mObj['Columns'] = [];
                _this.c.mObj['Columns'].push({ "Name": colName });
            },
            makeColumnsSortable() {
                $('#addColumnBtn').hide();
                $('#sortColumnsBtn').hide();
                $('#finishSortColumnsBtn').show();
                $('.sortable-columns').sortable();
            },
            finishColumnsSortable() {
                $('#addColumnBtn').show();
                $('#sortColumnsBtn').show();
                $('#finishSortColumnsBtn').hide();
                $('.sortable-columns').sortable('destroy');

                let sortedItems = [];
                $(".sortable-columns .list-group-item").each(function () {
                    let colName = $(this).find('.data-ae-key').text().trim();
                    let colAs = $(this).find('.data-ae-as').text().trim();
                    if (colName !== '') {
                        sortedItems.push(_.filter(_this.c.mObj['Columns'], function (i) { return i.Name === colName; })[0]);
                    } else {
                        sortedItems.push(_.filter(_this.c.mObj['Columns'], function (i) { return i.As === colAs; })[0]);
                    }
                });
                _this.c.mObj['Columns'] = [];
                setTimeout(function () {
                    _this.c.mObj['Columns'] = sortedItems;
                }, 1);
            },
            ok(e) {
                if (_this.row.callback) _this.row.callback(_this.c.mObj);
                shared.closeComponent(_this.cid);
            },
            cancel(e) {
                shared.closeComponent(_this.cid);
            }
        },
        created() {
            _this.c = this;
        },
        mounted() {
            
        }
    }

</script>
