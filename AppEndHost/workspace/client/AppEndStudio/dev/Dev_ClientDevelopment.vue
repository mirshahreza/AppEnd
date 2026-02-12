<template>
    <div class="card h-100 bg-transparent rounded-0 border-0 dev-guide">
        <div class="card-header p-2 bg-body-subtle rounded-0 border-0">
            <div class="hstack gap-2">
                <div class="fw-bold text-primary">
                    <i class="fa-solid fa-laptop-code me-1"></i>
                    <span>Client Development</span>
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
                                    This page documents client development patterns with a focus on opening components in different window modes.
                                </p>
                                <ul class="mb-0">
                                    <li>Use the same call patterns across Studio and apps.</li>
                                    <li>Control modal vs floating windows with consistent options.</li>
                                    <li>Pass parameters safely into opened components.</li>
                                </ul>
                            </section>

                            <section id="opening" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-window-restore me-1 text-primary"></i>
                                    <span class="fw-bold">Opening components</span>
                                </div>
                                <p class="mb-1">Core API: <span class="fw-bold">openComponent(src, options)</span> from <span class="fw-bold">append-window-manager.js</span>.</p>
                                <pre class="mb-2">openComponent("/a.Components/BaseUsers_ReadList", {
  title: "Users",
  modal: true,
  modalSize: "modal-lg",
  params: { role: "admin" }
});</pre>
                                <div class="dev-demo-panel">
                                    <div class="fw-bold mb-2"><i class="fa-solid fa-play me-1 text-success"></i> Live Demo</div>
                                    <div class="d-flex flex-wrap gap-2">
                                        <button class="btn btn-sm btn-primary" @click="demoOpenModal"><i class="fa-solid fa-window-maximize me-1"></i> Open Modal</button>
                                        <button class="btn btn-sm btn-outline-primary" @click="demoOpenFloating"><i class="fa-solid fa-clone me-1"></i> Open Floating</button>
                                    </div>
                                </div>
                            </section>

                            <section id="modes" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-layer-group me-1 text-primary"></i>
                                    <span class="fw-bold">Window modes</span>
                                </div>
                                <ul class="mb-0">
                                    <li><span class="fw-bold">modal: true</span> opens a Bootstrap modal.</li>
                                    <li><span class="fw-bold">modal: false</span> opens a floating window.</li>
                                    <li><span class="fw-bold">modalSize: "modal-fullscreen"</span> opens fullscreen and disables resize switch.</li>
                                    <li><span class="fw-bold">windowSizeSwitchable: true</span> shows the maximize switch button.</li>
                                </ul>
                            </section>

                            <section id="params" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-sliders me-1 text-primary"></i>
                                    <span class="fw-bold">Passing parameters</span>
                                </div>
                                <p class="mb-1">Parameters are stored by window id and read inside the component.</p>
                                <pre class="mb-2">openComponent("/a.Components/BaseUsers_ReadList", {
  params: { roleId: 2, status: "Active" }
});</pre>
                                <div class="fw-bold mb-1">Read inside the component</div>
                                <pre class="mb-0">setup(props) {
  _this.cid = props["cid"];
  _this.inputs = shared["params_" + _this.cid];
}</pre>
                            </section>

                            <section id="modal-options" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-gear me-1 text-primary"></i>
                                    <span class="fw-bold">Modal options (common)</span>
                                </div>
                                <pre class="mb-0">openComponent("/a.Components/BaseInfo_Create", {
  title: "Create BaseInfo",
  modal: true,
  modalSize: "modal-lg",
  placement: "modal-dialog-centered",
  showHeader: true,
  showCloseButton: true,
  backdrop: true,
  closeByOverlay: false,
  headerCSS: "bg-light bg-gradient",
  modalBodyCSS: "bg-light bg-gradient",
  border: "border-4 border-secondary",
  animation: "fade",
  modalMargin: "p-lg-5 p-md-3 p-sm-1"
});</pre>
                            </section>

                            <section id="floating" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-clone me-1 text-primary"></i>
                                    <span class="fw-bold">Floating windows</span>
                                </div>
                                <p class="mb-1">Floating windows support drag/resize and explicit sizing.</p>
                                <pre class="mb-0">openComponent("/a.Components/BaseInfo_ReadList", {
  modal: false,
  width: 900,
  height: 600,
  top: 80,
  left: 120,
  draggable: true,
  resizable: true,
  showHeader: true,
  windowSizeSwitchable: true
});</pre>
                            </section>

                            <section id="element" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-link me-1 text-primary"></i>
                                    <span class="fw-bold">Opening from markup</span>
                                </div>
                                <p class="mb-1">Use <span class="fw-bold">openComponentByEl</span> with data attributes.</p>
                                <pre class="mb-0">&lt;button
  data-ae-src="/a.Components/BaseUsers_ReadList"
  data-ae-options='{ "title": "Users", "modal": true, "modalSize": "modal-xl" }'
  onclick="openComponentByEl(event)"&gt;
  Open Users
&lt;/button&gt;</pre>
                            </section>

                            <section id="callback" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-arrow-rotate-left me-1 text-primary"></i>
                                    <span class="fw-bold">Caller and callback</span>
                                </div>
                                <p class="mb-1">Use <span class="fw-bold">caller</span> and <span class="fw-bold">callback</span> for post-close actions.</p>
                                <pre class="mb-0">openComponent("/a.Components/BaseUsers_Create", {
  title: "Create User",
  caller: this,
  callback: function(result) {
    if (result?.IsSucceeded) {
      this.reloadList();
    }
  }
});</pre>
                            </section>

                            <section id="component-loader" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-puzzle-piece me-1 text-primary"></i>
                                    <span class="fw-bold">Component loader basics</span>
                                </div>
                                <p class="mb-1">Windows are rendered using <span class="fw-bold">BaseComponentLoader</span> with <span class="fw-bold">ismodal</span>.</p>
                                <pre class="mb-0">&lt;comp-loader
  src="/a.Components/BaseUsers_ReadList"
  uid="c_{{id}}"
  cid="{{id}}"
  ismodal="true" /&gt;</pre>
                            </section>

                            <section id="promptex" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-comment-dots me-1 text-primary"></i>
                                    <span class="fw-bold">PromptEx (structured confirmation)</span>
                                </div>
                                <p class="mb-1">Use <span class="fw-bold">showPromptEx</span> to display a rich confirmation dialog with reason, note, and callback.</p>
                                <pre class="mb-2">showPromptEx({
  title: "Confirm Action",
  message1: "Are you sure you want to proceed?",
  message2: "This action cannot be undone.",
  reasonTitle: "Reason",
  reasonRequired: true,
  noteTitle: "Additional Notes",
  noteRequired: true,
  noteRule: ":=s(8,4000)",
  reasonsParentId: 10000,
  callback: function (ret) {
    console.log(ret);
  }
});</pre>
                                <div class="fw-bold mb-1">Options reference</div>
                                <ul class="mb-2">
                                    <li><span class="fw-bold">title</span> — dialog title.</li>
                                    <li><span class="fw-bold">message1</span> — primary message (bold).</li>
                                    <li><span class="fw-bold">message2</span> — secondary description text.</li>
                                    <li><span class="fw-bold">reasonTitle / reasonRequired</span> — adds a reason dropdown (from BaseInfo by <span class="fw-bold">reasonsParentId</span>).</li>
                                    <li><span class="fw-bold">noteTitle / noteRequired / noteRule</span> — adds a free-text note field with optional validation.</li>
                                    <li><span class="fw-bold">callback(ret)</span> — called with <span class="fw-bold">{ Reason, Note }</span> when user confirms.</li>
                                </ul>
                                <div class="dev-demo-panel">
                                    <div class="fw-bold mb-2"><i class="fa-solid fa-play me-1 text-success"></i> Live Demo</div>
                                    <button class="btn btn-sm btn-success" @click="demoPromptEx"><i class="fa-solid fa-comment-dots me-1"></i> Show PromptEx</button>
                                </div>
                            </section>

                            <section id="showmessage" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-bell me-1 text-primary"></i>
                                    <span class="fw-bold">Show Message (toast notifications)</span>
                                </div>
                                <p class="mb-1">Display toast-style notifications using these global helpers:</p>
                                <pre class="mb-2">// Success
showSuccess("Record saved successfully.");

// Info
showInfo("Data loaded from cache.");

// Error
showError("Something went wrong. Please try again.");

// Warning
showWarning("You have unsaved changes.");</pre>
                                <div class="fw-bold mb-1">Functions</div>
                                <ul class="mb-2">
                                    <li><span class="fw-bold">showSuccess(msg)</span> — green toast for success actions.</li>
                                    <li><span class="fw-bold">showInfo(msg)</span> — blue toast for informational messages.</li>
                                    <li><span class="fw-bold">showError(msg)</span> — red toast for error conditions.</li>
                                    <li><span class="fw-bold">showWarning(msg)</span> — yellow toast for warnings.</li>
                                </ul>
                                <div class="dev-demo-panel">
                                    <div class="fw-bold mb-2"><i class="fa-solid fa-play me-1 text-success"></i> Live Demo</div>
                                    <div class="d-flex flex-wrap gap-2">
                                        <button class="btn btn-sm btn-success" @click="demoShowSuccess">ShowSuccess</button>
                                        <button class="btn btn-sm btn-info text-white" @click="demoShowInfo">ShowInfo</button>
                                        <button class="btn btn-sm btn-danger" @click="demoShowError">ShowError</button>
                                        <button class="btn btn-sm btn-warning" @click="demoShowWarning">ShowWarning</button>
                                    </div>
                                </div>
                            </section>

                            <section id="bestpractices" class="dev-guide-section">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-circle-check me-1 text-primary"></i>
                                    <span class="fw-bold">Best practices</span>
                                </div>
                                <ul class="mb-0">
                                    <li>Prefer modal dialogs for CRUD flows; use floating windows for multi-tasking.</li>
                                    <li>Always pass data via <span class="fw-bold">params</span> instead of globals.</li>
                                    <li>Keep window titles meaningful for better UX.</li>
                                </ul>
                            </section>
                        </div>
                    </div>
                    <div class="col-48 col-lg-12 order-1 order-lg-2 mb-2 mb-lg-0">
                        <nav id="devGuideNavClient" class="dev-guide-nav nav flex-column p-2">
                            <div class="fw-bold text-uppercase text-muted fs-d9 mb-2">On this page</div>
                            <a class="nav-link dev-guide-link" href="#purpose" @click.prevent="scrollTo('purpose')">Purpose</a>
                            <a class="nav-link dev-guide-link" href="#opening" @click.prevent="scrollTo('opening')">Opening components</a>
                            <a class="nav-link dev-guide-link" href="#modes" @click.prevent="scrollTo('modes')">Window modes</a>
                            <a class="nav-link dev-guide-link" href="#params" @click.prevent="scrollTo('params')">Parameters</a>
                            <a class="nav-link dev-guide-link" href="#modal-options" @click.prevent="scrollTo('modal-options')">Modal options</a>
                            <a class="nav-link dev-guide-link" href="#floating" @click.prevent="scrollTo('floating')">Floating windows</a>
                            <a class="nav-link dev-guide-link" href="#element" @click.prevent="scrollTo('element')">Open from markup</a>
                            <a class="nav-link dev-guide-link" href="#callback" @click.prevent="scrollTo('callback')">Callback</a>
                            <a class="nav-link dev-guide-link" href="#component-loader" @click.prevent="scrollTo('component-loader')">Component loader</a>
                            <a class="nav-link dev-guide-link" href="#promptex" @click.prevent="scrollTo('promptex')">PromptEx</a>
                            <a class="nav-link dev-guide-link" href="#showmessage" @click.prevent="scrollTo('showmessage')">Show Message</a>
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
                    target: "#devGuideNavClient",
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
                    title: "Demo PromptEx",
                    message1: "This is the primary message",
                    message2: "This is the secondary description with more detail.",
                    reasonTitle: "Reason",
                    reasonRequired: true,
                    noteTitle: "Note",
                    noteRequired: true,
                    noteRule: ":=s(8,4000)",
                    reasonsParentId: 10000,
                    callback: function (ret) { showJson(ret); }
                });
            },
            demoShowSuccess() { showSuccess("This is a success message."); },
            demoShowInfo() { showInfo("This is an info message."); },
            demoShowError() { showError("This is an error message."); },
            demoShowWarning() { showWarning("This is a warning message."); },
            demoOpenModal() {
                openComponent("/a.Components/BaseRoles_ReadList", {
                    title: "Roles (Modal Demo)",
                    modal: true,
                    modalSize: "modal-lg",
                    closeByOverlay: true
                });
            },
            demoOpenFloating() {
                openComponent("/a.Components/BaseRoles_ReadList", {
                    title: "Roles (Floating Demo)",
                    modal: false,
                    width: 700,
                    height: 450,
                    draggable: true,
                    resizable: true,
                    windowSizeSwitchable: true
                });
            }
        },
        props: { cid: String }
    }
</script>
