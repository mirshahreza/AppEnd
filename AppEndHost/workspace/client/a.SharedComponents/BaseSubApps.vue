<template>
    <div class="card shadow-sm mb-2">
        <div class="card-body">
            <div class="text-dark fs-d9 fw-bold px-2">
                {{shared.translate('Applications')}}
            </div>
            <hr class="my-1" />
            <div v-for="app in apps" v-if="apps.length>0">
                <a :href="'/'+app.Name+'/'" target="_blank" class="btn btn-sm btn-outline-primary rounded-3 border-0 bg-light-subtle w-100 my-2 text-decoration-none pointer text-start">
                    <i class="fa-solid fa-fw fa-play"></i>
                    <span class="">
                        {{app.Name}} :: {{app.Value.lang}} / {{app.Value.dir}} / {{app.Value.calendar}}
                    </span>
                    <span class="d-block fw-normal ps-3">
                        {{app.Value.title}} :: {{app.Value['sub-title']}}
                    </span>
                </a>
            </div>
            <div v-else>{{shared.translate('ThereIsNotOtherApplication')}}</div>
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
                    let apps = [];
                    _.each(r, function (i) {
                        i.Value = JSON.parse(i.Value);
                        if (i.Name !== shared.getThemeName()) apps.push(i);
                    });
                    _this.c.apps = apps;
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
