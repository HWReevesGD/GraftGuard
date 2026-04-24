using GraftGuard.Graphics;
using GraftGuard.UI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Utility;

/// <summary>
/// Runnable Task
/// </summary>
struct Task
{
    /// <summary>
    /// Time that this task starts running
    /// </summary>
    internal float StartTime;

    /// <summary>
    /// Time that this task stops running. Set to -1 to run only once
    /// </summary>
    internal float EndTime;

    public delegate void NoArgsCallback();
    public delegate void GameTimeOnlyCallback(GameTime gameTime);
    public delegate void CombinedCallback(GameTime gameTime, float elapsedTime);

    private NoArgsCallback noArgsCallback;
    private GameTimeOnlyCallback gameTimeOnlyCallback;
    private CombinedCallback combinedCallback;

    /// <summary>
    /// Create a task with a callback with no arguments
    /// </summary>
    /// <param name="startTime">Time that this task starts running</param>
    /// <param name="endTime">Time that this task stops running. Set to -1 to run only once</param>
    /// <param name="callback">NoArgsCallback</param>
    public Task(float startTime, float endTime, NoArgsCallback callback)
    {
        StartTime = startTime;
        EndTime = endTime;
        noArgsCallback = callback;
    }

    /// <summary>
    /// Create a task with a callback that only receives the current frame's GameTime
    /// </summary>
    /// <param name="startTime">Time that this task starts running</param>
    /// <param name="endTime">Time that this task stops running. Set to -1 to run only once</param>
    /// <param name="callback">GameTimeOnlyCallback</param>
    public Task(float startTime, float endTime, GameTimeOnlyCallback callback)
    {
        StartTime = startTime;
        EndTime = endTime;
        gameTimeOnlyCallback = callback;
    }

    /// <summary>
    /// Create a task with a callback that receives both the current frame's GameTime and the time elapsed since the task's StartTime
    /// </summary>
    /// <param name="startTime">Time that this task starts running</param>
    /// <param name="endTime">Time that this task stops running. Set to -1 to run only once</param>
    /// <param name="callback">CombinedCallback</param>
    public Task(float startTime, float endTime, CombinedCallback callback)
    {
        StartTime = startTime;
        EndTime = endTime;
        combinedCallback = callback;
    }

    /// <summary>
    /// Run any callbacks in this Task
    /// </summary>
    /// <param name="gameTime">GameTime from this frame</param>
    /// <param name="elapsedTime">Time since StartTime</param>
    internal void RunCallback(GameTime gameTime, float elapsedTime)
    {
        if (noArgsCallback != null)
            noArgsCallback();

        if (gameTimeOnlyCallback != null)
            gameTimeOnlyCallback(gameTime);

        if (combinedCallback != null)
            combinedCallback(gameTime, elapsedTime);
    }
}

/// <summary>
/// Allows scheduling of tasks ahead of time, allowing for creation of sequential code to actually look sequential
/// </summary>
internal class TaskSchedule
{
    private static readonly List<TaskSchedule> schedules = new List<TaskSchedule>();
    private static readonly List<TaskSchedule> toRemoveNext = new List<TaskSchedule>();

    /// <summary>
    /// Update global task manager
    /// </summary>
    /// <param name="gameTime">GameTime from this frame</param>
    public static void UpdateAll(GameTime gameTime)
    {
        foreach (TaskSchedule schedule in schedules)
        {
            if (toRemoveNext.Contains(schedule))
                continue;
            bool isOk = schedule.Update(gameTime);
            if (!isOk)
                toRemoveNext.Add(schedule);
        }

        foreach (TaskSchedule schedule in toRemoveNext)
            schedules.Remove(schedule);
    }

    private static string RoundTime(float time) => time.ToString("F2");

    /// <summary>
    /// Draw debug text to the screen
    /// </summary>
    /// <param name="drawing">DrawManager</param>
    public static void DrawDebug(DrawManager drawing)
    {
        drawing.DrawString($"# OF SCHEDULES: {schedules.Count}", new Vector2(0, 0), font: Fonts.Arial, drawLayer: 3, isUi: true);

        for (int i = 0; i < schedules.Count; i++)
        {
            TaskSchedule schedule = schedules[i];
            int x = i * 350;

            // draw schedule stats
            //drawing.DrawString($"SCHEDULE #{i}", new Vector2(x, 60), font: Fonts.Arial, drawLayer: 3, isUi: true);
            //drawing.DrawString($"TIME: {RoundTime(schedule.elapsed)}", new Vector2(x, 90), font: Fonts.Arial, drawLayer: 3, isUi: true);
            //drawing.DrawString($"NUM TASKS: {schedule.tasks.Count}", new Vector2(x, 120), font: Fonts.Arial, drawLayer: 3, isUi: true);

            for (int v = 0; v < schedule.tasks.Count; v++)
            {
                Task task = schedule.tasks[v];
                int y = 180 + v * 30;

                // draw start time
                drawing.DrawString($"{RoundTime(task.StartTime)}", new Vector2(x, y), font: Fonts.Arial, drawLayer: 3, isUi: true);

                if (task.EndTime != -1)
                {
                    // draw end time and progress bar
                    drawing.DrawString($"{RoundTime(task.EndTime)}", new Vector2(x + 250, y), font: Fonts.Arial, drawLayer: 3, isUi: true);
                    float barScale = (schedule.elapsed - task.StartTime) / (task.EndTime - task.StartTime);
                    drawing.Draw(Placeholders.TexturePixel, new Rectangle(x + 80, y + 15, 160, 2), drawLayer: 3, isUi: true);
                    drawing.Draw(Placeholders.TexturePixel, new Rectangle(x + 80, y + 15, (int)(160 * barScale), 2), drawLayer: 3, isUi: true, color: Color.Green);
                }
            }
        }
    }

    private float time;
    private float elapsed;
    private List<Task> tasks;
    private event Action OnCancel;

    /// <summary>
    /// Initialize this schedule
    /// </summary>
    public TaskSchedule()
    {
        time = 0;
        tasks = new List<Task>();
        schedules.Add(this);
    }

    /// <summary>
    /// Advance schedule forward without scheduling anything
    /// </summary>
    /// <param name="waitTime">Time to wait</param>
    /// <returns>this</returns>
    public TaskSchedule Wait(float waitTime)
    {
        time += waitTime;
        return this;
    }

    /// <summary>
    /// Add task to the global list and local list
    /// </summary>
    /// <param name="task">Task to add</param>
    private void AddTask(Task task)
    {
        tasks.Add(task);
    }

    /// <summary>
    /// Add a function to run at the current scheduled time
    /// </summary>
    /// <param name="callback">Function to be called</param>
    /// <returns>this</returns>
    public TaskSchedule Run(Task.NoArgsCallback callback)
    {
        AddTask(new Task(time, -1, callback));
        return this;
    }

    /// <summary>
    /// Add a function to run some delay time after the current scheduled time asynchronously (Does not increase time like Wait())
    /// </summary>
    /// <param name="callback">Function to be called</param>
    /// <returns>this</returns>
    public TaskSchedule DelayRun(float delay, Task.NoArgsCallback callback)
    {
        AddTask(new Task(time + delay, -1, callback));
        return this;
    }

    private void AddLoop(float startTime, float endTime, Task.GameTimeOnlyCallback callback) => AddTask(new Task(startTime, endTime, callback));
    private void AddLoop(float startTime, float endTime, Task.CombinedCallback callback) => AddTask(new Task(startTime, endTime, callback));

    /// <summary>
    /// Add a function to run every frame for a given amount of time
    /// </summary>
    /// <param name="loopTime">Time in seconds</param>
    /// <param name="callback">Function to be called</param>
    /// <returns>this</returns>
    public TaskSchedule Loop(float loopTime, Task.GameTimeOnlyCallback callback)
    {
        AddLoop(time, time + loopTime, callback);
        time += loopTime;
        return this;
    }

    /// <summary>
    /// Add a function to run every frame for a given amount of time
    /// </summary>
    /// <param name="loopTime">Time in seconds</param>
    /// <param name="callback">Function to be called</param>
    /// <returns>this</returns>
    public TaskSchedule Loop(float loopTime, Task.CombinedCallback callback)
    {
        AddLoop(time, time + loopTime, callback);
        time += loopTime;
        return this;
    }

    /// <summary>
    /// Add a delegate that runs when this task is cancelled
    /// </summary>
    /// <param name="callback">Delegate (no params)</param>
    /// <returns>this</returns>
    public TaskSchedule OnCancelled(Action callback)
    {
        OnCancel += callback;
        return this;
    }

    /// <summary>
    /// Advance schedule execution forward
    /// </summary>
    /// <param name="gameTime">This frame's GameTime</param>
    public bool Update(GameTime gameTime)
    {
        if (tasks.Count == 0) // if this is called after being removed for whatever reason
            return false;

        elapsed += gameTime.Delta();

        List<Task> toRemove = new List<Task>();

        foreach (Task task in tasks)
        {
            if (task.StartTime <= elapsed)
            {
                task.RunCallback(gameTime, elapsed - task.StartTime);
                if (task.EndTime == -1 || elapsed > task.EndTime)
                    toRemove.Add(task);
            }
            else
                break;
        }

        foreach (Task task in toRemove)
            tasks.Remove(task);

        if (tasks.Count == 0)
            return false;

        return true;
    }

    /// <summary>
    /// Cancel this schedule
    /// </summary>
    public void Cancel()
    {
        toRemoveNext.Add(this);
        OnCancel?.Invoke();
    }
}
