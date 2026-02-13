<template>
    <div class="p-4 h-100">
        <div :id="cid" :ae-data-ready="dataReady" class="card border-0 shadow-lg rounded h-100 ae-rpc-fn-caller" style="overflow:hidden;">
            <div class="card-body bg-transparent fs-d8 p-0 h-100">
                <div class="d-flex flex-column h-100">

                    <div v-if="headerPosition === 'top'" class="card-header px-2 ae-rpc-fn-toolbar" style="background-color:#ffdaa6 !important; color:#fff !important;">
                        <div class="hstack">
                            <button type="button" class="btn btn-sm px-3" 
                                    style="min-width:84px; background-color: rgb(247, 255, 232) !important; border-color: rgb(255, 240, 240) !important;" 
                                    @click="call" :disabled="loading">
                                <i class="fa-solid fa-fw fa-play me-1"></i>{{ submitText }}
                            </button>
                            <div class="p-0 ms-auto"></div>
                            <span class="fw-bold text-end" style="min-width:0;">{{ titleComputed }}</span>
                        </div>
                    </div>

                    <div class="card-body p-2" style="flex:1 1 auto; min-height:0; overflow:hidden;">
                        <div v-if="parseError" class="alert alert-danger py-2 mb-0">
                            {{ parseError }}
                        </div>

                        <div v-else class="d-flex flex-column h-100">
                            <div v-if="meta.params.length === 0" class="text-secondary fs-d8 mb-2 flex-shrink-0">No input parameters</div>

                            <div v-if="showOutputs && splitOutputs" class="w-100" data-flex-splitter-vertical style="flex:1 1 auto; min-height:0; overflow: hidden;">
                                <div style="min-height:140px;height:calc(55% - 4px);overflow:hidden;">
                                    <div class="scrollable" style="height:100%;padding-right:2px;">
                                        <div v-for="p in meta.params" :key="p.name" class="mb-2">
                                            <label class="form-label mb-1">{{ p.name }} <span class="text-secondary fs-d8">({{ p.type }})</span></label>
                                            <input v-if="isSimple(p.type)" type="text" class="form-control form-control-sm" v-model="inputs[p.name]" />
                                            <textarea v-else class="form-control form-control-sm" rows="4" style="direction:ltr;text-align:left;" v-model="inputsJson[p.name]"></textarea>
                                        </div>
                                    </div>
                                </div>
                                <div role="separator" tabindex="1" style="height:8px; min-height:8px; background: linear-gradient(180deg, rgba(0,0,0,0.06) 0%, rgba(0,0,0,0.18) 50%, rgba(0,0,0,0.06) 100%);"></div>
                                <div style="min-height:140px;height:calc(45% - 4px);overflow:hidden;">
                                    <div class="h-100 w-100">
                                        <div v-if="outputRenderKind === 'empty'" class="text-secondary fs-d8">(empty)</div>

                                        <div v-else-if="outputRenderKind === 'primitive'" class="p-2 bg-light border h-100" style="direction:ltr;text-align:left;overflow:auto;white-space:pre-wrap;word-break:break-word;">
                                            {{ outputPrimitiveText }}
                                        </div>

                                        <div v-else-if="outputRenderKind === 'grid'" class="table-responsive h-100" style="overflow:auto;">
                                            <table class="table table-sm table-striped table-bordered mb-0 ae-rpc-fn-grid">
                                                <thead>
                                                    <tr>
                                                        <th v-for="c in outputGridColumns" :key="c" class="text-nowrap">{{ c }}</th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    <tr v-for="(row, idx) in outputGridRows" :key="idx">
                                                        <td v-for="c in outputGridColumns" :key="c" style="direction:ltr;text-align:left;white-space:pre-wrap;word-break:break-word;">
                                                            {{ formatCell(row, c) }}
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </div>

                                        <div v-else-if="outputRenderKind === 'kv'" class="table-responsive h-100" style="overflow:auto;">
                                            <table class="table table-sm table-striped table-bordered mb-0 ae-rpc-fn-kv">
                                                <tbody>
                                                    <tr v-for="k in outputObjectKeys" :key="k">
                                                        <th class="text-nowrap" style="width:1%;">{{ k }}</th>
                                                        <td style="direction:ltr;text-align:left;white-space:pre-wrap;word-break:break-word;">{{ formatAny(output[k]) }}</td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </div>

                                        <pre v-else class="p-2 bg-light border h-100 mb-0" style="direction:ltr;text-align:left;overflow:auto;white-space:pre-wrap;word-break:break-word;">{{ outputText }}</pre>
                                    </div>
                                </div>
                            </div>

                            <div v-else style="flex:1 1 auto; min-height:0; overflow:auto;">
                                <div v-for="p in meta.params" :key="p.name" class="mb-2">
                                    <label class="form-label mb-1">{{ p.name }} <span class="text-secondary fs-d8">({{ p.type }})</span></label>
                                    <input v-if="isSimple(p.type)" type="text" class="form-control form-control-sm" v-model="inputs[p.name]" />
                                    <textarea v-else class="form-control form-control-sm" rows="4" style="direction:ltr;text-align:left;" v-model="inputsJson[p.name]"></textarea>
                                </div>

                                <div v-if="showOutputs" class="mt-3">
                                    <div class="fw-bold mb-1">Output</div>

                                    <div v-if="outputRenderKind === 'empty'" class="text-secondary fs-d8">(empty)</div>

                                    <div v-else-if="outputRenderKind === 'primitive'" class="p-2 bg-light border" style="direction:ltr;text-align:left;max-height:340px;overflow:auto;white-space:pre-wrap;word-break:break-word;">
                                        {{ outputPrimitiveText }}
                                    </div>

                                    <div v-else-if="outputRenderKind === 'grid'" class="table-responsive" style="max-height:340px;overflow:auto;">
                                        <table class="table table-sm table-striped table-bordered mb-0 ae-rpc-fn-grid">
                                            <thead>
                                                <tr>
                                                    <th v-for="c in outputGridColumns" :key="c" class="text-nowrap">{{ c }}</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr v-for="(row, idx) in outputGridRows" :key="idx">
                                                    <td v-for="c in outputGridColumns" :key="c" style="direction:ltr;text-align:left;white-space:pre-wrap;word-break:break-word;">
                                                        {{ formatCell(row, c) }}
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>

                                    <div v-else-if="outputRenderKind === 'kv'" class="table-responsive" style="max-height:340px;overflow:auto;">
                                        <table class="table table-sm table-striped table-bordered mb-0 ae-rpc-fn-kv">
                                            <tbody>
                                                <tr v-for="k in outputObjectKeys" :key="k">
                                                    <th class="text-nowrap" style="width:1%;">{{ k }}</th>
                                                    <td style="direction:ltr;text-align:left;white-space:pre-wrap;word-break:break-word;">{{ formatAny(output[k]) }}</td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>

                                    <pre v-else class="p-2 bg-light border" style="direction:ltr;text-align:left;max-height:340px;overflow:auto;white-space:pre-wrap;word-break:break-word;">{{ outputText }}</pre>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div v-if="headerPosition === 'bottom'" class="card-header px-2 ae-rpc-fn-toolbar mt-auto" style="background-color:#17a2b8 !important; color:#fff !important;">
                        <div class="hstack">
                            <button type="button" class="btn btn-sm text-white shadow-sm px-3" style="min-width:84px; background-color: rgba(13, 110, 253, 0.85) !important; border-color: rgba(13, 110, 253, 0.85) !important;" @click="call" :disabled="loading">
                                <i class="fa-solid fa-fw fa-play me-1"></i>{{ submitText }}
                            </button>
                            <div class="p-0 ms-auto"></div>
                            <span class="fw-bold text-end" style="min-width:0;">{{ titleComputed }}</span>
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </div>
</template>

<script>

    let _this = { cid: "", uid: "", c: null };

    export default {
        props: {
            cid: String,
            uid: String,
            title: String,
            submitText: { type: String, default: 'Call' },
            signature: { type: String, required: true },
            method: { type: String, required: true },
            defaultInputs: { type: Object, default: () => ({}) },
            silent: { type: Boolean, default: true },
            showOutputs: { type: Boolean, default: true },
            splitOutputs: { type: Boolean, default: true },
            headerPosition: { type: String, default: 'top' } // 'top' | 'bottom'
        },
        data() {
            return {
                dataReady: "false",
                loading: false,
                meta: { returnType: "void", name: "", params: [] },
                parseError: "",
                inputs: {},
                inputsJson: {},
                output: null,
                rawResponse: null
            };
        },
        computed: {
            titleComputed() {
                return fixNull(this.title, fixNull(this.meta.name, this.method));
            },
            outputText() {
                if (this.output === null || this.output === undefined) return "";
                try { return JSON.stringify(this.output, null, 2); }
                catch { return this.output.toString(); }
            },
            outputRenderKind() {
                const o = this.output;
                if (o === null || o === undefined) return 'empty';
                if (_.isString(o) || _.isNumber(o) || _.isBoolean(o)) return 'primitive';
                if (_.isDate && _.isDate(o)) return 'primitive';
                if (_.isArray(o)) {
                    if (o.length === 0) return 'grid';
                    const allObjects = _.every(o, x => x && typeof x === 'object' && !_.isArray(x));
                    const allPrimitive = _.every(o, x => x === null || x === undefined || _.isString(x) || _.isNumber(x) || _.isBoolean(x) || (_.isDate && _.isDate(x)));
                    return (allObjects || allPrimitive) ? 'grid' : 'json';
                }
                if (typeof o === 'object') return 'kv';
                return 'json';
            },
            outputPrimitiveText() {
                const o = this.output;
                if (o === null || o === undefined) return '';
                if (_.isDate && _.isDate(o)) return o.toISOString ? o.toISOString() : o.toString();
                return o.toString();
            },
            outputGridColumns() {
                const rows = this.outputGridRows;
                const set = new Set();
                _.forEach(rows, r => {
                    if (r === null || r === undefined) return;
                    if (_.isString(r) || _.isNumber(r) || _.isBoolean(r) || (_.isDate && _.isDate(r))) {
                        set.add('Value');
                        return;
                    }
                    _.forEach(Object.keys(r || {}), k => set.add(k));
                });
                const cols = Array.from(set);
                if (cols.length === 0) cols.push('Value');
                return cols;
            },
            outputGridRows() {
                const o = this.output;
                if (!_.isArray(o)) return [];
                return o;
            },
            outputObjectKeys() {
                const o = this.output;
                if (!o || typeof o !== 'object' || _.isArray(o)) return [];
                return Object.keys(o);
            }
        },
        created() {
            _this.c = this;
            _this.uid = fixNull(this.uid, genUN('rpcfn_'));
            _this.cid = _this.uid;

            try {
                this.meta = this.parseSignature(this.signature);
                this.initInputs();
                this.dataReady = "true";
            } catch (ex) {
                this.parseError = ex && ex.message ? ex.message : (ex || "Signature parse error").toString();
                this.dataReady = "true";
            }
        },
        mounted() {
            // Splitters are handled by the repo's flex-splitter plugin via `data-flex-splitter-*` attributes.
        },
        beforeUnmount() {
            try {
                // nothing
            } catch {
                // ignore
            }
        },
        methods: {
            formatAny(v) {
                if (v === null || v === undefined) return '';
                if (_.isString(v) || _.isNumber(v) || _.isBoolean(v)) return v.toString();
                if (_.isDate && _.isDate(v)) return v.toISOString ? v.toISOString() : v.toString();
                try { return JSON.stringify(v); }
                catch { return v.toString(); }
            },
            formatCell(row, col) {
                if (!row) return '';
                if (col === 'Value' && (_.isString(row) || _.isNumber(row) || _.isBoolean(row) || (_.isDate && _.isDate(row)))) {
                    return this.formatAny(row);
                }
                if (typeof row !== 'object') return this.formatAny(row);
                return this.formatAny(row[col]);
            },
            isSimple(typeName) {
                typeName = fixNull(typeName, "");
                const t = typeName.replaceAll("?", "").toLowerCase();
                return ["string", "int", "long", "short", "byte", "decimal", "double", "float", "bool", "boolean", "datetime", "guid"].includes(t);
            },
            initInputs() {
                this.inputs = _.cloneDeep(this.defaultInputs || {});
                this.inputsJson = {};

                _.forEach(this.meta.params, (p) => {
                    if (this.inputs[p.name] === undefined) this.inputs[p.name] = "";
                    if (!this.isSimple(p.type)) {
                        const existing = this.inputs[p.name];
                        this.inputsJson[p.name] = _.isString(existing) ? existing : JSON.stringify(existing ?? {}, null, 2);
                    }
                });
            },
            parseSignature(sig) {
                if (!sig || !sig.toString().trim()) throw new Error("Empty signature");
                sig = sig.toString().trim();
                sig = sig.replaceAll("\r", " ").replaceAll("\n", " ");
                sig = sig.replace(/\s+/g, " ").trim();

                // remove method body if user pasted more than one line
                const bodyIdx = sig.indexOf("{");
                if (bodyIdx > -1) sig = sig.substring(0, bodyIdx).trim();

                // remove attributes in front (simple)
                while (sig.startsWith("[")) {
                    const end = sig.indexOf("]");
                    if (end === -1) break;
                    sig = sig.substring(end + 1).trim();
                }

                // remove common modifiers
                const modifiers = ["public", "private", "protected", "internal", "static", "virtual", "override", "abstract", "async", "sealed", "extern", "unsafe", "new", "partial"];
                for (let i = 0; i < 20; i++) {
                    const first = sig.split(" ")[0];
                    if (!modifiers.includes(first)) break;
                    sig = sig.substring(first.length).trim();
                }

                const open = sig.indexOf("(");
                const close = sig.lastIndexOf(")");
                if (open === -1 || close === -1 || close < open) throw new Error("Invalid signature: missing parentheses");

                const before = sig.substring(0, open).trim();
                const inside = sig.substring(open + 1, close).trim();

                // before: "ReturnType MethodName" OR "Task<...> MethodName"
                const parts = before.split(" ");
                if (parts.length < 2) throw new Error("Invalid signature: cannot detect return type and name");
                const name = parts[parts.length - 1].trim();
                const returnType = parts.slice(0, parts.length - 1).join(" ").trim();

                const paramList = this.splitParams(inside)
                    .filter(s => s.trim() !== "")
                    .map(p => this.parseParam(p));

                return { returnType, name, params: paramList };
            },
            splitParams(s) {
                if (!s) return [];
                const res = [];
                let depthAngle = 0;
                let depthParen = 0;
                let sb = "";
                for (let i = 0; i < s.length; i++) {
                    const ch = s[i];
                    if (ch === '<') depthAngle++;
                    else if (ch === '>') depthAngle = Math.max(0, depthAngle - 1);
                    else if (ch === '(') depthParen++;
                    else if (ch === ')') depthParen = Math.max(0, depthParen - 1);

                    if (ch === ',' && depthAngle === 0 && depthParen === 0) {
                        res.push(sb);
                        sb = "";
                        continue;
                    }
                    sb += ch;
                }
                if (sb.trim() !== "") res.push(sb);
                return res;
            },
            parseParam(p) {
                p = p.trim();
                // remove default value "= ..."
                const eq = p.indexOf('=');
                if (eq > -1) p = p.substring(0, eq).trim();

                // strip modifiers
                const mods = ["in", "out", "ref", "params", "this"];
                let tokens = p.split(/\s+/g).filter(x => x);
                while (tokens.length > 0 && mods.includes(tokens[0])) tokens = tokens.slice(1);
                if (tokens.length < 2) throw new Error("Invalid parameter: " + p);

                const name = tokens[tokens.length - 1];
                const type = tokens.slice(0, tokens.length - 1).join(" ");
                return { name, type };
            },
            buildInputs() {
                const obj = {};
                _.forEach(this.meta.params, (p) => {
                    if (this.isSimple(p.type)) {
                        obj[p.name] = this.inputs[p.name];
                    } else {
                        const raw = fixNull(this.inputsJson[p.name], "").toString().trim();
                        if (raw === "") obj[p.name] = null;
                        else {
                            try { obj[p.name] = JSON.parse(raw); }
                            catch { obj[p.name] = raw; }
                        }
                    }
                });
                return obj;
            },
            call() {
                this.loading = true;
                this.output = null;
                this.rawResponse = null;

                const inputs = this.buildInputs();

                rpc({
                    requests: [{ Method: this.method, Inputs: inputs }],
                    silent: this.silent,
                    onDone: (r) => {
                        this.rawResponse = r;
                        // common pattern in this repo: R0R extracts first Result
                        this.output = R0R(r);
                        this.loading = false;
                        this.$emit('done', { inputs, response: r, output: this.output });
                    },
                    onFail: (e) => {
                        this.loading = false;
                        this.$emit('fail', e);
                    }
                });
            }
        }
    };
</script>


