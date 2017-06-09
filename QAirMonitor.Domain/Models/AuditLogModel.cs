using QAirMonitor.Domain.Enums;
using System;

namespace QAirMonitor.Domain.Models
{
    public class AuditLogModel
    {
        public int AuditLogID { get; set; }
        public DateTime EventDateTime { get; set; }
        public string Message { get; set; }
        public AuditLogEventType EventType { get; set; }
    }
}
