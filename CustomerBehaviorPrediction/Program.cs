using Microsoft.ML;
using Microsoft.ML.Data;
using System;

class Program
{
    static void Main(string[] args)
    {
        var context = new MLContext();

        // Bước 1: Đọc dữ liệu
        var data = context.Data.LoadFromTextFile<CustomerData>(
            "customer_data.csv", hasHeader: true, separatorChar: ',');

        // Tạo cột Label từ WillBuy
        var dataWithLabel = context.Transforms.CopyColumns("Label", nameof(CustomerData.WillBuy))
            .Fit(data)
            .Transform(data);

        // Bước 2: Chia dữ liệu train/test
        var split = context.Data.TrainTestSplit(dataWithLabel, testFraction: 0.2);

        // Bước 3: Pipeline huấn luyện
        var pipeline = context.Transforms.Categorical.OneHotEncoding("Gender")
            .Append(context.Transforms.Categorical.OneHotEncoding("FavoriteCategory"))
            .Append(context.Transforms.Concatenate("Features",
                "Age", "Gender", "TotalPageViews", "TimeOnSite", "PreviousPurchases", "FavoriteCategory"))
            .Append(context.Transforms.NormalizeMinMax("Features"))
            .Append(context.BinaryClassification.Trainers.FastTree());

        // Bước 4: Huấn luyện
        var model = pipeline.Fit(split.TrainSet);
        var predictions = model.Transform(split.TestSet);
        var metrics = context.BinaryClassification.Evaluate(predictions);

        Console.WriteLine($"🎯 Accuracy: {metrics.Accuracy:P2}");
        Console.WriteLine($"🎯 AUC: {metrics.AreaUnderRocCurve:P2}");
        Console.WriteLine($"🎯 F1 Score: {metrics.F1Score:P2}");

        // Dự đoán thử
        var predictor = context.Model.CreatePredictionEngine<CustomerData, CustomerPrediction>(model);

        var input = new CustomerData
        {
            Age = 30,
            Gender = "Female",
            TotalPageViews = 1221,
            TimeOnSite = 2530,
            PreviousPurchases = 6,
            FavoriteCategory = "Books"
        };

        var result = predictor.Predict(input);
        Console.WriteLine($"\n🧠 Dự đoán: {(result.WillBuy ? "Sẽ mua" : "Không mua")} - Xác suất: {result.Probability:P2}");

        // Lưu mô hình
        context.Model.Save(model, split.TrainSet.Schema, "customer_model.zip");
        Console.WriteLine("\n✅ Đã lưu mô hình vào 'customer_model.zip'");
    }
}
