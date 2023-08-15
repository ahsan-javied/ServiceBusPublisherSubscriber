using System.Text.Json;

namespace DAL.DataModels
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int UserRoleTypeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public bool IsDelivered { get; set; } = false;
        public DateTime? DeliveredDT { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime? ReadDT { get; set; }
        public bool PickAndLock { get; set; } = false;
        public DateTime? PickAndLockDT { get; set; }

        public string ToJsonString()
        {
            return JsonSerializer.Serialize(this);
        }

        public Notification ToClassObj(string jsonData)
        {
            return JsonSerializer.Deserialize<Notification>(jsonData) ?? new Notification();
        }
    }
}
