using System.Collections.Generic;

namespace VVVVID_Downloader
{
    public class RootObject
    {
        public string result { get; set; }
        public string message { get; set; }
        public List<Anime> data { get; set; }
    }

    public class RootEpisodeObject
    {
        public string result { get; set; }
        public string message { get; set; }
        public List<Episode> data { get; set; }
    }
}
