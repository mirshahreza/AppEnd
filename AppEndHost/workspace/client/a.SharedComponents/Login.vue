<template>
    <div class="row h-100 bg-login">
        <div class="col-48 h-100">
            <div class="vertical-center" id="login-box">
                
                <div class="card border-0 shadow-lg animate__animated animate__bounce" style="max-width:320px;width:320px" id="loginCenter">
                    <div class="card-header fb text-center p-0 bg-white" style="background-color: #f0f8ff;">
                        <div class="row">
                            <div class="col-48 p-4">

                                <div style="position:absolute;left:5px;right:5px;margin-top:-37px;" class="card shadow-sm collapse" id="falseMessage">
                                    <div class="text-danger text-center card-body p-0 py-1 fs-d7">
                                        <span>Login failed</span>
                                    </div>
                                </div>

                                <img src="/a..lib/images/AppEnd-Logo-Full.png" class="w-100" />

                            </div>
                        </div>
                    </div>
                    <div class="card-body p-4">
                        <label class="fs-d8">{{shared.translate("UserName")}}</label>
                        <input type="text" class="form-control form-control-sm ae-focus" @keyup.enter="submit" v-model="local.UserName" />
                        <div class="my-2"></div>
                        <label class="fs-d8">{{shared.translate("Password")}}</label>
                        <input type="password" class="form-control form-control-sm" @keyup.enter="submit" v-model="local.Password" />
                        <div class="my-2"></div>
                        <div class="form-check">
                            <input class="form-check-input" type="checkbox" value="" id="chk_Remember" v-model="local.RememberMe" />
                            <label class="form-check-label fs-d8 mb-1" for="chk_Remember">{{shared.translate("RememberMe")}}</label>
                        </div>
                        <div class="my-4"></div>
                        <div class="row">
                            <div class="col-6"></div>
                            <div class="col">
                                <button class="btn btn-primary btn-sm w-100 btn-login" @click="submit">
                                    <i class="fa-solid fa-sign-in-alt"></i><span class="mx-1"></span><span>{{shared.translate("Login")}}</span>
                                </button>
                            </div>
                            <div class="col-6"></div>
                        </div>
                    </div>
                    <div class="card-footer">
                        <div class="p-2 py-1 fw-bolder fs-d6 text-muted text-shadowed text-center">
                            A Full Stack And Low Code System
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </div>

</template>

<script>
    let _this = { cid: "", c: null, inputs: {}, local: {} };
    _this.local = { UserName: "", Password: "", RememberMe: "" };
    export default {
        methods: {
            submit() {
                let r = shared.login(_this.local);
                if (r !== true) {
                    $(".btn-login").find(".fas").removeClass("fa-cog fa-spin").addClass("fa-sign-in-alt");
                    $("#falseMessage").fadeIn("slow");
                    setTimeout(function () {
                        $("#falseMessage").fadeOut("slow");
                    }, 3000);
                } else {
                    refereshPage();
                }
            }
        },
        setup(props) { _this.cid = props['cid']; },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() {
            $(document).ready(function () {
                setTimeout(function () {
                    $(`.ae-focus`).focus();
                }, 500);
            });
        },

    }
</script>