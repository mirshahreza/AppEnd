<template>
    <div class="control-designer h-100 d-flex flex-column">
        <!-- Header Toolbar -->
        <div class="designer-header p-2 bg-body-subtle border-bottom">
            <div class="hstack gap-1">
                <!-- Actions -->
                <button type="button" class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="saveComponent" :disabled="saving">
                    <i class="fa-solid fa-save fa-bounce" style="--fa-animation-iteration-count:1"></i> <span>{{ saving ? 'Saving...' : 'Save' }}</span>
                </button>
                <button type="button" class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="loadComponent" :disabled="loading">
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

                    <!-- Layout Components -->
                    <div class="component-group mb-3">
                        <div class="group-title small fw-bold text-secondary mb-2">
                            <i class="fa-solid fa-layer-group me-1"></i>Root Elements
                        </div>
                        <div class="component-grid">
                            <div v-for="comp in rootElements" :key="comp.type"
                                 class="component-item"
                                 draggable="true"
                                 @dragstart="onDragStart(comp, $event)">
                                <i :class="comp.icon + ' fa-2x'"></i>
                                <div class="item-label">{{ comp.label }}</div>
                            </div>
                        </div>
                    </div>

                    <!-- HTML Elements -->
                    <div class="component-group mb-3" v-show="!isCanvasEmpty">
                        <div class="group-title small fw-bold text-secondary mb-2">
                            <i class="fa-solid fa-code me-1"></i>HTML
                        </div>
                        <div class="component-grid">
                            <div v-for="comp in htmlComponents" :key="comp.type"
                                 class="component-item"
                                 :class="{ 'disabled-item': isCanvasEmpty }"
                                 :draggable="!isCanvasEmpty"
                                 @dragstart="onDragStart(comp, $event)">
                                <i :class="comp.icon + ' fa-2x'"></i>
                                <div class="item-label">{{ comp.label }}</div>
                            </div>
                        </div>
                    </div>

                    <!-- Bootstrap -->
                    <div class="component-group mb-3" v-show="!isCanvasEmpty">
                        <div class="group-title small fw-bold text-secondary mb-2">
                            <i class="fa-brands fa-bootstrap me-1"></i>Bootstrap
                        </div>
                        <div class="component-grid">
                            <div v-for="comp in bootstrapComponents" :key="comp.type"
                                 class="component-item"
                                 :class="{ 'disabled-item': isCanvasEmpty }"
                                 :draggable="!isCanvasEmpty"
                                 @dragstart="onDragStart(comp, $event)">
                                <i :class="comp.icon + ' fa-2x'"></i>
                                <div class="item-label">{{ comp.label }}</div>
                            </div>
                        </div>
                    </div>

                    <!-- Form Elements -->
                    <div class="component-group" v-show="!isCanvasEmpty">
                        <div class="group-title small fw-bold text-secondary mb-2">
                            <i class="fa-solid fa-wpforms me-1"></i>Forms
                        </div>
                        <div class="component-grid">
                            <div v-for="comp in formComponents" :key="comp.type"
                                 class="component-item"
                                 :class="{ 'disabled-item': isCanvasEmpty }"
                                 :draggable="!isCanvasEmpty"
                                 @dragstart="onDragStart(comp, $event)">
                                <i :class="comp.icon + ' fa-2x'"></i>
                                <div class="item-label">{{ comp.label }}</div>
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
                <div class="canvas-container flex-grow-1 p-3 bg-body-secondary"
                     @drop="onDrop"
                     @dragover="onDragOver"
                     @click="onCanvasClick">
                    <div id="designCanvas"
                         class="design-canvas bg-white shadow-sm"
                         v-html="canvasContent"></div>
                </div>

                <!-- Properties Panel -->
                <div class="properties-panel p-2 bg-body-subtle border-top">
                    <div v-if="selectedElement">
                        <div class="d-flex align-items-center mb-2">
                            <span class="badge bg-primary me-2">{{ selectedElement.tagName }}</span>
                            <span v-if="selectedElement.id" class="badge bg-info me-2">#{{ selectedElement.id }}</span>
                            <span v-if="selectedElement.classes" class="badge bg-secondary">{{ selectedElement.classes }}</span>
                            <button type="button" class="btn btn-sm btn-link text-decoration-none bg-hover-light text-danger ms-auto" @click="deleteElement">
                                <i class="fa-solid fa-trash"></i>
                            </button>
                        </div>
                        <div class="hstack gap-1">
                            <button type="button" class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="editElementText">
                                <i class="fa-solid fa-pen"></i> <span>Text</span>
                            </button>
                            <button type="button" class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="editElementClasses">
                                <i class="fa-solid fa-palette"></i> <span>Classes</span>
                            </button>
                            <button type="button" class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="editElementAttributes">
                                <i class="fa-solid fa-cog"></i> <span>Attributes</span>
                            </button>
                            <button type="button" class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="editElementStyle">
                                <i class="fa-solid fa-paint-brush"></i> <span>Style</span>
                            </button>
                        </div>
                    </div>
                    <div v-else class="text-muted small">
                        <i class="fa-solid fa-info-circle me-1"></i>
                        Select an element to edit properties
                    </div>
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
                activeTab: "design",
                codePanelVisible: true,
                toolboxPanelVisible: true,
                codePanelWidth: 450,
                loading: false,
                saving: false,
                isCanvasEmpty: true,

                // Ace Editor instance
                aceVueEditor: null,

                componentCode: "",

                canvasContent: '<div class="empty-canvas text-center text-muted p-5"><i class="fa-solid fa-paintbrush fa-3x mb-3"></i><p>Drag a layout component to start</p></div>',
                draggedComponent: null,
                draggedElement: null,

                selectedElement: null,
                selectedDomElement: null,

                history: [],
                historyIndex: -1,
                canUndo: false,
                canRedo: false,

                // Sync control flags
                isSyncingFromCanvas: false,
                isSyncingFromCode: false,
                syncDebounceTimer: null,
                codeSyncDebounceTimer: null,

                rootElements: [
                    {
                        type: 'div', label: 'Div', icon: 'fa-solid fa-square',
                        template: '<div class="p-3">Div Container</div>'
                    },
                    {
                        type: 'container', label: 'Container', icon: 'fa-solid fa-rectangle-xmark',
                        template: '<div class="container p-3"><div class="row"><div class="col">Col 1</div><div class="col">Col 2</div></div></div>'
                    },
                    {
                        type: 'container-fluid', label: 'Fluid', icon: 'fa-solid fa-arrows-left-right',
                        template: '<div class="container-fluid p-3"><div class="row"><div class="col">Col 1</div><div class="col">Col 2</div></div></div>'
                    },
                    {
                        type: 'card', label: 'Card', icon: 'fa-solid fa-id-card',
                        template: '<div class="card"><div class="card-header">Header</div><div class="card-body"><h5 class="card-title">Title</h5><p class="card-text">Content</p></div></div>'
                    }
                ],

                htmlComponents: [
                    { type: 'h1', label: 'Heading', icon: 'fa-solid fa-heading', template: '<h1>Heading</h1>' },
                    { type: 'p', label: 'Paragraph', icon: 'fa-solid fa-paragraph', template: '<p>Paragraph text</p>' },
                    { type: 'span', label: 'Span', icon: 'fa-solid fa-text-width', template: '<span>Text</span>' },
                    {
                        type: 'img', label: 'Image', icon: 'fa-solid fa-image',
                        template: '<img src="https://via.placeholder.com/150" class="img-fluid" alt="Image" />'
                    },
                    { type: 'a', label: 'Link', icon: 'fa-solid fa-link', template: '<a href="#">Link</a>' },
                    { type: 'hr', label: 'Line', icon: 'fa-solid fa-minus', template: '<hr />' }
                ],

                bootstrapComponents: [
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
                ],

                formComponents: [
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
            };
        },

        methods: {
            switchTab(tab) {
                this.activeTab = tab;
            },

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
                    if (canvas && !canvas.innerHTML.includes('empty-canvas')) {
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
                                const selectedId = this.selectedDomElement ? this.selectedDomElement.getAttribute('data-designer-id') : null;
                                
                                canvas.innerHTML = newHTML;
                                
                                // Update isCanvasEmpty based on actual content
                                const hasEmptyCanvas = newHTML.includes('empty-canvas');
                                const hasNoContent = newHTML === '' || !newHTML;
                                this.isCanvasEmpty = hasEmptyCanvas || hasNoContent;
                                
                                this.attachElementHandlers();
                                
                                // Restore selection if possible
                                if (selectedId && !this.isCanvasEmpty) {
                                    const restoredElement = canvas.querySelector(`[data-designer-id="${selectedId}"]`);
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
                const defaultCode = `<template>
    <div class="component-container">
        <!-- Your HTML here -->
    </div>
</template>

<script>
export default {
    data() {
        return {}
    }
}
<\/script>

<style scoped>
/* Your CSS here */
</style>`;

                this.aceVueEditor = ace.edit("aceVueEditor", {
                    theme: "ace/theme/cloud9_day",
                    mode: "ace/mode/html",
                    value: this.componentCode || defaultCode,
                    fontSize: 14
                });

                // Bind change event with our sync method
                this.aceVueEditor.session.on('change', this.onCodeEditorChange);
            },

            onDragStart(component, event) {
                // Check if canvas is empty
                const canvas = document.getElementById('designCanvas');
                const isEmpty = !canvas || canvas.innerHTML.includes('empty-canvas') || this.isCanvasEmpty;
                
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
                
                console.log('onDrop called', {
                    draggedComponent: this.draggedComponent,
                    draggedElement: this.draggedElement,
                    targetId: e.target.id,
                    targetClass: e.target.className,
                    isCanvasEmpty: this.isCanvasEmpty
                });
                
                // Remove drop target highlights
                document.querySelectorAll('.drop-target-active').forEach(el => {
                    el.classList.remove('drop-target-active');
                });
                
                // Handle dragging from toolbox (new component)
                if (this.draggedComponent) {
                    const canvas = document.getElementById('designCanvas');
                    const isEmpty = this.isCanvasEmpty || canvas.innerHTML.includes('empty-canvas');

                    console.log('Dropping component', { isEmpty, componentType: this.draggedComponent.type });

                    if (isEmpty) {
                        // First element - must be a layout component
                        if (!['div', 'container', 'container-fluid', 'card'].includes(this.draggedComponent.type)) {
                            alert('Please start with a layout component (Div, Container, Fluid, or Card)');
                            this.draggedComponent = null;
                            return;
                        }
                        canvas.innerHTML = this.draggedComponent.template;
                        this.isCanvasEmpty = false;
                        
                        console.log('Added first element to canvas, isCanvasEmpty:', this.isCanvasEmpty);
                    } else {
                        // Canvas is not empty - need to find where to drop
                        let dropTarget = e.target;
                        
                        // If dropped on canvas itself, try to find the root element
                        if (dropTarget.id === 'designCanvas') {
                            dropTarget = canvas.querySelector('.designer-element');
                            console.log('Dropped on canvas, trying to find root element', dropTarget);
                        } else if (!dropTarget.classList.contains('designer-element')) {
                            // If dropped on a child element, find the closest designer element
                            dropTarget = dropTarget.closest('.designer-element');
                            console.log('Finding closest designer element', dropTarget);
                        }
                        
                        if (!dropTarget) {
                            console.log('No drop target found');
                            alert('Could not find a valid drop target. Please try dropping on an element.');
                            this.draggedComponent = null;
                            return;
                        }
                        
                        console.log('Dropping inside element', dropTarget);
                        // Drop inside the target element - ALWAYS ALLOWED
                        dropTarget.insertAdjacentHTML('beforeend', this.draggedComponent.template);
                    }

                    this.draggedComponent = null;
                    this.saveState();
                    this.attachElementHandlers();
                    
                    // Sync canvas changes to code
                    this.syncCanvasToCode();
                }
                
                // Handle dragging existing elements (rearrange/nest)
                if (this.draggedElement) {
                    const dropTarget = e.target.closest('.designer-element');
                    
                    if (!dropTarget) {
                        // Trying to drop directly on canvas - NOT ALLOWED
                        alert('Cannot move element to root level. Template must have a single root element.');
                        this.draggedElement = null;
                        return;
                    }
                    
                    if (dropTarget && dropTarget !== this.draggedElement && !this.isDescendant(dropTarget, this.draggedElement)) {
                        // Check if shift key is pressed for nesting inside
                        if (e.shiftKey) {
                            // Drop inside the target element - ALWAYS OK
                            dropTarget.appendChild(this.draggedElement);
                        } else {
                            // Drop after the target element (sibling)
                            // Only check if we're creating a sibling at root level
                            const targetParent = dropTarget.parentNode;
                            if (targetParent && targetParent.id === 'designCanvas') {
                                // Target is a root element, check if dragged element is also root
                                const draggedParent = this.draggedElement.parentNode;
                                if (draggedParent && draggedParent.id !== 'designCanvas') {
                                    // Moving from inside to root level - NOT ALLOWED
                                    alert('Cannot move element to root level. Template must have a single root element. Use Shift to drop inside.');
                                    this.draggedElement = null;
                                    return;
                                }
                                // Otherwise it's just reordering at root level which is OK
                            }
                            
                            targetParent.insertBefore(this.draggedElement, dropTarget.nextSibling);
                        }
                        
                        this.saveState();
                        
                        // Sync canvas changes to code
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

            // Helper method to count root elements in canvas
            getRootElementsCount() {
                const canvas = document.getElementById('designCanvas');
                if (!canvas) return 0;
                
                const rootElements = Array.from(canvas.children).filter(child => 
                    child.classList && 
                    child.classList.contains('designer-element') &&
                    !child.classList.contains('empty-canvas')
                );
                
                return rootElements.length;
            },

            // Helper method to get the root element
            getRootElement() {
                const canvas = document.getElementById('designCanvas');
                if (!canvas) return null;
                
                const rootElements = Array.from(canvas.children).filter(child => 
                    child.classList && 
                    child.classList.contains('designer-element') &&
                    !child.classList.contains('empty-canvas')
                );
                
                return rootElements.length > 0 ? rootElements[0] : null;
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
                    
                    // Add unique ID for selection tracking
                    if (!el.getAttribute('data-designer-id')) {
                        el.setAttribute('data-designer-id', `designer-${Date.now()}-${idCounter++}`);
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
                        this.draggedComponent = null; // Clear any toolbox drag
                        el.classList.add('dragging');
                    };
                    
                    el.ondragend = (e) => {
                        e.stopPropagation();
                        el.classList.remove('dragging');
                        // Clean up drop target highlights
                        document.querySelectorAll('.drop-target-active').forEach(elem => {
                            elem.classList.remove('drop-target-active');
                        });
                    };

                    el.ondragover = (e) => {
                        e.preventDefault();
                        e.stopPropagation();
                        
                        if (this.draggedElement && this.draggedElement !== el && !this.isDescendant(el, this.draggedElement)) {
                            e.dataTransfer.dropEffect = 'move';
                            // Add visual indicator
                            el.classList.add('drop-target-active');
                        } else if (this.draggedComponent) {
                            e.dataTransfer.dropEffect = 'copy';
                            el.classList.add('drop-target-active');
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
                        
                        // Handle moving existing elements
                        if (this.draggedElement && this.draggedElement !== el && !this.isDescendant(el, this.draggedElement)) {
                            if (e.shiftKey) {
                                // Shift key: drop inside the element - ALWAYS OK
                                el.appendChild(this.draggedElement);
                            } else {
                                // No shift key: drop after the element (sibling)
                                const targetParent = el.parentNode;
                                
                                // Check if we're at root level
                                if (targetParent && targetParent.id === 'designCanvas') {
                                    // Target is at root level
                                    const draggedParent = this.draggedElement.parentNode;
                                    if (draggedParent && draggedParent.id !== 'designCanvas') {
                                        // Trying to move from inside to root level - NOT ALLOWED
                                        alert('Cannot move element to root level. Use Shift to drop inside the element.');
                                        this.draggedElement = null;
                                        return;
                                    }
                                    // Otherwise it's reordering at root level which is OK
                                }
                                
                                targetParent.insertBefore(this.draggedElement, el.nextSibling);
                            }
                            
                            this.draggedElement = null;
                            this.saveState();
                            this.attachElementHandlers();
                            
                            // Sync canvas changes to code
                            this.syncCanvasToCode();
                        }
                        
                        // Handle dropping from toolbox
                        if (this.draggedComponent) {
                            // Drop inside this element - ALWAYS OK
                            el.insertAdjacentHTML('beforeend', this.draggedComponent.template);
                            this.draggedComponent = null;
                            this.saveState();
                            this.attachElementHandlers();
                            
                            // Sync canvas changes to code
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
                    text: domElement.textContent,
                    html: domElement.innerHTML
                };
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
            },

            deleteElement() {
                if (!this.selectedDomElement) return;
                
                // Check if this is the root element
                const canvas = document.getElementById('designCanvas');
                const isRootElement = this.selectedDomElement.parentNode === canvas;
                
                if (isRootElement) {
                    // Check if there are children - if yes, warn user
                    const hasChildren = this.selectedDomElement.children.length > 0;
                    if (hasChildren) {
                        if (!confirm('This is the root element and contains children. Deleting it will remove all content. Continue?')) {
                            return;
                        }
                    }
                }
                
                if (confirm('Delete this element?')) {
                    this.selectedDomElement.remove();
                    this.deselectElement();
                    
                    // Check if canvas is now empty (only has the design-canvas div or is truly empty)
                    const remainingElements = canvas.querySelectorAll('*:not(script):not(style)');
                    
                    if (remainingElements.length === 0 || canvas.innerHTML.trim() === '') {
                        canvas.innerHTML = '<div class="empty-canvas text-center text-muted p-5"><i class="fa-solid fa-paintbrush fa-3x mb-3"></i><p>Drag a layout component to start</p></div>';
                        this.isCanvasEmpty = true;
                    }
                    
                    this.saveState();
                    
                    // Sync canvas changes to code
                    this.syncCanvasToCode();
                }
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

            editElementClasses() {
                if (!this.selectedDomElement) return;
                const current = Array.from(this.selectedDomElement.classList).filter(c => !c.startsWith('designer-')).join(' ');
                const newClasses = prompt('Edit CSS classes:', current);
                if (newClasses !== null) {
                    const designerClasses = Array.from(this.selectedDomElement.classList).filter(c => c.startsWith('designer-'));
                    this.selectedDomElement.className = newClasses + ' ' + designerClasses.join(' ');
                    this.selectedElement.classes = newClasses;
                    this.saveState();
                    
                    // Sync canvas changes to code
                    this.syncCanvasToCode();
                }
            },

            editElementAttributes() {
                if (!this.selectedDomElement) return;
                alert('Attribute editor coming soon!');
            },

            editElementStyle() {
                if (!this.selectedDomElement) return;
                const currentStyle = this.selectedDomElement.getAttribute('style') || '';
                const newStyle = prompt('Edit inline style:', currentStyle);
                if (newStyle !== null) {
                    this.selectedDomElement.setAttribute('style', newStyle);
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
                    this.isCanvasEmpty = canvas.innerHTML.includes('empty-canvas');
                    
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
                    this.isCanvasEmpty = canvas.innerHTML.includes('empty-canvas');
                    
                    this.attachElementHandlers();
                    this.canRedo = this.historyIndex < this.history.length - 1;
                    this.canUndo = true;
                    
                    // Sync canvas changes to code
                    this.syncCanvasToCode();
                }
            },

            clearCanvas() {
                if (confirm('Clear canvas?')) {
                    const canvas = document.getElementById('designCanvas');
                    canvas.innerHTML = '<div class="empty-canvas text-center text-muted p-5"><i class="fa-solid fa-paintbrush fa-3x mb-3"></i><p>Drag a layout component to start</p></div>';
                    this.isCanvasEmpty = true;
                    this.deselectElement();
                    this.saveState();
                    
                    // Sync canvas changes to code
                    this.syncCanvasToCode();
                }
            },

            saveComponent() {
                this.saving = true;
                this.syncEditorContent();
                const canvas = document.getElementById('designCanvas');
                if (canvas && !canvas.innerHTML.includes('empty-canvas')) {
                    // Extract template from component code and update with canvas content
                    const templateMatch = this.componentCode.match(/<template>([\s\S]*?)<\/template>/);
                    if (templateMatch) {
                        this.componentCode = this.componentCode.replace(
                            /<template>[\s\S]*?<\/template>/,
                            `<template>\n${canvas.innerHTML}\n</template>`
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
                    if (this.componentCode) {
                        // Extract template from component code
                        const templateMatch = this.componentCode.match(/<template>([\s\S]*?)<\/template>/);
                        if (templateMatch) {
                            const canvas = document.getElementById('designCanvas');
                            const templateContent = templateMatch[1].trim();
                            
                            // Check if template is actually empty or just whitespace
                            if (!templateContent || templateContent === '') {
                                canvas.innerHTML = '<div class="empty-canvas text-center text-muted p-5"><i class="fa-solid fa-paintbrush fa-3x mb-3"></i><p>Drag a layout component to start</p></div>';
                                this.isCanvasEmpty = true;
                            } else {
                                canvas.innerHTML = templateContent;
                                this.isCanvasEmpty = false;
                            }
                            
                            this.attachElementHandlers();
                        }

                        // Update Ace editor with loaded content
                        if (this.aceVueEditor) {
                            this.aceVueEditor.setValue(this.componentCode, -1);
                        }
                    } else {
                        // No component code, ensure canvas is empty
                        const canvas = document.getElementById('designCanvas');
                        canvas.innerHTML = '<div class="empty-canvas text-center text-muted p-5"><i class="fa-solid fa-paintbrush fa-3x mb-3"></i><p>Drag a layout component to start</p></div>';
                        this.isCanvasEmpty = true;
                    }
                    this.loading = false;
                }, 300);
            },

            previewComponent() {
                alert('Preview coming soon!');
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
    }

    /* Toolbox Panel - Fixed width */
    .toolbox-panel {
        flex-shrink: 0;
    }

    .toolbox-body {
        height: 100%;
    }

    .component-grid {
        display: grid;
        grid-template-columns: repeat(2, 1fr);
        gap: 8px;
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
    }

    .design-canvas {
        min-height: 500px;
        padding: 20px;
        border-radius: 4px;
    }

    .empty-canvas {
        min-height: 500px;
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
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
