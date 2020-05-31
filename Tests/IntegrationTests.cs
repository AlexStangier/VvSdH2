using Core;
using Microsoft.EntityFrameworkCore;
using NetArchTest.Rules;
using NUnit.Framework;

namespace Tests
{
    public class IntegrationTests
    {
        /**
         * Database Tests
         */
        [Test]
        public void CheckIfDatabaseConnectionExists()
        {
            using var context = new ReservationContext();
            Assert.True(context.Database.CanConnect());
        }
        
        [Test]
        public void CheckIfDatabaseTablesExists()
        {
            using var context = new ReservationContext();
            Assert.NotNull(context.Database.GetMigrations());
        }
        
        [Test]
        public void CheckIfDatabaseIsRelational()
        {
            using var context = new ReservationContext();
            Assert.True(context.Database.IsRelational());
        }

        /**
         *Test naming Schemes 
         */
        [Test]
        public void CheckIfApplicationNameingSchemeIsConsistent()
        {
            var types = Types.InCurrentDomain();
            var result = types.That().ResideInNamespace("Application").And().AreClasses().Should().HaveNameEndingWith("Controller").GetResult().IsSuccessful;
            Assert.True(result);
        }
        
        [Test]
        public void CheckIfApplicationSharedNameingSchemeIsConsistent()
        {
            var types = Types.InCurrentDomain();
            var result = types.That().ResideInNamespace("ApplicationShared").And().AreInterfaces().Should().HaveNameStartingWith("I").GetResult().IsSuccessful;
            Assert.True(result);
        }
        
        /**
         * Test Project Attributes
         */
        [Test]
        public void CheckIfApplicationClassesAreSealed()
        {
            var types = Types.InCurrentDomain();
            var result = types.That().ResideInNamespace("Application").And().AreClasses().Should().BeSealed().GetResult().IsSuccessful;
            Assert.True(result);
        }
        
        [Test]
        public void CheckIfApplicationClassesArePublic()
        {
            var types = Types.InCurrentDomain();
            var result = types.That().ResideInNamespace("Application").And().AreClasses().Should().BePublic().GetResult().IsSuccessful;
            Assert.True(result);
        }
        
        [Test]
        public void CheckIfApplicationSharedConsistsOfOnlyInterfaces()
        {
            var types = Types.InCurrentDomain();
            var result = types.That().ResideInNamespace("ApplicationShared").Should().BeInterfaces().GetResult().IsSuccessful;
            Assert.True(result);
        }
        
        /**
         * Test dependencies
         */
        [Test]
        public void CheckIfApplicationSharedDependsOnCore()
        {
            var types = Types.InCurrentDomain();
            var result = types.That().ResideInNamespace("ApplicationShared").Should().HaveDependencyOn("Core")
                .GetResult().IsSuccessful;
            Assert.True(result);
        }
        
        [Test]
        public void CheckIfApplicationDependsOnCore()
        {
            var types = Types.InCurrentDomain();
            var result = types.That().ResideInNamespace("Application").Should().HaveDependencyOn("Core")
                .GetResult().IsSuccessful;
            Assert.True(result);
        }
        
        [Test]
        public void CheckIfApplicationDependsOnApplicationShared()
        {
            var types = Types.InCurrentDomain();
            var result = types.That().ResideInNamespace("Application").Should().HaveDependencyOn("ApplicationShared")
                .GetResult().IsSuccessful;
            Assert.True(result);
        }
    }
}