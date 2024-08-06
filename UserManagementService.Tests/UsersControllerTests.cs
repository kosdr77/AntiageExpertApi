using Moq;
using Microsoft.AspNetCore.Mvc;
using UserManagementService.Controllers;
using UserManagementService.Domain.Models;
using UserManagementService.DTOs;
using UserManagementService.Services;

namespace UserManagementService.Tests
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _controller = new UsersController(_userServiceMock.Object);
        }

        [Fact]
        public async Task CreateUser_ValidUser_ReturnsCreatedUser()
        {
            // Arrange
            var user = new User
            {
                FirstName = "John",
                Email = "john.doe@example.com"
            };
            var device = "mail";
            _userServiceMock.Setup(service => service.CreateUserAsync(user, device))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.CreateUser(user, device);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnValue = Assert.IsType<User>(createdAtActionResult.Value);
            Assert.Equal(user.FirstName, returnValue.FirstName);
            Assert.Equal(user.Email, returnValue.Email);
        }

        [Fact]
        public async Task CreateUser_InvalidUser_ThrowsArgumentException()
        {
            // Arrange
            var user = new User();
            var device = "mail";
            _userServiceMock.Setup(service => service.CreateUserAsync(user, device))
                .ThrowsAsync(new ArgumentException("First Name and Email are required for mail device"));

            // Act
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _controller.CreateUser(user, device));

            // Assert
            Assert.Equal("First Name and Email are required for mail device", exception.Message);
        }

        [Fact]
        public async Task GetUserById_ExistingId_ReturnsUser()
        {
            // Arrange
            var userId = 1;
            var user = new User { Id = userId };
            _userServiceMock.Setup(service => service.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.GetUserById(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<User>(okResult.Value);
            Assert.Equal(userId, returnValue.Id);
        }

        [Fact]
        public async Task GetUserById_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var userId = 1;
            _userServiceMock.Setup(service => service.GetUserByIdAsync(userId))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _controller.GetUserById(userId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task SearchUsers_ValidParameters_ReturnsUsers()
        {
            // Arrange
            var searchDto = new UserSearchDto { LastName = "Doe" };
            var users = new List<User>
            {
                new() { LastName = "Doe" },
                new() { LastName = "Doe" }
            };
            _userServiceMock.Setup(service => service.SearchUsersAsync(searchDto))
                .ReturnsAsync(users);

            // Act
            var result = await _controller.SearchUsers(searchDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<User>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task SearchUsers_EmptyParameters_ReturnsBadRequest()
        {
            // Arrange
            var userSearchDto = new UserSearchDto();

            // Act
            var result = await _controller.SearchUsers(userSearchDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateUser_InvalidDeviceHeader_ReturnsBadRequest()
        {
            // Arrange
            var user = new User
            {
                FirstName = "John",
                Email = "john.doe@example.com"
            };
            var invalidDevice = "unknown_device";
            _userServiceMock.Setup(service => service.CreateUserAsync(user, invalidDevice))
                .ThrowsAsync(new ArgumentException("Unknown device type"));

            // Act
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _controller.CreateUser(user, invalidDevice));

            // Assert
            Assert.Equal("Unknown device type", exception.Message);
        }

        [Fact]
        public async Task GetUserById_UserCached_ReturnsCachedUser()
        {
            // Arrange
            var userId = 1;
            var user = new User { Id = userId };
            _userServiceMock.Setup(service => service.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.GetUserById(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<User>(okResult.Value);
            Assert.Equal(userId, returnValue.Id);
            _userServiceMock.Verify(service => service.GetUserByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task SearchUsers_WithDifferentParameters_ReturnsCorrectResults()
        {
            // Arrange
            var searchDto = new UserSearchDto
            {
                LastName = "Doe",
                Email = "john.doe@example.com"
            };

            var users = new List<User>
            {
                new() { LastName = "Doe", Email = "john.doe@example.com" },
                new() { LastName = "Doe", Email = "jane.doe@example.com" }
            };

            _userServiceMock.Setup(service => service.SearchUsersAsync(searchDto))
                .ReturnsAsync(users);

            // Act
            var result = await _controller.SearchUsers(searchDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<User>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
            Assert.Contains(returnValue, u => u.Email == "john.doe@example.com");
            Assert.Contains(returnValue, u => u.Email == "jane.doe@example.com");
        }

        [Fact]
        public async Task SearchUsers_WithNoParameters_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Error", "At least one search parameter is required");

            // Act
            var result = await _controller.SearchUsers(new UserSearchDto());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("At least one search parameter is required", badRequestResult.Value);
        }
    }
}