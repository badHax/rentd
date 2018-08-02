namespace Rentd.IdServer
{
    public class AppSettings
    {
        public BaseUrls BaseUrls { get; set; }
    }

    public class BaseUrls
    {
        public string Web { get; set; }
        public string ProfileApi { get; set; }
        public string App { get; set; }
    }
}
