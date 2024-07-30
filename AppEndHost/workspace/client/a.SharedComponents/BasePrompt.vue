<template>
    <div class="card border-0 shadow-lg bg-transparent rounded-0">
        <div class="card-body p-3 pb-4 bg-transparent fs-d8 scrollable">
            <div class="">{{inputs.message1}}</div>
            <div class="fw-bold px-2">{{inputs.message2}}</div>
            <div class="row">
                <div class="col-48">
                    <input class="form-control form-control-sm ae-focus" v-model="inputs.retVal" @keyup.enter="ok"
                           :data-ae-validation-required="inputs.validation.required" :data-ae-validation-rule="inputs.validation.rule"/>
                </div>
            </div>
        </div>
        <div class="card-footer p-3 bg-secondary-subtle bg-gradient border-0 rounded-0">
            <div class="row">
                <div class="col-24">
                    <button :class="inputs.cancelClass" @click="cancel">
                        <i class="fa-solid fa-cancel"></i>
                        &nbsp;
                        <span>{{shared.translate(inputs.cancelText)}}</span>
                    </button>
                </div>
                <div class="col-24">
                    <button :class="inputs.okClass" @click="ok">
                        <i class="fa-solid fa-check"></i>
                        &nbsp;
                        <span>{{shared.translate(inputs.okText)}}</span>
                    </button>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    let _this = { cid: "", c: null, inputs: {}, regulator: null };
    export default {
        methods: {
            ok() {
                if (!_this.regulator.isValid()) return;
                if (_this.c.inputs.callback) _this.inputs.callback(_this.c.inputs.retVal);
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