<template>
    <div class="card h-100 bg-transparent rounded-0 border-0" v-if="configured">
        <div class="card-header p-2 bg-primary-subtle-light rounded-0 border-0">
            <div class="hstack gap-2 align-items-center">
                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" :disabled="prompt.trim()===''" @click="send">
                    <i class="fa-solid fa-paper-plane"></i>
                    <span>{{shared.translate("Send")}}</span>
                </button>
                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" :disabled="messages.length===0" @click="clearChat">
                    <i class="fa-solid fa-trash"></i>
                    <span>{{shared.translate("Clear")}}</span>
                </button>
                <div class="ms-auto"></div>
                <select class="form-select form-select-sm w-auto" v-model="model" title="Model">
                    <option v-for="m in models" :key="m" :value="m">{{m}}</option>
                </select>
            </div>
        </div>
        <div class="card-body p-2 d-flex flex-column overflow-hidden">
            <div ref="chatPanel" class="flex-grow-1 overflow-auto border rounded p-2 bg-light-subtle" style="min-height:140px">
                <div v-if="messages.length===0" class="text-muted fst-italic small">{{shared.translate('StartTypingPrompt')}}</div>
                <div v-for="(msg,idx) in messages" :key="msg.id" class="mb-2">
                    <div :class="msg.role==='user'? 'text-end' : 'text-start'">
                        <span :class="bubbleClass(msg.role)">
                            <strong class="me-1" v-if="msg.role==='user'">{{shared.translate('You')}}:</strong>
                            <strong class="me-1" v-else>{{shared.translate('Assistant')}}:</strong>
                            <span v-html="formatMessage(msg.content)"></span>
                        </span>
                    </div>
                </div>
            </div>
            <div class="mt-2 position-relative">
                <textarea ref="inputBox" class="form-control form-control-sm" rows="3" v-model="prompt" :placeholder="shared.translate('TypeYourMessage')" @keyup.enter.exact.prevent="send"></textarea>
            </div>
        </div>
    </div>
    <div v-else class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-body d-flex flex-column justify-content-center align-items-center text-center">
            <i class="fa-solid fa-robot fa-3x text-secondary mb-3"></i>
            <div class="fw-bold mb-2">{{shared.translate('AI Not Configured')}}</div>
            <div class="text-muted mb-3" style="max-width:450px">{{shared.translate('GitHub AI key missing. Go to Settings, add Ai.GitHub.ApiKey and optionally BaseUrl, then reopen this chat.')}}</div>
            <button class="btn btn-sm btn-outline-primary" @click="openSettings">
                <i class="fa-solid fa-gear me-1"></i>{{shared.translate('Open Settings')}}
            </button>
        </div>
    </div>
</template>
<script>
    shared.setAppTitle(`<i class="fa-solid fa-robot fa-fw"></i> <span>AI Chat</span>`);

    let _this = { cid: "", c: null, prompt: "", model: "gpt-4o-mini", models: ["gpt-4o-mini", "gpt-4o", "gpt-4o-mini-translate"], messages: [], configured: false };
    export default {
        methods: {
            loadConfigStatus() {
                rpc({
                    requests: [{ Method: "Zzz.Ai.ConfigStatus", Inputs: {} }],
                    silent: true, onDone: (res) => { try { let r = R0R(res); let payload = r && r.Result ? r.Result : r; _this.c.configured = payload.HasApiKey === true || payload.HasApiKey === 'true'; } catch { _this.configured = false; } }, onFail: () => { _this.configured = false; }
                });
            },
            send() {
                if (!_this.configured) return; const userText = _this.prompt.trim(); if (userText === "" || _this.model.trim() === "") return;
                const reqId = 'chat_' + Date.now() + '_' + Math.floor(Math.random() * 100000);
                // push user message
                _this.messages.push({ id: reqId + '_u', role: 'user', content: userText });
                // push assistant placeholder
                _this.messages.push({ id: reqId + '_a', role: 'assistant', content: '...' });
                this.scrollToEnd();
                _this.prompt = ""; this.$nextTick(() => { if (this.$refs.inputBox) { this.$refs.inputBox.value = ''; this.$refs.inputBox.focus(); } });
                rpc({
                    requests: [{ Id: reqId, Method: "Zzz.Ai.Generate", Inputs: { "prompt": userText, "model": _this.c.model } }],
                    onDone: (res) => { try { let resp = Array.isArray(res) ? res.find(x => x && x.Id === reqId) : null; if (!resp) return; let payload = resp.Result && resp.Result.Result ? resp.Result.Result : resp.Result; let text = (payload && payload.Text) ? payload.Text : (payload && payload.Error ? payload.Error : JSON.stringify(payload)); let aMsg = _this.messages.find(m => m.id === reqId + '_a'); if (aMsg) aMsg.content = text; } catch (e) { let aMsg = _this.messages.find(m => m.id === reqId + '_a'); if (aMsg) aMsg.content = e.message; } this.scrollToEnd(); },
                    onFail: (err) => { let aMsg = _this.messages.find(m => m.id === reqId + '_a'); if (aMsg) aMsg.content = JSON.stringify(err); this.scrollToEnd(); }
                });
            },
            clearChat() { _this.messages = []; _this.prompt = ''; this.$nextTick(() => { if (this.$refs.inputBox) { this.$refs.inputBox.value = ''; this.$refs.inputBox.focus(); } }); },
            scrollToEnd() { this.$nextTick(() => { const panel = this.$refs.chatPanel; if (panel) panel.scrollTop = panel.scrollHeight; }); },
            bubbleClass(role) { return role === 'user' ? 'd-inline-block bg-primary text-white rounded px-2 py-1 shadow-sm' : 'd-inline-block bg-white border rounded px-2 py-1 shadow-sm'; },
            formatMessage(text) { return text.replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/\n/g, '<br />'); },
            openSettings() { window.location.href = '/AppEndStudio/?c=/AppEndStudio/components/BaseAppEndSettings'; }
        },
        setup(props) { _this.cid = props['cid']; },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { this.loadConfigStatus(); },
        props: { cid: String }
    }
</script>
