<template>
<div class="card h-100 rounded rounded-2 rounded-bottom-0 rounded-end-0 bg-transparent border-0">
		<div class="card-header p-2 bg-light-subtle rounded-end-0 border-0">
			<div class="input-group input-group-sm border-0 bg-transparent">
				<div class="form-control rounded-0 border-0 bg-transparent p-0">
					<div class="row">
						<div class="col-48 col-md-6">
							<div class="form-control pointer data-ae-validation" data-ae-widget="nullableCheckbox" data-ae-widget-options="{&quot;shownull&quot;:true}">
								<i class="fa-solid fa-fw me-1"></i>
								<span>{{shared.translate('IsBuiltIn')}}</span>
								<input type="hidden" v-model="searchOptions.IsBuiltIn" data-ae-validation-required="false">
							</div>
						</div>
						<div class="col-48 col-md-6">
							<input type="text" class="form-control" id="input_UserName" @keyup.enter="localCrudLoadRecords" v-model="searchOptions.UserName" :placeholder="shared.translate('UserName')">
						</div>
					</div>
				</div>
				<button class="btn btn-primary rounded-1 px-3 mx-2" @click="localCrudLoadRecords">
					<i class="fa-solid fa-search mx-1"></i>
					<span class="mx-1">{{shared.translate('Search')}}</span>
				</button>
				<button type="button" class="btn btn-sm bg-hover-light" onclick="switchVisibility(this,'.simple-search','show','fa-chevron-down','fa-chevron-up')">
					<i class="fa-solid fa-chevron-down"></i>
				</button>
				<span class="input-group-text border-0 bg-transparent fs-d4 text-secondary d-none d-md-block d-lg-block d-xl-block pt-2" data-ae-actions="DefaultRepo.AAA_Users.Create">|</span>
				<button type="button" class="btn btn-sm border-0 btn-outline-primary px-4 rounded-2" data-ae-actions="DefaultRepo.AAA_Users.Create" @click="localOpenCreate">
					<i class="fa-solid fa-file-alt fa-bounce pe-1" style="--fa-animation-iteration-count:1"></i>
					<span>{{shared.translate("Create")}}</span>
				</button>
			</div>
		</div>
		<div class="simple-search card-header p-2 bg-transparent rounded-0 collapse">
			<div class="row">
				<div class="col-48 col-md-6">
					<div class="form-control pointer data-ae-validation" data-ae-widget="nullableCheckbox" data-ae-widget-options="{&quot;shownull&quot;:true}">
						<i class="fa-solid fa-fw me-1"></i>
						<span>{{shared.translate('IsActive')}}</span>
						<input type="hidden" v-model="searchOptions.IsActive" data-ae-validation-required="false">
					</div>
				</div>
				<div class="col-48 col-md-6">
					<div class="form-control pointer data-ae-validation" data-ae-widget="nullableCheckbox" data-ae-widget-options="{&quot;shownull&quot;:true}">
						<i class="fa-solid fa-fw me-1"></i>
						<span>{{shared.translate('LoginLocked')}}</span>
						<input type="hidden" v-model="searchOptions.LoginLocked" data-ae-validation-required="false">
					</div>
				</div>
				<div class="col-48 col-md-6">
					<div class="form-control pointer data-ae-validation" data-ae-widget="nullableCheckbox" data-ae-widget-options="{&quot;shownull&quot;:true}">
						<i class="fa-solid fa-fw me-1"></i>
						<span>{{shared.translate('LoginTry')}}</span>
						<input type="hidden" v-model="searchOptions.LoginTry" data-ae-validation-required="false">
					</div>
				</div>
				<div class="col-48 col-md-6">
					<input type="text" class="form-control" id="input_Email" @keyup.enter="localCrudLoadRecords" v-model="searchOptions.Email" :placeholder="shared.translate('Email')">
				</div>
				<div class="col-48 col-md-6">
					<input type="text" class="form-control" id="input_Mobile" @keyup.enter="localCrudLoadRecords" v-model="searchOptions.Mobile" :placeholder="shared.translate('Mobile')">
				</div>
				<div class="col-48 col-md-6">
					<input type="text" class="form-control" id="input_IsActiveUpdatedBy" @keyup.enter="localCrudLoadRecords" v-model="searchOptions.IsActiveUpdatedBy" :placeholder="shared.translate('IsActiveUpdatedBy')">
				</div>
				<div class="col-48 col-md-6">
					<input type="text" class="form-control" id="input_LoginTryFails" @keyup.enter="localCrudLoadRecords" v-model="searchOptions.LoginTryFails" :placeholder="shared.translate('LoginTryFails')">
				</div>
				<div class="col-48 col-md-6">
					<input type="text" class="form-control" id="input_CreatedBy" @keyup.enter="localCrudLoadRecords" v-model="searchOptions.CreatedBy" :placeholder="shared.translate('CreatedBy')">
				</div>
				<div class="col-48 col-md-6">
					<input type="text" class="form-control" id="input_UpdatedBy" @keyup.enter="localCrudLoadRecords" v-model="searchOptions.UpdatedBy" :placeholder="shared.translate('UpdatedBy')">
				</div>
				<div class="col-48 col-md-6">
					<input type="text" class="form-control" id="input_Id" @keyup.enter="localCrudLoadRecords" v-model="searchOptions.Id" :placeholder="shared.translate('Id')">
				</div>
			</div>
			<div class="row">
			</div>
			<div class="row">
			</div>
		</div>
		<div class="card-body p-1">
			<div class="card h-100 border-light bg-light bg-opacity-75 border-0">
				<div class="card-body rounded rounded-1 border border-3 border-light fs-d8 p-0 bg-transparent scrollable">
					<table class="table table-sm table-hover w-100 ae-table m-0 bg-transparent">
						<thead>
							<tr class="d-none d-md-table-row d-lg-table-row d-xl-table-row">
								<td class="sticky-top ae-thead-td fb text-primary fw-bold text-center" style="width:75px;overflow: hidden;text-overflow: ellipsis;">
									<div>{{shared.translate("Id")}}</div>
								</td>
								<td class="sticky-top ae-thead-td text-center" style="width:75px;overflow: hidden;text-overflow: ellipsis;">
									<div>{{shared.translate("Picture")}}</div>
								</td>
								<td class="sticky-top ae-thead-td fb text-success" style="min-width:185px;">
									<div>{{shared.translate("HumanIds")}}</div>
								</td>
								<td class="sticky-top ae-thead-td text-center" style="width:75px;overflow: hidden;text-overflow: ellipsis;">
									<div>{{shared.translate("IsBuiltIn")}}</div>
								</td>
								<td class="sticky-top ae-thead-td " style="min-width:185px;">
									<div>{{shared.translate("Roles")}}</div>
								</td>
								<td class="sticky-top ae-thead-td " style="min-width:185px;">
									<div>{{shared.translate("Contact")}}</div>
								</td>
								<td class="sticky-top ae-thead-td " style="min-width:185px;">
									<div>{{shared.translate("LoginTry")}}</div>
								</td>
								<td class="sticky-top ae-thead-td " style="min-width:185px;">
									<div>{{shared.translate("IsActiveUpdate")}}</div>
								</td>
								<td class="sticky-top ae-thead-td " style="min-width:185px;">
									<div>{{shared.translate("LoginLockedUpdate")}}</div>
								</td>
								<td style="width:40px;" class="sticky-top ae-thead-td text-center" data-ae-actions="DefaultRepo.AAA_Users.DeleteByKey"></td>
							</tr>
						</thead>
						<tbody>
							<tr v-for="i in initialResponses[0]['Result']['Master']">
								<td class="ae-table-td text-dark text-center" style="width:75px;overflow: hidden;text-overflow: ellipsis;" @click="localCrudOpenById('/.dbcomponents/DefaultRepo_AAA_Users_UpdateByKey','modal-lg',i.Id,true,'DefaultRepo.AAA_Users.UpdateByKey','');">
									<div class="pointer text-primary hover-success">
										<i class="fa-solid fa-fw fa-edit"></i>
										<br>
										<span class="pk">{{i.Id}}</span>
									</div>
								</td>
								<td class="text-center" style="width:75px;overflow: hidden;text-overflow: ellipsis;">
									<img :src="'data:image/png;base64, '+i.Picture_FileBody_xs" v-if="shared.fixNull(i.Picture_FileBody_xs,'')!==''" class="rounded-2 shadow-sm" style="width:95%;">
									<i class="fa-solid fa-fw fa-image fa-5x text-light" v-else=""></i>
								</td>
								<td class="ae-table-td" style="min-width:185px;">
									<div>
										<span class="text-muted fs-d9">{{shared.translate("UserName")}} :
										</span>
										<span class="fb">{{shared.fixNull(i["UserName"],'-')}}</span>
									</div>
								</td>
								<td class="ae-table-td text-center" style="width:75px;overflow: hidden;text-overflow: ellipsis;">
									<span v-html="shared.convertBoolToIconWithOptions(i.IsBuiltIn ,{})"></span>
								</td>
								<td class="ae-table-td " style="min-width:185px;">
									<ul class="my-auto">
										<li v-for="r in shared.ld().values(JSON.parse(i['Roles']))">
											{{JSON.stringify(shared.ld().values(r)).replace('[','').replace(']','').replaceAll('"','')}}
										</li>
									</ul>
								</td>
								<td class="ae-table-td  " style="min-width:185px;">
									<div class="input-group input-group-sm bg-hover-primary rounded-2 p-2">
										<div class="more-info" style="">
											<table class="w-100 h-100 fs-d7">
												<tbody>
													<tr>
														<td class="text-muted align-middle">{{shared.translate("Email")}}</td>
														<td class="text-dark fb align-middle">
															<span class="fw-bold">{{shared.fixNull(i["Email"],'-')}}</span>
														</td>
													</tr>
													<tr>
														<td class="text-muted align-middle">{{shared.translate("Mobile")}}</td>
														<td class="text-dark fb align-middle">
															<span class="fw-bold">{{shared.fixNull(i["Mobile"],'-')}}</span>
														</td>
													</tr>
												</tbody>
											</table>
										</div>
									</div>
								</td>
								<td class="ae-table-td  " style="min-width:185px;">
									<div class="input-group input-group-sm bg-hover-primary rounded-2 p-2">
										<div class="input-group-text rounded-2 me-1">
											<span v-html="shared.convertBoolToIconWithOptions(i.LoginTry ,{})"></span>
										</div>
										<div class="more-info" style="">
											<table class="w-100 h-100 fs-d7">
												<tbody>
													<tr>
														<td class="text-muted align-middle">{{shared.translate("Fails")}}</td>
														<td class="text-dark fb align-middle">
															<span class="fw-bold">{{shared.fixNull(i["LoginTryFails"],'-')}}</span>
														</td>
													</tr>
													<tr>
														<td class="text-muted align-middle">{{shared.translate("On")}}</td>
														<td class="text-dark fb align-middle">
															<span class="fw-bold">{{shared.fixNullOrEmpty(shared.formatDate(i["LoginTryOn"]),'-')}}</span>
														</td>
													</tr>
												</tbody>
											</table>
										</div>
									</div>
								</td>
								<td class="ae-table-td   pointer" style="min-width:185px;" @click="localCrudOpenById('/.dbcomponents/DefaultRepo_AAA_Users_IsActiveUpdate','modal-md',i.Id,true,'DefaultRepo.AAA_Users.IsActiveUpdate','');">
									<div class="input-group input-group-sm bg-hover-primary rounded-2 p-2">
										<div class="input-group-text rounded-2 me-1">
											<span v-html="shared.convertBoolToIconWithOptions(i.IsActive ,{})"></span>
										</div>
										<div class="more-info" style="">
											<table class="w-100 h-100 fs-d7">
												<tbody>
													<tr>
														<td class="text-muted align-middle">{{shared.translate("dBy")}}</td>
														<td class="text-dark fb align-middle">
															<span class="fw-bold">{{shared.fixNull(i["IsActiveUpdatedBy"],'-')}}</span>
														</td>
													</tr>
													<tr>
														<td class="text-muted align-middle">{{shared.translate("dOn")}}</td>
														<td class="text-dark fb align-middle">
															<span class="fw-bold">{{shared.fixNullOrEmpty(shared.formatDate(i["IsActiveUpdatedOn"]),'-')}}</span>
														</td>
													</tr>
												</tbody>
											</table>
										</div>
									</div>
								</td>
								<td class="ae-table-td   pointer" style="min-width:185px;" @click="localCrudOpenById('/.dbcomponents/DefaultRepo_AAA_Users_LoginLockedUpdate','modal-md',i.Id,true,'DefaultRepo.AAA_Users.LoginLockedUpdate','');">
									<div class="input-group input-group-sm bg-hover-primary rounded-2 p-2">
										<div class="input-group-text rounded-2 me-1">
											<span v-html="shared.convertBoolToIconWithOptions(i.LoginLocked ,{    &quot;shownull&quot;: true,    &quot;nullClasses&quot;: &quot;fa-minus text-secondary&quot;,    &quot;trueClasses&quot;: &quot;fa-lock text-danger&quot;,    &quot;falseClasses&quot;: &quot;fa-lock-open text-success&quot;})"></span>
										</div>
										<div class="more-info" style="">
											<table class="w-100 h-100 fs-d7">
												<tbody>
													<tr>
														<td class="text-muted align-middle">{{shared.translate("dOn")}}</td>
														<td class="text-dark fb align-middle">
															<span class="fw-bold">{{shared.fixNullOrEmpty(shared.formatDate(i["LoginLockedUpdatedOn"]),'-')}}</span>
														</td>
													</tr>
												</tbody>
											</table>
										</div>
									</div>
								</td>
								<td style="width:40px;vertical-align:middle" class="text-center" data-ae-actions="DefaultRepo.AAA_Users.DeleteByKey">
									<span @click="localCrudDeleteRecord(i.Id)">
										<i class="fa-solid fa-fw fa-times text-muted hover-danger pointer"></i>
									</span>
								</td>
							</tr>
						</tbody>
					</table>
				</div>
			</div>
		</div>
		<div class="card-footer bg-light-subtle rounded-bottom-0 p-0">
			<div class="input-group input-group-sm border-0 bg-transparent p-0">
				<div class="input-group-text border-0 d-none d-md-block d-lg-block d-xl-block fs-d7 pt-2">
					<span class="text-secondary">{{shared.translate("OrderBy")}}</span>
				</div>
				<select class="form-select form-select-sm border-0 bg-light ae-input d-none d-md-block d-lg-block d-xl-block" style="max-width:135px;" v-model="initialRequests[0].Inputs.ClientQueryJE.OrderClauses[0].Name" @change="localCrudLoadRecords">
					<option v-for="o in orderableColumns" :value="o">{{shared.translate(o)}}</option>
				</select>
				<select class="form-select form-select-sm border-0 bg-light ae-input d-none d-md-block d-lg-block d-xl-block" style="max-width:125px;" v-model="initialRequests[0].Inputs.ClientQueryJE.OrderClauses[0].OrderDirection" @change="localCrudLoadRecords">
					<option value="ASC">{{shared.translate("Asc")}}</option>
					<option value="DESC">{{shared.translate("Desc")}}</option>
				</select>
				<span class="input-group-text border-0 bg-transparent fs-d4 text-secondary d-none d-md-block d-lg-block d-xl-block">|</span>
				<div class="input-group-text border-0 d-none d-md-block d-lg-block d-xl-block fs-d7 pt-2">
					<span class="text-secondary">{{shared.translate("PageSize")}}</span>
				</div>
				<select class="form-select form-select-sm border-0 bg-light ae-input d-none d-md-block d-lg-block d-xl-block" style="max-width:75px;" v-model.number="initialRequests[0].Inputs.ClientQueryJE.Pagination.PageSize" @change="localCrudLoadRecords">
					<option value="10">10</option>
					<option value="25">25</option>
					<option value="50">50</option>
				</select>
				<div class="input-group-text border-0 d-none d-md-block d-lg-block d-xl-block">
					<div class="pagination"></div>
				</div>
				<input type="text" class="form-control form-control-sm border-0 rounded-0 bg-transparent d-none d-md-block d-lg-block d-xl-block" disabled="">
				<div class="input-group-text border-0 fs-d7 pt-2">
					<span class="text-secondary">{{shared.translate("Rows")}}</span>
					:
					<span class="text-success fw-bold mx-1">{{initialResponses[0]["Result"]["Aggregations"][0]["Count"]}}</span>
				</div>
				<div class="input-group-text border-0 fs-d7 pt-2">
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
let _this = { cid: "", c: null, dbConfName: "", objectName: "", loadMethod: "", deleteMethod: "", initialRequests: [], initialResponses: [], pickerRequests: [], pickerHumanIds: [], searchOptions: {}, clientQueryMetadata: {}, orderClauses: [], orderableColumns: [] };
_this.dbConfName = "DefaultRepo";
_this.objectName = "AAA_Users";
_this.loadMethod = "DefaultRepo.AAA_Users.ReadList";
_this.deleteMethod = `${_this.dbConfName}.${_this.objectName}.DeleteByKey`;
_this.orderableColumns = ["Id","UserName","Picture_FileSize","LoginTryFails","CreatedOn","UpdatedOn"];
_this.orderClauses = [{ Name: "Id", OrderDirection: "ASC" }];
_this.initialResponses = [{ Duration: 0, Result: { Master: [], Aggregations: [{ "Count": 0 }] } }];
_this.initialRequests = [genListRequest(_this.loadMethod, {}, _this.orderClauses, { PageNumber: 1, PageSize: 10 })];
_this.searchOptions = {"IsBuiltIn":null,"UserName":null,"Id":null,"Email":null,"Mobile":null,"Picture_FileName":null,"Picture_FileSize":null,"Picture_FileMime":null,"IsActive":null,"IsActiveUpdatedBy":null,"LoginLocked":null,"LoginTry":null,"LoginTryFails":null,"CreatedBy":null,"UpdatedBy":null};
_this.clientQueryMetadata = {"ParentObjectColumns":[{"Name":"IsBuiltIn","DevNote":"","DbType":"BIT","DbDefault":"0","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Checkbox","UiWidgetOptions":"","SearchType":"Fast","Required":true}},{"Name":"UserName","DevNote":"","DbType":"NVARCHAR","Size":"64","IsHumanId":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Fast","Required":true}},{"Name":"Id","DevNote":"","IsPrimaryKey":true,"DbType":"BIGINT","IsIdentity":true,"IdentityStart":"100000","IdentityStep":"1","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":true,"ValidationRule":":=i(0,10000)"}},{"Name":"Email","DevNote":"","DbType":"VARCHAR","Size":"64","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"Contact","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false}},{"Name":"Mobile","DevNote":"","DbType":"VARCHAR","Size":"14","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"Contact","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false}},{"Name":"Picture_FileName","DevNote":"","DbType":"NVARCHAR","Size":"128","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false}},{"Name":"Picture_FileSize","DevNote":"","DbType":"INT","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Numberbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=i(0,10000)"}},{"Name":"Picture_FileMime","DevNote":"","DbType":"VARCHAR","Size":"32","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false}},{"Name":"IsActive","DevNote":"","DbType":"BIT","DbDefault":"1","UpdateGroup":"IsActiveUpdate","UiProps":{"Group":"","UiWidget":"Checkbox","UiWidgetOptions":"","SearchType":"Expandable","Required":true}},{"Name":"IsActiveUpdatedBy","DevNote":"","DbType":"NVARCHAR","Size":"64","AllowNull":true,"UpdateGroup":"IsActiveUpdate","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false}},{"Name":"LoginLocked","DevNote":"","DbType":"BIT","DbDefault":"0","UpdateGroup":"LoginLockedUpdate","UiProps":{"Group":"","UiWidget":"Checkbox","UiWidgetOptions":"{\n    \u0022shownull\u0022: true,\n    \u0022nullClasses\u0022: \u0022fa-minus text-secondary\u0022,\n    \u0022trueClasses\u0022: \u0022fa-lock text-danger\u0022,\n    \u0022falseClasses\u0022: \u0022fa-lock-open text-success\u0022\n}","SearchType":"Expandable","Required":true}},{"Name":"LoginTry","DevNote":"","DbType":"BIT","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"LoginTry","UiWidget":"Checkbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false}},{"Name":"LoginTryFails","DevNote":"","DbType":"INT","AllowNull":true,"DbDefault":"0","UpdateGroup":"","UiProps":{"Group":"LoginTry","UiWidget":"Numberbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=i(0,10000)"}},{"Name":"CreatedBy","DevNote":"","DbType":"NVARCHAR","Size":"64","UpdateGroup":"","UiProps":{"Group":"Auditing","UiWidget":"DisabledTextbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":true}},{"Name":"UpdatedBy","DevNote":"","DbType":"NVARCHAR","Size":"64","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"Auditing","UiWidget":"DisabledTextbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":false}}],"Name":"ReadList","Type":"ReadList","QueryColumns":["Id","IsBuiltIn","UserName","Email","Mobile","Picture_FileBody_xs","IsActive","IsActiveUpdatedBy","IsActiveUpdatedOn","LoginLocked","LoginLockedUpdatedOn","LoginTry","LoginTryFails","LoginTryOn","CreatedBy","CreatedOn","UpdatedBy","UpdatedOn"],"FastSearchColumns":[{"Name":"IsBuiltIn","DevNote":"","DbType":"BIT","DbDefault":"0","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Checkbox","UiWidgetOptions":"","SearchType":"Fast","Required":true}},{"Name":"UserName","DevNote":"","DbType":"NVARCHAR","Size":"64","IsHumanId":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Fast","Required":true}}],"ExpandableSearchColumns":[{"Name":"Id","DevNote":"","IsPrimaryKey":true,"DbType":"BIGINT","IsIdentity":true,"IdentityStart":"100000","IdentityStep":"1","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":true,"ValidationRule":":=i(0,10000)"}},{"Name":"Email","DevNote":"","DbType":"VARCHAR","Size":"64","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"Contact","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false}},{"Name":"Mobile","DevNote":"","DbType":"VARCHAR","Size":"14","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"Contact","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false}},{"Name":"Picture_FileName","DevNote":"","DbType":"NVARCHAR","Size":"128","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false}},{"Name":"Picture_FileSize","DevNote":"","DbType":"INT","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Numberbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=i(0,10000)"}},{"Name":"Picture_FileMime","DevNote":"","DbType":"VARCHAR","Size":"32","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false}},{"Name":"IsActive","DevNote":"","DbType":"BIT","DbDefault":"1","UpdateGroup":"IsActiveUpdate","UiProps":{"Group":"","UiWidget":"Checkbox","UiWidgetOptions":"","SearchType":"Expandable","Required":true}},{"Name":"IsActiveUpdatedBy","DevNote":"","DbType":"NVARCHAR","Size":"64","AllowNull":true,"UpdateGroup":"IsActiveUpdate","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false}},{"Name":"LoginLocked","DevNote":"","DbType":"BIT","DbDefault":"0","UpdateGroup":"LoginLockedUpdate","UiProps":{"Group":"","UiWidget":"Checkbox","UiWidgetOptions":"{\n    \u0022shownull\u0022: true,\n    \u0022nullClasses\u0022: \u0022fa-minus text-secondary\u0022,\n    \u0022trueClasses\u0022: \u0022fa-lock text-danger\u0022,\n    \u0022falseClasses\u0022: \u0022fa-lock-open text-success\u0022\n}","SearchType":"Expandable","Required":true}},{"Name":"LoginTry","DevNote":"","DbType":"BIT","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"LoginTry","UiWidget":"Checkbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false}},{"Name":"LoginTryFails","DevNote":"","DbType":"INT","AllowNull":true,"DbDefault":"0","UpdateGroup":"","UiProps":{"Group":"LoginTry","UiWidget":"Numberbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=i(0,10000)"}},{"Name":"CreatedBy","DevNote":"","DbType":"NVARCHAR","Size":"64","UpdateGroup":"","UiProps":{"Group":"Auditing","UiWidget":"DisabledTextbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":true}},{"Name":"UpdatedBy","DevNote":"","DbType":"NVARCHAR","Size":"64","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"Auditing","UiWidget":"DisabledTextbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":false}}],"OptionalQueries":[]};





	export default {
	methods: {
		localOpenPicker(colName) { crudOpenPicker(_this, _this.c.searchOptions, colName); },
		localCrudLoadRecords() { crudLoadRecords(_this); },
		localCrudOpenById(compPath, modalSize, recordKey, refereshOnCallback, actionsAllowed) { crudOpenById(_this, compPath, modalSize, recordKey, refereshOnCallback, actionsAllowed); },
		localCrudDeleteRecord(recordKey) { crudDeleteRecord(_this, "Id", recordKey); },
		localOpenCreate() {
			crudOpenCreate(_this, `/.dbcomponents/${_this.dbConfName}_${_this.objectName}_Create`, 'modal-lg');
		}
	},
	setup(props) { _this.cid = props['cid']; },
	data() { return _this; },
	created() { _this.c = this; },
	mounted() { _this.c.localCrudLoadRecords(); },
	props: { cid: String }
}

</script>