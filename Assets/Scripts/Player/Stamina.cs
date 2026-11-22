using System;
using System.Collections;
using UnityEngine;

public class Stamina : Singleton<Stamina>
{
    public static event Action<int, int> StaminaChanged;

    public int CurrentStamina { get; private set; }

    [SerializeField] private int maxStamina = 4;
    [SerializeField] private int timeBetweenRefresh = 3;

    protected override void Awake()
    {
        base.Awake();

        CurrentStamina = maxStamina;
    }

    private void Start()
    {
        RaiseStaminaChanged();
    }

    public void UseStamina()
    {
        if(CurrentStamina > 0)
        {
            CurrentStamina--;
            RaiseStaminaChanged();
            StopAllCoroutines();
            StartCoroutine(RefreshStaminaRoutine());
        }
    }

    public void RefreshStamina()
    {
        if(CurrentStamina < maxStamina && !PlayerHealth.Instance.IsDead)
        {
            CurrentStamina++;
            RaiseStaminaChanged();
        }
    }

    public void ReplenishStaminaOnDeath()
    {
        CurrentStamina = maxStamina;
        RaiseStaminaChanged();
    }

    private IEnumerator RefreshStaminaRoutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(timeBetweenRefresh);
            RefreshStamina();
        }
    }

    private void RaiseStaminaChanged()
    {
        StaminaChanged?.Invoke(CurrentStamina, maxStamina);
    }
}
