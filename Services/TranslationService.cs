using Google.Cloud.Translate.V3;

namespace CarsLoader.Services;

public static class TranslationService
{
	public static async Task<IEnumerable<string>> TranslateToEnglish(IEnumerable<string> texts)
	{
		var client = await TranslationServiceClient.CreateAsync();
		var request = new TranslateTextRequest
		{
			Contents = { texts },
			TargetLanguageCode = "en-US",
			Parent = "projects/encar-432715"
		};
		
		var response = await client.TranslateTextAsync(request);
		var translations = response.Translations.Select(translation => translation.TranslatedText);

		return translations;
	}
}