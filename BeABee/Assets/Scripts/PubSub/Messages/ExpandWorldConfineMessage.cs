using PubSub;

public class ExpandWorldConfineMessage : IMessage
{
	public bool GoingToTheSky;
	public ExpandWorldConfineMessage(bool goingToTheSky)
	{
		GoingToTheSky = goingToTheSky;
	}
}