using MPP_2.Exceptions;
using MPP_2.MyFaker;
using MPP_5.DIConfig;
using MPP_5.DIUtils;
using MPP_5.DIExceptions;
using System.Reflection;


namespace MPP_5.DIContainer
{
    public class DependencyProvider
    {
        private Dictionary<MyDependency, object> dependencies;
        private Faker faker = new Faker();

        public DependencyProvider(DependenciesConfiguration dependencies)
        {
            List<MyDependency> deps = dependencies.GetDependencies();
            this.dependencies = new Dictionary<MyDependency, object>();
            foreach (MyDependency dep in deps)
            {
                if (!dep.TImplementation.IsAssignableTo(dep.TDependency) || dep.TImplementation.IsInterface || dep.TImplementation.IsAbstract)
                    if(dep.TImplementation.GetInterfaces().Where(i => i.Name == dep.TDependency.Name).Count()==0)
                        throw new ImplementationClassException(dep.TDependency, dep.TImplementation);
                this.dependencies.Add(dep, null);
            }
        }

        public List<TDependency> ResolveAll<TDependency>()
        {
            var temp = new List<TDependency>();
            foreach (var d in FindByTypeAll(typeof(TDependency)))
            {
                object res;
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
                temp.Add((TDependency)res); 
            }
            return temp;
        }

        public IEnumerable<MyDependency> FindByTypeAll(Type t)
        {
            foreach (MyDependency dep in dependencies.Keys)
            {
                if (dep.TDependency == t)
                {
                    yield return dep;
                }
            }
        }

        public TDependency Resolve<TDependency>(string? name = null)
        { 
            object res = null;
            bool replace = false;
            Type TDep = null;
            Type TImpl = null;
            MyDependency d;
            if (name != null)
                d = FindByName(name);
            else
                d = FindByType(typeof(TDependency));
            if (d == null)
            {
                replace = true;
                d = FindByDepName(typeof(TDependency));
                if (d == null)
                    throw new DependencyNotFoundException(typeof(TDependency));
                var tmp = FindByType(typeof(TDependency).GenericTypeArguments[0]);
                if (tmp == null)
                    throw new DependencyNotFoundException(typeof(TDependency));
                var tImpl = d.TImplementation.MakeGenericType(typeof(TDependency).GenericTypeArguments[0]);
                d.TDependency = typeof(TDependency);
                d.TImplementation = tImpl;
            }
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
            if (replace) {
                d.TDependency = TDep;
                d.TImplementation = TImpl;
            }
            var t = res.GetType();
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

        private MyDependency? FindByDepName(Type t)
        {
            foreach (MyDependency dep in dependencies.Keys)
            {
                if (dep.TDependency.Name == t.Name)
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
                    throw new CunstructorNotFoundException(type);
                ConstructorInfo constructor = constrs[0];
                var parms = constructor.GetParameters();
                List<object> args = new List<object>();
                foreach (var param in parms)
                {
                    Type paramType = param.ParameterType;
                    if (paramType.IsInterface || paramType.IsAbstract || paramType.IsGenericType)
                    {
                        if (paramType.IsGenericType)
                        {
                            Type paramGenType = paramType.GenericTypeArguments[0];
                            if (paramType.FullName.Contains("System"))
                            {
                                Type listType = typeof(List<>).MakeGenericType(paramGenType);
                                try
                                {
                                    args.Add(faker.Create(listType));
                                }
                                catch (KeyNotFoundException)
                                {
                                    MethodInfo genericMethod = typeof(DependencyProvider).GetMethod("ResolveAll");
                                    MethodInfo closedMethod = genericMethod.MakeGenericMethod(paramGenType);
                                    var t = closedMethod.Invoke(this, null);
                                    args.Add(t);
                                }
                            }
                            else 
                            {
                                MethodInfo genericMethod = typeof(DependencyProvider).GetMethod("Resolve");
                                MethodInfo closedMethod = genericMethod.MakeGenericMethod(paramType);
                                var t = closedMethod.Invoke(this, new object[] { null });
                                args.Add(t);
                            }
                        }
                        else
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
                            if (dep == null) 
                                throw new DependencyNotFoundException(paramType);
                            args.Add(Create(dep));
                        }
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
