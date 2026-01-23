<template>
    <div class="card h-100 bg-transparent rounded-0 border-0" data-did="d-951248-0" draggable="true">
        <div class="card-header p-2 bg-body-subtle rounded-0 border-0" data-did="d-48012-1" draggable="true">
            <div class="container-fluid" data-did="d-168652-2" draggable="true">
                <div class="row" data-did="d-529258-3" draggable="true">
                    <div class="col-48 col-md-4">
                        <select class="form-select form-select-sm" v-model="filter.EntityTypeId">
                            <option value="">{{shared.translate("EntityTypeId")}}</option>
                            <option v-for="i in shared.enum(102)" :value="i['Id']">{{i.Title}}</option>
                        </select>
                    </div>
                    <div class="col-48 col-md-4">
                        <select class="form-select form-select-sm" v-model="filter.RecordStateId">
                            <option value="">{{shared.translate("RecordStateId")}}</option>
                            <option v-for="i in shared.enum(101)" :value="i['Id']">{{i.Title}}</option>
                        </select>
                    </div>
                </div>

                <div class="row mt-1" data-did="d-529258-66" draggable="true">
                    <div class="col-48 col-md-4" data-did="d-905746-17" draggable="true" v-if="shared.fixNull(filter.EntityTypeId,'')==='' || filter.EntityTypeId==='102.1'">
                        <select class="form-select form-select-sm" v-model="filter.GenderId">
                            <option value="">{{shared.translate("Gender")}}</option>
                            <option v-for="i in shared.enum(100)" :value="i['Id']">{{i.Title}}</option>
                        </select>
                    </div>
                    <div class="col-48 col-md-4" data-did="d-480784-4" draggable="true" v-if="shared.fixNull(filter.EntityTypeId,'')==='' || filter.EntityTypeId==='102.1'">
                        <input type="text" class="form-control form-control-sm" id="input_FirstName" @keyup.enter="loadRecords()" v-model="filter.FirstName" :placeholder="shared.translate('FirstName')" data-did="d-728352-5" draggable="true">
                    </div>
                    <div class="col-48 col-md-4" data-did="d-661845-6" draggable="true" v-if="shared.fixNull(filter.EntityTypeId,'')==='' || filter.EntityTypeId==='102.1'">
                        <input type="text" class="form-control form-control-sm" id="input_LastName" @keyup.enter="loadRecords()" v-model="filter.LastName" :placeholder="shared.translate('LastName')" data-did="d-254218-7" draggable="true">
                    </div>
                    <div class="col-48 col-md-4" data-did="d-661845-64" draggable="true" v-if="shared.fixNull(filter.EntityTypeId,'')==='' || filter.EntityTypeId==='102.2'">
                        <input type="text" class="form-control form-control-sm" id="input_Title" @keyup.enter="loadRecords()" v-model="filter.Title" :placeholder="shared.translate('Title')" data-did="d-254218-7" draggable="true">
                    </div>
                    <div class="col-48 col-md-4" data-did="d-973520-27" draggable="true">
                        <input type="text" class="form-control form-control-sm text-center ltr" id="input_Mobile" @keyup.enter="loadRecords()" v-model="filter.Mobile" :placeholder="shared.translate('Mobile')" data-did="d-781382-28" draggable="true">
                    </div>
                    <div class="col-48 col-md-4" data-did="d-973520-273" draggable="true">
                        <input type="text" class="form-control form-control-sm text-center ltr" id="input_LandPhone" @keyup.enter="loadRecords()" v-model="filter.LandPhone" :placeholder="shared.translate('LandPhone')" data-did="d-781382-228" draggable="true">
                    </div>
                </div>

            </div>
        </div>
        <div class="card-header simple-search p-2 px-0 bg-transparent rounded-0 border-0 collapse" data-did="d-706224-8" draggable="true">
            <div class="container-fluid" data-did="d-88976-9" draggable="true">
                <div class="row" data-did="d-152222-10" draggable="true">
                    <div class="col-48 col-md-4" data-did="d-624846-21" draggable="true">
                        <input type="text" class="form-control form-control-sm" id="input_BirthYear" @keyup.enter="loadRecords()" v-model="filter.BirthYear" 
                               :placeholder="shared.translate('BirthYear')+' / '+shared.translate('RegistrationYear')" data-did="d-818180-22" draggable="true">
                    </div>
                    <div class="col-48 col-md-4" data-did="d-600232-23" draggable="true">
                        <input type="text" class="form-control form-control-sm" id="input_BirthMonth" @keyup.enter="loadRecords()" v-model="filter.BirthMonth" 
                               :placeholder="shared.translate('BirthMonth')+' / '+shared.translate('RegistrationMonth')" data-did="d-538820-24" draggable="true">
                    </div>
                    <div class="col-48 col-md-4" data-did="d-52466-25" draggable="true">
                        <input type="text" class="form-control form-control-sm" id="input_BirthDay" @keyup.enter="loadRecords()" v-model="filter.BirthDay" 
                               :placeholder="shared.translate('BirthDay')+' / '+shared.translate('RegistrationDay')" data-did="d-478443-26" draggable="true">
                    </div>                    
                </div>
                <div class="row mt-1" data-did="d-152222-10" draggable="true">
                    <div class="col-48 col-md-4" data-did="d-681139-19" draggable="true">
                        <input type="text" class="form-control form-control-sm" id="input_NationalCode" @keyup.enter="loadRecords()" v-model="filter.NationalCode" 
                               :placeholder="shared.translate('NationalCode')+' / '+shared.translate('RegistrationNumber')" data-did="d-483709-20" draggable="true">
                    </div>
                    <div class="col-48 col-md-4" data-did="d-265047-29" draggable="true">
                        <input type="text" class="form-control form-control-sm" id="input_Id" @keyup.enter="loadRecords()" v-model="filter.Id" :placeholder="shared.translate('Id')" data-did="d-924380-30" draggable="true">
                    </div>
                </div>
            </div>
        </div>
        <div class="card-header p-2 px-3 rounded-0 border-0" data-did="d-895662-33" draggable="true">
            <div class="hstack gap-1" data-did="d-334347-34" draggable="true">
                <button class="btn btn-sm btn-outline-primary px-3" @click="loadRecords()" data-did="d-730482-35" draggable="true">
                    <i class="fa-solid fa-search me-1" data-did="d-47490-36" draggable="true">
                    </i>
                    <span data-did="d-574556-37" draggable="true">{{shared.translate("Search")}}</span>
                </button>
                <button class="btn btn-sm btn-outline-secondary px-3" @click="resetSearchOptions" data-did="d-250781-38" draggable="true">
                    <i class="fa-solid fa-eraser me-1" data-did="d-42023-39" draggable="true">
                    </i>
                    <span data-did="d-939170-40" draggable="true">{{shared.translate("Reset")}}</span>
                </button>
                <button type="button" class="btn btn-sm bg-hover-light px-3" onclick="switchVisibility(this,'.simple-search','show','fa-chevron-down','fa-chevron-up')" data-did="d-328988-41" draggable="true">
                    <i class="fa-solid fa-chevron-down me-1" data-did="d-106286-42" draggable="true">
                    </i>
                </button>
                <div class="p-0 ms-auto" data-did="d-452187-43" draggable="true">
                </div>
                <button type="button" class="btn btn-sm border-0 btn-outline-success px-2" data-ae-actions="DefaultRepo.BasePersons.Create" @click="openCreate({dialog:{modalSize:'modal-lg'}})" data-did="d-256899-44" draggable="true">
                    <i class="fa-solid fa-file-alt fa-bounce" style="--fa-animation-iteration-count:1" data-did="d-147615-45" draggable="true">
                    </i>
                    <span data-did="d-859962-46" draggable="true">{{shared.translate("Create")}}</span>
                </button>
                <div class="vr" data-did="d-321688-47" draggable="true">
                </div>
                <div class="btn btn-sm border-0 btn-outline-success px-2" data-ae-actions="DefaultRepo.BasePersons.ReadList" @click="exportExcel" data-did="d-549668-48" draggable="true">
                    <i class="fa-solid fa-file-excel" data-did="d-448067-49" draggable="true">
                    </i>
                    <span data-did="d-685050-50" draggable="true">{{shared.translate("Export")}}</span>
                </div>
            </div>
        </div>
        <div class="card-body p-0 border-0" data-did="d-473483-51" draggable="true">
            <div class="card h-100 border-light bg-light bg-opacity-75 border-0" data-did="d-869535-52" draggable="true">
                <div class="card-body border-0 p-0 scrollable" data-did="d-640070-53" draggable="true">
                    <table class="table table-sm table-hover w-100 ae-table m-0 bg-transparent fs-d8" data-did="d-112625-54" draggable="true">
                        <thead data-did="d-217956-55" draggable="true">
                            <tr class="d-none d-md-table-row d-lg-table-row d-xl-table-row" data-did="d-427715-56" draggable="true">
                                <th class="sticky-top ae-thead-th fw-bold text-primary fw-bold text-center" style="width:95px;overflow: hidden;text-overflow: ellipsis;" data-did="d-699535-57" draggable="true">
                                    <i class="fa-solid fa-fw fa-window-restore" data-did="d-251477-58" draggable="true">
                                    </i>
                                </th>
                                <th class="sticky-top ae-thead-th text-center" style="width:95px;overflow: hidden;text-overflow: ellipsis;" data-did="d-812927-59" draggable="true">
                                    <div data-did="d-604346-60" draggable="true">{{shared.translate("Picture")}}</div>
                                </th>
                                <th class="sticky-top ae-thead-th fw-bold text-success" style="width:185px;" data-did="d-899321-61" draggable="true">
                                    <div data-did="d-239016-62" draggable="true">{{shared.translate("Title")}}</div>
                                </th>
                                <th class="sticky-top ae-thead-th text-center" style="width:150px;overflow: hidden;text-overflow: ellipsis;" data-did="d-160400-69" draggable="true">
                                    <div data-did="d-548653-70" draggable="true">{{shared.translate("Birth")}} / {{shared.translate("Registration")}}</div>
                                </th>
                                <th class="sticky-top ae-thead-th text-center" style="width:185px;" data-did="d-339831-75" draggable="true">
                                    <div data-did="d-709581-76" draggable="true">{{shared.translate("Phone")}}</div>
                                </th>

                                <th class="sticky-top ae-thead-th" style="overflow: hidden;text-overflow: ellipsis;" data-did="d-530529-63" draggable="true">
                                    <div data-did="d-911187-64" draggable="true">{{shared.translate("Membership")}}</div>
                                </th>

                                <th class="sticky-top ae-thead-th text-center" style="width:125px;">
                                    <div>{{shared.translate("EntityTypeId")}}</div>
                                </th>

                                <th class="sticky-top ae-thead-th text-center" style="width:125px;">
                                    <div>{{shared.translate("RecordStateId")}}</div>
                                </th>

                                <th style="width:40px;" class="sticky-top ae-thead-th text-center" data-ae-actions="DefaultRepo.BasePersons.DeleteByKey" data-did="d-797504-78" draggable="true">
                                </th>
                            </tr>
                        </thead>
                        <tbody v-if="initialResponses[0].IsSucceeded===true" data-did="d-773138-79" draggable="true">
                            <tr v-for="i in initialResponses[0]['Result']['Master']" data-did="d-678995-80" draggable="true">
                                <td class="ae-table-td text-dark text-center" @click="openById({compPath:'/a.Components/BasePersons_UpdateByKey',recordKey:i.Id,refereshOnCallback:true,actionsAllowed:'DefaultRepo.BasePersons.UpdateByKey',fkToParent:'',dialog:{modalSize:'modal-lg'}});" data-did="d-262060-81" draggable="true">
                                    <div class="text-primary text-hover-success pointer" data-did="d-222766-82" draggable="true">
                                        <i class="fa-solid fa-fw fa-edit" data-did="d-499876-83" draggable="true"></i>
                                        <div class="pk font-monospace" data-did="d-830810-85" draggable="true">{{i.Id}}</div>
                                    </div>
                                </td>
                                <td class="ae-table-td text-center" data-did="d-57255-86" draggable="true">
                                    <img :src="'data:image/png;base64, '+i.Picture_FileBody_xs" v-if="shared.fixNull(i.Picture_FileBody_xs,'')!==''"
                                         class="rounded-4 shadow-sm my-2" style="width:95%;min-height:45px;max-height:45px;max-width:45px;" data-did="d-723649-87" draggable="true">
                                    <i class="fa-solid fa-fw fa-image fa-3x text-light rounded" v-else="" data-did="d-819491-88" draggable="true"></i>
                                </td>
                                <td class="ae-table-td fw-bold" data-did="d-233665-89" draggable="true">
                                    <table class="w-100">
                                        <tr>
                                            <td style="width:35px;vertical-align:middle;" class="text-center">
                                                <i :class="i.GenderId_UiIcon+' fa-2x'"></i>
                                            </td>
                                            <td>
                                                <div data-did="d-999031-90" draggable="true" v-if="i.EntityTypeId==='102.1'">
                                                    <span data-did="d-880092-93" draggable="true">{{shared.fixNull(i["FirstName"],'')}}</span>
                                                    <span class="fs-d8">&nbsp;</span>
                                                    <span data-did="d-294450-97" draggable="true">{{shared.fixNull(i["LastName"],'')}}</span>
                                                </div>
                                                <div data-did="d-999031-90" draggable="true" v-if="i.EntityTypeId==='102.2'">
                                                    <span data-did="d-294450-397" draggable="true">{{shared.fixNull(i["Title"],'')}}</span>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td class="ae-table-td text-center" >
                                    <div style="direction:ltr !important;">{{i["NationalCode"]}}</div>
                                    <div style="direction:ltr !important;" class="text-secondary fs-d8">
                                        {{shared.format2Char(i["BirthYear"])}}-{{shared.format2Char(i["BirthMonth"])}}-{{shared.format2Char(i["BirthDay"])}}
                                    </div>
                                </td>

                                <td class="ae-table-td text-center" data-did="d-450199-110" draggable="true">
                                    <div data-did="d-603301-111" draggable="true">{{i["Mobile"]}}</div>
                                    <div data-did="d-603301-121" draggable="true">{{i["LandPhone"]}}</div>
                                </td>

                                <td class="ae-table-td" data-did="d-305003-98" draggable="true">
                                    <div class="btn btn-sm btn-link text-start py-1" style="min-width:100px;">
                                        <div v-if="shared.fixNull(i['UserId'],'')===''" @click="createMembership(i)">
                                            {{shared.translate("CreateMembership")}}
                                        </div>
                                        <div v-else @click="openById({compPath:'/a.Components/BaseUsers_UpdateByKey',recordKey:i.UserId,refereshOnCallback:true,actionsAllowed:'DefaultRepo.BaseUsers.UpdateByKey',fkToParent:'',dialog:{modalSize:'modal-md'}});">
                                            {{i.UserId_UserName}}
                                        </div>
                                    </div>
                                </td>

                                <td class="ae-table-td text-center">
                                    <div>{{i.EntityTypeId_Title}}</div>
                                </td>

                                <td class="ae-table-td text-center bg-hover-light pointer" @click="openById({compPath:'/a.Components/BasePersons_RecordStateIdUpdate',recordKey:i.Id,refereshOnCallback:true,actionsAllowed:'DefaultRepo.BasePersons.BasePersons_RecordStateIdUpdate',fkToParent:''});">
                                    <div v-if="shared.fixNull(i['RecordStateId_Title'],'')!==''">
                                        <div class="fw-bold">
                                            <i :class="i.RecordStateId_UiIcon"></i>
                                            <span :class="i.RecordStateId_UiClass">{{i["RecordStateId_Title"]}}</span>
                                        </div>
                                        <div class="text-secondary fs-d8">{{i["RecordStateIdUpdatedBy_UserName"]}} ({{shared.formatDateL(i["RecordStateIdUpdatedOn"])}})</div>
                                    </div>
                                    <div v-else>
                                        -
                                    </div>
                                </td>

                                <td style="width:40px;vertical-align:middle" class="text-center text-secondary text-hover-danger pointer" data-ae-actions="DefaultRepo.BasePersons.DeleteByKey" @click="deleteById({pkValue:i.Id})" data-did="d-187813-113" draggable="true">
                                    <i class="fa-solid fa-fw fa-trash" data-did="d-658149-114" draggable="true"></i>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
        <div class="card-footer ae-list-footer" data-did="d-55297-115" draggable="true">
            <div class="d-flex flex-wrap align-items-center justify-content-center justify-content-md-between gap-3" data-did="d-956731-116" draggable="true">
                <!-- Controls Group -->
                <div class="d-none d-md-flex align-items-center gap-3 flex-wrap" data-did="d-796473-117" draggable="true">
                    <!-- Order By -->
                    <div class="d-flex align-items-center gap-2" data-did="d-634758-118" draggable="true">
                        <span class="text-secondary small" data-did="d-490005-119" draggable="true">{{shared.translate("OrderBy")}}</span>
                        <div class="d-flex gap-1" data-did="d-335763-120" draggable="true">
                            <select class="form-select form-select-sm border-0 bg-transparent text-secondary fw-medium" style="min-width: 100px;" v-model="initialRequests[0].Inputs.ClientQueryJE.OrderClauses[0].Name" @change="loadRecords()" data-did="d-227412-121" draggable="true">
                                <option v-for="o in orderableColumns" :value="o" data-did="d-595009-122" draggable="true">{{shared.translate(o)}}</option>
                            </select>
                            <select class="form-select form-select-sm border-0 bg-transparent text-secondary fw-medium" style="width: 80px;" v-model="initialRequests[0].Inputs.ClientQueryJE.OrderClauses[0].OrderDirection" @change="loadRecords()" data-did="d-194112-123" draggable="true">
                                <option value="ASC" data-did="d-116836-124" draggable="true">{{shared.translate("Asc")}}</option>
                                <option value="DESC" data-did="d-786109-125" draggable="true">{{shared.translate("Desc")}}</option>
                            </select>
                        </div>
                    </div>
                    <div class="vr text-secondary opacity-25 d-none d-md-block" data-did="d-669171-126" draggable="true">
                    </div>
                    <!-- Page Size -->
                    <div class="d-flex align-items-center gap-2" data-did="d-863926-127" draggable="true">
                        <span class="text-secondary small" data-did="d-595693-128" draggable="true">{{shared.translate("PageSize")}}</span>
                        <select class="form-select form-select-sm border-0 bg-transparent text-secondary fw-medium" style="width: 70px;" v-model.number="initialRequests[0].Inputs.ClientQueryJE.Pagination.PageSize" @change="loadRecords()" data-did="d-813422-129" draggable="true">
                            <option value="10" data-did="d-656337-130" draggable="true">10</option>
                            <option value="25" data-did="d-664165-131" draggable="true">25</option>
                            <option value="50" data-did="d-423287-132" draggable="true">50</option>
                        </select>
                    </div>
                </div>
                <!-- Pagination -->
                <div class="d-flex align-items-center" data-did="d-880682-133" draggable="true">
                    <div class="pagination ae-pagination m-0" data-did="d-508799-134" draggable="true">
                    </div>
                </div>
                <!-- Stats -->
                <div class="d-none d-md-flex align-items-center gap-3 text-secondary small" v-if="initialResponses[0].IsSucceeded===true" data-did="d-171178-135" draggable="true">
                    <div data-did="d-109956-136" draggable="true">
                        <span data-did="d-112957-137" draggable="true">{{shared.translate("Rows")}}:</span>
                        <span class="fw-bold text-primary ms-1" data-did="d-940730-138" draggable="true">{{initialResponses[0]["Result"]["Aggregations"][0]["Count"]}}</span>
                    </div>
                    <div class="vr opacity-25" data-did="d-599004-139" draggable="true">
                    </div>
                    <div data-did="d-470956-140" draggable="true">
                        <span data-did="d-326851-141" draggable="true">{{shared.translate("Duration")}}:</span>
                        <span class="fw-bold text-primary ms-1" data-did="d-90665-142" draggable="true">{{initialResponses[0]["Duration"]/1000}}s</span>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    shared.setAppTitle("$auto$");
    let _this = { cid: "", c: null, templateType: "ReadList", filePrefix: "", dbConfName: "", objectName: "", loadMethod: "", deleteMethod: "", initialRequests: [], initialResponses: [], pickerRequests: [], pickerHumanIds: [], filter: {}, clientQueryMetadata: {}, orderClauses: [], orderableColumns: [] };
    _this.dbConfName = "DefaultRepo";
    _this.objectName = "BasePersons";
    _this.loadMethod = "DefaultRepo.BasePersons.ReadList";
    _this.filePrefix = "";
    _this.deleteMethod = `${_this.dbConfName}.${_this.objectName}.DeleteByKey`;
    _this.orderableColumns = ["CreatedOn", "UpdatedOn"];
    _this.orderClauses = [{ Name: "CreatedOn", OrderDirection: "ASC" }];
    _this.initialResponses = [{ Duration: 0, Result: { Master: [], Aggregations: [{ "Count": 0 }] } }];
    _this.initialRequests = [genListRequest(_this.loadMethod, {}, _this.orderClauses, { PageNumber: 1, PageSize: 10 })];
    _this.filter = { "FirstName": null, "LastName": null, "Title": null, "EntityTypeId": '', "RecordStateId": '', "Id": null, "CreatedBy": null, "UpdatedBy": null, "UserId": null, "GenderId": '', "NationalCode": null, "BirthYear": null, "BirthMonth": null, "BirthDay": null, "Mobile": null, "Picture_FileName": null, "Picture_FileSize": null, "Picture_FileMime": null };
    _this.initialSearchOptions = _.cloneDeep(_this.filter);
    _this.clientQueryMetadata = {
        "ParentObjectColumns": [
            { "Name": "Id", "DevNote": "", "IsPrimaryKey": true, "DbType": "INT", "IsIdentity": true, "IdentityStart": "1000000", "IdentityStep": "1", "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "IsDisabled": true, "Required": true, "ValidationRule": ":=i(0,2147483647)" } },
            { "Name": "CreatedBy", "DevNote": "", "DbType": "INT", "UpdateGroup": "", "UiProps": { "Group": "Auditing", "UiWidget": "DisabledTextbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "IsDisabled": true, "Required": true, "ValidationRule": ":=i(0,2147483647)" } },
            { "Name": "CreatedOn", "DevNote": "", "DbType": "DATETIME", "IsSortable": true, "UpdateGroup": "", "UiProps": { "Group": "Auditing", "UiWidget": "DisabledTextbox", "UiWidgetOptions": "{}", "IsDisabled": true, "Required": true, "ValidationRule": "dt(1900-01-01 00:01:00,2100-12-30 11:59:59)" } },
            { "Name": "UpdatedBy", "DevNote": "", "DbType": "INT", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "Auditing", "UiWidget": "DisabledTextbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "IsDisabled": true, "Required": false, "ValidationRule": ":=i(0,2147483647)" } },
            { "Name": "UpdatedOn", "DevNote": "", "DbType": "DATETIME", "AllowNull": true, "IsSortable": true, "UpdateGroup": "", "UiProps": { "Group": "Auditing", "UiWidget": "DisabledTextbox", "UiWidgetOptions": "{}", "IsDisabled": true, "Required": false, "ValidationRule": "dt(1900-01-01 00:01:00,2100-12-30 11:59:59)" } },
            { "Name": "UserId", "DevNote": "", "DbType": "INT", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false, "ValidationRule": ":=i(0,2147483647)" } },
            { "Name": "GenderId", "DevNote": "", "DbType": "INT", "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": true, "ValidationRule": ":=i(0,2147483647)" } },
            { "Name": "EntityTypeId", "DevNote": "", "DbType": "INT", "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": true, "ValidationRule": ":=i(0,2147483647)" } },
            { "Name": "RecordStateId", "DevNote": "", "DbType": "INT", "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": true, "ValidationRule": ":=i(0,2147483647)" } },
            { "Name": "NationalCode", "DevNote": "", "DbType": "VARCHAR", "Size": "16", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false, "ValidationRule": ":=s(0,16)" } },
            { "Name": "Title", "DevNote": "", "DbType": "NVARCHAR", "Size": "64", "IsHumanId": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Fast", "Required": true, "ValidationRule": ":=s(0,64)" } },
            { "Name": "FirstName", "DevNote": "", "DbType": "NVARCHAR", "Size": "64", "IsHumanId": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Fast", "Required": true, "ValidationRule": ":=s(0,64)" } },
            { "Name": "LastName", "DevNote": "", "DbType": "NVARCHAR", "Size": "64", "IsHumanId": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Fast", "Required": true, "ValidationRule": ":=s(0,64)" } },
            { "Name": "BirthYear", "DevNote": "", "DbType": "INT", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false, "ValidationRule": ":=i(0,2147483647)" } },
            { "Name": "BirthMonth", "DevNote": "", "DbType": "TINYINT", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false } },
            { "Name": "BirthDay", "DevNote": "", "DbType": "TINYINT", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false } },
            { "Name": "Mobile", "DevNote": "", "DbType": "VARCHAR", "Size": "16", "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": true, "ValidationRule": ":=s(0,14)" } },
            { "Name": "LandPhone", "DevNote": "", "DbType": "VARCHAR", "Size": "16", "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": true, "ValidationRule": ":=s(0,14)" } },
            { "Name": "Picture_FileBody", "DevNote": "", "DbType": "IMAGE", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "ImageView", "UiWidgetOptions": "{}", "Required": false } },
            { "Name": "Picture_FileBody_xs", "DevNote": "", "DbType": "IMAGE", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "ImageView", "UiWidgetOptions": "{}", "Required": false } },
            { "Name": "Picture_FileName", "DevNote": "", "DbType": "NVARCHAR", "Size": "128", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false, "ValidationRule": ":=s(0,128)" } },
            { "Name": "Picture_FileSize", "DevNote": "", "DbType": "INT", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false, "ValidationRule": ":=i(0,2147483647)" } },
            { "Name": "Picture_FileMime", "DevNote": "", "DbType": "VARCHAR", "Size": "128", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false, "ValidationRule": ":=s(0,128)" } }
        ],
        "Name": "ReadList", "Type": "ReadList", "QueryColumns": ["Id", "CreatedBy", "CreatedOn", "UpdatedBy", "UpdatedOn", "UserId", "GenderId", "NationalCode", "FirstName", "LastName", "BirthYear", "BirthMonth", "BirthDay", "Mobile", "Picture_FileBody_xs"], "FastSearchColumns": [{ "Name": "FirstName", "DevNote": "", "DbType": "NVARCHAR", "Size": "64", "IsHumanId": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Fast", "Required": true, "ValidationRule": ":=s(0,64)" } }, { "Name": "LastName", "DevNote": "", "DbType": "NVARCHAR", "Size": "64", "IsHumanId": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Fast", "Required": true, "ValidationRule": ":=s(0,64)" } }], "ExpandableSearchColumns": [{ "Name": "Id", "DevNote": "", "IsPrimaryKey": true, "DbType": "INT", "IsIdentity": true, "IdentityStart": "1000000", "IdentityStep": "1", "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "IsDisabled": true, "Required": true, "ValidationRule": ":=i(0,2147483647)" } }, { "Name": "CreatedBy", "DevNote": "", "DbType": "INT", "UpdateGroup": "", "UiProps": { "Group": "Auditing", "UiWidget": "DisabledTextbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "IsDisabled": true, "Required": true, "ValidationRule": ":=i(0,2147483647)" } }, { "Name": "UpdatedBy", "DevNote": "", "DbType": "INT", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "Auditing", "UiWidget": "DisabledTextbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "IsDisabled": true, "Required": false, "ValidationRule": ":=i(0,2147483647)" } }, { "Name": "UserId", "DevNote": "", "DbType": "INT", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false, "ValidationRule": ":=i(0,2147483647)" } }, { "Name": "GenderId", "DevNote": "", "DbType": "INT", "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": true, "ValidationRule": ":=i(0,2147483647)" } }, { "Name": "NationalCode", "DevNote": "", "DbType": "VARCHAR", "Size": "16", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false, "ValidationRule": ":=s(0,16)" } }, { "Name": "BirthYear", "DevNote": "", "DbType": "INT", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false, "ValidationRule": ":=i(0,2147483647)" } }, { "Name": "BirthMonth", "DevNote": "", "DbType": "TINYINT", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false } }, { "Name": "BirthDay", "DevNote": "", "DbType": "TINYINT", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false } }, { "Name": "Mobile", "DevNote": "", "DbType": "VARCHAR", "Size": "14", "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": true, "ValidationRule": ":=s(0,14)" } }, { "Name": "Picture_FileName", "DevNote": "", "DbType": "NVARCHAR", "Size": "128", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false, "ValidationRule": ":=s(0,128)" } }, { "Name": "Picture_FileSize", "DevNote": "", "DbType": "INT", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false, "ValidationRule": ":=i(0,2147483647)" } }, { "Name": "Picture_FileMime", "DevNote": "", "DbType": "VARCHAR", "Size": "128", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false, "ValidationRule": ":=s(0,128)" } }], "OptionalQueries": []
    };


    export default {
        methods: {
            createMembership(member) {
                openComponent("/a.Components/BaseUsers_Create", {
                    title: shared.translate("CreateMembership"), modalSize: "modal-md",
                    params: {
                        Mobile: member.Mobile, MemberId: member.Id,
                        callback: function () {
                            _this.c.loadRecords();
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