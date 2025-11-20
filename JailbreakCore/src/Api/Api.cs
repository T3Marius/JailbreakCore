using System;
using Jailbreak.Shared;
using JailbreakCore;
using SwiftlyS2.Shared.Misc;
using SwiftlyS2.Shared.Natives;
using SwiftlyS2.Shared.Players;
using CorePlugin = JailbreakCore.JailbreakCore;

namespace Jailbreak;

public class Api : IJailbreakApi
{
    public ILastRequestService LastRequest { get; } = new LastRequestService();
    public ISpecialDayService SpecialDay { get; } = new SpecialDayService();
    public IPlayerService Players { get; } = new PlayerService();
    public IHookService Hooks { get; } = new HookService();
    public IUtilityService Utilities { get; } = new UtilityService();

    private sealed class LastRequestService : ILastRequestService
    {
        public void Register(ILastRequest request) => CorePlugin.LastRequest.Register(request);
        public ILastRequest? GetActive() => CorePlugin.LastRequest.GetActiveRequest();
        public IReadOnlyList<ILastRequest> GetAll() => CorePlugin.LastRequest.GetRequests();
        public void EndActive() => CorePlugin.LastRequest.EndRequest();
    }

    private sealed class SpecialDayService : ISpecialDayService
    {
        public void Register(ISpecialDay day) => CorePlugin.SpecialDay.Register(day);
        public void Unregister(ISpecialDay day) => CorePlugin.SpecialDay.Unregister(day);
        public ISpecialDay? GetActive() => CorePlugin.SpecialDay.GetActiveDay();
        public IReadOnlyList<ISpecialDay> GetAll() => CorePlugin.SpecialDay.GetAllDays();
        public void EndActive() => CorePlugin.SpecialDay.EndDay();
    }

    private sealed class PlayerService : IPlayerService
    {
        public IJBPlayer? GetPlayer(IPlayer controller) => CorePlugin.JBPlayerManagement.GetOrCreate(controller);
        public IJBPlayer? GetWarden() => CorePlugin.JBPlayerManagement.GetWarden();
    }

    private sealed class HookService : IHookService
    {
        public IDisposable HookTakeDamage(Func<DamageHookContext, HookResult> callback)
        {
            ArgumentNullException.ThrowIfNull(callback);

            if (CorePlugin.Hooks == null)
                throw new InvalidOperationException("TakeDamage hook is not available yet.");

            return CorePlugin.Hooks.SubscribeTakeDamage(callback);
        }
    }

    private sealed class UtilityService : IUtilityService
    {
        public CancellationTokenSource StartTimer(int seconds, Action<int> onTick, Action onFinished)
        {
            return CorePlugin.Extensions.StartTimer(seconds, onTick, onFinished);
        }
        public void ToggleCells(bool state, string callerName = "")
        {
            CorePlugin.Extensions.ToggleCells(state, callerName);
        }
        public void PrintToChatAll(string key, bool showPrefix = true, IPrefix prefixType = IPrefix.JB, params object[] args)
        {
            CorePlugin.Extensions.PrintToChatAll(key, showPrefix, prefixType, args);
        }
        public void PrintToChatAll(string message, bool showPrefix, IPrefix prefixType)
        {
            CorePlugin.Extensions.PrintToChatAll(message, showPrefix, prefixType);
        }
        public void PrintToAlertAll(string key, params object[] args)
        {
            CorePlugin.Extensions.PrintToAlertAll(key, args);
        }
        public void PrintToCenterAll(string key, params object[] args)
        {
            CorePlugin.Extensions.PrintToCenterAll(key, args);
        }

        public Guid DrawLaserBetweenPlayers(IJBPlayer playerA, IJBPlayer playerB, Color? colorOverride = null, float width = 2.0f, float heightOffset = 64.0f, float durationSeconds = 0f)
        {
            var internalA = RequireInternalPlayer(playerA);
            var internalB = RequireInternalPlayer(playerB);

            return CorePlugin.Extensions.StartPlayerLinkLaser(internalA, internalB, colorOverride, width, heightOffset, durationSeconds);
        }

        public void RemovePlayerLinkLaser(Guid effectId)
        {
            CorePlugin.Extensions.StopPlayerLinkLaser(effectId);
        }

        public void RemoveAllPlayerLinkLasers()
        {
            CorePlugin.Extensions.StopAllPlayerLinkLasers();
        }

        public Guid CreatePlayerBeacon(IJBPlayer player, Color? colorOverride = null, int segments = 20, float startRadius = 20.0f, float radiusStep = 10.0f, float durationSeconds = 0.9f, float heightOffset = 5.0f, float stepIntervalSeconds = 0.1f, float width = 2.0f, bool loop = false)
        {
            var internalPlayer = RequireInternalPlayer(player);

            return CorePlugin.Extensions.CreateBeaconAnimationOnPlayer(internalPlayer, colorOverride, segments, startRadius, radiusStep, durationSeconds, heightOffset, stepIntervalSeconds, width, loop);
        }

        public void RemovePlayerBeacon(Guid beaconId)
        {
            CorePlugin.Extensions.StopPlayerBeacon(beaconId);
        }

        public void RemoveAllPlayerBeacons()
        {
            CorePlugin.Extensions.StopAllPlayerBeacons();
        }

        private static JBPlayer RequireInternalPlayer(IJBPlayer player)
        {
            if (player is not JBPlayer jbPlayer)
                throw new ArgumentException("The provided player reference is not managed by JailbreakCore.", nameof(player));

            return jbPlayer;
        }
    }
}
