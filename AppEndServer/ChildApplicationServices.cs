using AppEndCommon;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace AppEndServer
{
    public class ChildAppInfo
    {
        public string AppName { get; set; } = "";
        public int Port { get; set; }
        public string Description { get; set; } = "";
        public bool IsRunning { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool AutoStart { get; set; }
        public string EnvironmentVariables { get; set; } = "";
        public int? ProcessId { get; set; }
    }

    public static class ChildApplicationServices
    {
        private static string ConfigFilePath => AppEndSettings.StandaloneWebApps + "/childapps.json";
        private static Dictionary<string, Process> RunningProcesses = new();

        static ChildApplicationServices()
        {
            if (!Directory.Exists(AppEndSettings.StandaloneWebApps))
            {
                Directory.CreateDirectory(AppEndSettings.StandaloneWebApps);
            }

            if (!File.Exists(ConfigFilePath))
            {
                File.WriteAllText(ConfigFilePath, "[]");
            }
        }

        public static List<ChildAppInfo> GetChildApps()
        {
            JArray apps = File.ReadAllText(ConfigFilePath).ToJArrayByNewtonsoft();
            List<ChildAppInfo> result = new();

            foreach (var app in apps)
            {
                var appInfo = new ChildAppInfo
                {
                    AppName = app["AppName"]?.ToString() ?? "",
                    Port = app["Port"]?.ToObject<int>() ?? 0,
                    Description = app["Description"]?.ToString() ?? "",
                    CreatedOn = app["CreatedOn"]?.ToObject<DateTime>() ?? DateTime.Now,
                    AutoStart = app["AutoStart"]?.ToObject<bool>() ?? false,
                    EnvironmentVariables = app["EnvironmentVariables"]?.ToString() ?? "",
                    ProcessId = app["ProcessId"]?.ToObject<int?>()
                };

                // Simple and reliable check: just verify the process is running
                appInfo.IsRunning = IsProcessRunning(appInfo.ProcessId);
                
                // If process is dead but still has a ProcessId, clear it
                if (!appInfo.IsRunning && appInfo.ProcessId.HasValue)
                {
                    app["ProcessId"] = null;
                    Console.WriteLine($"Process {appInfo.ProcessId} for '{appInfo.AppName}' is dead, clearing ProcessId");
                }

                result.Add(appInfo);
            }

            // Save any ProcessId updates
            File.WriteAllText(ConfigFilePath, apps.ToJsonStringByNewtonsoft(true));
            return result;
        }

        public static int GetAvailablePort()
        {
            var random = new Random();
            var apps = GetChildApps();
            var usedPorts = apps.Select(a => a.Port).ToHashSet();
            
            int port;
            int attempts = 0;
            do
            {
                port = random.Next(5000, 6000);
                attempts++;
            } while (usedPorts.Contains(port) && attempts < 100);
            
            return port;
        }

        public static bool CreateChildApp(string appName, int port, string description, string template)
        {
            if (string.IsNullOrWhiteSpace(appName))
                return false;

            JArray apps = File.ReadAllText(ConfigFilePath).ToJArrayByNewtonsoft();

            if (apps.Any(a => a["AppName"]?.ToString()?.Equals(appName, StringComparison.OrdinalIgnoreCase) == true))
                return false;

            if (apps.Any(a => a["Port"]?.ToObject<int>() == port))
                return false;

            string appPath = Path.Combine(AppEndSettings.StandaloneWebApps, appName);
            if (!Directory.Exists(appPath))
            {
                Directory.CreateDirectory(appPath);
            }

            // Create template files
            CreateTemplateFiles(appPath, appName, port, template);

            JObject newApp = new JObject
            {
                ["AppName"] = appName,
                ["Port"] = port,
                ["Description"] = description,
                ["CreatedOn"] = DateTime.Now,
                ["AutoStart"] = false,
                ["EnvironmentVariables"] = "",
                ["ProcessId"] = null,
                ["Template"] = template
            };

            apps.Add(newApp);
            File.WriteAllText(ConfigFilePath, apps.ToJsonStringByNewtonsoft(true));

            return true;
        }

        private static void CreateTemplateFiles(string appPath, string appName, int port, string template)
        {
            switch (template)
            {
                case "EmptyWebApi":
                    CreateEmptyWebApiTemplate(appPath, appName, port);
                    break;
                case "ApiWithClient":
                    CreateApiWithClientTemplate(appPath, appName, port);
                    break;
                default:
                    CreateEmptyWebApiTemplate(appPath, appName, port);
                    break;
            }
        }

        private static void CreateEmptyWebApiTemplate(string appPath, string appName, int port)
        {
            // Create .csproj file
            string csproj = $@"<Project Sdk=""Microsoft.NET.Sdk.Web"">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <InvariantGlobalization>false</InvariantGlobalization>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""Microsoft.AspNetCore.OpenApi"" Version=""10.0.0"" />
    <PackageReference Include=""Swashbuckle.AspNetCore"" Version=""6.4.6"" />
  </ItemGroup>

</Project>";
            File.WriteAllText(Path.Combine(appPath, $"{appName}.csproj"), csproj);

            // Create Program.cs with try-catch for debugging
            string programCs = $@"using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

try
{{
    var builder = WebApplication.CreateBuilder(args);
    
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    
    var app = builder.Build();
    
    if (app.Environment.IsDevelopment())
    {{
        app.UseSwagger();
        app.UseSwaggerUI();
    }}
    
    app.UseAuthorization();
    app.MapControllers();
    
    app.MapGet(""/health"", () => new {{ status = ""healthy"", app = ""{appName}"" }});
    
    Console.WriteLine($""[{appName}] Application starting on {{app.Urls.FirstOrDefault()}}"");
    await app.RunAsync();
}}
catch (Exception ex)
{{
    Console.Error.WriteLine($""Fatal error in {appName}: {{ex.Message}}"");
    Console.Error.WriteLine($""Stack: {{ex.StackTrace}}"");
    Environment.Exit(1);
}}
";
            File.WriteAllText(Path.Combine(appPath, "Program.cs"), programCs);

            // Create appsettings.json
            string appsettings = $@"{{
  ""Logging"": {{
    ""LogLevel"": {{
      ""Default"": ""Information"",
      ""Microsoft.AspNetCore"": ""Warning""
    }}
  }},
  ""AllowedHosts"": ""*"",
  ""AppName"": ""{appName}"",
  ""Port"": {port}
}}";
            File.WriteAllText(Path.Combine(appPath, "appsettings.json"), appsettings);

            // Create README.md
            string readme = $@"# {appName}

## Description
Empty Web API application

## Port
{port}

## Run
```bash
dotnet run
```

## Access
http://localhost:{port}
http://localhost:{port}/swagger
http://localhost:{port}/health
";
            File.WriteAllText(Path.Combine(appPath, "README.md"), readme);
        }

        private static void CreateApiWithClientTemplate(string appPath, string appName, int port)
        {
            // Create .csproj file
            string csproj = $@"<Project Sdk=""Microsoft.NET.Sdk.Web"">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <InvariantGlobalization>false</InvariantGlobalization>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""Microsoft.AspNetCore.OpenApi"" Version=""10.0.0"" />
    <PackageReference Include=""Swashbuckle.AspNetCore"" Version=""6.4.6"" />
  </ItemGroup>

</Project>";
            File.WriteAllText(Path.Combine(appPath, $"{appName}.csproj"), csproj);

            // Create Program.cs with try-catch and CORS
            string programCs = $@"using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

try
{{
    var builder = WebApplication.CreateBuilder(args);
    
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddCors(options =>
    {{
        options.AddDefaultPolicy(policy =>
        {{
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }});
    }});
    
    var app = builder.Build();
    
    if (app.Environment.IsDevelopment())
    {{
        app.UseSwagger();
        app.UseSwaggerUI();
    }}
    
    app.UseCors();
    app.UseStaticFiles();
    app.UseAuthorization();
    app.MapControllers();
    
    app.MapGet(""/api/hello"", () => new {{ message = ""Hello from {appName}!"", timestamp = DateTime.UtcNow }});
    app.MapGet(""/api/status"", () => new {{ status = ""running"", appName = ""{appName}"", port = {port} }});
    app.MapGet(""/health"", () => new {{ status = ""healthy"", app = ""{appName}"" }});
    
    Console.WriteLine($""[{appName}] Application starting on {{app.Urls.FirstOrDefault()}}"");
    await app.RunAsync();
}}
catch (Exception ex)
{{
    Console.Error.WriteLine($""Fatal error in {appName}: {{ex.Message}}"");
    Console.Error.WriteLine($""Stack: {{ex.StackTrace}}"");
    Environment.Exit(1);
}}
";
            File.WriteAllText(Path.Combine(appPath, "Program.cs"), programCs);

            // Create appsettings.json
            string appsettings = $@"{{
  ""Logging"": {{
    ""LogLevel"": {{
      ""Default"": ""Information"",
      ""Microsoft.AspNetCore"": ""Warning""
    }}
  }},
  ""AllowedHosts"": ""*"",
  ""AppName"": ""{appName}"",
  ""Port"": {port}
}}";
            File.WriteAllText(Path.Combine(appPath, "appsettings.json"), appsettings);

            // Create wwwroot folder and index.html
            string wwwrootPath = Path.Combine(appPath, "wwwroot");
            Directory.CreateDirectory(wwwrootPath);

            // Create minimal HTML
            var html = new System.Text.StringBuilder();
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html lang=\"en\">");
            html.AppendLine("<head>");
            html.AppendLine("    <meta charset=\"UTF-8\">");
            html.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            html.AppendLine($"    <title>{appName}</title>");
            html.AppendLine("    <style>");
            html.AppendLine("        * {{ margin: 0; padding: 0; box-sizing: border-box; }}");
            html.AppendLine("        body {{ font-family: sans-serif; background: #f5f5f5; display: flex; justify-content: center; align-items: center; min-height: 100vh; }}");
            html.AppendLine("        .container {{ background: white; padding: 40px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); max-width: 600px; text-align: center; }}");
            html.AppendLine("        h1 {{ color: #333; margin-bottom: 20px; }}");
            html.AppendLine("        button {{ padding: 10px 20px; margin: 5px; cursor: pointer; border: none; border-radius: 4px; background: #007bff; color: white; }}");
            html.AppendLine("        button:hover {{ background: #0056b3; }}");
            html.AppendLine("        .response {{ margin-top: 20px; background: #f9f9f9; padding: 15px; border-radius: 4px; text-align: left; white-space: pre-wrap; word-wrap: break-word; }}");
            html.AppendLine("    </style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            html.AppendLine("    <div class=\"container\">");
            html.AppendLine($"        <h1>{appName}</h1>");
            html.AppendLine("        <p>Welcome!</p>");
            html.AppendLine("        <button onclick=\"testHello()\">Test Hello API</button>");
            html.AppendLine("        <button onclick=\"testStatus()\">Test Status API</button>");
            html.AppendLine("        <div id=\"response\" class=\"response\" style=\"display:none;\"></div>");
            html.AppendLine("    </div>");
            html.AppendLine("    <script>");
            html.AppendLine("        async function testHello() {{");
            html.AppendLine("            try {{");
            html.AppendLine("                const res = await fetch('/api/hello');");
            html.AppendLine("                const data = await res.json();");
            html.AppendLine("                document.getElementById('response').style.display = 'block';");
            html.AppendLine("                document.getElementById('response').textContent = JSON.stringify(data, null, 2);");
            html.AppendLine("            }} catch (e) {{ alert('Error: ' + e.message); }}");
            html.AppendLine("        }}");
            html.AppendLine("        async function testStatus() {{");
            html.AppendLine("            try {{");
            html.AppendLine("                const res = await fetch('/api/status');");
            html.AppendLine("                const data = await res.json();");
            html.AppendLine("                document.getElementById('response').style.display = 'block';");
            html.AppendLine("                document.getElementById('response').textContent = JSON.stringify(data, null, 2);");
            html.AppendLine("            }} catch (e) {{ alert('Error: ' + e.message); }}");
            html.AppendLine("        }}");
            html.AppendLine("    </script>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            File.WriteAllText(Path.Combine(wwwrootPath, "index.html"), html.ToString());

            // Create README.md
            string readme = $@"# {appName}

## Description
Web API application with HTML client

## Port
{port}

## API Endpoints
- GET /api/hello
- GET /api/status
- GET /health

## Run
```bash
dotnet run
```

## Access
- http://localhost:{port}
- http://localhost:{port}/swagger
";
            File.WriteAllText(Path.Combine(appPath, "README.md"), readme);
        }

        public static bool DeleteChildApp(string appName)
        {
            JArray apps = File.ReadAllText(ConfigFilePath).ToJArrayByNewtonsoft();
            var app = apps.FirstOrDefault(a => a["AppName"]?.ToString()?.Equals(appName, StringComparison.OrdinalIgnoreCase) == true);

            if (app == null)
                return false;

            int? processId = app["ProcessId"]?.ToObject<int?>();
            if (IsProcessRunning(processId))
            {
                StopChildApp(appName);
            }

            apps.Remove(app);
            File.WriteAllText(ConfigFilePath, apps.ToJsonStringByNewtonsoft(true));

            string appPath = AppEndSettings.StandaloneWebApps + "/" + appName;
            if (Directory.Exists(appPath))
            {
                Directory.Delete(appPath, true);
            }

            return true;
        }

        public static bool RunChildApp(string appName)
        {
            JArray apps = File.ReadAllText(ConfigFilePath).ToJArrayByNewtonsoft();
            var app = apps.FirstOrDefault(a => a["AppName"]?.ToString()?.Equals(appName, StringComparison.OrdinalIgnoreCase) == true);

            if (app == null)
            {
                Console.WriteLine($"ERROR: Child app '{appName}' not found in config");
                return false;
            }

            int? processId = app["ProcessId"]?.ToObject<int?>();
            if (IsProcessRunning(processId))
            {
                Console.WriteLine($"Child app '{appName}' is already running (PID: {processId})");
                return true;
            }

            int port = app["Port"]?.ToObject<int>() ?? 0;
            string envVars = app["EnvironmentVariables"]?.ToString() ?? "";
            string appPath = Path.Combine(AppEndSettings.StandaloneWebApps, appName);

            Console.WriteLine($"\n{'='*60}");
            Console.WriteLine($"Starting child app '{appName}'");
            Console.WriteLine($"  Path: {appPath}");
            Console.WriteLine($"  Port: {port}");
            Console.WriteLine($"{'='*60}");

            try
            {
                // Verify directory exists
                if (!Directory.Exists(appPath))
                {
                    Console.WriteLine($"ERROR: App directory does not exist: {appPath}");
                    return false;
                }

                // Verify .csproj exists
                string csprojPath = Path.Combine(appPath, $"{appName}.csproj");
                if (!File.Exists(csprojPath))
                {
                    Console.WriteLine($"ERROR: .csproj file not found: {csprojPath}");
                    return false;
                }

                // Restore dependencies first
                Console.WriteLine($"Step 1/3: Restoring NuGet packages...");
                ProcessStartInfo restoreInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "restore",
                    WorkingDirectory = appPath,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                int restoreExitCode = -1;
                using (Process restoreProcess = Process.Start(restoreInfo))
                {
                    restoreProcess?.WaitForExit(120000);
                    restoreExitCode = restoreProcess?.ExitCode ?? -1;
                    
                    string restoreOutput = restoreProcess?.StandardOutput.ReadToEnd() ?? "";
                    string restoreError = restoreProcess?.StandardError.ReadToEnd() ?? "";
                    
                    if (restoreExitCode != 0)
                    {
                        Console.WriteLine($"  WARNING: dotnet restore failed with exit code {restoreExitCode}");
                        if (!string.IsNullOrWhiteSpace(restoreError))
                            Console.WriteLine($"  Stderr: {restoreError}");
                        if (!string.IsNullOrWhiteSpace(restoreOutput))
                            Console.WriteLine($"  Stdout: {restoreOutput}");
                    }
                    else
                    {
                        Console.WriteLine($"  ✓ NuGet packages restored");
                    }
                }

                // Build the project
                Console.WriteLine($"Step 2/3: Building project...");
                ProcessStartInfo buildInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "build --configuration Debug --no-restore",
                    WorkingDirectory = appPath,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                int buildExitCode = -1;
                using (Process buildProcess = Process.Start(buildInfo))
                {
                    buildProcess?.WaitForExit(120000);
                    buildExitCode = buildProcess?.ExitCode ?? -1;
                    
                    string buildOutput = buildProcess?.StandardOutput.ReadToEnd() ?? "";
                    string buildError = buildProcess?.StandardError.ReadToEnd() ?? "";
                    
                    if (buildExitCode != 0)
                    {
                        Console.WriteLine($"  ERROR: dotnet build failed with exit code {buildExitCode}");
                        if (!string.IsNullOrWhiteSpace(buildError))
                            Console.WriteLine($"  Build Error: {buildError}");
                        if (!string.IsNullOrWhiteSpace(buildOutput))
                            Console.WriteLine($"  Build Output: {buildOutput}");
                        return false;
                    }
                    else
                    {
                        Console.WriteLine($"  ✓ Project built successfully");
                    }
                }

                // Now run the application
                Console.WriteLine($"Step 3/3: Starting application...");
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"run --no-build --no-restore",
                    WorkingDirectory = appPath,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                // Set port via environment variable
                startInfo.EnvironmentVariables["ASPNETCORE_URLS"] = $"http://localhost:{port}";
                startInfo.EnvironmentVariables["ASPNETCORE_ENVIRONMENT"] = "Development";

                if (!string.IsNullOrWhiteSpace(envVars))
                {
                    foreach (var line in envVars.Split('\n'))
                    {
                        var parts = line.Trim().Split('=', 2);
                        if (parts.Length == 2)
                        {
                            startInfo.EnvironmentVariables[parts[0].Trim()] = parts[1].Trim();
                        }
                    }
                }

                Process process = Process.Start(startInfo);
                if (process != null)
                {
                    Console.WriteLine($"  ✓ Process started with PID: {process.Id}");

                    // Start reading output immediately
                    _ = Task.Run(() =>
                    {
                        try
                        {
                            using (var reader = process.StandardError)
                            {
                                string line;
                                while ((line = reader.ReadLine()) != null)
                                {
                                    if (!string.IsNullOrWhiteSpace(line))
                                    {
                                        Console.WriteLine($"  [{appName}] ERROR: {line}");
                                    }
                                }
                            }
                        }
                        catch { }
                    });

                    _ = Task.Run(() =>
                    {
                        try
                        {
                            using (var reader = process.StandardOutput)
                            {
                                string line;
                                while ((line = reader.ReadLine()) != null)
                                {
                                    if (!string.IsNullOrWhiteSpace(line))
                                    {
                                        Console.WriteLine($"  [{appName}] {line}");
                                    }
                                }
                            }
                        }
                        catch { }
                    });

                    app["ProcessId"] = process.Id;
                    File.WriteAllText(ConfigFilePath, apps.ToJsonStringByNewtonsoft(true));
                    RunningProcesses[appName] = process;
                    
                    // Wait 3 seconds to see if it crashes immediately
                    System.Threading.Thread.Sleep(3000);
                    
                    if (process.HasExited)
                    {
                        int exitCode = process.ExitCode;
                        Console.WriteLine($"ERROR: Process exited with code {exitCode}");
                        app["ProcessId"] = null;
                        File.WriteAllText(ConfigFilePath, apps.ToJsonStringByNewtonsoft(true));
                        RunningProcesses.Remove(appName);
                        return false;
                    }
                    
                    // Monitor for unexpected exit in background
                    _ = Task.Run(() =>
                    {
                        try
                        {
                            process.WaitForExit();
                            Console.WriteLine($"⚠ WARNING: Child app '{appName}' (PID: {process.Id}) exited with code {process.ExitCode}");
                            
                            // Clean up
                            app["ProcessId"] = null;
                            File.WriteAllText(ConfigFilePath, apps.ToJsonStringByNewtonsoft(true));
                            RunningProcesses.Remove(appName);
                        }
                        catch { }
                    });

                    Console.WriteLine($"{'='*60}");
                    return true;
                }
                else
                {
                    Console.WriteLine($"ERROR: Failed to start process for '{appName}'");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.GetType().Name}: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"  Inner: {ex.InnerException.Message}");
                }
                return false;
            }
        }

        public static bool StopChildApp(string appName)
        {
            JArray apps = File.ReadAllText(ConfigFilePath).ToJArrayByNewtonsoft();
            var app = apps.FirstOrDefault(a => a["AppName"]?.ToString()?.Equals(appName, StringComparison.OrdinalIgnoreCase) == true);

            if (app == null)
                return false;

            int? processId = app["ProcessId"]?.ToObject<int?>();

            try
            {
                if (RunningProcesses.TryGetValue(appName, out var process))
                {
                    if (!process.HasExited)
                    {
                        process.Kill(true);
                        process.WaitForExit(5000);
                    }
                    RunningProcesses.Remove(appName);
                }
                else if (processId.HasValue)
                {
                    var processById = Process.GetProcessById(processId.Value);
                    if (processById != null && !processById.HasExited)
                    {
                        processById.Kill(true);
                        processById.WaitForExit(5000);
                    }
                }

                app["ProcessId"] = null;
                File.WriteAllText(ConfigFilePath, apps.ToJsonStringByNewtonsoft(true));
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to stop child app {appName}: {ex.Message}");
                app["ProcessId"] = null;
                File.WriteAllText(ConfigFilePath, apps.ToJsonStringByNewtonsoft(true));
            }

            return false;
        }

        public static ChildAppInfo? GetChildAppConfig(string appName)
        {
            JArray apps = File.ReadAllText(ConfigFilePath).ToJArrayByNewtonsoft();
            var app = apps.FirstOrDefault(a => a["AppName"]?.ToString()?.Equals(appName, StringComparison.OrdinalIgnoreCase) == true);

            if (app == null)
                return null;

            return new ChildAppInfo
            {
                AppName = app["AppName"]?.ToString() ?? "",
                Port = app["Port"]?.ToObject<int>() ?? 0,
                Description = app["Description"]?.ToString() ?? "",
                CreatedOn = app["CreatedOn"]?.ToObject<DateTime>() ?? DateTime.Now,
                AutoStart = app["AutoStart"]?.ToObject<bool>() ?? false,
                EnvironmentVariables = app["EnvironmentVariables"]?.ToString() ?? "",
                ProcessId = app["ProcessId"]?.ToObject<int?>()
            };
        }

        public static bool UpdateChildAppConfig(string appName, int port, string description, bool autoStart, string environmentVariables)
        {
            JArray apps = File.ReadAllText(ConfigFilePath).ToJArrayByNewtonsoft();
            var app = apps.FirstOrDefault(a => a["AppName"]?.ToString()?.Equals(appName, StringComparison.OrdinalIgnoreCase) == true);

            if (app == null)
                return false;

            app["Port"] = port;
            app["Description"] = description;
            app["AutoStart"] = autoStart;
            app["EnvironmentVariables"] = environmentVariables;

            File.WriteAllText(ConfigFilePath, apps.ToJsonStringByNewtonsoft(true));
            return true;
        }

        private static bool IsProcessRunning(int? processId)
        {
            if (!processId.HasValue)
                return false;

            try
            {
                var process = Process.GetProcessById(processId.Value);
                return process != null && !process.HasExited;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsPortListening(int port)
        {
            try
            {
                using (var client = new System.Net.Sockets.TcpClient())
                {
                    var result = client.BeginConnect("localhost", port, null, null);
                    bool success = result.AsyncWaitHandle.WaitOne(1000, false);
                    
                    if (success && client.Connected)
                    {
                        client.EndConnect(result);
                        return true;
                    }
                    else if (!success)
                    {
                        client.Close();
                    }
                }
            }
            catch
            {
                // Port not listening
            }
            return false;
        }

        public static void StartAutoStartApps()
        {
            var apps = GetChildApps();
            foreach (var app in apps.Where(a => a.AutoStart))
            {
                RunChildApp(app.AppName);
            }
        }

        public static string GetChildAppDiagnostics(string appName)
        {
            try
            {
                var appInfo = GetChildAppConfig(appName);
                if (appInfo == null)
                    return $"ERROR: App '{appName}' not found";

                var sb = new System.Text.StringBuilder();
                sb.AppendLine($"=== Diagnostics for '{appName}' ===");
                sb.AppendLine($"App Name: {appInfo.AppName}");
                sb.AppendLine($"Port: {appInfo.Port}");
                sb.AppendLine($"Description: {appInfo.Description}");
                sb.AppendLine($"Process ID: {appInfo.ProcessId ?? 0}");
                sb.AppendLine($"Is Running: {appInfo.IsRunning}");
                sb.AppendLine($"Auto Start: {appInfo.AutoStart}");

                string appPath = Path.Combine(AppEndSettings.StandaloneWebApps, appName);
                sb.AppendLine($"\nDirectory: {appPath}");
                sb.AppendLine($"Directory Exists: {Directory.Exists(appPath)}");
                
                if (Directory.Exists(appPath))
                {
                    string csprojPath = Path.Combine(appPath, $"{appName}.csproj");
                    sb.AppendLine($"Csproj File: {csprojPath}");
                    sb.AppendLine($"Csproj Exists: {File.Exists(csprojPath)}");

                    string programPath = Path.Combine(appPath, "Program.cs");
                    sb.AppendLine($"Program.cs Exists: {File.Exists(programPath)}");

                    string settingsPath = Path.Combine(appPath, "appsettings.json");
                    sb.AppendLine($"appsettings.json Exists: {File.Exists(settingsPath)}");

                    var files = Directory.GetFiles(appPath);
                    sb.AppendLine($"Files in directory: {files.Length}");
                    foreach (var file in files)
                    {
                        sb.AppendLine($"  - {Path.GetFileName(file)}");
                    }
                }

                if (appInfo.ProcessId.HasValue)
                {
                    try
                    {
                        var process = Process.GetProcessById(appInfo.ProcessId.Value);
                        sb.AppendLine($"\nProcess Status:");
                        sb.AppendLine($"  Process Name: {process.ProcessName}");
                        sb.AppendLine($"  Has Exited: {process.HasExited}");
                        sb.AppendLine($"  Working Set: {process.WorkingSet64 / 1024 / 1024} MB");
                        if (process.HasExited)
                        {
                            sb.AppendLine($"  Exit Code: {process.ExitCode}");
                        }
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine($"\nProcess Error: {ex.Message}");
                    }
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                return $"ERROR: {ex.Message}";
            }
        }
    }
}
