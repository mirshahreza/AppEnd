<template>
    <div class="code-editor-container h-100">
        <div class="h-100" id="jView"></div>
    </div>
</template>
<script>
    let _this = { cid: "", c: null, inputs: {}, editor: null };
    export default {
        methods: {
            load() {
                _this.editor = ace.edit("jView", {
                    theme: "ace/theme/cloud9_day",
                    mode: "ace/mode/json",
                    value: _.isObject(_this.c.inputs.jsonToView) ? JSON.stringify(_this.c.inputs.jsonToView, null, 4) : _this.c.inputs.jsonToView 
                });
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

