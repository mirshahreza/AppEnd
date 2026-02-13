<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-body bg-primary-subtle-light scrollable" v-if="d!==null" id="formArea" data-ae-widget="inputsRegulator" data-ae-widget-options="{}">
            <div class="card">
                <div class="card-header p-2">
                    <div class="fw-bold fs-d7">Denied Access</div>
                </div>
                <div class="card-body p-2 pt-0">
                    <div class="mb-1 ae-addable-list">
                        <div class="input-group input-group-sm mt-3">
                            <span class="input-group-text text-dark fs-d7 fw-bold">Roles</span>
                            <input type="text" class="form-control form-control-sm" @keyup.enter="addDeniedRole" />
                        </div>
                        <div class="form-control form-control-lg rounded-top-0">
                            <span class="btn btn-sm bg-danger-subtle text-danger-emphasis me-1" @click="removeDeniedRole"
                                  v-for="i in d['DeniedRoles']">
                                <i class="fa-solid fa-fw fa-user-group me-1"></i> {{i}}
                            </span>
                        </div>
                    </div>
                    <div class="mb-1 ae-addable-list">
                        <div class="input-group input-group-sm mt-3">
                            <span class="input-group-text text-dark fs-d7 fw-bold">Users</span>
                            <input type="text" class="form-control form-control-sm" @keyup.enter="addDeniedUser" />
                        </div>
                        <div class="form-control form-control-lg rounded-top-0">
                            <span class="btn btn-sm bg-danger-subtle text-danger-emphasis me-1" @click="removeDeniedUser"
                                  v-for="i in d['DeniedUsers']">
                                <i class="fa-solid fa-fw fa-user me-1"></i> {{i}}
                            </span>
                        </div>
                    </div>
                </div>
            </div>
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
    let _this = { cid: "", c: null, inputs: {}, d: null };
    export default {
        props: { cid: String },
        methods: {
            addDeniedRole(event) {
                let v = $(event.target).val().trim();
                if (v === '') return;
                _this.c.d['DeniedRoles'].push(v);
                $(event.target).val("");
            },
            removeDeniedRole(event) {
                let v = $(event.target).text().trim();
                _this.c.d['DeniedRoles'] = _.filter(_this.c.d['DeniedRoles'], function (i) { return i !== v; });
            },
            addDeniedUser(event) {
                let v = $(event.target).val().trim();
                if (v === '') return;
                _this.c.d['DeniedUsers'].push(v);
                $(event.target).val("");
            },
            removeDeniedUser(event) {
                let v = $(event.target).text().trim();
                _this.c.d['DeniedUsers'] = _.filter(_this.c.d['DeniedUsers'], function (i) { return i !== v; });
            },
            ok() {
                if (_this.inputs.callback) _this.inputs.callback(_this.c.d);
                shared.closeComponent(_this.cid);
            },
            cancel() {
                shared.closeComponent(_this.cid);
            }
        },
        setup(props) {
            _this.cid = props['cid'];
            _this.inputs = shared["params_" + _this.cid] || {};
        },
        data() {
            _this.inputs = _this.inputs || {};
            let access = _this.inputs.accessDeny || {};
            if (fixNull(access.DeniedRoles, '') === '' && fixNull(access.DeniedUsers, '') === '') {
                access.DeniedRoles = access.DeniedViewRoles || access.DeniedEditRoles || [];
                access.DeniedUsers = access.DeniedViewUsers || access.DeniedEditUsers || [];
            }
            _this.d = _.defaults(access, { DeniedRoles: [], DeniedUsers: [] });
            return _this;
        },
        created() { _this.c = this; },
        mounted() { }
    }

</script>
