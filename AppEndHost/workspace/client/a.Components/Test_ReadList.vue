<template>
<div class="card h-100 bg-transparent rounded-0 border-0">
		<div class="card-header p-2 bg-body-subtle rounded-0 border-0">
			<div class="container-fluid">
				<div class="row">
					<div class="col-48 col-md-6">
						<input type="text" class="form-control form-control-sm" id="input_Title" @keyup.enter="loadRecords()" v-model="filter.Title" :placeholder="shared.translate('Title')">
					</div>
				</div>
			</div>
		</div>
		<div class="card-header simple-search p-2 px-0 bg-transparent rounded-0 border-0 collapse">
			<div class="container-fluid">
				<div class="row">
					<div class="col-48 col-md-6">
						<input type="text" class="form-control form-control-sm" id="input_Age" @keyup.enter="loadRecords()" v-model="filter.Age" :placeholder="shared.translate('Age')">
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
				<button type="button" class="btn btn-sm border-0 btn-outline-success px-2" data-ae-actions="DefaultRepo.Test.Create" @click="openCreate()">
					<i class="fa-solid fa-file-alt fa-bounce" style="--fa-animation-iteration-count:1"></i>
					<span>{{shared.translate("Create")}}</span>
				</button>
				<div class="vr"></div>
				<div class="btn btn-sm border-0 btn-outline-success px-2" data-ae-actions="DefaultRepo.Test.ReadList" @click="exportExcel">
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
								<th class="sticky-top ae-thead-th fw-bold text-primary fw-bold text-center" style="width:95px;overflow: hidden;text-overflow: ellipsis;">
									<i class="fa-solid fa-fw fa-window-restore"></i>
								</th>
								<th class="sticky-top ae-thead-th fw-bold text-success" style="width:185px;">
									<div>{{shared.translate("HumanIds")}}</div>
								</th>
								<th class="sticky-top ae-thead-th text-center" style="width:95px;overflow: hidden;text-overflow: ellipsis;">
									<div>{{shared.translate("Age")}}</div>
								</th>
								<th class="sticky-top ae-thead-th"></th>
								<th style="width:40px;" class="sticky-top ae-thead-th text-center" data-ae-actions="DefaultRepo.Test.DeleteByKey"></th>
							</tr>
						</thead>
						<tbody v-if="initialResponses[0].IsSucceeded===true">
							<tr v-for="i in initialResponses[0]['Result']['Master']">
								<td class="ae-table-td text-primary bg-hover-light text-center pointer" @click="openById({compPath:'/a.Components/Test_UpdateByKey',recordKey:i.Id,refereshOnCallback:true,actionsAllowed:'DefaultRepo.Test.UpdateByKey',fkToParent:''});">
									<i class="fa-solid fa-fw fa-edit"></i>
									<div class="pk font-monospace">{{i.Id}}</div>
								</td>
								<td class="ae-table-td">
									<div>
										<span class="badge text-muted fs-d7 text-start me-1">{{shared.translate("Title")}}</span>
										<span class="fw-bold">
											<span>{{shared.fixNull(i["Title"],'-')}}</span>
										</span>
									</div>
								</td>
								<td class="ae-table-td text-center">
									<div>{{i["Age"]}}</div>
								</td>
								<td></td>
								<td style="width:40px;vertical-align:middle" class="text-center text-secondary text-hover-danger pointer" data-ae-actions="DefaultRepo.Test.DeleteByKey" @click="deleteById({pkValue:i.Id})">
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
_this.objectName = "Test";
_this.loadMethod = "DefaultRepo.Test.ReadList";
_this.filePrefix = "";
_this.deleteMethod = `${_this.dbConfName}.${_this.objectName}.DeleteByKey`;
_this.orderableColumns = ["Title"];
_this.orderClauses = [{ Name: "Title", OrderDirection: "ASC" }];
_this.initialResponses = [{ Duration: 0, Result: { Master: [], Aggregations: [{ "Count": 0 }] } }];
_this.initialRequests = [genListRequest(_this.loadMethod, {}, _this.orderClauses, { PageNumber: 1, PageSize: 10 })];
_this.filter = {"Title":null,"Id":null,"Age":null};
_this.initialSearchOptions = _.cloneDeep(_this.filter);
_this.columns = {"ParentObjectColumns":[{"Name":"Id","DevNote":"","IsPrimaryKey":true,"DbType":"INT","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true,"ValidationRule":":=i(0,2147483647)"}},{"Name":"Title","DevNote":"","DbType":"NVARCHAR","Size":"50","AllowNull":true,"IsHumanId":true,"IsSortable":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Fast","Required":false,"ValidationRule":":=s(0,50)"}},{"Name":"Age","DevNote":"","DbType":"INT","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=i(0,2147483647)"}}],"Name":"ReadList","Type":"ReadList","QueryColumns":["Id","Title","Age"],"FastSearchColumns":[{"Name":"Title","DevNote":"","DbType":"NVARCHAR","Size":"50","AllowNull":true,"IsHumanId":true,"IsSortable":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Fast","Required":false,"ValidationRule":":=s(0,50)"}}],"ExpandableSearchColumns":[{"Name":"Id","DevNote":"","IsPrimaryKey":true,"DbType":"INT","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true,"ValidationRule":":=i(0,2147483647)"}},{"Name":"Age","DevNote":"","DbType":"INT","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=i(0,2147483647)"}}],"OptionalQueries":[]};


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