using System;
using System.Linq;

namespace AppEndServer
{
	/// <summary>
	/// Cron expression parser and evaluator
	/// Format: Minute Hour DayOfMonth Month DayOfWeek
	/// </summary>
	public class CronExpression
	{
		public string Expression { get; }
		private readonly int[] _minutes;
		private readonly int[] _hours;
		private readonly int[] _daysOfMonth;
		private readonly int[] _months;
		private readonly int[] _daysOfWeek;

		public CronExpression(string expression)
		{
			Expression = NormalizeExpression(expression);
			var parts = Expression.Split(' ');
			
			if (parts.Length != 5)
				throw new ArgumentException("Cron expression must have 5 parts: Minute Hour DayOfMonth Month DayOfWeek");

			_minutes = ParseField(parts[0], 0, 59);
			_hours = ParseField(parts[1], 0, 23);
			_daysOfMonth = ParseField(parts[2], 1, 31);
			_months = ParseField(parts[3], 1, 12);
			_daysOfWeek = ParseField(parts[4], 0, 6);
		}

		private static string NormalizeExpression(string expr)
		{
			return expr.Trim().ToLower() switch
			{
				"@hourly" => "0 * * * *",
				"@daily" or "@midnight" => "0 0 * * *",
				"@weekly" => "0 0 * * 0",
				"@monthly" => "0 0 1 * *",
				"@yearly" or "@annually" => "0 0 1 1 *",
				_ => expr.Trim()
			};
		}

		private static int[] ParseField(string field, int min, int max)
		{
			if (field == "*")
				return Enumerable.Range(min, max - min + 1).ToArray();

			if (field.StartsWith("*/"))
			{
				int step = int.Parse(field[2..]);
				return Enumerable.Range(min, max - min + 1).Where(x => (x - min) % step == 0).ToArray();
			}

			if (field.Contains('-'))
			{
				var range = field.Split('-');
				int start = int.Parse(range[0]);
				int end = int.Parse(range[1]);
				return Enumerable.Range(start, end - start + 1).ToArray();
			}

			if (field.Contains(','))
				return field.Split(',').Select(int.Parse).ToArray();

			return [int.Parse(field)];
		}

		public DateTime? GetNextRunTime(DateTime from)
		{
			var candidate = new DateTime(from.Year, from.Month, from.Day, from.Hour, from.Minute, 0).AddMinutes(1);
			var maxTime = from.AddYears(1);

			while (candidate < maxTime)
			{
				if (IsMatch(candidate))
					return candidate;
				candidate = candidate.AddMinutes(1);
			}

			return null;
		}

		public bool IsMatch(DateTime time)
		{
			return _minutes.Contains(time.Minute) &&
				   _hours.Contains(time.Hour) &&
				   _daysOfMonth.Contains(time.Day) &&
				   _months.Contains(time.Month) &&
				   _daysOfWeek.Contains((int)time.DayOfWeek);
		}

		public string GetDescription()
		{
			if (Expression == "* * * * *") return "Every minute";
			if (Expression.StartsWith("*/")) return $"Every {Expression.Split(' ')[0][2..]} minutes";
			if (Expression == "0 * * * *") return "Every hour";
			if (Expression == "0 0 * * *") return "Daily at midnight";
			if (Expression == "0 0 * * 0") return "Weekly on Sunday";
			if (Expression == "0 0 1 * *") return "Monthly on 1st";
			return Expression;
		}
	}
}
