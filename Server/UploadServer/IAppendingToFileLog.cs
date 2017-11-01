//<unit_header>
//----------------------------------------------------------------
//
// Martin Korneffel: IT Beratung/Softwareentwicklung
// Stuttgart, den 1.11.2017
//
//  Projekt.......: Server
//  Name..........: IAssemblyLog.cs
//  Aufgabe/Fkt...: Represents result of an assembly step.
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

namespace FileUploadRestful.UploadServer
{

    public enum AppendingToFileErrorTypes
    {
        none,
        ChunkMissing,
        MaxChunkMissing,
        fileCurrentlyBlockedForAppending,
        openFileBuilderFails
    }

    public interface IAppendingToFileLog : IErrorDescription<AppendingToFileErrorTypes>
    {
        /// <summary>
        /// Returns true, if file assembly process finished successful.
        /// </summary>
        bool IsComplete { get; }

        /// <summary>
        /// Number of recently appended chunk to file.
        /// </summary>
        long NoOfRecentlyAppendedChunk { get; }
    }



    public class AppendingToFileLog : IAppendingToFileLog
    {

        public static IAppendingToFileLog Create(AppendingToFileErrorTypes type, string description)
        {
            return new AppendingToFileLog(type, description);
        }


        /// <summary>
        /// Use this constuctor to document a failed append
        /// </summary>
        /// <param name="type"></param>
        /// <param name="description"></param>
        public AppendingToFileLog(AppendingToFileErrorTypes type, string description)
        {
            Description = description;
            ErrorType = type;
            IsComplete = false;
            NoOfRecentlyAppendedChunk = -1;

        }

        public static IAppendingToFileLog Create(bool isComplete, long noOfRecentlyAppendedChunk)
        {
            return new AppendingToFileLog(isComplete, noOfRecentlyAppendedChunk);
        }

        public AppendingToFileLog(bool isComplete, long noOfRecentlyAppendedChunk)
        {
            Description = "";
            ErrorType = AppendingToFileErrorTypes.none;
            IsComplete = isComplete;
            NoOfRecentlyAppendedChunk = noOfRecentlyAppendedChunk;

        }


        public string Description
        {
            get;
        }

        public AppendingToFileErrorTypes ErrorType
        {
            get;
        }

        public bool IsComplete
        {
            get;
        }

        public long NoOfRecentlyAppendedChunk
        {
            get;
        }
    }
}
