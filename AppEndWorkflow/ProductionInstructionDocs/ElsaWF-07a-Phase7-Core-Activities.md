# Phase 7.1–7.14 — Core Activity Library

> Part of [Phase 7 — Custom Activity Library](ElsaWF-07-Phase7-Index.md)

---

## Overview

Phase 1 contains **48 core activities** across 14 essential categories. These form the foundation of the custom activity library and are already implemented. ✅

---

## 7.1 — Notification Activities

### `SendEmailActivity`
**File:** `Activities/SendEmailActivity.cs`
**Category:** `Notifications`

| Input | Type | Description |
|---|---|---|
| `To` | `string` | Recipient email address(es), comma-separated |
| `Subject` | `string` | Email subject |
| `Body` | `string` | Email body (HTML supported) |
| `Cc` | `string?` | CC recipients (optional) |
| `Bcc` | `string?` | BCC recipients (optional) |

| Output | Type | Description |
|---|---|---|
| `Success` | `bool` | Whether the email was sent successfully |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses SMTP settings from `AppEndSettings` (or a new `Smtp` section in `appsettings.json`).

---

### `SendSmsActivity`
**File:** `Activities/SendSmsActivity.cs`
**Category:** `Notifications`

| Input | Type | Description |
|---|---|---|
| `PhoneNumber` | `string` | Recipient phone number |
| `Message` | `string` | SMS text content |
| `Provider` | `string` | SMS provider name (e.g., `"Kavenegar"`, `"Ghasedak"`) |

| Output | Type | Description |
|---|---|---|
| `Success` | `bool` | Whether the SMS was sent |
| `MessageId` | `string?` | Provider's message tracking ID |
| `Error` | `string?` | Error message if failed |

**Implementation:** Calls SMS provider REST API. Provider settings (API key, sender number) read from `appsettings.json` under a new `SmsProviders` section.

---

### `SendTelegramActivity`
**File:** `Activities/SendTelegramActivity.cs`
**Category:** `Notifications`

| Input | Type | Description |
|---|---|---|
| `ChatId` | `string` | Telegram chat/group/channel ID |
| `Message` | `string` | Message text (Markdown supported) |
| `BotToken` | `string?` | Bot token (optional — falls back to settings) |
| `ParseMode` | `string` | `"Markdown"` or `"HTML"` (default: `"Markdown"`) |

| Output | Type | Description |
|---|---|---|
| `Success` | `bool` | Whether the message was sent |
| `MessageId` | `int?` | Telegram message ID |
| `Error` | `string?` | Error message if failed |

**Implementation:** Calls Telegram Bot API (`https://api.telegram.org/bot{token}/sendMessage`). Default bot token from `appsettings.json` under `Telegram.BotToken`.

---

### `SendPushNotificationActivity`
**File:** `Activities/SendPushNotificationActivity.cs`
**Category:** `Notifications`

| Input | Type | Description |
|---|---|---|
| `UserId` | `string` | Target user ID |
| `Title` | `string` | Notification title |
| `Body` | `string` | Notification body |
| `Data` | `string?` | Optional JSON payload |

| Output | Type | Description |
|---|---|---|
| `Success` | `bool` | Whether the notification was sent |
| `Error` | `string?` | Error message if failed |

**Implementation:** Integrates with AppEnd's existing notification system or Firebase Cloud Messaging (FCM).

---

## 7.2 — Database Activities

### `RunSqlQueryActivity`
**File:** `Activities/RunSqlQueryActivity.cs`
**Category:** `Database`

| Input | Type | Description |
|---|---|---|
| `DbConfName` | `string` | Database config name (e.g., `"AppDB"`) — from `AppEnd.DbServers[]` |
| `Query` | `string` | SQL SELECT query |
| `Parameters` | `string?` | JSON object of query parameters (for parameterized queries) |

| Output | Type | Description |
|---|---|---|
| `Result` | `string` | JSON array of result rows |
| `RowCount` | `int` | Number of rows returned |
| `Success` | `bool` | Whether the query executed successfully |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses `DbConf.FromSettings(dbConfName)` to get connection string, executes query via `SqlCommand`, returns results as JSON.

---

### `RunSqlCommandActivity`
**File:** `Activities/RunSqlCommandActivity.cs`
**Category:** `Database`

| Input | Type | Description |
|---|---|---|
| `DbConfName` | `string` | Database config name |
| `Command` | `string` | SQL command (INSERT / UPDATE / DELETE / EXEC) |
| `Parameters` | `string?` | JSON object of command parameters |

| Output | Type | Description |
|---|---|---|
| `AffectedRows` | `int` | Number of rows affected |
| `Success` | `bool` | Whether the command executed successfully |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses `DbConf.FromSettings(dbConfName)`, executes via `SqlCommand.ExecuteNonQuery()`.

---

## 7.3 — AppEnd Integration Activities

### `CallRpcMethodActivity`
**File:** `Activities/CallRpcMethodActivity.cs`
**Category:** `AppEnd`

| Input | Type | Description |
|---|---|---|
| `MethodName` | `string` | Name of the static RPC method (e.g., `"GetOrderById"`) |
| `InputParams` | `string?` | JSON string of input parameters |

| Output | Type | Description |
|---|---|---|
| `Result` | `string` | JSON result from the method |
| `Success` | `bool` | Whether the call succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Dynamically invokes the existing static method via reflection (same mechanism as `RpcNet` uses), allowing workflows to call any existing AppEnd business logic without duplication.

---

### `WriteLogActivity`
**File:** `Activities/WriteLogActivity.cs`
**Category:** `AppEnd`

| Input | Type | Description |
|---|---|---|
| `Level` | `string` | Log level: `"Info"`, `"Warning"`, `"Error"` |
| `Message` | `string` | Log message |
| `Source` | `string` | Source identifier (e.g., `"Workflow:order-approval"`) |
| `Details` | `string?` | Optional JSON details |

| Output | Type | Description |
|---|---|---|
| `Success` | `bool` | Whether the log was written |

**Implementation:** Uses AppEnd's existing `ExtensionsForLogging` methods to write to the logging system.

---

## 7.4 — Human Workflow Activities

### `AssignToUserActivity`
**File:** `Activities/AssignToUserActivity.cs`
**Category:** `Human Tasks`

| Input | Type | Description |
|---|---|---|
| `UserId` | `string` | Target user ID or username |
| `RoleName` | `string?` | Alternative: assign to a role (any user in role can pick up) |
| `TaskTitle` | `string` | Title shown in kartabl |
| `TaskDescription` | `string?` | Detailed description |
| `DueDate` | `DateTime?` | Optional due date |
| `Priority` | `string` | `"Low"`, `"Normal"`, `"High"`, `"Critical"` (default: `"Normal"`) |
| `ContextData` | `string?` | JSON payload with task context (order details, etc.) |

| Output | Type | Description |
|---|---|---|
| `TaskId` | `string` | Created task ID |
| `Success` | `bool` | Whether the task was created |

**Implementation:** Creates a task record accessible via `WorkflowInbox.vue` kartabl. Associates task with workflow instance for resumption.

---

### `WaitForApprovalActivity`
**File:** `Activities/WaitForApprovalActivity.cs`
**Category:** `Human Tasks`

| Input | Type | Description |
|---|---|---|
| `ApproverUserId` | `string?` | Specific approver (optional) |
| `ApproverRole` | `string?` | Role-based approval (optional) |
| `TaskTitle` | `string` | Title shown in kartabl |
| `AllowedOutcomes` | `string` | Comma-separated outcomes, e.g., `"Approved,Rejected,NeedMoreInfo"` |
| `ContextData` | `string?` | JSON data shown to approver |
| `TimeoutDays` | `int?` | Auto-escalate or auto-reject after N days |

| Output | Type | Description |
|---|---|---|
| `Outcome` | `string` | The outcome selected by the approver |
| `ApprovedBy` | `string` | User who completed the task |
| `Comment` | `string?` | Optional comment from approver |
| `CompletedAt` | `DateTime` | When the task was completed |

**Implementation:** Combines task creation (`AssignToUser`) + workflow suspension (Elsa bookmark). When the user completes the task in kartabl, the bookmark is resumed with the outcome. Supports timeout via Elsa Timer.

**This is the key activity for the kartabl (inbox) system.**

---

## 7.5 — Data & Document Activities

### `GeneratePdfActivity`
**File:** `Activities/GeneratePdfActivity.cs`
**Category:** `Documents`

| Input | Type | Description |
|---|---|---|
| `TemplateName` | `string` | PDF template name/path |
| `Data` | `string` | JSON data to fill the template |
| `OutputPath` | `string?` | File path to save (optional — returns bytes if not set) |

| Output | Type | Description |
|---|---|---|
| `FilePath` | `string?` | Path to generated file |
| `FileSize` | `long` | Size in bytes |
| `Success` | `bool` | Whether generation succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses a PDF library (e.g., QuestPDF, iTextSharp) to generate PDF from template + data.

---

### `GenerateExcelActivity`
**File:** `Activities/GenerateExcelActivity.cs`
**Category:** `Documents`

| Input | Type | Description |
|---|---|---|
| `Data` | `string` | JSON array of rows to export |
| `Columns` | `string` | JSON array of column definitions `[{Name, Title, Width}]` |
| `SheetName` | `string` | Worksheet name (default: `"Sheet1"`) |
| `OutputPath` | `string?` | File path to save (optional — returns bytes if not set) |

| Output | Type | Description |
|---|---|---|
| `FilePath` | `string?` | Path to generated file |
| `FileSize` | `long` | Size in bytes |
| `Success` | `bool` | Whether generation succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses ClosedXML or EPPlus to generate `.xlsx` file from JSON data.

---

### `TransformJsonActivity`
**File:** `Activities/TransformJsonActivity.cs`
**Category:** `Data`

| Input | Type | Description |
|---|---|---|
| `InputJson` | `string` | Source JSON string |
| `TransformExpression` | `string` | JavaScript expression for transformation |
| `MappingRules` | `string?` | JSON mapping rules (alternative to JS expression) |

| Output | Type | Description |
|---|---|---|
| `Result` | `string` | Transformed JSON output |
| `Success` | `bool` | Whether transformation succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses Elsa's built-in JavaScript engine to evaluate transformation expressions.

---

### `ValidateDataActivity`
**File:** `Activities/ValidateDataActivity.cs`
**Category:** `Data`

| Input | Type | Description |
|---|---|---|
| `Data` | `string` | JSON data to validate |
| `Rules` | `string` | JSON validation rules `[{Field, Rule, Message}]` — rules: `required`, `minLength`, `maxLength`, `regex`, `range`, `email`, `numeric` |
| `StopOnFirstError` | `bool` | If `true`, stops at first validation error (default: `false`) |

| Output | Type | Description |
|---|---|---|
| `IsValid` | `bool` | Whether all validations passed |
| `Errors` | `string` | JSON array of `{Field, Rule, Message}` for failed validations |
| `ErrorCount` | `int` | Number of validation errors |

**Implementation:** Parses rules JSON, applies each rule to the corresponding field in data. Supports nested field paths (e.g., `"Order.Customer.Email"`).

---

### `MergeJsonActivity`
**File:** `Activities/MergeJsonActivity.cs`
**Category:** `Data`

| Input | Type | Description |
|---|---|---|
| `Source` | `string` | Primary JSON object |
| `Overlay` | `string` | JSON object to merge on top |
| `ArrayMergeStrategy` | `string` | `"Replace"`, `"Concat"`, `"Union"` (default: `"Replace"`) |

| Output | Type | Description |
|---|---|---|
| `Result` | `string` | Merged JSON output |
| `Success` | `bool` | Whether merge succeeded |

**Implementation:** Deep-merges two JSON objects using `System.Text.Json`.

---

## 7.6 — HTTP & External API Activities

### `CallHttpApiActivity`
**File:** `Activities/CallHttpApiActivity.cs`
**Category:** `HTTP`

| Input | Type | Description |
|---|---|---|
| `Url` | `string` | Target URL |
| `Method` | `string` | HTTP method: `GET`, `POST`, `PUT`, `DELETE`, `PATCH` |
| `Headers` | `string?` | JSON object of headers `{"Authorization": "Bearer ...", ...}` |
| `Body` | `string?` | Request body (for POST/PUT/PATCH) |
| `ContentType` | `string` | Content type (default: `"application/json"`) |
| `TimeoutSeconds` | `int` | Request timeout in seconds (default: `30`) |

| Output | Type | Description |
|---|---|---|
| `StatusCode` | `int` | HTTP status code |
| `ResponseBody` | `string` | Response body as string |
| `ResponseHeaders` | `string` | JSON object of response headers |
| `Success` | `bool` | Whether status code is 2xx |
| `Error` | `string?` | Error message if request failed |

**Implementation:** Uses `HttpClient` to make HTTP requests. Supports retry on transient failures.

---

### `CallSoapServiceActivity`
**File:** `Activities/CallSoapServiceActivity.cs`
**Category:** `HTTP`

| Input | Type | Description |
|---|---|---|
| `WsdlUrl` | `string` | WSDL endpoint URL |
| `Action` | `string` | SOAP action name |
| `Body` | `string` | SOAP envelope body |
| `Headers` | `string?` | JSON object of HTTP headers |
| `TimeoutSeconds` | `int` | Request timeout (default: `30`) |

| Output | Type | Description |
|---|---|---|
| `ResponseXml` | `string` | Full SOAP response XML |
| `ResponseBody` | `string` | Extracted SOAP body content |
| `StatusCode` | `int` | HTTP status code |
| `Success` | `bool` | Whether call succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Sends SOAP envelope via `HttpClient` with `text/xml` content type. Many Iranian banking/government APIs still use SOAP.

---

### `DownloadFileActivity`
**File:** `Activities/DownloadFileActivity.cs`
**Category:** `HTTP`

| Input | Type | Description |
|---|---|---|
| `Url` | `string` | File download URL |
| `Headers` | `string?` | JSON headers (auth, etc.) |
| `SavePath` | `string` | Local path to save downloaded file |
| `TimeoutSeconds` | `int` | Timeout (default: `120`) |

| Output | Type | Description |
|---|---|---|
| `FilePath` | `string` | Path to saved file |
| `FileSize` | `long` | Downloaded file size in bytes |
| `ContentType` | `string` | MIME type of the file |
| `Success` | `bool` | Whether download succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses `HttpClient` with streaming download to handle large files.

---

## 7.7 — File Operation Activities

### `ReadFileActivity`
**File:** `Activities/ReadFileActivity.cs`
**Category:** `FileSystem`

| Input | Type | Description |
|---|---|---|
| `FilePath` | `string` | Path to file |
| `Encoding` | `string` | Text encoding (default: `"UTF-8"`) |

| Output | Type | Description |
|---|---|---|
| `Content` | `string` | File content as string |
| `FileSize` | `long` | File size in bytes |
| `Exists` | `bool` | Whether the file exists |
| `Success` | `bool` | Whether read succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Reads text file content using `System.IO.File.ReadAllTextAsync()`.

---

### `WriteFileActivity`
**File:** `Activities/WriteFileActivity.cs`
**Category:** `FileSystem`

| Input | Type | Description |
|---|---|---|
| `FilePath` | `string` | Path to file |
| `Content` | `string` | Content to write |
| `Mode` | `string` | `"Create"`, `"Append"`, `"Overwrite"` (default: `"Create"`) |
| `Encoding` | `string` | Text encoding (default: `"UTF-8"`) |

| Output | Type | Description |
|---|---|---|
| `FilePath` | `string` | Path to written file |
| `FileSize` | `long` | Resulting file size |
| `Success` | `bool` | Whether write succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Writes content to file using `System.IO.File.WriteAllTextAsync()` or `AppendAllTextAsync()`.

---

### `CopyMoveFileActivity`
**File:** `Activities/CopyMoveFileActivity.cs`
**Category:** `FileSystem`

| Input | Type | Description |
|---|---|---|
| `SourcePath` | `string` | Source file path |
| `DestinationPath` | `string` | Destination file path |
| `Operation` | `string` | `"Copy"` or `"Move"` |
| `Overwrite` | `bool` | Overwrite if destination exists (default: `false`) |

| Output | Type | Description |
|---|---|---|
| `DestinationPath` | `string` | Final destination path |
| `Success` | `bool` | Whether operation succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses `System.IO.File.Copy()` or `System.IO.File.Move()`.

---

### `DeleteFileActivity`
**File:** `Activities/DeleteFileActivity.cs`
**Category:** `FileSystem`

| Input | Type | Description |
|---|---|---|
| `FilePath` | `string` | Path to file to delete |
| `IgnoreIfNotExists` | `bool` | Don't fail if file doesn't exist (default: `true`) |

| Output | Type | Description |
|---|---|---|
| `Existed` | `bool` | Whether the file existed before deletion |
| `Success` | `bool` | Whether deletion succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses `System.IO.File.Delete()`.

---

### `ListFilesActivity`
**File:** `Activities/ListFilesActivity.cs`
**Category:** `FileSystem`

| Input | Type | Description |
|---|---|---|
| `DirectoryPath` | `string` | Path to directory |
| `Pattern` | `string` | Search pattern (default: `"*.*"`) |
| `Recursive` | `bool` | Search subdirectories (default: `false`) |

| Output | Type | Description |
|---|---|---|
| `Files` | `string` | JSON array of `{Name, Path, Size, LastModified}` |
| `Count` | `int` | Number of files found |
| `Success` | `bool` | Whether listing succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses `System.IO.Directory.GetFiles()` with optional `SearchOption.AllDirectories`.

---

## 7.8 — String & Text Processing Activities

### `RenderTemplateActivity`
**File:** `Activities/RenderTemplateActivity.cs`
**Category:** `Text`

| Input | Type | Description |
|---|---|---|
| `Template` | `string` | Template string with placeholders `{{FieldName}}` |
| `Data` | `string` | JSON object of replacement values |
| `MissingKeyBehavior` | `string` | `"LeaveBlank"`, `"KeepPlaceholder"`, `"Error"` (default: `"LeaveBlank"`) |

| Output | Type | Description |
|---|---|---|
| `Result` | `string` | Rendered text with values substituted |
| `MissingKeys` | `string` | JSON array of placeholder keys not found in data |
| `Success` | `bool` | Whether rendering succeeded |

**Implementation:** Parses `{{key}}` placeholders and replaces with values from data JSON. Supports nested paths like `{{Order.Customer.Name}}`.

---

### `RegexMatchActivity`
**File:** `Activities/RegexMatchActivity.cs`
**Category:** `Text`

| Input | Type | Description |
|---|---|---|
| `Input` | `string` | String to search |
| `Pattern` | `string` | Regular expression pattern |
| `Options` | `string?` | Regex options: `"IgnoreCase"`, `"Multiline"`, etc. |

| Output | Type | Description |
|---|---|---|
| `IsMatch` | `bool` | Whether the pattern matched |
| `Matches` | `string` | JSON array of matched groups |
| `MatchCount` | `int` | Number of matches |

**Implementation:** Uses `System.Text.RegularExpressions.Regex`.

---

### `FormatStringActivity`
**File:** `Activities/FormatStringActivity.cs`
**Category:** `Text`

| Input | Type | Description |
|---|---|---|
| `Format` | `string` | Format string (e.g., `"Order {0} created on {1}"`) |
| `Arguments` | `string` | JSON array of arguments |
| `Culture` | `string?` | Culture name for formatting (e.g., `"fa-IR"`, `"en-US"`) |

| Output | Type | Description |
|---|---|---|
| `Result` | `string` | Formatted string |
| `Success` | `bool` | Whether formatting succeeded |

**Implementation:** Uses `string.Format()` with optional `CultureInfo`.

---

### `ParseCsvActivity`
**File:** `Activities/ParseCsvActivity.cs`
**Category:** `Text`

| Input | Type | Description |
|---|---|---|
| `CsvContent` | `string` | CSV text content |
| `Delimiter` | `string` | Column delimiter (default: `","`) |
| `HasHeader` | `bool` | First row is header (default: `true`) |
| `Encoding` | `string` | Text encoding (default: `"UTF-8"`) |

| Output | Type | Description |
|---|---|---|
| `Result` | `string` | JSON array of row objects |
| `RowCount` | `int` | Number of data rows |
| `Columns` | `string` | JSON array of column names |
| `Success` | `bool` | Whether parsing succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Parses CSV manually or using a lightweight library. Returns structured JSON.

---

## 7.9 — Security & Cryptography Activities

### `HashDataActivity`
**File:** `Activities/HashDataActivity.cs`
**Category:** `Security`

| Input | Type | Description |
|---|---|---|
| `Data` | `string` | Data to hash |
| `Algorithm` | `string` | `"SHA256"`, `"SHA512"`, `"MD5"`, `"SHA1"` (default: `"SHA256"`) |
| `OutputFormat` | `string` | `"Hex"` or `"Base64"` (default: `"Hex"`) |

| Output | Type | Description |
|---|---|---|
| `Hash` | `string` | Hash result |
| `Algorithm` | `string` | Algorithm used |

**Implementation:** Uses `System.Security.Cryptography` hash algorithms.

---

### `EncryptDecryptActivity`
**File:** `Activities/EncryptDecryptActivity.cs`
**Category:** `Security`

| Input | Type | Description |
|---|---|---|
| `Data` | `string` | Data to encrypt or decrypt |
| `Key` | `string` | Encryption key (or key name from settings) |
| `Operation` | `string` | `"Encrypt"` or `"Decrypt"` |
| `Algorithm` | `string` | `"AES"` (default: `"AES"`) |

| Output | Type | Description |
|---|---|---|
| `Result` | `string` | Encrypted/decrypted output (Base64 for encrypted) |
| `Success` | `bool` | Whether operation succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses `System.Security.Cryptography.Aes` for AES encryption. Keys can be read from `appsettings.json` `EncryptionKeys` section.

---

### `GenerateTokenActivity`
**File:** `Activities/GenerateTokenActivity.cs`
**Category:** `Security`

| Input | Type | Description |
|---|---|---|
| `Type` | `string` | Token type: `"Guid"`, `"Random"`, `"Numeric"`, `"JWT"` |
| `Length` | `int?` | Length for `Random`/`Numeric` types (default: `32`) |
| `JwtPayload` | `string?` | JSON payload for JWT type |
| `ExpirationMinutes` | `int?` | JWT expiration (default: `60`) |

| Output | Type | Description |
|---|---|---|
| `Token` | `string` | Generated token |
| `ExpiresAt` | `DateTime?` | Expiration time (for JWT) |

**Implementation:** Uses `System.Security.Cryptography.RandomNumberGenerator` for random tokens, or `System.IdentityModel.Tokens.Jwt` for JWTs.

---

### `CheckPermissionActivity`
**File:** `Activities/CheckPermissionActivity.cs`
**Category:** `Security`

| Input | Type | Description |
|---|---|---|
| `UserId` | `string` | User to check |
| `Permission` | `string` | Permission name or resource path |
| `Action` | `string` | Action type: `"Read"`, `"Write"`, `"Delete"`, `"Execute"` |

| Output | Type | Description |
|---|---|---|
| `HasPermission` | `bool` | Whether user has the permission |
| `UserRoles` | `string` | JSON array of user's roles |

**Implementation:** Queries AppEnd's existing permission/role system to check user access.

---

## 7.10 — Collection & Array Activities

### `FilterArrayActivity`
**File:** `Activities/FilterArrayActivity.cs`
**Category:** `Collections`

| Input | Type | Description |
|---|---|---|
| `InputArray` | `string` | JSON array to filter |
| `FilterExpression` | `string` | JavaScript filter expression (e.g., `"item.Amount > 1000"`) |

| Output | Type | Description |
|---|---|---|
| `Result` | `string` | Filtered JSON array |
| `Count` | `int` | Number of items after filtering |
| `OriginalCount` | `int` | Number of items before filtering |

**Implementation:** Uses Elsa's JavaScript engine to evaluate filter expression against each element.

---

### `SortArrayActivity`
**File:** `Activities/SortArrayActivity.cs`
**Category:** `Collections`

| Input | Type | Description |
|---|---|---|
| `InputArray` | `string` | JSON array to sort |
| `SortBy` | `string` | Property name to sort by (e.g., `"CreatedDate"`) |
| `Direction` | `string` | `"Asc"` or `"Desc"` (default: `"Asc"`) |
| `SortType` | `string` | `"String"`, `"Number"`, `"Date"` (default: `"String"`) |

| Output | Type | Description |
|---|---|---|
| `Result` | `string` | Sorted JSON array |
| `Count` | `int` | Number of items |

**Implementation:** Parses JSON array, sorts using `System.Linq` with appropriate comparer.

---

### `AggregateArrayActivity`
**File:** `Activities/AggregateArrayActivity.cs`
**Category:** `Collections`

| Input | Type | Description |
|---|---|---|
| `InputArray` | `string` | JSON array to aggregate |
| `Field` | `string` | Property name to aggregate |
| `Operation` | `string` | `"Sum"`, `"Average"`, `"Min"`, `"Max"`, `"Count"`, `"Distinct"` |

| Output | Type | Description |
|---|---|---|
| `Result` | `string` | Aggregation result (as string) |
| `NumericResult` | `double?` | Numeric result (for Sum/Average/Min/Max) |
| `Count` | `int` | Count of items |

**Implementation:** Parses JSON array, applies LINQ aggregation on specified field.

---

### `GroupByActivity`
**File:** `Activities/GroupByActivity.cs`
**Category:** `Collections`

| Input | Type | Description |
|---|---|---|
| `InputArray` | `string` | JSON array to group |
| `GroupByField` | `string` | Property name to group by |
| `AggregateField` | `string?` | Property to aggregate per group (optional) |
| `AggregateOperation` | `string?` | `"Sum"`, `"Count"`, `"Average"` (optional) |

| Output | Type | Description |
|---|---|---|
| `Result` | `string` | JSON array of `{Key, Items, AggregateValue?}` |
| `GroupCount` | `int` | Number of distinct groups |

**Implementation:** Uses LINQ `GroupBy()` on parsed JSON array.

---

### `PickFromArrayActivity`
**File:** `Activities/PickFromArrayActivity.cs`
**Category:** `Collections`

| Input | Type | Description |
|---|---|---|
| `InputArray` | `string` | JSON array |
| `Operation` | `string` | `"First"`, `"Last"`, `"Random"`, `"ByIndex"` |
| `Index` | `int?` | Index for `ByIndex` operation |

| Output | Type | Description |
|---|---|---|
| `Result` | `string` | Selected item as JSON |
| `Found` | `bool` | Whether an item was found |

**Implementation:** Uses LINQ/random selection on parsed JSON array.

---

## 7.11 — Flow Control & Timing Activities

### `DelayActivity`
**File:** `Activities/DelayActivity.cs`
**Category:** `Flow Control`

| Input | Type | Description |
|---|---|---|
| `Duration` | `string` | Duration expression: `"00:05:00"` (5 min), `"1.00:00:00"` (1 day) |
| `Unit` | `string?` | Alternative: `"Seconds"`, `"Minutes"`, `"Hours"`, `"Days"` with `Value` |
| `Value` | `int?` | Numeric value when using `Unit` |

| Output | Type | Description |
|---|---|---|
| `ResumedAt` | `DateTime` | When the workflow resumed |
| `ActualDelay` | `string` | Actual delay duration |

**Implementation:** Uses Elsa's built-in Timer bookmark to suspend workflow for specified duration. Workflow state is persisted — survives app restarts.

---

### `WaitForSignalActivity`
**File:** `Activities/WaitForSignalActivity.cs`
**Category:** `Flow Control`

| Input | Type | Description |
|---|---|---|
| `SignalName` | `string` | Name of the signal to wait for |
| `TimeoutMinutes` | `int?` | Optional timeout (auto-resume with timeout outcome) |

| Output | Type | Description |
|---|---|---|
| `SignalData` | `string?` | JSON data sent with the signal |
| `ReceivedAt` | `DateTime` | When the signal was received |
| `TimedOut` | `bool` | Whether the wait timed out |

**Implementation:** Creates an Elsa bookmark. External systems (or other workflows) send the signal via `WorkflowServices.SendSignal(signalName, data)` through RPC.

---

### `ParallelForEachActivity`
**File:** `Activities/ParallelForEachActivity.cs`
**Category:** `Flow Control`

| Input | Type | Description |
|---|---|---|
| `InputArray` | `string` | JSON array of items to process |
| `MaxConcurrency` | `int` | Maximum parallel executions (default: `5`) |
| `ActivityToExecute` | `string` | Name/ID of the activity to run for each item |

| Output | Type | Description |
|---|---|---|
| `Results` | `string` | JSON array of results from each execution |
| `TotalCount` | `int` | Total items processed |
| `SuccessCount` | `int` | Items processed successfully |
| `FailedCount` | `int` | Items that failed |

**Implementation:** Executes specified activity for each item in parallel with concurrency limit using `SemaphoreSlim`.

---

### `RetryActivity`
**File:** `Activities/RetryActivity.cs`
**Category:** `Flow Control`

| Input | Type | Description |
|---|---|---|
| `MaxRetries` | `int` | Maximum number of retry attempts (default: `3`) |
| `DelaySeconds` | `int` | Delay between retries in seconds (default: `5`) |
| `BackoffMultiplier` | `double` | Exponential backoff multiplier (default: `2.0`) |
| `RetryableErrors` | `string?` | Comma-separated error types to retry on (optional — retries all if empty) |

| Output | Type | Description |
|---|---|---|
| `AttemptCount` | `int` | Number of attempts made |
| `Success` | `bool` | Whether the activity eventually succeeded |
| `LastError` | `string?` | Last error message if all retries failed |

**Implementation:** Wraps the next activity in a retry loop with configurable backoff strategy.

---

### `SwitchActivity`
**File:** `Activities/SwitchActivity.cs`
**Category:** `Flow Control`

| Input | Type | Description |
|---|---|---|
| `Expression` | `string` | Value to evaluate |
| `Cases` | `string` | JSON object of `{"value": "branchName"}` mappings |
| `DefaultBranch` | `string?` | Branch name for unmatched values |

| Output | Type | Description |
|---|---|---|
| `MatchedCase` | `string` | The case value that matched |
| `SelectedBranch` | `string` | The branch that was selected |

**Implementation:** Evaluates expression and routes to the matching branch. More readable than nested If/Else chains.

---

## 7.12 — Compression & Archive Activities

### `CompressFilesActivity`
**File:** `Activities/CompressFilesActivity.cs`
**Category:** `Archive`

| Input | Type | Description |
|---|---|---|
| `SourcePaths` | `string` | JSON array of file/directory paths to compress |
| `OutputPath` | `string` | Path for the output ZIP file |
| `CompressionLevel` | `string` | `"Fastest"`, `"Optimal"`, `"NoCompression"` (default: `"Optimal"`) |
| `Password` | `string?` | Optional ZIP password |

| Output | Type | Description |
|---|---|---|
| `FilePath` | `string` | Path to created archive |
| `FileSize` | `long` | Archive size in bytes |
| `FileCount` | `int` | Number of files included |
| `Success` | `bool` | Whether compression succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses `System.IO.Compression.ZipFile` for ZIP creation. Password protection via `SharpZipLib` if needed.

---

### `DecompressFilesActivity`
**File:** `Activities/DecompressFilesActivity.cs`
**Category:** `Archive`

| Input | Type | Description |
|---|---|---|
| `ArchivePath` | `string` | Path to ZIP archive |
| `OutputDirectory` | `string` | Directory to extract files to |
| `Password` | `string?` | Archive password (if encrypted) |
| `Overwrite` | `bool` | Overwrite existing files (default: `false`) |

| Output | Type | Description |
|---|---|---|
| `ExtractedFiles` | `string` | JSON array of extracted file paths |
| `FileCount` | `int` | Number of files extracted |
| `Success` | `bool` | Whether extraction succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses `System.IO.Compression.ZipFile.ExtractToDirectory()`.

---

## 7.13 — Math & Calculation Activities

### `EvaluateExpressionActivity`
**File:** `Activities/EvaluateExpressionActivity.cs`
**Category:** `Math`

| Input | Type | Description |
|---|---|---|
| `Expression` | `string` | Math expression (e.g., `"(Price * Quantity) - (Price * Quantity * DiscountRate)"`) |
| `Variables` | `string` | JSON object of variable values `{"Price": 50000, "Quantity": 3, "DiscountRate": 0.1}` |

| Output | Type | Description |
|---|---|---|
| `Result` | `double` | Calculation result |
| `FormattedResult` | `string` | Formatted result string |
| `Success` | `bool` | Whether evaluation succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses Elsa's JavaScript engine to evaluate mathematical expressions with variable substitution.

---

### `ConvertCurrencyActivity`
**File:** `Activities/ConvertCurrencyActivity.cs`
**Category:** `Math`

| Input | Type | Description |
|---|---|---|
| `Amount` | `decimal` | Amount to convert |
| `FromCurrency` | `string` | Source currency code (e.g., `"IRR"`, `"USD"`) |
| `ToCurrency` | `string` | Target currency code |
| `Rate` | `decimal?` | Manual rate (optional — uses API if not set) |
| `RateApiUrl` | `string?` | Custom rate API URL (optional) |

| Output | Type | Description |
|---|---|---|
| `ConvertedAmount` | `decimal` | Converted amount |
| `RateUsed` | `decimal` | Exchange rate used |
| `Success` | `bool` | Whether conversion succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Supports manual rate or fetches live rate from a configurable API endpoint.

---

## 7.14 — Caching Activities

### `SetCacheActivity`
**File:** `Activities/SetCacheActivity.cs`
**Category:** `Cache`

| Input | Type | Description |
|---|---|---|
| `Key` | `string` | Cache key |
| `Value` | `string` | Value to cache (JSON or string) |
| `ExpirationMinutes` | `int?` | Expiration time in minutes (optional — no expiration if not set) |

| Output | Type | Description |
|---|---|---|
| `Success` | `bool` | Whether value was cached |

**Implementation:** Uses `IMemoryCache` (or AppEnd's existing cache mechanism if available).

---

### `GetCacheActivity`
**File:** `Activities/GetCacheActivity.cs`
**Category:** `Cache`

| Input | Type | Description |
|---|---|---|
| `Key` | `string` | Cache key |
| `DefaultValue` | `string?` | Value to return if key not found |

| Output | Type | Description |
|---|---|---|
| `Value` | `string?` | Cached value |
| `Found` | `bool` | Whether the key was found in cache |

**Implementation:** Reads from `IMemoryCache`.

---

### `RemoveCacheActivity`
**File:** `Activities/RemoveCacheActivity.cs`
**Category:** `Cache`

| Input | Type | Description |
|---|---|---|
| `Key` | `string` | Cache key to remove |
| `Pattern` | `string?` | Pattern to match multiple keys (e.g., `"workflow:*"`) |

| Output | Type | Description |
|---|---|---|
| `RemovedCount` | `int` | Number of keys removed |
| `Success` | `bool` | Whether removal succeeded |

**Implementation:** Removes entries from `IMemoryCache`. Pattern-based removal iterates registered keys.

---

## Summary of Core Activities

**Total: 48 activities across 14 categories** ✅

| # | Category | Activities |
|---|---|---|
| 7.1 | Notifications | 4 |
| 7.2 | Database | 2 |
| 7.3 | AppEnd Integration | 2 |
| 7.4 | Human Tasks | 2 |
| 7.5 | Data & Documents | 5 |
| 7.6 | HTTP & APIs | 3 |
| 7.7 | File Operations | 5 |
| 7.8 | String & Text | 4 |
| 7.9 | Security & Crypto | 4 |
| 7.10 | Collections & Arrays | 5 |
| 7.11 | Flow Control | 5 |
| 7.12 | Compression | 2 |
| 7.13 | Math & Calculation | 2 |
| 7.14 | Caching | 3 |

---

## Implementation Pattern

All activities follow this consistent pattern:

```csharp
using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Models;
using System.ComponentModel;

namespace AppEndWorkflow.Activities;

[DisplayName("Activity Name")]
[Category("Category Name")]
[Description("Brief description of what this activity does.")]
public class ActivityNameActivity : CodeActivity<ActivityResultType>
{
    [Input(Description = "Input parameter description")]
    public Input<string> ParameterName { get; set; } = default!;

    protected override void Execute(ActivityExecutionContext context)
    {
        var parameter = context.Get(ParameterName)!;
        
        // Implementation logic here
        
        context.SetResult(new ActivityResultType { Success = true });
    }
}
```

> **Auto-discovery:** All activities in the `AppEndWorkflow.Activities` namespace are automatically
> registered by Elsa and appear in the Activity Browser.

---

**Status:** ✅ Phase 1 Complete — All 48 core activities implemented and tested.

**Next:** Review [Phase 2 Extended Activities](ElsaWF-07b-Phase7-Extended-Activities.md)
