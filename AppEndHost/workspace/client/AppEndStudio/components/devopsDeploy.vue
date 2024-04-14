<template>
    <div class="card h-100 rounded rounded-2 rounded-bottom-0 rounded-end-0 bg-transparent border-0">
        <div class="card-header p-2 bg-light-subtle rounded-end-0 border-0">
            <div class="input-group input-group-sm border-0 bg-transparent">
                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="refreshNodes" :disabled="inProgress">
                    <i class="fa-solid fa-fw fa-refresh"></i> <span class="fb">Refresh</span>
                </button>
                <button class="btn btn-sm btn-link text-success text-decoration-none bg-hover-light" @click="startDeploy">
                    <i class="fa-solid fa-fw fa-play"></i> <span class="fb">Start deployment</span>
                </button>
                <input type="text" class="form-control form-control-sm border-0 rounded-0 bg-transparent" disabled />
                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="reCalcNodes" :disabled="inProgress">
                    <i class="fa-solid fa-fw fa-refresh"></i> <span class="fb">ReCalc</span>
                </button>
                <button class="btn btn-sm btn-link text-secondary text-decoration-none bg-hover-light text-hover-primary" @click="addNode">
                    <i class="fa-solid fa-fw fa-plus"></i> <span class="fb">Add Node</span>
                </button>
            </div>
        </div>
        
        <div class="card-body p-2">
            <div class="h-100 w-100" data-flex-splitter-horizontal style="flex: auto;">
                <div class="h-100" style="min-width:250px;width:25.5%;">
                    <div class="card h-100 shadow-sm">
                        <div class="card-header fw-bold fs-d8 p-2">
                            Nodes
                        </div>
                        <div class="card-header">
                            <div class="form-check">
                                <input class="form-check-input" type="checkbox" value="1" id="chkConsiderLastDeploy" checked="checked">
                                <label class="form-check-label" for="chkConsiderLastDeploy">
                                    Considering Last Deploy
                                </label>
                            </div>
                        </div>
                        <div class="card-body scrollable p-2">
                            <div v-for="n,ind in nodes">
                                <div class="card mb-2 pointer"
                                     @click="showNodeFiles(ind)"
                                     :class="(selectedNode!==null && selectedNode['Ip']===n['Ip'] && selectedNode['Port']===n['Port'])===true ? 'shadow-sm border-primary-subtle' : 'border-light-subtle'">
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
                                    <div class="card-header text-secondary bg-light-subtle border-light-subtle p-1 fs-d7">
                                        Updated on : <span class="fs-d9 fw-bold">{{shared.formatDateTime(n.LastDeploy)}}</span>
                                    </div>

                                    <div class="card-body p-2 fs-1d1">
                                        <table class="w-100">
                                            <tbody>
                                                <tr>
                                                    <td class="fs-d9">
                                                        <span class="fw-bold text-primary">{{shared.fixNull(n.FilesToDo,[]).length}}</span>
                                                        /
                                                        <span class="fw-bold text-success">{{shared.ld().filter(shared.fixNull(n.FilesToDo,[]),function(i){return i.Done===true;}).length}}</span>
                                                    </td>
                                                    <td style="width:32px;">
                                                        <i class="fa-solid fa-fw fa-ellipsis text-secondary" v-if="n.InProgress===false"></i>
                                                        <i class="fa-solid fa-fw fa-spinner fa-spin" v-if="n.InProgress===true"></i>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div role="separator" tabindex="1" class="bg-light" style="width:.5%;"></div>
                <div class="h-100" style="min-width:300px;width:74%;">
                    <div class="card h-100 shadow-sm">
                        <div class="card-header fw-bold fs-d8">
                            Files to deploy to <span v-if="selectedNode!==null">[{{selectedNode["Ip"]}} : {{selectedNode["Port"]}}]</span> 
                        </div>
                        <div class="card-body scrollable p-0">

                            <table class="table table-sm table-hover table-striped w-100" v-if="selectedNode!==null">
                                <tbody>
                                    <tr v-for="f in selectedNode['FilesToDo']">
                                        <td class="fs-d8 text-muted">
                                            {{f["FilePath"]}}
                                        </td>
                                        <td class="fs-d7 text-muted text-center" style="width:115px;">
                                            {{shared.formatDateTime(f["LastWrite"])}}
                                        </td>
                                        <td class="fs-d7 text-muted text-center" style="width:32px;">
                                            <i class="fa-solid fa-fw fa-check text-success" v-if="f['Done']===true"></i>
                                            <i class="fa-solid fa-fw fa-minus text-secondary" v-else></i>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>

                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    shared.setAppTitle("Deploy");

    let _this = { cid: "", c: null, inputs: {}, inProgress: false, nodes: [], selectedNode: null };

    export default {
        methods: {
            startDeploy() {
                _this.c.inProgress = true;
                let ind = 0;
                _.forEach(_this.c.nodes, function (n) {
                    if (n["FilesToDo"].length > 0) {
                        n["InProgress"] = true;
                        rpcAEP("StartDeployToNode", { ConsiderLastTime: $("#chkConsiderLastDeploy").prop("checked"), ConsiderIgnoreRules: $("#chkConsiderIgnorRules").prop("checked"), Ind: ind }, function (res) {
                            n["InProgress"] = false;
                            _this.c.inProgress = _this.c.calcInProggress();
                            if (res[0]["IsSucceeded"] !== true) {
                                showJson(res);
                            }
                        });
                        ind++;
                    }
                });
            },
            showNodeFiles(ind) {
                _this.c.selectedNode = _this.c.nodes[ind];
            },
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
            refreshNodes() {
                let ind = 0;
                _.forEach(_this.c.nodes, function (n) {
                    _this.c.calculateItemsByNodeIndex(ind, false);
                    ind++;
                });
            },
            reCalcNodes() {
                let ind = 0;
                _.forEach(_this.c.nodes, function (n) {
                    _this.c.calculateItemsByNodeIndex(ind, true);
                    ind++;
                });
            },
            calculateItemsByNodeIndex(nodeInd, overrideExistingCalc) {
                let cLastTime = $("#chkConsiderLastDeploy").prop("checked");
                rpcAEP("GetNodeToDoItems", {
                    ConsiderLastTime: cLastTime, Ind: nodeInd, OverrideExistingCalc: overrideExistingCalc
                }, function (res) {
                    _this.c.nodes[nodeInd]["FilesToDo"] = R0R(res);
                });
            },
            getNodes() {
                rpcAEP("GetNodes", {}, function (res) {
                    _this.c.nodes = _this.c.setAllPending(R0R(res));
                    setTimeout(function () { _this.c.refreshNodes(); }, 250);
                });
            },
            setAllPending(nodes) {
                _.forEach(nodes, function (n) {
                    n["InProgress"] = false;
                });
                return nodes;
            },
            calcInProggress() {
                _.forEach(_this.c.nodes, function (n) {
                    if (n["InProgress"] === true) return true;
                });
                return false;
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