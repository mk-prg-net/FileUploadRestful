//<unit_header>
//----------------------------------------------------------------
//
// Martin Korneffel: IT Beratung/Softwareentwicklung
// Stuttgart, den 31.10.2017
//
//  Projekt.......: Projektkontext
//  Name..........: Dateiname
//  Aufgabe/Fkt...: Kurzbeschreibung
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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using mko.Logging;
using FileUploadRestful.FileBuilder;
using RTopen = mko.Logging.RC<FileUploadRestful.IErrorDescription<FileUploadRestful.FileBuilder.OpenErrorTypes>>;
using RTsave = mko.Logging.RC<FileUploadRestful.IErrorDescription<FileUploadRestful.FileBuilder.SaveErrorTypes>>;

namespace FileUploadRestful.Impl
{
    public class FilesystemFileBuilder : IFileBuilder, IDisposable
    {
        Stream f;

        public RTopen Open(string fileName)
        {
            var res = RTopen.Ok(OpenError.CreateNullobject());
            try
            {
                f = File.Open(fileName, FileMode.Append, FileAccess.Write, FileShare.None);                             
            }
            catch (FileNotFoundException ex)
            {
                res = RTopen.Failed(OpenError.Create(OpenErrorTypes.fileNotFound, mko.ExceptionHelper.FlattenExceptionMessages(ex)));
            }
            catch (IOException ex)
            {                
                res =  RTopen.Failed(OpenError.Create(OpenErrorTypes.fileAlreadyOpened, mko.ExceptionHelper.FlattenExceptionMessages(ex)));
            }
            catch(Exception ex)
            {
                res = RTopen.Failed(OpenError.Create(OpenErrorTypes.generalError, mko.ExceptionHelper.FlattenExceptionMessages(ex)));
            }
            return res;
        }

        public RTsave SaveChunk(byte[] chunk, int count)
        {
            var res = RTsave.Ok(SaveError.CreateNullobject()); 
            try
            {
                f.Write(chunk, 0, count);                
            }catch(Exception ex)
            {
                res = RTsave.Failed(SaveError.Create(SaveErrorTypes.generalError, mko.ExceptionHelper.FlattenExceptionMessages(ex)));
            }
            return res;
        }

        public void Close()
        {
            f.Flush();
            f.Close();
        }

        public void Dispose()
        {            
            f?.Dispose();
        }
    }
}
