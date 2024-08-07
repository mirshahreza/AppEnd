<template>
    <div class="card h-100 border-0 shadow-lg bg-transparent rounded-0">
        <div class="card-body p-3 pb-4 bg-transparent fs-d8">

            <label class="">{{shared.translate("OldPassword")}}</label>
            <input type="text" name="old_pass" autocomplete="old-password" class="form-control form-control-sm ltr text-center ae-focus" @keyup.enter="submit"
                   v-model="d.oldPass" data-ae-validation-required="true" data-ae-validation-rule="^[^a-zA-Z0-9]?.{1,32}$" />

            <div class="my-3"></div>

            <label class="">{{shared.translate("NewPassword")}}</label>
            <input type="password" name="new_pass" autocomplete="new-password" class="form-control form-control-sm ltr text-center" @keyup.enter="submit"
                   v-model="d.newPass" data-ae-validation-required="true" data-ae-validation-rule="^[^a-zA-Z0-9]?.{8,32}$" />

            <label class="">{{shared.translate("ConfirmNewPassword")}}</label>
            <input type="password" name="renew_pass" autocomplete="renew-password" class="form-control form-control-sm ltr text-center" @keyup.enter="submit"
                   v-model="d.reNewPass" data-ae-validation-required="true" data-ae-validation-rule="^[^a-zA-Z0-9]?.{8,32}$" />
        </div>
        <div class="card-footer p-3 bg-secondary-subtle bg-gradient border-0 rounded-0">
            <div class="row">
                <div class="col-4"></div>
                <div class="col-40">
                    <button class="btn btn-sm btn-primary w-100 py-2" @click="submit">
                        <i class="fa-solid fa-check"></i> <span>{{shared.translate("ChangePassword")}}</span>
                    </button>
                </div>
                <div class="col-4"></div>
            </div>
        </div>
    </div>
</template>
<script>
    let _this = { cid: "", c: null, inputs: {}, d: { oldPass: "", newPass: "", reNewPass: "" }, regulator: null };
    export default {
        methods: {
            submit(e) {
                if (!_this.regulator.isValid()) return;
                if (_this.c.d.newPass.trim() !== _this.c.d.reNewPass.trim()) {
                    showError("Password confirmation is not correct!!!");
                    return;
                }

                rpcAEP("ChangePassword", { "OldPassword": _this.c.d.oldPass, "NewPassword": _this.c.d.newPass }, function (res) {
                    if (res[0].Result === true) {
                        showSuccess(translate("PasswordChanged"));
                        _this.c.close();
                    } else {
                        showError(res.Message);
                    }
                });

            },
            close() { shared.closeComponent(_this.cid); }
        },
        setup(props) {
            _this.cid = props['cid'];
            _this["inputs"] = shared["params_" + _this.cid];
        },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { initVueComponent(_this); },
        props: { cid: String }
    }
</script>
