<template>
    <div class="card border-0 shadow-lg bg-transparent rounded-0" id="formArea" data-ae-widget="inputsRegulator" data-ae-widget-options="{}">
        <div class="card-body p-3 pb-4 bg-transparent fs-d8">

            <label class="ms-2">Folder</label>
            <input type="text" class="form-control form-control-sm" v-model="inputs.FolderName"
                   data-ae-validation-required="true" data-ae-validation-rule=":=s(3,64)" />
            <div class="my-2"></div>

            <label class="ms-2">Title</label>
            <input type="text" class="form-control form-control-sm" v-model="inputs.Title"
                   data-ae-validation-required="true" data-ae-validation-rule=":=s(3,32)" />
            <div class="my-2"></div>

            <label class="ms-2">SubTitle</label>
            <input type="text" class="form-control form-control-sm" v-model="inputs.SubTitle"
                   data-ae-validation-required="true" data-ae-validation-rule=":=s(1,32)" />
            <div class="my-2"></div>

        </div>
        <div class="card-footer p-0">
            <div class="container-fluid pt-2 pb-1">
                <div class="row p-0">
                    <div class="col-36 px-2">
                        <button class="btn btn-sm btn-primary w-100" @click="ok" data-ae-key="ok">
                            <i class="fa-solid fa-check me-1"></i>
                            <span>Ok</span>
                        </button>
                    </div>
                    <div class="col-12 px-2">
                        <button class="btn btn-sm btn-secondary w-100" @click="cancel">
                            <i class="fa-solid fa-xmark me-1"></i>
                            <span>Cancel</span>
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    let _this = { cid: "", c: null, inputs: {} };

    export default {
        methods: {
            ok(e) {
                if (!isAreaValidById("formArea")) return false;
                showSuccess("Record saved");
                if (_this.inputs.callback) _this.inputs.callback(_this.inputs);
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
        mounted() { initVueComponent(_this); },
        props: { cid: String }
    }
</script>
