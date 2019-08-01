namespace FizzCode.DbTools.DataGenerator
{
    public abstract class GeneratorBase<TResult> : GeneratorBase
    {
        public GeneratorBase() : base()
        {
        }
    }

    public abstract class GeneratorBase
    {
        public GeneratorBase()
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
