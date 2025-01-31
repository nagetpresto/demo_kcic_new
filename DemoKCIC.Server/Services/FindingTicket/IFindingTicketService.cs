namespace DemoKCIC.Server.Services.FindingTicket
{
    public interface IFindingTicketService
    {
        Task<string> StartRecordingAsync(string languageCode);

        Task<string> ExtractPromptAsync(string prompt);

        Task<string> GetTicket(string from, string to, string date);
    }
}
