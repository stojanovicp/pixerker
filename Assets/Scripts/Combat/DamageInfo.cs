using UnityEngine;

public readonly struct DamageInfo
{
    public readonly int Amount;
    public readonly float KnockbackForce;
    public readonly Vector2 SourcePosition;
    public readonly DamageFlags Flags;

    public DamageInfo(int amount, float knockbackForce, Vector2 sourcePosition, DamageFlags flags)
    {
        Amount = amount;
        KnockbackForce = knockbackForce;
        SourcePosition = sourcePosition;
        Flags = flags;
    }
}
