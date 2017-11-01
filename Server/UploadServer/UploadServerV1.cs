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

using FileUploadRestful.FileBuilder;

namespace FileUploadRestful.UploadServer
{
    public class UploadServerV1 : IUploadServer
    {

        /// <summary>
        /// Name of directory, where files temporary stored. 
        /// A queue is implemented as a subfolder in directory.
        /// Chunks are Files in the subfolder with following 
        /// filename- pattern:
        /// #ChunkId#.ck
        /// </summary>
        /// <param name="tempDirName"></param>
        public UploadServerV1(string tempDirName)
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

        /// <summary>
        /// Checks the consistency of previously uploaded set of chunks.
        /// If chunks are lost during upload, this will be detected and
        /// the method signals in ISuccseeded this fact with succeeded == false
        /// </summary>
        /// <param name="QueueId"></param>
        /// <param name="maxChunkNo"></param>
        /// <returns>Item1.Succeede = false if inconsistent. Item2 lists all lost chunks</returns>
        public Tuple<ISucceeded, long[]> ConsistencyCheck(string QueueId, long maxChunkNo)
        {
            mko.TraceHlp.ThrowArgExIf(string.IsNullOrWhiteSpace(QueueId), "QueueId is null or empty");
            mko.TraceHlp.ThrowArgExIf(maxChunkNo <= 0, "MaxChunkNo is invalid");

            var dir = Path.Combine(tempDirName, QueueId);

            mko.TraceHlp.ThrowArgExIfNot(Directory.Exists(dir), "Queue does not exists");

            var LostChunks = new List<long>();

            for (long i = 1; i <= maxChunkNo; i++)
            {
                var ChunkFile = Path.Combine(tempDirName, QueueId, i.ToString());
                if (!File.Exists(ChunkFile))
                {
                    LostChunks.Add(i);
                }
            }

            return Tuple.Create(Impl.OpSucceeded.Yes, LostChunks.ToArray());
        }

        /// <summary>
        /// Append the next n chunks on Server to File, thats representing 
        /// the result of upload process. Deletes appended chunks on server.
        /// </summary>
        /// <returns></returns>
        public Tuple<ISucceeded, IAppendingToFileLog> AppendingChunksToFile(string QueueId, long maxChunkNo, string FileName)
        {
            Tuple<ISucceeded, IAppendingToFileLog> res = null;

            mko.TraceHlp.ThrowArgExIf(string.IsNullOrWhiteSpace(QueueId), "QueueId is null or empty");
            mko.TraceHlp.ThrowArgExIf(maxChunkNo <= 0, "MaxChunkNo is invalid");

            var dir = Path.Combine(tempDirName, QueueId);
            mko.TraceHlp.ThrowArgExIfNot(Directory.Exists(dir), "Queue does not exists");

            var resFb = filebuilder.Open(FileName);

            if (!resFb.Item1.Succeeded)
            {
                res = res = Tuple.Create(
                        Impl.OpSucceeded.No,
                        AppendingToFileLog.Create(
                            AppendingToFileErrorTypes.openFileBuilderFails,
                            $"Error Type Filebuilder: {resFb.Item2.ErrorType}: {resFb.Item2.Description}"));

            }
            else
            {
                var chunkNumbers = Directory.GetFiles(dir).Select(r => long.Parse(r)).ToArray();

                // Find first chunk to append. 
                // The first chunk can be recognized by its name, which represents the smallest numeric value.
                var firstChunkNo = chunkNumbers.Min();
#if DEBUG
                // check consistency of chunk- set
                long oldNo = firstChunkNo;
                for (long i = 1, count = chunkNumbers.Count(); i < count; i++)
                {
                    if (chunkNumbers[i] - oldNo > 1)
                    {
                        return Tuple.Create(Impl.OpSucceeded.No, AppendingToFileLog.Create(AppendingToFileErrorTypes.ChunkMissing, $"Missing chunks between {oldNo} and {i}"));
                    }
                    oldNo = i;
                }
#endif

                long chunkNo = firstChunkNo;
                long lastNo = Math.Min(firstChunkNo + 100, maxChunkNo);
                for (; chunkNo <= lastNo; chunkNo++)
                {
                    string file = chunkNo.ToString();

                    var fstream = File.Open(file, FileMode.Open, FileAccess.Read);
                    var buffer = new byte[fstream.Length];
                    var count = fstream.Read(buffer, 0, buffer.Length);
                    fstream.Close();

                    filebuilder.SaveChunk(buffer, count);

                    File.Delete(file);

                }
                filebuilder.Close();
                res = Tuple.Create(Impl.OpSucceeded.Yes, AppendingToFileLog.Create(chunkNo < maxChunkNo, lastNo));
            }
            return res;
        }
    }
}
