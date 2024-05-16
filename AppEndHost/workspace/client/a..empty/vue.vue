<template>
    <div class="card border-0 shadow-lg bg-transparent rounded-0">
        <div class="card-body p-3 pb-4 bg-transparent fs-d8">

            Empty Component

        </div>
    </div>
</template>

<script>
    let _this = { cid: "", c: null, inputs: {}, d: { }, regulator: null };
    export default {
        methods: {
        },
        setup(props) {
            _this.cid = props['cid'];
            _this.["inputs"] = shared["params_" + _this.cid];
        },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { initVueComponent(_this); },
        props: { cid: String }
    }
</script>
