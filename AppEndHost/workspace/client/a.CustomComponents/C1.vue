<template>
<div class="card" data-did="d-834518-0" draggable="true">
    <div class="card-header" data-did="d-904331-1" draggable="true">A test component</div>
    <div class="card-body" data-did="d-784120-2" draggable="true">
        <p data-did="d-341619-0" draggable="true">
            this is a test component just to check that custom components are working
        </p>
    </div>
</div>
</template>

<script>
    let _this = { cid: "", c: null, inputs: {}, d: {} };
    export default {
        methods: {
        },
        setup(props) {
            _this.cid = props['cid'];
            _this.inputs = shared["params_" + _this.cid];
        },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { initVueComponent(_this); },
        props: { cid: String }
    }
</script>

<style scoped>
</style>