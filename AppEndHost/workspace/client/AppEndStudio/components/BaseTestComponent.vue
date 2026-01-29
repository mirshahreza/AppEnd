<template>
    <div class="container-fluid h-100 p-5 scrollable">

        <div class="card my-2">
            <div class="card-body text-center">
                <button class="btn btn-primary"
                        @click="shared.openComponentByEl($event);"
                        data-ae-src="/a.SharedComponents/WizTest.vue"
                        data-ae-options='{"resizable":false,"draggable":false,"closeByOverlay":true,"modalSize":"modal-fullscreen","title":"Test Wizard","windowSizeSwitchable":false}'>
                    Start Wizard
                </button>
            </div>
        </div>

    </div>
</template>

<script>
    shared.setAppTitle("$auto$");
    let _this = { cid: "", c: null, d: { dt: "" }, row: { Picture_FileBody: null, Picture_FileName: null, Picture_FileSize: null, Picture_FileMime: null } };

    export default {
        methods: {
            start() {
            }
        },
        setup(props) { _this.cid = props['cid']; },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() {
            initVueComponent(_this);
        },
        props: { cid: String }
    }

</script>