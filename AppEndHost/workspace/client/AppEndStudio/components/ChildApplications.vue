<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-header p-2 bg-body-subtle rounded-0 border-0">
            <div class="hstack">
                <input type="text" class="form-control form-control-sm" style="max-width:250px;" 
                       @keyup.enter="readList" v-model='rowsFilter.Filter' placeholder="Search applications" />
                <div class="vr"></div>
                <button class="btn btn-sm btn-outline-primary" @click="readList" title="Refresh">
                    <i class="fa-solid fa-search"></i>
                </button>
                <div class="p-0 ms-auto"></div>
                <div class="vr"></div>
                <button class="btn btn-sm btn-primary" @click="createNewApp" title="Create New Application">
                    <i class="fa-solid fa-plus"></i> Create App
                </button>
            </div>
        </div>
        <div class="card-body rounded rounded-2 border border-3 border-light fs-d8 p-0 bg-transparent scrollable">
            <table class="table table-sm table-hover w-100 ae-table m-0 bg-transparent">
                <thead>
                    <tr>
                        <th class="sticky-top ae-thead-th text-dark fw-bold" style="width:200px;vertical-align:middle">App Name</th>
                        <th class="sticky-top ae-thead-th text-dark fw-bold" style="width:120px;vertical-align:middle">Port</th>
                        <th class="sticky-top ae-thead-th text-dark fw-bold" style="width:150px;vertical-align:middle;text-align:center">Status</th>
                        <th class="sticky-top ae-thead-th text-dark fw-bold" style="vertical-align:middle">Description</th>
                        <th class="sticky-top ae-thead-th text-dark fw-bold text-center" style="width:300px;vertical-align:middle">Actions</th>
                        <th class="sticky-top ae-thead-th text-dark fw-bold text-center text-secondary" style="width:130px;vertical-align:middle">Created On</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="i in filteredD" :key="i.AppName">
                        <td style="width:200px;vertical-align:middle">
                            <span class="fw-bold">{{i.AppName}}</span>
                        </td>
                        <td style="width:120px;vertical-align:middle">
                            <span class="badge text-bg-info">{{i.Port}}</span>
                        </td>
                        <td style="width:150px;vertical-align:middle;text-align:center">
                            <span class="badge text-bg-success" v-if="i.IsRunning" title="Running">
                                <i class="fa-solid fa-play"></i> Running
                            </span>
                            <span class="badge text-bg-secondary" v-else title="Stopped">
                                <i class="fa-solid fa-stop"></i> Stopped
                            </span>
                        </td>
                        <td style="vertical-align:middle">
                            <span class="text-secondary">{{i.Description}}</span>
                        </td>
                        <td style="width:300px;vertical-align:middle;text-align:center;white-space:nowrap;">
                            <button class="btn btn-sm btn-outline-success" 
                                    @click="runApp(i.AppName)"
                                    v-if="!i.IsRunning">
                                <i class="fa-solid fa-fw fa-play"></i> Run
                            </button>
                            <button class="btn btn-sm btn-outline-warning" 
                                    @click="stopApp(i.AppName)"
                                    v-if="i.IsRunning">
                                <i class="fa-solid fa-fw fa-stop"></i> Stop
                            </button>
                            <button class="btn btn-sm btn-outline-info" 
                                    @click="configureApp(i.AppName)">
                                <i class="fa-solid fa-fw fa-gear"></i> Settings
                            </button>
                            <button class="btn btn-sm btn-outline-primary" 
                                    @click="openAppInBrowser(i.Port)"
                                    v-if="i.IsRunning">
                                <i class="fa-solid fa-fw fa-external-link-alt"></i> Open
                            </button>
                            <button class="btn btn-sm btn-outline-danger" 
                                    @click="deleteApp(i.AppName)">
                                <i class="fa-solid fa-fw fa-trash"></i> Delete
                            </button>
                        </td>
                        <td style="width:130px;vertical-align:middle;text-align:center">
                            <div class="fs-d8 text-secondary">
                                {{shared.formatDateTime(i.CreatedOn)}}
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>

<script>
    shared.setAppTitle("$auto$");
    let _this = { cid: "", c: null, d: [], rowsFilter: {} };
    _this.rowsFilter = { "Filter": "" };
    export default {
        methods: {
            createNewApp() {
                // Get a random available port first
                rpcAEP("GetAvailablePort", {}, function (portRes) {
                    let suggestedPort = R0R(portRes);
                    
                    openComponent("components/CreateChildAppDialog", {
                        title: "Create New Child Application",
                        params: {
                            suggestedPort: suggestedPort,
                            callback: function (appConfig) {
                                rpcAEP("CreateChildApp", {
                                    "AppName": appConfig.AppName.trim(),
                                    "Port": parseInt(appConfig.Port),
                                    "Description": appConfig.Description,
                                    "Template": appConfig.Template
                                }, function (res) {
                                    if (R0R(res) === true) {
                                        showSuccess("Application created successfully");
                                        _this.c.readList();
                                    } else {
                                        showError("Failed to create application: " + R0R(res));
                                    }
                                });
                            }
                        }
                    });
                });
            },
            deleteApp(appName) {
                shared.showConfirm({
                    title: "Delete Application",
                    message1: "Are you sure you want to delete this application?",
                    message2: appName,
                    callback: function () {
                        rpcAEP("DeleteChildApp", { "AppName": appName }, function (res) {
                            if (R0R(res) === true) {
                                showSuccess("Application deleted successfully");
                                _this.c.readList();
                            } else {
                                showError("Failed to delete application");
                            }
                        });
                    }
                });
            },
            runApp(appName) {
                rpcAEP("RunChildApp", { "AppName": appName }, function (res) {
                    if (R0R(res) === true) {
                        showSuccess("Application started successfully. Initializing...");
                        
                        // First refresh after 1 second
                        setTimeout(function () {
                            _this.c.readList();
                        }, 1000);
                        
                        // Auto-refresh every 2 seconds for 20 seconds total
                        let checkCount = 0;
                        let checkInterval = setInterval(function () {
                            checkCount++;
                            _this.c.readList();
                            
                            // Check if app is running
                            let app = _this.c.d.find(x => x.AppName === appName);
                            if (app && app.IsRunning) {
                                clearInterval(checkInterval);
                                showSuccess("✓ Application is now running!");
                            }
                            
                            // Give up after 20 seconds
                            if (checkCount >= 10) {
                                clearInterval(checkInterval);
                                if (!app || !app.IsRunning) {
                                    showError("⚠ Application startup timeout. Check console logs:\n- Application may have failed to start\n- Check port is not already in use\n- Check appsettings are valid");
                                }
                            }
                        }, 2000);
                    } else {
                        showError("Failed to start application. Check the server console for details.");
                    }
                });
            },
            stopApp(appName) {
                shared.showConfirm({
                    title: "Stop Application",
                    message1: "Are you sure you want to stop this application?",
                    message2: appName,
                    callback: function () {
                        rpcAEP("StopChildApp", { "AppName": appName }, function (res) {
                            if (R0R(res) === true) {
                                showSuccess("Application stopped successfully");
                                setTimeout(function () {
                                    _this.c.readList();
                                }, 500);
                            } else {
                                showError("Failed to stop application");
                            }
                        });
                    }
                });
            },
            configureApp(appName) {
                openComponent("components/ChildApplicationSettings", {
                    title: "Application Settings",
                    params: {
                        AppName: appName,
                        callback: function (config) {
                            rpcAEP("UpdateChildAppConfig", {
                                "AppName": config.AppName,
                                "Port": config.Port,
                                "Description": config.Description,
                                "AutoStart": config.AutoStart,
                                "EnvironmentVariables": config.EnvironmentVariables
                            }, function (res) {
                                if (R0R(res) === true) {
                                    showSuccess("Settings saved successfully");
                                    _this.c.readList();
                                } else {
                                    showError("Failed to save settings");
                                }
                            });
                        }
                    }
                });
            },
            openAppInBrowser(port) {
                window.open(`http://localhost:${port}`, '_blank');
            },
            readList() {
                rpcAEP("GetChildApps", {}, function (res) {
                    _this.c.d = R0R(res);
                });
            }
        },
        computed: {
            filteredD() {
                let list = this.d || [];
                if (this.rowsFilter.Filter && this.rowsFilter.Filter.trim() !== '') {
                    const filter = this.rowsFilter.Filter.toLowerCase();
                    list = _.filter(list, (x) => {
                        return (x.AppName && x.AppName.toLowerCase().includes(filter)) ||
                               (x.Description && x.Description.toLowerCase().includes(filter));
                    });
                }
                return list;
            }
        },
        setup(props) { _this.cid = props['cid']; },
        data() { return { d: _this.d, rowsFilter: _this.rowsFilter }; },
        created() { _this.c = this; },
        mounted() { initVueComponent(_this); _this.c.readList(); },
        props: { cid: String }
    }
</script>
