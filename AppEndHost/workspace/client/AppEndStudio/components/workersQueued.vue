<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-header p-2 bg-light-subtle rounded-0 border-0">
            <div class="hstack gap-1">
                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="readList">
                    <i class="fa-solid fa-fw fa-refresh"></i> <span >Refresh</span>
                </button>
                <div class="p-0 ms-auto"></div>
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
                                        <i class="fa-solid fa-fw fa-spinner fa-spin text-success" v-if="wInfo.ProgressState==='Running'"></i>
                                        <i class="fa-solid fa-fw fa-q text-danger" v-if="wInfo.ProgressState==='Waiting'"></i>
                                        <span class="fw-bold">{{wName}}</span>
                                    </div>
                                    <div class="card-header p-2">
                                        QueuedOn <span class="fw-bold fs-d8">{{shared.formatDateTime(wInfo["StartedOn"])}}</span>
                                    </div>
                                    <div class="card-body p-1">
                                        <pre class="m-0">
{{JSON.stringify(shared.removeProp(shared.removeProp(wInfo, "StartedOn"),"ProgressState"),null,4)}}
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
    shared.setAppTitle("Queued Workers");
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
