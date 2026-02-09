<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <!-- Filter -->
        <div class="card-header p-2 bg-body-subtle rounded-0 border-0">
            <div class="hstack gap-1">
                <input type="text" class="form-control form-control-sm" style="max-width:250px;" @keyup.enter="readList" v-model="keysFilter" placeholder="Search keys..." />
                <div class="btn btn-sm btn-outline-primary px-3 border-0" @click="readList">
                    <i class="fa-solid fa-search me-1"></i>
                    <span class="d-none d-md-inline">Search</span>
                </div>
                <div class="ms-auto"></div>
                <button class="btn btn-sm btn-outline-danger px-3 border-0" @click="removeAllCacheItems">
                    <i class="fa-solid fa-eraser me-1"></i> <span class="d-none d-md-inline">Remove All Items</span>
                </button>
            </div>
        </div>
        <!-- BODY -->
        <div class="card-body p-0">
            <div class="card h-100 border-light bg-light bg-opacity-75 border-0">
                <div class="card-body rounded-1 border border-1 border-light fs-d8 p-0 bg-transparent scrollable">
                    <table class="table table-sm table-hover w-100 ae-table m-0 bg-transparent">
                        <thead class="small bg-light">
                            <tr class="align-middle">
                                <th class="sticky-top ae-thead-th text-center" style="width:60px;">#</th>
                                <th class="sticky-top ae-thead-th">Key</th>
                                <th class="sticky-top ae-thead-th text-center" style="width:50px;"><i class="fa-solid fa-eye"></i></th>
                                <th class="sticky-top ae-thead-th text-center" style="width:50px;"><i class="fa-solid fa-trash"></i></th>
                            </tr>
                        </thead>
                        <tbody v-if="pagedKeys.length > 0">
                            <tr v-for="(k, index) in pagedKeys" :key="k" class="align-middle">
                                <td class="text-center text-muted">{{ (pageNumber - 1) * pageSize + index + 1 }}</td>
                                <td class="font-monospace text-truncate" :title="k">{{ k }}</td>
                                <td class="text-center pointer text-primary" @click="showValue(k)">
                                    <i class="fa-solid fa-eye"></i>
                                </td>
                                <td class="text-center pointer text-danger" @click="removeKey(k)">
                                    <i class="fa-solid fa-trash"></i>
                                </td>
                            </tr>
                        </tbody>
                        <tbody v-else>
                            <tr>
                                <td colspan="4" class="text-center py-5 text-muted small">No cached items found</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
        <!-- Footer -->
        <div class="card-footer ae-list-footer">
            <div class="d-flex flex-wrap align-items-center justify-content-center justify-content-md-between gap-3">
                <!-- Page Size -->
                <div class="d-none d-md-flex align-items-center gap-3 flex-wrap">
                    <div class="d-flex align-items-center gap-2">
                        <span class="text-secondary small">PageSize</span>
                        <select class="form-select form-select-sm border-0 bg-transparent text-secondary fw-medium" style="width: 70px;" v-model.number="pageSize" @change="pageNumber = 1">
                            <option value="25">25</option>
                            <option value="50">50</option>
                            <option value="100">100</option>
                            <option value="200">200</option>
                        </select>
                    </div>
                </div>
                <!-- Pagination -->
                <div class="d-flex align-items-center gap-1">
                    <button class="btn btn-sm btn-outline-secondary border-0" :disabled="pageNumber <= 1" @click="pageNumber--">
                        <i class="fa-solid fa-chevron-left"></i>
                    </button>
                    <span class="small text-secondary mx-2">{{ pageNumber }} / {{ totalPages }}</span>
                    <button class="btn btn-sm btn-outline-secondary border-0" :disabled="pageNumber >= totalPages" @click="pageNumber++">
                        <i class="fa-solid fa-chevron-right"></i>
                    </button>
                </div>
                <!-- Stats -->
                <div class="d-none d-md-flex align-items-center gap-3 text-secondary small">
                    <div>EntryCount: <span class="fw-bold text-primary">{{ cacheState.CurrentEntryCount }}</span></div>
                    <div class="vr opacity-25"></div>
                    <div>EstimatedSize: <span class="fw-bold text-primary">{{ cacheState.CurrentEstimatedSize }}</span></div>
                    <div class="vr opacity-25"></div>
                    <div>TotalHits: <span class="fw-bold text-primary">{{ cacheState.TotalHits }}</span></div>
                    <div class="vr opacity-25"></div>
                    <div>TotalMisses: <span class="fw-bold text-primary">{{ cacheState.TotalMisses }}</span></div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    shared.setAppTitle("$auto$");
    let _this = { cid: "", c: null, cacheState: { CachedKeys: [], CurrentEntryCount: 0, CurrentEstimatedSize: 0, TotalHits: 0, TotalMisses: 0 }, keysFilter: "", pageNumber: 1, pageSize: 50 };

    export default {
        methods: {
            removeAllCacheItems() {
                rpcAEP("RemoveAllCacheItems", {}, function (res) {
                    _this.c.readList();
                });
            },
            showValue(k) {
                rpcAEP("GetCacheItem", { Key: k }, function (res) {
                    showJson(res[0]['Result']);
                });
            },
            removeKey(k) {
                rpcAEP("RemoveCacheItem", { Key: k }, function (res) {
                    _this.c.readList();
                });
            },
            readList() {
                _this.c.pageNumber = 1;
                rpcAEP("GetCacheItems", { LikeStr: _this.c.keysFilter }, function (res) {
                    _this.c.cacheState = res[0]['Result'];
                });
            }
        },
        computed: {
            pagedKeys() {
                if (!this.cacheState.CachedKeys) return [];
                let start = (this.pageNumber - 1) * this.pageSize;
                return this.cacheState.CachedKeys.slice(start, start + this.pageSize);
            },
            totalPages() {
                if (!this.cacheState.CachedKeys || this.cacheState.CachedKeys.length === 0) return 1;
                return Math.ceil(this.cacheState.CachedKeys.length / this.pageSize);
            }
        },
        setup(props) { _this.cid = props['cid']; },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { initVueComponent(_this); _this.c.readList(); },
        props: { cid: String }
    }
</script>
