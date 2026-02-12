<template>
    <div class="card h-100 bg-transparent rounded-0 border-0 dev-guide">
        <div class="card-header p-2 bg-body-subtle rounded-0 border-0">
            <div class="hstack gap-2">
                <div class="fw-bold text-primary">
                    <i class="fa-solid fa-memory me-1"></i>
                    <span>Caching</span>
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
                                    This page documents the caching architecture used in AppEnd — covering server-side caching with <span class="fw-bold">AppEndCache</span>, method-level cache policies, and client-side RPC caching.
                                </p>
                                <ul class="mb-0">
                                    <li>Understand how the server cache stores and evicts data.</li>
                                    <li>Configure <span class="fw-bold">CachePolicy</span> for server methods.</li>
                                    <li>Use client-side <span class="fw-bold">cacheKey</span> / <span class="fw-bold">cacheTime</span> for RPC calls.</li>
                                </ul>
                            </section>

                            <section id="overview" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-diagram-project me-1 text-info"></i>
                                    <span class="fw-bold">Caching overview</span>
                                </div>
                                <p class="mb-1">AppEnd has <span class="text-primary fw-bold">two caching layers</span> that work independently:</p>
                                <ul class="mb-0">
                                    <li><span class="text-success fw-bold">Server-side</span> — <span class="fw-bold">AppEndCache</span> backed by <a href="https://github.com/ZiggyCreatures/FusionCache" target="_blank" rel="noopener">FusionCache</a>. Stores method results in memory, keyed by method + inputs + user.</li>
                                    <li><span class="text-warning fw-bold">Client-side</span> — <span class="fw-bold">sessionStorage</span>-based. RPC responses are cached by a hash of the request, controlled via <span class="fw-bold">cacheKey</span> and <span class="fw-bold">cacheTime</span>.</li>
                                </ul>
                            </section>

                            <section id="server-api" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-server me-1 text-primary"></i>
                                    <span class="fw-bold">Server cache API (AppEndCache)</span>
                                </div>
                                <p class="mb-1">The <span class="fw-bold">AppEndCache</span> static class provides these core operations:</p>
                                <pre class="mb-2">// Store a value with expiration
AppEndCache.Set&lt;T&gt;(string key, T value, int durationSeconds);

// Try to get a cached value
bool found = AppEndCache.TryGet&lt;T&gt;(string key, out T? value);

// Get or compute and cache
T result = AppEndCache.GetOrSet&lt;T&gt;(string key, Func&lt;T&gt; factory, int durationSeconds);

// Remove a specific entry
AppEndCache.Remove(string key);

// Clear all cache entries
AppEndCache.Clear();</pre>
                                <div class="fw-bold mb-1">Querying the cache</div>
                                <pre class="mb-0">// Get all cache keys
ICollection&lt;string&gt; keys = AppEndCache.GetKeys();

// Search keys containing a phrase
List&lt;string&gt; filtered = AppEndCache.GetKeysContaining("BaseUsers");

// Get cache statistics
AppEndCacheState state = AppEndCache.GetState("search");
// → { CurrentEntryCount, TotalHits, TotalMisses, CachedKeys }</pre>
                            </section>

                            <section id="cache-policy" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-shield-halved me-1 text-warning"></i>
                                    <span class="fw-bold">Method-level CachePolicy (.settings.json)</span>
                                </div>
                                <p class="mb-1">Each server method can have a <span class="fw-bold">CachePolicy</span> in its <span class="fw-bold">.settings.json</span> file:</p>
                                <pre class="mb-2">{
  "DefaultRepo.BaseInfo.ReadList": {
    "AccessRules": { "AllowedRoles": ["*"] },
    "CachePolicy": {
      "CacheLevel": "AllUsers",
      "AbsoluteExpirationSeconds": 300
    },
    "LogPolicy": "TrimInputs"
  }
}</pre>
                                <div class="fw-bold mb-1">CacheLevel enum</div>
                                <ul class="mb-0">
                                    <li><span class="text-secondary fw-bold">None</span> — no server caching (default).</li>
                                    <li><span class="text-info fw-bold">PerUser</span> — each user gets a separate cache entry (key includes user ID).</li>
                                    <li><span class="text-success fw-bold">AllUsers</span> — a single shared cache entry for all users.</li>
                                </ul>
                            </section>

                            <section id="cache-key-calc" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-key me-1 text-success"></i>
                                    <span class="fw-bold">How server cache keys are calculated</span>
                                </div>
                                <p class="mb-1">The cache key is computed from the method info, settings, input parameters, and optionally the user:</p>
                                <ul class="mb-0">
                                    <li><span class="fw-bold">AllUsers</span> → key = <span class="text-success">method name + serialized inputs</span>.</li>
                                    <li><span class="fw-bold">PerUser</span> → key = <span class="text-info">method name + serialized inputs + user ID</span>.</li>
                                    <li>Long-running methods also cache their final result using the same key.</li>
                                </ul>
                            </section>

                            <section id="cache-services" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-toolbox me-1 text-danger"></i>
                                    <span class="fw-bold">CacheServices (management API)</span>
                                </div>
                                <p class="mb-1">The <span class="fw-bold">CacheServices</span> class exposes management operations (used by the Cache UI in Studio):</p>
                                <pre class="mb-0">// Get cache state (filterable)
CacheServices.GetCacheItems("searchPhrase")

// Get a specific cached value
CacheServices.GetCacheItem("cacheKey")

// Remove a specific entry
CacheServices.RemoveCacheItem("cacheKey")

// Clear all cache
CacheServices.RemoveAllCacheItems()

// Clear all cache entries for a specific user
CacheServices.ClearActorCacheEntries(Actor)</pre>
                            </section>

                            <section id="user-context" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-user me-1 text-info"></i>
                                    <span class="fw-bold">User context caching</span>
                                </div>
                                <p class="mb-1">The user context (roles, permissions) is cached server-side for <span class="fw-bold">600 seconds</span>:</p>
                                <pre class="mb-2">// Cache key format
"U::Context,{UserId},{UserName}"

// Example
"U::Context,5,admin"</pre>
                                <ul class="mb-0">
                                    <li>Built by <span class="fw-bold">ActorServices.GetActor()</span> on first request.</li>
                                    <li>Cleared when <span class="fw-bold">ClearActorCacheEntries()</span> is called (e.g., role changes).</li>
                                    <li>Expired entries are automatically rebuilt on next request.</li>
                                </ul>
                            </section>

                            <section id="client-cache" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-desktop me-1 text-warning"></i>
                                    <span class="fw-bold">Client-side caching (RPC)</span>
                                </div>
                                <p class="mb-1">Add <span class="fw-bold">cacheKey</span> and <span class="fw-bold">cacheTime</span> to any RPC request to enable browser-session caching:</p>
                                <pre class="mb-2">rpc({
  requests: [
    {
      Id: "r1",
      Method: "DefaultRepo.BaseInfo.ReadList",
      Inputs: {
        ClientQueryJE: {
          QueryFullName: "DefaultRepo.BaseInfo.ReadList",
          Pagination: { PageNumber: 1, PageSize: 100 }
        }
      },
      cacheKey: "baseinfo-list-p1",
      cacheTime: 60
    }
  ],
  onDone: function(responses) {
    console.log(responses[0].Result);
  }
});</pre>
                                <div class="fw-bold mb-1">How it works</div>
                                <ul class="mb-0">
                                    <li><span class="text-success fw-bold">1.</span> Before sending, <span class="fw-bold">analyzeRequests()</span> checks <span class="fw-bold">sessionStorage</span> for a cached response.</li>
                                    <li><span class="text-success fw-bold">2.</span> If found, the request is skipped — the cached response is returned directly.</li>
                                    <li><span class="text-success fw-bold">3.</span> If not found, the request goes to the server.</li>
                                    <li><span class="text-success fw-bold">4.</span> On success, <span class="fw-bold">cacheResponses()</span> saves the response in <span class="fw-bold">sessionStorage</span> for <span class="fw-bold">cacheTime</span> seconds.</li>
                                    <li><span class="text-warning fw-bold">5.</span> Cache is cleared when the browser tab/session ends.</li>
                                </ul>
                            </section>

                            <section id="cache-state" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-chart-bar me-1 text-success"></i>
                                    <span class="fw-bold">Cache statistics (AppEndCacheState)</span>
                                </div>
                                <p class="mb-1">The cache tracks hits, misses, and active keys:</p>
                                <pre class="mb-0">public class AppEndCacheState
{
    public long CurrentEntryCount { get; set; }
    public long TotalMisses { get; set; }
    public long TotalHits { get; set; }
    public List&lt;string&gt; CachedKeys { get; set; }
}</pre>
                            </section>

                            <section id="bestpractices" class="dev-guide-section">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-circle-check me-1 text-success"></i>
                                    <span class="fw-bold">Best practices</span>
                                </div>
                                <ul class="mb-0">
                                    <li>Use <span class="fw-bold">AllUsers</span> cache for data that is the same for everyone (e.g., BaseInfo lookups).</li>
                                    <li>Use <span class="fw-bold">PerUser</span> cache for user-specific results (e.g., filtered dashboards).</li>
                                    <li>Keep <span class="fw-bold">AbsoluteExpirationSeconds</span> short for frequently changing data.</li>
                                    <li>Use client-side <span class="fw-bold">cacheTime</span> for dropdown/enum data that rarely changes within a session.</li>
                                    <li>Monitor <span class="fw-bold">TotalHits</span> vs <span class="fw-bold">TotalMisses</span> via the Cache Management UI to tune expiration times.</li>
                                    <li>Call <span class="fw-bold">ClearActorCacheEntries()</span> after changing user roles or permissions.</li>
                                </ul>
                            </section>
                        </div>
                    </div>
                    <div class="col-48 col-lg-12 order-1 order-lg-2 mb-2 mb-lg-0">
                        <nav id="devGuideNavCache" class="dev-guide-nav nav flex-column p-2">
                            <div class="fw-bold text-uppercase text-muted fs-d9 mb-2">On this page</div>
                            <a class="nav-link dev-guide-link" href="#purpose" @click.prevent="scrollTo('purpose')">Purpose</a>
                            <a class="nav-link dev-guide-link" href="#overview" @click.prevent="scrollTo('overview')">Overview</a>
                            <a class="nav-link dev-guide-link" href="#server-api" @click.prevent="scrollTo('server-api')">Server cache API</a>
                            <a class="nav-link dev-guide-link" href="#cache-policy" @click.prevent="scrollTo('cache-policy')">CachePolicy</a>
                            <a class="nav-link dev-guide-link" href="#cache-key-calc" @click.prevent="scrollTo('cache-key-calc')">Cache key calc</a>
                            <a class="nav-link dev-guide-link" href="#cache-services" @click.prevent="scrollTo('cache-services')">CacheServices</a>
                            <a class="nav-link dev-guide-link" href="#user-context" @click.prevent="scrollTo('user-context')">User context</a>
                            <a class="nav-link dev-guide-link" href="#client-cache" @click.prevent="scrollTo('client-cache')">Client cache</a>
                            <a class="nav-link dev-guide-link" href="#cache-state" @click.prevent="scrollTo('cache-state')">Statistics</a>
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
                    target: "#devGuideNavCache",
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
