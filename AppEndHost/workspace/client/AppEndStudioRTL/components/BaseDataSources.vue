<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-header p-2 bg-primary-subtle-light rounded-0 border-0">
            <div class="hstack gap-1">
                <button type="button" class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="create">
                    <i class="fa-solid fa-file-alt fa-bounce" style="--fa-animation-iteration-count:1"></i> <span>New Datasource</span>
                </button>
                <div class="p-0 ms-auto"></div>
            </div>
        </div>
        <div class="card-body p-0">
            <div class="card h-100 bg-light bg-opacity-75 border-0">
                <div class="card-body fs-d8 p-0 bg-transparent scrollable">

                    <table class="table table-sm table-hover w-100 ae-table m-0 bg-transparent">
                        <thead>
                            <tr>
                                <th style="width:250px;">Name</th>
                                <th style="width:120px;">Type</th>
                                <th>ConnectionInfo</th>
                                <th style="width:150px;" class="text-end"></th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="ds in d">
                                <td :data-ae-key="ds.Name">
                                    <button class="btn btn-link btn-sm text-primary text-hover-success p-0 text-decoration-none" @click="edit">
                                        <i class="fa-solid fa-edit me-1"></i>{{ds.Name}}
                                    </button>
                                </td>
                                <td>{{ds.ServerType}}</td>
                                <td>{{ds.ConnectionString}}</td>
                                <td class="text-end">
                                    <button class="btn btn-link btn-sm text-secondary text-hover-danger p-0 text-decoration-none" @click="delete" v-if="ds.Name!=='DefaultRepo'">
                                        <i class="fa-solid fa-trash me-1"></i>Delete
                                    </button>
                                </td>
                            </tr>
                        </tbody>
                    </table>

                </div>
            </div>

        </div>
    </div>
</template>

<script>
    shared.setAppTitle("$auto$");
    let _this = { cid: "", c: null, d: []};
    export default {
        methods: {
            create() {
                openComponent("components/BaseDataSourcesCreateAlter", {
                    title: "ConnectionString Editor",
                    params: {
                        "IsNew": true, "Name": "", "ServerType": "", "ConnectionString": "",
                        callback: function () {
                            _this.c.readList();
                        }
                    }
                });
            },
            readList() {
                rpcAEP("GetDataSourcesWithCnn", {}, function (res) {
                    _this.c.d = R0R(res);
                });
            },
            edit(e) {
                let r = getRow(_this.c.d, "Name", getKey(e));
                r.callback = function () {
                    _this.c.readList();
                };
                openComponent("components/BaseDataSourcesCreateAlter", { title: "ConnectionInfo Editor", params: r });
            },
            delete(e) {
                let k = getKey(e);
                shared.showConfirm({
                    title: "Remove Datasource", message1: "Are you sure you want to remove this item?", message2: k,
                    callback: function () {
                        rpcAEP("RemoveDbServer", { "DbServerName": k }, function () {
                            _this.c.readList();
                        });
                    }
                });
            }
        },
        setup(props) {
            _this.cid = props['cid'];
        },
        data() { return _this; },
        created() {_this.c = this;},
        mounted() {_this.c.readList();},
        props: { cid: String },
    }

</script>
