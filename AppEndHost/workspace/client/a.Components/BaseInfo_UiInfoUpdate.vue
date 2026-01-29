<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-header p-2 bg-success-subtle rounded-0 border-0" v-if="ismodal!=='true'">
            <div class="hstack">
                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="ok" data-ae-key="ok">
                    <i class="fa-solid fa-save pe-1"></i>
                    <span>{{shared.translate("Save")}}</span>
                </button>
                <div class="p-0 ms-auto"></div>
            </div>
        </div>
        <div class="card-body bg-primary-subtle-light scrollable">
            <div class="card rounded-1 border-light mb-1">
                <div class="card-body">
                    <div class="row">
                        <div class="col-48">
                            <label class="fs-d8 text-muted ms-2" for="input_UiColor">{{shared.translate('UiColor')}}</label>
                            <div class="input-group input-group-sm border-0" style="text-align:left !important;">
                                <span class="form-control form-control-sm bg-transparent">{{row.UiColor}}</span>
                                <input type="color" class="input-group-text p-3" :style="'background-color:'+row.UiColor" id="input_UiColor" v-model="row.UiColor" data-ae-validation-required="false" data-ae-validation-rule=":=s(0,16)">
                            </div>
                        </div>
                        <div class="col-48">
                            <label class="fs-d8 text-muted ms-2" for="input_UiClass">{{shared.translate('UiClass')}}</label>
                            <input type="text" class="form-control form-control-sm" style="text-align:left !important;" id="input_UiClass" v-model="row.UiClass" data-ae-validation-required="false" data-ae-validation-rule=":=s(0,64)">
                        </div>
                        <div class="col-48">
                            <label class="fs-d8 text-muted ms-2" for="input_UiIcon">{{shared.translate('UiIcon')}}</label>
                            <input type="text" class="form-control form-control-sm" style="text-align:left !important;" id="input_UiIcon" v-model="row.UiIcon" data-ae-validation-required="false" data-ae-validation-rule=":=s(0,64)">
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="card-footer p-0" v-if="ismodal==='true'">
            <div class="container-fluid pt-2 pb-1">
                <div class="row p-0">
                    <div class="col-36 px-2">
                        <button class="btn btn-sm btn-primary w-100" @click="ok" data-ae-key="ok">
                            <i class="fa-solid fa-check me-1"></i>
                            <span>{{shared.translate("Save")}}</span>
                        </button>
                    </div>
                    <div class="col-12 px-2">
                        <button class="btn btn-sm btn-secondary w-100" @click="cancel">
                            <i class="fa-solid fa-xmark me-1"></i>
                            <span>{{shared.translate("Cancel")}}</span>
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
let _this = { cid: "", ismodal:"", c: null, templateType:"UpdateByKey", inputs: {}, dbConfName: "", objectName: "", loadMethod: "", submitMethod: "", masterRequest: {}, initialRequests: [], pickerRequests: [], pickerHumanIds: [], initialResponses: [], row: {}, Relations: {}, RelationsMetaData: {}, createComponent: "", updateComponent: "" };
_this.dbConfName = "DefaultRepo";
_this.objectName = "BaseInfo";
_this.submitMethod = "UiInfoUpdate";
_this.createComponent = "";
_this.updateComponent = "";

_this.masterRequest = {"Id":"","Method":"DefaultRepo.BaseInfo.ReadByKey","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.BaseInfo.ReadByKey","Params":[{"Name":"Id","Value":""}]}}};

_this.pickerRequests.push({"Id":"ParentId_Lookup","Method":"DefaultRepo.BaseInfo.ReadList","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.BaseInfo.ReadList","OrderClauses":[{"Name":"ViewOrder","OrderDirection":"ASC"}],"Pagination":{"PageNumber":1,"PageSize":500},"IncludeSubQueries":false}}});

_this.pickerHumanIds.push({Id:'ParentId_HumanIds',Items:["Title"]});

export default {
        methods: {
        },
        setup(props) {
            _this.cid = props['cid'];
            _this.ismodal = props['ismodal'];
            _this.inputs = shared["params_" + _this.cid];
            if(fixNull(getQueryString("key"),'') !== '') {
                _this.inputs = {};
                _this.inputs["key"] = getQueryString("key");
            }
        },
        data() { return _this; },
        created() { _this.c = this; assignDefaultMethods(_this); },
        mounted() { initVueComponent(_this); _this.c.loadMasterRecord(); _this.c.componentFinalization(); },
        props: { cid: String, ismodal: String }
}

</script>