using AppEndWorkflow.Activities.LLM;
using AppEndWorkflow.Activities.Email;
using AppEndWorkflow.Activities.Notifications;
using AppEndWorkflow.Activities.DataConversion;
using Elsa.Workflows;
using Microsoft.Extensions.Configuration;

namespace AppEndWorkflow.Tests;

/// <summary>
/// Integration tests for Phase 2 Extended Activities.
/// These are example tests - run with proper configuration.
/// </summary>
public class Phase2ActivitiesTests
{
    private readonly IConfiguration _configuration;

    public Phase2ActivitiesTests()
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddUserSecrets<Phase2ActivitiesTests>(optional: true)
            .AddEnvironmentVariables();

        _configuration = builder.Build();
    }

    // ============ AI/LLM Tests ============

    public void TestChatWithOpenAI()
    {
        Console.WriteLine("Testing ChatWithLlmActivity...");

        // Create mock context
        var activity = new ChatWithLlmActivity();

        // Test inputs
        var inputs = new Dictionary<string, object?>
        {
            { nameof(ChatWithLlmActivity.Message), "What is the capital of France?" },
            { nameof(ChatWithLlmActivity.Model), "gpt-3.5-turbo" },
            { nameof(ChatWithLlmActivity.Provider), "OpenAI" },
            { nameof(ChatWithLlmActivity.Temperature), 0.7f }
        };

        // In real test: Create ActivityExecutionContext and execute
        // context.Set(ChatWithLlmActivity.Message, "What is the capital of France?");
        // activity.Execute(context);

        Console.WriteLine("✓ ChatWithLlmActivity test structure created");
    }

    public void TestSummarize()
    {
        Console.WriteLine("Testing SummarizeActivity...");

        var longText = @"
            This is a long article about the importance of renewable energy in fighting climate change...
            It contains multiple paragraphs and detailed information about solar, wind, and hydroelectric power.
        ";

        var activity = new SummarizeActivity();

        var inputs = new Dictionary<string, object?>
        {
            { nameof(SummarizeActivity.Content), longText },
            { nameof(SummarizeActivity.Provider), "OpenAI" },
            { nameof(SummarizeActivity.MaxLength), 100 }
        };

        Console.WriteLine("✓ SummarizeActivity test structure created");
    }

    public void TestTranslate()
    {
        Console.WriteLine("Testing TranslateActivity...");

        var activity = new TranslateActivity();

        var inputs = new Dictionary<string, object?>
        {
            { nameof(TranslateActivity.Text), "Hello, how are you?" },
            { nameof(TranslateActivity.SourceLanguage), "en" },
            { nameof(TranslateActivity.TargetLanguage), "fa" },
            { nameof(TranslateActivity.Provider), "OpenAI" }
        };

        Console.WriteLine("✓ TranslateActivity test structure created");
    }

    public void TestGenerateContent()
    {
        Console.WriteLine("Testing GenerateContentActivity...");

        var activity = new GenerateContentActivity();

        var inputs = new Dictionary<string, object?>
        {
            { nameof(GenerateContentActivity.Prompt), "Write a short product description for a smartphone" },
            { nameof(GenerateContentActivity.ContentType), "Article" },
            { nameof(GenerateContentActivity.Tone), "Professional" },
            { nameof(GenerateContentActivity.Length), "Medium" },
            { nameof(GenerateContentActivity.Provider), "OpenAI" }
        };

        Console.WriteLine("✓ GenerateContentActivity test structure created");
    }

    // ============ Email Tests ============

    public void TestSendBulkEmail()
    {
        Console.WriteLine("Testing SendBulkEmailActivity...");

        var recipientsJson = @"[
            { ""Email"": ""user1@example.com"", ""Name"": ""User One"", ""ActivationLink"": ""https://app.local/activate/123"" },
            { ""Email"": ""user2@example.com"", ""Name"": ""User Two"", ""ActivationLink"": ""https://app.local/activate/456"" }
        ]";

        var activity = new SendBulkEmailActivity();

        var inputs = new Dictionary<string, object?>
        {
            { nameof(SendBulkEmailActivity.Recipients), recipientsJson },
            { nameof(SendBulkEmailActivity.TemplateName), "<h1>Welcome {{Name}}!</h1>" },
            { nameof(SendBulkEmailActivity.Subject), "Welcome to AppEnd!" },
            { nameof(SendBulkEmailActivity.BatchSize), 100 },
            { nameof(SendBulkEmailActivity.DelayMs), 1000 }
        };

        Console.WriteLine("✓ SendBulkEmailActivity test structure created");
    }

    public void TestSendEmailWithAttachments()
    {
        Console.WriteLine("Testing SendEmailWithAttachmentsActivity...");

        var attachmentsJson = @"[
            { ""FilePath"": ""./files/report.pdf"", ""FileName"": ""Monthly Report.pdf"", ""ContentType"": ""application/pdf"" },
            { ""FilePath"": ""./files/data.xlsx"", ""FileName"": ""Data.xlsx"", ""ContentType"": ""application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"" }
        ]";

        var activity = new SendEmailWithAttachmentsActivity();

        var inputs = new Dictionary<string, object?>
        {
            { nameof(SendEmailWithAttachmentsActivity.To), "recipient@example.com" },
            { nameof(SendEmailWithAttachmentsActivity.Subject), "Your Monthly Reports" },
            { nameof(SendEmailWithAttachmentsActivity.Body), "<p>Please find attached your reports.</p>" },
            { nameof(SendEmailWithAttachmentsActivity.Attachments), attachmentsJson }
        };

        Console.WriteLine("✓ SendEmailWithAttachmentsActivity test structure created");
    }

    // ============ Notifications Tests ============

    public void TestSendSlack()
    {
        Console.WriteLine("Testing SendSlackActivity...");

        var activity = new SendSlackActivity();

        var inputs = new Dictionary<string, object?>
        {
            { nameof(SendSlackActivity.ChannelId), "#notifications" },
            { nameof(SendSlackActivity.Message), "🚀 New workflow started!" },
            { nameof(SendSlackActivity.BotToken), null } // Falls back to config
        };

        Console.WriteLine("✓ SendSlackActivity test structure created");
    }

    public void TestSendWhatsapp()
    {
        Console.WriteLine("Testing SendWhatsappActivity...");

        var activity = new SendWhatsappActivity();

        var inputs = new Dictionary<string, object?>
        {
            { nameof(SendWhatsappActivity.PhoneNumber), "+989123456789" },
            { nameof(SendWhatsappActivity.Message), "Hello! Your verification code is: 123456" },
            { nameof(SendWhatsappActivity.Provider), "Twilio" }
        };

        Console.WriteLine("✓ SendWhatsappActivity test structure created");
    }

    // ============ Data Conversion Tests ============

    public void TestJsonToXml()
    {
        Console.WriteLine("Testing ConvertJsonToXmlActivity...");

        var jsonInput = @"{
            ""order"": {
                ""id"": 123,
                ""customer"": ""John Doe"",
                ""items"": [
                    { ""product"": ""Laptop"", ""quantity"": 1, ""price"": 1000 },
                    { ""product"": ""Mouse"", ""quantity"": 2, ""price"": 25 }
                ]
            }
        }";

        var activity = new ConvertJsonToXmlActivity();

        var inputs = new Dictionary<string, object?>
        {
            { nameof(ConvertJsonToXmlActivity.InputJson), jsonInput },
            { nameof(ConvertJsonToXmlActivity.RootElementName), "root" },
            { nameof(ConvertJsonToXmlActivity.AttributePrefix), "@" }
        };

        Console.WriteLine("✓ ConvertJsonToXmlActivity test structure created");
    }

    public void TestXmlToJson()
    {
        Console.WriteLine("Testing ConvertXmlToJsonActivity...");

        var xmlInput = @"<?xml version=""1.0""?>
        <root>
            <order>
                <id>123</id>
                <customer>John Doe</customer>
                <items>
                    <item>Laptop</item>
                    <item>Mouse</item>
                </items>
            </order>
        </root>";

        var activity = new ConvertXmlToJsonActivity();

        var inputs = new Dictionary<string, object?>
        {
            { nameof(ConvertXmlToJsonActivity.InputXml), xmlInput },
            { nameof(ConvertXmlToJsonActivity.PreserveAttributes), true }
        };

        Console.WriteLine("✓ ConvertXmlToJsonActivity test structure created");
    }

    // ============ Summary ============

    public void RunAllTests()
    {
        Console.WriteLine("\n═══════════════════════════════════════════════════");
        Console.WriteLine("Phase 2 Extended Activities Test Suite");
        Console.WriteLine("═══════════════════════════════════════════════════\n");

        Console.WriteLine("📋 AI/LLM Activities:");
        TestChatWithOpenAI();
        TestSummarize();
        TestTranslate();
        TestGenerateContent();

        Console.WriteLine("\n📧 Email Activities:");
        TestSendBulkEmail();
        TestSendEmailWithAttachments();

        Console.WriteLine("\n📢 Notification Activities:");
        TestSendSlack();
        TestSendWhatsapp();

        Console.WriteLine("\n🔄 Data Conversion Activities:");
        TestJsonToXml();
        TestXmlToJson();

        Console.WriteLine("\n═══════════════════════════════════════════════════");
        Console.WriteLine("✅ All activity test structures created successfully!");
        Console.WriteLine("═══════════════════════════════════════════════════\n");

        Console.WriteLine("Next Steps:");
        Console.WriteLine("1. Configure appsettings.json with API keys");
        Console.WriteLine("2. Install required NuGet packages");
        Console.WriteLine("3. Create proper ActivityExecutionContext for tests");
        Console.WriteLine("4. Test activities in Elsa Dashboard");
    }
}
