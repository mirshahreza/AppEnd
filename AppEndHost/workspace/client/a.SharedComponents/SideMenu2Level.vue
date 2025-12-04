<template>
    <div class="d-flex h-100">
        <!-- Level 1: Icon Bar -->
        <div class="p-2 pt-3 position-relative" style="width: 64px;">
            <ul class="list-unstyled">
                <li v-for="nItem in local.navItems" class="mb-2">
                    <div @click.prevent="selectMenu(nItem)"
                       class="w-100 btn btn-light shadow-sm ico-shadow d-flex justify-content-center align-items-center rounded-4"
                       style="padding:11px 5px 11px 5px; aspect-ratio:1 / 1;"
                       :class="isL1Active(nItem) ? 'selected-el' : 'text-secondary'">
                        <i :class="nItem.icon"></i>
                    </div>
                </li>
            </ul>
            <!-- Fixed AI button at the bottom (square) -->
            <div class="position-absolute bottom-0 start-0 end-0 p-2">
                <button class="w-100 btn btn-light shadow-sm ico-shadow d-flex justify-content-center align-items-center rounded-4"
                        title="AI Chat" @click="openAiChat" 
                        style="aspect-ratio:1 / 1;"
                        :class="isLBActive() ? 'selected-el' : 'text-secondary'">
                    <i class="fa-solid fa-robot fa-fw"></i>
                </button>
            </div>
        </div>

        <!-- Level 2: Expanded Menu -->
        <div class="card shadow-lg h-100 border-0 rounded-2 rounded-end-0 rounded-bottom-0 fs-d9" style="width: 220px;min-width:220px;" v-if="local.selectedMenu && local.selectedMenu.items">
            <div class="card-body">
                <div class="mb-3 text-secondary text-uppercase fw-bold fs-d9">{{ shared.translate(local.selectedMenu.title) }}</div>
                <ul class="list-unstyled ps-0">
                    <li v-for="link in local.selectedMenu.items" class="mb-1">
                        <a class="d-flex text-decoration-none fs-d9" style="padding:5px;" v-if="shared.fixNull(link.title, '') !== '---'"
                           :class="isL2Active(link) ? 'selected-el' : 'text-secondary-emphasis hover-bg-light'" :href="'?c=' + link.component + shared.fixNull(link.params, '')"
                           draggable="true" v-on:dragstart="onDragStart" :data-ae-title="link.title">
                            <i :class="link.icon + ' fa-fw ms-2 me-1 mt-1'"></i>
                            <span>{{ shared.translate(link.title) }}</span>
                        </a>
                        <hr class="my-2" v-else />
                    </li>
                </ul>
            </div>
        </div>
    </div>
</template>

<script>
    let _this = {
        cid: "",
        c: null,
        inputs: {},
        local: {
            navItems: [],
            selectedMenu: null,
            activeComponent: ''
        }
    };

    export default {
        methods: {
            selectMenu(nItem) {
                // If the item has no sub-menu, navigate directly
                if (!nItem.items || nItem.items.length === 0) {
                    if (nItem.component) {
                        window.location.href = '?c=' + nItem.component + shared.fixNull(nItem.params, '');
                    }
                    this.local.selectedMenu = null;
                    return;
                }

                // If the same menu is clicked again, close it
                if (this.local.selectedMenu && this.local.selectedMenu.title === nItem.title) {
                    this.local.selectedMenu = null;
                } else {
                    this.local.selectedMenu = nItem;
                }
            },
            isLBActive() {
                const c = (this.shared.getQueryString('c') || '').toLowerCase();
                return c === '/a.sharedcomponents/chatai.vue';
            },
            isL1Active(nItem) {
                return this.local.selectedMenu && this.local.selectedMenu.title === nItem.title;
            },
            isL2Active(link) {
                return this.local.activeComponent === link.component;
            },

            onDragStart(event) {
                let data = { href: event.target.href.split('?')[1], title: $(event.target).attr("data-ae-title"), icon: $(event.target).find("i:first").attr("class") };
                event.dataTransfer.setData("menu-item", JSON.stringify(data));
            },
            openAiChat() {
                // Open AI Chat component directly, stays independent of menu
                window.location.href = '?c=/a.SharedComponents/ChatAi.vue';
            }
        },
        data() {
            return _this;
        },
        mounted() {
            // Load nav and remove any "AI Chat" entries
            this.local.navItems = this.shared.getAppNav();
            this.local.navItems = this.local.navItems
                .map(cat => ({
                    ...cat,
                    items: (cat.items || []).filter(l => (this.shared.fixNull(l.title, '').toLowerCase() !== 'ai chat' && this.shared.fixNull(l.component, '').toLowerCase() !== '/a.sharedcomponents/chatai.vue'))
                }))
                .filter(cat => (cat.items || []).length > 0 || !cat.items);

            this.local.activeComponent = this.shared.getQueryString('c');

            // Find which parent menu is active on load
            if (this.local.activeComponent) {
                for (const navItem of this.local.navItems) {
                    if (navItem.items && navItem.items.some(link => link.component === this.local.activeComponent)) {
                        this.local.selectedMenu = navItem;
                        break;
                    }
                }
            }
        }
    }
</script>


