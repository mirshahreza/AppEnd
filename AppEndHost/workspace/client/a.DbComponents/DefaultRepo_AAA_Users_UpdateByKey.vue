<template>
<div class="card h-100 bg-transparent rounded-0 border-0">
		<div class="card-body bg-dark-subtle bg-opacity-75 scrollable">
			<div class="row">
				<div class="card rounded-1 border-light mb-1">
					<div class="card-body p-2">
						<div class="row">
							<div class="col"></div>
							<div class="col-12">
								<table class="w-100">
									<tbody>
										<tr>
											<td></td>
											<td style="width:100px;" class="py-3">
												<div style="height:100px;width:100px;">
													<div data-ae-widget="aeFileField" data-ae-widget-options="{&quot;accept&quot;:&quot;image/x-png,image/gif,image/jpeg&quot;,&quot;resize&quot;:true,&quot;resizeMaxWidth&quot;:950,&quot;resizeMaxHeight&quot;:950,&quot;maxSize&quot;:800000}" class="ae-file-field w-100 h-100 border border-2 rounded-circle pointer data-ae-validation ">
														<input type="hidden" class="FileBody" v-model="row['Picture_FileBody']" data-ae-validation-required="false">
														<input type="hidden" class="FileName" v-model="row['Picture_FileName']">
														<input type="hidden" class="FileSize" v-model="row['Picture_FileSize']">
														<input type="hidden" class="FileMime" v-model="row['Picture_FileMime']">
													</div>
												</div>
											</td>
											<td></td>
										</tr>
									</tbody>
								</table>
							</div>
							<div class="col"></div>
						</div>
					</div>
				</div>
				<div class="card rounded-1 border-light mb-1">
					<div class="card-body">
						<div class="row">
							<div class="col-48" v-if="inputs.fkColumn!=='UserName'">
								<label class="fs-d9 text-muted ms-2" for="input_UserName">{{shared.translate('UserName')}}</label>
								<input type="text" class="form-control form-control-sm" id="input_UserName" v-model="row.UserName" data-ae-validation-required="true" data-ae-validation-rule="">
							</div>
						</div>
					</div>
				</div>
				<div class="card rounded-1 border-light mb-1">
					<div class="card-body">
						<div class="row">
							<div class="col-48" v-if="inputs.fkColumn!=='IsBuiltIn'">
								<div class="form-control mt-2 pointer text-nowrap data-ae-validation " data-ae-widget="nullableCheckbox">
									<i class="fa-solid fa-fw me-1"></i>
									<span>{{shared.translate('IsBuiltIn')}}</span>
									<input type="hidden" v-model="row.IsBuiltIn" data-ae-validation-required="true">
								</div>
							</div>
							<div class="col-48" v-if="inputs.fkColumn!=='Email'">
								<label class="fs-d9 text-muted ms-2" for="input_Email">{{shared.translate('Email')}}</label>
								<input type="text" class="form-control form-control-sm" id="input_Email" v-model="row.Email" data-ae-validation-required="false" data-ae-validation-rule="">
							</div>
							<div class="col-48" v-if="inputs.fkColumn!=='Mobile'">
								<label class="fs-d9 text-muted ms-2" for="input_Mobile">{{shared.translate('Mobile')}}</label>
								<input type="text" class="form-control form-control-sm" id="input_Mobile" v-model="row.Mobile" data-ae-validation-required="false" data-ae-validation-rule="">
							</div>
							<div class="col-48" v-if="inputs.fkColumn!=='LoginTryFails'">
								<label class="fs-d9 text-muted ms-2" for="input_LoginTryFails">{{shared.translate('LoginTryFails')}}</label>
								<input disabled="" type="text" class="form-control form-control-sm" id="input_LoginTryFails" v-model="row.LoginTryFails" data-ae-validation-required="false" data-ae-validation-rule=":=i(0,10000)">
							</div>
							<div class="col-48" v-if="inputs.fkColumn!=='Settings'">
								<label class="fs-d9 text-muted ms-2" for="input_Settings">{{shared.translate('Settings')}}</label>
								<textarea disabled="" type="text" class="form-control form-control-sm disabled" id="input_Settings" v-model="row.Settings" data-ae-validation-required="false" data-ae-validation-rule=""></textarea>
							</div>
						</div>
					</div>
				</div>
				<div class="card rounded-1 border-light mb-1">
					<div class="card-body">
						<div class="row">
							<div class="col-48">
								<div class="form-control mt-2 pointer text-nowrap data-ae-validation disabled" data-ae-widget="nullableCheckbox">
									<i class="fa-solid fa-fw me-1"></i>
									<span>{{shared.translate('IsActive')}}</span>
									<input type="hidden" v-model="row.IsActive" data-ae-validation-required="true" disabled="">
								</div>
							</div>
							<div class="col-48">
								<label class="fs-d9 text-muted ms-2" for="input_IsActiveUpdatedBy">{{shared.translate('IsActiveUpdatedBy')}}</label>
								<input disabled="" type="text" class="form-control form-control-sm" id="input_IsActiveUpdatedBy" v-model="row.IsActiveUpdatedBy" data-ae-validation-required="false" data-ae-validation-rule=":=i(0,10000)">
							</div>
							<div class="col-48">
								<label class="fs-d9 text-muted ms-2" for="input_IsActiveUpdatedOn">{{shared.translate('IsActiveUpdatedOn')}}</label>
								<div class="input-group input-group-sm">
									<button class="btn btn-sm btn-outline-secondary" id="dp_IsActiveUpdatedOn" data-ae-widget="dtPicker" data-ae-widget-options="{&quot;targetTextSelector&quot;:&quot;#dpText_IsActiveUpdatedOn&quot;,&quot;targetDateSelector&quot;:&quot;#dpDate_IsActiveUpdatedOn&quot;,&quot;enableTimePicker&quot;:true,&quot;dateFormat&quot;:&quot;yyyy-MM-dd hh:mm tt&quot;,&quot;textFormat&quot;:&quot;yyyy-MM-dd hh:mm tt&quot;}" disabled="">
										<i class="fa-solid fa-fw fa-calendar"></i>
									</button>
									<input class="form-control form-control-sm text-center" style="direction:ltr" id="dpText_IsActiveUpdatedOn" disabled="">
									<input class="form-control form-control-sm" id="dpDate_IsActiveUpdatedOn" type="hidden" v-model="row.IsActiveUpdatedOn">
								</div>
							</div>
							<div class="col-48">
								<div class="form-control mt-2 pointer text-nowrap data-ae-validation disabled" data-ae-widget="nullableCheckbox">
									<i class="fa-solid fa-fw me-1"></i>
									<span>{{shared.translate('LoginLocked')}}</span>
									<input type="hidden" v-model="row.LoginLocked" data-ae-validation-required="true" disabled="">
								</div>
							</div>
							<div class="col-48">
								<label class="fs-d9 text-muted ms-2" for="input_LoginLockedUpdatedOn">{{shared.translate('LoginLockedUpdatedOn')}}</label>
								<div class="input-group input-group-sm">
									<button class="btn btn-sm btn-outline-secondary" id="dp_LoginLockedUpdatedOn" data-ae-widget="dtPicker" data-ae-widget-options="{&quot;targetTextSelector&quot;:&quot;#dpText_LoginLockedUpdatedOn&quot;,&quot;targetDateSelector&quot;:&quot;#dpDate_LoginLockedUpdatedOn&quot;,&quot;enableTimePicker&quot;:true,&quot;dateFormat&quot;:&quot;yyyy-MM-dd hh:mm tt&quot;,&quot;textFormat&quot;:&quot;yyyy-MM-dd hh:mm tt&quot;}" disabled="">
										<i class="fa-solid fa-fw fa-calendar"></i>
									</button>
									<input class="form-control form-control-sm text-center" style="direction:ltr" id="dpText_LoginLockedUpdatedOn" disabled="">
									<input class="form-control form-control-sm" id="dpDate_LoginLockedUpdatedOn" type="hidden" v-model="row.LoginLockedUpdatedOn">
								</div>
							</div>
							<div class="col-48">
								<div class="form-control mt-2 pointer text-nowrap disabled" data-ae-widget="nullableCheckbox">
									<i class="fa-solid fa-fw me-1"></i>
									<span>{{shared.translate('LoginTry')}}</span>
									<input type="hidden" v-model="row.LoginTry" disabled="">
								</div>
							</div>
							<div class="col-48">
								<label class="fs-d9 text-muted ms-2" for="input_LoginTryOn">{{shared.translate('LoginTryOn')}}</label>
								<div class="input-group input-group-sm">
									<button class="btn btn-sm btn-outline-secondary" id="dp_LoginTryOn" data-ae-widget="dtPicker" data-ae-widget-options="{&quot;targetTextSelector&quot;:&quot;#dpText_LoginTryOn&quot;,&quot;targetDateSelector&quot;:&quot;#dpDate_LoginTryOn&quot;,&quot;enableTimePicker&quot;:true,&quot;dateFormat&quot;:&quot;yyyy-MM-dd hh:mm tt&quot;,&quot;textFormat&quot;:&quot;yyyy-MM-dd hh:mm tt&quot;}" disabled="">
										<i class="fa-solid fa-fw fa-calendar"></i>
									</button>
									<input class="form-control form-control-sm text-center" style="direction:ltr" id="dpText_LoginTryOn" disabled="">
									<input class="form-control form-control-sm" id="dpDate_LoginTryOn" type="hidden" v-model="row.LoginTryOn">
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
let _this = { cid: "", c: null, inputs: {}, dbConfName: "", objectName: "", loadMethod: "", submitMethod: "", masterRequest: {}, initialRequests: [], pickerRequests: [], pickerHumanIds: [], initialResponses: [], row: {}, Relations: {}, RelationsMetaData: {}, createComponent: "", updateComponent: "", regulator: null };
_this.dbConfName = "DefaultRepo";
_this.objectName = "AAA_Users";
_this.submitMethod = "UpdateByKey";
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