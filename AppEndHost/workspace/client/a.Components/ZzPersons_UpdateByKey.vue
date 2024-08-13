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
						<div class="col-48" v-if="inputs.fkColumn!=='FirstName'">
							<label class="fs-d8 text-muted ms-2" for="input_FirstName">{{shared.translate('FirstName')}}</label>
							<input type="text" class="form-control form-control-sm" id="input_FirstName" v-model="row.FirstName" data-ae-validation-required="false" data-ae-validation-rule=":=s(0,64)">
						</div>
						<div class="col-48" v-if="inputs.fkColumn!=='LastName'">
							<label class="fs-d8 text-muted ms-2" for="input_LastName">{{shared.translate('LastName')}}</label>
							<input type="text" class="form-control form-control-sm" id="input_LastName" v-model="row.LastName" data-ae-validation-required="false" data-ae-validation-rule=":=s(0,64)">
						</div>
					</div>
				</div>
			</div>
			<div class="card rounded-1 border-light mb-1">
				<div class="card-body">
					<div class="row">
						<div class="col-48" v-if="inputs.fkColumn!=='GenderId'">
							<label class="fs-d8 text-muted ms-2" for="input_GenderId">{{shared.translate('GenderId')}}</label>
							<select class="form-select form-select-sm" v-model="row.GenderId" data-ae-validation-required="false">
								<option value="">-</option>
								<option v-for="i in shared.enum(10000)" :value="i['Id']">{{i.Title}}</option>
							</select>
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
											<button class="btn btn-sm btn-outline-primary" @click="addRelation({relName:'ZzPhones'});">
												<i class="fa-solid fa-fw fa-plus"></i>
												{{shared.translate("AddRow")}}</button>
										</td>
										<td class="text-end">
											<span class="fw-bold text-dark fs-d9">{{Relations['ZzPhones'].length}}</span>
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
										<th class="sticky-top ae-thead-th fb text-success" style="width:185px;">
											<div>{{shared.translate("HumanIds")}}</div>
										</th>
										<th class="sticky-top ae-thead-th text-center" style="width:75px;overflow: hidden;text-overflow: ellipsis;">
											<div>{{shared.translate("PhoneType")}}</div>
										</th>
										<th class="sticky-top ae-thead-th text-center" style="width:185px;">
											<div>{{shared.translate("PhoneNumber")}}</div>
										</th>
										<td style="width:40px;" class="sticky-top ae-thead-th text-center" data-ae-actions="DefaultRepo.ZzPhones.DeleteByKey"></td>
									</tr>
								</thead>
								<tbody>
									<tr v-for="(i,ind) in Relations['ZzPhones']">
										<td class="ae-table-td text-dark text-center" style="" @click="updateRelation({compPath:'/a.Components/ZzPhones_UpdateByKey',recordKey:i.Id,ind:ind,fkToParent:'PersonId',relName:'ZzPhones'});">
											<div class="text-primary text-hover-success pointer">
												<i class="fa-solid fa-fw fa-edit"></i>
												<br>
												<span class="pk">{{i.Id}}</span>
											</div>
										</td>
										<td class="ae-table-td" style="">
										</td>
										<td class="ae-table-td text-center" style="">
											<div>{{i["PhoneType"]}}</div>
										</td>
										<td class="ae-table-td text-center" style="">
											<div>{{i["PhoneNumber"]}}</div>
										</td>
										<td style="width:40px;vertical-align:middle" class="text-center" data-ae-actions="DefaultRepo.ZzPhones.DeleteByKey">
											<span @click="deleteRelation({relationTable:'ZzPhones',ind:ind})">
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
_this.objectName = "ZzPersons";
_this.submitMethod = "UpdateByKey";
_this.createComponent = ""; 
_this.updateComponent = "";

_this.masterRequest = {"Id":"","Method":"DefaultRepo.ZzPersons.ReadByKey","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.ZzPersons.ReadByKey","Params":[{"Name":"Id","Value":""}]}}};





_this.Relations['ZzPhones']=[];

_this.RelationsMetaData['Persons_Phones']={"RelationName":"Persons_Phones","RelationTable":"ZzPhones","RelationPkColumn":"Id","RelationFkColumn":"PersonId","RelationType":"OneToMany","CreateQuery":"Create","ReadListQuery":"ReadList","UpdateByKeyQuery":"UpdateByKey","DeleteByKeyQuery":"DeleteByKey","DeleteQuery":"Delete","IsFileCentric":false,"RelationUiWidget":"Grid"};

_this.RelationsMetaData['Persons_Phones']['createComponent']='/a.Components/DefaultRepo_ZzPhones_Create';

_this.RelationsMetaData['Persons_Phones']['updateComponent']='/a.Components/DefaultRepo_ZzPhones_UpdateByKey';


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