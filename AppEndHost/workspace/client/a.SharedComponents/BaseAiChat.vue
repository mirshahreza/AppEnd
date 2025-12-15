<template>
    <div class="card shadow-sm" style="min-height:300px;">
        <div class="card-header py-2 px-3 d-flex align-items-center gap-2">
            <i class="fa-solid fa-robot text-primary"></i>
            <span class="fw-bold flex-grow-1">AI Chat</span>
            <small class="text-secondary">Model:</small>


            <div class="d-none d-lg-block fs-d8 fw-bold dropdown">
                <div class="animate__animated animate__slideInDown border border-2 border-0 rounded-2 p-1 bg-elevated shadow-sm pointer" 
                     data-bs-toggle="dropdown" aria-expanded="false" style="min-width:200px; height:36px;">
                    <i class="fa-solid fa-robot text-primary me-2"></i>
                    <span>{{ selectedModelKey }}</span>
                    <i class="fa-solid fa-chevron-down ms-2 text-secondary"></i>
                </div>
                <ul class="dropdown-menu bg-elevated shadow-lg border-2">
                    <template v-for="(provider, providerIndex) in modelOptions">
                        <li class="dropdown-header text-primary fw-bold">
                            {{ provider.Name }}
                        </li>
                        <li v-for="opt in provider.Models" :key="opt">
                            <span class="dropdown-item p-1 px-3 fs-d7 text-secondary hover-primary pointer" :class="{ 'bg-primary text-white': selectedModelKey === opt }"
                                  @click="selectedModelKey = opt">
                                <i class="fa-solid fa-fw fa-microchip text-secondary"></i>
                                <span>{{ opt }}</span>
                            </span>
                        </li>
                        <li v-if="providerIndex < modelOptions.length - 1">
                            <hr class="dropdown-divider">
                        </li>
                    </template>
                </ul>
            </div>

        </div>
        <div class="card-body p-2 fs-d8 scrollable d-flex flex-column">
            <div class="flex-grow-1 mb-2 pe-1 overflow-auto" ref="messagesContainer">
                <div v-for="(m, idx) in messages" :key="idx" class="mb-2">
                    <div class="small text-secondary">{{ m.role === 'user' ? 'You' : 'AI' }} - {{ m.modelLabel }}</div>
                    <div :class="['p-2 rounded', m.role === 'user' ? 'bg-primary text-white' : 'bg-light']">
                        <pre class="m-0" style="white-space: pre-wrap;">{{ m.content }}</pre>
                    </div>
                </div>
            </div>
        </div>
        <div class="card-footer p-2">
            <div class="position-relative">
                <textarea class="form-control ae-focus pe-5" rows="2" v-model="prompt" @keyup.enter.ctrl.exact="send"
                          placeholder="Type your prompt and press Ctrl+Enter to send..."
                          style="resize:none; border-radius:12px;"></textarea>
                <button class="btn btn-primary btn-sm position-absolute top-50 translate-middle-y rounded rounded-circle" type="button"
                        @click="send" :disabled="busy || !trimmedPrompt"
                        style="right:12px; ">
                    <i class="fa-solid fa-paper-plane"></i>
                </button>
            </div>
        </div>
    </div>
</template>

<script>
    let _this = { cid: '', c: null, inputs: {}, prompt: '', messages: [], busy: false, modelOptions: [], selectedModelKey: '' };

    export default {
        computed: {
            trimmedPrompt() {
                return _this.c.prompt ? _this.c.prompt.trim() : '';
            },
            groupedModelOptions() {
                let groups = [];
                let providerMap = new Map();
                
                _this.c.modelOptions.forEach(opt => {
                    if (!providerMap.has(opt.provider)) {
                        providerMap.set(opt.provider, []);
                    }
                    providerMap.get(opt.provider).push(opt);
                });
                
                providerMap.forEach((models, providerName) => {
                    groups.push({
                        providerName: providerName,
                        models: models
                    });
                });
                
                return groups;
            }
        },
        methods: {
            send() {
                if (_this.c.busy) return;
                let p = this.trimmedPrompt;
                if (p === '') return;
                if (!_this.c.selectedModelKey) {
                    showError('No model selected');
                    return;
                }

                let opt = _this.c.modelOptions.find(x => x.Name === _this.c.selectedModelKey);
                if (!opt) {
                    showError('Unknown model');
                    return;
                }

                _this.c.busy = true;
                let userMsg = { role: 'user', content: p, model: opt.model, provider: opt.provider, modelLabel: opt.label };
                _this.c.messages.push(userMsg);
                _this.c.prompt = '';

                rpcAEP('Zzz.Ai.Generate', { prompt: p, model: opt.model }, (resp) => {
                    _this.c.busy = false;
                    let content = resp && resp.Result ? resp.Result : shared.fixNull(resp, '');
                    _this.messages.push({ role: 'assistant', content: content, model: opt.model, provider: opt.provider, modelLabel: opt.label });
                    _this.c.$nextTick(_this.c.scrollToBottom);
                }, (err) => {
                    _this.c.busy = false;
                    showError(err);
                });
            },
            scrollToBottom() {
                try {
                    let el = _this.c.$refs.messagesContainer;
                    if (el) el.scrollTop = el.scrollHeight;
                } catch (e) { }
            },
            loadModels() {
                rpcAEP('GetAiProvidersWithModels', {}, (resp) => {


                    //let list = R0R(resp);
                    //let items = [];

                    //list.forEach(p => {


                    //});

                    _this.c.modelOptions = R0R(resp);
                    _this.c.$forceUpdate();

                }, (err) => {
                    showError(err);
                });
            }
        },
        setup(props) {
            _this.cid = props['cid'];
            _this.inputs = shared['params_' + _this.cid] || {};
            return _this;
        },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() {
            initVueComponent(_this);
            _this.c.loadModels();
        },
        props: { cid: String }
    };
</script>
