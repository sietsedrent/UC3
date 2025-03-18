using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UC3.Business;
using UC3.Controllers;
using UC3.Models;
using NToastNotify;
using Microsoft.Extensions.Configuration;
using Xunit;
using System.Collections.Generic;
using System.Net.Http;
using UC3.Data;

public class AccountControllerTests
{
    private readonly AccountController _controller;
    private readonly Dictionary<string, byte[]> _sessionData;
    private readonly Mock<WorkoutContext> _mockContext;

    public AccountControllerTests()
    {
        // Mock the WorkoutContext first
        _mockContext = new Mock<WorkoutContext>();

        // Now we can create the AccountService with the mocked context
        var accountService = new AccountService(_mockContext.Object);

        // Create other dependencies
        var toastNotification = new Mock<IToastNotification>().Object;
        var configuration = new Mock<IConfiguration>().Object;

        _controller = new AccountController(
            _mockContext.Object, // Pass the mock context here too
            accountService,
            new HttpClient(),
            toastNotification,
            configuration
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
    public void Login_Get_ReturnsView()
    {
        var result = _controller.Login();
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Login_Post_InvalidEmail_ReturnsNotFound()
    {
        var result = await _controller.Login(null, "password", 1234, "Send verification");
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Login_Post_UserNotFound_ReturnsViewWithError()
    {
        var result = await _controller.Login("test@example.com", "password", 1234, "Send verification");
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.True(viewResult.ViewData.ModelState.ContainsKey(""));
    }

    [Fact]
    public void Register_Get_ReturnsView()
    {
        var result = _controller.Register();
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Logout_ClearsSession_AndRedirectsToLogin()
    {
        // Setup session
        SetSessionString("IsLoggedIn", "true");

        // Call the method
        var result = _controller.Logout();

        // Check session is cleared
        Assert.Equal("false", GetSessionString("IsLoggedIn"));

        // Check redirect to login
        var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Login", redirectToAction.ActionName);
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
}

// Session implementation
public class TestSession : ISession
{
    private readonly Dictionary<string, byte[]> _data;

    public TestSession(Dictionary<string, byte[]> data)
    {
        _data = data;
    }

    public string Id => "TestSessionId";
    public bool IsAvailable => true;
    public IEnumerable<string> Keys => _data.Keys;

    public void Clear() => _data.Clear();
    public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public void Remove(string key) => _data.Remove(key);
    public void Set(string key, byte[] value) => _data[key] = value;
    public bool TryGetValue(string key, out byte[] value) => _data.TryGetValue(key, out value);
}