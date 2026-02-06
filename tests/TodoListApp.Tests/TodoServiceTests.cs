using TodoListApp.Api.Models;
using TodoListApp.Api.Services;
using Xunit;
using FluentAssertions;

namespace TodoListApp.Tests;

public class TodoServiceTests
{
    private readonly ITodoService _todoService;

    public TodoServiceTests()
    {
        _todoService = new TodoService();
    }

    [Fact]
    public void Create_ShouldAddNewTodo_AndReturnWithId()
    {
        // Arrange
        var newTodo = new TodoItem
        {
            Title = "Test Todo",
            Description = "Test Description",
            IsCompleted = false
        };

        // Act
        var result = _todoService.Create(newTodo);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Title.Should().Be("Test Todo");
        result.Description.Should().Be("Test Description");
        result.IsCompleted.Should().BeFalse();
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void GetAll_ShouldReturnAllTodos()
    {
        // Arrange
        _todoService.Create(new TodoItem { Title = "Todo 1", Description = "Description 1" });
        _todoService.Create(new TodoItem { Title = "Todo 2", Description = "Description 2" });

        // Act
        var result = _todoService.GetAll();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(t => t.Title == "Todo 1");
        result.Should().Contain(t => t.Title == "Todo 2");
    }

    [Fact]
    public void GetById_ShouldReturnTodo_WhenExists()
    {
        // Arrange
        var created = _todoService.Create(new TodoItem { Title = "Test Todo", Description = "Test" });

        // Act
        var result = _todoService.GetById(created.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(created.Id);
        result.Title.Should().Be("Test Todo");
    }

    [Fact]
    public void GetById_ShouldReturnNull_WhenNotExists()
    {
        // Act
        var result = _todoService.GetById(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Update_ShouldModifyTodo_WhenExists()
    {
        // Arrange
        var created = _todoService.Create(new TodoItem { Title = "Original", Description = "Original Desc" });
        var updateData = new TodoItem
        {
            Title = "Updated",
            Description = "Updated Desc",
            IsCompleted = true
        };

        // Act
        var result = _todoService.Update(created.Id, updateData);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Updated");
        result.Description.Should().Be("Updated Desc");
        result.IsCompleted.Should().BeTrue();
        result.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public void Update_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        var updateData = new TodoItem { Title = "Updated", Description = "Updated" };

        // Act
        var result = _todoService.Update(999, updateData);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Delete_ShouldRemoveTodo_WhenExists()
    {
        // Arrange
        var created = _todoService.Create(new TodoItem { Title = "To Delete", Description = "Test" });

        // Act
        var result = _todoService.Delete(created.Id);

        // Assert
        result.Should().BeTrue();
        _todoService.GetById(created.Id).Should().BeNull();
    }

    [Fact]
    public void Delete_ShouldReturnFalse_WhenNotExists()
    {
        // Act
        var result = _todoService.Delete(999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Create_ShouldIncrementId_ForMultipleTodos()
    {
        // Arrange & Act
        var todo1 = _todoService.Create(new TodoItem { Title = "Todo 1", Description = "Desc 1" });
        var todo2 = _todoService.Create(new TodoItem { Title = "Todo 2", Description = "Desc 2" });
        var todo3 = _todoService.Create(new TodoItem { Title = "Todo 3", Description = "Desc 3" });

        // Assert
        todo1.Id.Should().Be(1);
        todo2.Id.Should().Be(2);
        todo3.Id.Should().Be(3);
    }
}
