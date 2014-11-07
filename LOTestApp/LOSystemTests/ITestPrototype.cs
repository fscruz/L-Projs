using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LOSystemTests
{
    [TestClass]
    public interface ITestPrototype
    {
        string DisplayName { get; }

        [TestMethod]
        string Test();

    }
}
