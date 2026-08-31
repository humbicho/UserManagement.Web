using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using UserManagement.Client.Models;
using UserManagement.Client.Pages;
using UserManagement.Client.Services;

namespace UserManagement.Tests.Components;

public class UsersTests : BunitContext
{
    private Mock<IUserService> RegisterServices()
    {
        var userService = new Mock<IUserService>();

        userService
            .Setup(x => x.GetUsersAsync())
            .ReturnsAsync(
            [
                new UserDto
                {
                    Id = 1,
                    FirstName = "Humberto",
                    LastName = "Lopez",
                    Username = "hlopez",
                    IsActive = true,
                    Roles = ["Admin"]
                }
            ]);

        Services.AddSingleton(userService.Object);

        return userService;
    }

    [Fact]
    public void ShouldRenderUsers_WhenUsersExist()
    {
        RegisterServices();

        var cut = Render<Users>();

        Assert.Contains(
            "Humberto Lopez",
            cut.Markup);
    }

    [Fact]
    public void ShouldShowNoUsersMessage_WhenUserListIsEmpty()
    {
        // Arrange
        var userService = new Mock<IUserService>();

        userService
            .Setup(x => x.GetUsersAsync())
            .ReturnsAsync([]);

        Services.AddSingleton(userService.Object);

        // Act
        var cut = Render<Users>();

        // Assert
        Assert.Contains(
            "No users found",
            cut.Markup);
    }

    [Fact]
    public async Task ShouldNotCallDeleteUser_WhenDeleteIsCancelled()
    {
        // Arrange
        var userService = RegisterServices();

        JSInterop
            .Setup<bool>(
                "confirm",
                "Are you sure you want to delete this user?")
            .SetResult(false);

        var cut = Render<Users>();

        // Act
        await cut.InvokeAsync(() =>
            cut.Find("button.btn-danger")
                .Click());

        // Assert
        userService.Verify(
            service => service.DeleteUserAsync(
                It.IsAny<int>()),
            Times.Never);
    }

    [Fact]
    public async Task ShouldCallDeleteUserWithCorrectId_WhenDeleteIsConfirmed()
    {
        // Arrange
        var userService = RegisterServices();

        JSInterop
            .Setup<bool>(
                "confirm",
                "Are you sure you want to delete this user?")
            .SetResult(true);

        var cut = Render<Users>();

        // Act
        await cut.InvokeAsync(() =>
            cut.Find("button.btn-danger")
                .Click());

        // Assert
        userService.Verify(
            service => service.DeleteUserAsync(1),
            Times.Once);
    }

    [Fact]
    public async Task ShouldReloadUsers_WhenDeleteIsConfirmed()
    {
        // Arrange
        var userService = RegisterServices();

        JSInterop
            .Setup<bool>(
                "confirm",
                "Are you sure you want to delete this user?")
            .SetResult(true);

        var cut = Render<Users>();

        // Act
        await cut.InvokeAsync(() =>
            cut.Find("button.btn-danger")
                .Click());

        // Assert
        userService.Verify(
            service => service.GetUsersAsync(),
            Times.Exactly(2));
    }

    [Fact]
    public async Task ShouldNavigateToUserForm_WhenEditIsClicked()
    {
        // Arrange
        RegisterServices();

        var navManager =
    Services.GetRequiredService<NavigationManager>();

        var cut = Render<Users>();

        // Act
        await cut.InvokeAsync(() =>
            cut.Find("button.btn-warning")
                .Click());

        // Assert
        Assert.EndsWith(
            "/userform/1",
            navManager.Uri);
    }
}