using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploadRestful.FileBuilder
{
    public enum OpenErrorTypes
    {
        none,
        fileNotFound,
        fileAlreadyOpened,
        generalError        
    }

    public class OpenError : IErrorDescription<OpenErrorTypes>
    {

        public static IErrorDescription<OpenErrorTypes> CreateNullobject()
        {
            return new OpenError(OpenErrorTypes.none, "");
        }

        public static IErrorDescription<OpenErrorTypes> Create(OpenErrorTypes type, string description)
        {
            return new OpenError(type, description);
        }

        private OpenError(OpenErrorTypes type, string description)
        {
            this.Description = description;
            this.ErrorType = type;
        }

        public string Description
        {
            get;
        }

        public OpenErrorTypes ErrorType
        {
            get;
        }
    }
}
