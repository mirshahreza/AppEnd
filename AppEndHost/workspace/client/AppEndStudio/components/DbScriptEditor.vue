<template>
    <div class="card h-100 rounded rounded-2 rounded-bottom-0 rounded-end-0 bg-transparent border-0">
        <div class="card-header p-2 bg-light-subtle rounded-end-0 border-0">
            <div class="input-group input-group-sm border-0 bg-transparent">
                <button class="btn btn-primary rounded-1" @click="executeSqlScript"><i class="fa-solid fa-play"></i> Execute</button>
                <input type="text" class="form-control form-control-sm border-0 rounded-0 bg-transparent" disabled />
            </div>
        </div>
        <div class="card-body p-2">
            <div class="card h-100 border-light bg-light bg-opacity-75 border-0">
                <div class="card-body rounded rounded-2 border border-3 border-light fs-d8 p-0 bg-transparent">
                    <div class="code-editor-container h-100" id="sqlScriptEditor"></div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    shared.setAppTitle("Sql Script Editor");
    shared.setAppSubTitle(getQueryString("o"));
    let _this = { cid: "", c: null, dbConfName: "", objectName: "", sqlScript: "", editor: null };
    _this.dbConfName = getQueryString("cnn");
    _this.objectName = getQueryString("o");

    export default {
        methods: {
            executeSqlScript() {
                let sqlScript = _this.editor.getValue().trim();
                let scriptName = getSqlScriptName(sqlScript);
                if (scriptName === "" || scriptName.indexOf(' ') > -1) {
                    showError("Syntax error : A valid object name can not be found in the script body !!!");
                    return;
                }
                rpcAEP("AlterObjectScript", { "DbConfName": _this.dbConfName, "ObjectScript": sqlScript },
                    function (res) {
                        _this.sqlScript = sqlScript;
                        setQueryString("o", scriptName);
                        removeQueryString("template");
                        shared.setAppSubTitle(scriptName);
                    },
                    function res(res) {
                        showError(res.Message);
                    }
                );
            },
            readScript() {
                if (_this.objectName === "__new__") {
                    if (getQueryString("template") === "View") {
                        _this.sqlScript = "CREATE OR ALTER VIEW [DBO].[YourNewViewName] AS\r\n SELECT 1 A; ";
                    }
                    if (getQueryString("template") === "Procedure") {
                        _this.sqlScript = "CREATE OR ALTER PROCEDURE [DBO].[YourNewProcedureName] \r\n@Param1 INT\r\nAS\r\nBEGIN\r\n SELECT @Param1 A;\r\nEND";
                    }
                    if (getQueryString("template") === "ScalarFunction") {
                        _this.sqlScript = "CREATE OR ALTER FUNCTION [DBO].[YourNewScalarFunctionName] \r\n(@Param1 INT, @Param2 INT) \r\nRETURNS INT\r\nBEGIN\r\n RETURN 1;\r\nEND";
                    }
                    if (getQueryString("template") === "TableFunction") {
                        _this.sqlScript = "CREATE OR ALTER FUNCTION [DBO].[YourNewTableFunctionName] \r\n(@Param1 INT, @Param2 INT) \r\nRETURNS TABLE\r\nAS RETURN\r\n(\r\n SELECT @Param1 A, @Param2 B;\r\n)";
                    }
                    _this.c.setupEditor();
                } else {
                    rpcAEP("GetCreateOrAlterTable", { "DbConfName": _this.dbConfName, "ObjectName": _this.objectName }, function (res) {
                        _this.sqlScript = R0R(res);
                        _this.c.setupEditor();
                    });
                }
            },
            setupEditor() {
                _this.editor = ace.edit("sqlScriptEditor", {
                    theme: "ace/theme/cloud9_day",
                    mode: "ace/mode/sqlserver",
                    value: _this.sqlScript
                });
            }
        },
        setup(props) {
            _this.cid = props['cid'];
        },
        data() { return _this; },
        created() { _this.c = this; },
        mounted() { _this.c.readScript(); },
        props: { cid: String }
    }

</script>