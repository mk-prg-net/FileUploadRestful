﻿//<unit_header>
//----------------------------------------------------------------
//
// Martin Korneffel: IT Beratung/Softwareentwicklung
// Stuttgart, den 30.10.2017
//
//  Projekt.......: Server
//  Name..........: IFileStoreStrategy.cs
//  Aufgabe/Fkt...: Abstraction of file bulding process
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
//  Autor.........: Martin Korneffel (mko)
//  Datum.........: 5.11.2017
//  Änderungen....: Return values of kind Tuple<ISucceeded...> replaced by RC<...>
//
//</unit_history>
//</unit_header>        

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RTopen = mko.Logging.RC<FileUploadRestful.IErrorDescription<FileUploadRestful.FileBuilder.OpenErrorTypes>>;
using RTsave = mko.Logging.RC<FileUploadRestful.IErrorDescription<FileUploadRestful.FileBuilder.SaveErrorTypes>>;


namespace FileUploadRestful.FileBuilder
{   
    /// <summary>
    /// mko, 30.10.2017
    /// Strategy pattern for storing file.
    /// </summary>
    public interface IFileBuilder
    {
        RTopen Open(string fileName);

        /// <summary>
        /// Save the specified stream under the filename
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="File"></param>
        RTsave  SaveChunk(byte[] chunk, int count);

        void Close();
    }    
}
