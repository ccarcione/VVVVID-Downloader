namespace VVVVID_Downloader.ConsoleApp
{
    public class Episode
    {
        public int id { get; set; }
        public int season_id { get; set; }
        public int video_id { get; set; }
        public string number { get; set; }
        public string title { get; set; }
        public string thumbnail { get; set; }
        public long views { get; set; }
        public long length { get; set; }
        public string description { get; set; }
        public bool expired { get; set; }
        public bool seen { get; set; }
        public bool playable { get; set; }
        public int ondemand_type { get; set; }
        public int vod_mode { get; set; }
        public string videoLink { get; set; }
        public string embed_info { get; set; }
        public string embed_info_sd { get; set; }
        public long video_shares { get; set; }
        public long video_likes { get; set; }
    }
}
