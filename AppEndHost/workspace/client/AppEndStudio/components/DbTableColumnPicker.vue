<template>
    <div class="card border-0 shadow-lg bg-transparent rounded-0">
        <div class="card-body p-3 pb-4 bg-transparent fs-d8">

           <div class="row">
               <div class="col-48" v-for="i in columns">
                   <div class="form-control form-control-sm text-primary pointer" @click="pickColumn(i.Name)">
                       {{i.Name}}
                   </div>
               </div>
           </div>

        </div>
        
    </div>
</template>

<script>
    let _this = { cid: "", c: null, inputs: {}, columns: [] };

    export default {
        methods: {
            pickColumn(colName) {
                if (_this.inputs.callback) _this.inputs.callback(colName);
                shared.closeComponent(_this.cid);
            },
            cancel(e) {
                shared.closeComponent(_this.cid);
            },
            readTableColumns() {
                rpcAEP("ReadObjectSchema", { "DbConfName": _this.c.inputs.DbConfName, "ObjectName": _this.c.inputs.TableName }, function (res) {
                    _this.c.columns = R0R(res);
                });
            }
        },
        setup(props) {
            _this.cid = props['cid'];
            _this.inputs = shared["params_" + _this.cid];
        },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { _this.c.readTableColumns(); },
        props: { cid: String }
    }

</script>
