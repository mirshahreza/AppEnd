<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-body bg-primary-subtle-light scrollable">
            <div class="card rounded-1 border-light mb-1">
                <div class="card-body">
                    <div class="row">
                        <div class="col-48">
                            <label class="fs-d8 text-muted ms-2" for="input_TokensQuantity">{{shared.translate('TokensQuantity')}}</label>
                            <input type="text" class="form-control form-control-lg text-center" style="direction:ltr" id="input_TokensQuantity" v-model="row.TokensQuantity" 
                                   data-ae-validation-required="true" data-ae-validation-rule=":=i(0,2147483647)">
                        </div>
                        <div class="col-48">
                            <label class="fs-d8 text-muted ms-2" for="input_DueDate">{{shared.translate('DueDate')}}</label>
                            <div class="input-group input-group-sm data-ae-validation">
                                <button class="btn btn-sm btn-outline-secondary" id="dp_DueDate" data-ae-widget="dtPicker"
                                        data-ae-widget-options="{&quot;modalMode&quot;:&quot;true&quot;,&quot;targetTextSelector&quot;:&quot;#dpText_DueDate&quot;,&quot;targetDateSelector&quot;:&quot;#dpDate_DueDate&quot;,&quot;dateFormat&quot;:&quot;yyyy-MM-dd&quot;,&quot;textFormat&quot;:&quot;yyyy-MM-dd&quot;}">
                                    <i class="fa-solid fa-fw fa-calendar"></i>
                                </button>
                                <input class="form-control form-control-sm text-center" style="direction:ltr" id="dpText_DueDate" >
                                <input class="form-control form-control-sm" id="dpDate_DueDate" v-model="row.DueDate" 
                                        data-ae-validation-required="true" data-ae-validation-rule=":=d(1900-01-01,2100-12-30)">
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="card-footer p-0 bg-secondary-subtle bg-gradient border-0 rounded-0">
            <button class="btn btn-link text-decoration-none bg-hover-light w-100 py-3 rounded-0" @click="ok2" data-ae-key="ok">
                <i class="fa-solid fa-save pe-1"></i>
                <span>{{shared.translate("Save")}}</span>
            </button>
        </div>
    </div>
</template>
<script>
    let _this = { cid: "", ismodal: "", c: null, templateType: "Create", inputs: {}, dbConfName: "", objectName: "", submitMethod: "", initialRequests: [], initialResponses: [], pickerRequests: [], pickerHumanIds: [], row: {}, Relations: {}, RelationsMetaData: {}, regulator: null };
    _this.dbConfName = "DefaultRepo";
    _this.objectName = "TokensDocuments";
    _this.submitMethod = "Create";
    _this.row = { "TokensQuantity": 1, "DueDate": "2025-01-01" };
    _this.pickerRequests.push({ "Id": "IssuerMemberId_Lookup", "Method": "DefaultRepo.Members.ReadList", "Inputs": { "ClientQueryJE": { "QueryFullName": "DefaultRepo.Members.ReadList", "OrderClauses": [{ "Name": "Title", "OrderDirection": "ASC" }], "Pagination": { "PageNumber": 1, "PageSize": 500 }, "IncludeSubQueries": false } } });
    _this.pickerHumanIds.push({ Id: 'IssuerMemberId_HumanIds', Items: ["UserName", "FirstName", "LastName", "Title"] });
    export default {
        methods: {
            ok2: function () {
                alert(JSON.stringify(_this.row));
            }
        },
        setup(props) {
            _this.cid = props['cid'];
            _this.ismodal = props['ismodal'];
            _this.inputs = shared["params_" + _this.cid];
        },
        data() { return _this; },
        created() { _this.c = this; assignDefaultMethods(_this); },
        mounted() { initVueComponent(_this); _this.c.loadBaseInfo(); _this.c.componentFinalization(); },
        props: { cid: String, ismodal: String }
    }

</script>