<template>
    <div class="p-3">
        <div class="mb-3">
            <label class="form-label fw-bold">Application Name</label>
            <input type="text" class="form-control" v-model="config.AppName" readonly />
        </div>
        <div class="mb-3">
            <label class="form-label fw-bold">Port</label>
            <input type="number" class="form-control" v-model="config.Port" />
        </div>
        <div class="mb-3">
            <label class="form-label fw-bold">Description</label>
            <textarea class="form-control" rows="3" v-model="config.Description"></textarea>
        </div>
        <div class="mb-3">
            <label class="form-label fw-bold">Environment Variables</label>
            <textarea class="form-control font-monospace" rows="5" v-model="config.EnvironmentVariables" 
                      placeholder="KEY1=VALUE1&#10;KEY2=VALUE2"></textarea>
            <div class="form-text">Enter one variable per line in format: KEY=VALUE</div>
        </div>
        <div class="mb-3">
            <label class="form-label fw-bold">Auto Start</label>
            <div class="form-check">
                <input class="form-check-input" type="checkbox" v-model="config.AutoStart" id="autoStart">
                <label class="form-check-label" for="autoStart">
                    Start automatically with base application
                </label>
            </div>
        </div>
        <hr />
        <div class="d-flex gap-2">
            <button class="btn btn-primary" @click="save">
                <i class="fa-solid fa-save"></i> Save Settings
            </button>
            <button class="btn btn-secondary" @click="cancel">
                <i class="fa-solid fa-times"></i> Cancel
            </button>
        </div>
    </div>
</template>

<script>
    shared.setAppTitle("Application Settings");
    let _this = { cid: "", c: null, inputs: {}, config: {} };
    export default {
        methods: {
            save() {
                _this.inputs.callback({
                    AppName: _this.c.config.AppName,
                    Port: _this.c.config.Port,
                    Description: _this.c.config.Description,
                    AutoStart: _this.c.config.AutoStart,
                    EnvironmentVariables: _this.c.config.EnvironmentVariables
                });
                closeComponent(_this.cid);
            },
            cancel() {
                closeComponent(_this.cid);
            },
            loadConfig() {
                rpcAEP("GetChildAppConfig", { "AppName": _this.inputs.AppName }, function (res) {
                    let config = R0R(res);
                    _this.c.config = {
                        AppName: config.AppName || _this.inputs.AppName,
                        Port: config.Port || 5000,
                        Description: config.Description || "",
                        AutoStart: config.AutoStart || false,
                        EnvironmentVariables: config.EnvironmentVariables || ""
                    };
                });
            }
        },
        setup(props) {
            _this.cid = props['cid'];
            _this.inputs = shared["params_" + _this.cid];
        },
        data() { return { config: _this.config }; },
        created() { _this.c = this; },
        mounted() { 
            initVueComponent(_this); 
            _this.c.loadConfig();
        },
        props: { cid: String }
    }
</script>
