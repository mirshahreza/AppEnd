<template>
    <div class="card border-0 bg-transparent rounded-0 h-100" :id="cid" :ae-data-ready="dataReady">
        <div class="card-body bg-transparent fs-d8 p-0">
            <div class="h-100 w-100" data-flex-splitter-horizontal style="flex: auto; overflow-x:hidden;" id="splitContainer">
                <div class="h-100" style="min-width:350px;width:32%;overflow:hidden">
                    <div class="card h-100 shadow-sm rounded-0 border-0">
                        <div class="card-header px-2 bg-warning-subtle ae-rpcstudio-toolbar">
                            <div class="hstack">
                                <span class="fw-bold">RPC Function Studio</span>
                                <div class="p-0 ms-auto"></div>
                                <button type="button" class="btn btn-sm btn-success" @click="renderKey = genUN('rk_')">Render</button>
                                <div class="vr mx-2"></div>
                                <button type="button" class="btn btn-sm btn-outline-primary" @click="openUsage">Usage</button>
                            </div>
                        </div>
                        <div class="card-body p-2 scrollable">
                            <div class="card border-0 shadow-sm rounded-0 mb-2">
                                <div class="card-header px-2 py-1 bg-light">
                                    <span class="fw-bold">UI</span>
                                </div>
                                <div class="card-body p-2">
                                    <div class="ae-rpcstudio-fieldgrid">
                                        <label class="form-label mb-0">Form title</label>
                                        <input class="form-control form-control-sm" v-model="formTitle" placeholder="(optional)" />
                                    </div>

                                    <div class="ae-rpcstudio-fieldgrid">
                                        <label class="form-label mb-0">Submit button text</label>
                                        <input class="form-control form-control-sm" v-model="submitText" placeholder="Call" />
                                    </div>

                                    <div class="ae-rpcstudio-fieldgrid mb-0">
                                        <label class="form-label mb-0">Header position</label>
                                        <select class="form-select form-select-sm" v-model="headerPosition">
                                            <option value="top">Top</option>
                                            <option value="bottom">Bottom</option>
                                        </select>
                                    </div>
                                </div>
                            </div>

                            <div class="card border-0 shadow-sm rounded-0 mb-2">
                                <div class="card-header px-2 py-1 bg-light">
                                    <div class="hstack">
                                        <span class="fw-bold">RPC Method</span>
                                        <input class="form-control form-control-sm" style="max-width:360px;" v-model="method" placeholder="e.g. Zzz.AppEndProxy.SomeMethod" />
                                        <button type="button" class="btn btn-sm btn-outline-secondary" @click="applyExample">
                                            <i class="fa-solid fa-fw fa-search"></i>
                                        </button>
                                        <div class="p-0 ms-auto"></div>
                                        <div class="vr mx-2"></div>
                                        <button type="button" class="btn btn-sm btn-outline-secondary" @click="applyExample">Example</button>
                                    </div>
                                </div>
                                <div class="card-body p-2">
                                    <div class="mb-2">
                                        <label class="form-label mb-1">Method Signature <span class="text-secondary fs-d8 mt-1">First line of C# method</span></label>
                                        <textarea class="form-control form-control-sm" rows="3" style="direction:ltr;text-align:left;" v-model="signature"></textarea>
                                        
                                    </div>
                                </div>
                            </div>

                            <div class="card border-0 shadow-sm rounded-0 mb-2">
                                <div class="card-header px-2 py-1 bg-light">
                                    <div class="hstack">
                                        <span class="fw-bold">Default inputs (JSON)</span>
                                        <div class="p-0 ms-auto"></div>
                                        <div class="form-check mb-0">
                                            <input class="form-check-input" type="checkbox" id="enableDefaultInputs" v-model="enableDefaultInputs">
                                            <label class="form-check-label" for="enableDefaultInputs">Apply default inputs</label>
                                        </div>
                                    </div>
                                </div>
                                <div class="card-body p-2">
                                    <textarea class="form-control form-control-sm" rows="8" style="direction:ltr;text-align:left;" v-model="defaultInputsText"></textarea>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div role="separator" tabindex="1" class="ae-rpcstudio-sep" style="width:6px; min-width:6px; cursor: col-resize;"></div>

                <div class="h-100" style="min-width:520px;width:46%;overflow:hidden">
                    <div class="card h-100 shadow-sm rounded-0 border-0">
                        <div class="card-body p-2 scrollable">
                            <component
                                :key="renderKey"
                                :is="callerComp"
                                :title="formTitle"
                                :submitText="submitText"
                                :signature="signature"
                                :method="method"
                                :defaultInputs="defaultInputs"
                                :headerPosition="headerPosition"
                                :showOutputs="true"
                                :silent="false"
                                @done="onDone"
                                @fail="onFail"
                            />
                        </div>
                    </div>
                </div>

                <div role="separator" tabindex="1" class="ae-rpcstudio-sep" style="width:6px; min-width:6px; cursor: col-resize;"></div>

                <div class="h-100" style="min-width:360px;width:22%;overflow:hidden">
                    <div class="card h-100 shadow-sm rounded-0 border-0">
                        <div class="card-header px-2 ae-rpcstudio-toolbar">
                            <div class="hstack">
                                <span class="fw-bold">Last call</span>
                            </div>
                        </div>
                        <div class="card-body p-2 scrollable">
                            <div v-if="last" class="h-100">
                                <pre class="p-2 bg-light border h-100 mb-0" style="direction:ltr;text-align:left;max-height:100%;overflow:auto;white-space:pre-wrap;word-break:break-word;">{{ lastText }}</pre>
                            </div>
                            <div v-else class="h-100 d-flex align-items-center text-center">
                                <div class="w-100 fst-italic fs-1d2 text-secondary">No calls yet</div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
import BaseRpcFunctionCaller from "../../a.SharedComponents/BaseRpcFunctionCaller.vue";

let _this = { cid: "", uid: "", c: null };

export default {
    components: { BaseRpcFunctionCaller },
    props: { cid: String, uid: String },
    data() {
        return {
            dataReady: "false",
            formTitle: "PingMe",
            submitText: "Call",
            signature: "public static object? PingMe()",
            method: "Zzz.AppEndProxy.PingMe",
            enableDefaultInputs: true,
            defaultInputsText: "{}",
            headerPosition: "top",
            renderKey: "rk_0",
            last: null,
            lastError: null
        };
    },
    computed: {
        callerComp() { return BaseRpcFunctionCaller; },
        defaultInputs() {
            if (this.enableDefaultInputs !== true) return {};
            try { return JSON.parse(this.defaultInputsText); }
            catch { return {}; }
        },
        usageSnippet() {
            const sig = (this.signature || "").replaceAll('`', "'");
            const method = (this.method || "").replaceAll('`', "'");
            return `<BaseComponentLoader\n  src=\"/a.SharedComponents/BaseRpcFunctionCaller\"\n  :params=\"{\n    signature: \\\"${sig}\\\",\n    method: \\\"${method}\\\",\n    defaultInputs: ${this.pretty(this.defaultInputs)},\n    headerPosition: \\\"${this.headerPosition}\\\"\n  }\"\n/>`;
        },
        lastText() {
            if (!this.last) return "";
            try { return JSON.stringify(this.last, null, 2); }
            catch { return this.last.toString(); }
        }
    },
    created() {
        _this.c = this;
        _this.uid = fixNull(this.uid, genUN('rpcstudio_'));
        _this.cid = _this.uid;
        this.dataReady = "true";
    },
    methods: {
        pretty(o) {
            try { return JSON.stringify(o, null, 2); }
            catch { return "{}"; }
        },
        openUsage() {
            openComponent("/a.SharedComponents/BaseContent", {
                title: "Usage snippet",
                windowSizeSwitchable: true,
                modalSize: "modal-lg",
                params: {
                    content: {
                        Title: "Usage snippet",
                        ContentBody: `<pre class="p-2 bg-light border" style="direction:ltr;text-align:left;max-height:70vh;overflow:auto;">${(this.usageSnippet || "").replaceAll("<", "&lt;")}</pre>`
                    }
                }
            });
        },
        applyExample() {
            this.formTitle = "PingMe";
            this.submitText = "Call";
            this.signature = "public static object? PingMe()";
            this.method = "Zzz.AppEndProxy.PingMe";
            this.enableDefaultInputs = true;
            this.defaultInputsText = "{}";
            this.headerPosition = "top";
            this.renderKey = genUN('rk_');
            this.last = null;
            this.lastError = null;
        },
        onDone(payload) {
            this.last = payload;
            this.lastError = null;
        },
        onFail(e) {
            this.lastError = e;
            this.last = { error: e };
        }
    }
};
</script>

<style>
.ae-rpcstudio-toolbar {
    min-height: 44px;
    display: flex;
    align-items: center;
    padding-top: 0.25rem !important;
    padding-bottom: 0.25rem !important;
    border-bottom-width: 1px !important;
}

.ae-rpcstudio-sep {
    background: linear-gradient(90deg,
        rgba(0, 0, 0, 0.08) 0%,
        rgba(0, 0, 0, 0.14) 50%,
        rgba(0, 0, 0, 0.08) 100%);
    box-shadow: inset 0 0 0 1px rgba(0, 0, 0, 0.10);
}

.ae-rpcstudio-fieldgrid {
    display: grid;
    grid-template-columns: 160px minmax(0, 1fr);
    align-items: center;
    column-gap: 0.5rem;
    margin-bottom: 0.5rem;
}

</style>
