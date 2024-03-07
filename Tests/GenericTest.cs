using FluentAssertions;
using MPP_5.DIConfig;
using MPP_5.DIContainer;
using Tests.HelperClasses;

namespace Tests
{
    public class GenericTest
    {
        [Test]
        public void GenericFirstTest()
        {
            DependenciesConfiguration dependenciesConfiguration = new DependenciesConfiguration();
            dependenciesConfiguration.Register(typeof(IService<>), typeof(ServiceImpl<>));
            dependenciesConfiguration.Register<IRepository, RepositoryImpl1>();
            DependencyProvider dependencyProvider = new DependencyProvider(dependenciesConfiguration);
            var service = (ServiceImpl<IRepository>)dependencyProvider.Resolve<IService<IRepository>>();
            service.r.Should().BeOfType<RepositoryImpl1>();
        }

        [Test]
        public void GenericSecondTest()
        {
            DependenciesConfiguration dependenciesConfiguration = new DependenciesConfiguration();
            dependenciesConfiguration.Register<IService<IRepository>, ServiceImpl<IRepository>>();
            dependenciesConfiguration.Register<IRepository, RepositoryImpl1>();
            DependencyProvider dependencyProvider = new DependencyProvider(dependenciesConfiguration);
            var service = (ServiceImpl<IRepository>)dependencyProvider.Resolve<IService<IRepository>>();
            service.r.Should().BeOfType<RepositoryImpl1>();
        }

        [Test]
        public void ResolveAllTest()
        {
            DependenciesConfiguration dependenciesConfiguration = new DependenciesConfiguration();
            dependenciesConfiguration.Register<IService, ServiceImpl>();
            dependenciesConfiguration.Register<IRepository, RepositoryImpl1>();
            dependenciesConfiguration.Register<IService, ServiceImpl1>();
            DependencyProvider dependencyProvider = new DependencyProvider(dependenciesConfiguration);
            var services = dependencyProvider.ResolveAll<IService>();
            services.Should().HaveCount(2);
            services[0].Should().BeOfType<ServiceImpl>();
            services[1].Should().BeOfType<ServiceImpl1>();
        }

        [Test]
        public void GenericToConstructorTest()
        {
            DependenciesConfiguration dependenciesConfiguration = new DependenciesConfiguration();
            dependenciesConfiguration.Register<IService, ServiceImpl>();
            dependenciesConfiguration.Register<IRepository, RepositoryImpl1>();
            dependenciesConfiguration.Register<IService, ServiceImpl1>();
            dependenciesConfiguration.Register<ITest, ServiceImpl2>();
            DependencyProvider dependencyProvider = new DependencyProvider(dependenciesConfiguration);
            var services = (ServiceImpl2)dependencyProvider.Resolve<ITest>();
            services.list.Should().HaveCount(2);
            services.list.ElementAt(0).Should().BeOfType<ServiceImpl>();
            services.list.ElementAt(1).Should().BeOfType<ServiceImpl1>();
        }

        [Test]
        public void GenericToConstructorTestsa()
        {
            DependenciesConfiguration dependenciesConfiguration = new DependenciesConfiguration();
            dependenciesConfiguration.Register<IService, ServiceImpl>();
            dependenciesConfiguration.Register<IRepository, RepositoryImpl1>();
            dependenciesConfiguration.Register<IService, ServiceImpl1>();
            dependenciesConfiguration.Register<ITest, ServiceImpl3>();
            dependenciesConfiguration.Register<IService<IRepository>, ServiceImpl<IRepository>>();
            DependencyProvider dependencyProvider = new DependencyProvider(dependenciesConfiguration);
            var services = (ServiceImpl3)dependencyProvider.Resolve<ITest>();
            services.temp.Should().BeOfType<ServiceImpl<IRepository>>();
        }
    }
}
