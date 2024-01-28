using CQRS.Core.Commands;
using CQRS.Core.Infrastructure;

namespace Post.Cmd.Infrastructure.Dispatchers;

public class CommandDispatcher : ICommandDispatcher
{
    private readonly Dictionary<Type, Func<BaseCommand, Task>?> _handlers = new();

    public void RegisterHandler<T>(Func<T, Task> handler) where T : BaseCommand
    {
        if (_handlers.ContainsKey(typeof(T)))
            throw new ArgumentOutOfRangeException(nameof(handler), "You cannot register the same command handler.");

        _handlers.Add(typeof(T), x => handler((T)x));
    }

    public async Task SendAsync(BaseCommand command)
    {
        if (!_handlers.TryGetValue(command.GetType(), out Func<BaseCommand, Task> handler))
            throw new NullReferenceException("No command handler registered.");

        await handler?.Invoke(command);
    }
}