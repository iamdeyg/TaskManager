using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Rewrite;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ITaskservice>(new InMemoryTaskService());

var app = builder.Build();

// In-built middleware
app.UseRewriter(new RewriteOptions().AddRedirect("tasks/(.*)", "todos/$1"));
// Custom middleware
app.Use(async (context, next) => {
    Console.WriteLine($"[{context.Request.Method}{context.Request.Path}{DateTime.UtcNow}] Started.");
    await next(context);
    Console.WriteLine($"[{context.Request.Method}{context.Request.Path}{DateTime.UtcNow}] Finished.");
});

var todos = new List<Todo>();
app.MapGet("/todos", (ITaskservice service) => service.GetTodos());
app.MapGet("/todos/{id}", Results<Ok<Todo>, NotFound> (int id, ITaskservice service) =>
{
    var targetTodo = service.GetTodoById(id);
    return targetTodo is null
    ? TypedResults.NotFound()
    : TypedResults.Ok(targetTodo);
});

app.MapPost("/todos", (Todo task,ITaskservice service) =>
{
    service.AddTodo(task);
    return TypedResults.Created("/todos/{id}", task);
})
.AddEndpointFilter(async (context, next) =>
{
    var taskArgument = context.GetArgument<Todo>(0);
    var errors = new Dictionary<string, string[]>();
    if (taskArgument.DueDate < DateTime.UtcNow)
    {
        errors.Add(nameof(Todo.DueDate), ["Due date cannot be in the past." ]);
    }
    if (taskArgument.IsCompleted)
    {
        errors.Add(nameof(Todo.IsCompleted), ["Cannot add completed todo."]);
    }
      // Return validation errors if any exist
    if (errors.Count > 0)

    {
        return Results.ValidationProblem(errors);
    }

    return await next(context); 
});

app.MapDelete("/todos/{id}", (int id, ITaskservice service) =>
{
    service.DeleteTodoById(id);
    return TypedResults.NoContent();
});

app.Run();

public record Todo(int Id, string Title, bool IsCompleted, DateTime? DueDate);


interface ITaskservice
{
    Todo? GetTodoById(int id);
    List<Todo> GetTodos();
    void DeleteTodoById(int id);
    Todo AddTodo(Todo task);
}

class InMemoryTaskService : ITaskservice
{
    private readonly List<Todo> _todos = [];

    public Todo AddTodo(Todo task)
    {
        _todos.Add(task);
        return task;
    }

    public void DeleteTodoById(int id)
    {
        _todos.RemoveAll(task => id == task.Id);
    }
    public Todo? GetTodoById(int id)
    {
        return _todos.SingleOrDefault(t => id == t.Id);
    }
    public List<Todo> GetTodos()
    {
        return _todos;
    }
}