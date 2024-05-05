<template>
    <div class="card h-100 rounded-bottom-0 rounded-end-0 bg-transparent border-0">
        <div class="card-body p-0 pt-2">
            <div class="card h-100 border-light bg-light bg-opacity-75 border-0">
                <div class="card-body rounded rounded-2 border border-3 border-light fs-d8 pt-0 ps-3 pe-3 bg-transparent scrollable">

                    <label class=""><span class="" v-for="i,j in inputs.humanIds">{{shared.translate(i)}}<span v-if="j<inputs.humanIds.length-1"> || </span></span></label>

                    <div class="row mb-2">
                        <div class="col-48">
                            <input class="form-control form-control-sm bg-light-subtle ae-focus" v-model="searchPhrase" @keyup="localLoadPickerRows" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-48">
                            <div class="form-control form-control-sm p-0" style="min-height:300px;">
                                <table class="table table-sm table-hover w-100 ae-table m-0 bg-transparent">
                                    <tbody>
                                        <tr v-for="i in pickerRows" class="pointer" @click="pickRow(i.Id)">
                                            <td class="ae-table-td text-dark text-center" style="width:75px;overflow: hidden;text-overflow: ellipsis;">
                                                <span class="pk text-primary">{{i["Id"]}}</span>
                                            </td>
                                            <td class="" style="overflow: hidden;text-overflow: ellipsis;">
                                                <span class="">{{i["DisplayTitle"]}}</span>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </div>
</template>

<script>
    let _this = { cid: "", c: null, inputs: {}, searchPhrase: "", pickerRows: [], origWhere: null };
    export default {
        methods: {
            pickRow(id) {
                if (_this.c.inputs.callback) {
                    let ret = _.filter(_this.c.pickerRows, function (i) { return i.Id.toString() === id.toString(); });
                    if (ret.length > 0) {
                        ret = ret[0];
                        _this.inputs.callback(ret);
                    }
                }
                _this.c.close();
            },
            localLoadPickerRows() {
                let sPhrase = _this.c.searchPhrase.trim();
                let _w;
                if (sPhrase !== '') {
                    _w = _.cloneDeep(_this.c.inputs.api['Inputs']['ClientQueryJE']['Where']);
                    let searchClauses = [];
                    if (fixNull(_w, '') === '') _w = { "ConjunctiveOperator": "AND", "ComplexClauses": [] };
                    _w['ComplexClauses'] = [];
                    _.forEach(_this.c.inputs.humanIds, function (colName) {
                        searchClauses.push({ "Name": colName, "Value": sPhrase, "CompareOperator": "Contains" });
                    });
                    _w["ComplexClauses"].push({ "ConjunctiveOperator": "OR", "CompareClauses": searchClauses });
                } else {
                    _this.c.inputs.api['Inputs']['ClientQueryJE']['Where'] = _.cloneDeep(_this.c.origWhere);
                }

                _this.c.inputs.api['Inputs']['ClientQueryJE']['Where']=_w;

                console.log(JSON.stringify(_this.inputs.api,null,4));

                rpc({
                    requests: [_this.inputs.api],
                    onDone: function (res) {
                        let r = res[0]['Result']['Master'];
                        _.forEach(r, function (i) {
                            i['DisplayTitle'] = '';
                            _.forEach(_this.c.inputs.humanIds, function (colName) {
                                i['DisplayTitle'] += ' ' + fixNull(i[colName],'');
                            });
                        });
                        _this.c.pickerRows = r;
                    }
                });

            },
            close() { shared.closeComponent(_this.cid); }
        },
        setup(props) {
            _this.cid = props['cid'];
            _this.inputs = shared["params_" + _this.cid];
            _this.origWhere = _.cloneDeep(_this.inputs.api['Inputs']['ClientQueryJE']['Where']);
        },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { initVueComponent(_this); _this.c.localLoadPickerRows();  },
        props: { cid: String }
    }

</script>