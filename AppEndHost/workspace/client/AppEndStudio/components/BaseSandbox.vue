<template>
    <div class="container-fluid h-100 p-5 scrollable">

        <div class="card my-2">
            <div class="card-body text-center">

                <div class="p-3 mb-2 bg-primary text-white">.bg-primary</div>

                <div class="p-3 mb-2 bg-primary-subtle text-primary-emphasis">.bg-primary-subtle</div>
                <div class="p-3 mb-2 bg-primary-subtle text-primary-emphasis bg-gradient">.bg-primary-subtle .bg-gradient</div>

                <div class="p-3 mb-2 bg-secondary text-white">.bg-secondary</div>
                <div class="p-3 mb-2 bg-secondary-subtle text-secondary-emphasis">.bg-secondary-subtle</div>

                <div class="row">
                    <div class="col">
                        <div class="p-3 mb-2 bg-success text-white">
                            .bg-success
                            <br />
                            .bg-success
                            <br />
                            .bg-success
                            <br />
                            .bg-success
                            <br />
                            .bg-success
                            <br />
                            .bg-success
                            <br />
                            .bg-success
                            <br />
                        </div>
                    </div>
                    <div class="col">
                        <div class="p-3 mb-2 bg-success text-white bg-gradient">
                            .bg-success .bg-gradient
                            <br />
                            .bg-success .bg-gradient
                            <br />
                            .bg-success .bg-gradient
                            <br />
                            .bg-success .bg-gradient
                            <br />
                            .bg-success .bg-gradient
                            <br />
                            .bg-success .bg-gradient
                            <br />
                            .bg-success .bg-gradient
                            <br />
                        </div>
                    </div>
                </div>

                <div class="p-3 mb-2 bg-success-subtle text-success-emphasis">.bg-success-subtle</div>
                <div class="p-3 mb-2 bg-danger text-white">.bg-danger</div>
                <div class="p-3 mb-2 bg-danger-subtle text-danger-emphasis">.bg-danger-subtle</div>
                <div class="p-3 mb-2 bg-warning text-dark">.bg-warning</div>
                <div class="p-3 mb-2 bg-warning-subtle text-warning-emphasis">.bg-warning-subtle</div>

                <div class="row">
                    <div class="col">
                        <div class="p-3 mb-2 bg-info text-dark">.bg-info</div>
                    </div>
                    <div class="col">
                        <div class="p-3 mb-2 bg-info text-dark bg-gradient">.bg-info .bg-gradient</div>
                    </div>
                </div>

                <div class="p-3 mb-2 bg-info-subtle text-info-emphasis">.bg-info-subtle</div>
                <div class="p-3 mb-2 bg-light text-dark">.bg-light</div>
                <div class="p-3 mb-2 bg-light-subtle text-light-emphasis">.bg-light-subtle</div>
                <div class="p-3 mb-2 bg-dark text-white">.bg-dark</div>
                <div class="p-3 mb-2 bg-dark-subtle text-dark-emphasis">.bg-dark-subtle</div>
                <div class="p-3 mb-2 bg-body-secondary">.bg-body-secondary</div>
                <div class="p-3 mb-2 bg-body-tertiary">.bg-body-tertiary</div>
                <div class="p-3 mb-2 bg-body text-body">.bg-body</div>
                <div class="p-3 mb-2 bg-black text-white">.bg-black</div>
                <div class="p-3 mb-2 bg-white text-dark">.bg-white</div>
                <div class="p-3 mb-2 bg-transparent text-body">.bg-transparent</div>


            </div>
        </div>


        <div class="card my-2">
            <div class="card-body text-center">
                <i class="fa-solid fa-fw fa-a text-danger"></i>
                <i class="fa-solid fa-fw fa-p text-danger"></i>
                <i class="fa-solid fa-fw fa-p text-danger"></i>
                <i class="fa-solid fa-fw fa-e text-danger"></i>
                <i class="fa-solid fa-fw fa-n text-danger"></i>
                <i class="fa-solid fa-fw fa-d text-danger"></i>
            </div>
        </div>

        <div class="card my-2">
            <div class="card-body text-center">
                <button class="btn btn-link"
                        @click="shared.openComponentByEl($event);"
                        data-ae-src="/a.SharedComponents/WizTest.vue"
                        data-ae-options='{"resizable":false,"draggable":false,"closeByOverlay":true,"modalSize":"modal-fullscreen","title":"Test Wizard","windowSizeSwitchable":false}'>
                    Start Wizard
                </button>
                &nbsp;&nbsp;&nbsp;
                <div class="badge border border-2 rounded-4 p-0 text-bg-light shadow-sm" style="height:30px;">
                    <img :src="shared.getImageURI(shared.getLogedInUserContext()['Picture_FileBody'])"
                         class="border border-2 rounded rounded-4 shadow-sm h-100" />
                    <span class="mx-2">Mohammad</span>
                </div>
            </div>
        </div>

        <div class="card my-2">
            <div class="card-header">
                Validation
            </div>
            <div class="card-body">
                <label class="">int between 10 and 15</label>
                <input type="text" class="form-control form-control-sm ae-focus" data-ae-validation-required="true" data-ae-validation-rule=":=i(10,15)" />

                <br />

                <label class="">Starts with 07, total chars 11 and all chars numbers basedon regular expression : ^07\d{9}$</label>
                <input type="text" class="form-control form-control-sm ae-focus" data-ae-validation-required="true" data-ae-validation-rule="^07\d{9}$" />

            </div>
            <div class="card-footer">
                <button class="btn btn-sm btn-outline-primary" @click="validate">Validate</button>
            </div>
        </div>

        <div class="card my-2">
            <div class="card-header">
                PromptEx
            </div>
            <div class="card-body">
                <div class="row m-5">
                    <div class="col-6">
                        <button class="btn btn-sm btn-success w-100" @click="showPromptEx">Show PromptEx</button>
                    </div>
                </div>
            </div>
        </div>

        <div class="card my-2">
            <div class="card-header">
                Show Message
            </div>
            <div class="card-body">
                <div class="row m-5">
                    <div class="col-6">
                        <button class="btn btn-sm btn-success w-100" @click="mShowSuccess">ShowSuccess</button>
                    </div>
                    <div class="col-6">
                        <button class="btn btn-sm btn-info w-100" @click="mShowInfo">ShowInfo</button>
                    </div>
                    <div class="col-6">
                        <button class="btn btn-sm btn-danger w-100" @click="mShowError">ShowError</button>
                    </div>
                    <div class="col-6">
                        <button class="btn btn-sm btn-warning w-100" @click="mShowWarning">ShowWarning</button>
                    </div>
                </div>
            </div>
        </div>

        <div class="card my-2">
            <div class="card-header">
                Boxes & Colors
            </div>
            <div class="card-body">
                <div class="row m-5">
                    <div class="col"></div>
                    <div class="col-12">
                        <div class="card shadow-sm" style="background-color:var(--bs-gray)">
                            <div class="card-body">
                                <b>Not bg-gradient</b><br />
                                this is a test<br />
                                this is a test<br />
                                this is a test<br />
                                this is a test<br />
                            </div>
                        </div>
                    </div>
                    <div class="col-12">
                        <div class="card shadow-sm bg-primary-subtle">
                            <div class="card-body">
                                <b>Not bg-gradient</b><br />
                                this is a test<br />
                                this is a test<br />
                                this is a test<br />
                                this is a test<br />
                            </div>
                        </div>
                    </div>
                    <div class="col-12">
                        <div class="card shadow-sm" style="background-color:var(--bs-gray-400)">
                            <div class="card-body">
                                <b>Not bg-gradient</b><br />
                                this is a test<br />
                                this is a test<br />
                                this is a test<br />
                                this is a test<br />
                            </div>
                        </div>
                    </div>
                    <div class="col"></div>
                </div>
            </div>
        </div>

    </div>

</template>

<script>
    shared.setAppTitle("$auto$");
    let _this = { cid: "", c: null, d: { dt: "" }, regulator: null };

    export default {
        methods: {
            showPromptEx() {

                showPromptEx({
                    title: "Test",
                    message1: "This is the forst message", message2: "This is the second message that you can write more description here even tow or three lines.",
                    reasonTitle: "ReasonTitle", reasonRequired: true,
                    noteTitle: "NoteTitle", noteRequired: true, noteRule: ":=s(8,4000)",
                    reasonsParentId: 10000,
                    callback: function (ret) {
                        showJson(ret);
                    }
                });

            },
            start() {
                //_this.c.regulator = $("#main").inputsRegulator();
            },
            validate() {

                //alert($("#main").html());

                //$("#main").find("[data-ae-validation-required]").each(function () {

                //    let i = $(this);

                //    console.log("v : " + i.val());

                //});

                _this.c.regulator.validateArea();
            },
            mShowInfo() {
                showInfo("This is a test message.");
            },
            mShowSuccess() {
                showSuccess("This is a test message.");
            },
            mShowError() {
                showError("This is a test message.");
            },
            mShowWarning() {
                showWarning("This is a test message.");
            }
        },
        setup(props) { _this.cid = props['cid']; },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { initVueComponent(_this); _this.c.start(); },
        props: { cid: String }
    }

</script>