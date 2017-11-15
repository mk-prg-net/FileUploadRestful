//<unit_header>
//----------------------------------------------------------------
//
// Martin Korneffel: IT Beratung/Softwareentwicklung
// Stuttgart, den 12.11.2017
//
//  Projekt.......: Server.AspNetCore (FileUploadRestful)
//  Name..........: UploadController
//  Aufgabe/Fkt...: REST- API zum hochladen von Dateien auf einen Server
//                  
//
//
//
//
//<unit_environment>
//------------------------------------------------------------------
//  Zielmaschine..: PC 
//  Betriebssystem: Windows 7 mit .NET 4.5
//  Werkzeuge.....: Visual Studio 2013
//  Autor.........: Martin Korneffel (mko)
//  Version 1.0...: 
//
// </unit_environment>
//
//<unit_history>
//------------------------------------------------------------------
//
//  Version.......: 1.1
//  Autor.........: Martin Korneffel (mko)
//  Datum.........: 
//  Änderungen....: 
//
//</unit_history>
//</unit_header>        

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
    /// <summary>
    /// mko, 12.11.2017
    /// REST Api- Controller to upload files.
    /// </summary>
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
        public void UploadChunk(string QueueId, long ChunkNo, int CountBytes)
        {
            var Chunk = new byte[CountBytes];
            //await Request.Body.FlushAsync();
            int count = 0;

            // Reads continously out the stream. Consider, underlying tcp is asynchron.
            while (count < CountBytes)
            {
                count += Request.Body.Read(Chunk, count, CountBytes - count);
            }

            uploadServer.UploadChunk(QueueId, ChunkNo, Chunk, CountBytes);
        }

        [HttpGet]
        public RC<long[]> ConsistencyCheck(string QueueId, long maxChunkNo)
        {
            return uploadServer.ConsistencyCheck(QueueId, maxChunkNo);
        }

        [HttpGet]
        public RC<IUploadServerStatistics> Statistics()
        {
            return uploadServer.GetStatistics();
        }



        [HttpPut]
        public RC<IAppendingToFileLog> AppendingToFile(string QueueId, long maxChunkNo)
        {

            var reader = new System.IO.StreamReader(Request.Body);
            string Filename = reader.ReadToEnd();

            return uploadServer.AppendingChunksToFile(QueueId, maxChunkNo, Filename);
        }

    }
}
