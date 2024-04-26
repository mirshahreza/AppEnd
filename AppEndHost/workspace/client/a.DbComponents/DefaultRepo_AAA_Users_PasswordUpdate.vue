<template>
<div class="card h-100 bg-transparent rounded-0 border-0">
		<div class="card-body bg-dark-subtle bg-opacity-75 scrollable">
			<div class="row">
				<div class="card rounded-1 border-light mb-1">
					<div class="card-body">
						<div class="row">
							<div class="col-48">
								<label class="fs-d9 text-muted ms-2" for="input_Password">{{shared.translate('Password')}}</label>
								<textarea type="text" class="form-control form-control-sm " id="input_Password" v-model="row.Password" data-ae-validation-required="false" data-ae-validation-rule=""></textarea>
							</div>
						</div>
					</div>
				</div>
			</div>
		</div>
		<div class="card-footer p-3 bg-secondary-subtle bg-gradient border-0 rounded-0">
			<div class="row">
				<div class="col-12">
					<button class="btn btn-sm btn-secondary w-100 py-2" @click="cancel" data-ae-key="ok">
						<i class="fa-solid fa-cancel pe-1"></i>
						<span>{{shared.translate("Cancel")}}</span>
					</button>
				</div>
				<div class="col-36">
					<button class="btn btn-sm btn-primary w-100 py-2" @click="ok" data-ae-key="ok">
						<i class="fa-solid fa-check pe-1"></i>
						<span>{{shared.translate("Ok")}}</span>
					</button>
				</div>
			</div>
		</div>
	</div>
</template>
<script>
let _this = { cid: "", c: null, inputs: {}, dbConfName: "", objectName: "", loadMethod: "", submitMethod: "", masterRequest: {}, initialRequests: [], pickerRequests: [], pickerHumanIds: [], initialResponses: [], row: {}, Relations: {}, RelationsMetaData: {}, createComponent: "", updateComponent: "", regulator: null };
_this.dbConfName = "DefaultRepo";
_this.objectName = "AAA_Users";
_this.submitMethod = "PasswordUpdate";
_this.createComponent = ""; 
_this.updateComponent = "";

_this.masterRequest = {"Id":"","Method":"DefaultRepo.AAA_Users.ReadByKey","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.AAA_Users.ReadByKey","Params":[{"Name":"Id","Value":""}]}}};






export default {
	methods: {
		localSelectFiles(relName, parentId, fieldName_FileContent, fieldName_FileName, fieldName_FileSize, fieldName_FileType) {
			crudSelectFiles(_this, relName, parentId, fieldName_FileContent, fieldName_FileName, fieldName_FileSize, fieldName_FileType);
		},
		localAddRelation(relName) { crudAddRelation(_this, relName); },
		localRemoveRelation(relName, ind) { crudRemoveRelation(_this, relName, ind); },
		localOpenPicker(colName) { crudOpenPicker(_this, _this.c.row, colName); },
		localCrudUpdateRelation(compPath, modalSize, recordKey,ind,fkColumn,relName) { crudUpdateRelation(_this, compPath, modalSize, recordKey,ind,fkColumn,relName); },
		localCrudBaseInfo() { crudLoadBaseInfo(_this); },
		localLoadMasterRecord() { 
			if (_this.c.inputs.okAction !== 'return') 
				crudLoadMasterRecord(_this);
			else 
				_this.c.row=_this.c.inputs.row;
		},
		ok() {
			if (!_this.regulator.isValid()) return;
			if (_this.c.inputs.okAction === 'return') {
				if (_this.inputs.callback) _this.inputs.callback(_this.row);
				_this.c.close();
			} else {
				crudSaveRecord(_this, function () {
					if (_this.inputs.callback) _this.inputs.callback();
					_this.c.close();
				});
			}
		},
		cancel() { _this.c.close(); },
		close() { shared.closeComponent(_this.cid); }
	},
	setup(props) {
		_this.cid = props['cid'];
		_this.inputs = shared["params_" + _this.cid];
	},
	data() { return _this; },
	created() { _this.c = this; },
	mounted() { initVueComponent(_this); _this.c.localCrudBaseInfo(); _this.c.localLoadMasterRecord() },
	props: { cid: String }
}


</script>