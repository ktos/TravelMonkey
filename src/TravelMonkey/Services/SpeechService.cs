using Microsoft.CognitiveServices.Speech;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelMonkey.Services
{
    /// <summary>
    /// Provides support for Text-To-Speech using Azure Cognitive Services
    /// </summary>
    internal static class SpeechService
    {
        // list from https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/language-support
        private readonly static List<string> AvailableTtsLanguages = new List<string> { "ar-AE", "ca-ES", "da-DK", "de-DE", "en-US",
            "es-ES", "fr-FR", "hi-IN", "it-IT", "ja-JP", "ko-KR", "mr-IN", "nb-NO", "nl-NL", "pl-PL", "pt-BR", "pt-PT", "ru-RU",
            "sv-SE", "ta-IN", "te-IN", "th-TH", "tr-TR", "zh-CN" };

        /// <summary>
        /// Maps between translation language code and available TTS languages in the service
        /// </summary>
        /// <param name="language">Short language code</param>
        /// <returns>Full culture descriptor</returns>
        private static string MapLanguageToSpeech(string language)
        {
            return AvailableTtsLanguages.FirstOrDefault(x => x.StartsWith(language));
        }

        /// <summary>
        /// Speaks aloud provided text using the provided locale
        /// </summary>
        /// <param name="text">Text to be spoken</param>
        /// <param name="language">Locale code, can be short or long, if not available, en-US will be used by default</param>
        /// <returns></returns>
        public async static Task SpeakAsync(string text, string language)
        {
            var configuration = SpeechConfig.FromSubscription(ApiKeys.SpeechApiKey, ApiKeys.SpeechApiRegion);
            configuration.SpeechSynthesisLanguage = MapLanguageToSpeech(language) ?? "en-US";

            var synthethizer = new SpeechSynthesizer(configuration);

            var result = await synthethizer.SpeakTextAsync(text);

            var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
            Debug.WriteLine($"CANCELED: Reason={cancellation.Reason}");

            if (cancellation.Reason == CancellationReason.Error)
            {
                Debug.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                Debug.WriteLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                Debug.WriteLine($"CANCELED: Did you update the subscription info?");
            }
        }
    }
}