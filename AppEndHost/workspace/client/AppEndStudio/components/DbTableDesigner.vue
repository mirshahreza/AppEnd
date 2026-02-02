<template>
    <div class="card h-100 bg-transparent rounded-0 border-0">
        <div class="card-header p-2 bg-frame rounded-0 border-0">
            <div class="hstack gap-2">

                <button class="btn btn-sm btn-link text-decoration-none" @click="save">
                    <i class="fa-solid fa-fw fa-save"></i> <span>Save</span>
                </button>

                <div class="dropdown">
                    <button class="btn btn-sm btn-link text-decoration-none dropdown-toggle" type="button" data-bs-toggle="dropdown">
                        <i class="fa-solid fa-fw fa-plus"></i> <span>Add Field</span>
                    </button>
                    <ul class="dropdown-menu shadow-sm">
                        <li>
                            <a class="dropdown-item text-decoration-none pointer text-nowrap d-flex align-items-center" @click="addField">
                                <i class="fa-solid fa-fw fa-plus text-muted me-2"></i>
                                <span class="fs-d9">Add Field</span>
                            </a>
                        </li>
                        <li><hr class="dropdown-divider"></li>
                        <li>
                            <a class="dropdown-item text-decoration-none pointer text-nowrap d-flex align-items-center" @click="addAuditingFields">
                                <i class="fa-solid fa-fw fa-plus text-muted me-2"></i>
                                <span class="fs-d9">Auditing Fields</span>
                            </a>
                        </li>
                        <li>
                            <a class="dropdown-item text-decoration-none pointer text-nowrap d-flex align-items-center" @click="addImageFields">
                                <i class="fa-solid fa-fw fa-plus text-muted me-2"></i>
                                <span class="fs-d9">Image Fields</span>
                            </a>
                        </li>
                        <li>
                            <a class="dropdown-item text-decoration-none pointer text-nowrap d-flex align-items-center" @click="addFileFields">
                                <i class="fa-solid fa-fw fa-plus text-muted me-2"></i>
                                <span class="fs-d9">File Fields</span>
                            </a>
                        </li>
                        <li>
                            <a class="dropdown-item text-decoration-none pointer text-nowrap d-flex align-items-center" @click="addTreeFields">
                                <i class="fa-solid fa-fw fa-plus text-muted me-2"></i>
                                <span class="fs-d9">Tree Fields</span>
                            </a>
                        </li>
                        <li>
                            <a class="dropdown-item text-decoration-none pointer text-nowrap d-flex align-items-center" @click="addTitleNoteFields">
                                <i class="fa-solid fa-fw fa-plus text-muted me-2"></i>
                                <span class="fs-d9">Title+Note</span>
                            </a>
                        </li>
                    </ul>
                </div>

                <div class="p-0 ms-auto"></div>

            </div>
        </div>
        <div class="card-body fs-d8 p-0 scrollable">
            <table class="table table-bordered w-100 ae-table m-0 bg-white">
                <thead class="table-light">
                    <tr>
                        <th style="width:35px" class="text-center align-middle">Pk</th>
                        <th class="align-middle">Name</th>
                        <th style="width:140px" class="align-middle">DbType</th>
                        <th style="width:100px" class="text-center align-middle">Size</th>
                        <th style="width:60px" class="text-center align-middle">Default</th>
                        <th style="width:200px" class="text-center align-middle">Start/Step</th>
                        <th style="width:50px" class="text-center align-middle">Null</th>
                        <th style="width:300px" class="align-middle">Fk</th>
                        <th style="width:30px" class="text-center align-middle"></th>
                        <th style="width:32px" class="text-center align-middle"></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(i,ind) in tableDef.Columns">
                        <td class="ae-table-td text-center align-middle p-0">
                            <i class="fa fa-fw fa-key text-primary fs-d8" v-if="i.IsPrimaryKey===true"></i>
                        </td>
                        <td class="ae-table-td align-middle p-0">
                            <span class="ae-data-key" style="display:none;visibility:hidden;">{{i.Name}}</span>
                            <input type="text" class="ae-ingrid-input ae-input-field-name" :class="i.ValidationCss"
                                   v-model="i.Name" v-if="i.State!=='d'" @input="fieldNameKeyup" @keydown="handleArrowKeys" />
                            <div class="ae-ingrid-input text-decoration-line-through" v-if="i.State==='d'">{{i.Name}}</div>
                        </td>
                        <td class="ae-table-td align-middle p-0">
                            <select class="ae-ingrid-input" v-model="i.DbType" @change="typeChanged" @keydown="handleArrowKeys">
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

                                <option value="VECTOR" v-if="!i.IsPrimaryKey">VECTOR</option>

                                <option value="XML" v-if="!i.IsPrimaryKey">XML</option>
                            </select>
                        </td>
                        <td class="ae-table-td align-middle text-center p-0">
                            <input type="text" onkeypress="return isNumberKey(event)" @input="generalKeyup" @keydown="handleArrowKeys" class="ae-ingrid-input text-center" v-model="i.Size"
                                   v-if="'[DECIMAL],[NUMERIC],[DATETIME2],[DATETIMEOFFSET],[TIME],[VARCHAR],[NVARCHAR],[CHAR],[NCHAR],[VARBINARY],[VECTOR]'.indexOf('['+i.DbType+']')>-1" />
                        </td>
                        <td class="align-middle p-0">
                            <input type="text" class="ae-ingrid-input text-center ae-input-field-default" v-model="i.DbDefault" @input="generalKeyup" @keydown="handleArrowKeys" />
                        </td>
                        <td class="align-middle p-0">
                            <table class="w-100 m-0">
                                <tr>
                                    <td style="width:50%;">
                                        <input type="text" onkeypress="return isNumberKey(event)" @input="generalKeyup" @keydown="handleArrowKeys"
                                               class="ae-ingrid-input text-center ae-input-field-start"
                                               v-if="i.IsPrimaryKey===true" v-model="i.IdentityStart" />
                                    </td>
                                    <td style="width:50%;">
                                        <input type="text" onkeypress="return isNumberKey(event)" @input="generalKeyup" @keydown="handleArrowKeys"
                                               class="ae-ingrid-input text-center ae-input-field-step"
                                               v-if="i.IsPrimaryKey===true" v-model="i.IdentityStep" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td class="ae-table-td align-middle p-0 text-center">
                            <span v-if="i.IsPrimaryKey!==true">
                                <input type="checkbox" class="form-check-input" @change="generalKeyup" v-model="i.AllowNull" />
                            </span>
                            <span v-else></span>
                        </td>

                        <td class="ae-table-td align-middle p-0">
                            <div v-if="i.DbType==='INT' || i.DbType==='BIGINT' || i.DbType==='TINYINT' || i.DbType==='SMALLINT'">
                                <div class="text-primary ps-2 pointer">
                                    <div v-if="shared.fixNull(i.Fk,'')==='' || shared.fixNull(i.Fk.TargetTable,'')===''" @click="openFkEditor">
                                        <i class="text-secondary fs-d7">Define...</i>
                                    </div>
                                    <div v-else>
                                        <i @click="deleteFk" class="fa-solid fa-times text-muted hover-danger me-1" style="vertical-align:middle"></i>
                                        <span @click="openFkEditor" style="vertical-align:central">{{i.Fk.TargetTable}} :: {{i.Fk.TargetColumn}}</span>
                                    </div>
                                </div>
                            </div>
                        </td>
                        <td class="ae-table-td align-middle p-0 text-center">
                            <span @click="deleteField" v-if="i.IsPrimaryKey!==true && i.State!=='d'">
                                <i class="fa-solid fa-times pointer text-muted hover-danger"></i>
                            </span>

                            <span @click="deleteFieldUndo" v-if="i.State==='d'">
                                <i class="fa-solid fa-undo pointer text-primary hover-success"></i>
                            </span>
                        </td>
                        <td class="ae-table-td align-middle p-0 text-center">
                            <span v-if="i.State==='d'">
                                <span class="fw-bold text-danger" title="Deleted">{{i.State}}</span>
                            </span>
                            <span v-if="i.State==='u'">
                                <span class="fw-bold text-primary" title="Changed">{{i.State}}</span>
                            </span>
                            <span v-if="i.State==='n'">
                                <span class="fw-bold text-success" title="New">{{i.State}}</span>
                            </span>
                            <span v-else>
                            </span>
                        </td>

                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>

<style scoped>
.ae-ingrid-input {
    width: 100%;
    border: none;
    outline: none;
    background: transparent;
    padding: 4px;
}

.ae-ingrid-input:focus {
    outline: none;
    border: none;
    box-shadow: none;
}

select.ae-ingrid-input:focus {
    outline: none;
    border: none;
    box-shadow: none;
}

input.ae-ingrid-input:focus {
    outline: none;
    border: none;
    box-shadow: none;
}

.form-check-input:focus {
    border-color: #0d6efd;
    box-shadow: none;
}
</style>

<script>
    shared.setAppTitle(`<a href="?c=components/DbDbObjects" class="text-decoration-none"><i class="fa-solid fa-fw fa-database"></i><span>DbObjects</span></a> / `);
    shared.setAppSubTitle(`<span class="text-dark">Table Designer</span> (${getQueryString("o")})`);
    let _this = { cid: "", dbConfName: getQueryString("cnn"), tableDef: { Name: getQueryString("o"), Columns: [] }, originalColumns: [], c: null, editor: null };
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
                let rowIndex = $(event.target).parents("tr:first").index();
                let fkCol = _.find(_this.c.tableDef["Columns"], function (i) { return i.Name === fieldName; });
                let fkName = "";

                if (fixNull(fkCol["Fk"], '') === '') {
                    fkCol["Fk"] = { "TargetTable": "", "TargetColumn": "", "EnforceRelation": true };
                } else {
                    fkName = fkCol.Fk.FkName;
                }

                openComponent("components/DbFkEditor", {
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

                            _this.c.checkAndSetColState(rowIndex);

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
                let rowIndex = $(event.target).parents("tr:first").index();
                let fieldName = $(event.target).parents("tr:first").find(".ae-data-key").text();
                _.forEach(_this.c.tableDef["Columns"], function (i) {
                    if (i.Name === fieldName) {
                        _this.c.checkAndSetColState(rowIndex);
                    }
                });
            },
            addField() {
                _this.c.tableDef["Columns"].push(_this.c.getNewFieldConfig());
            },
            addAuditingFields() {
                _this.c.tableDef["Columns"].push({ "Name": "CreatedBy", "DbType": "INT", "Size": null, "AllowNull": false, "IsPrimaryKey": false, "ValidationCss": "", "State": "n" });
                _this.c.tableDef["Columns"].push({ "Name": "CreatedOn", "DbType": "DATETIME", "Size": null, "AllowNull": false, "IsPrimaryKey": false, "ValidationCss": "", "State": "n" });
                _this.c.tableDef["Columns"].push({ "Name": "UpdatedBy", "DbType": "INT", "Size": null, "AllowNull": true, "IsPrimaryKey": false, "ValidationCss": "", "State": "n" });
                _this.c.tableDef["Columns"].push({ "Name": "UpdatedOn", "DbType": "DATETIME", "Size": null, "AllowNull": true, "IsPrimaryKey": false, "ValidationCss": "", "State": "n" });
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
                let rowIndex = $(event.target).parents("tr:first").index();
                setTimeout(() => {
                    _this.c.checkAndSetColState(rowIndex);
                }, 10);
            },
            generalKeyup(event) {
                let rowIndex = $(event.target).parents("tr:first").index();
                setTimeout(() => {
                    _this.c.checkAndSetColState(rowIndex);
                }, 10);
            },
            fieldNameKeyup(event) {
                let rowIndex = $(event.target).parents("tr:first").index();
                setTimeout(() => {
                    _this.c.checkAndSetColState(rowIndex);
                }, 10);
            },
            handleArrowKeys(event) {
                if (event.key === 'ArrowUp' || event.key === 'ArrowDown') {
                    event.preventDefault();
                    
                    const currentRow = $(event.target).closest('tr');
                    const currentCell = $(event.target).closest('td');
                    const cellIndex = currentCell.index();
                    
                    let targetRow;
                    if (event.key === 'ArrowUp') {
                        targetRow = currentRow.prev('tr');
                    } else {
                        targetRow = currentRow.next('tr');
                    }
                    
                    if (targetRow.length > 0) {
                        const targetCell = targetRow.children().eq(cellIndex);
                        const targetInput = targetCell.find('input, select').first();
                        
                        if (targetInput.length > 0 && targetInput.is(':visible')) {
                            setTimeout(() => {
                                targetInput.focus();
                                if (targetInput.is('input[type="text"]')) {
                                    targetInput.select();
                                }
                            }, 0);
                        }
                    }
                }
            },
            checkAndSetColState(rowIndex) {
                const col = _this.c.tableDef.Columns[rowIndex];
                if (!col || col.State === 'n' || col.State === 'd') return;
                
                const original = _this.originalColumns[rowIndex];
                if (!original) {
                    col.State = 'u';
                    return;
                }
                
                if (_this.c.isColumnModified(col, original)) {
                    col.State = 'u';
                } else {
                    delete col.State;
                }
            },
            isColumnModified(col, original) {
                const fieldsToCompare = ['Name', 'DbType', 'Size', 'DbDefault', 'IdentityStart', 'IdentityStep'];
                
                for (let field of fieldsToCompare) {
                    const colVal = col[field];
                    const origVal = original[field];
                    
                    if (colVal != origVal) {
                        if (!(colVal == null && origVal == null)) {
                            if (!(colVal === '' && origVal == null)) {
                                if (!(colVal == null && origVal === '')) {
                                    return true;
                                }
                            }
                        }
                    }
                }
                
                const colAllowNull = !!col.AllowNull;
                const origAllowNull = !!original.AllowNull;
                if (colAllowNull !== origAllowNull) return true;
                
                const colFk = col.Fk ? JSON.stringify(col.Fk) : null;
                const origFk = original.Fk ? JSON.stringify(original.Fk) : null;
                if (colFk !== origFk) return true;
                
                return false;
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
                    _this.c.storeOriginalColumns();
                });
            },
            storeOriginalColumns() {
                _this.originalColumns = [];
                _.forEach(_this.c.tableDef.Columns, function (col) {
                    _this.originalColumns.push(JSON.parse(JSON.stringify(col)));
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
        mounted() { initVueComponent(_this); _this.c.readTableDesigne(); },
        props: { cid: String }
    }

</script>