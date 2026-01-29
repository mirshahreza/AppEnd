<template>
    <div class="card border-0 h-100">
        <div class="card-header py-3 fs-1d2 fw-bold text-primary" v-if="shared.fixNull(content.Title,'')!==''" v-html="content.Title">
        </div>
        <div class="card-header py-1 bg-success-subtle" v-if="shared.fixNull(content.CreatedOn,'')!==''">
            <div class="fw-bold text-danger fs-d8">
                {{shared.formatDate(content.CreatedOn)}}
            </div>
        </div>
        <div class="card-body p-4 pt-3 scrollable">
            <div v-html="content.ContentBody"></div>
        </div>
    </div>
</template>

<script>
    let _this = { cid: "", c: null, content: { Title: "", ContentBody: "" } };
    export default {
        methods: {
        },
        setup(props) {
            _this.cid = props['cid'];
            _this.inputs = shared["params_" + _this.cid];
            _this.content = fixNull(_this.inputs["content"], '');
        },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { },
        props: { cid: String }
    }
</script>