<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <!-- Filters -->
        <div class="card-header p-2 bg-body-subtle rounded-0 border-0">
            <div class="container-fluid">
                <div class="row g-1">
                    <div class="col-48 col-md-6">
                        <div class="form-control form-control-sm pointer data-ae-validation" data-ae-widget="nullableCheckbox" data-ae-widget-options="{&quot;shownull&quot;:true}">
                            <i class="fa-solid fa-fw me-1" aria-hidden="true"></i>
                            <span>{{shared.translate('IsBuiltIn')}}</span>
                            <input type="hidden" v-model="filter.IsBuiltIn" data-ae-validation-required="false" aria-label="Is Built In">
                        </div>
                    </div>
                    <div class="col-48 col-md-6">
                        <div class="form-control form-control-sm pointer data-ae-validation" data-ae-widget="nullableCheckbox" data-ae-widget-options="{&quot;shownull&quot;:true}">
                            <i class="fa-solid fa-fw me-1" aria-hidden="true"></i>
                            <span>{{shared.translate('IsActive')}}</span>
                            <input type="hidden" v-model="filter.IsActive" data-ae-validation-required="false" aria-label="Is Active">
                        </div>
                    </div>
                    <div class="col-48 col-md-6">
                        <input type="text" class="form-control form-control-sm" id="input_UserName" @keyup.enter="loadRecords()" v-model="filter.UserName" :placeholder="shared.translate('UserName')" aria-label="Username search">
                    </div>
                </div>
            </div>
        </div>
        <!-- Advanced Filters -->
        <div class="simple-search card-header p-2 bg-transparent rounded-0 border-0 collapse">
            <div class="container-fluid">
                <div class="row g-1">
                    <div class="col-48 col-md-6">
                        <div class="form-control form-control-sm pointer data-ae-validation" data-ae-widget="nullableCheckbox"
                             data-ae-widget-options="{&quot;shownull&quot;:true,&quot;nullClasses&quot;: &quot;fa-minus text-secondary&quot;,&quot;trueClasses&quot;: &quot;fa-lock text-danger&quot;,&quot;falseClasses&quot;: &quot;fa-lock-open text-success&quot;}">
                            <i class="fa-solid fa-fw me-1" aria-hidden="true"></i>
                            <span>{{shared.translate('LoginLocked')}}</span>
                            <input type="hidden" v-model="filter.LoginLocked" data-ae-validation-required="false" aria-label="Login Locked">
                        </div>
                    </div>
                    <div class="col-48 col-md-6">
                        <input type="text" class="form-control form-control-sm" id="input_Email" @keyup.enter="loadRecords()" v-model="filter.Email" :placeholder="shared.translate('Email')" aria-label="Email search">
                    </div>
                    <div class="col-48 col-md-6">
                        <input type="text" class="form-control form-control-sm" id="input_Mobile" @keyup.enter="loadRecords()" v-model="filter.Mobile" :placeholder="shared.translate('Mobile')" aria-label="Mobile search">
                    </div>
                </div>
            </div>
        </div>
        <!-- Actions -->
        <div class="card-header py-1 px-2 rounded-0 border-0">
            <div class="hstack gap-1">
                <div class="btn btn-sm btn-outline-primary px-3 border-0" @click="loadRecords()" aria-label="Search">
                    <i class="fa-solid fa-search me-1" aria-hidden="true"></i>
                    <span class="d-none d-md-inline">{{shared.translate("Search")}}</span>
                </div>
                <button class="btn btn-sm btn-outline-secondary px-3 border-0" @click="resetSearchOptions" aria-label="Reset">
                    <i class="fa-solid fa-eraser me-1" aria-hidden="true"></i>
                    <span class="d-none d-md-inline">{{shared.translate("Reset")}}</span>
                </button>
                <button type="button" class="btn btn-sm bg-hover-light px-3" onclick="switchVisibility(this,'.simple-search','show','fa-chevron-down','fa-chevron-up')" aria-label="Toggle advanced filters">
                    <i class="fa-solid fa-chevron-down me-1" aria-hidden="true"></i>
                </button>
                <div class="ms-auto"></div>
                <button type="button" class="btn btn-sm border-0 btn-outline-success px-2" data-ae-actions="DefaultRepo.BaseUsers.Create" @click="openCreate()" aria-label="Create User">
                    <i class="fa-solid fa-file fa-bounce pe-1" style="--fa-animation-iteration-count:1" aria-hidden="true"></i>
                    <span class="d-none d-md-inline">{{shared.translate("Create")}}</span>
                </button>
                <div class="vr d-none d-sm-block"></div>
                <button type="button" class="btn btn-sm border-0 btn-outline-success px-2" data-ae-actions="DefaultRepo.BaseUsers.ReadList" @click="exportExcel" aria-label="Export Excel">
                    <i class="fa-solid fa-file-excel pe-1" aria-hidden="true"></i>
                    <span class="d-none d-md-inline">{{shared.translate("Export")}}</span>
                </button>
            </div>
        </div>
        <!-- BODY -->
        <div class="card-body p-0">
            <div class="card h-100 border-light bg-light bg-opacity-75 border-0">
                <div class="card-body rounded-1 border border-1 border-light fs-d8 p-0 bg-transparent scrollable">
                    <!-- Desktop Grid -->
                    <div class="d-none d-md-block">
                        <table class="table table-sm table-hover w-100 ae-table m-0 bg-transparent">
                            <thead class="small bg-light">
                                <tr class="align-middle">
                                    <th class="sticky-top ae-thead-th text-center" style="width:75px;"><i class="fa-solid fa-fw fa-edit"></i></th>
                                    <th class="sticky-top ae-thead-th text-center" style="width:125px;">{{shared.translate("UserName")}}</th>
                                    <th class="sticky-top ae-thead-th" style="width:175px;">{{shared.translate("Contact")}}</th>
                                    <th class="sticky-top ae-thead-th text-center" style="width:100px;">{{shared.translate("BuiltIn")}}</th>
                                    <th class="sticky-top ae-thead-th" style="width:125px;">{{shared.translate("Login")}}</th>
                                    <th class="sticky-top ae-thead-th text-center" style="width:100px;">{{shared.translate("Active")}}</th>
                                    <th class="sticky-top ae-thead-th text-center" style="width:100px;"><i class="fa-solid fa-fw fa-edit"></i> {{shared.translate("Locked")}}</th>
                                    <th class="sticky-top ae-thead-th text-center" style="width:100px;"><i class="fa-solid fa-fw fa-edit"></i> {{shared.translate("Attributes")}}</th>
                                    <th class="sticky-top ae-thead-th"><i class="fa-solid fa-fw fa-edit"></i> {{shared.translate("RolesOfUser")}}</th>
                                    <th class="sticky-top ae-thead-th text-center"></th>
                                    <th class="sticky-top ae-thead-th text-center" data-ae-actions="DefaultRepo.BaseUsers.DeleteByKey" aria-label="Delete" style="width:40px;"></th>
                                </tr>
                            </thead>
                            <tbody v-if="initialResponses[0].IsSucceeded===true">
                                <tr v-for="i in records" :key="i.Id" class="align-middle">
                                    <td class="text-center p-0">
                                        <button type="button" class="btn btn-link btn-sm text-decoration-none py-0" @click="openById({compPath:'/a.Components/BaseUsers_UpdateByKey',recordKey:i.Id,refereshOnCallback:true,actionsAllowed:'DefaultRepo.BaseUsers.UpdateByKey',fkToParent:''});" aria-label="Edit {{i.UserName}}">
                                            <i class="fa-solid fa-fw fa-edit"></i> <span class="fw-semibold font-monospace">{{i.Id}}</span>
                                        </button>
                                    </td>
                                    <td class="text-center fw-semibold text-truncate" :title="shared.fixNull(i['UserName'],'-')">{{shared.fixNull(i['UserName'],'-')}}</td>
                                    <td class="small">
                                        <div><i class="fa-solid fa-at fa-xs text-secondary me-1"></i><span class="fw-bold">{{shared.fixNull(i['Email'],'-')}}</span></div>
                                        <div><i class="fa-solid fa-phone fa-xs text-secondary me-1"></i><span class="fw-bold">{{shared.fixNull(i['Mobile'],'-')}}</span></div>
                                    </td>
                                    <td class="text-center" v-html="shared.convertBoolToIconWithOptions(i.IsBuiltIn ,{})"></td>
                                    <td class="small">
                                        <div class="text-success">{{shared.fixNull(i['LoginTrySuccessesCount'],'0')}} <span>@ {{shared.formatDate(i['LoginTrySuccessLastOn'])}}</span></div>
                                        <div class="text-danger">{{shared.fixNull(i['LoginTryFailsCount'],'0')}} <span>@ {{shared.formatDate(i['LoginTryFailLastOn'])}}</span></div>
                                    </td>
                                    <td class="text-center pointer" @click="openById({compPath:'/a.Components/BaseUsers_IsActiveUpdate',recordKey:i.Id,refereshOnCallback:true,actionsAllowed:'DefaultRepo.BaseUsers.IsActiveUpdate',fkToParent:''});" v-html="shared.convertBoolToIconWithOptions(i.IsActive ,{})"></td>
                                    <td class="text-center pointer" @click="openById({compPath:'/a.Components/BaseUsers_LoginLockedUpdate',recordKey:i.Id,refereshOnCallback:true,actionsAllowed:'DefaultRepo.BaseUsers.LoginLockedUpdate',fkToParent:''});" v-html="shared.convertBoolToIconWithOptions(i.LoginLocked ,{ 'shownull': true,'nullClasses': 'fa-minus text-secondary','trueClasses': 'fa-lock text-danger','falseClasses': 'fa-lock-open text-success'})"></td>
                                    <td class="text-center pointer" @click="openAttributesAccessSettings(i.Id,i.UserName)">
                                        <i class="fa-solid fa-list" aria-hidden="true"></i>
                                    </td>
                                    <td class="small bg-hover-light pointer" @click="openById({compPath:'/a.Components/BaseUsers_RolesUpdate',recordKey:i.Id,refereshOnCallback:true,actionsAllowed:'DefaultRepo.BaseUsers.IsActiveUpdate',fkToParent:''});">
                                        <div class="d-flex flex-wrap gap-1 overflow-hidden" style="max-height:2.4rem;" v-if="roleValues(i).length>0">
                                            <span v-for="r in roleValues(i)" :key="r" class="badge rounded-pill text-bg-light border text-secondary" :title="r">{{r}}</span>
                                        </div>
                                        <div v-else>
                                            ...
                                        </div>
                                    </td>
                                    <td></td>
                                    <td class="text-center pointer text-danger" data-ae-actions="DefaultRepo.BaseUsers.DeleteByKey" @click="deleteById({pkValue:i.Id})" aria-label="Delete user">
                                        <i class="fa-solid fa-trash" aria-hidden="true"></i>
                                    </td>
                                </tr>
                            </tbody>
                            <tbody v-else>
                                <tr>
                                    <td colspan="11" class="text-center py-5 text-muted small">{{shared.translate('NoData') }}</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>


                    <!-- Mobile / Tablet Cards -->
                    <div class="d-md-none" v-if="initialResponses[0].IsSucceeded===true">
                        <div v-for="i in initialResponses[0]['Result']['Master']" :key="'m'+i.Id" class="card mx-1 mb-2 border rounded-3">
                            <div class="card-header py-1 px-2 d-flex bg-white">
                                <div class="fw-semibold small text-dark text-truncate" :title="shared.fixNull(i['UserName'],'-')">{{shared.fixNull(i['UserName'],'-')}}</div>
                            </div>
                            <div class="card-body p-1 small">
                                <div class="d-flex flex-wrap bg-light rounded-2 mb-1 px-1 py-1">
                                    <div class="me-2 d-flex align-items-center"><span class="text-muted me-1">{{shared.translate('BuiltIn')}}:</span><span v-html="shared.convertBoolToIconWithOptions(i.IsBuiltIn ,{})"></span></div>
                                    <div class="me-2 d-flex align-items-center pointer" @click="openById({compPath:'/a.Components/BaseUsers_IsActiveUpdate',recordKey:i.Id,refereshOnCallback:true,actionsAllowed:'DefaultRepo.BaseUsers.IsActiveUpdate',fkToParent:''});"><span class="text-muted me-1">{{shared.translate('Active')}}:</span><span v-html="shared.convertBoolToIconWithOptions(i.IsActive ,{})"></span></div>
                                    <div class="me-2 d-flex align-items-center pointer" @click="openById({compPath:'/a.Components/BaseUsers_LoginLockedUpdate',recordKey:i.Id,refereshOnCallback:true,actionsAllowed:'DefaultRepo.BaseUsers.LoginLockedUpdate',fkToParent:''});"><span class="text-muted me-1">{{shared.translate('Locked')}}:</span><span v-html="shared.convertBoolToIconWithOptions(i.LoginLocked ,{ 'shownull': true,'nullClasses': 'fa-minus text-secondary','trueClasses': 'fa-lock text-danger','falseClasses': 'fa-lock-open text-success'})"></span></div>
                                    <div class="ms-auto text-end text-muted">{{shared.formatDate(i['UpdatedOn'])}}</div>
                                </div>
                                <div class="mb-1 px-1" v-if="shared.fixNull(i['RolesOfUser'],'')!==''">
                                    <span v-for="r in roleValues(i)" :key="'mr'+r" class="badge rounded-pill text-bg-light border text-secondary me-1 mb-1">{{r}}</span>
                                </div>
                                <div class="px-1 mb-1">
                                    <i class="fa-solid fa-at fa-xs text-secondary me-1"></i><span class="fw-bold">{{shared.fixNull(i['Email'],'-')}}</span>
                                    <span class="mx-2 small text-secondary">|</span>
                                    <i class="fa-solid fa-phone fa-xs text-secondary me-1"></i><span class="fw-bold">{{shared.fixNull(i['Mobile'],'-')}}</span>
                                </div>
                            </div>
                            <div class="card-footer py-1 px-2 d-flex bg-white border-top">
                                <div class="small text-primary fw-bold pointer" @click="openById({compPath:'/a.Components/BaseUsers_UpdateByKey',recordKey:i.Id,refereshOnCallback:true,actionsAllowed:'DefaultRepo.BaseUsers.UpdateByKey',fkToParent:''});">
                                    <i class="fa-solid fa-pen me-1" aria-hidden="true"></i>{{i.Id}}
                                </div>
                                <div class="mx-2 small text-secondary">|</div>
                                <div class="small pointer text-success" @click="openAttributesAccessSettings(i.Id,i.UserName)"><i class="fa-solid fa-list me-1" aria-hidden="true"></i>{{shared.translate('Attributes')}}</div>

                                <div class="ms-auto small text-end text-muted pointer" data-ae-actions="DefaultRepo.BaseUsers.DeleteByKey" @click="deleteById({pkValue:i.Id})">
                                    <i class="fa-solid fa-trash" aria-hidden="true"></i>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- Footer -->
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
    </div>
</template>
<script>
    shared.setAppTitle("$auto$");
    let _this = { cid: "", c: null, templateType: "ReadList", dbConfName: "", objectName: "", loadMethod: "", deleteMethod: "", initialRequests: [], initialResponses: [], pickerRequests: [], pickerHumanIds: [], filter: {}, clientQueryMetadata: {}, orderClauses: [], orderableColumns: [] };
    _this.dbConfName = "DefaultRepo";
    _this.objectName = "BaseUsers";
    _this.loadMethod = "DefaultRepo.BaseUsers.ReadList";
    _this.filePrefix = "";
    _this.deleteMethod = `${_this.dbConfName}.${_this.objectName}.DeleteByKey`;
    _this.orderableColumns = ["CreatedOn", "UpdatedOn"];
    _this.orderClauses = [{ Name: "CreatedOn", OrderDirection: "ASC" }];
    _this.initialResponses = [{ Duration: 0, Result: { Master: [], Aggregations: [{ "Count": 0 }] } }];
    _this.initialRequests = [genListRequest(_this.loadMethod, {}, _this.orderClauses, { PageNumber: 1, PageSize: 50 })];
    _this.filter = { "UserName": null, "Id": null, "CreatedBy": null, "UpdatedBy": null, "IsBuiltIn": null, "Email": null, "Mobile": null, "Picture_FileName": null, "Picture_FileSize": null, "Picture_FileMime": null, "IsActive": null, "IsActiveUpdatedBy": null, "LoginLocked": null, "LoginTry": null, "LoginTryFailsCount": null, "Settings": null };
    _this.initialSearchOptions = _.cloneDeep(_this.filter);
    _this.clientQueryMetadata = {}; 

    export default {
        methods: {
            openAttributesAccessSettings(UserId, UserName) {
                openComponent("/a.Components/BaseUsers_Attributes", { title: "Role Attributes", modalSize: "modal-fullscreen", params: { UserId, UserName, callback: function () { } } });
            },
            roleValues(i) {
                try { return shared.ld().values(JSON.parse(i['RolesOfUser'])).map(r => JSON.stringify(shared.ld().values(r)).replace('[','').replace(']','').replaceAll('"','')); } catch { return []; }
            },
            rowClass(i) {
                return [ i.IsActive===false ? 'opacity-50' : '', i.LoginLocked===true ? 'bg-danger-subtle' : '', 'hover-bg-light' ];
            }
        },
        computed: {
            records(){
                if(this.initialResponses[0] && this.initialResponses[0].Result && this.initialResponses[0].Result.Master){return this.initialResponses[0].Result.Master;} return [];
            }
        },
        setup(props) { _this.cid = props['cid']; },
        data() { return _this; },
        created() { _this.c = this; assignDefaultMethods(_this); },
        mounted() { _this.c.loadRecords(function () { initVueComponent(_this); }); },
        props: { cid: String, ismodal: String }
    }
</script>