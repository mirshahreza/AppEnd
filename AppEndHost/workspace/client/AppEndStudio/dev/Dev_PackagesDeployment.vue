<template>
    <div class="card h-100 bg-transparent rounded-0 border-0 dev-guide">
        <div class="card-header p-2 bg-body-subtle rounded-0 border-0">
            <div class="hstack gap-2">
                <div class="fw-bold text-primary">
                    <i class="fa-solid fa-box-open me-1"></i>
                    <span>Packages &amp; Deployment</span>
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
                                    This page documents the AppEnd package format (<span class="fw-bold">.aepkg</span>), how packages are installed, and the file operations available on the host.
                                </p>
                                <ul class="mb-0">
                                    <li>Understand the <span class="fw-bold">AppEndPackage</span> metadata model.</li>
                                    <li>Manage packages via the Package Manager UI.</li>
                                    <li>Use FileServices for host-level file operations.</li>
                                </ul>
                            </section>

                            <section id="package-model" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-cube me-1 text-info"></i>
                                    <span class="fw-bold">AppEndPackage model</span>
                                </div>
                                <p class="mb-1">Packages are described by the <span class="fw-bold">AppEndPackage</span> record:</p>
                                <pre class="mb-2">public record AppEndPackage
{
    public string Name { get; set; }
    public string Title { get; set; }
    public string Note { get; set; }
    public string Version { get; set; }
    public string Url { get; set; }

    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string UpdatedBy { get; set; }
    public DateTime UpdatedOn { get; set; }

    public string InstallSql { get; set; }
    public string UnInstallSql { get; set; }

    public bool Installed { get; set; }
    public string InstalledBy { get; set; }
    public DateTime InstalledOn { get; set; }

    public JArray MenuItems { get; set; }
}</pre>
                                <p class="mb-0">Package metadata is stored alongside the package and drives install/uninstall behavior.</p>
                            </section>

                            <section id="package-paths" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-folder-tree me-1 text-primary"></i>
                                    <span class="fw-bold">Package paths</span>
                                </div>
                                <p class="mb-1">Packages are stored under:</p>
                                <pre class="mb-0">workspace/appendpackages</pre>
                                <p class="mb-0">Package archives use the <span class="fw-bold">.aepkg</span> extension.</p>
                            </section>

                            <section id="package-manager" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-boxes-stacked me-1 text-warning"></i>
                                    <span class="fw-bold">Package Manager UI</span>
                                </div>
                                <p class="mb-1">Use the Studio UI to install, update, or remove packages:</p>
                                <ul class="mb-0">
                                    <li>Navigation: <span class="fw-bold">Server → Package Manager</span>.</li>
                                    <li>Upload an <span class="fw-bold">.aepkg</span> file to install.</li>
                                    <li>Install/uninstall triggers <span class="fw-bold">InstallSql</span> / <span class="fw-bold">UnInstallSql</span>.</li>
                                    <li>Menu items from <span class="fw-bold">MenuItems</span> can be merged into app navigation.</li>
                                </ul>
                                <div class="dev-demo-panel">
                                    <div class="fw-bold mb-2"><i class="fa-solid fa-play me-1 text-success"></i> Live Demo</div>
                                    <button class="btn btn-sm btn-primary" @click="demoOpenPackageManager"><i class="fa-solid fa-box-open me-1"></i> Open Package Manager</button>
                                </div>
                            </section>

                            <section id="file-services" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-file-arrow-up me-1 text-success"></i>
                                    <span class="fw-bold">FileServices API</span>
                                </div>
                                <p class="mb-1">Host file operations are exposed through <span class="fw-bold">FileServices</span>:</p>
                                <pre class="mb-2">// Download a file (byte[])
FileServices.DownloadFile("path/to/file");

// Upload base64 file body
FileServices.UploadFile("path/to/file", base64Body);

// List UI components in a folder
FileServices.GetUiComponents("a.Components");

// Create an empty component from template
FileServices.CreateEmptyComponent("/workspace/client/a.Components/NewComp.vue");

// File or folder operations
FileServices.RenameItem("/old/path", "/new/path");
FileServices.DuplicateItem("/path", "file" | "folder");
FileServices.DeleteItem("/path", "file" | "folder");</pre>
                            </section>

                            <section id="component-template" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-file-pen me-1 text-info"></i>
                                    <span class="fw-bold">Empty component template</span>
                                </div>
                                <p class="mb-1">When creating a new component, AppEnd copies this template:</p>
                                <pre class="mb-0">/workspace/client/a..templates/BaseEmptyComponent.cshtml</pre>
                            </section>

                            <section id="bestpractices" class="dev-guide-section">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-circle-check me-1 text-success"></i>
                                    <span class="fw-bold">Best practices</span>
                                </div>
                                <ul class="mb-0">
                                    <li>Version packages semantically (e.g., <span class="fw-bold">1.2.3</span>).</li>
                                    <li>Keep <span class="fw-bold">InstallSql</span> and <span class="fw-bold">UnInstallSql</span> idempotent.</li>
                                    <li>Validate package contents before deployment to production.</li>
                                    <li>Use <span class="fw-bold">MenuItems</span> to append navigation entries safely.</li>
                                    <li>Prefer package distribution over manual copying of files.</li>
                                </ul>
                            </section>
                        </div>
                    </div>
                    <div class="col-48 col-lg-12 order-1 order-lg-2 mb-2 mb-lg-0">
                        <nav id="devGuideNavPackages" class="dev-guide-nav nav flex-column p-2">
                            <div class="fw-bold text-uppercase text-muted fs-d9 mb-2">On this page</div>
                            <a class="nav-link dev-guide-link" href="#purpose" @click.prevent="scrollTo('purpose')">Purpose</a>
                            <a class="nav-link dev-guide-link" href="#package-model" @click.prevent="scrollTo('package-model')">Package model</a>
                            <a class="nav-link dev-guide-link" href="#package-paths" @click.prevent="scrollTo('package-paths')">Package paths</a>
                            <a class="nav-link dev-guide-link" href="#package-manager" @click.prevent="scrollTo('package-manager')">Package Manager</a>
                            <a class="nav-link dev-guide-link" href="#file-services" @click.prevent="scrollTo('file-services')">FileServices</a>
                            <a class="nav-link dev-guide-link" href="#component-template" @click.prevent="scrollTo('component-template')">Component template</a>
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
                    target: "#devGuideNavPackages",
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
            demoOpenPackageManager() {
                openComponent("components/DevopsPackageManager", {
                    title: "Package Manager",
                    modal: true,
                    modalSize: "modal-fullscreen"
                });
            }
        },
        props: { cid: String }
    }
</script>
