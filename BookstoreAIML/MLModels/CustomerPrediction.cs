using Microsoft.ML.Data;

namespace BookstoreAIML.MLModels
{
    public class CustomerPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool WillBuy { get; set; }

        public float Probability { get; set; }

        
    }
}
