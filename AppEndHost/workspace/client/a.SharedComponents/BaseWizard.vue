<template>
    <div class="container-fluid h-100">
        <div class="row h-100">
            <div class="col-48 h-100">
                <div class="card border-0 rounded-0 h-100 shadow-lg">
                    <!-- Wizard Header -->
                    <div class="card-header border-0 rounded-0 px-4 py-3" :class="headerClass">
                        <div class="d-flex align-items-start">
                            <div class="flex-grow-1">
                                <div class="fs-1d1 fw-bold shadow5 mb-1">
                                    <i class="fa-solid fa-magic me-2 text-primary"></i>
                                    {{inputs.title || shared.translate('Wizard')}}
                                </div>
                                <div class="fs-9 text-secondary mb-1" v-if="inputs.subtitle">
                                    {{inputs.subtitle}}
                                </div>
                                <div class="fs-d9 text-muted" v-if="inputs.description">
                                    {{inputs.description}}
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Progress Indicator (Top Navigation) -->
                    <div class="card-header bg-primary-subtle opacity-75 border-0 px-3 py-2" v-if="showProgressIndicator">
                        <div :id="'wizNavTop_' + wizardId" 
                             data-ae-widget="bsTabsAutoNav" 
                             :data-ae-widget-options='navTopOptions'>
                            <ul class="nav nav-underline nav-justified mb-0">
                                <li class="nav-item">
                                    <button class="nav-link">
                                        <i class="fa-solid fa-circle-notch fa-spin"></i>
                                    </button>
                                </li>
                            </ul>
                        </div>
                    </div>

                    <!-- Wizard Steps Content -->
                    <div class="card-body p-0 overflow-auto">
                        <div class="tab-content h-100" :id="'wizContent_' + wizardId">
                            <div v-for="(step, index) in steps" 
                                 :key="'step_' + index"
                                 :class="['tab-pane', 'h-100', 'fade', { 'show active': currentStepIndex === index }]"
                                 :id="'wizStep_' + wizardId + '_' + index"
                                 role="tabpanel"
                                 :tabindex="currentStepIndex === index ? 0 : -1"
                                 :data-ae-tab-title="step.title || shared.translate('Step') + ' ' + (index + 1)"
                                 :data-ae-tab-icon="step.icon || 'fa-circle'"
                                 :data-ae-widget="step.useValidation ? 'inputsRegulator' : ''"
                                 :data-ae-widget-options="step.useValidation ? '{}' : ''">
                                
                                <!-- Step Content -->
                                <div class="p-4 h-100" :id="'stepContent_' + wizardId + '_' + index">
                                    <slot :name="'step-' + index" :step="step" :stepIndex="index" :wizardData="wizardData">
                                        <!-- Render component if specified -->
                                        <component-loader 
                                            v-if="step.component" 
                                            :src="step.component" 
                                            :cid="'wizStep_' + wizardId + '_' + index"
                                            :uid="'wizStep_' + wizardId + '_' + index">
                                        </component-loader>
                                        <!-- Render HTML content if specified -->
                                        <div v-else-if="step.content" v-html="step.content"></div>
                                        <!-- Default placeholder -->
                                        <div v-else class="text-center text-muted py-5">
                                            <i class="fa-solid fa-3x fa-circle-notch fa-spin mb-3"></i>
                                            <div>{{shared.translate('Step')}} {{index + 1}}: {{step.title}}</div>
                                        </div>
                                    </slot>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Wizard Footer (Navigation Buttons) -->
                    <div class="card-footer border-0 bg-light-subtle p-0">
                        <div class="container-fluid">
                            <!-- Navigation Buttons (Previous/Next) -->
                            <div class="row py-2 px-3">
                                <div class="col-12">
                                    <div :id="'wizNavBottom_' + wizardId"
                                         data-ae-widget="bsTabsAutoNav"
                                         :data-ae-widget-options='navBottomOptions'>
                                        <table class="w-100 text-center mb-0">
                                            <tr>
                                                <td style="width:100px">
                                                    <button class="btn btn-sm btn-link fw-bold text-decoration-none bg-hover-light w-100">
                                                        <i class="fa-solid fa-circle-notch fa-spin"></i>
                                                    </button>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </div>
                            </div>

                            <!-- Additional Action Buttons and Step Counter -->
                            <div class="row border-top bg-white py-2 px-3" v-if="showActionButtons">
                                <div class="col-12">
                                    <div class="d-flex justify-content-between align-items-center">
                                        <!-- Left Side: Action Buttons -->
                                        <div class="d-flex">
                                            <button class="btn btn-sm btn-outline-secondary me-2" 
                                                    @click="cancel" 
                                                    v-if="allowCancel">
                                                <i class="fa-solid fa-times me-1"></i>
                                                <span>{{shared.translate(inputs.cancelText || 'Cancel')}}</span>
                                            </button>
                                            <button class="btn btn-sm btn-outline-info" 
                                                    @click="saveProgress" 
                                                    v-if="allowSaveProgress">
                                                <i class="fa-solid fa-save me-1"></i>
                                                <span>{{shared.translate('Save Progress')}}</span>
                                            </button>
                                        </div>
                                        
                                        <!-- Right Side: Step Counter -->
                                        <div class="d-flex align-items-center" v-if="showStepCounter">
                                            <span class="text-muted fs-d9 me-2">
                                                <i class="fa-solid fa-list-ol me-1"></i>
                                                {{shared.translate('Step')}}
                                            </span>
                                            <span class="fw-bold text-primary fs-d9">
                                                {{currentStepIndex + 1}}
                                            </span>
                                            <span class="text-muted fs-d9 mx-1">
                                                {{shared.translate('of')}}
                                            </span>
                                            <span class="fw-bold text-secondary fs-d9">
                                                {{steps.length}}
                                            </span>
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
</template>

<script>
    let _this = { 
        cid: "", 
        c: null, 
        inputs: {}, 
        wizardId: "",
        currentStepIndex: 0,
        steps: [],
        wizardData: {},
        completedSteps: [],
        navTopWidget: null,
        navBottomWidget: null,
        navObserver: null
    };

    export default {
        methods: {
            /**
             * Navigate to next step
             */
            async next() {
                if (!await this.validateCurrentStep()) return false;
                
                if (this.currentStepIndex < this.steps.length - 1) {
                    await this.leaveStep(this.currentStepIndex);
                    this.currentStepIndex++;
                    await this.enterStep(this.currentStepIndex);
                    this.updateNavigation();
                } else {
                    await this.finish();
                }
            },

            /**
             * Navigate to previous step
             */
            async previous() {
                if (this.currentStepIndex > 0) {
                    await this.leaveStep(this.currentStepIndex);
                    this.currentStepIndex--;
                    await this.enterStep(this.currentStepIndex);
                    this.updateNavigation();
                }
            },

            /**
             * Navigate to specific step
             */
            async goToStep(stepIndex) {
                if (stepIndex < 0 || stepIndex >= this.steps.length) return false;
                if (stepIndex === this.currentStepIndex) return true;

                // Validate current step before leaving
                if (!await this.validateCurrentStep()) return false;

                await this.leaveStep(this.currentStepIndex);
                this.currentStepIndex = stepIndex;
                await this.enterStep(this.currentStepIndex);
                this.updateNavigation();
                return true;
            },

            /**
             * Validate current step
             */
            async validateCurrentStep() {
                const step = this.steps[this.currentStepIndex];
                if (!step) return true;

                // Check if step is optional
                if (step.optional === true) return true;

                // Use inputsRegulator validation if enabled
                if (step.useValidation) {
                    const stepContentId = `stepContent_${this.wizardId}_${this.currentStepIndex}`;
                    if (!isAreaValidById(stepContentId)) {
                        return false;
                    }
                }

                // Call custom validation function if provided
                if (step.onValidate && typeof step.onValidate === 'function') {
                    try {
                        const result = await step.onValidate(this.wizardData, this.currentStepIndex);
                        if (result === false) return false;
                    } catch (ex) {
                        console.error('Step validation error:', ex);
                        showError(shared.translate('Validation failed') + ': ' + ex.message);
                        return false;
                    }
                }

                return true;
            },

            /**
             * Enter a step
             */
            async enterStep(stepIndex) {
                const step = this.steps[stepIndex];
                if (!step) return;

                // Mark step as visited
                if (this.completedSteps.indexOf(stepIndex) === -1) {
                    this.completedSteps.push(stepIndex);
                }

                // Call onEnter callback if provided
                if (step.onEnter && typeof step.onEnter === 'function') {
                    try {
                        await step.onEnter(this.wizardData, stepIndex);
                    } catch (ex) {
                        console.error('Step onEnter error:', ex);
                    }
                }

                // Trigger step entered event
                if (this.inputs.onStepEntered && typeof this.inputs.onStepEntered === 'function') {
                    try {
                        await this.inputs.onStepEntered(stepIndex, step, this.wizardData);
                    } catch (ex) {
                        console.error('onStepEntered callback error:', ex);
                    }
                }
            },

            /**
             * Leave a step
             */
            async leaveStep(stepIndex) {
                const step = this.steps[stepIndex];
                if (!step) return;

                // Call onLeave callback if provided
                if (step.onLeave && typeof step.onLeave === 'function') {
                    try {
                        await step.onLeave(this.wizardData, stepIndex);
                    } catch (ex) {
                        console.error('Step onLeave error:', ex);
                    }
                }

                // Trigger step left event
                if (this.inputs.onStepLeft && typeof this.inputs.onStepLeft === 'function') {
                    try {
                        await this.inputs.onStepLeft(stepIndex, step, this.wizardData);
                    } catch (ex) {
                        console.error('onStepLeft callback error:', ex);
                    }
                }
            },

            /**
             * Finish wizard
             */
            async finish() {
                // Validate all steps if required
                if (this.inputs.validateAllOnFinish) {
                    for (let i = 0; i < this.steps.length; i++) {
                        if (this.steps[i].optional !== true) {
                            this.currentStepIndex = i;
                            if (!await this.validateCurrentStep()) {
                                showWarning(shared.translate('Please complete all required steps'));
                                return;
                            }
                        }
                    }
                }

                // Call finish callback
                if (this.inputs.onFinish && typeof this.inputs.onFinish === 'function') {
                    try {
                        await this.inputs.onFinish(this.wizardData, this.completedSteps);
                    } catch (ex) {
                        console.error('onFinish callback error:', ex);
                        showError(shared.translate('Error completing wizard') + ': ' + ex.message);
                        return;
                    }
                }

                // Close wizard if it's a modal
                if (this.inputs.closeOnFinish !== false) {
                    this.close();
                }
            },

            /**
             * Cancel wizard
             */
            async cancel() {
                if (this.inputs.onCancel && typeof this.inputs.onCancel === 'function') {
                    try {
                        const result = await this.inputs.onCancel(this.wizardData, this.currentStepIndex);
                        if (result === false) return; // Cancel was prevented
                    } catch (ex) {
                        console.error('onCancel callback error:', ex);
                    }
                }

                this.close();
            },

            /**
             * Save progress
             */
            async saveProgress() {
                if (this.inputs.onSaveProgress && typeof this.inputs.onSaveProgress === 'function') {
                    try {
                        await this.inputs.onSaveProgress({
                            wizardData: this.wizardData,
                            currentStep: this.currentStepIndex,
                            completedSteps: this.completedSteps
                        });
                        showSuccess(shared.translate('Progress saved successfully'));
                    } catch (ex) {
                        console.error('onSaveProgress callback error:', ex);
                        showError(shared.translate('Error saving progress') + ': ' + ex.message);
                    }
                } else {
                    // Default: save to localStorage
                    try {
                        const progressData = {
                            wizardId: this.wizardId,
                            wizardData: this.wizardData,
                            currentStep: this.currentStepIndex,
                            completedSteps: this.completedSteps,
                            timestamp: new Date().toISOString()
                        };
                        localStorage.setItem(`wizard_progress_${this.wizardId}`, JSON.stringify(progressData));
                        showSuccess(shared.translate('Progress saved successfully'));
                    } catch (ex) {
                        showError(shared.translate('Error saving progress'));
                    }
                }
            },

            /**
             * Load saved progress
             */
            loadProgress() {
                if (this.inputs.onLoadProgress && typeof this.inputs.onLoadProgress === 'function') {
                    try {
                        const saved = this.inputs.onLoadProgress();
                        if (saved) {
                            this.wizardData = saved.wizardData || {};
                            this.currentStepIndex = saved.currentStep || 0;
                            this.completedSteps = saved.completedSteps || [];
                        }
                    } catch (ex) {
                        console.error('onLoadProgress callback error:', ex);
                    }
                } else {
                    // Default: load from localStorage
                    try {
                        const saved = localStorage.getItem(`wizard_progress_${this.wizardId}`);
                        if (saved) {
                            const progressData = JSON.parse(saved);
                            this.wizardData = progressData.wizardData || {};
                            this.currentStepIndex = progressData.currentStep || 0;
                            this.completedSteps = progressData.completedSteps || [];
                        }
                    } catch (ex) {
                        console.error('Error loading progress:', ex);
                    }
                }
            },

            /**
             * Update navigation state
             */
            updateNavigation() {
                this.$nextTick(() => {
                    // Re-run widgets to update navigation state
                    runWidgets();
                });
            },

            /**
             * Close wizard
             */
            close() {
                if (this.inputs.closeOnCancel !== false) {
                    shared.closeComponent(this.cid);
                }
            },

            /**
             * Get current step
             */
            getCurrentStep() {
                return this.steps[this.currentStepIndex];
            },

            /**
             * Check if step is completed
             */
            isStepCompleted(stepIndex) {
                return this.completedSteps.indexOf(stepIndex) !== -1;
            },

            /**
             * Check if step is accessible (for conditional navigation)
             */
            isStepAccessible(stepIndex) {
                const step = this.steps[stepIndex];
                if (!step) return false;

                // Check conditional access
                if (step.condition && typeof step.condition === 'function') {
                    return step.condition(this.wizardData, stepIndex);
                }

                return true;
            }
        },

        computed: {
            /**
             * Navigation top options
             */
            navTopOptions() {
                return JSON.stringify({
                    tabsContentsId: `wizContent_${this.wizardId}`,
                    justAllowByBackNext: this.inputs.justAllowByBackNext !== false,
                    dir: this.inputs.dir || 'ltr',
                    navStyle: this.inputs.navStyle || 'nav-underline nav-justified'
                });
            },

            /**
             * Navigation bottom options
             */
            navBottomOptions() {
                return JSON.stringify({
                    tabsContentsId: `wizContent_${this.wizardId}`,
                    mode: 'back-next',
                    justAllowByBackNext: this.inputs.justAllowByBackNext !== false,
                    nextTitle: this.inputs.nextText || shared.translate('Next'),
                    prevTitle: this.inputs.prevText || shared.translate('Previous'),
                    dir: this.inputs.dir || 'ltr'
                });
            },

            /**
             * Header class
             */
            headerClass() {
                return this.inputs.headerClass || 'bg-success-subtle';
            },

            /**
             * Show progress indicator
             */
            showProgressIndicator() {
                return this.inputs.showProgressIndicator !== false && this.steps.length > 1;
            },

            /**
             * Show action buttons
             */
            showActionButtons() {
                return this.inputs.showActionButtons !== false;
            },

            /**
             * Show step counter
             */
            showStepCounter() {
                return this.inputs.showStepCounter !== false;
            },

            /**
             * Allow cancel
             */
            allowCancel() {
                return this.inputs.allowCancel !== false;
            },

            /**
             * Allow save progress
             */
            allowSaveProgress() {
                return this.inputs.allowSaveProgress === true;
            }
        },

        setup(props) {
            _this.cid = props['cid'];
            _this.inputs = shared["params_" + _this.cid] || {};
            _this.wizardId = _this.inputs.wizardId || genUN('wiz_');
            
            // Initialize steps
            _this.steps = _this.inputs.steps || [];
            
            // Initialize wizard data
            _this.wizardData = _this.inputs.initialData || {};

            // Load saved progress if enabled
            if (_this.inputs.autoLoadProgress === true) {
                _this.loadProgress();
            }

            return {};
        },

        data() {
            return _this;
        },

        created() {
            _this.c = this;
        },

        async mounted() {
            initVueComponent(_this);

            // Enter first step
            if (_this.steps.length > 0) {
                await _this.c.enterStep(0);
            }

            // Apply padding to navigation tabs after widget initialization
            this.$nextTick(() => {
                setTimeout(() => {
                    const navTopId = `#wizNavTop_${_this.wizardId}`;
                    $(navTopId).find('.nav-link').css({
                        'padding-left': '8px',
                        'padding-right': '8px'
                    });
                    
                    // Watch for dynamically added nav-links
                    _this.navObserver = new MutationObserver(() => {
                        $(navTopId).find('.nav-link').css({
                            'padding-left': '8px',
                            'padding-right': '8px'
                        });
                    });
                    
                    const navElement = document.querySelector(navTopId);
                    if (navElement) {
                        _this.navObserver.observe(navElement, {
                            childList: true,
                            subtree: true
                        });
                    }
                }, 100);
            });

            // Set up keyboard navigation
            $(document).on('keydown.wizard_' + _this.wizardId, (e) => {
                if (e.target.tagName === 'INPUT' || e.target.tagName === 'TEXTAREA' || e.target.tagName === 'SELECT') {
                    return;
                }
                
                if (e.key === 'ArrowRight' && (e.ctrlKey || e.metaKey)) {
                    e.preventDefault();
                    _this.c.next();
                } else if (e.key === 'ArrowLeft' && (e.ctrlKey || e.metaKey)) {
                    e.preventDefault();
                    _this.c.previous();
                }
            });
        },

        beforeUnmount() {
            // Clean up keyboard listeners
            $(document).off('keydown.wizard_' + _this.wizardId);
            
            // Clean up mutation observer if exists
            if (_this.navObserver) {
                _this.navObserver.disconnect();
            }
        },

        props: { cid: String }
    }
</script>

<style scoped>
    .shadow5 {
        text-shadow: 0 1px 2px rgba(0, 0, 0, 0.1);
    }

    .tab-pane {
        transition: opacity 0.15s linear;
    }

    .tab-pane:not(.active) {
        display: none;
    }

    /* Wizard Footer Styling */
    .card-footer .container-fluid {
        padding-left: 0;
        padding-right: 0;
    }

    .card-footer .row {
        margin-left: 0;
        margin-right: 0;
    }

    /* Navigation Buttons Styling */
    #wizNavBottom_ + * table {
        margin-bottom: 0;
    }

    /* Step Counter Styling */
    .wizard-step-counter {
        display: inline-flex;
        align-items: center;
        gap: 0.25rem;
    }

    /* Header Icon Styling */
    .card-header .fa-magic {
        opacity: 0.8;
    }

    /* Top Navigation Tabs Styling - 8px padding on left and right for nav-link buttons */
    :deep(.card-header ul.nav .nav-item .nav-link),
    :deep(.card-header ul.nav .nav-item .nav-link.active),
    :deep(.card-header ul.nav .nav-item .nav-link:not(.active)) {
        padding-left: 8px !important;
        padding-right: 8px !important;
    }
</style>

