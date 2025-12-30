<template>
    <div class="card border-0 bg-transparent rounded-0 h-100">
        <div class="card-body bg-transparent fs-d8 p-0">
            <!-- Main container with horizontal splitter -->
            <div class="h-100 w-100" data-flex-splitter-horizontal style="flex: auto;" id="splitContainer">
                <!-- Left panel: File tree navigation -->
                <div class="h-100" style="min-width:350px;width:30%;" v-if="shared.fixNull(lockToSelectedPath, '') === ''">
                    <div class="card h-100 shadow-sm rounded-0 border-0">
                        <!-- Toolbar with file operations -->
                        <div class="card-header px-2 bg-warning-subtle host-toolbar">
                            
                            <div class="hstack">
                                <!-- Refresh folder button -->
                                <button class="btn btn-link text-decoration-none bg-hover-light host-toolbar-btn" @click="refreshFolder(null)" title="Refresh Folder">
                                    <i class="fa-solid fa-fw fa-refresh"></i>
                                </button>
                                <div class="vr mx-1"></div>
                                <!-- Upload file button -->
                                <label for="fileToUpload" class="btn btn-link text-decoration-none bg-hover-light host-toolbar-btn" title="Upload">
                                    <i class="fa-solid fa-upload"></i>
                                </label>
                                <input class="form-control collapse" type="file" id="fileToUpload" @change="uploadFile">
                                <!-- Download button -->
                                <button class="btn btn-link text-decoration-none bg-hover-light host-toolbar-btn" @click="duplicateItem" title="Download">
                                    <i class="fa-solid fa-fw fa-download"></i>
                                </button>
                                <div class="vr mx-1"></div>
                                <!-- Duplicate item button -->
                                <button class="btn btn-link text-decoration-none bg-hover-light host-toolbar-btn" @click="duplicateItem" title="Duplicate">
                                    <i class="fa-solid fa-fw fa-copy"></i>
                                </button>
                                <div class="vr mx-1"></div>
                                <!-- Create new folder button -->
                                <button class="btn btn-link text-decoration-none bg-hover-light host-toolbar-btn" @click="newFolder" title="New Folder">
                                    <i class="fa-solid fa-fw fa-folder-blank"></i>
                                </button>
                                <!-- Create new file button -->
                                <button class="btn btn-link text-decoration-none bg-hover-light host-toolbar-btn" @click="newFile" title="New File">
                                    <i class="fa-solid fa-fw fa-file-alt"></i>
                                </button>
                                <div class="vr mx-1"></div>
                                <!-- Rename item button -->
                                <button class="btn btn-link text-decoration-none bg-hover-light host-toolbar-btn" @click="renameItem" title="Rename">
                                    <i class="fa-solid fa-fw fa-i-cursor"></i>
                                </button>
                                <div class="p-0 ms-auto"></div>
                                <!-- Delete item button -->
                                <button class="btn btn-link text-secondary text-hover-danger text-decoration-none bg-hover-light host-toolbar-btn" @click="deleteItem">
                                    <i class="fa-solid fa-fw fa-trash"></i>
                                </button>
                            </div>

                        </div>
                        <!-- Tree view container -->
                        <div class="card-body p-0 pt-2 scrollable">
                            <div id="hostTree"></div>
                        </div>
                    </div>
                </div>
                <!-- Splitter separator -->
                <div role="separator" tabindex="1" class="bg-warning-subtle" style="width:8px; min-width:8px; cursor: col-resize; background: linear-gradient(90deg, transparent 0%, rgba(0,0,0,0.02) 45%, rgba(0,0,0,0.06) 50%, rgba(0,0,0,0.02) 55%, transparent 100%);" v-if="shared.fixNull(lockToSelectedPath, '') === ''"></div>
                <!-- Right panel: File content viewer/editor -->
                <div class="h-100" :style="shared.fixNull(lockToSelectedPath, '') === '' ? 'min-width:600px;width:69.5%;overflow:hidden' : ''">
                    <div class="card h-100 shadow-sm rounded-0 border-0">
                        <!-- Header showing file path and design mode button for .vue files -->
                        <div class="card-header file-header" id="selectedNodeHeader">
                             <div class="hstack">
                                 <span class="fw-bold" v-if="selectedNode===null">Not selected</span>
                                 <span class="fw-bold" v-if="selectedNode!==null">{{selectedNode.id.replaceAll('/',' / ')}}</span>
                                 <div class="p-0 ms-auto"></div>
                                 <!-- Design mode button (only visible for .vue files) -->
                                 <a v-if="selectedNode!==null && selectedNode.id.endsWith('.vue')" 
                                    :href="'?c=components/ControlDesigner&edt='+selectedNode.id" 
                                    class="btn btn-sm btn-link text-decoration-none bg-hover-light" 
                                    title="Go to Design Mode">
                                     <i class="fa-solid fa-fw fa-palette"></i> Design Mode
                                 </a>
                             </div>
                         </div>
                        <div class="card-body p-0">
                            <div class="container-fluid p-0 h-100">
                                
                                <!-- Text editor view (Ace Editor) -->
                                <div class="row h-100" v-if="selectedNode!==null && contentType==='text' && preview===false">
                                    <div class="code-editor-container h-100" id="aceTextEditor"></div>
                                </div>

                                <!-- Zip/Package file editor view -->
                                <div class="row h-100" v-if="(contentType==='zip' || contentType==='aepkg') && preview===false">
                                    <div class="col pt-0 h-100">
                                        <div class="card border-0 rounded-0 h-100">
                                            <div class="card-header px-2 bg-warning-subtle host-toolbar">
                                                <div class="hstack">
                                                    <!-- Pack to zip button -->
                                                    <button class="btn btn-link text-decoration-none bg-hover-light host-toolbar-btn" @click="packTo">
                                                        <i class="fa-solid fa-fw fa-minimize"></i> Pack Selected Item 
                                                    </button>
                                                </div>
                                            </div>
                                            <div class="card-body border-0 rounded-0 p-0">
                                                <div class="row pt-2 h-100">
                                                    <!-- Workspace tree (source) -->
                                                    <div class="col-24 h-100 p-0 scrollable">
                                                        <div class="h-100" id="workspaceTree"></div>
                                                    </div>
                                                    <!-- Zip content tree (target) -->
                                                    <div class="col-24 h-100 p-0 scrollable">
                                                        <div class="h-100" id="zipTree"></div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <!-- Preview states for different file types -->
                                <!-- Text file preview hint -->
                                <div class="row h-100 align-items-center text-center" v-if="selectedNode!==null && contentType==='text' && preview===true">
                                    <div class="col-48">
                                        <div class="fst-italic fs-1d4 text-secondary">
                                            Double click to edit
                                        </div>
                                    </div>
                                </div>

                                <!-- Folder preview -->
                                <div class="row h-100 align-items-center text-center" v-if="selectedNode!==null && contentType==='folder' && preview===true">
                                    <div class="col-48">
                                        <div class="fst-italic fs-1d4 text-secondary">
                                            Folder
                                        </div>
                                    </div>
                                </div>

                                <!-- Image preview -->
                                <div class="row h-100 align-items-center text-center" v-if="selectedNode!==null && contentType==='image' && preview===true">
                                    <div class="col-48">
                                        <img :src="selectedNode.id.replace('/workspace/client','')" style="max-width:90%;max-height:90%;" />
                                    </div>
                                </div>

                                <!-- Zip file preview -->
                                <div class="row h-100 align-items-center text-center" v-if="selectedNode!==null && contentType==='zip' && preview===true">
                                    <div class="col-48">
                                        <i class="fa-solid fa-fw fa-4x fa-file-zipper"></i>
                                        <br />
                                        <div>Zip file</div>
                                        <div class="fst-italic fs-1d4 text-secondary">
                                            Double click to edit
                                        </div>
                                    </div>
                                </div>

                                <!-- AppEnd Package file preview -->
                                <div class="row h-100 align-items-center text-center" v-if="selectedNode!==null && contentType==='aepkg' && preview===true">
                                    <div class="col-48">
                                        <img src="/a..lib/images/AppEnd-Logo-Only.png" style="max-width:100%;max-height:100%;" />
                                        <br />
                                        <div>AppEnd Package file</div>
                                        <div class="fst-italic fs-1d4 text-secondary">
                                            Double click to edit
                                        </div>
                                    </div>
                                </div>

                                <!-- Other file types preview -->
                                <div class="row h-100 align-items-center text-center" v-if="selectedNode!==null && contentType==='other' && preview===true">
                                    <div class="col-48">
                                        Other
                                    </div>
                                </div>

                                <!-- No selection state -->
                                <div class="row h-100 align-items-center text-center" v-if="selectedNode===null && shared.fixNull(lockToSelectedPath, '') === ''">
                                    <div class="col-48">
                                        <div class="fst-italic fs-1d4 text-secondary">
                                            Select a node on the host tree view
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
    shared.setAppTitle("$auto$");
    // Component state object
    let _this = { 
        cid: "",                    // Component ID
        c: null,                    // Component instance reference
        inputs: {},                 // Input parameters
        lockToSelectedPath: "",     // Locked path (for direct file opening)
        selectedNode: null,         // Currently selected tree node
        regulator: null,            // Input validation regulator
        preview: false,             // Preview mode flag
        contentType: null,          // Type of content (text, zip, image, etc.)
        editView: false,            // Edit view flag
        textToEdit: "aaa"          // Text to edit (legacy)
    };
    
    export default {
        methods: {
            /**
             * Pack selected workspace item into the currently opened zip file
             */
            packTo() {
                let packingNode = _this.c.getSelectedWorkspaceHostNode();
                packingNode = packingNode === "#" ? "" : packingNode.id;
                let zipFile = _this.c.selectedNode !== null ? _this.c.selectedNode.id : _this.c.lockToSelectedPath;
                rpcAEP("PackItemToZipFile", { ItemToPack: packingNode, ZipFile: zipFile }, function (res) {
                    rpcAEP("GetZipFileContent", { "PathToRead": zipFile }, function (res) {
                        _this.c.setupZipTree(R0R(res), false);
                    });
                });
            },
            
            /**
             * Rename the selected file or folder
             * Shows a prompt dialog and updates the tree after renaming
             */
            renameItem() {
                let tree = $("#hostTree:first");
                let node = _this.c.getSelectedHostNode();
                if (node === "#") return;
                showPrompt({
                    title: "Rename", 
                    message1: `Enter a new name for ${node.id}`, 
                    message2: "Spaces and Wildcards are not allowed",
                    retVal: node.text,
                    callback: function (ret) {
                        let oldName = node.text;
                        let newName = ret;
                        // Preserve file extension
                        if (node.type === "file") {
                            let ext = ret.split('.')[ret.split('.').length - 1];
                            newName = fixEndBy(newName, '.' + ext);
                        }
                        let fullNewName = node.id.replace(oldName, newName);
                        rpcAEP("RenameItem", { ItemPath: node.id, NewItemPath: fullNewName }, function (res) {
                            _this.c.refreshFolder(node.parent === "#" ? "#" : tree.jstree(true).get_node(node.parent));
                        });
                    }
                });
            },
            
            /**
             * Create a new folder under the selected node
             */
            newFolder() {
                let node = _this.c.getSelectedHostNode();
                showPrompt({
                    title: "New Folder", 
                    message1: "Enter a name for new folder", 
                    message2: "Spaces and Wildcards are not allowed",
                    callback: function (ret) {
                        let folderName = node === "#" ? ret : node.id + "/" + ret;
                        rpcAEP("CreateNewFolder", { PathToCreate: folderName }, function (res) {
                            _this.c.refreshFolder(node);
                        });
                    }
                });
            },
            
            /**
             * Create a new file under the selected node
             * Defaults to .vue extension if no extension provided
             */
            newFile() {
                let node = _this.c.getSelectedHostNode();
                showPrompt({
                    title: "New File", 
                    message1: "Enter a name for new file", 
                    message2: "Spaces and Wildcards are not allowed",
                    callback: function (ret) {
                        if (ret.indexOf('.') === -1) ret = ret + '.vue';
                        let fileName = node === "#" ? ret : node.id + "/" + ret;
                        rpcAEP("CreateNewFile", { PathToCreate: fileName }, function (res) {
                            _this.c.refreshFolder(node);
                        });
                    }
                });
            },
            
            /**
             * Refresh folder content
             * If node is invalid or doesn't exist, operation is silently ignored
             * @param {Object|null} node - The tree node to refresh, or null for current folder
             */
            refreshFolder(node) {
                let tree = $("#hostTree:first");
                node = fixNull(node, _this.c.getCurrentFolder());
                
                // Handle root node refresh
                if (node === "#") {
                    _this.c.cleanTree(tree);
                    _this.c.setupHostTree("#hostTree:first");
                } else if (node && node.id) {
                    // Verify node exists in tree before attempting refresh
                    let existingNode = tree.jstree(true).get_node(node.id);
                    if (existingNode && existingNode !== false) {
                        node.loaded = false;
                        tree.jstree(true).close_node(node);
                        tree.jstree(true).delete_node(node.children);
                        _this.c.readFolderContent(tree, node, node.id);
                    }
                }
                // If folder is invalid, silently ignore to prevent errors
            },
            
            /**
             * Get current folder node
             * Returns the selected folder or parent folder if a file is selected
             * @returns {Object|string} The folder node or "#" for root
             */
            getCurrentFolder() {
                let node = _this.c.getSelectedHostNode();
                if (node === "#") return node;
                if (node.type === "folder") return node
                return node.parent;
            },
            
            /**
             * Duplicate the selected file or folder
             */
            duplicateItem() {
                let tree = $("#hostTree:first");
                let node = _this.c.getSelectedHostNode();
                if (node === "#") return;
                rpcAEP("DuplicateItem", { PathToDuplicate: node.id, PathType: node.type }, function (res) {
                    _this.c.refreshFolder(node.parent === "#" ? "#" : tree.jstree(true).get_node(node.parent));
                });
            },
            
            /**
             * Delete the selected file or folder
             * Shows confirmation dialog before deletion
             */
            deleteItem() {
                let tree = $("#hostTree:first");
                let node = _this.c.getSelectedHostNode();
                if (node === "#") return;
                let message2 = node.type === "folder" ? "Becareful, Folder will delete recursively" : "";
                showConfirm({
                    title: "Delete Item", 
                    message1: `Are you sure you want to remove [${node.id}]?`, 
                    message2: message2,
                    callback: function () {
                        rpcAEP("DeleteItem", { ItemPath: node.id, PathType: node.type }, function (res) {
                            _this.c.refreshFolder(node.parent === "#" ? "#" : tree.jstree(true).get_node(node.parent));
                        });
                    }
                });
            },
            
            /**
             * Get the currently selected node from the host tree
             * @returns {Object|string} Selected node or "#" if none selected
             */
            getSelectedHostNode() {
                let tree = $("#hostTree:first");
                let selectedNodes = tree.jstree(true).get_selected(true);
                if (selectedNodes.length > 0) return selectedNodes[0];
                return "#";
            },
            
            /**
             * Get the currently selected node from the workspace tree
             * @returns {Object|string} Selected node or "#" if none selected
             */
            getSelectedWorkspaceHostNode() {
                let tree = $("#workspaceTree:first");
                let selectedNodes = tree.jstree(true).get_selected(true);
                if (selectedNodes.length > 0) return selectedNodes[0];
                return "#";
            },
            
            /**
             * Read folder content from server and populate tree
             * @param {jQuery} tree - The tree instance
             * @param {Object} par - Parent node
             * @param {string} folderPath - Path to read
             * @param {string} filter - Optional filter string
             */
            readFolderContent(tree, par, folderPath, filter) {
                rpcAEP("GetFolderContent", { PathToRead: folderPath }, function (res) {
                    _this.c.addContent(tree, par, R0R(res), filter);
                });
            },
            
            /**
             * Add folders and files to the tree
             * @param {jQuery} tree - The tree instance
             * @param {Object} par - Parent node
             * @param {Object} content - Content object with folders and files arrays
             * @param {string} filter - Optional filter string
             */
            addContent(tree, par, content, filter) {
                // Add folders
                _.forEach(content["folders"], function (i) {
                    if (fixNull(filter, '') === '' || i.Value.indexOf(filter) === -1) {
                        tree.jstree(true).create_node(par, { id: i.Value, text: i.Name, type: "folder", data: i }, "last");
                    }
                });
                // Add files
                _.forEach(content["files"], function (i) {
                    tree.jstree(true).create_node(par, { id: i.Value, text: i.Name, type: "file", data: i }, "last");
                });
                tree.jstree(true).open_node(par);
            },
            
            /**
             * Setup zip file tree view
             * @param {Array} content - Array of file paths in zip
             * @param {boolean} setupHostWorkspace - Whether to setup workspace tree
             */
            setupZipTree(content, setupHostWorkspace) {
                $(`.scrollable`).overlayScrollbars({});
                let tree = $("#zipTree:first");
                _this.c.cleanTree(tree);
                tree.jstree(_this.c.getTreeConfig());

                // Extract and build folder structure
                let folders = _.map(content, function (i) { return i.substring(0, i.lastIndexOf('/')); });
                let tempFolders = folders;
                _.forEach(folders, function (i) {
                    let iParts = i.split("/");
                    let ff = "";
                    _.forEach(iParts, function (ip) {
                        if (ip !== "") {
                            ff = ff + "/" + ip;
                            tempFolders.push(ff);
                        }
                    });
                });
                folders = tempFolders;
                folders = _.uniq(folders, true);
                folders = _.filter(folders, function (i) { return i !== ""; });
                folders = _.sortBy(folders, [i => i.toLowerCase()]);

                // Create folder nodes
                _.forEach(folders, function (f) {
                    let folderName = f.split('/')[f.split('/').length - 1];
                    let d = { value: f, name: folderName };
                    let parentFolderId = f.substring(0, f.lastIndexOf('/'));
                    console.log(folderName + " : " + parentFolderId);
                    let par = tree.jstree(true).get_node(parentFolderId);
                    tree.jstree(true).create_node((par === false ? "#" : par), { id: d.value, text: d.name, type: "folder", data: d }, "last");
                });

                // Create file nodes
                _.forEach(content, function (f) {
                    let fileName = f.split('/')[f.split('/').length - 1];
                    if (fileName !== '') {
                        let folderFullName = f.replace("/" + fileName, "");
                        let par = tree.jstree(true).get_node(folderFullName);
                        let d = { name: fileName, value: f };
                        tree.jstree(true).create_node((par === false ? "#" : par), { id: d.value, text: d.name, type: "file", data: d }, "last");
                        if (par !== false) tree.jstree(true).open_node(par);
                    }
                });
                
                // Setup workspace tree if needed
                if (setupHostWorkspace === true) _this.c.setupHostWorkspaceTree("#workspaceTree:first");
            },
            
            /**
             * Initialize the main host file tree
             * Sets up tree structure and event handlers
             * @param {string} treeSelector - jQuery selector for tree container
             */
            setupHostTree(treeSelector) {
                let tree = $(treeSelector);
                tree.jstree(_this.c.getTreeConfig());
                _this.c.readFolderContent(tree, null, '/');
                
                // Double-click handler: open folder or edit file
                tree.bind("dblclick.jstree", function (event) {
                    _this.c.preview = false;
                    let node = fixNull(tree.jstree(true).get_selected(true)[0], null);
                    if (node === null) return;
                    if (node.type === "folder") {
                        if (node.loaded !== true) {
                            _this.c.readFolderContent(tree, node, node.data.Value);
                            node.loaded = true;
                            _this.c.preview = true;
                        }
                    } else {
                        _this.c.goEditView(tree, node);
                    }
                });
                
                // Single-click handler: show preview after delay
                tree.bind("select_node.jstree", function (evt, data) {
                    _this.c.preview = true;
                    _this.c.selectedNode = data.node;
                    setTimeout(function () {
                        if (_this.c.preview === true) _this.c.makePreview(tree, _this.c.selectedNode);
                    }, 250);
                });
            },
            
            /**
             * Initialize workspace tree for zip packing
             * @param {string} treeSelector - jQuery selector for tree container
             */
            setupHostWorkspaceTree(treeSelector) {
                let tree = $(treeSelector);
                _this.c.cleanTree(tree);
                tree.jstree(_this.c.getTreeConfig());
                _this.c.readFolderContent(tree, null, '/workspace', 'appendpackages');
                
                // Double-click handler for folders only
                tree.bind("dblclick.jstree", function (event) {
                    let node = fixNull(tree.jstree(true).get_selected(true)[0], null);
                    if (node === null) return;
                    if (node.type === "folder") {
                        if (node.loaded !== true) {
                            _this.c.readFolderContent(tree, node, node.data.Value);
                            node.loaded = true;
                        }
                    }
                });
            },
            
            /**
             * Switch to edit view for the selected file
             * Loads appropriate editor based on content type
             * @param {jQuery} tree - Tree instance
             * @param {Object} node - Selected node
             */
            goEditView(tree, node) {
                _this.c.preview = false;
                _this.c.contentType = getContentType(node.id);
                $(document).ready(function () {
                    if (_this.c.contentType === "text") {
                        // Load text file into Ace editor
                        $(document).ready(function () {
                            rpcAEP("GetFileContent", { "PathToRead": node.id }, function (res) {
                                ace.edit("aceTextEditor", { mode: getEditorMode(node.id), value: R0R(res) });
                            });
                        });
                    } else if (_this.c.contentType === "zip" || _this.c.contentType === "aepkg") {
                        // Load zip/package content
                        rpcAEP("GetZipFileContent", { "PathToRead": node.id }, function (res) {
                            _this.c.setupZipTree(R0R(res), true);
                        });
                    } else {
                        // Show preview for other types
                        _this.c.preview = true;
                        this.makePreview(tree, node);
                    }
                });
            },
            
            /**
             * Prepare preview for the selected node
             * Determines content type based on node type
             * @param {jQuery} tree - Tree instance
             * @param {Object} node - Selected node
             */
            makePreview(tree, node) {
                if (node.type === "file") {
                    _this.c.contentType = getContentType(node.id);
                } else {
                    _this.c.contentType = "folder";
                }
            },
            
            /**
             * Get jsTree configuration object
             * @param {Object} dndConf - Optional drag-and-drop configuration
             * @returns {Object} jsTree configuration
             */
            getTreeConfig(dndConf) {
                let conf = {
                    core: { check_callback: true, },
                    plugins: ["types"],
                    types: {
                        folder: { icon: "fa-solid fa-folder", valid_children: ["file", "folder"] },
                        file: { icon: "fa-solid fa-file text-info", valid_children: [] }
                    },
                };

                if (dndConf) {
                    conf["plugins"].push("dnd");
                    conf["dnd"] = dndConf;
                }
                return conf;
            },
            
            /**
             * Handle file upload
             * Uploads selected file to current folder or selected folder
             */
            uploadFile() {
                let tree = $("#hostTree:first");
                let node = fixNull(tree.jstree(true).get_selected(true)[0], null);
                let uploadingFolder = (node === null ? "" : node.id);

                // If a file is selected, upload to its parent folder
                if (node !== null) {
                    if (node.type === "file") {
                        let lastPart = uploadingFolder.split('/')[uploadingFolder.split('/').length - 1];
                        uploadingFolder = uploadingFolder.replace(lastPart, "");
                        if (uploadingFolder === "/") uploadingFolder = "";
                    }
                }

                let thisInput = document.getElementById('fileToUpload');
                let fileReader = new FileReader();
                fileReader.onload = function () {
                    let fileBody = getB64Str(fileReader.result);
                    let fileName = thisInput.files[0].name;
                    let finalFileName = uploadingFolder === "" ? fileName : uploadingFolder + "/" + fileName;
                    if (finalFileName.startsWith("/")) finalFileName = finalFileName.replace("/", "");
                    
                    // Upload file to server
                    rpcAEP("UploadFile", { FileName: finalFileName, FileBody: fileBody }, function (res) {
                        thisInput.value = '';
                        let par = node === null ? null : (node.type === "folder" ? node : node.parent);
                        if (par !== null) {
                            par.loaded = false;
                            tree.jstree(true).delete_node(par.children);
                            _this.c.readFolderContent(tree, par, par.id);
                        }
                    });
                }
                fileReader.readAsArrayBuffer(thisInput.files[0]);
            },
            
            /**
             * Clean and destroy tree instance
             * @param {jQuery} tree - Tree instance to clean
             */
            cleanTree(tree) {
                try {
                    tree.html("");
                    tree.jstree(true).destroy();
                } catch { }
            },
            
            /**
             * Handle OK button click (for modal mode)
             */
            ok(e) {
                if (_this.c.inputs.callback) _this.c.inputs.callback();
                _this.c.close();
            },
            
            /**
             * Handle Cancel button click
             */
            cancel() { 
                _this.c.close(); 
            },
            
            /**
             * Close component (modal)
             */
            close() { 
                shared.closeComponent(_this.cid); 
            }
        },
        
        /**
         * Setup component props and inputs
         */
        setup(props) {
            _this.cid = props['cid'];
            _this.inputs = shared["params_" + _this.cid];
            _this.lockToSelectedPath = (fixNull(_this.inputs, '') !== '') ? _this.inputs["lockToSelectedPath"] : null;
        },
        
        /**
         * Return component data
         */
        data() { 
            return _this; 
        },
        
        /**
         * Component created lifecycle hook
         */
        created() { 
            _this.c = this; 
        },
        
        /**
         * Component mounted lifecycle hook
         * Initializes the component based on mode (normal or locked path)
         */
        mounted() {
            initVueComponent(_this);
            
            if (fixNull(_this.lockToSelectedPath, "") === "") {
                // Normal mode: show file tree
                _this.c.setupHostTree("#hostTree:first");
            } else {
                // Locked path mode: directly open specified file
                $("#selectedNodeHeader").remove();
                $("#splitContainer").removeAttr("data-flex-splitter-horizontal");
                _this.c.preview = false;
                _this.c.contentType = getContentType(_this.c.lockToSelectedPath);
                rpcAEP("GetZipFileContent", { "PathToRead": _this.c.lockToSelectedPath }, function (res) {
                    _this.c.setupZipTree(R0R(res), true);
                });
            }
        },
        
        /**
         * Component props definition
         */
        props: { 
            cid: String 
        }
    }
</script>

<style scoped>
.host-toolbar, .file-header {
    min-height: 56px;
    display: flex;
    align-items: center;
}
.host-toolbar .hstack, #selectedNodeHeader .hstack {
    width: 100%;
    display: flex;
    align-items: center;
}
.host-toolbar-btn {
    padding: .36rem .6rem;
    font-size: .95rem;
    border-radius: .375rem;
    transition: background-color .12s ease, transform .08s ease;
}
.host-toolbar-btn:hover {
    background-color: rgba(0,0,0,0.04);
    transform: translateY(-1px);
}
</style>
