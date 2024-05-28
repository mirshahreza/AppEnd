<template>
    <div class="col-48 p-2">
        <div class="px-2 fw-bold fs-1d1">Linked Nodes</div>
        <div class="bg-primary-subtle fs-d5 p-1"> </div>
        <div class="fs-d9">
            <div class="row p-2">
                <div class="col-48">
                    <div class="badge border border-1 border-secondary-subtle shadow-sm bg-light mb-2 me-2 p-2 text-start" v-for="n,ind in nodes">
                        <div class="text-dark fw-bold">
                            <span>{{n.Name}}</span>
                        </div>
                        <hr class="my-1" />
                        <div class="text-secondary">
                            {{n.Ip}} : {{n.Port}}
                        </div>
                        <hr class="my-1" />
                        <div class="text-secondary">
                            LastDeploy : <span class="fs-d9 fw-bold">{{shared.formatDateTime(n.LastDeploy)}}</span>
                        </div>
                    </div>
                </div>
            </div>
            
        </div>
    </div>
</template>

<script>
    let _this = { cid: "", c: null, inputs: {}, inProgress: false, nodes: [], selectedNode: null, refreshInterval: null };

    export default {
        methods: {
            getNodes() {
                rpcAEP("GetNodes", {}, function (res) {
                    _this.c.nodes = R0R(res);
                });
            }
        },
        setup(props) {
            _this.cid = props['cid'];
            _this.inputs = shared["params_" + _this.cid];
        },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { _this.c.getNodes(); },
        props: { cid: String }
    }
</script>