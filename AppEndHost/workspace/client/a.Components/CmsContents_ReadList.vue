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
						<select class="form-select form-select-sm" v-model="filter.ParentId" data-ae-validation-required="false">
							<option value="">{{shared.translate('ParentId')}}</option>
							<option v-for="i in shared.getResponseObjectById(initialRequests,initialResponses,filter,'ParentId_Lookup')" :value="i['Id']"></option>
						</select>
					</div>
					<div class="col-48 col-md-6">
						<select class="form-select form-select-sm" v-model="filter.ContentTypeId" data-ae-validation-required="false">
							<option value="">{{shared.translate('ContentTypeId')}}</option>
							<option v-for="i in shared.enum(150)" :value="i['Id']">{{i.Title}} {{i.TitleEn}} {{i.TitleFa}} {{i.TitleAr}}</option>
						</select>
					</div>
					<div class="col-48 col-md-6">
						<select class="form-select form-select-sm" v-model="filter.LanguageId" data-ae-validation-required="false">
							<option value="">{{shared.translate('LanguageId')}}</option>
							<option v-for="i in shared.enum(103)" :value="i['Id']">{{i.Title}} {{i.TitleEn}} {{i.TitleFa}} {{i.TitleAr}}</option>
						</select>
					</div>
					<div class="col-48 col-md-6">
						<select class="form-select form-select-sm" v-model="filter.RecordStateId" data-ae-validation-required="false">
							<option value="">{{shared.translate('RecordStateId')}}</option>
							<option v-for="i in shared.enum(101)" :value="i['Id']">{{i.Title}} {{i.TitleEn}} {{i.TitleFa}} {{i.TitleAr}}</option>
						</select>
					</div>
					<div class="col-48 col-md-6">
						<select class="form-select form-select-sm" v-model="filter.CommentsPolicyId" data-ae-validation-required="false">
							<option value="">{{shared.translate('CommentsPolicyId')}}</option>
							<option v-for="i in shared.enum(151)" :value="i['Id']">{{i.Title}} {{i.TitleEn}} {{i.TitleFa}} {{i.TitleAr}}</option>
						</select>
					</div>
					<div class="col-48 col-md-6">
						<input type="text" class="form-control form-control-sm" id="input_ViewOrder" @keyup.enter="loadRecords()" v-model="filter.ViewOrder" :placeholder="shared.translate('ViewOrder')">
					</div>
					<div class="col-48 col-md-6">
						<input type="text" class="form-control form-control-sm" id="input_ParentsIds" @keyup.enter="loadRecords()" v-model="filter.ParentsIds" :placeholder="shared.translate('ParentsIds')">
					</div>
					<div class="col-48 col-md-6">
						<input type="text" class="form-control form-control-sm" id="input_CreatedBy" @keyup.enter="loadRecords()" v-model="filter.CreatedBy" :placeholder="shared.translate('CreatedBy')">
					</div>
					<div class="col-48 col-md-6">
						<input type="text" class="form-control form-control-sm" id="input_UpdatedBy" @keyup.enter="loadRecords()" v-model="filter.UpdatedBy" :placeholder="shared.translate('UpdatedBy')">
					</div>
					<div class="col-48 col-md-6">
						<input type="text" class="form-control form-control-sm" id="input_Summary" @keyup.enter="loadRecords()" v-model="filter.Summary" :placeholder="shared.translate('Summary')">
					</div>
					<div class="col-48 col-md-6">
						<input type="text" class="form-control form-control-sm" id="input_Body" @keyup.enter="loadRecords()" v-model="filter.Body" :placeholder="shared.translate('Body')">
					</div>
					<div class="col-48 col-md-6">
						<input type="text" class="form-control form-control-sm" id="input_RecordStateIdUpdatedBy" @keyup.enter="loadRecords()" v-model="filter.RecordStateIdUpdatedBy" :placeholder="shared.translate('RecordStateIdUpdatedBy')">
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
				<button type="button" class="btn btn-sm border-0 btn-outline-success px-2" data-ae-actions="DefaultRepo.CmsContents.Create" @click="openCreate()">
					<i class="fa-solid fa-file-alt fa-bounce" style="--fa-animation-iteration-count:1"></i>
					<span>{{shared.translate("Create")}}</span>
				</button>
				<div class="vr"></div>
				<div class="btn btn-sm border-0 btn-outline-success px-2" data-ae-actions="DefaultRepo.CmsContents.ReadList" @click="exportExcel">
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
								<th class="sticky-top ae-thead-th text-center" style="width:95px;overflow: hidden;text-overflow: ellipsis;">
									<div>{{shared.translate("File")}}</div>
								</th>
								<th class="sticky-top ae-thead-th fw-bold text-success">
									<div>{{shared.translate("Title")}}</div>
								</th>
								<th class="sticky-top ae-thead-th text-center" style="width:95px;overflow: hidden;text-overflow: ellipsis;">
									<div>{{shared.translate("ParentId")}}</div>
								</th>
								<th class="sticky-top ae-thead-th " style="width:185px;">
									<div>{{shared.translate("ParentsIds")}}</div>
								</th>
								<th class="sticky-top ae-thead-th " style="width:185px;">
									<div>{{shared.translate("ContentTypeId")}}</div>
								</th>
								<th class="sticky-top ae-thead-th " style="width:185px;">
									<div>{{shared.translate("LanguageId")}}</div>
								</th>
								<th class="sticky-top ae-thead-th " style="width:185px;">
									<div>{{shared.translate("CommentsPolicyId")}}</div>
								</th>
								<th class="sticky-top ae-thead-th " style="width:185px;">
									<div>{{shared.translate("RecordStateId")}}</div>
								</th>
								<th style="width:40px;" class="sticky-top ae-thead-th text-center" data-ae-actions="DefaultRepo.CmsContents.DeleteByKey"></th>
							</tr>
						</thead>
						<tbody v-if="initialResponses[0].IsSucceeded===true">
							<tr v-for="i in initialResponses[0]['Result']['Master']">
								<td class="ae-table-td text-dark text-center" @click="openById({compPath:'/a.Components/CmsContents_UpdateByKey',recordKey:i.Id,refereshOnCallback:true,actionsAllowed:'DefaultRepo.CmsContents.UpdateByKey',fkToParent:''});">
									<div class="text-primary text-hover-success pointer">
										<i class="fa-solid fa-fw fa-edit"></i>
										<div class="pk font-monospace">{{i.Id}}</div>
									</div>
								</td>
								<td class="ae-table-td text-center">
									<img :src="'data:image/png;base64, '+i.File_FileBody_xs" v-if="shared.fixNull(i.File_FileBody_xs,'')!==''" class="rounded-4 shadow-sm" style="width:95%;min-height:50px;max-height:50px;max-width:50px;">
									<i class="fa-solid fa-fw fa-image fa-5x text-light" v-else=""></i>
								</td>
								<td class="ae-table-td">
									<div>{{i["Title"]}}</div>
								</td>
								<td class="ae-table-td text-center">
									<div>{{i["ParentId"]}}</div>
								</td>
								<td class="ae-table-td ">
									<div>{{i["ParentsIds"]}}</div>
								</td>
								<td class="ae-table-td ">
									<div class="text-dark fw-bold ">
										<div>{{shared.translate(i["ContentTypeId_Title"])}}</div>
									</div>
									<div class="text-muted fs-d7">{{i["ContentTypeId"]}}</div>
								</td>
								<td class="ae-table-td ">
									<div class="text-dark fw-bold ">
										<div>{{shared.translate(i["LanguageId_Title"])}}</div>
									</div>
									<div class="text-muted fs-d7">{{i["LanguageId"]}}</div>
								</td>
								<td class="ae-table-td ">
									<div class="text-dark fw-bold ">
										<div>{{shared.translate(i["CommentsPolicyId_Title"])}}</div>
									</div>
									<div class="text-muted fs-d7">{{i["CommentsPolicyId"]}}</div>
								</td>
								<td class="ae-table-td   pointer" @click="openById({compPath:'/a.Components/CmsContents_RecordStateIdUpdate',recordKey:i.Id,refereshOnCallback:true,actionsAllowed:'DefaultRepo.CmsContents.RecordStateIdUpdate',fkToParent:''});">
									<div class="input-group input-group-sm bg-hover-primary rounded-2 p-2">
										<div class="input-group-text border-0 bg-transparent me-1">
											<i class="fa-solid fa-fw fa-edit"></i>
										</div>
										<div class="more-info" style="">
											<table class="w-100 h-100 fs-d8">
												<tbody>
													<tr>
														<td class="text-muted align-middle" style="min-width:65px;">{{shared.translate("RecordStateId")}}</td>
														<td class="text-dark fw-bold align-middle">
															<span class="fw-bold">{{shared.fixNull(i["RecordStateId"],'-')}}</span>
														</td>
													</tr>
													<tr>
														<td class="text-muted align-middle" style="min-width:65px;">{{shared.translate("By")}}</td>
														<td class="text-dark fw-bold align-middle">
															<span class="fw-bold">{{shared.fixNull(i["RecordStateIdUpdatedBy"],'-')}}</span>
														</td>
													</tr>
													<tr>
														<td class="text-muted align-middle" style="min-width:65px;">{{shared.translate("On")}}</td>
														<td class="text-dark fw-bold align-middle">
															<span class="fw-bold">{{shared.fixNullOrEmpty(shared.formatDateL(i["RecordStateIdUpdatedOn"]),'-')}}</span>
														</td>
													</tr>
													<tr>
														<td class="text-muted align-middle" style="min-width:65px;">{{shared.translate("RecordStateId_Title")}}</td>
														<td class="text-dark fw-bold align-middle">
															<span class="fw-bold">{{shared.fixNull(i["RecordStateId_Title"],'-')}}</span>
														</td>
													</tr>
												</tbody>
											</table>
										</div>
									</div>
								</td>
								<td></td>
								<td style="width:40px;vertical-align:middle" class="text-center text-secondary text-hover-danger pointer" data-ae-actions="DefaultRepo.CmsContents.DeleteByKey" @click="deleteById({pkValue:i.Id})">
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
_this.objectName = "CmsContents";
_this.loadMethod = "DefaultRepo.CmsContents.ReadList";
_this.filePrefix = "";
_this.deleteMethod = `${_this.dbConfName}.${_this.objectName}.DeleteByKey`;
_this.orderableColumns = ["CreatedOn","UpdatedOn","Title"];
_this.orderClauses = [{ Name: "CreatedOn", OrderDirection: "ASC" }];
_this.initialResponses = [{ Duration: 0, Result: { Master: [], Aggregations: [{ "Count": 0 }] } }];
_this.initialRequests = [genListRequest(_this.loadMethod, {}, _this.orderClauses, { PageNumber: 1, PageSize: 10 })];
_this.filter = {"Title":null,"Id":null,"ParentId":null,"ViewOrder":null,"ParentsIds":null,"CreatedBy":null,"UpdatedBy":null,"ContentTypeId":"","Summary":null,"Body":null,"File_FileName":null,"File_FileSize":null,"File_FileMime":null,"LanguageId":"","RecordStateId":"","RecordStateIdUpdatedBy":null,"CommentsPolicyId":""};
_this.initialSearchOptions = _.cloneDeep(_this.filter);
_this.clientQueryMetadata = {"ParentObjectColumns":[{"Name":"Id","DevNote":"","IsPrimaryKey":true,"DbType":"INT","IsIdentity":true,"IdentityStart":"100000000","IdentityStep":"1","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":true,"ValidationRule":":=i(0,2147483647)"}},{"Name":"ParentId","DevNote":"","DbType":"INT","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Combo","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=i(0,2147483647)"}},{"Name":"ViewOrder","DevNote":"","DbType":"INT","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=i(0,2147483647)"}},{"Name":"ParentsIds","DevNote":"","DbType":"VARCHAR","Size":"128","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,128)"}},{"Name":"CreatedBy","DevNote":"","DbType":"INT","UpdateGroup":"","UiProps":{"Group":"Auditing","UiWidget":"DisabledTextbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":true,"ValidationRule":":=i(0,2147483647)"}},{"Name":"CreatedOn","DevNote":"","DbType":"DATETIME","IsSortable":true,"UpdateGroup":"","UiProps":{"Group":"Auditing","UiWidget":"DisabledTextbox","UiWidgetOptions":"{}","IsDisabled":true,"Required":true,"ValidationRule":"dt(1900-01-01 00:01:00,2100-12-30 11:59:59)"}},{"Name":"UpdatedBy","DevNote":"","DbType":"INT","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"Auditing","UiWidget":"DisabledTextbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":false,"ValidationRule":":=i(0,2147483647)"}},{"Name":"UpdatedOn","DevNote":"","DbType":"DATETIME","AllowNull":true,"IsSortable":true,"UpdateGroup":"","UiProps":{"Group":"Auditing","UiWidget":"DisabledTextbox","UiWidgetOptions":"{}","IsDisabled":true,"Required":false,"ValidationRule":"dt(1900-01-01 00:01:00,2100-12-30 11:59:59)"}},{"Name":"ContentTypeId","DevNote":"","DbType":"VARCHAR","Size":"64","AllowNull":true,"Fk":{"FkName":"CmsContents_ContentTypeId_BaseInfo_Id","TargetTable":"BaseInfo","TargetColumn":"Id","Lookup":{"Id":"","Method":""},"JsLookupParentId":"150"},"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Combo","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,64)"}},{"Name":"Title","DevNote":"","DbType":"NVARCHAR","Size":"256","IsHumanId":true,"IsSortable":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Fast","Required":true,"ValidationRule":":=s(0,256)"}},{"Name":"Summary","DevNote":"","DbType":"NVARCHAR","Size":"4000","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"MultilineTextbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,4000)"}},{"Name":"Body","DevNote":"","DbType":"NTEXT","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Htmlbox","UiWidgetOptions":"{\n    \u0022svgPath\u0022: \u0022/a..lib/Trumbowyg/ui/icons.svg\u0022\n}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,256)"}},{"Name":"File_FileBody","DevNote":"","DbType":"IMAGE","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"FileView","UiWidgetOptions":"{}","Required":true}},{"Name":"File_FileBody_xs","DevNote":"","DbType":"IMAGE","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"FileView","UiWidgetOptions":"{}","Required":true}},{"Name":"File_FileName","DevNote":"","DbType":"NVARCHAR","Size":"128","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true,"ValidationRule":":=s(0,128)"}},{"Name":"File_FileSize","DevNote":"","DbType":"INT","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true,"ValidationRule":":=i(0,2147483647)"}},{"Name":"File_FileMime","DevNote":"","DbType":"VARCHAR","Size":"128","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true,"ValidationRule":":=s(0,128)"}},{"Name":"LanguageId","DevNote":"","DbType":"VARCHAR","Size":"64","AllowNull":true,"Fk":{"FkName":"CmsContents_LanguageId_BaseInfo_Id","TargetTable":"BaseInfo","TargetColumn":"Id","Lookup":{"Id":"","Method":""},"JsLookupParentId":"103"},"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Combo","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,64)"}},{"Name":"RecordStateId","DevNote":"","DbType":"VARCHAR","Size":"64","AllowNull":true,"Fk":{"FkName":"CmsContents_RecordStateId_BaseInfo_Id","TargetTable":"BaseInfo","TargetColumn":"Id","Lookup":{"Id":"","Method":""},"JsLookupParentId":"101"},"UpdateGroup":"RecordStateIdUpdate","UiProps":{"Group":"","UiWidget":"Combo","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,64)"}},{"Name":"RecordStateIdUpdatedBy","DevNote":"","DbType":"INT","AllowNull":true,"UpdateGroup":"RecordStateIdUpdate","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=i(0,2147483647)"}},{"Name":"RecordStateIdUpdatedOn","DevNote":"","DbType":"DATETIME","AllowNull":true,"UpdateGroup":"RecordStateIdUpdate","UiProps":{"Group":"","UiWidget":"DateTimePicker","UiWidgetOptions":"{}","Required":false,"ValidationRule":"dt(1900-01-01 00:01:00,2100-12-30 11:59:59)"}},{"Name":"CommentsPolicyId","DevNote":"","DbType":"VARCHAR","Size":"64","AllowNull":true,"Fk":{"FkName":"CmsContents_CommentsPolicyId_BaseInfo_Id","TargetTable":"BaseInfo","TargetColumn":"Id","Lookup":{"Id":"","Method":""},"JsLookupParentId":"151"},"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Combo","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,64)"}}],"Name":"ReadList","Type":"ReadList","QueryColumns":["Id","ParentId","ViewOrder","ParentsIds","CreatedBy","CreatedOn","UpdatedBy","UpdatedOn","ContentTypeId","Title","File_FileBody_xs","LanguageId","RecordStateId","RecordStateIdUpdatedBy","RecordStateIdUpdatedOn","CommentsPolicyId"],"FastSearchColumns":[{"Name":"Title","DevNote":"","DbType":"NVARCHAR","Size":"256","IsHumanId":true,"IsSortable":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Fast","Required":true,"ValidationRule":":=s(0,256)"}}],"ExpandableSearchColumns":[{"Name":"Id","DevNote":"","IsPrimaryKey":true,"DbType":"INT","IsIdentity":true,"IdentityStart":"100000000","IdentityStep":"1","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":true,"ValidationRule":":=i(0,2147483647)"}},{"Name":"ParentId","DevNote":"","DbType":"INT","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Combo","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=i(0,2147483647)"}},{"Name":"ViewOrder","DevNote":"","DbType":"INT","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=i(0,2147483647)"}},{"Name":"ParentsIds","DevNote":"","DbType":"VARCHAR","Size":"128","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,128)"}},{"Name":"CreatedBy","DevNote":"","DbType":"INT","UpdateGroup":"","UiProps":{"Group":"Auditing","UiWidget":"DisabledTextbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":true,"ValidationRule":":=i(0,2147483647)"}},{"Name":"UpdatedBy","DevNote":"","DbType":"INT","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"Auditing","UiWidget":"DisabledTextbox","UiWidgetOptions":"{}","SearchType":"Expandable","IsDisabled":true,"Required":false,"ValidationRule":":=i(0,2147483647)"}},{"Name":"ContentTypeId","DevNote":"","DbType":"VARCHAR","Size":"64","AllowNull":true,"Fk":{"FkName":"CmsContents_ContentTypeId_BaseInfo_Id","TargetTable":"BaseInfo","TargetColumn":"Id","Lookup":{"Id":"","Method":""},"JsLookupParentId":"150"},"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Combo","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,64)"}},{"Name":"Summary","DevNote":"","DbType":"NVARCHAR","Size":"4000","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"MultilineTextbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,4000)"}},{"Name":"Body","DevNote":"","DbType":"NTEXT","AllowNull":true,"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Htmlbox","UiWidgetOptions":"{\n    \u0022svgPath\u0022: \u0022/a..lib/Trumbowyg/ui/icons.svg\u0022\n}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,256)"}},{"Name":"File_FileName","DevNote":"","DbType":"NVARCHAR","Size":"128","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true,"ValidationRule":":=s(0,128)"}},{"Name":"File_FileSize","DevNote":"","DbType":"INT","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true,"ValidationRule":":=i(0,2147483647)"}},{"Name":"File_FileMime","DevNote":"","DbType":"VARCHAR","Size":"128","UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":true,"ValidationRule":":=s(0,128)"}},{"Name":"LanguageId","DevNote":"","DbType":"VARCHAR","Size":"64","AllowNull":true,"Fk":{"FkName":"CmsContents_LanguageId_BaseInfo_Id","TargetTable":"BaseInfo","TargetColumn":"Id","Lookup":{"Id":"","Method":""},"JsLookupParentId":"103"},"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Combo","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,64)"}},{"Name":"RecordStateId","DevNote":"","DbType":"VARCHAR","Size":"64","AllowNull":true,"Fk":{"FkName":"CmsContents_RecordStateId_BaseInfo_Id","TargetTable":"BaseInfo","TargetColumn":"Id","Lookup":{"Id":"","Method":""},"JsLookupParentId":"101"},"UpdateGroup":"RecordStateIdUpdate","UiProps":{"Group":"","UiWidget":"Combo","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,64)"}},{"Name":"RecordStateIdUpdatedBy","DevNote":"","DbType":"INT","AllowNull":true,"UpdateGroup":"RecordStateIdUpdate","UiProps":{"Group":"","UiWidget":"Textbox","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=i(0,2147483647)"}},{"Name":"CommentsPolicyId","DevNote":"","DbType":"VARCHAR","Size":"64","AllowNull":true,"Fk":{"FkName":"CmsContents_CommentsPolicyId_BaseInfo_Id","TargetTable":"BaseInfo","TargetColumn":"Id","Lookup":{"Id":"","Method":""},"JsLookupParentId":"151"},"UpdateGroup":"","UiProps":{"Group":"","UiWidget":"Combo","UiWidgetOptions":"{}","SearchType":"Expandable","Required":false,"ValidationRule":":=s(0,64)"}}],"OptionalQueries":[]};


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