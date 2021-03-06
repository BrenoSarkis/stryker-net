using Moq;
using Serilog.Events;
using Stryker.Core;
using Stryker.Core.Options;
using System;
using Xunit;

namespace Stryker.CLI.UnitTest
{
    public class StrykerCLITests
    {
        [Theory]
        [InlineData("--help")]
        [InlineData("-h")]
        [InlineData("-?")]
        public void StrykerCLI_HelpShouldNotStartStryker(string argName)
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName });

            mock.VerifyNoOtherCalls();
        }

        [Fact]
        public void StrykerCLI_ShouldStartStryker()
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>())).Verifiable();

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { });

            mock.VerifyAll();
        }

        [Fact]
        public void StrykerCLI_ShouldExitOnExceptionInStryker()
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>())).Throws(new Exception("Initial testrun failed")).Verifiable();

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { });

            mock.VerifyAll();
        }

        [Theory]
        [InlineData("--reporter")]
        [InlineData("-r")]
        public void StrykerCLI_ShouldPassReporterArgumentsToStryker(string argName)
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>()));

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "Console" });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o => o.Reporter == "Console")));
        }

        [Theory]
        [InlineData("--project")]
        [InlineData("-p")]
        public void StrykerCLI_ShouldPassProjectArgumentsToStryker(string argName)
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>()));

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "SomeProjectName.csproj" });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o => o.ProjectUnderTestNameFilter == "SomeProjectName.csproj")));
        }

        [Theory]
        [InlineData("--logConsole")]
        [InlineData("-l")]
        public void StrykerCLI_ShouldPassLogConsoleArgumentsToStryker(string argName)
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>()));

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName, "debug" });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o => 
                o.LogOptions.LogLevel == LogEventLevel.Debug && 
                o.LogOptions.LogToFile == false)));
        }

        [Theory]
        [InlineData("--logFile")]
        public void StrykerCLI_ShouldPassLogFileArgumentsToStryker(string argName)
        {
            var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
            mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerOptions>()));

            var target = new StrykerCLI(mock.Object);

            target.Run(new string[] { argName });

            mock.Verify(x => x.RunMutationTest(It.Is<StrykerOptions>(o => o.LogOptions.LogToFile == true)));
        }
    }
}
