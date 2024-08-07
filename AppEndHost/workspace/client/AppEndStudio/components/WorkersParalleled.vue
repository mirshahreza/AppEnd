<template>
    <div class="card h-100 rounded rounded-2 rounded-bottom-0 rounded-end-0 bg-transparent border-0">
        <div class="card-header p-2 bg-light-subtle rounded-end-0 border-0">
            <div class="input-group input-group-sm border-0 bg-transparent">
                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="readList"><i class="fa-solid fa-refresh"></i> Refresh</button>
                <input type="text" class="form-control form-control-sm border-0 rounded-0 bg-transparent" disabled />
            </div>
        </div>
        <div class="card-body p-2">
            <div class="card h-100 border-light bg-light bg-opacity-75 border-0">
                <div class="card-body scrollable">

                    <div v-else class="container-fluid">
                        <div class="row row-cols-1 row-cols-md-3">
                            <div class="col" v-for="(wInfo,wName) in workers">
                                <div class="card h-100 shadow-sm fs-d7">
                                    <div class="card-header p-2 pb-1">
                                        <span class="fw-bold">{{wName}}</span>
                                    </div>
                                    <div class="card-header p-2 fs-d8">
                                        StartedOn <span class="fw-bold">{{shared.formatDateTime(wInfo["StartedOn"])}}</span>
                                    </div>
                                    <div class="card-body p-1">
<pre class="m-0">
{{JSON.stringify(shared.removeProp(wInfo, "StartedOn"),null,4)}}
</pre>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>
            </div>

        </div>
    </div>
</template>

<script>
    shared.setAppTitle("Paralleled Workers");
    let _this = { cid: "", c: null, workers: [] };

    export default {
        methods: {
            readList() {
                rpcAEP("GetAppEndBackgroundWorkerQueueItems", {}, function (res) {
                    _this.c.workers = R0R(res);
                });
            },
            refreshEvery(seconds) {
                setInterval(function () { _this.c.readList(); }, seconds * 1000);
            }
        },
        setup(props) { _this.cid = props['cid']; },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { _this.c.readList(); _this.c.refreshEvery(10) },
        props: { cid: String }
    }


</script>
