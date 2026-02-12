<template>
    <div class="card h-100 bg-transparent rounded-0 border-0 dev-guide">
        <div class="card-header p-2 bg-body-subtle rounded-0 border-0">
            <div class="hstack gap-2">
                <div class="fw-bold text-primary">
                    <i class="fa-solid fa-shield-halved me-1"></i>
                    <span>Authentication &amp; Security</span>
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
                                    This page covers the complete authentication and authorization model used in AppEnd, including JWT tokens, role-based access, and method-level security settings.
                                </p>
                                <ul class="mb-0">
                                    <li>Understand the login/logout flow and token management.</li>
                                    <li>Configure <span class="fw-bold">AccessRules</span> for server methods.</li>
                                    <li>Use client-side helpers to check roles, permissions, and user context.</li>
                                </ul>
                            </section>

                            <section id="overview" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-diagram-project me-1 text-info"></i>
                                    <span class="fw-bold">Authentication overview</span>
                                </div>
                                <p class="mb-1">AppEnd uses <span class="fw-bold">JWT tokens</span> for authentication. The flow is:</p>
                                <ul class="mb-0">
                                    <li><span class="text-success fw-bold">1.</span> Client calls <span class="fw-bold">Zzz.AppEndProxy.Login</span> with credentials.</li>
                                    <li><span class="text-success fw-bold">2.</span> Server validates and returns a signed JWT token.</li>
                                    <li><span class="text-success fw-bold">3.</span> Token is stored in <span class="fw-bold">localStorage</span> (remember me) or <span class="fw-bold">sessionStorage</span>.</li>
                                    <li><span class="text-success fw-bold">4.</span> Every RPC request sends the token via the <span class="fw-bold">token</span> HTTP header.</li>
                                    <li><span class="text-success fw-bold">5.</span> Server decodes the token using <span class="fw-bold">AppEndSettings.Secret</span> and builds the <span class="fw-bold">AppEndUser</span> object.</li>
                                </ul>
                            </section>

                            <section id="appenduser" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-user-gear me-1 text-warning"></i>
                                    <span class="fw-bold">AppEndUser model</span>
                                </div>
                                <p class="mb-1">Every request is associated with an <span class="fw-bold">AppEndUser</span> object built by <span class="fw-bold">ActorServices.GetActor()</span>.</p>
                                <pre class="mb-2">public class AppEndUser
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string[] Roles { get; set; }
    public string[] RoleNames { get; set; }
    public Hashtable? ContextInfo { get; set; }
}</pre>
                                <ul class="mb-0">
                                    <li><span class="fw-bold">ContextInfo</span> is populated by <span class="fw-bold">Zzz.AppEndProxy.CreateUserServerContext</span> and cached for 600 seconds.</li>
                                    <li>If no valid token is provided, the user is <span class="fw-bold">nobody</span> with <span class="fw-bold">Id = -1</span>.</li>
                                </ul>
                            </section>

                            <section id="login" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-right-to-bracket me-1 text-success"></i>
                                    <span class="fw-bold">Login / Logout (client-side)</span>
                                </div>
                                <div class="fw-bold mb-1">Login</div>
                                <pre class="mb-2">// Using the global helper
let success = login({
    UserName: "admin",
    Password: "mypass",
    RememberMe: true
});
// Returns true if login succeeded</pre>
                                <div class="fw-bold mb-1">Logout</div>
                                <pre class="mb-2">logout(function() {
    // Redirect after logout
    window.location.href = "/";
});</pre>
                                <div class="fw-bold mb-1">Login as another user (admin only)</div>
                                <pre class="mb-0">loginAs("targetUserName");</pre>
                            </section>

                            <section id="token-helpers" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-key me-1 text-primary"></i>
                                    <span class="fw-bold">Token &amp; user helpers</span>
                                </div>
                                <p class="mb-1">Available globally from <span class="fw-bold">append-auth.js</span>:</p>
                                <pre class="mb-2">// Check if user is logged in
isLogedIn()              // → true / false

// Get JWT token string
getUserToken()           // → "eyJhbGciOiJ..."

// Get decoded user object from token
getUserObject()          // → { Id, UserName, Roles, ... }

// Get user roles array
getUserRoles()           // → ["admin", "editor"]

// Get full user context (from server)
getLogedInUserContext()  // → { AllowedActions, DeniedActions, HasPublicKeyRole, Settings, ... }</pre>
                                <div class="fw-bold mb-1">Decode a JWT token manually</div>
                                <pre class="mb-0">let decoded = decodeJwt(getUserToken());
// → { header, payload, signature }</pre>
                            </section>

                            <section id="role-checks" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-user-check me-1 text-info"></i>
                                    <span class="fw-bold">Role &amp; permission checks (client)</span>
                                </div>
                                <pre class="mb-2">// Check if user has any of the specified roles
isInRole(["admin", "editor"])   // → true / false

// Check if user is explicitly allowed
isAllowed(["john", "jane"])     // → true / false

// Check if user is denied
isDenied(["blockedUser"])       // → true / false

// Check if user is admin (public key user or public key role)
isAdmin()                       // → true / false

// Check specific flags
isPublicKey()                   // → true / false
hasPublicKeyRole()              // → true / false</pre>
                                <p class="mb-0">These functions are also available via <span class="fw-bold">shared.isInRole()</span>, <span class="fw-bold">shared.isAdmin()</span>, etc.</p>
                            </section>

                            <section id="access-rules" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-lock me-1 text-danger"></i>
                                    <span class="fw-bold">Server-side access rules (.settings.json)</span>
                                </div>
                                <p class="mb-1">Each server method has access rules defined in its <span class="fw-bold">.settings.json</span> file:</p>
                                <pre class="mb-2">{
  "DefaultRepo.BaseUsers.ReadList": {
    "AccessRules": {
      "AllowedRoles": ["admin", "manager"],
      "AllowedUsers": ["specificUser"],
      "DeniedUsers": ["blockedUser"]
    }
  }
}</pre>
                                <div class="fw-bold mb-1">Access resolution order</div>
                                <ul class="mb-0">
                                    <li><span class="text-success fw-bold">1.</span> If method is in <span class="fw-bold">PublicMethods</span> list → <span class="text-success">allowed</span>.</li>
                                    <li><span class="text-success fw-bold">2.</span> If user is the <span class="fw-bold">PublicKeyUser</span> → <span class="text-success">allowed</span>.</li>
                                    <li><span class="text-success fw-bold">3.</span> If user has the <span class="fw-bold">PublicKeyRole</span> → <span class="text-success">allowed</span>.</li>
                                    <li><span class="text-success fw-bold">4.</span> If user's roles intersect with <span class="fw-bold">AllowedRoles</span> → <span class="text-success">allowed</span>.</li>
                                    <li><span class="text-success fw-bold">5.</span> If <span class="fw-bold">AllowedRoles</span> contains <span class="fw-bold">"*"</span> → <span class="text-success">allowed</span>.</li>
                                    <li><span class="text-success fw-bold">6.</span> If user is in <span class="fw-bold">AllowedUsers</span> → <span class="text-success">allowed</span>.</li>
                                    <li><span class="text-danger fw-bold">7.</span> Otherwise → <span class="text-danger">denied</span>.</li>
                                </ul>
                            </section>

                            <section id="nav-security" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-bars me-1 text-warning"></i>
                                    <span class="fw-bold">Navigation security (app.json)</span>
                                </div>
                                <p class="mb-1">Navigation items in <span class="fw-bold">app.json</span> can be restricted using <span class="fw-bold">roles</span> and <span class="fw-bold">actions</span>:</p>
                                <pre class="mb-2">{
  "title": "Admin Panel",
  "icon": "fa-solid fa-gear",
  "component": "components/AdminPanel",
  "roles": ["admin"],
  "actions": ["Zzz.AppEndProxy.SomeMethod"]
}</pre>
                                <p class="mb-0">The <span class="fw-bold">trimNav()</span> function automatically hides menu items the current user cannot access.</p>
                            </section>

                            <section id="publickey" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-crown me-1 text-warning"></i>
                                    <span class="fw-bold">PublicKey system</span>
                                </div>
                                <p class="mb-1">AppEnd has two super-admin concepts configured in <span class="fw-bold">appsettings.json</span>:</p>
                                <ul class="mb-0">
                                    <li><span class="fw-bold">PublicKeyUser</span> — a specific username that bypasses all access checks.</li>
                                    <li><span class="fw-bold">PublicKeyRole</span> — a role name that grants full access to any user holding it.</li>
                                    <li><span class="fw-bold">PublicMethods</span> — an array of method names accessible without authentication.</li>
                                </ul>
                                <pre class="mb-0">{
  "AppEnd": {
    "AAA": {
      "PublicKeyUser": "admin",
      "PublicKeyRole": "admin",
      "PublicMethods": [
        "Zzz.AppEndProxy.Login",
        "Zzz.AppEndProxy.Ping"
      ]
    }
  }
}</pre>
                            </section>

                            <section id="bestpractices" class="dev-guide-section">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-circle-check me-1 text-success"></i>
                                    <span class="fw-bold">Best practices</span>
                                </div>
                                <ul class="mb-0">
                                    <li>Always set <span class="fw-bold">AccessRules</span> for every server method — default is deny-all.</li>
                                    <li>Keep <span class="fw-bold">PublicMethods</span> to the absolute minimum (login, ping, etc.).</li>
                                    <li>Use <span class="fw-bold">isInRole()</span> in Vue components to show/hide UI elements.</li>
                                    <li>Change the <span class="fw-bold">Secret</span> key in production environments.</li>
                                    <li>Use <span class="fw-bold">roles</span> on nav items to prevent leaking admin screens.</li>
                                </ul>
                            </section>
                        </div>
                    </div>
                    <div class="col-48 col-lg-12 order-1 order-lg-2 mb-2 mb-lg-0">
                        <nav id="devGuideNavAuth" class="dev-guide-nav nav flex-column p-2">
                            <div class="fw-bold text-uppercase text-muted fs-d9 mb-2">On this page</div>
                            <a class="nav-link dev-guide-link" href="#purpose" @click.prevent="scrollTo('purpose')">Purpose</a>
                            <a class="nav-link dev-guide-link" href="#overview" @click.prevent="scrollTo('overview')">Overview</a>
                            <a class="nav-link dev-guide-link" href="#appenduser" @click.prevent="scrollTo('appenduser')">AppEndUser</a>
                            <a class="nav-link dev-guide-link" href="#login" @click.prevent="scrollTo('login')">Login / Logout</a>
                            <a class="nav-link dev-guide-link" href="#token-helpers" @click.prevent="scrollTo('token-helpers')">Token helpers</a>
                            <a class="nav-link dev-guide-link" href="#role-checks" @click.prevent="scrollTo('role-checks')">Role checks</a>
                            <a class="nav-link dev-guide-link" href="#access-rules" @click.prevent="scrollTo('access-rules')">Access rules</a>
                            <a class="nav-link dev-guide-link" href="#nav-security" @click.prevent="scrollTo('nav-security')">Nav security</a>
                            <a class="nav-link dev-guide-link" href="#publickey" @click.prevent="scrollTo('publickey')">PublicKey system</a>
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
                    target: "#devGuideNavAuth",
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
