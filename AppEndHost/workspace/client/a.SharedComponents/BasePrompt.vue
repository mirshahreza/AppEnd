<template>
    <div class="card border-0 shadow-lg bg-transparent rounded-0">
        <div class="card-body p-3 pb-4 bg-transparent fs-d8 scrollable">
            <div :class="inputs.message1Class">{{inputs.message1}}</div>
            <div :class="inputs.message2Class">{{inputs.message2}}</div>
            <div class="row">
                <div class="col-48">
                    <input class="form-control form-control-sm ae-focus" v-model="inputs.retVal" @keyup.enter="ok" v-if="shared.fixNull(inputs.rows,1)===1"
                           :data-ae-validation-required="inputs.validation.required" :data-ae-validation-rule="inputs.validation.rule"/>

                    <textarea v-else class="form-control form-control-sm ae-focus" v-model="inputs.retVal" @keyup.enter="ok" :rows="inputs.rows"
                              :data-ae-validation-required="inputs.validation.required" 
                              :data-ae-validation-rule="inputs.validation.rule"></textarea>

                </div>
            </div>
        </div>
        <div class="card-footer p-0">
            <div class="container-fluid pt-2 pb-1">
                <div class="row p-0">
                    <div class="col-36 px-2">
                        <button class="btn btn-sm btn-primary w-100" @click="ok" data-ae-key="ok">
                            <i class="fa-solid fa-check me-1"></i>
                            <span>{{shared.translate(inputs.okText)}}</span>
                        </button>
                    </div>
                    <div class="col-12 px-2">
                        <button class="btn btn-sm btn-secondary w-100" @click="cancel">
                            <i class="fa-solid fa-xmark me-1"></i>
                            <span>{{shared.translate("Cancel")}}</span>
                        </button>
                    </div>
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