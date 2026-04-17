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
    public static TaskSchedule GlobalTasks { get; private set; }
    private static float elapsed;
    private static List<Task> tasks;

    /// <summary>
    /// Create the global task manager
    /// </summary>
    public static void CreateGlobalTaskSchedule()
    {
        tasks = new List<Task>();
    }

    /// <summary>
    /// Update global task manager
    /// </summary>
    /// <param name="gameTime">GameTime from this frame</param>
    public static void Update(GameTime gameTime)
    {
        elapsed += gameTime.Delta();

        if (tasks.Count == 0)
            return;
        
        Task firstTask = tasks[0];

        if (firstTask.StartTime <= elapsed)
        {
            firstTask.RunCallback(gameTime, elapsed - firstTask.StartTime);
            if (firstTask.EndTime == -1 || elapsed > firstTask.EndTime)
            {
                tasks.RemoveAt(0);
            }
        }
    }

    /// <summary>
    /// Draw debug text to the screen
    /// </summary>
    /// <param name="drawing">DrawManager</param>
    public static void DrawDebug(DrawManager drawing)
    {
        drawing.DrawString($"TIME: {elapsed}", new Vector2(10, 0), font: Fonts.Arial, drawLayer: 3, isUi: true);
        drawing.DrawString($"NUM TASKS: {tasks.Count}", new Vector2(10, 30), font: Fonts.Arial, drawLayer: 3, isUi: true);

        for (int i = 0; i < tasks.Count; i++)
        {
            Task task = tasks[i];
            drawing.DrawString($"TASK | {task.StartTime} -> {task.EndTime}", new Vector2(10, 90 + i * 30), font: Fonts.Arial, drawLayer: 3, isUi: true);
        }
    }

    private float time;
    public List<Task> myTasks;

    /// <summary>
    /// Initialize this schedule
    /// </summary>
    public TaskSchedule()
    {
        time = elapsed;
        myTasks = new List<Task>();
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
        myTasks.Add(task);
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
    /// Add a function to run every frame for a given amount of time
    /// </summary>
    /// <param name="loopTime">Time in seconds</param>
    /// <param name="callback">Function to be called</param>
    /// <returns>this</returns>
    public TaskSchedule Loop(float loopTime, Task.GameTimeOnlyCallback callback)
    {
        AddTask(new Task(time, time + loopTime, callback));
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
        AddTask(new Task(time, time + loopTime, callback));
        time += loopTime;
        return this;
    }

    /// <summary>
    /// Cancel this schedule
    /// </summary>
    public void Cancel()
    {
        foreach (Task task in myTasks)
        {
            tasks.Remove(task);
        }
    }
}
