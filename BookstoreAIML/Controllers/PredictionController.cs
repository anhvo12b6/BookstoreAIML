using Microsoft.AspNetCore.Mvc;
using BookstoreAIML.MLModels;
using Microsoft.ML;

namespace BookstoreAIML.Controllers
{
    public class PredictionController : Controller
    {
        private static PredictionEngine<CustomerData, CustomerPrediction> _predictionEngine;

        public PredictionController()
        {
            if (_predictionEngine == null)
            {
                var mlContext = new MLContext();
                var model = mlContext.Model.Load("customer_model.zip", out _);
                _predictionEngine = mlContext.Model.CreatePredictionEngine<CustomerData, CustomerPrediction>(model);
            }
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(CustomerData input)
        {
            var result = _predictionEngine.Predict(input);
            ViewBag.Result = result.WillBuy ? "Sẽ mua" : "Không mua";
            ViewBag.Probability = $"{result.Probability:P2}";
            return View();
        }
    }
}
