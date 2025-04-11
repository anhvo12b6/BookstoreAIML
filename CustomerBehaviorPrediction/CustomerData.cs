using Microsoft.ML.Data;

public class CustomerData
{
    [LoadColumn(0)]
    public float Age { get; set; }

    [LoadColumn(1)]
    public string Gender { get; set; }

    [LoadColumn(2)]
    public float TotalPageViews { get; set; }

    [LoadColumn(3)]
    public float TimeOnSite { get; set; }

    [LoadColumn(4)]
    public float PreviousPurchases { get; set; }

    [LoadColumn(5)]
    public string FavoriteCategory { get; set; }

    [LoadColumn(6)]
    public bool WillBuy { get; set; }
}


public class CustomerPrediction
{
    [ColumnName("PredictedLabel")]
    public bool WillBuy { get; set; }

    public float Probability { get; set; }
    public float Score { get; set; }
}
