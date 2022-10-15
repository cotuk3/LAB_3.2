using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityService
{
    public class MyException : Exception
    {
        public MyException()
            :base()
        {

        }

        public MyException(string input)
            :base($"{input}were in wrong format")
        {

        }
    }
}
