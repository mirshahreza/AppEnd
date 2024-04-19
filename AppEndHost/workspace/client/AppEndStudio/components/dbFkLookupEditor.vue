<template>
    <div class="card h-100 border-0 shadow-lg bg-transparent rounded-0">
        <div class="card-header fw-bold fs-d9">
            Lookup items comes from an internal service, define it and execute to see the result
        </div>
        <div class="card-header bg-light-subtle">
            <div class="input-group input-group-sm">
                <select class="form-select form-select-sm" v-model="selectedTemplate">
                    <option value="">Select Template</option>
                    <option v-for="i in requestTemplates" :value="i">{{i}}</option>
                </select>
                <button class="btn btn-outline-primary" @click="insertTemplate">
                    Insert Template
                </button>
            </div>
        </div>

        <div class="card-body p-2 pb-4 bg-transparent fs-d8">
            <div class="h-100 w-100" data-flex-splitter-horizontal style="flex: auto;">
                <div class="h-100" style="min-width:200px;width:60%;">
                    <label class="mx-3">Request</label>
                    <textarea class="form-control form-control-sm fs-d8 h-100" rows="25" id="lookupServiceBody">{{inputs.Lookup}}</textarea>
                </div>
                <div role="separator" tabindex="1" class="bg-light" style="width:5%;">
                    <button class="btn btn-sm btn-light p-2" @click="execLookupRequest">
                        <i class="fa-solid fa-2x fa-play"></i>
                        <br />
                        <span >Exec</span>
                    </button>
                </div>
                <div class="h-100" style="min-width:200px;width:35%;">
                    <label class="mx-3">Result</label>
                    <textarea class="form-control form-control-sm fs-d8 h-100" rows="25" id="lookupServiceResult"></textarea>
                </div>
            </div>
        </div>
        <div class="card-footer p-3 bg-secondary-subtle bg-gradient border-0 rounded-0">
            <div class="row">
                <div class="col-24">
                    <button class="btn btn-sm btn-secondary w-100 py-2" @click="cancel" data-ae-key="ok">
                        <i class="fa-solid fa-cancel"></i>
                        &nbsp;
                        <span>Cancel</span>
                    </button>
                </div>
                <div class="col-24">
                    <button class="btn btn-sm btn-primary w-100 py-2" @click="ok" data-ae-key="ok">
                        <i class="fa-solid fa-check"></i>
                        &nbsp;
                        <span>Ok</span>
                    </button>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    let _this = { cid: "", c: null, inputs: {}, dbConfName: "", baseTable: "", requestTemplates: [], selectedTemplate: "" };
    _this.dbConfName = getQueryString("cnn");
    _this.baseTable = getQueryString("o");

    export default {
        methods: {
            ok(e) {

                let v = JSON.parse($("#lookupServiceBody").val());
                v["Id"]=`${_this.c.inputs.ColName}_Lookup`;

                if (_this.inputs.callback) _this.inputs.callback(JSON.stringify(v, null, 4));
                shared.closeComponent(_this.cid);
            },
            cancel(e) {
                shared.closeComponent(_this.cid);
            },
            execLookupRequest() {
                rpc({
                    requests: [JSON.parse($("#lookupServiceBody").val())],
                    onDone: function (res) {
                        $("#lookupServiceResult").val(JSON.stringify(R0R(res), null, '\t'));
                    }
                });
            },
            insertTemplate() {
                if (fixNull(_this.c.selectedTemplate, '') === '') return;
                rpcAEP("GetFileContent", { "PathToRead": _this.c.turnToFullPath(_this.c.selectedTemplate) }, function (res) {
                    _this.c.insertLookupService(R0R(res));
                });
            },
            insertLookupService(str) {
                if (fixNull(str, '') === '') str = _this.c.getSampleRequest();
                $("#lookupServiceBody").val(JSON.stringify(JSON.parse(str), null, '\t'));
            },
            loadRequestTemplates() {
                rpcAEP("GetStoredApiCalls", {}, function (res) {
                    _this.c.requestTemplates = R0R(res);
                });
            },
            turnToFullPath(fileName) {
                return 'workspace/apicalls/' + fileName + '.json';
            },
            getSampleRequest() {
                return `{
  "Id":"${_this.c.inputs.ColName}_Lookup",
  "Method": "DefaultRepo.Common_BaseInfo.ReadList",
  "Inputs": {
    "ClientQueryJE": {
        "QueryFullName": "DefaultRepo.Common_BaseInfo.ReadList",
        "Where": {"CompareClauses": [{"Name": "ParentId","Value": 10000,"ClauseOperator": "Equal"}]
      },
      "OrderClauses": [{"Name": "ViewOrder", "OrderDirection": "ASC"}],
      "Pagination": {"PageNumber": 1,"PageSize": 500},
      "AggregationsContainment": "ExcludeAll",
      "IncludeSubQueries": false
    }
  }
}`;
            }
        },
        setup(props) {
            _this.cid = props['cid'];
            _this.inputs = shared["params_" + _this.cid];
        },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { _this.c.loadRequestTemplates(); },
        props: { cid: String }
    }

</script>
