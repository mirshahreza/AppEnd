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
                    <div class="col-30 h-100 px-2">
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
                    <div class="col-12 h-100 px-2">
                        <div class="h-100 bg-light rounded-3 p-2">
                            <div class="card h-100">
                                <div class="card-header">
                                    <table class="w-100">
                                        <tbody>
                                            <tr>
                                                <td class="text-start">
                                                    <button class="btn btn-sm btn-outline-primary" @click="addRelation({relName:'CmsFiles'});">
                                                        <i class="fa-solid fa-fw fa-plus"></i>
                                                        {{shared.translate("AddFile")}}
                                                    </button>
                                                </td>
                                                <td class="text-end">
                                                    <span class="fw-bold text-dark fs-d9">{{Relations['CmsFiles'].length}}</span>
                                                    <span class="fw-bold text-secondary fs-d8">
                                                        row(s)
                                                    </span>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                                <div class="card-body p-0 data-ae-filearea data-ae-validation" data-ae-validation-required="false" data-ae-validation-rule=":=n(0,100)">
                                    <table class="table table-sm table-hover w-100 ae-table m-0 bg-transparent fs-d8">
                                        <thead>
                                            <tr class="d-none d-md-table-row d-lg-table-row d-xl-table-row">
                                                <th class="sticky-top ae-thead-th fb text-primary fw-bold text-center" style="width:30px;overflow: hidden;text-overflow: ellipsis;">
                                                    <i class="fa-solid fa-fw fa-window-restore"></i>
                                                </th>
                                                <th class="sticky-top ae-thead-th text-center" style="width:95px;overflow: hidden;text-overflow: ellipsis;">
                                                    <div>{{shared.translate("File")}}</div>
                                                </th>
                                                <th class="sticky-top ae-thead-th ">
                                                    <div>{{shared.translate("Note")}}</div>
                                                </th>
                                                <th style="width:40px;" class="sticky-top ae-thead-th text-center" data-ae-actions="DefaultRepo.CmsFiles.DeleteByKey"></th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr v-for="(i,ind) in Relations['CmsFiles']">
                                                <td class="ae-table-td text-primary bg-hover-light text-center pointer" @click="openById({compPath:'/a.Components/CmsFiles_UpdateByKey',recordKey:i.Id,refereshOnCallback:true,actionsAllowed:'DefaultRepo.CmsFiles.UpdateByKey',fkColumn:'ContentId'});">
                                                    <i class="fa-solid fa-fw fa-edit"></i>
                                                    <div class="pk font-monospace fs-d7 text-secondary">{{i.Id}}</div>
                                                </td>
                                                <td class="ae-table-td text-center">
                                                    <img :src="'data:image/png;base64, '+i.File_FileBody_xs" v-if="shared.fixNull(i.File_FileBody_xs,'')!==''" class="rounded-4 shadow-sm" style="width:95%;min-height:50px;max-height:50px;max-width:50px;">
                                                    <i class="fa-solid fa-fw fa-image fa-5x text-light" v-else=""></i>
                                                </td>
                                                <td class="ae-table-td ">
                                                    <div>{{i["Note"]}}</div>
                                                </td>
                                                <td style="width:40px;vertical-align:middle" class="text-center" data-ae-actions="DefaultRepo.CmsFiles.DeleteByKey">
                                                    <span @click="deleteRelation({relationTable:'CmsFiles',ind:ind})">
                                                        <i class="fa-solid fa-fw fa-times text-muted hover-danger pointer"></i>
                                                    </span>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
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
    _this.Relations['CmsFiles'] = [];
    _this.RelationsMetaData['RelatedFiles'] = { "RelationName": "RelatedFiles", "RelationTable": "CmsFiles", "RelationPkColumn": "Id", "RelationFkColumn": "ContentId", "RelationType": "OneToMany", "CreateQuery": "Create", "ReadListQuery": "ReadList", "UpdateByKeyQuery": "UpdateByKey", "DeleteByKeyQuery": "DeleteByKey", "DeleteQuery": "Delete", "OrderClauses":[{ Name: "IsBuiltIn", OrderDirection: "DESC" }], "IsFileCentric": false, "MaxN": "100", "RelationUiWidget": "Cards" };
    _this.RelationsMetaData['RelatedFiles']['createComponent'] = '/a.Components/CmsFiles_Create';
    _this.RelationsMetaData['RelatedFiles']['updateComponent'] = '/a.Components/CmsFiles_UpdateByKey';

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

