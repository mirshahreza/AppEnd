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
						<div class="col-48" v-if="inputs.fkColumn!=='UserId'">
							<label class="fs-d8 text-muted ms-2" for="input_UserId">{{shared.translate('UserId')}}</label>
							<input type="text" class="form-control form-control-sm" id="input_UserId" v-model="row.UserId" data-ae-validation-required="true" data-ae-validation-rule=":=i(0,2147483647)">
						</div>
						<div class="col-48" v-if="inputs.fkColumn!=='AttributeId'">
							<label class="fs-d8 text-muted ms-2" for="input_AttributeId">{{shared.translate('AttributeId')}}</label>
							<input type="text" class="form-control form-control-sm" id="input_AttributeId" v-model="row.AttributeId" data-ae-validation-required="true" data-ae-validation-rule=":=i(0,2147483647)">
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
_this.objectName = "AAA_Users_Attributes";
_this.submitMethod = "Create";

_this.row = {"UserId":null,"AttributeId":null};






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