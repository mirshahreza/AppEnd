<template>
    <div class="card h-100 border-0 rounded-0 bg-transparent">

        <div class="card-body p-2 pt-3 d-none d-md-block d-lg-block bg-transparent">
            <div class="fs-d8 p-1 ps-0">
                <a href="?c=components/baseHome" class="text-decoration-none">
                    <i class="fa-solid fa-fw fa-home"></i> <span>{{shared.translate('Home')}}</span>
                </a>
            </div>

            <div v-for="nItem in shared.getAppNav()" class="mb-3">
                <div class="fw-bolder text-dark fs-d7"><i :class="nItem['icon']"></i> {{shared.translate(nItem["title"])}}</div>
                <div class="ps-2">
                    <div class="list-group rounded-0 fs-d8 bg-transparent">
                        <div class="list-group-item bg-hover bg-transparent hover-slow border-0 rounded rounded-2 p-1 ps-2" v-for="link in nItem.items">
                            <a draggable="true" v-on:dragstart="onDragStart" :data-ae-title="link.title"
                               :href="'?c='+link.component+shared.fixNull(link.params,'')"
                               class="bg-transparent p-0 border-0 text-primary-emphasis text-hover-primary text-decoration-none" v-if="shared.fixNull(link.title,'')!=='---'">
                                <i :class="link.icon"></i> {{shared.translate(link.title)}}
                            </a>
                            <div v-else>
                                <hr class="my-0 me-2" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="card-body p-2 pt-3 d-sm-block d-md-none d-lg-none bg-transparent">
            <div class="bg-transparent text-center">
                <a href="?c=components/baseHome" class="text-decoration-none">
                    <i class="fa-solid fa-fw fa-home"></i>
                </a>
            </div>
            <div v-for="nItem in shared.getAppNav()" class="mb-3 text-center">
                <div class="fs-d7">&nbsp;</div>
                <div class="mb-2" v-for="link in nItem.items">
                    <a :href="'?c='+link.component+shared.fixNull(link.params,'')" class="text-decoration-none" v-if="shared.fixNull(link.title,'')!=='---'">
                        <i class="text-secondary fs-1d2" :class="link.icon"></i>
                    </a>
                    <div v-else>
                        <hr class="my-0" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>


<script>
    let _this = { cid: "", c: null, inputs: {}, local: {} };
    export default {
        methods: {
            onDragStart(event) {
                let data = { href: event.target.href.split('?')[1], title: $(event.target).attr("data-ae-title"), icon: $(event.target).find("i:first").attr("class") };
                event.dataTransfer.setData("menu-item", JSON.stringify(data));
            }
        },
        data() { return _this; }
    }
</script>


