
namespace DemoKCIC.Server.Repositories.FindingTicket
{
    public interface IFindingTicketRepository
    {
        Task<string> StartRecordingAsync(string languageCode);
        Task<string> ExtractPromptAsync(string prompt);
        Task<string> GetTicket(string from, string to, string date);
    }

}
