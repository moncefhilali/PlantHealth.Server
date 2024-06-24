using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;

namespace PlantHealth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AzureController : ControllerBase
    {
        private readonly string endpoint = "https://planthealth-prediction.cognitiveservices.azure.com/";
        private readonly string projectId = "ea8de1b3-6066-42f5-bc97-583ccd4e4270";
        private readonly string publishedName = "HouseplantHealth";
        private readonly string predictionKey = "71210b602d7844fa82d5c232debb3bd7";
        private readonly CustomVisionPredictionClient predictionClient;
        
        public AzureController()
        {
            predictionClient = new CustomVisionPredictionClient(new ApiKeyServiceClientCredentials(predictionKey))
            {
                Endpoint = endpoint
            };
        }

        [HttpPost("classify")]
        public async Task<IActionResult> Classify(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("No image provided.");

            using var ms = new MemoryStream();
            await image.CopyToAsync(ms);
            ms.Position = 0;

            var result = await predictionClient.ClassifyImageAsync(
                new Guid(projectId), publishedName, ms);

            var prediction = result.Predictions[0];
            string classificationResult = prediction.TagName;

            return Ok(new { result = classificationResult });
        }
    }
}
