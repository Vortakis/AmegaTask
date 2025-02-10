using FinPriceFeed.Service;
using Microsoft.AspNetCore.SignalR;

namespace FinPriceFeed.ExternalProviders
{
    public class LivePricesHub : Hub
    {
        private readonly ILivePriceWebSocketService _livePriceWSService;

        public LivePricesHub(ILivePriceWebSocketService livePriceWSService)
        {
            _livePriceWSService = livePriceWSService;
        }

        public override async Task OnConnectedAsync()
        {
            _livePriceWSService.AddSubscriber(Context.ConnectionId);
            await Clients.Caller.SendAsync("Connected", $"Caller with ConnectionId: '{Context.ConnectionId}' Connected!");
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await _livePriceWSService.RemoveSubscriber(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public Task SubscribeToSymbols(string symbolString) =>
            _livePriceWSService.SubscribeToSymbols(Context.ConnectionId, symbolString);

        public Task UnsubscribeFromSymbols(string symbolString) =>
            _livePriceWSService.UnsubscribeFromSymbols(Context.ConnectionId, symbolString);

    }
}
