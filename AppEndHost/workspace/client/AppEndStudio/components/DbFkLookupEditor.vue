<template>
    <div class="card h-100 border-0 shadow-lg bg-transparent rounded-0">
        <div class="card-header fw-bold fs-d9">
            Lookup items comes from an internal service, define it and execute to see the result. You can simplify indicate a parent id of BaseInfo or write a JSON query to fetch items
        </div>
        <div class="card-header bg-light-subtle">
            <div class="input-group input-group-sm">
                <div class="input-group-text">ParentId</div>
                <input class="form-control form-control-sm" v-model="inputs.JsLookupParentId" />
            </div>
        </div>
        <div class="card-header bg-light">
            You can put an item from your dataset in the request body to cascading request call by control local values, for example:
            <br />
            <div class="fw-bold text-info">
                "ParentId" : "&[OtherFieldName]"
            </div>
        </div>

        <div class="card-body p-2 pb-4 bg-transparent fs-d8">
            <div class="h-100 w-100" data-flex-splitter-horizontal style="flex: auto;">
                <div class="card h-100" style="min-width:200px;width:60%;">
                    <div class="card-header p-1">
                        <select class="form-select form-select-sm" v-model="selectedTemplate" @change="insertTemplate">
                            <option value="">Select Request Template</option>
                            <option v-for="i in requestTemplates" :value="i">{{i}}</option>
                        </select>
                    </div>
                    <div class="card-body p-0">
                        <div class="data-ae-validation h-100">
                            <div class="code-editor-container h-100" data-ae-widget="editorBox" data-ae-widget-options="{&quot;mode&quot;: &quot;ace/mode/json&quot;}" id="ace_Lookup"></div>
                            <input type="hidden" v-model="inputs.Lookup" data-ae-validation-required="false" data-ae-validation-rule="" id="lookupServiceBody" />
                        </div>
                    </div>
                </div>
                <div role="separator" tabindex="1" class="bg-light" style="width:5%;">
                    <button class="btn btn-sm btn-light p-2" @click="execLookupRequest">
                        <i class="fa-solid fa-2x fa-play"></i>
                        <br />
                        <span >Exec</span>
                    </button>
                </div>
                <div class="card h-100" style="min-width:200px;width:35%;">
                    <div class="card-header">
                        <span>Result</span>
                    </div>
                    <div class="card-body p-0">
                        <div class="data-ae-validation h-100">
                            <div class="code-editor-container h-100" data-ae-widget="editorBox" data-ae-widget-options="{    &quot;mode&quot;: &quot;ace/mode/json&quot;}" id="ace_Result"></div>
                            <input type="hidden" v-model="inputs.Result" data-ae-validation-required="false" data-ae-validation-rule="" id="lookupServiceResult" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="card-footer p-0">
            <div class="container-fluid pt-2 pb-1">
                <div class="row p-0">
                    <div class="col-36 px-2">
                        <button class="btn btn-sm btn-primary w-100" @click="ok" data-ae-key="ok">
                            <i class="fa-solid fa-check me-1"></i>
                            <span>Ok</span>
                        </button>
                    </div>
                    <div class="col-12 px-2">
                        <button class="btn btn-sm btn-secondary w-100" @click="cancel">
                            <i class="fa-solid fa-xmark me-1"></i>
                            <span>Cancel</span>
                        </button>
                    </div>
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
                let ret = {};
                if (fixNull(_this.c.inputs.JsLookupParentId, '') !== '') {
                    ret["JsLookupParentId"] = _this.c.inputs.JsLookupParentId;
                } else {
                    let v = JSON.parse($("#lookupServiceBody").val());
                    v["Id"] = `${_this.c.inputs.ColName}_Lookup`;

                    if (JSON.stringify(v).length > 50) {
                        ret["Lookup"] = JSON.stringify(v, null, '\t');
                    } else {
                        ret["Lookup"] = JSON.stringify({}, null, '\t');
                    }
                }

                if (_this.inputs.callback) _this.inputs.callback(ret);
                shared.closeComponent(_this.cid);
            },
            cancel(e) {
                shared.closeComponent(_this.cid);
            },
            execLookupRequest() {
                try {
                    let lookupValue = fixNull(_this.inputs.Lookup, '').trim();
                    
                    if (lookupValue === '' || lookupValue === '{}') {
                        alert('Please create a valid request first.');
                        return;
                    }
                    
                    let parsedRequest = JSON.parse(lookupValue);
                    
                    if (!parsedRequest || Object.keys(parsedRequest).length === 0) {
                        alert('Please create a valid request first.');
                        return;
                    }
                    
                    rpc({
                        requests: [parsedRequest],
                        onDone: function (res) {
                            shared.editors["ace_Result"].getSession().setValue(JSON.stringify(R0R(res), null, '\t'));
                        }
                    });
                } catch (e) {
                    alert('Please create a valid request first.');
                }
            },
            insertTemplate() {
                if (fixNull(_this.c.selectedTemplate, '') === '') return;
                rpcAEP("GetFileContent", { "PathToRead": _this.c.turnToFullPath(_this.c.selectedTemplate) }, function (res) {
                    _this.c.insertLookupService(R0R(res));
                });
            },
            insertLookupService(str) {
                if (fixNull(str, '') === '') str = _this.c.getSampleRequest();
                shared.editors["ace_Lookup"].getSession().setValue(JSON.stringify(JSON.parse(str), null, '\t'));
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
  "Method": "DefaultRepo.BaseInfo.ReadList",
  "Inputs": {
    "ClientQueryJE": {
        "QueryFullName": "DefaultRepo.BaseInfo.ReadList",
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
            _this.inputs.Result = "";
        },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { initVueComponent(_this); _this.c.loadRequestTemplates(); },
        props: { cid: String }
    }

</script>
