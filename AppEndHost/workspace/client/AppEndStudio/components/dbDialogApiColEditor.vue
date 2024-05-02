<template>
    <div class="col-48 h-100">

        <div class="card h-100 border-0 bg-transparent">
            <div class="card-body p-3 pb-4 bg-transparent fs-d8">

                <div class="row">
                    <label class="col-10 col-form-label">Name</label>
                    <div class="col-38">
                        <input class="form-control form-control-sm" v-model="inputs.methodCol['Name']" disabled />
                    </div>
                </div>

                <div class="row">
                    <label class="col-10 col-form-label">As</label>
                    <div class="col-38">
                        <input class="form-control form-control-sm" v-model="inputs.methodCol['As']" :disabled="shared.fixNull(inputs.methodCol['RefTo'],'')!==''" />
                    </div>
                </div>

                <div v-if="shared.fixNull(inputs.methodCol['Phrase'],'')!==''">
                    <div class="fs-d7">&nbsp;</div>
                    <div class="fb p-1 fs-d7 border-0">
                        Phrase
                    </div>
                    <div spellcheck="false">
                        <textarea class="form-control form-control-sm" rows="5" v-model="inputs.methodCol['Phrase']"></textarea>
                    </div>
                </div>

                <div class="fs-d7">&nbsp;</div>
                <div class="fb p-1 fs-d9 border-0">
                    <div class="badge bg-primary-subtle text-primary-emphasis hover-primary text-center py-2 pointer" @click="switchRefTo">
                        RefTo
                        <i class="fa-solid fa-plus fa-fw" v-if="shared.fixNull(inputs.methodCol['RefTo'],'')===''"></i>
                        <i class="fa-solid fa-times fa-fw" v-if="shared.fixNull(inputs.methodCol['RefTo'],'')!==''"></i>
                    </div>
                </div>
                <div class="card rounded-1" v-if="shared.fixNull(inputs.methodCol['RefTo'],'')!==''">
                    <div class="card-body">
                        <div class="row">
                            <div class="col-24">
                                <div>TargetTable</div>
                                <input class="form-control form-control-sm" v-model="inputs.methodCol['RefTo']['TargetTable']" />
                            </div>
                            <div class="col-24">
                                <div>TargetColumn</div>
                                <input class="form-control form-control-sm" v-model="inputs.methodCol['RefTo']['TargetColumn']" />
                            </div>
                        </div>
                        <div class="card border-0 mt-1">
                            <div class="card-header bg-transparent p-1">
                                <div class="badge bg-primary-subtle text-primary-emphasis hover-primary text-center py-2 pointer" @click="addRefToColumn">
                                    Columns <i class="fa-solid fa-plus fa-fw"></i>
                                </div>
                            </div>
                            <div class="card-body border-1 p-1">
                                <div class="p-0" v-for="i,j in inputs.methodCol['RefTo']['Columns']">
                                    <div class="row" :ae-data-index="j">
                                        <div class="col-6">
                                            <div class="form-control form-control-sm text-center text-muted-light hover-danger pointer" @click="removeRefToColumn">
                                                <i class="fa-solid fa-times fa-fw"></i>
                                            </div>
                                        </div>
                                        <div class="col">
                                            <input class="form-control form-control-sm" v-model="i.Name" />
                                        </div>
                                        <div class="col">
                                            <input class="form-control form-control-sm" v-model="i.As" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

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
    let _this = { cid: "", c: null, inputs: {} };
    export default {
        methods: {
            removeRefToColumn(event) {
                _.remove(_this.c.inputs.methodCol['RefTo']['Columns'], function (i, j) {
                    return j.toString() == $(event.target).parents(".row").attr("ae-data-index");
                });
            },
            addRefToColumn() {
                _this.c.inputs.methodCol['RefTo']['Columns'].push({ "Name": "", "As": "" });
            },
            switchRefTo() {
                if (fixNull(_this.c.inputs.methodCol['RefTo'], '') === '') {
                    _this.c.inputs.methodCol['RefTo'] = {
                        "TargetTable": "Common_BaseInfo",
                        "TargetColumn": "Id",
                        "Columns": [{ "Name": "Title", "As": _this.c.inputs.methodCol["Name"] + "_Title" }]
                    };
                    delete _this.c.inputs.methodCol["Phrase"];
                    delete _this.c.inputs.methodCol["As"];
                } else {
                    delete _this.c.inputs.methodCol['RefTo'];
                }
            },
            ok(e) {
                if (_this.c.inputs.callback) _this.c.inputs.callback(_this.c.inputs.methodCol);
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