//<unit_header>
//----------------------------------------------------------------
//
// Martin Korneffel: IT Beratung/Softwareentwicklung
// Stuttgart, den 13.11.2017
//
//  Projekt.......: Server
//  Name..........: AppendingToFileLogBuilder.cs
//  Aufgabe/Fkt...: Implements a Builder od IAppendingToFileLog for deserialization purpose.
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
    public class AppendingToFileLogBuilder : IAppendingToFileLog
    {
        public string Description
        {
            get;
            set;
        }

        public AppendingToFileErrorTypes ErrorType
        {
            get;
            set;
        }

        public bool IsComplete
        {
            get;
            set;
        }

        public long NoOfRecentlyAppendedChunk
        {
            get;
            set;
        }


        public IAppendingToFileLog Create()
        {
            return new AppendingToFileLog(ErrorType, IsComplete, NoOfRecentlyAppendedChunk, Description);
        }
    }
}
