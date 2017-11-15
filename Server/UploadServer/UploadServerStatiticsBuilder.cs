//<unit_header>
//----------------------------------------------------------------
//
// Martin Korneffel: IT Beratung/Softwareentwicklung
// Stuttgart, den 13.11.2017
//
//  Projekt.......: FileUploadRestful
//  Name..........: UploadServerStatisticsBuilder.cs
//  Aufgabe/Fkt...: Implements a Builder of IUploadStatistics for Derserialization 
//                  purpose.
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
    /// <summary>
    /// mko, 13.11.2017
    /// Implements a builder of IUploadStatistics- Objects for deserialization purpose.
    /// </summary>
    public class UploadServerStatiticsBuilder : IUploadServerStatistics
    {
        public int MaxLengthsOfActiveQueues
        {
            get;
            set;
        }

        public int MinLengthsOfActiveQueues
        {
            get;
            set;
        }

        public int NumberOfCurrentlyActiveQueues
        {
            get;
            set;
        }


        public IUploadServerStatistics Create()
        {
            return new UploadServerStatistics(NumberOfCurrentlyActiveQueues, MinLengthsOfActiveQueues, MaxLengthsOfActiveQueues);
        }
    }
}
