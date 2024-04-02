<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-body p-1">
            <div class="card h-100 border-light bg-light bg-opacity-75 border-0">
                <div class="card-body rounded rounded-1 border border-3 border-light fs-d9 p-3 bg-transparent scrollable">
                    <div class="container-fluid">
                        <div class="row mt-1">
                            <div class="col-48 col-md-6 text-center align-self-center">
                                <img :src="shared.getImageURI(shared.getLogedInUserContext()['Picture_FileBody'])" style="width:100%" class="border border-2 rounded rounded-2" v-if="shared.fixNull(shared.getLogedInUserContext()['Picture_FileBody'],'')!==''" />
                                <img src="/..lib/images/avatar.png" style="width:100%" class="border border-2 rounded rounded-2" v-else />
                            </div>
                            <div class="col-48 col-md-42 align-self-center">
                                <table class="bg-transparent w-100">
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

                                <div class="btn btn-sm btn-link m-2 mb-0 text-decoration-none" @click="refreshSession">
                                    <i class="fa-solid fa-refresh"></i> {{shared.translate("RefreshSession")}}
                                </div>
                                <br />
                                <div class="btn btn-sm btn-link m-2 mt-0 text-decoration-none" @click="showTokenInfo">
                                    <i class="fa-solid fa-passport"></i> {{shared.translate("ShowTokenInfo")}}
                                </div>


                            </div>
                           

                        </div>
                        <div class="row mt-2">
                            <div class="col-48">
                                <div class="card">
                                    <div class="card-header">
                                        <div class="fw-bold text-dark-emphasis"><i class="fa-solid fa-fw fa-check me-1"></i>{{shared.translate("AllowedActions")}}</div>
                                    </div>
                                    <div class="card-body p-2 mb-2">
                                        <div class="card text-bg-light my-1 border border-1 border-secondary-subtle border-0 bg-transparent" v-for="i in alloweds">
                                            <div class="card-header p-2 py-1 fs-d8 bg-transparent">
                                                <span class="fw-bold">{{shared.translate(i.ns)}}, {{shared.translate(i.cs)}}</span>
                                            </div>
                                            <div class="card-body p-2">
                                                <span class="badge fw-bold me-2 text-success text-hover-primary pointer" v-for="m in i.methods" @click="showApiInfo">{{shared.translate(m)}}</span>
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
            
            <span class="text-secondary text-hover-primary pointer"
                  @click="shared.openComponentByEl($event);"
                  data-ae-src="/.publiccomponents/authChangePassword.vue"
                  data-ae-options='{"title":"ChangePassword","modalSize":"modal-sm"}'>
                <i class="fa-solid fa-fw fa-key"></i> <span>{{shared.translate("ChangePassword")}}</span>
            </span>

            <span class="mx-3" data-ae-actions="Zzz.AppEndProxy.LoginAs">|</span>

            <span class="text-secondary text-hover-primary pointer" data-ae-actions="Zzz.AppEndProxy.LoginAs"
                  @click="shared.openComponentByEl($event);"
                  data-ae-src="/.publiccomponents/authLoginAs.vue"
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

                //showJson({ "t1": t1, "t2": t2 });
                //return;

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