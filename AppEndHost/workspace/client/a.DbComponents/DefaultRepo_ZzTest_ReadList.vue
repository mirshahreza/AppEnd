<template>
<div class="card h-100 bg-transparent rounded-0 border-0">
		<div class="card-header p-2 bg-success-subtle rounded-0 border-0">
			<div class="row">
				<div class="col-48 col-md-12">
					<div class="form-control form-control-sm pb-0">
						<div class="form-check form-check-inline" v-for="i in shared.getBiItemsByParentId(10000)">
							<input class="form-check-input" type="checkbox" v-model="searchOptions.GenderId" :value="i.Id" :id="i.Id+'GenderId_Lookup'">
							<label class="form-check-label" :for="i.Id+'GenderId_Lookup'">
								{{i.Title}}
							</label>
						</div>
					</div>
				</div>
				<div class="col-48 col-md-6">
					<div class="form-control form-control-sm text-nowrap pointer data-ae-validation" data-ae-widget="nullableCheckbox" data-ae-widget-options="{&quot;shownull&quot;:true}">
						<i class="fa-solid fa-fw me-1"></i>
						<span>{{shared.translate('IsProgrammable')}}</span>
						<input type="hidden" v-model="searchOptions.IsProgrammable" data-ae-validation-required="false">
					</div>
				</div>
				<div class="col-48 col-md-6">
					<input type="text" class="form-control form-control-sm" id="input_Title" @keyup.enter="loadRecords" v-model="searchOptions.Title" :placeholder="shared.translate('Title')">
				</div>
			</div>
		</div>
		<div class="card-header simple-search p-2 bg-transparent rounded-0 collapse">
			<div class="row">
				<div class="col-48 col-md-6">
					<input type="text" class="form-control form-control-sm" id="input_Id" @keyup.enter="loadRecords" v-model="searchOptions.Id" :placeholder="shared.translate('Id')">
				</div>
			</div>
			<div class="row">
			</div>
			<div class="row">
				<div class="col-48 col-md-24">
					<input type="text" class="form-control form-control-sm" id="input_CreatedOn" @keyup.enter="loadRecords" v-model="searchOptions.CreatedOn__startof" :placeholder="shared.translate('CreatedOn')">
				</div>
				<div class="col-48 col-md-24">
					<input type="text" class="form-control form-control-sm" id="input_CreatedOn" @keyup.enter="loadRecords" v-model="searchOptions.CreatedOn__endof" :placeholder="shared.translate('CreatedOn')">
				</div>
			</div>
		</div>
		<div class="card-header p-2 rounded-0">
			<div class="hstack gap-1">
				<button class="btn btn-sm btn-outline-primary px-3" @click="loadRecords">
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
				<button type="button" class="btn btn-sm border-0 btn-outline-success px-2" data-ae-actions="DefaultRepo.ZzTest.Create" @click="openCreate()">
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
								<th class="sticky-top ae-thead-th fb text-success" style="width:185px;">
									<div>{{shared.translate("HumanIds")}}</div>
								</th>
								<th class="sticky-top ae-thead-th"></th>
								<th style="width:40px;" class="sticky-top ae-thead-th text-center" data-ae-actions="DefaultRepo.ZzTest.DeleteByKey"></th>
							</tr>
						</thead>
						<tbody v-if="initialResponses[0].IsSucceeded===true">
							<tr v-for="i in initialResponses[0]['Result']['Master']">
								<td class="ae-table-td text-dark text-center" style="" @click="openById({compPath:'/a.DbComponents/DefaultRepo_ZzTest_UpdateByKey',recordKey:i.Id,refereshOnCallback:true,actionsAllowed:'DefaultRepo.ZzTest.UpdateByKey',fkToParent:''});">
									<div class="text-primary text-hover-success pointer">
										<i class="fa-solid fa-fw fa-edit"></i>
										<br>
										<span class="pk">{{i.Id}}</span>
									</div>
								</td>
								<td class="ae-table-td" style="">
									<div>
										<span class="badge text-muted fs-d8 text-start" style="min-width:85px;">{{shared.translate("Title")}}</span>
										<span class="fw-bold">
											<span>{{shared.fixNull(i["Title"],'-')}}</span>
										</span>
									</div>
								</td>
								<td></td>
								<td style="width:40px;vertical-align:middle" class="text-center text-secondary text-hover-danger pointer" data-ae-actions="DefaultRepo.ZzTest.DeleteByKey" @click="deleteById({pkValue:i.Id})">
									<i class="fa-solid fa-fw fa-times"></i>
								</td>
							</tr>
						</tbody>
					</table>
				</div>
			</div>
		</div>
		<div class="card-footer rounded-0 border-0 border-top border-top-1 border-warning-subtle p-0 bg-white">
			<div class="input-group input-group-sm border-0 bg-white">
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
shared.setAppTitle(shared.translate("ZzTest, ReadList"));
let _this = { cid: "", c: null, templateType:"ReadList", dbConfName: "", objectName: "", loadMethod: "", deleteMethod: "", initialRequests: [], initialResponses: [], pickerRequests: [], pickerHumanIds: [], searchOptions: {}, clientQueryMetadata: {}, orderClauses: [], orderableColumns: [] };
_this.dbConfName = "DefaultRepo";
_this.objectName = "ZzTest";
_this.loadMethod = "DefaultRepo.ZzTest.ReadList";
_this.deleteMethod = `${_this.dbConfName}.${_this.objectName}.DeleteByKey`;
_this.orderableColumns = ["Title","CreatedOn","UpdatedOn"];
_this.orderClauses = [{ Name: "Title", OrderDirection: "ASC" }];
_this.initialResponses = [{ Duration: 0, Result: { Master: [], Aggregations: [{ "Count": 0 }] } }];
_this.initialRequests = [genListRequest(_this.loadMethod, {}, _this.orderClauses, { PageNumber: 1, PageSize: 10 })];
_this.searchOptions = {"Title":null,"GenderId":[],"IsProgrammable":null,"Id":null,"CreatedOn":null};
_this.initialSearchOptions = _.cloneDeep(_this.searchOptions);
_this.clientQueryMetadata = {"ParentObjectColumns":[{"Name":"Id","DevNote":"","IsPrimaryKey":true,"DbType":"INT","IsIdentity":true,"IdentityStart":"1","IdentityStep":"1","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":true,"ValidationRule":":=i(0,2147483647)"}},{"Name":"Title","DevNote":"","DbType":"NVARCHAR","Size":"32","AllowNull":true,"IsHumanId":true,"IsSortable":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Fast","Required":false,"ValidationRule":":=s(0,32)"}},{"Name":"CreatedBy","DevNote":"","DbType":"INT","UpdateGroup":"","UiProps":{"Group":"Auditing","UiWidget":"DisabledTextbox","UiWidgetOptions":"{}","IsDisabled":true,"Required":true,"ValidationRule":":=i(0,2147483647)"}},{"Name":"CreatedOn","DevNote":"","DbType":"DATETIME","IsSortable":true,"UpdateGroup":"","UiProps":{"Group":"Auditing","UiWidget":"DateTimePicker","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":true,"ValidationRule":"dt(1900-01-01 00:01:00,2100-12-30 11:59:59)"}},{"Name":"UpdatedBy","DevNote":"","DbType":"INT","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"Auditing","UiWidget":"DisabledTextbox","UiWidgetOptions":"{}","IsDisabled":true,"Required":false,"ValidationRule":":=i(0,2147483647)"}},{"Name":"UpdatedOn","DevNote":"","DbType":"DATETIME","AllowNull":true,"IsSortable":true,"UpdateGroup":"","UiProps":{"Group":"Auditing","UiWidget":"DisabledTextbox","UiWidgetOptions":"{}","IsDisabled":true,"Required":false,"ValidationRule":"dt(1900-01-01 00:01:00,2100-12-30 11:59:59)"}},{"Name":"GenderId","DevNote":"","DbType":"INT","AllowNull":true,"Fk":{"FkName":"ZzTest_GenderId_Common_BaseInfo_Id","TargetTable":"Common_BaseInfo","TargetColumn":"Id","Lookup":{"Id":"GenderId_Lookup","Method":"DefaultRepo.Common_BaseInfo.ReadList","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.Common_BaseInfo.ReadList","Where":{"CompareClauses":[{"Name":"ParentId","Value":10000,"ClauseOperator":"Equal"}]},"OrderClauses":[{"Name":"ViewOrder","OrderDirection":"ASC"}],"Pagination":{"PageNumber":1,"PageSize":500},"IncludeSubQueries":false}}},"JsLookupParentId":"10000"},"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Combo","UiWidgetOptions":"{}","SearchType":"Fast","SearchMultiselect":true,"Required":false,"ValidationRule":":=i(0,2147483647)"}},{"Name":"IsProgrammable","DevNote":"","DbType":"BIT","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Checkbox","UiWidgetOptions":"{\n    \u0022shownull\u0022: true,\n    \u0022nullClasses\u0022: \u0022fa-minus text-secondary\u0022,\n    \u0022trueClasses\u0022: \u0022fa-check text-success\u0022,\n    \u0022falseClasses\u0022: \u0022fa-xmark text-danger\u0022\n}","SearchType":"Fast","Required":false}}],"Name":"ReadList","Type":"ReadList","QueryColumns":["Id","Title","CreatedBy","CreatedOn","UpdatedBy","UpdatedOn"],"FastSearchColumns":[{"Name":"Title","DevNote":"","DbType":"NVARCHAR","Size":"32","AllowNull":true,"IsHumanId":true,"IsSortable":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Fast","Required":false,"ValidationRule":":=s(0,32)"}},{"Name":"GenderId","DevNote":"","DbType":"INT","AllowNull":true,"Fk":{"FkName":"ZzTest_GenderId_Common_BaseInfo_Id","TargetTable":"Common_BaseInfo","TargetColumn":"Id","Lookup":{"Id":"GenderId_Lookup","Method":"DefaultRepo.Common_BaseInfo.ReadList","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.Common_BaseInfo.ReadList","Where":{"CompareClauses":[{"Name":"ParentId","Value":10000,"ClauseOperator":"Equal"}]},"OrderClauses":[{"Name":"ViewOrder","OrderDirection":"ASC"}],"Pagination":{"PageNumber":1,"PageSize":500},"IncludeSubQueries":false}}},"JsLookupParentId":"10000"},"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Combo","UiWidgetOptions":"{}","SearchType":"Fast","SearchMultiselect":true,"Required":false,"ValidationRule":":=i(0,2147483647)"}},{"Name":"IsProgrammable","DevNote":"","DbType":"BIT","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Checkbox","UiWidgetOptions":"{\n    \u0022shownull\u0022: true,\n    \u0022nullClasses\u0022: \u0022fa-minus text-secondary\u0022,\n    \u0022trueClasses\u0022: \u0022fa-check text-success\u0022,\n    \u0022falseClasses\u0022: \u0022fa-xmark text-danger\u0022\n}","SearchType":"Fast","Required":false}}],"ExpandableSearchColumns":[{"Name":"Id","DevNote":"","IsPrimaryKey":true,"DbType":"INT","IsIdentity":true,"IdentityStart":"1","IdentityStep":"1","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":true,"ValidationRule":":=i(0,2147483647)"}},{"Name":"CreatedOn","DevNote":"","DbType":"DATETIME","IsSortable":true,"UpdateGroup":"","UiProps":{"Group":"Auditing","UiWidget":"DateTimePicker","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":true,"ValidationRule":"dt(1900-01-01 00:01:00,2100-12-30 11:59:59)"}}],"OptionalQueries":[]};


export default {
	methods: {
	},
	setup(props) { _this.cid = props['cid']; },
	data() { return _this; },
	created() { _this.c = this; assignDefaultMethods(_this); },
	mounted() { _this.c.loadRecords(); },
	props: { cid: String, ismodal: String }
}

</script>