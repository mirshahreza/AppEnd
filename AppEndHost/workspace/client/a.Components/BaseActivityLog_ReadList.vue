<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-header p-2 bg-body-subtle rounded-0 border-0">
            <div class="container-fluid">
                <div class="row">
                    <div class="col-48 col-md-6">
                        <div class="form-control form-control-sm text-nowrap pointer data-ae-validation" data-ae-widget="nullableCheckbox" data-ae-widget-options="{&quot;shownull&quot;:true}">
                            <i class="fa-solid fa-fw me-1"></i>
                            <span>{{shared.translate('IsSucceeded')}}</span>
                            <input type="hidden" v-model="filter.IsSucceeded" data-ae-validation-required="false">
                        </div>
                    </div>
                    <div class="col-48 col-md-6">
                        <input type="text" class="form-control form-control-sm" id="input_Controller" @keyup.enter="loadRecords()" v-model="filter.Controller" :placeholder="shared.translate('Controller')">
                    </div>
                    <div class="col-48 col-md-6">
                        <input type="text" class="form-control form-control-sm" id="input_Method" @keyup.enter="loadRecords()" v-model="filter.Method" :placeholder="shared.translate('Method')">
                    </div>
                    <div class="col-48 col-md-6">
                        <input type="text" class="form-control form-control-sm" id="input_RecordId" @keyup.enter="loadRecords()" v-model="filter.RecordId" :placeholder="shared.translate('RecordId')">
                    </div>
                </div>
            </div>
        </div>
        <div class="simple-search card-header p-2 px-0 bg-transparent rounded-0 collapse border-0">
            <div class="container-fluid">
                <div class="row">
                    <div class="col-48 col-md-6">
                        <div class="form-control form-control-sm pointer data-ae-validation" data-ae-widget="nullableCheckbox" data-ae-widget-options="{&quot;shownull&quot;:true}">
                            <i class="fa-solid fa-fw me-1"></i>
                            <span>{{shared.translate('FromCache')}}</span>
                            <input type="hidden" v-model="filter.FromCache" data-ae-validation-required="false">
                        </div>
                    </div>
                    <div class="col-48 col-md-6">
                        <input type="text" class="form-control form-control-sm" id="input_EventById" @keyup.enter="loadRecords()" v-model="filter.EventById" :placeholder="shared.translate('EventById')">
                    </div>
                    <div class="col-48 col-md-6">
                        <input type="text" class="form-control form-control-sm" id="input_EventByName" @keyup.enter="loadRecords()" v-model="filter.EventByName" :placeholder="shared.translate('EventByName')">
                    </div>
                    <!-- Removed ClientIp and ClientAgent from search UI -->
                </div>
            </div>
        </div>
        <div class="card-header p-2 px-3 rounded-0 border-0">
            <div class="hstack">
                <button class="btn btn-sm btn-outline-primary px-3" @click="loadRecords()">
                    <i class="fa-solid fa-search me-1"></i> <span>{{shared.translate("Search")}}</span>
                </button>
                <button class="btn btn-sm btn-outline-secondary px-3" @click="resetSearchOptions">
                    <i class="fa-solid fa-eraser me-1"></i>
                    <span>{{shared.translate("Reset")}}</span>
                </button>
                <button type="button" class="btn btn-sm bg-hover-light px-3" onclick="switchVisibility(this,'.simple-search','show','fa-chevron-down','fa-chevron-up')">
                    <i class="fa-solid fa-chevron-down me-1"></i>
                </button>
                <div class="p-0 ms-auto"></div>
            </div>
        </div>

        <div class="card-body p-0 border-0">
            <div class="card h-100 border-light bg-light bg-opacity-75 border-0">
                <div class="card-body rounded rounded-1 border border-3 border-light fs-d8 p-0 bg-transparent scrollable">
                    <table class="table table-sm table-hover w-100 ae-table m-0 bg-transparent">
                        <thead>
                            <tr class="d-none d-md-table-row d-lg-table-row d-xl-table-row">
                                <th class="sticky-top ae-thead-th" style="width:300px;">
                                    <div>{{shared.translate("Method")}}</div>
                                </th>
                                <th class="sticky-top ae-thead-th text-center" style="width:200px;">
                                    <div>{{shared.translate("Duration")}}</div>
                                </th>
                                <th class="sticky-top ae-thead-th text-center" style="width:40px;">
                                    <div>{{shared.translate("IsSucceeded")}}</div>
                                </th>
                                <th class="sticky-top ae-thead-th text-center" style="width:60px;">
                                    <div>{{shared.translate("Cache")}}</div>
                                </th>
                                <th class="sticky-top ae-thead-th text-center" style="width:100px;">
                                    <div>{{shared.translate("RecordId")}}</div>
                                </th>
                                <th class="sticky-top ae-thead-th text-center" style="width:75px;">
                                    <div>{{shared.translate("EventById")}}</div>
                                </th>
                                <th class="sticky-top ae-thead-th text-center" style="width:75px;">
                                    <div>{{shared.translate("EventByName")}}</div>
                                </th>
                                <th class="sticky-top ae-thead-th text-center" style="width:100px;">
                                    <div>{{shared.translate("EventOn")}}</div>
                                </th>
                                <th class="sticky-top ae-thead-th text-center" style="width:10px;">
                                    <div>&nbsp;</div>
                                </th>
                            </tr>
                        </thead>
                        <tbody v-if="initialResponses[0].IsSucceeded===true">
                            <tr v-for="i in initialResponses[0]['Result']['Master']">
                                <td class="ae-table-td ">
                                    <div>{{i["Namespace"]}}.{{i["Controller"]}}.{{i["Method"]}}</div>
                                </td>
                                <td class="ae-table-td text-center">
                                    <div>{{i["Duration"]}}</div>
                                </td>
                                <td class="ae-table-td text-center">
                                    <span v-html="shared.convertBoolToIconWithOptions(i.IsSucceeded ,{})"></span>
                                </td>
                                <td class="ae-table-td text-center">
                                    <span v-html='shared.convertBoolToIconWithOptions(i.FromCache ,{"falseClasses":"fa-minus text-secondary"})'></span>
                                </td>
                                <td class="ae-table-td text-center">
                                    <div>{{i["RecordId"]}}</div>
                                </td>
                                <td class="ae-table-td text-center">
                                    <div>{{i["EventById"]}}</div>
                                </td>
                                <td class="ae-table-td text-center">
                                    <div>{{i["EventByName"]}}</div>
                                </td>
                                <td class="ae-table-td text-center">
                                    <div class="fs-d8" style="direction:ltr">{{shared.formatDateTimeL(i["EventOn"])}}</div>
                                </td>
                                <td class="ae-table-td text-center">
                                    <button class="btn btn-sm btn-link text-decoration-none" @click="showDetails(i)"><i class="fa-solid fa-circle-info"></i></button>
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
                            <option value="10">10</option>
                            <option value="25">25</option>
                            <option value="50">50</option>
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
        <!-- Details Modal -->
        <div class="modal fade" id="detailsModal" tabindex="-1" aria-hidden="true">
            <div class="modal-dialog modal-lg modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h6 class="modal-title">{{shared.translate('Details')}}</h6>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <pre class="mb-0"><code>{{JSON.stringify(detailsItem, null, 2)}}</code></pre>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-sm btn-secondary" data-bs-dismiss="modal">{{shared.translate('Close')}}</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    shared.setAppTitle("$auto$");
    let _this = { cid: "", c: null, templateType: "ReadList", dbConfName: "", objectName: "", loadMethod: "", deleteMethod: "", initialRequests: [], initialResponses: [], pickerRequests: [], pickerHumanIds: [], filter: {}, clientQueryMetadata: {}, orderClauses: [], orderableColumns: [], detailsItem: {} };
    _this.dbConfName = "DefaultRepo";
    _this.objectName = "BaseActivityLog";
    _this.loadMethod = "DefaultRepo.BaseActivityLog.ReadList";
    _this.deleteMethod = `${_this.dbConfName}.${_this.objectName}.`;
    _this.orderableColumns = ["EventOn"];
    _this.orderClauses = [{ Name: "EventOn", OrderDirection: "DESC" }];
    _this.initialResponses = [{ Duration: 0, Result: { Master: [], Aggregations: [{ "Count": 0 }] } }];
    _this.initialRequests = [genListRequest(_this.loadMethod, {}, _this.orderClauses, { PageNumber: 1, PageSize: 50 })];
    _this.filter = { "Method": null, "IsSucceeded": null, "RecordId": null, "Id": null, "FromCache": null, "EventById": null, "EventByName": null };
    _this.initialSearchOptions = _.cloneDeep(_this.filter);
    _this.clientQueryMetadata = {
        "ParentObjectColumns": [
            {
                "Name": "Id", "DevNote": "", "IsPrimaryKey": true, "DbType": "INT", "IsIdentity": true, "IdentityStart": "1", "IdentityStep": "1", "UpdateGroup": "",
                "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "IsDisabled": true, "Required": true, "ValidationRule": ":=i(0,2147483647)" }
            },
            { "Name": "Method", "DevNote": "", "DbType": "VARCHAR", "Size": "128", "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Fast", "Required": true, "ValidationRule": ":=s(0,128)" } }, { "Name": "IsSucceeded", "DevNote": "", "DbType": "BIT", "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Checkbox", "UiWidgetOptions": "{}", "SearchType": "Fast", "Required": true } }, { "Name": "FromCache", "DevNote": "", "DbType": "BIT", "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Checkbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": true } }, { "Name": "RecordId", "DevNote": "", "DbType": "VARCHAR", "Size": "64", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Fast", "Required": false, "ValidationRule": ":=s(0,64)" } }
            ,
            {
                "Name": "EventById", "DevNote": "", "DbType": "INT", "UpdateGroup": "",
                "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": true, "ValidationRule": ":=i(0,2147483647)" }
            }
            ,
            {
                "Name": "EventByName", "DevNote": "", "DbType": "NVARCHAR", "Size": "64", "UpdateGroup": "",
                "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": true, "ValidationRule": ":=s(0,64)" }
            }
            ,
            {
                "Name": "EventOn", "DevNote": "", "DbType": "DATETIME", "IsSortable": true, "UpdateGroup": "",
                "UiProps": { "Group": "", "UiWidget": "DateTimePicker", "UiWidgetOptions": "{}", "Required": true, "ValidationRule": "dt(1900-01-01 00:01:00,2100-12-30 11:59:59)" }
            },
            {
                "Name": "Duration", "DevNote": "", "DbType": "FLOAT", "UpdateGroup": "",
                "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "Required": true }
            }
        ], "Name": "ReadList", "Type": "ReadList", "QueryColumns": ["Id", "Method", "IsSucceeded", "FromCache", "RecordId", "EventById", "EventByName", "EventOn", "Duration"], "FastSearchColumns": [{ "Name": "Method", "DevNote": "", "DbType": "VARCHAR", "Size": "128", "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Fast", "Required": true, "ValidationRule": ":=s(0,128)" } }, { "Name": "IsSucceeded", "DevNote": "", "DbType": "BIT", "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Checkbox", "UiWidgetOptions": "{}", "SearchType": "Fast", "Required": true } }, { "Name": "RecordId", "DevNote": "", "DbType": "VARCHAR", "Size": "64", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Fast", "Required": false, "ValidationRule": ":=s(0,64)" } }], "ExpandableSearchColumns": [{ "Name": "Id", "DevNote": "", "IsPrimaryKey": true, "DbType": "INT", "IsIdentity": true, "IdentityStart": "1", "IdentityStep": "1", "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "IsDisabled": true, "Required": true, "ValidationRule": ":=i(0,2147483647)" } }, { "Name": "FromCache", "DevNote": "", "DbType": "BIT", "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Checkbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": true } },
        {
            "Name": "EventById", "DevNote": "", "DbType": "INT", "UpdateGroup": "",
            "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": true, "ValidationRule": ":=i(0,2147483647)" }
        },
        {
            "Name": "EventByName", "DevNote": "", "DbType": "NVARCHAR", "Size": "64", "UpdateGroup": "",
            "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": true, "ValidationRule": ":=s(0,256)" }
        }
        ], "OptionalQueries": []
    };

    export default {
        methods: {
            showDetails(row) {
                let rid = row["Id"] || row["RecordId"];
                let method = `${_this.dbConfName}.${_this.objectName}.ReadByKey`;
                let req = { requests: [{ Method: method, Inputs: { ClientQueryJE: { QueryFullName: method, "Where": { "CompareClauses": [{ "Name": "Id", "Value": rid, "ClauseOperator": "Equal" }] } } } }] };
                rpc({
                    requests: req.requests, onDone: function (res) {
                        res = res[0]["Result"]["Master"][0];
                        res["Inputs"] = JSON.parse(fixNull(res["Inputs"], '{}'));
                        showJson(res);
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