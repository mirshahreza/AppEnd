<template>
    <div class="text-center p-1 p-md-3 pb-0">
        <img :src="shared.getImageURI(shared.getLogedInUserContext()['Picture_FileBody_xs'])" v-if="shared.fixNull(shared.getLogedInUserContext()['Picture_FileBody_xs'],'')!==''"
             style="width:100%" class="border border-2 rounded rounded-4 shadow shadow-sm" />
        <img src="/a..lib/images/avatar.png" style="width:75%" class="border border-2 rounded rounded-circle shadow shadow-sm" v-else />
        <div class="text-center mt-2">
            <span class="fw-bold">{{shared.getUserObject()["UserName"]}}</span>
        </div>
    </div>
</template>
<script>
    let _this = { cid: "", c: null };

    export default {
        methods: {
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