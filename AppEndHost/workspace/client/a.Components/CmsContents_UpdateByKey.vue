<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-header p-2 bg-success-subtle rounded-0 border-0" v-if="ismodal!=='true'">
            <div class="hstack gap-1">
                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="ok" data-ae-key="ok">
                    <i class="fa-solid fa-save pe-1"></i>
                    <span>{{shared.translate("Save")}}</span>
                </button>
                <div class="p-0 ms-auto"></div>
            </div>
        </div>
        <div class="card-body bg-primary-subtle-light scrollable" id="formArea" data-ae-widget="inputsRegulator" data-ae-widget-options="{}">
    
            <div class="container-fluid h-100">
                <div class="row h-100">
                    <div class="col-6 h-100 px-2">
                        <div class="h-100 bg-light rounded-3 p-2">

                            <div class="card rounded-1 border-light mb-1">
                                <div class="card-body">
                                    <div class="row">
                                        <div class="col-48" id="container_ContentTypeId">
                                            <label class="fs-d8 text-muted ms-2" for="input_ContentTypeId">{{shared.translate('ContentTypeId')}}</label>
                                            <select class="form-select form-select-sm" v-model="row.ContentTypeId" data-ae-validation-required="false">
                                                <option value="">-</option>
                                                <option v-for="i in shared.enum(150)" :value="i['Id']">{{i.Title}}</option>
                                            </select>
                                        </div>
                                        <div class="col-48" id="container_CommentsPolicyId">
                                            <label class="fs-d8 text-muted ms-2" for="input_CommentsPolicyId">{{shared.translate('CommentsPolicyId')}}</label>
                                            <select class="form-select form-select-sm" v-model="row.CommentsPolicyId" data-ae-validation-required="false">
                                                <option value="">-</option>
                                                <option v-for="i in shared.enum(151)" :value="i['Id']">{{i.Title}}</option>
                                            </select>
                                        </div>
                                        <div class="col-48" id="container_LanguageId">
                                            <label class="fs-d8 text-muted ms-2" for="input_LanguageId">{{shared.translate('LanguageId')}}</label>
                                            <select class="form-select form-select-sm" v-model="row.LanguageId" data-ae-validation-required="false">
                                                <option value="">-</option>
                                                <option v-for="i in shared.enum(103)" :value="i['Id']">{{i.Title}}</option>
                                            </select>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="card rounded-1 border-light mb-1">
                                <div class="card-body p-1 fs-d7">
                                    <div class="row">
                                        <div class="col-48">
                                            <div class="input-group input-group-sm">
                                                <div class="input-group-text" style="min-width:100px">{{shared.translate('RecordStateId')}}</div>
                                                <div class="form-control">
                                                    <i :class="row.RecordStateId_UiIcon"></i>
                                                    {{row.RecordStateId_Title}}
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-48">
                                            <div class="input-group input-group-sm">
                                                <div class="input-group-text" style="min-width:100px">{{shared.translate('By')}}</div>
                                                <div class="form-control">{{row.RecordStateIdUpdatedBy_UserName}}</div>
                                            </div>
                                        </div>
                                        <div class="col-48">
                                            <div class="input-group input-group-sm">
                                                <div class="input-group-text" style="min-width:100px">{{shared.translate('On')}}</div>
                                                <div class="form-control">{{shared.formatDateL(row.RecordStateIdUpdatedOn)}}</div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="card rounded-1 border-light mb-1">
                                <div class="card-body p-1 pt-3 fs-d7">
                                    <div class="row">
                                        <div class="col-24 text-center">
                                            <div class="fs-d8 text-muted">{{shared.translate('CreatedBy')}}</div>
                                            <div class="form-control form-control-sm border-0">{{row.CreatedBy_UserName}}</div>
                                        </div>
                                        <div class="col-24 text-center">
                                            <div class="fs-d8 text-muted">{{shared.translate('CreatedOn')}}</div>
                                            <div class="form-control form-control-sm border-0">{{shared.formatDateL(row.CreatedOn)}}</div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-24 text-center">
                                            <div class="fs-d8 text-muted">{{shared.translate('UpdatedBy')}}</div>
                                            <div class="form-control form-control-sm border-0">{{row.UpdatedBy_UserName}}</div>
                                        </div>
                                        <div class="col-24 text-center">
                                            <div class="fs-d8 text-muted">{{shared.translate('UpdatedOn')}}</div>
                                            <div class="form-control form-control-sm border-0">{{shared.formatDateL(row.UpdatedOn)}}</div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                        </div>
                    </div>
                    <div class="col-34 h-100 px-2">
                        <div class="h-100 bg-light rounded-3 p-2">
                            <div class="card rounded-1 border-light mb-1">
                                <div class="card-header">
                                    <label class="fs-d8 text-muted ms-2" for="input_Title">{{shared.translate('Title')}}</label>
                                    <input type="text" class="form-control form-control-sm" id="input_Title" v-model="row.Title" data-ae-validation-required="true" data-ae-validation-rule=":=s(0,256)">
                                </div>
                                <div class="card-body">
                                    <table class="w-100">
                                        <tr>
                                            <td style="width:150px;">
                                                <table class="w-100">
                                                    <tbody>
                                                        <tr>
                                                            <td></td>
                                                            <td style="width:150px;" class="py-3">
                                                                <div style="height:150px;width:150px;">
                                                                    <div data-ae-widget="aeFileField" class="ae-file-field w-100 h-100 border border-2 rounded-circle pointer data-ae-validation ">
                                                                        <input type="hidden" class="FileBody" v-model="row.File_FileBody" data-ae-validation-required="true">
                                                                        <input type="hidden" class="FileName" v-model="row['File_FileName']">
                                                                        <input type="hidden" class="FileSize" v-model="row['File_FileSize']">
                                                                        <input type="hidden" class="FileMime" v-model="row['File_FileMime']">
                                                                    </div>
                                                                </div>
                                                            </td>
                                                            <td></td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </td>
                                            <td class="px-4">
                                                <label class="fs-d8 text-muted ms-2" for="input_Summary">{{shared.translate('Summary')}}</label>
                                                <textarea type="text" class="form-control form-control-sm w-100" rows="4" id="input_Summary" v-model="row.Summary" data-ae-validation-required="false" data-ae-validation-rule=":=s(0,4000)"></textarea>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </div>

                            <div class="card rounded-1 border-light mb-1">
                                <div class="card-body p-2" id="container_Body">
                                    <div class="border border-2 rounded-2 data-ae-validation">
                                        <textarea type="text" v-model="row.Body"
                                                  data-ae-widget="trumbowyg" id="input_Body"
                                                  data-ae-widget-options="{&quot;svgPath&quot;: &quot;/a..lib/Trumbowyg/ui/icons.svg&quot;}"
                                                  style="display:none" data-ae-validation-required="false" data-ae-validation-rule=":=s(0,9999999)"></textarea>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-8 h-100 px-2">
                        <div class="h-100 bg-light rounded-3 p-2">

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
    let _this = { cid: "", ismodal: "", c: null, templateType: "UpdateByKey", inputs: {}, dbConfName: "", objectName: "", loadMethod: "", submitMethod: "", masterRequest: {}, initialRequests: [], pickerRequests: [], pickerHumanIds: [], initialResponses: [], row: {}, Relations: {}, RelationsMetaData: {}, createComponent: "", updateComponent: "" };
    _this.dbConfName = "DefaultRepo";
    _this.objectName = "CmsContents";
    _this.submitMethod = "UpdateByKey";
    _this.createComponent = "";
    _this.updateComponent = "";

    _this.masterRequest = { "Id": "", "Method": "DefaultRepo.CmsContents.ReadByKey", "Inputs": { "ClientQueryJE": { "QueryFullName": "DefaultRepo.CmsContents.ReadByKey", "Params": [{ "Name": "Id", "Value": "" }] } } };

    export default {
        methods: {
        },
        setup(props) {
            _this.cid = props['cid'];
            _this.ismodal = props['ismodal'];
            _this.inputs = shared["params_" + _this.cid];
            if (fixNull(getQueryString("key"), '') !== '') {
                _this.inputs = {};
                _this.inputs["key"] = getQueryString("key");
            }
        },
        data() { return _this; },
        created() { _this.c = this; assignDefaultMethods(_this); },
        mounted() {
            _this.c.loadMasterRecord(function () {
                initVueComponent(_this);
            });
            _this.c.componentFinalization();
        },
        props: { cid: String, ismodal: String }
    }


</script>

