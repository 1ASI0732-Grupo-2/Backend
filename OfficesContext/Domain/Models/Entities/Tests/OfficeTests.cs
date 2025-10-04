using NUnit.Framework;
using workstation_backend.OfficesContext.Domain.Models.Entities;
using System.Collections.Generic;

namespace workstation_backend.OfficesContext.Domain.Models.Entities.Tests
{
    [TestFixture]
    public class OfficeTests
    {
        [Test]
        public void Constructor_InitializesPropertiesCorrectly()
        {
            // Arrange
            string location = "Lima";
            string description = "Oficina moderna";
            string imageUrl = "http://example.com/image.jpg";
            int capacity = 10;
            int costPerDay = 100;
            bool available = true;

            // Act
            var office = new Office(location, description, imageUrl, capacity, costPerDay, available);

            // Assert

            Assert.That(office.Location, Is.EqualTo(location));
            Assert.That(office.Description, Is.EqualTo(description));
            Assert.That(office.ImageUrl, Is.EqualTo(imageUrl));
            Assert.That(office.Capacity, Is.EqualTo(capacity));
            Assert.That(office.CostPerDay, Is.EqualTo(costPerDay));
            Assert.That(office.Available, Is.EqualTo(available));
            Assert.That(office.IsActive);
            Assert.That(office.Services, Is.Not.Null);
            Assert.That(office.Ratings, Is.Not.Null);
            Assert.That(office.Services, Is.Empty);
            Assert.That(office.Ratings, Is.Empty);
        }

        [Test]
        public void ParameterlessConstructor_InitializesCollections()
        {
            // Act
            var office = new Office();

            // Assert
            Assert.That(office.Services, Is.Not.Null);
            Assert.That(office.Ratings, Is.Not.Null);
        }

        [Test]
        public void Constructor_WithNegativeCapacity_ThrowsArgumentException()
        {
            // Arrange
            string location = "Lima";
            string description = "Oficina moderna";
            string imageUrl = "http://example.com/image.jpg";
            int negativeCapacity = -1;
            int costPerDay = 100;
            bool available = true;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                new Office(location, description, imageUrl, negativeCapacity, costPerDay, available));
        }

        [Test]
        public void Constructor_WithNegativeCostPerDay_ThrowsArgumentException()
        {
            // Arrange
            string location = "Lima";
            string description = "Oficina moderna";
            string imageUrl = "http://example.com/image.jpg";
            int capacity = 10;
            int negativeCostPerDay = -100;
            bool available = true;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                new Office(location, description, imageUrl, capacity, negativeCostPerDay, available));
        }

        [Test]
        public void Constructor_WithNullLocation_ThrowsArgumentNullException()
        {
            // Arrange
            string? location = null;
            string description = "Oficina moderna";
            string imageUrl = "http://example.com/image.jpg";
            int capacity = 10;
            int costPerDay = 100;
            bool available = true;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new Office(location, description, imageUrl, capacity, costPerDay, available));
        }

        [Test]
        public void AddService_AddsServiceToCollection()
        {
            // Arrange
            var office = new Office("Lima", "Oficina", "http://example.com/image.jpg", 10, 100, true);
            var service = new OfficeService("WiFi", "High-speed internet", 50);

            // Act
            office.Services.Add(service);

            // Assert
            Assert.That(office.Services, Has.Count.EqualTo(1));
            Assert.That(office.Services, Does.Contain(service));
        }

        [Test]
        public void AddRating_AddsRatingToCollection()
        {
            // Arrange
            var office = new Office("Lima", "Oficina", "http://example.com/image.jpg", 10, 100, true);
            var rating = new Rating { Score = 5, Comment = "Excellent office!" };

            // Act
            office.Ratings.Add(rating);

            // Assert
            Assert.That(office.Ratings, Has.Count.EqualTo(1));
            Assert.That(office.Ratings, Does.Contain(rating));
        }

        [Test]
        public void Constructor_WithZeroCapacity_ThrowsArgumentException()
        {
            // Arrange
            string location = "Lima";
            string description = "Oficina moderna";
            string imageUrl = "http://example.com/image.jpg";
            int zeroCapacity = 0;
            int costPerDay = 100;
            bool available = true;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                new Office(location, description, imageUrl, zeroCapacity, costPerDay, available));
        }

        [Test]
        public void Constructor_WithZeroCostPerDay_ThrowsArgumentException()
        {
            // Arrange
            string location = "Lima";
            string description = "Oficina moderna";
            string imageUrl = "http://example.com/image.jpg";
            int capacity = 10;
            int zeroCostPerDay = 0;
            bool available = true;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                new Office(location, description, imageUrl, capacity, zeroCostPerDay, available));
        }
    }
}
