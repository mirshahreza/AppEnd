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

                                <div class="p-5">
                                    <div class="col-48 my-3">
                                        <label class="fs-d9 text-muted ms-2" for="input_GrandFatherName">{{shared.translate('GrandFatherName')}}</label>
                                        <input type="text" class="form-control form-control-sm" id="input_GrandFatherName"
                                               data-ae-validation-required="true" data-ae-validation-rule=":=i(10,20)">
                                    </div>
                                    <div class="col-48 my-3">
                                        <label class="fs-d9 text-muted ms-2" for="input_GrandFatherName">{{shared.translate('GrandFatherName')}}</label>
                                        <input type="text" class="form-control form-control-sm" id="input_GrandFatherName"
                                               data-ae-validation-required="false" data-ae-validation-rule=":=s(3,20)">


                                        <div class="card">
                                            <div class="card-header">
                                                Validation
                                            </div>
                                            <div class="card-body">
                                                <label class="">int between 10 and 15</label>
                                                <input type="text" class="form-control form-control-sm ae-focus" id="input_Title"
                                                       data-ae-validation-required="true" data-ae-validation-rule=":=i(10,15)" />
                                            </div>
                                            <div class="card-footer">
                                                <button class="btn btn-sm btn-outline-primary" @click="validate">Validate</button>
                                            </div>
                                        </div>


                                    </div>
                                </div>

                            </div>
                            <div class="tab-pane h-100 fade" id="cStep2" role="tabpanel" tabindex="0" data-ae-tab-title="Users List" data-ae-tab-icon="fa-user">
                                <!--<comp-loader src="/a.DbComponents/DefaultRepo_AAA_Users_ReadList" uid="users" />-->
                                Step 2

                            </div>
                            <div class="tab-pane h-100 fade" id="cStep3" role="tabpanel" tabindex="0" data-ae-tab-title="About a test :)" data-ae-tab-icon="fa-address-book">
                                <comp-loader src="/a.UserComponents/aboutEn" uid="aboutEn3" />

                                Step 3

                            </div>
                            <div class="tab-pane h-100 fade" id="cStep4" role="tabpanel" tabindex="0" data-ae-tab-title="Step 4" data-ae-tab-icon="fa-address-book">

                                Step 4

                            </div>
                            <div class="tab-pane h-100 fade" id="cStep5" role="tabpanel" tabindex="0" data-ae-tab-title="Step 5" data-ae-tab-icon="fa-address-book">

                                Step 5

                            </div>
                            <div class="tab-pane h-100 fade" id="cStep6" role="tabpanel" tabindex="0" data-ae-tab-title="Step 6" data-ae-tab-icon="fa-address-book">

                                Step 6

                            </div>
                            <div class="tab-pane h-100 fade" id="cStep7" role="tabpanel" tabindex="0" data-ae-tab-title="Step 7" data-ae-tab-icon="fa-address-book">

                                Step 7

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
    shared.setAppTitle("WizTest");

    let _this = { cid: "", c: null, storedCalls: [], jsonRequestEditor: null, jsonResultEditor: null, appSubTitle: "" };

    export default {
        methods: {

        },
        setup(props) { _this.cid = props['cid']; },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() {

            //let app = Vue.createApp();
            //app.component('comp-loader', loadVM("/a.PublicComponents/baseComponentLoader.vue"));
            //app.config.globalProperties.shared = shared;

        },
        props: { cid: String }
    }

</script>

