//<unit_header>
//----------------------------------------------------------------
//
// Martin Korneffel: IT Beratung/Softwareentwicklung
// Stuttgart, den 1.11.2017
//
//  Projekt.......: Sever
//  Name..........: IErrorDescription.cs
//  Aufgabe/Fkt...: Flat presentation of error, occurred in a API- call
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

namespace FileUploadRestful
{
    public interface IErrorDescription<T> where T: struct
    {
        /// <summary>
        /// T is a enumeration of error types. 
        /// </summary>
        T ErrorType { get; }

        string Description { get; }
    }
}
