<template>
    <div class="card h-100 border-0 rounded-0 bg-transparent">

        <div class="card-body p-2 bg-transparent scrollable">
            <div v-for="nItem in shared.getAppNav()" class="mb-3">
                <div class="fw-bolder text-dark fs-d7"><i :class="nItem['icon']"></i> {{shared.translate(nItem["title"])}}</div>
                <div class="ps-1">
                    <div class="list-group rounded-0 fs-d8 bg-transparent">
                        <div class="list-group-item text-nowrap bg-hover bg-transparent hover-slow border-0 rounded rounded-2 p-1 ps-2" v-for="link in nItem.items">
                            <a class="bg-transparent p-0 border-0 text-decoration-none" :class="(shared.fixNull(shared.getQueryString('c'),'')!=='' && shared.getQueryString('c')===link.component) ? 'text-success' : 'text-primary-emphasis text-hover-primary'" 
                               draggable="true" v-on:dragstart="onDragStart" :data-ae-title="link.title"
                               :href="'?c='+link.component+shared.fixNull(link.params,'')" v-if="shared.fixNull(link.title,'')!=='---'">
                                <i :class="link.icon+' fa-fw'"></i> <span class="fw-bold fs-d9">{{shared.translate(link.title)}}</span>
                            </a>
                            <div v-else>
                                <hr class="my-0 me-2 border-2 border-secondary-subtle" />
                            </div>
                        </div>
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


