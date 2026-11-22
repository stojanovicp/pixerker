using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    [SerializeField] private GameObject goldCoin, healthGlobe, staminaGlobe;


    public void DropItems()
    {
        int randomNum = Random.Range(1,100);

        if(randomNum < 30)
        {
            return;
        }
        else if(randomNum < 70)
        {
            Instantiate(goldCoin,transform.position,Quaternion.identity);
        }
        else if(randomNum < 85)
        {
            Instantiate(healthGlobe,transform.position,Quaternion.identity);
        }
        else
        {
            Instantiate(staminaGlobe,transform.position,Quaternion.identity);
        }
    }
}
