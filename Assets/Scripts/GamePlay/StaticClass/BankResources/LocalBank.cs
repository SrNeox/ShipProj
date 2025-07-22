using YG;

public static class LocalBank
{
    public static int Score { get; private set; }

    public static void AddScore(int countScore)
    {
        Score += countScore;
    }    

    public static void TryChangeScore()
    {
        if(Score > YG2.saves.Score)
        {
            YG2.saves.Score = Score;
            YG2.SaveProgress();
            Score = 0;
        }
        else
        {
            Score = 0;
        }
    }
}