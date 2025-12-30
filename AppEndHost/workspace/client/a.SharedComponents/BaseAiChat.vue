<template>
    <div class="card shadow-sm mb-2" style="height:440px; display: flex; flex-direction: column;">
        <div class="card-body d-flex flex-column" style="flex: 1; min-height: 0;">
            <div class="text-dark fs-d9 fw-bold px-2 d-flex align-items-center justify-content-between flex-shrink-0">
                <span>AI Chat</span>
                <div class="dropdown">
                    <button class="btn btn-sm btn-outline-secondary dropdown-toggle" type="button" 
                            id="modelDropdown" data-bs-toggle="dropdown" aria-expanded="false"
                            style="min-width:180px; text-align:left; font-size:0.75rem;">
                        <span>{{ selectedModelKey || 'Select a model...' }}</span>
                    </button>
                    <ul class="dropdown-menu" aria-labelledby="modelDropdown" style="max-height: 300px; overflow-y: auto;">
                        <template v-if="modelOptions && modelOptions.length > 0">
                            <template v-for="(provider, providerIndex) in modelOptions" :key="providerIndex">
                                <li><h6 class="dropdown-header text-primary fw-bold">{{ provider.Name || 'Unknown Provider' }}</h6></li>
                                <li v-for="(modelName, modelIndex) in (Array.isArray(provider.Models) ? provider.Models : [])" :key="modelIndex">
                                    <a class="dropdown-item" href="#" 
                                       :class="{ 'active': selectedModelKey === modelName }"
                                       @click.stop.prevent="selectModel(modelName, $event)"
                                       style="cursor: pointer;">
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
            <hr class="my-1 flex-shrink-0" />
            <div class="p-2 fs-d9 flex-grow-1 overflow-auto" style="min-height: 0;" ref="messagesContainer">
                <template v-if="messages && messages.length > 0">
                    <div v-for="(m, idx) in messages" :key="'msg-' + idx" 
                         class="mb-3 d-flex"
                         :class="m.role === 'user' ? 'justify-content-end' : 'justify-content-start'">
                        <div :class="['message-bubble p-2 rounded', m.role === 'user' ? 'user-message' : 'ai-message']"
                             :style="getMessageStyle(m.content)"
                             style="max-width: 70%; word-wrap: break-word;">
                            <div class="message-text">{{ m.content }}</div>
                        </div>
                    </div>
                </template>
                <div v-else class="text-muted small text-center py-4">
                    No messages yet. Start a conversation!
                </div>
            </div>
            <div class="px-2 pb-2 flex-shrink-0">
                <div class="position-relative">
                    <textarea class="form-control form-control-sm ae-focus fs-d9" 
                              :class="getPromptDirectionClass()"
                              :style="getPromptStyle()"
                              rows="2" 
                              v-model="prompt" 
                              @keydown="handleKeydown"
                              @input="handlePromptInput"
                              placeholder="Type your prompt and press Ctrl+Enter to send..."
                              style="resize:none;"></textarea>
                    <button class="btn btn-primary btn-sm position-absolute top-50 translate-middle-y rounded rounded-circle" 
                            type="button"
                            @click.prevent="send" 
                            :disabled="busy || !trimmedPrompt || !selectedModelKey || selectedModelKey === 'Select a model...'"
                            :class="{ 'disabled': busy || !trimmedPrompt || !selectedModelKey || selectedModelKey === 'Select a model...' }"
                            :style="getSendButtonStyle()"
                            style="z-index:10; width:28px; height:28px; padding:0; display:flex; align-items:center; justify-content:center;">
                        <i class="fa-solid fa-paper-plane" style="font-size:0.75rem;"></i>
                    </button>
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
                // Access prompt directly from Vue reactive data
                let p = (this.$data && this.$data.prompt) || this.prompt || '';
                return p ? String(p).trim() : '';
            }
        },
        methods: {
            // Detect if text is primarily Persian/Arabic (RTL) or English/Latin (LTR)
            isPersianText(text) {
                if (!text || text.trim() === '') return false;
                
                // Persian/Arabic Unicode ranges
                const persianArabicPattern = /[\u0600-\u06FF\u0750-\u077F\u08A0-\u08FF\uFB50-\uFDFF\uFE70-\uFEFF]/;
                
                // Count Persian/Arabic characters
                let persianCount = 0;
                let latinCount = 0;
                
                for (let i = 0; i < text.length; i++) {
                    const char = text[i];
                    if (persianArabicPattern.test(char)) {
                        persianCount++;
                    } else if (/[a-zA-Z0-9]/.test(char)) {
                        latinCount++;
                    }
                }
                
                // If there are Persian/Arabic characters and they are more or equal to Latin, consider it Persian
                return persianCount > 0 && persianCount >= latinCount;
            },
            getPromptDirectionClass() {
                return this.isPersianText(this.prompt) ? 'text-end' : 'text-start';
            },
            getPromptStyle() {
                const isPersian = this.isPersianText(this.prompt);
                // For Persian: padding-right for send button (on left), for English: padding-left for send button (on right)
                return {
                    direction: isPersian ? 'rtl' : 'ltr',
                    textAlign: isPersian ? 'right' : 'left',
                    paddingRight: isPersian ? '40px' : '12px',
                    paddingLeft: isPersian ? '12px' : '40px'
                };
            },
            getSendButtonStyle() {
                const isPersian = this.isPersianText(this.prompt);
                return {
                    right: isPersian ? 'auto' : '8px',
                    left: isPersian ? '8px' : 'auto'
                };
            },
            getMessageStyle(content) {
                const isPersian = this.isPersianText(content);
                return {
                    direction: isPersian ? 'rtl' : 'ltr',
                    textAlign: isPersian ? 'right' : 'left'
                };
            },
            handlePromptInput() {
                // Force update to refresh button position and text direction
                this.$forceUpdate();
            },
            selectModel(modelName, event) {
                if (!modelName || modelName === 'Select a model...') {
                    return;
                }
                
                // Update selected model
                this.selectedModelKey = modelName;
                _this.selectedModelKey = modelName;
                
                
                // Close Bootstrap dropdown programmatically
                this.$nextTick(() => {
                    try {
                        const dropdownButton = this.$el?.querySelector('#modelDropdown');
                        const dropdownMenu = this.$el?.querySelector('.dropdown-menu');
                        
                        if (dropdownButton && dropdownMenu) {
                            // Method 1: Try using Bootstrap Dropdown API
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
                                console.log('Bootstrap API not available, using fallback');
                            }
                            
                            // Method 2: Fallback - manually hide dropdown by removing 'show' class and clicking outside
                            dropdownMenu.classList.remove('show');
                            dropdownButton.classList.remove('show');
                            dropdownButton.setAttribute('aria-expanded', 'false');
                            
                            // Trigger a click on the document body to ensure dropdown closes
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
                        console.error('Error closing dropdown:', e);
                    }
                });
                
                // Force Vue update to ensure UI reflects the change
                this.$forceUpdate();
            },
            handleKeydown(event) {
                if (event.ctrlKey && (event.key === 'Enter' || event.keyCode === 13)) {
                    event.preventDefault();
                    event.stopPropagation();
                    this.send(event);
                }
            },
            send(event) {
                if (event) {
                    event.preventDefault();
                    event.stopPropagation();
                }
                
                // Get prompt directly from the textarea element to ensure we have the latest value
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

                // Find the provider that contains the selected model
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
                
                // Update messages - use Vue reactivity properly
                let currentMessages = Array.isArray(this.messages) ? this.messages : [];
                let newMessages = [...currentMessages, userMsg];
                this.messages = newMessages;
                _this.messages = newMessages;
                this.$forceUpdate();
                
                // Clear prompt from both Vue instance and textarea
                this.prompt = '';
                _this.prompt = '';
                if (textareaEl) textareaEl.value = '';

                rpcAEP('Generate', { prompt: p, model: selectedModelName }, (resp) => {
                    
                    this.busy = false;
                    let content = '';
                    
                    // Parse response if it's a string
                    let parsedResp = resp;
                    if (typeof resp === 'string') {
                        try {
                            parsedResp = JSON.parse(resp);
                        } catch (e) {
                            parsedResp = resp;
                        }
                    }
                    
                    // Extract content from response
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
                    let errorMsg = err && err.message ? err.message : (typeof err === 'string' ? err : JSON.stringify(err));
                    showError('AI Error: ' + errorMsg);
                    let errorMsgObj = { role: 'assistant', content: 'Error: ' + errorMsg, model: selectedModelName, provider: selectedProvider.Name, modelLabel: modelLabel };
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
                    // Parse response if it's a string
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
                    
                    // Auto-select first model if available
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
            // Load models immediately and also after a short delay to ensure API is ready
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
.message-bubble {
    box-shadow: 0 1px 2px rgba(0, 0, 0, 0.1);
}

.user-message {
    background-color: #007bff;
    color: #ffffff;
    border-bottom-right-radius: 4px !important;
}

.ai-message {
    background-color: #f8f9fa;
    color: #212529;
    border: 1px solid #dee2e6;
    border-bottom-left-radius: 4px !important;
}

.message-text {
    white-space: pre-wrap;
    word-wrap: break-word;
    line-height: 1.5;
}

.message-bubble pre {
    margin: 0;
    font-family: inherit;
    white-space: pre-wrap;
    word-wrap: break-word;
}
</style>