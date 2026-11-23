// An attacker that can run any AttackPattern (Shooter fires it, Grape lobs it). Bosses use this to pick
// a different pattern for each attack.
public interface IPatternAttacker
{
    void AttackWith(AttackPattern pattern);
}
