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
    public class PackageTests
    {
        [Test]
        public void Clone()
        {
            var package = new Package();
            package.Repository = "https://localhost/test.git";
            package.Folder = "test";
            package.Branch = "master";

            var clone = (Package)package.Clone();

            Assert.AreEqual(package.Repository, clone.Repository);
            Assert.AreEqual(package.Folder, clone.Folder);
            Assert.AreEqual(package.Branch, clone.Branch);
        }

        [Test]
        public void AutoFolder()
        {
            var package = new Package();
            package.Repository = "https://localhost/test.git";

            package.SetDefaultFolder();

            Assert.AreEqual("test", package.Folder);
        }
    }
}
