using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GovGraphExplorerMaui;
public class TimeoutManager {
    private readonly ConcurrentDictionary<int, CancellationTokenSource> _timeouts = new();
    private int _nextId = 0;
    private ILogger<TimeoutManager> logger;

    public TimeoutManager(ILoggerFactory loggerFactory) {
        logger = loggerFactory.CreateLogger<TimeoutManager>();
    }

    /// <summary>
    /// Starts a timeout and returns a unique ID.
    /// </summary>
    public int SetTimeout(Func<Task> callback, int milliseconds) {
        int id = Interlocked.Increment(ref _nextId);
        var cts = new CancellationTokenSource();
        _timeouts[id] = cts;
        var token = cts.Token;

        Task.Run(async () => {
            try {
                await Task.Delay(milliseconds, token);
                if (!token.IsCancellationRequested) {
                    await callback();
                }
            } catch (TaskCanceledException) {
                // Ignore cancellation
            } finally {
                _timeouts.TryRemove(id, out _);
            }
        }, token);

        return id;
    }

    /// <summary>
    /// Starts a timeout and returns a unique ID.
    /// </summary>
    public int SetTimeout(Action callback, int milliseconds) {
        int id = Interlocked.Increment(ref _nextId);
        var cts = new CancellationTokenSource();
        _timeouts[id] = cts;
        var token = cts.Token;

        Task.Run(async () => {
            try {
                await Task.Delay(milliseconds, token);
                if (!token.IsCancellationRequested) {
                    callback();
                }
            } catch (TaskCanceledException) {
                // Ignore cancellation
            } finally {
                _timeouts.TryRemove(id, out _);
            }
        }, token);

        return id;
    }

    /// <summary>
    /// Cancels a timeout using its ID.
    /// </summary>
    public void ClearTimeout(int id) {
        if (_timeouts.TryRemove(id, out var cts)) {
            cts.Cancel();
            cts.Dispose();
            logger.LogInformation($"Cleared timeout {id}");
        }
    }
}
