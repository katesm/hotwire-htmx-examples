namespace hotwire_turbo_stimulus_demo.Models;

public sealed class TaskStore
{
    private readonly object _sync = new();
    private readonly List<LearningTask> _tasks =
    [
        new(1, "Open the Turbo Frame source", false),
        new(2, "Click a task to toggle it", false),
        new(3, "Use the Stimulus controls", true)
    ];
    private int _nextId = 4;

    public IReadOnlyList<LearningTask> GetAll()
    {
        lock (_sync)
        {
            return _tasks.ToList();
        }
    }

    public LearningTask Add(string title)
    {
        lock (_sync)
        {
            var task = new LearningTask(_nextId++, title.Trim(), false);
            _tasks.Add(task);
            return task;
        }
    }

    public LearningTask? Toggle(int id)
    {
        lock (_sync)
        {
            var index = _tasks.FindIndex(task => task.Id == id);
            if (index < 0)
            {
                return null;
            }

            var task = _tasks[index] with { IsComplete = !_tasks[index].IsComplete };
            _tasks[index] = task;
            return task;
        }
    }
}
