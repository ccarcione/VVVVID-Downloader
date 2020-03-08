using System.Collections.Generic;

namespace VVVVID_Downloader.ConsoleApp
{
    public class Anime
    {
        public int id { get; set; }
        public int show_id { get; set; }
        public int season_id { get; set; }
        public int show_type { get; set; }
        public int number { get; set; }
        public List<Episode> episodes { get; set; }
        public string name { get; set; }
        public string title { get; set; }

    }
}
