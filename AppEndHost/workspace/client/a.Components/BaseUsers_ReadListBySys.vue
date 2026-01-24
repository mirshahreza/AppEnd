<template>
<div class="card h-100 bg-transparent rounded-0 border-0">
		<div class="card-header p-2 bg-body-subtle rounded-0 border-0">
			<div class="container-fluid">
				<div class="row">
					<div class="col-48 col-md-6">
						<input type="text" class="form-control form-control-sm" id="input_UserName" @keyup.enter="loadRecords()" v-model="filter.UserName" :placeholder="shared.translate('UserName')">
					</div>
				</div>
			</div>
		</div>
		<div class="card-header simple-search p-2 px-0 bg-transparent rounded-0 border-0 collapse">
			<div class="container-fluid">
				<div class="row">
					<div class="col-48 col-md-6">
						<div class="form-control form-control-sm pointer data-ae-validation" data-ae-widget="nullableCheckbox" data-ae-widget-options="{&quot;shownull&quot;:true}">
							<i class="fa-solid fa-fw me-1"></i>
							<span>{{shared.translate('IsBuiltIn')}}</span>
							<input type="hidden" v-model="filter.IsBuiltIn" data-ae-validation-required="false">
						</div>
					</div>
					<div class="col-48 col-md-6">
						<div class="form-control form-control-sm pointer data-ae-validation" data-ae-widget="nullableCheckbox" data-ae-widget-options="{&quot;shownull&quot;:true}">
							<i class="fa-solid fa-fw me-1"></i>
							<span>{{shared.translate('IsActive')}}</span>
							<input type="hidden" v-model="filter.IsActive" data-ae-validation-required="false">
						</div>
					</div>
					<div class="col-48 col-md-6">
						<div class="form-control form-control-sm pointer data-ae-validation" data-ae-widget="nullableCheckbox" data-ae-widget-options="{&quot;shownull&quot;:true}">
							<i class="fa-solid fa-fw me-1"></i>
							<span>{{shared.translate('LoginLocked')}}</span>
							<input type="hidden" v-model="filter.LoginLocked" data-ae-validation-required="false">
						</div>
					</div>
					<div class="col-48 col-md-6">
						<input type="text" class="form-control form-control-sm" id="input_Email" @keyup.enter="loadRecords()" v-model="filter.Email" :placeholder="shared.translate('Email')">
					</div>
					<div class="col-48 col-md-6">
						<input type="text" class="form-control form-control-sm" id="input_Mobile" @keyup.enter="loadRecords()" v-model="filter.Mobile" :placeholder="shared.translate('Mobile')">
					</div>
					<div class="col-48 col-md-6">
						<input type="text" class="form-control form-control-sm" id="input_Settings" @keyup.enter="loadRecords()" v-model="filter.Settings" :placeholder="shared.translate('Settings')">
					</div>
					<div class="col-48 col-md-6">
						<input type="text" class="form-control form-control-sm" id="input_Id" @keyup.enter="loadRecords()" v-model="filter.Id" :placeholder="shared.translate('Id')">
					</div>
				</div>
				<div class="row">
				</div>
				<div class="row">
				</div>
			</div>
		</div>
		<div class="card-header p-2 px-3 rounded-0 border-0">
			<div class="hstack gap-1">
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
				<button type="button" class="btn btn-sm border-0 btn-outline-success px-2" data-ae-actions="DefaultRepo.BaseUsers.Create" @click="openCreate()">
					<i class="fa-solid fa-file-alt fa-bounce" style="--fa-animation-iteration-count:1"></i>
					<span>{{shared.translate("Create")}}</span>
				</button>
				<div class="vr"></div>
				<div class="btn btn-sm border-0 btn-outline-success px-2" data-ae-actions="DefaultRepo.BaseUsers.ReadList" @click="exportExcel">
					<i class="fa-solid fa-file-excel"></i>
					<span>{{shared.translate("Export")}}</span>
				</div>
			</div>
		</div>
		<div class="card-body p-0 border-0">
			<div class="card h-100 border-light bg-light bg-opacity-75 border-0">
				<div class="card-body border-0 p-0 scrollable">
					<table class="table table-sm table-hover w-100 ae-table m-0 bg-transparent fs-d8">
						<thead>
							<tr class="d-none d-md-table-row d-lg-table-row d-xl-table-row">
								<th class="sticky-top ae-thead-th fb text-primary fw-bold text-center" style="width:95px;overflow: hidden;text-overflow: ellipsis;">
									<i class="fa-solid fa-fw fa-window-restore"></i>
								</th>
								<th class="sticky-top ae-thead-th fb text-success" style="width:185px;">
									<div>{{shared.translate("HumanIds")}}</div>
								</th>
								<th class="sticky-top ae-thead-th text-center" style="width:95px;overflow: hidden;text-overflow: ellipsis;">
									<div>{{shared.translate("IsBuiltIn")}}</div>
								</th>
								<th class="sticky-top ae-thead-th " style="width:185px;">
									<div>{{shared.translate("")}}</div>
								</th>
								<th class="sticky-top ae-thead-th " style="width:185px;">
									<div>{{shared.translate("RolesOfUser")}}</div>
								</th>
								<th class="sticky-top ae-thead-th " style="width:185px;">
									<div>{{shared.translate("Contact")}}</div>
								</th>
								<th class="sticky-top ae-thead-th " style="width:185px;">
									<div>{{shared.translate("Login")}}</div>
								</th>
								<th class="sticky-top ae-thead-th " style="width:185px;">
									<div>{{shared.translate("IsActive")}}</div>
								</th>
								<th class="sticky-top ae-thead-th " style="width:185px;">
									<div>{{shared.translate("LoginLocked")}}</div>
								</th>
								<th class="sticky-top ae-thead-th"></th>
								<th style="width:40px;" class="sticky-top ae-thead-th text-center" data-ae-actions="DefaultRepo.BaseUsers.DeleteByKey"></th>
							</tr>
						</thead>
						<tbody v-if="initialResponses[0].IsSucceeded===true">
							<tr v-for="i in initialResponses[0]['Result']['Master']">
								<td class="ae-table-td text-primary bg-hover-light text-center pointer" @click="openById({compPath:'/a.Components/BaseUsers_UpdateByKey',recordKey:i.Id,refereshOnCallback:true,actionsAllowed:'DefaultRepo.BaseUsers.UpdateByKey',fkColumn:''});">
									<i class="fa-solid fa-fw fa-edit"></i>
									<div class="pk font-monospace">{{i.Id}}</div>
								</td>
								<td class="ae-table-td">
									<div>
										<span class="badge text-muted fs-d7 text-start me-1">{{shared.translate("UserName")}}</span>
										<span class="fw-bold">
											<span>{{shared.fixNull(i["UserName"],'-')}}</span>
										</span>
									</div>
								</td>
								<td class="ae-table-td text-center">
									<span v-html="shared.convertBoolToIconWithOptions(i.IsBuiltIn ,{})"></span>
								</td>
								<td class="ae-table-td ">
									<div>{{i["Picture_FileBody_xs"]}}</div>
								</td>
								<td class="ae-table-td ">
									<ul class="my-auto">
										<li v-for="r in shared.ld().values(JSON.parse(i['RolesOfUser']))">
											{{JSON.stringify(shared.ld().values(r)).replace('[','').replace(']','').replaceAll('"','')}}
										</li>
									</ul>
								</td>
								<td class="ae-table-td  ">
									<div class="input-group input-group-sm bg-hover-primary rounded-2 p-2">
										<div class="input-group-text border-0 bg-transparent me-1">
											<i class="fa-solid fa-fw fa-edit"></i>
										</div>
										<div class="more-info" style="">
											<table class="w-100 h-100 fs-d8">
												<tbody>
													<tr>
														<td class="text-muted align-middle" style="min-width:65px;">{{shared.translate("Email")}}</td>
														<td class="text-dark fw-bold align-middle">
															<span class="fw-bold">{{shared.fixNull(i["Email"],'-')}}</span>
														</td>
													</tr>
													<tr>
														<td class="text-muted align-middle" style="min-width:65px;">{{shared.translate("Mobile")}}</td>
														<td class="text-dark fw-bold align-middle">
															<span class="fw-bold">{{shared.fixNull(i["Mobile"],'-')}}</span>
														</td>
													</tr>
												</tbody>
											</table>
										</div>
									</div>
								</td>
								<td class="ae-table-td  ">
									<div class="input-group input-group-sm bg-hover-primary rounded-2 p-2">
										<div class="input-group-text border-0 bg-transparent me-1">
											<i class="fa-solid fa-fw fa-edit"></i>
										</div>
										<div class="more-info" style="">
											<table class="w-100 h-100 fs-d8">
												<tbody>
													<tr>
														<td class="text-muted align-middle" style="min-width:65px;">{{shared.translate("LoginTryFailsCount")}}</td>
														<td class="text-dark fw-bold align-middle">
															<span class="fw-bold">{{shared.fixNull(i["LoginTryFailsCount"],'-')}}</span>
														</td>
													</tr>
													<tr>
														<td class="text-muted align-middle" style="min-width:65px;">{{shared.translate("LoginTryFailLastOn")}}</td>
														<td class="text-dark fw-bold align-middle">
															<span class="fw-bold">{{shared.fixNullOrEmpty(shared.formatDateL(i["LoginTryFailLastOn"]),'-')}}</span>
														</td>
													</tr>
													<tr>
														<td class="text-muted align-middle" style="min-width:65px;">{{shared.translate("LoginTrySuccessesCount")}}</td>
														<td class="text-dark fw-bold align-middle">
															<span class="fw-bold">{{shared.fixNull(i["LoginTrySuccessesCount"],'-')}}</span>
														</td>
													</tr>
													<tr>
														<td class="text-muted align-middle" style="min-width:65px;">{{shared.translate("LoginTrySuccessLastOn")}}</td>
														<td class="text-dark fw-bold align-middle">
															<span class="fw-bold">{{shared.fixNullOrEmpty(shared.formatDateL(i["LoginTrySuccessLastOn"]),'-')}}</span>
														</td>
													</tr>
												</tbody>
											</table>
										</div>
									</div>
								</td>
								<td class="ae-table-td   pointer" @click="openById({compPath:'/a.Components/BaseUsers_IsActiveUpdate',recordKey:i.Id,refereshOnCallback:true,actionsAllowed:'DefaultRepo.BaseUsers.IsActiveUpdate',fkColumn:''});">
									<div class="input-group input-group-sm bg-hover-primary rounded-2 p-2">
										<div class="input-group-text rounded-2 me-1">
											<span v-html="shared.convertBoolToIconWithOptions(i.IsActive ,{})"></span>
										</div>
										<div class="more-info" style="">
											<table class="w-100 h-100 fs-d8">
												<tbody>
													<tr>
														<td class="text-muted align-middle" style="min-width:65px;">{{shared.translate("By")}}</td>
														<td class="text-dark fw-bold align-middle">
															<span class="fw-bold">{{shared.fixNull(i["IsActiveUpdatedBy"],'-')}}</span>
														</td>
													</tr>
													<tr>
														<td class="text-muted align-middle" style="min-width:65px;">{{shared.translate("On")}}</td>
														<td class="text-dark fw-bold align-middle">
															<span class="fw-bold">{{shared.fixNullOrEmpty(shared.formatDateL(i["IsActiveUpdatedOn"]),'-')}}</span>
														</td>
													</tr>
												</tbody>
											</table>
										</div>
									</div>
								</td>
								<td class="ae-table-td   pointer" @click="openById({compPath:'/a.Components/BaseUsers_LoginLockedUpdate',recordKey:i.Id,refereshOnCallback:true,actionsAllowed:'DefaultRepo.BaseUsers.LoginLockedUpdate',fkColumn:''});">
									<div class="input-group input-group-sm bg-hover-primary rounded-2 p-2">
										<div class="input-group-text rounded-2 me-1">
											<span v-html="shared.convertBoolToIconWithOptions(i.LoginLocked ,{    &quot;shownull&quot;: true,    &quot;nullClasses&quot;: &quot;fa-minus text-secondary&quot;,    &quot;trueClasses&quot;: &quot;fa-lock text-danger&quot;,    &quot;falseClasses&quot;: &quot;fa-lock-open text-success&quot;})"></span>
										</div>
										<div class="more-info" style="">
											<table class="w-100 h-100 fs-d8">
												<tbody>
													<tr>
														<td class="text-muted align-middle" style="min-width:65px;">{{shared.translate("On")}}</td>
														<td class="text-dark fw-bold align-middle">
															<span class="fw-bold">{{shared.fixNullOrEmpty(shared.formatDateL(i["LoginLockedUpdatedOn"]),'-')}}</span>
														</td>
													</tr>
													<tr>
														<td class="text-muted align-middle" style="min-width:65px;">{{shared.translate("By")}}</td>
														<td class="text-dark fw-bold align-middle">
															<span class="fw-bold">{{shared.fixNull(i["LoginLockedUpdatedBy"],'-')}}</span>
														</td>
													</tr>
												</tbody>
											</table>
										</div>
									</div>
								</td>
								<td></td>
								<td style="width:40px;vertical-align:middle" class="text-center text-secondary text-hover-danger pointer" data-ae-actions="DefaultRepo.BaseUsers.DeleteByKey" @click="deleteById({pkValue:i.Id})">
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
let _this = { cid: "", c: null, templateType: "ReadList", filePrefix: "", dbConfName: "", objectName: "", loadMethod: "", deleteMethod: "", initialRequests: [], initialResponses: [], pickerRequests: [], pickerHumanIds: [], filter: {}, clientQueryMetadata: {}, orderClauses: [], orderableColumns: [] };
_this.dbConfName = "DefaultRepo";
_this.objectName = "BaseUsers";
_this.loadMethod = "DefaultRepo.BaseUsers.ReadList";
_this.filePrefix = "";
_this.deleteMethod = `${_this.dbConfName}.${_this.objectName}.DeleteByKey`;
_this.orderableColumns = ["CreatedOn","UpdatedOn"];
_this.orderClauses = [{ Name: "CreatedOn", OrderDirection: "ASC" }];
_this.initialResponses = [{ Duration: 0, Result: { Master: [], Aggregations: [{ "Count": 0 }] } }];
_this.initialRequests = [genListRequest(_this.loadMethod, {}, _this.orderClauses, { PageNumber: 1, PageSize: 10 })];
_this.filter = {"UserName":null,"Id":null,"IsBuiltIn":null,"Email":null,"Mobile":null,"IsActive":null,"LoginLocked":null,"Settings":null};
_this.initialSearchOptions = _.cloneDeep(_this.filter);
_this.clientQueryMetadata = {"ParentObjectColumns":[{"Name":"Id","DevNote":"","IsPrimaryKey":true,"DbType":"INT","IsIdentity":true,"IdentityStart":"100000","IdentityStep":"1","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":true,"ValidationRule":":=i(0,2147483647)"}},{"Name":"CreatedBy","DevNote":"","DbType":"INT","UpdateGroup":"","UiProps":{"Group":"Auditing","UiWidget":"DisabledTextbox","UiWidgetOptions":"{}","IsDisabled":true,"Required":true,"ValidationRule":":=i(0,2147483647)"}},{"Name":"CreatedOn","DevNote":"","DbType":"DATETIME","IsSortable":true,"UpdateGroup":"","UiProps":{"Group":"Auditing","UiWidget":"DisabledTextbox","UiWidgetOptions":"{}","IsDisabled":true,"Required":true,"ValidationRule":"dt(1900-01-01 00:01:00,2100-12-30 11:59:59)"}},{"Name":"UpdatedBy","DevNote":"","DbType":"INT","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"Auditing","UiWidget":"DisabledTextbox","UiWidgetOptions":"{}","IsDisabled":true,"Required":false,"ValidationRule":":=i(0,2147483647)"}},{"Name":"UpdatedOn","DevNote":"","DbType":"DATETIME","AllowNull":true,"IsSortable":true,"UpdateGroup":"","UiProps":{"Group":"Auditing","UiWidget":"DisabledTextbox","UiWidgetOptions":"{}","IsDisabled":true,"Required":false,"ValidationRule":"dt(1900-01-01 00:01:00,2100-12-30 11:59:59)"}},{"Name":"IsBuiltIn","DevNote":"","DbType":"BIT","DbDefault":"0","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Checkbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true}},{"Name":"UserName","DevNote":"","DbType":"NVARCHAR","Size":"64","IsHumanId":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Fast","Required":true,"ValidationRule":":=s(0,64)"}},{"Name":"Email","DevNote":"","DbType":"VARCHAR","Size":"64","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"Contact","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,128)"}},{"Name":"Mobile","DevNote":"","DbType":"VARCHAR","Size":"14","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"Contact","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,14)"}},{"Name":"IsActive","DevNote":"","DbType":"BIT","DbDefault":"1","UpdateGroup":"IsActiveUpdate","UiProps":{"Group":"","UiWidget":"Checkbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true}},{"Name":"IsActiveUpdatedBy","DevNote":"","DbType":"INT","AllowNull":true,"UpdateGroup":"IsActiveUpdate","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","Required":false,"ValidationRule":":=i(0,2147483647)"}},{"Name":"IsActiveUpdatedOn","DevNote":"","DbType":"DATETIME","AllowNull":true,"UpdateGroup":"IsActiveUpdate","UiProps":{"Group":"","UiWidget":"DateTimePicker","UiWidgetOptions":"{}","Required":false,"ValidationRule":"dt(1900-01-01 00:01:00,2100-12-30 11:59:59)"}},{"Name":"LoginLocked","DevNote":"","DbType":"BIT","DbDefault":"0","UpdateGroup":"LoginLockedUpdate","UiProps":{"Group":"","UiWidget":"Checkbox","UiWidgetOptions":"{\n    \u0022shownull\u0022: true,\n    \u0022nullClasses\u0022: \u0022fa-minus text-secondary\u0022,\n    \u0022trueClasses\u0022: \u0022fa-lock text-danger\u0022,\n    \u0022falseClasses\u0022: \u0022fa-lock-open text-success\u0022\n}","SearchType":"Expandable","Required":true}},{"Name":"LoginLockedUpdatedOn","DevNote":"","DbType":"DATETIME","AllowNull":true,"UpdateGroup":"LoginLockedUpdate","UiProps":{"Group":"","UiWidget":"DateTimePicker","UiWidgetOptions":"{}","Required":false,"ValidationRule":"dt(1900-01-01 00:01:00,2100-12-30 11:59:59)"}},{"Name":"LoginTryFailsCount","DevNote":"","DbType":"INT","AllowNull":true,"DbDefault":"0","UpdateGroup":"","UiProps":{"Group":"Login","UiWidget":"Textbox","UiWidgetOptions":"{}","Required":false,"ValidationRule":":=i(0,2147483647)"}},{"Name":"LoginTrySuccessesCount","DevNote":"","DbType":"INT","AllowNull":true,"DbDefault":"0","UpdateGroup":"","UiProps":{"Group":"Login","UiWidget":"Textbox","UiWidgetOptions":"{}","Required":false,"ValidationRule":":=i(0,2147483647)"}},{"Name":"LoginTryFailLastOn","DevNote":"","DbType":"DATETIME","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"Login","UiWidget":"DateTimePicker","UiWidgetOptions":"{}","Required":false,"ValidationRule":"dt(1900-01-01 00:01:00,2100-12-30 11:59:59)"}},{"Name":"LoginTrySuccessLastOn","DevNote":"","DbType":"DATETIME","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"Login","UiWidget":"DateTimePicker","UiWidgetOptions":"{}","Required":false,"ValidationRule":"dt(1900-01-01 00:01:00,2100-12-30 11:59:59)"}},{"Name":"Settings","DevNote":"","DbType":"NTEXT","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"CodeEditorbox","UiWidgetOptions":"{\n    \u0022mode\u0022: \u0022ace/mode/json\u0022\n}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,256)"}},{"Name":"LoginLockedUpdatedBy","DevNote":"","DbType":"INT","AllowNull":true,"UpdateGroup":"LoginLockedUpdate","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}"}}],"Name":"ReadList","Type":"ReadList","QueryColumns":["Id","CreatedBy","CreatedOn","UpdatedBy","UpdatedOn","IsBuiltIn","UserName","Email","Mobile","IsActive","IsActiveUpdatedBy","IsActiveUpdatedOn","LoginLocked","LoginLockedUpdatedOn","LoginTryFailsCount","LoginTryFailLastOn","LoginTrySuccessesCount","LoginTrySuccessLastOn","LoginLockedUpdatedBy"],"FastSearchColumns":[{"Name":"UserName","DevNote":"","DbType":"NVARCHAR","Size":"64","IsHumanId":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Fast","Required":true,"ValidationRule":":=s(0,64)"}}],"ExpandableSearchColumns":[{"Name":"Id","DevNote":"","IsPrimaryKey":true,"DbType":"INT","IsIdentity":true,"IdentityStart":"100000","IdentityStep":"1","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":true,"ValidationRule":":=i(0,2147483647)"}},{"Name":"IsBuiltIn","DevNote":"","DbType":"BIT","DbDefault":"0","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Checkbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true}},{"Name":"Email","DevNote":"","DbType":"VARCHAR","Size":"64","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"Contact","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,128)"}},{"Name":"Mobile","DevNote":"","DbType":"VARCHAR","Size":"14","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"Contact","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,14)"}},{"Name":"IsActive","DevNote":"","DbType":"BIT","DbDefault":"1","UpdateGroup":"IsActiveUpdate","UiProps":{"Group":"","UiWidget":"Checkbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true}},{"Name":"LoginLocked","DevNote":"","DbType":"BIT","DbDefault":"0","UpdateGroup":"LoginLockedUpdate","UiProps":{"Group":"","UiWidget":"Checkbox","UiWidgetOptions":"{\n    \u0022shownull\u0022: true,\n    \u0022nullClasses\u0022: \u0022fa-minus text-secondary\u0022,\n    \u0022trueClasses\u0022: \u0022fa-lock text-danger\u0022,\n    \u0022falseClasses\u0022: \u0022fa-lock-open text-success\u0022\n}","SearchType":"Expandable","Required":true}},{"Name":"Settings","DevNote":"","DbType":"NTEXT","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"CodeEditorbox","UiWidgetOptions":"{\n    \u0022mode\u0022: \u0022ace/mode/json\u0022\n}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,256)"}}],"OptionalQueries":[]};


export default {
	methods: {
	},
	setup(props) { _this.cid = props['cid']; },
	data() { return _this; },
	created() { _this.c = this; assignDefaultMethods(_this); },
	mounted() { _this.c.loadRecords(function () { initVueComponent(_this); }); },
	props: { cid: String, ismodal: String }
}

</script>