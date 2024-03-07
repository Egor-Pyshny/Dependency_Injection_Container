namespace MPP_5.DIExceptions
{
    public class CunstructorNotFoundException : Exception
    {
        public CunstructorNotFoundException(Type t) : base($"Class {t.Name} constructor dont have constructor with DependencyConstructtorAttribute") { }
    }
}
