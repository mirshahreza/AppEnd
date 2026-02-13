<template>
    <div :id="cid" :ae-data-ready="dataReady" class="card border-0 shadow-sm rounded-0 h-100 ae-rpc-fn-caller">
        <div class="card-body bg-transparent fs-d8 p-0 h-100">
            <div class="d-flex flex-column h-100">

                <div v-if="headerPosition === 'top'" class="card-header px-2 bg-warning-subtle ae-rpc-fn-toolbar">
                    <div class="hstack">
                        <span class="fw-bold">{{ titleComputed }}</span>
                        <div class="p-0 ms-auto"></div>
                        <button type="button" class="btn btn-sm btn-primary" @click="call" :disabled="loading">{{ submitText }}</button>
                    </div>
                </div>

                <div class="card-body p-2 scrollable" style="flex:1 1 auto; min-height:0;">
                    <div v-if="parseError" class="alert alert-danger py-2 mb-0">
                        {{ parseError }}
                    </div>

                    <div v-else>
                        <div class="mb-2 text-secondary fs-d8" style="direction:ltr;text-align:left;white-space:nowrap;overflow:hidden;text-overflow:ellipsis;">
                            {{ signature }}
                        </div>

                        <div v-if="meta.params.length === 0" class="text-secondary fs-d8 mb-2">No input parameters</div>

                        <div v-for="p in meta.params" :key="p.name" class="mb-2">
                            <label class="form-label mb-1">{{ p.name }} <span class="text-secondary fs-d8">({{ p.type }})</span></label>
                            <input v-if="isSimple(p.type)" type="text" class="form-control form-control-sm" v-model="inputs[p.name]" />
                            <textarea v-else class="form-control form-control-sm" rows="4" style="direction:ltr;text-align:left;" v-model="inputsJson[p.name]"></textarea>
                        </div>

                        <div v-if="showOutputs" class="mt-3">
                            <div class="fw-bold mb-1">Output</div>
                            <pre class="p-2 bg-light border" style="direction:ltr;text-align:left;max-height:340px;overflow:auto;white-space:pre-wrap;word-break:break-word;">{{ outputText }}</pre>
                        </div>
                    </div>
                </div>

                <div v-if="headerPosition === 'bottom'" class="card-header px-2 bg-warning-subtle ae-rpc-fn-toolbar mt-auto">
                    <div class="hstack">
                        <span class="fw-bold">{{ titleComputed }}</span>
                        <div class="p-0 ms-auto"></div>
                        <button type="button" class="btn btn-sm btn-primary" @click="call" :disabled="loading">{{ submitText }}</button>
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
    methods: {
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


