using AppEndCommon;

namespace AppEndDbIO
{
    public class DbColumn(string name)
	{
		public string Name { set; get; } = name;
		public string DevNote { set; get; } = "";
		public bool IsPrimaryKey { set; get; } = false;
        public string DbType { set; get; } = "VARCHAR";
        public string? Size { set; get; } 
        public bool IsIdentity { set; get; } = false;
        public string? IdentityStart { set; get; }
        public string? IdentityStep { set; get; }
        public bool AllowNull { set; get; }
        public string? DbDefault { set; get; }
        public DbFk? Fk { set; get; }
		public bool? IsHumanId { set; get; }
		public bool? IsSortable { set; get; }

		public string? UpdateGroup { set; get; } = "";

		public UiProps? UiProps { set; get; }

		public bool IsAuditing()
		{
            if(LibSV.AuditingFields.ContainsIgnoreCase(Name)) return true;
			return false;
		}
		public bool IsFileOrRelatedColumns()
		{
            if (DbType.EqualsIgnoreCase("image")) return true;
			if (Name.EndsWithIgnoreCase("_filesize")) return true;
			if (Name.EndsWithIgnoreCase("_filemime")) return true;
			if (Name.EndsWithIgnoreCase("_filename")) return true;
			return false;
		}
		public bool IsNumerical()
		{
			if (DbType.ContainsIgnoreCase("int")) return true;
			if (DbType.ContainsIgnoreCase("numeric")) return true;
			if (DbType.ContainsIgnoreCase("real")) return true;
			if (DbType.ContainsIgnoreCase("money")) return true;
			if (DbType.ContainsIgnoreCase("float")) return true;
			if (DbType.ContainsIgnoreCase("numeric")) return true;
			return false;
		}
		public bool IsLargContent()
		{
			if (DbType.EqualsIgnoreCase("text")) return true;
			if (DbType.EqualsIgnoreCase("ntext")) return true;
			if ((DbType.EqualsIgnoreCase("varchar") || DbType.EqualsIgnoreCase("nvarchar")) && Size?.ToIntSafe() > 512) return true;
			if (DbType.EqualsIgnoreCase("image") && !Name.EndsWith("_xs")) return true;

			return false;
		}
		public bool IsDateTime()
        {
            return DbType.EqualsIgnoreCase("datetime");
        }

		public bool IsDate()
		{
			return DbType.EqualsIgnoreCase("date");
		}
		public bool IsString()
		{
			return DbType.ContainsIgnoreCase("char") || DbType.ContainsIgnoreCase("text");
		}

		public UiWidget CalculateBestUiWidget()
        {
            if (IsIdentity) return UiWidget.NoWidget;

            if (IsAuditing()) return UiWidget.DisabledTextbox;

            if (Fk is not null) return UiWidget.Combo;			
            
            if (DbType.EqualsIgnoreCase("bit")) return UiWidget.Checkbox;

            if (DbType.EqualsIgnoreCase("image") && Name.StartsWith("picture", StringComparison.OrdinalIgnoreCase)) return UiWidget.ImageView;
			if (DbType.EqualsIgnoreCase("image")) return UiWidget.FileView;
			
            if (DbType.ContainsIgnoreCase("datetime")) return UiWidget.DateTimePicker;
            if (DbType.EqualsIgnoreCase("date")) return UiWidget.DatePicker;
            if (DbType.EqualsIgnoreCase("time")) return UiWidget.TimePicker;

			if (Name.ContainsIgnoreCase("html")) return UiWidget.Htmlbox;

			if (DbType.EqualsIgnoreCase("text")) return UiWidget.MultilineTextbox;
            if (DbType.EqualsIgnoreCase("ntext")) return UiWidget.MultilineTextbox;
            if (Size is not null && Size.ToIntSafe() > 160) return UiWidget.MultilineTextbox;

            if (IsNumerical()) return UiWidget.Textbox;

            return UiWidget.Textbox;
        }

        public bool CalculateIsDisabled()
        {
            if (IsIdentity || IsAuditing()) return true;
            return false;
        }


        public DbColumnChangeTrackable ToDbColumnChangeTrackable()
        {
            return new DbColumnChangeTrackable(this.Name)
            {
                AllowNull = this.AllowNull,
                DbDefault = this.DbDefault,
                DbType = this.DbType,
                Fk = this.Fk,
                IsHumanId = this.IsHumanId,
                IdentityStart = this.IdentityStart,
                IdentityStep = this.IdentityStep,
                IsIdentity = this.IsIdentity,
                IsPrimaryKey = this.IsPrimaryKey,
                Size = this.Size,
                InitialName = "",
                State = ""
            };
        }

       

    }
}
