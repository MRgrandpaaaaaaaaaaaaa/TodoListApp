using Microsoft.AspNetCore.Mvc;
using TodoListApp.Api.Controllers;
using TodoListApp.Api.Models;
using TodoListApp.Api.Services;
using Xunit;
using FluentAssertions;

namespace TodoListApp.Tests;

public class TodosControllerTests
{
    private readonly TodosController _controller;
    private readonly ITodoService _todoService;

    public TodosControllerTests()
    {
        _todoService = new TodoService();
        _controller = new TodosController(_todoService);
    }

    [Fact]
    public void GetAll_ShouldReturnOkWithAllTodos()
    {
        // Arrange
        _todoService.Create(new TodoItem { Title = "Todo 1", Description = "Desc 1" });
        _todoService.Create(new TodoItem { Title = "Todo 2", Description = "Desc 2" });

        // Act
        var result = _controller.GetAll();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var todos = okResult!.Value as IEnumerable<TodoItem>;
        todos.Should().HaveCount(2);
    }

    [Fact]
    public void GetById_ShouldReturnOk_WhenTodoExists()
    {
        // Arrange
        var created = _todoService.Create(new TodoItem { Title = "Test Todo", Description = "Test" });

        // Act
        var result = _controller.GetById(created.Id);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var todo = okResult!.Value as TodoItem;
        todo.Should().NotBeNull();
        todo!.Id.Should().Be(created.Id);
    }

    [Fact]
    public void GetById_ShouldReturnNotFound_WhenTodoDoesNotExist()
    {
        // Act
        var result = _controller.GetById(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public void Create_ShouldReturnCreated_WithValidTodo()
    {
        // Arrange
        var newTodo = new TodoItem
        {
            Title = "New Todo",
            Description = "New Description",
            IsCompleted = false
        };

        // Act
        var result = _controller.Create(newTodo);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result.Result as CreatedAtActionResult;
        var todo = createdResult!.Value as TodoItem;
        todo.Should().NotBeNull();
        todo!.Title.Should().Be("New Todo");
        todo.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Create_ShouldReturnBadRequest_WhenTitleIsEmpty()
    {
        // Arrange
        var newTodo = new TodoItem
        {
            Title = "",
            Description = "Description",
            IsCompleted = false
        };

        // Act
        var result = _controller.Create(newTodo);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void Update_ShouldReturnOk_WhenTodoExists()
    {
        // Arrange
        var created = _todoService.Create(new TodoItem { Title = "Original", Description = "Original" });
        var updateData = new TodoItem
        {
            Title = "Updated",
            Description = "Updated Description",
            IsCompleted = true
        };

        // Act
        var result = _controller.Update(created.Id, updateData);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var todo = okResult!.Value as TodoItem;
        todo.Should().NotBeNull();
        todo!.Title.Should().Be("Updated");
        todo.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public void Update_ShouldReturnNotFound_WhenTodoDoesNotExist()
    {
        // Arrange
        var updateData = new TodoItem { Title = "Updated", Description = "Updated" };

        // Act
        var result = _controller.Update(999, updateData);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public void Update_ShouldReturnBadRequest_WhenTitleIsEmpty()
    {
        // Arrange
        var created = _todoService.Create(new TodoItem { Title = "Original", Description = "Original" });
        var updateData = new TodoItem { Title = "", Description = "Updated" };

        // Act
        var result = _controller.Update(created.Id, updateData);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void Delete_ShouldReturnNoContent_WhenTodoExists()
    {
        // Arrange
        var created = _todoService.Create(new TodoItem { Title = "To Delete", Description = "Test" });

        // Act
        var result = _controller.Delete(created.Id);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public void Delete_ShouldReturnNotFound_WhenTodoDoesNotExist()
    {
        // Act
        var result = _controller.Delete(999);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }
}
