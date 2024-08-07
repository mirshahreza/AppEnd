using System.Text.Json.Serialization;

namespace AppEndDbIO
{
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum OpenningPlace
	{
		Unknown,
		InlineDialog,
		NewWindow,
        Both
	}

	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum SearchType
	{
		None,
		Fast,
		Expandable
	}


	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum ServerType
	{
		Unknown,
		MsSql,
		MySql,
		Oracle,
		Postgres
	}

	[JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DbObjectType
    {
        Unknown,
        Table,
        View,
        Procedure,
        ScalarFunction,
        TableFunction
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum QueryType
    {
        Unknown,
        Create,
        ReadList,
        AggregatedReadList,
        ReadByKey,
        UpdateByKey,
        DeleteByKey,
        Delete,
        Procedure,
        TableFunction,
        ScalarFunction
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ConjunctiveOperator
    {
        Unknown,
        AND,
        OR
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CompareOperator
    {
		Equal,
		NotEqual,
		Contains,
        StartsWith,
        EndsWith,
        MoreThan,
        MoreThanOrEqual,
        LessThan,
        LessThanOrEqual,
        In,
        NotIn,
        IsNull,
        IsNotNull
    }

	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum RelationType
	{
		Unknown,
		ManyToMany,
		OneToMany
	}

	[JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OrderDirection
    {
        Unknown,
        ASC,
        DESC
    }

	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum RelationUiWidget
	{
		Unknown,
		CheckboxList,
		AddableList,
		Grid,
		Cards
	}

	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum UiWidget
	{
		NoWidget,
		Textbox,
        DisabledTextbox,
        MultilineTextbox,
		Htmlbox,
		CodeEditorbox,
		Sliderbox,
		Checkbox,
		Combo,
		Radio,
		DateTimePicker,
		DatePicker,
		TimePicker,
		ImageView,
		FileView,
		ColorPicker,
        ObjectPicker
	}

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Containment
    {
        IncludeAll,
        IncludeIndicatedItems,
        ExcludeAll,
        ExcludeIndicatedItems
    }

}
