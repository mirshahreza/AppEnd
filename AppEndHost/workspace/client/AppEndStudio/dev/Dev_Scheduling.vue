<template>
    <div class="card h-100 bg-transparent rounded-0 border-0 dev-guide">
        <div class="card-header p-2 bg-body-subtle rounded-0 border-0">
            <div class="hstack gap-2">
                <div class="fw-bold text-primary">
                    <i class="fa-solid fa-clock me-1"></i>
                    <span>Scheduling &amp; Background Tasks</span>
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
                                    This page explains how to create, manage, and monitor scheduled background tasks in AppEnd using the built-in Scheduler system with cron expressions.
                                </p>
                                <ul class="mb-0">
                                    <li>Understand the <span class="fw-bold">SchedulerService</span> and <span class="fw-bold">SchedulerManager</span> architecture.</li>
                                    <li>Define tasks with cron expressions.</li>
                                    <li>Monitor execution history and manage task states.</li>
                                </ul>
                            </section>

                            <section id="architecture" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-sitemap me-1 text-info"></i>
                                    <span class="fw-bold">Architecture</span>
                                </div>
                                <p class="mb-1">The scheduling system consists of three main components:</p>
                                <ul class="mb-0">
                                    <li><span class="text-primary fw-bold">SchedulerService</span> — a .NET <span class="fw-bold">BackgroundService</span> that runs a timer loop, checking tasks every tick.</li>
                                    <li><span class="text-success fw-bold">SchedulerManager</span> — manages the task registry, persists tasks to <span class="fw-bold">appsettings.json</span>, and exposes APIs.</li>
                                    <li><span class="text-warning fw-bold">CronExpression</span> — parses and evaluates standard 5-field cron expressions.</li>
                                </ul>
                            </section>

                            <section id="task-model" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-cube me-1 text-warning"></i>
                                    <span class="fw-bold">ScheduledTask model</span>
                                </div>
                                <pre class="mb-2">public class ScheduledTask
{
    public string TaskId { get; set; }            // Auto-generated GUID
    public string Name { get; set; }              // Human-readable name
    public string Description { get; set; }       // Optional description
    public bool Enabled { get; set; }             // Active or disabled
    public string CronExpression { get; set; }    // e.g. "*/10 * * * *"
    public string MethodFullName { get; set; }    // Namespace.Class.Method
    public string? MethodParameters { get; set; } // JSON parameters

    // Runtime state
    public DateTime? LastRunTime { get; set; }
    public DateTime? NextRunTime { get; set; }
    public int ExecutionCount { get; set; }
    public int FailureCount { get; set; }
    public string? LastError { get; set; }
    public TaskState State { get; set; }          // Stopped, Running, Paused, Failed
}</pre>
                                <div class="fw-bold mb-1">TaskState enum</div>
                                <ul class="mb-0">
                                    <li><span class="text-secondary fw-bold">Stopped</span> — task is registered but not running.</li>
                                    <li><span class="text-primary fw-bold">Running</span> — task is active and will execute on schedule.</li>
                                    <li><span class="text-warning fw-bold">Paused</span> — temporarily suspended.</li>
                                    <li><span class="text-danger fw-bold">Failed</span> — last execution encountered an error.</li>
                                </ul>
                            </section>

                            <section id="cron" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-calendar-days me-1 text-success"></i>
                                    <span class="fw-bold">Cron expression format</span>
                                </div>
                                <p class="mb-1">Standard 5-field cron format: <span class="fw-bold">minute hour day-of-month month day-of-week</span></p>
                                <pre class="mb-2">┌───────── minute (0–59)
│ ┌─────── hour (0–23)
│ │ ┌───── day of month (1–31)
│ │ │ ┌─── month (1–12)
│ │ │ │ ┌─ day of week (0–6, Sunday=0)
│ │ │ │ │
* * * * *</pre>
                                <div class="fw-bold mb-1">Common examples</div>
                                <ul class="mb-0">
                                    <li><span class="fw-bold">*/10 * * * *</span> — every 10 minutes.</li>
                                    <li><span class="fw-bold">0 * * * *</span> — every hour at minute 0.</li>
                                    <li><span class="fw-bold">0 3 * * *</span> — daily at 3:00 AM.</li>
                                    <li><span class="fw-bold">0 0 * * 0</span> — every Sunday at midnight.</li>
                                    <li><span class="fw-bold">0 9 1 * *</span> — first day of every month at 9:00 AM.</li>
                                    <li><span class="fw-bold">30 2 * * 1-5</span> — weekdays at 2:30 AM.</li>
                                </ul>
                            </section>

                            <section id="server-method" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-code me-1 text-primary"></i>
                                    <span class="fw-bold">Creating a schedulable method</span>
                                </div>
                                <p class="mb-1">Any static server method can be scheduled. The <span class="fw-bold">MethodFullName</span> follows the standard <span class="fw-bold">Namespace.Class.Method</span> convention:</p>
                                <pre class="mb-2">// File: workspace/server/DefaultRepo.Maintenance.cs
namespace DefaultRepo
{
    public static class Maintenance
    {
        public static object? CleanOldLogs(AppEndUser? Actor)
        {
            // Your cleanup logic here
            return new { Cleaned = true, Timestamp = DateTime.UtcNow };
        }
    }
}</pre>
                                <p class="mb-0">Set the <span class="fw-bold">MethodFullName</span> to <span class="text-success fw-bold">"DefaultRepo.Maintenance.CleanOldLogs"</span>.</p>
                            </section>

                            <section id="settings-config" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-file-code me-1 text-info"></i>
                                    <span class="fw-bold">Configuring tasks in appsettings.json</span>
                                </div>
                                <p class="mb-1">Tasks are stored and auto-loaded from <span class="fw-bold">AppEnd.ScheduledTasks</span>:</p>
                                <pre class="mb-0">{
  "AppEnd": {
    "ScheduledTasks": [
      {
        "TaskId": "task-cleanup-001",
        "Name": "Clean Old Logs",
        "Description": "Remove logs older than 30 days",
        "Enabled": true,
        "CronExpression": "0 3 * * *",
        "MethodFullName": "DefaultRepo.Maintenance.CleanOldLogs",
        "MethodParameters": null
      },
      {
        "TaskId": "task-report-002",
        "Name": "Daily Report",
        "Enabled": false,
        "CronExpression": "0 8 * * 1-5",
        "MethodFullName": "DefaultRepo.Reports.GenerateDaily"
      }
    ]
  }
}</pre>
                            </section>

                            <section id="management" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-sliders me-1 text-warning"></i>
                                    <span class="fw-bold">Task management operations</span>
                                </div>
                                <p class="mb-1">The <span class="fw-bold">SchedulerManager</span> provides these operations:</p>
                                <ul class="mb-0">
                                    <li><span class="text-success fw-bold">Register</span> — add and start a new task.</li>
                                    <li><span class="text-warning fw-bold">Pause</span> — temporarily suspend a running task.</li>
                                    <li><span class="text-primary fw-bold">Resume</span> — continue a paused task.</li>
                                    <li><span class="text-danger fw-bold">Unregister</span> — stop and remove a task.</li>
                                    <li><span class="text-info fw-bold">Update</span> — modify task settings (name, cron, method).</li>
                                </ul>
                                <div class="dev-demo-panel">
                                    <div class="fw-bold mb-2"><i class="fa-solid fa-play me-1 text-success"></i> Live Demo</div>
                                    <button class="btn btn-sm btn-primary" @click="demoOpenScheduler"><i class="fa-solid fa-clock me-1"></i> Open Scheduler Manager</button>
                                </div>
                            </section>

                            <section id="history" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-clock-rotate-left me-1 text-info"></i>
                                    <span class="fw-bold">Execution history</span>
                                </div>
                                <p class="mb-1">Each execution is recorded in a <span class="fw-bold">TaskExecutionHistory</span> object:</p>
                                <pre class="mb-2">public class TaskExecutionHistory
{
    public string HistoryId { get; set; }
    public string TaskId { get; set; }
    public string TaskName { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public bool IsSuccessful { get; set; }
    public long DurationMs { get; set; }
    public string? ErrorMessage { get; set; }
    public string? Result { get; set; }
}</pre>
                                <p class="mb-0">History is kept in memory (up to <span class="fw-bold">1000</span> records). Use the <span class="fw-bold">SchedulerManagement</span> UI component or API to view it.</p>
                            </section>

                            <section id="statistics" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-chart-bar me-1 text-success"></i>
                                    <span class="fw-bold">Scheduler statistics</span>
                                </div>
                                <p class="mb-1">Get an overview of the scheduler state:</p>
                                <pre class="mb-0">public class SchedulerStatistics
{
    public int TotalTasks { get; set; }
    public int EnabledTasks { get; set; }
    public int DisabledTasks { get; set; }
    public int RunningTasks { get; set; }
    public int PausedTasks { get; set; }
    public int FailedTasks { get; set; }
    public int TotalExecutions { get; set; }
    public int TotalFailures { get; set; }
    public DateTime? LastExecutionTime { get; set; }
    public bool SchedulerRunning { get; set; }
}</pre>
                            </section>

                            <section id="bestpractices" class="dev-guide-section">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-circle-check me-1 text-success"></i>
                                    <span class="fw-bold">Best practices</span>
                                </div>
                                <ul class="mb-0">
                                    <li>Always validate that the <span class="fw-bold">MethodFullName</span> exists before registering a task.</li>
                                    <li>Use descriptive <span class="fw-bold">Name</span> and <span class="fw-bold">Description</span> fields for easier management.</li>
                                    <li>Avoid scheduling tasks with very short intervals (<span class="text-danger">&lt; 1 minute</span>) unless necessary.</li>
                                    <li>Monitor <span class="fw-bold">FailureCount</span> and <span class="fw-bold">LastError</span> to detect recurring issues.</li>
                                    <li>Use the <span class="fw-bold">CronBuilder</span> shared component for visual cron expression building.</li>
                                </ul>
                            </section>
                        </div>
                    </div>
                    <div class="col-48 col-lg-12 order-1 order-lg-2 mb-2 mb-lg-0">
                        <nav id="devGuideNavScheduler" class="dev-guide-nav nav flex-column p-2">
                            <div class="fw-bold text-uppercase text-muted fs-d9 mb-2">On this page</div>
                            <a class="nav-link dev-guide-link" href="#purpose" @click.prevent="scrollTo('purpose')">Purpose</a>
                            <a class="nav-link dev-guide-link" href="#architecture" @click.prevent="scrollTo('architecture')">Architecture</a>
                            <a class="nav-link dev-guide-link" href="#task-model" @click.prevent="scrollTo('task-model')">Task model</a>
                            <a class="nav-link dev-guide-link" href="#cron" @click.prevent="scrollTo('cron')">Cron format</a>
                            <a class="nav-link dev-guide-link" href="#server-method" @click.prevent="scrollTo('server-method')">Server method</a>
                            <a class="nav-link dev-guide-link" href="#settings-config" @click.prevent="scrollTo('settings-config')">Settings config</a>
                            <a class="nav-link dev-guide-link" href="#management" @click.prevent="scrollTo('management')">Management</a>
                            <a class="nav-link dev-guide-link" href="#history" @click.prevent="scrollTo('history')">History</a>
                            <a class="nav-link dev-guide-link" href="#statistics" @click.prevent="scrollTo('statistics')">Statistics</a>
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
                    target: "#devGuideNavScheduler",
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
            demoOpenScheduler() {
                openComponent("components/BaseAppEndSettings", {
                    title: "AppEnd Settings",
                    modal: true,
                    modalSize: "modal-fullscreen"
                });
            }
        },
        props: { cid: String }
    }
</script>
