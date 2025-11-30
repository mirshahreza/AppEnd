<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-header p-2 bg-primary-subtle-light rounded-0 border-0">
            <div class="d-flex align-items-center gap-2">
                <button class="btn btn-sm btn-secondary" @click="cancel" aria-label="Cancel">
                    <i class="fa-solid fa-xmark me-1" aria-hidden="true"></i>Cancel
                </button>
                <button class="btn btn-sm btn-primary" @click="ok" aria-label="Save">
                    <i class="fa-solid fa-check me-1" aria-hidden="true"></i>Save
                </button>
            </div>
        </div>
        <div class="card-body p-0 d-flex" style="overflow: hidden; min-width:0; contain: layout paint size; transform: translateZ(0);">
            <!-- Left Sidebar: Category tree -->
            <nav class="border-end bg-white flex-shrink-0 scrollable" style="width:220px; min-width:220px; contain: layout paint;" aria-label="Settings categories">
                <div class="list-group list-group-flush" role="tablist">
                    <button v-for="cat in categories" :key="cat.key" 
                            class="list-group-item list-group-item-action border-0 py-2 px-3 text-start text-truncate"
                            :class="{'active': activeCategory === cat.key}"
                            @click="activeCategory = cat.key"
                            role="tab"
                            :aria-selected="activeCategory === cat.key"
                            :aria-controls="`panel-${cat.key}`"
                            :title="cat.label">
                        <i :class="cat.icon + ' me-2'" aria-hidden="true"></i>{{cat.label}}
                    </button>
                </div>
            </nav>

            <!-- Right Panel: Property editor -->
            <div class="flex-grow-1 p-3 bg-light scrollable" style="min-width:0; contain: layout paint size;" role="tabpanel">
                <div v-if="activeCategory === 'general'" :id="`panel-general`" style="max-width:100%;">
                    <h5 class="mb-3">General</h5>
                    
                    <div class="mb-3" style="max-width:100%;">
                        <div class="d-flex align-items-center gap-2 mb-1 flex-wrap">
                            <label for="talkpoint" class="form-label small text-secondary mb-0">TalkPoint</label>
                            <small class="text-muted" style="font-size:0.7rem;">Endpoint path for API calls</small>
                        </div>
                        <input id="talkpoint" type="text" class="form-control form-control-sm" v-model="model.TalkPoint" style="max-width:100%;" />
                    </div>

                    <div class="mb-3" style="max-width:100%;">
                        <div class="d-flex align-items-center gap-2 mb-1 flex-wrap">
                            <label for="logwriterqueuecap" class="form-label small text-secondary mb-0">LogWriterQueueCap</label>
                            <small class="text-muted" style="font-size:0.7rem;">Max queue size</small>
                        </div>
                        <input id="logwriterqueuecap" type="number" class="form-control form-control-sm" v-model.number="model.LogWriterQueueCap" min="0" style="max-width:100%;" />
                    </div>

                    <div class="mb-3" style="max-width:100%;">
                        <label for="defaultdbconfname" class="form-label small text-secondary">DefaultDbConfName</label>
                        <input id="defaultdbconfname" type="text" class="form-control form-control-sm" v-model="model.DefaultDbConfName" style="max-width:100%;" />
                    </div>

                    <div class="mb-3" style="max-width:100%;">
                        <label for="logindbconfname" class="form-label small text-secondary">LoginDbConfName</label>
                        <input id="logindbconfname" type="text" class="form-control form-control-sm" v-model="model.LoginDbConfName" style="max-width:100%;" />
                    </div>

                    <div class="mb-3" style="max-width:100%;">
                        <label for="logdbconfname" class="form-label small text-secondary">LogDbConfName</label>
                        <input id="logdbconfname" type="text" class="form-control form-control-sm" v-model="model.LogDbConfName" style="max-width:100%;" />
                    </div>

                    <div class="mb-3" style="max-width:100%;">
                        <div class="d-flex align-items-center gap-2 mb-1 flex-wrap">
                            <label for="secret" class="form-label small text-secondary mb-0">Secret</label>
                            <small class="text-muted" style="font-size:0.7rem;">JWT signing key</small>
                        </div>
                        <div class="input-group input-group-sm" style="max-width:100%;">
                            <input id="secret" :type="showSecret ? 'text' : 'password'" class="form-control" v-model="model.Secret" style="min-width:0;" />
                            <button class="btn btn-outline-secondary flex-shrink-0" @click="showSecret = !showSecret" :aria-label="showSecret ? 'Hide secret' : 'Show secret'" type="button">
                                {{ showSecret ? 'Hide' : 'Show' }}
                            </button>
                        </div>
                    </div>
                </div>

                <div v-else-if="activeCategory === 'auth'" :id="`panel-auth`" style="max-width:100%;">
                    <h5 class="mb-3">Authentication</h5>
                    
                    <div class="mb-3" style="max-width:100%;">
                        <div class="d-flex align-items-center gap-2 mb-1 flex-wrap">
                            <label for="publickeyuser" class="form-label small text-secondary mb-0">PublicKeyUser</label>
                            <small class="text-muted" style="font-size:0.7rem;">Admin user name</small>
                        </div>
                        <input id="publickeyuser" type="text" class="form-control form-control-sm" v-model="model.PublicKeyUser" style="max-width:100%;" />
                    </div>

                    <div class="mb-3" style="max-width:100%;">
                        <div class="d-flex align-items-center gap-2 mb-1 flex-wrap">
                            <label for="publickeyrole" class="form-label small text-secondary mb-0">PublicKeyRole</label>
                            <small class="text-muted" style="font-size:0.7rem;">Admin role name</small>
                        </div>
                        <input id="publickeyrole" type="text" class="form-control form-control-sm" v-model="model.PublicKeyRole" style="max-width:100%;" />
                    </div>

                    <div class="mb-3" style="max-width:100%;">
                        <div class="d-flex align-items-center gap-2 mb-1 flex-wrap">
                            <label for="publicmethods-input" class="form-label small text-secondary mb-0">PublicMethods</label>
                            <small class="text-muted" style="font-size:0.7rem;">Methods accessible without authentication</small>
                        </div>
                        <div class="border rounded p-2 bg-white mb-2" style="max-height: 200px; max-width:100%; overflow-y: auto; overflow-x: hidden;">
                            <div v-if="model.PublicMethods && model.PublicMethods.length > 0" class="d-flex flex-wrap gap-1" role="list" aria-label="Current public methods">
                                <span v-for="(m, idx) in model.PublicMethods" :key="idx" 
                                      class="badge bg-light text-dark border d-inline-flex align-items-center gap-1 flex-shrink-0"
                                      role="listitem"
                                      :title="m"
                                      style="max-width: calc(100% - 8px);">
                                    <code class="fs-d8" style="overflow: hidden; text-overflow: ellipsis; white-space: nowrap; max-width:180px;">{{m}}</code>
                                    <button class="btn btn-sm btn-link text-danger p-0 lh-1 flex-shrink-0" @click="removePublicMethod(idx)" :aria-label="`Remove ${m}`" type="button">
                                        <i class="fa-solid fa-times" aria-hidden="true"></i>
                                    </button>
                                </span>
                            </div>
                            <div v-else class="text-muted small">No public methods configured</div>
                        </div>
                        <div class="input-group input-group-sm" style="max-width:100%;">
                            <input id="publicmethods-input" type="text" class="form-control" v-model="newPublicMethod" placeholder="Namespace.Class.Method" @keyup.enter="addPublicMethod" style="min-width:0;" />
                            <button class="btn btn-primary flex-shrink-0" @click="addPublicMethod" type="button" aria-label="Add public method">Add</button>
                        </div>
                    </div>
                </div>

                <div v-else-if="activeCategory === 'serilog'" :id="`panel-serilog`" style="max-width:100%;">
                    <h5 class="mb-3">Serilog</h5>
                    
                    <div class="mb-3" style="max-width:100%;">
                        <label for="serilog-tablename" class="form-label small text-secondary">TableName</label>
                        <input id="serilog-tablename" type="text" class="form-control form-control-sm" v-model="model.Serilog.TableName" style="max-width:100%;" />
                    </div>

                    <div class="mb-3" style="max-width:100%;">
                        <label for="serilog-connection" class="form-label small text-secondary">Connection</label>
                        <input id="serilog-connection" type="text" class="form-control form-control-sm" v-model="model.Serilog.Connection" style="max-width:100%;" />
                    </div>

                    <div class="mb-3" style="max-width:100%;">
                        <label for="serilog-batchpostinglimit" class="form-label small text-secondary">BatchPostingLimit</label>
                        <input id="serilog-batchpostinglimit" type="number" class="form-control form-control-sm" v-model.number="model.Serilog.BatchPostingLimit" min="1" style="max-width:100%;" />
                    </div>

                    <div class="mb-3" style="max-width:100%;">
                        <label for="serilog-batchperiodseconds" class="form-label small text-secondary">BatchPeriodSeconds</label>
                        <input id="serilog-batchperiodseconds" type="number" class="form-control form-control-sm" v-model.number="model.Serilog.BatchPeriodSeconds" min="1" style="max-width:100%;" />
                    </div>
                </div>

                <div v-else-if="activeCategory === 'dbservers'" :id="`panel-dbservers`" style="max-width:100%;">
                    <h5 class="mb-3">Database Servers</h5>
                    <div class="table-responsive" style="max-width:100%; overflow-x:auto;">
                        <table class="table table-sm table-bordered bg-white" aria-label="Database servers configuration" style="min-width:600px;">
                            <thead class="table-light">
                                <tr>
                                    <th scope="col" style="width:180px;">Name</th>
                                    <th scope="col" style="width:120px;">ServerType</th>
                                    <th scope="col">ConnectionString</th>
                                    <th scope="col" style="width:60px;"><span class="visually-hidden">Actions</span></th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="(db, idx) in model.DbServers" :key="idx">
                                    <td><input type="text" class="form-control form-control-sm" v-model="db.Name" :aria-label="`Database server name ${idx + 1}`" /></td>
                                    <td>
                                        <select class="form-select form-select-sm" v-model="db.ServerType" :aria-label="`Server type ${idx + 1}`">
                                            <option>MsSql</option>
                                            <option>PostgreSql</option>
                                            <option>MySql</option>
                                        </select>
                                    </td>
                                    <td><input type="text" class="form-control form-control-sm" v-model="db.ConnectionString" :aria-label="`Connection string ${idx + 1}`" /></td>
                                    <td class="text-center">
                                        <button class="btn btn-sm btn-danger" @click="removeDbServer(idx)" :aria-label="`Remove database server ${db.Name || idx + 1}`" type="button">
                                            <i class="fa-solid fa-trash" aria-hidden="true"></i>
                                        </button>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                    <button class="btn btn-sm btn-primary mt-2" @click="addDbServer" type="button" aria-label="Add new database server">
                        <i class="fa-solid fa-plus me-1" aria-hidden="true"></i>Add Server
                    </button>
                </div>

                <div v-else-if="activeCategory === 'ai'" :id="`panel-ai`" style="max-width:100%;">
                    <h5 class="mb-3">AI (GitHub Copilot)</h5>
                    
                    <div class="mb-3" style="max-width:100%;">
                        <div class="d-flex align-items-center gap-2 mb-1 flex-wrap">
                            <label for="ai-apikey" class="form-label small text-secondary mb-0">ApiKey</label>
                            <small class="text-muted" style="font-size:0.7rem;">GitHub PAT token</small>
                        </div>
                        <div class="input-group input-group-sm" style="max-width:100%;">
                            <input id="ai-apikey" :type="showApiKey ? 'text' : 'password'" class="form-control" v-model="model.Ai.GitHub.ApiKey" style="min-width:0;" />
                            <button class="btn btn-outline-secondary flex-shrink-0" @click="showApiKey = !showApiKey" :aria-label="showApiKey ? 'Hide API key' : 'Show API key'" type="button">
                                {{ showApiKey ? 'Hide' : 'Show' }}
                            </button>
                        </div>
                    </div>

                    <div class="mb-3" style="max-width:100%;">
                        <label for="ai-baseurl" class="form-label small text-secondary">BaseUrl</label>
                        <input id="ai-baseurl" type="url" class="form-control form-control-sm" v-model="model.Ai.GitHub.BaseUrl" style="max-width:100%;" />
                    </div>

                    <div class="mb-3" style="max-width:100%;">
                        <label for="ai-timeout" class="form-label small text-secondary">TimeoutSeconds</label>
                        <input id="ai-timeout" type="number" class="form-control form-control-sm" v-model.number="model.Ai.GitHub.TimeoutSeconds" min="1" max="300" style="max-width:100%;" />
                    </div>

                    <div class="mb-3" style="max-width:100%;">
                        <label for="ai-models-input" class="form-label small text-secondary">Models</label>
                        <div class="border rounded p-2 bg-white mb-2">
                            <div v-if="model.Ai.GitHub.Models && model.Ai.GitHub.Models.length > 0" class="d-flex flex-wrap gap-1" role="list" aria-label="Current AI models">
                                <span v-for="(m, idx) in model.Ai.GitHub.Models" :key="idx"
                                      class="badge bg-light text-dark border d-inline-flex align-items-center gap-1 flex-shrink-0"
                                      role="listitem"
                                      :title="m"
                                      style="max-width: calc(100% - 8px);">
                                    <code class="fs-d8" style="overflow: hidden; text-overflow: ellipsis; white-space: nowrap; max-width: 150px;">{{m}}</code>
                                    <button class="btn btn-sm btn-link text-danger p-0 lh-1 flex-shrink-0" @click="removeModel(idx)" :aria-label="`Remove model ${m}`" type="button">
                                        <i class="fa-solid fa-times" aria-hidden="true"></i>
                                    </button>
                                </span>
                            </div>
                            <div v-else class="text-muted small">No models configured</div>
                        </div>
                        <div class="input-group input-group-sm" style="max-width:100%;">
                            <input id="ai-models-input" type="text" class="form-control" v-model="newModel" placeholder="model name" @keyup.enter="addModel" style="min-width:0;" />
                            <button class="btn btn-primary flex-shrink-0" @click="addModel" type="button" aria-label="Add AI model">Add</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    shared.setAppTitle("$auto$");

    let _this = { cid: '', c: null, model: {}, showSecret: false, showApiKey: false, newPublicMethod: '', newModel: '', activeCategory: 'general', 
        categories: [
            { key: 'general', label: 'General', icon: 'fa-solid fa-cog' },
            { key: 'auth', label: 'Authentication', icon: 'fa-solid fa-lock' },
            { key: 'serilog', label: 'Serilog', icon: 'fa-solid fa-file-lines' },
            { key: 'dbservers', label: 'Database Servers', icon: 'fa-solid fa-database' },
            { key: 'ai', label: 'AI Settings', icon: 'fa-solid fa-robot' }
        ]
    };
    export default {
        methods: {
            ok() {
                // Deep clone and clean model before sending
                let payload = JSON.parse(JSON.stringify(_this.model));
                
                // Filter out empty DbServers (rows with empty Name AND ConnectionString)
                if (payload.DbServers && Array.isArray(payload.DbServers)) {
                    payload.DbServers = payload.DbServers.filter(db => 
                        (db.Name && db.Name.trim() !== '') || (db.ConnectionString && db.ConnectionString.trim() !== '')
                    );
                }
                
                // Filter out empty PublicMethods
                if (payload.PublicMethods && Array.isArray(payload.PublicMethods)) {
                    payload.PublicMethods = payload.PublicMethods.filter(m => m && m.trim() !== '');
                }
                
                // Filter out empty AI Models
                if (payload.Ai?.GitHub?.Models && Array.isArray(payload.Ai.GitHub.Models)) {
                    payload.Ai.GitHub.Models = payload.Ai.GitHub.Models.filter(m => m && m.trim() !== '');
                }
                
                rpc({
                    requests: [{ Method: 'Zzz.AppEndProxy.SaveAppEndSettings', Inputs: { AppEnd: payload } }],
                    onDone(res) {
                        let result = R0R(res);
                        if (result === true) {
                            showSuccess('Saved');
                            shared.closeComponent(_this.cid);
                        } else {
                            showError('Save failed');
                        }
                    },
                    onFail(err) {
                        showError('Save error');
                        console.error(err);
                    }
                });
            },
            cancel() { shared.closeComponent(_this.cid); },
            addPublicMethod() {
                let v = _this.newPublicMethod.trim();
                if (v === '') return;
                if (!_this.model.PublicMethods) _this.model.PublicMethods = [];
                _this.model.PublicMethods.push(v);
                _this.newPublicMethod = '';
            },
            removePublicMethod(idx) { 
                if (_this.model.PublicMethods) _this.model.PublicMethods.splice(idx, 1); 
            },
            addModel() {
                let v = _this.newModel.trim();
                if (v === '') return;
                if (!_this.model.Ai) _this.model.Ai = {};
                if (!_this.model.Ai.GitHub) _this.model.Ai.GitHub = { Models: [] };
                if (!_this.model.Ai.GitHub.Models) _this.model.Ai.GitHub.Models = [];
                _this.model.Ai.GitHub.Models.push(v);
                _this.newModel = '';
            },
            removeModel(idx) { 
                if (_this.model.Ai?.GitHub?.Models) {
                    _this.model.Ai.GitHub.Models.splice(idx,1); 
                }
            },
            addDbServer() {
                if (!_this.model.DbServers) _this.model.DbServers = [];
                _this.model.DbServers.push({ Name: '', ServerType: 'MsSql', ConnectionString: '' });
            },
            removeDbServer(idx) { 
                if (_this.model.DbServers) _this.model.DbServers.splice(idx, 1); 
            }
        },
        setup(props) { _this.cid = props['cid']; },
        data() {
            try {
                let r = rpcSync({ requests: [{ Method: 'Zzz.AppEndProxy.GetAppEndSettings', Inputs: {} }] });
                let raw = R0R(r);
                _this.model = typeof raw === 'string' ? JSON.parse(raw) : (raw || {});
            } catch (ex) {
                console.error('Load error', ex);
                _this.model = {};
            }

            // Ensure defaults with safe navigation
            if (!_this.model.Serilog) _this.model.Serilog = {};
            if (!_this.model.Serilog.TableName) _this.model.Serilog.TableName = 'BaseActivityLog';
            if (!_this.model.Serilog.Connection) _this.model.Serilog.Connection = 'DefaultRepo';
            if (_this.model.Serilog.BatchPostingLimit === undefined) _this.model.Serilog.BatchPostingLimit = 100;
            if (_this.model.Serilog.BatchPeriodSeconds === undefined) _this.model.Serilog.BatchPeriodSeconds = 15;

            if (!_this.model.DbServers) _this.model.DbServers = [];
            if (!_this.model.PublicMethods) _this.model.PublicMethods = [];
            
            if (!_this.model.Ai) _this.model.Ai = {};
            if (!_this.model.Ai.GitHub) _this.model.Ai.GitHub = {};
            if (!_this.model.Ai.GitHub.ApiKey) _this.model.Ai.GitHub.ApiKey = '';
            if (!_this.model.Ai.GitHub.BaseUrl) _this.model.Ai.GitHub.BaseUrl = '';
            if (_this.model.Ai.GitHub.TimeoutSeconds === undefined) _this.model.Ai.GitHub.TimeoutSeconds = 30;
            if (!_this.model.Ai.GitHub.Models) _this.model.Ai.GitHub.Models = [];

            return _this;
        },
        created() { _this.c = this; },
        props: { cid: String }
    };
</script>
