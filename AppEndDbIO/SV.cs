using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppEndDbIO
{
    public static class LibSV
    {
        public static readonly string CreatedBy = "CreatedBy";
        public static readonly string CreatedOn = "CreatedOn";

        public static readonly string UpdatedBy = "UpdatedBy";
        public static readonly string UpdatedOn = "UpdatedOn";

        public static readonly string HistoryBy = "HistoryBy";
        public static readonly string HistoryOn = "HistoryOn";

        public static readonly string StateBy = "StateBy";
        public static readonly string StateOn = "StateOn";

        public static readonly List<string> AuditingFields = ["CreatedBy", "CreatedOn", "UpdatedBy", "UpdatedOn"];
        public static readonly List<string> UpdatedFields = ["UpdatedBy", "UpdatedOn"];
        public static readonly List<string> CreatedFields = ["CreatedBy", "CreatedOn"];


        public static readonly string ViewOrder = "ViewOrder";
		public static readonly string ReadByKey = "ReadByKey";
		public static readonly string Update = "Update";
		public static readonly string AsStr = " AS ";
	}
}
