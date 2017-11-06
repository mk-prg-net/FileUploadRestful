//<unit_header>
//----------------------------------------------------------------
//
// Martin Korneffel: IT Beratung/Softwareentwicklung
// Stuttgart, den 6.11.2017
//
//  Projekt.......: Server
//  Name..........: IUploadStatistics.cs
//  Aufgabe/Fkt...: Defines a set of statistical parameters
//                  that reflects current state of upload server.
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
    /// mko, 6.11.2017
    /// Defines set of staatistical parameters wich reflects 
    /// current state of upload server.
    /// </summary>
    public interface IUploadServerStatistics
    {
        int NumberOfCurrentlyActiveQueues { get; }
        int MinLengthsOfActiveQueues { get; }
        int MaxLengthsOfActiveQueues { get; }        
    }


    public interface IUploadServerStatisticsBuilder
    {
        int NumberOfCurrentlyActiveQueues { get;  set; }
        int MinLengthsOfActiveQueues { get;  set; }
        int MaxLengthsOfActiveQueues { get; set; }

        IUploadServerStatistics Build();
    }

    public class UploadServerStatistics : IUploadServerStatistics
    {

        public static IUploadServerStatistics CreateNullObject()
        {
            return new UploadServerStatistics(-1, -1, -1);
        }

        internal UploadServerStatistics(int NumberOfCurrentlyActiveQueues, int MinLengthsOfActiveQueues,  int MaxLengthsOfActiveQueues)
        {
            this.NumberOfCurrentlyActiveQueues = NumberOfCurrentlyActiveQueues;
            this.MinLengthsOfActiveQueues = MinLengthsOfActiveQueues;
            this.MaxLengthsOfActiveQueues = MaxLengthsOfActiveQueues;
        }

        public int MaxLengthsOfActiveQueues
        {
            get;
        }

        public int MinLengthsOfActiveQueues
        {
            get;
        }

        public int NumberOfCurrentlyActiveQueues
        {
            get;
        }
    }




    public class UploadServerStatisticsBuilder : IUploadServerStatisticsBuilder
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

        public IUploadServerStatistics Build()
        {
            return new UploadServerStatistics(
                NumberOfCurrentlyActiveQueues,
                MinLengthsOfActiveQueues,
                MaxLengthsOfActiveQueues);
        }
    }
}
