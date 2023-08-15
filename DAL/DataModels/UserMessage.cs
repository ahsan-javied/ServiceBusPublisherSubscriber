using System.Text.Json;

namespace DAL.DataModels
{
    public class UserMessage
    {
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsDelivered { get; set; } = false;
        public DateTime? DeliveredDT { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime? ReadDT { get; set; }
        public bool PickAndLock { get; set; } = false;
        public DateTime? PickAndLockDT { get; set; }
        public string? GroupSubscriptionName { get; set; }


        public string ToJsonString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
