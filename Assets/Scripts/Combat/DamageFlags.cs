using System;

[Flags]
public enum DamageFlags
{
    None = 0,

    // Ignores the damage amount: a normal enemy loses all remaining HP, a boss a fixed share of its max HP.
    Execute = 1 << 0
}
