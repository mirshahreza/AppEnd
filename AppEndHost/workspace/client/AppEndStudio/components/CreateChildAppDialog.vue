<template>
    <div class="p-3" style="min-width:500px;">
        <div class="mb-3">
            <label class="form-label fw-bold">Application Name <span class="text-danger">*</span></label>
            <input type="text" class="form-control" v-model="config.AppName" 
                   placeholder="e.g., MyApp" @input="validateAppName" />
            <div class="form-text">No spaces or special characters allowed</div>
            <div v-if="errors.AppName" class="text-danger fs-d9">{{ errors.AppName }}</div>
        </div>
        
        <div class="mb-3">
            <label class="form-label fw-bold">Port <span class="text-danger">*</span></label>
            <input type="number" class="form-control" v-model="config.Port" min="1024" max="65535" />
            <div class="form-text">Suggested: {{ inputs.suggestedPort }}</div>
            <div v-if="errors.Port" class="text-danger fs-d9">{{ errors.Port }}</div>
        </div>
        
        <div class="mb-3">
            <label class="form-label fw-bold">Description</label>
            <textarea class="form-control" rows="2" v-model="config.Description" 
                      placeholder="Brief description of the application"></textarea>
        </div>
        
        <div class="mb-3">
            <label class="form-label fw-bold">Template <span class="text-danger">*</span></label>
            <div class="row g-2">
                <div class="col-24">
                    <div class="card pointer" :class="{'border-primary': config.Template === 'EmptyWebApi'}" 
                         @click="config.Template = 'EmptyWebApi'">
                        <div class="card-body">
                            <div class="hstack gap-2">
                                <i class="fa-solid fa-circle-check text-primary" v-if="config.Template === 'EmptyWebApi'"></i>
                                <i class="fa-regular fa-circle text-muted" v-else></i>
                                <div>
                                    <div class="fw-bold">Empty Web API</div>
                                    <div class="text-muted fs-d9">Minimal ASP.NET Core Web API project</div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-24">
                    <div class="card pointer" :class="{'border-primary': config.Template === 'ApiWithClient'}" 
                         @click="config.Template = 'ApiWithClient'">
                        <div class="card-body">
                            <div class="hstack gap-2">
                                <i class="fa-solid fa-circle-check text-primary" v-if="config.Template === 'ApiWithClient'"></i>
                                <i class="fa-regular fa-circle text-muted" v-else></i>
                                <div>
                                    <div class="fw-bold">API + Client App</div>
                                    <div class="text-muted fs-d9">Web API with test endpoint + HTML client page</div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        
        <hr />
        <div class="d-flex gap-2">
            <button class="btn btn-primary" @click="create" :disabled="!isValid">
                <i class="fa-solid fa-plus"></i> Create Application
            </button>
            <button class="btn btn-secondary" @click="cancel">
                <i class="fa-solid fa-times"></i> Cancel
            </button>
        </div>
    </div>
</template>

<script>
    shared.setAppTitle("Create Child Application");
    let _this = { cid: "", c: null, inputs: {}, config: {}, errors: {} };
    export default {
        methods: {
            validateAppName() {
                _this.c.errors.AppName = "";
                if (!_this.c.config.AppName || _this.c.config.AppName.trim() === '') {
                    _this.c.errors.AppName = "Application name is required";
                    return false;
                }
                if (!/^[a-zA-Z0-9_-]+$/.test(_this.c.config.AppName)) {
                    _this.c.errors.AppName = "Only letters, numbers, hyphens and underscores allowed";
                    return false;
                }
                return true;
            },
            validatePort() {
                _this.c.errors.Port = "";
                const port = parseInt(_this.c.config.Port);
                if (isNaN(port) || port < 1024 || port > 65535) {
                    _this.c.errors.Port = "Port must be between 1024 and 65535";
                    return false;
                }
                return true;
            },
            create() {
                if (!_this.c.validateAppName()) return;
                if (!_this.c.validatePort()) return;
                if (!_this.c.config.Template) {
                    showError("Please select a template");
                    return;
                }
                
                if (_this.inputs.callback) {
                    _this.inputs.callback(_this.c.config);
                }
                closeComponent(_this.cid);
            },
            cancel() {
                closeComponent(_this.cid);
            }
        },
        computed: {
            isValid() {
                return this.config.AppName && 
                       this.config.Port && 
                       this.config.Template &&
                       !this.errors.AppName &&
                       !this.errors.Port;
            }
        },
        setup(props) {
            _this.cid = props['cid'];
            _this.inputs = shared["params_" + _this.cid];
        },
        data() { 
            return { 
                config: _this.config, 
                errors: _this.errors,
                inputs: _this.inputs
            }; 
        },
        created() { _this.c = this; },
        mounted() { 
            initVueComponent(_this);
            _this.c.config = {
                AppName: "",
                Port: _this.inputs.suggestedPort || 5000,
                Description: "",
                Template: "EmptyWebApi"
            };
        },
        props: { cid: String }
    }
</script>

<style scoped>
.pointer {
    cursor: pointer;
    transition: all 0.2s;
}
.pointer:hover {
    transform: translateY(-2px);
    box-shadow: 0 4px 12px rgba(0,0,0,0.1);
}
</style>
