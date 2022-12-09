using PubSub;

public class AddBirdMessage : IMessage
{
    public BirdVariant BirdVariant;
    public bool DoubleBirds;
    public AddBirdMessage(BirdVariant birdVariant, bool doubleBirds)
    {
        BirdVariant = birdVariant;
        DoubleBirds = doubleBirds;
    }
}

