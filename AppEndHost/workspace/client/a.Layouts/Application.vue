<template>
    <div class="d-flex flex-column h-100" :dir="layoutDir">
        <div class="border-0 p-0 bg-frame flex-shrink-0">
            <table class="w-100 bg-transparent">
                <tr>
                    <td style="width:85px;min-width:85px;max-width:85px;padding-top:1px;height:55px;align-content:center" class="d-none d-lg-table-cell text-center">
                        <img src="assets/Logo-Only.png" style="width:38px; height:38px;"
                             class="animate__animated animate__slideInDown shadow-sm rounded rounded-circle pointer border-secondary-subtle"
                             @click="shared.openComponentByEl($event);"
                             data-ae-src="components/BaseAbout.vue"
                             data-ae-options='{"showFooter":false,"showHeader":false,"resizable":false,"modalSize":"modal-md","closeByOverlay":true,"placement":"end"}' />
                    </td>
                    <td class="px-3">
                        
                        <div class="input-group input-group-sm border-0 align-items-center">

                            <div class="fw-bold shadow5 fs-d8 d-block d-lg-none me-3">
                                <span @click="toggleSideMenu">
                                    <i class="fa-solid fa-bars"></i>
                                </span>
                            </div>
                            <div class="fw-bold shadow5 fs-d8" v-if="shared.fixNull(shared.getQueryString('c'),'')!=='' && shared.fixNull(shared.getQueryString('c'),'').toLowerCase().indexOf('home')===-1">
                                <a href="?c=components/BaseHome" class="text-decoration-none shadow5">
                                    <i class="fa-solid fa-fw fa-home fa-lg"></i>
                                </a>
                            </div>

                            <div class="fw-bold mx-2 shadow5 fs-d8" v-if="shared.fixNull(shared.getQueryString('c'),'')!=='' && shared.fixNull(shared.getQueryString('c'),'').toLowerCase().indexOf('home')===-1">/</div>

                            <div class="fs-d8">
                                <span class="fw-bold text-secondary shadow5 app-title"></span>
                                <span class="fw-bolder text-dark shadow5 app-subtitle"></span>
                            </div>

                            <input type="text" class="form-control form-control-sm border-0 rounded-0 bg-transparent" disabled />

                            <!-- Theme Picker -->
                            <div class="d-none d-lg-block fs-d7 fw-bold mx-1 mb-0 animate__animated animate__slideInDown dropdown">
                                <component-loader src="/a.SharedComponents/ThemePicker.vue" uid="themePicker" />
                            </div>

                            <div class="d-none d-lg-block fs-d7 fw-bold mx-1 mb-0 animate__animated animate__slideInDown dropdown">
                                <div class="dropdown">
                                    <div class="profile-button animate__animated animate__slideInDown pointer" data-bs-toggle="dropdown" aria-expanded="false">
                                        <img :src="shared.getImageURI(shared.getLogedInUserContext()['Picture_FileBody_xs'])"
                                             class="profile-avatar" style="width:30px !important;height:30px !important;"
                                             v-if="shared.fixNull(shared.getLogedInUserContext()['Picture_FileBody_xs'],'')!==''" />
                                        <img src="/a..lib/images/avatar.png"
                                             class="profile-avatar"
                                             v-else />

                                        <span class="vr mx-1"></span>
                                        <span class="profile-username ms-1">{{shared.getUserObject()["UserName"]}}</span>
                                    </div>
                                    <ul class="dropdown-menu bg-elevated shadow-lg border-2">
                                        <li>
                                            <a class="dropdown-item p-1 px-3 fs-d7 text-secondary hover-primary pointer" href="?c=/a.SharedComponents/MyProfile">
                                                <i class="fa-solid fa-fw fa-user text-secondary"></i> <span>{{shared.translate("Profile")}}</span>
                                            </a>
                                        </li>
                                        <li><hr class="dropdown-divider"></li>
                                        <li>
                                            <span class="dropdown-item p-1 px-3 fs-d7 text-secondary hover-primary pointer" @click="refreshSession">
                                                <i class="fa-solid fa-fw fa-user text-secondary"></i> <span>{{shared.translate("RefreshSession")}}</span>
                                            </span>
                                        </li>
                                        <li><hr class="dropdown-divider"></li>
                                        <li>
                                            <span class="dropdown-item p-1 px-3 fs-d7 text-secondary hover-primary pointer"
                                                  @click="shared.openComponentByEl($event);"
                                                  data-ae-src="/a.SharedComponents/AuthChangePassword.vue"
                                                  data-ae-options='{"title":"ChangePassword","modalSize":"modal-md","resizable":false,"draggable":false,"closeByOverlay":true}'>
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
                            </div>

                            
                            <div class="d-block d-lg-none dropdown">
                                <div class="d-flex align-items-center" data-bs-toggle="dropdown" aria-expanded="false">
                                    <img :src="shared.getImageURI(shared.getLogedInUserContext()['Picture_FileBody_xs'])" :class="mobileImageClasses" style="height:24px;" v-if="shared.fixNull(shared.getLogedInUserContext()['Picture_FileBody_xs'],'')!==''" />
                                    <img src="/a..lib/images/avatar.png" :class="mobileImageClasses" style="height:24px;" v-else />
                                    <img src="assets/Logo-Only.png" :class="mobileLogoClasses" style="width:24px;"
                                         data-ae-src="components/BaseAbout.vue"
                                         data-ae-options='{"showFooter":false,"showHeader":false,"resizable":false,"draggable":false,"closeByOverlay":true}' />
                                </div>
                                <ul :class="dropdownMenuClasses">
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
                                              data-ae-options='{"title":"ChangePassword","modalSize":"modal-md","resizable":false,"draggable":false,"closeByOverlay":true}'>
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

                        </div>

                    </td>
                </tr>
            </table>
        </div>
        <div class="bg-frame p-0 d-flex flex-grow-1 position-relative">
            <div :class="['sidebar-container', 'd-lg-block', { 'open': isSideMenuVisible }]">
                <component-loader src="/a.SharedComponents/SideMenu2Level.vue" uid="sideMenu" />
            </div>
            <main :class="mainClasses" style="z-index:1;">
                <div :class="cardClasses">
                    <div class="card-body border-0 p-0">
                        <component-loader src="qs:c" cid="dynamicContent" />
                    </div>
                </div>
            </main>
        </div>
    </div>
</template>

<script>
    export default {
        data() {
            return {
                isSideMenuVisible: false,
                isDesktop: window.innerWidth >= 992,
                themeColor: '#0d6efd'
            };
        },
        computed: {
            layoutDir() {
                const appConfig = this.shared.getAppConfig();
                return appConfig.dir || 'ltr';
            },
            isRTL() {
                return this.layoutDir === 'rtl';
            },
            mobileImageClasses() {
                return this.isRTL 
                    ? 'border rounded-start-3 shadow-sm' 
                    : 'border rounded-2 shadow-sm';
            },
            mobileLogoClasses() {
                return this.isRTL
                    ? 'shadow-sm border-0 rounded-start-2 pointer me-2'
                    : 'shadow-sm border-0 rounded-2 pointer ms-2';
            },
            dropdownMenuClasses() {
                return this.isRTL
                    ? 'dropdown-menu dropdown-menu-start bg-elevated shadow-lg border-2'
                    : 'dropdown-menu bg-elevated shadow-lg border-2';
            },
            mainClasses() {
                const baseClasses = ['flex-grow-1', 'h-100', 'overflow-auto', 'position-relative','rounded-start-2 rounded-bottom-0'];
                const marginClass = this.isRTL ? 'me-0' : 'ms-0';
                const conditionalClasses = { 
                    'blurred': this.isSideMenuVisible && !this.isDesktop, 
                    'shadow border-start border-2': this.isDesktop 
                };
                return [...baseClasses, marginClass, conditionalClasses];
            },
            cardClasses() {
                return this.isRTL
                    ? 'card h-100 border-0 rounded-start-2 rounded-bottom-0'
                    : 'card h-100 border-0 rounded-start-2 rounded-bottom-0';
            }
        },
        methods: {
            refreshSession() {
                let t1 = getUserToken();
                refereshSession();
                let t2 = getUserToken();
                setTimeout(function () { refereshPage(); }, 200);
            },
            toggleSideMenu() {
                this.isSideMenuVisible = !this.isSideMenuVisible;
            },
            hideSideMenu(event) {
                if (this.isSideMenuVisible && !this.isDesktop) {
                    const sidebar = this.$el.querySelector('.sidebar-container');
                    const toggleButton = this.$el.querySelector('.fa-bars');
                    if (sidebar && !sidebar.contains(event.target) && toggleButton && !toggleButton.contains(event.target)) {
                        this.isSideMenuVisible = false;
                    }
                }
            },
            handleResize() {
                this.isDesktop = window.innerWidth >= 992;
                if (this.isDesktop) {
                    this.isSideMenuVisible = false; 
                }
            },
            changeThemeColor(event) {
                const color = event.target.value;
                this.themeColor = color;
                document.documentElement.style.setProperty('--bs-primary', color);

                let r = 0, g = 0, b = 0;
                if (color.length == 4) {
                    r = parseInt(color[1] + color[1], 16);
                    g = parseInt(color[2] + color[2], 16);
                    b = parseInt(color[3] + color[3], 16);
                } else if (color.length == 7) {
                    r = parseInt(color.substring(1, 3), 16);
                    g = parseInt(color.substring(3, 5), 16);
                    b = parseInt(color.substring(5, 7), 16);
                }
                document.documentElement.style.setProperty('--bs-primary-rgb', `${r},${g},${b}`);
                
                const primarySubtle = this.blendColors(color, '#FFFFFF', 0.9);
                document.documentElement.style.setProperty('--bs-primary-subtle-light', primarySubtle);
            },
            blendColors(color1, color2, percentage) {
                const c1 = color1.substring(1);
                const c2 = color2.substring(1);

                const r1 = parseInt(c1.substring(0, 2), 16);
                const g1 = parseInt(c1.substring(2, 4), 16);
                const b1 = parseInt(c1.substring(4, 6), 16);

                const r2 = parseInt(c2.substring(0, 2), 16);
                const g2 = parseInt(c2.substring(2, 4), 16);
                const b2 = parseInt(c2.substring(4, 6), 16);

                const r = Math.round(r1 * percentage + r2 * (1 - percentage));
                const g = Math.round(g1 * percentage + g2 * (1 - percentage));
                const b = Math.round(b1 * percentage + b2 * (1 - percentage));

                return `#${r.toString(16).padStart(2, '0')}${g.toString(16).padStart(2, '0')}${b.toString(16).padStart(2, '0')}`;
            }
        },
        mounted() {
            document.addEventListener('click', this.hideSideMenu);
            window.addEventListener('resize', this.handleResize);
        },
        beforeUnmount() {
            document.removeEventListener('click', this.hideSideMenu);
            window.removeEventListener('resize', this.handleResize);
        }
    }
</script>

