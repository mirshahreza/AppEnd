<template>
    <div class="card border-0 shadow-lg bg-transparent rounded-0 h-100">
        <div class="card-body p-3 pb-4 bg-transparent fs-d8 scrollable">
            <div class="row h-100">
                <div class="col-48 h-100">
                    

                    <div class="vertical-center" style="height:75%;width:90%" id="editorPlace">
                        <img class="vertical-center img-view" src="/a..lib/images/avatar.png" style="max-height:100%;max-width:100%" id="img" />
                    </div>


                </div>
            </div>
        </div>

        <div class="card-footer p-0">
            <div class="container-fluid pt-2 pb-1">
                <div class="row p-0">
                    <div class="col-36 px-2">
                        <button class="btn btn-sm btn-primary w-100" @click="ok">
                            <i class="fa-solid fa-check me-1"></i>
                            <span>{{shared.translate("Ok")}}</span>
                        </button>
                    </div>
                    <div class="col-12 px-2">
                        <button class="btn btn-sm btn-secondary w-100" @click="cancel">
                            <i class="fa-solid fa-xmark me-1"></i>
                            <span>{{shared.translate("Cancel")}}</span>
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    let _this = { cid: "", c: null, inputs: {}, croppie: null, imgEditor: null };
    export default {
        methods: {
            loadImage() {
                $(document).ready(function () {
                    _this.c.imgEditor = $("#" + _this.c.cid + " #img:first");
                    _this.c.imgEditor.on("load", function () {
                        setTimeout(function () {
                            _this.w = _this.c.imgEditor.width();
                            _this.h = _this.c.imgEditor.height();
                            _this.c.setupEditor();
                        }, 250);
                    });
                    _this.c.imgEditor.attr("src", "data:image/png;base64," + _this.c.inputs.image);
                });
            },
            setupEditor() {
                _this.c.croppie = new Croppie(_this.c.imgEditor.get(0), {
                    viewport: { width: _this.w - 25, height: _this.h - 25 },
                    boundary: { width: _this.w, height: _this.h },
                    enableResize: true,
                    enableOrientation: true
                });
                _this.c.croppie.bind({
                    //zoom: 5
                });
            },
            ok() {

                let o = { type: 'base64', size: 'viewport', format: 'jpeg', quality: 1, circle: false };
                _this.c.croppie.result(o).then(function (blob) {
                    if (_this.c.inputs.callback) _this.c.inputs.callback({ rr: true, rv: blob.replace("data:image/jpeg;base64,", "") });
                    _this.c.close();
                });

                if (_this.c.inputs.callback) _this.inputs.callback(_this.c.inputs.retVal);
                _this.c.close();
            },
            cancel() { _this.c.close(); },
            close() { shared.closeComponent(_this.cid); }
        },
        setup(props) {
            _this.cid = props['cid'];
            _this.inputs = shared["params_" + _this.cid];
        },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { initVueComponent(_this); _this.c.loadImage(); },
        props: { cid: String }
    }

</script>