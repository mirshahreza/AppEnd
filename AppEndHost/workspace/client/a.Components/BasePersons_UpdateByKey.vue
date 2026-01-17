<template>
    <div class="card h-100 bg-transparent rounded-0 border-0" data-did="d-914006-0" draggable="true">
        <div class="card-header p-2 bg-success-subtle rounded-0 border-0" v-if="ismodal!=='true'" data-did="d-69400-1" draggable="true">
            <div class="hstack gap-1" data-did="d-942857-2" draggable="true">
                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="ok" data-ae-key="ok" data-did="d-511729-3" draggable="true">
                    <i class="fa-solid fa-save pe-1" data-did="d-388810-4" draggable="true">
                    </i>
                    <span data-did="d-726211-5" draggable="true">{{shared.translate("Save")}}</span>
                </button>
                <div class="p-0 ms-auto" data-did="d-583318-6" draggable="true">
                </div>
            </div>
        </div>
        <div class="card-body bg-primary-subtle-light scrollable" data-did="d-842351-7" draggable="true" id="formArea" data-ae-widget="inputsRegulator" data-ae-widget-options='{}'>

            <div class="card rounded-1 border-light mb-1" data-did="d-137139-8" draggable="true">
                <div class="card-body p-2" data-did="d-84259-9" draggable="true">
                    <div class="row d-table-row" data-did="d-299393-10" draggable="true">
                        <div class="col-12 d-table-cell text-center" style="vertical-align:middle" data-did="d-334382-11" draggable="true">
                            <table class="w-100" data-did="d-742396-12" draggable="true">
                                <tbody data-did="d-165715-13" draggable="true">
                                    <tr data-did="d-254898-14" draggable="true">
                                        <td data-did="d-521643-15" draggable="true">
                                        </td>
                                        <td style="width:100px;" class="py-3" data-did="d-453385-16" draggable="true">
                                            <div style="height:100px;width:100px;" data-did="d-430526-17" draggable="true">
                                                <div class="ae-file-field w-100 h-100 border border-2 rounded-circle pointer data-ae-validation" data-ae-widget="aeFileField" data-ae-widget-options="{&quot;accept&quot;:&quot;image/x-png,image/gif,image/jpeg&quot;,&quot;resize&quot;:true,&quot;resizeMaxWidth&quot;:950,&quot;resizeMaxHeight&quot;:950,&quot;maxSize&quot;:800000}" data-did="d-192823-18" draggable="true">
                                                    <input type="hidden" class="FileBody" v-model="row['Picture_FileBody']" data-ae-validation-required="false" data-did="d-278457-19" draggable="true">
                                                    <input type="hidden" class="FileName" v-model="row['Picture_FileName']" data-did="d-77836-20" draggable="true">
                                                    <input type="hidden" class="FileSize" v-model="row['Picture_FileSize']" data-did="d-903553-21" draggable="true">
                                                    <input type="hidden" class="FileMime" v-model="row['Picture_FileMime']" data-did="d-232249-22" draggable="true">
                                                </div>
                                            </div>
                                            <div class="fw-bold text-secondary text-center font-monospace" data-did="d-845795-24" draggable="true">
                                                {{row.UserId}}
                                            </div>
                                        </td>
                                        <td style="width:50px;">

                                        </td>
                                        <td data-did="d-67068-23" draggable="true" class="px-4">

                                            <div class="text-center mb-1" data-did="d-250597-26" draggable="true">
                                                <i class="fa-solid fa-fw fa-check fa-2x text-success" v-if="row.RecordStateId==='101.1'" data-did="d-420950-27" draggable="true"></i>
                                                <i class="fa-solid fa-fw fa-clock-four fa-2x text-warning" v-if="row.RecordStateId==='101.2'" data-did="d-723597-28" draggable="true"></i>
                                                <i class="fa-solid fa-fw fa-ban fa-2x text-danger" v-if="row.RecordStateId==='101.3'" data-did="d-73701-29" draggable="true"></i>
                                            </div>
                                            <div class="" data-did="d-545355-30" draggable="true">
                                                <select class="form-select form-select-sm" v-model="row.RecordStateId" data-ae-validation-required="false" data-did="d-182943-31" draggable="true">
                                                    <option value="" data-did="d-339986-32" draggable="true">-</option>
                                                    <option v-for="i in shared.enum(101)" :value="i['Id']" data-did="d-709123-33" draggable="true">{{i.Title}}</option>
                                                </select>
                                            </div>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                            
                        </div>
                        
                    </div>
                </div>
            </div>

            <div class="card rounded-1 border-light mb-1">
                <div class="card-body">
                    <div class="row">
                        <div class="col-16 d-table-cell px-3" style="vertical-align:middle">
                            <label class="fs-d8 text-muted ms-2" for="input_EntityTypeId">{{shared.translate('EntityTypeId')}}</label>
                            <select class="form-select form-select-sm" id="input_EntityTypeId" v-model="row.EntityTypeId" data-ae-validation-required="true" disabled>
                                <option value="">-</option>
                                <option v-for="i in shared.enum(102)" :value="i['Id']">{{i.Title}}</option>
                            </select>
                        </div>

                        <div class="col-12">
                            <label class="fs-d8 text-muted ms-2" for="input_NationalCode" v-if="row.EntityTypeId==='102.1'">{{shared.translate('NationalCode')}}</label>
                            <label class="fs-d8 text-muted ms-2" for="input_NationalCode" v-if="row.EntityTypeId==='102.2'">{{shared.translate('RegistrationNumber')}}</label>
                            <input type="text" class="form-control form-control-sm text-center ltr font-monospace" id="input_NationalCode" v-model="row.NationalCode" data-ae-validation-required="false" data-ae-validation-rule=":=s(0,16)">
                        </div>

                    </div>
                </div>
            </div>

            <div class="card rounded-1 border-light mb-1" v-if="row.EntityTypeId==='102.1'">
                <div class="card-body">
                    <div class="row">
                        <div class="col-12">
                            <label class="fs-d8 text-muted ms-2" for="input_GenderId">{{shared.translate('Gender')}}</label>
                            <select class="form-select form-select-sm" v-model="row.GenderId" data-ae-validation-required="true">
                                <option v-for="i in shared.enum(100)" :value="i['Id']">{{i.Title}}</option>
                            </select>
                        </div>
                        <div class="col-12">
                            <label class="fs-d8 text-muted ms-2" for="input_FirstName">{{shared.translate('FirstName')}}</label>
                            <input type="text" class="form-control form-control-sm" id="input_FirstName" v-model="row.FirstName" data-ae-validation-required="true" data-ae-validation-rule=":=s(0,64)">
                        </div>
                        <div class="col-12">
                            <label class="fs-d8 text-muted ms-2" for="input_LastName">{{shared.translate('LastName')}}</label>
                            <input type="text" class="form-control form-control-sm" id="input_LastName" v-model="row.LastName" data-ae-validation-required="true" data-ae-validation-rule=":=s(0,64)">
                        </div>
                    </div>
                </div>
            </div>

            <div class="card rounded-1 border-light mb-1" v-if="row.EntityTypeId==='102.2'">
                <div class="card-body">
                    <div class="row">
                        <div class="col-48">
                            <label class="fs-d8 text-muted ms-2" for="input_Title">{{shared.translate('Title')}}</label>
                            <input type="text" class="form-control form-control-sm" id="input_Title" v-model="row.Title" data-ae-validation-required="true" data-ae-validation-rule=":=s(0,128)">
                        </div>
                    </div>
                </div>
            </div>

            <div class="card rounded-1 border-light mb-1" data-did="d-334375-65" draggable="true">
                <div class="card-body" data-did="d-59087-66" draggable="true">
                    <div class="row" data-did="d-836769-67" draggable="true">
                        <div class="col-12" data-did="d-914508-68" draggable="true">
                            <label class="fs-d8 text-muted ms-2" for="input_Mobile" data-did="d-378777-69" draggable="true">{{shared.translate('Mobile')}}</label>
                            <input type="text" class="form-control form-control-sm text-center ltr font-monospace" id="input_Mobile" v-model="row.Mobile" data-ae-validation-required="true" data-ae-validation-rule=":=s(0,14)" data-did="d-259667-70" draggable="true">
                        </div>
                        <div class="col-12" data-did="d-914508-68" draggable="true">
                            <label class="fs-d8 text-muted ms-2" for="input_LandPhone" data-did="d-378777-69" draggable="true">{{shared.translate('LandPhone')}}</label>
                            <input type="text" class="form-control form-control-sm text-center ltr font-monospace" id="input_LandPhone" v-model="row.LandPhone" data-ae-validation-required="false" data-ae-validation-rule=":=s(0,14)" data-did="d-259667-70" draggable="true">
                        </div>
                    </div>
                </div>
            </div>

            <div class="card rounded-1 border-light mb-1">
                <div class="card-header">
                    <span v-if="row.EntityTypeId==='102.1'">{{shared.translate('BirthDate')}}</span>
                    <span v-if="row.EntityTypeId==='102.2'">{{shared.translate('RegistrationDate')}}</span>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-12">
                            <label class="fs-d8 text-muted ms-2" for="input_BirthYear">{{shared.translate('Year')}}</label>
                            <input type="text" class="form-control form-control-sm text-center ltr font-monospace" id="input_BirthYear" v-model="row.BirthYear" data-ae-validation-required="false" data-ae-validation-rule=":=i(0,2147483647)">
                        </div>
                        <div class="col-12">
                            <label class="fs-d8 text-muted ms-2" for="input_BirthMonth">{{shared.translate('Month')}}</label>
                            <input type="text" class="form-control form-control-sm text-center ltr font-monospace" id="input_BirthMonth" v-model="row.BirthMonth" data-ae-validation-required="false" data-ae-validation-rule="">
                        </div>
                        <div class="col-12">
                            <label class="fs-d8 text-muted ms-2" for="input_BirthDay">{{shared.translate('Day')}}</label>
                            <input type="text" class="form-control form-control-sm text-center ltr font-monospace" id="input_BirthDay" v-model="row.BirthDay" data-ae-validation-required="false" data-ae-validation-rule="">
                        </div>
                    </div>
                </div>
            </div>

            <div class="card rounded-1 border-light mb-1" data-did="d-751749-71" draggable="true">
                <div class="card-body" data-did="d-297861-72" draggable="true">
                    <div class="row" data-did="d-80130-73" draggable="true">
                        <div class="col-12 text-center" data-did="d-523061-74" draggable="true">
                            <label class="fs-d8 text-muted ms-2" data-did="d-257821-75" draggable="true">{{shared.translate('CreatedOn')}}</label>
                            <div type="text" class="form-control form-control-sm text-center ltr font-monospace disabled" data-did="d-618336-76" draggable="true">{{shared.formatDate(row.CreatedOn)}}</div>
                        </div>
                        <div class="col-12 text-center" data-did="d-871308-77" draggable="true">
                            <label class="fs-d8 text-muted ms-2" data-did="d-665884-78" draggable="true">{{shared.translate('CreatedBy')}}</label>
                            <div type="text" class="form-control form-control-sm text-center ltr font-monospace disabled" data-did="d-887109-79" draggable="true">{{row.CreatedBy_UserName}}</div>
                        </div>
                        <div class="col-12 text-center" data-did="d-431929-80" draggable="true">
                            <label class="fs-d8 text-muted ms-2" data-did="d-373831-81" draggable="true">{{shared.translate('UpdatedOn')}}</label>
                            <div type="text" class="form-control form-control-sm text-center ltr font-monospace disabled" data-did="d-95288-82" draggable="true">{{shared.formatDate(row.UpdatedOn)}}</div>
                        </div>
                        <div class="col-12 text-center" data-did="d-797686-83" draggable="true">
                            <label class="fs-d8 text-muted ms-2" data-did="d-945000-84" draggable="true">{{shared.translate('UpdatedBy')}}</label>
                            <div type="text" class="form-control form-control-sm text-center ltr font-monospace disabled" data-did="d-709257-85" draggable="true">{{row.UpdatedBy_UserName}}</div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="card-footer p-0" v-if="ismodal==='true'" data-did="d-139238-86" draggable="true">
            <div class="container-fluid pt-2 pb-1" data-did="d-393409-87" draggable="true">
                <div class="row p-0" data-did="d-768331-88" draggable="true">
                    <div class="col-36 px-2" data-did="d-410052-89" draggable="true">
                        <button class="btn btn-sm btn-primary w-100" @click="ok" data-ae-key="ok" data-did="d-672909-90" draggable="true">
                            <i class="fa-solid fa-check me-1" data-did="d-82006-91" draggable="true">
                            </i>
                            <span data-did="d-258050-92" draggable="true">{{shared.translate("Save")}}</span>
                        </button>
                    </div>
                    <div class="col-12 px-2" data-did="d-140365-93" draggable="true">
                        <button class="btn btn-sm btn-secondary w-100" @click="cancel" data-did="d-195272-94" draggable="true">
                            <i class="fa-solid fa-xmark me-1" data-did="d-257171-95" draggable="true">
                            </i>
                            <span data-did="d-244252-96" draggable="true">{{shared.translate("Cancel")}}</span>
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
    _this.objectName = "BasePersons";
    _this.submitMethod = "UpdateByKey";
    _this.createComponent = "";
    _this.updateComponent = "";

    _this.masterRequest = { "Id": "", "Method": "DefaultRepo.BasePersons.ReadByKey", "Inputs": { "ClientQueryJE": { "QueryFullName": "DefaultRepo.BasePersons.ReadByKey", "Params": [{ "Name": "Id", "Value": "" }] } } };

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
        mounted() { _this.c.loadMasterRecord(function () { initVueComponent(_this); }); _this.c.componentFinalization(); },
        props: { cid: String, ismodal: String }
    }

</script>