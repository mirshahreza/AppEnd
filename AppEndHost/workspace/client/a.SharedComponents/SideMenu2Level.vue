<template>
    <div class="d-flex h-100">
        <!-- Level 1: Icon Bar -->
        <div class="p-3 position-relative" style="width: 85px;">
            <ul class="list-unstyled">
                <li v-for="nItem in local.navItems" class="mb-2">
                    <div @click.prevent="selectMenu(nItem)"
                       class="w-100 btn btn-light modern-icon-btn d-flex justify-content-center align-items-center"
                       style="padding:11px 5px 11px 5px; aspect-ratio:1 / 1;"
                       :class="isL1Active(nItem) ? 'modern-selected-el' : 'text-secondary'">
                        <i :class="nItem.icon"></i>
                    </div>
                </li>
            </ul>
        </div>

        <!-- Level 2: Expanded Menu -->
        <div class="modern-menu-card shadow h-100 border-0 fs-d9" style="width: 220px;min-width:220px;" v-if="local.selectedMenu && local.selectedMenu.items">
            <div class="card-body px-3">
                <div class="my-3 text-secondary text-uppercase fw-bold fs-d9">{{ shared.translate(local.selectedMenu.title) }}</div>
                <ul class="list-unstyled ps-0">
                    <li v-for="link in local.selectedMenu.items" class="mb-1">
                        <a class="d-flex text-decoration-none fs-d9 modern-menu-link" style="padding:10px 10px;" v-if="shared.fixNull(link.title, '') !== '---'"
                           :class="isL2Active(link) ? 'modern-selected-el' : 'text-secondary-emphasis modern-hover-bg-light'" :href="'?c=' + link.component + shared.fixNull(link.params, '')"
                           draggable="true" v-on:dragstart="onDragStart" :data-ae-title="link.title">
                            <i :class="link.icon + ' fa-fw me-2'"></i>
                            <span>{{ shared.translate(link.title) }}</span>
                        </a>
                        <hr class="my-2 border-secondary-subtle" v-else />
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
        data() {
            return _this;
        },
        mounted() {
            this.loadNavigation();
        },
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
            },
            loadNavigation() {
                try {
                    // Load nav and remove any "AI Chat" entries
                    this.local.navItems = this.shared.getAppNav() || [];
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
                } catch (error) {
                    console.warn('Error loading navigation:', error);
                    // Retry after a short delay in case user object wasn't ready
                    const retryCount = this._navRetryCount || 0;
                    if (retryCount < 3) {
                        this._navRetryCount = retryCount + 1;
                        setTimeout(() => this.loadNavigation(), 200);
                    } else {
                        this.local.navItems = [];
                    }
                }
            }
        }
    }
</script>


