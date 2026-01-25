<template>
    <div class="col-48 h-100">

        <div class="card h-100 border-0 bg-transparent">
            <div class="card-body p-3 pb-4 bg-transparent fs-d8" id="formArea" data-ae-widget="inputsRegulator" data-ae-widget-options="{}">

                <div class=""><span>Method type</span></div>
                <select class="form-select form-select-sm fw-bold " @change="methodTypeChanged" v-model="newMethod.MethodType"
                        data-ae-validation-required="true">
                    <option></option>
                    <option value="Create">Create</option>
                    <option value="ReadList">ReadList</option>
                    <option value="AggregatedReadList">AggregatedReadList</option>
                    <option value="ReadByKey">ReadByKey</option>
                </select>

                <div>&nbsp;</div>

                <div class=""><span>Method name</span></div>
                <input class="form-control form-control-sm" v-model="newMethod.MethodName" type="text"
                       data-ae-validation-required="true" data-ae-validation-rule=":=s(1,32)" />

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

    </div>
</template>

<script>
    let _this = { cid: "", c: null, inputs: {}, newMethod: { "MethodType": "", "MethodName": "" } };
    export default {
        methods: {
            ok(e) {
                if (!isAreaValidById("formArea")) return false;
                if (_this.c.inputs.callback) _this.c.inputs.callback(_this.c.newMethod);
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
        mounted() { },
        props: { cid: String }
    }

</script>