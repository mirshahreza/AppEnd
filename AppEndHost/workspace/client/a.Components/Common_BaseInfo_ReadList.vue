<template>
	<div class="card h-100 bg-transparent rounded-0 border-0">
		<div class="card-header p-2 bg-success-subtle rounded-0 border-0">
			<div class="row">
				<div class="col-48 col-md-6">
					<div class="form-control form-control-sm py-0 data-ae-validation">
						<div class="input-group input-group-sm p-0 pt-1" data-ae-widget="objectPicker">
							<input type="hidden" v-model="filter.ParentId">
							<input type="hidden" v-model="filter.ParentId_Title">
							<input type="text" class="form-control bg-transparent p-0 m-0 border-0" :value="shared.fixNull(filter.ParentId+' '+filter.ParentId_Title,'',true)" :placeholder="shared.translate('ParentId')" disabled="">
							<span>&nbsp;</span>
							<button class="btn btn-sm btn-outline-secondary bg-transparent p-0 m-0 me-1 border-0 text-hover-primary ae-objectpicker-search" type="button" @click="openPicker({colName:'ParentId'})">
								<i class="fa-solid fa-hand-pointer"></i>
							</button>
							<button class="btn btn-sm btn-outline-secondary bg-transparent p-0 m-0 ms-1 border-0 text-hover-danger ae-objectpicker-clear" type="button">
								<i class="fa-solid fa-times"></i>
							</button>
						</div>
					</div>
				</div>
				<div class="col-48 col-md-6">
					<div class="form-control form-control-sm pointer data-ae-validation" data-ae-widget="nullableCheckbox" data-ae-widget-options="{&quot;shownull&quot;:true}">
						<i class="fa-solid fa-fw me-1"></i>
						<span>{{shared.translate('IsActive')}}</span>
						<input type="hidden" v-model="filter.IsActive" data-ae-validation-required="false">
					</div>
				</div>
				<div class="col-48 col-md-6">
					<input type="text" class="form-control form-control-sm" id="input_Title" @keyup.enter="loadRecords" v-model="filter.Title" :placeholder="shared.translate('Title')">
				</div>
			</div>
		</div>
		<div class="simple-search card-header p-2 bg-transparent rounded-0 collapse">
			<div class="row">
				<div class="col-48 col-md-6">
					<input type="text" class="form-control form-control-sm" id="input_ShortName" @keyup.enter="loadRecords" v-model="filter.ShortName" :placeholder="shared.translate('ShortName')">
				</div>
				<div class="col-48 col-md-6">
					<input type="text" class="form-control form-control-sm" id="input_Note" @keyup.enter="loadRecords" v-model="filter.Note" :placeholder="shared.translate('Note')">
				</div>
				<div class="col-48 col-md-6">
					<input type="text" class="form-control form-control-sm" id="input_Id" @keyup.enter="loadRecords" v-model="filter.Id" :placeholder="shared.translate('Id')">
				</div>
			</div>
		</div>
		<div class="card-header p-2 rounded-0">
			<div class="hstack gap-1">
				<button class="btn btn-sm btn-outline-primary px-3" @click="loadRecords">
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
				<button type="button" class="btn btn-sm border-0 btn-outline-success px-2" data-ae-actions="DefaultRepo.AAA_Users.Create" @click="openCreate()">
					<i class="fa-solid fa-file-alt fa-bounce pe-1" style="--fa-animation-iteration-count:1"></i>
					<span class="ms-1">{{shared.translate("Create")}}</span>
				</button>
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
								<th class="sticky-top ae-thead-th fb text-success" style="width:180px;">
									<div>{{shared.translate("Title")}}</div>
								</th>
								<th class="sticky-top ae-thead-th" style="width:180px;">
									<div>{{shared.translate("Parent")}}</div>
								</th>
								<th class="sticky-top ae-thead-th text-center" style="width:100px;">
									<div>{{shared.translate("ViewOrder")}}</div>
								</th>
								<th class="sticky-top ae-thead-th" style="width:165px;">
									<div>{{shared.translate("IsActive")}}</div>
								</th>
								<th class="sticky-top ae-thead-th text-center" style="width:75px;">
									<div>{{shared.translate("MetaInfo")}}</div>
								</th>
								<th class="sticky-top ae-thead-th text-center" style="width:74px;">
									<div>{{shared.translate("UiInfo")}}</div>
								</th>
								<th class="sticky-top ae-thead-th"></th>
								<th style="width:40px;" class="sticky-top ae-thead-th text-center" data-ae-actions="DefaultRepo.Common_BaseInfo.DeleteByKey"></th>
							</tr>
						</thead>
						<tbody v-if="initialResponses[0].IsSucceeded===true">
							<tr v-for="i in initialResponses[0]['Result']['Master']">
								<td class="ae-table-td text-dark text-center"
									@click="openById({dialog:{modalSize:'modal-fullscreen'},compPath:'/a.Components/Common_BaseInfo_UpdateByKey',recordKey:i.Id,refereshOnCallback:true,actionsAllowed:'DefaultRepo.Common_BaseInfo.UpdateByKey',fkToParent:'ParentId'});">
									<div class="text-primary text-hover-success pointer">
										<i class="fa-solid fa-fw fa-edit"></i>
										<br>
										<span class="pk">{{i.Id}}</span>
									</div>
								</td>
								<td class="ae-table-td">
									<span>{{shared.fixNull(i["Title"],'-')}}</span>
								</td>
								<td class="ae-table-td">
									<div class="text-dark fb">
										<div>{{shared.translate(i["ParentId_Title"])}}</div>
									</div>
									<div class="text-muted fs-d7">{{i["ParentId"]}}</div>
								</td>
								<td class="ae-table-td text-center">
									<div>{{i["ViewOrder"]}}</div>
								</td>
								<td class="ae-table-td pointer" @click="openById({compPath:'/a.Components/Common_BaseInfo_IsActiveUpdate',recordKey:i.Id,refereshOnCallback:true,actionsAllowed:'DefaultRepo.Common_BaseInfo.IsActiveUpdate',fkToParent:'ParentId'});">
									<div class="input-group input-group-sm bg-hover-primary rounded-2 p-2">
										<div class="input-group-text rounded-2 me-1">
											<span v-html="shared.convertBoolToIconWithOptions(i.IsActive ,{})"></span>
										</div>
										<div class="more-info" style="">
											<table class="w-100 h-100 fs-d8">
												<tbody>
													<tr>
														<td class="text-muted align-middle" style="min-width:25px;">{{shared.translate("By")}}</td>
														<td class="text-dark fb align-middle">
															<span class="fw-bold">{{shared.fixNull(i["UpdatedBy"],'-')}}</span>
														</td>
													</tr>
													<tr>
														<td class="text-muted align-middle" style="min-width:25px;">{{shared.translate("On")}}</td>
														<td class="text-dark fb align-middle">
															<span class="fw-bold">{{shared.fixNullOrEmpty(shared.formatDateL(i["UpdatedOn"]),'-')}}</span>
														</td>
													</tr>
												</tbody>
											</table>
										</div>
									</div>
								</td>
								<td class="ae-table-td text-primary text-hover-success text-center pointer" @click="openById({compPath:'/a.Components/Common_BaseInfo_MetaInfoUpdate',recordKey:i.Id,refereshOnCallback:true,actionsAllowed:'DefaultRepo.Common_BaseInfo.MetaInfoUpdate',fkToParent:'ParentId'});">
									<i class="fa-solid fa-fw fa-edit"></i>
								</td>
								<td class="ae-table-td text-primary text-hover-success text-center pointer" @click="openById({compPath:'/a.Components/Common_BaseInfo_UiInfoUpdate',recordKey:i.Id,refereshOnCallback:true,actionsAllowed:'DefaultRepo.Common_BaseInfo.UiInfoUpdate',fkToParent:'ParentId'});">
									<i class="fa-solid fa-fw fa-edit"></i>
								</td>
								<td></td>
								<td style="width:40px;vertical-align:middle" class="text-center text-secondary text-hover-danger pointer" data-ae-actions="DefaultRepo.Common_BaseInfo.DeleteByKey" @click="deleteById({pkValue:i.Id})">
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
shared.setAppTitle(shared.translate("Common_BaseInfo, ReadList"));
let _this = { cid: "", c: null, templateType: "ReadList", dbConfName: "", objectName: "", loadMethod: "", deleteMethod: "", initialRequests: [], initialResponses: [], pickerRequests: [], pickerHumanIds: [], filter: {}, clientQueryMetadata: {}, orderClauses: [], orderableColumns: [] };
_this.dbConfName = "DefaultRepo";
_this.objectName = "Common_BaseInfo";
_this.loadMethod = "DefaultRepo.Common_BaseInfo.ReadList";
_this.deleteMethod = `${_this.dbConfName}.${_this.objectName}.DeleteByKey`;
_this.orderableColumns = ["CreatedOn","UpdatedOn","Title"];
_this.orderClauses = [{ Name: "CreatedOn", OrderDirection: "ASC" }];
_this.initialResponses = [{ Duration: 0, Result: { Master: [], Aggregations: [{ "Count": 0 }] } }];
_this.initialRequests = [genListRequest(_this.loadMethod, {}, _this.orderClauses, { PageNumber: 1, PageSize: 50 })];
_this.filter = {"ParentId":"","Title":null,"Id":null,"CreatedBy":null,"UpdatedBy":null,"ShortName":null,"ViewOrder":null,"Note":null,"Metadata":null,"IsActive":null,"UiColor":null,"UiIcon":null};
_this.initialSearchOptions = _.cloneDeep(_this.filter);
_this.clientQueryMetadata = {"ParentObjectColumns":[{"Name":"Id","DevNote":"","IsPrimaryKey":true,"DbType":"INT","IsIdentity":true,"IdentityStart":"10000","IdentityStep":"1","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":true,"ValidationRule":":=i(0,2147483647)"}},{"Name":"CreatedBy","DevNote":"","DbType":"INT","UpdateGroup":"","UiProps":{"Group":"Auditing","UiWidget":"DisabledTextbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":true,"ValidationRule":":=i(0,2147483647)"}},{"Name":"CreatedOn","DevNote":"","DbType":"DATETIME","IsSortable":true,"UpdateGroup":"","UiProps":{"Group":"Auditing","UiWidget":"DisabledTextbox","UiWidgetOptions":"{}","IsDisabled":true,"Required":true,"ValidationRule":"dt(1900-01-01 00:01:00,2100-12-30 11:59:59)"}},{"Name":"UpdatedBy","DevNote":"","DbType":"INT","AllowNull":true,"UpdateGroup":"IsActiveUpdate","UiProps":{"Group":"Auditing","UiWidget":"DisabledTextbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":false,"ValidationRule":":=i(0,2147483647)"}},{"Name":"UpdatedOn","DevNote":"","DbType":"DATETIME","AllowNull":true,"IsSortable":true,"UpdateGroup":"IsActiveUpdate","UiProps":{"Group":"Auditing","UiWidget":"DisabledTextbox","UiWidgetOptions":"{}","IsDisabled":true,"Required":false,"ValidationRule":"dt(1900-01-01 00:01:00,2100-12-30 11:59:59)"}},{"Name":"ParentId","DevNote":"","DbType":"INT","AllowNull":true,"Fk":{"FkName":"Common_BaseInfo_ParentId_Common_BaseInfo_Id","TargetTable":"Common_BaseInfo","TargetColumn":"Id","EnforceRelation":true,"Lookup":{"Id":"ParentId_Lookup","Method":"DefaultRepo.Common_BaseInfo.ReadList","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.Common_BaseInfo.ReadList","OrderClauses":[{"Name":"ViewOrder","OrderDirection":"ASC"}],"Pagination":{"PageNumber":1,"PageSize":500},"IncludeSubQueries":false}}}},"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"ObjectPicker","UiWidgetOptions":"{}","SearchType":"Fast","Required":false,"ValidationRule":":=i(0,2147483647)"}},{"Name":"Title","DevNote":"","DbType":"NVARCHAR","Size":"128","IsHumanId":true,"IsSortable":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Fast","Required":true,"ValidationRule":":=s(0,128)"}},{"Name":"ShortName","DevNote":"","DbType":"NVARCHAR","Size":"16","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,16)"}},{"Name":"ViewOrder","DevNote":"","DbType":"FLOAT","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false}},{"Name":"Note","DevNote":"","DbType":"NVARCHAR","Size":"256","AllowNull":true,"UpdateGroup":"MetaInfoUpdate","UiProps":{"Group":"","UiWidget":"Htmlbox","UiWidgetOptions":"{\n    \u0022svgPath\u0022: \u0022/a..lib/Trumbowyg/ui/icons.svg\u0022\n}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,256)"}},{"Name":"Metadata","DevNote":"","DbType":"NVARCHAR","Size":"4000","AllowNull":true,"UpdateGroup":"MetaInfoUpdate","UiProps":{"Group":"","UiWidget":"CodeEditorbox","UiWidgetOptions":"{\n    \u0022mode\u0022: \u0022ace/mode/json\u0022\n}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,4000)"}},{"Name":"IsActive","DevNote":"","DbType":"BIT","AllowNull":true,"UpdateGroup":"IsActiveUpdate","UiProps":{"Group":"","UiWidget":"Checkbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false}},{"Name":"UiColor","DevNote":"","DbType":"VARCHAR","Size":"16","AllowNull":true,"UpdateGroup":"UiInfoUpdate","UiProps":{"Group":"","UiWidget":"ColorPicker","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,16)"}},{"Name":"UiIcon","DevNote":"","DbType":"VARCHAR","Size":"64","AllowNull":true,"UpdateGroup":"UiInfoUpdate","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,64)"}}],"Name":"ReadList","Type":"ReadList","QueryColumns":["Id","CreatedBy","CreatedOn","UpdatedBy","UpdatedOn","ParentId","Title","ShortName","ViewOrder","Note","Metadata","IsActive","UiColor","UiIcon"],"FastSearchColumns":[{"Name":"ParentId","DevNote":"","DbType":"INT","AllowNull":true,"Fk":{"FkName":"Common_BaseInfo_ParentId_Common_BaseInfo_Id","TargetTable":"Common_BaseInfo","TargetColumn":"Id","EnforceRelation":true,"Lookup":{"Id":"ParentId_Lookup","Method":"DefaultRepo.Common_BaseInfo.ReadList","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.Common_BaseInfo.ReadList","OrderClauses":[{"Name":"ViewOrder","OrderDirection":"ASC"}],"Pagination":{"PageNumber":1,"PageSize":500},"IncludeSubQueries":false}}}},"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"ObjectPicker","UiWidgetOptions":"{}","SearchType":"Fast","Required":false,"ValidationRule":":=i(0,2147483647)"}},{"Name":"Title","DevNote":"","DbType":"NVARCHAR","Size":"128","IsHumanId":true,"IsSortable":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Fast","Required":true,"ValidationRule":":=s(0,128)"}}],"ExpandableSearchColumns":[{"Name":"Id","DevNote":"","IsPrimaryKey":true,"DbType":"INT","IsIdentity":true,"IdentityStart":"10000","IdentityStep":"1","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":true,"ValidationRule":":=i(0,2147483647)"}},{"Name":"CreatedBy","DevNote":"","DbType":"INT","UpdateGroup":"","UiProps":{"Group":"Auditing","UiWidget":"DisabledTextbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":true,"ValidationRule":":=i(0,2147483647)"}},{"Name":"UpdatedBy","DevNote":"","DbType":"INT","AllowNull":true,"UpdateGroup":"IsActiveUpdate","UiProps":{"Group":"Auditing","UiWidget":"DisabledTextbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":false,"ValidationRule":":=i(0,2147483647)"}},{"Name":"ShortName","DevNote":"","DbType":"NVARCHAR","Size":"16","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,16)"}},{"Name":"ViewOrder","DevNote":"","DbType":"FLOAT","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false}},{"Name":"Note","DevNote":"","DbType":"NVARCHAR","Size":"256","AllowNull":true,"UpdateGroup":"MetaInfoUpdate","UiProps":{"Group":"","UiWidget":"Htmlbox","UiWidgetOptions":"{\n    \u0022svgPath\u0022: \u0022/a..lib/Trumbowyg/ui/icons.svg\u0022\n}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,256)"}},{"Name":"Metadata","DevNote":"","DbType":"NVARCHAR","Size":"4000","AllowNull":true,"UpdateGroup":"MetaInfoUpdate","UiProps":{"Group":"","UiWidget":"CodeEditorbox","UiWidgetOptions":"{\n    \u0022mode\u0022: \u0022ace/mode/json\u0022\n}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,4000)"}},{"Name":"IsActive","DevNote":"","DbType":"BIT","AllowNull":true,"UpdateGroup":"IsActiveUpdate","UiProps":{"Group":"","UiWidget":"Checkbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false}},{"Name":"UiColor","DevNote":"","DbType":"VARCHAR","Size":"16","AllowNull":true,"UpdateGroup":"UiInfoUpdate","UiProps":{"Group":"","UiWidget":"ColorPicker","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,16)"}},{"Name":"UiIcon","DevNote":"","DbType":"VARCHAR","Size":"64","AllowNull":true,"UpdateGroup":"UiInfoUpdate","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,64)"}}],"OptionalQueries":[]};



_this.pickerRequests.push({"Id":"ParentId_Lookup","Method":"DefaultRepo.Common_BaseInfo.ReadList","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.Common_BaseInfo.ReadList","OrderClauses":[{"Name":"ViewOrder","OrderDirection":"ASC"}],"Pagination":{"PageNumber":1,"PageSize":500},"IncludeSubQueries":false}}});

_this.pickerHumanIds.push({Id:'ParentId_HumanIds',Items:["Title"]});
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