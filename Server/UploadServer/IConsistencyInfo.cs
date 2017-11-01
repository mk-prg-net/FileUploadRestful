//<unit_header>
//----------------------------------------------------------------
//
// Martin Korneffel: IT Beratung/Softwareentwicklung
// Stuttgart, den 01.11.2017
//
//  Projekt.......: Server
//  Name..........: IConsistencyInfo.cs
//  Aufgabe/Fkt...: Describes consistency of set of previously uploaded chunks.
//                  If chunks are lost, id's of these chunks are listed in 
//                  "LostChunks" list. 
//                  If set of chunks complete, property "IsConsistent" returns true
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
    public interface IConsistencyInfo
    {
        /// <summary>
        /// True, if set of previously uploaded chunks is consitent.
        /// </summary>
        bool IsConsistent { get; }

        /// <summary>
        /// List id's of chunks are lost in previously upload process.
        /// </summary>
        long[] LostChunks { get; }

    }


    /// <summary>
    /// Implements IConsistencyLog
    /// </summary>
    public class ConsistencyInfo : IConsistencyInfo
    {
        public ConsistencyInfo(IEnumerable<long> LostChunks)
        {
            if (LostChunks != null && LostChunks.Any())
            {
                this.LostChunks = LostChunks.ToArray();
            }
            else
            {
                LostChunks = new long[] { };
            }
        }

        public bool IsConsistent
        {
            get
            {
                return !LostChunks.Any();
            }
        }

        public long[] LostChunks
        {
            get;
        }
    }

}
