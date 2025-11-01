<template>
    <div class="card border-0 shadow-lg bg-white rounded-0 h-100">
        <div class="card-header">
            <div class="fw-bold fs-d7 border-0">
                <div>Select Columns</div>
            </div>
        </div>
        <div class="card-body px-4 py-3 bg-transparent fs-d8 scrollable">
            <div class="row">
                <div class="col-24 form-check" v-for='i in inputs.Cols'>
                    <input class="form-check-input" type="checkbox" v-model="selectedCols" :value="i.Name" :id="'chk_'+i.Name">
                    <label class="form-check-label" :for="'chk_'+i.Name">
                        {{i.Name}}
                    </label>
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
                if (i["IsSortable"] === true) _this.selectedCols.push(i.Name);
            });
        },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { },
        props: { cid: String },
    }

</script>
