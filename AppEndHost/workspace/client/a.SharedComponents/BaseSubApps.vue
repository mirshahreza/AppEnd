<template>
    <div class="card shadow-sm mb-2">
        <div class="card-body">
            <div v-for="app in apps">
                <a :href="'/'+app.Name+'/'" target="_blank" class="btn btn-sm btn-outline-primary rounded-3 w-100 my-2 text-decoration-none pointer text-start">
                    <i class="fa-solid fa-fw fa-play"></i>

                    <span>Folder : <span class="fw-bold">{{app.Name}}</span></span>

                    <span class="">
                        [ {{app.Value.lang}} / {{app.Value.dir}} / {{app.Value.calendar}} ]
                    </span>

                    <span class="d-block fw-normal ps-3">
                        {{app.Value.title}} {{app.Value['sub-title']}}
                    </span>
                </a>
            </div>
        </div>
    </div>
</template>
<script>
    let _this = { cid: "", c: null, apps: [] };
    export default {
        methods: {
            readList() {
                rpcAEP("GetSubApps", {}, function (res) {
                    let r = R0R(res);
                    _.each(r, function (i) {
                        i.Value = JSON.parse(i.Value);
                    });
                    _this.c.apps = r;
                });
            }
        },
        setup(props) {
            _this.cid = props['cid'];
        },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { _this.c.readList(); },
        props: { cid: String },
    }
</script>
