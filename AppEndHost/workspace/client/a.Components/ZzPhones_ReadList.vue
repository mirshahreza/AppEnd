<template>
<div class="card h-100 bg-transparent rounded-0 border-0">
		<div class="card-header p-2 bg-success-subtle rounded-0 border-0">
			<div class="row">
			</div>
		</div>
		<div class="card-header simple-search p-2 bg-transparent rounded-0 collapse">
			<div class="row">
				<div class="col-48 col-md-6">
					<select class="form-select form-select-sm" v-model="filter.PhoneType" data-ae-validation-required="false">
						<option value="">{{shared.translate('PhoneType')}}</option>
						<option v-for="i in shared.enum(10015)" :value="i['Id']">{{i.Title}}</option>
					</select>
				</div>
				<div class="col-48 col-md-6">
					<div class="form-control form-control-sm py-0 data-ae-validation">
						<div class="input-group input-group-sm p-0 pt-1" data-ae-widget="objectPicker">
							<input type="hidden" v-model="filter.PersonId">
							<input type="hidden" v-model="filter.PersonId_FirstName">
							<input type="hidden" v-model="filter.PersonId_LastName">
							<input type="text" class="form-control bg-transparent p-0 m-0 border-0" :value="shared.fixNull(filter.PersonId+' '+filter.PersonId_FirstName+' '+filter.PersonId_LastName,'',true)" :placeholder="shared.translate('PersonId')" disabled="">
							<span></span>
							<button class="btn btn-sm btn-outline-secondary bg-transparent p-0 m-0 me-1 border-0 text-hover-primary ae-objectpicker-search" type="button" @click="openPicker({colName:'PersonId'})">
								<i class="fa-solid fa-hand-pointer"></i>
							</button>
							<button class="btn btn-sm btn-outline-secondary bg-transparent p-0 m-0 ms-1 border-0 text-hover-danger ae-objectpicker-clear" type="button">
								<i class="fa-solid fa-times"></i>
							</button>
						</div>
					</div>
				</div>
				<div class="col-48 col-md-6">
					<input type="text" class="form-control form-control-sm" id="input_PhoneNumber" @keyup.enter="loadRecords()" v-model="filter.PhoneNumber" :placeholder="shared.translate('PhoneNumber')">
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
		<div class="card-header p-2 rounded-0">
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
				<button type="button" class="btn btn-sm border-0 btn-outline-success px-2" data-ae-actions="DefaultRepo.ZzPhones.Create" @click="openCreate()">
					<i class="fa-solid fa-file-alt fa-bounce pe-1" style="--fa-animation-iteration-count:1"></i>
					<span class="ms-1">{{shared.translate("Create")}}</span>
				</button>
				<div class="vr"></div>
				<div class="btn btn-sm border-0 btn-outline-success px-2" data-ae-actions="DefaultRepo.ZzPhones.ReadList" @click="exportExcel">
					<i class="fa-solid fa-file-excel pe-1"></i>
					<span class="ms-1">{{shared.translate("Export")}}</span>
				</div>
			</div>
		</div>
		<div class="card-body p-0">
			<div class="card h-100 border-light bg-light bg-opacity-75 border-0">
				<div class="card-body border-0 p-0 scrollable">
					<table class="table table-sm table-hover table-striped table-bordered w-100 ae-table m-0 fs-d8">
						<thead>
							<tr class="d-none d-md-table-row d-lg-table-row d-xl-table-row">
								<th class="sticky-top ae-thead-th fb text-primary fw-bold text-center" style="width:75px;overflow: hidden;text-overflow: ellipsis;">
									<i class="fa-solid fa-fw fa-window-restore"></i>
								</th>
								<th class="sticky-top ae-thead-th text-center" style="width:75px;overflow: hidden;text-overflow: ellipsis;">
									<div>{{shared.translate("PhoneType")}}</div>
								</th>
								<th class="sticky-top ae-thead-th text-center" style="width:185px;">
									<div>{{shared.translate("PhoneNumber")}}</div>
								</th>
								<th class="sticky-top ae-thead-th text-center" style="width:75px;overflow: hidden;text-overflow: ellipsis;">
									<div>{{shared.translate("PersonId")}}</div>
								</th>
								<th class="sticky-top ae-thead-th"></th>
								<th style="width:40px;" class="sticky-top ae-thead-th text-center" data-ae-actions="DefaultRepo.ZzPhones.DeleteByKey"></th>
							</tr>
						</thead>
						<tbody v-if="initialResponses[0].IsSucceeded===true">
							<tr v-for="i in initialResponses[0]['Result']['Master']">
								<td class="ae-table-td text-dark text-center" style="" @click="openById({compPath:'/a.Components/ZzPhones_UpdateByKey',recordKey:i.Id,refereshOnCallback:true,actionsAllowed:'DefaultRepo.ZzPhones.UpdateByKey',fkToParent:''});">
									<div class="text-primary text-hover-success pointer">
										<i class="fa-solid fa-fw fa-edit"></i>
										<br>
										<span class="pk">{{i.Id}}</span>
									</div>
								</td>
								<td class="ae-table-td text-center" style="">
									<div class="text-dark fb">
										<div>{{shared.translate(i["PhoneType_Title"])}}</div>
									</div>
									<div class="text-muted fs-d7">{{i["PhoneType"]}}</div>
								</td>
								<td class="ae-table-td text-center" style="">
									<div>{{i["PhoneNumber"]}}</div>
								</td>
								<td class="ae-table-td text-center" style="">
									<div class="text-dark fb">
										<div>{{shared.translate(i["PersonId_FirstName"])}}</div>
										<div>{{shared.translate(i["PersonId_LastName"])}}</div>
									</div>
									<div class="text-muted fs-d7">{{i["PersonId"]}}</div>
								</td>
								<td></td>
								<td style="width:40px;vertical-align:middle" class="text-center text-secondary text-hover-danger pointer" data-ae-actions="DefaultRepo.ZzPhones.DeleteByKey" @click="deleteById({pkValue:i.Id})">
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
shared.setAppTitle("$auto$");
let _this = { cid: "", c: null, templateType: "ReadList", filePrefix: "", dbConfName: "", objectName: "", loadMethod: "", deleteMethod: "", initialRequests: [], initialResponses: [], pickerRequests: [], pickerHumanIds: [], filter: {}, clientQueryMetadata: {}, orderClauses: [], orderableColumns: [] };
_this.dbConfName = "DefaultRepo";
_this.objectName = "ZzPhones";
_this.loadMethod = "DefaultRepo.ZzPhones.ReadList";
_this.filePrefix = "";
_this.deleteMethod = `${_this.dbConfName}.${_this.objectName}.DeleteByKey`;
_this.orderableColumns = ["PhoneNumber"];
_this.orderClauses = [{ Name: "PhoneNumber", OrderDirection: "ASC" }];
_this.initialResponses = [{ Duration: 0, Result: { Master: [], Aggregations: [{ "Count": 0 }] } }];
_this.initialRequests = [genListRequest(_this.loadMethod, {}, _this.orderClauses, { PageNumber: 1, PageSize: 10 })];
_this.filter = {"Id":null,"PhoneType":"","PhoneNumber":null,"PersonId":""};
_this.initialSearchOptions = _.cloneDeep(_this.filter);
_this.clientQueryMetadata = {"ParentObjectColumns":[{"Name":"Id","DevNote":"","IsPrimaryKey":true,"DbType":"INT","IsIdentity":true,"IdentityStart":"1","IdentityStep":"1","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":true,"ValidationRule":":=i(0,2147483647)"}},{"Name":"PhoneType","DevNote":"","DbType":"INT","AllowNull":true,"Fk":{"FkName":"ZzPhones_PhoneType_Common_BaseInfo_Id","TargetTable":"Common_BaseInfo","TargetColumn":"Id","Lookup":{"Id":"PhoneType_Lookup","Method":"DefaultRepo.Common_BaseInfo.ReadList","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.Common_BaseInfo.ReadList","Where":{"CompareClauses":[{"Name":"ParentId","Value":10000,"ClauseOperator":"Equal"}]},"OrderClauses":[{"Name":"ViewOrder","OrderDirection":"ASC"}],"Pagination":{"PageNumber":1,"PageSize":500},"IncludeSubQueries":false}}},"JsLookupParentId":"10015"},"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Combo","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=i(0,2147483647)"}},{"Name":"PhoneNumber","DevNote":"","DbType":"VARCHAR","Size":"16","AllowNull":true,"IsSortable":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,16)"}},{"Name":"PersonId","DevNote":"","DbType":"INT","AllowNull":true,"Fk":{"FkName":"ZzPhones_PersonId_ZzPersons_Id","TargetTable":"ZzPersons","TargetColumn":"Id","Lookup":{"Id":"PersonId_Lookup","Method":"DefaultRepo.ZzPersons.ReadList","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.ZzPersons.ReadList","Where":{},"OrderClauses":[{"Name":"FirstName","OrderDirection":"ASC"}],"Pagination":{"PageNumber":1,"PageSize":500},"IncludeSubQueries":false}}}},"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"ObjectPicker","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=i(0,2147483647)"}}],"Name":"ReadList","Type":"ReadList","QueryColumns":["Id","PhoneType","PhoneNumber","PersonId"],"FastSearchColumns":[],"ExpandableSearchColumns":[{"Name":"Id","DevNote":"","IsPrimaryKey":true,"DbType":"INT","IsIdentity":true,"IdentityStart":"1","IdentityStep":"1","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":true,"ValidationRule":":=i(0,2147483647)"}},{"Name":"PhoneType","DevNote":"","DbType":"INT","AllowNull":true,"Fk":{"FkName":"ZzPhones_PhoneType_Common_BaseInfo_Id","TargetTable":"Common_BaseInfo","TargetColumn":"Id","Lookup":{"Id":"PhoneType_Lookup","Method":"DefaultRepo.Common_BaseInfo.ReadList","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.Common_BaseInfo.ReadList","Where":{"CompareClauses":[{"Name":"ParentId","Value":10000,"ClauseOperator":"Equal"}]},"OrderClauses":[{"Name":"ViewOrder","OrderDirection":"ASC"}],"Pagination":{"PageNumber":1,"PageSize":500},"IncludeSubQueries":false}}},"JsLookupParentId":"10015"},"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Combo","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=i(0,2147483647)"}},{"Name":"PhoneNumber","DevNote":"","DbType":"VARCHAR","Size":"16","AllowNull":true,"IsSortable":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,16)"}},{"Name":"PersonId","DevNote":"","DbType":"INT","AllowNull":true,"Fk":{"FkName":"ZzPhones_PersonId_ZzPersons_Id","TargetTable":"ZzPersons","TargetColumn":"Id","Lookup":{"Id":"PersonId_Lookup","Method":"DefaultRepo.ZzPersons.ReadList","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.ZzPersons.ReadList","Where":{},"OrderClauses":[{"Name":"FirstName","OrderDirection":"ASC"}],"Pagination":{"PageNumber":1,"PageSize":500},"IncludeSubQueries":false}}}},"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"ObjectPicker","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=i(0,2147483647)"}}],"OptionalQueries":[]};



_this.pickerRequests.push({"Id":"PersonId_Lookup","Method":"DefaultRepo.ZzPersons.ReadList","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.ZzPersons.ReadList","Where":{},"OrderClauses":[{"Name":"FirstName","OrderDirection":"ASC"}],"Pagination":{"PageNumber":1,"PageSize":500},"IncludeSubQueries":false}}});

_this.pickerHumanIds.push({Id:'PersonId_HumanIds',Items:["FirstName","LastName"]});
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