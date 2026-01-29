<template>
    <div class="ai-chat-container">
        <div class="h-100 w-100 d-flex flex-column">
            <!-- Header with model selector -->
            <div class="chat-header d-flex align-items-center justify-content-between flex-shrink-0 p-2 border-bottom">
                <div class="d-flex align-items-center gap-2">
                    <i class="fa-solid fa-robot text-primary" style="font-size: 1rem;"></i>
                    <span class="fw-bold text-dark" style="font-size: 0.85rem;">AI Chat</span>
                </div>
                <div class="dropdown flex-grow-1 ms-2">
                    <button class="btn btn-sm btn-outline-primary dropdown-toggle model-selector w-100" type="button" 
                            id="modelDropdown" data-bs-toggle="dropdown" aria-expanded="false">
                        <i class="fa-solid fa-microchip me-1"></i>
                        <span class="model-text">{{ selectedModelKey || 'Select a model...' }}</span>
                    </button>
                    <ul class="dropdown-menu shadow" aria-labelledby="modelDropdown">
                        <template v-if="modelOptions && modelOptions.length > 0">
                            <template v-for="(provider, providerIndex) in modelOptions" :key="providerIndex">
                                <li><h6 class="dropdown-header text-primary fw-bold">{{ provider.Name || 'Unknown Provider' }}</h6></li>
                                <li v-for="(modelName, modelIndex) in (Array.isArray(provider.Models) ? provider.Models : [])" :key="modelIndex">
                                    <a class="dropdown-item" href="#" 
                                       :class="{ 'active': selectedModelKey === modelName }"
                                       @click.stop.prevent="selectModel(modelName, $event)">
                                        {{ modelName }}
                                    </a>
                                </li>
                                <li v-if="providerIndex < modelOptions.length - 1"><hr class="dropdown-divider"></li>
                            </template>
                        </template>
                        <li v-else>
                            <span class="dropdown-item text-muted small">
                                <i class="fa-solid fa-spinner fa-spin me-2"></i>
                                Loading models...
                            </span>
                        </li>
                    </ul>
                </div>
            </div>
            
            <!-- Messages container -->
            <div class="messages-container flex-grow-1 overflow-auto" ref="messagesContainer">
                <template v-if="messages && messages.length > 0">
                    <div v-for="(m, idx) in messages" :key="'msg-' + idx" 
                         class="message-wrapper"
                         :class="m.role === 'user' ? 'user-msg-wrapper' : 'ai-msg-wrapper'">
                        <div :class="['message-bubble', m.role === 'user' ? 'user-message' : 'ai-message']"
                             :style="getMessageStyle(m.content)">
                            <div class="message-text">{{ formatMessageText(m) }}</div>

                            <template v-if="m.role === 'assistant' && m.isError && m.raw">
                                <button type="button" class="btn btn-link btn-sm p-0 mt-2 raw-toggle" @click.prevent="toggleRaw(idx)">
                                    {{ m.showRaw ? 'Hide details' : 'Show details' }}
                                </button>
                                <pre v-if="m.showRaw" class="raw-error mt-2">{{ prettyJson(m.raw) }}</pre>
                            </template>
                        </div>
                    </div>
                </template>
                <div v-else class="empty-state">
                    <i class="fa-solid fa-comments fs-1 text-muted mb-3"></i>
                    <p class="text-muted">No messages yet. Start a conversation!</p>
                </div>
            </div>
            
            <!-- Input area with rainbow gradient -->
            <div class="input-area flex-shrink-0 p-2 border-top">
                <div class="input-container">
                    <div class="prompt-wrapper">
                        <textarea class="form-control prompt-textarea" 
                                  :class="getPromptDirectionClass()"
                                  :style="getPromptStyle()"
                                  rows="2" 
                                  v-model="prompt" 
                                  @keydown="handleKeydown"
                                  @input="handlePromptInput"
                                  placeholder="Type your prompt and press Enter to send..."></textarea>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    let _this = { cid: '', c: null, inputs: {}, prompt: '', messages: [], busy: false, modelOptions: [], selectedModelKey: '' };

    export default {
        computed: {
            trimmedPrompt() {
                let p = (this.$data && this.$data.prompt) || this.prompt || '';
                return p ? String(p).trim() : '';
            }
        },
        methods: {
            isPersianText(text) {
                if (!text || text.trim() === '') return false;
                
                // Check first character to determine direction
                const firstChar = text.trim()[0];
                const persianArabicPattern = /[\u0600-\u06FF\u0750-\u077F\u08A0-\u08FF\uFB50-\uFDFF\uFE70-\uFEFF]/;
                
                return persianArabicPattern.test(firstChar);
            },
            getPromptDirectionClass() {
                return this.isPersianText(this.prompt) ? 'text-end' : 'text-start';
            },
            getPromptStyle() {
                const isPersian = this.isPersianText(this.prompt);
                return {
                    direction: isPersian ? 'rtl' : 'ltr',
                    textAlign: isPersian ? 'right' : 'left',
                    paddingRight: '7px',
                    paddingLeft: '7px'
                };
            },
            getMessageStyle(content) {
                const isPersian = this.isPersianText(content);
                return {
                    direction: isPersian ? 'rtl' : 'ltr',
                    textAlign: isPersian ? 'right' : 'left'
                };
            },
            toSingleLine(content) {
                let s = content === null || content === undefined ? '' : String(content);

                // collapse whitespace/newlines into a single line
                s = s.replace(/\s+/g, ' ').trim();

                return s;
            },
            handlePromptInput() {
                this.$forceUpdate();
            },
            selectModel(modelName, event) {
                if (!modelName || modelName === 'Select a model...') {
                    return;
                }
                this.selectedModelKey = modelName;
                _this.selectedModelKey = modelName;
                
                this.$nextTick(() => {
                    try {
                        const dropdownButton = this.$el?.querySelector('#modelDropdown');
                        const dropdownMenu = this.$el?.querySelector('.dropdown-menu');
                        
                        if (dropdownButton && dropdownMenu) {
                            try {
                                const bootstrapLib = window.bootstrap || (typeof bootstrap !== 'undefined' ? bootstrap : null);
                                if (bootstrapLib && bootstrapLib.Dropdown) {
                                    const dropdownInstance = bootstrapLib.Dropdown.getInstance(dropdownButton);
                                    if (dropdownInstance) {
                                        dropdownInstance.hide();
                                        return;
                                    }
                                }
                            } catch (bsError) {
                            }
                            
                            dropdownMenu.classList.remove('show');
                            dropdownButton.classList.remove('show');
                            dropdownButton.setAttribute('aria-expanded', 'false');
                            
                            setTimeout(() => {
                                const clickEvent = new MouseEvent('click', {
                                    bubbles: true,
                                    cancelable: true,
                                    view: window
                                });
                                document.body.dispatchEvent(clickEvent);
                            }, 10);
                        }
                    } catch (e) {
                    }
                });
                
                this.$forceUpdate();
            },
            handleKeydown(event) {
                if (event.key === 'Enter' || event.keyCode === 13) {
                    if (event.shiftKey) {
                        // Allow Shift+Enter for new line
                        return;
                    }
                    event.preventDefault();
                    event.stopPropagation();
                    this.send(event);
                }
            },
            toggleRaw(idx) {
                try {
                    if (!Array.isArray(this.messages) || !this.messages[idx]) return;
                    let m = this.messages[idx];
                    m.showRaw = !m.showRaw;
                    this.$forceUpdate();
                } catch (e) {
                }
            },
            prettyJson(raw) {
                try {
                    if (raw === null || raw === undefined) return '';
                    if (typeof raw === 'string') {
                        try {
                            return JSON.stringify(JSON.parse(raw), null, 2);
                        } catch (e) {
                            return raw;
                        }
                    }
                    return JSON.stringify(raw, null, 2);
                } catch (e) {
                    return String(raw || '');
                }
            },
            extractErrorMessage(err) {
                try {
                    // keep original for fallback
                    let original = err;

                    // Try parse string to object
                    let obj = err;
                    if (typeof obj === 'string') {
                        // If wrapper text contains embedded JSON, parse that JSON part
                        let jsonStart = obj.indexOf('{');
                        if (jsonStart >= 0) {
                            let jsonText = obj.substring(jsonStart);
                            try {
                                obj = JSON.parse(jsonText);
                            } catch (e) {
                                // ignore
                            }
                        }

                        // Extract nested message directly from wrapper
                        let m = obj.match(/"message"\s*:\s*"([^\"]+)"/i);
                        if (m && m[1]) return m[1];

                        // try json parse of full string
                        try {
                            obj = JSON.parse(obj);
                        } catch (e) {
                            // no structured message
                            return '';
                        }
                    }

                    // Dive into common shapes
                    if (obj && typeof obj === 'object') {
                        if (typeof obj.message === 'string' && obj.message.trim()) return obj.message.trim();
                        if (obj.error) {
                            if (typeof obj.error.message === 'string' && obj.error.message.trim()) return obj.error.message.trim();
                            if (obj.error.error && typeof obj.error.error.message === 'string' && obj.error.error.message.trim()) return obj.error.error.message.trim();
                        }
                        if (obj.Result) {
                            if (typeof obj.Result.message === 'string' && obj.Result.message.trim()) return obj.Result.message.trim();
                            if (obj.Result.error && typeof obj.Result.error.message === 'string' && obj.Result.error.message.trim()) return obj.Result.error.message.trim();
                        }
                    }

                    // Best-effort regex fallback from original stringified
                    let ss = typeof original === 'string' ? original : JSON.stringify(original);
                    let mm = ss.match(/"message"\s*:\s*"([^\"]+)"/i);
                    if (mm && mm[1]) return mm[1];

                    return '';
                } catch (e) {
                    return '';
                }
            },
            isLikelyErrorResponse(parsedResp, content) {
                try {
                    let msg = this.extractErrorMessage(parsedResp);
                    if (msg && msg.trim()) return true;

                    // structured error payload
                    if (parsedResp && typeof parsedResp === 'object') {
                        if (parsedResp.error) return true;
                        if (parsedResp.Result && parsedResp.Result.error) return true;
                    }

                    let lc = String(content || '').toLowerCase();
                    if (
                        lc.includes('error from llm provider') ||
                        lc.includes('unauthorized') ||
                        lc.includes('invalid_request_error') ||
                        lc.includes('invalid_api_key') ||
                        lc.includes('incorrect api key')
                    ) return true;

                    return false;
                } catch (e) {
                    return false;
                }
            },
            formatMessageText(m) {
                if (!m) return '';
                if (m.role === 'assistant' && m.isError) {
                    return (m.content === null || m.content === undefined) ? '' : String(m.content);
                }
                return this.toSingleLine(m.content);
            },
            send(event) {
                if (event) {
                    event.preventDefault();
                    event.stopPropagation();
                }
                
                let textareaEl = this.$el ? this.$el.querySelector('textarea') : null;
                let rawPrompt = textareaEl ? textareaEl.value : (this.prompt || _this.prompt || '');
                let p = rawPrompt ? String(rawPrompt).trim() : '';
                
                if (this.busy) {
                    return;
                }
                
                if (!p || p === '') {
                    return;
                }
                
                if (!this.selectedModelKey || this.selectedModelKey === 'Select a model...') {
                    showError('Please select a model first');
                    return;
                }

                let selectedProvider = null;
                let selectedModelName = null;
                
                for (let provider of this.modelOptions) {
                    if (Array.isArray(provider.Models) && provider.Models.includes(this.selectedModelKey)) {
                        selectedProvider = provider;
                        selectedModelName = this.selectedModelKey;
                        break;
                    }
                }
                
                if (!selectedProvider || !selectedModelName) {
                    showError('Unknown model selected');
                    return;
                }

                this.busy = true;
                let modelLabel = `${selectedProvider.Name} - ${selectedModelName}`;
                let userMsg = { role: 'user', content: p, model: selectedModelName, provider: selectedProvider.Name, modelLabel: modelLabel };
                
                let currentMessages = Array.isArray(this.messages) ? this.messages : [];
                let newMessages = [...currentMessages, userMsg];
                this.messages = newMessages;
                _this.messages = newMessages;
                this.$forceUpdate();
                
                this.prompt = '';
                _this.prompt = '';
                if (textareaEl) textareaEl.value = '';

                rpcAEP('Generate', { prompt: p, model: selectedModelName }, (resp) => {
                    
                    this.busy = false;

                    let parsedResp = resp;
                    if (typeof resp === 'string') {
                        try {
                            parsedResp = JSON.parse(resp);
                        } catch (e) {
                            parsedResp = resp;
                        }
                    }

                    // Build content (normal case)
                    let content = '';
                    if (Array.isArray(parsedResp) && parsedResp.length > 0) {
                        let result = R0R(parsedResp);
                        if (typeof result === 'string') {
                            content = result;
                        } else if (result && result.toString) {
                            content = result.toString();
                        } else {
                            content = JSON.stringify(result);
                        }
                    } else if (parsedResp && parsedResp.Result !== undefined) {
                        content = typeof parsedResp.Result === 'string' ? parsedResp.Result : JSON.stringify(parsedResp.Result);
                    } else {
                        content = shared.fixNull(parsedResp, '');
                    }

                    // If response is an error, show ONLY `message` and add a toggle to view raw response
                    if (this.isLikelyErrorResponse(parsedResp, content)) {
                        const msg = this.extractErrorMessage(parsedResp) || this.extractErrorMessage(content) || 'Error';
                        showError(msg);

                        let assistantMsg = {
                            role: 'assistant',
                            content: msg,
                            model: selectedModelName,
                            provider: selectedProvider.Name,
                            modelLabel: modelLabel,
                            isError: true,
                            raw: parsedResp,
                            showRaw: false
                        };

                        let currentMessages = Array.isArray(this.messages) ? this.messages : [];
                        let newMessages = [...currentMessages, assistantMsg];
                        this.messages = newMessages;
                        _this.messages = newMessages;
                        this.$forceUpdate();
                        this.$nextTick(() => this.scrollToBottom());
                        return;
                    }

                    // Normal assistant message
                    let assistantMsg = { role: 'assistant', content: content, model: selectedModelName, provider: selectedProvider.Name, modelLabel: modelLabel };
                    let currentMessages = Array.isArray(this.messages) ? this.messages : [];
                    let newMessages = [...currentMessages, assistantMsg];
                    this.messages = newMessages;
                    _this.messages = newMessages;
                    this.$forceUpdate();
                    
                    this.$nextTick(() => {
                        this.scrollToBottom();
                    });
                }, (err) => {
                    this.busy = false;

                    const msg = this.extractErrorMessage(err) || 'Error';
                    showError(msg);

                    let errorMsgObj = {
                        role: 'assistant',
                        content: msg,
                        model: selectedModelName,
                        provider: selectedProvider.Name,
                        modelLabel: modelLabel,
                        isError: true,
                        raw: err,
                        showRaw: false
                    };

                    let currentMessages = Array.isArray(this.messages) ? this.messages : [];
                    let newMessages = [...currentMessages, errorMsgObj];
                    this.messages = newMessages;
                    _this.messages = newMessages;
                    this.$forceUpdate();
                });
            },
            scrollToBottom() {
                try {
                    let el = this.$refs.messagesContainer;
                    if (el) el.scrollTop = el.scrollHeight;
                } catch (e) { 
                }
            },
            loadModels() {
                rpcAEP('GetAiProvidersWithModels', {}, (resp) => {
                    let parsedResp = resp;
                    if (typeof resp === 'string') {
                        try {
                            parsedResp = JSON.parse(resp);
                        } catch (e) {
                        }
                    }
                    
                    let list = R0R(parsedResp);
                    
                    if (!Array.isArray(list)) {
                        showError('Invalid response format. Please check LLM Providers in Settings.');
                        this.modelOptions = [];
                        this.selectedModelKey = '';
                        return;
                    }
                    
                    if (list.length === 0) {
                        showError('No LLM providers configured. Please add providers in AppEnd Settings.');
                        this.modelOptions = [];
                        this.selectedModelKey = '';
                        return;
                    }
                    
                    this.modelOptions = list;
                    _this.modelOptions = list;
                    
                    if (this.modelOptions.length > 0 && this.modelOptions[0].Models && Array.isArray(this.modelOptions[0].Models) && this.modelOptions[0].Models.length > 0) {
                        this.selectedModelKey = this.modelOptions[0].Models[0];
                        _this.selectedModelKey = this.modelOptions[0].Models[0];
                    } else {
                        this.selectedModelKey = '';
                        _this.selectedModelKey = '';
                    }
                    
                    this.$forceUpdate();
                }, (err) => {
                    showError('Failed to load AI models. Error: ' + (err && err.message ? err.message : JSON.stringify(err)));
                    this.modelOptions = [];
                    this.selectedModelKey = '';
                });
            }
        },
        setup(props) {
            _this.cid = props['cid'];
            _this.inputs = shared['params_' + _this.cid] || {};
            return _this;
        },
        data() { 
            return _this; 
        },
        created() { 
            _this.c = this;
        },
        mounted() {
            initVueComponent(_this);
            this.loadModels();
            setTimeout(() => {
                if (!this.modelOptions || this.modelOptions.length === 0) {
                    this.loadModels();
                }
            }, 500);
        },
        props: { cid: String }
    };
</script>

<style scoped>
/* AI Chat Container - Side panel with AppEnd splitter */
.ai-chat-container {
    height: 100%;
    width: 100%;
    background: linear-gradient(135deg, #f8f9fa 0%, #ffffff 100%);
    display: flex;
    flex-direction: column;
}

/* Header */
.chat-header {
    flex-shrink: 0;
    border-bottom: 1px solid #e9ecef;
    padding: 4px 8px !important;
}

.chat-header i.fa-robot {
    font-size: 1rem;
}

/* Model selector dropdown */
.model-selector {
    font-size: 0.7rem;
    border-radius: 6px;
    transition: all 0.3s ease;
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 6px 10px !important;
    height: 32px;
    text-align: left;
    background-color: transparent;
    border: 1px solid #dee2e6;
}

.model-selector .model-text {
    flex: 1;
    text-align: left;
    margin-right: 4px;
    font-size: 0.7rem;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
}

.model-selector::after {
    margin-left: auto;
    flex-shrink: 0;
}

.model-selector:hover {
    background-color: #f8f9fa;
    border-color: #adb5bd;
}

.model-selector:focus,
.model-selector:active {
    background-color: #ffffff;
    border-color: #dee2e6;
    box-shadow: none;
}

/* Dropdown menu styling */
.dropdown-menu {
    border-radius: 8px;
    border: 1px solid #e9ecef;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
    padding: 8px 0;
    max-height: 400px;
    overflow-y: auto;
}

.dropdown-item {
    padding: 6px 12px;
    font-size: 0.8rem;
    transition: all 0.2s ease;
    border-radius: 4px;
    margin: 2px 8px;
    color: #212529;
    background-color: transparent;
}

.dropdown-item:hover {
    background-color: #f8f9fa;
    color: #212529;
}

.dropdown-item.active,
.dropdown-item:active {
    background-color: #e9ecef;
    color: #212529;
}

.dropdown-header {
    padding: 6px 12px;
    font-size: 0.7rem;
    text-transform: uppercase;
    letter-spacing: 0.5px;
}

/* Messages container */
.messages-container {
    padding: 12px;
    flex: 1;
    min-height: 0;
    overflow-y: auto;
    scroll-behavior: smooth;
}

.messages-container::-webkit-scrollbar {
    width: 6px;
}

.messages-container::-webkit-scrollbar-track {
    background: #f1f1f1;
    border-radius: 3px;
}

.messages-container::-webkit-scrollbar-thumb {
    background: #c0c0c0;
    border-radius: 3px;
}

.messages-container::-webkit-scrollbar-thumb:hover {
    background: #a0a0a0;
}

/* Message wrapper */
.message-wrapper {
    display: flex;
    margin-bottom: 12px;
    animation: fadeIn 0.3s ease-in;
}

@keyframes fadeIn {
    from {
        opacity: 0;
        transform: translateY(10px);
    }
    to {
        opacity: 1;
        transform: translateY(0);
    }
}

.user-msg-wrapper {
    justify-content: flex-end;
}

.ai-msg-wrapper {
    justify-content: flex-start;
}

/* Message bubbles */
.message-bubble {
    max-width: 85%;
    padding: 10px 12px;
    border-radius: 12px;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
    word-wrap: break-word;
    overflow-wrap: break-word;
    transition: all 0.2s ease;
}

.message-bubble:hover {
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.12);
}

.user-message {
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    color: #ffffff;
    border-bottom-right-radius: 4px;
}

.ai-message {
    background-color: #f8f9fa;
    color: #212529;
    border: 1px solid #e9ecef;
    border-bottom-left-radius: 4px;
}

.message-text {
    white-space: pre-wrap;
    word-wrap: break-word;
    line-height: 1.4;
    font-size: 0.85rem;
}

/* Empty state */
.empty-state {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    height: 100%;
    text-align: center;
    padding: 20px;
}

/* Input area with rainbow gradient */
.input-area {
    flex-shrink: 0;
}

.input-container {
    position: relative;
}

.prompt-wrapper {
    position: relative;
    padding: 2px;
    background: linear-gradient(90deg, 
        #ff0080 0%, 
        #ff8c00 16%, 
        #40e0d0 33%, 
        #00bfff 50%, 
        #9370db 66%, 
        #ff1493 83%, 
        #ff0080 100%
    );
    background-size: 200% 100%;
    border-radius: 12px;
    animation: rainbow-flow 3s ease-in-out infinite;
    transition: all 0.3s ease;
}

@keyframes rainbow-flow {
    0%, 100% {
        background-position: 0% 50%;
    }
    50% {
        background-position: 100% 50%;
    }
}

.prompt-wrapper::before {
    content: '';
    position: absolute;
    inset: 2px;
    background: #ffffff;
    border-radius: 10px;
    z-index: 1;
}

.prompt-textarea {
    position: relative;
    z-index: 2;
    width: 100%;
    border: none;
    border-radius: 10px;
    padding: 8px 7px;
    font-size: 0.85rem;
    resize: none;
    background: transparent;
    transition: all 0.3s ease;
    line-height: 1.3;
}

.prompt-textarea:focus {
    outline: none;
    box-shadow: 0 0 0 3px rgba(var(--bs-primary-rgb), 0.1);
}

.prompt-textarea::placeholder {
    color: #adb5bd;
    font-size: 0.8rem;
}

/* Raw JSON error details */
.raw-toggle {
    font-size: 0.75rem;
    text-decoration: none;
}

.raw-error {
    white-space: pre-wrap;
    word-break: break-word;
    background: #f8f9fa;
    border: 1px solid #e9ecef;
    border-radius: 8px;
    padding: 8px;
    font-size: 0.75rem;
    color: #212529;
}

/* Responsive adjustments */
@media (max-width: 768px) {
    .message-bubble {
        max-width: 80%;
    }
    
    .model-selector {
        min-width: 120px;
        font-size: 0.65rem;
    }
}
</style>