using Azure;
using Azure.AI.Translation.Document;

namespace BatchDocumentTranslation;

/**
 * This application demonstrates how to use the Azure Document Translation service to translate documents in a batch.
 * It initializes the DocumentTranslationClient object with the provided endpoint and resource key.
 * The source and target URIs are specified along with the target language code.
 * The application starts the translation operation and waits for it to complete.
 * It then retrieves and displays the status and details of the translation operation, including the translated documents.
 * If any errors occur during the translation process, they are also displayed.
 */
public class Program
{
    // Create variables for your custom endpoint and resource key.
    private static readonly string Endpoint = "<azure-ai-translator-service-endpoint>";
    private static readonly string Key = "<azure-ai-translator-service-key>";

    public static async Task Main(string[] args)
    {
        try
        {
            // Create variables for your sourceUrl, targetUrl, and targetLanguageCode
            Uri sourceUri = new("<source-container-sas-uri>");
            Uri targetUri = new("<target-container-sas-uri>");
            const string targetLanguage = "it";

            // Initialize a new instance  of the DocumentTranslationClient object to interact with the Document Translation feature.
            DocumentTranslationClient client = new(new Uri(Endpoint), new AzureKeyCredential(Key));

            // Initialize a new instance of the `DocumentTranslationInput` object to provide the location of input for the translation operation.
            DocumentTranslationInput input = new(sourceUri, targetUri, targetLanguage);

            // Initialize a new instance of the DocumentTranslationOperation class to track the status of the translation operation.
            DocumentTranslationOperation operation = await client.StartTranslationAsync(input);

            await operation.WaitForCompletionAsync();

            Console.WriteLine($"Status: {operation.Status}");
            Console.WriteLine($"Created on: {operation.CreatedOn}");
            Console.WriteLine($"Last modified: {operation.LastModified}");
            Console.WriteLine($"Total documents: {operation.DocumentsTotal}");
            Console.WriteLine($"Succeeded: {operation.DocumentsSucceeded}");
            Console.WriteLine($"Failed: {operation.DocumentsFailed}");
            Console.WriteLine($"In Progress: {operation.DocumentsInProgress}");
            Console.WriteLine($"Not started: {operation.DocumentsNotStarted}");

            await foreach (var document in operation.Value)
            {
                Console.WriteLine($"Document with Id: {document.Id}");
                Console.WriteLine($"Status:{document.Status}");
                if (document.Status == DocumentTranslationStatus.Succeeded)
                {
                    Console.WriteLine($"Translated Document Uri: {document.TranslatedDocumentUri}");
                    Console.WriteLine($"Translated to language: {document.TranslatedToLanguageCode}.");
                    Console.WriteLine($"Document source Uri: {document.SourceDocumentUri}");
                }
                else
                {
                    Console.WriteLine($"Error Code: {document.Error.Code}");
                    Console.WriteLine($"Message: {document.Error.Message}");
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}