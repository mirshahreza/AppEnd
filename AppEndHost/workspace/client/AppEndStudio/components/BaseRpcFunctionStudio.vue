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
                                <button type="button" class="btn btn-sm btn-success" @click="renderKey = nextRenderKey()">Render</button>
                                <div class="vr mx-2"></div>
                                <button type="button" class="btn btn-sm btn-outline-primary" @click="openUsage">Usage</button>
                            </div>
                        </div>
                        <div class="card-body p-2 scrollable">
                            <div class="card border-0 shadow-sm rounded-0 mb-2 ae-card-accent ae-card-accent--info">
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

                            <div class="card border-0 shadow-sm rounded-0 mb-2 ae-card-accent ae-card-accent--warning">
                                <div class="card-header px-2 py-1 bg-light">
                                    <div class="hstack">
                                        <span class="fw-bold">Method</span>
                                        <input class="form-control form-control-sm" style="max-width:360px;" v-model="method" placeholder="e.g. Zzz.AppEndProxy.SomeMethod" />
                                        <button type="button" class="btn btn-sm btn-outline-secondary" @click="openMethodPicker">
                                            <i class="fa-solid fa-fw fa-search"></i>
                                        </button>
                                        <div class="p-0 ms-auto"></div>
                                        <div class="vr mx-2"></div>
                                        <button type="button" class="btn btn-sm btn-outline-secondary" @click="applyExample">Example</button>
                                    </div>
                                </div>
                                <div class="card-body bg-light p-1 ps-2">
                                    <label class="form-label mb-1">Method Signature <span class="text-secondary fs-d8 mt-1">First line of C# method</span></label>
                                </div>
                                <div class="card-body p-0">
                                    <div class="border-0" style="height: 86px; direction:ltr; text-align:left;" ref="signatureAce"></div>
                                </div>
                            </div>

                            <div class="card border-0 shadow-sm rounded-0 mb-2 ae-card-accent ae-card-accent--success">
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
                                <div class="card-body p-0">
                                    <div class="border" style="height: 220px; direction:ltr; text-align:left;" ref="defaultInputsAce"></div>
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
    mounted() {
        this.initDefaultInputsAce();
        this.initSignatureAce();

        // Ace needs a resize after the DOM is fully laid out (especially with flex/splitter).
        this.$nextTick(() => {
            setTimeout(() => this.resizeEditors(), 0);
            setTimeout(() => this.resizeEditors(), 100);
            setTimeout(() => this.resizeEditors(), 400);
        });

        window.addEventListener('resize', this.resizeEditors);
    },
    beforeUnmount() {
        try {
            window.removeEventListener('resize', this.resizeEditors);
            if (this._defaultInputsAce) {
                this._defaultInputsAce.destroy();
                this._defaultInputsAce = null;
            }
            if (this._signatureAce) {
                this._signatureAce.destroy();
                this._signatureAce = null;
            }
        } catch {
            // ignore
        }
    },
    methods: {
        ensureAceBasePath() {
            try {
                if (typeof ace === 'undefined') return;
                if (!ace.config || !ace.config.set) return;
                if (this._aceBasePathSet === true) return;

                // Needed for dynamic loading of modes such as `mode-csharp.js`
                ace.config.set('basePath', '/a..lib/ace/src-min');
                ace.config.set('modePath', '/a..lib/ace/src-min');
                ace.config.set('themePath', '/a..lib/ace/src-min');
                ace.config.set('workerPath', '/a..lib/ace/src-min');
                this._aceBasePathSet = true;
            } catch {
                // ignore
            }
        },
        ensureAceMode(modeName) {
            try {
                if (typeof ace === 'undefined') return;
                if (!modeName) return;

                this.ensureAceBasePath();

                const modePath = `ace/mode/${modeName}`;
                if (ace.require && ace.require(modePath)) return;

                if (ace.config && ace.config.loadModule) {
                    ace.config.loadModule(modePath, () => { });
                }
            } catch {
                // ignore
            }
        },
        resizeEditors() {
            try { if (this._defaultInputsAce) this._defaultInputsAce.resize(true); } catch { /* ignore */ }
            try { if (this._signatureAce) this._signatureAce.resize(true); } catch { /* ignore */ }
        },
        initSignatureAce() {
            const start = () => {
                if (!this.$refs.signatureAce) return;
                if (typeof ace === 'undefined') return;

                this.ensureAceMode('csharp');

                const editor = ace.edit(this.$refs.signatureAce, {
                    theme: 'ace/theme/cloud9_day',
                    mode: 'ace/mode/csharp',
                    fontSize: 12,
                    showPrintMargin: false,
                    wrap: false,
                    useWorker: false,
                    readOnly: true,
                    highlightActiveLine: false,
                    highlightGutterLine: false
                });
                this._signatureAce = editor;

                editor.session.setTabSize(4);
                editor.session.setUseSoftTabs(true);

                editor.renderer.setShowGutter(false);
                editor.renderer.setPadding(8);

                editor.session.setValue(this.signature || '');
                editor.clearSelection();

                this.$nextTick(() => {
                    try { editor.resize(true); } catch { /* ignore */ }
                });
            };

            const retry = (n) => {
                start();
                if (this._signatureAce) return;
                if (n <= 0) return;
                setTimeout(() => retry(n - 1), 50);
            };

            retry(40);
        },
        initDefaultInputsAce() {
            const start = () => {
                if (!this.$refs.defaultInputsAce) return;
                if (typeof ace === 'undefined') return;

                this.ensureAceBasePath();

                const editor = ace.edit(this.$refs.defaultInputsAce);
                this._defaultInputsAce = editor;

                editor.setTheme('ace/theme/cloud9_day');
                editor.session.setMode('ace/mode/json');
                editor.session.setTabSize(2);
                editor.session.setUseSoftTabs(true);
                editor.setOptions({
                    fontSize: 12,
                    showPrintMargin: false,
                    wrap: true,
                    useWorker: false
                });

                editor.session.setValue(this.defaultInputsText || '');

                editor.session.on('change', () => {
                    this.defaultInputsText = editor.session.getValue();
                });

                this.$nextTick(() => {
                    try { editor.resize(true); } catch { /* ignore */ }
                });
            };

            const retry = (n) => {
                start();
                if (this._defaultInputsAce) return;
                if (n <= 0) return;
                setTimeout(() => retry(n - 1), 50);
            };

            retry(40);
        },
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
        openMethodPicker() {
            const htmlEscape = (s) => fixNull(s, "").toString().replaceAll("&", "&amp;").replaceAll("<", "&lt;").replaceAll(">", "&gt;").replaceAll('"', "&quot;").replaceAll("'", "&#039;");

            const highlight = (text, q) => {
                text = fixNull(text, "").toString();
                q = fixNull(q, "").toString().trim();
                if (!q) return htmlEscape(text);
                const idx = text.toLowerCase().indexOf(q.toLowerCase());
                if (idx < 0) return htmlEscape(text);
                const a = text.substring(0, idx);
                const b = text.substring(idx, idx + q.length);
                const c = text.substring(idx + q.length);
                return `${htmlEscape(a)}<span class=\"ae-rpcstudio-match\">${htmlEscape(b)}</span>${htmlEscape(c)}`;
            };

            const renderList = (classes, q) => {
                q = fixNull(q, "").toString().trim().toLowerCase();
                let out = "";
                (classes || []).forEach((c) => {
                    const ns = fixNull(c.Namespace, "");
                    const cn = fixNull(c.ClassName, "");
                    const methods = (c.Methods || []).filter((m) => {
                        const mn = fixNull(m.Name, "");
                        const sig = fixNull(m.Signature, "");
                        const fqn = `${ns}.${cn}.${mn}`;
                        if (!q) return true;
                        return fqn.toLowerCase().includes(q) || sig.toLowerCase().includes(q);
                    });
                    if (methods.length === 0) return;

                    out += `<div class=\"mb-3\">`;
                    out += `<div class=\"ae-rpcstudio-methodgroup-title text-primary\" style=\"direction:ltr;text-align:left;\">${highlight(ns, q)}<span class=\"text-secondary\">.</span>${highlight(cn, q)}</div>`;
                    out += `<div class=\"border rounded bg-white\">`;
                    methods.forEach((m) => {
                        const mn = fixNull(m.Name, "");
                        const signature = fixNull(m.Signature, "");
                        const fqn = `${ns}.${cn}.${mn}`;
                        out += `<div class=\"border-bottom p-2 ae-rpcstudio-methodrow\" style=\"cursor:pointer;\" data-method=\"${htmlEscape(fqn)}\" data-signature=\"${htmlEscape(signature)}\">`;
                        out += `<div style=\"direction:ltr;text-align:left;\">`;
                        out += `<span class=\"ae-rpcstudio-methodname\">${highlight(mn, q)}</span>`;
                        out += `<span class=\"ae-rpcstudio-methodsig ms-2\">${highlight(signature || "", q)}</span>`;
                        out += `</div>`;
                        out += `</div>`;
                    });
                    out += `</div></div>`;
                });
                if (!out) out = `<div class=\"text-secondary fst-italic\">No methods found</div>`;
                return out;
            };

            const attachHandlers = (cid, classes) => {
                const root = document.getElementById(cid);
                if (!root) return false;
                const s = root.querySelector('#aeRpcStudioMethodSearch');
                const list = root.querySelector('#aeRpcStudioMethodList');
                if (!s || !list) return false;

                const update = () => {
                    list.innerHTML = renderList(classes, s.value);
                    wireRows();
                };

                const wireRows = () => {
                    let rows = list.querySelectorAll('.ae-rpcstudio-methodrow');
                    rows.forEach((r) => {
                        r.addEventListener('click', () => {
                            const pickedMethod = r.getAttribute('data-method');
                            const pickedSig = r.getAttribute('data-signature');
                            if (pickedMethod) this.method = pickedMethod;
                            if (pickedSig) {
                                this.signature = pickedSig;
                                if (this._signatureAce) {
                                    try {
                                        this._signatureAce.session.setValue(this.signature);
                                        this._signatureAce.clearSelection();
                                    } catch { /* ignore */ }
                                }
                            }
                            this.renderKey = this.nextRenderKey();
                            try { shared.closeComponent(cid); } catch { /* ignore */ }
                        });
                    });
                };

                s.addEventListener('input', update);
                setTimeout(() => { try { s.focus(); } catch { /* ignore */ } }, 0);
                update();
                return true;
            };

            const waitForModal = (cid, classes, n) => {
                if (attachHandlers(cid, classes) === true) return;
                if (n <= 0) return;
                setTimeout(() => waitForModal(cid, classes, n - 1), 50);
            };

            rpcAEP("GetRpcMethodCatalog", {}, (res) => {
                const classes = R0R(res) || [];
                const contentBody = `
<div class=\"p-2\">
  <div class=\"mb-2\">
    <input id=\"aeRpcStudioMethodSearch\" class=\"form-control form-control-sm ae-rpcstudio-search\" placeholder=\"Search (controller / method / signature)\" style=\"direction:ltr;text-align:left;\" />
  </div>
  <div id=\"aeRpcStudioMethodList\" class=\"scrollable\" style=\"max-height:70vh; overflow:auto;\"></div>
</div>`;

                openComponent("/a.SharedComponents/BaseContent", {
                    title: "Pick server method",
                    windowSizeSwitchable: true,
                    modalSize: "modal-xl",
                    params: {
                        content: { Title: "", ContentBody: contentBody }
                    },
                    caller: this
                });

                // Resolve the newest overlay `cid` by scanning `shared.params_*` keys (openComponent stores params there).
                // This is more reliable than DOM selectors because the modal might not be `.show` yet.
                setTimeout(() => {
                    try {
                        const keys = Object.keys(shared || {}).filter(k => k && k.startsWith('params_overlay_component_'));
                        if (keys.length === 0) return;
                        keys.sort();
                        const lastKey = keys[keys.length - 1];
                        const cid = lastKey.replace('params_', '');
                        if (cid) waitForModal(cid, classes, 60);
                    } catch {
                        // ignore
                    }
                }, 20);
            }, (e) => {
                showError(fixNull(e, "Failed to load methods"));
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
            this.renderKey = this.nextRenderKey();
            this.last = null;
            this.lastError = null;

            if (this._defaultInputsAce) {
                try { this._defaultInputsAce.session.setValue(this.defaultInputsText); } catch { /* ignore */ }
            }

            if (this._signatureAce) {
                try {
                    this._signatureAce.session.setValue(this.signature);
                    this._signatureAce.clearSelection();
                } catch { /* ignore */ }
            }
        },
        nextRenderKey() {
            try {
                if (typeof genUN === 'function') return genUN('rk_');
            } catch {
                // ignore
            }
            return `rk_${this.uid || this.cid || '0'}_${Date.now()}`;
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
