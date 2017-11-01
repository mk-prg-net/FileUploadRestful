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

using FileUploadRestful.FileBuilder;

namespace FileUploadRestful.Impl
{
    public class FilesystemFileBuilder : IFileBuilder
    {
        Stream f;

        public void Close()
        {
            f.Flush();
            f.Close();
        }

        public Tuple<ISucceeded, IErrorDescription<OpenErrorTypes>> Open(string fileName)
        {
            var res = Tuple.Create(OpSucceeded.Yes, OpenError.Create(OpenErrorTypes.none, ""));
            try
            {
                f = File.Open(fileName, FileMode.Append, FileAccess.Write, FileShare.None);                
            }
            catch (FileNotFoundException ex)
            {
                res = Tuple.Create(OpSucceeded.No, OpenError.Create(OpenErrorTypes.fileNotFound, mko.ExceptionHelper.FlattenExceptionMessages(ex)));
            }
            catch (IOException ex)
            {                
                res =  Tuple.Create(OpSucceeded.No, OpenError.Create(OpenErrorTypes.fileAlreadyOpened, mko.ExceptionHelper.FlattenExceptionMessages(ex)));
            }
            catch(Exception ex)
            {
                return Tuple.Create(OpSucceeded.No, OpenError.Create(OpenErrorTypes.generalError, mko.ExceptionHelper.FlattenExceptionMessages(ex)));
            }
            return res;
        }

        public Tuple<ISucceeded, IErrorDescription<SaveErrorTypes>> SaveChunk(byte[] chunk, int count)
        {
            var res = Tuple.Create(OpSucceeded.Yes, SaveError.Create(SaveErrorTypes.none, "")); 
            try
            {
                f.Write(chunk, 0, count);                
            }catch(Exception ex)
            {
                res = Tuple.Create(OpSucceeded.No, SaveError.Create(SaveErrorTypes.generalError, mko.ExceptionHelper.FlattenExceptionMessages(ex)));
            }
            return res;
        }
    }
}
