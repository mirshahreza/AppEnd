<template>
    <div class="card bg-transparent rounded-0 border-0 h-100">
        <div class="card-header p-2 fw-bold fs-d9">
            {{inputs.RoleName}} [{{inputs.RoleId}}]
        </div>
        <div class="card-body p-2">

        </div>        
    </div>
</template>

<script>
    let _this = { cid: "", c: null, inputs: {} };

    export default {
        methods: {
            loadMethodsAndPermissions() {
                rpcAEP("GetDynaClassesForRole", { RoleName: inputs.RoleName }, function (res) {
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