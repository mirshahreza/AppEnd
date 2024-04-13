<template>
    <div class="card border-0 shadow-lg bg-transparent rounded-0">
        <div class="card-body p-3 pb-4 bg-transparent fs-d8">
            <label class="ms-2">Index</label>
            <input type="text" name="Name" class="form-control form-control-sm" v-model="node.Ind" disabled />
            <div class="my-2"></div>

            <label class="ms-2">Ip</label>
            <input type="text" name="Name" class="form-control form-control-sm" v-model="node.Ip" data-ae-validation-required="true" />
            <div class="my-2"></div>

            <label class="ms-2">Port</label>
            <input type="text" name="Name" class="form-control form-control-sm" v-model="node.Port" data-ae-validation-required="true" />
            <div class="my-2"></div>

            <label class="ms-2">Name</label>
            <input type="text" name="Name" class="form-control form-control-sm" v-model="node.Name" data-ae-validation-required="true" />
            <div class="my-2"></div>

            <hr class="my-3 mt-4" />

            <label class="ms-2">UserName</label>
            <input type="text" name="Name" class="form-control form-control-sm" v-model="node.UserName" data-ae-validation-required="true" />
            <div class="my-2"></div>

            <label class="ms-2">UserName</label>
            <input type="text" name="Name" class="form-control form-control-sm" v-model="node.Password" data-ae-validation-required="true" />
            <div class="my-2"></div>
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
    let _this = { cid: "", c: null, inputs: {}, node: {}, regulator: null };
    
    export default {
        methods: {
            ok(e) {
                if (!_this.c.regulator.isValid()) return;
                if (_this.c.inputs.callback) _this.c.inputs.callback(_this.c.node);
                _this.c.close();
            },
            cancel() { _this.c.close(); },
            close() { shared.closeComponent(_this.cid); }
        },
        setup(props) {
            _this.cid = props['cid'];
            _this.inputs = shared["params_" + _this.cid];
            _this.node = _this.inputs["node"];
        },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { initVueComponent(_this); },
        props: { cid: String }
    }
</script>
