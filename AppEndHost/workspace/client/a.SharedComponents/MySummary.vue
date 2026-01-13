<template>
    <div>
        <div class="text-center mb-1 p-4">
            <img :src="shared.getImageURI(shared.getLogedInUserContext()['Picture_FileBody_xs'])" v-if="shared.fixNull(shared.getLogedInUserContext()['Picture_FileBody_xs'],'')!==''"
                 style="width:100%" class="border border-2 rounded rounded-4 shadow shadow-sm" />
            <img src="/a..lib/images/avatar.png" style="width:75%" class="border border-2 rounded rounded-circle shadow shadow-sm" v-else />
        </div>
        <div class="text-center">
            <div class="btn btn-sm btn-link text-secondary text-hover-primary p-0 text-decoration-none" title="Refresh Session" @click="refreshSession">
                <i class="fa-solid fa-fw fa-refresh"></i>
                <span class="fw-bold">{{shared.getUserObject()["UserName"]}}</span>
            </div>
        </div>
    </div>
</template>
<script>
    let _this = { cid: "", c: null };

    export default {
        methods: {
            refreshSession() {
                showWorking(shared.heavyWorkingCover);
                refereshSession();
                setTimeout(function () { refereshPage(); }, 100);
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