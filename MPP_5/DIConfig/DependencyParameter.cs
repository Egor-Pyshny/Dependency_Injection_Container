using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPP_5.DIConfig
{
    public class DependencyParameter
    {
        public string parameterName;
        public object parameterValue;

        public DependencyParameter(string parameterName, object parameterValue)
        {
            this.parameterName = parameterName;
            this.parameterValue = parameterValue;
        }
    }
}
