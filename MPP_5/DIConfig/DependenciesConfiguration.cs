using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPP_5.DIConfig
{
    public class DependenciesConfiguration
    {
        private List<MyDependency> dependencies = new List<MyDependency>();

        public void Register<TDependency, TImplementation>(LifeCycle lifeCycle = LifeCycle.InstancePerDependency, string? name = null, params DependencyParameter[] parameters)
        {
            Type dep = typeof(TDependency);
            Type impl = typeof(TImplementation);
            if (dependencies.Where(d => d.TDependency == dep && d.name == name).ToList().Count == 1)
                throw new Exception($"{dep.Name} with name {name} already exists");
            dependencies.Add(new MyDependency(dep, impl, lifeCycle, name, parameters.ToList()));
        }

        public void Register(Type TDependency, Type TImplementation, LifeCycle lifeCycle = LifeCycle.InstancePerDependency, string? name = null, params DependencyParameter[] parameters)
        {
            Type dep = TDependency;
            Type impl = TImplementation;
            if (dependencies.Where(d => d.TDependency == dep && d.name == name).ToList().Count == 1)
                throw new Exception($"{dep.Name} with name {name} already exists");
            dependencies.Add(new MyDependency(dep, impl, lifeCycle, name, parameters.ToList()));
        }

        public List<MyDependency> GetDependencies() => new List<MyDependency>(dependencies);
    }
}
