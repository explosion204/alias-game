namespace AliasGame.Service
{
    public class ApplicationOptions
    {
        public const string SectionName = "Project";

        public string ConnectionString { get; set; }
        public string SecretKey { get; set; }
    }
}