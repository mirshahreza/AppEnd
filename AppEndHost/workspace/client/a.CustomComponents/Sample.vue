<template>
<div class="container-fluid p-3" data-did="d-221025-0" draggable="true">
    <div class="row" data-did="d-375956-1" draggable="true">
        <div class="col" data-did="d-984885-2" draggable="true">Col 1</div>
        <div class="col" data-did="d-536858-3" draggable="true">Col 2</div>
    </div>
    <div class="form-check" data-did="d-256448-0" draggable="true">
        <input class="form-check-input" type="checkbox" data-did="d-303042-1" draggable="true">
            <label class="form-check-label" data-did="d-166573-2" draggable="true">Checkbox</label>
        </div>
    </div>
</template>

<script>
    let _this = { cid: "", c: null, inputs: {}, d: {}, regulator: null };
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