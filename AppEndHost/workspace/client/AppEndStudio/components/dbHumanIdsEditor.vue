<template>
    <div class="card border-0 shadow-lg bg-transparent rounded-0">
        <div class="card-body p-3 pb-4 bg-transparent fs-d8">

            <div class="fb p-1 fs-d7 border-0">
                <div>Columns</div>
            </div>
            <div class="card">
                <div class="card-body p-2">
                    <div class="row">
                        <div class="col-24 form-check" v-for='i in inputs.Cols'>
                            <input class="form-check-input" type="checkbox" v-model="selectedCols" :value="i.Name" :id="'chk_'+i.Name">
                            <label class="form-check-label" :for="'chk_'+i.Name">
                                {{i.Name}}
                            </label>
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
</template>

<script>
    let _this = { cid: "", c: null, inputs: {}, selectedCols: [] };
    export default {
        methods: {
            ok(e) {
                if (_this.c.inputs.callback) _this.c.inputs.callback(_this.c.selectedCols);
                _this.c.close();
            },
            cancel() { _this.c.close(); },
            close() { shared.closeComponent(_this.cid); }
        },
        setup(props) {
            _this.cid = props['cid'];
            _this.inputs = shared["params_" + _this.cid];
            _.forEach(_this.inputs["Cols"], function (i) {
                if (i["IsHumanId"] === true) _this.selectedCols.push(i.Name);
            });
        },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { },
        props: { cid: String },
    }

</script>
