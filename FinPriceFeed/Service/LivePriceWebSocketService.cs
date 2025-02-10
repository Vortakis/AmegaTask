using FinPriceFeed.Domain.Enum;
using FinPriceFeed.ExternalProviders;
using FinPriceFeed.ExternalProviders.TwelveData.Model;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace FinPriceFeed.Service
{
    public class LivePriceWebSocketService : ILivePriceWebSocketService
    {
        private readonly IExternalProviderWebSocketService _extWebSocketService;
        private readonly IHubContext<LivePricesHub> _hubContext;

        private static readonly ConcurrentDictionary<string, HashSet<string>> _activeSymbols = new();
        private static readonly ConcurrentDictionary<string, bool> _subscribers = new();

        public LivePriceWebSocketService(
            IExternalProviderWebSocketService extWebSocketService, 
            IHubContext<LivePricesHub> hubContext)
        {
            _hubContext = hubContext;
            _extWebSocketService = extWebSocketService;
            extWebSocketService.OnPriceUpdate += BroadcastPriceUpdate;
            extWebSocketService.OnSubscriptionUpdate += BroadcastSubscriptionFailed;
        }

        public void AddSubscriber(string connectionId) => _subscribers.TryAdd(connectionId, true);

        public void RemoveSubscriber(string connectionId)
        {
            _subscribers.TryRemove(connectionId, out _);
            UnsubscribeFromAllSymbols(connectionId);
        }

        public async Task SubscribeToSymbols(string connectionId, string symbolString)
        {
            if (!_subscribers.ContainsKey(connectionId)) return;

            symbolString = Uri.UnescapeDataString(symbolString).ToUpper();

            var symbols = symbolString.Split(",", StringSplitOptions.TrimEntries);
            var newSymbols = new List<string>();

            foreach (var symbol in symbols)
            {
                var symbolSubs = _activeSymbols.GetOrAdd(symbol, _ => new HashSet<string>());
                lock (symbolSubs)
                {
                    if (symbolSubs.Add(connectionId) && symbolSubs.Count == 1)
                    {
                        newSymbols.Add(symbol);
                    }
                }
            }

            if (newSymbols.Any())
            {
                await _extWebSocketService.SubscribeToLivePrices(string.Join(",", newSymbols));
            }
        }

        public async Task UnsubscribeFromSymbols(string connectionId, string symbolString)
        {
            if (!_subscribers.ContainsKey(connectionId)) return;

            symbolString = Uri.UnescapeDataString(symbolString).ToUpper();

            var symbols = symbolString.Split(",", StringSplitOptions.TrimEntries);
            var toRemoveSymbols = new List<string>();

            foreach (var symbol in symbols)
            {
                if (_activeSymbols.TryGetValue(symbol, out var symbolSubs))
                {
                    lock (symbolSubs)
                    {
                        symbolSubs.Remove(connectionId);
                        if (symbolSubs.Count == 0)
                        {
                            _activeSymbols.TryRemove(symbol, out _);
                            toRemoveSymbols.Add(symbol);
                        }
                    }
                }
            }

            if (toRemoveSymbols.Any())
            {
                await _extWebSocketService.UnsubscribeFromLivePrices(string.Join(",", toRemoveSymbols));
            }
        }

        public async Task BroadcastPriceUpdate(string symbol, decimal price)
        {
            symbol = symbol.ToUpper();

            if (_activeSymbols.TryGetValue(symbol, out var subscribers))
            {
                var tasks = subscribers.Select(subscriber => _hubContext.Clients.Client(subscriber).SendAsync("ReceivePriceUpdate", symbol, price));
                await Task.WhenAll(tasks);
            }
        }

        public async Task BroadcastSubscriptionFailed(SubscriptionType type, string[] failedSymbols)
        {
            var subscribersSymbolsToNotify = new Dictionary<string, HashSet<string>>();

            foreach (var failedSymbol in failedSymbols)
            {
                var upperSymbol = failedSymbol.ToUpper();

                if (_activeSymbols.TryGetValue(upperSymbol, out var subscribers))
                {
                    foreach (var subscriber in subscribers)
                    {
                        if (!subscribersSymbolsToNotify.ContainsKey(subscriber))
                            subscribersSymbolsToNotify[subscriber] = new HashSet<string>();

                        subscribersSymbolsToNotify[subscriber].Add(upperSymbol);
                    }
                }
            }

            var notificationTasks = subscribersSymbolsToNotify.Select(subscriberSymbols =>
                _hubContext.Clients.Client(subscriberSymbols.Key)
                    .SendAsync("SubscriptionFailed", type.ToString(), string.Join(",", subscriberSymbols.Value))
            );

            await Task.WhenAll(notificationTasks);
        }

        private void UnsubscribeFromAllSymbols(string connectionId)
        {
            foreach (var symbol in _activeSymbols.Keys)
            {
                if (_activeSymbols.TryGetValue(symbol, out var symbolSubs))
                {
                    lock (symbolSubs)
                    {
                        symbolSubs.Remove(connectionId);
                        if (!symbolSubs.Any())
                        {
                            _activeSymbols.TryRemove(symbol, out _);
                            _extWebSocketService.UnsubscribeFromLivePrices(symbol);
                        }
                    }
                }
            }
        }
    }
}
