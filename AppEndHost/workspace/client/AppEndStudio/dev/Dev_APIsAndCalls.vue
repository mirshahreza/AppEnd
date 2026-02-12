<template>
    <div class="card h-100 bg-transparent rounded-0 border-0 dev-guide">
        <div class="card-header p-2 bg-body-subtle rounded-0 border-0">
            <div class="hstack gap-2">
                <div class="fw-bold text-primary">
                    <i class="fa-solid fa-plug me-1"></i>
                    <span>APIs & Calls</span>
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
                                    This page is a complete guide for building and calling AppEnd server functions, from file locations and method definitions to access settings, RPC calls, and response handling.
                                </p>
                                <ul class="mb-0">
                                    <li>Know exactly where methods are defined and how full names are formed.</li>
                                    <li>See real RPC request/response formats.</li>
                                    <li>Learn async/sync and batch calls with multiple examples.</li>
                                </ul>
                            </section>

                            <section id="paths" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-folder-tree me-1 text-primary"></i>
                                    <span class="fw-bold">Core paths and building blocks</span>
                                </div>
                                <ul class="mb-0">
                                    <li>Server files live under <span class="fw-bold">AppEndHost/workspace/server</span>.</li>
                                    <li>Each C# class has a <span class="fw-bold">.settings.json</span> file for access/log/cache rules.</li>
                                    <li>RPC method names always follow <span class="fw-bold">Namespace.Class.Method</span>.</li>
                                </ul>
                                <div class="mt-2">
                                    <div class="fw-bold mb-1">Sample paths</div>
                                    <pre class="mb-2">AppEndHost/workspace/server/DefaultRepo.BaseUsers.cs
AppEndHost/workspace/server/DefaultRepo.BaseUsers.settings.json</pre>
                                    <div class="fw-bold mb-1">Sample full method names</div>
                                    <pre class="mb-0">DefaultRepo.BaseUsers.ReadList
DefaultRepo.BaseUsers.Create
MyNamespace.ReportService.BuildMonthlyReport</pre>
                                </div>
                            </section>

                            <section id="types" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-sitemap me-1 text-primary"></i>
                                    <span class="fw-bold">Server function types</span>
                                </div>
                                <ul class="mb-0">
                                    <li>DbIO generated CRUD methods: <span class="fw-bold">ReadList</span>, <span class="fw-bold">ReadByKey</span>, <span class="fw-bold">Create</span>, <span class="fw-bold">UpdateByKey</span>, <span class="fw-bold">DeleteByKey</span>.</li>
                                    <li>Custom methods: <span class="fw-bold">static</span> C# methods (namespace + class + method).</li>
                                    <li>Long-running methods: prefixed with <span class="fw-bold">__LR_*</span> and use <span class="fw-bold">TaskToken</span>.</li>
                                </ul>
                            </section>

                            <section id="dbio" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-database me-1 text-primary"></i>
                                    <span class="fw-bold">Building DbIO CRUD methods</span>
                                </div>
                                <p class="mb-1">In Studio, use <span class="fw-bold">Create Server Objects</span> for each table to generate the C# class.</p>
                                <div class="fw-bold mb-1">Generated method example</div>
                                <pre class="mb-2">namespace DefaultRepo
{
    public static class BaseUsers
    {
        public static object? ReadList(JsonElement ClientQueryJE, AppEndUser? Actor)
        {
            return ClientQuery.GetInstanceByQueryJson(ClientQueryJE, Actor?.ContextInfo).Exec();
        }
    }
}</pre>
                                <div class="fw-bold mb-1">ReadList Inputs example</div>
                                <pre class="mb-0">{
  "ClientQueryJE": {
    "QueryFullName": "DefaultRepo.BaseUsers.ReadList",
    "Pagination": { "PageNumber": 1, "PageSize": 20 },
    "OrderClauses": [{ "Name": "Id", "OrderDirection": "DESC" }],
    "Where": [{ "Field": "IsActive", "Op": "=", "Value": true }]
  }
}</pre>
                            </section>

                            <section id="customize" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-pen-to-square me-1 text-primary"></i>
                                    <span class="fw-bold">Customizing a DbIO method</span>
                                </div>
                                <p class="mb-1">You can inject filters or logic before executing the query.</p>
                                <pre class="mb-0">public static object? ReadList(JsonElement ClientQueryJE, AppEndUser? Actor)
{
    ClientQuery cq = ClientQuery.GetInstanceByQueryJson(ClientQueryJE, Actor?.ContextInfo);
    if (cq.Where == null) cq.Where = new Where();
    cq.Where.And("IsActive", CompareOperator.Equal, true);
    return cq.Exec();
}</pre>
                            </section>

                            <section id="custom" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-code me-1 text-primary"></i>
                                    <span class="fw-bold">Creating a custom method (non-DbIO)</span>
                                </div>
                                <ul class="mb-2">
                                    <li>Create a file in <span class="fw-bold">workspace/server</span>.</li>
                                    <li>The class and method must be <span class="fw-bold">static</span>.</li>
                                    <li>The RPC name is the full namespace + class + method.</li>
                                </ul>
                                <div class="fw-bold mb-1">Example</div>
                                <pre class="mb-0">// File: workspace/server/MyNamespace.ReportService.cs
namespace MyNamespace
{
    public static class ReportService
    {
        public static object? BuildMonthlyReport(AppEndUser? Actor, string Year, string Month)
        {
            return new { Year, Month, Status = "Ready" };
        }
    }
}</pre>
                            </section>

                            <section id="settings" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-lock me-1 text-primary"></i>
                                    <span class="fw-bold">Access, log, cache settings (.settings.json)</span>
                                </div>
                                <p class="mb-1">Each server class can define a <span class="fw-bold">.settings.json</span> file next to its C# file. The file contains per-method rules keyed by full method name.</p>
                                <pre class="mb-2">// File: workspace/server/DefaultRepo.BaseUsers.settings.json
{
  "DefaultRepo.BaseUsers.ReadList": {
    "AccessRules": {
      "AllowedRoles": ["admin", "manager"],
      "AllowedUsers": [],
      "DeniedUsers": []
    },
    "CachePolicy": {
      "CacheLevel": "PerUser",
      "AbsoluteExpirationSeconds": 120
    },
    "LogPolicy": "TrimInputs"
  }
}</pre>
                                <div class="fw-bold mb-1">AccessRules</div>
                                <ul class="mb-2">
                                    <li><span class="fw-bold">AllowedRoles</span> — any role in this list can call the method.</li>
                                    <li><span class="fw-bold">AllowedUsers</span> — specific usernames allowed (supports <span class="fw-bold">"*"</span>).</li>
                                    <li><span class="fw-bold">DeniedUsers</span> — explicit block list (evaluated after login).</li>
                                </ul>
                                <div class="fw-bold mb-1">CachePolicy</div>
                                <ul class="mb-2">
                                    <li><span class="fw-bold">CacheLevel</span> — <span class="fw-bold">None</span>, <span class="fw-bold">PerUser</span>, <span class="fw-bold">AllUsers</span>.</li>
                                    <li><span class="fw-bold">AbsoluteExpirationSeconds</span> — TTL in seconds.</li>
                                </ul>
                                <div class="fw-bold mb-1">LogPolicy</div>
                                <ul class="mb-2">
                                    <li><span class="fw-bold">IgnoreLogging</span> — skip logging.</li>
                                    <li><span class="fw-bold">TrimInputs</span> — log with trimmed inputs.</li>
                                    <li><span class="fw-bold">Full</span> — log full inputs and outputs.</li>
                                </ul>
                                <div class="fw-bold mb-1">LongRunningPolicy (optional)</div>
                                <pre class="mb-0">{
  "DefaultRepo.Reports.BuildBigReport": {
    "LongRunningPolicy": { "TimeoutSeconds": 600 },
    "LogPolicy": "Full"
  }
}</pre>
                            </section>

                            <section id="rpc" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-paper-plane me-1 text-primary"></i>
                                    <span class="fw-bold">RPC request format</span>
                                </div>
                                <ul class="mb-0">
                                    <li>RPC endpoint: <span class="fw-bold">AppEndSettings.TalkPoint</span> hosted by <span class="fw-bold">RpcNet</span>.</li>
                                    <li>Request fields: <span class="fw-bold">Id</span>, <span class="fw-bold">Method</span>, <span class="fw-bold">Inputs</span>, <span class="fw-bold">Lang</span>.</li>
                                    <li>Response fields: <span class="fw-bold">IsSucceeded</span>, <span class="fw-bold">Result</span>, <span class="fw-bold">Duration</span>, <span class="fw-bold">FromCache</span>.</li>
                                </ul>
                                <div class="mt-2">
                                    <div class="fw-bold mb-1">Sample request (single call)</div>
                                    <pre class="mb-0">[
  {
    "Id": "rq-1",
    "Method": "DefaultRepo.BaseUsers.ReadList",
    "Lang": "En",
    "Inputs": {
      "ClientQueryJE": {
        "QueryFullName": "DefaultRepo.BaseUsers.ReadList",
        "Pagination": { "PageNumber": 1, "PageSize": 20 }
      }
    }
  }
]</pre>
                                </div>
                            </section>

                            <section id="vue" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-code me-1 text-primary"></i>
                                    <span class="fw-bold">Calling from Vue components (real example)</span>
                                </div>
                                <p class="mb-1">Example taken from components calling a DbDirect method:</p>
                                <pre class="mb-0">rpc({
  requests: [{
    Method: "DefaultRepo.DbDirect.ZzCalculateHID",
    Inputs: {
      TableName: "BaseInfo",
      ParentId: fixNull(_this.c.row.ParentId, ""),
      ParentDigits: "3",
      ChildDigits: _this.c.DigitsCount.toString(),
      Delimiter: "."
    }
  }],
  onDone: function(res) {
    _this.c.row.Id = res[0].Result;
  }
});</pre>
                            </section>

                            <section id="client-calls" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-square-check me-1 text-primary"></i>
                                    <span class="fw-bold">Client call examples (rpc / rpcSync / rpcAEP)</span>
                                </div>
                                <div class="fw-bold mb-1">Async call with rpc</div>
                                <pre class="mb-2">rpc({
  requests: [
    {
      Id: "rq-1",
      Method: "MyNamespace.ReportService.BuildMonthlyReport",
      Lang: "En",
      Inputs: { Year: "2025", Month: "01" }
    }
  ],
  onDone: function(responses) { console.log(responses[0]); },
  onFail: function(err) { console.error(err); }
});</pre>
                                <div class="fw-bold mb-1">Sync call with rpcSync</div>
                                <pre class="mb-2">let responses = rpcSync({
  requests: [
    {
      Id: "rq-2",
      Method: "DefaultRepo.BaseUsers.ReadByKey",
      Lang: "En",
      Inputs: { Id: 12 }
    }
  ]
});</pre>
                                <div class="fw-bold mb-1">rpcAEP wrapper (Zzz.AppEndProxy)</div>
                                <pre class="mb-0">rpcAEP("Ping", { Payload: "Hello" },
  function(responses){ console.log(responses[0]); },
  function(err){ console.error(err); }
);</pre>
                            </section>

                            <section id="batch-cache" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-gears me-1 text-primary"></i>
                                    <span class="fw-bold">Batch, cache, and real scenarios</span>
                                </div>
                                <div class="fw-bold mb-1">Batch multiple requests</div>
                                <pre class="mb-2">rpc({
  requests: [
    { Id: "r1", Method: "DefaultRepo.BaseUsers.ReadList", Lang: "En", Inputs: { ClientQueryJE: { QueryFullName: "DefaultRepo.BaseUsers.ReadList" } } },
    { Id: "r2", Method: "DefaultRepo.BaseRoles.ReadList", Lang: "En", Inputs: { ClientQueryJE: { QueryFullName: "DefaultRepo.BaseRoles.ReadList" } } }
  ],
  onDone: function(responses) {
    let users = responses[0].Result;
    let roles = responses[1].Result;
  }
});</pre>
                                <div class="fw-bold mb-1">Client cache (cacheKey / cacheTime)</div>
                                <pre class="mb-0">rpc({
  requests: [
    {
      Id: "r3",
      Method: "DefaultRepo.BaseUsers.ReadList",
      Lang: "En",
      Inputs: { ClientQueryJE: { QueryFullName: "DefaultRepo.BaseUsers.ReadList" } },
      cacheKey: "users-readlist-p1",
      cacheTime: 60
    }
  ]
});</pre>
                            </section>

                            <section id="longrunning" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-hourglass-half me-1 text-primary"></i>
                                    <span class="fw-bold">Long-running method pattern</span>
                                </div>
                                <p class="mb-1">A long-running response includes <span class="fw-bold">TaskToken</span> and <span class="fw-bold">IsLongRunning</span>. The full lifecycle: start → poll → get result (or cancel).</p>

                                <div class="fw-bold mb-1">1 — Start the task</div>
                                <p class="mb-1">Call the method normally. If it's long-running, the response contains a <span class="fw-bold">TaskToken</span> instead of the final result.</p>
                                <pre class="mb-3">rpc({
  requests: [
    { Method: "DefaultRepo.Test.LongRunningDemo", Inputs: { Seconds: 10 } }
  ],
  onDone: function(responses) {
    let resp = responses[0];
    if (resp.IsLongRunning && resp.TaskToken) {
      // Task enqueued — store the token
      let token = resp.TaskToken;
      console.log("Task started, token:", token);
      // Start polling...
    } else if (resp.IsSucceeded) {
      // Completed synchronously (not long-running)
      console.log("Result:", resp.Result);
    }
  },
  silent: true
});</pre>

                                <div class="fw-bold mb-1">2 — Poll status</div>
                                <p class="mb-1">Use <span class="fw-bold">__LR_GetStatus</span> to check progress. The response contains <span class="fw-bold">Status</span>, <span class="fw-bold">DurationMs</span>, and <span class="fw-bold">Error</span>.</p>
                                <pre class="mb-3">rpc({
  requests: [
    { Method: "__LR_GetStatus", Inputs: { TaskToken: token } }
  ],
  onDone: function(responses) {
    let info = responses[0].Result;
    console.log("Status:", info.Status, "Duration:", info.DurationMs + "ms");
    // Status values: "Pending", "Running", "Completed", "Failed", "Cancelled"
  },
  silent: true
});</pre>

                                <div class="fw-bold mb-1">3 — Auto-polling with setInterval</div>
                                <p class="mb-1">Poll every 2 seconds until the task finishes.</p>
                                <pre class="mb-3">let pollingTimer = setInterval(function() {
  rpc({
    requests: [
      { Method: "__LR_GetStatus", Inputs: { TaskToken: token } }
    ],
    onDone: function(responses) {
      let info = responses[0].Result;
      if (info.Status === "Completed" || info.Status === "Failed" || info.Status === "Cancelled") {
        clearInterval(pollingTimer);
        console.log("Task finished:", info.Status, "(" + info.DurationMs + "ms)");
        if (info.Status === "Completed") {
          // Fetch the final result...
        }
      }
    },
    silent: true
  });
}, 2000);</pre>

                                <div class="fw-bold mb-1">4 — Get result</div>
                                <p class="mb-1">Once status is <span class="fw-bold">Completed</span>, use <span class="fw-bold">__LR_GetResult</span> to fetch the output.</p>
                                <pre class="mb-3">rpc({
  requests: [
    { Method: "__LR_GetResult", Inputs: { TaskToken: token } }
  ],
  onDone: function(responses) {
    let resp = responses[0];
    if (resp.IsSucceeded) {
      console.log("Final result:", resp.Result);
    }
  },
  silent: true
});</pre>

                                <div class="fw-bold mb-1">5 — Cancel a running task</div>
                                <p class="mb-1">Use <span class="fw-bold">__LR_Cancel</span> while the task is still <span class="fw-bold">Running</span> or <span class="fw-bold">Pending</span>.</p>
                                <pre class="mb-3">rpc({
  requests: [
    { Method: "__LR_Cancel", Inputs: { TaskToken: token } }
  ],
  onDone: function(responses) {
    if (responses[0].IsSucceeded && responses[0].Result === true) {
      console.log("Task cancelled.");
    }
  },
  silent: true
});</pre>

                                <div class="fw-bold mb-1">Status values</div>
                                <ul class="mb-2">
                                    <li><span class="fw-bold">Pending</span> — task is queued but not yet started.</li>
                                    <li><span class="fw-bold">Running</span> — task is currently executing.</li>
                                    <li><span class="fw-bold">Completed</span> — task finished successfully (use <span class="fw-bold">__LR_GetResult</span>).</li>
                                    <li><span class="fw-bold">Failed</span> — task encountered an error (check <span class="fw-bold">Error</span> field).</li>
                                    <li><span class="fw-bold">Cancelled</span> — task was cancelled via <span class="fw-bold">__LR_Cancel</span>.</li>
                                </ul>
                                <div class="dev-demo-panel">
                                    <div class="fw-bold mb-2"><i class="fa-solid fa-play me-1 text-success"></i> Live Demo</div>
                                    <div class="d-flex flex-wrap gap-2 align-items-end mb-2">
                                        <div>
                                            <label class="form-label fs-d9 text-muted mb-0">Duration (sec)</label>
                                            <input type="number" class="form-control form-control-sm" v-model.number="lr.seconds" min="1" max="300" style="width:90px;" />
                                        </div>
                                        <button class="btn btn-sm btn-success" @click="lrStart" :disabled="lr.status === 'Running' || lr.status === 'Pending'"><i class="fa-solid fa-play me-1"></i> Start</button>
                                        <button class="btn btn-sm btn-warning" @click="lrGetStatus" :disabled="!lr.taskToken"><i class="fa-solid fa-circle-info me-1"></i> Get Status</button>
                                        <button class="btn btn-sm btn-danger" @click="lrCancel" :disabled="!lr.taskToken || (lr.status !== 'Running' && lr.status !== 'Pending')"><i class="fa-solid fa-stop me-1"></i> Cancel</button>
                                        <button class="btn btn-sm btn-info text-white" @click="lrGetResult" :disabled="!lr.taskToken || (lr.status !== 'Completed' && lr.status !== 'Failed')"><i class="fa-solid fa-download me-1"></i> Get Result</button>
                                        <button class="btn btn-sm btn-secondary" @click="lrReset"><i class="fa-solid fa-rotate-left me-1"></i> Reset</button>
                                    </div>
                                    <div v-if="lr.taskToken">
                                        <table class="table table-sm table-bordered mb-0" style="font-size:0.82rem;">
                                            <tbody>
                                                <tr><td class="fw-bold text-muted" style="width:120px;">Token</td><td><code>{{ lr.taskToken }}</code></td></tr>
                                                <tr>
                                                    <td class="fw-bold text-muted">Status</td>
                                                    <td>
                                                        <span class="badge" :class="lrBadge">{{ lr.status || '-' }}</span>
                                                        <span v-if="lr.polling" class="ms-2 spinner-border spinner-border-sm text-primary"></span>
                                                    </td>
                                                </tr>
                                                <tr><td class="fw-bold text-muted">Duration</td><td>{{ lr.durationMs != null ? lr.durationMs + ' ms' : '-' }}</td></tr>
                                                <tr v-if="lr.error"><td class="fw-bold text-danger">Error</td><td class="text-danger">{{ lr.error }}</td></tr>
                                                <tr v-if="lr.result !== null"><td class="fw-bold text-muted">Result</td><td><pre class="mb-0 p-1 bg-light border rounded" style="font-size:0.78rem;max-height:150px;overflow:auto;">{{ JSON.stringify(lr.result, null, 2) }}</pre></td></tr>
                                            </tbody>
                                        </table>
                                    </div>
                                    <div class="mt-2">
                                        <label class="form-label fw-bold text-muted fs-d9 mb-1"><i class="fa-solid fa-list-ul me-1"></i> Event Log</label>
                                        <div class="border rounded p-2 bg-light" style="min-height:60px;max-height:140px;overflow-y:auto;font-size:0.76rem;font-family:monospace;">
                                            <div v-for="(entry, idx) in lr.log" :key="idx" :class="'text-' + entry.color"><span class="text-muted">{{ entry.time }}</span> — {{ entry.text }}</div>
                                            <div v-if="lr.log.length === 0" class="text-muted fst-italic">No events yet. Click Start to begin.</div>
                                        </div>
                                    </div>
                                </div>
                            </section>

                            <section id="errors" class="dev-guide-section">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-triangle-exclamation me-1 text-primary"></i>
                                    <span class="fw-bold">Error output and response checks</span>
                                </div>
                                <div class="fw-bold mb-1">Sample success/failure response</div>
                                <pre class="mb-0">{
  "Id": "rq-1",
  "IsSucceeded": false,
  "Duration": 12,
  "FromCache": false,
  "Result": {
    "Message": "Invalid input",
    "Data": { "Field": "Month" }
  }
}</pre>
                                <div class="mt-2">Check <span class="fw-bold">IsSucceeded</span> first, then read details from <span class="fw-bold">Result</span>.</div>
                            </section>
                        </div>
                    </div>
                    <div class="col-48 col-lg-12 order-1 order-lg-2 mb-2 mb-lg-0">
                        <nav id="devGuideNavApi" class="dev-guide-nav nav flex-column p-2">
                            <div class="fw-bold text-uppercase text-muted fs-d9 mb-2">On this page</div>
                            <a class="nav-link dev-guide-link" href="#purpose" @click.prevent="scrollTo('purpose')">Purpose</a>
                            <a class="nav-link dev-guide-link" href="#paths" @click.prevent="scrollTo('paths')">Core paths</a>
                            <a class="nav-link dev-guide-link" href="#types" @click.prevent="scrollTo('types')">Function types</a>
                            <a class="nav-link dev-guide-link" href="#dbio" @click.prevent="scrollTo('dbio')">DbIO CRUD</a>
                            <a class="nav-link dev-guide-link" href="#customize" @click.prevent="scrollTo('customize')">Customize DbIO</a>
                            <a class="nav-link dev-guide-link" href="#custom" @click.prevent="scrollTo('custom')">Custom methods</a>
                            <a class="nav-link dev-guide-link" href="#settings" @click.prevent="scrollTo('settings')">Settings</a>
                            <a class="nav-link dev-guide-link" href="#rpc" @click.prevent="scrollTo('rpc')">RPC format</a>
                            <a class="nav-link dev-guide-link" href="#vue" @click.prevent="scrollTo('vue')">Vue example</a>
                            <a class="nav-link dev-guide-link" href="#client-calls" @click.prevent="scrollTo('client-calls')">Client calls</a>
                            <a class="nav-link dev-guide-link" href="#batch-cache" @click.prevent="scrollTo('batch-cache')">Batch & cache</a>
                            <a class="nav-link dev-guide-link" href="#longrunning" @click.prevent="scrollTo('longrunning')">Long-running</a>
                            <a class="nav-link dev-guide-link" href="#errors" @click.prevent="scrollTo('errors')">Errors</a>
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

    let _this = {
        cid: "", c: null,
        lr: { seconds: 10, taskToken: null, status: null, durationMs: null, error: null, result: null, polling: false, pollingTimer: null, log: [] }
    };

    export default {
        setup(props) {
            _this.cid = props["cid"];
        },
        data() { return _this; },
        created() { _this.c = this; },
        computed: {
            lrBadge() {
                const m = { 'Pending': 'bg-secondary', 'Running': 'bg-primary', 'Completed': 'bg-success', 'Failed': 'bg-danger', 'Cancelled': 'bg-warning text-dark' };
                return m[_this.c?.lr?.status] || 'bg-secondary';
            }
        },
        mounted() {
            let scrollEl = this.$el.querySelector(".dev-guide-content");
            if (window.bootstrap?.ScrollSpy) {
                new window.bootstrap.ScrollSpy(scrollEl, {
                    target: "#devGuideNavApi",
                    rootMargin: "0px 0px -60%"
                });
            }
            initDevCodeBlocks(this.$el);
        },
        methods: {
            scrollTo(id) {
                let el = this.$el.querySelector('#' + id);
                let container = this.$el.querySelector('.dev-guide-content');
                if (el && container) el.scrollIntoView({ behavior: 'smooth', block: 'start' });
            },
            lrLog(text, color) {
                _this.c.lr.log.unshift({ time: new Date().toLocaleTimeString(), text: text, color: color || 'dark' });
            },
            lrStart() {
                let c = _this.c;
                c.lr.taskToken = null; c.lr.status = null; c.lr.durationMs = null; c.lr.error = null; c.lr.result = null; c.lr.log = [];
                c.lrLog('Starting LongRunningDemo with ' + c.lr.seconds + 's ...', 'primary');
                rpc({
                    requests: [{ Method: "DefaultRepo.Test.LongRunningDemo", Inputs: { Seconds: c.lr.seconds || 10 } }],
                    onDone: function (r) {
                        let resp = r[0];
                        if (resp.IsLongRunning && resp.TaskToken) {
                            c.lr.taskToken = resp.TaskToken; c.lr.status = 'Pending';
                            c.lrLog('Token: ' + resp.TaskToken, 'success');
                            c.lrStartPolling();
                        } else if (resp.IsSucceeded) {
                            c.lr.result = resp.Result; c.lr.status = 'Completed';
                            c.lrLog('Completed synchronously', 'info');
                        } else {
                            c.lr.error = resp.Result?.Message || JSON.stringify(resp.Result); c.lr.status = 'Failed';
                            c.lrLog('Failed: ' + c.lr.error, 'danger');
                        }
                    },
                    onFail: function (err) { c.lr.status = 'Failed'; c.lr.error = JSON.stringify(err); c.lrLog('RPC failed', 'danger'); },
                    silent: true
                });
            },
            lrGetStatus() {
                let c = _this.c; if (!c.lr.taskToken) return;
                rpc({
                    requests: [{ Method: "__LR_GetStatus", Inputs: { TaskToken: c.lr.taskToken } }],
                    onDone: function (r) {
                        let info = r[0]?.Result;
                        if (info) { c.lr.status = info.Status; c.lr.durationMs = info.DurationMs; c.lr.error = info.Error; c.lrLog('Status: ' + info.Status + ' | ' + info.DurationMs + 'ms', 'info'); }
                    },
                    silent: true
                });
            },
            lrGetResult() {
                let c = _this.c; if (!c.lr.taskToken) return;
                c.lrLog('Fetching result...', 'info');
                rpc({
                    requests: [{ Method: "__LR_GetResult", Inputs: { TaskToken: c.lr.taskToken } }],
                    onDone: function (r) {
                        if (r[0].IsSucceeded) { c.lr.result = r[0].Result; c.lr.durationMs = r[0].Duration; c.lrLog('Result received.', 'success'); }
                        else { c.lr.error = r[0].Result?.Message || JSON.stringify(r[0].Result); c.lrLog('GetResult failed', 'danger'); }
                    },
                    silent: true
                });
            },
            lrCancel() {
                let c = _this.c; if (!c.lr.taskToken) return;
                c.lrLog('Cancelling...', 'warning');
                rpc({
                    requests: [{ Method: "__LR_Cancel", Inputs: { TaskToken: c.lr.taskToken } }],
                    onDone: function (r) {
                        if (r[0].IsSucceeded && r[0].Result === true) { c.lr.status = 'Cancelled'; c.lrStopPolling(); c.lrLog('Cancelled.', 'warning'); }
                        else { c.lrLog('Cancel failed.', 'danger'); }
                    },
                    silent: true
                });
            },
            lrStartPolling() {
                let c = _this.c; c.lrStopPolling(); c.lr.polling = true;
                c.lrLog('Polling started (2s)', 'secondary');
                c.lr.pollingTimer = setInterval(function () {
                    rpc({
                        requests: [{ Method: "__LR_GetStatus", Inputs: { TaskToken: c.lr.taskToken } }],
                        onDone: function (r) {
                            let info = r[0]?.Result;
                            if (info) {
                                c.lr.status = info.Status; c.lr.durationMs = info.DurationMs; c.lr.error = info.Error;
                                if (info.Status === 'Completed' || info.Status === 'Failed' || info.Status === 'Cancelled') {
                                    c.lrStopPolling();
                                    c.lrLog('Finished: ' + info.Status + ' (' + info.DurationMs + 'ms)', info.Status === 'Completed' ? 'success' : 'danger');
                                    if (info.Status === 'Completed') c.lrGetResult();
                                }
                            }
                        },
                        silent: true
                    });
                }, 2000);
            },
            lrStopPolling() {
                let c = _this.c;
                if (c.lr.pollingTimer) { clearInterval(c.lr.pollingTimer); c.lr.pollingTimer = null; }
                c.lr.polling = false;
            },
            lrReset() {
                let c = _this.c; c.lrStopPolling();
                c.lr.taskToken = null; c.lr.status = null; c.lr.durationMs = null; c.lr.error = null; c.lr.result = null; c.lr.log = [];
            }
        },
        props: { cid: String }
    }
</script>
