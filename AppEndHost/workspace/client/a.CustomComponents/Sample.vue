<template>
<div class="container-fluid p-3" data-did="d-221025-0" draggable="true">
    <div class="row" data-did="d-375956-1" draggable="true">
        <div class="col p-2" data-did="d-888677-0" draggable="true">Column
            <div class="card" data-did="d-556826-0" draggable="true">
                <div class="card-header" data-did="d-106436-1" draggable="true">Header</div>
                <div class="card-body" data-did="d-945067-2" draggable="true">
                </div>
            </div>
        </div>
        <div class="col" data-did="d-984885-2" draggable="true">Col 1
            <p class="card-text" data-did="d-805662-4" draggable="true">Content</p>
        </div>
        <div class="col" data-did="d-536858-3" draggable="true">Col 2</div>
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