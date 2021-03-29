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
using System.Linq;
using System.Net;
using System.Net.Http;
using HackAnalysis.Helpers;
using HackAnalysis.NModels;
using Microsoft.ML.TimeSeries;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using ILogger = Serilog.ILogger;
using Syncfusion.XlsIO;
using Microsoft.ML.Transforms.TimeSeries;

namespace HackAnalysis.Controllers
{
    [Route("api/[controller]")]
    public class AnalysisController : ControllerBase
    {
        private readonly ILogger<AnalysisController> _logger;
        HttpClient client = new HttpClient();
        private static MLContext mlContext => new MLContext();
        static string rootDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../"));
        string modelPath = Path.Combine(rootDir, "MLModel.zip");

        public AnalysisController(ILogger<AnalysisController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public class Stat
        {
            public float UsersCnt { get; set; }
            public int Month { get; set; }
            public float Year { get; set; }
        }
        
        public class PhoneCallsData
        {
            [LoadColumn(0)]
            public string timestamp;

            [LoadColumn(1)]
            public double value;
        }

        public class PhoneCallsPrediction
        {
            //vector to hold anomaly detection results. Including isAnomaly, anomalyScore, magnitude, expectedValue, boundaryUnits, upperBoundary and lowerBoundary.
            [VectorType(7)]
            public double[] Prediction { get; set; }
        }
        
        public static int GetAge(DateTime d1, DateTime d2)
        {
            var r = d2.Year - d1.Year;
            return d1.AddYears(r) <= d2 ? r : r -1;
        }

        [HttpGet(nameof(GetUserAnomaly))]
        public async Task<IActionResult> GetUserAnomaly()
        {
            MLContext mlContext = new MLContext();
            // </SnippetCreateMLContext>
            var users = await GetUsers();
            List<PhoneCallsData> ph = new List<PhoneCallsData>();

            foreach (var user in users.users)
            {
                ph.Add(new PhoneCallsData{timestamp = user.created_at.ToString(),value = GetAge(DateTime.Parse(user.date_birth),DateTime.Now)});
            }

            //STEP 1: Common data loading configuration
            // <SnippetLoadData>
            IDataView dataView = mlContext.Data.LoadFromEnumerable<PhoneCallsData>(ph);
            // </SnippetLoadData>

            // Detect seasonality for the series
            // <SnippetCallDetectPeriod>
            int period = DetectPeriod(mlContext, dataView);
            // </SnippetCallDetectPeriod>

            // Detect anomaly for the series with period information
            // <SnippetCallDetectAnomaly>
            DetectAnomaly(mlContext, dataView, period);
            
            return Ok();
        }
        
         int DetectPeriod(MLContext mlContext, IDataView phoneCalls)
        {
            _logger.LogError("Detect period of the series");

            // STEP 2: Detect seasonality
            // <SnippetDetectSeasonality>
            int period = mlContext.AnomalyDetection.DetectSeasonality(phoneCalls, nameof(PhoneCallsData.value));
            // </SnippetDetectSeasonality>

            // <SnippetDisplayPeriod>
            _logger.LogError("Period of the series is: {0}.", period);
            // </SnippetDisplayPeriod>

            return period;
        }

        void DetectAnomaly(MLContext mlContext, IDataView phoneCalls, int period)
        {
            _logger.LogError("Detect anomaly points in the series");

            //STEP 2: Setup the parameters
            // <SnippetSetupSrCnnParameters>
            var options = new SrCnnEntireAnomalyDetectorOptions()
            {
                Threshold = 0.3,
                Sensitivity = 64.0,
                DetectMode = SrCnnDetectMode.AnomalyAndMargin,
                Period = period,
            };
            // </SnippetSetupSrCnnParameters>

            //STEP 3: Detect anomaly by SR-CNN algorithm
            // <SnippetDetectAnomaly>
            IDataView outputDataView =
                mlContext
                    .AnomalyDetection.DetectEntireAnomalyBySrCnn(
                        phoneCalls,
                        nameof(PhoneCallsPrediction.Prediction),
                        nameof(PhoneCallsData.value),
                        options);
            // </SnippetDetectAnomaly>

            // <SnippetCreateEnumerableForResult>
            IEnumerable<PhoneCallsPrediction> predictions = mlContext.Data.CreateEnumerable<PhoneCallsPrediction>(
                outputDataView, reuseRowObject: false);
            // </SnippetCreateEnumerableForResult>

            // <SnippetDisplayHeader>
            _logger.LogError("Index\tAnomaly\tExpectedValue\tUpperBoundary\tLowerBoundary");
            // </SnippetDisplayHeader>

            // <SnippetDisplayAnomalyDetectionResults>
            var index = 0;

            foreach (var p in predictions)
            {
                if (p.Prediction[0] == 1)
                {
                    _logger.LogError("{0},{1},{2},{3},{4}  <-- alert is on, detected anomaly", index,
                        p.Prediction[0], p.Prediction[3], p.Prediction[5], p.Prediction[6]);
                }
                else
                {
                    _logger.LogError("{0},{1},{2},{3},{4}", index,
                        p.Prediction[0], p.Prediction[3], p.Prediction[5], p.Prediction[6]);
                }
                ++index;

            }

           
            // </SnippetDisplayAnomalyDetectionResults>
        }

        [HttpGet(nameof(GetUserPrognoz))]
        public async Task<IActionResult> GetUserPrognoz()
        {
            MLContext mlContext = new MLContext();
            var users = await GetUsers();
            IDataView data = mlContext.Data.LoadFromEnumerable<User>(users.users);

            List<Stat> stats = new List<Stat>();

            for (int i = 2012; i < 2016; i++)
            {
                for (int j = 1; j <= 12; j++)
                {
                    var cnt = users.users.Where(x => x._year == i && x._month == j);
                    stats.Add(new Stat
                    {
                        UsersCnt = cnt.Count(),
                        Month = j,
                        Year = i,
                    });
                }
            }

            foreach (var user in users.users)
            {
                user.Year = user._year;
                user.Month = user._month;
            }

            IDataView firstYearData = mlContext.Data.LoadFromEnumerable<Stat>(stats.Where(x => x.Year < 2016));
            IDataView secondYearData = mlContext.Data.LoadFromEnumerable<Stat>(stats.Where(x => x.Year > 2016));

            var userA = users.users.Where(x => x._year == 2018);
            var usersACount = userA.Count();
            var AA = stats.Where(x => x.Year < 2014);
            var BB = stats.Where(x => x.Year > 2016).Count();
 
            /*foreach (var VARIABLE in stats)
            {
                _logger.LogDebug($"{VARIABLE.Year}");
            }*/
            
            var forecastingPipeline = mlContext.Forecasting.ForecastBySsa(
                outputColumnName: "ForecastedCount",
                inputColumnName: "UsersCnt",
                windowSize: 28,
                seriesLength: 30,
                trainSize: 365,
                horizon: 28,
                confidenceLevel: 0.95f,
                confidenceLowerBoundColumn: "LowerBoundCount",
                confidenceUpperBoundColumn: "UpperBoundCount");
            
            SsaForecastingTransformer forecaster = forecastingPipeline.Fit(firstYearData);

            Evaluate(secondYearData, forecaster, mlContext);

            var forecastEngine = forecaster.CreateTimeSeriesEngine<Stat, ProgOutput>(mlContext);
            forecastEngine.CheckPoint(mlContext, modelPath);

            Forecast(firstYearData , 28, forecastEngine, mlContext);
            return Ok();
        }

         void Evaluate(IDataView testData, ITransformer model, MLContext mlContext)
        {
            // Make predictions
            IDataView predictions = model.Transform(testData);

            // Actual values
            IEnumerable<float> actual =
                mlContext.Data.CreateEnumerable<Stat>(testData, true)
                    .Select(observed => observed.UsersCnt);

            // Predicted values
            IEnumerable<float> forecast =
                mlContext.Data.CreateEnumerable<ProgOutput>(predictions, true)
                    .Select(prediction => prediction.ForecastedCount[0]);

            // Calculate error (actual - forecast)
            var metrics = actual.Zip(forecast, (actualValue, forecastValue) => actualValue - forecastValue);

            // Get metric averages
            /*var MAE = metrics.Average(error => Math.Abs(error)); // Mean Absolute Error
            var RMSE = Math.Sqrt(metrics.Average(error => Math.Pow(error, 2))); // Root Mean Squared Error

            // Output metrics
            Console.WriteLine("Evaluation Metrics");
            Console.WriteLine("---------------------");
            Console.WriteLine($"Mean Absolute Error: {MAE:F3}");
            Console.WriteLine($"Root Mean Squared Error: {RMSE:F3}\n");*/
        }

        void Forecast(IDataView testData, int horizon, TimeSeriesPredictionEngine<Stat, ProgOutput> forecaster, MLContext mlContext)
        {

            ProgOutput forecast = forecaster.Predict();

            IEnumerable<string> forecastOutput =
                mlContext.Data.CreateEnumerable<Stat>(testData, reuseRowObject: false)
                    .Take(horizon)
                    .Select((Stat rental, int index) =>
                    {
                        string rentalDate = rental.Year.ToString();
                        float actualRentals = rental.UsersCnt;
                        float lowerEstimate = Math.Max(0, forecast.LowerBoundCount[index]);
                        float estimate = forecast.ForecastedCount[index];
                        float upperEstimate = forecast.UpperBoundCount[index];
                        return
                            $"Actual Rentals: {actualRentals}\n";
                    });
            
            Console.WriteLine("Rental Forecast");
            Console.WriteLine("---------------------");
            foreach (var prediction in forecastOutput)
            {
               _logger.LogError(prediction);
            }
        }
    


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
                Sentiments[i].Sentiment = users.users[i].comment_positively == 0 ? true : false;
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
            //UseModelWithBatchItems(mlContext, model);
            // </SnippetCallUseModelWithBatchItems>


            _logger.LogDebug("=============== End of process ===============");
            _logger.LogDebug($"{comment} = {result.Probability}");
            return Ok(new
            {
                result = (Convert.ToBoolean(result.Prediction) ? true : false)
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

        private async Task<SentimentPrediction> UseModelWithSingleItem(MLContext mlContext, ITransformer model,
            string comment)
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