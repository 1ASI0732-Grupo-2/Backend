using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using workstation_backend.OfficesContext.Domain.Models.Entities;
using workstation_backend.OfficesContext.Infrastructure;
using workstation_backend.Shared.Infrastructure.Persistence.Configuration;

namespace workstation_backend.Tests.Integration.OfficesContext
{
    [TestFixture]
    public class OfficeRepositoryIntegrationTest
    {
    private WorkstationContext _dbContext;
    private OfficeRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<WorkstationContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;
            _dbContext = new WorkstationContext(options);
            _repository = new OfficeRepository(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }

        [Test]
        [Description("Should persist and retrieve an Office entity correctly")] 
        public async Task Add_And_Get_Office_Persists_Data()

        {
            var office = new Office("Centro", "Oficina moderna", "img.png", 10, 200, true);
            await _repository.AddAsync(office);
            await _dbContext.SaveChangesAsync();

            var found = await _repository.FindByIdAsync(office.Id);
            Assert.That(found, Is.Not.Null);
            Assert.That(found.Location, Is.EqualTo("Centro"));
            Assert.That(found.Capacity, Is.EqualTo(10));
        }

        [Test]
        [Description("Should update and persist changes to an Office entity")] 
        public async Task Update_Office_Changes_Are_Persisted()

        {
            var office = new Office("Surco", "Oficina pequeña", "img2.png", 5, 100, true);
            await _repository.AddAsync(office);
            await _dbContext.SaveChangesAsync();

            office.Description = "Oficina renovada";
            office.Capacity = 8;
            _repository.Update(office);
            await _dbContext.SaveChangesAsync();

            var found = await _repository.FindByIdAsync(office.Id);
            Assert.That(found.Description, Is.EqualTo("Oficina renovada"));
            Assert.That(found.Capacity, Is.EqualTo(8));
        }

        [Test]
        [Description("Should remove an Office entity from the database")]
        public async Task Delete_Office_Removes_Entity()
        {
            var office = new Office("San Isidro", "Oficina premium", "img3.png", 20, 500, true);
            await _repository.AddAsync(office);
            await _dbContext.SaveChangesAsync();

            _dbContext.Offices.Remove(office);
            await _dbContext.SaveChangesAsync();

            var found = await _repository.FindByIdAsync(office.Id);
            Assert.That(found, Is.Null);
        }
    }
}
