<template>
    <div class="card h-100 rounded rounded-2 rounded-bottom-0 rounded-end-0 bg-transparent border-0">
        <div class="card-header p-2 bg-light-subtle rounded-end-0 border-0">
            <div class="input-group input-group-sm border-0 bg-transparent">
                <div class="input-group-text">
                    To change the settings go to home folder, open appsettings.json and change instance settings
                </div>
                <input type="text" class="form-control form-control-sm border-0 rounded-0 bg-transparent" disabled />
            </div>
        </div>
        <div class="card-body p-2">
            <div class="card h-100 border-light bg-light bg-opacity-75 border-0">
                <div class="card-body rounded rounded-2 border border-3 border-light fs-d8 p-0 bg-transparent scrollable">
                    <table class="table table-sm table-striped table-hover w-100 ae-table m-0 bg-transparent">
                        <tbody>
                            <tr v-for="i,key in d">
                                <td style="width:250px;vertical-align:middle;" class="">{{key}}</td>
                                <td stty="vertical-align:middle;">
                                    <div>&nbsp;</div>
<pre>
{{JSON.stringify(i,null,4)}}
</pre>
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
    shared.setAppTitle("Instance Settings");

    let _this = { cid: "", c: null, inputs: {}, appName: getQueryString("app"), fullConf: {}, d: {} };

    export default {
        methods: {
            loadContent() {
                rpcAEP("GetFileContent", { "PathToRead": 'appsettings.json' }, function (res) {
                    _this.c.fullConf = JSON.parse(R0R(res));
                    _this.c.d = _this.c.fullConf["AppEnd"];
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
