<template>
    <div class="control-designer h-100 d-flex flex-column">
        <!-- Header Toolbar -->
        <div class="designer-header p-2 bg-body-subtle border-bottom">
            <div class="hstack gap-1">
                <!-- Actions -->
                <button type="button" class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="saveComponent" :disabled="saving || !hasUnsavedChanges">
                    <i class="fa-solid" :class="saving ? 'fa-spinner fa-spin' : 'fa-save'"></i> <span>Save</span>
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
            <div v-if="toolboxPanelVisible" class="toolbox-panel bg-white border-end" style="min-width:110px;width:10%;">
                <div class="toolbox-body overflow-auto">
                    <div v-for="group in toolboxGroups" :key="group.key">
                        <div v-if="group.alwaysShow || (typeof group.show === 'function' ? group.show.call($data) : group.show)" class="component-group border-bottom">
                            <div class="group-title small fw-bold text-dark p-2 cursor-pointer bg-body-tertiary d-flex justify-content-between align-items-center user-select-none" @click="group.expanded = !group.expanded">
                                <span><i :class="group.icon + ' me-1'"></i>{{ group.title }}</span>
                                <i class="fa-solid fa-xs" :class="group.expanded ? 'fa-chevron-up' : 'fa-chevron-down'"></i>
                            </div>
                            <div v-show="group.expanded" class="component-grid p-2">
                                <div v-for="comp in group.items" :key="comp.type"
                                     class="component-item"
                                     :class="{ 'disabled-item': !group.alwaysShow && isCanvasEmpty }"
                                     :draggable="group.alwaysShow || !isCanvasEmpty"
                                     @dragstart="onDragStart(comp, $event)">
                                    <i :class="comp.icon + ' fa-lg'"></i>
                                    <div class="item-label mt-2">{{ comp.label }}</div>
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
                <div class="canvas-container flex-grow-1 bg-body-secondary position-relative"
                     ref="canvasContainer"
                     @drop="onDrop"
                     @dragover="onDragOver"
                     @click="onCanvasClick">
                    <div id="designCanvas"
                         class="design-canvas bg-white shadow-sm"
                         :class="{ 'canvas-empty': isCanvasEmpty }">
                         <!-- Content will be dynamically loaded -->
                    </div>

                    <!-- Smart Tag Overlay -->
                    <div v-if="smartTagVisible" class="smart-tag-overlay d-flex justify-content-between" :style="smartTagStyle">
                        <div class="btn-group btn-group-sm shadow bg-white rounded-0">
                            <!-- Add Previous (Left/Above) -->
                            <button v-if="smartTagType === 'col'" class="btn btn-outline-success btn-xs py-1 px-2" @click.stop="addColumn('left')" title="Add Column Left"><i class="fa-solid fa-plus"></i></button>
                            <button v-if="smartTagType === 'row'" class="btn btn-outline-success btn-xs py-1 px-2" @click.stop="addRow('above')" title="Add Row Above"><i class="fa-solid fa-plus"></i></button>
                            
                            <!-- Move Previous (Left/Up) -->
                            <button v-if="canMoveActiveElement" class="btn btn-outline-secondary btn-xs py-1 px-2" @click.stop="moveElement('prev')" :title="smartTagType === 'col' ? 'Move Left' : 'Move Up'">
                                <i class="fa-solid" :class="smartTagType === 'col' ? 'fa-arrow-left' : 'fa-arrow-up'"></i>
                            </button>
                            
                            <!-- Move Next (Right/Down) -->
                            <button v-if="canMoveActiveElement" class="btn btn-outline-secondary btn-xs py-1 px-2" @click.stop="moveElement('next')" :title="smartTagType === 'col' ? 'Move Right' : 'Move Down'">
                                <i class="fa-solid" :class="smartTagType === 'col' ? 'fa-arrow-right' : 'fa-arrow-down'"></i>
                            </button>

                            <!-- Add Next (Right/Below) -->
                            <button v-if="smartTagType === 'col'" class="btn btn-outline-success btn-xs py-1 px-2" @click.stop="addColumn('right')" title="Add Column Right"><i class="fa-solid fa-plus"></i></button>
                            <button v-if="smartTagType === 'row'" class="btn btn-outline-success btn-xs py-1 px-2" @click.stop="addRow('below')" title="Add Row Below"><i class="fa-solid fa-plus"></i></button>
                            
                            <!-- Heading Level Controls -->
                            <button v-if="smartTagType === 'heading'" class="btn btn-outline-secondary btn-xs py-1 px-2" @click.stop="changeHeadingLevel(1)" title="Decrease Size (H+)"><i class="fa-solid fa-minus"></i></button>
                            <button v-if="smartTagType === 'heading'" class="btn btn-outline-secondary btn-xs py-1 px-2" @click.stop="changeHeadingLevel(-1)" title="Increase Size (H-)"><i class="fa-solid fa-plus"></i></button>
                            
                            <!-- Component Loader Edit Button -->
                            <button v-if="smartTagType === 'component-loader'" class="btn btn-outline-primary btn-xs py-1 px-2" @click.stop="editComponentLoader" title="Edit Component">
                                <i class="fa-solid fa-pen-to-square"></i> <span>Edit</span>
                            </button>
                        </div>

                        <div class="btn-group btn-group-sm shadow bg-white rounded-0">
                            <!-- Delete -->
                            <button class="btn btn-outline-danger btn-xs py-1 px-2" @click.stop="deleteSelectedElement" title="Delete"><i class="fa-solid fa-trash"></i></button>
                        </div>
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
                            <button v-if="selectedElement.classes" class="btn btn-outline-secondary btn-sm" @click="updateElementClasses('')" type="button" title="Clear">
                                <i class="fa-solid fa-times"></i>
                            </button>
                        </div>
                    </div>
                    <div>
                        <div class="input-group input-group-sm">
                            <span class="input-group-text bg-light text-secondary" style="width: 60px;">Style</span>
                            <input type="text" class="form-control" 
                                   :value="selectedElement.style" 
                                   @change="updateElementStyle($event.target.value)"
                                   placeholder="Inline styles...">
                            <button v-if="selectedElement.style" class="btn btn-outline-secondary btn-sm" @click="updateElementStyle('')" type="button" title="Clear">
                                <i class="fa-solid fa-times"></i>
                            </button>
                        </div>
                    </div>
                    
                    <!-- Component Loader Src -->
                    <div v-if="selectedElement.tagName === 'component-loader'" class="mt-2">
                        <div class="input-group input-group-sm">
                            <span class="input-group-text bg-light text-secondary" style="width: 60px;">Src</span>
                            <input type="text" class="form-control" 
                                   :value="selectedElement.src" 
                                   @change="updateElementAttribute('src', $event.target.value)"
                                   placeholder="Component path...">
                            <button v-if="selectedElement.src" class="btn btn-outline-secondary btn-sm" @click="updateElementAttribute('src', '')" type="button" title="Clear">
                                <i class="fa-solid fa-times"></i>
                            </button>
                            <button class="btn btn-outline-primary btn-sm" @click="editComponentLoader" type="button" title="Edit Component">
                                <i class="fa-solid fa-pen-to-square"></i>
                            </button>
                        </div>
                    </div>
                    
                    <!-- Image Src -->
                    <div v-if="selectedElement.tagName === 'img'" class="mt-2">
                        <div class="input-group input-group-sm">
                            <span class="input-group-text bg-light text-secondary" style="width: 60px;">Src</span>
                            <input type="text" class="form-control" 
                                   :value="selectedElement.src" 
                                   @change="updateElementAttribute('src', $event.target.value)"
                                   placeholder="Image URL...">
                            <button v-if="selectedElement.src" class="btn btn-outline-secondary btn-sm" @click="updateElementAttribute('src', '')" type="button" title="Clear">
                                <i class="fa-solid fa-times"></i>
                            </button>
                        </div>
                    </div>
                    
                    <!-- Link Href -->
                    <div v-if="selectedElement.tagName === 'a'" class="mt-2">
                        <div class="input-group input-group-sm">
                            <span class="input-group-text bg-light text-secondary" style="width: 60px;">Href</span>
                            <input type="text" class="form-control" 
                                   :value="selectedElement.href" 
                                   @change="updateElementAttribute('href', $event.target.value)"
                                   placeholder="URL...">
                            <button v-if="selectedElement.href" class="btn btn-outline-secondary btn-sm" @click="updateElementAttribute('href', '')" type="button" title="Clear">
                                <i class="fa-solid fa-times"></i>
                            </button>
                        </div>
                    </div>
                </div>

                <!-- Breadcrumb Path -->
                <div v-if="selectionPath.length > 0" class="px-2 py-1 bg-white border-top small flex-shrink-0 d-flex justify-content-between align-items-center">
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
                    <button type="button" class="btn btn-sm btn-link text-danger p-0 ms-2" @click="deleteSelectedElement" title="Delete Selected Element (Del)">
                        <i class="fa-solid fa-trash"></i>
                    </button>
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
                hasUnsavedChanges: false,

                // Ace Editor instance
                aceVueEditor: null,

                componentCode: "",
                originalComponentCode: "",

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
                isProgrammaticCursorMove: false,
                syncDebounceTimer: null,
                codeSyncDebounceTimer: null,
                codeSelectionTimer: null,

                // Smart Tag State
                smartTagVisible: false,
                smartTagType: null, // 'row' or 'col'
                smartTagStyle: {},
                activeSmartElement: null,

                // Toolbox groups definition for dynamic rendering (merged items)
                toolboxGroups: [
                    {
                        key: 'rootElements',
                        title: 'Root Elements',
                        icon: 'fa-solid fa-layer-group',
                        show: true,
                        alwaysShow: true,
                        expanded: true,
                        items: [
                            {
                                type: 'container', label: 'C-Fixed', icon: 'fa-solid fa-arrows-left-right-to-line',
                                template: '<div class="container p-3"><div class="row"><div class="col"><span>Col 1</span></div><div class="col"><span>Col 2</span></div></div></div>'
                            },
                            {
                                type: 'container-fluid', label: 'C-Fluid', icon: 'fa-solid fa-arrows-left-right',
                                template: '<div class="container-fluid p-3"><div class="row"><div class="col"><span>Col 1</span></div><div class="col"><span>Col 2</span></div></div></div>'
                            },
                            {
                                type: 'div', label: 'Div', icon: 'fa-solid fa-square',
                                template: '<div class="p-3">Div Container</div>'
                            },
                            {
                                type: 'card', label: 'Card', icon: 'fa-solid fa-id-card',
                                template: '<div class="card"><div class="card-header">Header</div><div class="card-body"><h5 class="card-title">Title</h5><p class="card-text">Content</p></div><div class="card-footer">Footer</div></div>'
                            }
                        ]
                    },
                    {
                        key: 'htmlComponents',
                        title: 'Simple',
                        icon: 'fa-solid fa-code',
                        show: function() { return !this.isCanvasEmpty; },
                        alwaysShow: false,
                        expanded: false,
                        items: [
                            { type: 'h1', label: 'Hx', icon: 'fa-solid fa-heading', template: '<h1>Heading</h1>' },
                            { type: 'p', label: 'Paragraph', icon: 'fa-solid fa-paragraph', template: '<p>Paragraph text</p>' },
                            { type: 'span', label: 'Span', icon: 'fa-solid fa-text-width', template: '<span>Text</span>' },
                            { type: 'hr', label: 'Line', icon: 'fa-solid fa-minus', template: '<hr />' },
                            { type: 'a', label: 'Link', icon: 'fa-solid fa-link', template: '<a href="#">Link</a>' },
                            { type: 'img', label: 'Image', icon: 'fa-solid fa-image', template: '<img src="/a..lib/images/AppEnd-Logo-Only.png" alt="...">' }
                        ]
                    },
                    {
                        key: 'bootstrapComponents',
                        title: 'Bootstrap',
                        icon: 'fa-brands fa-bootstrap',
                        show: function() { return !this.isCanvasEmpty; },
                        alwaysShow: false,
                        expanded: false,
                        items: [
                            {
                                type: 'button', label: 'Button', icon: 'fa-solid fa-hand-pointer',
                                template: '<button type="button" class="btn btn-primary">Button</button>'
                            },
                            {
                                type: 'alert', label: 'Alert', icon: 'fa-solid fa-triangle-exclamation',
                                template: '<div class="alert alert-info" role="alert">This is an alert</div>'
                            },
                            {
                                type: 'badge', label: 'Badge', icon: 'fa-solid fa-certificate',
                                template: '<span class="badge bg-secondary">New</span>'
                            },
                            {
                                type: 'btn-group', label: 'Btn Group', icon: 'fa-solid fa-layer-group',
                                template: '<div class="btn-group" role="group"><button type="button" class="btn btn-primary">Left</button><button type="button" class="btn btn-primary">Middle</button><button type="button" class="btn btn-primary">Right</button></div>'
                            },
                            {
                                type: 'list-group', label: 'List Group', icon: 'fa-solid fa-list-ul',
                                template: '<ul class="list-group"><li class="list-group-item">An item</li><li class="list-group-item">A second item</li><li class="list-group-item">A third item</li></ul>'
                            },
                            {
                                type: 'breadcrumb', label: 'Breadcrumb', icon: 'fa-solid fa-slash',
                                template: '<nav aria-label="breadcrumb"><ol class="breadcrumb"><li class="breadcrumb-item"><a href="#">Home</a></li><li class="breadcrumb-item active" aria-current="page">Library</li></ol></nav>'
                            },
                            {
                                type: 'progress', label: 'Progress', icon: 'fa-solid fa-bars-progress',
                                template: '<div class="progress"><div class="progress-bar" role="progressbar" style="width: 25%" aria-valuenow="25" aria-valuemin="0" aria-valuemax="100"></div></div>'
                            },
                            {
                                type: 'spinner', label: 'Spinner', icon: 'fa-solid fa-spinner',
                                template: '<div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div>'
                            },
                            {
                                type: 'table', label: 'Table', icon: 'fa-solid fa-table',
                                template: '<table class="table"><thead><tr><th scope="col">#</th><th scope="col">First</th><th scope="col">Last</th><th scope="col">Handle</th></tr></thead><tbody><tr><th scope="row">1</th><td>Mark</td><td>Otto</td><td>@mdo</td></tr><tr><th scope="row">2</th><td>Jacob</td><td>Thornton</td><td>@fat</td></tr></tbody></table>'
                            }
                        ]
                    },
                    {
                        key: 'formComponents',
                        title: 'Forms',
                        icon: 'fa-brands fa-wpforms',
                        show: function() { return !this.isCanvasEmpty; },
                        alwaysShow: false,
                        expanded: false,
                        items: [
                            {
                                type: 'input', label: 'Input', icon: 'fa-solid fa-keyboard',
                                template: '<div class="mb-3"><label class="form-label">Email address</label><input type="email" class="form-control" placeholder="name@example.com"></div>'
                            },
                            {
                                type: 'textarea', label: 'Textarea', icon: 'fa-solid fa-align-left',
                                template: '<div class="mb-3"><label class="form-label">Example textarea</label><textarea class="form-control" rows="3"></textarea></div>'
                            },
                            {
                                type: 'select', label: 'Select', icon: 'fa-solid fa-list',
                                template: '<select class="form-select" aria-label="Default select example"><option selected>Open this select menu</option><option value="1">One</option><option value="2">Two</option><option value="3">Three</option></select>'
                            },
                            {
                                type: 'checkbox', label: 'Checkbox', icon: 'fa-solid fa-check-square',
                                template: '<div class="form-check"><input class="form-check-input" type="checkbox" value="" id="flexCheckDefault"><label class="form-check-label" for="flexCheckDefault">Default checkbox</label></div>'
                            },
                            {
                                type: 'radio', label: 'Radio', icon: 'fa-solid fa-dot-circle',
                                template: '<div class="form-check"><input class="form-check-input" type="radio" name="flexRadioDefault" id="flexRadioDefault1"><label class="form-check-label" for="flexRadioDefault1">Default radio</label></div>'
                            },
                            {
                                type: 'switch', label: 'Switch', icon: 'fa-solid fa-toggle-on',
                                template: '<div class="form-check form-switch"><input class="form-check-input" type="checkbox" role="switch" id="flexSwitchCheckDefault"><label class="form-check-label" for="flexSwitchCheckDefault">Default switch checkbox input</label></div>'
                            },
                            {
                                type: 'range', label: 'Range', icon: 'fa-solid fa-sliders',
                                template: '<label for="customRange1" class="form-label">Example range</label><input type="range" class="form-range" id="customRange1">'
                            },
                            {
                                type: 'file', label: 'File Input', icon: 'fa-solid fa-file-arrow-up',
                                template: '<div class="mb-3"><label for="formFile" class="form-label">Default file input example</label><input class="form-control" type="file" id="formFile"></div>'
                            },
                            {
                                type: 'input-group', label: 'Input Group', icon: 'fa-solid fa-layer-group',
                                template: '<div class="input-group mb-3"><span class="input-group-text" id="basic-addon1">@</span><input type="text" class="form-control" placeholder="Username" aria-label="Username" aria-describedby="basic-addon1"></div>'
                            },
                            {
                                type: 'floating', label: 'Floating', icon: 'fa-solid fa-font',
                                template: '<div class="form-floating mb-3"><input type="email" class="form-control" id="floatingInput" placeholder="name@example.com"><label for="floatingInput">Email address</label></div>'
                            }
                        ]
                    }, 
                    {
                        key: 'advancedComponents',
                        title: 'Advanced',
                        icon: 'fa-solid fa-rectangle-list',
                        show: function() { return !this.isCanvasEmpty; },
                        alwaysShow: false,
                        expanded: false,
                        items: [
                            {
                                type: 'component-loader', label: 'C-Loader', icon: 'fa-solid fa-cubes',
                                template: '<component-loader src="/a.CustomComponents/C1.vue"></component-loader>'
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

        computed: {
            canMoveActiveElement() {
                if (!this.activeSmartElement) return false;
                const el = this.activeSmartElement;
                
                // component-loader elements cannot be moved
                if (el.tagName.toLowerCase() === 'component-loader') return false;
                
                return !el.classList.contains('card-header') && 
                       !el.classList.contains('card-body') && 
                       !el.classList.contains('card-footer');
            }
        },

        methods: {
            readFileContent() {
                if (!this.filePath) return;
                
                // Warn about unsaved changes before reloading
                if (this.hasUnsavedChanges) {
                    shared.showConfirm({
                        title: shared.translate("Reload"),
                        message1: shared.translate("You have unsaved changes. Are you sure you want to reload?"),
                        message2: shared.translate("All unsaved changes will be lost."),
                        okText: "Reload",
                        okClass: "btn btn-sm btn-danger w-100 py-2",
                        cancelText: "Cancel",
                        callback: () => {
                            this.performReload();
                        }
                    });
                } else {
                    this.performReload();
                }
            },
            
            performReload() {
                // filePath already includes the relative path like '/a.CustomComponents/Sample.vue'
                // Remove leading slash if exists
                const cleanPath = this.filePath.startsWith('/') ? this.filePath.substring(1) : this.filePath;
                const fullPath = cleanPath;
                this.loading = true;
                
                rpcAEP("GetFileContent", { "PathToRead": fullPath }, (res) => {
                    this.componentCode = R0R(res);
                    this.originalComponentCode = this.componentCode;
                    this.hasUnsavedChanges = false;
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
                            
                            // Mark as unsaved
                            this.markAsUnsaved();
                            
                            // Update Ace editor without triggering change event
                            if (this.aceVueEditor) {
                                const cursorPos = this.aceVueEditor.getCursorPosition();
                                const scrollTop = this.aceVueEditor.session.getScrollTop();
                                
                                // Temporarily remove change listener
                                this.aceVueEditor.session.off('change', this.onCodeEditorChange);
                                
                                this.isProgrammaticCursorMove = true;
                                try {
                                    this.aceVueEditor.setValue(this.componentCode, -1);
                                    
                                    // Restore cursor and scroll position
                                    this.aceVueEditor.moveCursorToPosition(cursorPos);
                                    this.aceVueEditor.session.setScrollTop(scrollTop);
                                } finally {
                                    this.isProgrammaticCursorMove = false;
                                }
                                
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
                this.markAsUnsaved();
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
                    fontSize: 13
                });

                // Bind change event with our sync method
                this.aceVueEditor.session.on('change', this.onCodeEditorChange);
                
                // Bind cursor change for auto-selection
                this.aceVueEditor.selection.on('changeCursor', this.onCodeCursorChange);
            },

            onCodeCursorChange() {
                if (this.isProgrammaticCursorMove) return;
                if (this.codeSelectionTimer) clearTimeout(this.codeSelectionTimer);
                this.codeSelectionTimer = setTimeout(() => {
                    const did = this.findDidBeforeCursor();
                    if (did) {
                        const canvas = document.getElementById('designCanvas');
                        if (canvas) {
                          const el = canvas.querySelector(`[data-did="${did}"]`);
                          if (el && el !== this.selectedDomElement) {
                            this.selectElement(el, true);
                            el.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
                          }
                        }
                    }
                }, 1000);
            },

            findDidBeforeCursor() {
                if (!this.aceVueEditor) return null;
                const cursor = this.aceVueEditor.getCursorPosition();
                const row = cursor.row;
                const col = cursor.column;
                
                // Get current line
                const currentLine = this.aceVueEditor.session.getLine(row);
                
                // Find if cursor is inside an opening tag on current line
                // Look for < before cursor and > after cursor
                let tagStart = -1;
                let tagEnd = -1;
                
                // Search backwards for <
                for (let i = col; i >= 0; i--) {
                    if (currentLine[i] === '<') {
                        tagStart = i;
                        break;
                    }
                    // If we hit > before <, cursor is not in a tag
                    if (currentLine[i] === '>') {
                        break;
                    }
                }
                
                // Search forwards for >
                if (tagStart !== -1) {
                    for (let i = col; i < currentLine.length; i++) {
                        if (currentLine[i] === '>') {
                            tagEnd = i;
                            break;
                        }
                    }
                }
                
                // If cursor is inside a tag, extract data-did from that tag
                if (tagStart !== -1 && tagEnd !== -1) {
                    const tagContent = currentLine.substring(tagStart, tagEnd + 1);
                    const didMatch = /data-did="([^"]+)"/.exec(tagContent);
                    if (didMatch) {
                        return didMatch[1];
                    }
                }
                
                // If not in a tag or tag doesn't have data-did, search backwards through all text
                let textBeforeCursor = '';
                for (let r = 0; r <= row; r++) {
                    const line = this.aceVueEditor.session.getLine(r);
                    if (r < row) {
                        textBeforeCursor += line + '\n';
                    } else {
                        textBeforeCursor += line.substring(0, col);
                    }
                }
                
                // Find all data-did attributes before cursor
                const regex = /data-did="([^"]+)"/g;
                let matches = [];
                let match;
                while ((match = regex.exec(textBeforeCursor)) !== null) {
                    matches.push({
                        did: match[1],
                        index: match.index
                    });
                }
                
                if (matches.length === 0) return null;
                
                // Get the last (closest) data-did before cursor
                const closestMatch = matches[matches.length - 1];
                
                // Now check if there's a nested element after cursor that closes first
                let textAfterCursor = '';
                const totalLines = this.aceVueEditor.session.getLength();
                for (let r = row; r < totalLines && r < row + 50; r++) {
                    const line = this.aceVueEditor.session.getLine(r);
                    if (r === row) {
                        textAfterCursor += line.substring(col) + '\n';
                    } else {
                        textAfterCursor += line + '\n';
                    }
                }
                
                // Look for nested data-did in the content after cursor
                const nestedDidMatch = /data-did="([^"]+)"/.exec(textAfterCursor);
                if (nestedDidMatch) {
                    const nestedDid = nestedDidMatch[1];
                    
                    // Extract tag name of the parent element
                    const tagMatch = textBeforeCursor.match(new RegExp(`<([a-zA-Z0-9-]+)[^>]*data-did="${closestMatch.did}"[^>]*>`, 'i'));
                    if (tagMatch) {
                        const tagName = tagMatch[1];
                        const closingTagPattern = new RegExp(`</${tagName}>`);
                        
                        // Find closing tags
                        const nestedTagMatch = textAfterCursor.match(new RegExp(`<([a-zA-Z0-9-]+)[^>]*data-did="${nestedDid}"`));
                        if (nestedTagMatch) {
                            const nestedTagName = nestedTagMatch[1];
                            const nestedClosingPattern = new RegExp(`</${nestedTagName}>`);
                            const nestedClosingMatch = textAfterCursor.match(nestedClosingPattern);
                            const parentClosingMatch = textAfterCursor.match(closingTagPattern);
                            
                            if (nestedClosingMatch && parentClosingMatch) {
                                const nestedClosingPos = nestedClosingMatch.index;
                                const parentClosingPos = parentClosingMatch.index;
                                
                                // If nested closes before parent, use nested DID
                                if (nestedClosingPos < parentClosingPos) {
                                    return nestedDid;
                                }
                            }
                        }
                    }
                }
                
                return closestMatch.did;
            },

            onDragStart(component, event) {
                // Check if canvas is empty
                const canvas = document.getElementById('designCanvas');
                const isEmpty = !canvas || canvas.innerHTML.trim() === '' || this.isCanvasEmpty;
                
                // If canvas is empty, only allow root elements
                if (isEmpty && !['div', 'container', 'container-fluid', 'card', 'p'].includes(component.type)) {
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
                    
                    let newElement = null;
                    if (isEmpty) {
                        canvas.innerHTML = this.draggedComponent.template;
                        this.isCanvasEmpty = false;
                        newElement = canvas.firstElementChild;
                    } else {
                        dropTarget.insertAdjacentHTML('beforeend', this.draggedComponent.template);
                        newElement = dropTarget.lastElementChild;
                    }
                    
                    this.draggedComponent = null;
                    this.saveState();
                    this.attachElementHandlers();
                    this.syncCanvasToCode();
                    
                    if (newElement) {
                        setTimeout(() => {
                            this.selectElement(newElement);
                        }, 50);
                      }
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
                    
                    const targetParent = e.shiftKey ? dropTarget : dropTarget.parentNode;
                    if (!this.isValidDrop(draggedType, targetParent)) {
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
                    el.draggable = true;
                    el.ondragstart = (e) => {
                        e.stopPropagation();
                        e.dataTransfer.effectAllowed = 'move';
                        
                        let targetEl = el;

                        // 1. Card Header/Body -> Card
                        if (el.classList.contains('card-header') || el.classList.contains('card-body')) {
                            const card = el.closest('.card');
                            if (card) targetEl = card;
                        }
                        // 2. Label -> Wrapper
                        else if (el.tagName.toLowerCase() === 'label') {
                            const parent = el.parentElement;
                            if (parent && parent.id !== 'designCanvas') {
                                // Check if parent is a layout container that we should NOT drag
                                const isLayout = parent.classList.contains('row') || 
                                               parent.classList.contains('container') || 
                                               parent.classList.contains('container-fluid') || 
                                               parent.classList.contains('card-body') || 
                                               parent.classList.contains('card-header') || 
                                               parent.classList.contains('card-footer') ||
                                               parent.classList.contains('col');
                                  
                                if (!isLayout) {
                                    targetEl = parent;
                                }
                            }
                        }

                        this.draggedElement = targetEl;
                        this.draggedComponent = null;
                        targetEl.classList.add('dragging');
                    };
                    el.ondragend = (e) => {
                        e.stopPropagation();
                        if (this.draggedElement) {
                            this.draggedElement.classList.remove('dragging');
                        } else {
                            el.classList.remove('dragging');
                        }
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
                            
                            let targetParent = e.shiftKey ? el : el.parentNode;

                            // Auto-detect inside mode if sibling drop is invalid but inside drop is valid
                            if (!e.shiftKey && !this.isValidDrop(draggedType, targetParent)) {
                                if (this.isValidDrop(draggedType, el)) {
                                    targetParent = el;
                                }
                            }

                            if (this.draggedElement !== el && !this.isDescendant(el, this.draggedElement) && this.isValidDrop(draggedType, targetParent)) {
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
							
                            let targetParent = e.shiftKey ? el : el.parentNode;
                            let isInside = e.shiftKey;

                            // Auto-detect inside mode if sibling drop is invalid but inside drop is valid
                            if (!isInside && !this.isValidDrop(draggedType, targetParent)) {
                                if (this.isValidDrop(draggedType, el)) {
                                    targetParent = el;
                                    isInside = true;
                                }
                            }

                            if (!this.isValidDrop(draggedType, targetParent)) {
                                showError('This element cannot be moved here!');
                                this.draggedElement = null;
                                return;
                            }

                            if (isInside) {
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

            selectElement(domElement, fromCode = false) {
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
                    html: domElement.innerHTML,
                    src: domElement.getAttribute('src') || '',
                    href: domElement.getAttribute('href') || ''
                };
                this.updateSelectionPath(domElement);
                
                // Show smart tag for selected element if applicable
                this.showSmartTag(domElement);

                // Highlight in code editor
                if (!fromCode) {
                    this.highlightCodeForElement(domElement);
                }
            },

            highlightCodeForElement(domElement) {
                if (!this.aceVueEditor || !domElement) return;
                
                const did = domElement.getAttribute('data-did');
                if (!did) return;
                
                this.isProgrammaticCursorMove = true;
                try {
                    this.aceVueEditor.find(`data-did="${did}"`, {
                        backwards: false,
                        wrap: true,
                        caseSensitive: true,
                        wholeWord: false,
                        regExp: false,
                        preventScroll: false
                    });
                    
                    if (!this.aceVueEditor.selection.isEmpty()) {
                        const range = this.aceVueEditor.getSelectionRange();
                        this.aceVueEditor.selection.clearSelection();
                        this.aceVueEditor.moveCursorTo(range.start.row, range.start.column);
                        this.aceVueEditor.scrollToLine(range.start.row, true, true, function() {});
                    }
                } finally {
                    this.isProgrammaticCursorMove = false;
                }
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

            updateElementAttribute(attr, value) {
                if (!this.selectedDomElement) return;
                if (value) {
                    this.selectedDomElement.setAttribute(attr, value);
                } else {
                    this.selectedDomElement.removeAttribute(attr);
                }
                
                if (attr === 'src') this.selectedElement.src = value;
                if (attr === 'href') this.selectedElement.href = value;
                
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
                
                // Hide smart tag when deselecting
                this.hideSmartTag();
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
                    
                    // Deselect any selected element
                    this.deselectElement();
            
                    // Sync canvas to code editor
                    this.syncCanvasToCode();
            
                    // Update button states
                    this.canUndo = this.historyIndex > 0;
                    this.canRedo = true;
                }
            },

            redoAction() {
                if (this.historyIndex < this.history.length - 1) {
                    this.historyIndex++;
                    const canvas = document.getElementById('designCanvas');
                    canvas.innerHTML = this.history[this.historyIndex];
                    
                    // Update isCanvasEmpty state
                    this.isCanvasEmpty = canvas.innerHTML.trim() === '';
                    
                    // Deselect any selected element
                    this.deselectElement();
            
                    // Sync canvas to code editor
                    this.syncCanvasToCode();
            
                    // Update button states
                    this.canRedo = this.historyIndex < this.history.length - 1;
                    this.canUndo = true;
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
                        this.originalComponentCode = this.componentCode;
                        this.hasUnsavedChanges = false;
                        showSuccess("Component saved successfully!");
                    });
                } else {
                    setTimeout(() => {
                        this.saving = false;
                        this.originalComponentCode = this.componentCode;
                        this.hasUnsavedChanges = false;
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
                        
                        // Store as original and reset unsaved flag
                        this.originalComponentCode = this.componentCode;
                        this.hasUnsavedChanges = false;
                    } else {
                        // No component code, ensure canvas is empty
                        this.isCanvasEmpty = true;
                        if (canvas) {
                            canvas.innerHTML = '';
                        }
                        this.hasUnsavedChanges = false;
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

            deleteSelectedElement() {
                const el = this.selectedDomElement || this.activeSmartElement;
                if (!el) return;
                
                // Hide smart tag immediately
                this.hideSmartTag();
                
                // Remove the element
                el.remove();
                
                // Update empty state
                const canvas = document.getElementById('designCanvas');
                this.isCanvasEmpty = !canvas || canvas.innerHTML.trim() === '';
                
                // Clear selection
                this.deselectElement();
                
                // Save state for undo
                this.saveState();
                
                // Sync to code
                this.syncCanvasToCode();
            },

            handleKeyDown(e) {
                // Only handle Delete key
                if (e.key !== 'Delete') return;
                
                // Check if an element is selected
                if (!this.selectedDomElement) return;
                
                // Check if focus is in an input or textarea or contenteditable
                const activeEl = document.activeElement;
                const tagName = activeEl.tagName.toLowerCase();
                
                if (tagName === 'input' || tagName === 'textarea' || activeEl.isContentEditable) {
                    return;
                }
                
                e.preventDefault();
                this.deleteSelectedElement();
            },

            showSmartTag(el) {
                this.activeSmartElement = el;
                
                if (el.classList.contains('row')) this.smartTagType = 'row';
                else if (el.classList.contains('col')) this.smartTagType = 'col';
                else if (/^H[1-6]$/.test(el.tagName)) this.smartTagType = 'heading';
                else if (el.tagName.toLowerCase() === 'component-loader') this.smartTagType = 'component-loader';
                else this.smartTagType = null;
                
                const container = this.$refs.canvasContainer;
                if (!container) return;

                const elRect = el.getBoundingClientRect();
                const containerRect = container.getBoundingClientRect();
                
                // Position at top-left of the element, slightly above (overlapping by 3px)
                const top = elRect.top - containerRect.top + container.scrollTop - 25; 
                const left = elRect.left - containerRect.left + container.scrollLeft;
                const width = elRect.width;
                
                this.smartTagStyle = {
                    top: `${Math.max(0, top)}px`,
                    left: `${left}px`,
                    width: `${width}px`
                };
                
                this.smartTagVisible = true;
            },

            hideSmartTag() {
                // Only hide if explicitly called
                this.smartTagVisible = false;
                this.activeSmartElement = null;
            },
            
            addColumn(direction) {
                if (!this.activeSmartElement) return;
                const template = '<div class="col p-2"><span>Column</span></div>';
                if (direction === 'left') {
                    this.activeSmartElement.insertAdjacentHTML('beforebegin', template);
                } else {
                    this.activeSmartElement.insertAdjacentHTML('afterend', template);
                }
                this.saveState();
                this.attachElementHandlers();
                this.syncCanvasToCode();
            },
            
            addRow(direction) {
                if (!this.activeSmartElement) return;
                const template = '<div class="row"><div class="col"><span>Column 1</span></div><div class="col"><span>Column 2</span></div></div>';
                if (direction === 'above') {
                    this.activeSmartElement.insertAdjacentHTML('beforebegin', template);
                } else {
                    this.activeSmartElement.insertAdjacentHTML('afterend', template);
                }
                this.saveState();
                this.attachElementHandlers();
                this.syncCanvasToCode();
            },

            moveElement(direction) {
                if (!this.activeSmartElement) return;
                const el = this.activeSmartElement;
                const parent = el.parentNode;
                if (!parent) return;
                
                if (direction === 'prev') {
                    const prev = el.previousElementSibling;
                    if (prev) {
                        parent.insertBefore(el, prev);
                    }
                } else if (direction === 'next') {
                    const next = el.nextElementSibling;
                    if (next) {
                        parent.insertBefore(next, el);
                    }
                }
                
                this.saveState();
                this.syncCanvasToCode();
                
                // Re-calculate position after move
                this.$nextTick(() => {
                    this.showSmartTag(el);
                });
            },

            changeHeadingLevel(change) {
                if (!this.activeSmartElement) return;
                const el = this.activeSmartElement;
                const currentTag = el.tagName;
                if (!/^H[1-6]$/.test(currentTag)) return;
                
                const currentLevel = parseInt(currentTag.substring(1));
                let newLevel = currentLevel + change;
                if (newLevel < 1) newLevel = 1;
                if (newLevel > 6) newLevel = 6;
                
                if (newLevel === currentLevel) return;
                
                const newTag = 'H' + newLevel;
                const newEl = document.createElement(newTag);
                
                // Copy attributes
                Array.from(el.attributes).forEach(attr => {
                    newEl.setAttribute(attr.name, attr.value);
                });
                
                // Copy content
                newEl.innerHTML = el.innerHTML;
                
                // Replace
                el.parentNode.replaceChild(newEl, el);
                
                this.saveState();
                this.attachElementHandlers();
                this.syncCanvasToCode();
                
                // Re-select
                this.selectElement(newEl);
            },    

            editComponentLoader() {
                if (!this.activeSmartElement) return;
                const el = this.activeSmartElement;

                // Get the src attribute
                const src = el.getAttribute('src');
                if (!src) {
                    showError('Component src not found!');
                    return;
                }

                let componentPath = !src.startsWith('/') ? '/' + src : src;
                const designerUrl = `?c=components/ControlDesigner&edt=/workspace/client/${encodeURIComponent(componentPath)}`;
                window.open(designerUrl, '_blank');
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

                // Strict rule: If parent is row, ONLY col is allowed
                if (parentElement.classList.contains('row')) {
                    return childType === 'col';
                }
                
                // Strict rule: Paragraph (p) cannot have any children
                if (parentElement.tagName && parentElement.tagName.toLowerCase() === 'p') {
                    return false;
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
            },

            markAsUnsaved() {
                if (!this.hasUnsavedChanges) {
                    this.hasUnsavedChanges = true;
                }
            },

            handleBeforeUnload(e) {
                if (this.hasUnsavedChanges) {
                    const message = 'You have unsaved changes. Are you sure you want to leave?';
                    e.preventDefault();
                    e.returnValue = message;
                    return message;
                }
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
            window.addEventListener('keydown', this.handleKeyDown);
            window.addEventListener('beforeunload', this.handleBeforeUnload);
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
                        this.hasUnsavedChanges = false;
                        this.attachElementHandlers();
                    }
                }, 100);
            });
        },
        beforeUnmount() {
            window.removeEventListener('keydown', this.handleKeyDown);
            window.removeEventListener('beforeunload', this.handleBeforeUnload);
            // Cleanup Ace editor
            if (this.aceVueEditor) {
                this.aceVueEditor.session.off('change', this.onCodeEditorChange);
                this.aceVueEditor.destroy();
            }
            
            // Clear timers
            if (this.syncDebounceTimer) clearTimeout(this.syncDebounceTimer);
            if (this.codeSyncDebounceTimer) clearTimeout(this.codeSyncDebounceTimer);
            if (this.codeSelectionTimer) clearTimeout(this.codeSelectionTimer);
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
        min-height: 0;
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
        min-width: 110px;
        width: 10%;
        display: flex;
        flex-direction: column;
    }

    .toolbox-body {
        flex: 1 1 auto;
        overflow-y: auto;
        min-height: 0;
    }

    .component-grid {
        display: flex;
        flex-wrap: wrap;
        gap: 4px;
        padding: 4px;
    }

    .component-item {
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        padding-top: 9px;
        background: #f8f9fa;
        border: 1px solid #dee2e6;
        border-radius: 4px;
        cursor: move;
        transition: all 0.2s;
        width: 60px;
        height: 50px;
        box-sizing: border-box;
        font-size:12px;
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
        font-size: 0.65rem;
        margin-top: 4px;
        text-align: center;
        width: 100%;
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
    }

    /* Canvas */
    .canvas-container {
        overflow: auto;
        padding: 8px;
        min-height: 0;
    }

    .smart-tag-overlay {
        position: absolute;
        z-index: 1050;
        pointer-events: none;
    }

    .smart-tag-overlay > * {
        pointer-events: auto;
    }

    .btn-xs {
        font-size: 0.65rem !important;
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

    /* Component Loader Placeholder Styling */
    :deep(component-loader) {
        display: block;
        padding: 12px 16px;
        background: #f8f9fa;
        border: 2px dashed #dee2e6;
        border-radius: 6px;
        color: #6c757d;
        font-family: 'Courier New', monospace;
        font-size: 13px;
        min-height: 50px;
        width: 100%;
        position: relative;
        margin: 4px 0;
    }

    :deep(component-loader::before) {
        content: '📦 ';
        font-size: 18px;
        margin-right: 6px;
        opacity: 0.6;
    }

    :deep(component-loader::after) {
        content: attr(src);
        font-weight: 500;
        word-break: break-all;
        opacity: 0.8;
    }
    
    :deep(component-loader:hover) {
        background: #e9ecef;
        border-color: #adb5bd;
    }

</style>
