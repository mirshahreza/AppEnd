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
					<div class="col-48">
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
			</div>
			<div class="card rounded-1 border-light mb-1">
				<div class="card-body">
					<div class="row">
						<div class="col-48" v-if="inputs.fkColumn!=='UserName'">
							<label class="fs-d8 text-muted ms-2" for="input_UserName">{{shared.translate('UserName')}}</label>
							<input type="text" class="form-control form-control-sm" id="input_UserName" v-model="row.UserName" data-ae-validation-required="true" data-ae-validation-rule=":=s(0,64)">
						</div>
					</div>
				</div>
			</div>
			<div class="card rounded-1 border-light mb-1">
				<div class="card-header text-bg-light p-1">
					{{shared.translate('Contact')}}
				</div>
				<div class="card-body">
					<div class="row">
						<div class="col-48" v-if="inputs.fkColumn!=='Email'">
							<label class="fs-d8 text-muted ms-2" for="input_Email">{{shared.translate('Email')}}</label>
							<input type="text" class="form-control form-control-sm" id="input_Email" v-model="row.Email" data-ae-validation-required="false" data-ae-validation-rule=":=s(0,128)">
						</div>
						<div class="col-48" v-if="inputs.fkColumn!=='Mobile'">
							<label class="fs-d8 text-muted ms-2" for="input_Mobile">{{shared.translate('Mobile')}}</label>
							<input type="text" class="form-control form-control-sm" id="input_Mobile" v-model="row.Mobile" data-ae-validation-required="false" data-ae-validation-rule=":=s(0,14)">
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
_this.objectName = "AAA_Users";
_this.submitMethod = "Create";

_this.row = {"UserName":null,"Email":null,"Mobile":null,"Picture_FileBody":null,"Picture_FileName":null,"Picture_FileSize":null,"Picture_FileMime":null};






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
	mounted() { initVueComponent(_this); _this.c.componentFinalization(); },
	props: { cid: String, ismodal: String }
}

</script>