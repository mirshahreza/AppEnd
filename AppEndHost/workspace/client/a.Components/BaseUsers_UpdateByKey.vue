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
                        <div class="col-48" v-if="inputs.fkColumn!=='UserName'">
                            <label class="fs-d8 text-muted ms-2" for="input_UserName">{{shared.translate('UserName')}}</label>
                            <input disabled="" type="text" class="form-control form-control-sm" id="input_UserName" v-model="row.UserName" data-ae-validation-required="true" data-ae-validation-rule=":=s(0,64)">
                        </div>
                    </div>
                </div>
            </div>
            <div class="card rounded-1 border-light mb-1">
                <div class="card-body">
                    <div class="row">
                        <div class="col-48" v-if="inputs.fkColumn!=='IsBuiltIn'">
                            <div class="form-control mt-2 pointer text-nowrap data-ae-validation " data-ae-widget="nullableCheckbox">
                                <i class="fa-solid fa-fw me-1"></i>
                                <span>{{shared.translate('IsBuiltIn')}}</span>
                                <input type="hidden" v-model="row.IsBuiltIn" data-ae-validation-required="true">
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="card rounded-1 border-light mb-1">
                <div class="card-header text-bg-light p-1">
                    {{shared.translate('Contact')}}
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-48" v-if="inputs.fkColumn!=='Email'">
                            <label class="fs-d8 text-muted ms-2" for="input_Email">{{shared.translate('Email')}}</label>
                            <input type="text" class="form-control form-control-sm" id="input_Email" v-model="row.Email" data-ae-validation-required="false" data-ae-validation-rule=":=s(0,128)">
                        </div>
                        <div class="col-48" v-if="inputs.fkColumn!=='Mobile'">
                            <label class="fs-d8 text-muted ms-2" for="input_Mobile">{{shared.translate('Mobile')}}</label>
                            <input type="text" class="form-control form-control-sm" id="input_Mobile" v-model="row.Mobile" data-ae-validation-required="false" data-ae-validation-rule=":=s(0,14)">
                        </div>
                    </div>
                </div>
            </div>
            <div class="card rounded-1 border-light mb-1">
                <div class="card-header text-bg-light p-1">
                    {{shared.translate('RolesOfUser')}}
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-48">
                            <div class="form-control form-control-sm pb-0 data-ae-validation" data-ae-validation-required="false" data-ae-validation-rule=":=n(0)">
                                <div class="form-check form-check-inline" v-for="i in shared.getResponseObjectById(initialRequests, initialResponses, row, 'RoleId_Lookup')">
                                    <input class="form-check-input" type="checkbox" v-model="Relations.BaseUsersRoles" :value="i.Id" :id="i.Id+'RoleId_Lookup'">
                                    <label class="form-check-label" :for="i.Id+'RoleId_Lookup'">
                                        {{i.RoleName}}
                                    </label>
                                </div>
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
    _this.objectName = "BaseUsers";
    _this.submitMethod = "UpdateByKey";
    _this.createComponent = "";
    _this.updateComponent = "";
    _this.masterRequest = { "Id": "", "Method": "DefaultRepo.BaseUsers.ReadByKey", "Inputs": { "ClientQueryJE": { "QueryFullName": "DefaultRepo.BaseUsers.ReadByKey", "Params": [{ "Name": "Id", "Value": "" }] } } };
    _this.initialRequests.push({ "Id": "RoleId_Lookup", "Method": "DefaultRepo.BaseRoles.ReadList", "Inputs": { "ClientQueryJE": { "QueryFullName": "DefaultRepo.BaseRoles.ReadList", "OrderClauses": [{ "Name": "RoleName", "OrderDirection": "ASC" }], "Pagination": { "PageNumber": 1, "PageSize": 500 }, "IncludeSubQueries": false } } });
    _this.RelationsMetaData['RolesOfUser'] = { "RelationName": "RolesOfUser", "RelationTable": "BaseUsersRoles", "RelationPkColumn": "Id", "RelationFkColumn": "UserId", "RelationType": "ManyToMany", "LinkingTargetTable": "BaseRoles", "LinkingColumnInManyToMany": "RoleId", "CreateQuery": "Create", "ReadListQuery": "ReadList", "UpdateByKeyQuery": "UpdateByKey", "DeleteByKeyQuery": "DeleteByKey", "DeleteQuery": "Delete", "IsFileCentric": false, "RelationUiWidget": "CheckboxList" };

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