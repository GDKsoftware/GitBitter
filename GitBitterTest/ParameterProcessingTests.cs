namespace GitBitterTest
{
    using System;
    using GitBitterLib;

#if MONO
    using NUnit.Framework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TestFixture = Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute;
    using Test = Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute;
#endif

    [TestFixture]
    public class ParameterProcessingTests
    {
        [Test]
        public void NoParams()
        {
            var parameters = new ParameterProcessing([]);

            Assert.AreEqual(ParameterCommand.None, parameters.Command);
            Assert.AreEqual("gitbitter.json", parameters.Filepath);
            Assert.AreEqual(null, parameters.CommandArg1);
        }

        [Test]
        public void AlternativeJSON()
        {
            var parameters = new ParameterProcessing(["c:/test/gitbitter.json"]);

            Assert.AreEqual(ParameterCommand.None, parameters.Command);
            Assert.AreEqual("c:/test/gitbitter.json", parameters.Filepath);
            Assert.AreEqual(null, parameters.CommandArg1);
        }

        [Test]
        public void AddPackage()
        {
            var parameters = new ParameterProcessing(["add","https://github.com/GDKsoftware/QueryDesk.git"]);

            Assert.AreEqual(ParameterCommand.Add, parameters.Command);
            Assert.AreEqual("gitbitter.json", parameters.Filepath);
            Assert.AreEqual("https://github.com/GDKsoftware/QueryDesk.git", parameters.CommandArg1);
        }

        [Test]
        public void AddPackageToAlternativeJSON()
        {
            var parameters = new ParameterProcessing(["c:/test/gitbitter.json","add","https://github.com/GDKsoftware/QueryDesk.git"]);

            Assert.AreEqual(ParameterCommand.Add, parameters.Command);
            Assert.AreEqual("c:/test/gitbitter.json", parameters.Filepath);
            Assert.AreEqual("https://github.com/GDKsoftware/QueryDesk.git", parameters.CommandArg1);
        }
    }
}
