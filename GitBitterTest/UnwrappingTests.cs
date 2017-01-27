namespace GitBitterTest
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using GitBitterLib;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class UnwrappingTests
    {
        [TestMethod]
        public void BasicPackageUnwrapping()
        {
            var currentWorkdirectory = Path.GetDirectoryName(Path.GetFullPath("gitbitter.json"));
            currentWorkdirectory = Path.GetFullPath(Path.Combine(currentWorkdirectory, ".."));

            var clonerMock = new Mock<ICloner>();
            GitBitterContainer.Default.RegisterInstance<ICloner>(clonerMock.Object);

            var package = new Package()
            {
                Folder = "test",
                Repository = "https://localhost/test.git",
                Branch = "master"
            };

            clonerMock
                .Setup(foo => foo.Clone(package.Repository, currentWorkdirectory, package.Folder, package.Branch))
                .Returns(new Task(() => { }));

            var unwrapper = new PackageUnwrapper();
            unwrapper.Settings.Packages.Add(package);

            unwrapper.StartAndWaitForUnwrapping();

            clonerMock.Verify(foo => foo.Clone(package.Repository, currentWorkdirectory, package.Folder, package.Branch));
        }
    }
}
