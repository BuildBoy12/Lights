// -----------------------------------------------------------------------
// <copyright file="Plugin.cs" company="Beryl">
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
    using PlayerHandlers = Exiled.Events.Handlers.Player;
    using ServerHandlers = Exiled.Events.Handlers.Server;
    using WarheadHandlers = Exiled.Events.Handlers.Warhead;

    /// <summary>
    /// The main plugin class.
    /// </summary>
    public class Plugin : Plugin<Config>
    {
        /// <summary>
        /// Gets an instance of the <see cref="Plugin"/> class.
        /// </summary>
        public static Plugin Instance { get; private set; }

        /// <summary>
        /// Gets an instance of the <see cref="Lights.EventHandlers"/> class.
        /// </summary>
        public static EventHandlers EventHandlers { get; private set; }

        /// <summary>
        /// Gets all coroutines used by this plugin.
        /// </summary>
        public static List<CoroutineHandle> Coroutines { get; } = new List<CoroutineHandle>();

        /// <summary>
        /// Gets the color queue class instance.
        /// </summary>
        public ColorQueue ColorQueue { get; private set; }

        /// <inheritdoc />
        public override string Name => "LightsRE";

        /// <inheritdoc />
        public override string Author => "Beryl - (Contributors: BuildBoy12)";

        /// <inheritdoc />
        public override string Prefix => "lights";

        /// <inheritdoc />
        public override Version Version { get; } = new Version(5, 0, 0);

        /// <inheritdoc />
        public override Version RequiredExiledVersion { get; } = new Version(5, 1, 2);

        /// <inheritdoc />
        public override void OnEnabled()
        {
            Instance = this;
            ColorQueue = new ColorQueue();
            EventHandlers = new EventHandlers(this);
            ServerHandlers.RoundStarted += EventHandlers.OnRoundStarted;
            ServerHandlers.RoundEnded += EventHandlers.OnRoundEnded;
            PlayerHandlers.TriggeringTesla += EventHandlers.OnTriggeringTesla;
            WarheadHandlers.Stopping += EventHandlers.OnStopping;

            base.OnEnabled();
        }

        /// <inheritdoc />
        public override void OnDisabled()
        {
            ServerHandlers.RoundStarted -= EventHandlers.OnRoundStarted;
            ServerHandlers.RoundEnded -= EventHandlers.OnRoundEnded;
            PlayerHandlers.TriggeringTesla -= EventHandlers.OnTriggeringTesla;
            WarheadHandlers.Stopping -= EventHandlers.OnStopping;
            EventHandlers = null;
            ColorQueue?.Clear();
            ColorQueue = null;
            Instance = null;

            base.OnDisabled();
        }
    }
}
