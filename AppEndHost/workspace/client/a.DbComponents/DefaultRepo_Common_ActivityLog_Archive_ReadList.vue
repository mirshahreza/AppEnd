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
				<button type="button" class="btn btn-sm border-0 btn-outline-success px-2" data-ae-actions="DefaultRepo.Common_ActivityLog_Archive.Create" @click="localOpenCreate">
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
					<div class="form-control pointer data-ae-validation" data-ae-widget="nullableCheckbox" data-ae-widget-options="{&quot;shownull&quot;:true}">
						<i class="fa-solid fa-fw me-1"></i>
						<span>{{shared.translate('IsSucceeded')}}</span>
						<input type="hidden" v-model="searchOptions.IsSucceeded" data-ae-validation-required="false">
					</div>
				</div>
				<div class="col-48 col-md-6">
					<div class="form-control pointer data-ae-validation" data-ae-widget="nullableCheckbox" data-ae-widget-options="{&quot;shownull&quot;:true}">
						<i class="fa-solid fa-fw me-1"></i>
						<span>{{shared.translate('FromCache')}}</span>
						<input type="hidden" v-model="searchOptions.FromCache" data-ae-validation-required="false">
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
								<td class="sticky-top ae-thead-td fb text-primary fw-bold text-center" style="width:75px;overflow: hidden;text-overflow: ellipsis;">
									<div>{{shared.translate("Id")}}</div>
								</td>
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
								<td class="sticky-top ae-thead-td text-center" style="width:75px;overflow: hidden;text-overflow: ellipsis;">
									<div>{{shared.translate("EventBy")}}</div>
								</td>
								<td class="sticky-top ae-thead-td " style="min-width:185px;">
									<div>{{shared.translate("EventOn")}}</div>
								</td>
								<td class="sticky-top ae-thead-td text-center" style="width:75px;overflow: hidden;text-overflow: ellipsis;">
									<div>{{shared.translate("Duration")}}</div>
								</td>
								<td class="sticky-top ae-thead-td " style="min-width:185px;">
									<div>{{shared.translate("ClientInfo")}}</div>
								</td>
								<td style="width:40px;" class="sticky-top ae-thead-td text-center" data-ae-actions="DefaultRepo.Common_ActivityLog_Archive.DeleteByKey"></td>
							</tr>
						</thead>
						<tbody>
							<tr v-for="i in initialResponses[0]['Result']['Master']">
								<td class="ae-table-td text-dark text-center" style="width:75px;overflow: hidden;text-overflow: ellipsis;" @click="localCrudOpenById('/a.DbComponents/DefaultRepo_Common_ActivityLog_Archive_UpdateByKey','modal-lg',i.Id,true,'DefaultRepo.Common_ActivityLog_Archive.UpdateByKey','');">
									<div class="pointer text-primary hover-success">
										<i class="fa-solid fa-fw fa-edit"></i>
										<br>
										<span class="pk">{{i.Id}}</span>
									</div>
								</td>
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
								<td class="ae-table-td text-center" style="width:75px;overflow: hidden;text-overflow: ellipsis;">
									<div>{{i["EventBy"]}}</div>
								</td>
								<td class="ae-table-td text-center " style="min-width:185px;">
									<div style="direction:ltr">{{shared.formatDateTimeL(i["EventOn"])}}</div>
								</td>
								<td class="ae-table-td text-center" style="width:75px;overflow: hidden;text-overflow: ellipsis;">
									<div>{{i["Duration"]}}</div>
								</td>
								<td class="ae-table-td " style="min-width:185px;">
									<div>{{i["ClientInfo"]}}</div>
								</td>
								<td style="width:40px;vertical-align:middle" class="text-center" data-ae-actions="DefaultRepo.Common_ActivityLog_Archive.DeleteByKey">
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
shared.setAppTitle(shared.translate("Common_ActivityLog_Archive, ReadList"));
let _this = { cid: "", c: null, dbConfName: "", objectName: "", loadMethod: "", deleteMethod: "", initialRequests: [], initialResponses: [], pickerRequests: [], pickerHumanIds: [], searchOptions: {}, clientQueryMetadata: {}, orderClauses: [], orderableColumns: [] };
_this.dbConfName = "DefaultRepo";
_this.objectName = "Common_ActivityLog_Archive";
_this.loadMethod = "DefaultRepo.Common_ActivityLog_Archive.ReadList";
_this.deleteMethod = `${_this.dbConfName}.${_this.objectName}.DeleteByKey`;
_this.orderableColumns = ["Id","EventBy","Duration"];
_this.orderClauses = [{ Name: "Id", OrderDirection: "ASC" }];
_this.initialResponses = [{ Duration: 0, Result: { Master: [], Aggregations: [{ "Count": 0 }] } }];
_this.initialRequests = [genListRequest(_this.loadMethod, {}, _this.orderClauses, { PageNumber: 1, PageSize: 10 })];
_this.searchOptions = {"Id":null,"Method":null,"IsSucceeded":null,"FromCache":null,"RecordId":null,"EventBy":null,"Duration":null,"ClientInfo":null};
_this.clientQueryMetadata = {"ParentObjectColumns":[{"Name":"Id","DevNote":"","IsPrimaryKey":true,"DbType":"INT","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true,"ValidationRule":":=i(0,10000)"}},{"Name":"Method","DevNote":"","DbType":"VARCHAR","Size":"128","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true}},{"Name":"IsSucceeded","DevNote":"","DbType":"BIT","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Checkbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true}},{"Name":"FromCache","DevNote":"","DbType":"BIT","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Checkbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true}},{"Name":"RecordId","DevNote":"","DbType":"VARCHAR","Size":"64","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false}},{"Name":"EventBy","DevNote":"","DbType":"INT","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true,"ValidationRule":":=i(0,10000)"}},{"Name":"EventOn","DevNote":"","DbType":"DATETIME","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"DateTimePicker","UiWidgetOptions":"{}","Required":true,"ValidationRule":"dt(1900-01-01 00:01:00,2100-12-30 11:59:59)"}},{"Name":"Duration","DevNote":"","DbType":"FLOAT","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true,"ValidationRule":":=i(0,10000)"}},{"Name":"ClientInfo","DevNote":"","DbType":"VARCHAR","Size":"256","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"MultilineTextbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false}}],"Name":"ReadList","Type":"ReadList","QueryColumns":["Id","Method","IsSucceeded","FromCache","RecordId","EventBy","EventOn","Duration","ClientInfo"],"FastSearchColumns":[],"ExpandableSearchColumns":[{"Name":"Id","DevNote":"","IsPrimaryKey":true,"DbType":"INT","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true,"ValidationRule":":=i(0,10000)"}},{"Name":"Method","DevNote":"","DbType":"VARCHAR","Size":"128","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true}},{"Name":"IsSucceeded","DevNote":"","DbType":"BIT","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Checkbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true}},{"Name":"FromCache","DevNote":"","DbType":"BIT","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Checkbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true}},{"Name":"RecordId","DevNote":"","DbType":"VARCHAR","Size":"64","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false}},{"Name":"EventBy","DevNote":"","DbType":"INT","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true,"ValidationRule":":=i(0,10000)"}},{"Name":"Duration","DevNote":"","DbType":"FLOAT","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true,"ValidationRule":":=i(0,10000)"}},{"Name":"ClientInfo","DevNote":"","DbType":"VARCHAR","Size":"256","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"MultilineTextbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false}}],"OptionalQueries":[]};





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