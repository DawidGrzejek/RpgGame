using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Domain.Events.Base
{
    public class StoredEvent
    {
        public Guid Id { get; set; }
        public Guid AggregateId { get; set; }
        public string AggregateType { get; set; }
        public int Version { get; set; }
        public string EventType { get; set; }
        public string EventData { get; set; }
        public DateTime Timestamp { get; set; }
        public string UserId { get; set; } // Optional: track which user/player caused the event
    }
}
