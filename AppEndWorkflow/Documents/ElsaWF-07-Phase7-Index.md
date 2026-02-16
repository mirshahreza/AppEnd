# Phase 7 — Custom Activity Library (Index)

> Part of [Elsa Integration Plan](ELSA-INTEGRATION-PLAN.md)

---

## Goal
Build a library of custom Elsa activities tailored to AppEnd's ecosystem, enabling workflow authors to use common operations as drag-and-drop building blocks.

All custom activities are implemented as classes inheriting from `CodeActivity` (or `Activity`) in the `AppEndWorkflow/Activities/` folder. They are auto-discovered by Elsa and appear in the Activity Browser (Phase 6, Component 4).

---

## 📚 Documentation Structure

This comprehensive activity library is organized into 3 phases with 26 categories and **140 total activities**:

### 📖 **Phase 1: Core Activities** (48 activities)
**File:** [`ElsaWF-07a-Phase7-Core-Activities.md`](ElsaWF-07a-Phase7-Core-Activities.md)

14 essential categories covering fundamental workflow operations:

| # | Category | Activities | File |
|---|---|---|---|
| 7.1 | Notifications | 4 | SendEmail, SendSms, SendTelegram, SendPushNotification |
| 7.2 | Database | 2 | RunSqlQuery, RunSqlCommand |
| 7.3 | AppEnd Integration | 2 | CallRpcMethod, WriteLog |
| 7.4 | Human Tasks | 2 | AssignToUser, WaitForApproval |
| 7.5 | Data & Documents | 5 | GeneratePdf, GenerateExcel, TransformJson, ValidateData, MergeJson |
| 7.6 | HTTP & APIs | 3 | CallHttpApi, CallSoapService, DownloadFile |
| 7.7 | File Operations | 5 | ReadFile, WriteFile, CopyMoveFile, DeleteFile, ListFiles |
| 7.8 | String & Text | 4 | RenderTemplate, RegexMatch, FormatString, ParseCsv |
| 7.9 | Security & Crypto | 4 | HashData, EncryptDecrypt, GenerateToken, CheckPermission |
| 7.10 | Collections & Arrays | 5 | FilterArray, SortArray, AggregateArray, GroupBy, PickFromArray |
| 7.11 | Flow Control | 5 | Delay, WaitForSignal, ParallelForEach, Retry, Switch |
| 7.12 | Compression | 2 | CompressFiles, DecompressFiles |
| 7.13 | Math & Calculation | 2 | EvaluateExpression, ConvertCurrency |
| 7.14 | Caching | 3 | SetCache, GetCache, RemoveCache |

---

### 📖 **Phase 2: Extended Activities** (35 activities)
**File:** [`ElsaWF-07b-Phase7-Extended-Activities.md`](ElsaWF-07b-Phase7-Extended-Activities.md)

13 specialized categories extending core functionality:

| # | Category | Activities | Use Case |
|---|---|---|---|
| 7.15 | Version Control | 4 | Git Clone, Commit, Push, Pull |
| 7.16 | File Transfer | 4 | FTP/SFTP Upload & Download |
| 7.17 | Email Advanced | 3 | Bulk Email, Attachments, IMAP |
| 7.18 | Notifications Adv. | 2 | WhatsApp, Slack |
| 7.19 | PDF Advanced | 3 | Merge, Extract Text, Watermark |
| 7.20 | Data Conversion | 2 | JSON↔XML |
| 7.21 | Database Advanced | 2 | Backup, Restore |
| 7.22 | Scheduling | 2 | Schedule/Cancel Workflows |
| 7.23 | Imaging | 4 | QR Code, Barcode, OCR, Image→PDF |
| 7.24 | Monitoring | 2 | Elasticsearch, Application Insights |
| 7.25 | Archive Advanced | 2 | RAR, 7z |
| 7.27 | AI/LLM ⭐ | 7 | Chat, Summarize, Translate, Generate, Extract, Sentiment, Code |

---

### 📖 **Phase 3: Advanced Activities** (57 activities)
**File:** [`ElsaWF-07c-Phase7-Advanced-Activities.md`](ElsaWF-07c-Phase7-Advanced-Activities.md)

19 enterprise-grade categories for complex integrations:

| # | Category | Activities | Integration |
|---|---|---|---|
| 7.28 | Calendar & Events | 4 | Google Calendar, Outlook, Scheduling |
| 7.29 | Cloud Storage | 6 | AWS S3, Google Drive, Dropbox, OneDrive |
| 7.30 | CRM Integration | 4 | HubSpot, Salesforce, Zoho, Sync |
| 7.31 | E-commerce | 4 | Shopify, WooCommerce, Magento |
| 7.32 | Project Management | 4 | Jira, Asana, Monday.com |
| 7.33 | Message Queues | 5 | RabbitMQ, Kafka, Redis |
| 7.34 | Webhooks & Events | 4 | Receive, Send, Transform, Retry |
| 7.35 | RSS & Feeds | 3 | Parse, Fetch, Monitor |
| 7.36 | Payment Processing | 4 | Stripe, PayPal, Square, Invoices |
| 7.37 | Analytics & Reporting | 3 | Google Analytics, Reports, Scheduling |
| 7.38 | Social Media | 5 | Twitter, LinkedIn, Instagram, Facebook, Stats |
| 7.39 | Media Processing | 5 | Transcribe, Convert, Extract, Watermark |
| 7.40 | Document Management | 4 | eSignature, OCR, Merge, Convert |
| 7.41 | IoT & Sensors | 3 | MQTT, Commands, Time Series |
| 7.42 | Machine Learning | 4 | Predict, Anomaly, Clustering, Recommend |
| 7.43 | Form Processing | 4 | Submit, Parse Survey, Validate, Generate |
| 7.44 | Authentication | 4 | OAuth, JWT, Refresh, 2FA |
| 7.45 | Data Enrichment | 3 | Geolocation, Public Data, Deduplication |
| 7.46 | Business Logic | 4 | Tax, Discount, Invoice #, Inventory |

---

## 🎯 Total Summary

| Phase | Categories | Activities | Status |
|---|---|---|---|
| **Phase 1 (Core)** | 14 | 48 | ✅ **Implemented** |
| **Phase 2 (Extended)** | 13 | 35 | 📝 **Documented** |
| **Phase 3 (Advanced)** | 19 | 57 | 🆕 **New** |
| **TOTAL** | **26** | **140** | 🚀 |

---

## ✨ Quick Navigation

### By Use Case

**🔔 Notifications & Communication:**
- Phase 1: SendEmail, SendSms, SendTelegram, SendPush
- Phase 2: SendBulkEmail, SendWhatsApp, SendSlack
- Phase 2: ReceiveEmail
- Phase 3: PostToTwitter, PostToLinkedIn, PostToInstagram, PostToFacebook

**💾 Data & Storage:**
- Phase 1: ReadFile, WriteFile, DeleteFile, ListFiles
- Phase 2: GitClone, GitCommit, GitPush, GitPull
- Phase 2: FtpUpload, FtpDownload, SftpUpload, SftpDownload
- Phase 3: S3Upload, S3Download, GoogleDriveUpload, DropboxUpload, OneDriveUpload

**🔗 External Integrations:**
- Phase 1: CallHttpApi, CallSoapService, DownloadFile
- Phase 2: BackupDatabase, RestoreDatabase
- Phase 3: HubSpot, Salesforce, Zoho, Jira, Asana, Monday.com
- Phase 3: ShopifyOrder, WooCommerceProducts, MagentoInvoice
- Phase 3: StripeCharge, PayPalInvoice, SquarePayment

**🤖 AI & Intelligence:**
- Phase 2: ChatWithLLM, Summarize, Translate, GenerateContent, ExtractEntities, AnalyzeSentiment, GenerateCode
- Phase 3: PredictWithModel, DetectAnomaly, ClusterData, RecommendItems
- Phase 3: TranscribeAudio, ExtractTextFromImage, ReadBarcode

**📊 Analytics & Reporting:**
- Phase 1: FilterArray, SortArray, AggregateArray, GroupBy
- Phase 2: SendToElasticsearch, SendToApplicationInsights
- Phase 3: GoogleAnalyticsQuery, CreateReport, SendScheduledReport

**🔒 Security & Auth:**
- Phase 1: HashData, EncryptDecrypt, GenerateToken, CheckPermission
- Phase 3: GenerateOAuthToken, ValidateJwt, RefreshAccessToken, TwoFactorAuth

**⚙️ Business Logic:**
- Phase 1: CallRpcMethod, EvaluateExpression, ConvertCurrency
- Phase 3: CalculateTax, CalculateDiscount, GenerateInvoiceNumber, CheckInventory

---

## 📋 Implementation Pattern

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

## 🚀 Next Steps

1. **Review Phase 1** → [`ElsaWF-07a-Phase7-Core-Activities.md`](ElsaWF-07a-Phase7-Core-Activities.md)
   - Understand the base implementation pattern
   - Review core activities (already implemented ✅)

2. **Review Phase 2** → [`ElsaWF-07b-Phase7-Extended-Activities.md`](ElsaWF-07b-Phase7-Extended-Activities.md)
   - Study extended integrations
   - Plan implementation for priority activities

3. **Review Phase 3** → [`ElsaWF-07c-Phase7-Advanced-Activities.md`](ElsaWF-07c-Phase7-Advanced-Activities.md)
   - Explore enterprise integrations
   - Schedule implementation phases

4. **Implementation Priority:**
   - ✅ Phase 1: All 48 core activities (DONE)
   - 🔲 Phase 2a: LLM activities (7 activities) - HIGH PRIORITY
   - 🔲 Phase 2b: Cloud & FTP (8 activities) - HIGH PRIORITY
   - 🔲 Phase 3: Enterprise integrations - AS NEEDED

---

## 📞 Integration Notes

- All credentials/API keys are read from `appsettings.json`
- Database operations use `DbConf.FromSettings(name)`
- Logging uses AppEnd's existing `ExtensionsForLogging`
- JSON data exchange ensures compatibility between activities
- All activities support timeout and error handling
- Async operations use `async/await` patterns

---

**Last Updated:** 2024
**Phase 7 Status:** 🚀 Ready for Implementation
