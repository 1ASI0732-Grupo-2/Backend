using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using NUnit.Framework;
using workstation_backend.OfficesContext.Application.CommandServices;
using workstation_backend.OfficesContext.Domain;
using workstation_backend.OfficesContext.Domain.Models.Commands;
using workstation_backend.OfficesContext.Domain.Models.Entities;
using workstation_backend.Shared.Domain.Repositories;

namespace workstation_backend.OfficesContext.Application.CommandServices.Test
{
    [TestFixture]
    public class OfficeCommandServiceTest
    {
        private Mock<IOfficeRepository> _officeRepositoryMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IValidator<CreateOfficeCommand>> _validatorMock;
        private OfficeCommandService _service;

        [SetUp]
        public void Setup()
        {
            _officeRepositoryMock = new Mock<IOfficeRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _validatorMock = new Mock<IValidator<CreateOfficeCommand>>();
            _service = new OfficeCommandService(
                _officeRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _validatorMock.Object
            );
        }

        [Test]
        public async Task Handle_CreateOfficeCommand_ValidCommand_CreatesOffice()
        {
            var services = new List<OfficeServiceCommand>
            {
                new OfficeServiceCommand("Service1", "Desc", 10)
            };
            var command = new CreateOfficeCommand(
                "TestLocation",
                "Desc",
                "img",
                10,
                100,
                true,
                services
            );

            _validatorMock.Setup(v => v.ValidateAsync(command, default))
                .ReturnsAsync(new ValidationResult());
            _officeRepositoryMock.Setup(r => r.GetByLocationAsync(command.Location))
                .ReturnsAsync((Office)null);

            Office? addedOffice = null;
            _officeRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Office>()))
                .Callback<Office>(o => addedOffice = o)
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.CompleteAsync()).Returns(Task.CompletedTask);

            var result = await _service.Handle(command);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Location, Is.EqualTo(command.Location));
            Assert.That(result.Description, Is.EqualTo(command.Description));
            Assert.That(result.Capacity, Is.EqualTo(command.Capacity));
            Assert.That(result.CostPerDay, Is.EqualTo(command.CostPerDay));
            Assert.That(result.Available, Is.EqualTo(command.Available));
            Assert.That(result.Services.Count, Is.EqualTo(1));
            Assert.That(result.Services.First().Name, Is.EqualTo("Service1"));
            Assert.That(result, Is.SameAs(addedOffice));
        }

        [Test]
        public void Handle_CreateOfficeCommand_DuplicateLocation_ThrowsDuplicateNameException()
        {
            var command = new CreateOfficeCommand(
                "TestLocation",
                "Desc",
                "img",
                10,
                100,
                true,
                new List<OfficeServiceCommand>()
            );
            _validatorMock.Setup(v => v.ValidateAsync(command, default))
                .ReturnsAsync(new ValidationResult());
            _officeRepositoryMock.Setup(r => r.GetByLocationAsync(command.Location))
                .ReturnsAsync(new Office());

            Assert.ThrowsAsync<DuplicateNameException>(async () => await _service.Handle(command));
        }

        [Test]
        public void Handle_CreateOfficeCommand_InvalidCommand_ThrowsValidationException()
        {
            var command = new CreateOfficeCommand(
                "TestLocation",
                "Desc",
                "img",
                10,
                100,
                true,
                new List<OfficeServiceCommand>()
            );
            var validationResult = new ValidationResult(new List<ValidationFailure>
            {
                new ValidationFailure("Location", "Location is required")
            });
            _validatorMock.Setup(v => v.ValidateAsync(command, default))
                .ReturnsAsync(validationResult);

            Assert.ThrowsAsync<ValidationException>(async () => await _service.Handle(command));
        }

        [Test]
        public async Task Handle_DeleteOfficeCommand_OfficeExists_DeletesOffice()
        {
            var office = new Office("loc", "desc", "img", 1, 1, true) { IsActive = true };
            var command = new DeleteOfficeCommand(Guid.NewGuid());
            _officeRepositoryMock.Setup(r => r.FindByIdAsync(command.Id)).ReturnsAsync(office);

            var result = await _service.Handle(command);

            Assert.That(office.IsActive, Is.False);
            Assert.That(office.UpdatedUserId, Is.EqualTo(87));
            Assert.That(result, Is.True);
            _officeRepositoryMock.Verify(r => r.Update(office), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Test]
        public async Task Handle_DeleteOfficeCommand_OfficeNotFound_ReturnsFalse()
        {
            var command = new DeleteOfficeCommand(Guid.NewGuid());
            _officeRepositoryMock.Setup(r => r.FindByIdAsync(command.Id)).ReturnsAsync((Office)null!);

            var result = await _service.Handle(command);

            Assert.That(result, Is.False);
        }

        [Test]
        public void Handle_UpdateOfficeCommand_OfficeNotFound_ThrowsDataException()
        {
            var id = Guid.NewGuid();
            var command = new UpdateOfficeCommand(id, "loc", "desc", "img", 1, 1, true);
            _officeRepositoryMock.Setup(r => r.FindByIdAsync(id)).ReturnsAsync((Office)null!);

            Assert.ThrowsAsync<DataException>(async () => await _service.Handle(command, id));
        }

        [Test]
        public void Handle_UpdateOfficeCommand_LessThan2DaysSinceCreation_ThrowsInvalidOperationException()
        {
            var id = Guid.NewGuid();
            var office = new Office("loc", "desc", "img", 1, 1, true)
            {
                CreatedDate = DateTime.UtcNow.AddDays(-1)
            };
            var command = new UpdateOfficeCommand(id, "loc", "desc", "img", 1, 1, true);
            _officeRepositoryMock.Setup(r => r.FindByIdAsync(id)).ReturnsAsync(office);

            Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.Handle(command, id));
        }

        [Test]
        public void Handle_UpdateOfficeCommand_LocationChangedBefore6Months_ThrowsInvalidOperationException()
        {
            var id = Guid.NewGuid();
            var office = new Office("loc", "desc", "img", 1, 1, true)
            {
                CreatedDate = DateTime.UtcNow.AddDays(-10),
                ModifiedDate = DateTime.UtcNow.AddMonths(-2)
            };
            var command = new UpdateOfficeCommand(id, "newloc", "desc", "img", 1, 1, true);
            _officeRepositoryMock.Setup(r => r.FindByIdAsync(id)).ReturnsAsync(office);

            Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.Handle(command, id));
        }

        [Test]
        public async Task Handle_UpdateOfficeCommand_ValidUpdate_UpdatesOffice()
        {
            var id = Guid.NewGuid();
            var office = new Office("loc", "desc", "img", 1, 1, true)
            {
                CreatedDate = DateTime.UtcNow.AddMonths(-7),
                ModifiedDate = DateTime.UtcNow.AddMonths(-7)
            };
            var command = new UpdateOfficeCommand(id, "newloc", "newdesc", "newimg", 5, 200, false);
            _officeRepositoryMock.Setup(r => r.FindByIdAsync(id)).ReturnsAsync(office);

            var result = await _service.Handle(command, id);

            Assert.That(office.Location, Is.EqualTo("newloc"));
            Assert.That(office.Description, Is.EqualTo("newdesc"));
            Assert.That(office.ImageUrl, Is.EqualTo("newimg"));
            Assert.That(office.Capacity, Is.EqualTo(5));
            Assert.That(office.CostPerDay, Is.EqualTo(200));
            Assert.That(office.Available, Is.False);
            Assert.That(office.UpdatedUserId, Is.EqualTo(87));
            Assert.That(result, Is.True);
            _officeRepositoryMock.Verify(r => r.Update(office), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }
    }
}
