<template>
    <div class="card border-0 shadow-lg bg-transparent rounded-0">
        <div class="card-body p-3 bg-transparent fs-d8">
            <label class="ms-2">Namespace Name</label> <span class="text-secondary fs-d7">Spaces and Wildcards are not allowed</span>
            <input type="text" name="Name" class="form-control form-control-sm" v-model="nc.NamespaceName" data-ae-validation-required="true" />
            <div class="my-2"></div>

            <label class="ms-2">Class Name</label> <span class="text-secondary fs-d7">Spaces and Wildcards are not allowed</span>
            <input type="text" name="Name" class="form-control form-control-sm" v-model="nc.ClassName" data-ae-validation-required="true" />
            <div class="my-2"></div>
            <hr />
            <div class="form-check me-2">
                <input class="form-check-input" type="checkbox" value="" id="chk_Required" v-model="nc.AddSampleMthod">
                <label class="form-check-label" for="chk_Required">
                    Add a sample API
                </label>
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
    let _this = { cid: "", c: null, inputs: {}, nc: { NamespaceName: "", ClassName: "", AddSampleMthod: true }, regulator: null };
    
    export default {
        methods: {
            ok(e) {
                if (!_this.c.regulator.isValid()) return;
                if (_this.c.inputs.callback) _this.c.inputs.callback(_this.c.nc);
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
        mounted() { initVueComponent(_this); },
        props: { cid: String }
    }
</script>
