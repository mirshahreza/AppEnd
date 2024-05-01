<template>
    <div class="card h-100 rounded rounded-2 rounded-bottom-0 rounded-end-0 bg-transparent border-0">
        <div class="card-header p-2 bg-light-subtle rounded-end-0 border-0">
            <div class="input-group input-group-sm border-0 bg-transparent">

                <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" @click="save"><i class="fa-solid fa-save"></i> Save</button>

                <div class="btn-group btn-group-sm mx-1">
                    <button class="btn btn-sm btn-link text-decoration-none bg-hover-light" type="button" @click="addField" title="Add Field">
                        Add Field
                    </button>
                    <button type="button" class="btn btn-sm btn-link text-decoration-none bg-hover-light dropdown-toggle dropdown-toggle-split" data-bs-toggle="dropdown" aria-expanded="false">
                        <span class="visually-hidden">Add Extended Fields</span>
                    </button>
                    <ul class="dropdown-menu">
                        <li>
                            <div class="dropdown-item pointer" @click="addAuditingFields">
                                <i class="fa-solid fa-plus"></i> <span class="fs-d9">Auditing Fields</span>
                            </div>
                        </li>
                        <li>
                            <div class="dropdown-item pointer" @click="addImageFields">
                                <i class="fa-solid fa-plus"></i> <span class="fs-d9">Image Fields</span>
                            </div>
                        </li>
                        <li>
                            <div class="dropdown-item pointer" @click="addFileFields">
                                <i class="fa-solid fa-plus"></i> <span class="fs-d9">File Fields</span>
                            </div>
                        </li>
                        <li>
                            <div class="dropdown-item pointer" @click="addTreeFields">
                                <i class="fa-solid fa-plus"></i> <span class="fs-d9">Tree Fields</span>
                            </div>
                        </li>
                        <li>
                            <div class="dropdown-item pointer" @click="addTitleNoteFields">
                                <i class="fa-solid fa-plus"></i> <span class="fs-d9">Title+Note</span>
                            </div>
                        </li>
                    </ul>
                </div>

                <input type="text" class="form-control form-control-sm border-0 rounded-0 bg-transparent" disabled />
            </div>
        </div>
        <div class="card-body p-2">
            <div class="card h-100 border-light bg-light bg-opacity-75 border-0">
                <div class="card-body rounded rounded-2 border border-3 border-light fs-d8 p-0 bg-transparent">

                    <table class="table table-sm table-hover w-100 ae-table m-0 bg-transparent">
                        <thead>
                            <tr>
                                <th style="width:18px" class="text-center">Pk</th>
                                <th class="">Name</th>
                                <th style="width:140px" class="">DbType</th>
                                <th style="width:60px" class="text-center">Size</th>
                                <th style="width:60px" class="text-center">Default</th>
                                <th style="width:100px" class="text-center">Start/Step</th>
                                <th style="width:50px" class="text-center">Null</th>
                                <th style="width:300px" class="">Fk</th>
                                <th style="width:30px" class="text-center"></th>
                                <th style="width:16px" class="text-center"></th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="(i,ind) in tableDef.Columns">
                                <td class="ae-table-td text-center p-0">
                                    <i class="fa fa-fw fa-key text-primary fs-d8" v-if="i.IsPrimaryKey===true"></i>
                                </td>
                                <td class="ae-table-td p-0 px-1">
                                    <span class="ae-data-key" style="display:none;visibility:hidden;">{{i.Name}}</span>
                                    <input type="text" class="ae-ingrid-input ae-input-field-name" :class="i.ValidationCss"
                                           v-model="i.Name" v-if="i.State!=='d'" @input="fieldNameKeyup" />
                                    <div class="ae-ingrid-input text-decoration-line-through" v-if="i.State==='d'">{{i.Name}}</div>
                                </td>
                                <td class="ae-table-td p-0">
                                    <select class="ae-ingrid-input" v-model="i.DbType" @change="typeChanged">
                                        <option value="BIGINT">BIGINT</option>
                                        <option value="INT">INT</option>
                                        <option value="TINYINT">TINYINT</option>
                                        <option value="SMALLINT">SMALLINT</option>
                                        <option value="DECIMAL">DECIMAL</option>
                                        <option value="FLOAT">FLOAT</option>
                                        <option value="NUMERIC">NUMERIC</option>
                                        <option value="REAL">REAL</option>

                                        <option value="BIT" v-if="!i.IsPrimaryKey">BIT</option>
                                        <option value="UNIQUEIDENTIFIER">UNIQUEIDENTIFIER</option>

                                        <option value="DATE" v-if="!i.IsPrimaryKey">DATE</option>
                                        <option value="TIME" v-if="!i.IsPrimaryKey">TIME</option>
                                        <option value="DATETIME" v-if="!i.IsPrimaryKey">DATETIME</option>
                                        <option value="DATETIME2" v-if="!i.IsPrimaryKey">DATETIME2</option>
                                        <option value="SMALLDATETIME" v-if="!i.IsPrimaryKey">SMALLDATETIME</option>
                                        <option value="DATETIMEOFFSET" v-if="!i.IsPrimaryKey">DATETIMEOFFSET</option>
                                        <option value="TIMESTAMP">TIMESTAMP</option>

                                        <option value="TEXT" v-if="!i.IsPrimaryKey">TEXT</option>
                                        <option value="NTEXT" v-if="!i.IsPrimaryKey">NTEXT</option>
                                        <option value="VARCHAR">VARCHAR</option>
                                        <option value="NVARCHAR">NVARCHAR</option>
                                        <option value="CHAR">CHAR</option>
                                        <option value="NCHAR">NCHAR</option>

                                        <option value="IMAGE" v-if="!i.IsPrimaryKey">IMAGE</option>

                                        <option value="XML" v-if="!i.IsPrimaryKey">XML</option>
                                    </select>
                                </td>
                                <td class="ae-table-td py-0 text-center">
                                    <div>
                                        <input type="text" onkeypress="return isNumberKey(event)" @input="generalKeyup" class="ae-ingrid-input text-center" v-model="i.Size"
                                               v-if="'[DECIMAL],[NUMERIC],[DATETIME2],[DATETIMEOFFSET],[TIME],[VARCHAR],[NVARCHAR],[CHAR],[NCHAR],[VARBINARY]'.indexOf('['+i.DbType+']')>-1" />
                                    </div>
                                </td>
                                <td class="p-0">
                                    <input type="text" class="ae-ingrid-input text-center ae-input-field-default" v-model="i.DbDefault" @input="generalKeyup" />
                                </td>
                                <td class="p-0">
                                    <input type="text" onkeypress="return isNumberKey(event)" @input="generalKeyup"
                                           class="ae-ingrid-input text-center ae-input-field-start mx-1" style="width:40%;"
                                           v-if="i.IsPrimaryKey===true" v-model="i.IdentityStart" />
                                    <input type="text" onkeypress="return isNumberKey(event)" @input="generalKeyup"
                                           class="ae-ingrid-input text-center ae-input-field-step mx-1" style="width:40%;"
                                           v-if="i.IsPrimaryKey===true" v-model="i.IdentityStep" />
                                </td>
                                <td class="ae-table-td p-0 text-center">
                                    <span v-if="i.IsPrimaryKey!==true">
                                        <input type="checkbox" class="form-check-input" @input="generalKeyup" v-model="i.AllowNull" />
                                    </span>
                                    <span v-else></span>
                                </td>

                                <td class="ae-table-td p-0 px-1">
                                    <div v-if="i.DbType==='INT' || i.DbType==='BIGINT' || i.DbType==='TINYINT' || i.DbType==='SMALLINT'">
                                        <div class="text-primary pointer">
                                            <div v-if="shared.fixNull(i.Fk,'')==='' || shared.fixNull(i.Fk.TargetTable,'')===''" @click="openFkEditor">...</div>
                                            <div v-else>
                                                <i @click="deleteFk" class="fa-solid fa-times text-muted hover-danger me-1" style="vertical-align:middle"></i>
                                                <span @click="openFkEditor" style="vertical-align:central">{{i.Fk.TargetTable}} :: {{i.Fk.TargetColumn}}</span>
                                            </div>
                                        </div>
                                    </div>
                                </td>
                                <td class="ae-table-td p-0 text-center text-center" style="padding-top:8px;">
                                    <span @click="deleteField" v-if="i.IsPrimaryKey!==true && i.State!=='d'">
                                        <i class="fa-solid fa-times pointer text-muted hover-danger"></i>
                                    </span>

                                    <span @click="deleteFieldUndo" v-if="i.State==='d'">
                                        <i class="fa-solid fa-undo pointer text-primary hover-success"></i>
                                    </span>
                                </td>
                                <td class="ae-table-td p-0 text-center text-center">
                                    <span v-if="i.State==='d'">
                                        <span class="fb text-danger" title="Deleted">{{i.State}}</span>
                                    </span>
                                    <span v-if="i.State==='u'">
                                        <span class="fb text-primary" title="Changed">{{i.State}}</span>
                                    </span>
                                    <span v-if="i.State==='n'">
                                        <span class="fb text-success" title="New">{{i.State}}</span>
                                    </span>
                                    <span v-else>
                                    </span>
                                </td>

                            </tr>
                        </tbody>
                    </table>

                </div>
            </div>
        </div>
    </div>
</template>

<script>
    shared.setAppTitle("Table Designer");
    shared.setAppSubTitle(getQueryString("o"));
    let _this = {
        cid: "",
        dbConfName: getQueryString("cnn"),
        tableDef: {
            Name: getQueryString("o"),
            Columns: []
        },
        c: null, editor: null
    };
    export default {
        methods: {
            deleteFk(event) {
                let fieldName = $(event.target).parents("tr:first").find(".ae-data-key").text();
                let fkName = _.find(_this.c.tableDef["Columns"], function (i) { return i.Name === fieldName; }).Fk.FkName;
                shared.showConfirm({
                    title: "Remove Fk", message1: "Are you sure you want to delete this Fk:", message2: fkName,
                    callback: function () {
                        rpcAEP("DropFk", { "DbConfName": _this.dbConfName, "ObjectName": _this.tableDef.Name, "FkName": fkName }, function (res) {
                            setTimeout(function () {
                                let col = _.find(_this.c.tableDef["Columns"], function (i) { return i.Name === fieldName; });
                                delete col.Fk;
                            }, 200);
                        });
                    }
                });
            },
            openFkEditor(event) {
                let fieldName = $(event.target).parents("tr:first").find(".ae-data-key").text();
                let fkCol = _.find(_this.c.tableDef["Columns"], function (i) { return i.Name === fieldName; });
                let fkName = "";

                if (fixNull(fkCol["Fk"], '') === '') {
                    fkCol["Fk"] = { "TargetTable": "", "TargetColumn": "", "EnforceRelation": true };
                } else {
                    fkName = fkCol.Fk.FkName;
                }

                openComponent("components/dbFkEditor", {
                    title: "Fk Editor", params: {
                        "DbConfName": _this.dbConfName,
                        "FkName": fkName,
                        "BaseTable": _this.tableDef.Name,
                        "BaseColumn": fieldName,
                        "TargetTable": fkCol.Fk.TargetTable,
                        "TargetColumn": fkCol.Fk.TargetColumn,
                        "EnforceRelation": fkCol.Fk.EnforceRelation === undefined || fkCol.Fk.EnforceRelation === null ? false : fkCol.Fk.EnforceRelation,
                        callback: function (ret) {
                            fkCol["Fk"] = {};
                            fkCol["Fk"]["FkName"] = ret["FkName"];
                            fkCol["Fk"]["BaseTable"] = ret["BaseTable"];
                            fkCol["Fk"]["BaseColumn"] = ret["BaseColumn"];
                            fkCol["Fk"]["TargetTable"] = ret["TargetTable"];
                            fkCol["Fk"]["TargetColumn"] = ret["TargetColumn"];
                            fkCol["Fk"]["EnforceRelation"] = ret["EnforceRelation"];

                            if (fkCol.State !== 'n') fkCol.State = "u";

                        }
                    }
                });
            },
            deleteField(event) {
                let fieldName = $(event.target).parents("tr:first").find(".ae-data-key").text();
                let deleteLocal = false;
                _.forEach(_this.c.tableDef["Columns"], function (i) {
                    if (i.Name === fieldName) {
                        if (i.State === 'n') {
                            deleteLocal = true;
                        } else {
                            i.State = "d";
                        }
                    }
                });
                if (deleteLocal === true) {
                    _this.c.tableDef["Columns"] = _.filter(_this.c.tableDef["Columns"], function (i) {
                        return i.Name !== fieldName;
                    });
                }
            },
            deleteFieldUndo(event) {
                let fieldName = $(event.target).parents("tr:first").find(".ae-data-key").text();
                _.forEach(_this.c.tableDef["Columns"], function (i) {
                    if (i.Name === fieldName) {
                        i.State = "u";
                    }
                });
            },
            addField() {
                _this.c.tableDef["Columns"].push(_this.c.getNewFieldConfig());
            },
            addAuditingFields() {
                _this.c.tableDef["Columns"].push({ "Name": "CreatedBy", "DbType": "INT", "Size": null, "AllowNull": false, "IsPrimaryKey": false, "ValidationCss": "", "State": "n" });
                _this.c.tableDef["Columns"].push({ "Name": "CreatedOn", "DbType": "DATETIME", "Size": null, "AllowNull": false, "IsPrimaryKey": false, "ValidationCss": "", "State": "n" });
                _this.c.tableDef["Columns"].push({ "Name": "StateBy", "DbType": "INT", "Size": null, "AllowNull": true, "IsPrimaryKey": false, "ValidationCss": "", "State": "n" });
                _this.c.tableDef["Columns"].push({ "Name": "StateOn", "DbType": "DATETIME", "Size": null, "AllowNull": true, "IsPrimaryKey": false, "ValidationCss": "", "State": "n" });
            },
            addImageFields() {
                _this.c.tableDef["Columns"].push({ "Name": "Picture_FileBody", "DbType": "IMAGE", "Size": null, "AllowNull": true, "IsPrimaryKey": false, "ValidationCss": "", "State": "n" });
                _this.c.tableDef["Columns"].push({ "Name": "Picture_FileBody_xs", "DbType": "IMAGE", "Size": null, "AllowNull": true, "IsPrimaryKey": false, "ValidationCss": "", "State": "n" });
                _this.c.tableDef["Columns"].push({ "Name": "Picture_FileName", "DbType": "NVARCHAR", "Size": "128", "AllowNull": true, "IsPrimaryKey": false, "ValidationCss": "", "State": "n" });
                _this.c.tableDef["Columns"].push({ "Name": "Picture_FileSize", "DbType": "INT", "Size": null, "AllowNull": true, "IsPrimaryKey": false, "ValidationCss": "", "State": "n" });
                _this.c.tableDef["Columns"].push({ "Name": "Picture_FileMime", "DbType": "VARCHAR", "Size": "128", "AllowNull": true, "IsPrimaryKey": false, "ValidationCss": "", "State": "n" });
            },
            addFileFields() {
                _this.c.tableDef["Columns"].push({ "Name": "File_FileBody", "DbType": "IMAGE", "Size": null, "AllowNull": true, "IsPrimaryKey": false, "ValidationCss": "", "State": "n" });
                _this.c.tableDef["Columns"].push({ "Name": "File_FileName", "DbType": "NVARCHAR", "Size": "128", "AllowNull": true, "IsPrimaryKey": false, "ValidationCss": "", "State": "n" });
                _this.c.tableDef["Columns"].push({ "Name": "File_FileSize", "DbType": "INT", "Size": null, "AllowNull": true, "IsPrimaryKey": false, "ValidationCss": "", "State": "n" });
                _this.c.tableDef["Columns"].push({ "Name": "File_FileMime", "DbType": "VARCHAR", "Size": "128", "AllowNull": true, "IsPrimaryKey": false, "ValidationCss": "", "State": "n" });
            },
            addTreeFields() {
                _this.c.tableDef["Columns"].push({ "Name": "ParentId", "DbType": "INT", "Size": null, "AllowNull": false, "IsPrimaryKey": false, "ValidationCss": "", "State": "n" });
                _this.c.tableDef["Columns"].push({ "Name": "Title", "DbType": "NVARCHAR", "Size": "128", "AllowNull": false, "IsPrimaryKey": false, "ValidationCss": "", "State": "n" });
                _this.c.tableDef["Columns"].push({ "Name": "Note", "DbType": "NVARCHAR", "Size": "4000", "AllowNull": true, "IsPrimaryKey": false, "ValidationCss": "", "State": "n" });
                _this.c.tableDef["Columns"].push({ "Name": "Icon", "DbType": "VARCHAR", "Size": "64", "AllowNull": true, "IsPrimaryKey": false, "ValidationCss": "", "State": "n" });
                _this.c.tableDef["Columns"].push({ "Name": "UiColor", "DbType": "VARCHAR", "Size": "32", "AllowNull": true, "IsPrimaryKey": false, "ValidationCss": "", "State": "n" });
                _this.c.tableDef["Columns"].push({ "Name": "ViewOrder", "DbType": "FLOAT", "Size": null, "AllowNull": true, "IsPrimaryKey": false, "ValidationCss": "", "State": "n" });
            },
            addTitleNoteFields() {
                _this.c.tableDef["Columns"].push({ "Name": "Title", "DbType": "NVARCHAR", "Size": "128", "AllowNull": true, "IsPrimaryKey": false, "ValidationCss": "", "State": "n" });
                _this.c.tableDef["Columns"].push({ "Name": "Note", "DbType": "NVARCHAR", "Size": "4000", "AllowNull": true, "IsPrimaryKey": false, "ValidationCss": "", "State": "n" });
            },
            addJunctionFields() {
                _this.c.tableDef["Columns"].push({ "Name": "FirstId", "DbType": "INT", "Size": null, "AllowNull": false, "IsPrimaryKey": false, "ValidationCss": "", "State": "n" });
                _this.c.tableDef["Columns"].push({ "Name": "SecondId", "DbType": "INT", "Size": null, "AllowNull": false, "IsPrimaryKey": false, "ValidationCss": "", "State": "n" });
            },
            addPk() {
                _this.c.tableDef["Columns"].push({ "Name": "Id", "DbType": "INT", "Size": null, "IsIdentity": true, "IdentityStart": "1", "IdentityStep": "1", "AllowNull": false, "IsPrimaryKey": true, "ValidationCss": "", "State": "n" });
            },
            getNewFieldConfig() {
                return { "Name": genUN('Col'), "DbType": "INT", "Size": null, "AllowNull": true, "IsPrimaryKey": false, "ValidationCss": "", "State": "n" };
            },
            typeChanged(event) {
                let fieldName = $(event.target).parents("tr:first").find(".ae-data-key").text();
                _this.c.setColState(fieldName, "u");
            },
            generalKeyup(event) {
                let fieldName = $(event.target).parents("tr:first").find(".ae-data-key").text();
                _this.c.setColState(fieldName, "u");
            },
            fieldNameKeyup(event) {
                _this.c.setColState($(event.target).val(), "u");
            },
            setColState(colName, state) {
                _.forEach(_this.c.tableDef["Columns"], function (i) {
                    if (i.Name === colName && i.State !== 'n') {
                        i.State = state;
                    }
                });
            },
            save() {
                if (_this.tableDef.Name === '__new__') {
                    showPrompt({
                        title: "Table Name",
                        message1: "Enter a name for new table",
                        message2: "Spaces and Wildcards are not allowed",
                        callback: function (ret) {
                            _this.c.tableDef.Name = ret;
                            setQueryString("o", ret);
                            removeQueryString("template");
                            shared.setAppSubTitle(ret);
                            _this.c.saveTable();
                        }
                    });
                } else {
                    _this.c.saveTable();
                }
            },
            saveTable() {
                rpcAEP("SaveTableSchema", { "DbConfName": _this.dbConfName, "TableDef": _this.c.tableDef }, function (res) {
                    showInfo("Object changes saved");
                    _this.c.readTableDesigne();
                });
            },
            readTableDesigne() {
                if (_this.tableDef.Name === '__new__') {
                    _this.c.createNewByTemplate(getQueryString("template"));
                    return;
                }
                rpcAEP("ReadObjectSchema", { "DbConfName": _this.dbConfName, "ObjectName": _this.tableDef.Name }, function (res) {
                    _this.c.tableDef.Columns = R0R(res);
                });
            },
            createNewByTemplate(template) {
                _this.c.addPk();
                if (template === "TableFiles") {
                    _this.c.addImageFields();
                    _this.c.addAuditingFields();
                }
                else if (template === "TableFilesWithTitleAndNote") {
                    _this.c.addImageFields();
                    _this.c.addTitleNoteFields();
                    _this.c.addAuditingFields();
                }
                else if (template === "TreeTable") {
                    _this.c.addTreeFields();
                    _this.c.addAuditingFields();
                }
                else if (template === "TableJunction") {
                    _this.c.addJunctionFields();
                }
            }
        },
        setup(props) {
            _this.cid = props['cid'];
        },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { _this.c.readTableDesigne(); },
        props: { cid: String }
    }

</script>