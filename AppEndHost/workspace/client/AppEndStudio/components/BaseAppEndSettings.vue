<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-header px-2 bg-warning-subtle host-toolbar">
            <div class="hstack">
                <button class="btn btn-link text-decoration-none bg-hover-light host-toolbar-btn" @click="ok" aria-label="Save">
                    <i class="fa-solid fa-check me-1" aria-hidden="true"></i>Save
                </button>
                <div class="vr mx-1"></div>
                <button class="btn btn-link text-decoration-none bg-hover-light host-toolbar-btn" @click="refresh" aria-label="Refresh">
                    <i class="fa-solid fa-rotate-right me-1" aria-hidden="true"></i>Refresh
                </button>

                <div class="p-0 ms-auto"></div>
            </div>
        </div>
        <div class="card-body p-0 d-flex" style="overflow: hidden;  contain: layout paint size; transform: translateZ(0);">
            <!-- Left Sidebar: Category tree -->
            <nav class="border-end bg-white flex-shrink-0 scrollable" style="width:220px; min-width:220px; contain: layout paint;" aria-label="Settings categories">
                <div class="list-group list-group-flush" role="tablist">
                    <button v-for="cat in categories" :key="cat.key"
                            class="list-group-item list-group-item-action border-0 py-2 px-3 text-start text-truncate"
                            :class="activeCategory === cat.key ? 'bg-light text-dark border-start border-3 border-secondary fw-semibold' : 'bg-white'"
                            @click="activeCategory = cat.key"
                            role="tab"
                            :aria-selected="activeCategory === cat.key"
                            :aria-controls="`panel-${cat.key}`"
                            :title="cat.label">
                        <i :class="cat.icon + ' me-2 text-secondary'" aria-hidden="true"></i>{{cat.label}}
                    </button>
                </div>
            </nav>

            <!-- Right Panel: Property editor -->
            <div class="flex-grow-1 p-3 bg-light scrollable" style=" contain: layout paint size;" role="tabpanel" id="formArea" data-ae-widget="inputsRegulator" data-ae-widget-options="{}">
                
                <div v-if="activeCategory === 'general'" :id="`panel-general`">
                    <h5 class="mb-3"><i class="fa-solid fa-fa-cog text-secondary"></i> General</h5>

                    <div class="d-none d-md-block mb-5">
                        <component-loader src="components/BaseServerSummary" uid="baseServerSummary" />
                    </div>


                    <div class="mb-3">
                        <div class="d-flex align-items-center gap-2 mb-1 flex-wrap">
                            <label for="talkpoint" class="form-label small text-secondary mb-0">TalkPoint <span class="text-danger">*</span></label>
                            <small class="text-muted" style="font-size:0.7rem;">Endpoint path for API calls</small>
                        </div>
                        <div class="data-ae-validation">
                            <input id="talkpoint" type="text" class="form-control form-control-sm" v-model="model.TalkPoint"
                                   data-ae-validation-required="true" data-ae-validation-rule=":=s(1,100)" />
                        </div>
                    </div>

                    <div class="mb-3">
                        <label for="defaultdbconfname" class="form-label small text-secondary">DefaultDbConfName <span class="text-danger">*</span></label>
                        <div class="data-ae-validation">
                            <input id="defaultdbconfname" type="text" class="form-control form-control-sm" v-model="model.DefaultDbConfName"
                                   data-ae-validation-required="true" data-ae-validation-rule=":=s(1,100)" />
                        </div>
                    </div>

                    <div class="mb-3">
                        <div class="d-flex align-items-center gap-2 mb-1 flex-wrap">
                            <label for="secret" class="form-label small text-secondary mb-0">Secret <span class="text-danger">*</span></label>
                            <small class="text-muted" style="font-size:0.7rem;">JWT signing key</small>
                        </div>
                        <div class="input-group input-group-sm">
                            <div class="data-ae-validation flex-grow-1" style="">
                                <input id="secret" :type="showSecret ? 'text' : 'password'" class="form-control" v-model="model.Secret" style=""
                                       data-ae-validation-required="true" data-ae-validation-rule=":=s(8,500)" />
                            </div>
                            <button class="btn btn-outline-secondary flex-shrink-0" @click="showSecret = !showSecret" :aria-label="showSecret ? 'Hide secret' : 'Show secret'" type="button">
                                {{ showSecret ? 'Hide' : 'Show' }}
                            </button>
                        </div>
                    </div>
                </div>

                <div v-else-if="activeCategory === 'aaa'" :id="`panel-aaa`" >
                    <h5 class="mb-3"><i class="fa-solid fa-user-shield text-secondary"></i> AAA</h5>

                    <div class="mb-3" >
                        <label for="logindbconfname" class="form-label small text-secondary">LoginDbConfName <span class="text-danger">*</span></label>
                        <div class="data-ae-validation" >
                            <input id="logindbconfname" type="text" class="form-control form-control-sm" v-model="model.AAA.LoginDbConfName"  
                                   data-ae-validation-required="true" data-ae-validation-rule=":=s(1,100)" />
                        </div>
                    </div>

                    <div class="mb-3" >
                        <label for="publickeyuser" class="form-label small text-secondary">PublicKeyUser <span class="text-danger">*</span></label>
                        <div class="data-ae-validation" >
                            <input id="publickeyuser" type="text" class="form-control form-control-sm" v-model="model.AAA.PublicKeyUser"  
                                   data-ae-validation-required="true" data-ae-validation-rule=":=s(1,100)" />
                        </div>
                    </div>

                    <div class="mb-3" >
                        <label for="publickeyrole" class="form-label small text-secondary">PublicKeyRole <span class="text-danger">*</span></label>
                        <div class="data-ae-validation" >
                            <input id="publickeyrole" type="text" class="form-control form-control-sm" v-model="model.AAA.PublicKeyRole"  
                                   data-ae-validation-required="true" data-ae-validation-rule=":=s(1,100)" />
                        </div>
                    </div>

                    <!-- PublicMethods box with list and add input inside -->
                    <div class="mb-3" >
                        <div class="d-flex align-items-center gap-2 mb-1 flex-wrap">
                            <label class="form-label small text-secondary mb-0">PublicMethods</label>
                            <small class="text-muted" style="font-size:0.7rem;">Methods accessible without authentication</small>
                        </div>
                        <div class="border rounded shadow-sm bg-white" style="max-width:100%; ">
                            <div class="p-2">
                                <div v-if="model.AAA && model.AAA.PublicMethods && model.AAA.PublicMethods.length > 0" class="d-flex flex-wrap gap-1" role="list" aria-label="Current public methods">
                                    <span v-for="(m, idx) in model.AAA.PublicMethods" :key="idx" :title="m"
                                          class="badge bg-light text-dark border d-inline-flex align-items-center gap-1 flex-shrink-0"
                                          role="listitem" style="max-width: calc(100% - 8px); ">
                                        <code class="fs-d8" style="overflow: hidden; text-overflow: ellipsis; white-space: nowrap; max-width:180px;">{{m}}</code>
                                        <button class="btn btn-sm btn-link text-danger p-0 lh-1 flex-shrink-0" @click="removePublicMethod(idx)" :aria-label="`Remove ${m}`" type="button">
                                            <i class="fa-solid fa-times" aria-hidden="true"></i>
                                        </button>
                                    </span>
                                </div>
                                <div v-else class="text-muted small">No public methods configured</div>
                            </div>
                            <div class="border-top p-2">
                                <div class="input-group input-group-sm" >
                                    <input type="text" class="form-control" v-model="newPublicMethod" placeholder="Namespace.Class.Method" @keyup.enter="addPublicMethod" style="" />
                                    <button class="btn btn-primary flex-shrink-0" @click="addPublicMethod" type="button" aria-label="Add public method">Add</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div v-else-if="activeCategory === 'serilog'" :id="`panel-serilog`" >
                    <h5 class="mb-3"><i class="fa-solid fa-shoe-prints text-secondary"></i> Serilog</h5>

                    <div class="mb-3" >
                        <label for="serilog-tablename" class="form-label small text-secondary">TableName <span class="text-danger">*</span></label>
                        <div class="data-ae-validation" >
                            <input id="serilog-tablename" type="text" class="form-control form-control-sm" v-model="model.Serilog.TableName" 
                                   data-ae-validation-required="true" data-ae-validation-rule=":=s(1,200)" />
                        </div>
                    </div>

                    <div class="mb-3" >
                        <label for="serilog-connection" class="form-label small text-secondary">Connection <span class="text-danger">*</span></label>
                        <div class="data-ae-validation" >
                            <input id="serilog-connection" type="text" class="form-control form-control-sm" v-model="model.Serilog.Connection" 
                                   data-ae-validation-required="true" data-ae-validation-rule=":=s(1,100)" />
                        </div>
                    </div>

                    <div class="mb-3" >
                        <label for="serilog-batchpostinglimit" class="form-label small text-secondary">BatchPostingLimit <span class="text-danger">*</span></label>
                        <div class="data-ae-validation" >
                            <input id="serilog-batchpostinglimit" type="number" class="form-control form-control-sm" v-model.number="model.Serilog.BatchPostingLimit" 
                                   data-ae-validation-required="true" data-ae-validation-rule=":=i(1,10000)" />
                        </div>
                    </div>

                    <div class="mb-3" >
                        <label for="serilog-batchperiodseconds" class="form-label small text-secondary">BatchPeriodSeconds <span class="text-danger">*</span></label>
                        <div class="data-ae-validation" >
                            <input id="serilog-batchperiodseconds" type="number" class="form-control form-control-sm" v-model.number="model.Serilog.BatchPeriodSeconds" 
                                   data-ae-validation-required="true" data-ae-validation-rule=":=i(1,3600)" />
                        </div>
                    </div>
                </div>

                <div v-else-if="activeCategory === 'dbservers'" :id="`panel-dbservers`" style="max-width:100%;">
                    <div class="d-flex align-items-center justify-content-between mb-3">
                        <h5 class="mb-0">Database Servers</h5>
                        <button class="btn btn-sm btn-primary" @click="saveAllDbServers" type="button" aria-label="Save all database servers">
                            <i class="fa-solid fa-save me-1"></i>Save All
                        </button>
                    </div>
                    <div v-if="local.dbConnectionsLoading" class="text-center p-5">
                        <div class="spinner-border text-primary" role="status">
                            <span class="visually-hidden">Loading...</span>
                        </div>
                    </div>
                    <div v-else class="d-flex flex-wrap gap-2">
                        <div v-for="(db, idx) in local.dbConnections" :key="db.Id"
                             class="card bg-white shadow-sm" style="min-width:300px; max-width:520px; flex: 1 1 360px; border-radius: 4px;">
                            <div class="card-header py-2 d-flex align-items-center justify-content-between">
                                <div class="d-flex align-items-center gap-2">
                                    <i class="fa-solid fa-database text-secondary"></i>
                                    <input type="text" class="form-control form-control-sm" v-model="db.Name" placeholder="Server Name" style="width:200px;" :aria-label="`Database server name ${idx + 1}`" />
                                </div>
                                <button class="btn btn-sm btn-danger" @click="removeDbServer(db, idx)" :aria-label="`Remove database server ${db.Name || idx + 1}`" type="button">
                <div v-else-if="activeCategory === 'dbservers'" :id="`panel-dbservers`" >
                    <h5 class="mb-3"><i class="fa-solid fa-database text-secondary"></i> Database Servers</h5>
                    <div class="d-flex flex-wrap gap-2">
                        <div v-for="(db, idx) in model.DbServers" :key="idx"
                             class="card bg-white shadow-sm" style="min-width:300px; max-width:520px; flex: 1 1 360px;">
                            <div class="card-header py-2 d-flex align-items-center justify-content-between">
                                <input type="text" class="form-control form-control-sm d-flex" v-model="db.Name" placeholder="Server Name" :aria-label="`Database server name ${idx + 1}`" />
                                <button class="btn btn-sm btn-danger" @click="removeDbServer(idx)" :aria-label="`Remove database server ${db.Name || idx + 1}`" type="button">
                                    <i class="fa-solid fa-trash"></i>
                                </button>
                            </div>
                            <div class="card-body py-2">
                                <div class="mb-2">
                                    <label class="form-label small text-secondary mb-1">ServerType</label>
                                    <select class="form-select form-select-sm" v-model="db.ServerType" :aria-label="`Server type ${idx + 1}`">
                                        <option>MsSql</option>
                                        <option>PostgreSql</option>
                                        <option>MySql</option>
                                        <option>Oracle</option>
                                    </select>
                                </div>
                                <div class="mb-2">
                                    <label class="form-label small text-secondary mb-1">ConnectionString</label>
                                    <textarea class="form-control form-control-sm" v-model="db.ConnectionString" placeholder="Server=...;Database=...;..." rows="3" :aria-label="`Connection string ${idx + 1}`"></textarea>
                                </div>
                            </div>
                            <div class="card-footer py-2 d-flex justify-content-end">
                                <button class="btn btn-sm btn-success" @click="testDbServer(db)" type="button" :aria-label="`Test database server ${db.Name || idx + 1}`">
                                    <i class="fa-solid fa-vial me-1"></i>Test
                                </button>
                            </div>
                        </div>
                    </div>
                    <button class="btn btn-sm btn-primary mt-3" @click="addDbServer" type="button" aria-label="Add new database server">
                        <i class="fa-solid fa-plus me-1" aria-hidden="true"></i>Add Server
                    </button>
                </div>

                <div v-else-if="activeCategory === 'llmproviders'" :id="`panel-llmproviders`" >
                    <h5 class="mb-3"><i class="fa-solid fa-brain text-secondary"></i> LLM Providers</h5>
                    <div class="d-flex flex-wrap gap-2">
                        <div v-for="(p, idx) in model.LLMProviders" :key="idx"
                             class="card bg-white shadow-sm" style="min-width:300px; max-width:520px; flex: 1 1 360px;">
                            <div class="card-header py-2 d-flex align-items-center justify-content-between">
                                <input type="text" class="form-control form-control-sm d-flex" v-model="p.Name" placeholder="Name" />
                                <button class="btn btn-sm btn-danger" @click="removeProvider(idx)" aria-label="Remove provider">
                                    <i class="fa-solid fa-trash"></i>
                                </button>
                            </div>
                            <div class="card-body py-2">
                                <div class="mb-2">
                                    <label class="form-label small text-secondary mb-1">ApiBaseUrl</label>
                                    <input type="text" class="form-control form-control-sm" v-model="p.ApiBaseUrl" placeholder="https://..." />
                                </div>
                                <div class="mb-2">
                                    <label class="form-label small text-secondary mb-1">API Key</label>
                                    <div class="input-group input-group-sm">
                                        <input :type="showApiKey[idx] ? 'text' : 'password'" class="form-control" v-model="p.ApiKey" placeholder="API key or leave blank" />
                                        <button class="btn btn-outline-secondary" @click="showApiKey[idx] = !showApiKey[idx]" type="button" :aria-label="showApiKey[idx] ? 'Hide API key' : 'Show API key'">
                                            {{ showApiKey[idx] ? 'Hide' : 'Show' }}
                                        </button>
                                    </div>
                                </div>

                                <div class="card mt-4 my-2 shadow-sm">
                                    <div class="card-header py-2 d-flex align-items-center justify-content-between">
                                        <span class="small text-secondary">Models</span>
                                        <div class="input-group input-group-sm" style="max-width: 280px;">
                                            <input type="text" class="form-control form-control-sm" v-model="newModelName[idx]" placeholder="model id" @keyup.enter="addModelStr(idx)" />
                                            <button class="btn btn-primary" @click="addModelStr(idx)" aria-label="Add model"><i class="fa-solid fa-plus"></i></button>
                                        </div>
                                    </div>
                                    <div class="card-body py-2">
                                        <div class="d-flex flex-wrap gap-1">
                                            <!-- Compact badge chips for models -->
                                            <span v-for="(m, midx) in (Array.isArray(p.Models) ? p.Models : [])" :key="midx"
                                                  class="badge bg-light text-dark border d-inline-flex align-items-center gap-1 py-0 pb-1">
                                                <span class="px-1">{{ m }}</span>
                                                <button class="btn btn-sm btn-link text-danger p-0" style="padding:2px !important;" @click="removeModelStr(idx, midx)" aria-label="Remove model">
                                                    <i class="fa-solid fa-times"></i>
                                                </button>
                                            </span>
                                            <span v-if="!p.Models || p.Models.length===0" class="text-muted small">No models</span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="dropdown mt-3">
                        <button class="btn btn-sm btn-primary dropdown-toggle" type="button"
                                id="addProviderDropdown" data-bs-toggle="dropdown" aria-expanded="false"
                                aria-label="Add new provider">
                            <i class="fa-solid fa-plus me-1" aria-hidden="true"></i> Add Provider
                        </button>
                        <ul class="dropdown-menu" aria-labelledby="addProviderDropdown">
                            <li>
                                <a class="dropdown-item" href="#" @click.prevent="addProvider('openai')">
                                    <i class="fa-solid fa-robot me-2"></i> OpenAI
                                </a>
                            </li>
                            <li>
                                <a class="dropdown-item" href="#" @click.prevent="addProvider('gemini')">
                                    <i class="fa-solid fa-gem me-2"></i> Gemini Direct
                                </a>
                            </li>
                            <li>
                                <a class="dropdown-item" href="#" @click.prevent="addProvider('custom')">
                                    <i class="fa-solid fa-gear me-2"></i> Custom
                                </a>
                            </li>
                        </ul>
                    </div>
                </div>

                <div v-else-if="activeCategory === 'scheduledtasks'" :id="`panel-scheduledtasks`" >
                    <h5 class="mb-3"><i class="fa-solid fa-clock text-secondary"></i> Scheduled Tasks</h5>
                    <div class="d-flex flex-wrap gap-2">
                        <div v-for="(st, idx) in model.ScheduledTasks" :key="idx"
                             class="card bg-white shadow-sm" style="min-width:300px; max-width:520px; flex: 1 1 360px;">
                            <div class="card-header py-2 d-flex align-items-center justify-content-between">
                                <div class="d-flex align-items-center gap-2" style="flex:1; ">
                                    <input type="text" class="form-control form-control-sm" v-model="st.Name" placeholder="Task name" style="flex:1; " 
                                           data-ae-validation-required="true" data-ae-validation-rule=":=s(1,200)" />
                                </div>
                                <button class="btn btn-sm btn-danger flex-shrink-0" @click="removeScheduledTask(idx)" :aria-label="`Remove scheduled task ${st.Name || idx + 1}`">
                                    <i class="fa-solid fa-trash"></i>
                                </button>
                            </div>
                            <div class="card-header py-2 d-flex align-items-center justify-content-between" :class="st.Enabled ? 'bg-success bg-opacity-10 border-success' : 'bg-danger bg-opacity-10 border-danger'">
                                <div class="form-check">
                                    <input class="form-check-input" type="checkbox" v-model="st.Enabled" :id="`enabled-${idx}`">
                                    <label class="form-check-label small fw-semibold" :for="`enabled-${idx}`" :class="st.Enabled ? 'text-success' : 'text-danger'">
                                        {{ st.Enabled ? 'Enabled' : 'Disabled' }}
                                    </label>
                                </div>
                            </div>
                            <div class="card-body py-3 bg-primary-subtle">
                                <div class="text-secondary fs-d7">Cron Expression <span class="text-danger">*</span></div>
                                <div class="input-group input-group rounded rounded-3">
                                    <input type="text" class="form-control font-monospace" v-model="st.CronExpression" placeholder="*/10 * * * *" 
                                           data-ae-validation-required="true" data-ae-validation-rule=":=s(1,50)" />
                                    <button class="btn btn-secondary flex-shrink-0" type="button" @click="openCronBuilder(idx)" aria-label="Open cron builder">
                                        <i class="fa-solid fa-clock"></i>
                                    </button>
                                </div>
                            </div>
                            <div class="card-body py-2">
                                <div class="mb-2">
                                    <label class="form-label small text-secondary mb-1">TaskId <span class="text-danger">*</span></label>
                                    <div class="data-ae-validation" >
                                        <input type="text" class="form-control form-control-sm" v-model="st.TaskId" placeholder="unique-task-id" 
                                               data-ae-validation-required="true" data-ae-validation-rule=":=s(1,100)" />
                                    </div>
                                </div>
                                <div class="mb-2">
                                    <label class="form-label small text-secondary mb-1">Description</label>
                                    <textarea class="form-control form-control-sm" v-model="st.Description" placeholder="Task description..." rows="2"></textarea>
                                </div>
                                <div class="mb-2">
                                    <label class="form-label small text-secondary mb-1">Method Full Name <span class="text-danger">*</span></label>
                                    <div class="data-ae-validation" >
                                        <input type="text" class="form-control form-control-sm" v-model="st.MethodFullName" placeholder="Namespace.Class.Method" 
                                               data-ae-validation-required="true" data-ae-validation-rule=":=s(1,500)" />
                                    </div>
                                </div>
                                <div class="mb-2">
                                    <label class="form-label small text-secondary mb-1">Method Parameters (JSON)</label>
                                    <textarea class="form-control form-control-sm" v-model="st.MethodParameters" placeholder='{"param1": "value1"}' rows="2"></textarea>
                                </div>
                            </div>
                        </div>
                    </div>
                    <button class="btn btn-sm btn-primary mt-3" @click="addScheduledTask" type="button" aria-label="Add new scheduled task">
                        <i class="fa-solid fa-plus me-1" aria-hidden="true"></i>Add Scheduled Task
                    </button>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    shared.setAppTitle("$auto$");

    let _this = { cid: '', c: null, model: {}, activeCategory: 'general', 
        categories: [
            { key: 'general', label: 'General', icon: 'fa-solid fa-cog' },
            { key: 'aaa', label: 'AAA', icon: 'fa-solid fa-user-shield' },
            { key: 'serilog', label: 'Serilog', icon: 'fa-solid fa-shoe-prints' },
            { key: 'dbservers', label: 'Database Servers', icon: 'fa-solid fa-database' },
            { key: 'llmproviders', label: 'LLM Providers', icon: 'fa-solid fa-brain' },
            { key: 'scheduledtasks', label: 'Scheduled Tasks', icon: 'fa-solid fa-clock' }
        ],
        newModelProviderIndex: 0,
        newModelName: {},
        showApiKey: {},
        newPublicMethod: '',
        showSecret: false,
        local: {
            dbConnections: [],
            dbConnectionsLoading: false
        }
    };
    export default {
        methods: {
            ok() {
                try {
                    if (!isAreaValidById("formArea")) return false;

                    let payload = JSON.parse(JSON.stringify(_this.model));
                    // Ensure AAA section exists
                    if (!payload.AAA) payload.AAA = {};

                    // DbServers are now stored in database, no need to save in settings
                    if (payload.AAA && Array.isArray(payload.AAA.PublicMethods)) {
                        payload.AAA.PublicMethods = payload.AAA.PublicMethods.filter(function(m){ return m && m.trim() !== ''; });
                    }
                    if (payload.LLMProviders && Array.isArray(payload.LLMProviders)) {
                        payload.LLMProviders = payload.LLMProviders.map(function(p){
                            // normalize models to string[]
                            if (!Array.isArray(p.Models)) p.Models = [];
                            p.Models = p.Models.map(function(m){ return (typeof m === 'string' ? m : (m && m.toString ? m.toString() : '')); });
                            // keep only supported fields
                            return {
                                Name: p.Name || '',
                                ApiBaseUrl: p.ApiBaseUrl || '',
                                ApiKey: typeof p.ApiKey === 'string' ? p.ApiKey : (p.ApiKey==null ? '' : String(p.ApiKey)),
                                Models: p.Models
                            };
                        }).filter(function(p){
                            return (p.Name && p.Name.trim() !== '') || (p.ApiBaseUrl && p.ApiBaseUrl.trim() !== '') || (p.ApiKey && p.ApiKey.trim() !== '') || (p.Models && p.Models.length>0);
                        });
                    }
                    if (payload.ScheduledTasks && Array.isArray(payload.ScheduledTasks)) {
                        payload.ScheduledTasks = payload.ScheduledTasks.filter(function(st){ 
                            return st && st.TaskId && st.TaskId.trim() !== '';
                        });
                    }
                    rpc({
                        requests: [{ Method: 'Zzz.AppEndProxy.SaveAppEndSettings', Inputs: { AppEnd: payload } }],
                        onDone(res) {
                            let result = R0R(res);
                            if (result === true) {
                                showSuccess('Settings saved and tasks reloaded successfully');
                            } else {
                                showError('Save failed');
                            }
                        },
                        onFail(err) {
                            showError('Save error');
                        }
                    });
                } catch (ex) {
                    showError('Save error');
                }
            },
            reloadTasks() {
                rpc({
                    requests: [{ Method: 'Zzz.AppEndProxy.SchedulerReloadTasks', Inputs: {} }],
                    onDone(res) {
                        let result = R0R(res);
                        // Check if result has Success property (OperationResult)
                        if (result && typeof result === 'object' && 'Success' in result) {
                            if (result.Success) {
                                showSuccess(result.Message || 'Tasks reloaded successfully');
                            } else {
                                showError(result.Message || 'Failed to reload tasks');
                            }
                        } else {
                            // Handle case where result is not an OperationResult
                            showSuccess('Tasks reload completed');
                        }
                    },
                    onFail(err) {
                        showError('Error reloading tasks');
                    }
                });
            },
            refresh() {
                try {
                    let r = rpcSync({ requests: [{ Method: 'Zzz.AppEndProxy.GetAppEndSettings', Inputs: {} }] });
                    let raw = R0R(r);
                    _this.model = typeof raw === 'string' ? JSON.parse(raw) : (raw || {});
                    if (!_this.model.AAA) _this.model.AAA = {};
                    if (!Array.isArray(_this.model.AAA.PublicMethods)) _this.model.AAA.PublicMethods = [];
                    if (!_this.model.LLMProviders) _this.model.LLMProviders = [];
                    _this.model.LLMProviders = _this.model.LLMProviders.map(function(p, idx){
                        if (!Array.isArray(p.Models)) p.Models = [];
                        p.Models = p.Models.map(function(m){ return (typeof m === 'string' ? m : (m && m.toString ? m.toString() : '')); });
                        if (typeof p.ApiKey === 'undefined' || p.ApiKey === null) p.ApiKey = '';
                        _this.showApiKey[idx] = false;
                        return p;
                    });
                    if (!_this.model.ScheduledTasks) _this.model.ScheduledTasks = [];
                    if (!_this.model.Serilog) _this.model.Serilog = {};
                    // Load DbConnections from database
                    if (_this.c && typeof _this.c.loadDbConnections === 'function') {
                        _this.c.loadDbConnections();
                    }
                    
                    // Re-initialize validation after data refresh
                    if (_this.c && typeof _this.c.$forceUpdate === 'function') {
                        _this.c.$forceUpdate();
                        _this.c.$nextTick(() => {
                            isAreaValidById("formArea");
                        });
                    }
                    showSuccess('Refreshed');
                } catch (ex) {
                    showError('Refresh failed');
                }
            },
            addProvider(type) {
                let list = Array.isArray(_this.model.LLMProviders) ? _this.model.LLMProviders.slice() : [];
                let newProvider = { Name: '', ApiBaseUrl: '', ApiKey: '', Models: [] };
                
                // Set default values based on type
                if (type === 'openai') {
                    newProvider.Name = 'OpenAI';
                    newProvider.ApiBaseUrl = 'https://api.openai.com/v1';
                } else if (type === 'gemini') {
                    newProvider.Name = 'Gemini Direct';
                    newProvider.ApiBaseUrl = 'https://generativelanguage.googleapis.com/v1beta';
                }
                // For 'custom', all fields remain empty
                
                list.push(newProvider);
                _this.model.LLMProviders = list;
                
                // Initialize showApiKey for the new provider
                const newIndex = list.length - 1;
                if (typeof _this.showApiKey === 'undefined') _this.showApiKey = {};
                _this.showApiKey[newIndex] = false;
                if (typeof _this.newModelName === 'undefined') _this.newModelName = {};
                _this.newModelName[newIndex] = '';
                
                // Close dropdown after adding provider
                this.$nextTick(() => {
                    try {
                        const dropdownButton = this.$el?.querySelector('#addProviderDropdown');
                        if (dropdownButton) {
                            const bootstrapLib = window.bootstrap || globalThis.bootstrap;
                            if (bootstrapLib && bootstrapLib.Dropdown) {
                                const dropdownInstance = bootstrapLib.Dropdown.getInstance(dropdownButton);
                                if (dropdownInstance) {
                                    dropdownInstance.hide();
                                } else {
                                    // Fallback: manually hide dropdown
                                    const dropdownMenu = dropdownButton.nextElementSibling;
                                    if (dropdownMenu && dropdownMenu.classList.contains('dropdown-menu')) {
                                        dropdownMenu.classList.remove('show');
                                        dropdownButton.classList.remove('show');
                                        dropdownButton.setAttribute('aria-expanded', 'false');
                                    }
                                }
                            } else {
                                // Fallback: manually hide dropdown
                                const dropdownMenu = dropdownButton.nextElementSibling;
                                if (dropdownMenu && dropdownMenu.classList.contains('dropdown-menu')) {
                                    dropdownMenu.classList.remove('show');
                                    dropdownButton.classList.remove('show');
                                    dropdownButton.setAttribute('aria-expanded', 'false');
                                }
                            }
                        }
                    } catch (e) {
                    }
                });
                
                if (_this.c && typeof _this.c.$forceUpdate === 'function') _this.c.$forceUpdate();
            },
            removeProvider(idx) { if (Array.isArray(_this.model.LLMProviders)) _this.model.LLMProviders.splice(idx, 1); },
            addModelStr(pidx) { var v = (_this.newModelName[pidx] || '').trim(); if (v==='') return; var p=_this.model.LLMProviders[pidx]; if(!Array.isArray(p.Models)) p.Models=[]; p.Models.push(v); _this.newModelName[pidx]=''; if (_this.c && typeof _this.c.$forceUpdate === 'function') _this.c.$forceUpdate(); },
            removeModelStr(pidx, midx) { var p=_this.model.LLMProviders[pidx]; if(!Array.isArray(p.Models)) return; p.Models.splice(midx,1); if (_this.c && typeof _this.c.$forceUpdate === 'function') _this.c.$forceUpdate(); },
            addPublicMethod() { var v = (_this.newPublicMethod || '').trim(); if(v==='') return; if(!_this.model.AAA) _this.model.AAA={}; if(!Array.isArray(_this.model.AAA.PublicMethods)) _this.model.AAA.PublicMethods=[]; _this.model.AAA.PublicMethods.push(v); _this.newPublicMethod=''; if (_this.c && typeof _this.c.$forceUpdate === 'function') _this.c.$forceUpdate(); },
            removePublicMethod(idx) { if(_this.model.AAA && Array.isArray(_this.model.AAA.PublicMethods)) _this.model.AAA.PublicMethods.splice(idx,1); if (_this.c && typeof _this.c.$forceUpdate === 'function') _this.c.$forceUpdate(); },
            addScheduledTask() {
                if (!Array.isArray(_this.model.ScheduledTasks)) _this.model.ScheduledTasks = [];
                var now = new Date().toISOString();
                _this.model.ScheduledTasks.push({
                    TaskId: 'task-' + Date.now(),
                    Name: '',
                    Description: '',
                    Enabled: false,
                    CronExpression: '*/10 * * * *',
                    MethodFullName: '',
                    MethodParameters: null,
                    CreatedOn: now,
                    CreatedBy: 'User'
                });
                if (_this.c && typeof _this.c.$forceUpdate === 'function') _this.c.$forceUpdate();
            },
            removeScheduledTask(idx) {
                if (Array.isArray(_this.model.ScheduledTasks)) {
                    _this.model.ScheduledTasks.splice(idx, 1);
                    if (_this.c && typeof _this.c.$forceUpdate === 'function') _this.c.$forceUpdate();
                }
            },
            openCronBuilder(idx) {
                const scheduledTask = _this.model.ScheduledTasks[idx];
                openComponent("/a.SharedComponents/CronBuilder", {
                    title: "Cron Expression Builder",
                    modalSize: "modal-lg",
                    params: {
                        cronExpression: scheduledTask.CronExpression || "*/10 * * * *",
                        callback: function (cronExpression) {
                            _this.model.ScheduledTasks[idx].CronExpression = cronExpression;
                            if (_this.c && typeof _this.c.$forceUpdate === 'function') _this.c.$forceUpdate();
                        }
                    }
                });
            },
            loadDbConnections() {
                _this.local.dbConnectionsLoading = true;
                rpc({
                    requests: [{
                        Method: "DefaultRepo.BaseDbConnections.ReadList",
                        Inputs: {
                            ClientQueryJE: {
                                QueryFullName: "DefaultRepo.BaseDbConnections.ReadList",
                                Pagination: { PageNumber: 1, PageSize: 1000 }
                            }
                        }
                    }],
                    onDone: function (res) {
                        try {
                            const result = R0R(res);
                            if (result && result.Master && Array.isArray(result.Master)) {
                                _this.local.dbConnections = result.Master;
                            } else {
                                _this.local.dbConnections = [];
                            }
                        } catch (e) {
                            _this.local.dbConnections = [];
                        }
                        _this.local.dbConnectionsLoading = false;
                        if (_this.c && typeof _this.c.$forceUpdate === 'function') {
                            _this.c.$forceUpdate();
                        }
                    },
                    onFail: function (err) {
                        _this.local.dbConnections = [];
                        _this.local.dbConnectionsLoading = false;
                        if (_this.c && typeof _this.c.$forceUpdate === 'function') _this.c.$forceUpdate();
                    }
                });
            },
            addDbServer() {
                // Add a new empty card to the UI immediately
                if (!Array.isArray(_this.local.dbConnections)) {
                    _this.local.dbConnections = [];
                }
                _this.local.dbConnections.push({
                    Id: null, // Temporary ID for new items
                    Name: '',
                    ServerType: 'MsSql',
                    ConnectionString: '',
                    Status: 'not_enriched',
                    EnrichmentProgress: 0
                });
                if (_this.c && typeof _this.c.$forceUpdate === 'function') {
                    _this.c.$forceUpdate();
                }
            },
            testDbServer(db) {
                // Validation
                if (!db.ConnectionString || (typeof db.ConnectionString === 'string' && db.ConnectionString.trim() === '')) {
                    showError('Please enter a connection string');
                    return;
                }
                if (!db.ServerType || (typeof db.ServerType === 'string' && db.ServerType.trim() === '')) {
                    showError('Please select a server type');
                    return;
                }
                
                const testData = {
                    Name: (db.Name || 'Test').trim(),
                    ServerType: (db.ServerType || 'MsSql').trim(),
                    ConnectionString: (db.ConnectionString || '').trim()
                };
                
                rpc({
                    requests: [{
                        Method: "Zzz.AppEndProxy.TestDbConnection",
                        Inputs: {
                            ServerInfo: testData
                        }
                    }],
                    onDone: function (res) {
                        try {
                            // Check if response has error
                            if (Array.isArray(res) && res.length > 0) {
                                const firstResponse = res[0];
                                if (firstResponse && firstResponse.IsSucceeded === false) {
                                    const errorMsg = firstResponse.ErrorMessage || firstResponse.Error || 'Connection test failed';
                                    showError(errorMsg);
                                    return;
                                }
                                if (firstResponse && firstResponse.Error) {
                                    const errorMsg = firstResponse.Error.Message || firstResponse.Error || 'Connection test failed';
                                    showError(errorMsg);
                                    return;
                                }
                            }
                            
                            const result = R0R(res);
                            if (result === true) {
                                showSuccess('Connection test successful!');
                            } else {
                                showError('Connection test failed');
                            }
                        } catch (e) {
                            showError('Connection test failed: ' + (e.message || e));
                        }
                    },
                    onFail: function (err) {
                        let errorMsg = 'Connection test failed';
                        
                        if (err) {
                            if (err.ErrorMessage) {
                                errorMsg = err.ErrorMessage;
                            } else if (err.Error && err.Error.Message) {
                                errorMsg = err.Error.Message;
                            } else if (err.message) {
                                errorMsg = err.message;
                            } else if (typeof err === 'string') {
                                errorMsg = err;
                            }
                        }
                        
                        showError(errorMsg);
                    }
                });
            },
            saveDbServer(db) {
                // Validation
                if (!db.Name || (typeof db.Name === 'string' && db.Name.trim() === '')) {
                    showError('Please enter a server name');
                    return;
                }
                if (!db.ConnectionString || (typeof db.ConnectionString === 'string' && db.ConnectionString.trim() === '')) {
                    showError('Please enter a connection string');
                    return;
                }
                if (!db.ServerType || (typeof db.ServerType === 'string' && db.ServerType.trim() === '')) {
                    showError('Please select a server type');
                    return;
                }
                
                // Check if this is a new item (no Id) or existing item
                const isNew = !db.Id || db.Id === null;
                const method = isNew ? "DefaultRepo.BaseDbConnections.Create" : "DefaultRepo.BaseDbConnections.UpdateByKey";
                
                const requestData = isNew ? {
                    Name: (db.Name || '').trim(),
                    ServerType: (db.ServerType || 'MsSql').trim(),
                    ConnectionString: (db.ConnectionString || '').trim(),
                    IsActive: db.IsActive !== undefined ? db.IsActive : true
                } : {
                    Id: db.Id,
                    Name: (db.Name || '').trim(),
                    ServerType: (db.ServerType || 'MsSql').trim(),
                    ConnectionString: (db.ConnectionString || '').trim(),
                    IsActive: db.IsActive !== undefined ? db.IsActive : true
                };
                
                rpc({
                    requests: [{
                        Method: method,
                        Inputs: {
                            ClientQueryJE: {
                                QueryFullName: method,
                                Data: requestData
                            }
                        }
                    }],
                    onDone: function (res) {
                        try {
                            // Check response structure - should be array with IsSucceeded property
                            let isSuccess = false;
                            let errorMessage = null;
                            
                            if (Array.isArray(res) && res.length > 0) {
                                const firstResponse = res[0];
                                
                                // Check for IsSucceeded property (standard AppEnd response)
                                if (firstResponse && firstResponse.IsSucceeded === true) {
                                    isSuccess = true;
                                } else if (firstResponse && firstResponse.IsSucceeded === false) {
                                    errorMessage = firstResponse.ErrorMessage || firstResponse.Error || 'Unknown error';
                                } else if (firstResponse && firstResponse.Error) {
                                    errorMessage = firstResponse.Error.Message || firstResponse.Error || 'Unknown error';
                                } else {
                                    // Fallback: check if Result exists
                                    const result = R0R(res);
                                    if (result && typeof result === 'object' && Object.keys(result).length > 0) {
                                        isSuccess = true;
                                    } else if (firstResponse && firstResponse.Result !== undefined) {
                                        isSuccess = true;
                                    }
                                }
                            } else if (res && res.IsSucceeded === true) {
                                isSuccess = true;
                            } else if (res && res.IsSucceeded === false) {
                                errorMessage = res.ErrorMessage || res.Error || 'Unknown error';
                            } else if (res && res.Result !== undefined) {
                                isSuccess = true;
                            }
                            
                            if (isSuccess) {
                                // If this was a new item (Create), update the Id from response
                                if (isNew) {
                                    try {
                                        const firstResponse = Array.isArray(res) && res.length > 0 ? res[0] : res;
                                        if (firstResponse && firstResponse.Result !== undefined && firstResponse.Result !== null) {
                                            // Create query returns the Id directly as a scalar value
                                            const resultValue = firstResponse.Result;
                                            if (typeof resultValue === 'number' || typeof resultValue === 'string') {
                                                // If result is directly the Id
                                                db.Id = resultValue;
                                            } else if (typeof resultValue === 'object' && resultValue.Id !== undefined && resultValue.Id !== null) {
                                                db.Id = resultValue.Id;
                                            }
                                        }
                                    } catch (e) {
                                        console.warn('Could not extract Id from response:', e);
                                    }
                                }
                                showSuccess('Database server saved successfully');
                                // Reload connections after a short delay to ensure data is saved
                                setTimeout(function() {
                                    _this.c.loadDbConnections();
                                }, 500);
                            } else {
                                const errorMsg = errorMessage || 'Failed to save database server - invalid response';
                                showError(errorMsg);
                            }
                        } catch (e) {
                            showError('Failed to save database server: ' + (e.message || e));
                        }
                    },
                    onFail: function (err) {
                        showError('Failed to save database server: ' + (err && err.message ? err.message : 'Unknown error'));
                    }
                });
            },
            saveAllDbServers() {
                if (!Array.isArray(_this.local.dbConnections) || _this.local.dbConnections.length === 0) {
                    showError('No database servers to save');
                    return;
                }
                
                // Validate all servers before saving
                for (let i = 0; i < _this.local.dbConnections.length; i++) {
                    const db = _this.local.dbConnections[i];
                    if (!db.Name || (typeof db.Name === 'string' && db.Name.trim() === '')) {
                        showError(`Please enter a server name for server ${i + 1}`);
                        return;
                    }
                    if (!db.ConnectionString || (typeof db.ConnectionString === 'string' && db.ConnectionString.trim() === '')) {
                        showError(`Please enter a connection string for server ${i + 1}`);
                        return;
                    }
                    if (!db.ServerType || (typeof db.ServerType === 'string' && db.ServerType.trim() === '')) {
                        showError(`Please select a server type for server ${i + 1}`);
                        return;
                    }
                }
                
                // Save all servers
                const requests = [];
                for (let i = 0; i < _this.local.dbConnections.length; i++) {
                    const db = _this.local.dbConnections[i];
                    const isNew = !db.Id || db.Id === null;
                    const method = isNew ? "DefaultRepo.BaseDbConnections.Create" : "DefaultRepo.BaseDbConnections.UpdateByKey";
                    
                    const requestData = isNew ? {
                        Name: (db.Name || '').trim(),
                        ServerType: (db.ServerType || 'MsSql').trim(),
                        ConnectionString: (db.ConnectionString || '').trim(),
                        IsActive: db.IsActive !== undefined ? db.IsActive : true
                    } : {
                        Id: db.Id,
                        Name: (db.Name || '').trim(),
                        ServerType: (db.ServerType || 'MsSql').trim(),
                        ConnectionString: (db.ConnectionString || '').trim(),
                        IsActive: db.IsActive !== undefined ? db.IsActive : true
                    };
                    
                    requests.push({
                        Method: method,
                        Inputs: {
                            ClientQueryJE: {
                                QueryFullName: method,
                                Data: requestData
                            }
                        }
                    });
                }
                
                rpc({
                    requests: requests,
                    onDone: function (res) {
                        try {
                            let allSuccess = true;
                            let errorMessages = [];
                            
                            if (Array.isArray(res)) {
                                for (let i = 0; i < res.length; i++) {
                                    const response = res[i];
                                    if (response && response.IsSucceeded === false) {
                                        allSuccess = false;
                                        const errorMsg = response.ErrorMessage || response.Error || 'Unknown error';
                                        errorMessages.push(`Server ${i + 1}: ${errorMsg}`);
                                    } else if (response && response.Error) {
                                        allSuccess = false;
                                        const errorMsg = response.Error.Message || response.Error || 'Unknown error';
                                        errorMessages.push(`Server ${i + 1}: ${errorMsg}`);
                                    } else if (response && response.IsSucceeded === true) {
                                        // If this was a new item (Create), update the Id from response
                                        const db = _this.local.dbConnections[i];
                                        const request = requests[i];
                                        const isNewItem = request && request.Inputs && request.Inputs.ClientQueryJE && 
                                                         request.Inputs.ClientQueryJE.Data && 
                                                         (!request.Inputs.ClientQueryJE.Data.Id || request.Inputs.ClientQueryJE.Data.Id === null);
                                        if (db && isNewItem && response.Result !== undefined && response.Result !== null) {
                                            try {
                                                // Create query returns the Id directly as a scalar value
                                                const resultValue = response.Result;
                                                if (typeof resultValue === 'number' || typeof resultValue === 'string') {
                                                    // If result is directly the Id
                                                    db.Id = resultValue;
                                                } else if (typeof resultValue === 'object' && resultValue.Id !== undefined && resultValue.Id !== null) {
                                                    db.Id = resultValue.Id;
                                                }
                                            } catch (e) {
                                                console.warn(`Could not extract Id from response for server ${i + 1}:`, e);
                                            }
                                        }
                                    }
                                }
                            }
                            
                            if (allSuccess) {
                                showSuccess('All database servers saved successfully');
                                // Reload connections after a short delay
                                setTimeout(function() {
                                    _this.c.loadDbConnections();
                                }, 500);
                            } else {
                                showError('Some servers failed to save:\n' + errorMessages.join('\n'));
                            }
                        } catch (e) {
                            showError('Failed to save database servers: ' + (e.message || e));
                        }
                    },
                    onFail: function (err) {
                        showError('Failed to save database servers: ' + (err && err.message ? err.message : 'Unknown error'));
                    }
                });
            },
            removeDbServer(db, idx) {
                if (!confirm('Are you sure you want to delete this database server?')) return;
                
                // If it's a new item (no Id), just remove it from the list
                if (!db.Id || db.Id === null) {
                    if (typeof idx === 'number' && idx >= 0 && idx < _this.local.dbConnections.length) {
                        _this.local.dbConnections.splice(idx, 1);
                        if (_this.c && typeof _this.c.$forceUpdate === 'function') {
                            _this.c.$forceUpdate();
                        }
                    }
                    return;
                }
                
                // For existing items, delete from database
                rpc({
                    requests: [{
                        Method: "DefaultRepo.BaseDbConnections.DeleteByKey",
                        Inputs: {
                            ClientQueryJE: {
                                QueryFullName: "DefaultRepo.BaseDbConnections.DeleteByKey",
                                Data: { Id: db.Id }
                            }
                        }
                    }],
                    onDone: function (res) {
                        showSuccess('Database server deleted successfully');
                        _this.c.loadDbConnections();
                    },
                    onFail: function (err) {
                        showError('Failed to delete database server');
                    }
                });
            }
        },
        setup(props) { _this.cid = props['cid']; },
        data() {
            try {
                let r = rpcSync({ requests: [{ Method: 'Zzz.AppEndProxy.GetAppEndSettings', Inputs: {} }] });
                let raw = R0R(r);
                _this.model = typeof raw === 'string' ? JSON.parse(raw) : (raw || {});
            } catch (ex) { _this.model = {}; }
            if (!_this.model.AAA) _this.model.AAA = {};
            if (!Array.isArray(_this.model.AAA.PublicMethods)) _this.model.AAA.PublicMethods = [];
            if (!_this.model.LLMProviders) _this.model.LLMProviders = [];
            _this.model.LLMProviders = _this.model.LLMProviders.map(function(p, idx){
                if (!Array.isArray(p.Models)) p.Models = [];
                p.Models = p.Models.map(function(m){ return (typeof m === 'string' ? m : (m && m.toString ? m.toString() : '')); });
                if (typeof p.ApiKey === 'undefined' || p.ApiKey === null) p.ApiKey = '';
                _this.showApiKey[idx] = false;
                return p;
            });
            if (!_this.model.ScheduledTasks) _this.model.ScheduledTasks = [];
            if (!_this.model.Serilog) _this.model.Serilog = {};
            // Load DbConnections from database
            if (_this.c && typeof _this.c.loadDbConnections === 'function') {
                _this.c.loadDbConnections();
            }
            return _this;
        },
        created() { _this.c = this; },
        mounted() { 
            initVueComponent(_this);
            // Load DbConnections when component is mounted
            if (this.loadDbConnections) {
                this.loadDbConnections();
            }
            initVueComponent(_this);            
        },
        props: { cid: String }
    };
</script>
