<template>
    <div class="card bg-transparent rounded-0 border-0">
        <div class="card-body p-1">
            <div class="card border-light bg-transparent bg-opacity-75 border-0">
                <div class="card-body rounded rounded-1 border border-3 border-light fs-d9 p-3 bg-transparent scrollable">
                    <div class="container-fluid">
                        <div class="row mt-1">
                            <div class="col-48">
                                <div class="text-center mb-2">
                                    <img :src="shared.getImageURI(shared.getLogedInUserContext()['Picture_FileBody'])" style="width:75%" class="border border-2 rounded rounded-circle shadow shadow-sm" v-if="shared.fixNull(shared.getLogedInUserContext()['Picture_FileBody'],'')!==''" />
                                    <img src="/..lib/images/avatar.png" style="width:75%" class="border border-2 rounded rounded-circle shadow shadow-sm" v-else />
                                </div>
                                <div class="text-center">
                                    <div class="btn btn-sm btn-link text-secondary text-hover-primary p-0 text-decoration-none" @click="refreshSession">
                                        <i class="fa-solid fa-fw fa-refresh"></i>
                                        <span class="fw-bold">{{shared.getUserObject()["UserName"]}}</span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        
    </div>
</template>

<script>
    let _this = { cid: "", c: null };

    export default {
        methods: {
            refreshSession() {
                refereshSession();
                setTimeout(function () { refereshPage(); }, 200);
            },
            loadPermissions() {
                _this.c.alloweds = makeDotsToTree(shared.getUserAlloweds());
            }
        },
        setup(props) { _this.cid = props['cid']; },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { _this.c.loadPermissions(); },
        props: { cid: String }
    }

</script>