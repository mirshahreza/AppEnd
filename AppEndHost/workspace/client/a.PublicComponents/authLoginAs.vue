<template>
    <div class="card h-100 border-0 shadow-lg bg-transparent rounded-0">
        <div class="card-body p-3 pb-4 bg-transparent fs-d8">
            <input type="text" class="form-control form-control-lg ae-focus"
                   @keyup.enter="ok" v-model="loginasUserName"
                   data-ae-validation-required="true" data-ae-validation-rule="" />
        </div>
        <div class="card-footer p-3 bg-secondary-subtle bg-gradient border-0 rounded-0">
            <div class="row">
                <div class="col-48">
                    <button class="btn btn-sm btn-primary w-100 py-2" @click="ok" data-ae-key="ok">
                        <i class="fa-solid fa-check pe-1"></i>
                        <span>Ok</span>
                    </button>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    let _this = { cid: "", c: null, loginasUserName: "" };

    export default {
        methods: {
            ok() {
                if (!_this.regulator.isValid()) return;
                let res = loginAs(_this.c.loginasUserName);
                if (res === true) {
                    goHome();
                }
                else {
                    showError("LoginAs failed, Maybe the UserName is wrong !!!");
                }
            }
        },
        setup(props) { _this.cid = props['cid']; },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { initVueComponent(_this); },
        props: { cid: String }
    }
</script>