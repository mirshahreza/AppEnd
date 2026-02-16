# Phase 7.28–7.46 — Advanced Activity Library

> Part of [Phase 7 — Custom Activity Library](ElsaWF-07-Phase7-Index.md)

---

## Overview

Phase 3 contains **57 advanced activities** across 19 enterprise-grade categories. These activities provide integration with major cloud platforms, SaaS applications, and specialized services.

---

## 7.28 — Calendar & Events Activities

### `GoogleCalendarCreateEvent`
**File:** `Activities/Calendar/GoogleCalendarCreateEvent.cs`
**Category:** `Calendar`

| Input | Type | Description |
|---|---|---|
| `CalendarId` | `string` | Google Calendar ID (default: `"primary"`) |
| `EventTitle` | `string` | Event title |
| `StartTime` | `DateTime` | Event start time |
| `EndTime` | `DateTime` | Event end time |
| `Description` | `string?` | Event description (optional) |
| `Location` | `string?` | Event location (optional) |
| `Attendees` | `string?` | JSON array of attendee emails (optional) |
| `TimeZone` | `string` | Timezone (default: `"UTC"`) |

| Output | Type | Description |
|---|---|---|
| `EventId` | `string` | Created event ID |
| `EventUrl` | `string` | URL to the event |
| `Success` | `bool` | Whether event was created |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses Google Calendar API via `Google.Apis.Calendar.v3`. Credentials from `appsettings.json`.

---

### `OutlookCreateEvent`
**File:** `Activities/Calendar/OutlookCreateEvent.cs`
**Category:** `Calendar`

| Input | Type | Description |
|---|---|---|
| `EventTitle` | `string` | Event title |
| `StartTime` | `DateTime` | Event start time |
| `EndTime` | `DateTime` | Event end time |
| `Description` | `string?` | Event description |
| `Location` | `string?` | Event location |
| `Attendees` | `string?` | JSON array of attendee emails |
| `Calendar` | `string` | Calendar name (default: `"Calendar"`) |

| Output | Type | Description |
|---|---|---|
| `EventId` | `string` | Created event ID |
| `Success` | `bool` | Whether event was created |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses Microsoft Graph API for Outlook calendar.

---

### `FindCalendarSlots`
**File:** `Activities/Calendar/FindCalendarSlots.cs`
**Category:** `Calendar`

| Input | Type | Description |
|---|---|---|
| `CalendarIds` | `string` | JSON array of calendar IDs to check |
| `StartDate` | `DateTime` | Search start date |
| `EndDate` | `DateTime` | Search end date |
| `DurationMinutes` | `int` | Required slot duration |
| `Minutes` | `int` | Resolution in minutes (default: `30`) |

| Output | Type | Description |
|---|---|---|
| `AvailableSlots` | `string` | JSON array of available time slots |
| `SlotCount` | `int` | Number of available slots found |
| `Success` | `bool` | Whether search succeeded |

**Implementation:** Queries multiple calendars and finds free time blocks.

---

### `SendCalendarInvite`
**File:** `Activities/Calendar/SendCalendarInvite.cs`
**Category:** `Calendar`

| Input | Type | Description |
|---|---|---|
| `FromEmail` | `string` | Sender email |
| `ToEmails` | `string` | JSON array of recipient emails |
| `EventTitle` | `string` | Event title |
| `EventTime` | `DateTime` | Event time |
| `Location` | `string?` | Location |
| `Message` | `string?` | Invitation message |

| Output | Type | Description |
|---|---|---|
| `InvitationsSent` | `int` | Number of invitations sent |
| `Success` | `bool` | Whether invitations were sent |
| `Error` | `string?` | Error message if failed |

**Implementation:** Sends calendar invitations via email.

---

## 7.29 — Cloud Storage Activities

### `S3UploadFile`
**File:** `Activities/CloudStorage/S3UploadFile.cs`
**Category:** `Cloud Storage`

| Input | Type | Description |
|---|---|---|
| `BucketName` | `string` | S3 bucket name |
| `FilePath` | `string` | Local file path |
| `S3Key` | `string` | S3 object key/path |
| `ContentType` | `string?` | MIME type (auto-detected if not set) |
| `Public` | `bool` | Make file publicly accessible (default: `false`) |
| `Metadata` | `string?` | JSON metadata to attach |

| Output | Type | Description |
|---|---|---|
| `S3Url` | `string` | S3 URL of uploaded file |
| `ETag` | `string` | File ETag |
| `Success` | `bool` | Whether upload succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses AWS SDK (`AWSSDK.S3`). Credentials from `appsettings.json`.

---

### `S3DownloadFile`
**File:** `Activities/CloudStorage/S3DownloadFile.cs`
**Category:** `Cloud Storage`

| Input | Type | Description |
|---|---|---|
| `BucketName` | `string` | S3 bucket name |
| `S3Key` | `string` | S3 object key/path |
| `SavePath` | `string` | Local path to save |
| `VersionId` | `string?` | Specific version ID (optional) |

| Output | Type | Description |
|---|---|---|
| `FilePath` | `string` | Path to downloaded file |
| `FileSize` | `long` | File size in bytes |
| `Success` | `bool` | Whether download succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Downloads S3 object to local filesystem.

---

### `GoogleDriveUploadFile`
**File:** `Activities/CloudStorage/GoogleDriveUploadFile.cs`
**Category:** `Cloud Storage`

| Input | Type | Description |
|---|---|---|
| `FilePath` | `string` | Local file path |
| `FileName` | `string` | Name in Google Drive |
| `ParentFolderId` | `string?` | Parent folder ID (optional) |
| `MimeType` | `string?` | File MIME type |
| `Description` | `string?` | File description |
| `Shared` | `bool` | Share with others (default: `false`) |

| Output | Type | Description |
|---|---|---|
| `FileId` | `string` | Google Drive file ID |
| `FileUrl` | `string` | Shareable link to file |
| `Success` | `bool` | Whether upload succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses Google Drive API.

---

### `GoogleDriveListFiles`
**File:** `Activities/CloudStorage/GoogleDriveListFiles.cs`
**Category:** `Cloud Storage`

| Input | Type | Description |
|---|---|---|
| `FolderId` | `string?` | Folder ID to list (default: root) |
| `Query` | `string?` | Search query (e.g., `"name contains 'invoice'"`) |
| `PageSize` | `int` | Results per page (default: `10`) |
| `OrderBy` | `string` | Sort order: `"name"`, `"modifiedTime"`, `"createdTime"` |

| Output | Type | Description |
|---|---|---|
| `Files` | `string` | JSON array of file metadata |
| `FileCount` | `int` | Number of files returned |
| `Success` | `bool` | Whether listing succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Queries Google Drive and returns file list.

---

### `DropboxUploadFile`
**File:** `Activities/CloudStorage/DropboxUploadFile.cs`
**Category:** `Cloud Storage`

| Input | Type | Description |
|---|---|---|
| `FilePath` | `string` | Local file path |
| `DropboxPath` | `string` | Dropbox destination path |
| `Overwrite` | `bool` | Overwrite if exists (default: `false`) |
| `AutoRename` | `bool` | Auto-rename if exists (default: `false`) |

| Output | Type | Description |
|---|---|---|
| `DropboxId` | `string` | Dropbox file ID |
| `DropboxPath` | `string` | Path in Dropbox |
| `Success` | `bool` | Whether upload succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses Dropbox API.

---

### `OneDriveUploadFile`
**File:** `Activities/CloudStorage/OneDriveUploadFile.cs`
**Category:** `Cloud Storage`

| Input | Type | Description |
|---|---|---|
| `FilePath` | `string` | Local file path |
| `OneDrivePath` | `string` | OneDrive destination path |
| `DriveId` | `string?` | OneDrive/SharePoint drive ID |

| Output | Type | Description |
|---|---|---|
| `ItemId` | `string` | OneDrive item ID |
| `ItemUrl` | `string` | URL to the item |
| `Success` | `bool` | Whether upload succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses Microsoft Graph API for OneDrive/SharePoint.

---

## 7.30 — CRM Integration Activities

### `HubSpotCreateContact`
**File:** `Activities/CRM/HubSpotCreateContact.cs`
**Category:** `CRM`

| Input | Type | Description |
|---|---|---|
| `Email` | `string` | Contact email |
| `FirstName` | `string?` | First name |
| `LastName` | `string?` | Last name |
| `Phone` | `string?` | Phone number |
| `Company` | `string?` | Company name |
| `CustomProperties` | `string?` | JSON object of custom properties |

| Output | Type | Description |
|---|---|---|
| `ContactId` | `string` | Created/updated contact ID |
| `Success` | `bool` | Whether operation succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses HubSpot API. API key from `appsettings.json`.

---

### `SalesforceQueryRecords`
**File:** `Activities/CRM/SalesforceQueryRecords.cs`
**Category:** `CRM`

| Input | Type | Description |
|---|---|---|
| `SObjectType` | `string` | Salesforce object type (e.g., `"Account"`, `"Contact"`) |
| `Query` | `string` | SOQL query |
| `Limit` | `int` | Maximum records to return (default: `100`) |

| Output | Type | Description |
|---|---|---|
| `Records` | `string` | JSON array of records |
| `RecordCount` | `int` | Number of records returned |
| `Success` | `bool` | Whether query succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Executes SOQL via Salesforce REST API.

---

### `ZohoCrmCreateDeal`
**File:** `Activities/CRM/ZohoCrmCreateDeal.cs`
**Category:** `CRM`

| Input | Type | Description |
|---|---|---|
| `DealName` | `string` | Deal name |
| `Amount` | `decimal` | Deal amount |
| `Stage` | `string` | Deal stage (e.g., `"Qualification"`, `"Proposal"`, `"Closed Won"`) |
| `AccountId` | `string?` | Associated account ID |
| `ContactIds` | `string?` | JSON array of contact IDs |
| `CustomFields` | `string?` | JSON object of custom field values |

| Output | Type | Description |
|---|---|---|
| `DealId` | `string` | Created deal ID |
| `Success` | `bool` | Whether creation succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses Zoho CRM API.

---

### `SyncContactToCRM`
**File:** `Activities/CRM/SyncContactToCRM.cs`
**Category:** `CRM`

| Input | Type | Description |
|---|---|---|
| `CrmProvider` | `string` | CRM platform: `"HubSpot"`, `"Salesforce"`, `"Zoho"` |
| `ContactData` | `string` | JSON contact object |
| `Mode` | `string` | `"Create"`, `"Update"`, `"Upsert"` (default: `"Upsert"`) |

| Output | Type | Description |
|---|---|---|
| `ContactId` | `string` | CRM contact ID |
| `Result` | `string` | `"Created"`, `"Updated"`, or `"Skipped"` |
| `Success` | `bool` | Whether sync succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Routes to appropriate CRM provider API.

---

## 7.31 — E-commerce Integration Activities

### `ShopifyCreateOrder`
**File:** `Activities/Ecommerce/ShopifyCreateOrder.cs`
**Category:** `E-commerce`

| Input | Type | Description |
|---|---|---|
| `CustomerId` | `string` | Shopify customer ID |
| `Items` | `string` | JSON array of line items `[{ProductId, Quantity, Price}]` |
| `Email` | `string` | Customer email |
| `ShippingAddress` | `string` | JSON shipping address |
| `Tags` | `string?` | Order tags (comma-separated) |

| Output | Type | Description |
|---|---|---|
| `OrderId` | `string` | Created order ID |
| `OrderNumber` | `string` | Order number |
| `Success` | `bool` | Whether order was created |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses Shopify GraphQL or REST API.

---

### `ShopifyUpdateProduct`
**File:** `Activities/Ecommerce/ShopifyUpdateProduct.cs`
**Category:** `E-commerce`

| Input | Type | Description |
|---|---|---|
| `ProductId` | `string` | Shopify product ID |
| `Title` | `string?` | Product title |
| `Description` | `string?` | Product description |
| `Price` | `decimal?` | Product price |
| `Inventory` | `int?` | Inventory count |
| `Status` | `string?` | `"active"`, `"archived"`, or `"draft"` |

| Output | Type | Description |
|---|---|---|
| `ProductId` | `string` | Updated product ID |
| `Success` | `bool` | Whether update succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Updates product via Shopify API.

---

### `WooCommerceGetProducts`
**File:** `Activities/Ecommerce/WooCommerceGetProducts.cs`
**Category:** `E-commerce`

| Input | Type | Description |
|---|---|---|
| `StoreUrl` | `string` | WooCommerce store URL |
| `Search` | `string?` | Product search term |
| `Category` | `int?` | Category ID |
| `Limit` | `int` | Results limit (default: `20`) |

| Output | Type | Description |
|---|---|---|
| `Products` | `string` | JSON array of products |
| `ProductCount` | `int` | Number of products returned |
| `Success` | `bool` | Whether query succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Queries WooCommerce REST API.

---

### `MagentoCreateInvoice`
**File:** `Activities/Ecommerce/MagentoCreateInvoice.cs`
**Category:** `E-commerce`

| Input | Type | Description |
|---|---|---|
| `OrderId` | `string` | Magento order ID |
| `Items` | `string?` | JSON array of line items (if partial invoice) |
| `Comment` | `string?` | Invoice comment |
| `Email` | `bool` | Email invoice (default: `false`) |

| Output | Type | Description |
|---|---|---|
| `InvoiceId` | `string` | Created invoice ID |
| `InvoiceNumber` | `string` | Invoice number |
| `Success` | `bool` | Whether invoice was created |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses Magento REST API.

---

## 7.32 — Project Management Activities

### `JiraCreateIssue`
**File:** `Activities/ProjectManagement/JiraCreateIssue.cs`
**Category:** `Project Management`

| Input | Type | Description |
|---|---|---|
| `ProjectKey` | `string` | Jira project key (e.g., `"PROJ"`) |
| `IssueType` | `string` | Issue type: `"Bug"`, `"Story"`, `"Task"`, `"Epic"` |
| `Summary` | `string` | Issue summary/title |
| `Description` | `string?` | Issue description |
| `Assignee` | `string?` | Assignee username |
| `Labels` | `string?` | Comma-separated labels |
| `CustomFields` | `string?` | JSON object of custom fields |

| Output | Type | Description |
|---|---|---|
| `IssueKey` | `string` | Created issue key (e.g., `"PROJ-123"`) |
| `IssueUrl` | `string` | URL to the issue |
| `Success` | `bool` | Whether creation succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses Jira REST API.

---

### `JiraUpdateIssue`
**File:** `Activities/ProjectManagement/JiraUpdateIssue.cs`
**Category:** `Project Management`

| Input | Type | Description |
|---|---|---|
| `IssueKey` | `string` | Jira issue key (e.g., `"PROJ-123"`) |
| `Status` | `string?` | New status |
| `Resolution` | `string?` | Resolution (if closing) |
| `Comment` | `string?` | Add comment to issue |
| `FieldUpdates` | `string?` | JSON object of field updates |

| Output | Type | Description |
|---|---|---|
| `Success` | `bool` | Whether update succeeded |
| `NewStatus` | `string?` | Updated status |
| `Error` | `string?` | Error message if failed |

**Implementation:** Updates issue via Jira API.

---

### `AsanaCreateTask`
**File:** `Activities/ProjectManagement/AsanaCreateTask.cs`
**Category:** `Project Management`

| Input | Type | Description |
|---|---|---|
| `ProjectId` | `string` | Asana project ID |
| `Name` | `string` | Task name |
| `Description` | `string?` | Task description |
| `AssigneeId` | `string?` | Assignee user ID |
| `DueDate` | `DateTime?` | Due date |
| `CustomFields` | `string?` | JSON custom fields |

| Output | Type | Description |
|---|---|---|
| `TaskId` | `string` | Created task ID |
| `Success` | `bool` | Whether creation succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses Asana REST API.

---

### `MondayComCreateItem`
**File:** `Activities/ProjectManagement/MondayComCreateItem.cs`
**Category:** `Project Management`

| Input | Type | Description |
|---|---|---|
| `BoardId` | `string` | Monday.com board ID |
| `ItemName` | `string` | Item name |
| `ColumnValues` | `string` | JSON object of column values |

| Output | Type | Description |
|---|---|---|
| `ItemId` | `string` | Created item ID |
| `Success` | `bool` | Whether creation succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses Monday.com GraphQL API.

---

## 7.33 — Message Queues & Pub/Sub Activities

### `PublishToRabbitMQ`
**File:** `Activities/MessageQueues/PublishToRabbitMQ.cs`
**Category:** `Message Queues`

| Input | Type | Description |
|---|---|---|
| `HostName` | `string` | RabbitMQ hostname |
| `ExchangeName` | `string` | Exchange name |
| `RoutingKey` | `string` | Routing key |
| `Message` | `string` | Message content (JSON or plain text) |
| `Priority` | `int?` | Message priority (0-10) |
| `Expiration` | `int?` | Expiration time in milliseconds |

| Output | Type | Description |
|---|---|---|
| `MessageId` | `string` | Published message ID |
| `Success` | `bool` | Whether publish succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses `RabbitMQ.Client` library.

---

### `ConsumeFromRabbitMQ`
**File:** `Activities/MessageQueues/ConsumeFromRabbitMQ.cs`
**Category:** `Message Queues`

| Input | Type | Description |
|---|---|---|
| `HostName` | `string` | RabbitMQ hostname |
| `QueueName` | `string` | Queue name to consume from |
| `TimeoutSeconds` | `int` | Timeout before giving up (default: `30`) |
| `AutoAck` | `bool` | Auto-acknowledge messages (default: `true`) |

| Output | Type | Description |
|---|---|---|
| `Message` | `string` | Consumed message body |
| `MessageId` | `string` | Message ID |
| `Headers` | `string` | JSON message headers |
| `Success` | `bool` | Whether consume succeeded |

**Implementation:** Consumes message from RabbitMQ queue.

---

### `PublishToKafka`
**File:** `Activities/MessageQueues/PublishToKafka.cs`
**Category:** `Message Queues`

| Input | Type | Description |
|---|---|---|
| `BootstrapServers` | `string` | Kafka bootstrap servers (comma-separated) |
| `Topic` | `string` | Kafka topic |
| `Message` | `string` | Message content |
| `Key` | `string?` | Message key (for partitioning) |
| `Headers` | `string?` | JSON headers |

| Output | Type | Description |
|---|---|---|
| `Partition` | `int` | Partition the message was sent to |
| `Offset` | `long` | Message offset |
| `Success` | `bool` | Whether publish succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses `Confluent.Kafka` library.

---

### `PublishToRedis`
**File:** `Activities/MessageQueues/PublishToRedis.cs`
**Category:** `Message Queues`

| Input | Type | Description |
|---|---|---|
| `RedisConnectionString` | `string` | Redis connection string |
| `ChannelName` | `string` | Channel name |
| `Message` | `string` | Message content |

| Output | Type | Description |
|---|---|---|
| `SubscribersReached` | `int` | Number of subscribers who received the message |
| `Success` | `bool` | Whether publish succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses `StackExchange.Redis` library.

---

### `SubscribeToRedis`
**File:** `Activities/MessageQueues/SubscribeToRedis.cs`
**Category:** `Message Queues`

| Input | Type | Description |
|---|---|---|
| `RedisConnectionString` | `string` | Redis connection string |
| `ChannelName` | `string` | Channel name to subscribe to |
| `TimeoutSeconds` | `int` | Timeout before giving up (default: `30`) |

| Output | Type | Description |
|---|---|---|
| `Message` | `string` | Received message |
| `Success` | `bool` | Whether subscription succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Subscribes to Redis channel and waits for message.

---

## 7.34 — Webhooks & Events Activities

### `ReceiveWebhook`
**File:** `Activities/Webhooks/ReceiveWebhook.cs`
**Category:** `Webhooks`

| Input | Type | Description |
|---|---|---|
| `WebhookPath` | `string` | Webhook endpoint path (e.g., `"/api/webhooks/order"`) |
| `Method` | `string` | HTTP method: `"POST"`, `"PUT"`, `"PATCH"` (default: `"POST"`) |
| `TimeoutSeconds` | `int` | Timeout before giving up (default: `300`) |
| `SignatureHeader` | `string?` | Header name for signature verification (optional) |

| Output | Type | Description |
|---|---|---|
| `PayloadJson` | `string` | Received webhook payload as JSON |
| `Headers` | `string` | JSON of request headers |
| `QueryParams` | `string` | JSON of query parameters |
| `Authenticated` | `bool` | Whether signature verification passed |
| `Success` | `bool` | Whether webhook was received |

**Implementation:** Registers webhook endpoint and waits for incoming request via Elsa bookmark.

---

### `SendWebhook`
**File:** `Activities/Webhooks/SendWebhook.cs`
**Category:** `Webhooks`

| Input | Type | Description |
|---|---|---|
| `Url` | `string` | Target webhook URL |
| `Payload` | `string` | JSON payload to send |
| `Secret` | `string?` | Secret key for HMAC-SHA256 signature (optional) |
| `Headers` | `string?` | JSON extra headers to include |
| `RetryCount` | `int` | Number of retries (default: `3`) |
| `TimeoutSeconds` | `int` | Request timeout (default: `30`) |

| Output | Type | Description |
|---|---|---|
| `StatusCode` | `int` | HTTP response status |
| `ResponseBody` | `string?` | Response body |
| `Success` | `bool` | Whether webhook was delivered |
| `Error` | `string?` | Error message if failed |

**Implementation:** Sends HTTP POST request with automatic retry and signature.

---

### `TransformWebhookPayload`
**File:** `Activities/Webhooks/TransformWebhookPayload.cs`
**Category:** `Webhooks`

| Input | Type | Description |
|---|---|---|
| `PayloadJson` | `string` | Original webhook payload |
| `MappingRules` | `string` | JSON transformation rules |

| Output | Type | Description |
|---|---|---|
| `TransformedPayload` | `string` | Normalized JSON output |
| `Success` | `bool` | Whether transformation succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Transforms webhook payload to standard format using mapping rules.

---

### `WebhookRetry`
**File:** `Activities/Webhooks/WebhookRetry.cs`
**Category:** `Webhooks`

| Input | Type | Description |
|---|---|---|
| `Url` | `string` | Webhook URL to retry |
| `Payload` | `string` | JSON payload |
| `MaxRetries` | `int` | Maximum retry attempts (default: `5`) |
| `InitialDelaySeconds` | `int` | Initial delay before first retry (default: `5`) |
| `BackoffMultiplier` | `double` | Exponential backoff multiplier (default: `2.0`) |

| Output | Type | Description |
|---|---|---|
| `AttemptCount` | `int` | Total attempts made |
| `Success` | `bool` | Whether webhook was delivered |
| `LastStatusCode` | `int?` | Last HTTP status received |
| `Error` | `string?` | Error message if all retries failed |

**Implementation:** Implements exponential backoff retry strategy for webhook delivery.

---

## 7.35 — RSS & Feeds Activities

### `ParseRssFeed`
**File:** `Activities/Feeds/ParseRssFeed.cs`
**Category:** `RSS`

| Input | Type | Description |
|---|---|---|
| `FeedUrl` | `string` | RSS/Atom feed URL |
| `ItemLimit` | `int` | Maximum items to parse (default: `50`) |
| `TimeoutSeconds` | `int` | Feed fetch timeout (default: `30`) |

| Output | Type | Description |
|---|---|---|
| `Items` | `string` | JSON array of parsed feed items |
| `ItemCount` | `int` | Number of items parsed |
| `Title` | `string` | Feed title |
| `Success` | `bool` | Whether parsing succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses `System.ServiceModel.Syndication` or `SyndicationClient`.

---

### `FetchFeedItems`
**File:** `Activities/Feeds/FetchFeedItems.cs`
**Category:** `RSS`

| Input | Type | Description |
|---|---|---|
| `FeedUrl` | `string` | Feed URL |
| `SinceDateUtc` | `DateTime?` | Only items published after this date |
| `Keyword` | `string?` | Filter items by keyword |

| Output | Type | Description |
|---|---|---|
| `Items` | `string` | JSON array of items |
| `NewItemCount` | `int` | Number of new items |
| `Success` | `bool` | Whether fetch succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Fetches and filters recent feed items.

---

### `MonitorFeedUpdates`
**File:** `Activities/Feeds/MonitorFeedUpdates.cs`
**Category:** `RSS`

| Input | Type | Description |
|---|---|---|
| `FeedUrl` | `string` | Feed URL to monitor |
| `CheckIntervalMinutes` | `int` | Check interval (default: `60`) |
| `MaxDuration` | `int?` | Maximum monitoring duration in minutes |

| Output | Type | Description |
|---|---|---|
| `UpdateFound` | `bool` | Whether new items were found |
| `NewItems` | `string` | JSON array of new items |
| `LastCheckTime` | `DateTime` | When the last check occurred |

**Implementation:** Periodically checks feed for updates using Elsa timer.

---

## 7.36 — Payment Processing Activities

### `StripeChargeCard`
**File:** `Activities/Payments/StripeChargeCard.cs`
**Category:** `Payments`

| Input | Type | Description |
|---|---|---|
| `Amount` | `decimal` | Amount to charge (in cents) |
| `Currency` | `string` | Currency code (default: `"USD"`) |
| `CardToken` | `string` | Stripe card token or customer ID |
| `Description` | `string?` | Charge description |
| `Metadata` | `string?` | JSON metadata to attach |

| Output | Type | Description |
|---|---|---|
| `ChargeId` | `string` | Stripe charge ID |
| `Status` | `string` | Charge status (`"succeeded"`, `"pending"`, `"failed"`) |
| `Success` | `bool` | Whether charge succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses Stripe API via `Stripe.net` library.

---

### `PayPalCreateInvoice`
**File:** `Activities/Payments/PayPalCreateInvoice.cs`
**Category:** `Payments`

| Input | Type | Description |
|---|---|---|
| `CustomerId` | `string` | PayPal customer ID |
| `Amount` | `decimal` | Invoice amount |
| `Currency` | `string` | Currency code (default: `"USD"`) |
| `Description` | `string` | Invoice description |
| `DueDate` | `DateTime?` | Due date |
| `Items` | `string?` | JSON array of line items |

| Output | Type | Description |
|---|---|---|
| `InvoiceId` | `string` | PayPal invoice ID |
| `InvoiceUrl` | `string` | URL to invoice |
| `Success` | `bool` | Whether creation succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses PayPal REST API.

---

### `SquareProcessPayment`
**File:** `Activities/Payments/SquareProcessPayment.cs`
**Category:** `Payments`

| Input | Type | Description |
|---|---|---|
| `Amount` | `long` | Amount in the smallest currency unit |
| `Currency` | `string` | Currency code (default: `"USD"`) |
| `SourceId` | `string` | Payment source ID (card nonce) |
| `Description` | `string?` | Payment description |

| Output | Type | Description |
|---|---|---|
| `PaymentId` | `string` | Square payment ID |
| `Status` | `string` | Payment status |
| `Success` | `bool` | Whether payment succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses Square API.

---

### `GenerateInvoice`
**File:** `Activities/Payments/GenerateInvoice.cs`
**Category:** `Payments`

| Input | Type | Description |
|---|---|---|
| `InvoiceNumber` | `string` | Invoice number |
| `CustomerData` | `string` | JSON customer information |
| `Items` | `string` | JSON array of line items with amounts |
| `Tax` | `decimal?` | Tax amount |
| `Discount` | `decimal?` | Discount amount |
| `Notes` | `string?` | Invoice notes |

| Output | Type | Description |
|---|---|---|
| `FilePath` | `string` | Path to generated PDF invoice |
| `FileSize` | `long` | Invoice file size |
| `Success` | `bool` | Whether generation succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Generates PDF invoice using QuestPDF or similar.

---

## 7.37 — Analytics & Reporting Activities

### `GoogleAnalyticsQuery`
**File:** `Activities/Analytics/GoogleAnalyticsQuery.cs`
**Category:** `Analytics`

| Input | Type | Description |
|---|---|---|
| `PropertyId` | `string` | Google Analytics 4 property ID |
| `StartDate` | `DateTime` | Report start date |
| `EndDate` | `DateTime` | Report end date |
| `Dimensions` | `string` | Comma-separated dimensions (e.g., `"country,city"`) |
| `Metrics` | `string` | Comma-separated metrics (e.g., `"activeUsers,screenPageViews"`) |

| Output | Type | Description |
|---|---|---|
| `Data` | `string` | JSON report data |
| `RowCount` | `int` | Number of data rows |
| `Success` | `bool` | Whether query succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses Google Analytics Data API.

---

### `CreateReport`
**File:** `Activities/Analytics/CreateReport.cs`
**Category:** `Analytics`

| Input | Type | Description |
|---|---|---|
| `Title` | `string` | Report title |
| `Description` | `string?` | Report description |
| `Data` | `string` | JSON data for the report |
| `Format` | `string` | Output format: `"PDF"`, `"Excel"`, `"HTML"` |
| `ChartType` | `string?` | Chart type if visualizing: `"BarChart"`, `"LineChart"`, `"PieChart"` |

| Output | Type | Description |
|---|---|---|
| `FilePath` | `string` | Path to generated report |
| `FileSize` | `long` | Report file size |
| `Success` | `bool` | Whether generation succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Generates report in requested format using charting libraries.

---

### `SendScheduledReport`
**File:** `Activities/Analytics/SendScheduledReport.cs`
**Category:** `Analytics`

| Input | Type | Description |
|---|---|---|
| `ReportPath` | `string` | Path to report file |
| `Recipients` | `string` | JSON array of recipient emails |
| `Subject` | `string` | Email subject |
| `Message` | `string?` | Email body message |
| `ScheduleExpression` | `string` | Cron expression for scheduling |

| Output | Type | Description |
|---|---|---|
| `ScheduledId` | `string` | Scheduled report ID |
| `NextSendTime` | `DateTime` | Next scheduled send time |
| `Success` | `bool` | Whether scheduling succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Schedules report to send via email at specified intervals.

---

## 7.38 — Social Media Activities

### `PostToTwitter`
**File:** `Activities/SocialMedia/PostToTwitter.cs`
**Category:** `Social Media`

| Input | Type | Description |
|---|---|---|
| `Text` | `string` | Tweet text (max 280 characters) |
| `MediaUrls` | `string?` | JSON array of media URLs (optional) |
| `ReplyToTweetId` | `string?` | Tweet ID to reply to (optional) |

| Output | Type | Description |
|---|---|---|
| `TweetId` | `string` | Created tweet ID |
| `TweetUrl` | `string` | URL to the tweet |
| `Success` | `bool` | Whether post succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses Twitter API v2.

---

### `PostToLinkedIn`
**File:** `Activities/SocialMedia/PostToLinkedIn.cs`
**Category:** `Social Media`

| Input | Type | Description |
|---|---|---|
| `Text` | `string` | Post text |
| `ImageUrl` | `string?` | Optional image URL |
| `ArticleUrl` | `string?` | Optional article link |

| Output | Type | Description |
|---|---|---|
| `PostId` | `string` | Created post ID |
| `PostUrl` | `string` | URL to the post |
| `Success` | `bool` | Whether post succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses LinkedIn API.

---

### `PostToInstagram`
**File:** `Activities/SocialMedia/PostToInstagram.cs`
**Category:** `Social Media`

| Input | Type | Description |
|---|---|---|
| `ImageUrl` | `string` | Image URL to post |
| `Caption` | `string?` | Post caption |
| `Hashtags` | `string?` | Hashtags (comma-separated or JSON array) |

| Output | Type | Description |
|---|---|---|
| `PostId` | `string` | Created post ID |
| `Success` | `bool` | Whether post succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses Instagram Graph API.

---

### `FacebookPostMessage`
**File:** `Activities/SocialMedia/FacebookPostMessage.cs`
**Category:** `Social Media`

| Input | Type | Description |
|---|---|---|
| `PageId` | `string` | Facebook page ID |
| `Message` | `string` | Message text |
| `ImageUrl` | `string?` | Optional image URL |
| `LinkUrl` | `string?` | Optional link URL |

| Output | Type | Description |
|---|---|---|
| `PostId` | `string` | Created post ID |
| `Success` | `bool` | Whether post succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses Facebook Graph API.

---

### `GetSocialMediaStats`
**File:** `Activities/SocialMedia/GetSocialMediaStats.cs`
**Category:** `Social Media`

| Input | Type | Description |
|---|---|---|
| `Platform` | `string` | Platform: `"Twitter"`, `"LinkedIn"`, `"Instagram"`, `"Facebook"` |
| `EntityId` | `string` | Post ID or account ID to query |
| `Metric` | `string` | Metric type: `"Engagement"`, `"Reach"`, `"Impressions"`, `"Shares"` |

| Output | Type | Description |
|---|---|---|
| `Stats` | `string` | JSON object of metrics |
| `Success` | `bool` | Whether query succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Queries social platform APIs for engagement metrics.

---

## 7.39 — Video & Audio Processing Activities

### `TranscribeAudio`
**File:** `Activities/MediaProcessing/TranscribeAudio.cs`
**Category:** `Media Processing`

| Input | Type | Description |
|---|---|---|
| `AudioFilePath` | `string` | Path to audio file |
| `Language` | `string` | Language code (e.g., `"en"`, `"fa"`, `"de"`) |
| `Provider` | `string` | Provider: `"OpenAI"`, `"Google"`, `"Azure"` |
| `OutputFormat` | `string` | Output format: `"Text"`, `"VTT"`, `"SRT"` |

| Output | Type | Description |
|---|---|---|
| `Transcript` | `string` | Transcribed text or subtitle file content |
| `Language` | `string` | Detected language |
| `Confidence` | `double` | Confidence score (0-1) |
| `Success` | `bool` | Whether transcription succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses OpenAI Whisper, Google Cloud Speech-to-Text, or Azure Speech Services.

---

### `ConvertVideoFormat`
**File:** `Activities/MediaProcessing/ConvertVideoFormat.cs`
**Category:** `Media Processing`

| Input | Type | Description |
|---|---|---|
| `InputPath` | `string` | Input video file path |
| `OutputPath` | `string` | Output video file path |
| `Format` | `string` | Output format: `"MP4"`, `"WebM"`, `"MKV"`, `"AVI"` |
| `Quality` | `string` | Quality preset: `"Low"`, `"Medium"`, `"High"`, `"Max"` |
| `Bitrate` | `string?` | Bitrate (e.g., `"2500k"`) |

| Output | Type | Description |
|---|---|---|
| `OutputPath` | `string` | Path to converted video |
| `FileSize` | `long` | Output file size |
| `Duration` | `string` | Video duration |
| `Success` | `bool` | Whether conversion succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses FFmpeg.

---

### `ExtractAudioFromVideo`
**File:** `Activities/MediaProcessing/ExtractAudioFromVideo.cs`
**Category:** `Media Processing`

| Input | Type | Description |
|---|---|---|
| `VideoPath` | `string` | Video file path |
| `OutputPath` | `string` | Output audio file path |
| `Format` | `string` | Audio format: `"MP3"`, `"WAV"`, `"AAC"`, `"OGG"` |

| Output | Type | Description |
|---|---|---|
| `AudioPath` | `string` | Path to extracted audio |
| `FileSize` | `long` | Audio file size |
| `Success` | `bool` | Whether extraction succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses FFmpeg to extract audio track.

---

### `GenerateVideoThumbnail`
**File:** `Activities/MediaProcessing/GenerateVideoThumbnail.cs`
**Category:** `Media Processing`

| Input | Type | Description |
|---|---|---|
| `VideoPath` | `string` | Video file path |
| `OutputPath` | `string` | Thumbnail output path |
| `TimeSeconds` | `int?` | Timestamp for thumbnail (default: 0 for first frame) |
| `Width` | `int` | Thumbnail width in pixels |
| `Height` | `int` | Thumbnail height in pixels |

| Output | Type | Description |
|---|---|---|
| `ThumbnailPath` | `string` | Path to generated thumbnail |
| `Success` | `bool` | Whether generation succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses FFmpeg to extract thumbnail frame.

---

### `AddVideoWatermark`
**File:** `Activities/MediaProcessing/AddVideoWatermark.cs`
**Category:** `Media Processing`

| Input | Type | Description |
|---|---|---|
| `VideoPath` | `string` | Input video file path |
| `OutputPath` | `string` | Output video file path |
| `WatermarkImagePath` | `string` | Watermark image file path |
| `Position` | `string` | Position: `"TopLeft"`, `"TopRight"`, `"BottomLeft"`, `"BottomRight"`, `"Center"` |
| `Opacity` | `float` | Opacity (0-1, default: 0.7) |
| `Scale` | `float?` | Scale factor for watermark (optional) |

| Output | Type | Description |
|---|---|---|
| `OutputPath` | `string` | Path to watermarked video |
| `Success` | `bool` | Whether watermarking succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses FFmpeg overlay filter.

---

## 7.40 — Document Management & Signatures Activities

### `SendDocumentForSignature`
**File:** `Activities/DocumentManagement/SendDocumentForSignature.cs`
**Category:** `Document Management`

| Input | Type | Description |
|---|---|---|
| `DocumentPath` | `string` | Path to document file |
| `SignerEmails` | `string` | JSON array of signer emails |
| `SignatureProvider` | `string` | Provider: `"DocuSign"`, `"SignRequest"`, `"HelloSign"` |
| `Subject` | `string` | Email subject line |
| `Message` | `string?` | Message to signers |
| `RedirectUrl` | `string?` | URL to redirect after signing |

| Output | Type | Description |
|---|---|---|
| `EnvelopeId` | `string` | Signature request ID |
| `TrackingUrl` | `string` | URL to track signature status |
| `Success` | `bool` | Whether request was sent |
| `Error` | `string?` | Error message if failed |

**Implementation:** Integrates with signature service APIs.

---

### `ExtractFromDocument`
**File:** `Activities/DocumentManagement/ExtractFromDocument.cs`
**Category:** `Document Management`

| Input | Type | Description |
|---|---|---|
| `DocumentPath` | `string` | Document file path (PDF, image, etc.) |
| `ExtractType` | `string` | Type: `"Text"`, `"Tables"`, `"Forms"`, `"Entities"` |
| `Language` | `string?` | Document language for OCR |

| Output | Type | Description |
|---|---|---|
| `ExtractedData` | `string` | JSON extracted data |
| `Success` | `bool` | Whether extraction succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses Tesseract for OCR and Azure Form Recognizer for structured extraction.

---

### `MergeDocuments`
**File:** `Activities/DocumentManagement/MergeDocuments.cs`
**Category:** `Document Management`

| Input | Type | Description |
|---|---|---|
| `DocumentPaths` | `string` | JSON array of document paths to merge |
| `OutputPath` | `string` | Output file path |
| `Format` | `string` | Output format: `"PDF"`, `"DOCX"` |

| Output | Type | Description |
|---|---|---|
| `OutputPath` | `string` | Path to merged document |
| `PageCount` | `int` | Total pages in merged document |
| `Success` | `bool` | Whether merge succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses iTextSharp or PdfSharp for PDF merging.

---

### `ConvertDocumentFormat`
**File:** `Activities/DocumentManagement/ConvertDocumentFormat.cs`
**Category:** `Document Management`

| Input | Type | Description |
|---|---|---|
| `DocumentPath` | `string` | Input document path |
| `OutputPath` | `string` | Output file path |
| `OutputFormat` | `string` | Output format: `"PDF"`, `"DOCX"`, `"XLSX"`, `"Image"` (PNG/JPG) |

| Output | Type | Description |
|---|---|---|
| `OutputPath` | `string` | Path to converted document |
| `FileSize` | `long` | Output file size |
| `Success` | `bool` | Whether conversion succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses LibreOffice UNO or similar for document conversion.

---

## 7.41 — IoT & Sensor Data Activities

### `ReadSensorData`
**File:** `Activities/IoT/ReadSensorData.cs`
**Category:** `IoT`

| Input | Type | Description |
|---|---|---|
| `BrokerUrl` | `string` | MQTT broker URL |
| `Topic` | `string` | MQTT topic to subscribe to |
| `ClientId` | `string?` | MQTT client ID |
| `TimeoutSeconds` | `int` | Wait timeout (default: `30`) |

| Output | Type | Description |
|---|---|---|
| `SensorData` | `string` | JSON sensor reading |
| `Timestamp` | `DateTime` | When data was received |
| `Success` | `bool` | Whether data was received |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses `MQTTnet` library for MQTT subscription.

---

### `PublishSensorCommand`
**File:** `Activities/IoT/PublishSensorCommand.cs`
**Category:** `IoT`

| Input | Type | Description |
|---|---|---|
| `BrokerUrl` | `string` | MQTT broker URL |
| `Topic` | `string` | MQTT topic to publish to |
| `Command` | `string` | Command JSON payload |
| `QualityOfService` | `int` | MQTT QoS (0, 1, or 2) |

| Output | Type | Description |
|---|---|---|
| `Success` | `bool` | Whether command was published |
| `Error` | `string?` | Error message if failed |

**Implementation:** Publishes command to MQTT topic.

---

### `ProcessTimeSeriesData`
**File:** `Activities/IoT/ProcessTimeSeriesData.cs`
**Category:** `IoT`

| Input | Type | Description |
|---|---|---|
| `Data` | `string` | JSON array of time series data points |
| `Operation` | `string` | Operation: `"Aggregate"`, `"Interpolate"`, `"Resample"` |
| `WindowSize` | `int` | Time window size for aggregation |
| `AggregationFunction` | `string` | `"Sum"`, `"Average"`, `"Min"`, `"Max"` |

| Output | Type | Description |
|---|---|---|
| `ProcessedData` | `string` | JSON processed time series |
| `Success` | `bool` | Whether processing succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Processes time series data using aggregation/interpolation algorithms.

---

## 7.42 — Machine Learning & AI Activities

### `PredictWithModel`
**File:** `Activities/ML/PredictWithModel.cs`
**Category:** `ML/AI`

| Input | Type | Description |
|---|---|---|
| `ModelPath` | `string` | Path to ML model file (ONNX, SavedModel, etc.) |
| `InputData` | `string` | JSON input features for prediction |
| `Framework` | `string` | Framework: `"ONNX"`, `"TensorFlow"`, `"Custom"` |

| Output | Type | Description |
|---|---|---|
| `Prediction` | `string` | Prediction result(s) |
| `Confidence` | `double?` | Confidence score if applicable |
| `Success` | `bool` | Whether prediction succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses ONNX Runtime or TensorFlow.NET for model inference.

---

### `DetectAnomaly`
**File:** `Activities/ML/DetectAnomaly.cs`
**Category:** `ML/AI`

| Input | Type | Description |
|---|---|---|
| `Data` | `string` | JSON array of data points to analyze |
| `Sensitivity` | `double` | Anomaly sensitivity (0-1, default: 0.7) |
| `Window` | `int` | Moving window size for detection |

| Output | Type | Description |
|---|---|---|
| `AnomaliesDetected` | `int` | Number of anomalies found |
| `AnomalyIndices` | `string` | JSON array of anomalous data point indices |
| `Success` | `bool` | Whether detection succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses statistical methods or isolation forest for anomaly detection.

---

### `ClusterData`
**File:** `Activities/ML/ClusterData.cs`
**Category:** `ML/AI`

| Input | Type | Description |
|---|---|---|
| `Data` | `string` | JSON array of data objects |
| `Features` | `string` | JSON array of feature names to use for clustering |
| `ClusterCount` | `int` | Number of clusters (K) |
| `Algorithm` | `string` | Algorithm: `"KMeans"`, `"DBSCAN"`, `"Hierarchical"` |

| Output | Type | Description |
|---|---|---|
| `Clusters` | `string` | JSON array of clusters with assigned data points |
| `ClusterCenters` | `string?` | Cluster center coordinates |
| `Success` | `bool` | Whether clustering succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses ML.NET or sklearn-like libraries.

---

### `RecommendItems`
**File:** `Activities/ML/RecommendItems.cs`
**Category:** `ML/AI`

| Input | Type | Description |
|---|---|---|
| `UserId` | `string` | User ID to get recommendations for |
| `ItemData` | `string` | JSON array of available items |
| `UserHistory` | `string` | JSON user interaction history |
| `Count` | `int` | Number of recommendations (default: `5`) |
| `Algorithm` | `string` | Algorithm: `"Collaborative"`, `"ContentBased"`, `"Hybrid"` |

| Output | Type | Description |
|---|---|---|
| `Recommendations` | `string` | JSON array of recommended items with scores |
| `Success` | `bool` | Whether recommendations succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses collaborative or content-based filtering.

---

## 7.43 — Form & Survey Processing Activities

### `ProcessFormSubmission`
**File:** `Activities/Forms/ProcessFormSubmission.cs`
**Category:** `Form Processing`

| Input | Type | Description |
|---|---|---|
| `FormData` | `string` | JSON form submission data |
| `FormSchema` | `string` | JSON form schema for validation |
| `StoreResults` | `bool` | Save results to database (default: `true`) |
| `NotifyEmail` | `string?` | Email to notify on submission |

| Output | Type | Description |
|---|---|---|
| `FormId` | `string` | Submission ID |
| `ValidationErrors` | `string` | JSON array of validation errors (if any) |
| `IsValid` | `bool` | Whether form is valid |
| `Stored` | `bool` | Whether results were stored |
| `Success` | `bool` | Whether processing succeeded |

**Implementation:** Validates and stores form submissions.

---

### `ParseSurveyResponse`
**File:** `Activities/Forms/ParseSurveyResponse.cs`
**Category:** `Form Processing`

| Input | Type | Description |
|---|---|---|
| `SurveyResponseJson` | `string` | JSON survey response |
| `QuestionMap` | `string` | JSON mapping of question IDs to response codes |

| Output | Type | Description |
|---|---|---|
| `ParsedResponses` | `string` | JSON standardized survey responses |
| `Score` | `double?` | Calculated score if applicable |
| `Success` | `bool` | Whether parsing succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Parses and standardizes survey responses.

---

### `ValidateFormData`
**File:** `Activities/Forms/ValidateFormData.cs`
**Category:** `Form Processing`

| Input | Type | Description |
|---|---|---|
| `Data` | `string` | Form data JSON |
| `Schema` | `string` | JSON Schema definition |
| `StrictMode` | `bool` | Strict validation (default: `true`) |

| Output | Type | Description |
|---|---|---|
| `IsValid` | `bool` | Whether data matches schema |
| `Errors` | `string` | JSON validation error details |
| `Success` | `bool` | Whether validation completed |

**Implementation:** Uses JSON Schema validators.

---

### `GenerateFormLink`
**File:** `Activities/Forms/GenerateFormLink.cs`
**Category:** `Form Processing`

| Input | Type | Description |
|---|---|---|
| `FormId` | `string` | Form identifier |
| `Title` | `string` | Form title |
| `Description` | `string?` | Form description |
| `ExpirationDays` | `int?` | Link expiration (optional) |

| Output | Type | Description |
|---|---|---|
| `FormUrl` | `string` | Shareable form URL |
| `QRCode` | `string?` | QR code image (optional) |
| `Success` | `bool` | Whether generation succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Generates shareable form links and optionally QR codes.

---

## 7.44 — Authentication & OAuth Activities

### `GenerateOAuthToken`
**File:** `Activities/Auth/GenerateOAuthToken.cs`
**Category:** `Authentication`

| Input | Type | Description |
|---|---|---|
| `ClientId` | `string` | OAuth client ID |
| `ClientSecret` | `string` | OAuth client secret |
| `GrantType` | `string` | Grant type: `"AuthorizationCode"`, `"ClientCredentials"`, `"RefreshToken"` |
| `Scope` | `string` | OAuth scope(s) |
| `RedirectUri` | `string?` | Redirect URI (for AuthorizationCode) |
| `Code` | `string?` | Authorization code (for AuthorizationCode) |

| Output | Type | Description |
|---|---|---|
| `AccessToken` | `string` | OAuth access token |
| `TokenType` | `string` | Token type (usually `"Bearer"`) |
| `ExpiresIn` | `int` | Expiration in seconds |
| `RefreshToken` | `string?` | Refresh token if available |
| `Success` | `bool` | Whether token generation succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Implements OAuth2 flows.

---

### `ValidateJwt`
**File:** `Activities/Auth/ValidateJwt.cs`
**Category:** `Authentication`

| Input | Type | Description |
|---|---|---|
| `Token` | `string` | JWT token to validate |
| `PublicKey` | `string` | Public key for signature verification |
| `Audience` | `string?` | Expected audience claim |
| `Issuer` | `string?` | Expected issuer claim |

| Output | Type | Description |
|---|---|---|
| `IsValid` | `bool` | Whether token is valid |
| `Claims` | `string` | JSON decoded token claims |
| `ExpiresAt` | `DateTime?` | Token expiration time |
| `Error` | `string?` | Validation error if failed |

**Implementation:** Validates JWT signature and claims.

---

### `RefreshAccessToken`
**File:** `Activities/Auth/RefreshAccessToken.cs`
**Category:** `Authentication`

| Input | Type | Description |
|---|---|---|
| `RefreshToken` | `string` | Refresh token |
| `ClientId` | `string` | OAuth client ID |
| `ClientSecret` | `string` | OAuth client secret |
| `TokenEndpoint` | `string` | OAuth token endpoint URL |

| Output | Type | Description |
|---|---|---|
| `AccessToken` | `string` | New access token |
| `ExpiresIn` | `int` | Expiration in seconds |
| `Success` | `bool` | Whether refresh succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Refreshes expired OAuth access tokens.

---

### `TwoFactorAuth`
**File:** `Activities/Auth/TwoFactorAuth.cs`
**Category:** `Authentication`

| Input | Type | Description |
|---|---|---|
| `UserId` | `string` | User ID |
| `Method` | `string` | 2FA method: `"Email"`, `"SMS"`, `"TOTP"` |
| `EmailOrPhone` | `string?` | Email or phone for sending code |

| Output | Type | Description |
|---|---|---|
| `CodeSent` | `bool` | Whether 2FA code was sent |
| `SessionId` | `string` | Session ID for verification |
| `Success` | `bool` | Whether operation succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Sends 2FA code via email, SMS, or generates TOTP.

---

## 7.45 — Data Enrichment Activities

### `EnrichWithGeolocation`
**File:** `Activities/DataEnrichment/EnrichWithGeolocation.cs`
**Category:** `Data Enrichment`

| Input | Type | Description |
|---|---|---|
| `Input` | `string` | IP address, address string, or coordinates |
| `Type` | `string` | Input type: `"IPAddress"`, `"Address"`, `"Coordinates"` |

| Output | Type | Description |
|---|---|---|
| `Location` | `string` | JSON location details (country, city, lat/long, timezone) |
| `Success` | `bool` | Whether enrichment succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Uses GeoIP database or geolocation API.

---

### `EnrichWithPublicData`
**File:** `Activities/DataEnrichment/EnrichWithPublicData.cs`
**Category:** `Data Enrichment`

| Input | Type | Description |
|---|---|---|
| `Entity` | `string` | Entity to enrich (company name, person name, domain, etc.) |
| `Sources` | `string?` | JSON array of data sources to use (optional) |

| Output | Type | Description |
|---|---|---|
| `EnrichedData` | `string` | JSON enriched data from public sources |
| `DataPoints` | `int` | Number of data points found |
| `Success` | `bool` | Whether enrichment succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Queries public data APIs (company registries, person lookups, etc.).

---

### `DeduplicateRecords`
**File:** `Activities/DataEnrichment/DeduplicateRecords.cs`
**Category:** `Data Enrichment`

| Input | Type | Description |
|---|---|---|
| `Records` | `string` | JSON array of records to check for duplicates |
| `MatchFields` | `string` | JSON array of field names to match on |
| `Threshold` | `double` | Similarity threshold (0-1, default: 0.85) |

| Output | Type | Description |
|---|---|---|
| `DuplicateGroups` | `string` | JSON array of grouped duplicate records |
| `DuplicateCount` | `int` | Total duplicate record pairs found |
| `MergedRecords` | `string` | JSON merged unique records |
| `Success` | `bool` | Whether deduplication succeeded |

**Implementation:** Uses fuzzy string matching and similarity algorithms.

---

## 7.46 — Business Logic Helper Activities

### `CalculateTax`
**File:** `Activities/BusinessLogic/CalculateTax.cs`
**Category:** `Business Logic`

| Input | Type | Description |
|---|---|---|
| `Amount` | `decimal` | Gross amount |
| `TaxRate` | `decimal` | Tax rate as percentage (e.g., `15` for 15%) |
| `TaxType` | `string` | `"Inclusive"` (included in amount) or `"Exclusive"` (added to amount) |
| `Country` | `string?` | Country code for VAT rules (optional) |

| Output | Type | Description |
|---|---|---|
| `TaxAmount` | `decimal` | Calculated tax |
| `NetAmount` | `decimal` | Amount before tax |
| `TotalAmount` | `decimal` | Total with tax |
| `Success` | `bool` | Whether calculation succeeded |

**Implementation:** Calculates tax with support for different tax models.

---

### `CalculateDiscount`
**File:** `Activities/BusinessLogic/CalculateDiscount.cs`
**Category:** `Business Logic`

| Input | Type | Description |
|---|---|---|
| `Amount` | `decimal` | Original amount |
| `DiscountRule` | `string` | Discount rule: `"PercentOf"`, `"FlatAmount"`, `"Tiered"` |
| `DiscountValue` | `decimal` | Discount percentage or amount |
| `MinQuantity` | `int?` | Minimum quantity for tiered discount |

| Output | Type | Description |
|---|---|---|
| `DiscountAmount` | `decimal` | Discount amount |
| `FinalAmount` | `decimal` | Amount after discount |
| `DiscountPercent` | `double` | Effective discount percentage |
| `Success` | `bool` | Whether calculation succeeded |

**Implementation:** Applies various discount models.

---

### `GenerateInvoiceNumber`
**File:** `Activities/BusinessLogic/GenerateInvoiceNumber.cs`
**Category:** `Business Logic`

| Input | Type | Description |
|---|---|---|
| `Prefix` | `string?` | Optional prefix (e.g., `"INV"`) |
| `Format` | `string?` | Format pattern (e.g., `"YYYY-MM-{seq}"`) |
| `Separator` | `string?` | Separator character (default: `"-"`) |

| Output | Type | Description |
|---|---|---|
| `InvoiceNumber` | `string` | Generated invoice number |
| `Success` | `bool` | Whether generation succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Generates sequential invoice numbers with custom formatting.

---

### `CheckInventory`
**File:** `Activities/BusinessLogic/CheckInventory.cs`
**Category:** `Business Logic`

| Input | Type | Description |
|---|---|---|
| `ProductId` | `string` | Product ID |
| `Quantity` | `int` | Quantity to check |
| `WarehouseId` | `string?` | Warehouse ID (optional, checks all if not set) |

| Output | Type | Description |
|---|---|---|
| `AvailableQuantity` | `int` | Available inventory |
| `IsAvailable` | `bool` | Whether enough quantity is available |
| `WarehouseDetails` | `string` | JSON inventory details by warehouse |
| `Success` | `bool` | Whether check succeeded |
| `Error` | `string?` | Error message if failed |

**Implementation:** Queries AppEnd's existing inventory system.

---

## Summary of Advanced Activities

**Total: 57 activities across 19 categories** 🆕

| # | Category | Activities |
|---|---|---|
| 7.28 | Calendar & Events | 4 |
| 7.29 | Cloud Storage | 6 |
| 7.30 | CRM Integration | 4 |
| 7.31 | E-commerce | 4 |
| 7.32 | Project Management | 4 |
| 7.33 | Message Queues | 5 |
| 7.34 | Webhooks & Events | 4 |
| 7.35 | RSS & Feeds | 3 |
| 7.36 | Payments | 4 |
| 7.37 | Analytics & Reporting | 3 |
| 7.38 | Social Media | 5 |
| 7.39 | Media Processing | 5 |
| 7.40 | Document Management | 4 |
| 7.41 | IoT & Sensors | 3 |
| 7.42 | ML/AI | 4 |
| 7.43 | Form Processing | 4 |
| 7.44 | Authentication | 4 |
| 7.45 | Data Enrichment | 3 |
| 7.46 | Business Logic | 4 |

---

**Status:** 🆕 Phase 3 Advanced — All 57 activities documented for enterprise integration scenarios.

**Complete:** Refer back to [Phase 7 Index](ElsaWF-07-Phase7-Index.md) for full navigation.
