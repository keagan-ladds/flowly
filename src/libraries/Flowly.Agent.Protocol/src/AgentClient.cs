using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;

namespace Flowly.Agent.Protocol
{
    public class AgentClient : IDisposable
    {
        private readonly AgentService.AgentServiceClient _agentServiceClient;
        private readonly GrpcChannel? _channel;

        public AgentClient(string address)
        {
            _channel = GrpcChannel.ForAddress(address);
            _agentServiceClient = new AgentService.AgentServiceClient(_channel);
        }

        protected AgentClient(GrpcChannel channel)
        {
            _agentServiceClient = new AgentService.AgentServiceClient(channel);
        }

        public async Task SubscribeToAgentEvents(Action<AgentEvent> eventAction)
        {
            var stream = _agentServiceClient.ServerStreamServerEvents(new AgentEventRequest()).ResponseStream;
            await foreach(var @event in stream.ReadAllAsync())
            {
                eventAction(@event);
            }
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
}
