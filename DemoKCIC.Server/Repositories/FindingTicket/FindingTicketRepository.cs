using DemoKCIC.Server.Repositories.FindingTicket;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class FindingTicketRepository : IFindingTicketRepository
{
    private readonly IConfiguration _configuration;
    private SpeechRecognizer _speechRecognizer;
    private CancellationTokenSource _cancellationTokenSource;

    public FindingTicketRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<string> StartRecordingAsync(string languageCode)
    {
        try
        {
            var apiKey = _configuration["AzureSpeech:ApiKey"];
            var region = _configuration["AzureSpeech:Region"];

            var speechConfig = SpeechConfig.FromSubscription(apiKey, region);
            speechConfig.SpeechRecognitionLanguage = languageCode;

            using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
            _speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);

            var speechRecognitionResult = await _speechRecognizer.RecognizeOnceAsync();

            return speechRecognitionResult.Text;
            //OutputSpeechRecognitionResult(speechRecognitionResult);

        }
        catch (Exception ex)
        {
            Console.WriteLine("");
            Console.WriteLine("[Repository] Finding Ticket start record: " + ex.Message);
            throw new ApplicationException("[Repository] Finding Ticket start record: " + ex.Message);
        }
    }

    public async Task<string> ExtractPromptAsync(string prompt)
    {
        try
        {
            var apiKey = _configuration["OpenAI:ApiKey"];
            var endpoint = _configuration["OpenAI:EndpointGPT4"];

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("api-key", apiKey);

                var requestBody = new
                {
                    model = "gpt-4",
                    messages = new[]
                    {
                        new { role = "user", content = prompt }
                    },
                    temperature = 0,
                    max_tokens = 100
                };

                var jsonContent = JsonConvert.SerializeObject(requestBody);
                //Console.WriteLine(jsonContent);

                var response = await httpClient.PostAsync(
                    endpoint,
                    new StringContent(jsonContent, Encoding.UTF8, "application/json")
                );

                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();

                var responseObject = JsonConvert.DeserializeObject<dynamic>(result);

                string messageContent = responseObject.choices[0].message.content;
                int totalTokens = responseObject.usage.total_tokens;
                int promptTokens = responseObject.usage.prompt_tokens;
                int completionTokens = responseObject.usage.completion_tokens;

                return messageContent;
            }

        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine("");
            Console.WriteLine($"[Repository] HTTP Request error: {ex.Message}");
            throw new ApplicationException($"[Repository] HTTP Request error: {ex.Message}");
        }
        catch (JsonException ex)
        {
            Console.WriteLine("");
            Console.WriteLine($"[Repository] JSON parsing error: {ex.Message}");
            throw new ApplicationException($"[Repository] JSON parsing error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("");
            Console.WriteLine("[Repository] Finding Ticket GPT error." + ex.Message);
            throw new ApplicationException("[Repository] Finding Ticket GPT error." + ex.Message);
        }
    }

    public async Task<string> GetTicket(string from, string to, string date)
    {
        try
        {
            var endpoint = _configuration["KCIC:Endpoint"];

            var queryParams = new Dictionary<string, string>
            {
                { "trainDate", date },
                { "fromStationTelecode", from },
                { "toStationTelecode", to },
                { "sortord", "1" },
                { "ticketFlag", "0" },
                { "startTimeInterval", "00:00-24:00" }
            };

            var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            var urlWithParams = $"{endpoint}?{queryString}";
            //Console.WriteLine(urlWithParams);

            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Accept", "application/json, text/plain, */*"),
                new KeyValuePair<string, string>("Accept-Language", "en-US,en;q=0.9"),
                new KeyValuePair<string, string>("Connection", "keep-alive"),
                new KeyValuePair<string, string>("Referer", "https://ticket.kcic.co.id/webTrade/"),
                new KeyValuePair<string, string>("Sec-Fetch-Dest", "empty"),
                new KeyValuePair<string, string>("Sec-Fetch-Mode", "cors"),
                new KeyValuePair<string, string>("Sec-Fetch-Site", "same-origin"),
                new KeyValuePair<string, string>("User-Agent", "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Mobile Safari/537.36"),
                new KeyValuePair<string, string>("appVersion", "1.2.002"),
                new KeyValuePair<string, string>("appVersionCode", "35"),
                new KeyValuePair<string, string>("deviceid", "00000000"),
                new KeyValuePair<string, string>("languageCode", "en_US"),
                new KeyValuePair<string, string>("platform", "web"),
                new KeyValuePair<string, string>("saleMode", "E"),
                new KeyValuePair<string, string>("sec-ch-ua", "\"Google Chrome\";v=\"131\", \"Chromium\";v=\"131\", \"Not_A Brand\";v=\"24\""),
                new KeyValuePair<string, string>("sec-ch-ua-mobile", "?1"),
                new KeyValuePair<string, string>("sec-ch-ua-platform", "\"Android\""),
                new KeyValuePair<string, string>("versionNo", "v1.0")
            };

            using (var httpClient = new HttpClient())
            {
                foreach (var header in headers)
                {
                    httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
                
                var response = await httpClient.GetAsync(urlWithParams);

                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();

                var responseObject = JsonConvert.DeserializeObject<dynamic>(result);
                //Console.WriteLine(responseObject.ToString());

                
                string dataJson = JsonConvert.SerializeObject(responseObject.data);

                return dataJson;
            }

        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine("");
            Console.WriteLine($"[Repository] HTTP Request error: {ex.Message}");
            throw new ApplicationException($"[Repository] HTTP Request error: {ex.Message}");
        }
        catch (JsonException ex)
        {
            Console.WriteLine("");
            Console.WriteLine($"[Repository] JSON parsing error: {ex.Message}");
            throw new ApplicationException($"[Repository] JSON parsing error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("");
            Console.WriteLine("[Repository] Finding Ticket GPT error." + ex.Message);
            throw new ApplicationException("[Repository] Finding Ticket GPT error." + ex.Message);
        }
    }
}
