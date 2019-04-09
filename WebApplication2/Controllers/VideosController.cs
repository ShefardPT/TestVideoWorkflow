using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Data;

namespace WebApplication2.Controllers
{
    [Route("test")]
    public class VideosController : Controller
    {
        private ApplicationDbContext _ctx;

        public VideosController
            (ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        [HttpGet]
        public IActionResult List()
        {
            var result = _ctx.Videos.OrderByDescending(v => v.DateUploaded);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromForm]Video data)
        {
            if (data.Id.Equals(0))
            {
                data.DateUploaded = DateTime.UtcNow;

                await _ctx.Videos.AddAsync(data);
                await _ctx.SaveChangesAsync();

                data.JobId = BackgroundJob.Enqueue(() => DoAction(data.Id, JobCancellationToken.Null));

                _ctx.Update(data);
                await _ctx.SaveChangesAsync();

                return Ok(data);
            }

            var video = _ctx.Videos.FirstOrDefault(v => v.Id.Equals(data.Id));

            if (!video.IsDone)
            {
                BackgroundJob.Delete(video.JobId);

                video.JobId = "";
                _ctx.Update(video);
                await _ctx.SaveChangesAsync();
            }

            return Ok(video);
        }

        [HttpDelete]
        public async Task<IActionResult> ClearDb()
        {
            var videos = _ctx.Videos;

            _ctx.RemoveRange(videos);

            await _ctx.SaveChangesAsync();

            return Ok();
        }

        public async Task DoAction(int videoId, IJobCancellationToken cancellationToken)
        {
            var video = _ctx.Videos.FirstOrDefault(v => v.Id.Equals(videoId));

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
            }
            catch (Exception ex)
            {
                var temp = System.IO.File.CreateText($"C:\\temp\\{videoId}.txt");
                temp.Close();

                throw;
            }

            System.Threading.Thread.Sleep(30_000);

            cancellationToken.ThrowIfCancellationRequested();

            video.JobId = "";
            video.IsDone = true;
            _ctx.Update(video);
            await _ctx.SaveChangesAsync();
        }
    }
}
