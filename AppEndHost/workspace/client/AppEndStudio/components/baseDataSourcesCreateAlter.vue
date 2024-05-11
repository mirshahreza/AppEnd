<template>
    <div class="card border-0 shadow-lg bg-transparent rounded-0">
        <div class="card-body p-3 pb-4 bg-transparent fs-d8">

            <label class="ms-2">Name</label>
            <input type="text" name="Name" class="form-control form-control-sm" v-model="inputs.Name" :disabled="inputs.IsNew!==true" />
            <div class="my-2"></div>

            <label class="ms-2">ServerType</label>
            <select class="form-select form-select-sm" v-model="inputs.ServerType">
                <option value="MsSql">MsSql</option>
                <option value="MySql">MySql</option>
                <option value="Postgres">Postgres</option>
                <option value="Oracle">Oracle</option>
            </select>
            <div class="my-2"></div>

            <label class="ms-2">ConnectionString</label>
            <textarea name="ConnectionString" rows="2" class="form-control form-control-sm" v-model="inputs.ConnectionString"></textarea>
            <div class="my-2"></div>
        </div>
        <div class="card-footer p-3 bg-secondary-subtle bg-gradient border-0 rounded-0">
            <div class="row">
                <div class="col-24">
                    <button class="btn btn-sm btn-secondary w-100 py-2" @click="cancel" data-ae-key="ok">
                        <i class="fa-solid fa-cancel"></i>
                        &nbsp;
                        <span>Cancel</span>
                    </button>
                </div>
                <div class="col-24">
                    <button class="btn btn-sm btn-primary w-100 py-2" @click="ok" data-ae-key="ok">
                        <i class="fa-solid fa-check"></i>
                        &nbsp;
                        <span>Ok</span>
                    </button>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    let _this = { cid: "", c: null, inputs: {} };
    _this.inputs = { IsNew: false, Name: "", ServerType: "", ConnectionString: "" };

    export default {
        methods: {
            ok(e) {
                rpcAEP("AddOrAlterDbServer", { "DataSourceInfo": _this.inputs }, function () {
                    showSuccess("Record saved");
                    if (_this.inputs.callback) _this.inputs.callback();
                    _this.c.close();
                });
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
        props: { cid: String }
    }
</script>
