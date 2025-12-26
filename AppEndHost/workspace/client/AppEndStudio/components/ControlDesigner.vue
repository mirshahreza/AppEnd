<template>
    <div class="control-designer h-100 d-flex flex-column">
        <!-- Header Toolbar -->
        <div class="designer-header p-2 bg-body-subtle border-bottom">
            <div class="hstack gap-1">
                <!-- Actions -->
                <button type="button" class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="saveComponent" :disabled="saving">
                    <i class="fa-solid fa-save fa-bounce" style="--fa-animation-iteration-count:1"></i> <span>{{ saving ? 'Saving...' : 'Save' }}</span>
                </button>
                <button type="button" class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="readFileContent" :disabled="loading">
                    <i class="fa-solid fa-sync fa-bounce" style="--fa-animation-iteration-count:1"></i> <span>Reload</span>
                </button>
                <div class="vr"></div>
                <button type="button" class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="previewComponent">
                    <i class="fa-solid fa-eye fa-bounce" style="--fa-animation-iteration-count:1"></i> <span>Preview</span>
                </button>
                <div class="vr"></div>
                <button type="button" class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="undoAction" :disabled="!canUndo">
                    <i class="fa-solid fa-undo"></i> <span>Undo</span>
                </button>
                <button type="button" class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="redoAction" :disabled="!canRedo">
                    <i class="fa-solid fa-redo"></i> <span>Redo</span>
                </button>

            </div>
        </div>

        <!-- Main Content Area -->
        <div class="designer-content flex-grow-1 d-flex" data-flex-splitter-horizontal style="flex: auto;">

            <!-- Column 1 (Left): Toolbox -->
            <div v-if="toolboxPanelVisible" class="toolbox-panel bg-white border-end" style="min-width:150px;width:10%;">
                <div class="toolbox-body p-2 overflow-auto">
                    <div v-for="group in toolboxGroups" :key="group.key">
                        <div v-if="group.alwaysShow || (typeof group.show === 'function' ? group.show.call($data) : group.show)" class="component-group mb-3">
                            <div class="group-title small fw-bold text-secondary mb-2">
                                <i :class="group.icon + ' me-1'"></i>{{ group.title }}
                            </div>
                            <div class="component-grid">
                                <div v-for="comp in group.items" :key="comp.type"
                                     class="component-item"
                                     :class="{ 'disabled-item': !group.alwaysShow && isCanvasEmpty }"
                                     :draggable="group.alwaysShow || !isCanvasEmpty"
                                     @dragstart="onDragStart(comp, $event)">
                                    <i :class="comp.icon + ' fa-2x'"></i>
                                    <div class="item-label">{{ comp.label }}</div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Splitter 1 (Toolbox / Canvas) with Handle -->
            <div role="separator" tabindex="1" class="splitter-handle bg-light" style="width:.5%;">
                <div class="splitter-icon" @click="toggleToolboxPanel" :title="toolboxPanelVisible ? 'Hide Toolbox' : 'Show Toolbox'">
                    <i class="fa-solid" :class="toolboxPanelVisible ? 'fa-chevron-left' : 'fa-chevron-right'"></i>
                </div>
            </div>

            <!-- Column 2 (Middle): Canvas/Designer -->
            <div class="canvas-area d-flex flex-column" ref="canvasArea"
                 :style="getCanvasStyle()">
                <!-- Canvas -->
                <div class="canvas-container flex-grow-1 bg-body-secondary"
                     @drop="onDrop"
                     @dragover="onDragOver"
                     @click="onCanvasClick">
                    <div id="designCanvas"
                         class="design-canvas bg-white shadow-sm"
                         :class="{ 'canvas-empty': isCanvasEmpty }">
                         <!-- Content will be dynamically loaded -->
                    </div>
                </div>

                <!-- Element Properties (Class/Style) -->
                <div v-if="selectedElement" class="px-2 py-2 bg-white border-top small flex-shrink-0">
                    <div class="mb-2">
                        <div class="input-group input-group-sm">
                            <span class="input-group-text bg-light text-secondary" style="width: 60px;">Class</span>
                            <input type="text" class="form-control" 
                                   :value="selectedElement.classes" 
                                   @change="updateElementClasses($event.target.value)"
                                   placeholder="CSS classes...">
                        </div>
                    </div>
                    <div>
                        <div class="input-group input-group-sm">
                            <span class="input-group-text bg-light text-secondary" style="width: 60px;">Style</span>
                            <input type="text" class="form-control" 
                                   :value="selectedElement.style" 
                                   @change="updateElementStyle($event.target.value)"
                                   placeholder="Inline styles...">
                        </div>
                    </div>
                </div>

                <!-- Breadcrumb Path -->
                <div v-if="selectionPath.length > 0" class="px-2 py-1 bg-white border-top small flex-shrink-0">
                    <nav aria-label="breadcrumb" style="--bs-breadcrumb-divider: '>';">
                        <ol class="breadcrumb mb-0">
                            <li v-for="(item, index) in selectionPath" :key="index" 
                                class="breadcrumb-item" 
                                :class="{ 'active': index === selectionPath.length - 1 }">
                                <a href="#" 
                                   @click.prevent="selectElement(item.element)" 
                                   class="text-decoration-none"
                                   :class="index === selectionPath.length - 1 ? 'text-dark fw-bold' : 'text-secondary'">
                                    {{ item.tagName }}<span v-if="item.id" class="text-muted ms-1">#{{ item.id }}</span><span v-if="item.classes" class="text-primary ms-1 fst-italic" style="font-size:0.9em">[{{ item.classes }}]</span>
                                </a>
                            </li>
                        </ol>
                    </nav>
                </div>
            </div>

            <!-- Splitter 2 (Canvas / Code) with Handle -->
            <div role="separator" tabindex="1" class="splitter-handle bg-light" style="width:.5%;">
                <div class="splitter-icon" @click="toggleCodePanel" :title="codePanelVisible ? 'Hide Code' : 'Show Code'">
                    <i class="fa-solid" :class="codePanelVisible ? 'fa-chevron-right' : 'fa-chevron-left'"></i>
                </div>
            </div>

            <!-- Column 3 (Right): Code Panel - Single unified editor -->
            <div v-if="codePanelVisible" class="code-panel bg-white d-flex flex-column border-start" ref="codePanel" style="min-width:300px;width:30%;">

                <!-- Code Editor Area -->
                <div class="code-editor-container flex-grow-1">
                    <div id="aceVueEditor" class="ace-editor"></div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    shared.setAppTitle("Visual Component Designer");
    shared.setAppSubTitle(` (${getQueryString("edt")})`);


    export default {
        data() {
            return {
                cid: "",
                c: null,
                filePath: "",
                codePanelVisible: true,
                toolboxPanelVisible: true,
                loading: false,
                saving: false,
                isCanvasEmpty: true,

                // Ace Editor instance
                aceVueEditor: null,

                componentCode: "",

                draggedComponent: null,
                draggedElement: null,

                selectedElement: null,
                selectedDomElement: null,
                selectionPath: [],

                history: [],
                historyIndex: -1,
                canUndo: false,
                canRedo: false,

                // Sync control flags
                isSyncingFromCanvas: false,
                isSyncingFromCode: false,
                syncDebounceTimer: null,
                codeSyncDebounceTimer: null,

                // Toolbox groups definition for dynamic rendering (merged items)
                toolboxGroups: [
                    {
                        key: 'rootElements',
                        title: 'Root Elements',
                        icon: 'fa-solid fa-layer-group',
                        show: true,
                        alwaysShow: true,
                        items: [
                            {
                                type: 'container', label: 'Container Fixed', icon: 'fa-solid fa-arrows-left-right-to-line',
                                template: '<div class="container p-3"><div class="row"><div class="col">Col 1</div><div class="col">Col 2</div></div></div>'
                            },
                            {
                                type: 'container-fluid', label: 'Container Fluid', icon: 'fa-solid fa-arrows-left-right',
                                template: '<div class="container-fluid p-3"><div class="row"><div class="col">Col 1</div><div class="col">Col 2</div></div></div>'
                            },
                            {
                                type: 'div', label: 'Div', icon: 'fa-solid fa-square',
                                template: '<div class="p-3">Div Container</div>'
                            },
                            {
                                type: 'card', label: 'Card', icon: 'fa-solid fa-id-card',
                                template: '<div class="card"><div class="card-header">Header</div><div class="card-body"><h5 class="card-title">Title</h5><p class="card-text">Content</p></div></div>'
                            }
                        ]
                    },
                    {
                        key: 'htmlComponents',
                        title: 'HTML',
                        icon: 'fa-solid fa-code',
                        show: function() { return !this.isCanvasEmpty; },
                        alwaysShow: false,
                        items: [
                            { type: 'h1', label: 'Heading', icon: 'fa-solid fa-heading', template: '<h1>Heading</h1>' },
                            { type: 'p', label: 'Paragraph', icon: 'fa-solid fa-paragraph', template: '<p>Paragraph text</p>' },
                            { type: 'span', label: 'Span', icon: 'fa-solid fa-text-width', template: '<span>Text</span>' },
                            { type: 'hr', label: 'Line', icon: 'fa-solid fa-minus', template: '<hr />' }
                        ]
                    },
                    {
                        key: 'bootstrapComponents',
                        title: 'Bootstrap',
                        icon: 'fa-brands fa-bootstrap',
                        show: function() { return !this.isCanvasEmpty; },
                        alwaysShow: false,
                        items: [
                            {
                                type: 'row', label: 'Row', icon: 'fa-solid fa-grip-lines',
                                template: '<div class="row"><div class="col">Column 1</div><div class="col">Column 2</div></div>'
                            },
                            {
                                type: 'col', label: 'Column', icon: 'fa-solid fa-table-columns',
                                template: '<div class="col p-2">Column</div>'
                            },
                            {
                                type: 'button', label: 'Button', icon: 'fa-solid fa-hand-pointer',
                                template: '<button type="button" class="btn btn-primary">Button</button>'
                            },
                            {
                                type: 'alert', label: 'Alert', icon: 'fa-solid fa-triangle-exclamation',
                                template: '<div class="alert alert-info" role="alert">This is an alert</div>'
                            }
                        ]
                    },
                    {
                        key: 'formComponents',
                        title: 'Forms',
                        icon: 'fa-solid fa-wpforms',
                        show: function() { return !this.isCanvasEmpty; },
                        alwaysShow: false,
                        items: [
                            {
                                type: 'input', label: 'Input', icon: 'fa-solid fa-keyboard',
                                template: '<input type="text" class="form-control" placeholder="Enter text" />'
                            },
                            {
                                type: 'textarea', label: 'Textarea', icon: 'fa-solid fa-align-left',
                                template: '<textarea class="form-control" rows="3"></textarea>'
                            },
                            {
                                type: 'select', label: 'Select', icon: 'fa-solid fa-list',
                                template: '<select class="form-select"><option>Option 1</option><option>Option 2</option></select>'
                            },
                            {
                                type: 'checkbox', label: 'Checkbox', icon: 'fa-solid fa-check-square',
                                template: '<div class="form-check"><input class="form-check-input" type="checkbox" /><label class="form-check-label">Checkbox</label></div>'
                            }
                        ]
                    }
                ],

                // Bootstrap 5 structure rules for designer validation
                designerRules: {
                    body: { allowedChildren: ["container", "container-fluid", "nav", "header", "footer", "main", "modal", "offcanvas", "toast-container"] },
                    container: { allowedParents: ["body", "main", "section", "div", "card-body", "column"], allowedChildren: ["row", "div", "nav"] },
                    "container-fluid": { allowedParents: ["body", "main", "section", "div", "card-body", "column"], allowedChildren: ["row", "div", "nav"] },
                    row: { allowedParents: ["container", "container-fluid", "column", "card-body", "modal-body"], allowedChildren: ["column"] },
                    column: { allowedParents: ["row"], allowedChildren: ["*any_except_row_and_container*"] },
                    card: { allowedParents: ["column", "div"], allowedChildren: ["card-img-top", "card-header", "card-body", "card-footer", "list-group", "card-img-overlay"] },
                    "card-header": { allowedParents: ["card"], allowedChildren: ["h1", "h2", "h3", "h4", "h5", "h6", "nav-tabs", "nav-pills", "div", "text"] },
                    "card-body": { allowedParents: ["card"], allowedChildren: ["card-title", "card-subtitle", "card-text", "row", "btn", "div", "table"] },
                    // ... (add more as needed)
                }
            };
        },

        methods: {
            readFileContent() {
                if (!this.filePath) return;
                
                // filePath already includes the relative path like '/a.CustomComponents/Sample.vue'
                // Remove leading slash if exists
                const cleanPath = this.filePath.startsWith('/') ? this.filePath.substring(1) : this.filePath;
                const fullPath = cleanPath;
                this.loading = true;
                
                rpcAEP("GetFileContent", { "PathToRead": fullPath }, (res) => {
                    this.componentCode = R0R(res);
                    this.loading = false;
                    
                    // Load the content into canvas and editor
                    this.loadComponent();
                });
            },
            
            toggleToolboxPanel() {
                this.toolboxPanelVisible = !this.toolboxPanelVisible;
            },

            toggleCodePanel() {
                this.codePanelVisible = !this.codePanelVisible;
                
                // Wait for DOM update then reinitialize editor when showing
                if (this.codePanelVisible) {
                    this.$nextTick(() => {
                        setTimeout(() => {
                            // Destroy old editor if exists
                            if (this.aceVueEditor) {
                                try {
                                    this.aceVueEditor.destroy();
                                } catch (e) {
                                    // Ignore if already destroyed
                                }
                                this.aceVueEditor = null;
                            }
                            
                            // Always reinitialize when panel is shown
                            this.initAceEditor();
                        }, 250);
                    });
                }
            },

            getCanvasStyle() {
                let width = 'width:59.5%;';
                
                if (!this.toolboxPanelVisible && !this.codePanelVisible) {
                    width = 'width:99%;';
                } else if (!this.toolboxPanelVisible && this.codePanelVisible) {
                    width = 'width:69.5%;';
                } else if (this.toolboxPanelVisible && !this.codePanelVisible) {
                    width = 'width:89.5%;';
                }
                
                return 'min-width:300px;' + width;
            },

            syncCanvasToCode() {
                // Prevent sync loops
                if (this.isSyncingFromCode) return;
                
                // Clear existing timer
                if (this.syncDebounceTimer) {
                    clearTimeout(this.syncDebounceTimer);
                }
                
                // Debounce the sync operation
                this.syncDebounceTimer = setTimeout(() => {
                    this.isSyncingFromCanvas = true;
                    
                    const canvas = document.getElementById('designCanvas');
                    if (canvas && canvas.innerHTML.trim() !== '') {
                        // Extract current template from component code
                        const templateMatch = this.componentCode.match(/<template>([\s\S]*?)<\/template>/);
                        if (templateMatch) {
                            // Get canvas HTML and format it nicely
                            const canvasHTML = canvas.innerHTML;
                            const formattedHTML = this.formatHTML(canvasHTML);
                            
                            // Update component code with new template
                            this.componentCode = this.componentCode.replace(
                                /<template>[\s\S]*?<\/template>/,
                                `<template>\n${formattedHTML}\n</template>`
                            );
                            
                            // Update Ace editor without triggering change event
                            if (this.aceVueEditor) {
                                const cursorPos = this.aceVueEditor.getCursorPosition();
                                const scrollTop = this.aceVueEditor.session.getScrollTop();
                                
                                // Temporarily remove change listener
                                this.aceVueEditor.session.off('change', this.onCodeEditorChange);
                                this.aceVueEditor.setValue(this.componentCode, -1);
                                
                                // Restore cursor and scroll position
                                this.aceVueEditor.moveCursorToPosition(cursorPos);
                                this.aceVueEditor.session.setScrollTop(scrollTop);
                                
                                // Re-attach change listener
                                this.aceVueEditor.session.on('change', this.onCodeEditorChange);
                            }
                        }
                    }
                    
                    setTimeout(() => {
                        this.isSyncingFromCanvas = false;
                    }, 100);
                }, 300);
            },

            syncCodeToCanvas() {
                // Prevent sync loops
                if (this.isSyncingFromCanvas) return;
                
                // Clear existing timer
                if (this.codeSyncDebounceTimer) {
                    clearTimeout(this.codeSyncDebounceTimer);
                }
                
                // Debounce the sync operation
                this.codeSyncDebounceTimer = setTimeout(() => {
                    this.isSyncingFromCode = true;
                    
                    // Sync editor content to componentCode
                    if (this.aceVueEditor) {
                        this.componentCode = this.aceVueEditor.getValue();
                    }
                    
                    // Extract template and update canvas
                    const templateMatch = this.componentCode.match(/<template>([\s\S]*?)<\/template>/);
                    if (templateMatch) {
                        const canvas = document.getElementById('designCanvas');
                        if (canvas) {
                            const newHTML = templateMatch[1].trim();
                            
                            // Only update if content actually changed
                            if (canvas.innerHTML !== newHTML) {
                                // Store selection before update
                                const selectedId = this.selectedDomElement ? this.selectedDomElement.getAttribute('data-did') : null;
                                
                                canvas.innerHTML = newHTML;
                                
                                // Update isCanvasEmpty based on actual content
                                this.isCanvasEmpty = newHTML === '' || !newHTML;
                                
                                this.attachElementHandlers();
                                
                                // Restore selection if possible
                                if (selectedId && !this.isCanvasEmpty) {
                                    const restoredElement = canvas.querySelector(`[data-did="${selectedId}"]`);
                                    if (restoredElement) {
                                        this.selectElement(restoredElement);
                                    } else {
                                        this.deselectElement();
                                    }
                                }
                            }
                        }
                    }
                    
                    setTimeout(() => {
                        this.isSyncingFromCode = false;
                    }, 100);
                }, 500);
            },

            formatHTML(html) {
                // Basic HTML formatting for better readability
                if (!html) return '';
                
                // Remove designer-specific classes and attributes
                let formatted = html.replace(/\s*(designer-element|designer-hover|designer-selected|drop-target-active|dragging)\s*/g, '');
                formatted = formatted.replace(/\s+class=""/g, '');
                formatted = formatted.replace(/\s+class="\s+"/g, '');
                
                // Simple indentation (basic implementation)
                let indent = 0;
                const indentSize = 4;
                formatted = formatted.replace(/>\s*</g, '>\n<');
                
                const lines = formatted.split('\n');
                const result = [];
                
                lines.forEach(line => {
                    const trimmed = line.trim();
                    if (!trimmed) return;
                    
                    // Decrease indent for closing tags
                    if (trimmed.startsWith('</')) {
                        indent = Math.max(0, indent - indentSize);
                    }
                    
                    result.push(' '.repeat(indent) + trimmed);
                    
                    // Increase indent for opening tags (not self-closing or immediately closed)
                    if (trimmed.startsWith('<') && !trimmed.startsWith('</') && !trimmed.endsWith('/>') && !trimmed.match(/<[^>]+>.*<\/[^>]+>$/)) {
                        indent += indentSize;
                    }
                });
                
                return result.join('\n');
            },

            onCodeEditorChange() {
                // This will be bound to Ace editor change event
                this.syncCodeToCanvas();
            },

            syncEditorContent() {
                if (this.aceVueEditor) {
                    this.componentCode = this.aceVueEditor.getValue();
                }
            },

            initAceEditor() {
                // Initialize Vue Editor with complete component code

                this.aceVueEditor = ace.edit("aceVueEditor", {
                    theme: "ace/theme/cloud9_day",
                    mode: "ace/mode/html",
                    value: this.componentCode,
                    fontSize: 14
                });

                // Bind change event with our sync method
                this.aceVueEditor.session.on('change', this.onCodeEditorChange);
            },

            onDragStart(component, event) {
                // Check if canvas is empty
                const canvas = document.getElementById('designCanvas');
                const isEmpty = !canvas || canvas.innerHTML.trim() === '' || this.isCanvasEmpty;
                
                // If canvas is empty, only allow root elements
                if (isEmpty && !['div', 'container', 'container-fluid', 'card'].includes(component.type)) {
                    event.preventDefault();
                    event.stopPropagation();
                    return false;
                }
                
                // Allow drag for root elements or when canvas is not empty
                this.draggedComponent = component;
            },

            onDragOver(e) {
                e.preventDefault();
                e.dataTransfer.dropEffect = this.draggedElement ? 'move' : 'copy';
                
                // Add visual feedback for drop target
                const target = e.target.closest('.designer-element');
                if (target && target !== this.draggedElement) {
                    // Remove previous drop target highlights
                    document.querySelectorAll('.drop-target-active').forEach(el => {
                        el.classList.remove('drop-target-active');
                    });
                    target.classList.add('drop-target-active');
                }
                
                // Visual feedback for canvas drop (root level) - show warning indicator
                if (e.target.id === 'designCanvas' && !this.isCanvasEmpty) {
                    e.dataTransfer.dropEffect = 'none';
                    const canvas = document.getElementById('designCanvas');
                    if (canvas && !canvas.classList.contains('drop-not-allowed')) {
                        canvas.classList.add('drop-not-allowed');
                        setTimeout(() => {
                            canvas.classList.remove('drop-not-allowed');
                        }, 1000);
                    }
                }
            },

            onDrop(e) {
                e.preventDefault();
                e.stopPropagation();
                // Remove drop target highlights
                document.querySelectorAll('.drop-target-active').forEach(el => {
                    el.classList.remove('drop-target-active');
                });
                // Handle dragging from toolbox (new component)
                if (this.draggedComponent) {
                    const canvas = document.getElementById('designCanvas');
                    const isEmpty = this.isCanvasEmpty || canvas.innerHTML.trim() === '';
                    let dropTarget = e.target;
                    if (isEmpty) {
                        dropTarget = canvas;
                    } else {
                        if (dropTarget.id === 'designCanvas') {
                            dropTarget = canvas.querySelector('.designer-element');
                        } else if (!dropTarget.classList.contains('designer-element')) {
                            dropTarget = dropTarget.closest('.designer-element');
                        }
                    }
                    if (!dropTarget) {
                        showError('Could not find a valid drop target. Please try dropping on an element.');
                        this.draggedComponent = null;
                        return;
                    }
                    // Validation for drop rules
                    if (!this.isValidDrop(this.draggedComponent.type, dropTarget)) {
                        showError('This object cannot be dropped here!');
                        this.draggedComponent = null;
                        return;
                    }
                    if (isEmpty) {
                        canvas.innerHTML = this.draggedComponent.template;
                        this.isCanvasEmpty = false;
                    } else {
                        dropTarget.insertAdjacentHTML('beforeend', this.draggedComponent.template);
                    }
                    this.draggedComponent = null;
                    this.saveState();
                    this.attachElementHandlers();
                    this.syncCanvasToCode();
                }
                // Handle dragging existing elements (rearrange/nest)
                if (this.draggedElement) {
                    const dropTarget = e.target.closest('.designer-element');
                    if (!dropTarget) {
                        showError('Cannot move element to root level. Template must have a single root element.');
                        this.draggedElement = null;
                        return;
                    }
                    // Validation for move rules
                    const draggedType = this.draggedElement.classList.contains('col') ? 'col'
                        : this.draggedElement.classList.contains('row') ? 'row'
                        : this.draggedElement.classList.contains('container') ? 'container'
                        : this.draggedElement.classList.contains('container-fluid') ? 'container-fluid'
                        : this.draggedElement.classList.contains('card') ? 'card'
                        : this.draggedElement.tagName.toLowerCase();
                    if (!this.isValidDrop(draggedType, dropTarget)) {
                        showError('This element cannot be moved here!');
                        this.draggedElement = null;
                        return;
                    }
                    if (dropTarget && dropTarget !== this.draggedElement && !this.isDescendant(dropTarget, this.draggedElement)) {
                        if (e.shiftKey) {
                            dropTarget.appendChild(this.draggedElement);
                        } else {
                            const targetParent = dropTarget.parentNode;
                            if (targetParent && targetParent.id === 'designCanvas') {
                                const draggedParent = this.draggedElement.parentNode;
                                if (draggedParent && draggedParent.id !== 'designCanvas') {
                                    showError('Cannot move element to root level. Template must have a single root element. Use Shift to drop inside.');
                                    this.draggedElement = null;
                                    return;
                                }
                            }
                            targetParent.insertBefore(this.draggedElement, dropTarget.nextSibling);
                        }
                        this.saveState();
                        this.syncCanvasToCode();
                    }
                    this.draggedElement = null;
                    this.attachElementHandlers();
                }
            },

            
            // Helper method to check if an element is a descendant of another
            isDescendant(parent, child) {
                let node = child.parentNode;
                while (node !== null) {
                    if (node === parent) {
                        return true;
                    }
                    node = node.parentNode;
                }
                return false;
            },

            onCanvasClick(e) {
                if (e.target.id === 'designCanvas') {
                    this.deselectElement();
                }
            },

            attachElementHandlers() {
                const canvas = document.getElementById('designCanvas');
                if (!canvas) return;
                canvas.querySelectorAll('.designer-element, .drop-target-active').forEach(el => {
                    el.classList.remove('designer-element', 'designer-hover', 'designer-selected', 'drop-target-active');
                });
                const elements = canvas.querySelectorAll('*:not(#designCanvas)');
                let idCounter = 0;
                elements.forEach(el => {
                    el.classList.add('designer-element');
                    if (!el.getAttribute('data-did')) {
                        el.setAttribute('data-did', `d-${Math.floor(Math.random() * 1000000)}-${idCounter++}`);
                    }
                    el.onclick = (e) => {
                        e.stopPropagation();
                        this.selectElement(el);
                    };
                    el.onmouseenter = (e) => {
                        e.stopPropagation();
                        if (!el.classList.contains('designer-selected')) {
                            el.classList.add('designer-hover');
                        }
                    };
                    el.onmouseleave = () => {
                        el.classList.remove('designer-hover');
                    };
                    el.ondblclick = (e) => {
                        e.stopPropagation();
                        this.editElementText();
                    };
                    el.draggable = true;
                    el.ondragstart = (e) => {
                        e.stopPropagation();
                        e.dataTransfer.effectAllowed = 'move';
                        this.draggedElement = el;
                        this.draggedComponent = null;
                        el.classList.add('dragging');
                    };
                    el.ondragend = (e) => {
                        e.stopPropagation();
                        el.classList.remove('dragging');
                        document.querySelectorAll('.drop-target-active').forEach(elem => {
                            elem.classList.remove('drop-target-active');
                        });
                    };
                    el.ondragover = (e) => {
                        e.preventDefault();
                        e.stopPropagation();
                        let draggedType = null;
                        if (this.draggedElement) {
                            draggedType = this.draggedElement.classList.contains('col') ? 'col'
                                : this.draggedElement.classList.contains('row') ? 'row'
                                : this.draggedElement.classList.contains('container') ? 'container'
                                : this.draggedElement.classList.contains('container-fluid') ? 'container-fluid'
                                : this.draggedElement.classList.contains('card') ? 'card'
                                : this.draggedElement.tagName.toLowerCase();
                            if (this.draggedElement !== el && !this.isDescendant(el, this.draggedElement) && this.isValidDrop(draggedType, el)) {
                                e.dataTransfer.dropEffect = 'move';
                                el.classList.add('drop-target-active');
                            }
                        } else if (this.draggedComponent) {
                            if (this.isValidDrop(this.draggedComponent.type, el)) {
                                e.dataTransfer.dropEffect = 'copy';
                                el.classList.add('drop-target-active');
                            }
                        }
                    };
                    el.ondragleave = (e) => {
                        e.stopPropagation();
                        el.classList.remove('drop-target-active');
                    };
                    el.ondrop = (e) => {
                        e.preventDefault();
                        e.stopPropagation();
                        el.classList.remove('drop-target-active');
                        // Validation for insert and move rules
                        if (this.draggedElement && this.draggedElement !== el && !this.isDescendant(el, this.draggedElement)) {
                            let draggedType = this.draggedElement.classList.contains('col') ? 'col'
                                : this.draggedElement.classList.contains('row') ? 'row'
                                : this.draggedElement.classList.contains('container') ? 'container'
                                : this.draggedElement.classList.contains('container-fluid') ? 'container-fluid'
                                : this.draggedElement.classList.contains('card') ? 'card'
                                : this.draggedElement.tagName.toLowerCase();
                            if (!this.isValidDrop(draggedType, el)) {
                                showError('This element cannot be moved here!');
                                this.draggedElement = null;
                                return;
                            }
                            if (e.shiftKey) {
                                el.appendChild(this.draggedElement);
                            } else {
                                const targetParent = el.parentNode;
                                if (targetParent && targetParent.id === 'designCanvas') {
                                    const draggedParent = this.draggedElement.parentNode;
                                    if (draggedParent && draggedParent.id !== 'designCanvas') {
                                        showError('Cannot move element to root level. Use Shift to drop inside the element.');
                                        this.draggedElement = null;
                                        return;
                                    }
                                }
                                targetParent.insertBefore(this.draggedElement, el.nextSibling);
                            }
                            this.draggedElement = null;
                            this.saveState();
                            this.attachElementHandlers();
                            this.syncCanvasToCode();
                        }
                        if (this.draggedComponent) {
                            if (!this.isValidDrop(this.draggedComponent.type, el)) {
                                showError('This object cannot be dropped here!');
                                this.draggedComponent = null;
                                return;
                            }
                            el.insertAdjacentHTML('beforeend', this.draggedComponent.template);
                            this.draggedComponent = null;
                            this.saveState();
                            this.attachElementHandlers();
                            this.syncCanvasToCode();
                        }
                    };
                });
            },

            selectElement(domElement) {
                this.deselectElement();
                domElement.classList.add('designer-selected');
                domElement.classList.remove('designer-hover');
                this.selectedDomElement = domElement;
                this.selectedElement = {
                    tagName: domElement.tagName.toLowerCase(),
                    id: domElement.id || '',
                    classes: Array.from(domElement.classList).filter(c => !c.startsWith('designer-')).join(' '),
                    style: domElement.getAttribute('style') || '',
                    text: domElement.textContent,
                    html: domElement.innerHTML
                };
                this.updateSelectionPath(domElement);
            },

            updateElementClasses(value) {
                if (!this.selectedDomElement) return;
                
                const designerClasses = Array.from(this.selectedDomElement.classList)
                    .filter(c => c.startsWith('designer-') || c === 'drop-target-active' || c === 'dragging');
                
                this.selectedDomElement.className = value || '';
                designerClasses.forEach(c => this.selectedDomElement.classList.add(c));
                
                this.selectedElement.classes = value || '';
                this.updateSelectionPath(this.selectedDomElement);
                this.saveState();
                this.syncCanvasToCode();
            },

            updateElementStyle(value) {
                if (!this.selectedDomElement) return;
                this.selectedDomElement.setAttribute('style', value || '');
                this.selectedElement.style = value || '';
                this.saveState();
                this.syncCanvasToCode();
            },

            deselectElement() {
                const canvas = document.getElementById('designCanvas');
                if (canvas) {
                    canvas.querySelectorAll('.designer-selected').forEach(el => {
                        el.classList.remove('designer-selected');
                    });
                }
                this.selectedElement = null;
                this.selectedDomElement = null;
                this.selectionPath = [];
            },

            updateSelectionPath(element) {
                const path = [];
                let current = element;
                const canvas = document.getElementById('designCanvas');

                while (current && current !== canvas) {
                    if (current.classList.contains('designer-element')) {
                        path.unshift({
                            tagName: current.tagName.toLowerCase(),
                            id: current.id,
                            classes: Array.from(current.classList).filter(c => !c.startsWith('designer-') && !c.startsWith('drop-target-') && c !== 'dragging').join(' '),
                            element: current
                        });
                    }
                    current = current.parentNode;
                }
                this.selectionPath = path;
            },

            editElementText() {
                if (!this.selectedDomElement) return;
                const newText = prompt('Edit text:', this.selectedDomElement.textContent);
                if (newText !== null) {
                    this.selectedDomElement.textContent = newText;
                    this.selectedElement.text = newText;
                    this.saveState();
                    
                    // Sync canvas changes to code
                    this.syncCanvasToCode();
                }
            },

            saveState() {
                const canvas = document.getElementById('designCanvas');
                if (!canvas) return;
                const state = canvas.innerHTML;
                if (this.historyIndex < this.history.length - 1) {
                    this.history = this.history.slice(0, this.historyIndex + 1);
                }
                this.history.push(state);
                this.historyIndex++;
                if (this.history.length > 50) {
                    this.history.shift();
                    this.historyIndex--;
                }
                this.canUndo = this.historyIndex > 0;
                this.canRedo = false;
            },

            undoAction() {
                if (this.historyIndex > 0) {
                    this.historyIndex--;
                    const canvas = document.getElementById('designCanvas');
                    canvas.innerHTML = this.history[this.historyIndex];
                    
                    // Update isCanvasEmpty state
                    this.isCanvasEmpty = canvas.innerHTML.trim() === '';
                    
                    this.attachElementHandlers();
                    this.canUndo = this.historyIndex > 0;
                    this.canRedo = true;
                    
                    // Sync canvas changes to code
                    this.syncCanvasToCode();
                }
            },

            redoAction() {
                if (this.historyIndex < this.history.length - 1) {
                    this.historyIndex++;
                    const canvas = document.getElementById('designCanvas');
                    canvas.innerHTML = this.history[this.historyIndex];
                    
                    // Update isCanvasEmpty state
                    this.isCanvasEmpty = canvas.innerHTML.trim() === '';
                    
                    this.attachElementHandlers();
                    this.canRedo = this.historyIndex < this.history.length - 1;
                    this.canUndo = true;
                    
                    // Sync canvas changes to code
                    this.syncCanvasToCode();
                }
            },

            saveComponent() {
                this.saving = true;
                this.syncEditorContent();
                const canvas = document.getElementById('designCanvas');
                if (canvas && canvas.innerHTML.trim() !== '') {
                    // Extract template from component code and update with canvas content
                    const templateMatch = this.componentCode.match(/<template>([\s\S]*?)<\/template>/);
                    if (templateMatch) {
                        const formattedHTML = this.formatHTML(canvas.innerHTML);
                        this.componentCode = this.componentCode.replace(
                            /<template>[\s\S]*?<\/template>/,
                            `<template>\n${formattedHTML}\n</template>`
                        );
                        if (this.aceVueEditor) {
                            this.aceVueEditor.setValue(this.componentCode, -1);
                        }
                    }
                }
                
                // Save to server if filePath exists
                if (this.filePath) {
                    // filePath already includes the relative path like '/a.CustomComponents/Sample.vue'
                    // Remove leading slash if exists
                    const cleanPath = this.filePath.startsWith('/') ? this.filePath.substring(1) : this.filePath;
                    const fullPath = cleanPath;
                    rpcAEP("SaveFileContent", { 
                        "PathToWrite": fullPath, 
                        "FileContent": this.componentCode.trim() 
                    }, (res) => {
                        this.saving = false;
                        showSuccess("Component saved successfully!");
                    });
                } else {
                    setTimeout(() => {
                        this.saving = false;
                        alert('Component saved!');
                    }, 500);
                }
            },

            loadComponent() {
                this.loading = true;
                setTimeout(() => {
                    const canvas = document.getElementById('designCanvas');
                    
                    if (this.componentCode) {
                        // Extract template from component code
                        const templateMatch = this.componentCode.match(/<template>([\s\S]*?)<\/template>/);
                        if (templateMatch) {
                            const templateContent = templateMatch[1].trim();
                            
                            // Set canvas content and empty state
                            this.isCanvasEmpty = !templateContent || templateContent === '';
                            if (canvas) {
                                canvas.innerHTML = templateContent || '';

                                // Attach handlers to newly added elements
                                this.attachElementHandlers();
                            }
                        }

                        // Update Ace editor with loaded content
                        if (this.aceVueEditor) {
                            this.aceVueEditor.setValue(this.componentCode, -1);
                        }
                    } else {
                        // No component code, ensure canvas is empty
                        this.isCanvasEmpty = true;
                        if (canvas) {
                            canvas.innerHTML = '';
                        }
                    }
                    this.loading = false;
                }, 300);
            },

            previewComponent() {
                if (!this.filePath) return;
                let path = this.filePath.replace(/\\/g, "/");
                path = path.replace(/workspace\/client\//i, "");
                if (!path.startsWith('/')) path = '/'+path;
                window.open(`?c=${path}`, '_blank');
            },

            // Helper method to validate drop (insertion or move)
            isValidDrop(childType, parentElement) {
                if (!parentElement) return false;
                const parentClass = parentElement.className || '';
                // 1. فقط یک عنصر ریشه در سطح canvas مجاز است
                if (parentElement.id === 'designCanvas') {
                    // فقط یک ریشه مجاز است
                    const canvas = document.getElementById('designCanvas');
                    const rootCount = Array.from(canvas.children).filter(child => child.classList && child.classList.contains('designer-element')).length;
                    if (rootCount > 0) return false;
                    // فقط عناصر ریشه مجازند
                    if (!['div', 'container', 'container-fluid', 'card'].includes(childType)) return false;
                    return true;
                }
                // 2. عناصر ریشه می‌توانند هر جایی درج شوند (محدودیت خاصی ندارند)
                if (['div', 'container', 'container-fluid', 'card'].includes(childType)) return true;
                // 3. row فقط داخل container یا container-fluid
                if (childType === 'row') {
                    if (parentClass.includes('container') || parentClass.includes('container-fluid')) return true;
                    return false;
                }
                // 4. col فقط dentro row
                if (childType === 'col') {
                    if (parentClass.includes('row')) return true;
                    return false;
                }
                // سایر موارد فعلاً آزاد
                return true;
            }
        },

        setup(props) {
            return {
                cid: props.cid,
                filePath: getQueryString("edt") || ""
            };
        },
        
        created() { 
            this.c = this; 
        },
        mounted() {
            this.saveState();
            this.$nextTick(() => {
                // Initialize Ace editor after DOM is ready
                setTimeout(() => {
                    this.initAceEditor();
                    // Load file content if filePath exists
                    if (this.filePath) {
                        this.readFileContent();
                    } else {
                        // Set initial empty state - no placeholder, just empty
                        this.isCanvasEmpty = true;
                        this.attachElementHandlers();
                    }
                }, 100);
            });
        },
        beforeUnmount() {
            // Cleanup Ace editor
            if (this.aceVueEditor) {
                this.aceVueEditor.session.off('change', this.onCodeEditorChange);
                this.aceVueEditor.destroy();
            }
            
            // Clear timers
            if (this.syncDebounceTimer) clearTimeout(this.syncDebounceTimer);
            if (this.codeSyncDebounceTimer) clearTimeout(this.codeSyncDebounceTimer);
        },

        props: { cid: String }
    }
</script>

<style scoped>
    .control-designer {
        background: #f8f9fa;
    }

    .designer-header {
        flex-shrink: 0;
    }

    .designer-content {
        overflow: hidden;
    }

    /* Canvas Area - Flex grow to fill space */
    .canvas-area {
        min-width: 300px;
        flex-grow: 1;
        flex-basis: 0;
        min-height: 0; /* Ensure it can shrink below content size */
    }

    /* Toolbox Panel - Fixed width */
    .toolbox-panel {
        flex-shrink: 0;
        height: 100%; /* Changed from 100vh to 100% to respect parent height */
        min-width: 150px;
        width: 10%;
        display: flex;
        flex-direction: column;
    }

    .toolbox-body {
        flex: 1 1 auto;
        overflow-y: auto;
        height: 100%;
        min-height: 0; /* مهم برای flexbox */
        max-height: 100%;
        /* padding: 2px;  اگر لازم بود */
    }

    .component-grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(90px, 1fr));
        gap: 8px;
        width: 100%;
        margin: 0 auto;
    }

    .component-item {
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        padding: 12px 8px;
        background: #f8f9fa;
        border: 1px solid #dee2e6;
        border-radius: 6px;
        cursor: move;
        transition: all 0.2s;
        min-width: 90px;
        max-width: 100px;
        box-sizing: border-box;
    }

        .component-item:hover {
            background: #e9ecef;
            transform: translateY(-2px);
            box-shadow: 0 4px 8px rgba(0,0,0,0.1);
        }

        .component-item.disabled-item {
            opacity: 0.5;
            cursor: not-allowed;
            pointer-events: none;
        }

    .item-label {
        font-size: 0.7rem;
        margin-top: 4px;
        text-align: center;
    }

    /* Canvas */
    .canvas-container {
        overflow: auto;
        padding: 8px;
        min-height: 0;
    }

    .design-canvas {
        min-height: 0;
        height: auto;
        padding: 0;
        border-radius: 4px;
        transition: min-height 0.2s ease;
        margin: 0;
        background: #fff;
    }

    .design-canvas.canvas-empty {
        min-height: 1.5rem;
        padding: 0;
        display: flex;
        align-items: center;
    }

    /* Designer Elements */
    :deep(.designer-element) {
        position: relative;
        cursor: pointer;
        transition: all 0.2s;
    }

    :deep(.designer-hover) {
        outline: 2px dashed #17a2b8 !important;
        outline-offset: 2px;
    }

    :deep(.designer-selected) {
        outline: 3px solid #0d6efd !important;
        outline-offset: 2px;
        background-color: rgba(13, 110, 253, 0.05) !important;
    }
    
    :deep(.dragging) {
        opacity: 0.5;
        cursor: move;
    }
    
    :deep(.drop-target-active) {
        outline: 3px dashed #198754 !important;
        outline-offset: 2px;
        background-color: rgba(25, 135, 84, 0.05) !important;
    }
    
    .drop-not-allowed {
        outline: 3px dashed #dc3545 !important;
        outline-offset: -3px;
        background-color: rgba(220, 53, 69, 0.05) !important;
    }

    /* Code Panel - Horizontal Accordion */
    .code-panel {
        flex-shrink: 0;
    }

    .code-editor-container {
        position: relative;
        overflow: hidden;
    }

    .ace-editor {
        width: 100%;
        height: 100%;
        font-family: 'Consolas', 'Monaco', monospace;
        font-size: 14px;
    }

    .cursor-pointer {
        cursor: pointer;
    }

    .code-selector {
        flex-shrink: 0;
    }

    .code-editor-wrapper {
        overflow: hidden;
    }

    /* Ace Editor Specific Styles */
    .ace_editor {
        position: relative;
        font-family: 'Courier New', Courier, monospace !important;
        font-size: 13px;
    }

    .ace_gutter {
        background: #f8f9fa;
        border-right: 1px solid #dee2e6;
    }

    .ace_print-margin {
        width: 1px;
        background: #e9ecef;
    }

    .ace_scroller {
        margin-bottom: 5px;
    }

    /* Splitter Handle */
    .splitter-handle {
        position: relative;
        cursor: col-resize;
        transition: background 0.2s;
        display: flex;
        align-items: center;
        justify-content: center;
        user-select: none;
    }

        .splitter-handle:hover {
            background: #dee2e6 !important;
        }

    .splitter-icon {
        display: flex;
        align-items: center;
        justify-content: center;
        width: 20px;
        height: 40px;
        color: #6c757d;
        font-size: 12px;
        opacity: 0.5;
        transition: opacity 0.2s;
        cursor: pointer;
        border-radius: 4px;
    }

        .splitter-icon:hover {
            opacity: 1;
            background: rgba(13, 110, 253, 0.1);
        }

        .splitter-icon:active {
            background: rgba(13, 110, 253, 0.2);
        }

    /* Warning indicator for invalid drop targets */
    .drop-not-allowed {
        outline: 2px dashed #dc3545 !important;
        outline-offset: 2px;
    }

</style>
