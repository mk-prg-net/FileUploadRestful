//<unit_header>
//----------------------------------------------------------------
//
// Martin Korneffel: IT Beratung/Softwareentwicklung
// Stuttgart, den 30.10.2017
//
//  Projekt.......: Server
//  Name..........: UploadServer.cs
//  Aufgabe/Fkt...: Implements IUploadServer by caching chunks in 
//                  local file system.
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
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace FileUploadRestful
{
    public class UploadServer : IUploadServer
    {

        /// <summary>
        /// Name of directory, where files temporary stored. 
        /// A queue is implemented as a subfolder in directory.
        /// Chunks are Files in the subfolder with following 
        /// filename- pattern:
        /// #ChunkId#.ck
        /// </summary>
        /// <param name="tempDirName"></param>
        public UploadServer(string tempDirName)
        {
            this.tempDirName = tempDirName;
        }

        string tempDirName { get; }

        /// <summary>
        /// Default waiting time is 100ms.
        /// </summary>
        const int waitingTime = 100;
        /// <summary>
        /// Terminates the file upload. 
        /// Assembles chunks to file. Saves file with presetted strategy.
        /// </summary>
        /// <param name="QueueId"></param>
        /// <param name="maxChunkNo"></param>
        /// <param name="Filename"></param>
        public void CloseQueue(string QueueId, long maxChunkNo, int timeoutInMs, string Filename)
        {
            var dir = Path.Combine(tempDirName, QueueId);
            var maxChunkFile = Path.Combine(tempDirName, QueueId, maxChunkNo.ToString());
            int currentWaitingTime = 0;

            bool maxChunkFileExists = false;
            do
            {
                maxChunkFileExists = File.Exists(maxChunkFile);
                if (!maxChunkFileExists)
                {
                    System.Threading.Thread.Sleep(100);
                    currentWaitingTime += waitingTime;
                }
            } while (!maxChunkFileExists && currentWaitingTime < timeoutInMs);

            if (currentWaitingTime >= timeoutInMs)
            {
                throw new TimeoutException("Chunk No. " + maxChunkFile + " never reached the server");
            }
            else
            {
                filebuilder.Open(Filename);
                foreach (var file in Directory.GetFiles(dir).OrderBy(fn => long.Parse(Path.GetFileName(fn))))
                {
                    var fstream = File.Open(file, FileMode.Open, FileAccess.Read);
                    var buffer = new byte[fstream.Length];
                    var count = fstream.Read(buffer, 0, buffer.Length);
                    fstream.Close();

                    filebuilder.SaveChunk(buffer, count);                    

                    File.Delete(file);
                }
                filebuilder.Close();
                Directory.Delete(dir);
            }
        }

        /// <summary>
        /// Set the file store strategy.
        /// </summary>
        /// <param name="fileBuilder"></param>
        public void DefineFileBuilder(IFileBuilder fileBuilder)
        {
            _filebuilder = fileBuilder;
        }

        IFileBuilder _filebuilder;
        IFileBuilder filebuilder => _filebuilder;


        /// <summary>
        /// Interrupts the file upload, rejects all chunks and deletes the queue.
        /// </summary>
        /// <param name="QueueId"></param>
        public void DeleteQueue(string QueueId)
        {
            var dir = Path.Combine(tempDirName, QueueId);
            foreach (var file in Directory.GetFiles(dir))
            {
                File.Delete(file);
            }

            Directory.Delete(dir);
        }

        /// <summary>
        /// Starts a new restful fileupload.
        /// </summary>
        /// <returns></returns>
        public string NewUploadQueue()
        {
            var id = Guid.NewGuid().ToString("D");
            Directory.CreateDirectory(Path.Combine(tempDirName, id));
            return id;
        }

        /// <summary>
        /// Uploads a chunk aof file.
        /// </summary>
        /// <param name="QueueId"></param>
        /// <param name="ChunkNo"></param>
        /// <param name="FileChunk"></param>
        public void UploadChunk(string QueueId, long ChunkNo, byte[] FileChunk, int CountBytes)
        {
            mko.TraceHlp.ThrowArgExIfNot(CountBytes <= FileChunk.Length, "CountBytes < FileChunk.Length");
            var file = File.Create(Path.Combine(tempDirName, QueueId, ChunkNo.ToString()));
            file.Write(FileChunk, 0, CountBytes);
            file.Flush();
            file.Close();
        }
    }
}
