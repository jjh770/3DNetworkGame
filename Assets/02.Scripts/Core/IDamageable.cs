public interface IDamageable
{
    // Photon에는 ActorNumber가 있음.
    public void TakeDamage(float damage, int attackerActorNumber);
}
