<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-header p-2 bg-success-subtle rounded-0 border-0">
            <div class="hstack gap-1">
                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="getNodes(null)">
                    <i class="fa-solid fa-fw fa-refresh"></i> <span>Refresh</span>
                </button>
                <div class="vr"></div>
                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="startDeploy" :disabled="inProgress">
                    <i class="fa-solid fa-fw fa-play"></i> <span>Start deployment</span>
                </button>
                <div class="p-0 ms-auto"></div>
                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="addNode" :disabled="inProgress">
                    <i class="fa-solid fa-fw fa-plus"></i> <span>Add Node</span>
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
                        <div class="card-body scrollable p-2">
                            <div v-for="n,ind in nodes">
                                <div class="card mb-2 pointer"
                                     @click="showNodeFiles(ind)"
                                     :class="(selectedNode!==null && selectedNode['Ip']===n['Ip'] && selectedNode['Port']===n['Port'] && selectedNode['RemotePath']===n['RemotePath'])===true ? 'shadow-sm border-primary-subtle' : 'border-light-subtle'">
                                    <div class="card-header bg-light-subtle border-light-subtle p-1 fs-d8">
                                        <div class="fw-bold">
                                            <table class="w-100 text-center">
                                                <tr>
                                                    <td class="text-start">
                                                        <button class="btn btn-sm btn-link text-decoration-none p-0 border-0 text-hover-primary" @click="editNode(ind)" :disabled="inProgress">
                                                            <i class="fa-solid fa-fw fa-edit"></i> <span>{{n.Name}}</span>
                                                        </button>
                                                    </td>
                                                    <td style="width:22px;">
                                                        <button class="btn btn-sm btn-link text-secondary p-0 text-hover-danger" @click="removeNode(ind)" :disabled="inProgress">
                                                            <i class="fa-solid fa-fw fa-trash-can"></i>
                                                        </button>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </div>
                                    <div class="card-header text-secondary bg-light-subtle border-light-subtle p-1 fs-d8">
                                        {{n.Ip}} : {{n.Port}}
                                    </div>
                                    <div class="card-header text-secondary bg-light-subtle border-light-subtle p-1 fs-d7">
                                        RemotePath : <span class="fs-d9 fw-bold">{{n.RemotePath}}</span>
                                    </div>
                                    <div class="card-header text-secondary bg-light-subtle border-light-subtle p-1 fs-d7">
                                        LastDeploy : <span class="fs-d9 fw-bold">{{shared.formatDateTime(n.LastDeploy)}}</span>
                                    </div>

                                    <div class="card-body p-2 fs-1d1">
                                        <table class="w-100">
                                            <tbody>
                                                <tr>
                                                    <td class="fs-d8">
                                                        <span>
                                                            <i class="fa-solid fa-fw fa-check text-success" v-if="shared.fixNull(n.FilesToDo,[]).length===0"></i>
                                                            <i class="fa-solid fa-fw fa-list-check text-danger" v-else></i>
                                                        </span>
                                                        Changed Items : <span class="fw-bold text-primary">{{shared.fixNull(n.FilesToDo,[]).length}}</span>
                                                    </td>
                                                    <td class="fs-d9" style="width:21px;">
                                                        <i class="fa-solid fa-fw fa-play text-success text-hover-primary" v-if="n.ProgressState==='NotExist'" @click="startDeployByIndex(ind)"></i>
                                                        <i class="fa-solid fa-fw fa-spinner fa-spin" v-if="n.ProgressState==='Running'"></i>
                                                        <i class="fa-solid fa-fw fa-q text-danger" v-if="n.ProgressState==='Waiting'"></i>
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
                            Estimate the files to be uploaded to <span v-if="selectedNode!==null">[{{selectedNode["Ip"]}} : {{selectedNode["Port"]}}]</span>
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
    </div></template>

<script>
    shared.setAppTitle("Linked Nodes");

    let _this = { cid: "", c: null, inputs: {}, inProgress: false, nodes: [], selectedNode: null, refreshInterval: null };

    export default {
        methods: {
            startDeployByIndex(ind) {
                rpcAEP("StartDeployToNode", { Ind: ind }, function (res) {
                    _this.c.getNodes();
                });
            },
            startDeploy() {
                _this.c.inProgress = true;
                let ind = 0;
                _.forEach(_this.c.nodes, function (n) {
                    let _ind = ind;
                    rpcAEP("StartDeployToNode", { Ind: _ind }, function (res) {
                        if (_this.c.nodes.length - 1 === _ind) _this.c.getNodes();
                    });
                    ind++;
                });
            },
            showNodeFiles(ind) {
                _this.c.selectedNode = _this.c.nodes[ind];
            },
            addNode() {
                let newNode = { "Ind": -1, Name: "", Ip: "", Port: "", RemotePath: "", UserName: "", Password: "" };
                openComponent("components/devopsNodesCreateAlter", {
                    title: "Node Editor",
                    params: {
                        "node": newNode,
                        callback: function (ret) {
                            rpcAEP("CreateAlterNode", ret, function (res) {
                                _this.c.getNodes();
                            });
                        }
                    }
                });
            },
            editNode(ind) {
                let i = _this.c.nodes[ind];
                let editNode = { "Ind": ind, Name: i["Name"], Ip: i["Ip"], Port: i["Port"], RemotePath: i["RemotePath"], UserName: i["UserName"], Password: i["Password"], RemotePath: i["RemotePath"] };
                openComponent("components/devopsNodesCreateAlter", {
                    title: "Node Editor",
                    params: {
                        "node": editNode,
                        callback: function (ret) {
                            rpcAEP("CreateAlterNode", ret, function (res) {
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
            getNodes() {
                rpcAEP("GetNodes", {}, function (res) {
                    _this.c.nodes = R0R(res);
                    if (fixNull(_this.c.selectedNode, '') === '' && _this.c.nodes.length > 0) _this.c.selectedNode = _this.c.nodes[0];
                    _this.c.calcPageState();
                });
            },
            calcPageState() {
                let itemsInProgress = _.filter(_this.c.nodes, function (n) { return n["ProgressState"] !== 'NotExist'; })
                _this.c.inProgress = (itemsInProgress.length > 0);
                if (_this.c.inProgress === false) {
                    if (_this.c.refreshInterval !== null) clearInterval(_this.c.refreshInterval);
                    _this.c.refreshInterval = null;
                }
                else {
                    if (_this.c.refreshInterval === null) _this.c.refreshInterval = setInterval(function () { _this.c.getNodes(); }, 5000);
                }
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