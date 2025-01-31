using DemoKCIC.Server.Repositories.FindingTicket;
using DemoKCIC.Server.Services.FindingTicket;
using System;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DemoKCIC.Server.Services.FindingTicket
{
    public class FindingTicketService : IFindingTicketService
    {
        private readonly IFindingTicketRepository _ftRepository;

        public FindingTicketService(IFindingTicketRepository ftRepository)
        {
            _ftRepository = ftRepository;
        }

        public async Task<string> StartRecordingAsync(string languageCode)
        {
            try
            {
                return await _ftRepository.StartRecordingAsync(languageCode);

            }
            catch (Exception ex)
            {
                Console.WriteLine("");
                Console.WriteLine("[Services] Error requesting FindingTicket: " + ex.Message);
                throw new ApplicationException("[Services] Error requesting FindingTicket: " + ex.Message);
            }
        }

        public async Task<string> ExtractPromptAsync(string prompt)
        {
            try
            {
                return await _ftRepository.ExtractPromptAsync(prompt);
            }
            catch (Exception ex)
            {
                Console.WriteLine("");
                Console.WriteLine("[Services] Error requesting FindingTicket: " + ex.Message);
                throw new ApplicationException("[Services] Error requesting FindingTicket: " + ex.Message);
            }
        }

        public async Task<string> GetTicket(string from, string to, string date)
        {
            try
            {
                return await _ftRepository.GetTicket(from, to, date);
            }
            catch (Exception ex)
            {
                Console.WriteLine("");
                Console.WriteLine("[Services] Error requesting FindingTicket: " + ex.Message);
                throw new ApplicationException("[Services] Error requesting FindingTicket: " + ex.Message);
            }
        }
        
    }
}
