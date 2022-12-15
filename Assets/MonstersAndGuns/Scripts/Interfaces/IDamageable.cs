public enum DamageMode { Collision, Shooting}

public interface IDamageable
{
    float Health { get; }
    float CurrentHealthPercentage { get; }
    bool IsDead { get; }
    public void Damage(float damage, DamageMode mode);
}