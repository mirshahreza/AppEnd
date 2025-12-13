<template>
    <div class="card shadow-sm mb-2">
        <div class="card-body">
            <div class="text-dark fs-d9 fw-bold px-2">Server Overview</div>
            <hr class="my-1" />
            <div class="p-2 fs-d9">
                <!-- System info -->
                <div class="mt-1 overflow-x-hidden">
                    <span class="text-muted">Phisical Address</span> :
                    <span class="text-secondary fw-bold">{{ d["ServerPhisicalAddress"] }}</span>
                </div>
                <div class="mt-1">
                    <span class="text-muted">HostName</span> :
                    <span class="text-secondary fw-bold">{{ d["HostName"] }}</span>
                </div>
                <div class="mt-1">
                    <span class="text-muted">IpAddress</span> :
                    <span class="text-secondary fw-bold">{{ d["IpAddress"] }}</span>
                </div>

                <div class="my-2 text-light">.</div>

                <div class="mt-1">
                    <span class="text-muted">Server DateTime</span> :
                    <span class="text-secondary fw-bold">{{ d["ServerDateTime"] }}</span>
                </div>
                <div class="mt-1">
                    <span class="text-muted">Server TimeZone</span> :
                    <span class="text-secondary fw-bold">{{ d["ServerTimeZone"] }}</span>
                </div>

                <div class="my-2 text-light">.</div>

                <!-- Serilog info -->
                <div class="mt-1">
                    <span class="text-muted">Serilog Batch Limit</span> :
                    <span class="text-secondary fw-bold">{{ d["SerilogBatchPostingLimit"] ?? '-' }}</span>
                </div>
                <div class="mt-1">
                    <span class="text-muted">Serilog Batch Period (sec)</span> :
                    <span class="text-secondary fw-bold">{{ d["SerilogBatchPeriodSeconds"] ?? '-' }}</span>
                </div>

                <!-- small gap between groups -->
                <div class="my-2 text-light">.</div>

                <!-- Integrations info -->
                <div class="mt-1">
                    <span class="text-muted">Db Connections</span> :
                    <span class="text-secondary fw-bold">{{ (d["DbServerNames"] || []).join(', ') }}</span>
                </div>
                <div class="mt-1">
                    <span class="text-muted">LLM Providers</span> :
                    <span class="text-secondary fw-bold">{{ (d["LLMProviderNames"] || []).join(', ') }}</span>
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

