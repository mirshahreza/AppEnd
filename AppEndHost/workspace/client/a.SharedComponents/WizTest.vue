<template>
    <component-loader :src="'/a.SharedComponents/BaseWizard.vue'" :cid="wizardCid" ref="wizardRef">
    </component-loader>
</template>

<script>
    shared.setAppTitle("WizTest");

    let _this = { 
        cid: "", 
        c: null, 
        wizardRef: null,
        wizardCid: "",
        availableRoles: []
    };

    export default {
        methods: {
            /**
             * Validate current step manually
             */
            validateStep() {
                const wizard = this.$refs.wizardRef;
                if (wizard) {
                    const stepContentId = `stepContent_${wizard.wizardId}_${wizard.currentStepIndex}`;
                    if (isAreaValidById(stepContentId)) {
                        showSuccess(shared.translate('Validation passed'));
                    }
                }
            },

            /**
             * Load available roles
             */
            loadRoles() {
                // Example: Load roles from enum or API
                if (shared.enum) {
                    this.availableRoles = shared.enum(10000) || []; // Assuming 10000 is the enum ID for roles
                    
                    // Populate select dropdown
                    const selectId = `select_Role_${this.wizardCid}`;
                    const $select = $(`#${selectId}`);
                    if ($select.length > 0) {
                        $select.empty();
                        $select.append(`<option value="">${shared.translate('Please select')}</option>`);
                        this.availableRoles.forEach(role => {
                            $select.append(`<option value="${role.Id}">${role.Title}</option>`);
                        });
                    }
                }
            }
        },

        setup(props) {
            _this.cid = props['cid'];
            _this.wizardCid = genUN('wiz_');
            
            // Generate step content HTML
            const step1Content = `
                <div class="p-5">
                    <h5 class="mb-4">
                        <i class="fa-solid fa-user me-2"></i>
                        ${shared.translate('User Information')}
                    </h5>
                    <div class="row">
                        <div class="col-48 my-3">
                            <label class="fs-d9 text-muted ms-2" for="input_FirstName_${_this.wizardCid}">
                                ${shared.translate('First Name')}
                            </label>
                            <input type="text" 
                                   class="form-control form-control-sm" 
                                   id="input_FirstName_${_this.wizardCid}"
                                   data-ae-validation-required="true" 
                                   data-ae-validation-rule=":=s(2,50)">
                        </div>
                        <div class="col-48 my-3">
                            <label class="fs-d9 text-muted ms-2" for="input_LastName_${_this.wizardCid}">
                                ${shared.translate('Last Name')}
                            </label>
                            <input type="text" 
                                   class="form-control form-control-sm" 
                                   id="input_LastName_${_this.wizardCid}"
                                   data-ae-validation-required="true" 
                                   data-ae-validation-rule=":=s(2,50)">
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-48 my-3">
                            <label class="fs-d9 text-muted ms-2" for="input_Email_${_this.wizardCid}">
                                ${shared.translate('Email')}
                            </label>
                            <input type="email" 
                                   class="form-control form-control-sm" 
                                   id="input_Email_${_this.wizardCid}"
                                   data-ae-validation-required="true" 
                                   data-ae-validation-rule=":=s(5,128)">
                        </div>
                        <div class="col-48 my-3">
                            <label class="fs-d9 text-muted ms-2" for="input_Phone_${_this.wizardCid}">
                                ${shared.translate('Phone')}
                            </label>
                            <input type="text" 
                                   class="form-control form-control-sm" 
                                   id="input_Phone_${_this.wizardCid}"
                                   data-ae-validation-required="false" 
                                   data-ae-validation-rule=":=s(0,14)">
                        </div>
                    </div>
                </div>
            `;

            const step2Content = `
                <div class="p-5">
                    <h5 class="mb-4">
                        <i class="fa-solid fa-users me-2"></i>
                        ${shared.translate('Role Selection')}
                    </h5>
                    <div class="card">
                        <div class="card-body">
                            <label class="fs-d9 text-muted ms-2 mb-2">
                                ${shared.translate('Select Role')}
                            </label>
                            <select class="form-select form-select-sm" 
                                    id="select_Role_${_this.wizardCid}"
                                    data-ae-validation-required="true">
                                <option value="">${shared.translate('Please select')}</option>
                            </select>
                        </div>
                    </div>
                </div>
            `;

            const step3Content = `
                <div class="p-5">
                    <h5 class="mb-4">
                        <i class="fa-solid fa-address-book me-2"></i>
                        ${shared.translate('Additional Information')}
                    </h5>
                    <div class="row">
                        <div class="col-48 my-3">
                            <label class="fs-d9 text-muted ms-2" for="input_Address_${_this.wizardCid}">
                                ${shared.translate('Address')}
                            </label>
                            <textarea class="form-control form-control-sm" 
                                      id="input_Address_${_this.wizardCid}"
                                      rows="3"
                                      data-ae-validation-required="false" 
                                      data-ae-validation-rule=":=s(0,200)"></textarea>
                        </div>
                        <div class="col-48 my-3">
                            <label class="fs-d9 text-muted ms-2" for="input_Notes_${_this.wizardCid}">
                                ${shared.translate('Notes')}
                            </label>
                            <textarea class="form-control form-control-sm" 
                                      id="input_Notes_${_this.wizardCid}"
                                      rows="3"
                                      data-ae-validation-required="false" 
                                      data-ae-validation-rule=":=s(0,500)"></textarea>
                        </div>
                    </div>
                    <div class="card mt-3">
                        <div class="card-header">
                            ${shared.translate('Validation Test')}
                        </div>
                        <div class="card-body">
                            <label class="fs-d9 text-muted ms-2">
                                ${shared.translate('Integer between 10 and 15')}
                            </label>
                            <input type="text" 
                                   class="form-control form-control-sm ae-focus" 
                                   id="input_TestNumber_${_this.wizardCid}"
                                   data-ae-validation-required="true" 
                                   data-ae-validation-rule=":=i(10,15)" />
                        </div>
                    </div>
                </div>
            `;

            const step4Content = `
                <div class="p-5">
                    <h5 class="mb-4">
                        <i class="fa-solid fa-clipboard-check me-2"></i>
                        ${shared.translate('Review Information')}
                    </h5>
                    <div class="card">
                        <div class="card-body">
                            <div class="alert alert-info">
                                ${shared.translate('Please review all information before completing the wizard.')}
                            </div>
                        </div>
                    </div>
                </div>
            `;

            // Configure wizard steps
            const wizardParams = {
                title: shared.translate('Test Wizard'),
                subtitle: shared.translate('Complete the wizard to test all features'),
                description: shared.translate('This wizard demonstrates all features of the BaseWizard component'),
                wizardId: 'wizTest_' + genUN(''),
                steps: [
                    {
                        title: shared.translate('User Information'),
                        icon: 'fa-user',
                        content: step1Content,
                        useValidation: true,
                        onEnter: async (wizardData, stepIndex) => {
                            console.log('Entered step 1: User Information');
                        },
                        onLeave: async (wizardData, stepIndex) => {
                            console.log('Left step 1: User Information');
                        }
                    },
                    {
                        title: shared.translate('Role Selection'),
                        icon: 'fa-users',
                        content: step2Content,
                        useValidation: true,
                        onEnter: async (wizardData, stepIndex) => {
                            _this.c.loadRoles();
                        }
                    },
                    {
                        title: shared.translate('Additional Information'),
                        icon: 'fa-address-book',
                        content: step3Content,
                        useValidation: true,
                        optional: false
                    },
                    {
                        title: shared.translate('Review'),
                        icon: 'fa-clipboard-check',
                        content: step4Content,
                        useValidation: false,
                        onEnter: async (wizardData, stepIndex) => {
                            console.log('Reviewing data:', wizardData);
                        }
                    }
                ],
                initialData: {
                    firstName: '',
                    lastName: '',
                    email: '',
                    phone: '',
                    roleId: '',
                    address: '',
                    notes: '',
                    testNumber: ''
                },
                onFinish: async (wizardData, completedSteps) => {
                    showSuccess(shared.translate('Wizard completed successfully!'));
                    console.log('Wizard Data:', wizardData);
                    console.log('Completed Steps:', completedSteps);
                    
                    // Here you can save the data or perform any action
                    // Example: rpcAEP("SaveWizardData", wizardData, function(res) { ... });
                },
                onCancel: async (wizardData, currentStep) => {
                    return confirm(shared.translate('Are you sure you want to cancel? All progress will be lost.'));
                },
                allowSaveProgress: true,
                showStepCounter: true,
                showProgressIndicator: true,
                justAllowByBackNext: true,
                validateAllOnFinish: false,
                closeOnFinish: true
            };

            shared["params_" + _this.wizardCid] = wizardParams;

            return {};
        },

        data() {
            return _this;
        },

        created() {
            _this.c = this;
        },

        mounted() {
            initVueComponent(_this);
        },

        props: { cid: String }
    }
</script>

