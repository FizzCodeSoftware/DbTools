namespace FizzCode.DbTools.DataGenerator
{
    public abstract class GeneratorBase<TResult> : GeneratorBase
    {
        protected GeneratorBase()
        {
        }
    }

    public abstract class GeneratorBase
    {
        protected GeneratorBase()
        {
        }

        public GeneratorContext Context { get; protected set; }

        public void SetContext(GeneratorContext context)
        {
            Context = context;
        }

        public abstract object Get();
    }
}
