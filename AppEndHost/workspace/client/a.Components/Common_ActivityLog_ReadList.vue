<template>
	<div class="card h-100 bg-transparent rounded-0 border-0">
		<div class="card-header p-2 bg-success-subtle rounded-0 border-0">
			<div class="row">
				<div class="col-48 col-md-6">
					<div class="form-control form-control-sm text-nowrap pointer data-ae-validation" data-ae-widget="nullableCheckbox" data-ae-widget-options="{&quot;shownull&quot;:true}">
						<i class="fa-solid fa-fw me-1"></i>
						<span>{{shared.translate('IsSucceeded')}}</span>
						<input type="hidden" v-model="filter.IsSucceeded" data-ae-validation-required="false">
					</div>
				</div>
				<div class="col-48 col-md-6">
					<input type="text" class="form-control form-control-sm" id="input_Method" @keyup.enter="loadRecords()" v-model="filter.Method" :placeholder="shared.translate('Method')">
				</div>
				<div class="col-48 col-md-6">
					<input type="text" class="form-control form-control-sm" id="input_RecordId" @keyup.enter="loadRecords()" v-model="filter.RecordId" :placeholder="shared.translate('RecordId')">
				</div>
			</div>
		</div>
		<div class="simple-search card-header p-2 bg-transparent rounded-0 collapse">
			<div class="row">
				<div class="col-48 col-md-6">
					<div class="form-control form-control-sm pointer data-ae-validation" data-ae-widget="nullableCheckbox" data-ae-widget-options="{&quot;shownull&quot;:true}">
						<i class="fa-solid fa-fw me-1"></i>
						<span>{{shared.translate('FromCache')}}</span>
						<input type="hidden" v-model="filter.FromCache" data-ae-validation-required="false">
					</div>
				</div>
				<div class="col-48 col-md-6">
					<input type="text" class="form-control form-control-sm" id="input_EventBy" @keyup.enter="loadRecords()" v-model="filter.EventBy" :placeholder="shared.translate('EventBy')">
				</div>
				<div class="col-48 col-md-6">
					<input type="text" class="form-control form-control-sm" id="input_ClientInfo" @keyup.enter="loadRecords()" v-model="filter.ClientInfo" :placeholder="shared.translate('ClientInfo')">
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
			</div>
		</div>

		<div class="card-body p-0">
			<div class="card h-100 border-light bg-light bg-opacity-75 border-0">
				<div class="card-body rounded rounded-1 border border-3 border-light fs-d8 p-0 bg-transparent scrollable">
					<table class="table table-sm table-hover w-100 ae-table m-0 bg-transparent">
						<thead>
							<tr class="d-none d-md-table-row d-lg-table-row d-xl-table-row">
								<th class="sticky-top ae-thead-th">
									<div>{{shared.translate("Method")}}</div>
								</th>
								<th class="sticky-top ae-thead-th text-center" style="width:100px;">
									<div>{{shared.translate("Duration")}}</div>
								</th>
								<th class="sticky-top ae-thead-th text-center" style="width:100px;">
									<div>{{shared.translate("IsSucceeded")}}</div>
								</th>
								<th class="sticky-top ae-thead-th text-center" style="width:100px;">
									<div>{{shared.translate("FromCache")}}</div>
								</th>
								<th class="sticky-top ae-thead-th text-center" style="width:125px;">
									<div>{{shared.translate("RecordId")}}</div>
								</th>
								<th class="sticky-top ae-thead-th text-center" style="width:100px;">
									<div>{{shared.translate("EventBy")}}</div>
								</th>
								<th class="sticky-top ae-thead-th text-center" style="width:125px;">
									<div>{{shared.translate("EventOn")}}</div>
								</th>
								<th class="sticky-top ae-thead-th">
									<div>{{shared.translate("ClientInfo")}}</div>
								</th>
							</tr>
						</thead>
						<tbody v-if="initialResponses[0].IsSucceeded===true">
							<tr v-for="i in initialResponses[0]['Result']['Master']">
								<td class="ae-table-td ">
									<div>{{i["Method"]}}</div>
								</td>
								<td class="ae-table-td text-center">
									<div>{{i["Duration"]}}</div>
								</td>
								<td class="ae-table-td text-center">
									<span v-html="shared.convertBoolToIconWithOptions(i.IsSucceeded ,{})"></span>
								</td>
								<td class="ae-table-td text-center">
									<span v-html="shared.convertBoolToIconWithOptions(i.FromCache ,{})"></span>
								</td>
								<td class="ae-table-td text-center">
									<div>{{i["RecordId"]}}</div>
								</td>
								<td class="ae-table-td text-center">
									<div>{{i["EventBy"]}}</div>
								</td>
								<td class="ae-table-td text-center">
									<div style="direction:ltr">{{shared.formatDateTimeL(i["EventOn"])}}</div>
								</td>
								<td class="ae-table-td">
									<div>{{i["ClientInfo"]}}</div>
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
				<select class="form-select form-select-sm text-primary border-0 ae-input d-none d-md-block d-lg-block d-xl-block bg-white" style="max-width:135px;" v-model="initialRequests[0].Inputs.ClientQueryJE.OrderClauses[0].Name" @change="loadRecords()">
					<option v-for="o in orderableColumns" :value="o">{{shared.translate(o)}}</option>
				</select>
				<select class="form-select form-select-sm text-primary border-0 ae-input d-none d-md-block d-lg-block d-xl-block bg-white" style="max-width:125px;" v-model="initialRequests[0].Inputs.ClientQueryJE.OrderClauses[0].OrderDirection" @change="loadRecords()">
					<option value="ASC">{{shared.translate("Asc")}}</option>
					<option value="DESC">{{shared.translate("Desc")}}</option>
				</select>
				<span class="input-group-text border-0 fs-d4 text-secondary d-none d-md-block d-lg-block d-xl-block bg-white"></span>
				<div class="input-group-text border-0 d-none d-md-block d-lg-block d-xl-block fs-d7 pt-2 bg-white">
					<span class="text-secondary">{{shared.translate("PageSize")}}</span>
				</div>
				<select class="form-select form-select-sm text-primary border-0 ae-input d-none d-md-block d-lg-block d-xl-block bg-white" style="max-width:75px;" v-model.number="initialRequests[0].Inputs.ClientQueryJE.Pagination.PageSize" @change="loadRecords()">
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
shared.setAppTitle(shared.translate("Common_ActivityLog, ReadList"));
let _this = { cid: "", c: null, templateType:"ReadList", dbConfName: "", objectName: "", loadMethod: "", deleteMethod: "", initialRequests: [], initialResponses: [], pickerRequests: [], pickerHumanIds: [], filter: {}, clientQueryMetadata: {}, orderClauses: [], orderableColumns: [] };
_this.dbConfName = "DefaultRepo";
_this.objectName = "Common_ActivityLog";
_this.loadMethod = "DefaultRepo.Common_ActivityLog.ReadList";
_this.deleteMethod = `${_this.dbConfName}.${_this.objectName}.`;
_this.orderableColumns = ["EventOn"];
_this.orderClauses = [{ Name: "EventOn", OrderDirection: "ASC" }];
_this.initialResponses = [{ Duration: 0, Result: { Master: [], Aggregations: [{ "Count": 0 }] } }];
_this.initialRequests = [genListRequest(_this.loadMethod, {}, _this.orderClauses, { PageNumber: 1, PageSize: 100 })];
_this.filter = {"Method":null,"IsSucceeded":null,"RecordId":null,"Id":null,"FromCache":null,"EventBy":null,"ClientInfo":null};
_this.initialSearchOptions = _.cloneDeep(_this.filter);
_this.clientQueryMetadata = {"ParentObjectColumns":[{"Name":"Id","DevNote":"","IsPrimaryKey":true,"DbType":"INT","IsIdentity":true,"IdentityStart":"1","IdentityStep":"1","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":true,"ValidationRule":":=i(0,2147483647)"}},{"Name":"Method","DevNote":"","DbType":"VARCHAR","Size":"128","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Fast","Required":true,"ValidationRule":":=s(0,128)"}},{"Name":"IsSucceeded","DevNote":"","DbType":"BIT","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Checkbox","UiWidgetOptions":"{}","SearchType":"Fast","Required":true}},{"Name":"FromCache","DevNote":"","DbType":"BIT","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Checkbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true}},{"Name":"RecordId","DevNote":"","DbType":"VARCHAR","Size":"64","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Fast","Required":false,"ValidationRule":":=s(0,64)"}},{"Name":"EventBy","DevNote":"","DbType":"INT","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true,"ValidationRule":":=i(0,2147483647)"}},{"Name":"EventOn","DevNote":"","DbType":"DATETIME","IsSortable":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"DateTimePicker","UiWidgetOptions":"{}","Required":true,"ValidationRule":"dt(1900-01-01 00:01:00,2100-12-30 11:59:59)"}},{"Name":"Duration","DevNote":"","DbType":"FLOAT","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","Required":true}},{"Name":"ClientInfo","DevNote":"","DbType":"VARCHAR","Size":"256","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"MultilineTextbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,256)"}}],"Name":"ReadList","Type":"ReadList","QueryColumns":["Id","Method","IsSucceeded","FromCache","RecordId","EventBy","EventOn","Duration","ClientInfo"],"FastSearchColumns":[{"Name":"Method","DevNote":"","DbType":"VARCHAR","Size":"128","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Fast","Required":true,"ValidationRule":":=s(0,128)"}},{"Name":"IsSucceeded","DevNote":"","DbType":"BIT","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Checkbox","UiWidgetOptions":"{}","SearchType":"Fast","Required":true}},{"Name":"RecordId","DevNote":"","DbType":"VARCHAR","Size":"64","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Fast","Required":false,"ValidationRule":":=s(0,64)"}}],"ExpandableSearchColumns":[{"Name":"Id","DevNote":"","IsPrimaryKey":true,"DbType":"INT","IsIdentity":true,"IdentityStart":"1","IdentityStep":"1","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":true,"ValidationRule":":=i(0,2147483647)"}},{"Name":"FromCache","DevNote":"","DbType":"BIT","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Checkbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true}},{"Name":"EventBy","DevNote":"","DbType":"INT","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true,"ValidationRule":":=i(0,2147483647)"}},{"Name":"ClientInfo","DevNote":"","DbType":"VARCHAR","Size":"256","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"MultilineTextbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,256)"}}],"OptionalQueries":[]};


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