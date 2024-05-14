using Newtonsoft.Json.Linq;

namespace AppEndCommon
{
    public record AppEndPackage 
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Note { get; set; }
        public string Version { get; set; }
        public string Url { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string InstallSql { get; set; }
        public string UnInstallSql { get; set; }
        public bool Installed { get; set; }
        public string InstalledBy { get; set; }
        public DateTime InstalledOn { get; set; }
        public JArray MenuItems { get; set; }

        public AppEndPackage()
        {
            Name = "";
            Title = "";
            Note = "";
            Version = "";
            Url = "";
            CreatedBy = "";
            CreatedOn = DateTime.Now;
            UpdatedBy = "";
            UpdatedOn = DateTime.Now;
            InstallSql = "";
            UnInstallSql = "";
            Installed = false;
            InstalledBy = "";
            InstalledOn = DateTime.Now;
            MenuItems = [];
        }
    }

}
