<template>
    <div class="row">
        <div class="col-48">
            <a class="text-secondary text-hover-primary pointer text-decoration-none" href="?c=/a.SharedComponents/MyProfile">
                <i class="fa-solid fa-fw fa-user"></i> <span>{{shared.translate("Profile")}}</span>
            </a>
            <span class="mx-3" data-ae-actions="Zzz.AppEndProxy.LoginAs">|</span>
            <span class="text-secondary text-hover-primary pointer" data-ae-actions="Zzz.AppEndProxy.LoginAs"
                  @click="shared.openComponentByEl($event);"
                  data-ae-src="/a.SharedComponents/AuthLoginAs.vue"
                  data-ae-options='{"title":"LoginAs","modalSize":"modal-sm"}'>
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

