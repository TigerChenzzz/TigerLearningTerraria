using ReLogic.Graphics;
using System.IO;
using System.Reflection;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;

namespace TigerLearning.Content.NPCs;
internal class TestShooter : ModNPC {
    public Vector2 targetPosition;
    public int seed;
    public Item shooterItem;

    public override void SetDefaults() {
        NPC.width = 16;
        NPC.height = 16;

        //temp this:
        NPC.damage = 0;
        NPC.defense = 12;
        NPC.lifeMax = 333;
        NPC.HitSound = SoundID.NPCHit10;
        NPC.DeathSound = SoundID.NPCDeath10;
        //NPC.value = 0;
        NPC.knockBackResist = .1f;
        NPC.aiStyle = -1; // = NPCAIStyleID.Flying;
        //AIType = NPCID.Hornet;
        NPC.noGravity = true;
        NPC.rarity = 3;
        NPC.spriteDirection = 1;
        shooterItem = new Item(ItemID.Zenith);
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
        {
            new FlavorTextBestiaryInfoElement("Test Creature"),
        });
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo) {
        //不会每帧都调用, 经测试, 大约一秒调用一次, 应该只在尝试刷怪的时候被调用
        return 0;
    }

    public override void OnSpawn(IEntitySource source) {
        //不会在多人客户端运行
        DebugPrint("Test Shooter On Spawn");
        seed = Main.rand.Next();
    }

    public override void SendExtraAI(BinaryWriter writer) {
        DebugPrint($"Test Shooter Send Extra AI({Main.rand.Next(2, 10)}), life is {NPC.life}");
        writer.Write(seed);
    }

    public override void ReceiveExtraAI(BinaryReader reader) {
        int readSeed = reader.ReadInt32();
        DebugPrint($"Test Shooter Recive Extra AI({Main.rand.Next(2, 10)}), life is {NPC.life}, seed is {seed}, read seed is {readSeed}");
        seed = readSeed;

    }

    public override void AI() {
        #region 寻敌和对准目标
        NPC.TargetClosest();
        NPCAimedTarget targetData = NPC.GetTargetData();
        NPC.rotation = (targetData.Center - NPC.Center).ToRotation();
        Player targetPlayer = null;
        if(targetData.Type == NPCTargetType.Player) {
            targetPlayer = Main.player[NPC.target];
            if(!targetPlayer.active || targetPlayer.dead) {
                targetPlayer = null;
            }
        }
        #endregion

        #region Wander
        NPC.ai[1] += 1;
        if(Do(false))  //暂时不做什么事
        {
            if(NPC.ai[1] >= 200) {
                NPC.ai[1] = -200;
            }
            if(MathF.Abs(NPC.ai[1]) < 100) {
                NPC.velocity.X = 2;
            }
            else {
                NPC.velocity.X = -2;
            }
            if(NPC.ai[1] < 0) {
                NPC.velocity.Y = -2;
            }
            else {
                NPC.velocity.Y = 2;
            }
        }
        #endregion

        #region 发射
        if(Main.netMode != NetmodeID.MultiplayerClient) {
            NPC.ai[0] += 1;
            if(NPC.ai[0] >= 240 && targetPlayer != null) {
                //这里用NewProjectile射出会自动同步, 且会使用projectile的各个SendExtraData以同步信息, projectile的OnSpawn只在服务器上被调用, 以下一行是没问题的
                //Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.UnitX * 16, Vector2.UnitX * 3, ProjectileID.FireArrow, 20, 3, -1);

                //int playerItemTime = targetPlayer.itemTime;
                //int playerItemAnimation = targetPlayer.itemAnimation;
                //Vector2 mountedCenter = targetPlayer.MountedCenter;
                //服务器的localPlayer是Main.player[255], 但就算在服务器中它也不是active的
                DebugPrint($"Test Shooter Origin Shoot");
                Main.player[255].whoAmI = Main.myPlayer;
                Main.player[255].MountedCenter = NPC.Center;
                Main.player[255].oldPosition = Main.player[255].position;
                int mouseX = Main.mouseX, mouseY = Main.mouseY;
                (Main.mouseX, Main.mouseY) = ((int)(targetPlayer.position.X - Main.screenPosition.X), (int)(targetPlayer.position.Y - Main.screenPosition.Y));
                //在服务器上用反射来调用Shoot就根本不会有弹幕产生, 服务器上都不会调用OnSpawn
                typeof(Player).GetMethod("ItemCheck_Shoot", BindingFlags.NonPublic | BindingFlags.Instance).
                    Invoke(Main.player[255], new object[3] { 255, shooterItem, 40 });
                Main.mouseX = mouseX;
                Main.mouseY = mouseY;

                //试了试把原版代码P下来, 但是一样的在多人中没用, 在单人中正常      !!TODO: 下一步就是debug原版代码了, 看看到底是什么阻止了弹幕生成
                //VanillaCode.ItemCheck_Shoot(Main.player[255], 255, shooterItem.item, 40);
                //targetPlayer.MountedCenter = mountedCenter;
                //targetPlayer.itemTime = playerItemTime;
                //targetPlayer.itemAnimation = playerItemAnimation;

                //尝试使用player[255]来做原始发射的假人, 在单人中好像没问题, 就是会对着鼠标位置发射,
                //但在多人仍然射不出来, 以下五行在调整鼠标位置后单人可用
                //if (Main.player[255] == null)   Main.player[255] = new();
                //Player fakePlayer = Main.player[255];
                //fakePlayer.MountedCenter = NPC.Center;
                //typeof(Player).GetMethod("ItemCheck_Shoot", BindingFlags.NonPublic | BindingFlags.Instance).
                //    Invoke(fakePlayer, new object[3] { 255, shooterItem.item, 40 });

                NPC.ai[0] = 0;
                //NPC.netUpdate = true;
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, number: NPC.whoAmI);
            }
        }
        #endregion
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        return base.PreDraw(spriteBatch, screenPos, drawColor);
    }

    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        spriteBatch.DrawString(FontAssets.ItemStack.Value, NPC.ai[0].ToString(), new Vector2(NPC.position.X - 15 - Main.screenPosition.X, NPC.position.Y + 22 - Main.screenPosition.Y), Color.White);
        spriteBatch.DrawString(FontAssets.ItemStack.Value, NPC.target.ToString(), new Vector2(NPC.position.X + 30 - Main.screenPosition.X, NPC.position.Y + 22 - Main.screenPosition.Y), Color.White);
    }

    /*
    生成时的执行顺序:
        服务器: SetDefaults * 2, OnSpawn, AI, SendExtraAI, AI...
        多人客户端: SetDefaults, ReceiveExtraAI, AI...
        单人: SetDefaults, OnSpawn, AI...
    死亡时的执行顺序:
        服务器: ...AI, OnKill, SendExtraAI
        多人客户端: ...AI, SetDefaults, ReceiveExtraAI  (不知道为什么还要SetDefaults, 
            就像不知道为什么生成时服务器要SetDefaults两次, 而且服务器开始的第二次和多人客户端
            结束的第二次SetDefaults好像都是崭新的NPC, ModNPC无论值或引用类字段都存不下来)
        单人: ...AI(结束, 单人就一点毛病没有)
    */
}
