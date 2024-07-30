<template>
    <div class="card bg-transparent rounded-0 border-0 h-100">
        <div class="card-header p-2 fw-bold fs-d9">
            {{inputs.RoleName}} [{{inputs.RoleId}}]
        </div>
        <div class="card-body scrollable">
            <div class="card mb-3 border-light-subtle" v-for="c in d">
                <div class="card-header bg-light-subtle border-light-subtle p-1 fs-d7 fw-bold">
                    <i class="fa-solid fa-fw fa-chevron-right"></i> <span>{{c.Controller}}</span>
                </div>
                <div class="card-body p-2 py-1">
                    <div v-for="m in c.Methods" @click="switchAccessSetting(c.Controller,m.MethodName)"
                         class="btn btn-sm me-1 my-1 p-1 fs-d7" :class="m.HasAccess===true ? 'btn-success' : 'btn-light'">
                        <div v-if="m.HasAccess===true">
                            <i class="fa-solid fa-fw fa-check"></i> <span>{{m.MethodName}}</span>
                        </div>
                        <div v-else>
                            <i class="fa-solid fa-fw fa-minus"></i> <span>{{m.MethodName}}</span>
                        </div>
                    </div>
                </div>
            </div>
            <br />
        </div>        
    </div>
</template>
<script>
    let _this = { cid: "", c: null, inputs: {}, d: null };

    export default {
        methods: {
            switchAccessSetting(controller, methodName) {
                let cont = _.find(_this.c.d, function (i) { return i.Controller === controller });
                let m = _.find(cont.Methods, function (i) { return i.MethodName === methodName });
                rpcAEP("SetAccessSettingsByRoleId", { MethodFullName: `${controller}.${methodName}`, RoleId: _this.c.inputs.RoleId, Access: !m.HasAccess }, function (res) {
                    m.HasAccess = !m.HasAccess;
                });
            },
            loadMethodsAndPermissions() {
                rpcAEP("GetDynaClassesAccessSettingsByRoleId", { RoleId: _this.c.inputs.RoleId }, function (res) {
                    _this.c.d = R0R(res);
                });
            }
        },
        setup(props) {
            _this.cid = props['cid'];
            _this.inputs = shared["params_" + _this.cid];
        },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { _this.c.loadMethodsAndPermissions(); },
        props: { cid: String }
    }
</script>