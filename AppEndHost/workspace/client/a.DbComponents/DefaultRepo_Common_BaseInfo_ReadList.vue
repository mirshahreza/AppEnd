<template>
<div class="card h-100 bg-transparent rounded-0 border-0">
		<div class="card-header p-2 bg-success-subtle rounded-0 border-0">
			<div class="hstack gap-1">
				<button class="btn btn-sm border-0 btn-outline-primary px-2" @click="localCrudLoadRecords">
					<i class="fa-solid fa-search"></i>
				</button>
				<button type="button" class="btn btn-sm bg-hover-light" onclick="switchVisibility(this,'.simple-search','show','fa-chevron-down','fa-chevron-up')">
					<i class="fa-solid fa-chevron-down"></i>
				</button>
				<div class="p-0 ms-auto"></div>
				<div class="vr"></div>
				<button type="button" class="btn btn-sm border-0 btn-outline-success px-2" data-ae-actions="DefaultRepo.Common_BaseInfo.Create" @click="localOpenCreate">
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
					<select class="form-select" v-model="searchOptions.ParentId" data-ae-validation-required="false">
						<option value="">{{shared.translate('ParentId')}}</option>
						<option v-for="i in shared.getResponseObjectById(initialResponses,'ParentId_Lookup')" :value="i['Id']">{{i.Title}} {{i.ShortName}}</option>
					</select>
				</div>
				<div class="col-48 col-md-6">
					<div class="form-control pointer data-ae-validation" data-ae-widget="nullableCheckbox" data-ae-widget-options="{&quot;shownull&quot;:true}">
						<i class="fa-solid fa-fw me-1"></i>
						<span>{{shared.translate('IsActive')}}</span>
						<input type="hidden" v-model="searchOptions.IsActive" data-ae-validation-required="false">
					</div>
				</div>
				<div class="col-48 col-md-6">
					<input type="text" class="form-control" id="input_Title" @keyup.enter="localCrudLoadRecords" v-model="searchOptions.Title" :placeholder="shared.translate('Title')">
				</div>
				<div class="col-48 col-md-6">
					<input type="text" class="form-control" id="input_ShortName" @keyup.enter="localCrudLoadRecords" v-model="searchOptions.ShortName" :placeholder="shared.translate('ShortName')">
				</div>
				<div class="col-48 col-md-6">
					<input type="text" class="form-control" id="input_ViewOrder" @keyup.enter="localCrudLoadRecords" v-model="searchOptions.ViewOrder" :placeholder="shared.translate('ViewOrder')">
				</div>
				<div class="col-48 col-md-6">
					<input type="text" class="form-control" id="input_Note" @keyup.enter="localCrudLoadRecords" v-model="searchOptions.Note" :placeholder="shared.translate('Note')">
				</div>
				<div class="col-48 col-md-6">
					<input type="text" class="form-control" id="input_UiColor" @keyup.enter="localCrudLoadRecords" v-model="searchOptions.UiColor" :placeholder="shared.translate('UiColor')">
				</div>
				<div class="col-48 col-md-6">
					<input type="text" class="form-control" id="input_UiIcon" @keyup.enter="localCrudLoadRecords" v-model="searchOptions.UiIcon" :placeholder="shared.translate('UiIcon')">
				</div>
				<div class="col-48 col-md-6">
					<input type="text" class="form-control" id="input_Metadata" @keyup.enter="localCrudLoadRecords" v-model="searchOptions.Metadata" :placeholder="shared.translate('Metadata')">
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
								<td class="sticky-top ae-thead-td fb text-success" style="min-width:185px;">
									<div>{{shared.translate("HumanIds")}}</div>
								</td>
								<td class="sticky-top ae-thead-td text-center" style="width:75px;overflow: hidden;text-overflow: ellipsis;">
									<div>{{shared.translate("ParentId")}}</div>
								</td>
								<td class="sticky-top ae-thead-td text-center" style="width:75px;overflow: hidden;text-overflow: ellipsis;">
									<div>{{shared.translate("ViewOrder")}}</div>
								</td>
								<td class="sticky-top ae-thead-td " style="min-width:185px;">
									<div>{{shared.translate("UiHints")}}</div>
								</td>
								<td class="sticky-top ae-thead-td " style="min-width:185px;">
									<div>{{shared.translate("IsActiveUpdate")}}</div>
								</td>
								<td style="width:40px;" class="sticky-top ae-thead-td text-center" data-ae-actions="DefaultRepo.Common_BaseInfo.DeleteByKey"></td>
							</tr>
						</thead>
						<tbody>
							<tr v-for="i in initialResponses[0]['Result']['Master']">
								<td class="ae-table-td text-dark text-center" style="width:75px;overflow: hidden;text-overflow: ellipsis;" @click="localCrudOpenById('/a.DbComponents/DefaultRepo_Common_BaseInfo_UpdateByKey','modal-lg',i.Id,true,'DefaultRepo.Common_BaseInfo.UpdateByKey','ParentId');">
									<div class="pointer text-primary hover-success">
										<i class="fa-solid fa-fw fa-edit"></i>
										<br>
										<span class="pk">{{i.Id}}</span>
									</div>
								</td>
								<td class="ae-table-td" style="min-width:185px;">
									<div>
										<span class="text-muted fs-d9">{{shared.translate("Title")}} :
										</span>
										<span>{{shared.fixNull(i["Title"],'-')}}</span>
									</div>
									<div>
										<span class="text-muted fs-d9">{{shared.translate("ShortName")}} :
										</span>
										<span>{{shared.fixNull(i["ShortName"],'-')}}</span>
									</div>
								</td>
								<td class="ae-table-td text-center" style="width:75px;overflow: hidden;text-overflow: ellipsis;">
									<div>{{i["ParentId"]}}</div>
								</td>
								<td class="ae-table-td text-center" style="width:75px;overflow: hidden;text-overflow: ellipsis;">
									<div>{{i["ViewOrder"]}}</div>
								</td>
								<td class="ae-table-td  " style="min-width:185px;">
									<div class="input-group input-group-sm bg-hover-primary rounded-2 p-2">
										<div class="more-info" style="">
											<table class="w-100 h-100 fs-d7">
												<tbody>
													<tr>
														<td class="text-muted align-middle">{{shared.translate("UiColor")}}</td>
														<td class="text-dark fb align-middle">
															<span class="fw-bold">{{shared.fixNull(i["UiColor"],'-')}}</span>
														</td>
													</tr>
													<tr>
														<td class="text-muted align-middle">{{shared.translate("UiIcon")}}</td>
														<td class="text-dark fb align-middle">
															<span class="fw-bold">{{shared.fixNull(i["UiIcon"],'-')}}</span>
														</td>
													</tr>
												</tbody>
											</table>
										</div>
									</div>
								</td>
								<td class="ae-table-td   pointer" style="min-width:185px;" @click="localCrudOpenById('/a.DbComponents/DefaultRepo_Common_BaseInfo_IsActiveUpdate','modal-md',i.Id,true,'DefaultRepo.Common_BaseInfo.IsActiveUpdate','ParentId');">
									<div class="input-group input-group-sm bg-hover-primary rounded-2 p-2">
										<div class="input-group-text rounded-2 me-1">
											<span v-html="shared.convertBoolToIconWithOptions(i.IsActive ,{})"></span>
										</div>
										<div class="more-info" style="">
											<table class="w-100 h-100 fs-d7">
											</table>
										</div>
									</div>
								</td>
								<td style="width:40px;vertical-align:middle" class="text-center" data-ae-actions="DefaultRepo.Common_BaseInfo.DeleteByKey">
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
shared.setAppTitle(shared.translate("Common_BaseInfo, ReadList"));
let _this = { cid: "", c: null, dbConfName: "", objectName: "", loadMethod: "", deleteMethod: "", initialRequests: [], initialResponses: [], pickerRequests: [], pickerHumanIds: [], searchOptions: {}, clientQueryMetadata: {}, orderClauses: [], orderableColumns: [] };
_this.dbConfName = "DefaultRepo";
_this.objectName = "Common_BaseInfo";
_this.loadMethod = "DefaultRepo.Common_BaseInfo.ReadList";
_this.deleteMethod = `${_this.dbConfName}.${_this.objectName}.DeleteByKey`;
_this.orderableColumns = ["Id","ParentId","Title","ShortName","ViewOrder","CreatedBy","UpdatedBy","MetaInfoUpdatedBy","IsActiveUpdatedBy","CreatedOn","UpdatedOn"];
_this.orderClauses = [{ Name: "Id", OrderDirection: "ASC" }];
_this.initialResponses = [{ Duration: 0, Result: { Master: [], Aggregations: [{ "Count": 0 }] } }];
_this.initialRequests = [genListRequest(_this.loadMethod, {}, _this.orderClauses, { PageNumber: 1, PageSize: 10 })];
_this.searchOptions = {"Id":null,"ParentId":"","Title":null,"ShortName":null,"ViewOrder":null,"Note":null,"IsActive":null,"UiColor":null,"UiIcon":null,"Metadata":null,"CreatedBy":null,"UpdatedBy":null};
_this.clientQueryMetadata = {"ParentObjectColumns":[{"Name":"Id","DevNote":"","IsPrimaryKey":true,"DbType":"INT","IsIdentity":true,"IdentityStart":"10000","IdentityStep":"1","UpdateGroup":"IsActiveUpdate","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":true,"ValidationRule":":=i(0,10000)"}},{"Name":"ParentId","DevNote":"","DbType":"INT","AllowNull":true,"Fk":{"FkName":"Common_BaseInfo_ParentId_Common_BaseInfo_Id","TargetTable":"Common_BaseInfo","TargetColumn":"Id","EnforceRelation":true,"Lookup":{"Id":"ParentId_Lookup","Method":"DefaultRepo.Common_BaseInfo.ReadList","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.Common_BaseInfo.ReadList","OrderClauses":[{"Name":"ViewOrder","OrderDirection":"ASC"}],"Pagination":{"PageNumber":1,"PageSize":500},"IncludeSubQueries":false}}}},"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Combo","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=i(0,10000)"}},{"Name":"Title","DevNote":"","DbType":"NVARCHAR","Size":"128","IsHumanId":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true}},{"Name":"ShortName","DevNote":"","DbType":"NVARCHAR","Size":"16","AllowNull":true,"IsHumanId":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false}},{"Name":"ViewOrder","DevNote":"","DbType":"FLOAT","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=i(0,10000)"}},{"Name":"Note","DevNote":"","DbType":"NVARCHAR","Size":"256","AllowNull":true,"UpdateGroup":"MetaInfoUpdate","UiProps":{"Group":"","UiWidget":"MultilineTextbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false}},{"Name":"IsActive","DevNote":"","DbType":"BIT","AllowNull":true,"UpdateGroup":"IsActiveUpdate","UiProps":{"Group":"","UiWidget":"Checkbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false}},{"Name":"UiColor","DevNote":"","DbType":"VARCHAR","Size":"16","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"UiHints","UiWidget":"ColorPicker","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false}},{"Name":"UiIcon","DevNote":"","DbType":"VARCHAR","Size":"64","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"UiHints","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false}},{"Name":"Metadata","DevNote":"","DbType":"NVARCHAR","Size":"4000","AllowNull":true,"UpdateGroup":"MetaInfoUpdate","UiProps":{"Group":"","UiWidget":"CodeEditorbox","UiWidgetOptions":"{\n    \u0022mode\u0022: \u0022ace/mode/json\u0022\n}","SearchType":"Expandable","Required":false}},{"Name":"CreatedBy","DevNote":"","DbType":"INT","UpdateGroup":"","UiProps":{"Group":"Auditing","UiWidget":"DisabledTextbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":true,"ValidationRule":":=i(0,10000)"}},{"Name":"CreatedOn","DevNote":"","DbType":"DATETIME","UpdateGroup":"","UiProps":{"Group":"Auditing","UiWidget":"DisabledTextbox","UiWidgetOptions":"{}","IsDisabled":true,"Required":true,"ValidationRule":"dt(1900-01-01 00:01:00,2100-12-30 11:59:59)"}},{"Name":"UpdatedBy","DevNote":"","DbType":"INT","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"Auditing","UiWidget":"DisabledTextbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":false,"ValidationRule":":=i(0,10000)"}},{"Name":"UpdatedOn","DevNote":"","DbType":"DATETIME","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"Auditing","UiWidget":"DisabledTextbox","UiWidgetOptions":"{}","IsDisabled":true,"Required":false,"ValidationRule":"dt(1900-01-01 00:01:00,2100-12-30 11:59:59)"}},{"Name":"MetaInfoUpdatedBy","DevNote":"","DbType":"INT","AllowNull":true,"UpdateGroup":"MetaInfoUpdate","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}"}},{"Name":"MetaInfoUpdatedOn","DevNote":"","DbType":"DATETIME","AllowNull":true,"UpdateGroup":"MetaInfoUpdate","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}"}},{"Name":"IsActiveUpdatedBy","DevNote":"","DbType":"INT","AllowNull":true,"UpdateGroup":"IsActiveUpdate","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}"}},{"Name":"IsActiveUpdatedOn","DevNote":"","DbType":"DATETIME","AllowNull":true,"UpdateGroup":"IsActiveUpdate","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}"}}],"Name":"ReadList","Type":"ReadList","QueryColumns":["Id","ParentId","Title","ShortName","ViewOrder","IsActive","UiColor","UiIcon","CreatedBy","CreatedOn","UpdatedBy","UpdatedOn"],"FastSearchColumns":[],"ExpandableSearchColumns":[{"Name":"Id","DevNote":"","IsPrimaryKey":true,"DbType":"INT","IsIdentity":true,"IdentityStart":"10000","IdentityStep":"1","UpdateGroup":"IsActiveUpdate","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":true,"ValidationRule":":=i(0,10000)"}},{"Name":"ParentId","DevNote":"","DbType":"INT","AllowNull":true,"Fk":{"FkName":"Common_BaseInfo_ParentId_Common_BaseInfo_Id","TargetTable":"Common_BaseInfo","TargetColumn":"Id","EnforceRelation":true,"Lookup":{"Id":"ParentId_Lookup","Method":"DefaultRepo.Common_BaseInfo.ReadList","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.Common_BaseInfo.ReadList","OrderClauses":[{"Name":"ViewOrder","OrderDirection":"ASC"}],"Pagination":{"PageNumber":1,"PageSize":500},"IncludeSubQueries":false}}}},"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Combo","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=i(0,10000)"}},{"Name":"Title","DevNote":"","DbType":"NVARCHAR","Size":"128","IsHumanId":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true}},{"Name":"ShortName","DevNote":"","DbType":"NVARCHAR","Size":"16","AllowNull":true,"IsHumanId":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false}},{"Name":"ViewOrder","DevNote":"","DbType":"FLOAT","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=i(0,10000)"}},{"Name":"Note","DevNote":"","DbType":"NVARCHAR","Size":"256","AllowNull":true,"UpdateGroup":"MetaInfoUpdate","UiProps":{"Group":"","UiWidget":"MultilineTextbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false}},{"Name":"IsActive","DevNote":"","DbType":"BIT","AllowNull":true,"UpdateGroup":"IsActiveUpdate","UiProps":{"Group":"","UiWidget":"Checkbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false}},{"Name":"UiColor","DevNote":"","DbType":"VARCHAR","Size":"16","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"UiHints","UiWidget":"ColorPicker","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false}},{"Name":"UiIcon","DevNote":"","DbType":"VARCHAR","Size":"64","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"UiHints","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false}},{"Name":"Metadata","DevNote":"","DbType":"NVARCHAR","Size":"4000","AllowNull":true,"UpdateGroup":"MetaInfoUpdate","UiProps":{"Group":"","UiWidget":"CodeEditorbox","UiWidgetOptions":"{\n    \u0022mode\u0022: \u0022ace/mode/json\u0022\n}","SearchType":"Expandable","Required":false}},{"Name":"CreatedBy","DevNote":"","DbType":"INT","UpdateGroup":"","UiProps":{"Group":"Auditing","UiWidget":"DisabledTextbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":true,"ValidationRule":":=i(0,10000)"}},{"Name":"UpdatedBy","DevNote":"","DbType":"INT","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"Auditing","UiWidget":"DisabledTextbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":false,"ValidationRule":":=i(0,10000)"}}],"OptionalQueries":[]};




_this.initialRequests.push({"Id":"ParentId_Lookup","Method":"DefaultRepo.Common_BaseInfo.ReadList","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.Common_BaseInfo.ReadList","OrderClauses":[{"Name":"ViewOrder","OrderDirection":"ASC"}],"Pagination":{"PageNumber":1,"PageSize":500},"IncludeSubQueries":false}}});


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