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
using System.Text.Json;

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
        var result = await _controller.GetWorkouts();

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var resultJson = JsonSerializer.Serialize(jsonResult.Value);
        var resultData = JsonSerializer.Deserialize<Dictionary<string, string>>(resultJson);
        Assert.Equal("Niet ingelogd", resultData["error"]);
    }

    

    [Fact]
    public async Task GetWorkoutDetails_UserNotLoggedIn_ReturnsError()
    {
        // Act
        var result = await _controller.GetWorkoutDetails(1);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var resultJson = JsonSerializer.Serialize(jsonResult.Value);
        var resultData = JsonSerializer.Deserialize<Dictionary<string, string>>(resultJson);
        Assert.Equal("Niet ingelogd", resultData["error"]);
    }

   

    [Fact]
    public async Task SaveWorkout_UserNotLoggedIn_ReturnsUnauthorized()
    {
        // Arrange
        var workoutDTO = new WorkoutDTO();

        // Act
        var result = await _controller.SaveWorkout(workoutDTO);

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }

  

    [Fact]
    public void ControllerConstructor_InitializesFields()
    {
        // Arrange & Act
        var context = new Mock<WorkoutContext>().Object;
        var service = new Mock<WorkoutService>(context).Object;
        var controller = new TrackController(context, service);

        // Assert
        Assert.NotNull(controller);
    }

    [Fact]
    public void ControllerContext_IsInitialized()
    {
        // Assert
        Assert.NotNull(_controller.ControllerContext);
        Assert.NotNull(_controller.ControllerContext.HttpContext);
        Assert.NotNull(_controller.ControllerContext.HttpContext.Session);
    }

    [Fact]
    public void SessionHelperMethods_WorkCorrectly()
    {
        // Arrange
        string testKey = "testKey";
        string testValue = "testValue";
        int testIntValue = 42;

        // Act
        SetSessionString(testKey, testValue);
        string retrievedValue = GetSessionString(testKey);

        SetSessionInt32("intKey", testIntValue);

        // Assert
        Assert.Equal(testValue, retrievedValue);
        Assert.True(_sessionData.ContainsKey("intKey"));
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