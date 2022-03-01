using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MimeTypes;

namespace VVVVID_Downloader.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VVVVIDController : ControllerBase
    {
        private readonly VVVID _vVVID;
        private IMemoryCache _cache;
        private const string nimeListCacheKey = "AnimeList";
        private const string showIdCacheKey = "showId-{0}";

        public VVVVIDController(IMemoryCache cache, VVVID vVVID)
        {
            _cache = cache;
            _vVVID = vVVID;
        }

        // GET: api/SerieTV
        [HttpGet("GetAllAnime")]
        public IActionResult Get()
        {
            List<Anime> animeList = GetAnimeListFromCache();
            return Ok(animeList);
        }

        private List<Anime> GetAnimeListFromCache()
        {
            List<Anime> animeList;

            // Look for a cache key.
            if (!_cache.TryGetValue(nimeListCacheKey, out animeList))
            {
                animeList = new List<Anime>();
                // Key not in cache, so get data
                for (char letter = 'a'; letter <= 'z'; letter++)
                {
                    animeList.AddRange(_vVVID.AnimeFilter(letter));
                }

                // Set cache options.
                var cacheOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time.
                    .SetAbsoluteExpiration(TimeSpan.FromHours(12));

                // Save data in cache.
                _cache.Set(nimeListCacheKey, animeList, cacheOptions);
            }

            return animeList;
        }

        // GET: api/SerieTV/5
        [HttpGet("Anime/{showId}")]
        public IActionResult Get(int showId)
        {
            List<Anime> animeInfo;
            string cacheKey = string.Format(showIdCacheKey, showId);
            // Look for a cache key.
            if (!_cache.TryGetValue(cacheKey, out animeInfo))
            {
                // Key not in cache, so get data
                animeInfo = _vVVID.GetAnimeData(showId).data;

                //get thumbnail of anime
                var cache = GetAnimeListFromCache()
                    .Where(a => a.show_id == showId)
                    .First();
                animeInfo.ForEach(f =>
                {
                    f.thumbnail = cache.thumbnail;
                    f.date_published = cache.date_published;
                    f.additional_info = cache.additional_info;
                    f.director = cache.director;
                });

                // Set cache options.
                var cacheOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time.
                    .SetAbsoluteExpiration(TimeSpan.FromHours(12));

                // Save data in cache.
                _cache.Set(cacheKey, animeInfo, cacheOptions);
            }

            return Ok(animeInfo);
        }

        [HttpGet("DownloadEpisode/{showId}/{seasonId}/{episodeId}")]
        public async Task<IActionResult> DownloadEpisode(int showId, int seasonId, int episodeId)
        {
            Anime anime = null;
            string cacheKey = string.Format(showIdCacheKey, showId);
            List<Anime> animeInfo;
            // Look for a cache key.
            if (!_cache.TryGetValue(cacheKey, out animeInfo))
            {
                // Key not in cache, so get data
                animeInfo = _vVVID.GetAnimeData(showId).data;

                // Set cache options.
                var cacheOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time.
                    .SetAbsoluteExpiration(TimeSpan.FromHours(12));

                // Save data in cache.
                _cache.Set(cacheKey, animeInfo, cacheOptions);
            }
            var episode = animeInfo
                .First(f => { anime = f; return f.season_id == seasonId; })
                .episodes
                .First(f => f.id == episodeId);

            string pathFilename = "";
            Task task1 = new Task(() => _vVVID.DownloadWithYoutubeDL(episode, anime));
            Task task2 = new Task(() => _vVVID.DownloadWithHDS(episode, anime));

            task1.Start();
            task2.Start();

            Task.WaitAll(new Task[] { task1, task2 });
            if (pathFilename == null || !System.IO.File.Exists(pathFilename))
                return NotFound("filename not present");
            try
            {
                var memory = new MemoryStream();
                using (var stream = new FileStream(pathFilename, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                return File(memory, MimeTypeMap.GetMimeType(Path.GetExtension(pathFilename)), Path.GetFileName(pathFilename));

            }
            catch (Exception e)
            {
                throw new Exception(string.Concat("cant download file. ", e.Message));
            }
        }

        // POST: api/SerieTV
        [HttpPost("AddToPlex/{showId}/{seasonId}/{episodeId}")]
        public void Post(int showId, int seasonId, int episodeId)
        {
        }
    }
}
