using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit;
using NUnit.Framework;

namespace SqlConnectify.Test
{
    [TestFixture]
    public class ConnectifyManagerTest
    {
        private string _connectionString = @"Data Source=tienquang-pc\fuckingserver;Initial Catalog=dasaigon;Integrated Security=True";
        private ConnectifyManager _connectManager;

        [SetUp]
        public void Setup()
        {
            _connectManager = new ConnectifyManager(_connectionString);
        }

        [Test]
        public void ReadTest()
        {   
            var table = _connectManager.Read("select * from products");
            
            Assert.That(table.Rows.Count, Is.EqualTo(8));
        }
    }
}
