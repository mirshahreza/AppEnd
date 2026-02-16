# Phase 7.15–7.27 — Extended Activity Library

> Part of [Phase 7 — Custom Activity Library](ElsaWF-07-Phase7-Index.md)

---

## Overview

Phase 2 contains **35 extended activities** across 13 specialized categories. These activities extend core functionality and add support for important integrations.

---

## 7.15 — Version Control Activities

### `GitCloneRepositoryActivity`
**File:** `Activities/GitCloneRepositoryActivity.cs`
**Category:** `Version Control`

| Input | Type | Description |
|---|---|---|
| `RepositoryUrl` | `string` | Git repository URL |
| `OutputPath` | `string` | Local path to clone into |
| `Branch` | `string?` | Branch name (optional, defaults to default branch) |
| `Username` | `string?` | Git username (for private repos) |
| `Password` | `string?` | Git password/token (for private repos) |

| Output | Type | Description |
|---|---|---|
| `LocalPath` | `string` | Path to cloned repository |
| `Success` | `bool` | Whether clone succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses `LibGit2Sharp` for Git operations.

---

### `GitCommitChangesActivity`
**File:** `Activities/GitCommitChangesActivity.cs`
**Category:** `Version Control`

| Input | Type | Description |
|---|---|---|
| `RepositoryPath` | `string` | Local repository path |
| `FilesToCommit` | `string` | JSON array of file paths (or `"*"` for all) |
| `CommitMessage` | `string` | Commit message |
| `AuthorName` | `string` | Commit author name |
| `AuthorEmail` | `string` | Commit author email |

| Output | Type | Description |
|---|---|---|
| `CommitHash` | `string` | Commit hash/SHA |
| `Success` | `bool` | Whether commit succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Stages files and commits using `LibGit2Sharp`.

---

### `GitPushChangesActivity`
**File:** `Activities/GitPushChangesActivity.cs`
**Category:** `Version Control`

| Input | Type | Description |
|---|---|---|
| `RepositoryPath` | `string` | Local repository path |
| `RemoteName` | `string` | Remote name (default: `"origin"`) |
| `BranchName` | `string` | Branch to push |
| `Username` | `string?` | Git username |
| `Password` | `string?` | Git password/token |

| Output | Type | Description |
|---|---|---|
| `Success` | `bool` | Whether push succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Pushes commits to remote repository.

---

### `GitPullChangesActivity`
**File:** `Activities/GitPullChangesActivity.cs`
**Category:** `Version Control`

| Input | Type | Description |
|---|---|---|
| `RepositoryPath` | `string` | Local repository path |
| `RemoteName` | `string` | Remote name (default: `"origin"`) |
| `BranchName` | `string` | Branch to pull |
| `Username` | `string?` | Git username |
| `Password` | `string?` | Git password/token |

| Output | Type | Description |
|---|---|---|
| `Success` | `bool` | Whether pull succeeded |
| `UpdatedFiles` | `int` | Number of files updated |
| `Error` | `string?` | Error message if failed |

**Implementation:** Pulls changes from remote repository.

---

## 7.16 — File Transfer Activities

### `FtpUploadFileActivity`
**File:** `Activities/FtpUploadFileActivity.cs`
**Category:** `File Transfer`

| Input | Type | Description |
|---|---|---|
| `Host` | `string` | FTP server hostname |
| `Port` | `int` | FTP port (default: `21`) |
| `Username` | `string` | FTP username |
| `Password` | `string` | FTP password |
| `LocalPath` | `string` | Local file path |
| `RemotePath` | `string` | Remote FTP path |
| `Passive` | `bool` | Passive mode (default: `true`) |

| Output | Type | Description |
|---|---|---|
| `RemotePath` | `string` | Full remote path |
| `FileSize` | `long` | Uploaded file size |
| `Success` | `bool` | Whether upload succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses `FluentFTP` library.

---

### `FtpDownloadFileActivity`
**File:** `Activities/FtpDownloadFileActivity.cs`
**Category:** `File Transfer`

| Input | Type | Description |
|---|---|---|
| `Host` | `string` | FTP server hostname |
| `Port` | `int` | FTP port (default: `21`) |
| `Username` | `string` | FTP username |
| `Password` | `string` | FTP password |
| `RemotePath` | `string` | Remote file path |
| `LocalPath` | `string` | Local save path |

| Output | Type | Description |
|---|---|---|
| `LocalPath` | `string` | Path to downloaded file |
| `FileSize` | `long` | Downloaded file size |
| `Success` | `bool` | Whether download succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Downloads file from FTP server.

---

### `SftpUploadFileActivity`
**File:** `Activities/SftpUploadFileActivity.cs`
**Category:** `File Transfer`

| Input | Type | Description |
|---|---|---|
| `Host` | `string` | SFTP server hostname |
| `Port` | `int` | SFTP port (default: `22`) |
| `Username` | `string` | SFTP username |
| `Password` | `string?` | SFTP password (optional if using key) |
| `PrivateKeyPath` | `string?` | Path to private key file |
| `LocalPath` | `string` | Local file path |
| `RemotePath` | `string` | Remote SFTP path |

| Output | Type | Description |
|---|---|---|
| `RemotePath` | `string` | Full remote path |
| `FileSize` | `long` | Uploaded file size |
| `Success` | `bool` | Whether upload succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses `SSH.NET` library for SFTP.

---

### `SftpDownloadFileActivity`
**File:** `Activities/SftpDownloadFileActivity.cs`
**Category:** `File Transfer`

| Input | Type | Description |
|---|---|---|
| `Host` | `string` | SFTP server hostname |
| `Port` | `int` | SFTP port (default: `22`) |
| `Username` | `string` | SFTP username |
| `Password` | `string?` | SFTP password (optional if using key) |
| `PrivateKeyPath` | `string?` | Path to private key file |
| `RemotePath` | `string` | Remote file path |
| `LocalPath` | `string` | Local save path |

| Output | Type | Description |
|---|---|---|
| `LocalPath` | `string` | Path to downloaded file |
| `FileSize` | `long` | Downloaded file size |
| `Success` | `bool` | Whether download succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Downloads file from SFTP server.

---

## 7.17 — Email Advanced Activities

### `SendBulkEmailActivity`
**File:** `Activities/SendBulkEmailActivity.cs`
**Category:** `Email`

| Input | Type | Description |
|---|---|---|
| `Recipients` | `string` | JSON array of recipient objects `[{Email, Name, Data}]` |
| `TemplateName` | `string` | Email template name |
| `Subject` | `string` | Email subject (supports placeholders) |
| `BatchSize` | `int` | Number of emails per batch (default: `100`) |
| `DelayMs` | `int` | Delay between batches in ms (default: `1000`) |

| Output | Type | Description |
|---|---|---|
| `SentCount` | `int` | Number of emails sent successfully |
| `FailedCount` | `int` | Number of failed sends |
| `Success` | `bool` | Whether operation completed |
| `Error` | `string?` | Error message if failed |

**Implementation:** Sends bulk emails with template support and rate limiting.

---

### `SendEmailWithAttachmentsActivity`
**File:** `Activities/SendEmailWithAttachmentsActivity.cs`
**Category:** `Email`

| Input | Type | Description |
|---|---|---|
| `To` | `string` | Recipient email address(es) |
| `Subject` | `string` | Email subject |
| `Body` | `string` | Email body |
| `Attachments` | `string` | JSON array of `{FilePath, ContentType?, FileName?}` |

| Output | Type | Description |
|---|---|---|
| `Success` | `bool` | Whether email was sent |
| `AttachmentCount` | `int` | Number of attachments sent |
| `Error` | `string?` | Error message if failed |

**Implementation:** Sends email with file attachments.

---

### `ReceiveEmailActivity`
**File:** `Activities/ReceiveEmailActivity.cs`
**Category:** `Email`

| Input | Type | Description |
|---|---|---|
| `ImapServer` | `string` | IMAP server address |
| `Port` | `int` | IMAP port (default: `993`) |
| `Username` | `string` | Email account username |
| `Password` | `string` | Email account password |
| `FolderName` | `string` | Folder name (default: `"INBOX"`) |
| `ReadUnread` | `bool` | Read only unread emails (default: `true`) |
| `MaxEmails` | `int` | Maximum emails to fetch (default: `10`) |

| Output | Type | Description |
|---|---|---|
| `Emails` | `string` | JSON array of email objects |
| `EmailCount` | `int` | Number of emails received |
| `Success` | `bool` | Whether operation succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Connects to IMAP server and retrieves emails.

---

## 7.18 — Advanced Notifications Activities

### `SendWhatsappActivity`
**File:** `Activities/SendWhatsappActivity.cs`
**Category:** `Notifications`

| Input | Type | Description |
|---|---|---|
| `PhoneNumber` | `string` | Recipient WhatsApp number (with country code) |
| `Message` | `string` | Message text |
| `Provider` | `string` | WhatsApp provider: `"Twilio"`, `"Meta"`, etc. |
| `MediaUrl` | `string?` | Media URL (optional image/video) |
| `MediaType` | `string?` | Media type: `"image"`, `"video"`, `"document"` |

| Output | Type | Description |
|---|---|---|
| `MessageId` | `string` | Message ID |
| `Success` | `bool` | Whether message was sent |
| `Error` | `string?` | Error message if failed |

**Implementation:** Sends WhatsApp messages via provider API.

---

### `SendSlackActivity`
**File:** `Activities/SendSlackActivity.cs`
**Category:** `Notifications`

| Input | Type | Description |
|---|---|---|
| `ChannelId` | `string` | Slack channel ID or name |
| `Message` | `string` | Message text (supports Markdown) |
| `BotToken` | `string?` | Bot token (falls back to settings) |
| `Blocks` | `string?` | JSON Slack blocks for rich formatting |
| `ThreadTs` | `string?` | Thread timestamp for replies |

| Output | Type | Description |
|---|---|---|
| `MessageTs` | `string` | Message timestamp |
| `Success` | `bool` | Whether message was sent |
| `Error` | `string?` | Error message if failed |

**Implementation:** Sends Slack messages via Bot API.

---

## 7.19 — PDF Advanced Activities

### `MergePdfActivity`
**File:** `Activities/MergePdfActivity.cs`
**Category:** `Documents`

| Input | Type | Description |
|---|---|---|
| `InputFiles` | `string` | JSON array of PDF file paths |
| `OutputPath` | `string` | Output PDF file path |
| `Compress` | `bool` | Compress output (default: `false`) |

| Output | Type | Description |
|---|---|---|
| `OutputPath` | `string` | Path to merged PDF |
| `PageCount` | `int` | Total pages in merged PDF |
| `FileSize` | `long` | Output file size |
| `Success` | `bool` | Whether merge succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses iTextSharp or PdfSharp to merge PDFs.

---

### `ExtractPdfTextActivity`
**File:** `Activities/ExtractPdfTextActivity.cs`
**Category:** `Documents`

| Input | Type | Description |
|---|---|---|
| `PdfPath` | `string` | Path to PDF file |
| `StartPage` | `int?` | Start page number (default: 1) |
| `EndPage` | `int?` | End page number (optional) |
| `IncludeFormatting` | `bool` | Preserve text formatting (default: `false`) |

| Output | Type | Description |
|---|---|---|
| `Text` | `string` | Extracted text content |
| `PageCount` | `int` | Total pages in PDF |
| `Success` | `bool` | Whether extraction succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Extracts text from PDF using iTextSharp.

---

### `AddWatermarkPdfActivity`
**File:** `Activities/AddWatermarkPdfActivity.cs`
**Category:** `Documents`

| Input | Type | Description |
|---|---|---|
| `InputPath` | `string` | Input PDF path |
| `OutputPath` | `string` | Output PDF path |
| `WatermarkText` | `string` | Watermark text |
| `Opacity` | `float` | Opacity (0-1, default: 0.5) |
| `Angle` | `float` | Rotation angle (default: `45`) |
| `FontSize` | `int` | Font size (default: `80`) |
| `AllPages` | `bool` | Apply to all pages (default: `true`) |

| Output | Type | Description |
|---|---|---|
| `OutputPath` | `string` | Path to watermarked PDF |
| `Success` | `bool` | Whether watermarking succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Adds text watermark to PDF.

---

## 7.20 — Data Conversion Activities

### `ConvertJsonToXmlActivity`
**File:** `Activities/ConvertJsonToXmlActivity.cs`
**Category:** `Data Conversion`

| Input | Type | Description |
|---|---|---|
| `InputJson` | `string` | JSON input |
| `RootElementName` | `string` | XML root element name (default: `"root"`) |
| `AttributePrefix` | `string` | Prefix for attributes (default: `"@"`) |

| Output | Type | Description |
|---|---|---|
| `OutputXml` | `string` | Converted XML string |
| `Success` | `bool` | Whether conversion succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Converts JSON to XML structure.

---

### `ConvertXmlToJsonActivity`
**File:** `Activities/ConvertXmlToJsonActivity.cs`
**Category:** `Data Conversion`

| Input | Type | Description |
|---|---|---|
| `InputXml` | `string` | XML input |
| `PreserveAttributes` | `bool` | Keep XML attributes (default: `true`) |

| Output | Type | Description |
|---|---|---|
| `OutputJson` | `string` | Converted JSON string |
| `Success` | `bool` | Whether conversion succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Converts XML to JSON structure.

---

## 7.21 — Database Advanced Activities

### `BackupDatabaseActivity`
**File:** `Activities/BackupDatabaseActivity.cs`
**Category:** `Database`

| Input | Type | Description |
|---|---|---|
| `DbConfName` | `string` | Database config name |
| `OutputPath` | `string` | Backup file output path |
| `BackupType` | `string` | `"Full"`, `"Differential"`, `"Log"` (default: `"Full"`) |
| `Compress` | `bool` | Compress backup (default: `false`) |

| Output | Type | Description |
|---|---|---|
| `BackupPath` | `string` | Path to backup file |
| `FileSize` | `long` | Backup file size |
| `Success` | `bool` | Whether backup succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Creates database backup using native SQL Server tools.

---

### `RestoreDatabaseActivity`
**File:** `Activities/RestoreDatabaseActivity.cs`
**Category:** `Database`

| Input | Type | Description |
|---|---|---|
| `DbConfName` | `string` | Target database config name |
| `BackupPath` | `string` | Path to backup file |
| `RestoreName` | `string?` | Database name after restore (optional) |
| `Overwrite` | `bool` | Overwrite existing database (default: `false`) |
| `VerifyBackup` | `bool` | Verify backup before restore (default: `true`) |

| Output | Type | Description |
|---|---|---|
| `RestoredDbName` | `string` | Restored database name |
| `Success` | `bool` | Whether restore succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Restores database from backup file.

---

## 7.22 — Scheduling Activities

### `ScheduleWorkflowActivity`
**File:** `Activities/ScheduleWorkflowActivity.cs`
**Category:** `Scheduling`

| Input | Type | Description |
|---|---|---|
| `WorkflowName` | `string` | Workflow name/ID to schedule |
| `ScheduleExpression` | `string` | Cron expression (e.g., `"0 9 * * MON"` for 9 AM Mondays) |
| `Input` | `string?` | JSON input for scheduled workflow |
| `TimeZone` | `string` | Timezone (default: `"UTC"`) |
| `MaxOccurrences` | `int?` | Max run count (optional, unlimited if not set) |

| Output | Type | Description |
|---|---|---|
| `ScheduledId` | `string` | Scheduled workflow ID |
| `NextRun` | `DateTime` | Next scheduled run time |
| `Success` | `bool` | Whether scheduling succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Registers workflow for periodic execution.

---

### `CancelScheduledWorkflowActivity`
**File:** `Activities/CancelScheduledWorkflowActivity.cs`
**Category:** `Scheduling`

| Input | Type | Description |
|---|---|---|
| `ScheduledId` | `string` | ID of scheduled workflow to cancel |

| Output | Type | Description |
|---|---|---|
| `Success` | `bool` | Whether cancellation succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Cancels a scheduled workflow.

---

## 7.23 — Imaging Activities

### `GenerateQrCodeActivity`
**File:** `Activities/GenerateQrCodeActivity.cs`
**Category:** `Imaging`

| Input | Type | Description |
|---|---|---|
| `Content` | `string` | Content to encode in QR code |
| `Size` | `int` | QR code size in pixels (default: `200`) |
| `ErrorCorrection` | `string` | Error correction level: `"L"`, `"M"`, `"H"`, `"Q"` (default: `"M"`) |
| `OutputPath` | `string?` | File path to save (PNG format) |

| Output | Type | Description |
|---|---|---|
| `ImagePath` | `string?` | Path to generated QR code image |
| `ImageBytes` | `string` | Base64-encoded image data |
| `Success` | `bool` | Whether generation succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Generates QR code using `QRCoder` library.

---

### `ReadBarcodeActivity`
**File:** `Activities/ReadBarcodeActivity.cs`
**Category:** `Imaging`

| Input | Type | Description |
|---|---|---|
| `ImagePath` | `string` | Path to barcode/QR code image |
| `BarcodeType` | `string?` | Expected barcode type (optional) |

| Output | Type | Description |
|---|---|---|
| `Value` | `string` | Decoded barcode/QR content |
| `BarcodeType` | `string` | Type of barcode detected |
| `Success` | `bool` | Whether decoding succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Reads barcodes and QR codes using ZXing library.

---

### `ExtractTextFromImageActivity`
**File:** `Activities/ExtractTextFromImageActivity.cs`
**Category:** `Imaging`

| Input | Type | Description |
|---|---|---|
| `ImagePath` | `string` | Path to image file |
| `Language` | `string` | OCR language (e.g., `"eng"`, `"fas"`, `"deu"`) |
| `PreProcess` | `bool` | Apply preprocessing (default: `true`) |

| Output | Type | Description |
|---|---|---|
| `Text` | `string` | Extracted text |
| `Confidence` | `double` | OCR confidence score (0-1) |
| `Success` | `bool` | Whether extraction succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Extracts text from images using Tesseract OCR.

---

### `ConvertImageToPdfActivity`
**File:** `Activities/ConvertImageToPdfActivity.cs`
**Category:** `Imaging`

| Input | Type | Description |
|---|---|---|
| `ImagePaths` | `string` | JSON array of image file paths |
| `OutputPath` | `string` | Output PDF path |
| `PageSize` | `string` | Page size: `"A4"`, `"Letter"`, etc. (default: `"A4"`) |

| Output | Type | Description |
|---|---|---|
| `OutputPath` | `string` | Path to created PDF |
| `PageCount` | `int` | Number of pages |
| `Success` | `bool` | Whether conversion succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Converts images to PDF document.

---

## 7.24 — Monitoring Activities

### `SendToElasticsearchActivity`
**File:** `Activities/SendToElasticsearchActivity.cs`
**Category:** `Monitoring`

| Input | Type | Description |
|---|---|---|
| `Url` | `string` | Elasticsearch endpoint URL |
| `IndexName` | `string` | Index name (default: `"app-logs"`) |
| `Document` | `string` | JSON document to index |
| `DocumentId` | `string?` | Document ID (auto-generated if not set) |
| `Username` | `string?` | Basic auth username (optional) |
| `Password` | `string?` | Basic auth password (optional) |

| Output | Type | Description |
|---|---|---|
| `DocumentId` | `string` | Indexed document ID |
| `Success` | `bool` | Whether indexing succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Sends log events to Elasticsearch.

---

### `SendToApplicationInsightsActivity`
**File:** `Activities/SendToApplicationInsightsActivity.cs`
**Category:** `Monitoring`

| Input | Type | Description |
|---|---|---|
| `InstrumentationKey` | `string` | Application Insights instrumentation key |
| `EventName` | `string` | Event name |
| `Properties` | `string?` | JSON event properties |
| `Metrics` | `string?` | JSON event metrics |

| Output | Type | Description |
|---|---|---|
| `Success` | `bool` | Whether event was sent |
| `Error` | `string?` | Error message if failed |

**Implementation:** Sends telemetry to Azure Application Insights.

---

## 7.25 — Advanced Archive Activities

### `RarCompressActivity`
**File:** `Activities/RarCompressActivity.cs`
**Category:** `Archive`

| Input | Type | Description |
|---|---|---|
| `SourcePaths` | `string` | JSON array of paths to compress |
| `OutputPath` | `string` | Output RAR file path |
| `CompressionRatio` | `int` | Compression ratio (0-5, default: `3`) |
| `Password` | `string?` | RAR password (optional) |

| Output | Type | Description |
|---|---|---|
| `FilePath` | `string` | Path to created archive |
| `FileSize` | `long` | Archive file size |
| `Success` | `bool` | Whether compression succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Creates RAR archive.

---

### `SevenZipCompressActivity`
**File:** `Activities/SevenZipCompressActivity.cs`
**Category:** `Archive`

| Input | Type | Description |
|---|---|---|
| `SourcePaths` | `string` | JSON array of paths to compress |
| `OutputPath` | `string` | Output 7z file path |
| `CompressionLevel` | `string` | Level: `"None"`, `"Fast"`, `"Normal"`, `"Maximum"` (default: `"Normal"`) |
| `Password` | `string?` | 7z password (optional) |

| Output | Type | Description |
|---|---|---|
| `FilePath` | `string` | Path to created archive |
| `FileSize` | `long` | Archive file size |
| `Success` | `bool` | Whether compression succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Creates 7z archive.

---

## 7.27 — AI & LLM Activities ⭐

### `ChatWithLlmActivity`
**File:** `Activities/ChatWithLlmActivity.cs`
**Category:** `AI/LLM`

| Input | Type | Description |
|---|---|---|
| `Message` | `string` | User message/prompt |
| `Model` | `string` | LLM model (e.g., `"gpt-4"`, `"gpt-3.5-turbo"`, `"claude-3"`) |
| `Provider` | `string` | Provider: `"OpenAI"`, `"Anthropic"`, `"Azure"`, `"Ollama"` |
| `ApiKey` | `string?` | API key (falls back to settings) |
| `Temperature` | `float?` | Randomness (0-1, default: `0.7`) |
| `MaxTokens` | `int?` | Max response tokens |
| `SystemPrompt` | `string?` | System instruction |
| `Context` | `string?` | JSON conversation history for context |

| Output | Type | Description |
|---|---|---|
| `Response` | `string` | LLM response message |
| `TokensUsed` | `int?` | Tokens consumed |
| `Success` | `bool` | Whether request succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Calls LLM API for chat interactions.

---

### `SummarizeActivity`
**File:** `Activities/SummarizeActivity.cs`
**Category:** `AI/LLM`

| Input | Type | Description |
|---|---|---|
| `Content` | `string` | Text to summarize |
| `MaxLength` | `int?` | Maximum summary length in characters |
| `Language` | `string?` | Content language (for better summarization) |
| `Provider` | `string` | LLM provider (default: `"OpenAI"`) |
| `Model` | `string?` | Model name (default: `"gpt-3.5-turbo"`) |

| Output | Type | Description |
|---|---|---|
| `Summary` | `string` | Generated summary |
| `OriginalLength` | `int` | Original text length |
| `SummaryLength` | `int` | Summary length |
| `Success` | `bool` | Whether summarization succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses LLM to generate concise summary.

---

### `TranslateActivity`
**File:** `Activities/TranslateActivity.cs`
**Category:** `AI/LLM`

| Input | Type | Description |
|---|---|---|
| `Text` | `string` | Text to translate |
| `SourceLanguage` | `string` | Source language (e.g., `"fa"`, `"en"`) |
| `TargetLanguage` | `string` | Target language (e.g., `"en"`, `"de"`) |
| `Provider` | `string` | Provider: `"OpenAI"`, `"GoogleTranslate"`, `"DeepL"` |

| Output | Type | Description |
|---|---|---|
| `TranslatedText` | `string` | Translated text |
| `DetectedLanguage` | `string?` | Auto-detected source language |
| `Success` | `bool` | Whether translation succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Calls translation API or LLM.

---

### `GenerateContentActivity`
**File:** `Activities/GenerateContentActivity.cs`
**Category:** `AI/LLM`

| Input | Type | Description |
|---|---|---|
| `Prompt` | `string` | Content generation prompt |
| `ContentType` | `string` | Type: `"Article"`, `"Email"`, `"BlogPost"`, `"SocialPost"`, `"Code"` |
| `Tone` | `string?` | Tone: `"Professional"`, `"Casual"`, `"Formal"` |
| `Length` | `string?` | Length: `"Short"`, `"Medium"`, `"Long"` |
| `Provider` | `string` | LLM provider |
| `Model` | `string?` | Model name |

| Output | Type | Description |
|---|---|---|
| `GeneratedContent` | `string` | Generated content |
| `Success` | `bool` | Whether generation succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses LLM to generate various content types.

---

### `ExtractEntitiesActivity`
**File:** `Activities/ExtractEntitiesActivity.cs`
**Category:** `AI/LLM`

| Input | Type | Description |
|---|---|---|
| `Text` | `string` | Text to analyze |
| `EntityTypes` | `string` | JSON array of entity types (e.g., `["PERSON", "ORG", "LOCATION", "DATE"]`) |
| `Provider` | `string` | Provider: `"OpenAI"`, `"NLP.js"`, `"spaCy"` |

| Output | Type | Description |
|---|---|---|
| `Entities` | `string` | JSON array of extracted entities |
| `EntityCount` | `int` | Number of entities found |
| `Success` | `bool` | Whether extraction succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Extracts named entities from text using NLP.

---

### `AnalyzeSentimentActivity`
**File:** `Activities/AnalyzeSentimentActivity.cs`
**Category:** `AI/LLM`

| Input | Type | Description |
|---|---|---|
| `Text` | `string` | Text to analyze |
| `Language` | `string?` | Language code (e.g., `"en"`, `"fa"`) |
| `Provider` | `string` | Provider: `"Azure"`, `"AWS"`, `"OpenAI"` |

| Output | Type | Description |
|---|---|---|
| `Sentiment` | `string` | Sentiment: `"Positive"`, `"Negative"`, `"Neutral"` |
| `Score` | `double` | Confidence score (0-1) |
| `Emotions` | `string?` | JSON detected emotions |
| `Success` | `bool` | Whether analysis succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Analyzes text sentiment using sentiment analysis API or LLM.

---

### `GenerateCodeActivity`
**File:** `Activities/GenerateCodeActivity.cs`
**Category:** `AI/LLM`

| Input | Type | Description |
|---|---|---|
| `Description` | `string` | Code generation description/prompt |
| `Language` | `string` | Programming language (e.g., `"CSharp"`, `"JavaScript"`, `"Python"`) |
| `Framework` | `string?` | Framework (e.g., `"ASP.NET"`, `"React"`) |
| `Style` | `string?` | Coding style/guidelines |
| `Provider` | `string` | LLM provider (default: `"OpenAI"`) |

| Output | Type | Description |
|---|---|---|
| `Code` | `string` | Generated code |
| `Language` | `string` | Target language used |
| `Success` | `bool` | Whether generation succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses LLM (GPT-4, etc.) to generate code snippets.

---

## Summary of Extended Activities

**Total: 35 activities across 13 categories** 📝

| # | Category | Activities |
|---|---|---|
| 7.15 | Version Control | 4 |
| 7.16 | File Transfer | 4 |
| 7.17 | Email Advanced | 3 |
| 7.18 | Notifications Adv. | 2 |
| 7.19 | PDF Advanced | 3 |
| 7.20 | Data Conversion | 2 |
| 7.21 | Database Advanced | 2 |
| 7.22 | Scheduling | 2 |
| 7.23 | Imaging | 4 |
| 7.24 | Monitoring | 2 |
| 7.25 | Archive Advanced | 2 |
| 7.27 | AI/LLM ⭐ | 7 |

---

**Status:** 📝 Phase 2 Extended — All 35 activities documented. Includes critical **AI/LLM integration** (7 activities) for intelligent workflow automation.

**Next:** Review [Phase 3 Advanced Activities](ElsaWF-07c-Phase7-Advanced-Activities.md)
