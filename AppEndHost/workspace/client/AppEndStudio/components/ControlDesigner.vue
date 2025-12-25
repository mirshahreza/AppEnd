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
                            <i class="fa-solid fa-layer-group me-1"></i>Layout
                        </div>
                        <div class="component-grid">
                            <div v-for="comp in layoutComponents" :key="comp.type"
                                 class="component-item"
                                 draggable="true"
                                 @dragstart="onDragStart(comp)">
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
                                 draggable="true"
                                 @dragstart="onDragStart(comp)">
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
                                 draggable="true"
                                 @dragstart="onDragStart(comp)">
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
                                 draggable="true"
                                 @dragstart="onDragStart(comp)">
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

    export default {
        data() {
            return {
                cid: "",
                c: null,
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

                layoutComponents: [
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

                // Auto-sync on content change
                const self = this;
                this.aceVueEditor.session.on('change', function () {
                    self.syncEditorContent();
                });
            },

            onDragStart(component) {
                this.draggedComponent = component;
            },

            onDragOver(e) {
                e.preventDefault();
                e.dataTransfer.dropEffect = 'copy';
            },

            onDrop(e) {
                e.preventDefault();
                if (!this.draggedComponent) return;

                const canvas = document.getElementById('designCanvas');
                const isEmpty = this.isCanvasEmpty || canvas.innerHTML.includes('empty-canvas');

                if (isEmpty) {
                    if (!['div', 'container', 'container-fluid', 'card'].includes(this.draggedComponent.type)) {
                        alert('Please start with a layout component');
                        this.draggedComponent = null;
                        return;
                    }
                }

                const dropTarget = e.target.closest('.designer-element') || canvas;
                if (dropTarget === canvas && isEmpty) {
                    canvas.innerHTML = this.draggedComponent.template;
                    this.isCanvasEmpty = false;
                } else if (dropTarget === canvas) {
                    canvas.innerHTML += this.draggedComponent.template;
                } else {
                    dropTarget.insertAdjacentHTML('beforeend', this.draggedComponent.template);
                }

                this.draggedComponent = null;
                this.saveState();
                this.attachElementHandlers();
            },

            onCanvasClick(e) {
                if (e.target.id === 'designCanvas') {
                    this.deselectElement();
                }
            },

            attachElementHandlers() {
                const canvas = document.getElementById('designCanvas');
                if (!canvas) return;

                canvas.querySelectorAll('.designer-element').forEach(el => {
                    el.classList.remove('designer-element', 'designer-hover', 'designer-selected');
                });

                const elements = canvas.querySelectorAll('*:not(#designCanvas)');
                elements.forEach(el => {
                    el.classList.add('designer-element');

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
                    };

                    el.ondragover = (e) => {
                        e.preventDefault();
                        e.stopPropagation();
                        e.dataTransfer.dropEffect = 'move';
                    };

                    el.ondrop = (e) => {
                        e.preventDefault();
                        e.stopPropagation();
                        if (this.draggedElement && this.draggedElement !== el) {
                            el.parentNode.insertBefore(this.draggedElement, el.nextSibling);
                            this.saveState();
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
                if (confirm('Delete this element?')) {
                    this.selectedDomElement.remove();
                    this.deselectElement();
                    this.saveState();
                }
            },

            editElementText() {
                if (!this.selectedDomElement) return;
                const newText = prompt('Edit text:', this.selectedDomElement.textContent);
                if (newText !== null) {
                    this.selectedDomElement.textContent = newText;
                    this.selectedElement.text = newText;
                    this.saveState();
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
                    this.attachElementHandlers();
                    this.canUndo = this.historyIndex > 0;
                    this.canRedo = true;
                }
            },

            redoAction() {
                if (this.historyIndex < this.history.length - 1) {
                    this.historyIndex++;
                    const canvas = document.getElementById('designCanvas');
                    canvas.innerHTML = this.history[this.historyIndex];
                    this.attachElementHandlers();
                    this.canRedo = this.historyIndex < this.history.length - 1;
                    this.canUndo = true;
                }
            },

            clearCanvas() {
                if (confirm('Clear canvas?')) {
                    const canvas = document.getElementById('designCanvas');
                    canvas.innerHTML = '<div class="empty-canvas text-center text-muted p-5"><i class="fa-solid fa-paintbrush fa-3x mb-3"></i><p>Drag a layout component to start</p></div>';
                    this.isCanvasEmpty = true;
                    this.deselectElement();
                    this.saveState();
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
                setTimeout(() => {
                    this.saving = false;
                    alert('Component saved!');
                }, 500);
            },

            loadComponent() {
                this.loading = true;
                setTimeout(() => {
                    if (this.componentCode) {
                        // Extract template from component code
                        const templateMatch = this.componentCode.match(/<template>([\s\S]*?)<\/template>/);
                        if (templateMatch) {
                            const canvas = document.getElementById('designCanvas');
                            canvas.innerHTML = templateMatch[1].trim();
                            this.isCanvasEmpty = false;
                            this.attachElementHandlers();
                        }

                        // Update Ace editor with loaded content
                        if (this.aceVueEditor) {
                            this.aceVueEditor.setValue(this.componentCode, -1);
                        }
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
                cid: props.cid
            };
        },
        
        created() { 
            this.c = this; 
        },
        mounted() {
            this.saveState();
            this.$nextTick(() => {
                this.attachElementHandlers();
                // Initialize Ace editor after DOM is ready
                setTimeout(() => {
                    this.initAceEditor();
                }, 100);
            });
        },
        beforeUnmount() {
            // Cleanup Ace editor
            if (this.aceVueEditor) this.aceVueEditor.destroy();
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
</style>
