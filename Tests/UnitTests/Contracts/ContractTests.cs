using NUnit.Framework;
using ContractsContext.Domain.Models.Entities;
using workstation_backend.ContractsContext.Domain.Models.Entities;
using workstation_backend.ContractsContext.Domain.Models.Enums;

namespace ContractsContext.Tests.Domain.Models.Entities
{
    [TestFixture]
    public class ContractTests
    {
        private Guid _officeId;
        private Guid _ownerId;
        private Guid _renterId;
        private DateTime _startDate;
        private DateTime _endDate;

        [SetUp]
        public void SetUp()
        {
            _officeId = Guid.NewGuid();
            _ownerId = Guid.NewGuid();
            _renterId = Guid.NewGuid();
            _startDate = DateTime.UtcNow;
            _endDate = DateTime.UtcNow.AddMonths(12);
        }

        #region Constructor Tests

        [Test]
        public void Constructor_WithValidParameters_CreatesContractSuccessfully()
        {
            // Arrange & Act
            var contract = new Contract(
                _officeId,
                _ownerId,
                _renterId,
                "Test contract",
                _startDate,
                _endDate,
                1500m,
                50m,
                5.5m
            );

            // Assert
            Assert.That(contract.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(contract.OfficeId, Is.EqualTo(_officeId));
            Assert.That(contract.OwnerId, Is.EqualTo(_ownerId));
            Assert.That(contract.RenterId, Is.EqualTo(_renterId));
            Assert.That(contract.Description, Is.EqualTo("Test contract"));
            Assert.That(contract.StartDate, Is.EqualTo(_startDate));
            Assert.That(contract.EndDate, Is.EqualTo(_endDate));
            Assert.That(contract.BaseAmount, Is.EqualTo(1500m));
            Assert.That(contract.LateFee, Is.EqualTo(50m));
            Assert.That(contract.InterestRate, Is.EqualTo(5.5m));
            Assert.That(contract.Status, Is.EqualTo(ContractStatus.Draft));
            Assert.That(contract.CreatedAt, Is.LessThanOrEqualTo(DateTime.UtcNow));
        }

        [Test]
        public void Constructor_ParameterlessConstructor_CreatesEmptyContract()
        {
            // Arrange & Act
            var contract = new Contract();

            // Assert
            Assert.That(contract, Is.Not.Null);
            Assert.That(contract.Status, Is.EqualTo(ContractStatus.Draft));
        }

        #endregion

        #region AddClause Tests

        [Test]
        public void AddClause_InDraftStatus_AddsClauseSuccessfully()
        {
            // Arrange
            var contract = CreateTestContract();
            var clause = new Clause(contract.Id, "Test Clause", "Content", 1, true);

            // Act
            contract.AddClause(clause);

            // Assert
            Assert.That(contract.Clauses.Count, Is.EqualTo(1));
            Assert.That(contract.Clauses, Does.Contain(clause));
        }

        [Test]
        public void AddClause_InPendingSignaturesStatus_AddsClauseSuccessfully()
        {
            // Arrange
            var contract = CreateTestContract();
            contract.AddSignature(new Signature(contract.Id, _ownerId, "hash1"));
            contract.AddSignature(new Signature(contract.Id, _renterId, "hash2"));
            var clause = new Clause(contract.Id, "Test Clause", "Content", 1, true);

            // Act
            contract.AddClause(clause);

            // Assert
            Assert.That(contract.Clauses.Count, Is.EqualTo(1));
        }

        [Test]
        public void AddClause_InActiveStatus_ThrowsInvalidOperationException()
        {
            // Arrange
            var contract = CreateActiveContract();
            var clause = new Clause(contract.Id, "Test Clause", "Content", 1, true);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => contract.AddClause(clause));
            Assert.That(ex?.Message, Does.Contain("No se pueden agregar cláusulas después de que el contrato esté activo"));
        }

        #endregion

        #region AddSignature Tests

        [Test]
        public void AddSignature_FirstSignature_AddsSuccessfully()
        {
            // Arrange
            var contract = CreateTestContract();
            var signature = new Signature(contract.Id, _ownerId, "hash123");

            // Act
            contract.AddSignature(signature);

            // Assert
            Assert.That(contract.Signatures.Count, Is.EqualTo(1));
            Assert.That(contract.Status, Is.EqualTo(ContractStatus.Draft));
        }

        [Test]
        public void AddSignature_BothPartiesSign_UpdatesStatusToPendingSignatures()
        {
            // Arrange
            var contract = CreateTestContract();
            var ownerSignature = new Signature(contract.Id, _ownerId, "hash1");
            var renterSignature = new Signature(contract.Id, _renterId, "hash2");

            // Act
            contract.AddSignature(ownerSignature);
            contract.AddSignature(renterSignature);

            // Assert
            Assert.That(contract.Signatures.Count, Is.EqualTo(2));
            Assert.That(contract.Status, Is.EqualTo(ContractStatus.PendingSignatures));
        }

        [Test]
        public void AddSignature_ToActiveContract_ThrowsInvalidOperationException()
        {
            // Arrange
            var contract = CreateActiveContract();
            var signature = new Signature(contract.Id, Guid.NewGuid(), "hash");

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => contract.AddSignature(signature));
            Assert.That(ex?.Message, Does.Contain("El contrato ya está activo"));
        }

        #endregion

        #region AddCompensation Tests

        [Test]
        public void AddCompensation_ToActiveContract_AddsSuccessfully()
        {
            // Arrange
            var contract = CreateActiveContract();
            var compensation = new Compensation(
                contract.Id,
                _ownerId,
                _renterId,
                100m,
                "Late payment"
            );

            // Act
            contract.AddCompensation(compensation);

            // Assert
            Assert.That(contract.Compensations.Count, Is.EqualTo(1));
            Assert.That(contract.Compensations, Does.Contain(compensation));
        }

        [Test]
        public void AddCompensation_ToInactiveContract_ThrowsInvalidOperationException()
        {
            // Arrange
            var contract = CreateTestContract();
            var compensation = new Compensation(
                contract.Id,
                _ownerId,
                _renterId,
                100m,
                "Test"
            );

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => contract.AddCompensation(compensation));
            Assert.That(ex?.Message, Does.Contain("Las compensaciones solo se permiten en contratos activos"));
        }

        #endregion

        #region SetReceipt Tests

        [Test]
        public void SetReceipt_WithValidReceipt_AssignsSuccessfully()
        {
            // Arrange
            var contract = CreateTestContract();
            var receipt = new PaymentReceipt(contract.Id, "REC-001", 1500m);

            // Act
            contract.SetReceipt(receipt);

            // Assert
            Assert.That(contract.Receipt, Is.Not.Null);
            Assert.That(contract.Receipt, Is.EqualTo(receipt));
        }

        #endregion

        #region Activate Tests

        [Test]
        public void Activate_WithBothSignatures_ActivatesSuccessfully()
        {
            // Arrange
            var contract = CreateTestContract();
            contract.AddSignature(new Signature(contract.Id, _ownerId, "hash1"));
            contract.AddSignature(new Signature(contract.Id, _renterId, "hash2"));

            // Act
            contract.Activate();

            // Assert
            Assert.That(contract.Status, Is.EqualTo(ContractStatus.Active));
            Assert.That(contract.ActivatedAt, Is.Not.Null);
            Assert.That(contract.ActivatedAt!.Value, Is.LessThanOrEqualTo(DateTime.UtcNow));
        }

        [Test]
        public void Activate_WithoutBothSignatures_ThrowsInvalidOperationException()
        {
            // Arrange
            var contract = CreateTestContract();
            contract.AddSignature(new Signature(contract.Id, _ownerId, "hash1"));

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => contract.Activate());
            Assert.That(ex?.Message, Does.Contain("El contrato debe estar pendiente de firmas."));
        }

        [Test]
        public void Activate_InDraftStatus_ThrowsInvalidOperationException()
        {
            // Arrange
            var contract = CreateTestContract();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => contract.Activate());
            Assert.That(ex?.Message, Does.Contain("El contrato debe estar pendiente de firmas"));
        }

        #endregion

        #region Terminate Tests

        [Test]
        public void Terminate_WithoutPendingCompensations_TerminatesSuccessfully()
        {
            // Arrange
            var contract = CreateActiveContract();

            // Act
            contract.Terminate();

            // Assert
            Assert.That(contract.Status, Is.EqualTo(ContractStatus.Completed));
            Assert.That(contract.TerminatedAt, Is.Not.Null);
            Assert.That(contract.TerminatedAt!.Value, Is.LessThanOrEqualTo(DateTime.UtcNow));
        }

        [Test]
        public void Terminate_WithApprovedCompensations_TerminatesSuccessfully()
        {
            // Arrange
            var contract = CreateActiveContract();
            var compensation = new Compensation(contract.Id, _ownerId, _renterId, 100m, "Test");
            compensation.Approve();
            contract.AddCompensation(compensation);

            // Act
            contract.Terminate();

            // Assert
            Assert.That(contract.Status, Is.EqualTo(ContractStatus.Completed));
        }

        [Test]
        public void Terminate_WithPendingCompensations_ThrowsInvalidOperationException()
        {
            // Arrange
            var contract = CreateActiveContract();
            var compensation = new Compensation(contract.Id, _ownerId, _renterId, 100m, "Pending payment");
            contract.AddCompensation(compensation);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => contract.Terminate());
            Assert.That(ex?.Message, Does.Contain("El contrato no puede finalizar con compensaciones pendientes"));
        }

        #endregion

        #region Cancel Tests

        [Test]
        public void Cancel_InDraftStatus_CancelsSuccessfully()
        {
            // Arrange
            var contract = CreateTestContract();

            // Act
            contract.Cancel();

            // Assert
            Assert.That(contract.Status, Is.EqualTo(ContractStatus.Cancelled));
            Assert.That(contract.TerminatedAt, Is.Not.Null);
            Assert.That(contract.TerminatedAt!.Value, Is.LessThanOrEqualTo(DateTime.UtcNow));
        }

        [Test]
        public void Cancel_InPendingSignaturesStatus_CancelsSuccessfully()
        {
            // Arrange
            var contract = CreateTestContract();
            contract.AddSignature(new Signature(contract.Id, _ownerId, "hash1"));
            contract.AddSignature(new Signature(contract.Id, _renterId, "hash2"));

            // Act
            contract.Cancel();

            // Assert
            Assert.That(contract.Status, Is.EqualTo(ContractStatus.Cancelled));
        }

        [Test]
        public void Cancel_InActiveStatus_ThrowsInvalidOperationException()
        {
            // Arrange
            var contract = CreateActiveContract();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => contract.Cancel());
            Assert.That(ex?.Message, Does.Contain("No se puede cancelar un contrato activo"));
        }

        #endregion

        #region Integration Tests

        [Test]
        public void CompleteWorkflow_DraftToActive_WorksCorrectly()
        {
            // Arrange
            var contract = CreateTestContract();

            // Act & Assert - Add clauses in draft
            var clause = new Clause(contract.Id, "Payment Terms", "Monthly payment", 1, true);
            contract.AddClause(clause);
            Assert.That(contract.Clauses.Count, Is.EqualTo(1));

            // Add signatures
            contract.AddSignature(new Signature(contract.Id, _ownerId, "hash1"));
            Assert.That(contract.Status, Is.EqualTo(ContractStatus.Draft));

            contract.AddSignature(new Signature(contract.Id, _renterId, "hash2"));
            Assert.That(contract.Status, Is.EqualTo(ContractStatus.PendingSignatures));

            // Activate
            contract.Activate();
            Assert.That(contract.Status, Is.EqualTo(ContractStatus.Active));
            Assert.That(contract.ActivatedAt, Is.Not.Null);
        }

        [Test]
        public void CompleteWorkflow_ActiveToCompleted_WorksCorrectly()
        {
            // Arrange
            var contract = CreateActiveContract();

            // Act & Assert - Add compensation
            var compensation = new Compensation(contract.Id, _ownerId, _renterId, 50m, "Repair");
            contract.AddCompensation(compensation);
            Assert.That(contract.Compensations.Count, Is.EqualTo(1));

            // Approve compensation
            compensation.Approve();

            // Terminate
            contract.Terminate();
            Assert.That(contract.Status, Is.EqualTo(ContractStatus.Completed));
            Assert.That(contract.TerminatedAt, Is.Not.Null);
        }

        #endregion

        #region Collections Tests

        [Test]
        public void Clauses_ReturnsReadOnlyCollection()
        {
            // Arrange
            var contract = CreateTestContract();

            // Act
            var clauses = contract.Clauses;

            // Assert
            Assert.That(clauses, Is.Not.Null);
            Assert.That(clauses, Is.InstanceOf<IReadOnlyCollection<Clause>>());
        }

        [Test]
        public void Signatures_ReturnsReadOnlyCollection()
        {
            // Arrange
            var contract = CreateTestContract();

            // Act
            var signatures = contract.Signatures;

            // Assert
            Assert.That(signatures, Is.Not.Null);
            Assert.That(signatures, Is.InstanceOf<IReadOnlyCollection<Signature>>());
        }

        [Test]
        public void Compensations_ReturnsReadOnlyCollection()
        {
            // Arrange
            var contract = CreateTestContract();

            // Act
            var compensations = contract.Compensations;

            // Assert
            Assert.That(compensations, Is.Not.Null);
            Assert.That(compensations, Is.InstanceOf<IReadOnlyCollection<Compensation>>());
        }

        #endregion

        #region Helper Methods

        private Contract CreateTestContract()
        {
            return new Contract(
                _officeId,
                _ownerId,
                _renterId,
                "Test contract description",
                _startDate,
                _endDate,
                1500m,
                50m,
                5.5m
            );
        }

        private Contract CreateActiveContract()
        {
            var contract = CreateTestContract();
            contract.AddSignature(new Signature(contract.Id, _ownerId, "ownerHash"));
            contract.AddSignature(new Signature(contract.Id, _renterId, "renterHash"));
            contract.Activate();
            return contract;
        }

        #endregion
    }
}