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
    public class UnitTest1
    {

        IUploadServer Create()
        {
            var tempDir = Environment.ExpandEnvironmentVariables("%TEMP%");
            var svr = new UploadServerV1(tempDir);
            var fbld = new FilesystemFileBuilder();
            svr.DefineFileBuilder(fbld);
            return svr;
        }

        
        [TestMethod]
        public async Task TestMethod1()
        {
            var srv = Create();

            var fl = File.OpenRead("..\\..\\Testdata\\CenterMilkyWay.jpg");

            var queueId = srv.NewUploadQueue();

            var buffer = new byte[0x1000];
            var chunkNo = 0L;            
            
            while(fl.Position < fl.Length)
            {
                var count = await fl.ReadAsync(buffer, 0, 0x1000);
                if(count > 0)
                {
                    chunkNo++;
                    srv.UploadChunk(queueId, chunkNo, buffer, count);
                }
            }

            if(chunkNo > 0)
            {
                srv.CloseQueue(queueId, chunkNo, 1000, "..\\..\\Testdata\\CopyOfMilkiWayCenter.jpg");
            } else
            {
                srv.DeleteQueue(queueId);
            }            

        }
    }
}
