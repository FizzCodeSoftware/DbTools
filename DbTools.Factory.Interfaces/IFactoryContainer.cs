namespace FizzCode.DbTools.Factory.Interfaces
{
    using System;

    public interface IFactoryContainer
    {
        TFactory Get<TFactory>() where TFactory : notnull;
        bool TryGet<TFactory>(out TFactory? factory) where TFactory : class;
        void Register<TFactory>(Type implementationType) where TFactory : notnull;
        void RegisterInstance<T>(T instance);
    }
}