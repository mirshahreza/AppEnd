<template>
<div class="card h-100 rounded rounded-2 rounded-bottom-0 rounded-end-0 bg-transparent border-0">
		<div class="card-header p-2 bg-light-subtle rounded-end-0 border-0">
			<div class="input-group input-group-sm border-0 bg-transparent">
				<div class="form-control rounded-0 border-0 bg-transparent p-0">
					<div class="row">
						<div class="col-48 col-md-6">
							<div class="form-control pointer data-ae-validation" data-ae-widget="nullableCheckbox" data-ae-widget-options="{&quot;shownull&quot;:true}">
								<i class="fa-solid fa-fw me-1"></i>
								<span>{{shared.translate('IsSucceeded')}}</span>
								<input type="hidden" v-model="searchOptions.IsSucceeded" data-ae-validation-required="false">
							</div>
						</div>
						<div class="col-48 col-md-6">
							<input type="text" class="form-control" id="input_Method" @keyup.enter="localCrudLoadRecords" v-model="searchOptions.Method" :placeholder="shared.translate('Method')">
						</div>
						<div class="col-48 col-md-6">
							<input type="text" class="form-control" id="input_RecordId" @keyup.enter="localCrudLoadRecords" v-model="searchOptions.RecordId" :placeholder="shared.translate('RecordId')">
						</div>
						<div class="col-48 col-md-6">
							<input type="text" class="form-control" id="input_EventBy" @keyup.enter="localCrudLoadRecords" v-model="searchOptions.EventBy" :placeholder="shared.translate('EventBy')">
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
			</div>
		</div>
		<div class="simple-search card-header p-2 bg-transparent rounded-0 collapse">
			<div class="row">
				<div class="col-48 col-md-6">
					<div class="form-control pointer data-ae-validation" data-ae-widget="nullableCheckbox" data-ae-widget-options="{&quot;shownull&quot;:true}">
						<i class="fa-solid fa-fw me-1"></i>
						<span>{{shared.translate('FromCache')}}</span>
						<input type="hidden" v-model="searchOptions.FromCache" data-ae-validation-required="false">
					</div>
				</div>
				<div class="col-48 col-md-6">
					<input type="text" class="form-control" id="input_Duration" @keyup.enter="localCrudLoadRecords" v-model="searchOptions.Duration" :placeholder="shared.translate('Duration')">
				</div>
				<div class="col-48 col-md-6">
					<input type="text" class="form-control" id="input_ClientInfo" @keyup.enter="localCrudLoadRecords" v-model="searchOptions.ClientInfo" :placeholder="shared.translate('ClientInfo')">
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
								<td class="sticky-top ae-thead-td " style="min-width:185px;">
									<div>{{shared.translate("Method")}}</div>
								</td>
								<td class="sticky-top ae-thead-td text-center" style="width:75px;overflow: hidden;text-overflow: ellipsis;">
									<div>{{shared.translate("IsSucceeded")}}</div>
								</td>
								<td class="sticky-top ae-thead-td text-center" style="width:75px;overflow: hidden;text-overflow: ellipsis;">
									<div>{{shared.translate("FromCache")}}</div>
								</td>
								<td class="sticky-top ae-thead-td " style="min-width:185px;">
									<div>{{shared.translate("RecordId")}}</div>
								</td>
								<td class="sticky-top ae-thead-td " style="min-width:185px;">
									<div>{{shared.translate("EventBy")}}</div>
								</td>
								<td class="sticky-top ae-thead-td " style="min-width:185px;">
									<div>{{shared.translate("EventOn")}}</div>
								</td>
								<td class="sticky-top ae-thead-td " style="min-width:185px;">
									<div>{{shared.translate("ClientInfo")}}</div>
								</td>
							</tr>
						</thead>
						<tbody>
							<tr v-for="i in initialResponses[0]['Result']['Master']">
								<td class="ae-table-td " style="min-width:185px;">
									<div>{{i["Method"]}}</div>
								</td>
								<td class="ae-table-td text-center" style="width:75px;overflow: hidden;text-overflow: ellipsis;">
									<span v-html="shared.convertBoolToIconWithOptions(i.IsSucceeded ,{})"></span>
								</td>
								<td class="ae-table-td text-center" style="width:75px;overflow: hidden;text-overflow: ellipsis;">
									<span v-html="shared.convertBoolToIconWithOptions(i.FromCache ,{})"></span>
								</td>
								<td class="ae-table-td " style="min-width:185px;">
									<div>{{i["RecordId"]}}</div>
								</td>
								<td class="ae-table-td " style="min-width:185px;">
									<div>{{i["EventBy"]}}</div>
								</td>
								<td class="ae-table-td " style="min-width:185px;">
									<div>{{i["EventOn"]}}</div>
								</td>
								<td class="ae-table-td " style="min-width:185px;">
									<div>{{i["ClientInfo"]}}</div>
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
shared.setAppTitle(shared.translate("Common_ActivityLog, ReadList"));
let _this = { cid: "", c: null, dbConfName: "", objectName: "", loadMethod: "", deleteMethod: "", initialRequests: [], initialResponses: [], pickerRequests: [], pickerHumanIds: [], searchOptions: {}, clientQueryMetadata: {}, orderClauses: [], orderableColumns: [] };
_this.dbConfName = "DefaultRepo";
_this.objectName = "Common_ActivityLog";
_this.loadMethod = "DefaultRepo.Common_ActivityLog.ReadList";
_this.deleteMethod = `${_this.dbConfName}.${_this.objectName}.`;
_this.orderableColumns = ["Duration","Id"];
_this.orderClauses = [{ Name: "Duration", OrderDirection: "ASC" }];
_this.initialResponses = [{ Duration: 0, Result: { Master: [], Aggregations: [{ "Count": 0 }] } }];
_this.initialRequests = [genListRequest(_this.loadMethod, {}, _this.orderClauses, { PageNumber: 1, PageSize: 10 })];
_this.searchOptions = {"Method":null,"IsSucceeded":null,"RecordId":null,"EventBy":null,"EventOn":null,"Id":null,"FromCache":null,"Duration":null,"ClientInfo":null};
_this.clientQueryMetadata = {"ParentObjectColumns":[{"Name":"Method","DevNote":"","DbType":"VARCHAR","Size":"128","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Fast","Required":true}},{"Name":"IsSucceeded","DevNote":"","DbType":"BIT","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Checkbox","UiWidgetOptions":"{}","SearchType":"Fast","Required":true}},{"Name":"RecordId","DevNote":"","DbType":"VARCHAR","Size":"64","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Fast","Required":false}},{"Name":"EventBy","DevNote":"","DbType":"NVARCHAR","Size":"64","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Fast","Required":true}},{"Name":"EventOn","DevNote":"","DbType":"DATETIME","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"DateTimePicker","UiWidgetOptions":"{}","SearchType":"Fast","Required":true,"ValidationRule":"dt(1900-01-01 00:01:00,2100-12-30 11:59:59)"}},{"Name":"Id","DevNote":"","IsPrimaryKey":true,"DbType":"UNIQUEIDENTIFIER","DbDefault":"newid","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true}},{"Name":"FromCache","DevNote":"","DbType":"BIT","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Checkbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true}},{"Name":"Duration","DevNote":"","DbType":"FLOAT","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Numberbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true,"ValidationRule":":=i(0,10000)"}},{"Name":"ClientInfo","DevNote":"","DbType":"VARCHAR","Size":"256","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false}}],"Name":"ReadList","Type":"ReadList","QueryColumns":["Method","IsSucceeded","FromCache","RecordId","EventBy","EventOn","Duration","ClientInfo"],"FastSearchColumns":[{"Name":"Method","DevNote":"","DbType":"VARCHAR","Size":"128","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Fast","Required":true}},{"Name":"IsSucceeded","DevNote":"","DbType":"BIT","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Checkbox","UiWidgetOptions":"{}","SearchType":"Fast","Required":true}},{"Name":"RecordId","DevNote":"","DbType":"VARCHAR","Size":"64","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Fast","Required":false}},{"Name":"EventBy","DevNote":"","DbType":"NVARCHAR","Size":"64","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Fast","Required":true}},{"Name":"EventOn","DevNote":"","DbType":"DATETIME","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"DateTimePicker","UiWidgetOptions":"{}","SearchType":"Fast","Required":true,"ValidationRule":"dt(1900-01-01 00:01:00,2100-12-30 11:59:59)"}}],"ExpandableSearchColumns":[{"Name":"Id","DevNote":"","IsPrimaryKey":true,"DbType":"UNIQUEIDENTIFIER","DbDefault":"newid","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true}},{"Name":"FromCache","DevNote":"","DbType":"BIT","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Checkbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true}},{"Name":"Duration","DevNote":"","DbType":"FLOAT","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Numberbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true,"ValidationRule":":=i(0,10000)"}},{"Name":"ClientInfo","DevNote":"","DbType":"VARCHAR","Size":"256","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false}}],"OptionalQueries":[]};





	export default {
	methods: {
		localOpenPicker(colName) { crudOpenPicker(_this, _this.c.searchOptions, colName); },
		localCrudLoadRecords() { crudLoadRecords(_this); },
		localCrudOpenById(compPath, modalSize, recordKey, refereshOnCallback, actionsAllowed) { crudOpenById(_this, compPath, modalSize, recordKey, refereshOnCallback, actionsAllowed); },
		localCrudDeleteRecord(recordKey) { crudDeleteRecord(_this, "Id", recordKey); },
		localOpenCreate() {
			crudOpenCreate(_this, `/.dbcomponents/${_this.dbConfName}_${_this.objectName}_`, 'modal-lg');
		}
	},
	setup(props) { _this.cid = props['cid']; },
	data() { return _this; },
	created() { _this.c = this; },
	mounted() { _this.c.localCrudLoadRecords(); },
	props: { cid: String }
}

</script>