<template>
    <div class="card border-0 shadow-lg bg-transparent rounded-0">
        <div class="card-body p-3 pb-4 bg-transparent fs-d8">
            <ul class="list-group fs-d9 sortable-columns">
                <li class="list-group-item p-1" v-for="c in columnsToSort">
                    {{c.Name}}
                </li>
            </ul>
        </div>

        <div class="card-footer p-3 bg-secondary-subtle bg-gradient border-0 rounded-0">
            <div class="row">
                <div class="col-24">
                    <button class="btn btn-sm btn-secondary w-100 py-2" @click="cancel" data-ae-key="ok">
                        <i class="fa-solid fa-cancel"></i>
                        &nbsp;
                        <span>Cancel</span>
                    </button>
                </div>
                <div class="col-24">
                    <button class="btn btn-sm btn-primary w-100 py-2" @click="ok" data-ae-key="ok">
                        <i class="fa-solid fa-check"></i>
                        &nbsp;
                        <span>Ok</span>
                    </button>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    let _this = { cid: "", c: null, inputs: {}, origColumns: [], columnsToSort: [] };
    export default {
        methods: {
            sortable() {
                $(".sortable-columns").sortable();
            },
            ok(e) {
                let sortedItems = [];
                sortedItems.push(_.filter(_this.c.origColumns, function (i) { return i.IsPrimaryKey === true; })[0]);
                $(".sortable-columns .list-group-item").each(function () {
                    let colName = $(this).text().trim();
                    sortedItems.push(_.filter(_this.c.origColumns, function (i) { return i.Name === colName; })[0]);
                });
                if (_this.inputs.callback) _this.inputs.callback(sortedItems);
                _this.c.close();
            },
            cancel() { _this.c.close(); },
            close() { shared.closeComponent(_this.cid); }
        },
        setup(props) {
            _this.cid = props['cid'];
            _this.inputs = shared["params_" + _this.cid];
            _this.origColumns = _.cloneDeep(_this.inputs["columns"]);
            _this.columnsToSort = _.filter(_.cloneDeep(_this.inputs["columns"]), function (i) { return i.IsPrimaryKey !== true; });
        },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { _this.c.sortable(); },
        props: { cid: String }
    }

</script>