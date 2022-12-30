namespace ClinicService.Core
{
    public class Constants
    {
        public const string AppSettingsJSON = "appsettings.json";
        public const string True = "True";
        public const string False = "False";

        public static class SQLProvider
        {
            public const string ConnectionStringPath = "Settings:DataBaseOptions:ConnectionString";
            public const string MSSQL = "mssql";
        }
    }
}
