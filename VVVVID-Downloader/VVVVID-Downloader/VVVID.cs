
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
        private static string _connectionId;
        private static CookieContainer _cookieContainer = new CookieContainer();
        private HdsDump _hds;

        public TimeSpan DownloadedTS { get; private set; }
        public int Percentage { get; private set;}
        public DownloadStatus DownloadStatus { get; private set; }

        public VVVID()
        {
            _connectionId = GetConnectionId();
        }

        /// <summary>
        /// Ottiene un codice di autorizzazione per comunicare con vvvvid.
        /// </summary>
        /// <returns></returns>
        private static string GetConnectionId()
        {
            string response = WebRequest("https://www.vvvvid.it/user/login");
            return response.Split(new[] { "\"conn_id\":\"" }, StringSplitOptions.None)[1].Split('\"')[0];
        }

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

        /// <summary>
        /// Ottieni l'elenco degli anime che iniziano con una determinata lettera.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
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
            if (animeFirst15.data != null)
                return animeFirst15.data.ToArray().ToList();
            return new List<Anime>();
        }

        /// <summary>
        /// Ottieni il dettaglio, compreso di stagioni ed episodi, di un preciso anime.
        /// </summary>
        /// <param name="idAnime"></param>
        /// <returns></returns>
        public RootObject GetAnimeData(int idAnime)
        {
            string url = $"https://www.vvvvid.it/vvvvid/ondemand/{idAnime}/seasons/?conn_id={_connectionId}";
            string response = WebRequest(url);
            RootObject animeData = JsonConvert.DeserializeObject<RootObject>(response);

            //get dati per titolo
            Anime anime = null;
            for (char letter = 'a'; letter <= 'z'; letter++)
            {
                List<Anime> list = AnimeFilter(letter);
                anime = list.FirstOrDefault(f => f.show_id == idAnime);
                if (anime != null)
                {
                    break;
                }
            }

            foreach (Anime a in animeData.data)
            {
                // questa chiamata è per avere più dettagli negli episodi per DownloadWithHDS(). controlla commit
                url = $"https://www.vvvvid.it/vvvvid/ondemand/{idAnime}/season/{a.season_id}?conn_id={_connectionId}";   //QUesta chiamata mi da più dettagli per episodio
                string response2 = WebRequest(url);
                a.episodes = JsonConvert.DeserializeObject<RootEpisodeObject>(response2).data;
             
                a.episodes.ForEach(ep => ep.videoLink = GetEpisodeVideoLink(ep, a.show_id));

                a.title = anime.title;
            }

            return animeData;
        }

        /// <summary>
        /// Genera link per il download dell'episodio.
        /// </summary>
        /// <param name="ep"></param>
        /// <param name="animeShowId"></param>
        /// <returns></returns>
        public string GetEpisodeVideoLink(Episode ep, int animeShowId)
        {
            if (ep.vod_mode == 2) throw new Exception("solo per utenti premium"); //vod_mode == 2 solo per utenti premium
            return $"https://www.vvvvid.it/#!show/{animeShowId}/text/{ep.season_id}/{ep.video_id}/text";
        }

        /// <summary>
        /// Scarica l'episodio tramite la libreria (python???) YoutubeDL.
        /// </summary>
        /// <param name="episode"></param>
        /// <param name="anime"></param>
        public void DownloadWithYoutubeDL(Episode episode, Anime anime)
        {
            if (!episode.playable)
            {
                throw new Exception("episode can not be playable");
            }
            if (string.IsNullOrWhiteSpace(episode.videoLink))
            {
                throw new Exception("videoLink can not be empty or null");
            }

            string rootPath = AppDomain.CurrentDomain.BaseDirectory;
            string fileName = $"{anime.title} S{anime.number}E{episode.number} - {episode.title}.mp4";

            Console.WriteLine($"Downloading {fileName}");
            YoutubeDL youtubeDL = new YoutubeDL();
            youtubeDL.Options.FilesystemOptions.Output = Path.Combine(rootPath, fileName);
            youtubeDL.VideoUrl = episode.videoLink;
            youtubeDL.Options.PostProcessingOptions.KeepVideo = true;
            //youtubeDL.Options.PostProcessingOptions.ExtractAudio = true;
            
            string env = string.Empty;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                env = "win64";
            }
            else if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                env = "linux64";
            }
            youtubeDL.Options.PostProcessingOptions.FfmpegLocation = Path.Combine(rootPath, "BtbN_FFmpeg-Builds", env, "bin"); //https://github.com/ytdl-org/youtube-dl/issues/2815

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

        /// <summary>
        /// Scarica l'episodio tramite la libreria HDS.
        /// </summary>
        /// <param name="episode"></param>
        public void DownloadWithHDS(Episode episode, Anime anime)
        {
            if (!episode.playable)
            {
                throw new Exception("episode can not be playable");
            }

            string url = $"http://www.vvvvid.it/vvvvid/ondemand/{anime.show_id}/season/{anime.season_id}?conn_id={_connectionId}";
            string response = WebRequest(url);
            var dettaglioEpisodio = JsonConvert.DeserializeObject<Episode>(response);


            string manifestLink = HdsTool.DecodeManifestLink(episode.embed_info);
            string fileName = string.Format("{0}\\{0} {1} - {2}.flv", HdsTool.SanitizeFileName(anime.title), episode.number, HdsTool.SanitizeFileName(episode.title));
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
