using Newtonsoft.Json;
using NYoutubeDL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using VVVVID_Downloader.Hds;
using static VVVVID_Downloader.HdsDump;

namespace VVVVID_Downloader
{
    public class VVVID
    {
        private static CookieContainer _cookieContainer = new CookieContainer();
        private static String _connectionId;
        public Anime Anime;

        private HdsDump _hds;
        private TimeSpan _downloadedTS;
        public TimeSpan DownloadedTS { get => _downloadedTS; private set { _downloadedTS = value; } }
        private int _percentage;
        public int Percentage { get => _percentage; private set { _percentage = value; } }
        private DownloadStatus _downloadStatus;
        public DownloadStatus DownloadStatus { get => _downloadStatus; private set { _downloadStatus = value; } }

        public VVVID()
        { _connectionId = GetConnectionId(); }
        private static string WebRequest(string url)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            HttpWebRequest webRequest = (HttpWebRequest)System.Net.WebRequest.Create(url);
            webRequest.Method = "GET";
            webRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:67.0) Gecko/20100101 Firefox/67.0";
            webRequest.CookieContainer = _cookieContainer;
            using (Stream s = webRequest.GetResponse().GetResponseStream())
            {
                using (StreamReader sr = new StreamReader(s))
                    return sr.ReadToEnd();
            }
        }
        private static string GetConnectionId()
        {
            string response = WebRequest("https://www.vvvvid.it/user/login");
            return response.Split(new[] { "\"conn_id\":\"" }, StringSplitOptions.None)[1].Split('\"')[0];
        }
        public List<Anime> AnimeFilter(char c)
        {
            String urlFirst15 = "https://www.vvvvid.it/vvvvid/ondemand/anime/channel/10003/last?filter=" + c + "&conn_id=" + _connectionId; //Questo link mostra solo i primi 15 anime
            String urlLast = "https://www.vvvvid.it/vvvvid/ondemand/anime/channel/10003?filter=" + c + "&conn_id=" + _connectionId; //Questo link mostra i restanti anime
            string response = WebRequest(urlFirst15);
            RootObject animeFirst15 = JsonConvert.DeserializeObject<RootObject>(response);
            response = WebRequest(urlLast);
            RootObject animeLast = JsonConvert.DeserializeObject<RootObject>(response);
            if (animeLast.data != null)
                return animeFirst15.data.ToArray().Union(animeLast.data.ToArray()).ToList();
            return animeFirst15.data.ToArray().ToList();
        }
        public RootObject GetAnimeData(int idAnime)
        {
            string url = $"https://www.vvvvid.it/vvvvid/ondemand/{idAnime}/seasons/?conn_id={_connectionId}";
            string response = WebRequest(url);
            RootObject animeData = JsonConvert.DeserializeObject<RootObject>(response);

            // questa chiamata è per avere più dettagli negli episodi per DownloadWithHDS(). controlla commit
            url = $"http://www.vvvvid.it/vvvvid/ondemand/{idAnime}/season/{animeData.data[0].season_id}?conn_id={_connectionId}";   //QUesta chiamata mi da più dettagli per episodio
            string response2 = WebRequest(url);
            RootEpisodeObject rootEpisodeObject = JsonConvert.DeserializeObject<RootEpisodeObject>(response2);
            animeData.data[0].episodes = rootEpisodeObject.data;

            return animeData;
        }
        public void Start(int episodioNumero)
        {
            GetLinks(episodioNumero);
            foreach (Episode ep in Anime.episodes)
                if (!String.IsNullOrEmpty(ep.videoLink) && ep.playable)
                {
                    DownloadWithYoutubeDL(ep);
                    DownloadWithHDS(ep);
                }
        }
        private void GetLinks(int episodioNumero)
        {
            if (episodioNumero < 0)
                foreach (Episode ep in Anime.episodes)
                    ep.videoLink = GetEpisodeVideoLink(ep);
            else Anime.episodes[episodioNumero].videoLink = GetEpisodeVideoLink(Anime.episodes[episodioNumero]);
        }
        private string GetEpisodeVideoLink(Episode ep)
        {
            if (ep.vod_mode == 2) throw new Exception("solo per utenti premium"); //vod_mode == 2 solo per utenti premium
            return $"https://www.vvvvid.it/#!show/{Anime.show_id}/text/{ep.season_id}/{ep.video_id}/text";
        }
        private void DownloadWithYoutubeDL(Episode ep)
        {
            string rootPath = AppDomain.CurrentDomain.BaseDirectory;
            string fileName = $"{Anime.title} S{Anime.number}E{ep.number} - {ep.title}.mp4";

            Console.WriteLine($"Downloading {fileName}");
            YoutubeDL youtubeDL = new YoutubeDL();
            youtubeDL.Options.FilesystemOptions.Output = Path.Combine(rootPath, fileName);
            youtubeDL.VideoUrl = ep.videoLink;
            youtubeDL.Options.PostProcessingOptions.KeepVideo = true;
            //youtubeDL.Options.PostProcessingOptions.ExtractAudio = true;
            youtubeDL.Options.PostProcessingOptions.FfmpegLocation = Path.Combine(rootPath, "ffmpeg-20200306-cfd9a65-win64-static", "bin"); //https://github.com/ytdl-org/youtube-dl/issues/2815

            // Subscribe to the console output (optional, but recommended)
            youtubeDL.StandardOutputEvent += (sender, output) => Console.WriteLine(output);
            youtubeDL.StandardErrorEvent += (sender, errorOutput) => Console.WriteLine(errorOutput);

            // Optional, required if binary is not in $PATH
            youtubeDL.YoutubeDlPath = Path.Combine(rootPath, "youtube-dl.exe");

            // Options can also be saved and loaded.Only changed options will be saved.
            //File.WriteAllText("options.config", youtubeDl.Options.Serialize());
            //youtubeDl.Options = Options.Deserialize(File.ReadAllText("options.config"));

            // Prepare the download (in case you need to validate the command before starting the download)
            //string commandToRun = await youtubeDL.PrepareDownloadAsync();
            // Alternatively
            string commandToRun = youtubeDL.PrepareDownload();

            // Just let it run
            //youtubeDL.DownloadAsync();

            // Wait for it
            youtubeDL.Download();

            // Or provide video url
            //youtubeDL.Download("http://videosite.com/videoUrl");

            Console.WriteLine("Download completed");

            //Console.WriteLine("Open downloaded video");
            //Process process = new Process();
            //process.StartInfo = new ProcessStartInfo(Path.Combine(rootPath, fileName));
            //process.Start();
        }
        private void DownloadWithHDS(Episode ep)
        {
            string url = $"http://www.vvvvid.it/vvvvid/ondemand/{Anime.show_id}/season/{Anime.season_id}?conn_id={_connectionId}";
            string response = WebRequest(url);
            var dettaglioEpisodio = JsonConvert.DeserializeObject<Episode>(response);


            string manifestLink = HdsTool.DecodeManifestLink(ep.embed_info);
            string fileName = string.Format("{0}\\{0} {1} - {2}.flv", HdsTool.SanitizeFileName(Anime.title), ep.number, HdsTool.SanitizeFileName(ep.title));
            _hds = new HdsDump(manifestLink, fileName);
            _hds.DownloadedFragment += Hds_DownloadedFragment;
            _hds.DownloadStatusChanged += Hds_DownloadStatusChanged;
            DownloadStatus = _hds.Status;
            if (_hds.Status == DownloadStatus.Paused)
            {
                var progress = _hds.GetProgressFromFile();
                DownloadedTS = TimeSpan.FromMilliseconds(progress.Item1);
                Percentage = progress.Item1 * 100 / progress.Item2;
            }

            System.Threading.Tasks.Task task = _hds.Start();
            string toWrite = "";
            task.ContinueWith(t =>
            {
                toWrite = $"finitoooo!! {DownloadedTS}";
            });
            task.Wait();
            Console.WriteLine($"stampa questo: ");
        }

        private void Hds_DownloadedFragment(object sender, EventArgs e)
        {
            DownloadedTS = _hds.LastDownloadedFragment.TimestampEnd;
            Percentage = (int)_hds.LastDownloadedFragment.Id * 100 / _hds.FragmentsCount;
            Console.WriteLine($"DownloadedTS: { DownloadedTS }; Percentage: { Percentage }");
        }

        private void Hds_DownloadStatusChanged(object sender, EventArgs e)
        {
            DownloadStatus = _hds.Status;
            Console.WriteLine($"STATO CAMBIATO: {_hds.Status}");
        }
    }
}
