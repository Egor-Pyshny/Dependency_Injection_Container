// See https://aka.ms/new-console-template for more information
using MPP_5.DIConfig;
using MPP_5.DIContainer;
using MPP_5.DIUtils;
using System.Reflection;

Console.WriteLine("Hello, World!");
var constrs = typeof(A).GetConstructors();
ConstructorInfo constructor = constrs[0];

DependenciesConfiguration dependenciesConfiguration = new DependenciesConfiguration();
dependenciesConfiguration.Register<B,A>(LifeCycle.Singleton);
dependenciesConfiguration.Register<IService, ServiceImpl1>(name: "f1");
dependenciesConfiguration.Register<IService, ServiceImpl>(name: "f2");
dependenciesConfiguration.Register<IRepository, RepositoryImpl>();
dependenciesConfiguration.Register<IRepository, RepositoryImpl1>(name: "d1");
DependencyProvider dependencyProvider = new DependencyProvider(dependenciesConfiguration);
var s = dependencyProvider.Resolve<B>();
var service1 = dependencyProvider.Resolve<IService>(name: "f1");

_ = 5;
public class A : B {
    [DependencyConstructor]
    public A() { }
    public A(int x) { }
}

public abstract class B { }


interface IService { }
class ServiceImpl : IService
{
    IRepository r;
    [DependencyConstructor]
    public ServiceImpl(IRepository repository) { r = repository; }
}
class ServiceImpl1 : IService
{
    IRepository r;
    [DependencyConstructor]
    public ServiceImpl1([DependencyKey("d1")]IRepository repository) { r = repository; }
}
interface IRepository { }
class RepositoryImpl1 : IRepository
{
    [DependencyConstructor]
    public RepositoryImpl1() { }
}

class RepositoryImpl : IRepository
{
    [DependencyConstructor]
    public RepositoryImpl() { }
}