<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-header p-2 bg-success-subtle rounded-0 border-0">
            <div class="hstack gap-1">
                <button type="button" class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="createPackage">
                    <i class="fa-solid fa-file-alt fa-bounce" style="--fa-animation-iteration-count:1"></i> <span>Create Package</span>
                </button>
                <button type="button" class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="uploadPackage">
                    <i class="fa-solid fa-upload fa-bounce" style="--fa-animation-iteration-count:1"></i> <span>Upload Package</span>
                </button>
                <div class="p-0 ms-auto"></div>
            </div>
        </div>
        <div class="card-body p-2">
            <div class="card h-100 border-light bg-light bg-opacity-75 border-0">
                <div class="card-body rounded rounded-2 border border-3 border-light fs-d8 p-3 bg-transparent scrollable">
                    <div class="row g-1">
                        <div class="col-48 col-md-12" v-for="pkg in packages">
                            <div class="card shadow-sm" :class="pkg.Installed===true ? 'border-success-subtle' : 'border-primary-subtle'">
                                <div class="card-header p-2 text-secondary">
                                    <div class="hstack gap-1">
                                        <div>
                                            <div v-if="pkg.Installed===true">
                                                <span class="fw-bold fs-1d2 text-success">{{pkg.Title}}</span>
                                            </div>
                                            <div v-if="pkg.Installed===false">
                                                <span class="fw-bold fs-1d2 text-dark">{{pkg.Title}}</span>
                                            </div>
                                            <div class="mx-2 fst-italic">{{pkg.Name}} {{pkg.Version}}</div>
                                            <div class="mx-2 fs-d8">
                                                <span>Url : </span> <a v-if="shared.fixNull(pkg.Url,'')!==''" :href="shared.fixStartBy(pkg.Url,'https://')" target="_blank">{{pkg.Url}}</a>
                                            </div>
                                        </div>
                                        <div class="p-0 ms-auto"></div>
                                        <a class="btn btn-sm btn-link text-decoration-none bg-hover-light fs-d9 text-center" @click="editInfoPackage(pkg.Name)">
                                            <i class="fa-solid fa-fw fa-tag fa-2x"></i>
                                            <br />
                                            <span>Info</span>
                                        </a>
                                        <a class="btn btn-sm btn-link text-decoration-none bg-hover-light fs-d9 text-center" @click="editFilesPackage(pkg.Name)">
                                            <i class="fa-solid fa-fw fa-file fa-2x"></i>
                                            <br />
                                            <span>Files</span>
                                        </a>
                                        <a class="btn btn-sm btn-link text-decoration-none bg-hover-light fs-d9 text-center" @click="rePackPackage(pkg.Name)">
                                            <i class="fa-solid fa-fw fa-minimize fa-2x"></i>
                                            <br />
                                            <span>RePack</span>
                                        </a>
                                    </div>
                                </div>

                                <div class="card-body">
                                    <div class="text-dark overflow-hidden" style="min-height:55px;max-height:55px;">
                                        <div>{{pkg.Note}}</div>
                                    </div>
                                </div>
                                <div class="card-footer fs-d8 p-1 px-2 text-secondary">
                                    <div>
                                        <div class="text-success" v-if="pkg.Installed===true">
                                            <i class="fa-solid fa-fw fa-check-double"></i>
                                            <span>Installed By</span> <span class="fw-bold">{{pkg.InstalledBy}}</span>
                                            <span> On</span> <span class="fw-bold">{{shared.formatDateTime(pkg.InstalledOn)}}</span>
                                        </div>
                                        <div class="text-secondary" v-if="pkg.Installed===false">
                                            <span><i class="fa-solid fa-fw fa-minus"></i> Not Installed</span>
                                        </div>
                                    </div>
                                    <div>
                                        <span class="d-inline-block">Created By</span> <span class="fw-bold">{{pkg.CreatedBy}}</span> On <span class="fw-bold">{{shared.formatDateTime(pkg.CreatedOn)}}</span>
                                    </div>
                                    <div>
                                        <span class="d-inline-block">Updated By</span> <span class="fw-bold">{{pkg.UpdatedBy}}</span> On <span class="fw-bold">{{shared.formatDateTime(pkg.UpdatedOn)}}</span>
                                    </div>
                                </div>
                                <div class="card-footer px-2">
                                    <div class="hstack gap-1">
                                        <a class="btn btn-sm btn-link text-decoration-none bg-hover-light fs-d9" @click="installPackage(pkg.Name)">
                                            <i class="fa-solid fa-fw fa-maximize me-1"></i><span v-if="pkg.Installed===false">Install</span><span v-else>ReInstall</span>
                                        </a>
                                        <a class="btn btn-sm btn-link text-decoration-none bg-hover-light fs-d9" @click="downloadPackage(pkg.Name)">
                                            <i class="fa-solid fa-fw fa-download me-1"></i><span v-else>Download</span>
                                        </a>
                                        <div class="p-0 ms-auto"></div>
                                        <a class="btn btn-sm btn-link text-secondary text-decoration-none bg-hover-light text-hover-danger fs-d9" @click="unInstallPackage(pkg.Name)">
                                            <i class="fa-solid fa-fw fa-eraser me-1"></i><span>UnInstall</span>
                                        </a>
                                        <a class="btn btn-sm btn-link text-secondary text-decoration-none bg-hover-light text-hover-danger fs-d9" @click="removePackage(pkg.Name)">
                                            <i class="fa-solid fa-fw fa-trash me-1"></i><span>Remove</span>
                                        </a>
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
    let _this = { cid: "", c: null, packages: [] };
    export default {
        methods: {
            uploadPackage() {
                alert("uploadPackage...");
            },
            downloadPackage(pkgName) {
                rpcAEP("DownloadPackage", { PackageName: pkgName }, function (res) {
                    let fileBody = R0R(res);
                    downloadFile(fileBody, pkgName);
                });
            },
            createPackage() {
                let packageInfo = { Title: "", Note: "", Version: "1.0.0", CreatedBy: shared.getUserObject()["UserName"], CreatedOn: formatDateTime(new Date()), UpdatedBy: shared.getUserObject()["UserName"], UpdatedOn: formatDateTime(new Date()), Url: "" };
                openComponent("components/devopsPackageInfoEditor", {
                    title: "PackageInfo Editor", modalSize: "modal-fullscreen",
                    params: {
                        packageName: "",
                        packageInfo: packageInfo,
                        callback: function (ret) {
                            rpcAEP("SavePackageInfo", { PackageName: "", PackageNewName: fixEndBy(ret.packageName, '.aepkg'), PackageInfo: ret.packageInfo }, function (res) {
                                _this.c.readPackages();
                            });
                        }
                    }
                });
            },
            editInfoPackage(pkgName) {
                let packageInfo = _.filter(_this.c.packages, function (i) { return i.Name.toLowerCase() === pkgName.toLowerCase() })[0];
                openComponent("components/devopsPackageInfoEditor", {
                    title: "PackageInfo Editor", modalSize: "modal-fullscreen",
                    params: {
                        packageName: pkgName,
                        packageInfo: _.cloneDeep(packageInfo),
                        callback: function (ret) {
                            rpcAEP("SavePackageInfo", { PackageName: pkgName, PackageNewName: fixEndBy(ret.packageName, '.aepkg'), PackageInfo: ret.packageInfo }, function (res) {
                                _this.c.readPackages();
                            });
                        }
                    }
                });
            },
            editFilesPackage(pkgName) {
                openComponent("components/devopsPackageFilesEditor", {
                    title: "PackageInfo Editor", modalSize: "modal-fullscreen",
                    params: {
                        packageName: pkgName,
                        callback: function (ret) {
                            showJson(ret);
                            return;
                            rpcAEP("SavePackageFiles", { PackageName: pkgName, PackageInfo: ret.files }, function (res) {
                            });
                        }
                    }
                });
            },
            removePackage(pkgName) {
                showConfirm({
                    title: "Remove Package", message1: `Are you sure you want to remove ${pkgName}?`, message2: "This action removes the package file from your host, but it will remain standing.",
                    callback: function () {

                    }
                });
            },
            unInstallPackage(pkgName) {
                showConfirm({
                    title: "UnInstall Package", message1: `Are you sure you want to uninstall this ${pkgName}?`, message2: "This action will uninstall the package",
                    callback: function () {

                    }
                });
            },
            installPackage(pkgName) {
                showConfirm({
                    title: "Install Package", message1: `Are you sure you want to install this ${pkgName}?`, message2: "This action will install/reinstall the package",
                    callback: function () {

                    }
                });
            },
            rePackPackage(pkgName) {
                showConfirm({
                    title: "RePack Package", message1: `Are you sure you want to repack this ${pkgName}?`, message2: "This action embed in all files in the package, files that do not exist on your host will be ignored",
                    callback: function () {

                    }
                });
            },
            readPackages() {
                rpcAEP("ReadPackages", {}, function (res) {
                    _this.c.packages = R0R(res);
                });
            }
        },
        setup(props) {
            _this.cid = props['cid'];
        },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { _this.c.readPackages(); },
        props: { cid: String },
    }

</script>
