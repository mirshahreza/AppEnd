<template>
    <div class="card h-100 bg-transparent rounded-0 border-0 dev-guide">
        <div class="card-header p-2 bg-body-subtle rounded-0 border-0">
            <div class="hstack gap-2">
                <div class="fw-bold text-primary">
                    <i class="fa-solid fa-layer-group me-1"></i>
                    <span>Stack Technologies</span>
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
                                    This section provides a high-level view of the AppEnd technical stack so developers understand where each layer lives and which technologies are used.
                                </p>
                                <ul class="mb-0">
                                    <li>Quickly locate main paths and projects.</li>
                                    <li>Know each layer's responsibility.</li>
                                    <li>Pick a good entry point into the codebase.</li>
                                </ul>
                            </section>

                            <section id="backend" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-server me-1 text-primary"></i>
                                    <span class="fw-bold">Backend (.NET)</span>
                                </div>
                                <ul class="mb-0">
                                    <li>Server projects live under <span class="fw-bold">AppEndServer</span> and <span class="fw-bold">AppEndHost</span>.</li>
                                    <li>Runtime: <a href="https://dotnet.microsoft.com/" target="_blank" rel="noopener">.NET</a> <span class="fw-bold">10</span>.</li>
                                    <li>Data-access logic is located in <span class="fw-bold">AppEndDbIO</span>.</li>
                                    <li>Configuration typically lives in <span class="fw-bold">appsettings</span> files and host services.</li>
                                </ul>
                            </section>

                            <section id="frontend" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-desktop me-1 text-primary"></i>
                                    <span class="fw-bold">Frontend (Studio)</span>
                                </div>
                                <ul class="mb-0">
                                    <li>The Studio lives at <span class="fw-bold">AppEndHost/workspace/client/AppEndStudio</span>.</li>
                                    <li>Components are stored as <span class="fw-bold">.vue</span> files.</li>
                                    <li>Framework: <a href="https://vuejs.org/" target="_blank" rel="noopener">Vue.js</a> <span class="fw-bold">3</span>.</li>
                                    <li>UI: <a href="https://getbootstrap.com/docs/5.3/" target="_blank" rel="noopener">Bootstrap</a> <span class="fw-bold">5.3.8</span>.</li>
                                    <li>Icons: <a href="https://fontawesome.com/v6/docs" target="_blank" rel="noopener">Font Awesome</a> <span class="fw-bold">6.4.0</span>.</li>
                                </ul>
                            </section>

                            <section id="data" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-database me-1 text-primary"></i>
                                    <span class="fw-bold">Data & Storage</span>
                                </div>
                                <ul class="mb-0">
                                    <li>Database: <a href="https://www.microsoft.com/sql-server" target="_blank" rel="noopener">Microsoft SQL Server</a>.</li>
                                    <li>Database models and query operations are handled through the DbIO layer.</li>
                                    <li>Data access is orchestrated through APIs and core services.</li>
                                </ul>
                            </section>

                            <section id="quickstart" class="dev-guide-section">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-wrench me-1 text-primary"></i>
                                    <span class="fw-bold">Quick start tips</span>
                                </div>
                                <ul class="mb-0">
                                    <li>Start by learning the project structure and key paths.</li>
                                    <li>For UI, follow an existing sample component in <span class="fw-bold">components</span>.</li>
                                    <li>For backend changes, review services under <span class="fw-bold">AppEndServer</span>.</li>
                                </ul>
                            </section>
                        </div>
                    </div>
                    <div class="col-48 col-lg-12 order-1 order-lg-2 mb-2 mb-lg-0">
                        <nav id="devGuideNavStack" class="dev-guide-nav nav flex-column p-2">
                            <div class="fw-bold text-uppercase text-muted fs-d9 mb-2">On this page</div>
                            <a class="nav-link dev-guide-link" href="#purpose" @click.prevent="scrollTo('purpose')">Purpose</a>
                            <a class="nav-link dev-guide-link" href="#backend" @click.prevent="scrollTo('backend')">Backend (.NET)</a>
                            <a class="nav-link dev-guide-link" href="#frontend" @click.prevent="scrollTo('frontend')">Frontend (Studio)</a>
                            <a class="nav-link dev-guide-link" href="#data" @click.prevent="scrollTo('data')">Data & Storage</a>
                            <a class="nav-link dev-guide-link" href="#quickstart" @click.prevent="scrollTo('quickstart')">Quick start tips</a>
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
                    target: "#devGuideNavStack",
                    rootMargin: "0px 0px -60%"
                });
            }
            initDevCodeBlocks(this.$el);
        },
        methods: {
            scrollTo(id) {
                let el = this.$el.querySelector('#' + id);
                if (el) el.scrollIntoView({ behavior: 'smooth', block: 'start' });
            }
        },
        props: { cid: String }
    }
</script>
