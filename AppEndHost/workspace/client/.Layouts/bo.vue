<template>
    <div class="row bg-page h-100">
        <div class="col-6 h-100">
            <div class="card h-100 rounded-0 border-0 bg-transparent">

                <div class="card-header border-0 rounded-0 bg-transparent text-center d-none d-md-block d-lg-block">
                    <img src="/..lib/images/AppEnd-Logo-Full.png" @click="shared.openComponentByEl($event);"
                         data-ae-src="/.PublicComponents/baseAbout.vue" data-ae-options='{"showFooter":false,"showHeader":false,"resizable":false,"draggable":false,"closeByOverlay":true}'
                         class="animate__animated animate__slideInDown shadow shadow-sm border border-0 rounded rounded-2 pointer mt-3" style="width:75%;" />
                </div>

                <div class="card-header border-0 rounded-0 bg-transparent text-center p-2 d-sm-block d-md-none d-lg-none">
                    <img src="/..lib/images/AppEnd-Logo-Only.png" @click="shared.openComponentByEl($event);"
                         data-ae-src="/.PublicComponents/baseAbout.vue" data-ae-options='{"showFooter":false,"showHeader":false,"resizable":false,"draggable":false,"closeByOverlay":true}'
                         class="animate__animated animate__slideInDown border border-o rounded rounded-1 pointer w-100" />
                </div>

                <div class="card-body p-0">
                    <component-loader src="/.SharedComponents/baseSideMenu" uid="sideMenu" />
                </div>

            </div>
        </div>
        <div class="col-42 h-100">
            <div class="card h-100 rounded-0 border-0 bg-transparent">
                <div class="card-header border-0 bg-transparent">
                    <div class="input-group input-group-sm border-0">
                        <div style="padding-top:3px;" class="d-sm-block d-md-none d-lg-none">
                            <span class="fw-bold text-secondary px-2 shadow5 fs-d8 app-title"></span>
                            <span class="fw-bolder text-success px-2 shadow5 fs-d8 app-subtitle"></span>
                            <span class="px-1 fs-d7" id="appMessage"></span>
                        </div>
                        <div style="padding-top:2px;" class="d-none d-md-block d-lg-block">
                            <span class="fw-bold text-secondary px-2 shadow5 fs-1d1 app-title"></span>
                            <span class="fw-bolder text-success px-2 shadow5 fs-1d1 app-subtitle"></span>
                            <span class="px-1 fs-d9" id="appMessage"></span>
                        </div>
                        <input type="text" class="form-control form-control-sm border-0 rounded-0 bg-transparent" disabled />
                        <div class="d-none d-md-block d-lg-block">
                            <div class="animate__animated animate__slideInDown border border-2 badge border border-0 rounded-3 p-0 text-bg-light shadow-sm pointer" data-bs-toggle="dropdown" aria-expanded="false" style="height:31px;">
                                <img :src="shared.getImageURI(shared.getLogedInUserContext()['Picture_FileBody'])" class="border border-2 rounded rounded-3 shadow-sm h-100" v-if="shared.fixNull(shared.getLogedInUserContext()['Picture_FileBody'],'')!==''" />
                                <img src="/..lib/images/avatar.png" class="border border-2 rounded rounded-3 shadow-sm h-100" v-else />
                                <span class="mx-2">{{shared.getUserObject()["UserName"]}}</span>
                            </div>
                            <ul class="dropdown-menu bg-white shadow-lg border-2">
                                <li>
                                    <a class="dropdown-item p-1 px-3 fs-d7 text-secondary hover-primary pointer" href="?c=/.PublicComponents/myProfile">
                                        <i class="fa-solid fa-fw fa-user text-secondary"></i> <span>{{shared.translate("Profile")}}</span>
                                    </a>
                                </li>
                                <li><hr class="dropdown-divider"></li>
                                <li>
                                    <span class="dropdown-item p-1 px-3 fs-d7 text-secondary hover-primary pointer"
                                          @click="shared.openComponentByEl($event);"
                                          data-ae-src="/.PublicComponents/authChangePassword.vue"
                                          data-ae-options='{"title":"ChangePassword","modalSize":"modal-sm","resizable":false,"draggable":false,"closeByOverlay":true}'>
                                        <i class="fa-solid fa-fw fa-key text-secondary"></i> <span>{{shared.translate("ChangePassword")}}</span>
                                    </span>
                                </li>
                                <li data-ae-allowed-roles="admin" data-ae-actions="Zzz.AppEndProxy.LoginAs">
                                    <span class="dropdown-item p-1 px-3 fs-d7 text-secondary hover-primary pointer"
                                          @click="shared.openComponentByEl($event);"
                                          data-ae-src="/.PublicComponents/authLoginAs.vue"
                                          data-ae-options='{"title":"LoginAs","modalSize":"modal-sm","resizable":false,"draggable":false,"closeByOverlay":true}'>
                                        <i class="fa-solid fa-sign-in-alt text-warning"></i> <span>{{shared.translate("LoginAs")}}</span>
                                    </span>
                                </li>
                                <li>
                                    <span class="dropdown-item p-1 px-3 fs-d7 text-secondary hover-primary pointer"
                                          onclick="shared.logout(function () { goHome(); });">
                                        <i class="fa-solid fa-sign-out-alt text-danger"></i> <span>{{shared.translate("Logout")}}</span>
                                    </span>
                                </li>
                            </ul>
                        </div>
                    </div>
                </div>
                <div class="card-body p-0">
                    <div class="h-100 position-relative">
                        <div class="fill bg-content rounded rounded-2 rounded-bottom-0 rounded-end-0"></div>
                        <div class="fill bg-content-color overflow-hidden  rounded rounded-2 rounded-bottom-0 rounded-end-0">
                            <component-loader src="qs:c" uid="dynamicContent" cid="dynamicContent" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
