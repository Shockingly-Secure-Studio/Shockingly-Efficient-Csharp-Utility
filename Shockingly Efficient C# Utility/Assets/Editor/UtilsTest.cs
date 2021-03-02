using System.Collections;
using NUnit.Framework;

namespace Editor
{
    public static class UtilsTest
    {

        [TestFixture]
        public class TestIsProgramInstalled
        {
            [Test]
            public void ProgramIsInstalled()
            {
                Assert.That(
                    Utils.IsProgrammInstalled("cmd"),
                    "Program cmd is installed but is not detected properly."
                    );
            }

            [Test]
            public void ProgramIsNotInstalled()
            {
                Assert.IsFalse(
                    Utils.IsProgrammInstalled("Idonotexists"),
                    "Program is detected as existing but is not"
                    );
            }
        }
        
        
    }
}