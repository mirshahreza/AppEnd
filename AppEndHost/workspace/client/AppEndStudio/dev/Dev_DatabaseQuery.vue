<template>
    <div class="card h-100 bg-transparent rounded-0 border-0 dev-guide">
        <div class="card-header p-2 bg-body-subtle rounded-0 border-0">
            <div class="hstack gap-2">
                <div class="fw-bold text-primary">
                    <i class="fa-solid fa-database me-1"></i>
                    <span>Database Query</span>
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
                                    This page provides end-to-end database query patterns with C# samples, from basic CRUD to advanced filters and direct SQL.
                                </p>
                                <ul class="mb-0">
                                    <li>Know which query style fits each use case.</li>
                                    <li>Use safe, structured queries with <span class="fw-bold">ClientQuery</span>.</li>
                                    <li>Fall back to direct SQL only when necessary.</li>
                                </ul>
                            </section>

                            <section id="readlist" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-list me-1 text-primary"></i>
                                    <span class="fw-bold">ReadList with ClientQuery</span>
                                </div>
                                <p class="mb-1">Typical list query with paging, ordering, and filter.</p>
                                <pre class="mb-0">public static object? ReadList(JsonElement clientQueryJE, AppEndUser? actor)
{
    ClientQuery cq = ClientQuery.GetInstanceByQueryJson(clientQueryJE, actor?.ContextInfo);
    if (cq.Where == null) cq.Where = new Where();
    cq.Where.And("IsActive", CompareOperator.Equal, true);
    cq.Pagination = new Pagination { PageNumber = 1, PageSize = 50 };
    cq.OrderClauses = [new OrderClause { Name = "Id", OrderDirection = "DESC" }];
    return cq.Exec();
}</pre>
                            </section>

                            <section id="readbykey" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-key me-1 text-primary"></i>
                                    <span class="fw-bold">ReadByKey</span>
                                </div>
                                <p class="mb-1">Use the generated method or execute a targeted query.</p>
                                <pre class="mb-0">public static object? ReadByKey(JsonElement clientQueryJE, AppEndUser? actor)
{
    return ClientQuery.GetInstanceByQueryJson(clientQueryJE, actor?.ContextInfo).Exec();
}</pre>
                            </section>

                            <section id="create" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-plus me-1 text-primary"></i>
                                    <span class="fw-bold">Create</span>
                                </div>
                                <p class="mb-1">Insert using DbIO through ClientQuery.</p>
                                <pre class="mb-0">public static object? Create(JsonElement clientQueryJE, AppEndUser? actor)
{
    return ClientQuery.GetInstanceByQueryJson(clientQueryJE, actor?.ContextInfo).Exec();
}</pre>
                            </section>

                            <section id="update" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-pen-to-square me-1 text-primary"></i>
                                    <span class="fw-bold">UpdateByKey</span>
                                </div>
                                <p class="mb-1">Update a row by its key using generated methods.</p>
                                <pre class="mb-0">public static object? UpdateByKey(JsonElement clientQueryJE, AppEndUser? actor)
{
    return ClientQuery.GetInstanceByQueryJson(clientQueryJE, actor?.ContextInfo).Exec();
}</pre>
                            </section>

                            <section id="delete" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-trash me-1 text-primary"></i>
                                    <span class="fw-bold">DeleteByKey</span>
                                </div>
                                <p class="mb-1">Delete a record by key (soft delete if enabled in the schema).</p>
                                <pre class="mb-0">public static object? DeleteByKey(JsonElement clientQueryJE, AppEndUser? actor)
{
    return ClientQuery.GetInstanceByQueryJson(clientQueryJE, actor?.ContextInfo).Exec();
}</pre>
                            </section>

                            <section id="filters" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-filter me-1 text-primary"></i>
                                    <span class="fw-bold">Advanced filters</span>
                                </div>
                                <p class="mb-1">Build complex conditions before execution.</p>
                                <pre class="mb-0">public static object? ReadList(JsonElement clientQueryJE, AppEndUser? actor)
{
    ClientQuery cq = ClientQuery.GetInstanceByQueryJson(clientQueryJE, actor?.ContextInfo);
    cq.Where = new Where()
        .And("CreatedOn", CompareOperator.GreaterOrEqual, DateTime.UtcNow.AddDays(-30))
        .And("Status", CompareOperator.In, new[] { "Draft", "Published" })
        .Or("Priority", CompareOperator.Equal, "High");
    return cq.Exec();
}</pre>
                            </section>

                            <section id="joins" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-link me-1 text-primary"></i>
                                    <span class="fw-bold">Sub-queries / joins</span>
                                </div>
                                <p class="mb-1">Enable sub-queries for related data.</p>
                                <pre class="mb-0">public static object? ReadList(JsonElement clientQueryJE, AppEndUser? actor)
{
    ClientQuery cq = ClientQuery.GetInstanceByQueryJson(clientQueryJE, actor?.ContextInfo);
    cq.IncludeSubQueries = true;
    return cq.Exec();
}</pre>
                            </section>

                            <section id="aggregate" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-chart-simple me-1 text-primary"></i>
                                    <span class="fw-bold">Aggregations</span>
                                </div>
                                <p class="mb-1">Use DbIO for aggregate operations.</p>
                                <pre class="mb-0">public static object? ReadList(JsonElement clientQueryJE, AppEndUser? actor)
{
    ClientQuery cq = ClientQuery.GetInstanceByQueryJson(clientQueryJE, actor?.ContextInfo);
    cq.Aggregates = [
        new AggregateClause { Name = "Total", AggregateFunction = "COUNT", ColumnName = "Id" }
    ];
    return cq.Exec();
}</pre>
                            </section>

                            <section id="directsql" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-terminal me-1 text-primary"></i>
                                    <span class="fw-bold">Direct SQL (DbIO)</span>
                                </div>
                                <p class="mb-1">Use direct SQL only when required.</p>
                                <pre class="mb-0">public static object? GetCustomData(AppEndUser? actor)
{
    DbIO db = DbIO.Instance(DbConf.FromSettings(AppEndSettings.DefaultDbConfName));
    return db.ToDataTable("SELECT TOP 10 Id, Title FROM BaseInfo ORDER BY Id DESC");
}</pre>
                            </section>

                            <section id="transactions" class="dev-guide-section mb-5">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-arrows-rotate me-1 text-primary"></i>
                                    <span class="fw-bold">Transactions</span>
                                </div>
                                <p class="mb-1">Wrap multiple commands in a single transaction.</p>
                                <pre class="mb-0">public static object? UpdateAndLog(AppEndUser? actor)
{
    DbIO db = DbIO.Instance(DbConf.FromSettings(AppEndSettings.DefaultDbConfName));
    using var trx = db.BeginTransaction();
    db.ToNoneQuery("UPDATE BaseInfo SET IsActive=1 WHERE Id=1", trx);
    db.ToNoneQuery("INSERT INTO BaseActivityLog (Title) VALUES ('Updated')", trx);
    trx.Commit();
    return new { IsSucceeded = true };
}</pre>
                            </section>

                            <section id="bestpractices" class="dev-guide-section">
                                <div class="dev-guide-title">
                                    <i class="fa-solid fa-circle-check me-1 text-primary"></i>
                                    <span class="fw-bold">Best practices</span>
                                </div>
                                <ul class="mb-0">
                                    <li>Prefer <span class="fw-bold">ClientQuery</span> for filters, paging, and permissions.</li>
                                    <li>Keep direct SQL limited and well-audited.</li>
                                    <li>Always validate inputs before passing them to the database.</li>
                                </ul>
                            </section>
                        </div>
                    </div>
                    <div class="col-48 col-lg-12 order-1 order-lg-2 mb-2 mb-lg-0">
                        <nav id="devGuideNavDb" class="dev-guide-nav nav flex-column p-2">
                            <div class="fw-bold text-uppercase text-muted fs-d9 mb-2">On this page</div>
                            <a class="nav-link dev-guide-link" href="#purpose" @click.prevent="scrollTo('purpose')">Purpose</a>
                            <a class="nav-link dev-guide-link" href="#readlist" @click.prevent="scrollTo('readlist')">ReadList</a>
                            <a class="nav-link dev-guide-link" href="#readbykey" @click.prevent="scrollTo('readbykey')">ReadByKey</a>
                            <a class="nav-link dev-guide-link" href="#create" @click.prevent="scrollTo('create')">Create</a>
                            <a class="nav-link dev-guide-link" href="#update" @click.prevent="scrollTo('update')">UpdateByKey</a>
                            <a class="nav-link dev-guide-link" href="#delete" @click.prevent="scrollTo('delete')">DeleteByKey</a>
                            <a class="nav-link dev-guide-link" href="#filters" @click.prevent="scrollTo('filters')">Advanced filters</a>
                            <a class="nav-link dev-guide-link" href="#joins" @click.prevent="scrollTo('joins')">Sub-queries</a>
                            <a class="nav-link dev-guide-link" href="#aggregate" @click.prevent="scrollTo('aggregate')">Aggregations</a>
                            <a class="nav-link dev-guide-link" href="#directsql" @click.prevent="scrollTo('directsql')">Direct SQL</a>
                            <a class="nav-link dev-guide-link" href="#transactions" @click.prevent="scrollTo('transactions')">Transactions</a>
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
                    target: "#devGuideNavDb",
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
