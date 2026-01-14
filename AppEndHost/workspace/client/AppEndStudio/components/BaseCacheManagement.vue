<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-header p-2 bg-body-subtle rounded-0 border-0">
            <div class="hstack">
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
        <div class="card-body scrollable">
            <div class="container-fluid">
                <div class="row">
                    <div class="col-6" v-for="k in cacheState.CachedKeys">
                        <div class="input-group" :data-ae-key="k">
                            <div class="form-control text-hover-underline text-hover-primary pointer" @click="showValue">{{k}}</div>
                            <div class="input-group-text pointer" @click="removeKey">
                                <i class="fa-solid fa-times text-secondary text-hover-danger"></i>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="card-footer bg-light-subtle rounded-0 p-0">
            <div class="hstack">
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
        mounted() { initVueComponent(_this); _this.c.readList(); },
        props: { cid: String }
    }


</script>
