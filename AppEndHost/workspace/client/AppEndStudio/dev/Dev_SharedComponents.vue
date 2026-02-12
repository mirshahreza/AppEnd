<template>
    <div class="card h-100 bg-transparent rounded-0 border-0 dev-guide">
        <div class="card-header p-2 bg-body-subtle rounded-0 border-0">
            <div class="hstack gap-2">
                <div class="fw-bold text-primary">
                    <i class="fa-solid fa-puzzle-piece me-1"></i>
                    <span>Shared Components</span>
                </div>
                <div class="ms-auto text-muted fs-d8">Dev Guide</div>
            </div>
        </div>
        <div class="card-body p-2">
            <div class="container-fluid h-100">
                <div class="row h-100">
                    <div class="col-48 col-lg-36 order-2 order-lg-1 scrollable h-100">
                        <div class="dev-guide-content p-2" tabindex="0">

                            <section id="purpose" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-bullseye me-1 text-primary"></i>
                                    <span class="fw-bold">Purpose of this page</span>
                                </div>
                                <p class="mb-1">
                                    This page catalogs the reusable components under <span class="fw-bold">a.SharedComponents</span> and shows how to open and reuse them across Studio and sub-apps.
                                </p>
                                <ul class="mb-0">
                                    <li>Find ready-to-use UI building blocks.</li>
                                    <li>Open shared components consistently via <span class="fw-bold">openComponent</span>.</li>
                                    <li>Reuse authentication, dialogs, navigation, and utilities.</li>
                                </ul>
                            </section>

                            <section id="location" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-folder-open me-1 text-info"></i>
                                    <span class="fw-bold">Location & naming</span>
                                </div>
                                <p class="mb-1">All shared components live here:</p>
                                <pre class="mb-2">/workspace/client/a.SharedComponents</pre>
                                <p class="mb-0">They are referenced by path (e.g., <span class="fw-bold">/a.SharedComponents/BasePromptEx</span>).</p>
                            </section>

                            <section id="loader" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-boxes-stacked me-1 text-success"></i>
                                    <span class="fw-bold">Component loader basics</span>
                                </div>
                                <p class="mb-1">Use the loader to embed shared components:</p>
                                <pre class="mb-0">&lt;component-loader
  src="/a.SharedComponents/ThemePicker.vue"
  uid="themePicker" /&gt;</pre>
                            </section>

                            <section id="auth" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-shield-halved me-1 text-warning"></i>
                                    <span class="fw-bold">Authentication & account</span>
                                </div>
                                <ul class="mb-0">
                                    <li><span class="fw-bold">Login</span> / <span class="fw-bold">LoginModern</span> — login screens.</li>
                                    <li><span class="fw-bold">AuthChangePassword</span> — change password dialog.</li>
                                    <li><span class="fw-bold">AuthLoginAs</span> — admin-only login-as dialog.</li>
                                    <li><span class="fw-bold">MyProfile</span> — user profile editor.</li>
                                    <li><span class="fw-bold">MyTokenInfo</span> — JWT token inspection.</li>
                                </ul>
                            </section>

                            <section id="dialogs" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-comment-dots me-1 text-primary"></i>
                                    <span class="fw-bold">Dialogs & prompts</span>
                                </div>
                                <ul class="mb-0">
                                    <li><span class="fw-bold">BaseConfirm</span> — simple confirmation dialog.</li>
                                    <li><span class="fw-bold">BasePrompt</span> — basic input prompt.</li>
                                    <li><span class="fw-bold">BasePromptEx</span> — structured confirmation with reason/note.</li>
                                    <li><span class="fw-bold">BaseContent</span> — generic content modal.</li>
                                </ul>
                                <div class="fw-bold mb-1 mt-2">Open a shared dialog</div>
                                <pre class="mb-0">openComponent("/a.SharedComponents/BasePromptEx", {
  title: "Confirm Action",
  modalSize: "modal-md",
  params: { message1: "Are you sure?" }
});</pre>
                                <div class="dev-demo-panel">
                                    <div class="fw-bold mb-2"><i class="fa-solid fa-play me-1 text-success"></i> Live Demo</div>
                                    <div class="d-flex flex-wrap gap-2">
                                        <button class="btn btn-sm btn-primary" @click="demoPromptEx"><i class="fa-solid fa-comment-dots me-1"></i> PromptEx</button>
                                        <button class="btn btn-sm btn-outline-primary" @click="demoJsonView"><i class="fa-solid fa-code me-1"></i> JsonView</button>
                                    </div>
                                </div>
                            </section>

                            <section id="navigation" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-bars me-1 text-info"></i>
                                    <span class="fw-bold">Navigation & menus</span>
                                </div>
                                <ul class="mb-0">
                                    <li><span class="fw-bold">SideMenu</span>, <span class="fw-bold">SideMenu2Level</span>, <span class="fw-bold">SideMenuBranded</span> — menu renderers.</li>
                                    <li><span class="fw-bold">MyShortcuts</span> — user-defined shortcuts panel.</li>
                                    <li><span class="fw-bold">MySummary</span> — user summary widget.</li>
                                </ul>
                            </section>

                            <section id="system" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-gears me-1 text-secondary"></i>
                                    <span class="fw-bold">System tools</span>
                                </div>
                                <ul class="mb-0">
                                    <li><span class="fw-bold">BaseComponentLoader</span> — dynamic component loader.</li>
                                    <li><span class="fw-bold">BaseFileEditor</span> — file viewer/editor with syntax highlighting.</li>
                                    <li><span class="fw-bold">BaseJsonView</span> — JSON viewer.</li>
                                    <li><span class="fw-bold">SchedulerManagement</span> — manage scheduled tasks.</li>
                                    <li><span class="fw-bold">CronBuilder</span> — visual cron expression builder.</li>
                                    <li><span class="fw-bold">PropertyGrid</span> — editable property grid.</li>
                                </ul>
                            </section>

                            <section id="media" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-photo-film me-1 text-success"></i>
                                    <span class="fw-bold">Media & visual</span>
                                </div>
                                <ul class="mb-0">
                                    <li><span class="fw-bold">ImageEditor</span> — crop/resize/edit images.</li>
                                    <li><span class="fw-bold">DigitalClock</span> — live clock widget.</li>
                                </ul>
                            </section>

                            <section id="ai" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-robot me-1 text-warning"></i>
                                    <span class="fw-bold">AI & assistants</span>
                                </div>
                                <ul class="mb-0">
                                    <li><span class="fw-bold">BaseAiChat</span> — embedded AI chat UI.</li>
                                </ul>
                            </section>

                            <section id="bestpractices" class="dev-guide-section">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-circle-check me-1 text-success"></i>
                                    <span class="fw-bold">Best practices</span>
                                </div>
                                <ul class="mb-0">
                                    <li>Prefer shared components over custom copies for consistency.</li>
                                    <li>Use <span class="fw-bold">openComponent</span> to control modal size and options.</li>
                                    <li>Keep shared components generic and reusable across apps.</li>
                                </ul>
                            </section>
                        </div>
                    </div>
                    <div class="col-48 col-lg-12 order-1 order-lg-2 mb-2 mb-lg-0">
                        <nav id="devGuideNavShared" class="dev-guide-nav nav flex-column p-2">
                            <div class="fw-bold text-uppercase text-muted fs-d9 mb-2">On this page</div>
                            <a class="nav-link dev-guide-link" href="#purpose" @click.prevent="scrollTo('purpose')">Purpose</a>
                            <a class="nav-link dev-guide-link" href="#location" @click.prevent="scrollTo('location')">Location</a>
                            <a class="nav-link dev-guide-link" href="#loader" @click.prevent="scrollTo('loader')">Component loader</a>
                            <a class="nav-link dev-guide-link" href="#auth" @click.prevent="scrollTo('auth')">Auth & account</a>
                            <a class="nav-link dev-guide-link" href="#dialogs" @click.prevent="scrollTo('dialogs')">Dialogs</a>
                            <a class="nav-link dev-guide-link" href="#navigation" @click.prevent="scrollTo('navigation')">Navigation</a>
                            <a class="nav-link dev-guide-link" href="#system" @click.prevent="scrollTo('system')">System tools</a>
                            <a class="nav-link dev-guide-link" href="#media" @click.prevent="scrollTo('media')">Media</a>
                            <a class="nav-link dev-guide-link" href="#ai" @click.prevent="scrollTo('ai')">AI</a>
                            <a class="nav-link dev-guide-link" href="#bestpractices" @click.prevent="scrollTo('bestpractices')">Best practices</a>
                        </nav>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    shared.setAppTitle("$auto$");
    shared.setAppSubTitle("Dev Guide");

    let _this = { cid: "", c: null };

    export default {
        setup(props) {
            _this.cid = props["cid"];
        },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() {
            let scrollEl = this.$el.querySelector(".dev-guide-content");
            if (window.bootstrap?.ScrollSpy) {
                new window.bootstrap.ScrollSpy(scrollEl, {
                    target: "#devGuideNavShared",
                    rootMargin: "0px 0px -60%"
                });
            }
            initDevCodeBlocks(this.$el);
        },
        methods: {
            scrollTo(id) {
                let el = this.$el.querySelector('#' + id);
                if (el) el.scrollIntoView({ behavior: 'smooth', block: 'start' });
            },
            demoPromptEx() {
                showPromptEx({
                    title: "Confirm Action",
                    message1: "Are you sure you want to proceed?",
                    message2: "This action cannot be undone.",
                    reasonTitle: "Reason",
                    reasonRequired: true,
                    noteTitle: "Additional Notes",
                    noteRequired: false,
                    reasonsParentId: 10000,
                    callback: function (ret) { showJson(ret); }
                });
            },
            demoJsonView() {
                showJson({ Id: 7, Title: "Sample", Status: "Active" });
            }
        },
        props: { cid: String }
    }
</script>
