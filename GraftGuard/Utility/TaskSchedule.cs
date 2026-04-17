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

struct ScheduledItem
{
    public float StartTime;
    public float EndTime;

    public delegate void NoArgsCallback();
    public delegate void GameTimeOnlyCallback(GameTime gameTime);
    public delegate void CombinedCallback(GameTime gameTime, float elapsedTime);

    private NoArgsCallback noArgsCallback;
    private GameTimeOnlyCallback gameTimeOnlyCallback;
    private CombinedCallback combinedCallback;

    public ScheduledItem(float startTime, float endTime, NoArgsCallback callback)
    {
        StartTime = startTime;
        EndTime = endTime;
        noArgsCallback = callback;
    }

    public ScheduledItem(float startTime, float endTime, GameTimeOnlyCallback callback)
    {
        StartTime = startTime;
        EndTime = endTime;
        gameTimeOnlyCallback = callback;
    }

    public ScheduledItem(float startTime, float endTime, CombinedCallback callback)
    {
        StartTime = startTime;
        EndTime = endTime;
        combinedCallback = callback;
    }

    public void RunCallback(GameTime gameTime, float elapsedTime)
    {
        Debug.WriteLine("Running this thing");

        if (noArgsCallback != null)
            noArgsCallback();

        if (gameTimeOnlyCallback != null)
            gameTimeOnlyCallback(gameTime);

        if (combinedCallback != null)
            combinedCallback(gameTime, elapsedTime);
    }
}

internal class TaskSchedule
{
    public static TaskSchedule GlobalTasks { get; private set; }
    private static float elapsed;
    private static List<ScheduledItem> tasks;

    /// <summary>
    /// Create the global task manager
    /// </summary>
    public static void CreateGlobalTaskSchedule()
    {
        tasks = new List<ScheduledItem>();
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
        
        ScheduledItem firstTask = tasks[0];

        if (firstTask.StartTime <= elapsed)
        {
            firstTask.RunCallback(gameTime, elapsed - firstTask.StartTime);
            if (firstTask.EndTime == -1 || elapsed > firstTask.EndTime)
            {
                tasks.RemoveAt(0);
            }
        }
    }

    public static void DrawDebug(DrawManager drawing)
    {
        drawing.DrawString($"TIME: {elapsed}", new Vector2(10, 0), font: Fonts.Arial, drawLayer: 3, isUi: true);
        drawing.DrawString($"NUM TASKS: {tasks.Count}", new Vector2(10, 30), font: Fonts.Arial, drawLayer: 3, isUi: true);

        for (int i = 0; i < tasks.Count; i++)
        {
            ScheduledItem task = tasks[i];
            drawing.DrawString($"TASK | {task.StartTime} -> {task.EndTime}", new Vector2(10, 90 + i * 30), font: Fonts.Arial, drawLayer: 3, isUi: true);
        }
    }

    private float time;
    public List<ScheduledItem> myTasks;

    public TaskSchedule()
    {
        time = elapsed;
        myTasks = new List<ScheduledItem>();
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

    private void AddTask(ScheduledItem task)
    {
        tasks.Add(task);
        myTasks.Add(task);
    }

    /// <summary>
    /// Add a function to run at the current scheduled time
    /// </summary>
    /// <param name="callback">Function to be called</param>
    /// <returns>this</returns>
    public TaskSchedule Run(ScheduledItem.NoArgsCallback callback)
    {
        AddTask(new ScheduledItem(time, -1, callback));
        return this;
    }

    /// <summary>
    /// Add a function to run every frame for a given amount of time
    /// </summary>
    /// <param name="loopTime">Time in seconds</param>
    /// <param name="callback">Function to be called</param>
    /// <returns>this</returns>
    public TaskSchedule Loop(float loopTime, ScheduledItem.GameTimeOnlyCallback callback)
    {
        AddTask(new ScheduledItem(time, time + loopTime, callback));
        time += loopTime;
        return this;
    }

    /// <summary>
    /// Add a function to run every frame for a given amount of time
    /// </summary>
    /// <param name="loopTime">Time in seconds</param>
    /// <param name="callback">Function to be called</param>
    /// <returns>this</returns>
    public TaskSchedule Loop(float loopTime, ScheduledItem.CombinedCallback callback)
    {
        AddTask(new ScheduledItem(time, time + loopTime, callback));
        time += loopTime;
        return this;
    }

    /// <summary>
    /// Cancel this schedule.
    /// </summary>
    public void Cancel()
    {
        foreach (ScheduledItem task in myTasks)
        {
            tasks.Remove(task);
        }
    }
}
