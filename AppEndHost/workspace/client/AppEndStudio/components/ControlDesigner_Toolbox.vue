<template>
    <div class="toolbox-panel bg-white border-end" style="min-width:110px;width:10%;">
        <div class="toolbox-body overflow-auto">
            <div v-for="group in toolboxGroups" :key="group.key">
                <div v-if="group.alwaysShow || (typeof group.show === 'function' ? group.show.call(this) : group.show)" class="component-group border-bottom">
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
</template>

<script>
    export default {
        props: {
            isCanvasEmpty: {
                type: Boolean,
                default: true
            }
        },
        data() {
            return {
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
                                type: 'component-loader', label: 'Component', icon: 'fa-solid fa-cubes',
                                template: '<component-loader src="/a.CustomComponents/C1.vue"></component-loader>'
                            }
                        ]
                    }
                ]
            };
        },
        methods: {
            onDragStart(component, event) {
                this.$emit('component-drag-start', component, event);
            }
        }
    }
</script>
