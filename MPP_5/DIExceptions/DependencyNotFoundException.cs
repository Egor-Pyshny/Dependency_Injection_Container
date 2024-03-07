namespace MPP_5.DIExceptions
{
    public class DependencyNotFoundException : Exception
    {
        public DependencyNotFoundException(Type TDep) : base($"Dependency not found: TDependency = {TDep}") { }
    }
}
