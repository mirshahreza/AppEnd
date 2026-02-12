<template>
    <div class="card h-100 bg-transparent rounded-0 border-0 dev-guide">
        <div class="card-header p-2 bg-body-subtle rounded-0 border-0">
            <div class="hstack gap-2">
                <div class="fw-bold text-primary">
                    <i class="fa-solid fa-screwdriver-wrench me-1"></i>
                    <span>Helpers &amp; Utilities</span>
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
                                    This page documents the core client-side helper utilities bundled with AppEnd. These helpers are used across the UI for formatting, validation, parsing, and file handling.
                                </p>
                                <ul class="mb-0">
                                    <li>Know where common utilities live (<span class="fw-bold">append-helpers.js</span>, <span class="fw-bold">append-formatters.js</span>, <span class="fw-bold">append-validators.js</span>, <span class="fw-bold">append-file-utils.js</span>).</li>
                                    <li>Reuse built-in helpers instead of writing duplicates.</li>
                                    <li>Understand formatting and localization helpers.</li>
                                </ul>
                            </section>

                            <section id="helpers" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-toolbox me-1 text-success"></i>
                                    <span class="fw-bold">Core helpers (append-helpers.js)</span>
                                </div>
                                <p class="mb-1">Common helpers for parsing, decoding, and handling null values:</p>
                                <pre class="mb-2">// Session storage helper
getSessionItemSync("key", true, () => ({ a: 1 }));

// Remove property from object or array
removeProp(obj, "Temp");

// Decode JWT
let decoded = decodeJwt(getUserToken());

// JSON parsing helpers
parseJO("{\"a\":1}")    // → object
parseJA("['a','b']")     // → array</pre>
                                <div class="fw-bold mb-1">JSON viewer helper</div>
                                <pre class="mb-2">// Opens a JSON viewer modal (BaseJsonView)
showJson({ Id: 12, Title: "Sample" });

// AccessDenied payloads show an error toast instead
showJson({ Message: "AccessDenied" });</pre>
                                <div class="dev-demo-panel">
                                    <div class="fw-bold mb-2"><i class="fa-solid fa-play me-1 text-success"></i> Live Demo</div>
                                    <div class="d-flex flex-wrap gap-2">
                                        <button class="btn btn-sm btn-primary" @click="demoShowJson"><i class="fa-solid fa-code me-1"></i> Show JSON</button>
                                        <button class="btn btn-sm btn-outline-danger" @click="demoShowAccessDenied"><i class="fa-solid fa-shield-halved me-1"></i> AccessDenied</button>
                                    </div>
                                </div>
                                <div class="fw-bold mb-1">Direction helpers</div>
                                <pre class="mb-0">getDir()      // → "ltr" or "rtl"
isRtl()       // → true / false
getTextAlignCss() // → "text-left" or "text-right"</pre>
                            </section>

                            <section id="formatters" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-pen-fancy me-1 text-info"></i>
                                    <span class="fw-bold">Formatters (append-formatters.js)</span>
                                </div>
                                <p class="mb-1">Date/number formatting and digit localization:</p>
                                <pre class="mb-2">formatDate("2025-01-10")           // → "2025-01-10"
formatDateTime("2025-01-10T08:45") // → "2025-01-10 08:45"
formatNumber(1234567)              // → "1,234,567"
format2Char(5)                     // → "05"</pre>
                                <div class="fw-bold mb-1">Calendar-aware formatting</div>
                                <pre class="mb-0">formatDateL(date, "Gregorian") // → English digits
formatDateL(date, "Jalali")     // → Persian digits
formatDateL(date, "Hijri")      // → Arabic digits

formatDateTimeL(date, calendarType)</pre>
                            </section>

                            <section id="validators" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-shield-check me-1 text-warning"></i>
                                    <span class="fw-bold">Validators (append-validators.js)</span>
                                </div>
                                <p class="mb-1">Built-in validation helpers for strings, numbers, and dates:</p>
                                <pre class="mb-2">isNumber("12")          // → true
isInteger("12")         // → true
isDate("2025-01-10")    // → true
isDateTime("2025-01-10 08:45:00") // → true

isEnglish("Hello123")  // → true
isPersian("سلام")       // → true
isArabic("مرحبا")        // → true

isNullOrUndefined(v)   // → true / false
isNaNOrEmpty(v)        // → true / false</pre>
                                <div class="fw-bold mb-1">Input helpers</div>
                                <pre class="mb-0">isNumberKey(evt)    // restrict to numeric input
isValidProgName("My_Field") // → true</pre>
                            </section>

                            <section id="fileutils" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-file-lines me-1 text-primary"></i>
                                    <span class="fw-bold">File utilities (append-file-utils.js)</span>
                                </div>
                                <p class="mb-1">Detect file types and pick syntax highlighting language:</p>
                                <pre class="mb-2">isImageFromName("photo.jpg")  // → true
isVideoFromName("movie.mp4")  // → true
isAudioFromName("track.mp3")  // → true
isPlainText("readme.txt")     // → true
isZipFile("archive.zip")      // → true
isAppEndPackage("module.aepkg") // → true</pre>
                                <div class="fw-bold mb-1">Syntax highlight by extension</div>
                                <pre class="mb-0">getLangFromFileName("App.cs")    // → "csharp"
getLangFromFileName("config.json") // → "json"
getLangFromFileName("view.vue")    // → "html"</pre>
                            </section>

                            <section id="stringhelpers" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-font me-1 text-success"></i>
                                    <span class="fw-bold">String and digit helpers</span>
                                </div>
                                <p class="mb-1">Digit conversion helpers in <span class="fw-bold">append-formatters.js</span>:</p>
                                <pre class="mb-0">traverseEn("۱۲۳") // → "123"
traverseFa("123") // → "۱۲۳"
traverseAr("123") // → "١٢٣"</pre>
                            </section>

                            <section id="bestpractices" class="dev-guide-section">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-circle-check me-1 text-success"></i>
                                    <span class="fw-bold">Best practices</span>
                                </div>
                                <ul class="mb-0">
                                    <li>Use <span class="fw-bold">formatDateL</span> and <span class="fw-bold">formatDateTimeL</span> for localized output.</li>
                                    <li>Prefer <span class="fw-bold">isNaNOrEmpty</span> before submitting form data.</li>
                                    <li>Use <span class="fw-bold">getLangFromFileName</span> when rendering code editors.</li>
                                    <li>Keep file type checks centralized via <span class="fw-bold">append-file-utils.js</span>.</li>
                                </ul>
                            </section>
                        </div>
                    </div>
                    <div class="col-48 col-lg-12 order-1 order-lg-2 mb-2 mb-lg-0">
                        <nav id="devGuideNavHelpers" class="dev-guide-nav nav flex-column p-2">
                            <div class="fw-bold text-uppercase text-muted fs-d9 mb-2">On this page</div>
                            <a class="nav-link dev-guide-link" href="#purpose" @click.prevent="scrollTo('purpose')">Purpose</a>
                            <a class="nav-link dev-guide-link" href="#helpers" @click.prevent="scrollTo('helpers')">Core helpers</a>
                            <a class="nav-link dev-guide-link" href="#formatters" @click.prevent="scrollTo('formatters')">Formatters</a>
                            <a class="nav-link dev-guide-link" href="#validators" @click.prevent="scrollTo('validators')">Validators</a>
                            <a class="nav-link dev-guide-link" href="#fileutils" @click.prevent="scrollTo('fileutils')">File utilities</a>
                            <a class="nav-link dev-guide-link" href="#stringhelpers" @click.prevent="scrollTo('stringhelpers')">String helpers</a>
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
                    target: "#devGuideNavHelpers",
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
            demoShowJson() {
                showJson({ Id: 12, Title: "Sample", Status: "Ready" });
            },
            demoShowAccessDenied() {
                showJson({ Message: "AccessDenied" });
            }
        },
        props: { cid: String }
    }
</script>
