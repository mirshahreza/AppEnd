<template>
    <div class="card h-100 bg-transparent rounded-0 border-0 dev-guide">
        <div class="card-header p-2 bg-body-subtle rounded-0 border-0">
            <div class="hstack gap-2">
                <div class="fw-bold text-primary">
                    <i class="fa-solid fa-hat-wizard me-1"></i>
                    <span>Wizard (bsTabsAutoNav)</span>
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
                                    This page documents the <span class="fw-bold">bsTabsAutoNav</span> jQuery plugin that turns Bootstrap tab-panes into a wizard with automatic navigation (nav-items or back/next buttons).
                                </p>
                                <ul class="mb-0">
                                    <li>Build step-by-step wizards from standard Bootstrap tabs.</li>
                                    <li>Choose between nav-items, back/next, or combined navigation modes.</li>
                                    <li>Pick a stepper style: default tabs, connected circles, arrow steps, or progress bar.</li>
                                </ul>
                            </section>

                            <section id="initialization" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-play me-1 text-primary"></i>
                                    <span class="fw-bold">Initialization</span>
                                </div>
                                <pre class="mb-0">$("#myNav").bsTabsAutoNav({
  tabsContentsId: "myTabs",
  mode: "nav-items",
  navStyle: "nav-underline nav-justified",
  nextTitle: "Next",
  prevTitle: "Previous",
  justAllowByBackNext: false,
  dir: "ltr"
});</pre>
                            </section>

                            <section id="options" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-gear me-1 text-primary"></i>
                                    <span class="fw-bold">Options reference</span>
                                </div>
                                <ul class="mb-0">
                                    <li><span class="fw-bold">tabsContentsId</span> — (required) id of the <code>.tab-content</code> container that holds <code>.tab-pane</code> elements.</li>
                                    <li><span class="fw-bold">mode</span> — <code>"nav-items"</code> renders clickable tab links; <code>"back-next"</code> renders Previous / Next buttons.</li>
                                    <li><span class="fw-bold">stepperStyle</span> — <code>"default"</code> uses Bootstrap nav tabs; <code>"connected-circles"</code> renders a horizontal stepper with connected circles; <code>"arrow-steps"</code> renders chevron/arrow shaped segments; <code>"progress-bar"</code> renders pill-shaped steps connected by an animated progress track.</li>
                                    <li><span class="fw-bold">navStyle</span> — Bootstrap nav classes applied to the generated <code>&lt;ul&gt;</code> (e.g. <code>"nav-underline nav-justified"</code>, <code>"nav-pills"</code>). Only used when <code>stepperStyle</code> is <code>"default"</code>.</li>
                                    <li><span class="fw-bold">nextTitle / prevTitle</span> — button labels (auto-translated if <code>translate()</code> exists).</li>
                                    <li><span class="fw-bold">completeTitle</span> — label for the final circle in <code>"connected-circles"</code> mode (default: <code>"Complete"</code>).</li>
                                    <li><span class="fw-bold">justAllowByBackNext</span> — when <code>true</code>, tab links are disabled; navigation is only via back/next buttons.</li>
                                    <li><span class="fw-bold">dir</span> — <code>"ltr"</code> or <code>"rtl"</code>; controls chevron direction on back/next buttons.</li>
                                </ul>
                            </section>

                            <section id="tab-attributes" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-code me-1 text-primary"></i>
                                    <span class="fw-bold">Tab-pane data attributes</span>
                                </div>
                                <p class="mb-1">Each <code>.tab-pane</code> must carry these attributes so the plugin can build nav items:</p>
                                <pre class="mb-0">&lt;div class="tab-pane" id="step1"
  data-ae-tab-title="Personal Info"
  data-ae-tab-icon="fa-user"&gt;
  ...content...
&lt;/div&gt;</pre>
                            </section>

                            <section id="nav-items" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-window-restore me-1 text-primary"></i>
                                    <span class="fw-bold">Nav-items mode</span>
                                </div>
                                <p class="mb-1">Renders clickable tab links above the tab content.</p>
                                <pre class="mb-2">$("#myNav").bsTabsAutoNav({
  tabsContentsId: "myTabs",
  mode: "nav-items",
  navStyle: "nav-underline nav-justified"
});</pre>
                                <div class="dev-demo-panel">
                                    <div class="fw-bold mb-2"><i class="fa-solid fa-play me-1 text-success"></i> Live Demo — nav-items mode</div>
                                    <div :id="'demoWizNav_' + cid" class="mb-2"></div>
                                    <div class="tab-content border rounded p-3" :id="'demoWizTabs_' + cid">
                                        <div class="tab-pane fade" :id="'demoStep1_' + cid" data-ae-tab-title="Personal" data-ae-tab-icon="fa-user">
                                            <div class="p-3 text-center"><i class="fa-solid fa-user fa-2x text-primary mb-2"></i><div>Step 1 — Personal Info</div></div>
                                        </div>
                                        <div class="tab-pane fade" :id="'demoStep2_' + cid" data-ae-tab-title="Address" data-ae-tab-icon="fa-location-dot">
                                            <div class="p-3 text-center"><i class="fa-solid fa-location-dot fa-2x text-primary mb-2"></i><div>Step 2 — Address</div></div>
                                        </div>
                                        <div class="tab-pane fade" :id="'demoStep3_' + cid" data-ae-tab-title="Review" data-ae-tab-icon="fa-check-circle">
                                            <div class="p-3 text-center"><i class="fa-solid fa-check-circle fa-2x text-success mb-2"></i><div>Step 3 — Review & Submit</div></div>
                                        </div>
                                    </div>
                                </div>
                            </section>

                            <section id="back-next" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-arrows-left-right me-1 text-primary"></i>
                                    <span class="fw-bold">Back / Next mode</span>
                                </div>
                                <pre class="mb-2">$("#myBackNext").bsTabsAutoNav({
  tabsContentsId: "myTabs",
  mode: "back-next",
  nextTitle: "Next",
  prevTitle: "Previous",
  dir: "ltr"
});</pre>
                                <p class="mb-2">In <span class="fw-bold">back-next</span> mode the plugin validates inputs (via <code>data-ae-widget="inputsRegulator"</code>) before allowing forward navigation. Buttons are automatically disabled at the first / last step.</p>
                                <div class="dev-demo-panel">
                                    <div class="fw-bold mb-2"><i class="fa-solid fa-play me-1 text-success"></i> Live Demo — back-next mode</div>
                                    <div class="tab-content border rounded p-3" :id="'demoWizTabsBN_' + cid">
                                        <div class="tab-pane fade" :id="'demoBNStep1_' + cid" data-ae-tab-title="Account" data-ae-tab-icon="fa-id-card">
                                            <div class="p-3 text-center"><i class="fa-solid fa-id-card fa-2x text-primary mb-2"></i><div>Step 1 — Account Setup</div></div>
                                        </div>
                                        <div class="tab-pane fade" :id="'demoBNStep2_' + cid" data-ae-tab-title="Preferences" data-ae-tab-icon="fa-sliders">
                                            <div class="p-3 text-center"><i class="fa-solid fa-sliders fa-2x text-primary mb-2"></i><div>Step 2 — Preferences</div></div>
                                        </div>
                                        <div class="tab-pane fade" :id="'demoBNStep3_' + cid" data-ae-tab-title="Confirm" data-ae-tab-icon="fa-circle-check">
                                            <div class="p-3 text-center"><i class="fa-solid fa-circle-check fa-2x text-success mb-2"></i><div>Step 3 — Confirm</div></div>
                                        </div>
                                    </div>
                                    <div :id="'demoWizBN_' + cid" class="mt-2"></div>
                                </div>
                            </section>

                            <section id="locked" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-lock me-1 text-primary"></i>
                                    <span class="fw-bold">Locked navigation (justAllowByBackNext)</span>
                                </div>
                                <pre class="mb-2">$("#lockedNav").bsTabsAutoNav({
  tabsContentsId: "myTabs",
  mode: "nav-items",
  justAllowByBackNext: true
});</pre>
                                <p class="mb-0">When <span class="fw-bold">justAllowByBackNext</span> is <code>true</code>, the tab links are rendered but disabled — the user can only move via a separate back/next bar.</p>
                            </section>

                            <section id="combined" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-layer-group me-1 text-primary"></i>
                                    <span class="fw-bold">Combined mode (top nav + bottom back/next)</span>
                                </div>
                                <p class="mb-1">You can attach two <span class="fw-bold">bsTabsAutoNav</span> instances to the <span class="fw-bold">same</span> tab-content — one as <code>nav-items</code> on top (with <code>justAllowByBackNext: true</code> to lock direct clicks) and one as <code>back-next</code> on the bottom. This gives users a progress indicator while enforcing sequential navigation.</p>
                                <pre class="mb-2">// Top: read-only progress indicator
$("#topNav").bsTabsAutoNav({
  tabsContentsId: "wizardTabs",
  mode: "nav-items",
  navStyle: "nav-underline nav-justified",
  justAllowByBackNext: true
});

// Bottom: back/next buttons (justAllowByBackNext keeps manual toggle)
$("#bottomNav").bsTabsAutoNav({
  tabsContentsId: "wizardTabs",
  mode: "back-next",
  justAllowByBackNext: true,
  dir: "ltr"
});</pre>
                                <div class="dev-demo-panel">
                                    <div class="fw-bold mb-2"><i class="fa-solid fa-play me-1 text-success"></i> Live Demo — combined (top nav + bottom back/next)</div>
                                    <div :id="'demoWizComboTop_' + cid" class="mb-2"></div>
                                    <div class="tab-content border rounded p-3" :id="'demoWizTabsCombo_' + cid">
                                        <div class="tab-pane fade" :id="'demoComboStep1_' + cid" data-ae-tab-title="Info" data-ae-tab-icon="fa-circle-info">
                                            <div class="p-3 text-center"><i class="fa-solid fa-circle-info fa-2x text-primary mb-2"></i><div>Step 1 — Basic Information</div></div>
                                        </div>
                                        <div class="tab-pane fade" :id="'demoComboStep2_' + cid" data-ae-tab-title="Details" data-ae-tab-icon="fa-list-check">
                                            <div class="p-3 text-center"><i class="fa-solid fa-list-check fa-2x text-primary mb-2"></i><div>Step 2 — Details</div></div>
                                        </div>
                                        <div class="tab-pane fade" :id="'demoComboStep3_' + cid" data-ae-tab-title="Upload" data-ae-tab-icon="fa-cloud-arrow-up">
                                            <div class="p-3 text-center"><i class="fa-solid fa-cloud-arrow-up fa-2x text-primary mb-2"></i><div>Step 3 — Upload Files</div></div>
                                        </div>
                                        <div class="tab-pane fade" :id="'demoComboStep4_' + cid" data-ae-tab-title="Done" data-ae-tab-icon="fa-circle-check">
                                            <div class="p-3 text-center"><i class="fa-solid fa-circle-check fa-2x text-success mb-2"></i><div>Step 4 — Done</div></div>
                                        </div>
                                    </div>
                                    <div :id="'demoWizComboBottom_' + cid" class="mt-2"></div>
                                </div>
                            </section>

                            <section id="connected-circles" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-circle-nodes me-1 text-primary"></i>
                                    <span class="fw-bold">Connected circles stepper</span>
                                </div>
                                <p class="mb-1">Set <code>stepperStyle: "connected-circles"</code> to render a horizontal stepper with connected circles. Each tab-pane becomes a numbered circle, and a final <span class="fw-bold">Complete</span> circle is appended automatically. Completed steps show a check icon and the connecting line turns teal.</p>
                                <pre class="mb-2">$("#myStepper").bsTabsAutoNav({
  tabsContentsId: "myTabs",
  mode: "nav-items",
  stepperStyle: "connected-circles",
  completeTitle: "Complete"
});</pre>
                                <div class="dev-demo-panel">
                                    <div class="fw-bold mb-2"><i class="fa-solid fa-play me-1 text-success"></i> Live Demo — connected-circles stepper</div>
                                    <div :id="'demoWizStepper_' + cid" class="mb-2"></div>
                                    <div class="tab-content border rounded p-3" :id="'demoWizTabsStepper_' + cid">
                                        <div class="tab-pane fade" :id="'demoStepperS1_' + cid" data-ae-tab-title="Custom label" data-ae-tab-icon="fa-pen">
                                            <div class="p-3 text-center"><i class="fa-solid fa-pen fa-2x text-primary mb-2"></i><div>Step 1 — Custom label</div></div>
                                        </div>
                                        <div class="tab-pane fade" :id="'demoStepperS2_' + cid" data-ae-tab-title="Step 2" data-ae-tab-icon="fa-list">
                                            <div class="p-3 text-center"><i class="fa-solid fa-list fa-2x text-primary mb-2"></i><div>Step 2 — Details</div></div>
                                        </div>
                                        <div class="tab-pane fade" :id="'demoStepperS3_' + cid" data-ae-tab-title="Step 3" data-ae-tab-icon="fa-image">
                                            <div class="p-3 text-center"><i class="fa-solid fa-image fa-2x text-primary mb-2"></i><div>Step 3 — Media</div></div>
                                        </div>
                                        <div class="tab-pane fade" :id="'demoStepperS4_' + cid" data-ae-tab-title="Step 4" data-ae-tab-icon="fa-gear">
                                            <div class="p-3 text-center"><i class="fa-solid fa-gear fa-2x text-primary mb-2"></i><div>Step 4 — Settings</div></div>
                                        </div>
                                        <div class="tab-pane fade" :id="'demoStepperS5_' + cid" data-ae-tab-title="Step 5" data-ae-tab-icon="fa-flag-checkered">
                                            <div class="p-3 text-center"><i class="fa-solid fa-flag-checkered fa-2x text-success mb-2"></i><div>Step 5 — Final Review</div></div>
                                        </div>
                                    </div>
                                    <div :id="'demoWizStepperBN_' + cid" class="mt-2"></div>
                                </div>
                            </section>

                            <section id="arrow-steps" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-chevron-right me-1 text-primary"></i>
                                    <span class="fw-bold">Arrow steps stepper</span>
                                </div>
                                <p class="mb-1">Set <code>stepperStyle: "arrow-steps"</code> to render a horizontal stepper where each step is a chevron/arrow shaped segment. The active step is highlighted with a white background and a red number badge. Completed steps appear darker.</p>
                                <pre class="mb-2">$("#myStepper").bsTabsAutoNav({
  tabsContentsId: "myTabs",
  mode: "nav-items",
  stepperStyle: "arrow-steps"
});</pre>
                                <div class="dev-demo-panel">
                                    <div class="fw-bold mb-2"><i class="fa-solid fa-play me-1 text-success"></i> Live Demo — arrow-steps stepper</div>
                                    <div :id="'demoWizArrow_' + cid" class="mb-2"></div>
                                    <div class="tab-content border rounded p-3" :id="'demoWizTabsArrow_' + cid">
                                        <div class="tab-pane fade" :id="'demoArrowS1_' + cid" data-ae-tab-title="Step 1" data-ae-tab-icon="fa-user">
                                            <div class="p-3 text-center"><i class="fa-solid fa-user fa-2x text-primary mb-2"></i><div>Step 1 — Account Info</div></div>
                                        </div>
                                        <div class="tab-pane fade" :id="'demoArrowS2_' + cid" data-ae-tab-title="Step 2" data-ae-tab-icon="fa-address-card">
                                            <div class="p-3 text-center"><i class="fa-solid fa-address-card fa-2x text-primary mb-2"></i><div>Step 2 — Profile</div></div>
                                        </div>
                                        <div class="tab-pane fade" :id="'demoArrowS3_' + cid" data-ae-tab-title="Step 3" data-ae-tab-icon="fa-circle-check">
                                            <div class="p-3 text-center"><i class="fa-solid fa-circle-check fa-2x text-success mb-2"></i><div>Step 3 — Confirm</div></div>
                                        </div>
                                    </div>
                                    <div :id="'demoWizArrowBN_' + cid" class="mt-2"></div>
                                </div>
                            </section>

                            <section id="progress-bar" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-bars-progress me-1 text-primary"></i>
                                    <span class="fw-bold">Progress bar stepper</span>
                                </div>
                                <p class="mb-1">Set <code>stepperStyle: "progress-bar"</code> to render a compact stepper with icon circles, short baseline bars, and a subtle connector line between steps. Completed steps show a filled circle with a check icon while the active step uses a white circle with a blue outline.</p>
                                <pre class="mb-2">$("#myStepper").bsTabsAutoNav({
  tabsContentsId: "myTabs",
  mode: "nav-items",
  stepperStyle: "progress-bar"
});</pre>
                                <div class="dev-demo-panel">
                                    <div class="fw-bold mb-2"><i class="fa-solid fa-play me-1 text-success"></i> Live Demo — progress-bar stepper</div>
                                    <div :id="'demoWizProgress_' + cid" class="mb-2"></div>
                                    <div class="tab-content border rounded p-3" :id="'demoWizTabsProgress_' + cid">
                                        <div class="tab-pane fade" :id="'demoProgressS1_' + cid" data-ae-tab-title="Setup" data-ae-tab-icon="fa-wrench">
                                            <div class="p-3 text-center"><i class="fa-solid fa-wrench fa-2x text-primary mb-2"></i><div>Step 1 — Project Setup</div></div>
                                        </div>
                                        <div class="tab-pane fade" :id="'demoProgressS2_' + cid" data-ae-tab-title="Config" data-ae-tab-icon="fa-sliders">
                                            <div class="p-3 text-center"><i class="fa-solid fa-sliders fa-2x text-primary mb-2"></i><div>Step 2 — Configuration</div></div>
                                        </div>
                                        <div class="tab-pane fade" :id="'demoProgressS3_' + cid" data-ae-tab-title="Deploy" data-ae-tab-icon="fa-rocket">
                                            <div class="p-3 text-center"><i class="fa-solid fa-rocket fa-2x text-primary mb-2"></i><div>Step 3 — Deploy</div></div>
                                        </div>
                                        <div class="tab-pane fade" :id="'demoProgressS4_' + cid" data-ae-tab-title="Verify" data-ae-tab-icon="fa-clipboard-check">
                                            <div class="p-3 text-center"><i class="fa-solid fa-clipboard-check fa-2x text-success mb-2"></i><div>Step 4 — Verify</div></div>
                                        </div>
                                    </div>
                                    <div :id="'demoWizProgressBN_' + cid" class="mt-2"></div>
                                </div>
                            </section>

                            <section id="bestpractices" class="dev-guide-section">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-circle-check me-1 text-primary"></i>
                                    <span class="fw-bold">Best practices</span>
                                </div>
                                <ul class="mb-0">
                                    <li>Use <span class="fw-bold">combined mode</span> (top nav + bottom back/next) for guided multi-step forms.</li>
                                    <li>Set <span class="fw-bold">justAllowByBackNext: true</span> when steps must be completed sequentially.</li>
                                    <li>Choose <span class="fw-bold">connected-circles</span>, <span class="fw-bold">arrow-steps</span>, or <span class="fw-bold">progress-bar</span> for a more visual progress indicator.</li>
                                    <li>Always provide meaningful <code>data-ae-tab-title</code> and <code>data-ae-tab-icon</code> on each tab-pane.</li>
                                </ul>
                            </section>

                        </div>
                    </div>
                    <div class="col-48 col-lg-12 order-1 order-lg-2 mb-2 mb-lg-0">
                        <nav id="devGuideNavWizard" class="dev-guide-nav nav flex-column p-2">
                            <div class="fw-bold text-uppercase text-muted fs-d9 mb-2">On this page</div>
                            <a class="nav-link dev-guide-link" href="#purpose" @click.prevent="scrollTo('purpose')">Purpose</a>
                            <a class="nav-link dev-guide-link" href="#initialization" @click.prevent="scrollTo('initialization')">Initialization</a>
                            <a class="nav-link dev-guide-link" href="#options" @click.prevent="scrollTo('options')">Options reference</a>
                            <a class="nav-link dev-guide-link" href="#tab-attributes" @click.prevent="scrollTo('tab-attributes')">Tab-pane attributes</a>
                            <a class="nav-link dev-guide-link" href="#nav-items" @click.prevent="scrollTo('nav-items')">Nav-items mode</a>
                            <a class="nav-link dev-guide-link" href="#back-next" @click.prevent="scrollTo('back-next')">Back / Next mode</a>
                            <a class="nav-link dev-guide-link" href="#locked" @click.prevent="scrollTo('locked')">Locked navigation</a>
                            <a class="nav-link dev-guide-link" href="#combined" @click.prevent="scrollTo('combined')">Combined mode</a>
                            <a class="nav-link dev-guide-link" href="#connected-circles" @click.prevent="scrollTo('connected-circles')">Connected circles</a>
                            <a class="nav-link dev-guide-link" href="#arrow-steps" @click.prevent="scrollTo('arrow-steps')">Arrow steps</a>
                            <a class="nav-link dev-guide-link" href="#progress-bar" @click.prevent="scrollTo('progress-bar')">Progress bar</a>
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
                    target: "#devGuideNavWizard",
                    rootMargin: "0px 0px -60%"
                });
            }
            initDevCodeBlocks(this.$el);
            this.initWizardDemos();
        },
        methods: {
            scrollTo(id) {
                let el = this.$el.querySelector('#' + id);
                if (el) el.scrollIntoView({ behavior: 'smooth', block: 'start' });
            },
            initWizardDemos() {
                this.$nextTick(() => {
                    let navId = "demoWizNav_" + this.cid;
                    let tabsId = "demoWizTabs_" + this.cid;
                    let bnBarId = "demoWizBN_" + this.cid;
                    let bnTabsId = "demoWizTabsBN_" + this.cid;
                    let comboTopId = "demoWizComboTop_" + this.cid;
                    let comboBottomId = "demoWizComboBottom_" + this.cid;
                    let comboTabsId = "demoWizTabsCombo_" + this.cid;
                    if ($("#" + tabsId).length) {
                        $("#" + navId).bsTabsAutoNav({ tabsContentsId: tabsId, mode: "nav-items", navStyle: "nav-underline nav-justified" });
                    }
                    if ($("#" + bnTabsId).length) {
                        $("#" + bnBarId).bsTabsAutoNav({ tabsContentsId: bnTabsId, mode: "back-next", dir: document.dir || "ltr" });
                    }
                    if ($("#" + comboTabsId).length) {
                        $("#" + comboTopId).bsTabsAutoNav({ tabsContentsId: comboTabsId, mode: "nav-items", navStyle: "nav-underline nav-justified", justAllowByBackNext: true });
                        $("#" + comboBottomId).bsTabsAutoNav({ tabsContentsId: comboTabsId, mode: "back-next", justAllowByBackNext: true, dir: document.dir || "ltr" });
                    }
                    let stepperNavId = "demoWizStepper_" + this.cid;
                    let stepperTabsId = "demoWizTabsStepper_" + this.cid;
                    let stepperBNId = "demoWizStepperBN_" + this.cid;
                    if ($("#" + stepperTabsId).length) {
                        $("#" + stepperNavId).bsTabsAutoNav({ tabsContentsId: stepperTabsId, mode: "nav-items", stepperStyle: "connected-circles", completeTitle: "Complete" });
                        $("#" + stepperBNId).bsTabsAutoNav({ tabsContentsId: stepperTabsId, mode: "back-next", dir: document.dir || "ltr" });
                    }
                    let arrowNavId = "demoWizArrow_" + this.cid;
                    let arrowTabsId = "demoWizTabsArrow_" + this.cid;
                    let arrowBNId = "demoWizArrowBN_" + this.cid;
                    if ($("#" + arrowTabsId).length) {
                        $("#" + arrowNavId).bsTabsAutoNav({ tabsContentsId: arrowTabsId, mode: "nav-items", stepperStyle: "arrow-steps" });
                        $("#" + arrowBNId).bsTabsAutoNav({ tabsContentsId: arrowTabsId, mode: "back-next", dir: document.dir || "ltr" });
                    }
                    let progressNavId = "demoWizProgress_" + this.cid;
                    let progressTabsId = "demoWizTabsProgress_" + this.cid;
                    let progressBNId = "demoWizProgressBN_" + this.cid;
                    if ($("#" + progressTabsId).length) {
                        $("#" + progressNavId).bsTabsAutoNav({ tabsContentsId: progressTabsId, mode: "nav-items", stepperStyle: "progress-bar" });
                        $("#" + progressBNId).bsTabsAutoNav({ tabsContentsId: progressTabsId, mode: "back-next", dir: document.dir || "ltr" });
                    }
                });
            }
        },
        props: { cid: String }
    }
</script>
