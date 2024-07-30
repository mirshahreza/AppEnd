<template>
    <div class="card border-0 shadow-lg bg-transparent rounded-0">
        <div class="card-body p-3 pb-4 bg-transparent fs-d8">
            <div class="">{{inputs.message1}}</div>
            <div class="fs-1d2 fw-bold">{{inputs.message2}}</div>
        </div>
        <div class="card-footer p-3 bg-secondary-subtle bg-gradient border-0 rounded-0">
            <div class="row">
                <div class="col-12 pe-1">
                    <button :class="inputs.cancelClass" @click="cancel">
                        <i class="fa-solid fa-cancel"></i>
                        &nbsp;
                        <span>{{shared.translate(inputs.cancelText)}}</span>
                    </button>
                </div>
                <div class="col-36 ps-1">
                    <button autofocus :class="inputs.okClass" @click="ok" id="btnOk">
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
    let _this = { cid: "", c: null, inputs: {}, d: {} };
    export default {
        methods: {
            ok() {
                if (_this.c.inputs.callback) _this.c.inputs.callback();
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
        mounted() {
            $(document).ready(function () {
                setTimeout(function () {
                    $("#btnOk").focus();
                    $("#btnOk").addClass("focus-ring");
                }, 500);
            });
        },
        props: { cid: String }
    }
</script>