<template>
    <div class="code-editor-container h-100">
        <div class="h-100" id="jView" style="text-align:left !important;direction:ltr !important;"></div>
    </div>
</template>
<script>
    let _this = { cid: "", c: null, inputs: {}, editor: null };
    export default {
        methods: {
            load() {
                let payload = _this.c.inputs.jsonToView;

                // If payload is a JSON string, try to parse it
                if (_.isString(payload)) {
                    try {
                        payload = JSON.parse(payload);
                    } catch (ex) {
                        // keep as string if not parseable
                    }
                }

                function normalizeForDisplay(obj) {
                    if (obj === null || obj === undefined) return obj;
                    if (_.isString(obj)) {
                        // if string contains escaped newlines or real newlines, convert to array of lines for readability
                        if (obj.indexOf('\\n') !== -1 || obj.indexOf('\n') !== -1) {
                            // split on escaped or real newlines, preserve empty lines
                            const parts = obj.split(/\\n|\r?\n/);
                            return parts;
                        }
                        return obj;
                    }
                    if (_.isArray(obj)) {
                        return _.map(obj, function (i) { return normalizeForDisplay(i); });
                    }
                    if (_.isObject(obj)) {
                        const res = {};
                        _.forEach(obj, function (v, k) {
                            res[k] = normalizeForDisplay(v);
                        });
                        return res;
                    }
                    return obj;
                }

                let displayValue;
                if (_.isObject(payload) || _.isArray(payload)) {
                    try {
                        const normalized = normalizeForDisplay(payload);
                        displayValue = JSON.stringify(normalized, null, 4);
                    } catch (ex) {
                        displayValue = JSON.stringify(payload, null, 4);
                    }
                } else {
                    displayValue = payload === undefined || payload === null ? '' : String(payload);
                }

                _this.editor = ace.edit("jView", {
                    theme: "ace/theme/cloud9_day",
                    mode: "ace/mode/json",
                    value: displayValue
                });

                // make editor read-only and nicely formatted
                _this.editor.setOptions({ readOnly: true, highlightActiveLine: false, highlightGutterLine: false });
                _this.editor.renderer.setShowGutter(true);
                _this.editor.clearSelection();
            }
        },
        setup(props) {
            _this.cid = props['cid'];
            _this.inputs = shared["params_" + _this.cid];
        },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { _this.c.load(); },
        props: { cid: String }
    }
</script>

