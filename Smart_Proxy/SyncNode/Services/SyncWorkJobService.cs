using Common.Models;
using Common.Utilities;
using SyncNode.Settings;
using System.Collections.Concurrent;

namespace SyncNode.Services
{
    public class SyncWorkJobService : IHostedService
    {
        private readonly ConcurrentDictionary<Guid, SyncEntity> documents = 
            new ConcurrentDictionary<Guid, SyncEntity>();
        private readonly IMovieAPISettings _settings;
        private Timer _timer;
        public SyncWorkJobService(IMovieAPISettings settings)
        {
            _settings = settings;
        }
        public void AddItem(SyncEntity entity)
        {
            SyncEntity document = null;
            bool isPresent = documents.TryGetValue(entity.Id, out document);
            if(!isPresent || (isPresent && entity.LastChangedAt > document.LastChangedAt))
            {
                documents[entity.Id] = entity;
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoSendWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
        private void DoSendWork(object state)
        {
            foreach(var doc in documents)
            {
                SyncEntity entity;
                var isPresent = documents.TryRemove(doc.Key, out entity);
                if(isPresent)
                {
                    var receivers = _settings.Hosts.Where(x => !x.Contains(entity.Origin));
                    foreach(var receiver in receivers)
                    { 
                        var url = $"{receiver}/{entity.ObjectType}/sync";
                        try
                        {
                            var result = HttpClientUtility.SendJson(entity.JsonData, url, entity.SyncType);
                            if(!result.IsSuccessStatusCode)
                            {
                                // log error
                            }
                        }
                        catch(Exception e)
                        {
                            //e
                        }
                    }
                }
            }
        }
    }
}
