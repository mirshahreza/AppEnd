<template>
<div class="card h-100 bg-transparent rounded-0 border-0">
		<div class="card-header p-2 bg-success-subtle rounded-0 border-0" v-if="ismodal!=='true'">
			<div class="hstack">
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
						<div class="col-48">
							<label class="fs-d8 text-muted ms-2" for="input_ShortName">{{shared.translate('ShortName')}}</label>
							<input type="text" class="form-control form-control-sm" id="input_ShortName" v-model="row.ShortName" data-ae-validation-required="false" data-ae-validation-rule=":=s(0,16)">
						</div>
					</div>
					<div class="row">
						<div class="col-48">
							<label class="fs-d8 text-muted ms-2" for="input_Title">{{shared.translate('Title')}}</label>
							<input type="text" class="form-control form-control-sm" id="input_Title" v-model="row.Title" data-ae-validation-required="true" data-ae-validation-rule=":=s(0,128)">
						</div>
					</div>
					<div class="row">
						<div class="col-48">
							<label class="fs-d8 text-muted ms-2" for="input_TitleEn">{{shared.translate('TitleEn')}}</label>
							<input type="text" class="form-control form-control-sm" id="input_TitleEn" v-model="row.TitleEn" data-ae-validation-required="true" data-ae-validation-rule=":=s(0,128)">
						</div>
					</div>
					<div class="row">
						<div class="col-48">
							<label class="fs-d8 text-muted ms-2" for="input_TitleFa">{{shared.translate('TitleFa')}}</label>
							<input type="text" class="form-control form-control-sm" id="input_TitleFa" v-model="row.TitleFa" data-ae-validation-required="true" data-ae-validation-rule=":=s(0,128)">
						</div>
					</div>
					<div class="row">
						<div class="col-48">
							<label class="fs-d8 text-muted ms-2" for="input_TitleAr">{{shared.translate('TitleAr')}}</label>
							<input type="text" class="form-control form-control-sm" id="input_TitleAr" v-model="row.TitleAr" data-ae-validation-required="true" data-ae-validation-rule=":=s(0,128)">
						</div>
					</div>
				</div>
			</div>
			<div class="card rounded-1 border-light mb-1">
				<div class="card-body">
					<div class="row">
						<div class="col-48" v-if="inputs.fkColumn!=='ParentId'">
							<label class="fs-d8 text-muted ms-2" for="input_ParentId">{{shared.translate('ParentId')}}</label>
							<div class="form-control form-control-sm py-0 data-ae-validation ">
								<div class="input-group input-group-sm p-0 pt-1" data-ae-widget="objectPicker">
									<input type="hidden" v-model="row.ParentId" data-ae-validation-required="false">
									<input type="hidden" v-model="row.ParentId_Title">
									<input type="text" class="form-control bg-transparent p-0 m-0 border-0" :value="shared.fixNull(row.ParentId+' '+row.ParentId_Title,'',true)" :placeholder="shared.translate('ParentId')" disabled="">
									<span></span>
									<span class="mx-1 text-hover-primary ae-objectpicker-search pointer" @click="openPicker({colName:'ParentId'})">
										<i class="fa-solid fa-hand-pointer"></i>
									</span>
									<span class="mx-1 text-hover-primary ae-objectpicker-clear pointer">
										<i class="fa-solid fa-times"></i>
									</span>
								</div>
							</div>
						</div>
						<div class="col-48" v-if="inputs.fkColumn!=='ViewOrder'">
							<label class="fs-d8 text-muted ms-2" for="input_ViewOrder">{{shared.translate('ViewOrder')}}</label>
							<input type="text" class="form-control form-control-sm" id="input_ViewOrder" v-model="row.ViewOrder" data-ae-validation-required="false" data-ae-validation-rule="">
						</div>
						<div class="col-48" v-if="inputs.fkColumn!=='Value'">
							<label class="fs-d8 text-muted ms-2" for="input_Value">{{shared.translate('Value')}}</label>
							<input type="text" class="form-control form-control-sm" id="input_Value" v-model="row.Value" data-ae-validation-required="false" data-ae-validation-rule=":=i(0,2147483647)">
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
											<button class="btn btn-sm btn-outline-primary" @click="addRelation({relName:'BaseInfo'});">
												<i class="fa-solid fa-fw fa-plus"></i>
												{{shared.translate("AddRow")}}</button>
										</td>
										<td class="text-end">
											<span class="fw-bold text-dark fs-d9">{{Relations['BaseInfo'].length}}</span>
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
										<th class="sticky-top ae-thead-th fb text-primary fw-bold text-center" style="width:125px;overflow: hidden;text-overflow: ellipsis;">
											<i class="fa-solid fa-fw fa-window-restore"></i>
										</th>
										<th class="sticky-top ae-thead-th fb text-success">
											<div>{{shared.translate("Title")}}</div>
										</th>
										<th class="sticky-top ae-thead-th text-center">
											<div>{{shared.translate("IsActive")}}</div>
										</th>
										<th class="sticky-top ae-thead-th"></th>
										
										<th class="sticky-top ae-thead-th text-center" style="width:40px;" data-ae-actions="DefaultRepo.BaseInfo.DeleteByKey"></th>
									</tr>
								</thead>
								<tbody>
									<tr v-for="(i,ind) in Relations['BaseInfo']">
										<td class="ae-table-td text-dark text-center" style="" @click="openById({compPath:'/a.Components/BaseInfo_UpdateByKey',recordKey:i.Id,refereshOnCallback:true,actionsAllowed:'DefaultRepo.BaseInfo.UpdateByKey',fkToParent:'ParentId'});">
											<div class="text-primary text-hover-success pointer">
												<i class="fa-solid fa-fw fa-edit"></i>
												<br>
												<span class="pk">{{i.Id}}</span>
											</div>
										</td>
										<td class="ae-table-td" style="">
											<div>
												<span>{{shared.fixNull(i["Title"],'-')}}</span>
											</div>
										</td>
										<td class="ae-table-td text-center pointer">
											<div class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="openById({compPath:'/a.Components/BaseInfo_IsActiveUpdate',recordKey:i.Id,refereshOnCallback:true,actionsAllowed:'DefaultRepo.BaseInfo.IsActiveUpdate',fkToParent:'ParentId'});">
												<span v-html="shared.convertBoolToIconWithOptions(i.IsActive ,{})"></span>
											</div>
										</td>
										<td class="ae-table-td pointer" style="">
											<div class="pointer text-hover-primary" @click="openById({compPath:'/a.Components/BaseInfo_MetaInfoUpdate',recordKey:i.Id,refereshOnCallback:true,actionsAllowed:'DefaultRepo.BaseInfo.MetaInfoUpdate',fkToParent:'ParentId'});">
												<i class="fa-solid fa-fw fa-edit"></i> {{shared.translate("MetaInfo")}}
											</div>
											<div class="pointer text-hover-primary" @click="openById({compPath:'/a.Components/BaseInfo_UiInfoUpdate',recordKey:i.Id,refereshOnCallback:true,actionsAllowed:'DefaultRepo.BaseInfo.UiInfoUpdate',fkToParent:'ParentId'});">
												<i class="fa-solid fa-fw fa-edit"></i> {{shared.translate("UiInfo")}}
											</div>
										</td>
										<td class="text-center" style="width:40px;vertical-align:middle" data-ae-actions="DefaultRepo.BaseInfo.DeleteByKey">
											<span @click="deleteRelation({relationTable:'BaseInfo',ind:ind})">
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
		<div class="card-footer p-0" v-if="ismodal==='true'">
            <div class="container-fluid pt-2 pb-1">
                <div class="row p-0">
                    <div class="col-36 px-2">
                        <button class="btn btn-sm btn-primary w-100" @click="ok" data-ae-key="ok">
                            <i class="fa-solid fa-check me-1"></i>
                            <span>{{shared.translate("Save")}}</span>
                        </button>
                    </div>
                    <div class="col-12 px-2">
                        <button class="btn btn-sm btn-secondary w-100" @click="cancel">
                            <i class="fa-solid fa-xmark me-1"></i>
                            <span>{{shared.translate("Cancel")}}</span>
                        </button>
                    </div>
                </div>
            </div>
		</div>
	</div>
</template>
<script>
let _this = { cid: "", ismodal:"", c: null, templateType:"UpdateByKey", inputs: {}, dbConfName: "", objectName: "", loadMethod: "", submitMethod: "", masterRequest: {}, initialRequests: [], pickerRequests: [], pickerHumanIds: [], initialResponses: [], row: {}, Relations: {}, RelationsMetaData: {}, createComponent: "", updateComponent: "", regulator: null };
_this.dbConfName = "DefaultRepo";
_this.objectName = "BaseInfo";
_this.submitMethod = "UpdateByKey";
_this.createComponent = ""; 
_this.updateComponent = "";

_this.masterRequest = {"Id":"","Method":"DefaultRepo.BaseInfo.ReadByKey","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.BaseInfo.ReadByKey","Params":[{"Name":"Id","Value":""}]}}};





_this.Relations['BaseInfo']=[];

_this.RelationsMetaData['Children']={"RelationName":"Children","RelationTable":"BaseInfo","RelationPkColumn":"Id","RelationFkColumn":"ParentId","RelationType":"OneToMany","CreateQuery":"Create","ReadListQuery":"ReadList","UpdateByKeyQuery":"UpdateByKey","DeleteByKeyQuery":"DeleteByKey","DeleteQuery":"Delete","IsFileCentric":false,"RelationUiWidget":"Grid"};

_this.RelationsMetaData['Children']['createComponent']='/a.Components/BaseInfo_Create';

_this.RelationsMetaData['Children']['updateComponent']='/a.Components/BaseInfo_UpdateByKey';

_this.pickerRequests.push({"Id":"ParentId_Lookup","Method":"DefaultRepo.BaseInfo.ReadList","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.BaseInfo.ReadList","OrderClauses":[{"Name":"ViewOrder","OrderDirection":"ASC"}],"Pagination":{"PageNumber":1,"PageSize":500},"IncludeSubQueries":false}}});

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
	mounted() { initVueComponent(_this); _this.c.loadMasterRecord(); _this.c.componentFinalization(); },
	props: { cid: String, ismodal: String }
}


</script>