<template>
<div class="card h-100 bg-transparent rounded-0 border-0">
		<div class="card-header p-2 bg-success-subtle rounded-0 border-0" v-if="ismodal!=='true'">
			<div class="hstack gap-1">
				<button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="ok" data-ae-key="ok">
					<i class="fa-solid fa-save pe-1"></i>
					<span>{{shared.translate("Save")}}</span>
				</button>
				<div class="p-0 ms-auto"></div>
			</div>
		</div>
		<div class="card-body bg-primary-subtle-light scrollable">
			<div class="card rounded-1 border-light mb-1">
				<div class="card-body">
					<div class="row">
						<div class="col-48" v-if="inputs.fkColumn!=='Title'">
							<label class="fs-d8 text-muted ms-2" for="input_Title">{{shared.translate('Title')}}</label>
							<input type="text" class="form-control form-control-sm" id="input_Title" v-model="row.Title" data-ae-validation-required="true" data-ae-validation-rule=":=s(0,128)">
						</div>
					</div>
				</div>
			</div>
			<div class="card rounded-1 border-light mb-1">
				<div class="card-body">
					<div class="row">
						<div class="col-48" v-if="inputs.fkColumn!=='ParentId'">
							<label class="fs-d8 text-muted ms-2" for="input_ParentId">{{shared.translate('ParentId')}}</label>
							<div class="form-control p-0 px-1 data-ae-validation ">
								<div class="input-group" data-ae-widget="objectPicker">
									<input type="hidden" v-model="row.ParentId" data-ae-validation-required="false">
									<input type="hidden" v-model="row.ParentId_Title">
									<input type="text" class="form-control bg-transparent border-0" :value="shared.fixNull(row.ParentId+' '+row.ParentId_Title,'',true)" :placeholder="shared.translate('ParentId')" disabled="">
									<button class="btn btn-outline-secondary bg-transparent border-0 text-hover-primary ae-objectpicker-search" type="button" @click="openPicker({colName:'ParentId'})">
										<i class="fa-solid fa-search"></i>
									</button>
									<button class="btn btn-outline-secondary bg-transparent border-0 text-hover-danger ae-objectpicker-clear" type="button">
										<i class="fa-solid fa-times"></i>
									</button>
								</div>
							</div>
						</div>
						<div class="col-48" v-if="inputs.fkColumn!=='ShortName'">
							<label class="fs-d8 text-muted ms-2" for="input_ShortName">{{shared.translate('ShortName')}}</label>
							<input type="text" class="form-control form-control-sm" id="input_ShortName" v-model="row.ShortName" data-ae-validation-required="false" data-ae-validation-rule=":=s(0,16)">
						</div>
						<div class="col-48" v-if="inputs.fkColumn!=='ViewOrder'">
							<label class="fs-d8 text-muted ms-2" for="input_ViewOrder">{{shared.translate('ViewOrder')}}</label>
							<input type="text" class="form-control form-control-sm" id="input_ViewOrder" v-model="row.ViewOrder" data-ae-validation-required="false" data-ae-validation-rule="">
						</div>
					</div>
				</div>
			</div>
			<div class="card rounded-1 border-light mb-1">
				<div class="card-body">
					<div class="row">
						<div class="col-48">
							<label class="fs-d8 text-muted ms-2" for="input_UpdatedBy">{{shared.translate('UpdatedBy')}}</label>
							<input disabled="" type="text" class="form-control form-control-sm" id="input_UpdatedBy" v-model="row.UpdatedBy" data-ae-validation-required="false" data-ae-validation-rule=":=i(0,2147483647)">
						</div>
						<div class="col-48">
							<label class="fs-d8 text-muted ms-2" for="input_UpdatedOn">{{shared.translate('UpdatedOn')}}</label>
							<input disabled="" type="text" class="form-control form-control-sm" id="input_UpdatedOn" v-model="row.UpdatedOn" data-ae-validation-required="false" data-ae-validation-rule="dt(1900-01-01 00:01:00,2100-12-30 11:59:59)">
						</div>
					</div>
				</div>
			</div>
			<div class="col-48">
				<div class="col-48">
					<div class="card mt-3">
						<div class="card-header">
							<table class="w-100">
								<tbody>
									<tr>
										<td class="text-start">
											<button class="btn btn-sm btn-outline-primary" @click="addRelation({relName:'Common_BaseInfo'});">
												<i class="fa-solid fa-fw fa-plus"></i>
												{{shared.translate("AddRow")}}</button>
										</td>
										<td class="text-end">
											<span class="fw-bold text-dark fs-d9">{{Relations['Common_BaseInfo'].length}}</span>
											<span class="fw-bold text-secondary fs-d8">
												row(s)</span>
										</td>
									</tr>
								</tbody>
							</table>
						</div>
						<div class="card-body p-0 data-ae-filearea data-ae-validation" data-ae-validation-required="false" data-ae-validation-rule=":=n(0)">
							<table class="table table-sm table-hover w-100 ae-table m-0 bg-transparent fs-d8">
								<thead>
									<tr class="d-none d-md-table-row d-lg-table-row d-xl-table-row">
										<th class="sticky-top ae-thead-th fb text-primary fw-bold text-center" style="width:75px;overflow: hidden;text-overflow: ellipsis;">
											<i class="fa-solid fa-fw fa-window-restore"></i>
										</th>
										<th class="sticky-top ae-thead-th fb text-success" style="min-width:185px;">
											<div>{{shared.translate("HumanIds")}}</div>
										</th>
										<th class="sticky-top ae-thead-th text-center" style="width:75px;overflow: hidden;text-overflow: ellipsis;">
											<div>{{shared.translate("ViewOrder")}}</div>
										</th>
										<th class="sticky-top ae-thead-th " style="width:160px;">
											<div>{{shared.translate("IsActive")}}</div>
										</th>
										<th class="sticky-top ae-thead-th " style="width:40px;">
											<div>{{shared.translate("MetaInfo")}}</div>
										</th>
										<th class="sticky-top ae-thead-th " style="width:40px;">
											<div>{{shared.translate("UiInfo")}}</div>
										</th>
										<td style="width:40px;" class="sticky-top ae-thead-th text-center" data-ae-actions="DefaultRepo.Common_BaseInfo.DeleteByKey"></td>
									</tr>
								</thead>
								<tbody>
									<tr v-for="(i,ind) in Relations['Common_BaseInfo']">
										<td class="ae-table-td text-dark text-center" style="width:75px;overflow: hidden;text-overflow: ellipsis;" @click="openById({compPath:'/a.Components/Common_BaseInfo_UpdateByKey',recordKey:i.Id,refereshOnCallback:true,actionsAllowed:'DefaultRepo.Common_BaseInfo.UpdateByKey',fkToParent:'ParentId'});">
											<div class="text-primary text-hover-success pointer">
												<i class="fa-solid fa-fw fa-edit"></i>
												<br>
												<span class="pk">{{i.Id}}</span>
											</div>
										</td>
										<td class="ae-table-td" style="min-width:185px;">
											<div>
												<span class="badge text-muted fs-d8 text-start">{{shared.translate("Title")}}</span>
												<span class="fw-bold">
													<span>{{shared.fixNull(i["Title"],'-')}}</span>
												</span>
											</div>
										</td>
										<td class="ae-table-td text-center" style="overflow: hidden;text-overflow: ellipsis;">
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
										<td class="ae-table-td   pointer" @click="openById({compPath:'/a.Components/Common_BaseInfo_MetaInfoUpdate',recordKey:i.Id,refereshOnCallback:true,actionsAllowed:'DefaultRepo.Common_BaseInfo.MetaInfoUpdate',fkToParent:'ParentId'});">
											<div class="input-group input-group-sm bg-hover-primary rounded-2 p-2">
												<div class="input-group-text border-0 bg-transparent me-1">
													<i class="fa-solid fa-fw fa-edit"></i>
												</div>
												<div class="more-info" style="">
													<table class="w-100 h-100 fs-d8">
													</table>
												</div>
											</div>
										</td>
										<td class="ae-table-td   pointer" @click="openById({compPath:'/a.Components/Common_BaseInfo_UiInfoUpdate',recordKey:i.Id,refereshOnCallback:true,actionsAllowed:'DefaultRepo.Common_BaseInfo.UiInfoUpdate',fkToParent:'ParentId'});">
											<div class="input-group input-group-sm bg-hover-primary rounded-2 p-2">
												<div class="input-group-text border-0 bg-transparent me-1">
													<i class="fa-solid fa-fw fa-edit"></i>
												</div>
												<div class="more-info" style="">
													<table class="w-100 h-100 fs-d8">
													</table>
												</div>
											</div>
										</td>
										<td style="width:40px;vertical-align:middle" class="text-center" data-ae-actions="DefaultRepo.Common_BaseInfo.DeleteByKey">
											<span @click="deleteRelation({relationTable:'Common_BaseInfo',ind:ind})">
												<i class="fa-solid fa-fw fa-times text-muted hover-danger pointer"></i>
											</span>
										</td>
									</tr>
								</tbody>
							</table>
						</div>
					</div>
				</div>
			</div>
		</div>
		<div class="card-footer p-0 bg-secondary-subtle bg-gradient border-0 rounded-0" v-if="ismodal==='true'">
			<button class="btn btn-link text-decoration-none bg-hover-light w-100 py-3 rounded-0" @click="ok" data-ae-key="ok">
				<i class="fa-solid fa-save pe-1"></i>
				<span>{{shared.translate("Save")}}</span>
			</button>
		</div>
	</div>
</template>
<script>
let _this = { cid: "", ismodal:"", c: null, templateType:"UpdateByKey", inputs: {}, dbConfName: "", objectName: "", loadMethod: "", submitMethod: "", masterRequest: {}, initialRequests: [], pickerRequests: [], pickerHumanIds: [], initialResponses: [], row: {}, Relations: {}, RelationsMetaData: {}, createComponent: "", updateComponent: "", regulator: null };
_this.dbConfName = "DefaultRepo";
_this.objectName = "Common_BaseInfo";
_this.submitMethod = "UpdateByKey";
_this.createComponent = ""; 
_this.updateComponent = "";

_this.masterRequest = {"Id":"","Method":"DefaultRepo.Common_BaseInfo.ReadByKey","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.Common_BaseInfo.ReadByKey","Params":[{"Name":"Id","Value":""}]}}};





_this.Relations['Common_BaseInfo']=[];

_this.RelationsMetaData['Children']={"RelationName":"Children","RelationTable":"Common_BaseInfo","RelationPkColumn":"Id","RelationFkColumn":"ParentId","RelationType":"OneToMany","CreateQuery":"Create","ReadListQuery":"ReadList","UpdateByKeyQuery":"UpdateByKey","DeleteByKeyQuery":"DeleteByKey","DeleteQuery":"Delete","IsFileCentric":false,"RelationUiWidget":"Grid"};

_this.RelationsMetaData['Children']['createComponent']='/a.Components/Common_BaseInfo_Create';

_this.RelationsMetaData['Children']['updateComponent']='/a.Components/Common_BaseInfo_UpdateByKey';


_this.pickerRequests.push({"Id":"ParentId_Lookup","Method":"DefaultRepo.Common_BaseInfo.ReadList","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.Common_BaseInfo.ReadList","OrderClauses":[{"Name":"ViewOrder","OrderDirection":"ASC"}],"Pagination":{"PageNumber":1,"PageSize":500},"IncludeSubQueries":false}}});

_this.pickerHumanIds.push({Id:'ParentId_HumanIds',Items:["Title"]});

export default {
	methods: {
	},
	setup(props) {
		_this.cid = props['cid'];
		_this.ismodal = props['ismodal'];
		_this.inputs = shared["params_" + _this.cid];
		if(fixNull(getQueryString("key"),'') !== '') {
			_this.inputs = {};
			_this.inputs["key"] = getQueryString("key");
		}
	},
	data() { return _this; },
	created() { _this.c = this; assignDefaultMethods(_this); },
	mounted() { initVueComponent(_this); _this.c.loadBaseInfo(); _this.c.loadMasterRecord(); _this.c.componentFinalization(); },
	props: { cid: String, ismodal: String }
}


</script>