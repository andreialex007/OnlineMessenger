using System.Data.Entity;
using NUnit.Framework;
using OnlineMessenger.Data.Ef.Models;
using OnlineMessenger.Data.Ef.Utils;

namespace OnlineMessenger.Data.Ef.Tests.Integration
{
    [TestFixture]
    [Category("Integration")]
    public class IntegrationTestBase
    {
        protected MessengerDbContext DbContext;

        protected void InitDataBase()
        {
            CleanTables();
        }

        private void CleanTables()
        {
            DbContext.ClearData();
            DbContext.SaveChanges();
        }

        #region setup/teardown

        [SetUp]
        public virtual void SetUp()
        {
            DbContext = new MessengerDbContext();
            DbContext.InitializeDb();
            InitDataBase();
        }

        [TearDown]
        public virtual void TearDown()
        {
            InitDataBase();
            DbContext.Dispose();
        }

        #endregion setup/teardown


    }

   
}
