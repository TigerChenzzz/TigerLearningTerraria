namespace TigerLearning.Common.Interface;

public interface ILocalPlayerHitCooldown {
    public int LocalPlayerHitCooldown { get; }
    public int LocalPlayerImmunity { get; }
    public bool CanHitPlayer(int playerWai);
}

public abstract class ModProjectileWithLocalPlayerHitCooldown : ModProjectile, ILocalPlayerHitCooldown {
    public int LocalPlayerHitCooldown { get; set; } = -2;
    public int LocalPlayerImmunity { get; set; }
    public bool CanHitPlayer(int playerWai) => LocalPlayerImmunity == 0;
    public override void AI() {
        if(LocalPlayerImmunity > 0) {
            LocalPlayerImmunity -= 1;
        }
    }
    public override bool CanHitPlayer(Player target) {
        return CanHitPlayer(target.whoAmI);
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo info) {
        if(LocalPlayerHitCooldown > -2) {
            LocalPlayerImmunity = LocalPlayerHitCooldown;
        }
        NetHandler.ProjectileHitPlayer(Projectile, target);
    }
    public void SetPlayerHitCooldown() {
        if(LocalPlayerHitCooldown > -2) {
            LocalPlayerImmunity = LocalPlayerHitCooldown;
        }
    }
}
