<template>
    <div class="card h-100 bg-transparent rounded-0 border-0" id="formArea" data-ae-widget="inputsRegulator" data-ae-widget-options="{}">
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
        <div class="card-body p-0 d-flex" style="overflow: hidden; min-width:0; contain: layout paint size; transform: translateZ(0);">
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
            <div class="flex-grow-1 p-3 bg-light scrollable" style="min-width:0; contain: layout paint size;" role="tabpanel">
                
                <div v-if="activeCategory === 'general'" :id="`panel-general`" style="max-width:100%;">
                    <h5 class="mb-3"><i class="fa-solid fa-fa-cog text-secondary"></i> General</h5>

                    <div class="mb-3" style="max-width:100%;">
                        <div class="d-flex align-items-center gap-2 mb-1 flex-wrap">
                            <label for="talkpoint" class="form-label small text-secondary mb-0">TalkPoint <span class="text-danger">*</span></label>
                            <small class="text-muted" style="font-size:0.7rem;">Endpoint path for API calls</small>
                        </div>
                        <div class="data-ae-validation" style="max-width:100%;">
                            <input id="talkpoint" type="text" class="form-control form-control-sm" v-model="model.TalkPoint" style="max-width:100%;" data-ae-validation-required="true" data-ae-validation-rule=":=s(1,100)" />
                        </div>
                    </div>

                    <div class="mb-3" style="max-width:100%;">
                        <label for="defaultdbconfname" class="form-label small text-secondary">DefaultDbConfName <span class="text-danger">*</span></label>
                        <div class="data-ae-validation" style="max-width:100%;">
                            <input id="defaultdbconfname" type="text" class="form-control form-control-sm" v-model="model.DefaultDbConfName" style="max-width:100%;" data-ae-validation-required="true" data-ae-validation-rule=":=s(1,100)" />
                        </div>
                    </div>

                    <div class="mb-3" style="max-width:100%;">
                        <div class="d-flex align-items-center gap-2 mb-1 flex-wrap">
                            <label for="secret" class="form-label small text-secondary mb-0">Secret <span class="text-danger">*</span></label>
                            <small class="text-muted" style="font-size:0.7rem;">JWT signing key</small>
                        </div>
                        <div class="input-group input-group-sm" style="max-width:100%;">
                            <div class="data-ae-validation flex-grow-1" style="min-width:0;">
                                <input id="secret" :type="showSecret ? 'text' : 'password'" class="form-control" v-model="model.Secret" style="min-width:0;" data-ae-validation-required="true" data-ae-validation-rule=":=s(8,500)" />
                            </div>
                            <button class="btn btn-outline-secondary flex-shrink-0" @click="showSecret = !showSecret" :aria-label="showSecret ? 'Hide secret' : 'Show secret'" type="button">
                                {{ showSecret ? 'Hide' : 'Show' }}
                            </button>
                        </div>
                    </div>
                </div>

                <div v-else-if="activeCategory === 'aaa'" :id="`panel-aaa`" style="max-width:100%;">
                    <h5 class="mb-3"><i class="fa-solid fa-user-shield text-secondary"></i> AAA</h5>

                    <div class="mb-3" style="max-width:100%;">
                        <label for="logindbconfname" class="form-label small text-secondary">LoginDbConfName <span class="text-danger">*</span></label>
                        <div class="data-ae-validation" style="max-width:100%;">
                            <input id="logindbconfname" type="text" class="form-control form-control-sm" v-model="model.AAA.LoginDbConfName" style="max-width:100%;" data-ae-validation-required="true" data-ae-validation-rule=":=s(1,100)" />
                        </div>
                    </div>

                    <div class="mb-3" style="max-width:100%;">
                        <label for="publickeyuser" class="form-label small text-secondary">PublicKeyUser <span class="text-danger">*</span></label>
                        <div class="data-ae-validation" style="max-width:100%;">
                            <input id="publickeyuser" type="text" class="form-control form-control-sm" v-model="model.AAA.PublicKeyUser" style="max-width:100%;" data-ae-validation-required="true" data-ae-validation-rule=":=s(1,100)" />
                        </div>
                    </div>

                    <div class="mb-3" style="max-width:100%;">
                        <label for="publickeyrole" class="form-label small text-secondary">PublicKeyRole <span class="text-danger">*</span></label>
                        <div class="data-ae-validation" style="max-width:100%;">
                            <input id="publickeyrole" type="text" class="form-control form-control-sm" v-model="model.AAA.PublicKeyRole" style="max-width:100%;" data-ae-validation-required="true" data-ae-validation-rule=":=s(1,100)" />
                        </div>
                    </div>

                    <!-- PublicMethods box with list and add input inside -->
                    <div class="mb-3" style="max-width:100%;">
                        <div class="d-flex align-items-center gap-2 mb-1 flex-wrap">
                            <label class="form-label small text-secondary mb-0">PublicMethods</label>
                            <small class="text-muted" style="font-size:0.7rem;">Methods accessible without authentication</small>
                        </div>
                        <div class="border rounded shadow-sm bg-white" style="max-width:100%; ">
                            <div class="p-2">
                                <div v-if="model.AAA && model.AAA.PublicMethods && model.AAA.PublicMethods.length > 0" class="d-flex flex-wrap gap-1" role="list" aria-label="Current public methods">
                                    <span v-for="(m, idx) in model.AAA.PublicMethods" :key="idx"
                                          class="badge bg-light text-dark border d-inline-flex align-items-center gap-1 flex-shrink-0"
                                          role="listitem"
                                          :title="m"
                                          style="max-width: calc(100% - 8px); ">
                                        <code class="fs-d8" style="overflow: hidden; text-overflow: ellipsis; white-space: nowrap; max-width:180px;">{{m}}</code>
                                        <button class="btn btn-sm btn-link text-danger p-0 lh-1 flex-shrink-0" @click="removePublicMethod(idx)" :aria-label="`Remove ${m}`" type="button">
                                            <i class="fa-solid fa-times" aria-hidden="true"></i>
                                        </button>
                                    </span>
                                </div>
                                <div v-else class="text-muted small">No public methods configured</div>
                            </div>
                            <div class="border-top p-2">
                                <div class="input-group input-group-sm" style="max-width:100%;">
                                    <input type="text" class="form-control" v-model="newPublicMethod" placeholder="Namespace.Class.Method" @keyup.enter="addPublicMethod" style="min-width:0;" />
                                    <button class="btn btn-primary flex-shrink-0" @click="addPublicMethod" type="button" aria-label="Add public method">Add</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div v-else-if="activeCategory === 'serilog'" :id="`panel-serilog`" style="max-width:100%;">
                    <h5 class="mb-3"><i class="fa-solid fa-shoe-prints text-secondary"></i> Serilog</h5>

                    <div class="mb-3" style="max-width:100%;">
                        <label for="serilog-tablename" class="form-label small text-secondary">TableName <span class="text-danger">*</span></label>
                        <div class="data-ae-validation" style="max-width:100%;">
                            <input id="serilog-tablename" type="text" class="form-control form-control-sm" v-model="model.Serilog.TableName" style="max-width:100%;" data-ae-validation-required="true" data-ae-validation-rule=":=s(1,200)" />
                        </div>
                    </div>

                    <div class="mb-3" style="max-width:100%;">
                        <label for="serilog-connection" class="form-label small text-secondary">Connection <span class="text-danger">*</span></label>
                        <div class="data-ae-validation" style="max-width:100%;">
                            <input id="serilog-connection" type="text" class="form-control form-control-sm" v-model="model.Serilog.Connection" style="max-width:100%;" data-ae-validation-required="true" data-ae-validation-rule=":=s(1,100)" />
                        </div>
                    </div>

                    <div class="mb-3" style="max-width:100%;">
                        <label for="serilog-batchpostinglimit" class="form-label small text-secondary">BatchPostingLimit <span class="text-danger">*</span></label>
                        <div class="data-ae-validation" style="max-width:100%;">
                            <input id="serilog-batchpostinglimit" type="number" class="form-control form-control-sm" v-model.number="model.Serilog.BatchPostingLimit" min="1" style="max-width:100%;" data-ae-validation-required="true" data-ae-validation-rule=":=i(1,10000)" />
                        </div>
                    </div>

                    <div class="mb-3" style="max-width:100%;">
                        <label for="serilog-batchperiodseconds" class="form-label small text-secondary">BatchPeriodSeconds <span class="text-danger">*</span></label>
                        <div class="data-ae-validation" style="max-width:100%;">
                            <input id="serilog-batchperiodseconds" type="number" class="form-control form-control-sm" v-model.number="model.Serilog.BatchPeriodSeconds" min="1" style="max-width:100%;" data-ae-validation-required="true" data-ae-validation-rule=":=i(1,3600)" />
                        </div>
                    </div>
                </div>

                <div v-else-if="activeCategory === 'dbservers'" :id="`panel-dbservers`" style="max-width:100%;">
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
                                    </select>
                                </div>
                                <div class="mb-2">
                                    <label class="form-label small text-secondary mb-1">ConnectionString</label>
                                    <textarea class="form-control form-control-sm" v-model="db.ConnectionString" placeholder="Server=...;Database=...;..." rows="3" :aria-label="`Connection string ${idx + 1}`"></textarea>
                                </div>
                            </div>
                        </div>
                    </div>
                    <button class="btn btn-sm btn-primary mt-3" @click="addDbServer" type="button" aria-label="Add new database server">
                        <i class="fa-solid fa-plus me-1" aria-hidden="true"></i>Add Server
                    </button>
                </div>

                <div v-else-if="activeCategory === 'llmproviders'" :id="`panel-llmproviders`" style="max-width:100%;">
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

                                <div class="card mt-2 shadow-sm">
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
                            <i class="fa-solid fa-plus me-1" aria-hidden="true"></i>Add Provider
                        </button>
                        <ul class="dropdown-menu" aria-labelledby="addProviderDropdown">
                            <li>
                                <a class="dropdown-item" href="#" @click.prevent="addProvider('openai')">
                                    <i class="fa-solid fa-robot me-2"></i>OpenAI
                                </a>
                            </li>
                            <li>
                                <a class="dropdown-item" href="#" @click.prevent="addProvider('gemini')">
                                    <i class="fa-solid fa-gem me-2"></i>Gemini Direct
                                </a>
                            </li>
                            <li>
                                <a class="dropdown-item" href="#" @click.prevent="addProvider('custom')">
                                    <i class="fa-solid fa-gear me-2"></i>Custom
                                </a>
                            </li>
                        </ul>
                    </div>
                </div>

                <div v-else-if="activeCategory === 'scheduledtasks'" :id="`panel-scheduledtasks`" style="max-width:100%;">
                    <h5 class="mb-3"><i class="fa-solid fa-clock text-secondary"></i> Scheduled Tasks</h5>
                    <div class="d-flex flex-wrap gap-2">
                        <div v-for="(st, idx) in model.ScheduledTasks" :key="idx"
                             class="card bg-white shadow-sm" style="min-width:300px; max-width:520px; flex: 1 1 360px;">
                            <div class="card-header py-2 d-flex align-items-center justify-content-between">
                                <div class="d-flex align-items-center gap-2" style="flex:1; min-width:0;">
                                    <input type="text" class="form-control form-control-sm" v-model="st.Name" placeholder="Task name" style="flex:1; min-width:0;" data-ae-validation-required="true" data-ae-validation-rule=":=s(1,200)" />
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
                                    <input type="text" class="form-control font-monospace" v-model="st.CronExpression" placeholder="*/10 * * * *" data-ae-validation-required="true" data-ae-validation-rule=":=s(1,100)" />
                                    <button class="btn btn-secondary flex-shrink-0" type="button" @click="openCronBuilder(idx)" aria-label="Open cron builder">
                                        <i class="fa-solid fa-clock"></i>
                                    </button>
                                </div>
                            </div>
                            <div class="card-body py-2">
                                <div class="mb-2">
                                    <label class="form-label small text-secondary mb-1">TaskId <span class="text-danger">*</span></label>
                                    <div class="data-ae-validation" style="max-width:100%;">
                                        <input type="text" class="form-control form-control-sm" v-model="st.TaskId" placeholder="unique-task-id" data-ae-validation-required="true" data-ae-validation-rule=":=s(1,100)" />
                                    </div>
                                </div>
                                <div class="mb-2">
                                    <label class="form-label small text-secondary mb-1">Description</label>
                                    <textarea class="form-control form-control-sm" v-model="st.Description" placeholder="Task description..." rows="2" data-ae-validation-required="false"></textarea>
                                </div>
                                <div class="mb-2">
                                    <label class="form-label small text-secondary mb-1">Method Full Name <span class="text-danger">*</span></label>
                                    <div class="data-ae-validation" style="max-width:100%;">
                                        <input type="text" class="form-control form-control-sm" v-model="st.MethodFullName" placeholder="Namespace.Class.Method" data-ae-validation-required="true" data-ae-validation-rule=":=s(1,500)" />
                                    </div>
                                </div>
                                <div class="mb-2">
                                    <label class="form-label small text-secondary mb-1">Method Parameters (JSON)</label>
                                    <textarea class="form-control form-control-sm" v-model="st.MethodParameters" placeholder='{"param1": "value1"}' rows="2" data-ae-validation-required="false"></textarea>
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
        showSecret: false
    };
    export default {
        methods: {
            ok() {
                try {
                    if (!isAreaValidById("formArea")) return false;

                    let payload = JSON.parse(JSON.stringify(_this.model));
                    // Ensure AAA section exists
                    if (!payload.AAA) payload.AAA = {};

                    if (payload.DbServers && Array.isArray(payload.DbServers)) {
                        payload.DbServers = payload.DbServers.filter(function(db){ 
                            return (db.Name && db.Name.trim() !== '') || (db.ConnectionString && db.ConnectionString.trim() !== '');
                        });
                    }
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
                            console.error(err);
                        }
                    });
                } catch (ex) {
                    console.error('Save error', ex);
                    showError('Save error');
                }
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
                    if (!_this.model.DbServers) _this.model.DbServers = [];
                    if (!_this.model.Serilog) _this.model.Serilog = {};
                    
                    // Re-initialize validation after data refresh
                    if (_this.c && typeof _this.c.$forceUpdate === 'function') {
                        _this.c.$forceUpdate();
                        _this.c.$nextTick(() => {
                            isAreaValidById("formArea");
                        });
                    }
                    showSuccess('Refreshed');
                } catch (ex) {
                    console.error('Refresh error', ex);
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
                        console.error('Error closing dropdown:', e);
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
            addDbServer() {
                if (!Array.isArray(_this.model.DbServers)) _this.model.DbServers = [];
                _this.model.DbServers.push({
                    Name: '',
                    ServerType: 'MsSql',
                    ConnectionString: ''
                });
                if (_this.c && typeof _this.c.$forceUpdate === 'function') _this.c.$forceUpdate();
            },
            removeDbServer(idx) {
                if (Array.isArray(_this.model.DbServers)) {
                    _this.model.DbServers.splice(idx, 1);
                    if (_this.c && typeof _this.c.$forceUpdate === 'function') _this.c.$forceUpdate();
                }
            }
        },
        setup(props) { _this.cid = props['cid']; },
        data() {
            try {
                let r = rpcSync({ requests: [{ Method: 'Zzz.AppEndProxy.GetAppEndSettings', Inputs: {} }] });
                let raw = R0R(r);
                _this.model = typeof raw === 'string' ? JSON.parse(raw) : (raw || {});
            } catch (ex) { console.error('Load error', ex); _this.model = {}; }
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
            if (!_this.model.DbServers) _this.model.DbServers = [];
            if (!_this.model.Serilog) _this.model.Serilog = {};
            return _this;
        },
        created() { _this.c = this; },
        mounted() { 
            initVueComponent(_this);
        },
        props: { cid: String }
    };
</script>
