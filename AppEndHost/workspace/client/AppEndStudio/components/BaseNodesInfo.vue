<template>
    <div class="card shadow-sm mb-2 h-100">
        <div class="card-body">
            <div class="text-dark fs-d9 fw-bold px-2">
                Linked Nodes
            </div>
            <hr class="my-1" />
            <div class="p-2 fs-d9">

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