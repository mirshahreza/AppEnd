<template>
    <div class="card border-0 shadow-lg bg-transparent rounded-0">
        <div class="card-body p-3 pb-4 bg-transparent fs-d8">
            <div class="">{{inputs.message1}}</div>
            <div class="fs-1d2 fw-bold">{{inputs.message2}}</div>
        </div>
        <div class="card-footer p-0">
            <div class="container-fluid pt-2 pb-1">
                <div class="row p-0">
                    <div class="col-36 px-2">
                        <button autofocus :class="inputs.okClass" @click="ok" id="btnOk">
                            <i class="fa-solid fa-check me-1"></i>
                            <span>{{shared.translate(inputs.okText)}}</span>
                        </button>
                    </div>
                    <div class="col-12 px-2">
                        <button :class="inputs.cancelClass || 'btn btn-sm btn-secondary w-100'" @click="cancel">
                            <i class="fa-solid fa-xmark me-1"></i>
                            <span>{{shared.translate(inputs.cancelText)}}</span>
                        </button>
                    </div>
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