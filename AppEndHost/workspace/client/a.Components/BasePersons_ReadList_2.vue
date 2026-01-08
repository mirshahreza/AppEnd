<template>
<div class="card h-100 bg-transparent rounded-0 border-0">
		<div class="card-header p-2 bg-body-subtle rounded-0 border-0">
			<div class="container-fluid">
				<div class="row">
					<div class="col-48 col-md-6">
						<select class="form-select form-select-sm" v-model="filter.EntityTypeId" data-ae-validation-required="false">
							<option value="">{{shared.translate('EntityTypeId')}}</option>
							<option v-for="i in shared.enum(102)" :value="i['Id']">{{i.Title}}</option>
						</select>
					</div>
					<div class="col-48 col-md-6">
						<input type="text" class="form-control form-control-sm" id="input_FirstName" @keyup.enter="loadRecords()" v-model="filter.FirstName" :placeholder="shared.translate('FirstName')">
					</div>
					<div class="col-48 col-md-6">
						<input type="text" class="form-control form-control-sm" id="input_LastName" @keyup.enter="loadRecords()" v-model="filter.LastName" :placeholder="shared.translate('LastName')">
					</div>
				</div>
			</div>
		</div>
		<div class="card-header simple-search p-2 px-0 bg-transparent rounded-0 border-0 collapse">
			<div class="container-fluid">
				<div class="row">
					<div class="col-48 col-md-6">
						<select class="form-select form-select-sm" v-model="filter.GenderId" data-ae-validation-required="false">
							<option value="">{{shared.translate('GenderId')}}</option>
							<option v-for="i in shared.enum(100)" :value="i['Id']">{{i.Title}}</option>
							<option>aaaaa</option>
						</select>
					</div>
					<div class="col-48 col-md-6">
						<select class="form-select form-select-sm" v-model="filter.RecordStateId" data-ae-validation-required="false">
							<option value="">{{shared.translate('RecordStateId')}}</option>
							<option v-for="i in shared.enum(101)" :value="i['Id']">{{i.Title}}</option>
							<option>aaaaa</option>
						</select>
					</div>
					<div class="col-48 col-md-6">
						<input type="text" class="form-control form-control-sm" id="input_CreatedBy" @keyup.enter="loadRecords()" v-model="filter.CreatedBy" :placeholder="shared.translate('CreatedBy')">
					</div>
					<div class="col-48 col-md-6">
						<input type="text" class="form-control form-control-sm" id="input_UpdatedBy" @keyup.enter="loadRecords()" v-model="filter.UpdatedBy" :placeholder="shared.translate('UpdatedBy')">
					</div>
					<div class="col-48 col-md-6">
						<input type="text" class="form-control form-control-sm" id="input_UserId" @keyup.enter="loadRecords()" v-model="filter.UserId" :placeholder="shared.translate('UserId')">
					</div>
					<div class="col-48 col-md-6">
						<input type="text" class="form-control form-control-sm" id="input_NationalCode" @keyup.enter="loadRecords()" v-model="filter.NationalCode" :placeholder="shared.translate('NationalCode')">
					</div>
					<div class="col-48 col-md-6">
						<input type="text" class="form-control form-control-sm" id="input_BirthYear" @keyup.enter="loadRecords()" v-model="filter.BirthYear" :placeholder="shared.translate('BirthYear')">
					</div>
					<div class="col-48 col-md-6">
						<input type="text" class="form-control form-control-sm" id="input_BirthMonth" @keyup.enter="loadRecords()" v-model="filter.BirthMonth" :placeholder="shared.translate('BirthMonth')">
					</div>
					<div class="col-48 col-md-6">
						<input type="text" class="form-control form-control-sm" id="input_BirthDay" @keyup.enter="loadRecords()" v-model="filter.BirthDay" :placeholder="shared.translate('BirthDay')">
					</div>
					<div class="col-48 col-md-6">
						<input type="text" class="form-control form-control-sm" id="input_Mobile" @keyup.enter="loadRecords()" v-model="filter.Mobile" :placeholder="shared.translate('Mobile')">
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
				<button type="button" class="btn btn-sm border-0 btn-outline-success px-2" data-ae-actions="DefaultRepo.BasePersons.Create" @click="openCreate()">
					<i class="fa-solid fa-file-alt fa-bounce" style="--fa-animation-iteration-count:1"></i>
					<span>{{shared.translate("Create")}}</span>
				</button>
				<div class="vr"></div>
				<div class="btn btn-sm border-0 btn-outline-success px-2" data-ae-actions="DefaultRepo.BasePersons.ReadList" @click="exportExcel">
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
								<th class="sticky-top ae-thead-th text-center" style="width:95px;overflow: hidden;text-overflow: ellipsis;">
									<div>{{shared.translate("Picture")}}</div>
								</th>
								<th class="sticky-top ae-thead-th fb text-success" style="width:185px;">
									<div>{{shared.translate("HumanIds")}}</div>
								</th>
								<th class="sticky-top ae-thead-th text-center" style="width:95px;overflow: hidden;text-overflow: ellipsis;">
									<div>{{shared.translate("UserId")}}</div>
								</th>
								<th class="sticky-top ae-thead-th text-center" style="width:185px;">
									<div>{{shared.translate("NationalCode")}}</div>
								</th>
								<th class="sticky-top ae-thead-th text-center" style="width:95px;overflow: hidden;text-overflow: ellipsis;">
									<div>{{shared.translate("BirthYear")}}</div>
								</th>
								<th class="sticky-top ae-thead-th text-center" style="width:95px;overflow: hidden;text-overflow: ellipsis;">
									<div>{{shared.translate("BirthMonth")}}</div>
								</th>
								<th class="sticky-top ae-thead-th text-center" style="width:95px;overflow: hidden;text-overflow: ellipsis;">
									<div>{{shared.translate("BirthDay")}}</div>
								</th>
								<th class="sticky-top ae-thead-th " style="width:185px;">
									<div>{{shared.translate("Mobile")}}</div>
								</th>
								<th class="sticky-top ae-thead-th text-center" style="width:95px;overflow: hidden;text-overflow: ellipsis;">
									<div>{{shared.translate("EntityTypeId")}}</div>
								</th>
								<th class="sticky-top ae-thead-th text-center" style="width:95px;overflow: hidden;text-overflow: ellipsis;">
									<div>{{shared.translate("GenderId")}}</div>
								</th>
								<th class="sticky-top ae-thead-th text-center" style="width:95px;overflow: hidden;text-overflow: ellipsis;">
									<div>{{shared.translate("RecordStateId")}}</div>
								</th>
								<th class="sticky-top ae-thead-th"></th>
								<th style="width:40px;" class="sticky-top ae-thead-th text-center" data-ae-actions="DefaultRepo.BasePersons.DeleteByKey"></th>
							</tr>
						</thead>
						<tbody v-if="initialResponses[0].IsSucceeded===true">
							<tr v-for="i in initialResponses[0]['Result']['Master']">
								<td class="ae-table-td text-dark text-center" @click="openById({compPath:'/a.Components/BasePersons_UpdateByKey',recordKey:i.Id,refereshOnCallback:true,actionsAllowed:'DefaultRepo.BasePersons.UpdateByKey',fkToParent:''});">
									<div class="text-primary text-hover-success pointer">
										<i class="fa-solid fa-fw fa-edit"></i>
										<div class="pk font-monospace">{{i.Id}}</div>
									</div>
								</td>
								<td class="ae-table-td text-center">
									<img :src="'data:image/png;base64, '+i.Picture_FileBody_xs" v-if="shared.fixNull(i.Picture_FileBody_xs,'')!==''" class="rounded-4 shadow-sm" style="width:95%;min-height:50px;max-height:50px;max-width:50px;">
									<i class="fa-solid fa-fw fa-image fa-5x text-light" v-else=""></i>
								</td>
								<td class="ae-table-td">
									<div>
										<span class="badge text-muted fs-d7 text-start me-1">{{shared.translate("FirstName")}}</span>
										<span class="fw-bold">
											<span>{{shared.fixNull(i["FirstName"],'-')}}</span>
										</span>
									</div>
									<div>
										<span class="badge text-muted fs-d7 text-start me-1">{{shared.translate("LastName")}}</span>
										<span class="fw-bold">
											<span>{{shared.fixNull(i["LastName"],'-')}}</span>
										</span>
									</div>
								</td>
								<td class="ae-table-td text-center">
									<div class="text-dark fb">
										<div>{{shared.translate(i["UserId_UserName"])}}</div>
									</div>
									<div class="text-muted fs-d7">{{i["UserId"]}}</div>
								</td>
								<td class="ae-table-td text-center">
									<div>{{i["NationalCode"]}}</div>
								</td>
								<td class="ae-table-td text-center">
									<div>{{i["BirthYear"]}}</div>
								</td>
								<td class="ae-table-td text-center">
									<div>{{i["BirthMonth"]}}</div>
								</td>
								<td class="ae-table-td text-center">
									<div>{{i["BirthDay"]}}</div>
								</td>
								<td class="ae-table-td ">
									<div>{{i["Mobile"]}}</div>
								</td>
								<td class="ae-table-td text-center">
									<div>{{i["EntityTypeId"]}}</div>
								</td>
								<td class="ae-table-td text-center">
									<div>{{i["GenderId"]}}</div>
								</td>
								<td class="ae-table-td text-center">
									<div class="text-dark fb">
										<div>{{shared.translate(i["RecordStateId_Title"])}}</div>
									</div>
									<div class="text-muted fs-d7">{{i["RecordStateId"]}}</div>
								</td>
								<td></td>
								<td style="width:40px;vertical-align:middle" class="text-center text-secondary text-hover-danger pointer" data-ae-actions="DefaultRepo.BasePersons.DeleteByKey" @click="deleteById({pkValue:i.Id})">
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
_this.objectName = "BasePersons";
_this.loadMethod = "DefaultRepo.BasePersons.ReadList";
_this.filePrefix = "";
_this.deleteMethod = `${_this.dbConfName}.${_this.objectName}.DeleteByKey`;
_this.orderableColumns = ["CreatedOn","UpdatedOn"];
_this.orderClauses = [{ Name: "CreatedOn", OrderDirection: "ASC" }];
_this.initialResponses = [{ Duration: 0, Result: { Master: [], Aggregations: [{ "Count": 0 }] } }];
_this.initialRequests = [genListRequest(_this.loadMethod, {}, _this.orderClauses, { PageNumber: 1, PageSize: 10 })];
	_this.filter = {
		"FirstName": null, "LastName": null, "EntityTypeId": "", "Id": null, "CreatedBy": null, "UpdatedBy": null, "UserId": null,
		"GenderId": "", "NationalCode": null, "BirthYear": null, "BirthMonth": null, "BirthDay": null, "Mobile": null,
		"Picture_FileName": null, "Picture_FileSize": null, "Picture_FileMime": null, "RecordStateId": ""
	};
_this.initialSearchOptions = _.cloneDeep(_this.filter);
_this.clientQueryMetadata = {"ParentObjectColumns":[{"Name":"Id","DevNote":"","IsPrimaryKey":true,"DbType":"INT","IsIdentity":true,"IdentityStart":"1000000","IdentityStep":"1","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":true,"ValidationRule":":=i(0,2147483647)"}},{"Name":"CreatedBy","DevNote":"","DbType":"INT","UpdateGroup":"","UiProps":{"Group":"Auditing","UiWidget":"DisabledTextbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":true,"ValidationRule":":=i(0,2147483647)"}},{"Name":"CreatedOn","DevNote":"","DbType":"DATETIME","IsSortable":true,"UpdateGroup":"","UiProps":{"Group":"Auditing","UiWidget":"DisabledTextbox","UiWidgetOptions":"{}","IsDisabled":true,"Required":true,"ValidationRule":"dt(1900-01-01 00:01:00,2100-12-30 11:59:59)"}},{"Name":"UpdatedBy","DevNote":"","DbType":"INT","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"Auditing","UiWidget":"DisabledTextbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":false,"ValidationRule":":=i(0,2147483647)"}},{"Name":"UpdatedOn","DevNote":"","DbType":"DATETIME","AllowNull":true,"IsSortable":true,"UpdateGroup":"","UiProps":{"Group":"Auditing","UiWidget":"DisabledTextbox","UiWidgetOptions":"{}","IsDisabled":true,"Required":false,"ValidationRule":"dt(1900-01-01 00:01:00,2100-12-30 11:59:59)"}},{"Name":"UserId","DevNote":"","DbType":"INT","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=i(0,2147483647)"}},{"Name":"GenderId","DevNote":"","DbType":"INT","Fk":{"FkName":"BasePersons_GenderId_BaseInfo_Id","TargetTable":"BaseInfo","TargetColumn":"Id","Lookup":{"Id":"","Method":""},"JsLookupParentId":"100"},"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Combo","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true,"ValidationRule":":=i(0,2147483647)"}},{"Name":"NationalCode","DevNote":"","DbType":"VARCHAR","Size":"16","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,16)"}},{"Name":"FirstName","DevNote":"","DbType":"NVARCHAR","Size":"64","IsHumanId":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Fast","Required":true,"ValidationRule":":=s(0,64)"}},{"Name":"LastName","DevNote":"","DbType":"NVARCHAR","Size":"64","IsHumanId":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Fast","Required":true,"ValidationRule":":=s(0,64)"}},{"Name":"BirthYear","DevNote":"","DbType":"INT","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=i(0,2147483647)"}},{"Name":"BirthMonth","DevNote":"","DbType":"TINYINT","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false}},{"Name":"BirthDay","DevNote":"","DbType":"TINYINT","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false}},{"Name":"Mobile","DevNote":"","DbType":"VARCHAR","Size":"14","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true,"ValidationRule":":=s(0,14)"}},{"Name":"Picture_FileBody","DevNote":"","DbType":"IMAGE","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"ImageView","UiWidgetOptions":"{}","Required":false}},{"Name":"Picture_FileBody_xs","DevNote":"","DbType":"IMAGE","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"ImageView","UiWidgetOptions":"{}","Required":false}},{"Name":"Picture_FileName","DevNote":"","DbType":"NVARCHAR","Size":"128","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,128)"}},{"Name":"Picture_FileSize","DevNote":"","DbType":"INT","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=i(0,2147483647)"}},{"Name":"Picture_FileMime","DevNote":"","DbType":"VARCHAR","Size":"128","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,128)"}},{"Name":"EntityTypeId","DevNote":"","DbType":"INT","AllowNull":true,"Fk":{"FkName":"BasePersons_EntityTypeId_BaseInfo_Id","TargetTable":"BaseInfo","TargetColumn":"Id","Lookup":{"Id":"","Method":""},"JsLookupParentId":"102"},"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Combo","UiWidgetOptions":"{}","SearchType":"Fast","Required":false,"ValidationRule":":=i(0,2147483647)"}},{"Name":"RecordStateId","DevNote":"","DbType":"INT","AllowNull":true,"Fk":{"FkName":"BasePersons_RecordState_BaseInfo_Id","TargetTable":"BaseInfo","TargetColumn":"Id","Lookup":{"Id":"","Method":""},"JsLookupParentId":"101"},"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Combo","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=i(0,2147483647)"}}],"Name":"ReadList","Type":"ReadList","QueryColumns":["UpdatedBy","UpdatedOn","UserId","NationalCode","FirstName","LastName","BirthYear","BirthMonth","BirthDay","Mobile","Picture_FileBody_xs","Id","EntityTypeId","GenderId","RecordState"],"FastSearchColumns":[{"Name":"FirstName","DevNote":"","DbType":"NVARCHAR","Size":"64","IsHumanId":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Fast","Required":true,"ValidationRule":":=s(0,64)"}},{"Name":"LastName","DevNote":"","DbType":"NVARCHAR","Size":"64","IsHumanId":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Fast","Required":true,"ValidationRule":":=s(0,64)"}},{"Name":"EntityTypeId","DevNote":"","DbType":"INT","AllowNull":true,"Fk":{"FkName":"BasePersons_EntityTypeId_BaseInfo_Id","TargetTable":"BaseInfo","TargetColumn":"Id","Lookup":{"Id":"","Method":""},"JsLookupParentId":"102"},"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Combo","UiWidgetOptions":"{}","SearchType":"Fast","Required":false,"ValidationRule":":=i(0,2147483647)"}}],"ExpandableSearchColumns":[{"Name":"Id","DevNote":"","IsPrimaryKey":true,"DbType":"INT","IsIdentity":true,"IdentityStart":"1000000","IdentityStep":"1","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":true,"ValidationRule":":=i(0,2147483647)"}},{"Name":"CreatedBy","DevNote":"","DbType":"INT","UpdateGroup":"","UiProps":{"Group":"Auditing","UiWidget":"DisabledTextbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":true,"ValidationRule":":=i(0,2147483647)"}},{"Name":"UpdatedBy","DevNote":"","DbType":"INT","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"Auditing","UiWidget":"DisabledTextbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":false,"ValidationRule":":=i(0,2147483647)"}},{"Name":"UserId","DevNote":"","DbType":"INT","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=i(0,2147483647)"}},{"Name":"GenderId","DevNote":"","DbType":"INT","Fk":{"FkName":"BasePersons_GenderId_BaseInfo_Id","TargetTable":"BaseInfo","TargetColumn":"Id","Lookup":{"Id":"","Method":""},"JsLookupParentId":"100"},"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Combo","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true,"ValidationRule":":=i(0,2147483647)"}},{"Name":"NationalCode","DevNote":"","DbType":"VARCHAR","Size":"16","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,16)"}},{"Name":"BirthYear","DevNote":"","DbType":"INT","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=i(0,2147483647)"}},{"Name":"BirthMonth","DevNote":"","DbType":"TINYINT","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false}},{"Name":"BirthDay","DevNote":"","DbType":"TINYINT","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false}},{"Name":"Mobile","DevNote":"","DbType":"VARCHAR","Size":"14","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true,"ValidationRule":":=s(0,14)"}},{"Name":"Picture_FileName","DevNote":"","DbType":"NVARCHAR","Size":"128","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,128)"}},{"Name":"Picture_FileSize","DevNote":"","DbType":"INT","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=i(0,2147483647)"}},{"Name":"Picture_FileMime","DevNote":"","DbType":"VARCHAR","Size":"128","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,128)"}},{"Name":"RecordState","DevNote":"","DbType":"INT","AllowNull":true,"Fk":{"FkName":"BasePersons_RecordState_BaseInfo_Id","TargetTable":"BaseInfo","TargetColumn":"Id","Lookup":{"Id":"","Method":""},"JsLookupParentId":"101"},"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Combo","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=i(0,2147483647)"}}],"OptionalQueries":[]};


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