using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Tools.Connections;

namespace TestConnectionTools
{
    [TestClass]
    public class UnitTest1
    {
        private const string CONNECTION_STRING = @"Data Source=DESKTOP-7UPNDP0\DEV2019;Initial Catalog=TestDatabase;Integrated Security=True;";

        [TestMethod]
        public void TestOutputParameter1()
        {
            Connection connection = new Connection(CONNECTION_STRING, SqlClientFactory.Instance);
            Command command = new Command("TestProcedure1", true);
            command.AddParameter("x", 5);
            command.AddParameter("y", 7);
            command.AddParameter("result", -1, Direction.Output);

            connection.ExecuteNonQuery(command);
            //récupère la valeur du paramètre output
            int result = (int)command["result"];
            //Test
            Assert.AreEqual(12, result);
        }

        [TestMethod]
        public void TestOutputParameter2()
        {
            Connection connection = new Connection(CONNECTION_STRING, SqlClientFactory.Instance);
            Command command = new Command("TestProcedure2", true);
            command.AddParameter("x", 5);
            command.AddParameter("y", 7);
            command.AddParameter("result", -1, Direction.Output);

            //récupère la valeur scalaire du select
            int resultSelect = (int)connection.ExecuteScalar(command);
            //récupère la valeur du paramètre output
            int result = (int)command["result"];

            //Test
            Assert.AreEqual(12, result);
            Assert.AreEqual(12, resultSelect);
        }

        [TestMethod]
        public void TestOutputParameter3()
        {
            Connection connection = new Connection(CONNECTION_STRING, SqlClientFactory.Instance);
            Command command = new Command("TestProcedure3", true);
            command.AddParameter("x", 5);
            command.AddParameter("y", 7);
            command.AddParameter("result", -1, Direction.Output);

            //récupère la valeur tabulaire du select
            var resultSelect = connection.ExecuteReader(command, dr => new { X = (int)dr["x"], Y = (int)dr["y"], Result = (int)dr["addition"] }).Single();

            //récupère la valeur du paramètre output
            int result = (int)command["result"];

            //Test
            Assert.AreEqual(12, result);
            Assert.AreEqual(12, resultSelect.Result);
            Assert.AreEqual(5, resultSelect.X);
            Assert.AreEqual(7, resultSelect.Y);
        }
    }
}
