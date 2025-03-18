using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UC3.Business;
using UC3.Models;
using UC3.Data;
using Xunit;
using System.Collections.Generic;
using System.Linq;

public class AccountServiceTests
{
    private readonly AccountService _service;
    private readonly Mock<WorkoutContext> _mockContext;

    public AccountServiceTests()
    {
        // Mock the WorkoutContext
        _mockContext = new Mock<WorkoutContext>();

        // Create the service with mocked context
        _service = new AccountService(_mockContext.Object);
    }

    [Fact]
    public void ValidLogin_WithValidCredentials_ReturnsTrue()
    {
        // Arrange
        string email = "test@example.com";
        string password = "password123";

        var users = new List<User>
        {
            new User { userId = 1, email = email, password = password }
        }.AsQueryable();

        var mockDbSet = new Mock<Microsoft.EntityFrameworkCore.DbSet<User>>();
        mockDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
        mockDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
        mockDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
        mockDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(() => users.GetEnumerator());

        mockDbSet.Setup(m => m.Any(It.IsAny<Func<User, bool>>()))
            .Returns<Func<User, bool>>(predicate => users.Any(predicate));

        _mockContext.Setup(c => c.UserModels).Returns(mockDbSet.Object);

        // Act
        bool result = _service.ValidLogin(email, password);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValidLogin_WithInvalidCredentials_ReturnsFalse()
    {
        // Arrange
        string validEmail = "test@example.com";
        string validPassword = "password123";
        string invalidEmail = "wrong@example.com";
        string invalidPassword = "wrongpassword";

        var users = new List<User>
        {
            new User { userId = 1, email = validEmail, password = validPassword }
        }.AsQueryable();

        var mockDbSet = new Mock<Microsoft.EntityFrameworkCore.DbSet<User>>();
        mockDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
        mockDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
        mockDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
        mockDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(() => users.GetEnumerator());

        mockDbSet.Setup(m => m.Any(It.IsAny<Func<User, bool>>()))
            .Returns<Func<User, bool>>(predicate => users.Any(predicate));

        _mockContext.Setup(c => c.UserModels).Returns(mockDbSet.Object);

        // Act
        bool resultWrongEmail = _service.ValidLogin(invalidEmail, validPassword);
        bool resultWrongPassword = _service.ValidLogin(validEmail, invalidPassword);
        bool resultBothWrong = _service.ValidLogin(invalidEmail, invalidPassword);

        // Assert
        Assert.False(resultWrongEmail);
        Assert.False(resultWrongPassword);
        Assert.False(resultBothWrong);
    }

    [Fact]
    public void Register_WithNewEmail_CreatesNewUser()
    {
        // Arrange
        string email = "new@example.com";
        string password = "password123";
        string name = "New User";

        var users = new List<User>().AsQueryable();
        var usersCollection = new List<User>();

        var mockDbSet = new Mock<Microsoft.EntityFrameworkCore.DbSet<User>>();
        mockDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
        mockDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
        mockDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
        mockDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(() => users.GetEnumerator());

        mockDbSet.Setup(d => d.Add(It.IsAny<User>())).Callback<User>(e => usersCollection.Add(e));

        _mockContext.Setup(m => m.UserModels).Returns(mockDbSet.Object);
        _mockContext.Setup(m => m.SaveChanges()).Returns(1);

        // Act
        _service.Register(email, password, name);

        // Assert
        Assert.Single(usersCollection);
        Assert.Equal(email, usersCollection[0].email);
        Assert.Equal(password, usersCollection[0].password);
        Assert.Equal(name, usersCollection[0].name);
        Assert.Equal("empty", usersCollection[0].bio);
        Assert.Equal(1, usersCollection[0].role);
        _mockContext.Verify(m => m.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Register_WithExistingEmail_DoesNotCreateUser()
    {
        // Arrange
        string existingEmail = "existing@example.com";
        string password = "password123";
        string name = "Test User";

        var users = new List<User>
        {
            new User { userId = 1, email = existingEmail, password = "oldpassword", name = "Old Name" }
        }.AsQueryable();

        var usersCollection = new List<User>(users);

        var mockDbSet = new Mock<Microsoft.EntityFrameworkCore.DbSet<User>>();
        mockDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
        mockDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
        mockDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
        mockDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(() => users.GetEnumerator());

        mockDbSet.Setup(d => d.Add(It.IsAny<User>())).Callback<User>(e => usersCollection.Add(e));

        _mockContext.Setup(m => m.UserModels).Returns(mockDbSet.Object);

        // Act
        _service.Register(existingEmail, password, name);

        // Assert
        Assert.Single(usersCollection); // Should still only have the original user
        _mockContext.Verify(m => m.SaveChanges(), Times.Never);
    }
}