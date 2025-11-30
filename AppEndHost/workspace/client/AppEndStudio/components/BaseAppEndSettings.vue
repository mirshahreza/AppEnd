<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-header p-2 bg-primary-subtle-light rounded-0 border-0">
            <div class="fw-bold fs-d7">AppEnd Settings</div>
        </div>
        <div class="card-body p-2 scrollable">
            <div class="alert alert-warning py-2">
                <i class="fa-solid fa-triangle-exclamation me-1"></i>
                ??? ??????? ???? AppEnd ?? ?????? ??????.
            </div>

            <div class="mb-2">
                <label class="form-label text-secondary fs-d8">TalkPoint</label>
                <input type="text" class="form-control form-control-sm" v-model="model.TalkPoint" />
            </div>
            <div class="row g-2">
                <div class="col-24">
                    <label class="form-label text-secondary fs-d8">DefaultDbConfName</label>
                    <input type="text" class="form-control form-control-sm" v-model="model.DefaultDbConfName" />
                </div>
                <div class="col-24">
                    <label class="form-label text-secondary fs-d8">LoginDbConfName</label>
                    <input type="text" class="form-control form-control-sm" v-model="model.LoginDbConfName" />
                </div>
            </div>

            <div class="row g-2 mt-0">
                <div class="col-24">
                    <label class="form-label text-secondary fs-d8">LogDbConfName</label>
                    <input type="text" class="form-control form-control-sm" v-model="model.LogDbConfName" />
                </div>
                <div class="col-24">
                    <label class="form-label text-secondary fs-d8">LogWriterQueueCap</label>
                    <input type="number" class="form-control form-control-sm" v-model.number="model.LogWriterQueueCap" />
                </div>
            </div>

            <div class="mb-2">
                <label class="form-label text-secondary fs-d8">Secret</label>
                <input type="password" class="form-control form-control-sm" v-model="model.Secret" />
            </div>

            <div class="mb-2">
                <label class="form-label text-secondary fs-d8">PublicKeyRole</label>
                <input type="text" class="form-control form-control-sm" v-model="model.PublicKeyRole" />
            </div>
            <div class="mb-2">
                <label class="form-label text-secondary fs-d8">PublicKeyUser</label>
                <input type="text" class="form-control form-control-sm" v-model="model.PublicKeyUser" />
            </div>

            <div class="card mt-2">
                <div class="card-header p-2">
                    <div class="fw-bold fs-d7">PublicMethods</div>
                </div>
                <div class="card-body p-2">
                    <div class="input-group input-group-sm mb-2">
                        <input type="text" class="form-control form-control-sm" placeholder="Namespace.Class.Method" v-model="newPublicMethod" @keyup.enter="addPublicMethod" />
                        <button class="btn btn-sm btn-primary" @click="addPublicMethod">Add</button>
                    </div>
                    <div>
                        <span class="badge bg-light text-dark border me-1 mb-1" v-for="(m,idx) in model.PublicMethods" :key="idx">
                            <i class="fa-solid fa-code me-1"></i>{{m}}
                            <i class="fa-solid fa-times ms-1 pointer text-danger" @click="removePublicMethod(idx)"></i>
                        </span>
                    </div>
                </div>
            </div>

            <div class="card mt-2">
                <div class="card-header p-2"><div class="fw-bold fs-d7">Serilog</div></div>
                <div class="card-body p-2">
                    <div class="row g-2">
                        <div class="col-16">
                            <label class="form-label text-secondary fs-d8">TableName</label>
                            <input type="text" class="form-control form-control-sm" v-model="model.Serilog.TableName" />
                        </div>
                        <div class="col-16">
                            <label class="form-label text-secondary fs-d8">Connection</label>
                            <input type="text" class="form-control form-control-sm" v-model="model.Serilog.Connection" />
                        </div>
                        <div class="col-8">
                            <label class="form-label text-secondary fs-d8">BatchPostingLimit</label>
                            <input type="number" class="form-control form-control-sm" v-model.number="model.Serilog.BatchPostingLimit" />
                        </div>
                        <div class="col-8">
                            <label class="form-label text-secondary fs-d8">BatchPeriodSeconds</label>
                            <input type="number" class="form-control form-control-sm" v-model.number="model.Serilog.BatchPeriodSeconds" />
                        </div>
                    </div>
                </div>
            </div>

            <div class="card mt-2">
                <div class="card-header p-2">
                    <div class="fw-bold fs-d7">DbServers</div>
                </div>
                <div class="card-body p-2">
                    <div class="input-group input-group-sm mb-2">
                        <input type="text" class="form-control form-control-sm" placeholder="Name" v-model="newDb.Name" />
                        <select class="form-select form-select-sm" v-model="newDb.ServerType">
                            <option>MsSql</option>
                            <option>PostgreSql</option>
                            <option>MySql</option>
                        </select>
                        <input type="text" class="form-control form-control-sm" placeholder="ConnectionString" v-model="newDb.ConnectionString" />
                        <button class="btn btn-sm btn-primary" @click="addDbServer">Add</button>
                    </div>
                    <div class="table-responsive">
                        <table class="table table-sm table-bordered">
                            <thead>
                                <tr>
                                    <th>Name</th>
                                    <th>ServerType</th>
                                    <th>ConnectionString</th>
                                    <th style="width:1px"></th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="(d,idx) in model.DbServers" :key="idx">
                                    <td><input type="text" class="form-control form-control-sm" v-model="d.Name" /></td>
                                    <td>
                                        <select class="form-select form-select-sm" v-model="d.ServerType">
                                            <option>MsSql</option>
                                            <option>PostgreSql</option>
                                            <option>MySql</option>
                                        </select>
                                    </td>
                                    <td><input type="text" class="form-control form-control-sm" v-model="d.ConnectionString" /></td>
                                    <td>
                                        <button class="btn btn-sm btn-danger" @click="removeDbServer(idx)"><i class="fa-solid fa-trash"></i></button>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
        <div class="card-footer p-3 bg-secondary-subtle bg-gradient border-0 rounded-0">
            <div class="row">
                <div class="col-24">
                    <button class="btn btn-sm btn-secondary w-100 py-2" @click="cancel">
                        <i class="fa-solid fa-cancel"></i>
                        <span>Cancel</span>
                    </button>
                </div>
                <div class="col-24">
                    <button class="btn btn-sm btn-primary w-100 py-2" @click="ok">
                        <i class="fa-solid fa-check"></i>
                        <span>Save</span>
                    </button>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    let _this = { cid: '', c: null, model: {}, newPublicMethod: '', newDb: { Name: '', ServerType: 'MsSql', ConnectionString: '' } };
    export default {
        methods: {
            addPublicMethod() {
                let v = _this.newPublicMethod.trim();
                if (v === '') return;
                if (!_this.model.PublicMethods) _this.model.PublicMethods = [];
                _this.model.PublicMethods.push(v);
                _this.newPublicMethod = '';
            },
            removePublicMethod(idx) {
                _this.model.PublicMethods.splice(idx, 1);
            },
            addDbServer() {
                if (!_this.model.DbServers) _this.model.DbServers = [];
                let item = JSON.parse(JSON.stringify(_this.newDb));
                _this.model.DbServers.push(item);
                _this.newDb = { Name: '', ServerType: 'MsSql', ConnectionString: '' };
            },
            removeDbServer(idx) {
                _this.model.DbServers.splice(idx, 1);
            },
            ok() {
                rpc({
                    requests: [{
                        Method: 'Zzz.AppEndProxy.SaveAppEndSettings',
                        Inputs: { AppEnd: _this.model }
                    }],
                    onDone: function (res) {
                        if (R0R(res) === true) {
                            showSuccess('Saved');
                            shared.closeComponent(_this.cid);
                        } else {
                            showError('Error');
                        }
                    }
                });
            },
            cancel() { shared.closeComponent(_this.cid); }
        },
        setup(props) { _this.cid = props['cid']; },
        data() {
            let r = rpcSync({ requests: [{ Method: 'Zzz.AppEndProxy.GetAppEndSettings', Inputs: {} }] });
            _this.model = R0R(r) || {};
            // Ensure objects exist
            if (!_this.model.Serilog) _this.model.Serilog = { TableName: 'BaseActivityLog', Connection: 'DefaultRepo', BatchPostingLimit: 100, BatchPeriodSeconds: 15 };
            if (!_this.model.DbServers) _this.model.DbServers = [];
            return _this;
        },
        created() { _this.c = this; },
        mounted() { },
        props: { cid: String }
    };
</script>
