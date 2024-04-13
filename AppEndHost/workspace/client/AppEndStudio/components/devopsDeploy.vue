<template>
    <div class="card h-100 rounded rounded-2 rounded-bottom-0 rounded-end-0 bg-transparent border-0">
        <div class="card-header p-2 bg-light-subtle rounded-end-0 border-0">
            <div class="input-group input-group-sm border-0 bg-transparent">
                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="calculateItems">
                    <i class="fa-solid fa-fw fa-refresh"></i> <span class="fb">Calculate items to deploy</span>
                </button>
                <button class="btn btn-sm btn-link text-success text-decoration-none bg-hover-light" 
                        :disabled="canStart===false"
                        @click="calculateItems">
                    <i class="fa-solid fa-fw fa-play"></i> <span class="fb">Start deployment</span>
                </button>
                <input type="text" class="form-control form-control-sm border-0 rounded-0 bg-transparent" disabled />
                <button class="btn btn-sm btn-link text-secondary text-decoration-none bg-hover-light text-hover-primary" @click="addNode">
                    <i class="fa-solid fa-fw fa-plus"></i> <span class="fb">Add Node</span>
                </button>
            </div>
        </div>
        <div class="card-header bg-white border border-start-0 border-end-0 border-1 border-secondary-subtle">
            <div class="row">
                <div class="col-48 col-md-24">
                    <div class="form-check">
                        <input class="form-check-input" type="checkbox" value="1" id="chkConsiderDateTime" checked="checked">
                        <label class="form-check-label" for="chkConsiderDateTime">
                            Consider las deploy Date and Time
                        </label>
                    </div>
                </div>
                <div class="col-48 col-md-24">
                    <div class="form-check">
                        <input class="form-check-input" type="checkbox" value="1" id="chkConsiderIgnoringRules" checked="checked">
                        <label class="form-check-label" for="chkConsiderIgnoringRules">
                            Consider ignoring rules
                        </label>
                    </div>
                </div>
            </div>
        </div>
        <div class="card-body p-2">
            <div class="h-100 w-100" data-flex-splitter-horizontal style="flex: auto;">
                <div class="h-100" style="min-width:200px;width:25.5%;">
                    <div class="card h-100 shadow-sm">
                        <div class="card-header fw-bold fs-d8 p-2">
                            Nodes
                        </div>
                        <div class="card-body scrollable p-2">
                            <div class="card mb-2 border-light-subtle" v-for="n,ind in nodes">
                                <div class="card-header bg-light-subtle border-light-subtle p-1 fs-d8">
                                    <div class="fw-bold">
                                        <table class="w-100 text-center">
                                            <tr>
                                                <td class="text-start">{{n.Name}}</td>
                                                <td style="width:22px;">
                                                    <i class="fa-solid fa-fw fa-edit text-primary pointer" @click="editNode(ind)"></i>
                                                </td>
                                                <td style="width:22px;">
                                                    <i class="fa-solid fa-fw fa-trash-can text-muted text-hover-danger pointer" @click="removeNode(ind)"></i>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </div>
                                <div class="card-header text-secondary bg-light-subtle border-light-subtle p-1 fs-d8">
                                    {{n.Ip}} : {{n.Port}}
                                </div>
                                <div class="card-header text-secondary bg-light-subtle border-light-subtle p-1 fs-d8">
                                    Updated on : <span class="fs-d9 fw-bold">{{n.LastDeploy}}</span>
                                </div>
                                <div class="card-body p-2 fs-1d1">
                                    <table class="w-100">
                                        <tr>
                                            <td class="fs-d9"><span class="fw-bold">{{n.FilesCount}}</span> <span class="fs-d8 text-muted">file(s)</span></td>
                                            <td style="width:32px;"><i class="fa-solid fa-fw fa-ellipsis text-secondary"></i></td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div role="separator" tabindex="1" class="bg-light" style="width:.5%;"></div>
                <div class="h-100" style="min-width:300px;width:74%;">
                    <div class="card h-100 shadow-sm">
                        <div class="card-header fw-bold fs-d8 p-2">
                            Files to do
                        </div>

                        <div class="card-body scrollable p-2">

                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    shared.setAppTitle("Deploy");

    let _this = { cid: "", c: null, inputs: {}, nodes: [], canStart: false };

    export default {
        methods: {
            addNode() {
                let newNode = { "Ind": -1, Ip: "", Port: "", Name: "", UserName: "", Password: "" };
                openComponent("components/devopsNodesCreateUpdate", {
                    title: "Node Editor",
                    params: {
                        "node": newNode,
                        callback: function (ret) {
                            rpcAEP("CreateUpdateNode", ret, function (res) {
                                _this.c.getNodes();
                            });
                        }
                    }
                });
            },
            editNode(ind) {
                let i = _this.c.nodes[ind];
                let editNode = { "Ind": ind, Ip: i["Ip"], Port: i["Port"], Name: i["Name"], UserName: i["UserName"], Password: i["Password"] };
                openComponent("components/devopsNodesCreateUpdate", {
                    title: "Node Editor",
                    params: {
                        "node": editNode,
                        callback: function (ret) {
                            rpcAEP("CreateUpdateNode", ret, function (res) {
                                _this.c.getNodes();
                            });
                        }
                    }
                });
            },
            removeNode(ind) {
                showConfirm({
                    title: "Remove Node", message1: "Are you sure you want to remove this node?", message2: ind,
                    callback: function () {
                        rpcAEP("RemoveNode", { Ind: ind }, function (res) {
                            _this.c.getNodes();
                        });
                    }
                });
            },
            calculateItems() {
                _this.c.canStart = true;
            },
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