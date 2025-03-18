using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using UC3.Business;
using UC3.Controllers;
using UC3.Models;
using UC3.Data;
using NToastNotify;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;

public class HomeControllerTests
{
    private readonly HomeController _controller;
    private readonly Dictionary<string, byte[]> _sessionData;
    private readonly Mock<WorkoutContext> _mockContext;

    public HomeControllerTests()
    {
        // Mock the WorkoutContext first
        _mockContext = new Mock<WorkoutContext>();

        // Create real instances of services (not mocked)
        var homeService = new HomeService(_mockContext.Object);
        var logger = new Mock<ILogger<HomeController>>().Object;
        var httpContextAccessor = new Mock<IHttpContextAccessor>().Object;
        var toastNotification = new Mock<IToastNotification>().Object;

        _controller = new HomeController(
            logger,
            _mockContext.Object,
            homeService,
            httpContextAccessor,
            toastNotification
        );

        // Create a simple session
        _sessionData = new Dictionary<string, byte[]>();
        var session = new TestSession(_sessionData);
        var httpContext = new DefaultHttpContext();
        httpContext.Session = session;

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    [Fact]
    public void Index_UserNotLoggedIn_RedirectsToLogin()
    {
        // Arrange
        SetSessionString("IsLoggedIn", "false");

        // Act
        var result = _controller.Index();

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Login", redirectResult.ActionName);
        Assert.Equal("Account", redirectResult.ControllerName);
    }

    [Fact]
    public void Privacy_UserNotLoggedIn_RedirectsToLogin()
    {
        // Arrange
        SetSessionString("IsLoggedIn", "false");

        // Act
        var result = _controller.Privacy();

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Login", redirectResult.ActionName);
        Assert.Equal("Account", redirectResult.ControllerName);
    }

    [Fact]
    public void Privacy_UserLoggedIn_ReturnsView()
    {
        // Arrange
        SetSessionString("IsLoggedIn", "true");

        // Act
        var result = _controller.Privacy();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Track_UserNotLoggedIn_RedirectsToLogin()
    {
        // Arrange
        SetSessionString("IsLoggedIn", "false");

        // Act
        var result = _controller.Track();

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Login", redirectResult.ActionName);
        Assert.Equal("Account", redirectResult.ControllerName);
    }

    [Fact]
    public void Track_UserLoggedIn_ReturnsView()
    {
        // Arrange
        SetSessionString("IsLoggedIn", "true");

        // Act
        var result = _controller.Track();

        // Assert
        Assert.IsType<ViewResult>(result);
        Assert.Equal("false", GetSessionString("Friends"));
        Assert.Equal("p1", GetSessionString("Photo"));
    }

    // Helper methods for session
    private void SetSessionString(string key, string value)
    {
        _sessionData[key] = System.Text.Encoding.UTF8.GetBytes(value);
    }

    private string GetSessionString(string key)
    {
        if (_sessionData.TryGetValue(key, out byte[] value))
        {
            return System.Text.Encoding.UTF8.GetString(value);
        }
        return null;
    }

    private void SetSessionInt32(string key, int value)
    {
        byte[] bytes = new byte[4];
        bytes[0] = (byte)(value);
        bytes[1] = (byte)(value >> 8);
        bytes[2] = (byte)(value >> 16);
        bytes[3] = (byte)(value >> 24);
        _sessionData[key] = bytes;
    }
}