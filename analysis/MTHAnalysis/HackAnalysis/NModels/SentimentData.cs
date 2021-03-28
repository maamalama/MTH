using Microsoft.ML.Data;

namespace HackAnalysis.NModels
{
    public class SentimentData
    {
        [LoadColumn(0)]
        public string SentimentText;

        [ColumnName("Label")]
        public bool Sentiment;

       
    }
    
    public class SentimentPrediction : SentimentData
    {

        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }

        public float Probability { get; set; }

        public float Score { get; set; }
    }
}