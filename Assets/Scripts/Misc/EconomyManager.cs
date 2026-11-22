using System;

public class EconomyManager : Singleton<EconomyManager>
{
    public static event Action<int> GoldChanged;

    private int currentGold = 0;

    private void Start()
    {
        GoldChanged?.Invoke(currentGold);
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
        GoldChanged?.Invoke(currentGold);
    }

    public int TakeAllGold()
    {
        int takenGold = currentGold;
        currentGold = 0;
        GoldChanged?.Invoke(currentGold);
        return takenGold;
    }
}
