using TodoListApp.Api.Models;

namespace TodoListApp.Api.Services;

public interface ITodoService
{
    IEnumerable<TodoItem> GetAll();
    TodoItem? GetById(int id);
    TodoItem Create(TodoItem item);
    TodoItem? Update(int id, TodoItem item);
    bool Delete(int id);
}

public class TodoService : ITodoService
{
    private readonly List<TodoItem> _todos = new();
    private int _nextId = 1;

    public IEnumerable<TodoItem> GetAll()
    {
        return _todos.ToList();
    }

    public TodoItem? GetById(int id)
    {
        return _todos.FirstOrDefault(t => t.Id == id);
    }

    public TodoItem Create(TodoItem item)
    {
        item.Id = _nextId++;
        item.CreatedAt = DateTime.UtcNow;
        _todos.Add(item);
        return item;
    }

    public TodoItem? Update(int id, TodoItem item)
{
    var existingTodo = _todos.FirstOrDefault(t => t.Id == id);
    if (existingTodo == null)
        return null;

    // Check if we're transitioning from not completed to completed
    if (item.IsCompleted && !existingTodo.IsCompleted)
    {
        existingTodo.CompletedAt = DateTime.UtcNow;
    }
    else if (!item.IsCompleted)
    {
        existingTodo.CompletedAt = null;
    }

    // Update other properties
    existingTodo.Title = item.Title;
    existingTodo.Description = item.Description;
    existingTodo.IsCompleted = item.IsCompleted;

    return existingTodo;
}

    public bool Delete(int id)
    {
        var todo = _todos.FirstOrDefault(t => t.Id == id);
        if (todo == null)
            return false;

        _todos.Remove(todo);
        return true;
    }
}
