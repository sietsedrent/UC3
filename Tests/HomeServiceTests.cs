using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using UC3.Business;
using UC3.Models;
using UC3.Data;
using Xunit;
using System.Collections.Generic;
using System.Linq;

public class HomeServiceTests
{
    private readonly HomeService _service;
    private readonly Mock<IWorkoutContext> _mockContext;

    public HomeServiceTests()
    {
        // Mock de interface in plaats van de concrete klasse
        _mockContext = new Mock<IWorkoutContext>();

        // Maak de service met de gemockte context
        _service = new HomeService(_mockContext.Object);
    }

    [Fact]
    public void SetBio_WithValidUser_UpdatesBio()
    {
        // Arrange
        var user = new User
        {
            userId = 1,
            name = "Test User",
            email = "test@example.com",
            bio = "Old bio"
        };
        string newBio = "This is my new bio";

        // Act
        _service.setBio(newBio, user);

        // Assert
        Assert.Equal(newBio, user.bio);
        _mockContext.Verify(m => m.SaveChanges(), Times.Once);
    }

    [Fact]
    public void SetBio_WithNullUser_DoesNotUpdateAnything()
    {
        // Arrange
        User user = null;
        string newBio = "This is my new bio";

        // Act
        _service.setBio(newBio, user);

        // Assert
        _mockContext.Verify(m => m.SaveChanges(), Times.Never);
    }

    [Fact]
    public void GetBio_WithExistingUser_ReturnsBio()
    {
        // Arrange
        int userId = 1;
        string expectedBio = "User's bio";
        var users = new List<User>
        {
            new User { userId = userId, bio = expectedBio }
        }.AsQueryable();

        var mockDbSet = new Mock<DbSet<User>>();
        mockDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
        mockDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
        mockDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
        mockDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(() => users.GetEnumerator());

        // Nu kunnen we de UserModels property zetten op de interface mock
        _mockContext.Setup(c => c.UserModels).Returns(mockDbSet.Object);

        // Act
        string result = _service.getBio(userId);

        // Assert
        Assert.Equal(expectedBio, result);
    }

    [Fact]
    public void GetBio_WithNonExistingUser_ReturnsEmptyString()
    {
        // Arrange
        int nonExistingUserId = 999;
        var users = new List<User>
        {
            new User { userId = 1, bio = "Some bio" }
        }.AsQueryable();

        var mockDbSet = new Mock<DbSet<User>>();
        mockDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
        mockDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
        mockDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
        mockDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(() => users.GetEnumerator());

        // Nu kunnen we de UserModels property zetten op de interface mock
        _mockContext.Setup(c => c.UserModels).Returns(mockDbSet.Object);

        // Act
        string result = _service.getBio(nonExistingUserId);

        // Assert
        Assert.Equal("", result);
    }
}