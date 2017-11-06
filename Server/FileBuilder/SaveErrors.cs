using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploadRestful.FileBuilder
{
    public enum SaveErrorTypes
    {
        none,
        generalError

    }

    public class SaveError : IErrorDescription<SaveErrorTypes>
    {
        public static IErrorDescription<SaveErrorTypes> CreateNullobject()
        {
            return new SaveError(SaveErrorTypes.none, "");
        }

        public static IErrorDescription<SaveErrorTypes> Create(SaveErrorTypes type, string description)
        {
            return new SaveError(type, description);
        }
        private SaveError(SaveErrorTypes type, string description)
        {
            this.Description = description;
            this.ErrorType = type;
        }
        
        public string Description
        {
            get;
        }

        public SaveErrorTypes ErrorType
        {
            get;
        }
    }
}
