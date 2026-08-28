using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using UserManagement.Client.Models;
using UserManagement.Client.Pages;
using UserManagement.Client.Services;

namespace UserManagement.Tests.Components;

public class UserFormTests : BunitContext
{
    private void RegisterServices()
    {
        var userService = new Mock<IUserService>();

        var roleService = new Mock<IRoleService>();

        roleService
        .Setup(x => x.GetRolesAsync())
        .ReturnsAsync(
        [
            new RoleDto
            {
                Id = 1,
                Name = "Admin"
            }
        ]);

        Services.AddSingleton(userService.Object);

        Services.AddSingleton(roleService.Object);
    }

    private async Task FillValidForm(IRenderedComponent<UserForm> cut)
    {
        await cut.InvokeAsync(() =>
            cut.FindAll("input[type='checkbox']")[1]
                .Change(true));

        await cut.InvokeAsync(() =>
            cut.Find("input[type='number']")
                .Change("25"));

        var inputs = cut.FindAll("input.form-control");

        await cut.InvokeAsync(() => inputs[0].Change("Humberto"));
        await cut.InvokeAsync(() => inputs[1].Change("Lopez"));
        await cut.InvokeAsync(() => inputs[3].Change("hlopez"));
        await cut.InvokeAsync(() => inputs[4].Change("Password123"));
    }

    [Fact]
    public void ShouldShowNameFields_WhenUseUsernameIsNotSelected()
    {
        RegisterServices();

        var cut = Render<UserForm>();

        Assert.Contains("First Name", cut.Markup);

        Assert.Contains("Last Name", cut.Markup);
    }

    [Fact]
    public void ShouldHideNameFields_WhenUseUsernameSelected()
    {
        RegisterServices();

        var cut = Render<UserForm>();

        var checkbox =
            cut.Find("input[type='checkbox']");

        checkbox.Change(true);

        Assert.DoesNotContain(
            "First Name",
            cut.Markup);

        Assert.DoesNotContain(
            "Last Name",
            cut.Markup);
    }

    [Fact]
    public void ShouldShowNameFields_WhenUseUsernameDeselected()
    {
        RegisterServices();

        var cut = Render<UserForm>();

        var checkbox =
            cut.Find("input[type='checkbox']");

        checkbox.Change(true);

        checkbox.Change(false);

        Assert.Contains(
            "First Name",
            cut.Markup);

        Assert.Contains(
            "Last Name",
            cut.Markup);
    }

    [Fact]
    public void ShouldShowAdminMessage_WhenAdminRoleSelected()
    {
        RegisterServices();

        var cut = Render<UserForm>();

        var checkboxes =
            cut.FindAll("input[type='checkbox']");

        var adminCheckbox = checkboxes[1];

        adminCheckbox.Change(true);

        Assert.Contains(
            "This user will have full access to the application.",
            cut.Markup);
    }

    [Fact]
    public void ShouldHideAdminMessage_WhenAdminRoleNotSelected()
    {
        RegisterServices();

        var cut = Render<UserForm>();

        Assert.DoesNotContain(
            "This user will have full access to the application.",
            cut.Markup);
    }

    [Fact]
    public void ShouldNotShowUnder21Message_WhenAgeIsNotEntered()
    {
        RegisterServices();

        var cut = Render<UserForm>();

        Assert.DoesNotContain(
            "Additional approval is required for users under 21.",
            cut.Markup);
    }

    [Fact]
    public void ShouldShowUnder21Message_WhenAgeIsUnder21()
    {
        RegisterServices();

        var cut = Render<UserForm>();

        var ageInput =
            cut.Find("input[type='number']");

        ageInput.Change("20");

        Assert.Contains(
            "Additional approval is required for users under 21.",
            cut.Markup);
    }

    [Fact]
    public void ShouldHideUnder21Message_WhenAgeIs21OrMore()
    {
        RegisterServices();

        var cut = Render<UserForm>();

        var ageInput =
            cut.Find("input[type='number']");

        ageInput.Change("20");
        ageInput.Change("21");

        Assert.DoesNotContain(
            "Additional approval is required for users under 21.",
            cut.Markup);
    }

    [Fact]
    public void ShouldDisableSave_WhenNoRolesAreSelected()
    {
        RegisterServices();

        var cut = Render<UserForm>();

        var saveButton =
            cut.Find("button[type='submit']");

        Assert.True(
            saveButton.HasAttribute("disabled"));
    }

    [Fact]
    public void ShouldEnableSave_WhenRoleSelected()
    {
        RegisterServices();

        var cut = Render<UserForm>();

        var saveButton =
            cut.Find("button[type='submit']");

        var checkboxes =
            cut.FindAll("input[type='checkbox']");

        var adminCheckbox = checkboxes[1];

        adminCheckbox.Change(true);

        Assert.False(
            saveButton.HasAttribute("disabled"));
    }

    [Fact]
    public async Task ShouldShowApiErrorMessage_WhenSaveFails()
    {
        // Arrange
        var userService = new Mock<IUserService>();

        userService
            .Setup(x => x.CreateUserAsync(
                It.IsAny<CreateUserRequest>()))
            .ThrowsAsync(new Exception());

        var roleService = new Mock<IRoleService>();

        roleService
            .Setup(x => x.GetRolesAsync())
            .ReturnsAsync(
            [
                new RoleDto
                {
                    Id = 1,
                    Name = "Admin"
                }
            ]);

        Services.AddSingleton(userService.Object);
        Services.AddSingleton(roleService.Object);

        var cut = Render<UserForm>();

        // Act
        await cut.InvokeAsync(() =>
            cut.FindAll("input[type='checkbox']")[1]
                .Change(true));

        await cut.InvokeAsync(() =>
            cut.FindAll("input.form-control")[0]
                .Change("Humberto"));

        await cut.InvokeAsync(() =>
            cut.FindAll("input.form-control")[1]
                .Change("Lopez"));

        await cut.InvokeAsync(() =>
            cut.Find("input[type='number']")
                .Change("25"));

        await cut.InvokeAsync(() =>
            cut.FindAll("input.form-control")[3]
                .Change("hlopez"));

        await cut.InvokeAsync(() =>
            cut.FindAll("input.form-control")[4]
                .Change("Password123"));

        await cut.InvokeAsync(() =>
            cut.Find("button[type='submit']")
                .Click());

        // Assert
        cut.WaitForAssertion(() =>
            Assert.Contains(
                "An error occurred while saving the user.",
                cut.Markup));
    }

    [Fact]
    public async Task ShouldShowSpinner_WhileSavingUser()
    {
        // Arrange
        var saveTask = new TaskCompletionSource(
            TaskCreationOptions.RunContinuationsAsynchronously);

        var userService = new Mock<IUserService>();

        userService
            .Setup(x => x.CreateUserAsync(
                It.IsAny<CreateUserRequest>()))
            .Returns(saveTask.Task);

        var roleService = new Mock<IRoleService>();

        roleService
            .Setup(x => x.GetRolesAsync())
            .ReturnsAsync(
            [
                new RoleDto
                {
                    Id = 1,
                    Name = "Admin"
                }
            ]);

        Services.AddSingleton(userService.Object);
        Services.AddSingleton(roleService.Object);

        var cut = Render<UserForm>();

        await cut.InvokeAsync(() =>
            cut.FindAll("input[type='checkbox']")[1]
                .Change(true));

        await cut.InvokeAsync(() =>
            cut.FindAll("input.form-control")[0]
                .Change("Humberto"));

        await cut.InvokeAsync(() =>
            cut.FindAll("input.form-control")[1]
                .Change("Lopez"));

        await cut.InvokeAsync(() =>
            cut.Find("input[type='number']")
                .Change("25"));

        await cut.InvokeAsync(() =>
            cut.FindAll("input.form-control")[3]
                .Change("hlopez"));

        await cut.InvokeAsync(() =>
            cut.FindAll("input.form-control")[4]
                .Change("Password123"));

        // Act
        var clickTask = cut.InvokeAsync(() =>
            cut.Find("button[type='submit']")
                .Click());

        // Assert
        cut.WaitForElement(".spinner-border");

        Assert.NotNull(
            cut.Find(".spinner-border"));

        // Finaliza la operación pendiente
        saveTask.SetResult();

        await clickTask;
    }

    [Fact]
    public async Task ShouldHideSpinner_WhenSaveCompletes()
    {
        var saveTask = new TaskCompletionSource(
            TaskCreationOptions.RunContinuationsAsynchronously);

        var userService = new Mock<IUserService>();

        userService
            .Setup(x => x.CreateUserAsync(
                It.IsAny<CreateUserRequest>()))
            .Returns(saveTask.Task);

        var roleService = new Mock<IRoleService>();

        roleService
            .Setup(x => x.GetRolesAsync())
            .ReturnsAsync(
            [
                new RoleDto
                {
                    Id = 1,
                    Name = "Admin"
                }
            ]);

        Services.AddSingleton(userService.Object);

        Services.AddSingleton(roleService.Object);

        var cut = Render<UserForm>();

        await cut.InvokeAsync(() =>
            cut.FindAll("input[type='checkbox']")[1]
                .Change(true));

        await cut.InvokeAsync(() =>
            cut.FindAll("input.form-control")[0]
                .Change("Humberto"));

        await cut.InvokeAsync(() =>
            cut.FindAll("input.form-control")[1]
                .Change("Lopez"));

        await cut.InvokeAsync(() =>
            cut.Find("input[type='number']")
                .Change("25"));

        await cut.InvokeAsync(() =>
            cut.FindAll("input.form-control")[3]
                .Change("hlopez"));

        await cut.InvokeAsync(() =>
            cut.FindAll("input.form-control")[4]
                .Change("Password123"));

        var clickTask = cut.InvokeAsync(() =>
        cut.Find("button[type='submit']")
        .Click());

        cut.WaitForElement(".spinner-border");

        saveTask.SetResult();

        await clickTask;

        cut.WaitForAssertion(() =>
            Assert.Empty(cut.FindAll(".spinner-border")));
    }
}
