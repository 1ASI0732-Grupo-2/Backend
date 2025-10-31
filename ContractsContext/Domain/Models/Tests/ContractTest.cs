using System;
using ContractsContext.Domain.Models.Entities;
using NUnit.Framework;
using workstation_backend.ContractsContext.Domain.Models.Entities;
using workstation_backend.ContractsContext.Domain.Models.Enums;

namespace workstation_backend.ContractsContext.Domain.Models.Tests;
[TestFixture]
public class ContractTest
{
    private Guid _officeId;
    private Guid _ownerId;
    private Guid _renterId;
    private Contract _contract;
        
    [SetUp]
    public void Setup()
    {
            
        _officeId = Guid.NewGuid();
        _ownerId = Guid.NewGuid();
        _renterId = Guid.NewGuid();
            
            _contract = new Contract(
                _officeId,
                _ownerId,
                _renterId,
                "Contrato de prueba",
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(30),
                1000m,
                50m,
                0.05m
            );
        }

        [Test]
        public void Should_Create_Contract_Successfully()
        {
            // Arrange & Act
            Assert.Multiple(() =>
            {
                Assert.That(_contract.OfficeId, Is.EqualTo(_officeId));
                Assert.That(_contract.OwnerId, Is.EqualTo(_ownerId));
                Assert.That(_contract.RenterId, Is.EqualTo(_renterId));
                Assert.That(_contract.Description, Is.EqualTo("Contrato de prueba"));
                Assert.That(_contract.Status, Is.EqualTo(ContractStatus.Draft));
                Assert.That(_contract.Clauses, Is.Empty);
                Assert.That(_contract.Signatures, Is.Empty);
                Assert.That(_contract.Compensations, Is.Empty);
            });

        }

        [Test]
        public void AddClause_WhenContractIsDraft_ShouldAddClause()
        {
            var clause = new Clause(_contract.Id, "Uso del espacio", "El arrendatario usará la oficina...", 1, true);

            _contract.AddClause(clause);

            Assert.That(_contract.Clauses, Has.Count.EqualTo(1));
            Assert.That(_contract.Clauses, Does.Contain(clause));
        }

        [Test]
        public void AddClause_WhenContractIsActive_ShouldThrowException()
        {
            var clause = new Clause(_contract.Id, "Pago", "El pago se realizará el día 5 de cada mes.", 1, true);
            typeof(Contract)
                .GetProperty("Status")!
                .SetValue(_contract, ContractStatus.Active);

            Assert.Throws<InvalidOperationException>(() => _contract.AddClause(clause));
        }

        [Test]
        public void Should_Change_Status_To_PendingSignatures_When_Both_Signed()
        {
            // Arrange
            var contract = CreateDraftContract();

            // Act
            contract.AddSignature(new Signature(contract.Id, contract.OwnerId, "Propietario"));
            contract.AddSignature(new Signature(contract.Id, contract.RenterId, "Arrendatario"));

            // Assert
            Assert.That(ContractStatus.PendingSignatures, Is.EqualTo(contract.Status));
        }

        [Test]
        public void Should_Activate_When_Both_Signed()
        {
            // Arrange
            var contract = CreateDraftContract();
            contract.AddSignature(new Signature(contract.Id, contract.OwnerId, "Propietario"));
            contract.AddSignature(new Signature(contract.Id, contract.RenterId, "Arrendatario"));

            // Act
            contract.Activate();

        // Assert
        Assert.That(ContractStatus.Active, Is.EqualTo(contract.Status));
        Assert.That(contract.ActivatedAt, Is.Not.Null);
        
        }

        [Test]
        public void Should_Throw_When_Activating_Without_Signatures()
        {
            var contract = CreateDraftContract();

            var ex = Assert.Throws<InvalidOperationException>(() => contract.Activate());
            Assert.That(ex!.Message, Does.Contain("El contrato debe estar pendiente de firmas."));
        }

        [Test]
        public void Should_Terminate_When_No_Pending_Compensations()
        {
            var contract = CreateActiveContract();

            // Act
            contract.Terminate();

            // Assert
            Assert.That(ContractStatus.Completed,Is.EqualTo(contract.Status));
            Assert.That(contract.TerminatedAt,Is.Not.Null);
        }

        [Test]
        public void Should_Throw_When_Terminating_With_Pending_Compensations()
        {
            var contract = CreateActiveContract();
            var pendingComp = new Compensation(contract.Id, contract.RenterId, contract.OwnerId, 500, "Pago pendiente");
            contract.AddCompensation(pendingComp);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => contract.Terminate());
            Assert.That(ex!.Message, Does.Contain("pendientes"));
        }

        [Test]
        public void Should_Cancel_When_Not_Active()
        {
            var contract = CreateDraftContract();

            contract.Cancel();

            Assert.That(ContractStatus.Cancelled, Is.EqualTo(contract.Status));
        }

        [Test]
        public void Should_Throw_When_Cancelling_Active_Contract()
        {
            var contract = CreateActiveContract();

            Assert.Throws<InvalidOperationException>(() => contract.Cancel());
        }

        // --- Helpers ---

        private Contract CreateDraftContract() =>
            new Contract(
                _officeId,
                _ownerId,
                _renterId,
                "Contrato de oficina",
                DateTime.UtcNow,
                DateTime.UtcNow.AddMonths(1),
                1000m,
                100m,
                0.05m
            );

        private Contract CreateActiveContract()
        {
            var contract = CreateDraftContract();
            contract.AddSignature(new Signature(contract.Id, contract.OwnerId, "Propietario"));
            contract.AddSignature(new Signature(contract.Id, contract.RenterId, "Arrendatario"));
            contract.Activate();
            return contract;
        }
    }
