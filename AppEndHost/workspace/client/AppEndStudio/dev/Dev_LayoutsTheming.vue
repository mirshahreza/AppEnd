<template>
    <div class="card h-100 bg-transparent rounded-0 border-0 dev-guide">
        <div class="card-header p-2 bg-body-subtle rounded-0 border-0">
            <div class="hstack gap-2">
                <div class="fw-bold text-primary">
                    <i class="fa-solid fa-palette me-1"></i>
                    <span>Layouts &amp; Theming</span>
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
                                    This page documents the layout system and theme engine used in AppEnd Studio, including layout components, the Theme Manager, and the Theme Picker UI.
                                </p>
                                <ul class="mb-0">
                                    <li>Understand which layout wraps your components.</li>
                                    <li>Customize and apply themes using built-in APIs.</li>
                                    <li>Persist user theme preferences.</li>
                                </ul>
                            </section>

                            <section id="layouts" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-layer-group me-1 text-info"></i>
                                    <span class="fw-bold">Layout components</span>
                                </div>
                                <p class="mb-1">Layouts live under <span class="fw-bold">a.Layouts</span> and wrap the entire application:</p>
                                <pre class="mb-2">// Application layout (full shell)
/a.Layouts/Application.vue

// Clean layout (no shell)
/a.Layouts/Clean.vue</pre>
                                <div class="fw-bold mb-1">Application layout highlights</div>
                                <ul class="mb-0">
                                    <li>Top bar with app title, home button, and profile dropdown.</li>
                                    <li>Built-in Theme Picker component.</li>
                                    <li>Supports mobile header controls.</li>
                                </ul>
                            </section>

                            <section id="clean-layout" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-square me-1 text-secondary"></i>
                                    <span class="fw-bold">Clean layout (minimal)</span>
                                </div>
                                <p class="mb-1">The clean layout renders only the requested component:</p>
                                <pre class="mb-0">&lt;template&gt;
  &lt;component-loader src="qs:c" cid="dynamicContent" /&gt;
&lt;/template&gt;</pre>
                                <p class="mb-0">Use this layout for standalone screens or minimal embedding.</p>
                            </section>

                            <section id="theme-manager" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-brush me-1 text-success"></i>
                                    <span class="fw-bold">Theme Manager</span>
                                </div>
                                <p class="mb-1">ThemeManager is defined in <span class="fw-bold">append-theme-manager.js</span> and exposes a simple API:</p>
                                <pre class="mb-2">ThemeManager.getThemes();       // list all themes
ThemeManager.getCurrentTheme(); // active theme id
ThemeManager.setTheme("blue");  // apply theme
ThemeManager.getThemeById("blue");
ThemeManager.DEFAULT_THEME;     // "blue"</pre>
                                <div class="fw-bold mb-1">How it applies themes</div>
                                <ul class="mb-0">
                                    <li>Sets <span class="fw-bold">data-theme</span> on <span class="fw-bold">document.documentElement</span>.</li>
                                    <li>Stores the theme in <span class="fw-bold">localStorage</span> under <span class="fw-bold">append-theme</span>.</li>
                                    <li>Emits a <span class="fw-bold">themeChanged</span> browser event.</li>
                                </ul>
                                <div class="dev-demo-panel">
                                    <div class="fw-bold mb-2"><i class="fa-solid fa-play me-1 text-success"></i> Live Demo</div>
                                    <div class="d-flex flex-wrap gap-2 align-items-center">
                                        <button class="btn btn-sm btn-primary" @click="applyTheme('blue')">Blue</button>
                                        <button class="btn btn-sm btn-outline-success" @click="applyTheme('green')">Green</button>
                                        <button class="btn btn-sm btn-outline-info" @click="applyTheme('teal')">Teal</button>
                                        <button class="btn btn-sm btn-outline-secondary" @click="applyTheme('gray')">Gray</button>
                                        <span class="text-muted fs-d8">Current: <span class="fw-bold">{{ currentTheme }}</span></span>
                                    </div>
                                </div>
                            </section>

                            <section id="theme-picker" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-swatchbook me-1 text-warning"></i>
                                    <span class="fw-bold">Theme Picker component</span>
                                </div>
                                <p class="mb-1">The Theme Picker UI is a shared component:</p>
                                <pre class="mb-2">// Used in the Application layout
&lt;component-loader src="/a.SharedComponents/ThemePicker.vue" uid="themePicker" /&gt;</pre>
                                <p class="mb-1">It offers 15 Microsoft Fluent-inspired themes grouped by color family.</p>
                                <div class="fw-bold mb-1">Theme list</div>
                                <pre class="mb-0">Blue, Cyan, Navy
Indigo, Purple, Magenta
Pink, Red
Orange, Yellow/Gold
Green, Teal
Brown, Gray, Light Gray</pre>
                            </section>

                            <section id="persistence" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-floppy-disk me-1 text-primary"></i>
                                    <span class="fw-bold">Theme persistence</span>
                                </div>
                                <p class="mb-1">Theme preference is stored in two places depending on context:</p>
                                <ul class="mb-0">
                                    <li><span class="fw-bold">User settings</span> (logged in): stored via <span class="fw-bold">shared.setUserSettings()</span>.</li>
                                    <li><span class="fw-bold">Local storage</span> (fallback): <span class="fw-bold">app-theme</span> in Theme Picker, <span class="fw-bold">append-theme</span> in ThemeManager.</li>
                                </ul>
                                <div class="fw-bold mb-1 mt-2">Example: Apply a theme manually</div>
                                <pre class="mb-0">ThemeManager.setTheme("teal");
// document.documentElement[data-theme="teal"]</pre>
                            </section>

                            <section id="bestpractices" class="dev-guide-section">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-circle-check me-1 text-success"></i>
                                    <span class="fw-bold">Best practices</span>
                                </div>
                                <ul class="mb-0">
                                    <li>Use <span class="fw-bold">Application.vue</span> for full Studio navigation and shell controls.</li>
                                    <li>Use <span class="fw-bold">Clean.vue</span> for minimal embedded screens.</li>
                                    <li>Prefer <span class="fw-bold">ThemeManager.setTheme()</span> to switch themes so events are emitted.</li>
                                    <li>Keep theme colors consistent with the palette defined in Theme Picker.</li>
                                </ul>
                            </section>
                        </div>
                    </div>
                    <div class="col-48 col-lg-12 order-1 order-lg-2 mb-2 mb-lg-0">
                        <nav id="devGuideNavLayouts" class="dev-guide-nav nav flex-column p-2">
                            <div class="fw-bold text-uppercase text-muted fs-d9 mb-2">On this page</div>
                            <a class="nav-link dev-guide-link" href="#purpose" @click.prevent="scrollTo('purpose')">Purpose</a>
                            <a class="nav-link dev-guide-link" href="#layouts" @click.prevent="scrollTo('layouts')">Layouts</a>
                            <a class="nav-link dev-guide-link" href="#clean-layout" @click.prevent="scrollTo('clean-layout')">Clean layout</a>
                            <a class="nav-link dev-guide-link" href="#theme-manager" @click.prevent="scrollTo('theme-manager')">Theme Manager</a>
                            <a class="nav-link dev-guide-link" href="#theme-picker" @click.prevent="scrollTo('theme-picker')">Theme Picker</a>
                            <a class="nav-link dev-guide-link" href="#persistence" @click.prevent="scrollTo('persistence')">Persistence</a>
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

    let _this = { cid: "", c: null, currentTheme: "" };

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
                    target: "#devGuideNavLayouts",
                    rootMargin: "0px 0px -60%"
                });
            }
            initDevCodeBlocks(this.$el);
            if (window.ThemeManager) {
                _this.c.currentTheme = ThemeManager.getCurrentTheme();
            }
        },
        methods: {
            scrollTo(id) {
                let el = this.$el.querySelector('#' + id);
                if (el) el.scrollIntoView({ behavior: 'smooth', block: 'start' });
            },
            applyTheme(themeId) {
                if (window.ThemeManager) {
                    ThemeManager.setTheme(themeId);
                    _this.c.currentTheme = ThemeManager.getCurrentTheme();
                }
            }
        },
        props: { cid: String }
    }
</script>
