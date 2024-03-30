namespace AppEndDbIO
{
    public class Where
    {
        public ConjunctiveOperator ConjunctiveOperator { set; get; } = ConjunctiveOperator.AND;
        public List<CompareClause>? CompareClauses { set; get; }
        public List<ComparePhrase>? SimpleClauses { set; get; }
        public List<Where>? ComplexClauses { set; get; }
    }

    public class CompareClause(string name, object? value = null)
	{
		public string Name { set; get; } = name;
		public object? Value { set; get; } = value;
		public CompareOperator CompareOperator { set; get; } = CompareOperator.Equal;
	}
	public class ComparePhrase(string phrase)
	{
		public string Phrase { set; get; } = phrase;
	}

}
