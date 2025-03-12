using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CatApiTests
{
    [TestFixture]
    public class CatFactTest
    {
        private HttpClient? _httpClient;
        private static readonly string LogDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Logs");
        private static readonly string LogFilePath = Path.Combine(LogDirectory, "ApiTestLog.txt");

        [OneTimeSetUp]
        public void Setup()
        {
            _httpClient = new HttpClient();
            Directory.CreateDirectory(LogDirectory);
            File.WriteAllText(LogFilePath, $"Test Execution Started: {DateTime.Now}\n\n");
        }

        [Test]
        public async Task Test_GetCatFact()
        {
            var response = await _httpClient!.GetAsync("https://catfact.ninja/fact?max_length=100");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            LogToFile($"API Response: {content}\n");
        
            Console.WriteLine(content);
            Assert.That(content, Is.Not.Null);
            Assert.That(content.Contains("cat"), Is.True);
        }

        [Test]
        public async Task Test_CatFactLengthIsLessThanMaxLength()
        {
            const int maxLength = 100;
            var response = await _httpClient!.GetAsync($"https://catfact.ninja/fact?max_length={maxLength}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var catFact = JsonConvert.DeserializeObject<CatFact>(content);

            LogToFile($"API Response: {content}\n");

            Assert.That(catFact, Is.Not.Null);
            Assert.That(catFact!.Length, Is.Not.Null);
            Assert.That(catFact!.Length, Is.LessThanOrEqualTo(maxLength));
        }

        [Test]
        public async Task Test_CatFactLengthIsLessThanOrEqualToMaxInt()
        {
            const int maxLength = int.MaxValue;
            var response = await _httpClient!.GetAsync($"https://catfact.ninja/fact?max_length={maxLength}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var catFact = JsonConvert.DeserializeObject<CatFact>(content);
            
            LogToFile($"API Response: {content}\n");

            Assert.That(catFact, Is.Not.Null);
            Assert.That(catFact!.Length, Is.LessThanOrEqualTo(maxLength));
        }

        [Test]
        public async Task Test_MissingMaxLengthParameter()
        {
            var response = await _httpClient!.GetAsync("https://catfact.ninja/fact");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var catFact = JsonConvert.DeserializeObject<CatFact>(content);

            LogToFile($"API Response: {content}\n");

            Assert.That(catFact, Is.Not.Null);
            Assert.That(catFact!.Fact, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public async Task Test_IncorrectMaxLengthValue_Negative()
        {
            var response = await _httpClient!.GetAsync("https://catfact.ninja/fact?max_length=-1");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var catFact = JsonConvert.DeserializeObject<CatFact>(content);

            LogToFile($"API Response: {content}\n");

            Assert.That(catFact, Is.Not.Null);
            Assert.That(catFact!.Fact, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public async Task Test_IncorrectMaxLengthValue_NonNumeric()
        {
            var response = await _httpClient!.GetAsync("https://catfact.ninja/fact?max_length=abc");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var catFact = JsonConvert.DeserializeObject<CatFact>(content);
            
            LogToFile($"API Response: {content}\n");

            Assert.That(catFact, Is.Not.Null);
            Assert.That(catFact!.Fact, Is.Not.Null.And.Not.Empty);
        }

        private void LogToFile(string message)
        {
            File.AppendAllText(LogFilePath, $"{DateTime.Now}: {message}\n");
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _httpClient.Dispose();
            LogToFile("Test Execution Completed.");
        }
    }
}