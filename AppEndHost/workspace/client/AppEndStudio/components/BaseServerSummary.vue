<template>
    <div class="container-fluid">
        <div class="row">
            <div class="col-48">
                <div class="card rounded-0 border-0 shadow-sm">
                    <div class="card-body">
                        <div class="text-dark fs-d9 fw-bold px-2">
                            Server Overview
                        </div>
                        <hr class="my-1" />
                        <div class="p-2 fs-d9">

                            <div class="mt-1 overflow-x-hidden">
                                <span class="text-muted">Phisical Address</span> : <span class="text-secondary fw-bold">{{d["ServerPhisicalAddress"]}}</span>
                            </div>
                            <div class="mt-1">
                                <span class="text-muted">Server DateTime</span> : <span class="text-secondary fw-bold">{{d["ServerDateTime"]}}</span>
                            </div>
                            <div class="mt-1">
                                <span class="text-muted">Server TimeZone</span> : <span class="text-secondary fw-bold">{{d["ServerTimeZone"]}}</span>
                            </div>
                            <div class="mt-1">
                                <span class="text-muted">HostName</span> : <span class="text-secondary fw-bold">{{d["HostName"]}}</span>
                            </div>
                            <div class="mt-1">
                                <span class="text-muted">IpAddress</span> : <span class="text-secondary fw-bold">{{d["IpAddress"]}}</span>
                            </div>


                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    let _this = { cid: "", c: null, d: {} };

    export default {
        methods: {
            loadAppEndSummary() {
                rpcAEP("GetAppEndSummary", {}, function (res) {
                    _this.c.d = res[0]["Result"];
                });
            }
        },
        setup(props) {
            _this.cid = props['cid'];
        },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { _this.c.loadAppEndSummary(); },
        props: { cid: String }
    }
</script>

