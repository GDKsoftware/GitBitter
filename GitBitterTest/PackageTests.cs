namespace GitBitterTest
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using GitBitterLib;

    [TestClass]
    public class PackageTests
    {
        [TestMethod]
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

        [TestMethod]
        public void AutoFolder()
        {
            var package = new Package();
            package.Repository = "https://localhost/test.git";

            package.SetDefaultFolder();

            Assert.AreEqual("test", package.Folder);
        }
    }
}
