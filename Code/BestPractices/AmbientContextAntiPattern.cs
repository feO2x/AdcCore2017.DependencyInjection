using Light.GuardClauses;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Xunit.Abstractions;

namespace BestPractices
{
    public sealed class AmbientContextAntiPattern
    {
        private readonly ITestOutputHelper _output;

        public AmbientContextAntiPattern(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void AmbientContext()
        {
            AmbientLoggerContext.Logger = new XunitTestLogger(_output);
            var controller = new SomeController();

            controller.DoSomething();
        }

        public static class AmbientLoggerContext
        {
            public static ILogger Logger { get; set; }
        }

        public sealed class SomeController : Controller
        {
            public IActionResult DoSomething()
            {
                Foo();
                AmbientLoggerContext.Logger.Log("I did something");
                return Ok();
            }

            private static void Foo() { }
        }

        public interface ILogger
        {
            void Log(string message);
        }

        public sealed class XunitTestLogger : ILogger
        {
            private readonly ITestOutputHelper _output;

            public XunitTestLogger(ITestOutputHelper output)
            {
                _output = output.MustNotBeNull();
            }


            public void Log(string message)
            {
                _output.WriteLine(message);
            }
        }
    }
}