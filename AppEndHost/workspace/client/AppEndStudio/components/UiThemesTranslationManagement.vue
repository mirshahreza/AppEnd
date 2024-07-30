<template>
    <div class="card h-100 rounded rounded-2 rounded-bottom-0 rounded-end-0 bg-transparent border-0">
        <div class="card-header p-2 bg-success-subtle rounded-0 border-0">
            <div class="hstack gap-1">

                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="saveContent">
                    <i class="fa-solid fa-fw fa-save"></i> <span>Save Keys</span>
                </button>
                <div class="p-0 ms-auto"></div>
                <button type="button" class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="addKey">
                    <i class="fa-solid fa-file-alt fa-bounce" style="--fa-animation-iteration-count:1"></i> <span>New Key</span>
                </button>
                <button type="button" class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="extractKeys">
                    <i class="fa-solid fa-search-plus"></i> <span>Extract keys</span>
                </button>

            </div>
        </div>
        <div class="card-body p-2">
            <div class="card h-100 border-light bg-light bg-opacity-75 border-0">
                <div class="card-body rounded rounded-2 border border-3 border-light fs-d8 p-0 bg-transparent scrollable">
                    <table class="table table-sm table-striped table-hover w-100 ae-table m-0 bg-transparent">
                        <tbody>
                            <tr v-for="i,key in d">
                                <td style="width:28px;vertical-align:middle;" class="text-secondary text-center text-hover-danger pointer"
                                    @click="removeKey(key)">
                                    <i class="fa-solid fa-fw fa-times"></i>
                                </td>
                                <td style="width:250px;vertical-align:middle;" class="">{{key}}</td>
                                <td>
                                    <input class="form-control form-control-sm p-1 py-0 fs-d9" :value="i" :data-ae-key="key" @keyup.enter="applyValue"
                                           data-ae-validation-required="true" data-ae-validation-rule=":=s(1,256)" />
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    shared.setAppTitle("Translation Management");
    shared.setAppSubTitle(getQueryString("app"));

    let _this = { cid: "", c: null, inputs: {}, appName: getQueryString("app"), fullConf: {}, d: {} };

    export default {
        methods: {
            extractKeys() {
                //if (!_this.regulator.isValid()) return false;
                _this.c.fullConf["translation"] = _this.c.d;
                rpcAEP("ExtractTranslationKeys", { "FolderName": _this.c.appName }, function (res) {
                    _this.c.loadContent();
                });
            },
            saveContent() {
                //if (!_this.regulator.isValid()) return false;
                _this.c.fullConf["translation"] = _this.c.d;
                rpcAEP("SaveFileContent", { "PathToWrite": 'workspace/client/' + _this.c.appName + '/app.json', "FileContent": JSON.stringify(_this.c.fullConf) }, function (res) {
                    showSuccess("Theme translation keys saved.");
                });
            },
            removeKey(key) {
                delete _this.c.d[key];
            },
            addKey() {
                showPrompt({
                    title: "New translation key", message1: "Enter a name for the new key", message2: "Spaces and Wildcards are not allowed",
                    callback: function (ret) {
                        let o = {};
                        o[ret] = "Enter the value";
                        _this.c.d = prepend(_this.c.d, o);
                    }
                });
            },
            applyValue(elm) {
                let elmJ = $(elm.target);
                _this.c.d[elmJ.attr("data-ae-key")] = elmJ.val();
            },
            loadContent() {
                rpcAEP("GetFileContent", { "PathToRead": 'workspace/client/' + _this.c.appName + '/app.json' }, function (res) {
                    _this.c.fullConf = JSON.parse(R0R(res));
                    _this.c.d = _this.c.fullConf["translation"];
                    _this.c.d = fixNull(_this.c.d, {});
                });
            }
        },
        setup(props) {
            _this.cid = props['cid'];
            _this.inputs = shared["params_" + _this.cid];
        },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { initVueComponent(_this); _this.c.loadContent(); },
        props: { cid: String }
    }
</script>
