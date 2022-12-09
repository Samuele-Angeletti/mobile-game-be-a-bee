using PubSub;
public class ScoreChangeMessage : IMessage
{
	public int Score;
	public ScoreChangeMessage(int score)
	{
		Score = score;
	}
}