<template>
    <div class="card border-0 shadow-lg bg-white rounded-0 h-100" id="formArea" data-ae-widget="inputsRegulator" data-ae-widget-options="{}">
        <div class="card-header bg-transparent p-2 border-0">
            <div class="container-fluid">
                <div class="row fs-d7">
                    <div class="col-12">
                        <div class="input-group input-group-sm">
                            <div class="input-group-text" style="min-width:100px;"><label>Name</label></div>
                            <input type="text" class="form-control form-control-sm" v-model="packageName" data-ae-validation-required="true" />
                        </div>
                    </div>
                    <div class="col-12">
                        <div class="input-group input-group-sm">
                            <div class="input-group-text" style="min-width:100px;"><label>Version</label></div>
                            <input type="text" class="form-control form-control-sm" v-model="packageInfo.Version" data-ae-validation-required="true" />
                        </div>
                    </div>
                    <div class="col-12">
                        <div class="input-group input-group-sm">
                            <div class="input-group-text" style="min-width:100px;"><label>Title</label></div>
                            <input type="text" class="form-control form-control-sm" v-model="packageInfo.Title" data-ae-validation-required="true" />
                        </div>
                    </div>
                    <div class="col-12">
                        <div class="input-group input-group-sm">
                            <div class="input-group-text" style="min-width:100px;"><label>Url</label></div>
                            <input type="text" class="form-control form-control-sm" v-model="packageInfo.Url" data-ae-validation-required="false" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="card-header bg-transparent p-2 border-0">
            <textarea class="form-control form-control-sm" v-model="packageInfo.Note" data-ae-validation-required="true" rows="2"></textarea>
        </div>
        <div class="card-body p-2 bg-transparent">
            <div class="container-fluid h-100">
                <div class="row g-1 h-100">
                    <div class="col-24 h-100">
                        <div class="card border-0 rounded-0 h-100">
                            <div class="card-header border-0 rounded-0 p-1 fs-d8 bg-transparent">
                                Install Sql
                            </div>
                            <div class="card-body p-0 border-0">
                                <div class="border border-2 rounded rounded-2 data-ae-validation h-100">
                                    <div class="code-editor-container h-100" data-ae-widget="editorBox" data-ae-widget-options="{&quot;mode&quot;: &quot;ace/mode/sql&quot;}" id="ace_installSql"></div>
                                    <input type="hidden" v-model="packageInfo.InstallSql" data-ae-validation-required="false" data-ae-validation-rule="" id="installSql" />
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-24 h-100">
                        <div class="card border-0 rounded-0 h-100">
                            <div class="card-header border-0 rounded-0 p-1 fs-d8 bg-transparent">
                                UnInstall Sql
                            </div>
                            <div class="card-body p-0 border-0">
                                <div class="border border-2 rounded rounded-2 data-ae-validation h-100">
                                    <div class="code-editor-container h-100" data-ae-widget="editorBox" data-ae-widget-options="{&quot;mode&quot;: &quot;ace/mode/sql&quot;}" id="ace_unInstallSql"></div>
                                    <input type="hidden" v-model="packageInfo.UnInstallSql" data-ae-validation-required="false" data-ae-validation-rule="" id="unInstallSql" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="card-footer bg-transparent p-2 border-0 rounded-0">
            <div class="container-fluid">
                <div class="row">
                    <div class="col-12">
                        <div class="input-group input-group-sm">
                            <div class="input-group-text" style="min-width:100px;"><label>CreatedBy</label></div>
                            <input type="text" class="form-control form-control-sm" v-model="packageInfo.CreatedBy" data-ae-validation-required="true" />
                        </div>
                    </div>
                    <div class="col-12">
                        <div class="input-group input-group-sm">
                            <div class="input-group-text" style="min-width:100px;"><label>CreatedOn</label></div>
                            <input type="text" class="form-control form-control-sm" v-model="packageInfo.CreatedOn" disabled />
                        </div>
                    </div>
                    <div class="col-12">
                        <div class="input-group input-group-sm">
                            <div class="input-group-text" style="min-width:100px;"><label>UpdatedBy</label></div>
                            <input type="text" class="form-control form-control-sm" v-model="packageInfo.UpdatedBy" data-ae-validation-required="true" />
                        </div>
                    </div>
                    <div class="col-12">
                        <div class="input-group input-group-sm">
                            <div class="input-group-text" style="min-width:100px;"><label>UpdatedOn</label></div>
                            <input type="text" class="form-control form-control-sm" v-model="packageInfo.UpdatedOn" disabled />
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="card-footer p-0 bg-secondary-subtle bg-gradient border-0 rounded-0">
            <button class="btn btn-link text-decoration-none bg-hover-light w-100 py-3 rounded-0" @click="ok" data-ae-key="ok">
                <i class="fa-solid fa-check me-2"></i><span>Ok</span>
            </button>
        </div>
    </div>
</template>

<script>
    let _this = { cid: "", c: null, inputs: {}, packageName: {}, packageInfo: {} };
    
    export default {
        methods: {
            ok(e) {
                if (!isAreaValidById("formArea")) return false;
                let ret = { packageName: _this.c.packageName, packageInfo: _this.c.packageInfo };
                if (_this.c.inputs.callback) _this.c.inputs.callback(ret);
                _this.c.close();
            },
            cancel() { _this.c.close(); },
            close() { shared.closeComponent(_this.cid); }
        },
        setup(props) {
            _this.cid = props['cid'];
            _this.inputs = shared["params_" + _this.cid];
            _this.packageName = _this.inputs["packageName"];
            _this.packageInfo = _this.inputs["packageInfo"];

            _this.packageInfo["CreatedOn"] = formatDateTime(_this.packageInfo["CreatedOn"]);
            _this.packageInfo["UpdatedOn"] = formatDateTime(_this.packageInfo["UpdatedOn"]);

        },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { initVueComponent(_this); },
        props: { cid: String }
    }
</script>
