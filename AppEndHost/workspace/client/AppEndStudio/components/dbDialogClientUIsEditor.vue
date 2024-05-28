<template>
    <div class="card border-0 shadow-lg bg-white rounded-0 h-100">
        <div class="card-body p-3 pb-4 bg-transparent fs-d8 scrollable">

            <div v-if="shared.fixNull(inputs.ClientUIs,'')!==''" class="row g-4">
                <div class="col-24" v-for="i,j in inputs.ClientUIs">
                    <div class="card shadow-sm">
                        <div class="card-header bg-primary-subtle p-2 py-1">
                            <table class="w-100">
                                <tr>
                                    <td style="width:100px;"><span class="text-primary fw-bold">File Name</span></td>
                                    <td>
                                        <input class="form-control form-control-sm border-0" v-model="i.FileName" />
                                    </td>
                                    <td style="width:28px;" class="text-end">
                                        <div class="btn btn-close btn-sm text-hover-danger" :ae-data-index="j" @click="removeClientUI"></div>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div class="card-body">
                            <div class="input-group input-group-sm border-0 bg-transparent">

                                <div class="text-secondary p-1">TemplateName</div>
                                <select class="form-select form-select-sm border-0" v-model="i.TemplateName">
                                    <option value="AggregatedReadList">AggregatedReadList</option>
                                    <option value="Create">Create</option>
                                    <option value="ReadByKey">ReadByKey</option>
                                    <option value="ReadList">ReadList</option>
                                    <option value="ReadTreeList">ReadTreeList</option>
                                    <option value="UpdateByKey">UpdateByKey</option>
                                </select>

                                <div class="px-3">&nbsp;</div>

                                <div class="text-secondary p-1">LoadAPI</div>
                                <select class="form-select form-select-sm border-0" v-model="i.LoadAPI" :disabled="i.TemplateName.indexOf('Create')>-1">
                                    <option v-for="o in shared.usableLoads(inputs.DbQueries,i.TemplateName)" v-bind:value="o.Name" :selected="i.SubmitAPI===o.Name">
                                        {{o.Name}}
                                    </option>
                                </select>

                                <div class="px-3">&nbsp;</div>

                                <div class="text-secondary p-1">SubmitAPI</div>
                                <select class="form-select form-select-sm border-0" v-model="i.SubmitAPI">
                                    <option v-for="o in shared.usableSubmits(inputs.DbQueries,i.TemplateName)" v-bind:value="o.Name" :selected="i.SubmitAPI===o.Name">
                                        {{o.Name}}
                                    </option>
                                </select>

                            </div>
                        </div>
                        <div class="card-footer p-1 pt-2 px-3">
                            <div class="form-check">
                                <input class="form-check-input" type="checkbox" value="" :id="'chk_PreventReBuilding_'+i.FileName" v-model="i.PreventReBuilding">
                                <label class="form-check-label" :for="'chk_PreventReBuilding_'+i.FileName">
                                    Prevent ReBuilding
                                </label>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row mt-2">
                <div class="col">
                    <button class="btn btn-sm btn-link text-decoration-none py-2" @click="addClientUI">
                        <i class="fa-solid fa-plus fa-fw"></i>
                        <span>Add Client UI</span>
                    </button>
                </div>
            </div>


        </div>
        <div class="card-footer p-0 bg-secondary-subtle bg-gradient border-0 rounded-0">
            <button class="btn btn-link text-decoration-none bg-hover-light w-100 py-3 rounded-0" @click="ok" data-ae-key="ok">
                <i class="fa-solid fa-check me-2"></i><span>Ok</span>
            </button>
        </div>
    </div>
</template>

<script>
    let _this = { cid: "", c: null, inputs: {} };
    _this.inputs = { ClientUIs: [], DbQueries: [] };
    export default {
        methods: {
            removeClientUI(event) {
                let jj = $(event.target).attr("ae-data-index");
                _.remove(_this.c.inputs.ClientUIs, function (i, j) { return j.toString() == jj; });
            },
            addClientUI() {
                if (fixNull(_this.c.inputs.ClientUIs, '') === '') _this.c.inputs.ClientUIs = [];
                _this.c.inputs.ClientUIs.push({ FileName: "", TemplateName: "", LoadAPI: "", SubmitAPI: "" });
            },
            ok() {
                if (_this.c.inputs.callback) _this.c.inputs.callback(_this.c.inputs.ClientUIs);
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