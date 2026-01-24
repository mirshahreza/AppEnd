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
		<div class="card-body bg-primary-subtle-light scrollable" id="formArea" data-ae-widget="inputsRegulator" data-ae-widget-options="{}">
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
													<div data-ae-widget="aeFileField" class="ae-file-field w-100 h-100 border border-2 rounded-circle pointer data-ae-validation ">
														<input type="hidden" class="FileBody" v-model="row.File_FileBody" data-ae-validation-required="false">
														<input type="hidden" class="FileName" v-model="row['File_FileName']">
														<input type="hidden" class="FileSize" v-model="row['File_FileSize']">
														<input type="hidden" class="FileMime" v-model="row['File_FileMime']">
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
						<div class="col-48" id="container_ContentId">
							<label class="fs-d8 text-muted ms-2" for="input_ContentId">{{shared.translate('ContentId')}}</label>
							<input type="text" class="form-control form-control-sm text-center" style="direction:ltr !important" id="input_ContentId" v-model="row.ContentId" data-ae-validation-required="true" data-ae-validation-rule=":=i(0,2147483647)">
						</div>
						<div class="col-48" id="container_Note">
							<label class="fs-d8 text-muted ms-2" for="input_Note">{{shared.translate('Note')}}</label>
							<input type="text" class="form-control form-control-sm" id="input_Note" v-model="row.Note" data-ae-validation-required="false" data-ae-validation-rule=":=s(0,64)">
						</div>
						<div class="col-48" id="container_ViewOrder">
							<label class="fs-d8 text-muted ms-2" for="input_ViewOrder">{{shared.translate('ViewOrder')}}</label>
							<input type="text" class="form-control form-control-sm text-center" style="direction:ltr !important" id="input_ViewOrder" v-model="row.ViewOrder" data-ae-validation-required="false" data-ae-validation-rule=":=i(0,2147483647)">
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
let _this = { cid: "", ismodal:"", c: null, templateType:"Create", inputs: {}, dbConfName: "", objectName: "", submitMethod: "", initialRequests: [], initialResponses: [], pickerRequests: [], pickerHumanIds: [], row: {}, Relations: {}, RelationsMetaData: {} };
_this.dbConfName = "DefaultRepo";
_this.objectName = "CmsFiles";
_this.submitMethod = "Create";

_this.row = {"ContentId":null,"Note":null,"ViewOrder":null,"File_FileBody":null,"File_FileName":null,"File_FileSize":null,"File_FileMime":null};






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