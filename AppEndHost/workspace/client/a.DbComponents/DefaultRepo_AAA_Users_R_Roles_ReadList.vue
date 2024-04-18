<template>
<div class="card h-100 bg-transparent rounded-0 border-0">
		<div class="card-header p-2 bg-light-subtle rounded-0 border-0">
			<div class="hstack gap-1">
				<button class="btn btn-sm border-0 btn-outline-primary px-2" @click="localCrudLoadRecords">
					<i class="fa-solid fa-search"></i>
				</button>
				<button type="button" class="btn btn-sm bg-hover-light" onclick="switchVisibility(this,'.simple-search','show','fa-chevron-down','fa-chevron-up')">
					<i class="fa-solid fa-chevron-down"></i>
				</button>
				<div class="p-0 ms-auto"></div>
				<div class="vr"></div>
				<button type="button" class="btn btn-sm border-0 btn-outline-success px-2" data-ae-actions="DefaultRepo.AAA_Users_R_Roles.Create" @click="localOpenCreate">
					<i class="fa-solid fa-file-alt fa-bounce pe-1" style="--fa-animation-iteration-count:1"></i>
					<span class="d-none d-md-inline-block d-lg-inline-block ms-1">{{shared.translate("Create")}}</span>
				</button>
				<button type="button" class="btn btn-sm border-0 btn-outline-success px-2" data-ae-actions="DefaultRepo.AAA_Users.Create" @click="localExportExcel">
					<i class="fa-solid fa-file-excel"></i>
					<span class="d-none d-md-inline-block d-lg-inline-block ms-1">{{shared.translate("Excel")}}</span>
				</button>
			</div>
		</div>
		<div class="simple-search card-header p-2 bg-transparent rounded-0 collapse">
			<div class="row">
				<div class="col-48 col-md-6">
					<select class="form-select" v-model="searchOptions.UserId" data-ae-validation-required="false">
						<option value="">{{shared.translate('UserId')}}</option>
						<option v-for="i in shared.getResponseObjectById(initialResponses,'UserId_Lookup')" :value="i['Id']">{{i.UserName}}</option>
					</select>
				</div>
				<div class="col-48 col-md-6">
					<select class="form-select" v-model="searchOptions.RoleId" data-ae-validation-required="false">
						<option value="">{{shared.translate('RoleId')}}</option>
						<option v-for="i in shared.getResponseObjectById(initialResponses,'RoleId_Lookup')" :value="i['Id']">{{i.RoleName}}</option>
					</select>
				</div>
				<div class="col-48 col-md-6">
					<input type="text" class="form-control" id="input_CreatedBy" @keyup.enter="localCrudLoadRecords" v-model="searchOptions.CreatedBy" :placeholder="shared.translate('CreatedBy')">
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
									<div>{{shared.translate("UserId")}}</div>
								</td>
								<td class="sticky-top ae-thead-td text-center" style="width:75px;overflow: hidden;text-overflow: ellipsis;">
									<div>{{shared.translate("RoleId")}}</div>
								</td>
								<td style="width:40px;" class="sticky-top ae-thead-td text-center" data-ae-actions="DefaultRepo.AAA_Users_R_Roles.DeleteByKey"></td>
							</tr>
						</thead>
						<tbody>
							<tr v-for="i in initialResponses[0]['Result']['Master']">
								<td class="ae-table-td text-dark text-center" style="width:75px;overflow: hidden;text-overflow: ellipsis;" @click="localCrudOpenById('/a.DbComponents/DefaultRepo_AAA_Users_R_Roles_UpdateByKey','modal-lg',i.Id,true,'DefaultRepo.AAA_Users_R_Roles.UpdateByKey','');">
									<div class="pointer text-primary hover-success">
										<i class="fa-solid fa-fw fa-edit"></i>
										<br>
										<span class="pk">{{i.Id}}</span>
									</div>
								</td>
								<td class="ae-table-td text-center" style="width:75px;overflow: hidden;text-overflow: ellipsis;">
									<div class="text-dark fb">
										<div>{{shared.translate(i["UserId_UserName"])}}</div>
										<div>{{shared.translate(i["UserId_Picture_FileName"])}}</div>
									</div>
									<div class="text-muted fs-d7">{{i["UserId"]}}</div>
								</td>
								<td class="ae-table-td text-center" style="width:75px;overflow: hidden;text-overflow: ellipsis;">
									<div class="text-dark fb">
										<div>{{shared.translate(i["RoleId_RoleName"])}}</div>
									</div>
									<div class="text-muted fs-d7">{{i["RoleId"]}}</div>
								</td>
								<td style="width:40px;vertical-align:middle" class="text-center" data-ae-actions="DefaultRepo.AAA_Users_R_Roles.DeleteByKey">
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
		<div class="card-footer bg-light-subtle rounded-0 border-0 p-0">
			<div class="input-group input-group-sm border-0 bg-transparent">
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
shared.setAppTitle(shared.translate("AAA_Users_R_Roles, ReadList"));
let _this = { cid: "", c: null, dbConfName: "", objectName: "", loadMethod: "", deleteMethod: "", initialRequests: [], initialResponses: [], pickerRequests: [], pickerHumanIds: [], searchOptions: {}, clientQueryMetadata: {}, orderClauses: [], orderableColumns: [] };
_this.dbConfName = "DefaultRepo";
_this.objectName = "AAA_Users_R_Roles";
_this.loadMethod = "DefaultRepo.AAA_Users_R_Roles.ReadList";
_this.deleteMethod = `${_this.dbConfName}.${_this.objectName}.DeleteByKey`;
_this.orderableColumns = ["Id","UserId","RoleId","CreatedOn"];
_this.orderClauses = [{ Name: "Id", OrderDirection: "ASC" }];
_this.initialResponses = [{ Duration: 0, Result: { Master: [], Aggregations: [{ "Count": 0 }] } }];
_this.initialRequests = [genListRequest(_this.loadMethod, {}, _this.orderClauses, { PageNumber: 1, PageSize: 10 })];
_this.searchOptions = {"Id":null,"UserId":"","RoleId":"","CreatedBy":null};
_this.clientQueryMetadata = {"ParentObjectColumns":[{"Name":"Id","DevNote":"","IsPrimaryKey":true,"DbType":"BIGINT","IsIdentity":true,"IdentityStart":"1000000","IdentityStep":"1","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":true,"ValidationRule":":=i(0,10000)"}},{"Name":"UserId","DevNote":"","DbType":"BIGINT","Fk":{"FkName":"AppEnd_Users_R_Roles_UserId_AppEnd_Users_Id","TargetTable":"AAA_Users","TargetColumn":"Id","EnforceRelation":true,"Lookup":{"Id":"UserId_Lookup","Method":"DefaultRepo.AAA_Users.ReadList","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.AAA_Users.ReadList","OrderClauses":[{"Name":"UserName","OrderDirection":"ASC"}],"Pagination":{"PageNumber":1,"PageSize":500},"ExceptAggregations":["Count"],"IncludeSubQueries":false}}}},"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Combo","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true,"ValidationRule":":=i(0,10000)"}},{"Name":"RoleId","DevNote":"","DbType":"INT","Fk":{"FkName":"AAA_Users_R_Roles_RoleId_AAA_Roles_Id","TargetTable":"AAA_Roles","TargetColumn":"Id","EnforceRelation":true,"Lookup":{"Id":"RoleId_Lookup","Method":"DefaultRepo.AAA_Roles.ReadList","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.AAA_Roles.ReadList","OrderClauses":[{"Name":"RoleName","OrderDirection":"ASC"}],"Pagination":{"PageNumber":1,"PageSize":500},"ExceptAggregations":["Count"],"IncludeSubQueries":false}}}},"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Combo","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true,"ValidationRule":":=i(0,10000)"}},{"Name":"CreatedBy","DevNote":"","DbType":"NVARCHAR","Size":"64","UpdateGroup":"","UiProps":{"Group":"Auditing","UiWidget":"DisabledTextbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":true}},{"Name":"CreatedOn","DevNote":"","DbType":"DATETIME","UpdateGroup":"","UiProps":{"Group":"Auditing","UiWidget":"DisabledTextbox","UiWidgetOptions":"{}","IsDisabled":true,"Required":true,"ValidationRule":"dt(1900-01-01 00:01:00,2100-12-30 11:59:59)"}}],"Name":"ReadList","Type":"ReadList","QueryColumns":["Id","UserId","RoleId","CreatedBy","CreatedOn"],"FastSearchColumns":[],"ExpandableSearchColumns":[{"Name":"Id","DevNote":"","IsPrimaryKey":true,"DbType":"BIGINT","IsIdentity":true,"IdentityStart":"1000000","IdentityStep":"1","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":true,"ValidationRule":":=i(0,10000)"}},{"Name":"UserId","DevNote":"","DbType":"BIGINT","Fk":{"FkName":"AppEnd_Users_R_Roles_UserId_AppEnd_Users_Id","TargetTable":"AAA_Users","TargetColumn":"Id","EnforceRelation":true,"Lookup":{"Id":"UserId_Lookup","Method":"DefaultRepo.AAA_Users.ReadList","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.AAA_Users.ReadList","OrderClauses":[{"Name":"UserName","OrderDirection":"ASC"}],"Pagination":{"PageNumber":1,"PageSize":500},"ExceptAggregations":["Count"],"IncludeSubQueries":false}}}},"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Combo","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true,"ValidationRule":":=i(0,10000)"}},{"Name":"RoleId","DevNote":"","DbType":"INT","Fk":{"FkName":"AAA_Users_R_Roles_RoleId_AAA_Roles_Id","TargetTable":"AAA_Roles","TargetColumn":"Id","EnforceRelation":true,"Lookup":{"Id":"RoleId_Lookup","Method":"DefaultRepo.AAA_Roles.ReadList","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.AAA_Roles.ReadList","OrderClauses":[{"Name":"RoleName","OrderDirection":"ASC"}],"Pagination":{"PageNumber":1,"PageSize":500},"ExceptAggregations":["Count"],"IncludeSubQueries":false}}}},"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Combo","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true,"ValidationRule":":=i(0,10000)"}},{"Name":"CreatedBy","DevNote":"","DbType":"NVARCHAR","Size":"64","UpdateGroup":"","UiProps":{"Group":"Auditing","UiWidget":"DisabledTextbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":true}}],"OptionalQueries":[]};




_this.initialRequests.push({"Id":"UserId_Lookup","Method":"DefaultRepo.AAA_Users.ReadList","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.AAA_Users.ReadList","OrderClauses":[{"Name":"UserName","OrderDirection":"ASC"}],"Pagination":{"PageNumber":1,"PageSize":500},"ExceptAggregations":["Count"],"IncludeSubQueries":false}}});

_this.initialRequests.push({"Id":"RoleId_Lookup","Method":"DefaultRepo.AAA_Roles.ReadList","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.AAA_Roles.ReadList","OrderClauses":[{"Name":"RoleName","OrderDirection":"ASC"}],"Pagination":{"PageNumber":1,"PageSize":500},"ExceptAggregations":["Count"],"IncludeSubQueries":false}}});


	export default {
	methods: {
		localOpenPicker(colName) { crudOpenPicker(_this, _this.c.searchOptions, colName); },
		localCrudLoadRecords() { crudLoadRecords(_this); },
		localExportExcel() { crudExportExcel(_this); },
		localCrudOpenById(compPath, modalSize, recordKey, refereshOnCallback, actionsAllowed) { crudOpenById(_this, compPath, modalSize, recordKey, refereshOnCallback, actionsAllowed); },
		localCrudDeleteRecord(recordKey) { crudDeleteRecord(_this, "Id", recordKey); },
		localOpenCreate() {crudOpenCreate(_this, `/a.DbComponents/${_this.dbConfName}_${_this.objectName}_Create`, 'modal-lg');}
	},
	setup(props) { _this.cid = props['cid']; },
	data() { return _this; },
	created() { _this.c = this; },
	mounted() { _this.c.localCrudLoadRecords(); },
	props: { cid: String }
}

</script>