using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using FileUploadRestful.UploadServer;
using mko.Logging;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Server.AspNetCore.Controllers
{
    public class UploadController : Controller
    {
        FileUploadRestful.UploadServer.IUploadServer uploadServer;

        public UploadController(IUploadServer uploadServer)
        {
            this.uploadServer = uploadServer;
        }


        // GET: /<controller>/
        public IActionResult Index()
        {
            var rc = uploadServer.GetStatistics();
            mko.TraceHlp.ThrowArgExIfNot(rc.Succeeded, rc.ToString());

            return View(rc.Value);
        }

        [HttpPost]
        public string NewUploadQueue()
        {

            return uploadServer.NewUploadQueue();
        }


        [HttpDelete]
        public void DeleteUploadQueue(string QueueId)
        {
            uploadServer.DeleteQueue(QueueId);
        }


        [HttpPost]
        public void UploadChunk(string QueueId, long ChunkNo, int CountBytes, byte[] Chunk)
        {
            uploadServer.UploadChunk(QueueId, ChunkNo, Chunk, CountBytes);
        }

        [HttpGet]
        public RC<long[]> ConsistencyCheck(string QueueId, long maxChunkNo)
        {
            return uploadServer.ConsistencyCheck(QueueId, maxChunkNo);
        }


        [HttpPut]
        public RC<IAppendingToFileLog> AppendingToFile(string QueueId, long maxChunkNo, string Filename)
        {
            return uploadServer.AppendingChunksToFile(QueueId, maxChunkNo, Filename);
        }

    }
}
