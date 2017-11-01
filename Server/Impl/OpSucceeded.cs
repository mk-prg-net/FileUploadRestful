using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploadRestful.Impl
{
    public class OpSucceeded : ISucceeded
    {
        public static ISucceeded Yes
        {
            get
            {
                return new OpSucceeded(true);
            }            
        }

        public static ISucceeded No
        {
            get
            {
                return new OpSucceeded(false);
            }
        }

        private OpSucceeded(bool v)
        {
            this.Succeeded = v;
        }

        public bool Succeeded
        {
            get;
        }
    }
}
