<template>
<div class="card h-100 rounded-bottom-0 rounded-end-0 bg-transparent border-0">
		<div class="card-body p-0">
			<div class="card h-100 border-light bg-primary-subtle bg-opacity-75 border-0 pt-2 border-0 rounded-0 scrollable">
				<div class="card-body fs-d8 pt-0 ps-3 pe-3 bg-transparent">
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
									<div class="col-48" v-if="inputs.fkColumn!=='Gender'">
										<label class="fs-d9 text-muted ms-2" for="input_Gender">{{shared.translate('Gender')}}</label>
										<select class="form-select form-select-sm" v-model="row.Gender" data-ae-validation-required="true">
											<option value="">-</option>
											<option v-for="i in shared.getResponseObjectById(initialResponses,'Gender_Lookup')" :value="i['Id']">{{i.Title}} {{i.ShortName}}</option>
										</select>
									</div>
									<div class="col-48" v-if="inputs.fkColumn!=='IsBuiltIn'">
										<label class="fs-d9 text-muted ms-2"></label>
										<div class="form-control pointer data-ae-validation " data-ae-widget="nullableCheckbox" data-ae-widget-options="">
											<i class="fa-solid fa-fw me-1"></i>
											<span>{{shared.translate('IsBuiltIn')}}</span>
											<input type="hidden" v-model="row.IsBuiltIn" data-ae-validation-required="true">
										</div>
									</div>
								</div>
							</div>
						</div>
						<div class="card rounded-1 border-light mb-1">
							<div class="card-body">
								<div class="row">
									<div class="col-48">
										<label class="fs-d9 text-muted ms-2"></label>
										<div class="form-control pointer data-ae-validation disabled" data-ae-widget="nullableCheckbox" data-ae-widget-options="">
											<i class="fa-solid fa-fw me-1"></i>
											<span>{{shared.translate('IsActive')}}</span>
											<input type="hidden" v-model="row.IsActive" data-ae-validation-required="true" disabled="">
										</div>
									</div>
									<div class="col-48">
										<label class="fs-d9 text-muted ms-2" for="input_IsActiveUpdatedBy">{{shared.translate('IsActiveUpdatedBy')}}</label>
										<input disabled="" type="text" class="form-control form-control-sm" id="input_IsActiveUpdatedBy" v-model="row.IsActiveUpdatedBy" data-ae-validation-required="false" data-ae-validation-rule="">
									</div>
									<div class="col-48">
										<label class="fs-d9 text-muted ms-2" for="input_IsActiveUpdatedOn">{{shared.translate('IsActiveUpdatedOn')}}</label>
										<input disabled="" type="text" class="form-control form-control-sm" id="input_IsActiveUpdatedOn" v-model="row.IsActiveUpdatedOn" data-ae-validation-required="false" data-ae-validation-rule="dt(1900-01-01 00:01:00,2100-12-30 11:59:59)">
									</div>
									<div class="col-48">
										<label class="fs-d9 text-muted ms-2"></label>
										<div class="form-control pointer data-ae-validation disabled" data-ae-widget="nullableCheckbox" data-ae-widget-options="{    &quot;shownull&quot;: true,    &quot;nullClasses&quot;: &quot;fa-minus text-secondary&quot;,    &quot;trueClasses&quot;: &quot;fa-lock-open text-success&quot;,    &quot;falseClasses&quot;: &quot;fa-lock text-danger&quot;}">
											<i class="fa-solid fa-fw me-1"></i>
											<span>{{shared.translate('LoginLocked')}}</span>
											<input type="hidden" v-model="row.LoginLocked" data-ae-validation-required="true" disabled="">
										</div>
									</div>
									<div class="col-48">
										<label class="fs-d9 text-muted ms-2" for="input_LoginLockedUpdatedOn">{{shared.translate('LoginLockedUpdatedOn')}}</label>
										<input disabled="" type="text" class="form-control form-control-sm" id="input_LoginLockedUpdatedOn" v-model="row.LoginLockedUpdatedOn" data-ae-validation-required="false" data-ae-validation-rule="dt(1900-01-01 00:01:00,2100-12-30 11:59:59)">
									</div>
								</div>
							</div>
						</div>
						<div class="card rounded-1 border-light mb-1">
							<div class="card-header text-bg-light">
								{{shared.translate('Contact')}}
							</div>
							<div class="card-body">
								<div class="row">
									<div class="col-48">
										<label class="fs-d9 text-muted ms-2" for="input_Email">{{shared.translate('Email')}}</label>
										<input type="text" class="form-control form-control-sm" id="input_Email" v-model="row.Email" data-ae-validation-required="false" data-ae-validation-rule="">
									</div>
									<div class="col-48">
										<label class="fs-d9 text-muted ms-2" for="input_Mobile">{{shared.translate('Mobile')}}</label>
										<input type="text" class="form-control form-control-sm" id="input_Mobile" v-model="row.Mobile" data-ae-validation-required="false" data-ae-validation-rule="">
									</div>
								</div>
							</div>
						</div>
						<div class="card rounded-1 border-light mb-1">
							<div class="card-header text-bg-light">
								{{shared.translate('LoginTry')}}
							</div>
							<div class="card-body">
								<div class="row">
									<div class="col-48">
										<label class="fs-d9 text-muted ms-2" for="input_LoginTryFails">{{shared.translate('LoginTryFails')}}</label>
										<input type="text" class="form-control form-control-sm" id="input_LoginTryFails" v-model="row.LoginTryFails" data-ae-validation-required="false" data-ae-validation-rule=":=i(0,10000)">
									</div>
								</div>
							</div>
						</div>
						<div class="card rounded-1 border-light mb-1">
							<div class="card-header text-bg-light">
								{{shared.translate('PasswordState')}}
							</div>
							<div class="card-body">
								<div class="row">
								</div>
							</div>
						</div>
						<div class="card rounded-1 border-light mb-1">
							<div class="card-header text-bg-light">
								{{shared.translate('AppEndDbIO.DbRelation')}}
							</div>
							<div class="card-body">
								<div class="row">
									<div class="col-48">
										<div class="form-control data-ae-validation" data-ae-validation-required="false" data-ae-validation-rule=":=n(0)">
											<div class="form-check form-check-inline" v-for="i in shared.getResponseObjectById(initialResponses,'RoleId_Lookup')">
												<input class="form-check-input" type="checkbox" v-model="Relations.AAA_Users_R_Roles" :value="i.Id" :id="i.Id+'RoleId_Lookup'">
												<label class="form-check-label" :for="i.Id+'RoleId_Lookup'">
													{{i.RoleName}}
												</label>
											</div>
										</div>
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
let _this = { cid: "", c: null, inputs: {}, dbConfName: "", objectName: "", loadMethod: "", submitMethod: "", masterRequest: {}, initialRequests: [], pickerRequests: [], pickerHumanIds: [], initialResponses: [], row: {}, Relations: {}, RelationsMetaData: {}, createComponent: "", updateComponent: "", regulator: null };
_this.dbConfName = "DefaultRepo";
_this.objectName = "AAA_Users";
_this.submitMethod = "UpdateByKey";
_this.createComponent = ""; 
_this.updateComponent = "";

_this.masterRequest = {"Id":"","Method":"DefaultRepo.AAA_Users.ReadByKey","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.AAA_Users.ReadByKey","Params":[{"Name":"Id","Value":""}]}}};



_this.initialRequests.push({"Id":"RoleId_Lookup","Method":"DefaultRepo.AAA_Roles.ReadList","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.AAA_Roles.ReadList","OrderClauses":[{"Name":"RoleName","OrderDirection":"ASC"}],"Pagination":{"PageNumber":1,"PageSize":500},"ExceptAggregations":["Count"],"IncludeSubQueries":false}}});

_this.RelationsMetaData['Roles']={"RelationName":"Roles","RelationTable":"AAA_Users_R_Roles","RelationPkColumn":"Id","RelationFkColumn":"UserId","RelationType":"ManyToMany","LinkingTargetTable":"AAA_Roles","LinkingColumnInManyToMany":"RoleId","CreateQuery":"Create","ReadListQuery":"ReadList","UpdateByKeyQuery":"UpdateByKey","DeleteByKeyQuery":"DeleteByKey","DeleteQuery":"Delete","IsFileCentric":false,"RelationUiWidget":"CheckboxList"};


_this.initialRequests.push({"Id":"Gender_Lookup","Method":"DefaultRepo.Common_BaseInfo.ReadList","Inputs":{"ClientQueryJE":{"QueryFullName":"DefaultRepo.Common_BaseInfo.ReadList","Where":{"CompareClauses":[{"Name":"ParentId","Value":10000,"ClauseOperator":"Equal"}]},"OrderClauses":[{"Name":"ViewOrder","OrderDirection":"ASC"}],"Pagination":{"PageNumber":1,"PageSize":500},"ExceptAggregations":["Count"],"IncludeSubQueries":false}}});

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