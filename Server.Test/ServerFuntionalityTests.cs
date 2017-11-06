using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading.Tasks;
using System.IO;
using FileUploadRestful;
using FileUploadRestful.UploadServer;
using FileUploadRestful.Impl;

namespace Server.Test
{
    [TestClass]
    public class ServerFuntionalityTests
    {

        IUploadServer Create()
        {
            var tempDir = Environment.ExpandEnvironmentVariables("%TEMP%");
            var queuesDir = Path.Combine(tempDir, Guid.NewGuid().ToString("D"));
            Directory.CreateDirectory(queuesDir);
            var svr = new UploadServerV1(queuesDir);
            var fbld = new FilesystemFileBuilder();
            svr.DefineFileBuilder(fbld);
            return svr;
        }


        [TestMethod]
        public async Task ServerFunctionality_CloseQueue()
        {
            var srv = Create();

            var fl = File.OpenRead("..\\..\\Testdata\\CenterMilkyWay.jpg");

            var queueId = srv.NewUploadQueue();

            var buffer = new byte[0x1000];
            var chunkNo = 0L;

            while (fl.Position < fl.Length)
            {
                var count = await fl.ReadAsync(buffer, 0, 0x1000);
                if (count > 0)
                {
                    chunkNo++;
                    srv.UploadChunk(queueId, chunkNo, buffer, count);
                }
            }

            if (chunkNo > 0)
            {
                var filename = "..\\..\\Testdata\\CopyOfMilkiWayCenter.jpg";
                if (File.Exists(filename))
                {
                    // Delete results from previously tests.
                    File.Delete(filename);
                }

                srv.CloseQueue(queueId, chunkNo, 1000, filename);
            }
            else
            {
                srv.DeleteQueue(queueId);
            }
        }

        /// <summary>
        /// Checks reconstruction of file from uploaded chunks by step-by-step appending chunks.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task ServerFunctionality_AppendChunks()
        {
            var srv = Create();            

            var fl = File.OpenRead("..\\..\\Testdata\\CenterMilkyWay.jpg");

            var queueId = srv.NewUploadQueue();

            var buffer = new byte[UploadServerV1.ChunkSize];
            var chunkNo = 0L;

            while (fl.Position < fl.Length)
            {
                var count = await fl.ReadAsync(buffer, 0, UploadServerV1.ChunkSize);
                if (count > 0)
                {
                    chunkNo++;
                    srv.UploadChunk(queueId, chunkNo, buffer, count);
                }
            }

            var stat = srv.GetStatistics();

            Assert.IsTrue(stat.Succeeded);
            Assert.AreEqual(1, stat.Value.NumberOfCurrentlyActiveQueues);
            Assert.AreEqual(stat.Value.MinLengthsOfActiveQueues, stat.Value.MaxLengthsOfActiveQueues);
            
            var filename = "..\\..\\Testdata\\CopyOfMilkiWayCenter.jpg";
            if (File.Exists(filename))
            {
                // Delete results from previously tests.
                File.Delete(filename);
            }

            if (chunkNo > 0)
            {
                //srv.CloseQueue(queueId, chunkNo, 1000, filename);
                var rc = srv.AppendingChunksToFile(queueId, chunkNo, filename);
                while (rc.Succeeded)
                {
                    rc = srv.AppendingChunksToFile(queueId, chunkNo, filename);                    
                } 

                Assert.IsTrue(rc.Value.ErrorType == AppendingToFileErrorTypes.none,  $"ErrorType{rc.Value.ErrorType}: {rc.Value.Description}");
                Assert.AreEqual(chunkNo, rc.Value.NoOfRecentlyAppendedChunk);

                srv.DeleteQueue(queueId);
            }        
            else
            {
                srv.DeleteQueue(queueId);
            }
        }
    }
}
