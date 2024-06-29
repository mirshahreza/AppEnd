<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-header p-2 bg-success-subtle rounded-0 border-0">
            <div class="hstack gap-1">

                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="refreshSession">
                    <i class="fa-solid fa-fw fa-refresh"></i> <span>{{shared.translate("RefreshSession")}}</span>
                </button>
                <button type="button" class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="showTokenInfo">
                    <i class="fa-solid fa-fw fa-passport"></i> <span>{{shared.translate("MyTokenInfo")}}</span>
                </button>

                <div class="p-0 ms-auto"></div>

                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light"
                     @click="shared.openComponentByEl($event);"
                     data-ae-src="/a.PublicComponents/authChangePassword.vue"
                     data-ae-options='{"title":"ChangePassword","modalSize":"modal-sm","windowSizeSwitchable":false}'>
                    <i class="fa-solid fa-fw fa-key"></i> <span>{{shared.translate("ChangePassword")}}</span>
                </button>


            </div>
        </div>
        <div class="card-body p-1">
            <div class="card h-100 border-light bg-light bg-opacity-75 border-0">
                <div class="card-body rounded rounded-1 border border-3 border-light fs-d9 p-3 bg-transparent scrollable">
                    <div class="container-fluid">
                        <div class="row mt-1">
                            <div class="col-48 text-center align-self-center">
                                <table class="table w-100">
                                    <tr>
                                        <td style="width:175px">
                                            <img :src="shared.getImageURI(shared.getLogedInUserContext()['Picture_FileBody'])" style="width:90%" class="border border-2 rounded rounded-2" v-if="shared.fixNull(shared.getLogedInUserContext()['Picture_FileBody'],'')!==''" />
                                            <img src="/a..lib/images/avatar.png" style="width:90%" class="border border-2 rounded rounded-2" v-else />
                                        </td>
                                        <td class="text-start">
                                            <table class="bg-transparent w-100 mt-1">
                                                <tbody>
                                                    <tr>
                                                        <td style="width:175px;"><i class="fa-solid fa-fw fa-user me-1"></i>{{shared.translate("UserName")}}</td>
                                                        <td>
                                                            <span class="text-dark fw-bold">{{shared.getUserObject()["UserName"]}}</span>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="width:175px;"><i class="fa-solid fa-fw fa-key me-1"></i>{{shared.translate("IsPublicKeyUser")}}</td>
                                                        <td>
                                                            <span class="text-dark fw-bold">{{shared.isPublicKey()}}</span>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="width:175px;"><i class="fa-solid fa-fw fa-key me-1"></i>{{shared.translate("HasPublicKeyRole")}}</td>
                                                        <td>
                                                            <span class="text-dark fw-bold">{{shared.hasPublicKeyRole()}}</span>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="width:175px;"><i class="fa-solid fa-fw fa-user-group me-1"></i>{{shared.translate("Roles")}}</td>
                                                        <td>
                                                            <span class="text-dark fw-bold me-1" v-for="i in shared.getUserObject()['Roles']">[{{i}}]</span>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>

                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                        <div class="row mt-2">
                            <div class="col-48">
                                <div class="card">
                                    <div class="card-header">
                                        <div class="fw-bold text-dark-emphasis"><i class="fa-solid fa-fw fa-check text-success me-1"></i>{{shared.translate("AllowedActions")}}</div>
                                    </div>
                                    <div class="card-body p-2 mb-2">
                                        <div class="card text-bg-light my-1 border border-1 border-secondary-subtle border-0 bg-transparent" v-for="i in alloweds">
                                            <div class="card-header p-2 py-1 fs-d8 bg-transparent">
                                                <span class="fw-bold">{{shared.translate(i.ns)}}, {{shared.translate(i.cs)}}</span>
                                            </div>
                                            <div class="card-body p-2">
                                                <span class="badge fw-bold me-2 text-primary text-hover-success pointer" v-for="m in i.methods" @click="showApiInfo">{{shared.translate(m)}}</span>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="card-footer p-2 bg-light-subtle rounded-bottom-0 fs-d8">
            <component-loader src="/a.PublicComponents/baseAcountActions" uid="baseAcountActions" />
        </div>
    </div>
</template>

<script>
    shared.setAppTitle(shared.translate("MyProfile"));
    let _this = { cid: "", c: null, alloweds: [] };

    export default {
        methods: {
            showTokenInfo() {
                let myInspect = decodeJwt(getUserToken()).payload;
                myInspect["token"] = getUserToken();
                showJson(myInspect);
            },
            showApiInfo() {
                showInfo("Not implemented yet")
            },
            refreshSession() {
                let t1 = getUserToken();
                refereshSession();
                let t2 = getUserToken();
                setTimeout(function () { refereshPage(); }, 200);
            },
            loadPermissions() {
                _this.c.alloweds = makeDotsToTree(shared.getUserAlloweds());
            }
        },
        setup(props) { _this.cid = props['cid']; },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { _this.c.loadPermissions(); },
        props: { cid: String }
    }

</script>