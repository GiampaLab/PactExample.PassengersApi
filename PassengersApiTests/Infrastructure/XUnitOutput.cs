using PactNet.Infrastructure.Outputters;
using Xunit.Abstractions;

namespace PassengersApiTests
{
    internal class XUnitOutput : IOutput
    {
        private ITestOutputHelper _output;

        public XUnitOutput(ITestOutputHelper output)
        {
            _output = output;
        }

        public void WriteLine(string line)
        {
            _output.WriteLine(line);
        }
    }
}