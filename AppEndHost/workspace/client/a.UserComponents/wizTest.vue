<template>
    <div class="container-fluid h-100">
        <div class="row h-100">
            <div class="col-48 h-100">

                <div class="card border-0 rounded-0 h-100">
                    <div class="card-header border-0 rounded-0 bg-success-subtle">
                        <div class="fs-1d1 fw-bold shadow5">
                            Test Wizard
                        </div>
                        <div class="fs-9 text-secondary">
                            This location is for some notes or shared content
                        </div>
                    </div>

                    <div class="card-header bg-primary-subtle opacity-75 border-0 p-0">
                        <div data-ae-widget="bsTabsAutoNav" data-ae-widget-options='{"tabsContentsId":"wizMain","justAllowByBackNext":true}'>
                            <ul class="nav nav-underline nav-justified">
                                <li class="nav-item"><button class="nav-link"><i class="fa-solid fa-circle-notch fa-spin"></i></button></li>
                            </ul>
                        </div>
                    </div>
                    <div class="card-body p-0">

                        <div class="tab-content h-100" id="wizMain">
                            <div class="tab-pane h-100 fade show active" id="cStep1" role="tabpanel" tabindex="0" data-ae-tab-title="Roles List" data-ae-tab-icon="fa-users">
                                <!--<comp-loader src="/a.DbComponents/DefaultRepo_AAA_Roles_ReadList" uid="roles" />-->

                                Step 1

                            </div>
                            <div class="tab-pane h-100 fade" id="cStep2" role="tabpanel" tabindex="0" data-ae-tab-title="Users List" data-ae-tab-icon="fa-user">
                                <!--<comp-loader src="/a.DbComponents/DefaultRepo_AAA_Users_ReadList" uid="users" />-->
                            
                                Step 2
                            
                            </div>
                            <div class="tab-pane h-100 fade" id="cStep3" role="tabpanel" tabindex="0" data-ae-tab-title="About a test :)" data-ae-tab-icon="fa-address-book">
                                <comp-loader src="/a.UserComponents/aboutEn" uid="aboutEn3" />

                                Step 3

                            </div>
                        </div>

                    </div>
                    <div class="card-footer">

                        <div data-ae-widget="bsTabsAutoNav" data-ae-widget-options='{"tabsContentsId":"wizMain","mode":"back-next","justAllowByBackNext":true}'>
                            <table class="w-100 text-center">
                                <tr>
                                    <td style="width:100px">
                                        <button class="btn btn-sm btn-link fw-bold text-decoration-none bg-hover-light w-100">
                                            <i class="fa-solid fa-circle-notch fa-spin"></i>
                                        </button>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </div>
</template>
<script>
    shared.setAppTitle("API Call Console");

    let _this = { cid: "", c: null, storedCalls: [], jsonRequestEditor: null, jsonResultEditor: null, appSubTitle: "" };

    export default {
        methods: {
            
        },
        setup(props) { _this.cid = props['cid']; },
        data() {return _this;},
        created() { _this.c = this; },
        mounted() {

            //let app = Vue.createApp();
            //app.component('comp-loader', loadVM("/a.PublicComponents/baseComponentLoader.vue"));
            //app.config.globalProperties.shared = shared;

        },
        props: { cid: String }
    }

</script>

