import * as SpeechSDK from 'microsoft-cognitiveservices-speech-sdk';
console.log('Hello, Webpack and Blazor!');

window.startRecording = async function (languageCode) {
    try {
        var stream = await navigator.mediaDevices.getUserMedia({ audio: true });

        var speechConfig = SpeechSDK.SpeechConfig.fromSubscription('D5MtE2Hof5pP4uQCOlC9whsW3T3nf4joLrSjYAVZqMLmrihlr42hJQQJ99ALACqBBLyXJ3w3AAAYACOGQH1b', 'southeastasia');
        speechConfig.speechRecognitionLanguage = languageCode;

        var audioConfig = SpeechSDK.AudioConfig.fromDefaultMicrophoneInput(); 
        var recognizer = new SpeechSDK.SpeechRecognizer(speechConfig, audioConfig);

        var result = await new Promise((resolve, reject) => {
            recognizer.recognizeOnceAsync(
                (result) => {
                    if (result.reason === SpeechSDK.ResultReason.RecognizedSpeech) {
                        resolve(result.text);
                    } else {
                        reject(`Recognition failed: ${result.errorDetails}`);
                    }
                },
                (error) => {
                    reject(`Error during recognition: ${error.details}`);
                }
            );
        });

        return result;

    } catch (err) {
        console.error("Error accessing the microphone: ", err);
        return "Error accessing microphone.";
    }
};
