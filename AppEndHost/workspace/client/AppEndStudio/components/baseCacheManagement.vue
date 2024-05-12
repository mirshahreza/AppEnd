<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-header p-2 bg-success-subtle rounded-0 border-0">
            <div class="hstack gap-1">
                <input type="text" class="form-control form-control-sm" style="max-width:175px;" @keyup.enter="readList" v-model='keysFilter' />
                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="readList">
                    <i class="fa-solid fa-search"></i>
                </button>
                <div class="p-0 ms-auto"></div>
                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="removeAllCacheItems">
                    <i class="fa-solid fa-eraser"></i> <span>Remove All Items</span>
                </button>
            </div>
        </div>
        <div class="card-body p-2">
            <div class="card h-100 border-light bg-light bg-opacity-75 border-0">
                <div class="card-body rounded rounded-2 border border-3 border-light fs-d9">
                    <div v-for="k in cacheState.CachedKeys" class="badge text-center text-decoration-none p-2 border border-1 me-1 text-secondary text-hover-primary position-relative text-nowrap" :data-ae-key="k">
                        <span @click="showValue" class="pointer">{{k}}</span>
                        <i class="fa-solid fa-times text-secondary text-hover-danger ms-2 pointer" @click="removeKey"></i>
                    </div>
                </div>
            </div>
        </div>
        <div class="card-footer bg-light-subtle rounded-0 p-0">
            <div class="hstack gap-1">
                <div class="mx-1 fs-d9 p-1">EntryCount : <span class="fw-bold">{{cacheState.CurrentEntryCount}}</span></div>
                <div class="mx-1 fs-d9 p-1">EstimatedSize : <span class="fw-bold">{{cacheState.CurrentEstimatedSize}}</span></div>
                <div class="mx-1 fs-d9 p-1">TotalHits : <span class="fw-bold">{{cacheState.TotalHits}}</span></div>
                <div class="mx-1 fs-d9 p-1">TotalMisses : <span class="fw-bold">{{cacheState.TotalMisses}}</span></div>
                <div class="p-0 ms-auto"></div>
            </div>
        </div>
    </div>
</template>

<script>
    shared.setAppTitle("$auto$");
    let _this = { cid: "", c: null, cacheState: {}, keysFilter: "" };
    

    export default {
        methods: {
            removeAllCacheItems() {
                rpcAEP("RemoveAllCacheItems", { }, function (res) {
                    _this.c.readList();
                });
            },
            showValue(event) {
                let k = $(event.target).parent().attr("data-ae-key");
                rpcAEP("GetCacheItem", { Key: k }, function (res) {
                    showJson(res[0]['Result']);
                });
            },
            removeKey(event) {
                let k = $(event.target).parent().attr("data-ae-key");
                rpcAEP("RemoveCacheItem", { Key: k }, function (res) {
                    _this.c.readList();
                });
            },
            readList() {
                rpcAEP("GetCacheItems", { LikeStr: _this.c.keysFilter }, function (res) {
                    _this.c.cacheState = res[0]['Result'];
                });
            }
        },
        setup(props) { _this.cid = props['cid']; },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { _this.c.readList(); },
        props: { cid: String }
    }


</script>
