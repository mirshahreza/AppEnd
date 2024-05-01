<template>
    <div class="card h-100 border-0 bg-transparent">
        <div class="card-body p-3 pb-4 bg-transparent fs-d8">

            <div v-if="shared.fixNull(inputs.ClientUIs,'')!==''">
                <div class="card shadow-sm mb-1" v-for="i,j in inputs.ClientUIs">
                    <div class="card-header bg-primary-subtle p-2 py-1">
                        <table class="w-100">
                            <tr>
                                <td style="width:100px;"><span class="text-primary fw-bold">File Name</span></td>
                                <td>
                                    <input class="form-control form-control-sm" v-model="i.FileName" />
                                </td>
                                <td style="width:28px;" class="text-end">
                                    <div class="btn btn-close btn-sm text-hover-danger" :ae-data-index="j" @click="removeClientUI"></div>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div class="card-body p-1">

                        <div class="input-group input-group-sm border-0 bg-transparent">

                            <div class="text-secondary p-1">TemplateName</div>
                            <select class="form-select form-select-sm" v-model="i.TemplateName">
                                <option value="AggregatedReadList">AggregatedReadList</option>
                                <option value="Create">Create</option>
                                <option value="ReadByKey">ReadByKey</option>
                                <option value="ReadList">ReadList</option>
                                <option value="ReadTreeList">ReadTreeList</option>
                                <option value="ChangeStateByKey">ChangeStateByKey</option>
                            </select>

                            <div class="text-secondary p-1 ps-2">LoadAPI</div>
                            <select class="form-select form-select-sm" v-model="i.LoadAPI" :disabled="i.TemplateName.indexOf('Create')>-1">
                                <option v-for="o in shared.usableLoads(inputs.DbQueries,i.TemplateName)" v-bind:value="o.Name" :selected="i.SubmitAPI===o.Name">
                                    {{o.Name}}
                                </option>
                            </select>


                            <div class="text-secondary p-1 ps-2">SubmitAPI</div>
                            <select class="form-select form-select-sm" v-model="i.SubmitAPI">
                                <option v-for="o in shared.usableSubmits(inputs.DbQueries,i.TemplateName)" v-bind:value="o.Name" :selected="i.SubmitAPI===o.Name">
                                    {{o.Name}}
                                </option>
                            </select>

                        </div>

                        
                    </div>
                </div>
            </div>

        </div>
        <div class="card-footer p-3 bg-secondary-subtle bg-gradient border-0 rounded-top-0">
            <div class="row">
                <div class="col-24">
                    <button class="btn btn-sm btn-success w-100 py-2" @click="addClientUI">
                        <i class="fa-solid fa-plus fa-fw"></i>
                        &nbsp;
                        <span>Add Client UI</span>
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