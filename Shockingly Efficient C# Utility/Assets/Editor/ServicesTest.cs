using System.Collections;
using System.Threading.Tasks;
using NUnit.Framework;
using Service;
using Machine = Machine.Machine;

namespace Editor
{
    // This file test the Service class and it's children (WebService, InputWebService, SSHService...)
    public class ServicesTest
    {

        [TestFixture]
        public class WebServiceTest
        {
            [Test]
            public async Task IsOnlineTest()
            {
                global::Machine.Machine machine = new global::Machine.Machine("127.0.0.1");
                WebService webService = new WebService(machine, "localhost", "127.0.0.1", 8181);
                bool result = await webService.IsOnline();
                Assert.True(result, "localhost is not detected as online.");
                
            }
        }
        
    }
}