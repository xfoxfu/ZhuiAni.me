using Elsa.Workflows.Core;
using Elsa.Workflows.Core.Activities;
using Elsa.Workflows.Core.Contracts;
using Elsa.Common.Contracts;
using Elsa.Workflows.Core.Models;

namespace Me.Xfox.ZhuiAnime.Modules.Bangumi.Workflows;

public class HeartbeatWorkflow : WorkflowBase
{
    private readonly ISystemClock _systemClock;

    public HeartbeatWorkflow(ISystemClock systemClock)
    {
        _systemClock = systemClock;
    }

    protected override void Build(IWorkflowBuilder builder)
    {
        builder.Id = Guid.NewGuid().ToString();
        builder.Root = new Sequence
        {
            Activities =
            {
                new Elsa.Scheduling.Activities.Timer()
                {
                    Interval = new(TimeSpan.FromSeconds(1)),
                    CanStartWorkflow = true,
                },
                new WriteLine(new Input<string>($"Heartbeat workflow triggered at {_systemClock.UtcNow.LocalDateTime}"))
            }
        };
    }
}
