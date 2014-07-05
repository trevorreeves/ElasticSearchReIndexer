using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchReIndexer.WindsorExtensions
{
    public interface IReleaser<T>
    {
        void Release(T obj);
    }

    public static class ReleasingWrapper
    {
        public static WrappingObject<T> CreateForRelease<T>(
            this IReleaser<T> releaser,
            Func<T> createFunc)
        {
            return new WrappingObject<T>(createFunc(), releaser.Release);
        }
    }

    public class WrappingObject<T> : IDisposable
    {
        private readonly Action<T> _releaseFunc;

        public WrappingObject(T obj, Action<T> releaseFunc)
        {
            this.WrappedObject = obj;
            _releaseFunc = releaseFunc;
        }

        public T WrappedObject { get; private set; }

        public void Dispose()
        {
            _releaseFunc(this.WrappedObject);
        }
    }
}
