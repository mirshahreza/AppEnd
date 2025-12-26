<template>
<div class="card h-100" data-did="d-834518-0" draggable="true">
    <div class="card-header" data-did="d-904331-1" draggable="true">Header<div class="container-fluid p-3" data-did="d-653118-0" draggable="true">
        <div class="row" data-did="d-587429-1" draggable="true">
            <div class="col" data-did="d-947736-2" draggable="true">
                <span data-did="d-893772-3" draggable="true">Col 1</span>
            </div>
            <div class="col p-2" data-did="d-8670-0" draggable="true">
                <span data-did="d-838216-1" draggable="true">Column</span>
            </div>
            <div class="col p-2" data-did="d-381585-0" draggable="true">
                <span data-did="d-123785-1" draggable="true">Column</span>
            </div>
            <div class="col p-2" data-did="d-256728-0" draggable="true">
                <span data-did="d-600208-1" draggable="true">Column</span>
            </div>
            <div class="col p-2" data-did="d-955095-0" draggable="true">
                <span data-did="d-613566-1" draggable="true">Column</span>
            </div>
            <div class="col p-2" data-did="d-397946-0" draggable="true">
                <span data-did="d-125111-1" draggable="true">Column</span>
            </div>
            <div class="col" data-did="d-165383-4" draggable="true">
                <span data-did="d-361286-5" draggable="true">Col 2</span>
            </div>
        </div>
    </div>
</div>
<div class="card-body" data-did="d-784120-2" draggable="true">
    <h5 class="card-title" data-did="d-223788-3" draggable="true">Title<a href="#" data-did="d-977677-0" draggable="true" style="">Link</a>
</h5>
<component-loader src="/a.CustomComponents/C1.vue" data-did="d-655051-0" draggable="true" style="">
</component-loader>
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