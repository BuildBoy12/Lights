// -----------------------------------------------------------------------
// <copyright file="ColorQueue.cs" company="Beryl">
// Copyright (c) Beryl. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Lights
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Features;
    using MEC;

    public class ColorQueue
    {
        private readonly Dictionary<Room, Queue<RoomAction>> actions = new Dictionary<Room, Queue<RoomAction>>();
        private readonly Dictionary<Room, CoroutineHandle> coroutines = new Dictionary<Room, CoroutineHandle>();

        public void Add(Room room, Action<Room> action, float duration, Action<Room> onCompletion)
        {
            if (!actions.ContainsKey(room))
                actions.Add(room, new Queue<RoomAction>());

            actions[room].Enqueue(new RoomAction(action, duration, onCompletion));
            if (!coroutines.TryGetValue(room, out CoroutineHandle coroutineHandle) || !coroutineHandle.IsRunning)
                coroutines[room] = Timing.RunCoroutine(RunDequeue(room));
        }

        public void Clear()
        {
            actions.Clear();
            foreach (CoroutineHandle coroutineHandle in coroutines.Values)
                Timing.KillCoroutines(coroutineHandle);

            coroutines.Clear();
        }

        private IEnumerator<float> RunDequeue(Room room)
        {
            Action<Room> onCompletion = null;
            while (actions.TryGetValue(room, out Queue<RoomAction> queue))
            {
                if (queue.Count == 0)
                {
                    onCompletion?.Invoke(room);
                    yield break;
                }

                RoomAction roomAction = queue.Dequeue();
                roomAction.Action(room);
                onCompletion = roomAction.CompletionAction;

                if (roomAction.Duration > 0)
                {
                    yield return Timing.WaitForSeconds(roomAction.Duration);
                    continue;
                }

                queue.Clear();
                yield break;
            }
        }

        private class RoomAction
        {
            public RoomAction(Action<Room> action, float duration, Action<Room> onCompletion)
            {
                Action = action;
                Duration = duration;
                CompletionAction = onCompletion;
            }

            public Action<Room> Action { get; }

            public float Duration { get; }

            public Action<Room> CompletionAction { get; }
        }
    }
}