<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-header p-2 bg-primary-subtle-light rounded-0 border-0">
            <div class="hstack">
                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="saveFileBody">
                    <i class="fa-solid fa-save"></i> <span>Save</span>
                </button>
                <div class="p-0 ms-auto"></div>
            </div>
        </div>
        <div class="card-body p-0">
            <div class="code-editor-container h-100" id="fileEditor"></div>
        </div>
    </div>
</template>
<script>
    shared.setAppTitle(`<i class="fa-solid fa-fw fa-edit"></i> <span>File Editor</span>`);

    let _this = { cid: "", c: null, fileBody: "", filePath: "", editor: null };
    export default {
        methods: {
            saveFileBody() {
                rpcAEP("SaveFileContent", { "PathToWrite": _this.filePath, "FileContent": _this.editor.getValue().trim() }, function (res) {
                    showSuccess("File content saved");
                });
            },
            readFileBody() {
                rpcAEP("GetFileContent", { "PathToRead": _this.filePath }, function (res) {
                    _this.c.fileBody = R0R(res);
                    _this.c.setupEditor();
                });
            },
            setupEditor() {
                _this.editor = ace.edit("fileEditor", {
                    theme: "ace/theme/cloud9_day",
                    mode: getEditorMode(_this.filePath),
                    value: _this.c.fileBody
                });
            }
        },
        setup(props) {
            _this.cid = props['cid'];
            let params = shared["params_" + _this.cid] || {};
            _this.filePath = fixNull(params.filePath, getQueryString("filePath"));
        },
        data() { return _this; },
        created() {
            _this.c = this;
            shared.setAppSubTitle(_this.filePath);
        },
        mounted() { _this.c.readFileBody(); },
        props: { cid: String }
    }
</script>