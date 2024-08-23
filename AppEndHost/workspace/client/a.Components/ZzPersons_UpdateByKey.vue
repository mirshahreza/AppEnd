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
        <div class="card-body bg-primary-subtle-light scrollable">
            <div class="card rounded-1 border-light mb-1">
                <div class="card-body">
                    <div class="row">
                        <div class="col-48" v-if="inputs.fkColumn!=='FirstName'">
                            <label class="fs-d8 text-muted ms-2" for="input_FirstName">{{shared.translate('FirstName')}}</label>
                            <input type="text" class="form-control form-control-sm" id="input_FirstName" v-model="row.FirstName" data-ae-validation-required="false" data-ae-validation-rule=":=s(0,64)">
                        </div>
                        <div class="col-48" v-if="inputs.fkColumn!=='LastName'">
                            <label class="fs-d8 text-muted ms-2" for="input_LastName">{{shared.translate('LastName')}}</label>
                            <input type="text" class="form-control form-control-sm" id="input_LastName" v-model="row.LastName" data-ae-validation-required="false" data-ae-validation-rule=":=s(0,64)">
                        </div>
                    </div>
                </div>
            </div>
            <div class="card rounded-1 border-light mb-1">
                <div class="card-body">
                    <div class="row">
                        <div class="col-48" v-if="inputs.fkColumn!=='GenderId'">
                            <label class="fs-d8 text-muted ms-2" for="input_GenderId">{{shared.translate('GenderId')}}</label>
                            <select class="form-select form-select-sm" v-model="row.GenderId" data-ae-validation-required="false">
                                <option value="">-</option>
                                <option v-for="i in shared.enum(10000)" :value="i['Id']">{{i.Title}}</option>
                            </select>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-48">
                <div class="col-48">
                    <div class="card mt-3">
                        <div class="card-header">
                            <table class="w-100">
                                <tbody>
                                    <tr>
                                        <td class="text-start">
                                            <div class="d-inline-block" id="btn_ZzPersonsPhotos">
                                                <input type="file" accept="*" style="visibility:hidden;display:none;" multiple="">
                                                <button class="btn btn-sm btn-outline-primary" @click="selectFiles('ZzPersonsPhotos','btn_ZzPersonsPhotos','Picture_FileBody','Picture_FileName','Picture_FileSize','Picture_FileMime');">
                                                    <i class="fa-solid fa-fw fa-plus"></i>{{shared.translate("AddFiles")}}
                                                </button>
                                            </div>
                                        </td>
                                        <td class="text-end">
                                            <span class="fw-bold text-dark fs-d9">{{Relations['ZzPersonsPhotos'].length}}</span>
                                            <span class="fw-bold text-secondary fs-d8">
                                                file(s)
                                            </span>
                                            /
                                            <span class="fw-bold text-dark fs-d9">{{shared.bytesToSize(shared.ld().sumBy(Relations['ZzPersonsPhotos'], function (o) { return shared.fixNull(o.Picture_FileBody,'').length; }))}}</span>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                        <div class="card-body data-ae-filearea data-ae-validation" data-ae-validation-required="false" data-ae-validation-rule=":=n(0)">
                            <div class="badge" v-for="(f,ind) in Relations['ZzPersonsPhotos']">
                                <table class="w-100">
                                    <tbody>
                                        <tr>
                                            <td></td>
                                            <td style="width:100px;">
                                                <div style="height:100px;width:100px;">
                                                    <div data-ae-widget="aeFileField" data-ae-widget-options="{&quot;accept&quot;:&quot;image/x-png,image/gif,image/jpeg&quot;,&quot;resize&quot;:true,&quot;resizeMaxWidth&quot;:950,&quot;resizeMaxHeight&quot;:950,&quot;maxSize&quot;:800000}" class="ae-file-field w-100 h-100 border border-2 rounded-circle pointer data-ae-validation text-dark">
                                                        <input type="hidden" class="FileBody" v-model="f['Picture_FileBody']" data-ae-validation-required="true">
                                                        <input type="hidden" class="FileName" v-model="f['Picture_FileName']">
                                                        <input type="hidden" class="FileSize" v-model="f['Picture_FileSize']">
                                                        <input type="hidden" class="FileMime" v-model="f['Picture_FileMime']">
                                                        <span @click="deleteRelation({relationTable:'ZzPersonsPhotos',ind:ind})" class="btn btn-sm btn-light pointer" style="padding:0px 1px 0px 1px !important;margin-top:88px !important;font-size:10px;">
                                                            <i class="fa-solid fa-fw fa-remove text-danger"></i>
                                                        </span>
                                                    </div>
                                                </div>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td colspan="3">
                                                <div class="mt-2 text-dark">{{shared.truncateString(f['Picture_FileName'],15)}}</div>
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
        <div class="card-footer p-0 bg-secondary-subtle bg-gradient border-0 rounded-0" v-if="ismodal==='true'">
            <button class="btn btn-link text-decoration-none bg-hover-light w-100 py-3 rounded-0" @click="ok" data-ae-key="ok">
                <i class="fa-solid fa-save pe-1"></i>
                <span>{{shared.translate("Save")}}</span>
            </button>
        </div>
    </div>
</template>
<script>
    let _this = { cid: "", ismodal: "", c: null, templateType: "UpdateByKey", inputs: {}, dbConfName: "", objectName: "", loadMethod: "", submitMethod: "", masterRequest: {}, initialRequests: [], pickerRequests: [], pickerHumanIds: [], initialResponses: [], row: {}, Relations: {}, RelationsMetaData: {}, createComponent: "", updateComponent: "", regulator: null };
    _this.dbConfName = "DefaultRepo";
    _this.objectName = "ZzPersons";
    _this.submitMethod = "UpdateByKey";
    _this.createComponent = "";
    _this.updateComponent = "";

    _this.masterRequest = { "Id": "", "Method": "DefaultRepo.ZzPersons.ReadByKey", "Inputs": { "ClientQueryJE": { "QueryFullName": "DefaultRepo.ZzPersons.ReadByKey", "Params": [{ "Name": "Id", "Value": "" }] } } };
    _this.Relations['ZzPersonsPhotos'] = [];
    _this.RelationsMetaData['PersonPhotos'] = { "RelationName": "PersonPhotos", "RelationTable": "ZzPersonsPhotos", "RelationPkColumn": "Id", "RelationFkColumn": "PersonId", "RelationType": "OneToMany", "CreateQuery": "Create", "ReadListQuery": "ReadList", "UpdateByKeyQuery": "UpdateByKey", "DeleteByKeyQuery": "DeleteByKey", "DeleteQuery": "Delete", "IsFileCentric": true, "RelationUiWidget": "Grid" };

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
        created() {
            _this.c = this;
            assignDefaultMethods(_this);
        },
        mounted() {
            initVueComponent(_this);
            _this.c.loadMasterRecord();
            _this.c.componentFinalization();
        },
        props: { cid: String, ismodal: String }
    }
</script>