﻿<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-header p-2 bg-success-subtle rounded-0 border-0">
            <div class="row">
                <div class="col-48 col-md-6">
                    <div class="form-control form-control-sm pointer data-ae-validation" data-ae-widget="nullableCheckbox" data-ae-widget-options="{&quot;shownull&quot;:true}">
                        <i class="fa-solid fa-fw me-1"></i>
                        <span>{{shared.translate('IsBuiltIn')}}</span>
                        <input type="hidden" v-model="filter.IsBuiltIn" data-ae-validation-required="false">
                    </div>
                </div>
                <div class="col-48 col-md-6">
                    <input type="text" class="form-control form-control-sm" id="input_RoleName" @keyup.enter="loadRecords()" v-model="filter.RoleName" placeholder="Title">
                </div>
                <div class="col-48 col-md-6">
                    <input type="text" class="form-control form-control-sm" id="input_Note" @keyup.enter="loadRecords()" v-model="filter.Note" placeholder="Note">
                </div>
            </div>
        </div>
        <div class="card-header p-2 rounded-0">
            <div class="hstack gap-1">
                <button class="btn btn-sm btn-outline-primary px-3" @click="loadRecords()">
                    <i class="fa-solid fa-search me-1"></i> <span>{{shared.translate("Search")}}</span>
                </button>
                <button class="btn btn-sm btn-outline-secondary px-3" @click="resetSearchOptions">
                    <i class="fa-solid fa-eraser me-1"></i>
                    <span>{{shared.translate("Reset")}}</span>
                </button>
                <div class="p-0 ms-auto"></div>
                <button type="button" class="btn btn-sm border-0 btn-outline-success px-2" data-ae-actions="DefaultRepo.AAA_Roles.Create" @click="openCreate()">
                    <i class="fa-solid fa-file-alt fa-bounce pe-1" style="--fa-animation-iteration-count:1"></i>
                    <span class="ms-1">{{shared.translate("Create")}}</span>
                </button>
            </div>
        </div>
        <div class="card-body p-0">
            <div class="card h-100 border-light bg-light bg-opacity-75 border-0">
                <div class="card-body rounded rounded-1 border border-3 border-light fs-d8 p-0 bg-transparent scrollable">
                    <table class="table table-sm table-hover w-100 ae-table m-0 bg-transparent">
                        <thead>
                            <tr class="d-none d-md-table-row d-lg-table-row d-xl-table-row">
                                <th class="sticky-top ae-thead-th fb text-primary fw-bold text-center" style="width:85px;">
                                    <i class="fa-solid fa-fw fa-window-restore"></i>
                                </th>
                                <th class="sticky-top ae-thead-th fb text-success">
                                    <div>
                                        Title / Note
                                    </div>
                                </th>
                                <th class="sticky-top ae-thead-th text-center" style="width:125px;">Methods</th>
                                <th class="sticky-top ae-thead-th text-center" style="width:125px;">Attributes</th>
                                <th class="sticky-top ae-thead-th text-center" style="width:75px;">
                                    <div>{{shared.translate("IsBuiltIn")}}</div>
                                </th>
                                <th class="sticky-top ae-thead-th text-center" style="width:100px;">
                                    <div>UsersCount</div>
                                </th>
                                <th class="sticky-top ae-thead-th text-center" style="width:40px;" data-ae-actions="DefaultRepo.AAA_Roles.DeleteByKey"></th>
                            </tr>
                        </thead>
                        <tbody v-if="initialResponses[0].IsSucceeded===true">
                            <tr v-for="i in initialResponses[0]['Result']['Master']">
                                <td class="ae-table-td text-dark text-center" @click="openById({compPath:'/a.Components/AAA_Roles_UpdateByKey',recordKey:i.Id,refereshOnCallback:true,actionsAllowed:'DefaultRepo.AAA_Roles.UpdateByKey',fkToParent:''});">
                                    <div class="text-primary text-hover-success pointer">
                                        <i class="fa-solid fa-fw fa-edit"></i>
                                        <br>
                                        <span class="pk">{{i.Id}}</span>
                                    </div>
                                </td>
                                <td class="ae-table-td">
                                    <div class="fw-bold">{{i["RoleName"]}}</div>
                                    <div class="fs-d7 text-secondary">{{i["Note"]}}</div>
                                </td>
                                <td class="ae-table-td text-center text-success text-hover-primary pointer" @click="openMethodsAccessSettings(i.Id,i.RoleName)">
                                    <i class="fa-solid fa-fw fa-lock-open shadow5"></i>
                                </td>
                                <td class="ae-table-td text-center text-success text-hover-primary pointer" @click="openAttributesAccessSettings(i.Id,i.RoleName)">
                                    <i class="fa-solid fa-fw fa-list shadow5"></i>
                                </td>
                                <td class="ae-table-td text-center">
                                    <span v-html="shared.convertBoolToIconWithOptions(i.IsBuiltIn ,{})"></span>
                                </td>
                                <td class="ae-table-td text-center">
                                    <span>{{i.UsersCount}}</span>
                                </td>
                                <td class="ae-table-td text-center text-secondary text-hover-danger pointer" data-ae-actions="DefaultRepo.AAA_Roles.DeleteByKey" @click="deleteById({pkValue:i.Id})">
                                    <i class="fa-solid fa-fw fa-trash"></i>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
        <div class="card-footer rounded-0 border-0 border-top border-top-1 border-warning-subtle p-0 bg-white">
            <div class="input-group input-group-sm border-0 bg-white">
                <div class="input-group-text border-0 d-none d-md-block d-lg-block d-xl-block fs-d7 pt-2 bg-white">
                    <span class="text-secondary">{{shared.translate("OrderBy")}}</span>
                </div>
                <select class="form-select form-select-sm text-primary border-0 ae-input d-none d-md-block d-lg-block d-xl-block bg-white" style="max-width:135px;" v-model="initialRequests[0].Inputs.ClientQueryJE.OrderClauses[0].Name" @change="loadRecords()">
                    <option v-for="o in orderableColumns" :value="o">{{shared.translate(o)}}</option>
                </select>
                <select class="form-select form-select-sm text-primary border-0 ae-input d-none d-md-block d-lg-block d-xl-block bg-white" style="max-width:125px;" v-model="initialRequests[0].Inputs.ClientQueryJE.OrderClauses[0].OrderDirection" @change="loadRecords()">
                    <option value="ASC">{{shared.translate("Asc")}}</option>
                    <option value="DESC">{{shared.translate("Desc")}}</option>
                </select>
                <span class="input-group-text border-0 fs-d4 text-secondary d-none d-md-block d-lg-block d-xl-block bg-white"></span>
                <div class="input-group-text border-0 d-none d-md-block d-lg-block d-xl-block fs-d7 pt-2 bg-white">
                    <span class="text-secondary">{{shared.translate("PageSize")}}</span>
                </div>
                <select class="form-select form-select-sm text-primary border-0 ae-input d-none d-md-block d-lg-block d-xl-block bg-white" style="max-width:75px;" v-model.number="initialRequests[0].Inputs.ClientQueryJE.Pagination.PageSize" @change="loadRecords()">
                    <option value="10">10</option>
                    <option value="25">25</option>
                    <option value="50">50</option>
                </select>
                <div class="input-group-text border-0 d-none d-md-block d-lg-block d-xl-block bg-white">
                    <div class="pagination"></div>
                </div>
                <input type="text" class="form-control form-control-sm border-0 rounded-0 bg-white d-none d-md-block d-lg-block d-xl-block" disabled="">
                <div class="input-group-text border-0 fs-d7 pt-2 bg-white" v-if="initialResponses[0].IsSucceeded===true">
                    <span class="text-secondary">{{shared.translate("Rows")}}</span>
                    :
                    <span class="text-success fw-bold mx-1">{{initialResponses[0]["Result"]["Aggregations"][0]["Count"]}}</span>
                </div>
                <div class="input-group-text border-0 fs-d7 pt-2 bg-white" v-if="initialResponses[0].IsSucceeded===true">
                    <span class="text-secondary">{{shared.translate("Duration")}}</span>
                    :
                    <span class="text-success fw-bold mx-1">{{initialResponses[0]["Duration"]/1000}}s</span>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    shared.setAppTitle(shared.translate("AAA_Roles, ReadList"));
    let _this = { cid: "", c: null, templateType: "ReadList", dbConfName: "", objectName: "", loadMethod: "", deleteMethod: "", initialRequests: [], initialResponses: [], pickerRequests: [], pickerHumanIds: [], filter: {}, clientQueryMetadata: {}, orderClauses: [], orderableColumns: [] };
    _this.dbConfName = "DefaultRepo";
    _this.objectName = "AAA_Roles";
    _this.loadMethod = "DefaultRepo.AAA_Roles.ReadList";
    _this.filePrefix = "";
    _this.deleteMethod = `${_this.dbConfName}.${_this.objectName}.DeleteByKey`;
    _this.orderableColumns = ["IsBuiltIn", "CreatedOn", "UpdatedOn"];
    _this.orderClauses = [{ Name: "IsBuiltIn", OrderDirection: "DESC" }];
    _this.initialResponses = [{ Duration: 0, Result: { Master: [], Aggregations: [{ "Count": 0 }] } }];
    _this.initialRequests = [genListRequest(_this.loadMethod, {}, _this.orderClauses, { PageNumber: 1, PageSize: 100 })];
    _this.filter = { "Id": null, "CreatedBy": null, "UpdatedBy": null, "IsBuiltIn": null, "RoleName": null, "Note": null };
    _this.initialSearchOptions = _.cloneDeep(_this.filter);
    _this.clientQueryMetadata = { "ParentObjectColumns": [{ "Name": "Id", "DevNote": "", "IsPrimaryKey": true, "DbType": "INT", "IsIdentity": true, "IdentityStart": "100", "IdentityStep": "1", "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "IsDisabled": true, "Required": true, "ValidationRule": ":=i(0,2147483647)" } }, { "Name": "CreatedBy", "DevNote": "", "DbType": "INT", "UpdateGroup": "", "UiProps": { "Group": "Auditing", "UiWidget": "DisabledTextbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "IsDisabled": true, "Required": true, "ValidationRule": ":=i(0,2147483647)" } }, { "Name": "CreatedOn", "DevNote": "", "DbType": "DATETIME", "IsSortable": true, "UpdateGroup": "", "UiProps": { "Group": "Auditing", "UiWidget": "DisabledTextbox", "UiWidgetOptions": "{}", "IsDisabled": true, "Required": true, "ValidationRule": "dt(1900-01-01 00:01:00,2100-12-30 11:59:59)" } }, { "Name": "UpdatedBy", "DevNote": "", "DbType": "INT", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "Auditing", "UiWidget": "DisabledTextbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "IsDisabled": true, "Required": false, "ValidationRule": ":=i(0,2147483647)" } }, { "Name": "UpdatedOn", "DevNote": "", "DbType": "DATETIME", "AllowNull": true, "IsSortable": true, "UpdateGroup": "", "UiProps": { "Group": "Auditing", "UiWidget": "DisabledTextbox", "UiWidgetOptions": "{}", "IsDisabled": true, "Required": false, "ValidationRule": "dt(1900-01-01 00:01:00,2100-12-30 11:59:59)" } }, { "Name": "IsBuiltIn", "DevNote": "", "DbType": "BIT", "AllowNull": true, "DbDefault": "0", "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Checkbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false } }, { "Name": "RoleName", "DevNote": "", "DbType": "NVARCHAR", "Size": "64", "IsHumanId": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": true, "ValidationRule": ":=s(0,64)" } }, { "Name": "Note", "DevNote": "", "DbType": "NVARCHAR", "Size": "256", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "MultilineTextbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false, "ValidationRule": ":=s(0,256)" } }], "Name": "ReadList", "Type": "ReadList", "QueryColumns": ["Id", "CreatedBy", "CreatedOn", "UpdatedBy", "UpdatedOn", "IsBuiltIn", "RoleName", "Note"], "FastSearchColumns": [], "ExpandableSearchColumns": [{ "Name": "Id", "DevNote": "", "IsPrimaryKey": true, "DbType": "INT", "IsIdentity": true, "IdentityStart": "100", "IdentityStep": "1", "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "IsDisabled": true, "Required": true, "ValidationRule": ":=i(0,2147483647)" } }, { "Name": "CreatedBy", "DevNote": "", "DbType": "INT", "UpdateGroup": "", "UiProps": { "Group": "Auditing", "UiWidget": "DisabledTextbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "IsDisabled": true, "Required": true, "ValidationRule": ":=i(0,2147483647)" } }, { "Name": "UpdatedBy", "DevNote": "", "DbType": "INT", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "Auditing", "UiWidget": "DisabledTextbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "IsDisabled": true, "Required": false, "ValidationRule": ":=i(0,2147483647)" } }, { "Name": "IsBuiltIn", "DevNote": "", "DbType": "BIT", "AllowNull": true, "DbDefault": "0", "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Checkbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false } }, { "Name": "RoleName", "DevNote": "", "DbType": "NVARCHAR", "Size": "64", "IsHumanId": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": true, "ValidationRule": ":=s(0,64)" } }, { "Name": "Note", "DevNote": "", "DbType": "NVARCHAR", "Size": "256", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "MultilineTextbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false, "ValidationRule": ":=s(0,256)" } }], "OptionalQueries": [] };

    export default {
        methods: {
            openMethodsAccessSettings(RoleId, RoleName) {
                openComponent("/a.SharedComponents/RolesMethods", {
                    title: "Role Methods Access Settings", modalSize: "modal-fullscreen", params: {
                        RoleId: RoleId,
                        RoleName: RoleName,
                        callback: function (ret) {

                        }
                    }
                });
            },
            openAttributesAccessSettings(RoleId, RoleName) {
                openComponent("/a.Components/AAA_Roles_Attributes", {
                    title: "Role Attributes", modalSize: "modal-fullscreen", params: {
                        RoleId: RoleId,
                        RoleName: RoleName,
                        callback: function (ret) {

                        }
                    }
                });
            }
        },
        setup(props) { _this.cid = props['cid']; },
        data() { return _this; },
        created() { _this.c = this; assignDefaultMethods(_this); },
        mounted() { _this.c.loadRecords(function () { initVueComponent(_this); }); },
        props: { cid: String, ismodal: String }
    }

</script>