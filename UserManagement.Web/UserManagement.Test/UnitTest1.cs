using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using UserManagement.Client.Models;
using UserManagement.Client.Pages;
using UserManagement.Client.Services;

namespace UserManagement.Test
{
    public class UnitTest1 : BunitContext 
    {
        [Fact]
        public void Validate_h3_tag_in_users_view()
        {
            var mockUserService = new Mock<IUserService>();

            mockUserService.Setup(x => x.GetUsersAsync()).ReturnsAsync(new List<UserDto>());

            Services.AddSingleton(mockUserService.Object);

            var cut = Render<Users>();

            cut.Find("h3").MarkupMatches("<h3>Users</h3>");
        }
    }
}