<template>
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
					<div class="form-control form-control-sm pointer data-ae-validation" data-ae-widget="nullableCheckbox" data-ae-widget-options="{&quot;shownull&quot;:true}">
						<i class="fa-solid fa-fw me-1"></i>
						<span>{{shared.translate('IsActive')}}</span>
						<input type="hidden" v-model="filter.IsActive" data-ae-validation-required="false">
					</div>
				</div>
				<div class="col-48 col-md-6">
					<input type="text" class="form-control form-control-sm" id="input_UserName" @keyup.enter="loadRecords()" v-model="filter.UserName" :placeholder="shared.translate('UserName')">
				</div>
			</div>
		</div>

		<div class="simple-search card-header p-2 bg-transparent rounded-0 collapse">
			<div class="row">
				<div class="col-48 col-md-6">
					<div class="form-control form-control-sm pointer data-ae-validation" data-ae-widget="nullableCheckbox"
						 data-ae-widget-options="{&quot;shownull&quot;:true,&quot;nullClasses&quot;: &quot;fa-minus text-secondary&quot;,&quot;trueClasses&quot;: &quot;fa-lock text-danger&quot;,&quot;falseClasses&quot;: &quot;fa-lock-open text-success&quot;}">
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
				<button type="button" class="btn btn-sm bg-hover-light px-3" onclick="switchVisibility(this,'.simple-search','show','fa-chevron-down','fa-chevron-up')">
					<i class="fa-solid fa-chevron-down me-1"></i>
				</button>
				<div class="p-0 ms-auto"></div>
				<button type="button" class="btn btn-sm border-0 btn-outline-success px-2" data-ae-actions="DefaultRepo.AAA_Users.Create" @click="openCreate()">
					<i class="fa-solid fa-file-alt fa-bounce pe-1" style="--fa-animation-iteration-count:1"></i>
					<span class="ms-1">{{shared.translate("Create")}}</span>
				</button>
				<div class="vr"></div>
				<div class="btn btn-sm border-0 btn-outline-success px-2" data-ae-actions="DefaultRepo.AAA_Users.ReadList" @click="exportExcel">
					<i class="fa-solid fa-file-excel pe-1"></i>
					<span class="ms-1">{{shared.translate("Export")}}</span>
				</div>
			</div>
		</div>
		<div class="card-body p-0">
			<div class="card h-100 border-light bg-light bg-opacity-75 border-0">
				<div class="card-body rounded rounded-1 border border-3 border-light fs-d8 p-0 bg-transparent scrollable">
					<table class="table table-sm table-hover w-100 ae-table m-0 bg-transparent">
						<thead>
							<tr class="d-none d-md-table-row d-lg-table-row d-xl-table-row">
								<th class="sticky-top ae-thead-th fb text-primary fw-bold text-center" style="width:75px;overflow: hidden;text-overflow: ellipsis;">
									<i class="fa-solid fa-fw fa-window-restore"></i>
								</th>
								<th class="sticky-top ae-thead-th text-center" style="width:75px;overflow: hidden;text-overflow: ellipsis;">
									<div>{{shared.translate("Picture")}}</div>
								</th>
								<th class="sticky-top ae-thead-th fb text-success" style="width:100px;">
									<div>{{shared.translate("UserName")}}</div>
								</th>
								<th class="sticky-top ae-thead-th text-center" style="width:75px;">
									<div>{{shared.translate("IsBuiltIn")}}</div>
								</th>
								<th class="sticky-top ae-thead-th " style="width:185px;">
									<div>{{shared.translate("RolesOfUser")}}</div>
								</th>
								<th class="sticky-top ae-thead-th" style="width:275px;">
									<div>{{shared.translate("Contact")}}</div>
								</th>
								<th class="sticky-top ae-thead-th" style="width:185px;">
									<div>{{shared.translate("Login")}}</div>
								</th>
								<th class="sticky-top ae-thead-th" style="width:175px;">
									<div>{{shared.translate("IsActive")}}</div>
								</th>
								<th class="sticky-top ae-thead-th" style="width:175px;">
									<div>{{shared.translate("LoginLocked")}}</div>
								</th>
								<th class="sticky-top ae-thead-th text-center" style="width:80px;">Attributes</th>

								<th class="sticky-top ae-thead-th"></th>
								<th class="sticky-top ae-thead-th text-center" style="width:40px;" data-ae-actions="DefaultRepo.AAA_Users.DeleteByKey"></th>
							</tr>
						</thead>
						<tbody v-if="initialResponses[0].IsSucceeded===true">
							<tr v-for="i in initialResponses[0]['Result']['Master']">
								<td class="ae-table-td text-dark text-center" @click="openById({compPath:'/a.Components/AAA_Users_UpdateByKey',recordKey:i.Id,refereshOnCallback:true,actionsAllowed:'DefaultRepo.AAA_Users.UpdateByKey',fkToParent:''});">
									<div class="text-primary text-hover-success pointer">
										<i class="fa-solid fa-fw fa-edit"></i>
										<br>
										<span class="pk">{{i.Id}}</span>
									</div>
								</td>
								<td class="text-center">
									<img :src="'data:image/png;base64, '+i.Picture_FileBody_xs" v-if="shared.fixNull(i.Picture_FileBody_xs,'')!==''" class="rounded-2 shadow-sm" style="width:95%;">
									<i class="fa-solid fa-fw fa-image fa-5x text-light" v-else=""></i>
								</td>
								<td class="ae-table-td">
									<span class="fw-bold">{{shared.fixNull(i["UserName"],'-')}}</span>
								</td>
								<td class="ae-table-td text-center">
									<span v-html="shared.convertBoolToIconWithOptions(i.IsBuiltIn ,{})"></span>
								</td>
								<td class="ae-table-td">
									<ul class="my-auto">
										<li v-for="r in shared.ld().values(JSON.parse(i['RolesOfUser']))">
											{{JSON.stringify(shared.ld().values(r)).replace('[','').replace(']','').replaceAll('"','')}}
										</li>
									</ul>
								</td>
								<td class="ae-table-td">
									<div class="input-group input-group-sm bg-hover-primary rounded-2 p-2">
										<div class="more-info" style="">
											<table class="w-100 h-100 fs-d8">
												<tbody>
													<tr>
														<td class="text-muted align-middle" style="min-width:45px;">{{shared.translate("Email")}}</td>
														<td class="text-dark fb align-middle">
															<span class="fw-bold">{{shared.fixNull(i["Email"],'-')}}</span>
														</td>
													</tr>
													<tr>
														<td class="text-muted align-middle" style="min-width:45px;">{{shared.translate("Mobile")}}</td>
														<td class="text-dark fb align-middle">
															<span class="fw-bold">{{shared.fixNull(i["Mobile"],'-')}}</span>
														</td>
													</tr>
												</tbody>
											</table>
										</div>
									</div>
								</td>
								<td class="ae-table-td">
									<table class="w-100 h-100 fs-d8">
										<tbody>
											<tr>
												<td class="align-middle text-danger" style="width:45px;">{{shared.translate("Fails")}}</td>
												<td class="fb align-middle text-danger">
													<div>{{shared.formatDate(i["LoginTryFailLastOn"])}} <span class="fw-bold">[ {{shared.fixNull(i["LoginTryFails"],'-')}} ]</span></div>
												</td>
											</tr>
											<tr>
												<td class="align-middle text-success" style="width:45px;">{{shared.translate("Seccess")}}</td>
												<td class="fb align-middle text-success">
													<div>{{shared.formatDate(i["LoginTrySuccessLastOn"])}} <span class="fw-bold">[ {{shared.fixNull(i["LoginTrySuccesses"],'-')}} ]</span></div>
												</td>
											</tr>
										</tbody>
									</table>
								</td>
								<td class="ae-table-td pointer" @click="openById({compPath:'/a.Components/AAA_Users_IsActiveUpdate',recordKey:i.Id,refereshOnCallback:true,actionsAllowed:'DefaultRepo.AAA_Users.IsActiveUpdate',fkToParent:''});">
									<div class="input-group input-group-sm bg-hover-primary rounded-2 p-2">
										<div class="input-group-text rounded-2 me-1">
											<span v-html="shared.convertBoolToIconWithOptions(i.IsActive ,{})"></span>
										</div>
										<div class="more-info" style="">
											<table class="w-100 h-100 fs-d8">
												<tbody>
													<tr>
														<td class="text-dark fb align-middle">
															<span class="fw-bold">{{shared.fixNull(i["IsActiveUpdatedBy"],'-')}}</span>
														</td>
													</tr>
													<tr>
														<td class="text-dark fb align-middle">
															<span class="fw-bold">{{shared.fixNullOrEmpty(shared.formatDateL(i["IsActiveUpdatedOn"]),'-')}}</span>
														</td>
													</tr>
												</tbody>
											</table>
										</div>
									</div>
								</td>
								<td class="ae-table-td pointer" @click="openById({compPath:'/a.Components/AAA_Users_LoginLockedUpdate',recordKey:i.Id,refereshOnCallback:true,actionsAllowed:'DefaultRepo.AAA_Users.LoginLockedUpdate',fkToParent:''});">
									<div class="input-group input-group-sm bg-hover-primary rounded-2 p-2">
										<div class="input-group-text rounded-2 me-1">
											<span v-html="shared.convertBoolToIconWithOptions(i.LoginLocked ,{    &quot;shownull&quot;: true,    &quot;nullClasses&quot;: &quot;fa-minus text-secondary&quot;,    &quot;trueClasses&quot;: &quot;fa-lock text-danger&quot;,    &quot;falseClasses&quot;: &quot;fa-lock-open text-success&quot;})"></span>
										</div>
										<div class="more-info" style="">
											<table class="w-100 h-100 fs-d8">
												<tbody>
													<tr>
														<td class="text-dark fb align-middle">
															<span class="fw-bold">{{shared.fixNull(i["LoginLockedUpdatedBy"],'-')}}</span>
														</td>
													</tr>
													<tr>
														<td class="text-dark fb align-middle">
															<span class="fw-bold">{{shared.fixNullOrEmpty(shared.formatDateL(i["LoginLockedUpdatedOn"]),'-')}}</span>
														</td>
													</tr>
												</tbody>
											</table>
										</div>
									</div>
								</td>
								<td class="ae-table-td text-center text-success text-hover-primary pointer" @click="openAttributesAccessSettings(i.Id,i.UserName)">
									<i class="fa-solid fa-fw fa-list shadow5"></i>
								</td>
								<td></td>
								<td class="ae-table-td text-center text-secondary text-hover-danger pointer" data-ae-actions="DefaultRepo.AAA_Users.DeleteByKey" @click="deleteById({pkValue:i.Id})">
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
				<div class="input-group-text border-0 fs-d9 d-none d-md-block d-lg-block d-xl-block bg-white pointer" data-ae-actions="DefaultRepo.AAA_Users.ReadList" @click="exportExcel">
					<i class="fa-solid fa-file-excel text-success"></i>
				</div>
				<span class="input-group-text border-0 fs-d6 fw-bold text-secondary pt-2 d-none d-md-block d-lg-block d-xl-block bg-white">|</span>
				<div class="input-group-text border-0 d-none d-md-block d-lg-block d-xl-block fs-d7 pt-2 bg-white">
					<span class="text-secondary">{{shared.translate("OrderBy")}}</span>
				</div>
				<select class="form-select form-select-sm text-primary border-0 ae-input d-none d-md-block d-lg-block d-xl-block bg-white" style="max-width:135px;" v-model="initialRequests[0].Inputs.ClientQueryJE.OrderClauses[0].Name" @change="loadRecords">
					<option v-for="o in orderableColumns" :value="o">{{shared.translate(o)}}</option>
				</select>
				<select class="form-select form-select-sm text-primary border-0 ae-input d-none d-md-block d-lg-block d-xl-block bg-white" style="max-width:125px;" v-model="initialRequests[0].Inputs.ClientQueryJE.OrderClauses[0].OrderDirection" @change="loadRecords">
					<option value="ASC">{{shared.translate("Asc")}}</option>
					<option value="DESC">{{shared.translate("Desc")}}</option>
				</select>
				<span class="input-group-text border-0 fs-d4 text-secondary d-none d-md-block d-lg-block d-xl-block bg-white"></span>
				<div class="input-group-text border-0 d-none d-md-block d-lg-block d-xl-block fs-d7 pt-2 bg-white">
					<span class="text-secondary">{{shared.translate("PageSize")}}</span>
				</div>
				<select class="form-select form-select-sm text-primary border-0 ae-input d-none d-md-block d-lg-block d-xl-block bg-white" style="max-width:75px;" v-model.number="initialRequests[0].Inputs.ClientQueryJE.Pagination.PageSize" @change="loadRecords">
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
	shared.setAppTitle(shared.translate("AAA_Users, ReadList"));
	let _this = { cid: "", c: null, templateType: "ReadList", dbConfName: "", objectName: "", loadMethod: "", deleteMethod: "", initialRequests: [], initialResponses: [], pickerRequests: [], pickerHumanIds: [], filter: {}, clientQueryMetadata: {}, orderClauses: [], orderableColumns: [] };
	_this.dbConfName = "DefaultRepo";
	_this.objectName = "AAA_Users";
	_this.loadMethod = "DefaultRepo.AAA_Users.ReadList";
    _this.filePrefix = "";
	_this.deleteMethod = `${_this.dbConfName}.${_this.objectName}.DeleteByKey`;
	_this.orderableColumns = ["CreatedOn", "UpdatedOn"];
	_this.orderClauses = [{ Name: "CreatedOn", OrderDirection: "ASC" }];
	_this.initialResponses = [{ Duration: 0, Result: { Master: [], Aggregations: [{ "Count": 0 }] } }];
	_this.initialRequests = [genListRequest(_this.loadMethod, {}, _this.orderClauses, { PageNumber: 1, PageSize: 50 })];
	_this.filter = { "UserName": null, "Id": null, "CreatedBy": null, "UpdatedBy": null, "IsBuiltIn": null, "Email": null, "Mobile": null, "Picture_FileName": null, "Picture_FileSize": null, "Picture_FileMime": null, "IsActive": null, "IsActiveUpdatedBy": null, "LoginLocked": null, "LoginTry": null, "LoginTryFails": null, "Settings": null };
	_this.initialSearchOptions = _.cloneDeep(_this.filter);
	_this.clientQueryMetadata = { "ParentObjectColumns": [{ "Name": "Id", "DevNote": "", "IsPrimaryKey": true, "DbType": "INT", "IsIdentity": true, "IdentityStart": "100000", "IdentityStep": "1", "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "IsDisabled": true, "Required": true, "ValidationRule": ":=i(0,2147483647)" } }, { "Name": "CreatedBy", "DevNote": "", "DbType": "INT", "UpdateGroup": "", "UiProps": { "Group": "Auditing", "UiWidget": "DisabledTextbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "IsDisabled": true, "Required": true, "ValidationRule": ":=i(0,2147483647)" } }, { "Name": "CreatedOn", "DevNote": "", "DbType": "DATETIME", "IsSortable": true, "UpdateGroup": "", "UiProps": { "Group": "Auditing", "UiWidget": "DisabledTextbox", "UiWidgetOptions": "{}", "IsDisabled": true, "Required": true, "ValidationRule": "dt(1900-01-01 00:01:00,2100-12-30 11:59:59)" } }, { "Name": "UpdatedBy", "DevNote": "", "DbType": "INT", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "Auditing", "UiWidget": "DisabledTextbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "IsDisabled": true, "Required": false, "ValidationRule": ":=i(0,2147483647)" } }, { "Name": "UpdatedOn", "DevNote": "", "DbType": "DATETIME", "AllowNull": true, "IsSortable": true, "UpdateGroup": "", "UiProps": { "Group": "Auditing", "UiWidget": "DisabledTextbox", "UiWidgetOptions": "{}", "IsDisabled": true, "Required": false, "ValidationRule": "dt(1900-01-01 00:01:00,2100-12-30 11:59:59)" } }, { "Name": "IsBuiltIn", "DevNote": "", "DbType": "BIT", "DbDefault": "0", "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Checkbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": true } }, { "Name": "UserName", "DevNote": "", "DbType": "NVARCHAR", "Size": "64", "IsHumanId": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Fast", "Required": true, "ValidationRule": ":=s(0,64)" } }, { "Name": "Email", "DevNote": "", "DbType": "VARCHAR", "Size": "64", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "Contact", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false, "ValidationRule": ":=s(0,128)" } }, { "Name": "Mobile", "DevNote": "", "DbType": "VARCHAR", "Size": "14", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "Contact", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false, "ValidationRule": ":=s(0,14)" } }, { "Name": "Picture_FileBody", "DevNote": "", "DbType": "IMAGE", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "ImageView", "UiWidgetOptions": "{}", "Required": false } }, { "Name": "Picture_FileBody_xs", "DevNote": "", "DbType": "IMAGE", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "ImageView", "UiWidgetOptions": "{}", "Required": false } }, { "Name": "Picture_FileName", "DevNote": "", "DbType": "NVARCHAR", "Size": "128", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false, "ValidationRule": ":=s(0,128)" } }, { "Name": "Picture_FileSize", "DevNote": "", "DbType": "INT", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false, "ValidationRule": ":=i(0,2147483647)" } }, { "Name": "Picture_FileMime", "DevNote": "", "DbType": "VARCHAR", "Size": "32", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false, "ValidationRule": ":=s(0,32)" } }, { "Name": "IsActive", "DevNote": "", "DbType": "BIT", "DbDefault": "1", "UpdateGroup": "IsActiveUpdate", "UiProps": { "Group": "", "UiWidget": "Checkbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": true } }, { "Name": "IsActiveUpdatedBy", "DevNote": "", "DbType": "INT", "AllowNull": true, "UpdateGroup": "IsActiveUpdate", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false, "ValidationRule": ":=i(0,2147483647)" } }, { "Name": "IsActiveUpdatedOn", "DevNote": "", "DbType": "DATETIME", "AllowNull": true, "UpdateGroup": "IsActiveUpdate", "UiProps": { "Group": "", "UiWidget": "DateTimePicker", "UiWidgetOptions": "{}", "Required": false, "ValidationRule": "dt(1900-01-01 00:01:00,2100-12-30 11:59:59)" } }, { "Name": "LoginLocked", "DevNote": "", "DbType": "BIT", "DbDefault": "0", "UpdateGroup": "LoginLockedUpdate", "UiProps": { "Group": "", "UiWidget": "Checkbox", "UiWidgetOptions": "{\n    \u0022shownull\u0022: true,\n    \u0022nullClasses\u0022: \u0022fa-minus text-secondary\u0022,\n    \u0022trueClasses\u0022: \u0022fa-lock text-danger\u0022,\n    \u0022falseClasses\u0022: \u0022fa-lock-open text-success\u0022\n}", "SearchType": "Expandable", "Required": true } }, { "Name": "LoginLockedUpdatedOn", "DevNote": "", "DbType": "DATETIME", "AllowNull": true, "UpdateGroup": "LoginLockedUpdate", "UiProps": { "Group": "", "UiWidget": "DateTimePicker", "UiWidgetOptions": "{}", "Required": false, "ValidationRule": "dt(1900-01-01 00:01:00,2100-12-30 11:59:59)" } }, { "Name": "LoginTry", "DevNote": "", "DbType": "BIT", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "Login", "UiWidget": "Checkbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false } }, { "Name": "LoginTryFails", "DevNote": "", "DbType": "INT", "AllowNull": true, "DbDefault": "0", "UpdateGroup": "", "UiProps": { "Group": "Login", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false, "ValidationRule": ":=i(0,2147483647)" } }, { "Name": "LoginTryOn", "DevNote": "", "DbType": "DATETIME", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "Login", "UiWidget": "DateTimePicker", "UiWidgetOptions": "{}", "Required": false, "ValidationRule": "dt(1900-01-01 00:01:00,2100-12-30 11:59:59)" } }, { "Name": "Settings", "DevNote": "", "DbType": "NTEXT", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "CodeEditorbox", "UiWidgetOptions": "{\n    \u0022mode\u0022: \u0022ace/mode/json\u0022\n}", "SearchType": "Expandable", "Required": false, "ValidationRule": ":=s(0,256)" } }, { "Name": "LoginLockedUpdatedBy", "DevNote": "", "DbType": "INT", "AllowNull": true, "UpdateGroup": "LoginLockedUpdate", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}" } }], "Name": "ReadList", "Type": "ReadList", "QueryColumns": ["Id", "CreatedBy", "CreatedOn", "UpdatedBy", "UpdatedOn", "IsBuiltIn", "UserName", "Email", "Mobile", "Picture_FileBody_xs", "IsActive", "IsActiveUpdatedBy", "IsActiveUpdatedOn", "LoginLocked", "LoginLockedUpdatedOn", "LoginTry", "LoginTryFails", "LoginTryOn", "LoginLockedUpdatedBy"], "FastSearchColumns": [{ "Name": "UserName", "DevNote": "", "DbType": "NVARCHAR", "Size": "64", "IsHumanId": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Fast", "Required": true, "ValidationRule": ":=s(0,64)" } }], "ExpandableSearchColumns": [{ "Name": "Id", "DevNote": "", "IsPrimaryKey": true, "DbType": "INT", "IsIdentity": true, "IdentityStart": "100000", "IdentityStep": "1", "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "IsDisabled": true, "Required": true, "ValidationRule": ":=i(0,2147483647)" } }, { "Name": "CreatedBy", "DevNote": "", "DbType": "INT", "UpdateGroup": "", "UiProps": { "Group": "Auditing", "UiWidget": "DisabledTextbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "IsDisabled": true, "Required": true, "ValidationRule": ":=i(0,2147483647)" } }, { "Name": "UpdatedBy", "DevNote": "", "DbType": "INT", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "Auditing", "UiWidget": "DisabledTextbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "IsDisabled": true, "Required": false, "ValidationRule": ":=i(0,2147483647)" } }, { "Name": "IsBuiltIn", "DevNote": "", "DbType": "BIT", "DbDefault": "0", "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Checkbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": true } }, { "Name": "Email", "DevNote": "", "DbType": "VARCHAR", "Size": "64", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "Contact", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false, "ValidationRule": ":=s(0,128)" } }, { "Name": "Mobile", "DevNote": "", "DbType": "VARCHAR", "Size": "14", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "Contact", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false, "ValidationRule": ":=s(0,14)" } }, { "Name": "Picture_FileName", "DevNote": "", "DbType": "NVARCHAR", "Size": "128", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false, "ValidationRule": ":=s(0,128)" } }, { "Name": "Picture_FileSize", "DevNote": "", "DbType": "INT", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false, "ValidationRule": ":=i(0,2147483647)" } }, { "Name": "Picture_FileMime", "DevNote": "", "DbType": "VARCHAR", "Size": "32", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false, "ValidationRule": ":=s(0,32)" } }, { "Name": "IsActive", "DevNote": "", "DbType": "BIT", "DbDefault": "1", "UpdateGroup": "IsActiveUpdate", "UiProps": { "Group": "", "UiWidget": "Checkbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": true } }, { "Name": "IsActiveUpdatedBy", "DevNote": "", "DbType": "INT", "AllowNull": true, "UpdateGroup": "IsActiveUpdate", "UiProps": { "Group": "", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false, "ValidationRule": ":=i(0,2147483647)" } }, { "Name": "LoginLocked", "DevNote": "", "DbType": "BIT", "DbDefault": "0", "UpdateGroup": "LoginLockedUpdate", "UiProps": { "Group": "", "UiWidget": "Checkbox", "UiWidgetOptions": "{\n    \u0022shownull\u0022: true,\n    \u0022nullClasses\u0022: \u0022fa-minus text-secondary\u0022,\n    \u0022trueClasses\u0022: \u0022fa-lock text-danger\u0022,\n    \u0022falseClasses\u0022: \u0022fa-lock-open text-success\u0022\n}", "SearchType": "Expandable", "Required": true } }, { "Name": "LoginTry", "DevNote": "", "DbType": "BIT", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "Login", "UiWidget": "Checkbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false } }, { "Name": "LoginTryFails", "DevNote": "", "DbType": "INT", "AllowNull": true, "DbDefault": "0", "UpdateGroup": "", "UiProps": { "Group": "Login", "UiWidget": "Textbox", "UiWidgetOptions": "{}", "SearchType": "Expandable", "Required": false, "ValidationRule": ":=i(0,2147483647)" } }, { "Name": "Settings", "DevNote": "", "DbType": "NTEXT", "AllowNull": true, "UpdateGroup": "", "UiProps": { "Group": "", "UiWidget": "CodeEditorbox", "UiWidgetOptions": "{\n    \u0022mode\u0022: \u0022ace/mode/json\u0022\n}", "SearchType": "Expandable", "Required": false, "ValidationRule": ":=s(0,256)" } }], "OptionalQueries": [] };


	export default {
		methods: {
            openAttributesAccessSettings(UserId, UserName) {
                openComponent("/a.Components/AAA_Users_Attributes", {
                    title: "Role Attributes", modalSize: "modal-fullscreen", params: {
                        UserId: UserId,
                        UserName: UserName,
                        callback: function (ret) {

                        }
                    }
                });
            }
		},
		setup(props) { _this.cid = props['cid']; },
		data() { return _this; },
		created() { _this.c = this; assignDefaultMethods(_this); },
		mounted() { _this.c.loadRecords(); },
		props: { cid: String, ismodal: String }
	}

</script>