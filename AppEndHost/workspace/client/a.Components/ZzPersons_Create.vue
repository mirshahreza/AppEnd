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
_this.objectName = "ZzPersons";
_this.submitMethod = "Create";

_this.row = {"FirstName":null,"LastName":null,"GenderId":""};






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