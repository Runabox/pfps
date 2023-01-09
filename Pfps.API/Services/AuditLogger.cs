using Pfps.API.Data;

namespace Pfps.API.Services
{
    public interface IAuditLogger
    {
        Task LogEventAsync(string message, AuditEvent type, User user);
    }

    public class AuditLogger : IAuditLogger
    {
        private readonly PfpsContext _ctx;

        public AuditLogger(PfpsContext ctx) => _ctx = ctx;

        public async Task LogEventAsync(string message, AuditEvent type, User user)
        {
            var audit = new Audit()
            {
                Type = type,
                Instigator = user,
                Message = message,
            };

            await _ctx.Audits.AddAsync(audit);
        }
    }
}