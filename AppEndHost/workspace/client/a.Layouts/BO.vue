<template>
    <div class="card border-0 h-100">
        <div class="card-header border-0 p-0 bg-frame">
            <table class="w-100 bg-transparent">
                <tr>
                    <td style="width:55px;min-width:55px;max-width:55px;padding-top:1px;padding-left:15px;height:55px;">
                        <img src="assets/Logo-Only.png" style="width:30px; height:30px;"
                             class="animate__animated animate__slideInDown shadow rounded rounded-circle pointer border-secondary-subtle"
                             @click="shared.openComponentByEl($event);"
                             data-ae-src="components/BaseAbout.vue"
                             data-ae-options='{"showFooter":false,"showHeader":false,"resizable":false,"modalSize":"modal-lg","closeByOverlay":true}' />
                    </td>
                    <td class="px-3">
                        
                        <div class="input-group input-group-sm border-0">

                            <div class="fw-bold shadow5 fs-d8 d-sm-block d-md-none d-lg-none me-3">
                                <span @click="shared.openComponentByEl($event);" data-ae-src="/a.SharedComponents/SideMenuBranded.vue"
                                      data-ae-options='{"showFooter":false,"showHeader":false,"animation":"animate__animated animate__slideIn$DirHand$","modalSize":"modal-fullscreen","modalMargin":"pe-5","closeByOverlay":true}'>
                                    <i class="fa-solid fa-bars"></i>
                                </span>
                            </div>

                            <div class="fw-bold shadow5 fs-d8 mt-2" v-if="shared.fixNull(shared.getQueryString('c'),'')!=='' && shared.fixNull(shared.getQueryString('c'),'').toLowerCase().indexOf('home')===-1">
                                <a href="?c=components/BaseHome" class="text-decoration-none shadow5">
                                    <i class="fa-solid fa-fw fa-home fa-xl"></i>
                                </a>
                            </div>

                            <div class="fw-bold mx-2 shadow5 fs-d8 mt-2" v-if="shared.fixNull(shared.getQueryString('c'),'')!=='' && shared.fixNull(shared.getQueryString('c'),'').toLowerCase().indexOf('home')===-1">/</div>

                            <div class="fs-d8 mt-2">
                                <span class="fw-bold text-success px-2 shadow5 app-title"></span>
                                <span class="fw-bolder text-danger px-2 shadow5 app-subtitle"></span>
                            </div>

                            <input type="text" class="form-control form-control-sm border-0 rounded-0 bg-transparent" disabled />
                            
                            <div class="d-none d-md-block d-lg-block fs-d8 fw-bold">
                                <div class="animate__animated animate__slideInDown border border-2 border border-0 rounded-2 p-1 text-bg-light shadow-sm pointer" data-bs-toggle="dropdown" aria-expanded="false" style="height:36px;">
                                    <img :src="shared.getImageURI(shared.getLogedInUserContext()['Picture_FileBody'])" class="border border-2 rounded rounded-2 shadow-sm h-100" v-if="shared.fixNull(shared.getLogedInUserContext()['Picture_FileBody'],'')!==''" />
                                    <img src="/a..lib/images/avatar.png" class="border border-2 rounded rounded-3 shadow-sm h-100" v-else />
                                    <span class="mx-2">{{shared.getUserObject()["UserName"]}}</span>
                                </div>
                                <ul class="dropdown-menu bg-white shadow-lg border-2">
                                    <li>
                                        <a class="dropdown-item p-1 px-3 fs-d7 text-secondary hover-primary pointer" href="?c=/a.SharedComponents/MyProfile">
                                            <i class="fa-solid fa-fw fa-user text-secondary"></i> <span>{{shared.translate("Profile")}}</span>
                                        </a>
                                    </li>
                                    <li><hr class="dropdown-divider"></li>
                                    <li>
                                        <span class="dropdown-item p-1 px-3 fs-d7 text-secondary hover-primary pointer"
                                              @click="shared.openComponentByEl($event);"
                                              data-ae-src="/a.SharedComponents/AuthChangePassword.vue"
                                              data-ae-options='{"title":"ChangePassword","modalSize":"modal-sm","resizable":false,"draggable":false,"closeByOverlay":true}'>
                                            <i class="fa-solid fa-fw fa-key text-secondary"></i> <span>{{shared.translate("ChangePassword")}}</span>
                                        </span>
                                    </li>
                                    <li data-ae-allowed-roles="admin" data-ae-actions="Zzz.AppEndProxy.LoginAs">
                                        <span class="dropdown-item p-1 px-3 fs-d7 text-secondary hover-primary pointer"
                                              @click="shared.openComponentByEl($event);"
                                              data-ae-src="/a.SharedComponents/AuthLoginAs.vue"
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
                            <div class="d-sm-block d-md-none d-lg-none">
                                <img src="assets/Logo-Only.png" class="shadow shadow-sm border border-0 rounded rounded-2 pointer" style="width:24px;"
                                     @click="shared.openComponentByEl($event);"
                                     data-ae-src="components/BaseAbout.vue"
                                     data-ae-options='{"showFooter":false,"showHeader":false,"resizable":false,"draggable":false,"closeByOverlay":true}' />
                            </div>

                        </div>

                    </td>
                </tr>
            </table>
        </div>
        <div class="card-body bg-frame p-0 d-flex">
            <div>
                <component-loader src="/a.SharedComponents/SideMenu2Level.vue" uid="sideMenu" />
            </div>
            <main class="flex-grow-1 h-100 ms-0">
                <div class="card h-100 border-0 shadow rounded-0">
                    <div class="card-body p-0">
                        <component-loader src="qs:c" cid="dynamicContent" />
                    </div>
                </div>
            </main>
        </div>
    </div>
</template>



