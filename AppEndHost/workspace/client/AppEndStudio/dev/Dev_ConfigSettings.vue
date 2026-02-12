<template>
    <div class="card h-100 bg-transparent rounded-0 border-0 dev-guide">
        <div class="card-header p-2 bg-body-subtle rounded-0 border-0">
            <div class="hstack gap-2">
                <div class="fw-bold text-primary">
                    <i class="fa-solid fa-gear me-1"></i>
                    <span>Configuration &amp; Settings</span>
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
                                    This page explains all configuration files and settings that control AppEnd behavior, from backend server options to client-side application structure.
                                </p>
                                <ul class="mb-0">
                                    <li>Understand the <span class="fw-bold">appsettings.json</span> structure.</li>
                                    <li>Know the purpose of each configuration key.</li>
                                    <li>Configure the client-side <span class="fw-bold">app.json</span> (navigation, translation, calendar).</li>
                                </ul>
                            </section>

                            <section id="appsettings" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-file-code me-1 text-info"></i>
                                    <span class="fw-bold">appsettings.json — the AppEnd section</span>
                                </div>
                                <p class="mb-1">All AppEnd-specific settings live under the <span class="fw-bold">"AppEnd"</span> key:</p>
                                <pre class="mb-2">{
  "AppEnd": {
    "TalkPoint": "talk-to-me",
    "Secret": "YourSecretKey",
    "DefaultDbConfName": "DefaultRepo",
    "IsDevelopment": true,
    "EnableFileLogging": true,
    "LogsPath": "log",
    "LogLevel": "Information",
    "DefaultSuccessLoggerMethod": "",
    "DefaultErrorLoggerMethod": ""
  }
}</pre>
                                <ul class="mb-0">
                                    <li><span class="fw-bold">TalkPoint</span> — the RPC endpoint path (default: <span class="text-success">"talk-to-me"</span>).</li>
                                    <li><span class="fw-bold">Secret</span> — used for JWT token encoding/decoding.</li>
                                    <li><span class="fw-bold">DefaultDbConfName</span> — default database configuration name.</li>
                                    <li><span class="fw-bold">IsDevelopment</span> — enables development mode features.</li>
                                    <li><span class="fw-bold">EnableFileLogging</span> — enables file-based logging.</li>
                                    <li><span class="fw-bold">LogsPath</span> — directory for log files.</li>
                                </ul>
                            </section>

                            <section id="aaa" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-shield-halved me-1 text-danger"></i>
                                    <span class="fw-bold">AAA section (Authentication, Authorization, Accounting)</span>
                                </div>
                                <p class="mb-1">Security-related settings are nested under <span class="fw-bold">AppEnd.AAA</span>:</p>
                                <pre class="mb-2">{
  "AppEnd": {
    "AAA": {
      "LoginDbConfName": "DefaultRepo",
      "PublicKeyUser": "admin",
      "PublicKeyRole": "admin",
      "PublicMethods": [
        "Zzz.AppEndProxy.Login",
        "Zzz.AppEndProxy.Ping"
      ]
    }
  }
}</pre>
                                <ul class="mb-0">
                                    <li><span class="fw-bold">LoginDbConfName</span> — database used for authentication queries.</li>
                                    <li><span class="fw-bold">PublicKeyUser</span> — super-admin username (bypasses all access checks).</li>
                                    <li><span class="fw-bold">PublicKeyRole</span> — super-admin role name.</li>
                                    <li><span class="fw-bold">PublicMethods</span> — methods callable without authentication.</li>
                                </ul>
                            </section>

                            <section id="dbservers" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-database me-1 text-primary"></i>
                                    <span class="fw-bold">DbServers configuration</span>
                                </div>
                                <p class="mb-1">Database connections are defined in <span class="fw-bold">AppEnd.DbServers</span>:</p>
                                <pre class="mb-0">{
  "AppEnd": {
    "DbServers": [
      {
        "Name": "DefaultRepo",
        "ConnectionString": "Server=.;Database=MyDb;Trusted_Connection=True;TrustServerCertificate=True;"
      }
    ]
  }
}</pre>
                            </section>

                            <section id="llm" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-robot me-1 text-success"></i>
                                    <span class="fw-bold">LLM Providers</span>
                                </div>
                                <p class="mb-1">AI/LLM providers are configured in <span class="fw-bold">AppEnd.LLMProviders</span>:</p>
                                <pre class="mb-2">{
  "AppEnd": {
    "LLMProviders": [
      {
        "Name": "OpenAI",
        "ApiBaseUrl": "https://api.openai.com/v1",
        "ApiKey": "sk-...",
        "Models": ["gpt-4", "gpt-3.5-turbo"]
      },
      {
        "Name": "Ollama",
        "ApiBaseUrl": "http://localhost:11434",
        "ApiKey": "",
        "Models": ["llama3", "codellama"]
      }
    ]
  }
}</pre>
                                <p class="mb-0">Supported provider types: <span class="text-success fw-bold">OpenAI</span>, <span class="text-info fw-bold">Gemini</span> (Vertex AI &amp; Direct), <span class="text-warning fw-bold">Ollama</span>, and other OpenAI-compatible APIs.</p>
                            </section>

                            <section id="scheduler" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-clock me-1 text-warning"></i>
                                    <span class="fw-bold">ScheduledTasks in settings</span>
                                </div>
                                <p class="mb-1">Background tasks are persisted in <span class="fw-bold">AppEnd.ScheduledTasks</span>:</p>
                                <pre class="mb-0">{
  "AppEnd": {
    "ScheduledTasks": [
      {
        "TaskId": "abc123",
        "Name": "Cleanup Old Logs",
        "Enabled": true,
        "CronExpression": "0 3 * * *",
        "MethodFullName": "DefaultRepo.Maintenance.CleanOldLogs"
      }
    ]
  }
}</pre>
                            </section>

                            <section id="environments" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-code-branch me-1 text-info"></i>
                                    <span class="fw-bold">Environment-specific settings</span>
                                </div>
                                <p class="mb-1">AppEnd supports environment-based configuration:</p>
                                <ul class="mb-0">
                                    <li><span class="fw-bold">appsettings.json</span> — base settings (always loaded).</li>
                                    <li><span class="fw-bold">appsettings.Development.json</span> — loaded when <span class="fw-bold">ASPNETCORE_ENVIRONMENT=Development</span>.</li>
                                    <li>In development mode, the dev settings file is used <span class="text-danger fw-bold">entirely</span> (no merge with base) to avoid leaking base-only sections.</li>
                                </ul>
                            </section>

                            <section id="paths" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-folder-tree me-1 text-primary"></i>
                                    <span class="fw-bold">Key workspace paths</span>
                                </div>
                                <p class="mb-1">Important paths used by <span class="fw-bold">AppEndSettings</span>:</p>
                                <pre class="mb-0">WorkspacePath        = "workspace"
ServerObjectsPath    = "workspace/server"
ClientObjectsPath    = "workspace/client"
ApiCallsPath         = "workspace/apicalls"
SqlQueriesPath       = "workspace/sqlqueries"
AppEndPackagesPath   = "workspace/appendpackages"</pre>
                            </section>

                            <section id="appjson" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-palette me-1 text-success"></i>
                                    <span class="fw-bold">Client-side app.json</span>
                                </div>
                                <p class="mb-1">Each client application has an <span class="fw-bold">app.json</span> that controls its appearance and navigation:</p>
                                <pre class="mb-2">{
  "title": "AppEnd",
  "sub-title": "Studio",
  "dir": "ltr",
  "lang": "En",
  "calendar": "Gregorian",
  "defaultComponent": "components/BaseHome",
  "translation": {
    "Save": "Save Changes",
    "Cancel": "Cancel"
  },
  "navigation": [
    {
      "title": "Server",
      "icon": "fa-solid fa-fw fa-server",
      "items": [
        {
          "title": "DbObjects",
          "icon": "fa-solid fa-fw fa-database",
          "component": "components/DbDbObjects"
        }
      ]
    }
  ]
}</pre>
                                <ul class="mb-0">
                                    <li><span class="fw-bold">dir</span> — layout direction: <span class="text-success">"ltr"</span> or <span class="text-success">"rtl"</span>.</li>
                                    <li><span class="fw-bold">lang</span> — language code (<span class="text-success">"En"</span>, <span class="text-success">"Fa"</span>, etc.).</li>
                                    <li><span class="fw-bold">calendar</span> — calendar type: <span class="text-success">"Gregorian"</span>, <span class="text-success">"Jalali"</span>, or <span class="text-success">"Hijri"</span>.</li>
                                    <li><span class="fw-bold">translation</span> — key-value pairs for UI string localization.</li>
                                    <li><span class="fw-bold">navigation</span> — menu groups and items (supports <span class="fw-bold">roles</span> and <span class="fw-bold">actions</span> filtering).</li>
                                </ul>
                            </section>

                            <section id="reserved" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-folder-closed me-1 text-danger"></i>
                                    <span class="fw-bold">Reserved folders</span>
                                </div>
                                <p class="mb-1">These folders are reserved by the framework and should not be renamed:</p>
                                <pre class="mb-0">a..lib            — core libraries (JS, CSS, Bootstrap, Font Awesome, Ace editor)
a..templates      — Razor/Vue templates for code generation
AppEndStudio      — the Studio application
a.Components      — auto-generated CRUD components
a.SharedComponents — shared reusable components
a.Layouts         — layout templates (Application, Clean)</pre>
                            </section>

                            <section id="bestpractices" class="dev-guide-section">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-circle-check me-1 text-success"></i>
                                    <span class="fw-bold">Best practices</span>
                                </div>
                                <ul class="mb-0">
                                    <li>Use <span class="fw-bold">appsettings.Development.json</span> for local overrides — never commit secrets.</li>
                                    <li>Keep <span class="fw-bold">translation</span> keys consistent across all app.json files.</li>
                                    <li>Always set a unique <span class="fw-bold">Secret</span> for production deployments.</li>
                                    <li>Review <span class="fw-bold">PublicMethods</span> carefully — fewer is better.</li>
                                </ul>
                            </section>
                        </div>
                    </div>
                    <div class="col-48 col-lg-12 order-1 order-lg-2 mb-2 mb-lg-0">
                        <nav id="devGuideNavConfig" class="dev-guide-nav nav flex-column p-2">
                            <div class="fw-bold text-uppercase text-muted fs-d9 mb-2">On this page</div>
                            <a class="nav-link dev-guide-link" href="#purpose" @click.prevent="scrollTo('purpose')">Purpose</a>
                            <a class="nav-link dev-guide-link" href="#appsettings" @click.prevent="scrollTo('appsettings')">appsettings.json</a>
                            <a class="nav-link dev-guide-link" href="#aaa" @click.prevent="scrollTo('aaa')">AAA section</a>
                            <a class="nav-link dev-guide-link" href="#dbservers" @click.prevent="scrollTo('dbservers')">DbServers</a>
                            <a class="nav-link dev-guide-link" href="#llm" @click.prevent="scrollTo('llm')">LLM Providers</a>
                            <a class="nav-link dev-guide-link" href="#scheduler" @click.prevent="scrollTo('scheduler')">ScheduledTasks</a>
                            <a class="nav-link dev-guide-link" href="#environments" @click.prevent="scrollTo('environments')">Environments</a>
                            <a class="nav-link dev-guide-link" href="#paths" @click.prevent="scrollTo('paths')">Workspace paths</a>
                            <a class="nav-link dev-guide-link" href="#appjson" @click.prevent="scrollTo('appjson')">app.json</a>
                            <a class="nav-link dev-guide-link" href="#reserved" @click.prevent="scrollTo('reserved')">Reserved folders</a>
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
                    target: "#devGuideNavConfig",
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
