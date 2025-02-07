using Microsoft.AspNetCore.Mvc;
using Microsoft.CognitiveServices.Speech;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.CognitiveServices.Speech.Audio;

[Route("api/audio")]
[ApiController]
public class AudioController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AudioController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadAudio([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        var apiKey = _configuration["AzureSpeech:ApiKey"];
        var region = _configuration["AzureSpeech:Region"];

        // Save file to memory stream
        using (var memoryStream = new MemoryStream())
        {
            await file.CopyToAsync(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Process the audio with Azure Speech-to-Text
            var transcription = await ProcessAudioWithAzureSpeech(memoryStream, apiKey, region);
            return Ok(transcription);
        }
    }

    private async Task<string> ProcessAudioWithAzureSpeech(MemoryStream audioStream, string apiKey, string region)
    {
        var speechConfig = SpeechConfig.FromSubscription(apiKey, region);

        // Create PushAudioInputStream for the memory stream
        var pushStream = AudioInputStream.CreatePushStream();

        // Copy the memory stream content to the pushStream
        using (var streamReader = new BinaryReader(audioStream))
        {
            byte[] buffer;
            while ((buffer = streamReader.ReadBytes(4096)).Length > 0)
            {
                pushStream.Write(buffer);
            }
        }

        // Set up audioConfig to use the push stream
        var audioConfig = AudioConfig.FromStreamInput(pushStream);

        using var recognizer = new SpeechRecognizer(speechConfig, audioConfig);

        // Perform speech recognition
        var result = await recognizer.RecognizeOnceAsync();

        if (result.Reason == ResultReason.RecognizedSpeech)
        {
            return result.Text; // Return recognized text
        }
        else
        {
            return $"Recognition failed: {result.Reason}";
        }
    }
}
