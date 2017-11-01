//<unit_header>
//----------------------------------------------------------------
//
// Martin Korneffel: IT Beratung/Softwareentwicklung
// Stuttgart, den 30.10.2017
//
//  Projekt.......: Server
//  Name..........: UploadServer.cs
//  Aufgabe/Fkt...: Implementiert einen Restful Upload- Server
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
using System.Text;
using System.Threading.Tasks;

using FileUploadRestful.FileBuilder;

namespace FileUploadRestful.UploadServer
{
    /// <summary>
    /// mko, 30.10.2017
    /// Restful Upload Server
    /// </summary>
    public interface IUploadServer
    {
        /// <summary>
        /// Implements strategy pattern for storing files.
        /// </summary>
        /// <param name="strategy"></param>
        void DefineFileBuilder(IFileBuilder strategy);        
                
        /// <summary>
        /// Starts Fileupload. Creates a server side queue.
        /// In the following clients will enqueue chuncks of file here.
        /// </summary>
        /// <returns>Id of created serverside Queue</returns>
        string NewUploadQueue();

        /// <summary>
        /// Uploads a chunk of file an enqueues it in the queue.
        /// </summary>
        /// <param name="QueueId"></param>
        /// <param name="ChunkNo"></param>
        /// <param name="FileChunk"></param>
        /// <param name="CountByte">Count of valid bytes in FileChunk</param>
        void UploadChunk(string QueueId, long ChunkNo, byte[] FileChunk, int CountBytes);

        ///// <summary>
        ///// Terminates the upload process. Assembles uploaded chunks to a file.
        ///// Saves the file under specified name.
        ///// </summary>
        ///// <param name="QueueId"></param>        
        ///// <param name="maxChunkNo"></param>
        ///// <param name="timeoutInMs"></param>
        ///// <param name="Filename"></param>
        void CloseQueue(string QueueId, long maxChunkNo, int timeoutInMs, string Filename);
        
        /// <summary>
        /// Checks consistency of previously uploaded chunk- set.
        /// </summary>
        /// <param name="QueueId"></param>
        /// <param name="maxChunkNo"></param>
        /// <returns></returns>
        Tuple<ISucceeded, long[]> ConsistencyCheck(string QueueId, long maxChunkNo);

        /// <summary>
        /// Appending a subset of chunks to a file. The file becomes the result of the upload.
        /// </summary>
        /// <param name="QueueId"></param>
        /// <param name="maxChunkNo"></param>
        /// <param name="FileName"></param>
        /// <returns></returns>
        Tuple<ISucceeded, IAppendingToFileLog> AppendingChunksToFile(string QueueId, long maxChunkNo, string FileName);


        /// <summary>
        /// Interrupts the upload process. Reject all uploaded chunks and 
        /// deletes the queue
        /// </summary>
        /// <param name="QueueId"></param>
        void DeleteQueue(string QueueId);

    }
}
