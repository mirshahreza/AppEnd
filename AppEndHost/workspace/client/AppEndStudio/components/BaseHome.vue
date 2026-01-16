<template>
    <div class="card h-100 rounded-bottom-0 rounded-end-0 border-0">
        <div class="card-body p-0 h-100 position-relative">
            <div class="h-100 w-100 d-flex" data-flex-splitter-horizontal style="flex: auto; overflow-x: hidden;">
                <!-- Left side: Main content -->
                <div class="h-100" :style="getMainContentStyle()">
                    <div class="card h-100 rounded-0 border-0">
                        <div class="card-body p-2 p-md-5 scrollable">
                            <div class="container-fluid">
                                <div class="row">
                                    <div class="col-48 col-md-12 col-lg-10 col-xl-8 col-xxl-6">

                                        <div class="card text-center shadow-sm">
                                            <div class="card-body p-2">
                                                <component-loader src="/a.SharedComponents/MySummary" uid="mySummary" />
                                            </div>
                                        </div>

                                        <div class="card mt-2 font-monospace text-center fs-1d5 shadow-sm">
                                            <div class="card-body">
                                                <component-loader src="/a.SharedComponents/DigitalClock" uid="digitalClock" />
                                            </div>
                                        </div>

                                        <div class="mt-2 shadow-sm">
                                            <component-loader src="components/ServerActions" uid="serverActions" />
                                        </div>


                                    </div>
                                    <div class="col-48 mt-3 mt-md-0 col-lg-24">
                                        <component-loader src="/a.SharedComponents/MyShortcuts" uid="myShortcuts" />
                                        <component-loader src="/a.SharedComponents/BaseSubApps" uid="baseSubApps" />
                                        <div class="d-none d-md-block">
                                            <component-loader src="components/BaseServerSummary" uid="baseServerSummary" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Splitter with toggle button (only visible when chat is open) -->
                <template v-if="chatPanelVisible">
                    <div role="separator" tabindex="1" class="splitter-handle bg-light" 
                         style="width:7px; min-width:7px; flex-shrink: 0;"
                         @mousedown.stop
                         @click.stop="handleSplitterClick">
                        <div class="splitter-icon" @click.stop.prevent="toggleChatPanel" 
                             title="Hide AI Chat">
                            <i class="fa-solid fa-chevron-right"></i>
                        </div>
                    </div>

                    <!-- Right side: AI Chat -->
                    <div class="h-100 d-flex flex-column bg-white" style="min-width:250px;width:350px; flex-shrink: 0;">
                        <component-loader src="/a.SharedComponents/BaseAiChat" uid="aiChat" />
                    </div>
                </template>
            </div>

            <!-- Floating button when chat is hidden -->
            <button v-if="!chatPanelVisible" 
                    class="btn btn-primary position-fixed floating-chat-toggle" 
                    @click="toggleChatPanel"
                    title="Show AI Chat">
                <i class="fa-solid fa-robot"></i>
            </button>
        </div>
    </div>
</template>


<script>
    shared.setAppTitle(`<i class="fa-solid fa-fw fa-home"></i> <span>${shared.translate('Home')}</span>`);
    
    // Check if mobile/tablet (screen width < 768px)
    const isMobile = window.innerWidth < 768;
    let _this = { cid: "", c: null, chatPanelVisible: !isMobile };

    export default {
        methods: {
            getMainContentStyle() {
                const width = this.chatPanelVisible ? 'width:calc(100% - 357px);' : 'width:100%;';
                return 'min-width:400px;' + width + 'overflow:hidden; flex-shrink: 0;';
            },
            handleSplitterClick(e) {
                // Prevent click event from reaching splitter library
                e.preventDefault();
                e.stopPropagation();
            },
            toggleChatPanel() {
                this.chatPanelVisible = !this.chatPanelVisible;
                _this.chatPanelVisible = this.chatPanelVisible;
            }
        },
        setup(props) { _this.cid = props['cid']; },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { },
        props: { cid: String }
    }
</script>

<style scoped>
/* Splitter handle - 7px width, lighter color */
.splitter-handle {
    position: relative;
    cursor: col-resize;
    transition: background 0.2s;
    display: flex;
    align-items: center;
    justify-content: center;
    user-select: none;
    background: #e0e4ea !important;
}

.splitter-handle:hover {
    background: #d0d5dd !important;
}

.splitter-icon {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 20px;
    height: 40px;
    color: #495057;
    font-size: 12px;
    opacity: 0.7;
    transition: opacity 0.2s;
    cursor: pointer;
    border-radius: 4px;
}

.splitter-icon:hover {
    opacity: 1;
    background: rgba(13, 110, 253, 0.15);
}

.splitter-icon:active {
    background: rgba(13, 110, 253, 0.25);
}

/* Floating toggle button */
.floating-chat-toggle {
    top: 50%;
    right: 10px;
    transform: translateY(-50%);
    z-index: 1000;
    border-radius: 50%;
    width: 50px;
    height: 50px;
    box-shadow: 0 4px 12px rgba(0,0,0,0.3);
    display: flex;
    align-items: center;
    justify-content: center;
    padding: 0;
}

.floating-chat-toggle:hover {
    transform: translateY(-50%) scale(1.1);
    box-shadow: 0 6px 16px rgba(0,0,0,0.4);
}

.floating-chat-toggle i {
    font-size: 1.25rem;
}
</style>