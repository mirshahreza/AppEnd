<template>
<div class="card h-100 rounded-bottom-0 rounded-end-0 bg-transparent border-0">
		<div class="card-body p-0">
			<div class="card h-100 border-light bg-primary-subtle bg-opacity-75 border-0 pt-2 border-0 rounded-0 scrollable">
				<div class="card-body fs-d8 pt-0 ps-3 pe-3 bg-transparent">
					<div class="row">
						<div class="card rounded-1 border-light mb-1">
							<div class="card-body">
								<div class="row">
									<div class="col-48" v-if="inputs.fkColumn!=='Method'">
										<label class="fs-d9 text-muted ms-2" for="input_Method">{{shared.translate('Method')}}</label>
										<input type="text" class="form-control form-control-sm" id="input_Method" v-model="row.Method" data-ae-validation-required="true" data-ae-validation-rule="">
									</div>
									<div class="col-48" v-if="inputs.fkColumn!=='IsSucceeded'">
										<label class="fs-d9 text-muted ms-2"></label>
										<div class="form-control pointer data-ae-validation" data-ae-widget="nullableCheckbox" data-ae-widget-options="{&quot;shownull&quot;:true}">
											<i class="fa-solid fa-fw me-1"></i>
											<span>{{shared.translate('IsSucceeded')}}</span>
											<input type="hidden" v-model="row.IsSucceeded" data-ae-validation-required="true">
										</div>
									</div>
									<div class="col-48" v-if="inputs.fkColumn!=='FromCache'">
										<label class="fs-d9 text-muted ms-2"></label>
										<div class="form-control pointer data-ae-validation" data-ae-widget="nullableCheckbox" data-ae-widget-options="{&quot;shownull&quot;:true}">
											<i class="fa-solid fa-fw me-1"></i>
											<span>{{shared.translate('FromCache')}}</span>
											<input type="hidden" v-model="row.FromCache" data-ae-validation-required="true">
										</div>
									</div>
									<div class="col-48" v-if="inputs.fkColumn!=='RecordId'">
										<label class="fs-d9 text-muted ms-2" for="input_RecordId">{{shared.translate('RecordId')}}</label>
										<input type="text" class="form-control form-control-sm" id="input_RecordId" v-model="row.RecordId" data-ae-validation-required="false" data-ae-validation-rule="">
									</div>
									<div class="col-48" v-if="inputs.fkColumn!=='EventBy'">
										<label class="fs-d9 text-muted ms-2" for="input_EventBy">{{shared.translate('EventBy')}}</label>
										<input type="text" class="form-control form-control-sm" id="input_EventBy" v-model="row.EventBy" data-ae-validation-required="true" data-ae-validation-rule="">
									</div>
									<div class="col-48" v-if="inputs.fkColumn!=='EventOn'">
										<label class="fs-d9 text-muted ms-2" for="input_EventOn">{{shared.translate('EventOn')}}</label>
										<input type="text" class="form-control form-control-sm" id="input_EventOn" v-model="row.EventOn" data-ae-validation-required="true" data-ae-validation-rule="dt(1900-01-01 00:01:00,2100-12-30 11:59:59)">
									</div>
									<div class="col-48" v-if="inputs.fkColumn!=='Duration'">
										<label class="fs-d9 text-muted ms-2" for="input_Duration">{{shared.translate('Duration')}}</label>
										<input type="text" class="form-control form-control-sm" id="input_Duration" v-model="row.Duration" data-ae-validation-required="true" data-ae-validation-rule=":=i(0,10000)">
									</div>
									<div class="col-48" v-if="inputs.fkColumn!=='ClientInfo'">
										<label class="fs-d9 text-muted ms-2" for="input_ClientInfo">{{shared.translate('ClientInfo')}}</label>
										<textarea type="text" class="form-control form-control-sm" id="input_ClientInfo" v-model="row.ClientInfo" data-ae-validation-required="false" data-ae-validation-rule=""></textarea>
									</div>
								</div>
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
let _this = { cid: "", c: null, inputs: {}, dbConfName: "", objectName: "", submitMethod: "", initialRequests: [], initialResponses: [], pickerRequests: [], pickerHumanIds: [], row: {}, Relations: {}, RelationsMetaData: {}, regulator: null };
_this.dbConfName = "DefaultRepo";
_this.objectName = "Common_ActivityLog";
_this.submitMethod = "Create";

_this.row = {"Method":null,"IsSucceeded":null,"FromCache":null,"RecordId":null,"EventBy":null,"EventOn":null,"Duration":null,"ClientInfo":null};






export default {
	methods: {
		localSelectFiles(relName, parentId, fieldName_FileContent, fieldName_FileName, fieldName_FileSize, fieldName_FileType) {
			crudSelectFiles(_this, relName, parentId, fieldName_FileContent, fieldName_FileName, fieldName_FileSize, fieldName_FileType);
		},
		localAddRelation(relName) { crudAddRelation(_this, relName); },
		localRemoveRelation(relName, ind) { crudRemoveRelation(_this, relName, ind); },
		localOpenPicker(colName) { crudOpenPicker(_this, _this.c.row, colName); },
		localCrudUpdateRelation(compPath, modalSize, recordKey, ind,fkColumn,relName) { crudUpdateRelation(_this, compPath, modalSize, recordKey, ind,fkColumn,relName); },
		localCrudBaseInfo() { crudLoadBaseInfo(_this); },
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
	mounted() { initVueComponent(_this); _this.c.localCrudBaseInfo(); },
	props: { cid: String }
}

</script>