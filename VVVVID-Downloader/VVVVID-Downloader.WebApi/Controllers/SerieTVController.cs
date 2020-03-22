using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeTypes;

namespace VVVVID_Downloader.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SerieTVController : ControllerBase
    {
        private readonly VVVID _vVVID;

        public SerieTVController(VVVID vVVID)
        {
            _vVVID = vVVID;
        }

        // GET: api/SerieTV
        [HttpGet("GetAllAnime")]
        public IActionResult Get()
        {
            List<Anime> toRet = new List<Anime>();
            for (char letter = 'a'; letter <= 'z'; letter++)
            {
                toRet.AddRange(_vVVID.AnimeFilter(letter));
            }
            return Ok(toRet);
        }

        // GET: api/SerieTV/5
        [HttpGet("{showId}")]
        public IActionResult Get(int showId)
        {
            return Ok(_vVVID.GetAnimeData(showId).data);
        }

        [HttpGet("DownloadEpisode/{showId}/{seasonId}/{episodeId}")]
        public async Task<IActionResult> DownloadEpisode(int showId, int seasonId, int episodeId)
        {
            Anime anime = null;
            var episode = _vVVID.GetAnimeData(showId)
                .data
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
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/SerieTV/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
