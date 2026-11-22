using UnityEngine;

public class DroppedCoinRecord
{
    public string SceneName { get; }
    public Vector2 Position { get; }
    public int Amount { get; }

    public DroppedCoinRecord(string sceneName, Vector2 position, int amount)
    {
        SceneName = sceneName;
        Position = position;
        Amount = amount;
    }
}
