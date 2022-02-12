using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace APIsASManager.CoreV3.MyClass
{
    class PeriodicTask 
    {
        static Dictionary<Action, CancellationTokenSource> scheduleAction = new Dictionary<Action, CancellationTokenSource>();
        
        public async Task Run(Action action, TimeSpan period)
        {
            if (scheduleAction.ContainsKey(action))
                return;

            var cancellationTokenSrc = new CancellationTokenSource();
            scheduleAction.Add(action, cancellationTokenSrc);

            Task task = null;
            while(!cancellationTokenSrc.IsCancellationRequested)
            {
                if (task == null || task.IsCompleted)
                    task = Task.Run(action);

                await Task.Delay(period, cancellationTokenSrc.Token);
            }
        }
        public void Stop(Action action)
        {
            if (!scheduleAction.ContainsKey(action))
                return;
            scheduleAction[action].Cancel();
            scheduleAction.Remove(action);
        }
    }
}
