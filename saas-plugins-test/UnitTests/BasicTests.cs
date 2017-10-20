using NUnit.Framework;
using System.Reflection;
using System.IO;
using System.Data;
using System.Collections;
using System.Linq;

namespace template_test.UnitTests
{
    [TestFixture]
    public class BasicTests
    {
        [SetUp] public void Setup() {}
        [TearDown] public void TestTearDown() {}

        #region " Set 1 " 

        public static IEnumerable InputCaseObjects {
            get {
                // No Rules or Error --- loading data without any rule or error objects (aka QuickLoad)
                yield return new TestCaseData(1, 2, 3);                    
            }
        }

        //[Ignore("")]
        [Test]
        [TestCaseSource("InputCaseObjects")]
        public void Test1(int a, int b, int c) {
            Assert.IsFalse(false);
        }

        #endregion
    }
}
  