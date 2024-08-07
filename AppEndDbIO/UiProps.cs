namespace AppEndDbIO
{
	public class UiProps
    {
        public string? Group { set; get; } = "";
		public UiWidget UiWidget { set; get; } = UiWidget.Textbox;
		public string UiWidgetOptions { set; get; } = "{}";

		public SearchType SearchType { set; get; } = SearchType.None;
		public bool SearchMultiselect { set; get; } = false;

		public bool IsDisabled { set; get; } = false;

        public bool? Required { set; get; }
        public string? ValidationRule { set; get; }

        public string? Note { set; get; }
    }
}
