using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UC3.Business;
using UC3.Controllers;
using UC3.Models;
using UC3.Data;
using Xunit;
using System.Collections.Generic;
using System.Linq;

public class TrackControllerTests
{
    private readonly TrackController _controller;
    private readonly Dictionary<string, byte[]> _sessionData;
    private readonly Mock<WorkoutContext> _mockContext;

    public TrackControllerTests()
    {
        // Mock the WorkoutContext first
        _mockContext = new Mock<WorkoutContext>();

        // Create real service with mocked context
        var workoutService = new WorkoutService(_mockContext.Object);

        // Create the controller with real service and mocked context
        _controller = new TrackController(
            _mockContext.Object,
            workoutService
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
    public void Index_ReturnsView()
    {
        // Act
        var result = _controller.Index();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void NewWorkout_ReturnsCorrectView()
    {
        // Act
        var result = _controller.NewWorkout() as ViewResult;

        // Assert
        Assert.IsType<ViewResult>(result);
        Assert.Equal("~/Views/Home/NewWorkout.cshtml", result.ViewName);
    }

    [Fact]
    public async Task GetWorkouts_UserNotLoggedIn_ReturnsError()
    {
        // Act
        var result = await _controller.GetWorkouts() as JsonResult;

        // Assert
        Assert.IsType<JsonResult>(result);
        dynamic data = result.Value;
        Assert.Equal("Niet ingelogd", data.error);
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