using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPP_5.DIExceptions
{
    public class ImplementationClassException : Exception
    {
        public ImplementationClassException(Type TDep, Type TImpl) : base($"TImplementation = {TImpl} donot implement TDependency = {TDep}") { }
    }
}
