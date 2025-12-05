<template>
    <div class="card border-0 shadow-lg bg-transparent rounded-0">
        <div class="card-body p-3 pb-4 bg-transparent fs-d8">

           <div class="row">
               <div class="col-48" v-for="i in tables">
                   <div class="form-control form-control-sm text-primary pointer" @click="pickTable(i.ObjectName)">
                       {{i.ObjectName}}
                   </div>
               </div>
           </div>

        </div>
        
    </div>
</template>

<script>
    let _this = { cid: "", c: null, inputs: {}, tables: [] };

    export default {
        methods: {
            pickTable(tableName) {
                if (_this.inputs.callback) _this.inputs.callback(tableName);
                shared.closeComponent(_this.cid);
            },
            cancel(e) {
                shared.closeComponent(_this.cid);
            },
            readTablesList() {
                rpcAEP("GetDbObjectsStack", { "DbConfName": _this.c.inputs.DbConfName, "ObjectType": "Table", "Filter": null }, function (res) {
                    _this.c.tables = R0R(res);
                });
            }
        },
        setup(props) {
            _this.cid = props['cid'];
            _this.inputs = shared["params_" + _this.cid];
        },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { _this.c.readTablesList(); },
        props: { cid: String }
    }

</script>
