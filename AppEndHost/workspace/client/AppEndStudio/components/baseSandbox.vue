<template>
    <div class="container-fluid h-100 scrollable">


        <div class="row m-5">
            <div class="col-8">
                <button class="btn btn-lg btn-primary w-100 mb-2" id="startButton">Start Webcam</button>
                <button class="btn btn-lg btn-primary w-100 mb-2" id="captureButton">Capture Photo</button>
            </div>
            <div class="col-20">

                <video id="videoElement" autoplay></video>
                

            </div>
            <div class="col-20">

                <canvas id="canvasElement" style="display: none;"></canvas>
                <img id="photoElement" style="display: none;">

            </div>
        </div>



        <div class="row m-5">
            <div class="col-48">

                <i class="fa-solid fa-fw fa-a text-danger"></i>
                <i class="fa-solid fa-fw fa-p text-danger"></i>
                <i class="fa-solid fa-fw fa-p text-danger"></i>
                <i class="fa-solid fa-fw fa-e text-danger"></i>
                <i class="fa-solid fa-fw fa-n text-danger"></i>
                <i class="fa-solid fa-fw fa-d text-danger"></i>

            </div>
        </div>

        <div class="row m-5">
            <div class="col-48">

                <button class="btn btn-link"
                        @click="shared.openComponentByEl($event);"
                        data-ae-src="/a.UserComponents/wizTest.vue"
                        data-ae-options='{"resizable":false,"draggable":false,"closeByOverlay":true,"modalSize":"modal-fullscreen","title":"Test Wizard","windowSizeSwitchable":false}'>
                    Start Wizard
                </button>

            </div>
        </div>

        <div class="row m-5">
            <div class="col-12">
                <div class="badge border border-2 rounded-4 p-0 text-bg-light shadow-sm" style="height:30px;">

                    <img :src="shared.getImageURI(shared.getLogedInUserContext()['Picture_FileBody'])"
                         class="border border-2 rounded rounded-4 shadow-sm h-100" />

                    <span class="mx-2">Mohammad</span>

                </div>

            </div>
        </div>

        <div class="row m-5">
            <div class="col-12">
                <label class="">Date between 2014-01-01 and 2014-03-30</label>
                <input type="text" class="form-control form-control-sm ae-focus"
                       data-ae-validation-required="true" data-ae-validation-rule=":=dt(2014-01-01,2014-03-30)"
                       v-model="d.dt" />
                <div>{{d.dt}}</div>

            </div>
        </div>

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

        <div class="row m-5">
            <div class="col"></div>
            <div class="col-12">
                <div class="card shadow-sm bg-secondary-subtle bg-gradient">
                    <div class="card-body">
                        this is a test<br />
                        this is a test<br />
                        this is a test<br />
                        this is a test<br />
                    </div>
                </div>
            </div>
            <div class="col-12">
                <div class="card shadow-sm bg-primary-subtle bg-gradient">
                    <div class="card-body">
                        this is a test<br />
                        this is a test<br />
                        this is a test<br />
                        this is a test<br />
                    </div>
                </div>
            </div>
            <div class="col-12">
                <div class="card shadow-sm bg-success-subtle bg-gradient">
                    <div class="card-body">
                        this is a test<br />
                        this is a test<br />
                        this is a test<br />
                        this is a test<br />
                    </div>
                </div>
            </div>
            <div class="col"></div>
        </div>

        <div class="row m-5">
            <div class="col"></div>
            <div class="col-12">
                <div class="card shadow-sm bg-danger-subtle">
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
                <div class="card shadow-sm bg-dark-subtle">
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
                <div class="card shadow-sm bg-warning-subtle">
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

        <div class="row m-5">
            <div class="col"></div>
            <div class="col-12">
                <div class="card shadow-sm bg-danger-subtle bg-gradient">
                    <div class="card-body">
                        <b>bg-gradient</b><br />
                        this is a test<br />
                        this is a test<br />
                        this is a test<br />
                        this is a test<br />
                    </div>
                </div>
            </div>
            <div class="col-12">
                <div class="card shadow-sm bg-dark-subtle bg-gradient">
                    <div class="card-body">
                        <b>bg-gradient</b><br />
                        this is a test<br />
                        this is a test<br />
                        this is a test<br />
                        this is a test<br />
                    </div>
                </div>
            </div>
            <div class="col-12">
                <div class="card shadow-sm bg-warning-subtle bg-gradient">
                    <div class="card-body">
                        <b>bg-gradient</b><br />
                        this is a test<br />
                        this is a test<br />
                        this is a test<br />
                        this is a test<br />
                    </div>
                </div>
            </div>
            <div class="col"></div>
        </div>


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

        <div class="row m-5">
            <div class="col"></div>
            <div class="col-12">
                <div class="card shadow-sm bg-gradient" style="background-color:var(--bs-gray)">
                    <div class="card-body">
                        <b>bg-gradient</b><br />
                        this is a test<br />
                        this is a test<br />
                        this is a test<br />
                        this is a test<br />
                    </div>
                </div>
            </div>
            <div class="col-12">
                <div class="card shadow-sm bg-primary-subtle bg-gradient">
                    <div class="card-body">
                        <b>bg-gradient</b><br />
                        this is a test<br />
                        this is a test<br />
                        this is a test<br />
                        this is a test<br />
                    </div>
                </div>
            </div>
            <div class="col-12">
                <div class="card shadow-sm bg-gradient" style="background-color:var(--bs-gray-400)">
                    <div class="card-body">
                        <b>bg-gradient</b><br />
                        this is a test<br />
                        this is a test<br />
                        this is a test<br />
                        this is a test<br />
                    </div>
                </div>
            </div>
            <div class="col"></div>
        </div>

        <div class="row m-5">
            <div class="col rounded rounded-2 border border-3 border-light shadow-sm bg-white fs-d9">

                <table class="table table-sm table-hover w-100 m-0 bg-transparent">
                    <thead>
                        <tr>
                            <th style="width:85px;">Col 1</th>
                            <th style="width:300px;">Col 2</th>
                            <th>Col 1</th>
                        </tr>
                    </thead>
                    <tr>
                        <td>123</td>
                        <td>Test Title</td>
                        <td>Test Title Test Title Test Title Test Title Test Title Test Title Test Title Test Title </td>
                    </tr>
                    <tr>
                        <td>123</td>
                        <td>Test Title</td>
                        <td>Test Title Test Title Test Title Test Title Test Title Test Title Test Title Test Title </td>
                    </tr>
                    <tr>
                        <td>123</td>
                        <td>Test Title</td>
                        <td>Test Title Test Title Test Title Test Title Test Title Test Title Test Title Test Title </td>
                    </tr>
                </table>

            </div>
        </div>

        <br />
        <br />
        <br />

    </div>

</template>

<script>
    shared.setAppTitle("$auto$");
    let _this = { cid: "", c: null, d: { dt: "" } };

    export default {
        methods: {
            start() {

                const videoElement = document.getElementById('videoElement');
                const canvasElement = document.getElementById('canvasElement');
                const photoElement = document.getElementById('photoElement');
                const startButton = document.getElementById('startButton');
                const captureButton = document.getElementById('captureButton');

                let stream;

                async function startWebcam() {
                    try {
                        stream = await navigator.mediaDevices.getUserMedia({ video: true });
                        videoElement.srcObject = stream;
                        startButton.disabled = true;
                        captureButton.disabled = false;
                    } catch (error) {
                        console.error('Error accessing webcam:', error);
                    }
                }

                function capturePhoto() {
                    canvasElement.width = videoElement.videoWidth;
                    canvasElement.height = videoElement.videoHeight;
                    canvasElement.getContext('2d').drawImage(videoElement, 0, 0);
                    const photoDataUrl = canvasElement.toDataURL('image/jpeg');
                    photoElement.src = photoDataUrl;
                    photoElement.style.display = 'block';
                }

                captureButton.addEventListener('click', capturePhoto);

                startButton.addEventListener('click', startWebcam);

                //const dtp1Instance = new mds.MdsPersianDateTimePicker(document.getElementById('dp'), {
                //    targetTextSelector: '[data-name="dpText"]',
                //    targetDateSelector: '[data-name="dpDate"]',
                //    //isGregorian: true,
                //    dateFormat: 'yyyy-MM-dd HH:mm:ss',
                //    textFormat:'yyyy-MM-dd HH:mm:ss'
                //});

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