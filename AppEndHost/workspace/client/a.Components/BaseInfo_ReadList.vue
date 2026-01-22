<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-header p-2 bg-body-subtle rounded-0 border-0">
            <div class="container-fluid">
                <div class="row">
                    <div class="col-48 col-md-6">
                        <div class="form-control form-control-sm py-0 data-ae-validation">
                            <div class="input-group input-group-sm p-0 pt-1" data-ae-widget="objectPicker">
                                <input type="hidden" v-model="filter.ParentId">
                                <input type="hidden" v-model="filter.ParentId_Title">
                                <input type="text" class="form-control bg-transparent p-0 m-0 border-0" :value="shared.fixNull(filter.ParentId+' '+filter.ParentId_Title,'',true)" :placeholder="shared.translate('ParentId')" disabled="">
                                <span></span>
                                <span class="mx-1 text-hover-danger ae-objectpicker-search pointer" @click="openPicker({colName:'ParentId'})">
                                    <i class="fa-solid fa-hand-pointer"></i>
                                </span>
                                <span class="mx-1 text-hover-danger ae-objectpicker-clear pointer">
                                    <i class="fa-solid fa-times"></i>
                                </span>
                            </div>
                        </div>
                    </div>
                    <div class="col-48 col-md-6">
                        <div class="form-control form-control-sm text-nowrap pointer data-ae-validation" data-ae-widget="nullableCheckbox" data-ae-widget-options="{&quot;shownull&quot;:true}">
                            <i class="fa-solid fa-fw me-1"></i>
                            <span>{{shared.translate('IsActive')}}</span>
                            <input type="hidden" v-model="filter.IsActive" data-ae-validation-required="false">
                        </div>
                    </div>
                    <div class="col-48 col-md-6">
                        <input type="text" class="form-control form-control-sm" id="input_Title" @keyup.enter="loadRecords()" v-model="filter.Title" :placeholder="shared.translate('Title')">
                    </div>
                </div>
            </div>

        </div>
        <div class="card-header simple-search p-2 px-0 bg-transparent rounded-0 collapse border-0">
            <div class="container-fluid">
                <div class="row">
                    <div class="col-48 col-md-6">
                        <input type="text" class="form-control form-control-sm" id="input_ShortName" @keyup.enter="loadRecords()" v-model="filter.ShortName" :placeholder="shared.translate('ShortName')">
                    </div>
                    <div class="col-48 col-md-6">
                        <input type="text" class="form-control form-control-sm" id="input_Note" @keyup.enter="loadRecords()" v-model="filter.Note" :placeholder="shared.translate('Note')">
                    </div>
                    <div class="col-48 col-md-6">
                        <input type="text" class="form-control form-control-sm" id="input_Value" @keyup.enter="loadRecords()" v-model="filter.Value" :placeholder="shared.translate('Value')">
                    </div>
                    <div class="col-48 col-md-6">
                        <input type="text" class="form-control form-control-sm" id="input_Id" @keyup.enter="loadRecords()" v-model="filter.Id" :placeholder="shared.translate('Id')">
                    </div>
                </div>
            </div>
        </div>
        <div class="card-header p-2 px-3 rounded-0 border-0">
            <div class="hstack">
                <button class="btn btn-sm btn-outline-primary px-3" @click="loadRecords()">
                    <i class="fa-solid fa-search me-1"></i>
                    <span>{{shared.translate("Search")}}</span>
                </button>
                <button class="btn btn-sm btn-outline-secondary px-3" @click="resetSearchOptions">
                    <i class="fa-solid fa-eraser me-1"></i>
                    <span>{{shared.translate("Reset")}}</span>
                </button>
                <button type="button" class="btn btn-sm bg-hover-light px-3" onclick="switchVisibility(this,'.simple-search','show','fa-chevron-down','fa-chevron-up')">
                    <i class="fa-solid fa-chevron-down me-1"></i>
                </button>
                <div class="p-0 ms-auto"></div>
                <button type="button" class="btn btn-sm border-0 btn-outline-success px-2" data-ae-actions="DefaultRepo.BaseInfo.Create" @click="openCreate({dialog:{modalSize:'modal-lg'}})">
                    <i class="fa-solid fa-file-alt fa-bounce pe-1" style="--fa-animation-iteration-count:1"></i>
                    <span class="ms-1">{{shared.translate("Create")}}</span>
                </button>
                <div class="vr"></div>
                <div class="btn btn-sm border-0 btn-outline-success px-2" data-ae-actions="DefaultRepo.BaseInfo.ReadList" @click="exportExcel">
                    <i class="fa-solid fa-file-excel pe-1"></i>
                    <span class="ms-1">{{shared.translate("Export")}}</span>
                </div>
            </div>
        </div>
        <div class="card-body p-0 border-0">
            <div class="card h-100 border-light bg-light bg-opacity-75 border-0">
                <div class="card-body border-0 p-0 scrollable">
                    <table class="table table-sm table-hover w-100 ae-table m-0 bg-transparent fs-d8">
                        <thead>
                            <tr class="d-none d-md-table-row d-lg-table-row d-xl-table-row">
                                <th class="sticky-top ae-thead-th fb text-primary fw-bold text-center" style="width:125px;overflow: hidden;text-overflow: ellipsis;">
                                    <i class="fa-solid fa-fw fa-window-restore"></i>
                                </th>
                                <th class="sticky-top ae-thead-th fb text-success" style="width:450px;">
                                    <div>{{shared.translate("Title")}}</div>
                                </th>
                                <th class="sticky-top ae-thead-th text-center" style="width:95px;overflow: hidden;text-overflow: ellipsis;">
                                    <div>{{shared.translate("ParentId")}}</div>
                                </th>
                                <th class="sticky-top ae-thead-th text-center" style="width:95px;overflow: hidden;text-overflow: ellipsis;">
                                    <div>{{shared.translate("ViewOrder")}}</div>
                                </th>
                                <th class="sticky-top ae-thead-th text-center" style="width:95px;overflow: hidden;text-overflow: ellipsis;">
                                    <div>{{shared.translate("Value")}}</div>
                                </th>
                                <th class="sticky-top ae-thead-th " style="width:200px;">
                                    <div>{{shared.translate("IsActive")}}</div>
                                </th>

                                <th class="sticky-top ae-thead-th"></th>
                        
                                <th style="width:40px;" class="sticky-top ae-thead-th text-center" data-ae-actions="DefaultRepo.BaseInfo.DeleteByKey"></th>
                            </tr>
                        </thead>
                        <tbody v-if="initialResponses[0].IsSucceeded===true">
                            <tr v-for="i in initialResponses[0]['Result']['Master']">
                                <td class="ae-table-td text-dark text-center" style="" @click="openById({compPath:'/a.Components/BaseInfo_UpdateByKey',recordKey:i.Id,refereshOnCallback:true,actionsAllowed:'DefaultRepo.BaseInfo.UpdateByKey',fkToParent:'ParentId',dialog:{modalSize:'modal-lg'}});">
                                    <div class="text-primary text-hover-success pointer">
                                        <i class="fa-solid fa-fw fa-edit"></i>
                                        <div class="pk font-monospace" data-did="d-830810-85" draggable="true">{{i.Id}}</div>
                                    </div>
                                </td>
                                <td class="ae-table-td" style="">
                                    <div>
                                        <span v-html="dotsToSpaces(i)" class="fs-d8"></span>
                                        <i class="fa-solid fa-fw fa-sm fa-search text-secondary text-hover-success pointer" title="Search as Parent" @click="searchAsParent(i)"></i>
                                        <span class="fw-bold">{{i["Title"]}}</span>
                                    </div>
                                </td>
                                <td class="ae-table-td text-center" style="">
                                    <div class="text-dark fb">
                                        <div>{{shared.translate(i["ParentId_Title"])}}</div>
                                    </div>
                                    <div class="text-muted fs-d8 font-monospace">{{i["ParentId"]}}</div>
                                </td>
                                <td class="ae-table-td text-center" style="">
                                    <div>{{i["ViewOrder"]}}</div>
                                </td>
                                <td class="ae-table-td text-center" style="">
                                    <div>{{i["Value"]}}</div>
                                </td>
                                <td class="ae-table-td pointer" style="" @click="openById({compPath:'/a.Components/BaseInfo_IsActiveUpdate',recordKey:i.Id,refereshOnCallback:true,actionsAllowed:'DefaultRepo.BaseInfo.IsActiveUpdate',fkToParent:'ParentId'});">
                                    <div class="input-group input-group-sm bg-hover-primary rounded-2 p-2">
                                        <div class="input-group-text rounded-2 me-1">
                                            <span v-html="shared.convertBoolToIconWithOptions(i.IsActive ,{})"></span>
                                        </div>
                                        <div class="more-info" style="">
                                            <table class="w-100 h-100 fs-d8">
                                                <tbody>
                                                    <tr>
                                                        <td class="text-muted align-middle" style="min-width:65px;">{{shared.translate("On")}}</td>
                                                        <td class="text-dark fb align-middle">
                                                            <span class="fw-bold">{{shared.fixNullOrEmpty(shared.formatDateL(i["UpdatedOn"]),'-')}}</span>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="text-muted align-middle" style="min-width:65px;">{{shared.translate("By")}}</td>
                                                        <td class="text-dark fb align-middle">
                                                            <span class="fw-bold font-monospace">{{shared.fixNull(i["UpdatedBy"],'-')}}</span>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </div>
                                    </div>
                                </td>

                                <td class="ae-table-td pointer" style="">
                                    <div class="btn btn-sm btn-link text-decoration-none" @click="openById({compPath:'/a.Components/BaseInfo_MetaInfoUpdate',recordKey:i.Id,refereshOnCallback:true,actionsAllowed:'DefaultRepo.BaseInfo.MetaInfoUpdate',fkToParent:'ParentId'});">
                                        <div>{{shared.translate("MetaInfo")}}</div>
                                    </div>

                                    <div class="btn btn-sm btn-link text-decoration-none" @click="openById({compPath:'/a.Components/BaseInfo_UiInfoUpdate',recordKey:i.Id,refereshOnCallback:true,actionsAllowed:'DefaultRepo.BaseInfo.UiInfoUpdate',fkToParent:'ParentId'});">
                                        <div>{{shared.translate("UiInfo")}}</div>
                                    </div>

                                    <div class="btn btn-sm btn-link text-decoration-none" @click="createNew(i.Id)">
                                        <div>{{shared.translate("Create")}}</div>
                                    </div>
                                </td>

                                <td style="width:40px;vertical-align:middle" class="text-center text-secondary text-hover-danger pointer" data-ae-actions="DefaultRepo.BaseInfo.DeleteByKey" @click="deleteById({pkValue:i.Id})">
                                    <i class="fa-solid fa-fw fa-trash"></i>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
        <div class="card-footer ae-list-footer">
            <div class="d-flex flex-wrap align-items-center justify-content-center justify-content-md-between gap-3">
                
                <!-- Controls Group -->
                <div class="d-none d-md-flex align-items-center gap-3 flex-wrap">
                    <!-- Order By -->
                    <div class="d-flex align-items-center gap-2">
                        <span class="text-secondary small">{{shared.translate("OrderBy")}}</span>
                        <div class="d-flex gap-1">
                            <select class="form-select form-select-sm border-0 bg-transparent text-secondary fw-medium" style="min-width: 100px;" v-model="initialRequests[0].Inputs.ClientQueryJE.OrderClauses[0].Name" @change="loadRecords()">
                                <option v-for="o in orderableColumns" :value="o">{{shared.translate(o)}}</option>
                            </select>
                            <select class="form-select form-select-sm border-0 bg-transparent text-secondary fw-medium" style="width: 80px;" v-model="initialRequests[0].Inputs.ClientQueryJE.OrderClauses[0].OrderDirection" @change="loadRecords()">
                                <option value="ASC">{{shared.translate("Asc")}}</option>
                                <option value="DESC">{{shared.translate("Desc")}}</option>
                            </select>
                        </div>
                    </div>

                    <div class="vr text-secondary opacity-25 d-none d-md-block"></div>

                    <!-- Page Size -->
                    <div class="d-flex align-items-center gap-2">
                        <span class="text-secondary small">{{shared.translate("PageSize")}}</span>
                        <select class="form-select form-select-sm border-0 bg-transparent text-secondary fw-medium" style="width: 70px;" v-model.number="initialRequests[0].Inputs.ClientQueryJE.Pagination.PageSize" @change="loadRecords()">
                            <option value="50">50</option>
                            <option value="100">100</option>
                            <option value="200">200</option>
                        </select>
                    </div>
                </div>

                <!-- Pagination -->
                <div class="d-flex align-items-center">
                    <div class="pagination ae-pagination m-0"></div>
                </div>

                <!-- Stats -->
                <div class="d-none d-md-flex align-items-center gap-3 text-secondary small" v-if="initialResponses[0].IsSucceeded===true">
                    <div>
                        <span>{{shared.translate("Rows")}}:</span>
                        <span class="fw-bold text-primary ms-1">{{initialResponses[0]["Result"]["Aggregations"][0]["Count"]}}</span>
                    </div>
                    <div class="vr opacity-25"></div>
                    <div>
                        <span>{{shared.translate("Duration")}}:</span>
                        <span class="fw-bold text-primary ms-1">{{initialResponses[0]["Duration"]/1000}}s</span>
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
    _this.objectName = "BaseInfo";
    _this.loadMethod = "DefaultRepo.BaseInfo.ReadList";
    _this.filePrefix = "";
    _this.deleteMethod = `${_this.dbConfName}.${_this.objectName}.DeleteByKey`;
    _this.orderableColumns = ["Id", "CreatedOn", "UpdatedOn", "Title"];
    _this.orderClauses = [{ Name: "Id", OrderDirection: "ASC" }];
    _this.initialResponses = [{ Duration: 0, Result: { Master: [], Aggregations: [{ "Count": 0 }] } }];
    _this.initialRequests = [genListRequest(_this.loadMethod, {}, _this.orderClauses, { PageNumber: 1, PageSize: 50 })];
    _this.filter = { "ParentId": "", "Title": null, "IsActive": null, "Id": null, "ShortName": null, "Note": null, "Value": null };
    _this.initialSearchOptions = _.cloneDeep(_this.filter);
    _this.clientQueryMetadata = { "ParentObjectColumns": [{ "Name": "Id", "DevNote": "", "IsPrimaryKey": true, "DbType": "INT", "IsIdentity": true, "IdentityStart": "10000", "IdentityStep": "1", "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "IsDisabled": true, "Required": true, "ValidationRule": ":=i(0,2147483647)" } }, { "Name": "CreatedBy", "DevNote": "", "DbType": "INT", "UpdateGroup": "", "UiProps": { "Group": "Auditing", "UiWidget": "DisabledTextbox", "UiWidgetOptions": "{}", "IsDisabled": true, "Required": true, "ValidationRule": ":=i(0,2147483647)" } }, { "Name": "CreatedOn", "DevNote": "", "DbType": "DATETIME", "IsSortable": true, "UpdateGroup": "", "UiProps": { "Group": "Auditing", "UiWidget": "DisabledTextbox", "UiWidgetOptions": "{}", "IsDisabled": true, "Required": true, "ValidationRule": "dt(1900-01-01 00:01:00,2100-12-30 11:59:59)" } }, { "Name": "UpdatedBy", "DevNote": "", "DbType": "INT", "AllowNull": true, "UpdateGroup": "IsActiveUpdate", "UiProps": { "Group": "Auditing", "UiWidget": "DisabledTextbox", "UiWidgetOptions": "{}", "IsDisabled": true, "Required": false, "ValidationRule": ":=i(0,2147483647)" } }, { "Name": "UpdatedOn", "DevNote": "", "DbType": "DATETIME", "AllowNull": true, "IsSortable": true, "UpdateGroup": "IsActiveUpdate", "UiProps": { "Group": "Auditing", "UiWidget": "DisabledTextbox", "UiWidgetOptions": "{}", "IsDisabled": true, "Required": false, "ValidationRule": "dt(1900-01-01 00:01:00,2100-12-30 11:59:59)" } }, { "Name": "ParentId", "DevNote": "", "DbType": "INT", "AllowNull": true, "Fk": { "FkName": "BaseInfo_ParentId_BaseInfo_Id", "TargetTable": "BaseInfo", "TargetColumn": "Id", "EnforceRelation": true, "Lookup": { "Id": "ParentId_Lookup", "Method": "DefaultRepo.BaseInfo.ReadList", "Inputs": { "ClientQueryJE": { "QueryFullName": "DefaultRepo.BaseInfo.ReadList", "OrderClauses": [{ "Name": "ViewOrder", "OrderDirection": "ASC" }], "Pagination": { "PageNumber": 1, "PageSize": 500 }, "IncludeSubQueries": false } } } }, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "ObjectPicker", "UiWidgetOptions": "{}", "SearchType": "Fast", "Required": false, "ValidationRule": ":=i(0,2147483647)" } }, { "Name": "Title", "DevNote": "", "DbType": "NVARCHAR", "Size": "128", "IsHumanId": true, "IsSortable": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Fast", "Required": true, "ValidationRule": ":=s(0,128)" } }, { "Name": "ShortName", "DevNote": "", "DbType": "NVARCHAR", "Size": "16", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false, "ValidationRule": ":=s(0,16)" } }, { "Name": "ViewOrder", "DevNote": "", "DbType": "FLOAT", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "Required": false } }, { "Name": "Note", "DevNote": "", "DbType": "NVARCHAR", "Size": "256", "AllowNull": true, "UpdateGroup": "MetaInfoUpdate", "UiProps": { "Group": "", "UiWidget": "Htmlbox", "UiWidgetOptions": "{\n    \u0022svgPath\u0022: \u0022/a..lib/Trumbowyg/ui/icons.svg\u0022\n}", "SearchType": "Expandable", "Required": false, "ValidationRule": ":=s(0,256)" } }, { "Name": "Metadata", "DevNote": "", "DbType": "NVARCHAR", "Size": "4000", "AllowNull": true, "UpdateGroup": "MetaInfoUpdate", "UiProps": { "Group": "", "UiWidget": "CodeEditorbox", "UiWidgetOptions": "{\n    \u0022mode\u0022: \u0022ace/mode/json\u0022\n}", "Required": false, "ValidationRule": ":=s(0,4000)" } }, { "Name": "IsActive", "DevNote": "", "DbType": "BIT", "AllowNull": true, "UpdateGroup": "IsActiveUpdate", "UiProps": { "Group": "", "UiWidget": "Checkbox", "UiWidgetOptions": "{}", "SearchType": "Fast", "Required": false } }, { "Name": "UiColor", "DevNote": "", "DbType": "VARCHAR", "Size": "16", "AllowNull": true, "UpdateGroup": "UiInfoUpdate", "UiProps": { "Group": "", "UiWidget": "ColorPicker", "UiWidgetOptions": "{}", "Required": false, "ValidationRule": ":=s(0,16)" } }, { "Name": "UiIcon", "DevNote": "", "DbType": "VARCHAR", "Size": "64", "AllowNull": true, "UpdateGroup": "UiInfoUpdate", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "Required": false, "ValidationRule": ":=s(0,64)" } }, { "Name": "Value", "DevNote": "", "DbType": "INT", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false, "ValidationRule": ":=i(0,2147483647)" } }], "Name": "ReadList", "Type": "ReadList", "QueryColumns": ["Id", "CreatedBy", "CreatedOn", "UpdatedBy", "UpdatedOn", "ParentId", "Title", "ShortName", "ViewOrder", "Note", "Metadata", "IsActive", "UiColor", "UiIcon", "Value"], "FastSearchColumns": [{ "Name": "ParentId", "DevNote": "", "DbType": "INT", "AllowNull": true, "Fk": { "FkName": "BaseInfo_ParentId_BaseInfo_Id", "TargetTable": "BaseInfo", "TargetColumn": "Id", "EnforceRelation": true, "Lookup": { "Id": "ParentId_Lookup", "Method": "DefaultRepo.BaseInfo.ReadList", "Inputs": { "ClientQueryJE": { "QueryFullName": "DefaultRepo.BaseInfo.ReadList", "OrderClauses": [{ "Name": "ViewOrder", "OrderDirection": "ASC" }], "Pagination": { "PageNumber": 1, "PageSize": 500 }, "IncludeSubQueries": false } } } }, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "ObjectPicker", "UiWidgetOptions": "{}", "SearchType": "Fast", "Required": false, "ValidationRule": ":=i(0,2147483647)" } }, { "Name": "Title", "DevNote": "", "DbType": "NVARCHAR", "Size": "128", "IsHumanId": true, "IsSortable": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Fast", "Required": true, "ValidationRule": ":=s(0,128)" } }, { "Name": "IsActive", "DevNote": "", "DbType": "BIT", "AllowNull": true, "UpdateGroup": "IsActiveUpdate", "UiProps": { "Group": "", "UiWidget": "Checkbox", "UiWidgetOptions": "{}", "SearchType": "Fast", "Required": false } }], "ExpandableSearchColumns": [{ "Name": "Id", "DevNote": "", "IsPrimaryKey": true, "DbType": "INT", "IsIdentity": true, "IdentityStart": "10000", "IdentityStep": "1", "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "IsDisabled": true, "Required": true, "ValidationRule": ":=i(0,2147483647)" } }, { "Name": "ShortName", "DevNote": "", "DbType": "NVARCHAR", "Size": "16", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false, "ValidationRule": ":=s(0,16)" } }, { "Name": "Note", "DevNote": "", "DbType": "NVARCHAR", "Size": "256", "AllowNull": true, "UpdateGroup": "MetaInfoUpdate", "UiProps": { "Group": "", "UiWidget": "Htmlbox", "UiWidgetOptions": "{\n    \u0022svgPath\u0022: \u0022/a..lib/Trumbowyg/ui/icons.svg\u0022\n}", "SearchType": "Expandable", "Required": false, "ValidationRule": ":=s(0,256)" } }, { "Name": "Value", "DevNote": "", "DbType": "INT", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false, "ValidationRule": ":=i(0,2147483647)" } }], "OptionalQueries": [] };

    _this.pickerRequests.push({ "Id": "ParentId_Lookup", "Method": "DefaultRepo.BaseInfo.ReadList", "Inputs": { "ClientQueryJE": { "QueryFullName": "DefaultRepo.BaseInfo.ReadList", "OrderClauses": [{ "Name": "ViewOrder", "OrderDirection": "ASC" }], "Pagination": { "PageNumber": 1, "PageSize": 500 }, "IncludeSubQueries": false } } });

    _this.pickerHumanIds.push({ Id: 'ParentId_HumanIds', Items: ["Title"] });
    export default {
        methods: {
            searchAsParent(i) {
                _this.c.filter.ParentId = i.Id;
                _this.c.filter.ParentId_Title = i.Title;
                _this.c.loadRecords();
            },
            createNew(id) {
                _this.c.openCreate({ fkColumn: 'ParentId', fkValue: id, refereshOnCallback: true, dialog: { modalSize: 'modal-lg' } });
            },
            dotsToSpaces(i) {
                let idPartsCount = i.Id.split('.').length-1;
                let parentIdPartsCount = fixNull(_this.c.filter.ParentId, '') === '' ? 0 : fixNull(_this.c.filter.ParentId, '').split('.').length;
                let spacesCount = idPartsCount - parentIdPartsCount;
                if (spacesCount < 0) return;
                return "&nbsp;".repeat((idPartsCount - parentIdPartsCount) * 20);
            }
        },
        setup(props) { _this.cid = props['cid']; },
        data() { return _this; },
        created() { _this.c = this; assignDefaultMethods(_this); },
        mounted() { _this.c.loadRecords(function () { initVueComponent(_this); }); },
        props: { cid: String, ismodal: String }
    }
</script>