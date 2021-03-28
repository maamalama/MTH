using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HackAnalysis.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.ML;
using Microsoft.ML.Data;
using static Microsoft.ML.DataOperationsCatalog;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms.Text;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Http;
using HackAnalysis.Helpers;
using HackAnalysis.NModels;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using ILogger = Serilog.ILogger;
using Syncfusion.XlsIO;

namespace HackAnalysis.Controllers
{
    [Route("api/[controller]")]
    public class AnalysisController : ControllerBase
    {
        private readonly ILogger<AnalysisController> _logger;
        HttpClient client = new HttpClient();
        private static MLContext mlContext => new MLContext();
        
        public AnalysisController(ILogger<AnalysisController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private static object _singlethone { get; set; }
        

        [HttpGet(nameof(GetUserAnalysis))]
        public async Task<IActionResult> GetUserAnalysis(string comment)
        {
            MLContext mlContext = new MLContext();
            var users = await GetUsers();
            List<SentimentData> Sentiments = new List<SentimentData>();
            for (int i = 0; i < users.users.Count; i++)
            {
                Sentiments.Add(new SentimentData());
                Sentiments[i].SentimentText = users.users[i].comment;
                Sentiments[i].Sentiment = users.users[i].comment_positively == 0 ? true: false;
                
                
                
            }
            IDataView data = mlContext.Data.LoadFromEnumerable<SentimentData>(Sentiments);
            var dt = data.GetRowCount();
            var preview = data.Preview();
            TrainTestData splitDataView = LoadData(mlContext, data);
            ITransformer model = BuildAndTrainModel(mlContext, splitDataView.TrainSet);
            // </SnippetCallBuildAndTrainModel>

            // <SnippetCallEvaluate>
            Evaluate(mlContext, model, splitDataView.TestSet);
            // </SnippetCallEvaluate>

            // <SnippetCallUseModelWithSingleItem>
            var result = await UseModelWithSingleItem(mlContext, model, comment);
            // </SnippetCallUseModelWithSingleItem>

            // <SnippetCallUseModelWithBatchItems>
            UseModelWithBatchItems(mlContext, model);
            // </SnippetCallUseModelWithBatchItems>


            _logger.LogDebug("=============== End of process ===============");
            return Ok(new
            {
                result = (Convert.ToBoolean(result.Prediction) ? true : false),
                prob = result.Probability
            });
        }

        public TrainTestData LoadData(MLContext mlContext, IDataView dataView)
        {
            TrainTestData splitDataView = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);

            return splitDataView;
        }

        public ITransformer BuildAndTrainModel(MLContext mlContext, IDataView splitTrainSet)
        {
            var estimator = mlContext.Transforms.Text.FeaturizeText(outputColumnName: "Features",
                    inputColumnName: nameof(SentimentData.SentimentText))
                .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label",
                    featureColumnName: "Features"));
            // </SnippetAddTrainer>

            // Create and train the model based on the dataset that has been loaded, transformed.
            // <SnippetTrainModel>

            _logger.LogWarning("=============== Create and Train the Model ===============");
            var model = estimator.Fit(splitTrainSet);
            _logger.LogWarning("=============== End of training ===============");

            // </SnippetTrainModel>

            // Returns the model we trained to use for evaluation.
            // <SnippetReturnModel>
            return model;
            // </SnippetReturnModel>
        }

        public void Evaluate(MLContext mlContext, ITransformer model, IDataView splitTestSet)
        {
            // Evaluate the model and show accuracy stats

            //Take the data in, make transformations, output the data.
            // <SnippetTransformData>
            _logger.LogWarning("=============== Evaluating Model accuracy with Test data===============");
            IDataView predictions = model.Transform(splitTestSet);
            // </SnippetTransformData>

            // BinaryClassificationContext.Evaluate returns a BinaryClassificationEvaluator.CalibratedResult
            // that contains the computed overall metrics.
            // <SnippetEvaluate>
            CalibratedBinaryClassificationMetrics metrics =
                mlContext.BinaryClassification.Evaluate(predictions, "Label");
            // </SnippetEvaluate>

            // The Accuracy metric gets the accuracy of a model, which is the proportion
            // of correct predictions in the test set.

            // The AreaUnderROCCurve metric is equal to the probability that the algorithm ranks
            // a randomly chosen positive instance higher than a randomly chosen negative one
            // (assuming 'positive' ranks higher than 'negative').

            // The F1Score metric gets the model's F1 score.
            // The F1 score is the harmonic mean of precision and recall:
            //  2 * precision * recall / (precision + recall).

            // <SnippetDisplayMetrics>

            _logger.LogWarning("Model quality metrics evaluation");
            _logger.LogWarning("--------------------------------");
            _logger.LogWarning($"Accuracy: {metrics.Accuracy:P2}");
            _logger.LogWarning($"Auc: {metrics.AreaUnderRocCurve:P2}");
            _logger.LogWarning($"F1Score: {metrics.F1Score:P2}");
            _logger.LogWarning("=============== End of model evaluation ===============");
            //</SnippetDisplayMetrics>
        }

        private async Task<SentimentPrediction> UseModelWithSingleItem(MLContext mlContext, ITransformer model, string comment)
        {
            // <SnippetCreatePredictionEngine1>
            PredictionEngine<SentimentData, SentimentPrediction> predictionFunction =
                mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);
            // </SnippetCreatePredictionEngine1>

            // <SnippetCreateTestIssue1>
            SentimentData sampleStatement = new SentimentData
            {
                SentimentText = comment
            };
            // </SnippetCreateTestIssue1>

            // <SnippetPredict>
            var resultPrediction = predictionFunction.Predict(sampleStatement);
            // </SnippetPredict>
            // <SnippetOutputPrediction>

            _logger.LogWarning(
                "=============== Prediction Test of model with a single sample and test dataset ===============");


            _logger.LogWarning(
                $"Sentiment: {resultPrediction.SentimentText} | Prediction: {(Convert.ToBoolean(resultPrediction.Prediction) ? "Positive" : "Negative")} | Probability: {resultPrediction.Probability} ");

            _logger.LogWarning("=============== End of Predictions ===============");

            return resultPrediction;
            // </SnippetOutputPrediction>
        }

        public void UseModelWithBatchItems(MLContext mlContext, ITransformer model)
        {
            // Adds some comments to test the trained model's data points.
            // <SnippetCreateTestIssues>
            IEnumerable<SentimentData> sentiments = new[]
            {
                new SentimentData
                {
                    SentimentText = "Отвратительная выставка"
                },
                new SentimentData
                {
                    SentimentText = "Прекрасная выставка"
                }
            };
            // </SnippetCreateTestIssues>

            // Load batch comments just created
            // <SnippetPrediction>
            IDataView batchComments = mlContext.Data.LoadFromEnumerable(sentiments);

            IDataView predictions = model.Transform(batchComments);

            // Use model to predict whether comment data is Positive (1) or Negative (0).
            IEnumerable<SentimentPrediction> predictedResults =
                mlContext.Data.CreateEnumerable<SentimentPrediction>(predictions, reuseRowObject: false);
            // </SnippetPrediction>

            // <SnippetAddInfoMessage>


            _logger.LogWarning("=============== Prediction Test of loaded model with multiple samples ===============");
            // </SnippetAddInfoMessage>


            // <SnippetDisplayResults>
            foreach (SentimentPrediction prediction in predictedResults)
            {
                _logger.LogWarning(
                    $"Sentiment: {prediction.SentimentText} | Prediction: {(Convert.ToBoolean(prediction.Prediction) ? "Positive" : "Negative")} | Probability: {prediction.Probability} ");
            }

            _logger.LogWarning("=============== End of predictions ===============");
            // </SnippetDisplayResults>
        }

        private async Task<Users> GetUsers()
        {
            HttpResponseMessage response = await client.GetAsync("http://back.com.xsph.ru/api/user");
            response.EnsureSuccessStatusCode();
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            string responseBody = await response.Content.ReadAsStringAsync();

            var users = JsonConvert.DeserializeObject<Users>(responseBody, settings);
            return users;
        }

        private async Task<MemoryStream> CreateUsersExcel()
        {
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Excel2016;

                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet worksheet = workbook.Worksheets[0];
                IMigrantRange migrantRange = worksheet.MigrantRange;
                worksheet.UsedRangeIncludesFormatting = false;
                worksheet.Name = "filename";
                worksheet.Range["A1:A1"].Text = "Наименование";
                worksheet.Range["B1:B1"].Text = "Обозначение сб. ед. куда входит";
                worksheet.Range["C1:C1"].Text = "Наименование сб. ед. куда входит";
                worksheet.Range["D1:D1"].Text = "Количество на ед.";
                worksheet.Range["E1:E1"].Text = "Всего";
                worksheet.Range["A1:A1"].ColumnWidth = 70;
                worksheet.Range["B1:B1"].ColumnWidth = 40;
                worksheet.Range["C1:C1"].ColumnWidth = 20;
                worksheet.Range["D1:D1"].ColumnWidth = 15;
                worksheet.Range["E1:E1"].ColumnWidth = 15;

                IStyle bodyStyle = workbook.Styles.Add("BodyStyle");
                bodyStyle.BeginUpdate();
                bodyStyle.Font.FontName = "Calibri";
                bodyStyle.Font.Size = 12;

                IStyle headerStyle = workbook.Styles.Add("HeaderStyle");
                headerStyle.BeginUpdate();
                headerStyle.Font.FontName = "Calibri";
                headerStyle.Font.Size = 16;
                headerStyle.Font.Bold = true;


                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream;
                }
            }
        }

        [HttpGet(nameof(GetUserReport))]
        public async Task<IActionResult> GetUserReport()
        {
            var usersData = await GetUsers();
            var fileDownloadName = $"Users_{DateTime.UtcNow.ToString()}.csv";
            return new UsersCsvResult(usersData.users, fileDownloadName);
        }
    }
}