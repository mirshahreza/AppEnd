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
						<div class="col-48" v-if="inputs.fkColumn!=='PhoneType'">
							<label class="fs-d8 text-muted ms-2" for="input_PhoneType">{{shared.translate('PhoneType')}}</label>
							<select class="form-select form-select-sm" v-model="row.PhoneType" data-ae-validation-required="false">
								<option value="">-</option>
								<option v-for="i in shared.enum(10015)" :value="i['Id']">{{i.Title}}</option>
							</select>
						</div>
						<div class="col-48" v-if="inputs.fkColumn!=='PhoneNumber'">
							<label class="fs-d8 text-muted ms-2" for="input_PhoneNumber">{{shared.translate('PhoneNumber')}}</label>
							<input type="text" class="form-control form-control-sm" id="input_PhoneNumber" v-model="row.PhoneNumber" data-ae-validation-required="false" data-ae-validation-rule=":=s(0,16)">
						</div>
						<div class="col-48" v-if="inputs.fkColumn!=='PersonId'">
							<label class="fs-d8 text-muted ms-2" for="input_PersonId">{{shared.translate('PersonId')}}</label>
							<div class="form-control form-control-sm py-0 data-ae-validation ">
								<div class="input-group input-group-sm p-0 pt-1" data-ae-widget="objectPicker">
									<input type="hidden" v-model="row.PersonId" data-ae-validation-required="false">
									<input type="hidden" v-model="row.PersonId_FirstName">
									<input type="hidden" v-model="row.PersonId_LastName">
									<input type="text" class="form-control bg-transparent p-0 m-0 border-0" :value="shared.fixNull(row.PersonId+' '+row.PersonId_FirstName+' '+row.PersonId_LastName,'',true)" :placeholder="shared.translate('PersonId')" disabled="">
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
					</div>
				</div>
			</div>
		</div>
		<div class="card-footer p-0 bg-secondary-subtle bg-gradient border-0 rounded-0">
			<button class="btn btn-link text-decoration-none bg-hover-light w-100 py-3 rounded-0" @click="ok" data-ae-key="ok">
				<i class="fa-solid fa-save pe-1"></i>
				<span>{{shared.translate("Save")}}</span>
			</button>
		</div>
	</div>
</template>
<script>
let _this = { cid: "", ismodal:"", c: null, templateType:"Create", inputs: {}, dbConfName: "", objectName: "", submitMethod: "", initialRequests: [], initialResponses: [], pickerRequests: [], pickerHumanIds: [], row: {}, Relations: {}, RelationsMetaData: {}, regulator: null };
_this.dbConfName = "DefaultRepo";
_this.objectName = "ZzPhones";
_this.submitMethod = "Create";

_this.row = {"PhoneType":"","PhoneNumber":null,"PersonId":""};







_this.pickerRequests.push({"Id":"PersonId_Lookup","Method":"DefaultRepo.ZzPersons.ReadList","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.ZzPersons.ReadList","Where":{},"OrderClauses":[{"Name":"FirstName","OrderDirection":"ASC"}],"Pagination":{"PageNumber":1,"PageSize":500},"IncludeSubQueries":false}}});

_this.pickerHumanIds.push({Id:'PersonId_HumanIds',Items:["FirstName","LastName"]});
export default {
	methods: {
	},
	setup(props) { 
		_this.cid = props['cid'];
		_this.ismodal = props['ismodal'];
		_this.inputs = shared["params_" + _this.cid];
	},
	data() { return _this; },
	created() { _this.c = this; assignDefaultMethods(_this); },
	mounted() { initVueComponent(_this); _this.c.loadBaseInfo(); _this.c.componentFinalization(); },
	props: { cid: String, ismodal: String }
}

</script>