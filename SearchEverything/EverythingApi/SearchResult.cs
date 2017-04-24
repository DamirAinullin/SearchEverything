namespace SearchEverything.EverythingApi
{
    public class SearchResult
    {
        public string FullPath { get; set; }
        public string ShowPath { get; set; }
        public ResultType Type { get; set; }
        public string IconPath { get; set; }
    }
}