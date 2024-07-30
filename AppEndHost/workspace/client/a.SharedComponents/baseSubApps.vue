<template>
    <div class="container-fluid">
        <div class="row">
            <div class="col-48 col-md-12 mb-2" v-for="app in d">
                <div class="card shadow-sm">
                    <div class="card-body">
                        <div class="text-dark fs-d9">
                            <a :href="'/'+app.Name+'/'" target="_blank" class="form-control bg-hover-primary p-1 border-light fs-1d1 text-decoration-none pointer">
                                <i class="fa-solid fa-fw fa-play text-success"></i>
                                <span>Folder : <span class="fw-bold">{{app.Name}}</span></span>
                            </a>
                        </div>
                        <hr class="my-1" />
                        <div class="fw-bold text-success">
                            {{app.Value.title}} {{app.Value['sub-title']}}
                        </div>
                        <div>
                            {{app.Value.lang}} / {{app.Value.dir}} / {{app.Value.calendar}}
                        </div>
                        <div>
                            <a class="text-primary text-hover-success text-decoration-none pointer" :href="'?c=components/uiThemesTranslationManagement&app='+app.Name"><i class="fa-solid fa-fw fa-globe"></i> Translation</a>
                            <span class="mx-2">|</span>
                            <a class="text-primary text-hover-success text-decoration-none pointer" :href="'?c=components/uiThemesNavigationManagement&app='+app.Name"><i class="fa-solid fa-fw fa-bars"></i>Navigation</a>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    let _this = { cid: "", c: null, d: [] };
    export default {
        methods: {
            readList() {
                rpcAEP("GetThemes", {}, function (res) {
                    let r = R0R(res);
                    _.each(r, function (i) {
                        i.Value = JSON.parse(i.Value);
                    });
                    _this.c.d = r;
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
