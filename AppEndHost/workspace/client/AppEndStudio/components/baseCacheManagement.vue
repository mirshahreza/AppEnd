<template>
    <div class="card h-100 rounded rounded-2 rounded-bottom-0 rounded-end-0 bg-transparent border-0">
        <div class="card-header p-2 bg-light-subtle rounded-end-0 border-0">
            <div class="input-group input-group-sm border-0 bg-transparent">
                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="removeAllCacheItems"><i class="fa-solid fa-eraser"></i> Remove All Items</button>
                <span class="input-group-text border-0 bg-transparent fs-d4"> </span>
                <input type="text" class="form-control" style="max-width:175px;" @keyup.enter="readList" v-model='keysFilter' />
                <span class="input-group-text border-0 bg-transparent fs-d4"> </span>
                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="readList"><i class="fa-solid fa-search"></i></button>
                <input type="text" class="form-control form-control-sm border-0 rounded-0 bg-transparent" disabled />
                <div class="input-group-text">
                    <div class="mx-1">EntryCount : <span class="fw-bold">{{cacheState.CurrentEntryCount}}</span></div>
                    <div class="mx-1">EstimatedSize : <span class="fw-bold">{{cacheState.CurrentEstimatedSize}}</span></div>
                    <div class="mx-1">TotalHits : <span class="fw-bold">{{cacheState.TotalHits}}</span></div>
                    <div class="mx-1">TotalMisses : <span class="fw-bold">{{cacheState.TotalMisses}}</span></div>
                </div>
            </div>
        </div>
        <div class="card-body p-2">
            <div class="card h-100 border-light bg-light bg-opacity-75 border-0">
                <div class="card-body rounded rounded-2 border border-3 border-light fs-d9 p-0">
                    <div v-for="k in cacheState.CachedKeys" class="badge text-bg-secondary m-1 pointer p-2" :data-ae-key="k">
                        <span @click="showValue">{{k}}</span>
                        <i class="fa-solid fa-times text-light text-hover-danger ms-1" @click="removeKey"></i>
                    </div>
                </div>
            </div>

        </div>
    </div>
</template>

<script>
    shared.setAppTitle("Cache Management");
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
