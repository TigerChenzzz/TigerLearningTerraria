using System.IO;

namespace TigerLearning.Common.MyUtils;

public class NetHandler {
    public delegate void HandlerDelegate(BinaryReader reader, int whoAmI);
    public static Mod Mod => TigerLearning.Instance;
    public static void Heal(NPC target, int amount, bool handle = false, int whoAmI = -1) {
        target.life += amount;
        if(target.life > target.lifeMax) {
            target.life = target.lifeMax;
        }
        if(Main.netMode != NetmodeID.Server) {
            target.HealEffect(amount, false);
        }
        if(Main.netMode == NetmodeID.Server || !handle && Main.netMode == NetmodeID.MultiplayerClient) {
            ModPacket packet = Mod.GetPacket();
            packet.Write(TigerMessageID.HealNPC);
            packet.Write(target.whoAmI);
            packet.Write(amount);
            packet.Send(-1, whoAmI);
        }
    }
    public static void HandleHealNPC(BinaryReader reader, int whoAmI) {
        int npcWai = reader.ReadInt32();
        int healAmount = reader.ReadInt32();
        if(npcWai >= 0 && npcWai < Main.npc.Length) {
            NPC npc = Main.npc[npcWai];
            if(npc.active) {
                Heal(npc, healAmount, true, whoAmI);
            }
        }
    }
    public static void PreHeal(NPC target, bool handle = false, int whoAmI = -1) {
        target.life += 1;
        if(Main.netMode == NetmodeID.Server || !handle && Main.netMode == NetmodeID.MultiplayerClient) {
            ModPacket packet = Mod.GetPacket();
            packet.Write(TigerMessageID.PreHealNPCWhenZeroDamage);
            packet.Write(target.whoAmI);
            packet.Send(-1, whoAmI);
        }
    }
    public static void HandlePreHealNPCWhenZeroDamage(BinaryReader reader, int whoAmI) {
        int npcWai = reader.ReadInt32();
        if(npcWai >= 0 && npcWai < Main.npc.Length) {
            NPC npc = Main.npc[npcWai];
            if(npc.active) {
                PreHeal(npc, true, whoAmI);
            }
        }
    }
    public static void Heal(Player target, int amount, bool handle = false, int whoAmI = -1) {
        target.statLife += amount;
        if(target.statLife > target.statLifeMax2) {
            target.statLife = target.statLifeMax2;
        }
        if(Main.netMode != NetmodeID.Server) {
            target.HealEffect(amount, false);
        }
        if(Main.netMode == NetmodeID.Server || !handle && Main.netMode == NetmodeID.MultiplayerClient) {
            ModPacket packet = Mod.GetPacket();
            packet.Write(TigerMessageID.HealPlayer);
            packet.Write(target.whoAmI);
            packet.Write(amount);
            packet.Send(-1, whoAmI);
        }
    }
    public static void HandleHealPlayer(BinaryReader reader, int whoAmI) {
        int wai = reader.ReadInt32();
        int healAmount = reader.ReadInt32();
        if(wai >= 0 && wai < Main.player.Length) {
            Player player = Main.player[wai];
            if(player.active && !player.dead && !player.ghost) {
                Heal(player, healAmount, true, whoAmI);
            }
        }
    }
    public static void HealMana(Player target, int amount, bool handle = false, int whoAmI = -1) {
        target.statMana += amount;
        if(target.statMana > target.statManaMax2) {
            target.statMana = target.statManaMax2;
        }
        if(Main.netMode != NetmodeID.Server) {
            CombatText.NewText(target.Hitbox, CombatText.HealMana, amount);
        }
        if(Main.netMode == NetmodeID.Server || !handle && Main.netMode == NetmodeID.MultiplayerClient) {
            ModPacket packet = Mod.GetPacket();
            packet.Write(TigerMessageID.HealPlayerMana);
            packet.Write(target.whoAmI);
            packet.Write(amount);
            packet.Send(-1, whoAmI);
        }
    }
    public static void HandleHealPlayerMana(BinaryReader reader, int whoAmI) {
        int playerWai = reader.ReadInt32();
        int healAmount = reader.ReadInt32();
        if(playerWai >= 0 && playerWai < Main.player.Length) {
            Player player = Main.player[playerWai];
            if(player.active && !player.dead && !player.ghost) {
                HealMana(player, healAmount, true, whoAmI);
            }
        }
    }
    public static void ProjectileHitPlayer(Projectile projectile, Player player, bool handle = false, int whoAmI = -1) {
        if(projectile.penetrate >= 0) {
            projectile.penetrate -= 1;
            if(projectile.penetrate <= 0) {
                projectile.Kill();
            }
        }
        if(Main.netMode == NetmodeID.Server || !handle && Main.netMode == NetmodeID.MultiplayerClient) {
            ModPacket packet = Mod.GetPacket();
            packet.Write(TigerMessageID.ProjectileHitPlayer);
            packet.Write(projectile.whoAmI);
            packet.Write(player.whoAmI);
            packet.Send(-1, whoAmI);
        }
    }
    public static void HandleProjectileHitPlayer(BinaryReader reader, int whoAmI) {
        int projectileWai = reader.ReadInt32();
        int playerWai = reader.ReadInt32();
        if(playerWai >= 0 && playerWai < Main.player.Length && projectileWai >= 0 && projectileWai < Main.projectile.Length) {
            Player player = Main.player[playerWai];
            Projectile projectile = Main.projectile[projectileWai];
            if(player.active && projectile.active && !player.dead && !player.ghost) {
                ProjectileHitPlayer(projectile, player, true, whoAmI);
            }
        }
    }
    public static void SetNPCUnactive(int npcWai, bool handle = false, int whoAmI = -1) {
        Do(Main.npc[npcWai] != null && (Main.npc[npcWai].active = false));
        if(Main.netMode == NetmodeID.Server || !handle && Main.netMode == NetmodeID.MultiplayerClient) {
            ModPacket packet = Mod.GetPacket();
            packet.Write(TigerMessageID.SetNPCUnactive);
            packet.Write(npcWai);
            packet.Send(-1, whoAmI);
        }
    }
    public static void HandleSetNPCUnactive(BinaryReader reader, int whoAmI) {
        int npcWai = reader.ReadInt32();
        SetNPCUnactive(npcWai, true, whoAmI);
    }
    public static Dictionary<int, HandlerDelegate> Handlers { get; } = new(){
        {TigerMessageID.HealNPC                  , HandleHealNPC                  },
        {TigerMessageID.PreHealNPCWhenZeroDamage , HandlePreHealNPCWhenZeroDamage },
        {TigerMessageID.HealPlayer               , HandleHealPlayer               },
        {TigerMessageID.HealPlayerMana           , HandleHealPlayerMana           },
        {TigerMessageID.ProjectileHitPlayer      , HandleProjectileHitPlayer      },
        {TigerMessageID.SetNPCUnactive           , HandleSetNPCUnactive           },
    };
}
