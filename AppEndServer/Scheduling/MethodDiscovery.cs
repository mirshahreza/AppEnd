using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using AppEndCommon;
using AppEndDynaCode;

namespace AppEndServer
{
	public static class MethodDiscovery
	{
		public static List<SchedulableMethod> GetAllMethods()
		{
			var methods = new List<SchedulableMethod>();

			try
			{
				// Get all types from DynaAsm
				var types = DynaCode.DynaAsm?.GetTypes() ?? [];

				foreach (var type in types)
				{
					// Skip non-public or non-static classes
					if (!type.IsClass || !type.IsPublic)
						continue;

					// Get namespace - ensure it's simple (no dots)
					var ns = type.Namespace;
					if (string.IsNullOrEmpty(ns))
						continue;

					// Extract simple namespace (first part only)
					var simpleName = ns.Contains('.') ? ns.Split('.')[0] : ns;

					// Get all public static methods
					var publicMethods = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

					foreach (var method in publicMethods)
					{
						// Skip special methods
						if (method.IsSpecialName || method.Name.StartsWith("get_") || method.Name.StartsWith("set_"))
							continue;

						// Build method full name: Namespace.ClassName.MethodName
						string methodFullName = $"{simpleName}.{type.Name}.{method.Name}";

						// Validate 3 parts
						if (methodFullName.Split('.').Length != 3)
							continue;

						var parameters = GetParameters(method);

						methods.Add(new SchedulableMethod
						{
							MethodFullName = methodFullName,
							Namespace = simpleName,
							ClassName = type.Name,
							MethodName = method.Name,
							ReturnType = method.ReturnType.Name,
							Parameters = parameters
						});
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[MethodDiscovery] Error: {ex.Message}");
			}

			return methods.OrderBy(m => m.MethodFullName).ToList();
		}

		private static List<MethodParameter> GetParameters(MethodInfo method)
		{
			var parameters = new List<MethodParameter>();

			foreach (var param in method.GetParameters())
			{
				// Skip Actor parameter
				if (param.ParameterType == typeof(AppEndUser))
					continue;

				parameters.Add(new MethodParameter
				{
					Name = param.Name ?? "",
					Type = param.ParameterType.Name,
					IsOptional = param.HasDefaultValue,
					DefaultValue = param.DefaultValue
				});
			}

			return parameters;
		}
	}

	public static class MethodExecutor
	{
		public static object? ExecuteMethod(string methodFullName, string? parametersJson)
		{
			// Validate format
			var parts = methodFullName.Split('.');
			if (parts.Length != 3)
				throw new ArgumentException($"Invalid method format: {methodFullName}");

			// Parse parameters
			JsonElement? jsonParams = null;
			if (!string.IsNullOrEmpty(parametersJson))
			{
				jsonParams = JsonSerializer.Deserialize<JsonElement>(parametersJson);
			}

			// Execute via DynaCode
			var result = DynaCode.InvokeByJsonInputs(
				methodFullName,
				jsonParams,
				dynaUser: null,
				clientIp: "Scheduler",
				clientAgent: "SchedulerService"
			);

			if (!result.IsSucceeded && result.Result is Exception ex)
				throw ex;

			return result.Result;
		}
	}
}
