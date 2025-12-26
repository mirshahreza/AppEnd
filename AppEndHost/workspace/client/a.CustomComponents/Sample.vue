<template>
<div class="card h-100" data-did="d-834518-0" draggable="true">
    <div class="card-header" data-did="d-904331-1" draggable="true">Header</div>
    <div class="card-body" data-did="d-784120-2" draggable="true">
        <h5 class="card-title" data-did="d-223788-3" draggable="true">Title</h5>
        <p class="card-text" data-did="d-180738-4" draggable="true">Content</p>
        <p data-did="d-341619-0" draggable="true">
            Paragraph text
        </p>
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