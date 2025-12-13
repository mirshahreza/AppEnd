<template>
    <div class="d-flex flex-column h-100" dir="rtl">
        <div class="border-0 p-0 bg-frame flex-shrink-0">
            <table class="w-100 bg-transparent">
                <tr>
                    <td style="width:55px;min-width:55px;max-width:55px;padding-top:1px;padding-right:18px;height:55px;" class="d-none d-lg-table-cell">
                        <img src="assets/Logo-Only.png" style="width:30px; height:30px;"
                             class="animate__animated animate__slideInDown shadow-sm rounded rounded-circle pointer border-secondary-subtle"
                             @click="shared.openComponentByEl($event);"
                             data-ae-src="components/BaseAbout.vue"
                             data-ae-options='{"showFooter":false,"showHeader":false,"resizable":false,"modalSize":"modal-md","closeByOverlay":true,"placement":"end"}' />
                    </td>
                    <td class="px-3">

                        <div class="input-group input-group-sm border-0 align-items-center">

                            <div class="fw-bold shadow5 fs-d8 d-block d-lg-none ms-3">
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
                                <span class="fw-bold text-success px-2 shadow5 app-title"></span>
                                <span class="fw-bolder text-danger px-2 shadow5 app-subtitle"></span>
                            </div>

                            <input type="text" class="form-control form-control-sm border-0 rounded-0 bg-transparent" disabled />

                            <div class="d-none d-lg-block fs-d8 fw-bold dropdown">
                                <div class="animate__animated animate__slideInDown border border-2 border-0 rounded-2 p-1 text-bg-light shadow-sm pointer" data-bs-toggle="dropdown" aria-expanded="false" style="height:36px;">
                                    <img :src="shared.getImageURI(shared.getLogedInUserContext()['Picture_FileBody'])" class="border border-2 rounded-rounded-2 shadow-sm h-100" v-if="shared.fixNull(shared.getLogedInUserContext()['Picture_FileBody'],'')!==''" />
                                    <img src="/a..lib/images/avatar.png" class="border border-2 rounded-rounded-3 shadow-sm h-100" v-else />
                                    <span class="mx-2" v-if="userName">{{userName}}</span>
                                </div>
                                <ul class="dropdown-menu bg-white shadow-lg border-2 dropdown-menu-end">
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
                            <div class="d-block d-lg-none dropdown">
                                <div class="d-flex align-items-center" data-bs-toggle="dropdown" aria-expanded="false">
                                    <img :src="shared.getImageURI(shared.getLogedInUserContext()['Picture_FileBody'])" class="border rounded-2 shadow-sm" style="height:24px;" v-if="shared.fixNull(shared.getLogedInUserContext()['Picture_FileBody'],'')!==''" />
                                    <img src="/a..lib/images/avatar.png" class="border rounded-3 shadow-sm" style="height:24px;" v-else />
                                    <img src="assets/Logo-Only.png" class="shadow-sm border-0 rounded-2 pointer me-2" style="width:24px;"
                                         data-ae-src="components/BaseAbout.vue"
                                         data-ae-options='{"showFooter":false,"showHeader":false,"resizable":false,"draggable":false,"closeByOverlay":true}' />
                                </div>
                                <ul class="dropdown-menu bg-white shadow-lg border-2 dropdown-menu-end">
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

                        </div>

                    </td>
                </tr>
            </table>
        </div>
        <div class="bg-frame p-0 d-flex flex-grow-1 position-relative rtl-layout-container">
            <main :class="['flex-grow-1', 'h-100', 'me-0', 'overflow-auto', 'position-relative', { 'blurred': isSideMenuVisible && !isDesktop, 'shadow border-end border-2': isDesktop }]" style="z-index:1;">
                <div class="card h-100 border-0 rounded-0">
                    <div class="card-body p-0">
                        <component-loader src="qs:c" cid="dynamicContent" />
                    </div>
                </div>
            </main>
            <div :class="['sidebar-container', 'd-lg-block', { 'open': isSideMenuVisible }]">
                <component-loader src="/a.SharedComponents/SideMenu2Level.vue" uid="sideMenu" />
            </div>
        </div>
    </div>
</template>

<style>
    /* RTL Layout Container - ensures sidebar on right */
    .rtl-layout-container {
        direction: ltr; /* Force LTR for flexbox, so sidebar stays on right */
    }

    .sidebar-container {
        z-index: 1000;
        height: 100%;
        overflow-y: auto;
        transition: transform 0.3s ease-in-out;
        direction: rtl; /* RTL for sidebar content */
    }

    .rtl-layout-container main {
        direction: rtl; /* RTL for main content */
    }

    @media (max-width: 991.98px) { /* Below lg breakpoint */
        .rtl-layout-container {
            direction: rtl; /* Back to RTL for mobile */
        }

        .sidebar-container {
            position: fixed;
            top: 55px;
            bottom: 0;
            right: 0;
            background-color: var(--bs-body-bg);
            box-shadow: -0.5rem 0 1rem rgba(0, 0, 0, 0.15);
            transform: translateX(100%);
            width: 280px;
        }

            .sidebar-container.open {
                transform: translateX(0);
            }
    }

    main.blurred {
        filter: blur(3px);
        pointer-events: none;
    }
</style>

<script>
    export default {
        data() {
            return {
                isSideMenuVisible: false,
                isDesktop: window.innerWidth >= 992,
                themeColor: '#0d6efd'
            };
        },
        methods: {
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
        computed: {
            userName() {
                try {
                    const userObj = shared.getUserObject();
                    if (userObj && userObj["UserName"]) {
                        return userObj["UserName"];
                    }
                    return "";
                } catch (e) {
                    return "";
                }
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
