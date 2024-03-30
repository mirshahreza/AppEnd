<template>
    <div class="card h-100 rounded-bottom-0 rounded-end-0 bg-transparent border-0">
        <div class="card-body p-2">
            <div class="card h-100 border-light bg-light bg-opacity-75 border-0">
                <div class="card-body rounded rounded-2 border border-3 border-light fs-d8 p-0 bg-transparent">

                    Home

                </div>
            </div>

        </div>
        <div class="card-footer p-2 bg-light-subtle rounded-bottom-0 fs-d8">
            <a class="text-secondary text-hover-primary pointer text-decoration-none" href="?c=/.PublicComponents/myProfile">
                <i class="fa-solid fa-fw fa-user"></i> <span>{{shared.translate("Profile")}}</span>
            </a>

            <span class="mx-3">|</span>

            <span class="text-secondary text-hover-primary pointer"
                  @click="shared.openComponentByEl($event);"
                  data-ae-src="/.PublicComponents/authChangePassword.vue"
                  data-ae-options='{"title":"ChangePassword","modalSize":"modal-sm"}'>
                <i class="fa-solid fa-fw fa-key"></i> <span>{{shared.translate("ChangePassword")}}</span>
            </span>

            <span class="mx-3" data-ae-actions="Zzz.AppEndProxy.LoginAs">|</span>

            <span class="text-secondary text-hover-primary pointer" data-ae-actions="Zzz.AppEndProxy.LoginAs"
                  @click="shared.openComponentByEl($event);"
                  data-ae-src="/.PublicComponents/authLoginAs.vue"
                  data-ae-options='{"title":"LoginAs","modalSize":"modal-sm","modalBodyCSS":"bg-primary bg-gradient"}'>
                <i class="fa-solid fa-sign-in-alt"></i> <span>{{shared.translate("LoginAs")}}</span>
            </span>

            <span class="mx-3">|</span>

            <span class="text-secondary text-hover-primary pointer"
                  onclick="shared.logout(function () { goHome(); });">
                <i class="fa-solid fa-sign-out-alt text-danger"></i> <span>{{shared.translate("Logout")}}</span>
            </span>

        </div>
    </div>
</template>


<script>
    shared.setAppTitle(shared.translate("Home"));
    let _this = { cid: "", c: null };

    export default {
        methods: {
        },
        setup(props) { _this.cid = props['cid']; },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() {  },
        props: { cid: String }
    }
</script>