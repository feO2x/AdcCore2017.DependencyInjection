using System;
using System.Threading;
using FluentAssertions;
using LightInject;
using Xunit;

namespace BestPractices
{
    public sealed class CustomLifetimes
    {
        [Fact]
        public void CustomPerThreadLifetime()
        {
            var container = new ServiceContainer();
            container.Register<ConcurrentSpy>(new PerThreadLifetime());
            void ResolveD() => container.GetInstance<ConcurrentSpy>();
            var thread1 = new Thread(ResolveD);
            var thread2 = new Thread(ResolveD);

            thread1.Start();
            thread2.Start();
            thread1.Join();
            thread2.Join();

            ConcurrentSpy.NumberOfInstances.Should().Be(2);
        }

        public sealed class ConcurrentSpy
        {
            private static int _numberOfInstances;

            public static int NumberOfInstances => Volatile.Read(ref _numberOfInstances);

            public ConcurrentSpy()
            {
                Interlocked.Increment(ref _numberOfInstances);
            }
        }

        public sealed class PerThreadLifetime : ILifetime
        {
            private readonly ThreadLocal<object> _threadLocal = new ThreadLocal<object>();

            public object GetInstance(Func<object> createInstance, Scope scope)
            {
                if (_threadLocal.IsValueCreated == false)
                    _threadLocal.Value = createInstance();

                return _threadLocal.Value;
            }
        }
    }
}