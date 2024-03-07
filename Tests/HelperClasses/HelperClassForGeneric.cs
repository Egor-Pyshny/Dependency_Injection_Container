using MPP_5.DIUtils;

namespace Tests.HelperClasses
{
    public interface IRepository { }
    public class RepositoryImpl1 : IRepository
    {
        [DependencyConstructor]
        public RepositoryImpl1() { }
    }
    public interface IService<TRepository> where TRepository : IRepository { }

    public class ServiceImpl<TRepository> : IService<TRepository> where TRepository : IRepository
    {
        public TRepository r;
        [DependencyConstructor]
        public ServiceImpl(TRepository repository) { r = repository; }
    }

    public interface IService { }
    public interface ITest { }
    public class ServiceImpl : IService
    {
        [DependencyConstructor]
        public ServiceImpl() { }
    }
    public class ServiceImpl1 : IService
    {
        [DependencyConstructor]
        public ServiceImpl1() { }
    }
    public class ServiceImpl2 : ITest
    {
        public IEnumerable<IService> list;
        [DependencyConstructor]
        public ServiceImpl2(IEnumerable<IService> list) { this.list = list; }
    }
    public class ServiceImpl3 : ITest
    {
        public IService<IRepository> temp;
        [DependencyConstructor]
        public ServiceImpl3(IService<IRepository> temp) { this.temp = temp; }
    }
}
