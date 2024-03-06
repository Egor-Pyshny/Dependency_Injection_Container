using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MPP_5.DIConfig
{
    public class MyDependency
    {
        public Type TDependency;
        public Type TImplementation;
        public LifeCycle lifeCycle = LifeCycle.Singleton;
        public string? name = null;
        public List<DependencyParameter> parameters;

        public MyDependency(Type tDependency, Type tImplementation, LifeCycle lifeCycle, string? name, List<DependencyParameter> parameters)
        {
            TDependency = tDependency;
            TImplementation = tImplementation;
            this.lifeCycle = lifeCycle;
            this.name = name;
            this.parameters = parameters;
        }
    }
}
