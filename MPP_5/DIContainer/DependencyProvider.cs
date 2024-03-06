using MPP_2.Exceptions;
using MPP_2.MyFaker;
using MPP_5.DIConfig;
using MPP_5.DIUtils;
using System.Reflection;

namespace MPP_5.DIContainer
{
    public class DependencyProvider
    {
        private readonly Dictionary<MyDependency, object> dependencies;
        private Faker faker = new Faker();

        public DependencyProvider(DependenciesConfiguration dependencies)
        {
            List<MyDependency> deps = dependencies.GetDependencies();
            this.dependencies = new Dictionary<MyDependency, object>();
            foreach (MyDependency dep in deps)
            {
                if (!dep.TImplementation.IsAssignableTo(dep.TDependency) || dep.TImplementation.IsInterface || dep.TImplementation.IsAbstract)
                    throw new Exception("DependencyProvider exception");
                this.dependencies.Add(dep, null);
            }
        }

        public object Resolve<TDependency>(string? name = null)
        { 
            object res = null;
            MyDependency d;
            if (name != null)
                d = FindByName(name);
            else
                d = FindByType(typeof(TDependency));
            if (d == null) throw new NullReferenceException("type is null");
            if (d.lifeCycle == LifeCycle.Singleton)
            {
                if (dependencies[d] == null) 
                { 
                    res = CreateDependency(d);
                    dependencies[d] = res;
                }
                else
                    res = dependencies[d];
            }
            else 
            {
                res = CreateDependency(d);
                dependencies[d] = res;
            }
            return (TDependency)res;
        }

        private MyDependency? FindByName(string name) { 
            foreach (MyDependency dep in dependencies.Keys)
            {
                if (name != null && dep.name == name)
                {
                    return dep;
                }
            }
            return null;
        }

        private MyDependency? FindByType(Type t) {
            foreach (MyDependency dep in dependencies.Keys)
            {
                if (dep.TDependency == t)
                {
                    return dep;
                }
            }
            return null;
        }

        private object CreateDependency(MyDependency dependency)
        {
            HashSet<Type> usedtypes = new HashSet<Type>();
            object Create(MyDependency dependency)
            {
                Type type = dependency.TImplementation;
                if (!usedtypes.Add(type)) throw new CyclicDependenceException(usedtypes, type);
                object res = null;
                var constrs = type.GetConstructors().Where(c => c.GetCustomAttributes<DependencyConstructorAttribute>().Count() > 0).ToArray();
                if (constrs.Length == 0)
                    throw new Exception("No constructor with attribute DependencyConstructor");
                ConstructorInfo constructor = constrs[0];
                var parms = constructor.GetParameters();
                List<object> args = new List<object>();
                foreach (var param in parms)
                {
                    Type paramType = param.ParameterType;
                    if (paramType.IsInterface || paramType.IsAbstract)
                    {
                        var temp = param.GetCustomAttribute<DependencyKeyAttribute>();
                        MyDependency? dep;
                        if (temp != null)
                        {
                            string name = temp.Key;
                            dep = FindByName(name);
                        }
                        else
                        {
                            dep = FindByType(paramType);
                        }
                        if (dep == null) throw new Exception("Dep not found");
                        args.Add(Create(dep));
                    }
                    else
                    {
                        var temp = dependency.parameters.Where(p => p.parameterName == param.Name).ToList();
                        if (temp.Count > 0)
                            args.Add(temp[0].parameterValue);
                        else
                            args.Add(faker.Create(paramType));
                    }
                }
                return constructor.Invoke(args.ToArray());
            }
            return Create(dependency);
        }
    }
}
