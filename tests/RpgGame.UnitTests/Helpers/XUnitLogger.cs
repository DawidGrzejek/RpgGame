using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.UnitTests.Helpers
{
    public class XUnitLogger<T> : ILogger<T>
    {
        private readonly Xunit.Abstractions.ITestOutputHelper _output;
        public XUnitLogger(Xunit.Abstractions.ITestOutputHelper output) => _output = output;
        public IDisposable BeginScope<TState>(TState state) => null!;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            _output.WriteLine($"[{logLevel}] {formatter(state, exception)}");
            if (exception != null)
                _output.WriteLine(exception.ToString());
        }
    }
}
