using Microsoft.Xna.Framework.Input;
using ReLogic.OS;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using Terraria.Audio;
using Terraria.Cinematics;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Achievements;
using Terraria.GameContent.Creative;
using Terraria.GameContent.Events;
using Terraria.GameContent.Liquid;
using Terraria.GameContent.Skies;
using Terraria.GameContent.UI;
using Terraria.GameInput;
using Terraria.Graphics.Capture;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Light;
using Terraria.Graphics.Shaders;
using Terraria.Initializers;
using Terraria.UI;

namespace TigerLearning.Documents;

public partial class Document {
    public class Entity_cls {
        public static Entity entity;
        public const string intro = "Player, Item, NPC的基类";
        public static void ShowEntity() {
            #region params
            Vector2 position = default;
            Rectangle hitbox = default;
            #endregion
            Show(entity.velocity);          //速度
            Show(entity.position);          //位置(世界坐标), 似乎是左上角(TBT)
            Show(entity.active);            //代表此实体是否存在于游戏世界中, 在遍历Main的各实体数组时都需先检查此值是否为真
            Show(entity.Hitbox);            //碰撞箱
            entity.Hitbox.Intersects(hitbox);//碰撞检测
            Show(entity.Center);            //中心位置
            Show(entity.Distance(position));//到点position(世界坐标)的像素距离
            Show(entity.DistanceSQ(position));//距离的平方
            Show(entity.Size);              //大小
            Show(entity.whoAmI);            //在Main的对应实体数组中的序号(Item不适用), 包括Projectile, NPC 和 Player
        }
    }
    public class Player_cls {
        public static Player player;
        public static void ShowPlayer() {
            #region params
            int i = 0, j = 0, distance = 10, pickPower = 0, itemID = 0, buffIndex = 0, buffID = 0, time = 60;
            int minionProjectileId = 0, originalDamageNotScaledByMinionDamage = 0;
            float knockBack = 0;
            Vector2 point = default;
            IEntitySource source = null;
            Item accessory = default;
            bool hideVisuals = false;
            #endregion
            Show(player.whoAmI);            //玩家在Main.player数组中的下标
            Show(player.inventory);         //库存数组, 包括第一排, 钱币和弹药, 但不包括垃圾桶
            string specialInventorySlot = """
                player.inventory[58]代指鼠标上拿起的物品, 由Main.mouseItem克隆而来。
                    实际上克隆发生在Player.dropItemCheck中, 而这个在失去焦点或者在不能使用物品时不会执行(参见Player.Update)
                    另外在!Main.LocalPlayer.JustDroppedAnItem 且 player.selectedItem == 58 且 player.itemAnimation != 0 时
                        Main.mouseItem反过来会是正在使用的物品的克隆 (参见Player.ItemCheck_Inner)
                        在Player.PlaceWeapon, Player.DropCoins, TEFoodPlatter.PlaceItemInFrame, TEItemFrame.PlaceItemInFrame,
                        TEWeaponsRack.PlaceItemInFrame等处中也有这样的克隆
                如果要对鼠标上的东西作出修改, 不能修改这里, 而需要修改Main.mouseItem。
                另外当鼠标上有物品时, player.HeldItem也是player.inventory[58], 所以
                此时对player.HeldItem作出修改也是没用的(但其他时候就可以对它作出修改)。
                """;
            Show(player.HeldItem);
            string playerHeldItemIntro = """
                player.HeldItem为玩家手上的物品
                (在第一排中选中的, 如果鼠标上有物品, 则是鼠标上的)
                实际上是player.inventory[player.selectedItem]的简称
                """;
            Show(player.numberOfDeathsPVE); //PVE死亡数
            Show(player.numberOfDeathsPVP); //PVP死亡数
            #region 是否拥有物品
            player.HasItem(itemID);
            player.HasItemInAnyInventory(itemID);
            player.HasItemInInventoryOrOpenVoidBag(itemID);
            #endregion
            player.IsWithinSnappngRangeToTile(i, j, distance);      //是否在某物块的一定距离内, i, j为物块坐标, distance为像素距离
            player.Distance(point);         //玩家到点的距离
            player.PickTile(i, j, pickPower);
            #region 控制
            Show(player.controlUp);         //上
            Show(player.controlDown);       //下
            Show(player.controlLeft);       //左
            Show(player.controlRight);      //右
            Show(player.controlJump);       //跳
            Show(player.controlDownHold);   //按住下
            Show(player.controlHook);       //钩子(E)
            Show(player.controlMount);      //坐骑(R)
            Show(player.controlInv);        //背包(待测试Esc)
            Show(player.controlMap);        //地图(M)
            Show(player.controlQuickHeal);  //快速治疗
            Show(player.controlQuickMana);  //快速回魔
            Show(player.controlUseItem);    //使用物品
            Show(player.controlUseTile);    //使用物块
            #endregion
            #region 附近的物块和液体
            Show(player.adjWater);
            Show(player.adjLava);
            Show(player.adjHoney);
            Show(player.adjShimmer);
            Show(player.adjTile[TileID.Sinks]);
            #endregion
            #region 位置
            Show(player.ZoneDungeon);           //地牢
            Show(player.ZoneCorrupt);           //腐化
            Show(player.ZoneHallow);            //神圣
            Show(player.ZoneMeteor);            //陨石
            Show(player.ZoneJungle);            //丛林
            Show(player.ZoneSnow);              //雪原
            Show(player.ZoneCrimson);           //猩红
            Show(player.ZoneWaterCandle);       //水蜡烛
            Show(player.ZonePeaceCandle);       //和平蜡烛
            Show(player.ZoneTowerSolar);        //以下四条为四柱
            Show(player.ZoneTowerVortex);
            Show(player.ZoneTowerNebula);
            Show(player.ZoneTowerStardust);
            Show(player.ZoneDesert);            //沙漠
            Show(player.ZoneGlowshroom);        //发光蘑菇
            Show(player.ZoneUndergroundDesert); //地下沙漠
            Show(player.ZoneSkyHeight);         //太空

            Show(player.ZoneOverworldHeight);   //地上
            Show(player.ZoneDirtLayerHeight);   //地下
            Show(player.ZoneRockLayerHeight);   //洞穴
            Show(player.ZoneUnderworldHeight);  //地狱
            string 更多关于高度的信息 ="参见Main_cls.ShowMain()中的关于高度信息区域";

            Show(player.ZoneBeach);             //沙滩
            Show(player.ZoneRain);              //下雨
            Show(player.ZoneSandstorm);         //沙尘暴
            Show(player.ZoneOldOneArmy);        //旧日军团
            Show(player.ZoneGranite);           //花岗岩
            Show(player.ZoneMarble);            //大理石
            Show(player.ZoneHive);              //蜂巢
            Show(player.ZoneGemCave);           //宝石洞穴
            Show(player.ZoneLihzhardTemple);    //神庙
            Show(player.ZoneGraveyard);         //墓地

            Show(player.ShoppingZone_Forest);   //森林购物区??
            #endregion
            #region Buff
            Show(player.buffTime[buffIndex]);   //某个buff的持续时间
            Show(player.buffImmune[buffID]);    //是否免疫某buff
            Show(player.buffType[buffIndex]);   //获得某个buff的ID
            player.AddBuff(buffID, time);       //增加一个buff, 自动处理免疫, 不同模式下时间增幅, 和重添加逻辑
            player.DelBuff(buffIndex);          //删除对应下标的buff
            player.ClearBuff(buffID);           //删除对应类型的buff
            #endregion
            #region 装备
            Show(player.armor); // 原版装甲栏和饰品栏, 包含时装
            Show(player.dye);   // 染料
            Show(player.miscEquips);    // "装备"
            Show(player.miscDyes);      // "装备"的染料
            #region 饰品
            // 如果要在其他地方调用以下三句最好是在 ModPlayer.UpdateEquips() 中
            player.GrantArmorBenefits(accessory);   // 获得饰品的基本效果
            player.ApplyEquipFunctional(accessory, hideVisuals);    // 获得额外的饰品效果
            player.GrantPrefixBenefits(accessory);  // 获得饰品前缀的效果
            #endregion
            #endregion
            Show(player.fallStart); //开始掉落的位置, 物块坐标Y
            Show(player.talkNPC);   //在对话的NPC在npc数组中的下标, -1表示没有对话的NPC
            Show(player.TalkNPC);   //在对话的NPC, 若没有则为null
            Show(player.currentShoppingSettings.PriceAdjustment);   //当前商店的定价调整, 1f代表正常, <= 0.9表示足够快乐
            player.SpawnMinionOnCursor(source, player.whoAmI, minionProjectileId, originalDamageNotScaledByMinionDamage, knockBack);
        }

        // player.Update(i)
        public static void ShowPlayerUpdate(int playerWai) {
            #region 将 Main._currentPlayerOverride 设置为 this, 在此方法结束时还原
            // using var _currentPlr = new Main.CurrentPlayerOverride(this);
            #endregion
            #region 更新 LockOnHelper
            if (playerWai == Main.myPlayer && Main.netMode != NetmodeID.Server)
                LockOnHelper.Update();
            #endregion
            #region 饥荒黑暗造成伤害
            if (playerWai == Main.myPlayer && Main.dontStarveWorld)
                DontStarveDarknessDamageDealer.Update(player);
            #endregion

            #region 设置一些运动相关的属性
            player.maxFallSpeed = 10f;
            player.gravity = Player.defaultGravity;
            Player.jumpHeight = 15;
            Player.jumpSpeed = 5.01f;
            player.maxRunSpeed = 3f;
            player.runAcceleration = 0.08f;
            player.runSlowdown = 0.2f;
            player.accRunSpeed = player.maxRunSpeed;
            if (!player.mount.Active || !player.mount.Cart)
                player.onWrongGround = false;

            player.heldProj = -1;
            player.instantMovementAccumulatedThisFrame = Vector2.Zero;
            if (player.PortalPhysicsEnabled)
                player.maxFallSpeed = 35f;

            if (player.shimmerWet || player.shimmering) {
                if (player.shimmering) {
                    player.gravity *= 0.9f;
                    player.maxFallSpeed *= 0.9f;
                }
                else {
                    player.gravity = 0.15f;
                    Player.jumpHeight = 23;
                    Player.jumpSpeed = 5.51f;
                }
            }
            else if (player.wet) {
                if (player.honeyWet) {
                    player.gravity = 0.1f;
                    player.maxFallSpeed = 3f;
                }
                else if (player.merman) {
                    player.gravity = 0.3f;
                    player.maxFallSpeed = 7f;
                }
                else if (player.trident && !player.lavaWet) {
                    player.gravity = 0.25f;
                    player.maxFallSpeed = 6f;
                    Player.jumpHeight = 25;
                    Player.jumpSpeed = 5.51f;
                    if (player.controlUp) {
                        player.gravity = 0.1f;
                        player.maxFallSpeed = 2f;
                    }
                }
                else {
                    player.gravity = 0.2f;
                    player.maxFallSpeed = 5f;
                    Player.jumpHeight = 30;
                    Player.jumpSpeed = 6.01f;
                }
            }

            if (player.vortexDebuff)
                player.gravity = 0f;

            player.maxFallSpeed += 0.01f;
            #endregion

            #region 单独对自己的 player 设置一些属性
            if (Main.myPlayer == playerWai) {
                if (Main.mapFullscreen)
                    player.GamepadEnableGrappleCooldown();
                else if (player._quickGrappleCooldown > 0)
                    player._quickGrappleCooldown--;

                TileObject.objectPreview.Reset();
                if (DD2Event.DownedInvasionAnyDifficulty)
                    player.downedDD2EventAnyDifficulty = true;

                player.autoReuseAllWeapons = Main.SettingsEnabled_AutoReuseAllItems;
            }
            #endregion
            #region 免费蛋糕
            if (NPC.freeCake && player.talkNPC >= 0 && Main.npc[player.talkNPC].type == NPCID.PartyGirl) {
                NPC.freeCake = false;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Item.NewItem(new EntitySource_Gift(Main.npc[player.talkNPC]), (int)player.position.X, (int)player.position.Y, player.width, player.height, 3750);
            }
            #endregion

            #region 继续设置一些属性 (emoteTime, ghostDmg, lifeSteal)
            if (player.emoteTime > 0)
                player.emoteTime--;

            if (player.ghostDmg > 0f)
                player.ghostDmg -= 6.6666665f;

            if (player.ghostDmg < 0f)
                player.ghostDmg = 0f;

            if (Main.expertMode) {
                if (player.lifeSteal < 70f)
                    player.lifeSteal += 0.5f;

                if (player.lifeSteal > 70f)
                    player.lifeSteal = 70f;
            }
            else {
                if (player.lifeSteal < 80f)
                    player.lifeSteal += 0.6f;

                if (player.lifeSteal > 80f)
                    player.lifeSteal = 80f;
            }
            #endregion
            #region ResizeHitbox (根据 player.HeightOffsetBoost 调整玩家高度)
            player.position.Y += player.height;
            player.height = 42 + player.HeightOffsetBoost;
            player.position.Y -= player.height;
            #endregion
            #region 0 号坐骑特判(发光)
            if (player.mount.Active && player.mount.Type == 0) {
                int num = (int)(player.position.X + (player.width / 2)) / 16;
                int j = (int)(player.position.Y + (player.height / 2) - 14f) / 16;
                Lighting.AddLight(num, j, 0.5f, 0.2f, 0.05f);
                Lighting.AddLight(num + player.direction, j, 0.5f, 0.2f, 0.05f);
                Lighting.AddLight(num + player.direction * 2, j, 0.5f, 0.2f, 0.05f);
            }
            #endregion

            #region 更新非自己玩家的 outOfRange 并作相应处理
            player.outOfRange = false;
            // bool flag = false;
            bool playerOutOfRange = false;
            if (player.whoAmI != Main.myPlayer) {
                int num2 = (int)(player.position.X + (player.width / 2)) / 16;
                int num3 = (int)(player.position.Y + (player.height / 2)) / 16;
                /*
				if (!WorldGen.InWorld(num2, num3, 4))
					flag = true;
				else if (Main.tile[num2, num3] == null)
					flag = true;
				else if (Main.tile[num2 - 3, num3] == null)
					flag = true;
				else if (Main.tile[num2 + 3, num3] == null)
					flag = true;
				else if (Main.tile[num2, num3 - 3] == null)
					flag = true;
				else if (Main.tile[num2, num3 + 3] == null)
					flag = true;
				*/
                if (Main.netMode == NetmodeID.MultiplayerClient && !Main.sectionManager.TilesLoaded(num2 - 3, num3 - 3, num2 + 3, num3 + 3))
                    playerOutOfRange = true;

                if (playerOutOfRange) {
                    player.outOfRange = true;
                    player.numMinions = 0;
                    player.slotsMinions = 0f;
                    player.itemAnimation = 0;
                    player.UpdateBuffs(playerWai);
                    player.PlayerFrame();
                }
            }
            #endregion
            #region player.tankPet 处理
            if (player.tankPet >= 0) {
                if (!player.tankPetReset)
                    player.tankPetReset = true;
                else
                    player.tankPet = -1;
            }
            #endregion
            #region 设置自己玩家的 IsVoidVaultEnabled
            if (playerWai == Main.myPlayer)
                player.IsVoidVaultEnabled = player.HasItem(ItemID.VoidLens);
            #endregion
            #region 一些计时 (chatOverhead.timeLeft, snowBallLauncherInteractionCooldown, environmentBuffImmunityTimer)
            if (player.chatOverhead.timeLeft > 0)
                player.chatOverhead.timeLeft--;

            if (player.snowBallLauncherInteractionCooldown > 0)
                player.snowBallLauncherInteractionCooldown--;

            player.environmentBuffImmunityTimer = Math.Max(0, player.environmentBuffImmunityTimer - 1);
            #endregion
            #region 如果玩家不在范围内(outOfRange), 则结束, 返回
            if (playerOutOfRange)
                return;
            #endregion
            #region 更新头发染料的粒子(UpdateHairDyeDust), 更新杂项的计时器(UpdateMiscCounter)
            player.UpdateHairDyeDust();
            player.UpdateMiscCounter();
            #endregion

            #region ModPlayer.PreUpdate
            PlayerLoader.PreUpdate(player);
            #endregion

            #region 一些计时器
            #region infernoCounter++, 若到达 180 则置为 0
            player.infernoCounter++;
            if (player.infernoCounter >= 180)
                player.infernoCounter = 0;
            #endregion
            #region timeSinceLastDashStarted++, 最大到 300
            player.timeSinceLastDashStarted++;
            if (player.timeSinceLastDashStarted >= 300)
                player.timeSinceLastDashStarted = 300;
            #endregion
            #region _framesLeftEligibleForDeadmansChestDeathAchievement--, 最小到 0
            player._framesLeftEligibleForDeadmansChestDeathAchievement--;
            if (player._framesLeftEligibleForDeadmansChestDeathAchievement < 0)
                player._framesLeftEligibleForDeadmansChestDeathAchievement = 0;
            #endregion
            #region titaniumStormCooldown--, 最小到 0
            if (player.titaniumStormCooldown > 0)
                player.titaniumStormCooldown--;
            #endregion
            #region 星星斗篷计时与效果
            if (player.starCloakCooldown > 0) {
                player.starCloakCooldown--;
                if (Main.rand.NextBool(5)) {
                    for (int k = 0; k < 2; k++) {
                        Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.ManaRegeneration, 0f, 0f, 255, default, Main.rand.Next(20, 26) * 0.1f);
                        dust.noLight = true;
                        dust.noGravity = true;
                        dust.velocity *= 0.5f;
                        dust.velocity.X = 0f;
                        dust.velocity.Y -= 0.5f;
                    }
                }

                if (player.starCloakCooldown == 0)
                    SoundEngine.PlaySound(SoundID.MaxMana);
            }
            #endregion
            #region _timeSinceLastImmuneGet++, 最大到 10000
            player._timeSinceLastImmuneGet++;
            if (player._timeSinceLastImmuneGet >= 10000)
                player._timeSinceLastImmuneGet = 10000;
            #endregion
            #endregion
            #region 重力调整 (计算 gravity multiplier 并乘给 player.gravity)
            float num4 = Main.maxTilesX / 4200f;
            num4 *= num4;
            float gravityMultiplier = (float)((double)(player.position.Y / 16f - (60f + 10f * num4)) / (Main.worldSurface / (Main.remixWorld ? 1.0 : 6.0))); // num5

            if (Main.remixWorld) {
                if (gravityMultiplier < 0.1f)
                    gravityMultiplier = 0.1f;
            }
            else if (gravityMultiplier < 0.25f) {
                gravityMultiplier = 0.25f;
            }

            if (gravityMultiplier > 1f)
                gravityMultiplier = 1f;
            player.gravity *= gravityMultiplier;
            #endregion
            #region 设置 maxRegenDelay
            player.maxRegenDelay = (1f - player.statMana / (float)player.statManaMax2) * 60f * 4f + 45f;
            player.maxRegenDelay *= 0.7f;
            #endregion
            #region 一些视觉效果 (UpdateSocialShadow, UpdateTeleportVisuals)
            player.UpdateSocialShadow();
            player.UpdateTeleportVisuals();
            #endregion

            #region 将 player.whoAmI 设置为传入的参数
            player.whoAmI = playerWai;
            #endregion

            #region 对于自己的玩家, 在非对应事件时清空身上的天国魔力, 尝试穿过传送门, 尝试自动开门(PurgeDD2EnergyCrystals, TryPortalJumping, doorHelper.Update)
            if (player.whoAmI == Main.myPlayer) {
                if (!DD2Event.Ongoing)
                    player.PurgeDD2EnergyCrystals();

                player.TryPortalJumping();
                if (player.whoAmI == Main.myPlayer)
                    player.doorHelper.Update(player);
            }
            #endregion
            #region 一些计时 (runSoundDelay, attackCD, potionDelay)
            if (player.runSoundDelay > 0)
                player.runSoundDelay--;

            if (player.attackCD > 0)
                player.attackCD--;

            if (player.itemAnimation == 0)
                player.attackCD = 0;

            if (player.potionDelay > 0)
                player.potionDelay--;
            #endregion
            #region 若是自己的玩家, 则销毁垃圾桶中的大水晶, 且更新群系和召唤物目标
            if (playerWai == Main.myPlayer) {
                #region 大水晶扔垃圾桶里直接销毁
                if (player.trashItem.type >= ItemID.LargeAmethyst && player.trashItem.type <= ItemID.LargeDiamond)
                    player.trashItem.SetDefaults();

                if (player.trashItem.type == ItemID.LargeAmber)
                    player.trashItem.SetDefaults();
                #endregion
                #region 更新群系和召唤物目标
                player.UpdateBiomes();
                player.UpdateMinionTarget();
                #endregion
            }
            #endregion

            #region 若是幽灵状态, 则处理幽灵状态并返回
            if (player.ghost) {
                player.Ghost();
                return;
            }
            #endregion
            #region 若是死亡状态则处理死亡状态并返回
            if (player.dead) {
                player.UpdateDead();
                player.ResetProjectileCaches();
                player.UpdateProjectileCaches(playerWai);
                return;
            }
            #endregion

            #region 尝试生成微光环境中的仙灵
            player.TrySpawningFaelings();
            #endregion
            #region 露西斧说话
            if (playerWai == Main.myPlayer && player.hasLucyTheAxe)
                LucyAxeMessage.TryPlayingIdleMessage();
            #endregion
            #region 在地上(velocity.Y == 0)时恢复坐骑的疲劳值
            if (player.velocity.Y == 0f)
                player.mount.FatigueRecovery();
            #endregion
            #region 自己玩家的控制...
            if (playerWai == Main.myPlayer && !player.isControlledByFilm) {
                #region 重置各种控制字段
                player.ResetControls();
                #endregion
                #region 当没有失焦时设置各种控制字段并依其作一些操作
                if (Main.hasFocus) {
                    if (!Main.drawingPlayerChat && !Main.editSign && !Main.editChest && !Main.blockInput) {
                        PlayerInput.Triggers.Current.CopyInto(player);
                        player.LocalInputCache = new Player.DirectionalInputSyncCache(player);
                        if (Main.mapFullscreen) {
                            if (player.controlUp)
                                Main.mapFullscreenPos.Y -= 1f * (16f / Main.mapFullscreenScale);

                            if (player.controlDown)
                                Main.mapFullscreenPos.Y += 1f * (16f / Main.mapFullscreenScale);

                            if (player.controlLeft)
                                Main.mapFullscreenPos.X -= 1f * (16f / Main.mapFullscreenScale);

                            if (player.controlRight)
                                Main.mapFullscreenPos.X += 1f * (16f / Main.mapFullscreenScale);

                            player.controlUp = false;
                            player.controlLeft = false;
                            player.controlDown = false;
                            player.controlRight = false;
                            player.controlJump = false;
                            player.controlUseItem = false;
                            player.controlUseTile = false;
                            player.controlThrow = false;
                            player.controlHook = false;
                            player.controlTorch = false;
                            player.controlSmart = false;
                            player.controlMount = false;
                        }

                        if (player.isOperatingAnotherEntity)
                            player.controlUp = (player.controlDown = (player.controlLeft = (player.controlRight = (player.controlJump = false))));

                        if (player.controlQuickHeal) {
                            if (player.releaseQuickHeal)
                                player.QuickHeal();

                            player.releaseQuickHeal = false;
                        }
                        else {
                            player.releaseQuickHeal = true;
                        }

                        if (player.controlQuickMana) {
                            if (player.releaseQuickMana)
                                player.QuickMana();

                            player.releaseQuickMana = false;
                        }
                        else {
                            player.releaseQuickMana = true;
                        }

                        if (player.controlCreativeMenu) {
                            if (player.releaseCreativeMenu)
                                player.ToggleCreativeMenu();

                            player.releaseCreativeMenu = false;
                        }
                        else {
                            player.releaseCreativeMenu = true;
                        }

                        if (player.controlLeft && player.controlRight) {
                            player.controlLeft = false;
                            player.controlRight = false;
                        }

                        if (PlayerInput.UsingGamepad || !player.mouseInterface || !ItemSlot.Options.DisableLeftShiftTrashCan) {
                            if (PlayerInput.SteamDeckIsUsed && PlayerInput.SettingsForUI.CurrentCursorMode == CursorMode.Mouse)
                                player.TryToToggleSmartCursor(ref Main.SmartCursorWanted_Mouse);
                            else if (PlayerInput.UsingGamepad)
                                player.TryToToggleSmartCursor(ref Main.SmartCursorWanted_GamePad);
                            else
                                player.TryToToggleSmartCursor(ref Main.SmartCursorWanted_Mouse);
                        }

                        if (player.controlSmart)
                            player.releaseSmart = false;
                        else
                            player.releaseSmart = true;

                        if (player.controlMount) {
                            if (player.releaseMount)
                                player.QuickMount();

                            player.releaseMount = false;
                        }
                        else {
                            player.releaseMount = true;
                        }

                        if (Main.mapFullscreen) {
                            if (player.mapZoomIn)
                                Main.mapFullscreenScale *= 1.05f;

                            if (player.mapZoomOut)
                                Main.mapFullscreenScale *= 0.95f;
                        }
                        else {
                            if (Main.mapStyle == 1) {
                                if (player.mapZoomIn)
                                    Main.mapMinimapScale *= 1.025f;

                                if (player.mapZoomOut)
                                    Main.mapMinimapScale *= 0.975f;

                                if (player.mapAlphaUp)
                                    Main.mapMinimapAlpha += 0.015f;

                                if (player.mapAlphaDown)
                                    Main.mapMinimapAlpha -= 0.015f;
                            }
                            else if (Main.mapStyle == 2) {
                                if (player.mapZoomIn)
                                    Main.mapOverlayScale *= 1.05f;

                                if (player.mapZoomOut)
                                    Main.mapOverlayScale *= 0.95f;

                                if (player.mapAlphaUp)
                                    Main.mapOverlayAlpha += 0.015f;

                                if (player.mapAlphaDown)
                                    Main.mapOverlayAlpha -= 0.015f;
                            }

                            if (player.mapStyle) {
                                if (player.releaseMapStyle) {
                                    SoundEngine.PlaySound(SoundID.MenuTick); // 12
                                    Main.mapStyle++;
                                    if (Main.mapStyle > 2)
                                        Main.mapStyle = 0;
                                }

                                player.releaseMapStyle = false;
                            }
                            else {
                                player.releaseMapStyle = true;
                            }
                        }

                        if (player.mapFullScreen) {
                            if (player.releaseMapFullscreen) {
                                if (Main.mapFullscreen) {
                                    SoundEngine.PlaySound(SoundID.MenuClose); // 11
                                    Main.mapFullscreen = false;
                                }
                                else {
                                    player.TryOpeningFullscreenMap();
                                }
                            }

                            player.releaseMapFullscreen = false;
                        }
                        else {
                            player.releaseMapFullscreen = true;
                        }
                    }
                    else if (!PlayerInput.UsingGamepad && !Main.editSign && !Main.editChest && !Main.blockInput) {
                        PlayerInput.Triggers.Current.CopyIntoDuringChat(player);
                    }

                    if (player.confused) {
                        bool flag2 = player.controlLeft;
                        bool flag3 = player.controlUp;
                        player.controlLeft = player.controlRight;
                        player.controlRight = flag2;
                        player.controlUp = player.controlRight;
                        player.controlDown = flag3;
                    }
                    else if (player.cartFlip) {
                        if (player.controlRight || player.controlLeft) {
                            (player.controlRight, player.controlLeft) = (player.controlLeft, player.controlRight);
                        }
                        else {
                            player.cartFlip = false;
                        }
                    }

                    for (int l = 0; l < player.doubleTapCardinalTimer.Length; l++) {
                        player.doubleTapCardinalTimer[l]--;
                        if (player.doubleTapCardinalTimer[l] < 0)
                            player.doubleTapCardinalTimer[l] = 0;
                    }

                    for (int m = 0; m < 4; m++) {
                        bool flag5 = false;
                        bool flag6 = false;
                        switch (m) {
                        case 0:
                            flag5 = player.controlDown && player.releaseDown;
                            flag6 = player.controlDown;
                            break;
                        case 1:
                            flag5 = player.controlUp && player.releaseUp;
                            flag6 = player.controlUp;
                            break;
                        case 2:
                            flag5 = player.controlRight && player.releaseRight;
                            flag6 = player.controlRight;
                            break;
                        case 3:
                            flag5 = player.controlLeft && player.releaseLeft;
                            flag6 = player.controlLeft;
                            break;
                        }

                        if (flag5) {
                            if (player.doubleTapCardinalTimer[m] > 0)
                                player.KeyDoubleTap(m);
                            else
                                player.doubleTapCardinalTimer[m] = 15;
                        }

                        if (flag6) {
                            player.holdDownCardinalTimer[m]++;
                            player.KeyHoldDown(m, player.holdDownCardinalTimer[m]);
                        }
                        else {
                            player.holdDownCardinalTimer[m] = 0;
                        }
                    }

                    player.controlDownHold = player.holdDownCardinalTimer[0] >= 45;

                    PlayerLoader.SetControls(player);

                    if (player.controlInv) {
                        if (player.releaseInventory)
                            player.ToggleInv();

                        player.releaseInventory = false;
                    }
                    else {
                        player.releaseInventory = true;
                    }

                    if (player.delayUseItem) {
                        if (!player.controlUseItem)
                            player.delayUseItem = false;

                        player.controlUseItem = false;
                    }

                    if (player.itemAnimation == 0 && player.ItemTimeIsZero && player.reuseDelay == 0) {
                        player.dropItemCheck();
                        int num6 = player.selectedItem;
                        bool flag7 = false;
                        if (!Main.drawingPlayerChat && player.selectedItem != 58 && !Main.editSign && !Main.editChest) {
                            #region 如果按下了 Hotbar 1 - 10, 则将 player.selectedItem 设置为 0 - 9, 并将 flag7 设置为 true 
                            if (PlayerInput.Triggers.Current.Hotbar1) {
                                player.selectedItem = 0;
                                flag7 = true;
                            }

                            if (PlayerInput.Triggers.Current.Hotbar2) {
                                player.selectedItem = 1;
                                flag7 = true;
                            }

                            if (PlayerInput.Triggers.Current.Hotbar3) {
                                player.selectedItem = 2;
                                flag7 = true;
                            }

                            if (PlayerInput.Triggers.Current.Hotbar4) {
                                player.selectedItem = 3;
                                flag7 = true;
                            }

                            if (PlayerInput.Triggers.Current.Hotbar5) {
                                player.selectedItem = 4;
                                flag7 = true;
                            }

                            if (PlayerInput.Triggers.Current.Hotbar6) {
                                player.selectedItem = 5;
                                flag7 = true;
                            }

                            if (PlayerInput.Triggers.Current.Hotbar7) {
                                player.selectedItem = 6;
                                flag7 = true;
                            }

                            if (PlayerInput.Triggers.Current.Hotbar8) {
                                player.selectedItem = 7;
                                flag7 = true;
                            }

                            if (PlayerInput.Triggers.Current.Hotbar9) {
                                player.selectedItem = 8;
                                flag7 = true;
                            }

                            if (PlayerInput.Triggers.Current.Hotbar10) {
                                player.selectedItem = 9;
                                flag7 = true;
                            }
                            #endregion

                            int selectedBinding = player.DpadRadial.SelectedBinding;
                            int selectedBinding2 = player.CircularRadial.SelectedBinding;
                            _ = player.QuicksRadial.SelectedBinding;
                            player.DpadRadial.Update();
                            player.CircularRadial.Update();
                            player.QuicksRadial.Update();
                            if (player.CircularRadial.SelectedBinding >= 0 && selectedBinding2 != player.CircularRadial.SelectedBinding)
                                player.DpadRadial.ChangeSelection(-1);

                            if (player.DpadRadial.SelectedBinding >= 0 && selectedBinding != player.DpadRadial.SelectedBinding)
                                player.CircularRadial.ChangeSelection(-1);

                            if (player.QuicksRadial.SelectedBinding != -1 && PlayerInput.Triggers.JustReleased.RadialQuickbar && !PlayerInput.MiscSettingsTEMP.HotbarRadialShouldBeUsed) {
                                switch (player.QuicksRadial.SelectedBinding) {
                                case 0:
                                    player.QuickMount();
                                    break;
                                case 1:
                                    player.QuickHeal();
                                    break;
                                case 2:
                                    player.QuickBuff();
                                    break;
                                case 3:
                                    player.QuickMana();
                                    break;
                                }
                            }

                            if (player.controlTorch || flag7) {
                                player.DpadRadial.ChangeSelection(-1);
                                player.CircularRadial.ChangeSelection(-1);
                            }

                            if (player.controlTorch && flag7) {
                                if (player.selectedItem != player.nonTorch)
                                    SoundEngine.PlaySound(SoundID.MenuTick); // 12

                                player.nonTorch = player.selectedItem;
                                player.selectedItem = num6;
                                flag7 = false;
                            }
                        }

                        bool flag8 = Main.hairWindow;
                        if (flag8) {
                            int y = Main.screenHeight / 2 + 60;
                            flag8 = new Rectangle(Main.screenWidth / 2 - TextureAssets.HairStyleBack.Width() / 2, y, TextureAssets.HairStyleBack.Width(), TextureAssets.HairStyleBack.Height()).Contains(Main.MouseScreen.ToPoint());
                        }

                        if (flag7 && CaptureManager.Instance.Active)
                            CaptureManager.Instance.Active = false;

                        if (num6 != player.selectedItem)
                            SoundEngine.PlaySound(SoundID.MenuTick); // 12

                        if (Main.mapFullscreen) {
                            float num7 = PlayerInput.ScrollWheelDelta / 120;
                            if (PlayerInput.UsingGamepad)
                                num7 += (PlayerInput.Triggers.Current.HotbarPlus.ToInt() - PlayerInput.Triggers.Current.HotbarMinus.ToInt()) * 0.1f;

                            Main.mapFullscreenScale *= 1f + num7 * 0.3f;
                        }
                        else if (CaptureManager.Instance.Active) {
                            CaptureManager.Instance.Scrolling();
                        }
                        else if (!flag8) {
                            if (PlayerInput.MouseInModdedUI.Count > 0) { }
                            else
                            if (!Main.playerInventory) {
                                player.HandleHotbar();
                            }
                            else {
                                int num8 = Player.GetMouseScrollDelta();
                                bool flag9 = true;
                                if (Main.recBigList) {
                                    int num9 = 42;
                                    int num10 = 340;
                                    int num11 = 310;
                                    PlayerInput.SetZoom_UI();
                                    int num12 = (Main.screenWidth - num11 - 280) / num9;
                                    int num13 = (Main.screenHeight - num10 - 20) / num9;
                                    if (new Rectangle(num11, num10, num12 * num9, num13 * num9).Contains(Main.MouseScreen.ToPoint())) {
                                        num8 *= -1;
                                        int num14 = Math.Sign(num8);
                                        while (num8 != 0) {
                                            if (num8 < 0) {
                                                Main.recStart -= num12;
                                                if (Main.recStart < 0)
                                                    Main.recStart = 0;
                                            }
                                            else {
                                                Main.recStart += num12;
                                                if (Main.recStart > Main.numAvailableRecipes - num12)
                                                    Main.recStart = Main.numAvailableRecipes - num12;
                                            }

                                            num8 -= num14;
                                        }
                                    }

                                    PlayerInput.SetZoom_World();
                                }

                                if (flag9) {
                                    Main.focusRecipe += num8;
                                    if (Main.focusRecipe > Main.numAvailableRecipes - 1)
                                        Main.focusRecipe = Main.numAvailableRecipes - 1;

                                    // Extra patch context.
                                    if (Main.focusRecipe < 0)
                                        Main.focusRecipe = 0;
                                }
                            }

                            PlayerInput.MouseInModdedUI.Clear();
                        }
                    }
                    else {
                        bool flag10 = false;
                        if (!Main.drawingPlayerChat && player.selectedItem != 58 && !Main.editSign && !Main.editChest) {
                            int num15 = -1;
                            if (Main.keyState.IsKeyDown(Keys.D1)) {
                                num15 = 0;
                                flag10 = true;
                            }

                            if (Main.keyState.IsKeyDown(Keys.D2)) {
                                num15 = 1;
                                flag10 = true;
                            }

                            if (Main.keyState.IsKeyDown(Keys.D3)) {
                                num15 = 2;
                                flag10 = true;
                            }

                            if (Main.keyState.IsKeyDown(Keys.D4)) {
                                num15 = 3;
                                flag10 = true;
                            }

                            if (Main.keyState.IsKeyDown(Keys.D5)) {
                                num15 = 4;
                                flag10 = true;
                            }

                            if (Main.keyState.IsKeyDown(Keys.D6)) {
                                num15 = 5;
                                flag10 = true;
                            }

                            if (Main.keyState.IsKeyDown(Keys.D7)) {
                                num15 = 6;
                                flag10 = true;
                            }

                            if (Main.keyState.IsKeyDown(Keys.D8)) {
                                num15 = 7;
                                flag10 = true;
                            }

                            if (Main.keyState.IsKeyDown(Keys.D9)) {
                                num15 = 8;
                                flag10 = true;
                            }

                            if (Main.keyState.IsKeyDown(Keys.D0)) {
                                num15 = 9;
                                flag10 = true;
                            }

                            if (flag10) {
                                if (num15 != player.nonTorch)
                                    SoundEngine.PlaySound(SoundID.MenuTick); // 12

                                player.nonTorch = num15;
                            }
                        }
                    }
                }
                #endregion
                /* Moved into ItemCheck
				if (selectedItem != 58)
					SmartSelectLookup();
				*/
                #region 石化时或石化解除时的效果
                if (player.stoned != player.lastStoned) {
                    if (player.whoAmI == Main.myPlayer && player.stoned) {
                        int damage = (int)(20.0 * (double)Main.GameModeInfo.EnemyDamageMultiplier);
                        player.Hurt(PlayerDeathReason.ByOther(5), damage, 0);
                    }

                    SoundEngine.PlaySound(SoundID.Dig, (int)player.position.X, (int)player.position.Y); // 0
                    for (int n = 0; n < 20; n++) {
                        int num16 = Dust.NewDust(player.position, player.width, player.height, DustID.Stone);
                        if (Main.rand.NextBool())
                            Main.dust[num16].noGravity = true;
                    }
                }
                player.lastStoned = player.stoned;
                #endregion
                #region 当被冻住, 网住, 或石化时将大多数控制字段设置为 false
                if (player.frozen || player.webbed || player.stoned) {
                    player.controlJump = false;
                    player.controlDown = false;
                    player.controlLeft = false;
                    player.controlRight = false;
                    player.controlUp = false;
                    player.controlUseItem = false;
                    player.controlUseTile = false;
                    player.controlThrow = false;
                    player.gravDir = 1f;
                }
                #endregion

                #region 设置一些控制相关的字段
                if (!player.controlThrow)
                    player.releaseThrow = true;
                else
                    player.releaseThrow = false;

                if (player.controlDown && player.releaseDown) {
                    if (player.tryKeepingHoveringUp)
                        player.tryKeepingHoveringUp = false;
                    else
                        player.tryKeepingHoveringDown = true;
                }

                if (player.controlUp && player.releaseUp) {
                    if (player.tryKeepingHoveringDown)
                        player.tryKeepingHoveringDown = false;
                    else
                        player.tryKeepingHoveringUp = true;
                }

                if (player.velocity.Y == 0f) {
                    player.tryKeepingHoveringUp = false;
                    player.tryKeepingHoveringDown = false;
                }

                if (Player.Settings.HoverControl == Player.Settings.HoverControlMode.Hold) {
                    player.tryKeepingHoveringUp = false;
                    player.tryKeepingHoveringDown = false;
                }
                #endregion

                #region 尝试将本地的玩家的控制字段同步出去
                player.TrySyncingInput();
                #endregion
                #region 当打开背包时, 设置接近的物块和液体 (AdjTiles)
                if (Main.playerInventory)
                    player.AdjTiles();
                #endregion
                #region 处理箱子范围(HandleBeingInChestRange)
                player.HandleBeingInChestRange();
                #endregion
                #region 调用 TileEntity 的 OnPlayerUpdate
                player.tileEntityAnchor.GetTileEntity()?.OnPlayerUpdate(player);
                #endregion
            }
            #endregion
            #region 对于自己的玩家, 摔落相关的处理
            if (playerWai == Main.myPlayer) {
                #region 若速度向上, 则更新摔落起始位置(fallStart2)
                if (player.velocity.Y <= 0f)
                    player.fallStart2 = (int)(player.position.Y / 16f);
                #endregion
                #region 如果摔到了地上 (velocity.Y == 0f)...
                if (player.velocity.Y == 0f) {
                    #region 计算摔伤距离和已下落距离
                    int fallDeltaToHurtInTile = 25;
                    fallDeltaToHurtInTile += player.extraFall;
                    int fallDeltaToCountInTile = (int)(player.position.Y / 16f) - player.fallStart;
                    #endregion
                    #region 如果坐骑能飞或坐矿车在轨道上或 MountID 为 1(兔兔), 则不计摔落距离
                    if (player.mount.CanFly())
                        fallDeltaToCountInTile = 0;

                    if (player.mount.Cart && Minecart.OnTrack(player.position, player.width, player.height))
                        fallDeltaToCountInTile = 0;

                    if (player.mount.Type == 1)
                        fallDeltaToCountInTile = 0;
                    #endregion
                    #region 若玩家脚下是某集中特定方块, 则不计摔落距离
                    if (fallDeltaToCountInTile > 0 || (player.gravDir == -1f && fallDeltaToCountInTile < 0)) {
                        int tileY = (int)((player.position.Y + player.height + 1f) / 16f);
                        if (player.gravDir == -1f)
                            tileY = (int)((player.position.Y - 1f) / 16f);

                        for (int tileX = (int)(player.position.X / 16f); tileX <= (int)((player.position.X + player.width) / 16f); tileX++) {
                            var tile = Main.tile[tileX, tileY];
                            if (tile != null && tile.active() && (tile.type == 189 || tile.type == 196 || tile.type == 460 || tile.type == 666)) {
                                fallDeltaToCountInTile = 0;
                                break;
                            }
                        }
                    }
                    #endregion

                    #region 处理石化时的摔落伤害
                    if (player.stoned) {
                        int fallDamageByStoned = (int)((fallDeltaToCountInTile * player.gravDir - 2f) * 20f);
                        if (fallDamageByStoned > 0) {
                            player.Hurt(PlayerDeathReason.ByOther(5), fallDamageByStoned, 0);
                            player.immune = false;
                        }
                    }
                    #endregion
                    else if (((player.gravDir == 1f && fallDeltaToCountInTile > fallDeltaToHurtInTile) || (player.gravDir == -1f && fallDeltaToCountInTile < -fallDeltaToHurtInTile)) && !player.noFallDmg && player.equippedWings == null) {
                        player.immune = false;
                        int num25 = (int)(fallDeltaToCountInTile * player.gravDir - fallDeltaToHurtInTile) * 10;
                        if (player.mount.Active)
                            num25 = (int)(num25 * player.mount.FallDamage);

                        player.Hurt(PlayerDeathReason.ByOther(0), num25, 0);
                        if (!player.dead && player.statLife <= player.statLifeMax2 / 10)
                            AchievementsHelper.HandleSpecialEvent(player, 8);
                    }

                    player.fallStart = (int)(player.position.Y / 16f);
                }
                #endregion
                #region 在某些条件下更新摔落起始位置(fallStart)
                if (player.jump > 0 || player.rocketDelay > 0 || player.wet || player.slowFall || (double)gravityMultiplier < 0.8 || player.tongued)
                    player.fallStart = (int)(player.position.Y / 16f);
                #endregion
            }
            #endregion
            #region 非多人客户端下的箱子处理 (生成宝箱怪, 瓦斯陷阱, lastChest)
            if (Main.netMode != NetmodeID.MultiplayerClient) {
                if (player.chest == -1 && player.lastChest >= 0 && Main.chest[player.lastChest] != null) {
                    int x = Main.chest[player.lastChest].x;
                    int y2 = Main.chest[player.lastChest].y;
                    NPC.BigMimicSummonCheck(x, y2, player);
                }

                if (player.lastChest != player.chest && player.chest >= 0 && Main.chest[player.chest] != null) {
                    int x2 = Main.chest[player.chest].x;
                    int y3 = Main.chest[player.chest].y;
                    Projectile.GasTrapCheck(x2, y3, player);
                    ItemSlot.forceClearGlowsOnChest = true;
                }

                player.lastChest = player.chest;
            }
            #endregion
            #region 根据 mouseInterface 设置 delayUseItem
            if (player.mouseInterface)
                player.delayUseItem = true;
            #endregion
            #region 设置 Player.tileTarget
            Player.tileTargetX = (int)((Main.mouseX + Main.screenPosition.X) / 16f);
            Player.tileTargetY = (int)((Main.mouseY + Main.screenPosition.Y) / 16f);
            if (player.gravDir == -1f)
                Player.tileTargetY = (int)((Main.screenPosition.Y + Main.screenHeight - Main.mouseY) / 16f);

            if (Player.tileTargetX >= Main.maxTilesX - 5)
                Player.tileTargetX = Main.maxTilesX - 5;

            if (Player.tileTargetY >= Main.maxTilesY - 5)
                Player.tileTargetY = Main.maxTilesY - 5;

            if (Player.tileTargetX < 5)
                Player.tileTargetX = 5;

            if (Player.tileTargetY < 5)
                Player.tileTargetY = 5;

            if (Main.tile[Player.tileTargetX - 1, Player.tileTargetY] == null)
                Main.tile[Player.tileTargetX - 1, Player.tileTargetY] = new Tile();

            if (Main.tile[Player.tileTargetX + 1, Player.tileTargetY] == null)
                Main.tile[Player.tileTargetX + 1, Player.tileTargetY] = new Tile();

            if (Main.tile[Player.tileTargetX, Player.tileTargetY] == null)
                Main.tile[Player.tileTargetX, Player.tileTargetY] = new Tile();

            Item selectedItem = player.inventory[player.selectedItem];
            if (selectedItem.axe > 0 && !Main.tile[Player.tileTargetX, Player.tileTargetY].active() && selectedItem.createWall <= 0 && (selectedItem.hammer <= 0 || selectedItem.axe != 0)) {
                if (Main.tile[Player.tileTargetX - 1, Player.tileTargetY].active() && Main.tile[Player.tileTargetX - 1, Player.tileTargetY].type == 323) {
                    if (Main.tile[Player.tileTargetX - 1, Player.tileTargetY].frameY > 4)
                        Player.tileTargetX--;
                }
                else if (Main.tile[Player.tileTargetX + 1, Player.tileTargetY].active() && Main.tile[Player.tileTargetX + 1, Player.tileTargetY].type == 323 && Main.tile[Player.tileTargetX + 1, Player.tileTargetY].frameY < -4) {
                    Player.tileTargetX++;
                }
            }
            #endregion
            #region 更新自己玩家附近的可交互弹幕(UpdateNearbyInteractibleProjectilesList)
            if (playerWai == Main.myPlayer)
                player.UpdateNearbyInteractibleProjectilesList();
            #endregion
            #region 智能光标
            try {
                if (player.whoAmI == Main.myPlayer && Main.instance.IsActive) {
                    SmartCursorHelper.SmartCursorLookup(player);
                    player.SmartInteractLookup();
                }
            }
            catch {
                Main.SmartCursorWanted_GamePad = false;
                Main.SmartCursorWanted_Mouse = false;
            }
            #endregion
            #region 更新免疫 (UpdateImmunity)
            player.UpdateImmunity();
            #endregion
            #region 一些计时器 (petalTimer, shadowDodgeTimer, boneGloveTimer, crystalLeafCooldown)
            if (player.petalTimer > 0)
                player.petalTimer--;

            if (player.shadowDodgeTimer > 0)
                player.shadowDodgeTimer--;

            if (player.boneGloveTimer > 0)
                player.boneGloveTimer--;

            if (player.crystalLeafCooldown > 0)
                player.crystalLeafCooldown--;
            #endregion
            #region 如果不在地上, 则重置一些在地上的字段
            if (player.jump > 0 || player.velocity.Y != 0f)
                player.ResetFloorFlags();
            #endregion

            #region 点金石效果
            bool oldPStone = player.pStone;
            player.potionDelayTime = Item.potionDelay;
            player.restorationDelayTime = Item.restorationDelay;
            player.mushroomDelayTime = Item.mushroomDelay;
            if (player.pStone) {
                player.potionDelayTime = (int)(player.potionDelayTime * Player.PhilosopherStoneDurationMultiplier);
                player.restorationDelayTime = (int)(player.restorationDelayTime * Player.PhilosopherStoneDurationMultiplier);
                player.mushroomDelayTime = (int)(player.mushroomDelayTime * Player.PhilosopherStoneDurationMultiplier);
            }
            #endregion
            #region Yoraiz0rEye
            if (player.yoraiz0rEye > 0)
                player.Yoraiz0rEye();
            #endregion
            #region 重置诸多字段(ResetEffects), 更新染料(UpdateDyes)
            player.ResetEffects();
            player.UpdateDyes();
            #endregion
            #region 设置无敌模式
            if (CreativePowerManager.Instance.GetPower<CreativePowers.GodmodePower>().IsEnabledForPlayer(player.whoAmI))
                player.creativeGodMode = true;
            #endregion
            #region 设置 afkCounter
            if (player.IsStandingStillForSpecialEffects && player.itemAnimation == 0)
                player.afkCounter++;
            else
                player.afkCounter = 0;
            #endregion
            #region 关于被注释掉的暴击的注释
            // TML:
            // This, right here, is the principal cause of crit chance being a massive pain.
            // By commenting this out, your critical strike chance for the vanilla "three" classes capable of crits will no longer be modified based on your current weapon.
            // This fixes a number of issues related to tooltip crit displays, and while it isn't the primary fix for crit swap, it definitely contributes to it.
            // - Thomas
            /*
			meleeCrit += inventory[selectedItem].crit;
			magicCrit += inventory[selectedItem].crit;
			rangedCrit += inventory[selectedItem].crit;
			*/
            #endregion
            #region 对于自己的玩家, 根据环境中的一些物块给自己加 Buff (篝火, 心灯, 水蜡烛等)
            if (player.whoAmI == Main.myPlayer) {
                Main.musicBox2 = -1;
                if (Main.SceneMetrics.WaterCandleCount > 0)
                    player.AddBuff(86, 2, quiet: false);

                if (Main.SceneMetrics.PeaceCandleCount > 0)
                    player.AddBuff(157, 2, quiet: false);

                if (Main.SceneMetrics.ShadowCandleCount > 0)
                    player.AddBuff(350, 2, quiet: false);

                if (Main.SceneMetrics.HasCampfire)
                    player.AddBuff(87, 2, quiet: false);

                if (Main.SceneMetrics.HasCatBast)
                    player.AddBuff(215, 2, quiet: false);

                if (Main.SceneMetrics.HasStarInBottle)
                    player.AddBuff(158, 2, quiet: false);

                if (Main.SceneMetrics.HasHeartLantern)
                    player.AddBuff(89, 2, quiet: false);

                if (Main.SceneMetrics.HasSunflower)
                    player.AddBuff(146, 2, quiet: false);

                if (Main.SceneMetrics.hasBanner)
                    player.AddBuff(147, 2, quiet: false);

                if (!player.behindBackWall && player.ZoneSandstorm)
                    player.AddBuff(194, 2, quiet: false);
            }
            #endregion

            #region 更新 buff
            #region ModPlayer.PreUpdateBuffs
            PlayerLoader.PreUpdateBuffs(player);
            #endregion
            #region 重置 buff 免疫 (将 buffImmune 数组全部设置为 false)
            for (int num26 = 0; num26 < BuffLoader.BuffCount; num26++) {
                player.buffImmune[num26] = false;
            }
            #endregion
            #region 更新弹幕缓存 (UpdateProjectileCaches, 主要为设置 ownedProjectileCounts), 然后更新 buff
            player.UpdateProjectileCaches(playerWai);
            player.UpdateBuffs(playerWai);
            #endregion
            #region ModPlayer.PostUpdateBuffs
            PlayerLoader.PostUpdateBuffs(player);
            #endregion
            #region 击退 buff 让玩家的击退提升
            // Moved from ItemCheck_OwnerOnlyCode/DashMovement/GetWeaponKnockback
            if (player.kbBuff)
                player.allKB *= 1.5f;
            #endregion
            #endregion
            #region 对于自己的玩家, 更新宠物和宠物光亮 (UpdatePet, UpdatePetLight), 设置 trapDebuffSource 和 isOperatingAnotherEntity
            if (player.whoAmI == Main.myPlayer) {
                if (!player.onFire && !player.poisoned)
                    player.trapDebuffSource = false;

                player.UpdatePet(playerWai);
                player.UpdatePetLight(playerWai);
                player.isOperatingAnotherEntity = player.ownedProjectileCounts[ProjectileID.JimsDrone] > 0; // 1020
            }
            #endregion
            #region 人鱼和狼人相关
            bool wetButNoLavaNoSlimeMount = player.wet && !player.lavaWet && (!player.mount.Active || !player.mount.IsConsideredASlimeMount); // flag13
            if (player.accMerman && wetButNoLavaNoSlimeMount) {
                player.releaseJump = true;
                player.wings = 0;
                player.merman = true;
                player.accFlipper = true;
                player.AddBuff(34, 2);
            }
            else {
                player.merman = false;
            }

            if (!wetButNoLavaNoSlimeMount && player.forceWerewolf)
                player.forceMerman = false;

            if (player.forceMerman && wetButNoLavaNoSlimeMount)
                player.wings = 0;

            player.accMerman = false;
            player.hideMerman = false;
            player.forceMerman = false;
            if (player.wolfAcc && !player.merman && !Main.dayTime && !player.wereWolf)
                player.AddBuff(28, 60);

            player.wolfAcc = false;
            player.hideWolf = false;
            player.forceWerewolf = false;
            #endregion
            #region 若 buffTime <= 0, 删除对应 buff
            if (player.whoAmI == Main.myPlayer) {
                for (int num27 = 0; num27 < Player.maxBuffs; num27++) {
                    if (player.buffType[num27] > 0 && player.buffTime[num27] <= 0)
                        player.DelBuff(num27);
                }
            }
            #endregion
            #region 更新装备
            player.beetleDefense = false;
            player.beetleOffense = false;
            player.setSolar = false;
            player.head = player.armor[0].headSlot;
            player.body = player.armor[1].bodySlot;
            player.legs = player.armor[2].legSlot;
            player.ResetVisibleAccessories();
            if (player.MountFishronSpecialCounter > 0f)
                player.MountFishronSpecialCounter -= 1f;

            if (player._portalPhysicsTime > 0)
                player._portalPhysicsTime--;

            player.UpdateEquips(playerWai);
            if (Main.npcShop <= 0)
                player.discountAvailable = player.discountEquipped;

            if (oldPStone != player.pStone)
                player.AdjustRemainingPotionSickness();
            #endregion

            #region 更新(微光带来的)永久增益 (UpdatePermanentBoosters)
            player.UpdatePermanentBoosters();
            #endregion
            #region 更新运气 (UpdateLuck)
            player.UpdateLuck();
            #endregion
            #region 微光隐身的参数更新
            player.shimmerUnstuckHelper.Update(player);
            #endregion
            #region 更新板凳
            player.UpdatePortableStoolUsage();
            #endregion
            #region 设置一些传送门相关字段 (portalPhysicsFlag, _portalPhysicsTime)
            if (player.velocity.Y == 0f || player.controlJump)
                player.portalPhysicsFlag = false;

            if (player.inventory[player.selectedItem].type == ItemID.PortalGun || player.portalPhysicsFlag)
                player._portalPhysicsTime = 30;
            #endregion
            #region 更新坐骑效果 (mount.UpdateEffects)
            if (player.mount.Active)
                player.mount.UpdateEffects(player);
            #endregion
            #region 大宝石相关字段的设置
            player.gemCount++;
            if (player.gemCount >= 10) {
                player.gem = -1;
                player.ownedLargeGems = 0;
                player.gemCount = 0;
                for (int num28 = 0; num28 <= 58; num28++) {
                    if (player.inventory[num28].type == ItemID.None || player.inventory[num28].stack == 0)
                        player.inventory[num28].TurnToAir();

                    if (player.inventory[num28].type >= ItemID.LargeAmethyst && player.inventory[num28].type <= ItemID.LargeDiamond) {
                        player.gem = player.inventory[num28].type - 1522;
                        player.ownedLargeGems[player.gem] = true;
                    }

                    if (player.inventory[num28].type == ItemID.LargeAmber) {
                        player.gem = 6;
                        player.ownedLargeGems[player.gem] = true;
                    }
                }
            }
            #endregion
            #region 更新盔甲发光, 盔甲套装效果 (UpdateArmorLights, UpdateArmorSets)
            player.UpdateArmorLights();
            player.UpdateArmorSets(playerWai);
            if (player.shadowDodge && !player.onHitDodge)
                player.ClearBuff(59);
            #endregion
            #region ModPlayer.PostUpdateEquips
            //TODO: Move down?
            PlayerLoader.PostUpdateEquips(player);
            #endregion
            #region 更新最大炮台
            if (player.maxTurretsOld != player.maxTurrets) {
                player.UpdateMaxTurrets();
                player.maxTurretsOld = player.maxTurrets;
            }
            #endregion
            #region 如果举盾 (shieldRaised), 则增加 20 防御
            if (player.shieldRaised)
                player.statDefense += 20;
            #endregion
            #region 人鱼状态取消翅膀
            if ((player.merman || player.forceMerman) && wetButNoLavaNoSlimeMount)
                player.wings = 0;
            #endregion
            #region 隐身状态降低仇恨
            if (player.invis) {
                if (player.itemAnimation == 0 && player.aggro > -750)
                    player.aggro = -750;
                else if (player.aggro > -250)
                    player.aggro = -250;
            }
            #endregion
            #region 潜行处理
            #region 手持变态刀时的处理
            if (player.inventory[player.selectedItem].type == ItemID.PsychoKnife) {
                if (player.itemAnimation > 0) {
                    player.stealthTimer = 15;
                    if (player.stealth > 0f)
                        player.stealth += 0.1f;
                }
                else if (player.velocity.X > -0.1 && player.velocity.X < 0.1 && player.velocity.Y > -0.1 && player.velocity.Y < 0.1 && !player.mount.Active) {
                    if (player.stealthTimer == 0 && player.stealth > 0f) {
                        player.stealth -= 0.02f;
                        if (player.stealth <= 0.0) {
                            player.stealth = 0f;
                            if (Main.netMode == NetmodeID.MultiplayerClient)
                                NetMessage.SendData(MessageID.PlayerStealth, -1, -1, null, player.whoAmI);
                        }
                    }
                }
                else {
                    if (player.stealth > 0f)
                        player.stealth += 0.1f;

                    if (player.mount.Active)
                        player.stealth = 1f;
                }

                if (player.stealth > 1f)
                    player.stealth = 1f;

                player.meleeDamage += (1f - player.stealth) * 3f;
                player.meleeCrit += (int)((1f - player.stealth) * 30f);

                // Psycho Knife knockback. Moved from ItemCheck_OwnerOnlyCode/GetWeaponKnockback
                player.GetKnockback(DamageClass.Melee) *= 1f + (1f - player.stealth);

                /*
				if (meleeCrit > 100)
					meleeCrit = 100;
				*/
                player.aggro -= (int)((1f - player.stealth) * 750f);
                if (player.stealthTimer > 0)
                    player.stealthTimer--;
            }
            #endregion
            #region shroomiteStealth 的处理
            else if (player.shroomiteStealth) {
                if (player.itemAnimation > 0)
                    player.stealthTimer = 5;

                if (player.velocity.X > -0.1 && player.velocity.X < 0.1 && player.velocity.Y > -0.1 && player.velocity.Y < 0.1 && !player.mount.Active) {
                    if (player.stealthTimer == 0 && player.stealth > 0f) {
                        player.stealth -= 0.015f;
                        if (player.stealth <= 0.0) {
                            player.stealth = 0f;
                            if (Main.netMode == NetmodeID.MultiplayerClient)
                                NetMessage.SendData(MessageID.PlayerStealth, -1, -1, null, player.whoAmI);
                        }
                    }
                }
                else {
                    float num29 = Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y);
                    player.stealth += num29 * 0.0075f;
                    if (player.stealth > 1f)
                        player.stealth = 1f;

                    if (player.mount.Active)
                        player.stealth = 1f;
                }

                player.rangedDamage += (1f - player.stealth) * 0.6f;
                player.rangedCrit += (int)((1f - player.stealth) * 10f);

                // Stealth knockback. Moved from GetWeaponKnockback
                player.GetKnockback(DamageClass.Ranged) *= 1f + (1f - player.stealth) * 0.5f;

                player.aggro -= (int)((1f - player.stealth) * 750f);
                if (player.stealthTimer > 0)
                    player.stealthTimer--;
            }
            #endregion
            #region 星璇套潜行的处理
            else if (player.setVortex) {
                bool flag14 = false;
                if (player.vortexStealthActive) {
                    float num30 = player.stealth;
                    player.stealth -= 0.04f;
                    if (player.stealth < 0f)
                        player.stealth = 0f;
                    else
                        flag14 = true;

                    if (player.stealth == 0f && num30 != player.stealth && Main.netMode == NetmodeID.MultiplayerClient)
                        NetMessage.SendData(MessageID.PlayerStealth, -1, -1, null, player.whoAmI);

                    player.rangedDamage += (1f - player.stealth) * 0.8f;
                    player.rangedCrit += (int)((1f - player.stealth) * 20f);

                    // Stealth knockback. Moved from GetWeaponKnockback
                    player.GetKnockback(DamageClass.Ranged) *= 1f + (1f - player.stealth) * 0.5f;

                    player.aggro -= (int)((1f - player.stealth) * 1200f);
                    player.accRunSpeed *= 0.3f;
                    player.maxRunSpeed *= 0.3f;
                    if (player.mount.Active)
                        player.vortexStealthActive = false;
                }
                else {
                    float num31 = player.stealth;
                    player.stealth += 0.04f;
                    if (player.stealth > 1f)
                        player.stealth = 1f;
                    else
                        flag14 = true;

                    if (player.stealth == 1f && num31 != player.stealth && Main.netMode == NetmodeID.MultiplayerClient)
                        NetMessage.SendData(MessageID.PlayerStealth, -1, -1, null, player.whoAmI);
                }

                if (flag14) {
                    if (Main.rand.NextBool(2)) {
                        Vector2 vector = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
                        Dust obj2 = Main.dust[Dust.NewDust(player.Center - vector * 30f, 0, 0, DustID.Vortex)];
                        obj2.noGravity = true;
                        obj2.position = player.Center - vector * Main.rand.Next(5, 11);
                        obj2.velocity = vector.RotatedBy(1.5707963705062866) * 4f;
                        obj2.scale = 0.5f + Main.rand.NextFloat();
                        obj2.fadeIn = 0.5f;
                    }

                    if (Main.rand.NextBool(2)) {
                        Vector2 vector2 = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
                        Dust obj3 = Main.dust[Dust.NewDust(player.Center - vector2 * 30f, 0, 0, DustID.Granite)];
                        obj3.noGravity = true;
                        obj3.position = player.Center - vector2 * 12f;
                        obj3.velocity = vector2.RotatedBy(-1.5707963705062866) * 2f;
                        obj3.scale = 0.5f + Main.rand.NextFloat();
                        obj3.fadeIn = 0.5f;
                    }
                }
            }
            #endregion
            #region 如果上述潜行都没有则重置潜行值 (stealth = 1)
            else {
                player.stealth = 1f;
            }
            #endregion
            #endregion
            #region 魔力病处理 (减少魔法伤害)
            if (player.manaSick)
                player.magicDamage *= 1f - player.manaSickReduction;
            #endregion
            #region 注释: 攻速调整现在 Player.GetWeaponAttackSpeed 中
            // Attack speed multipliers now applied in Player.GetWeaponAttackSpeed.
            /*
			float num32 = meleeSpeed - 1f;
			num32 *= ItemID.Sets.BonusMeleeSpeedMultiplier[inventory[selectedItem].type];
			meleeSpeed = 1f + num32;
			*/
            #endregion
            #region 设置 tileSpeed 和 wallSpeed
            if (player.tileSpeed > 3f)
                player.tileSpeed = 3f;

            player.tileSpeed = 1f / player.tileSpeed;
            if (player.wallSpeed > 3f)
                player.wallSpeed = 3f;

            player.wallSpeed = 1f / player.wallSpeed;
            #endregion
            #region 注释: 取消对最大魔力值为 400 的限制, 不用在此限制 statDefense 不小于 0, 它在 DefenseStat 处被限制了
            // Allow mana stat to exceed vanilla bounds (#HealthManaAPI)
            /*
			if (statManaMax2 > 400)
				statManaMax2 = 400;
			*/

            // positive value capping built-in to DefenseStat
            /*
			if (statDefense < 0)
				statDefense = 0;
			*/
            #endregion
            #region 根据一些字段限制水平速度 (slowOgreSpit, dazed, slow, chilled, shieldRaised  -> moveSpeed, velocity.X)
            if (player.slowOgreSpit) { // 食人魔唾液
                player.moveSpeed /= 3f;
                if (player.velocity.Y == 0f && Math.Abs(player.velocity.X) > 1f)
                    player.velocity.X /= 2f;
            }
            else if (player.dazed) {
                player.moveSpeed /= 3f;
            }
            else if (player.slow) {
                player.moveSpeed /= 2f;
            }
            else if (player.chilled) {
                player.moveSpeed *= 0.75f;
            }

            if (player.shieldRaised) {
                player.moveSpeed /= 3f;
                if (player.velocity.Y == 0f && Math.Abs(player.velocity.X) > 3f)
                    player.velocity.X /= 2f;
            }
            #endregion
            #region 天国入侵时添加不能建造的 debuff
            if (DD2Event.Ongoing) {
                DD2Event.FindArenaHitbox();
                if (DD2Event.ShouldBlockBuilding(player.Center)) {
                    player.noBuilding = true;
                    player.AddBuff(199, 3);
                }
            }
            #endregion
            #region 限制 pickSpeed 最小为 0.3
            if (player.pickSpeed < 0.3)
                player.pickSpeed = 0.3f;
            #endregion
            #region 限制攻速, 但实际上什么都没做
            player.CapAttackSpeeds();
            #endregion
            #region ModPlayer.PostUpdateMiscEffects
            PlayerLoader.PostUpdateMiscEffects(player);
            #endregion

            #region 更新生命回复和魔力回复
            player.UpdateLifeRegen();
            player.soulDrain = 0;
            player.UpdateManaRegen();
            if (player.manaRegenCount < 0)
                player.manaRegenCount = 0;

            if (player.statMana > player.statManaMax2)
                player.statMana = player.statManaMax2;
            #endregion
            #region 设置一些运动相关的字段 (runAcceleration, maxRunSpeed, jumpHeight, jumpSpeed)
            player.runAcceleration *= player.moveSpeed;
            player.maxRunSpeed *= player.moveSpeed;
            player.UpdateJumpHeight();
            #endregion
            #region 根据 buff 免疫去除对应的 buff
            for (int num33 = 0; num33 < Player.maxBuffs; num33++) {
                if (player.buffType[num33] > 0 && player.buffTime[num33] > 0 && player.buffImmune[player.buffType[num33]])
                    player.DelBuff(num33);
            }
            #endregion
            #region 一些 debuff 效果 (防御减半, 伤害减半)
            if (player.brokenArmor)
                player.statDefense /= 2;

            if (player.witheredArmor)
                player.statDefense /= 2;

            if (player.witheredWeapon) {
                player.allDamage *= 0.5f;
                /*
				meleeDamage *= 0.5f;
				rangedDamage *= 0.5f;
				magicDamage *= 0.5f;
				minionDamage *= 0.5f;
				rangedMultDamage *= 0.5f;
				*/
            }
            #endregion
            #region 设置 lastTileRange
            player.lastTileRangeX = Player.tileRangeX;
            player.lastTileRangeY = Player.tileRangeY;
            #endregion
            #region 根据坐骑状态转移 movementAbilitiesCache
            if (player.mount.Active)
                player.movementAbilitiesCache.CopyFrom(player);
            else
                player.movementAbilitiesCache.PasteInto(player);
            #endregion
            #region 多段跳处理
            if (player.mount.Active && player.mount.BlockExtraJumps) {
                player.ConsumeAllExtraJumps();
                /*
				canJumpAgain_Cloud = false;
				canJumpAgain_Sandstorm = false;
				canJumpAgain_Blizzard = false;
				canJumpAgain_Fart = false;
				canJumpAgain_Sail = false;
				canJumpAgain_Unicorn = false;
				canJumpAgain_Santank = false;
				canJumpAgain_WallOfFleshGoat = false;
				canJumpAgain_Basilisk = false;
				*/
            }
            else if (player.velocity.Y == 0f || player.sliding) {
                player.RefreshDoubleJumps();
            }
            else {
                ExtraJumpLoader.ConsumeAndStopUnavailableJumps(player);
                /*
				if (!hasJumpOption_Cloud)
					canJumpAgain_Cloud = false;

				if (!hasJumpOption_Sandstorm)
					canJumpAgain_Sandstorm = false;

				if (!hasJumpOption_Blizzard)
					canJumpAgain_Blizzard = false;

				if (!hasJumpOption_Fart)
					canJumpAgain_Fart = false;

				if (!hasJumpOption_Sail)
					canJumpAgain_Sail = false;

				if (!hasJumpOption_Unicorn)
					canJumpAgain_Unicorn = false;

				if (!hasJumpOption_Santank)
					canJumpAgain_Santank = false;

				if (!hasJumpOption_WallOfFleshGoat)
					canJumpAgain_WallOfFleshGoat = false;

				if (!hasJumpOption_Basilisk)
					canJumpAgain_Basilisk = false;
				*/
            }
            #endregion
            #region 飞毯相关的字段设置
            if (!player.carpet) {
                player.canCarpet = false;
                player.carpetFrame = -1;
            }
            else if (player.velocity.Y == 0f || player.sliding) {
                player.canCarpet = true;
                player.carpetTime = 0;
                player.carpetFrame = -1;
                player.carpetFrameCounter = 0f;
            }

            if (player.gravDir == -1f)
                player.canCarpet = false;
            #endregion
            #region ropeCount--, 不小于 0
            if (player.ropeCount > 0)
                player.ropeCount--;
            #endregion
            #region 尝试滑索 (爬绳子)
            if (!player.pulley && !player.frozen && !player.webbed && !player.stoned && !player.controlJump && player.gravDir == 1f && player.ropeCount == 0 && player.grappling[0] == -1 && !player.tongued && !player.mount.Active)
                player.FindPulley();
            #endregion

            #region 更新正在拍的宠物
            player.UpdatePettingAnimal();
            #endregion
            #region 更新坐姿, 睡眠和眼睛 (sitting, sleeping, eyeHelper)
            player.sitting.UpdateSitting(player);
            player.sleeping.UpdateState(player);
            player.eyeHelper.Update(player);
            #endregion
            #region 滑索状态的更新其一...
            if (player.pulley) {
                if (player.mount.Active)
                    player.pulley = false;

                player.sandStorm = false;
                player.CancelAllJumpVisualEffects();
                int num34 = (int)(player.position.X + (player.width / 2)) / 16;
                int num35 = (int)(player.position.Y - 8f) / 16;
                bool flag15 = false;
                if (player.pulleyDir == 0)
                    player.pulleyDir = 1;

                if (player.pulleyDir == 1) {
                    if (player.direction == -1 && player.controlLeft && (player.releaseLeft || player.leftTimer == 0)) {
                        player.pulleyDir = 2;
                        flag15 = true;
                    }
                    else if ((player.direction == 1 && player.controlRight && player.releaseRight) || player.rightTimer == 0) {
                        player.pulleyDir = 2;
                        flag15 = true;
                    }
                    else {
                        if (player.direction == 1 && player.controlLeft) {
                            player.direction = -1;
                            flag15 = true;
                        }

                        if (player.direction == -1 && player.controlRight) {
                            player.direction = 1;
                            flag15 = true;
                        }
                    }
                }
                else if (player.pulleyDir == 2) {
                    if (player.direction == 1 && player.controlLeft) {
                        flag15 = true;
                        if (!Collision.SolidCollision(new Vector2(num34 * 16 + 8 - player.width / 2, player.position.Y), player.width, player.height)) {
                            player.pulleyDir = 1;
                            player.direction = -1;
                            flag15 = true;
                        }
                    }

                    if (player.direction == -1 && player.controlRight) {
                        flag15 = true;
                        if (!Collision.SolidCollision(new Vector2(num34 * 16 + 8 - player.width / 2, player.position.Y), player.width, player.height)) {
                            player.pulleyDir = 1;
                            player.direction = 1;
                            flag15 = true;
                        }
                    }
                }

                int num36 = 1;
                if (player.controlLeft)
                    num36 = -1;

                bool flag16 = player.CanMoveForwardOnRope(num36, num34, num35);
                if (player.controlLeft && player.direction == -1 && flag16)
                    player.instantMovementAccumulatedThisFrame.X += -1f;

                if (player.controlRight && player.direction == 1 && flag16)
                    player.instantMovementAccumulatedThisFrame.X += 1f;

                bool flag17 = false;
                if (!flag15 && ((player.controlLeft && (player.releaseLeft || player.leftTimer == 0)) || (player.controlRight && (player.releaseRight || player.rightTimer == 0)))) {
                    int num37 = num34 + num36;
                    if (WorldGen.IsRope(num37, num35)) {
                        player.pulleyDir = 1;
                        player.direction = num36;
                        int num38 = num37 * 16 + 8 - player.width / 2;
                        float y4 = player.position.Y;
                        y4 = num35 * 16 + 22;
                        if (Main.tile[num37, num35 - 1] == null)
                            Main.tile[num37, num35 - 1] = new Tile();

                        if (Main.tile[num37, num35 + 1] == null)
                            Main.tile[num37, num35 + 1] = new Tile();

                        if (WorldGen.IsRope(num37, num35 - 1) || WorldGen.IsRope(num37, num35 + 1))
                            y4 = num35 * 16 + 22;

                        if (Collision.SolidCollision(new Vector2(num38, y4), player.width, player.height)) {
                            player.pulleyDir = 2;
                            player.direction = -num36;
                            num38 = ((player.direction != 1) ? (num37 * 16 + 8 - player.width / 2 + -6) : (num37 * 16 + 8 - player.width / 2 + 6));
                        }

                        if (playerWai == Main.myPlayer)
                            Main.cameraX = Main.cameraX + player.position.X - num38;

                        player.position.X = num38;
                        player.gfxOffY = player.position.Y - y4;
                        player.position.Y = y4;
                        flag17 = true;
                    }
                }

                if (!flag17 && !flag15 && !player.controlUp && ((player.controlLeft && player.releaseLeft) || (player.controlRight && player.releaseRight))) {
                    player.pulley = false;
                    if (player.controlLeft && player.velocity.X == 0f)
                        player.velocity.X = -1f;

                    if (player.controlRight && player.velocity.X == 0f)
                        player.velocity.X = 1f;
                }

                if (player.velocity.X != 0f)
                    player.pulley = false;

                if (Main.tile[num34, num35] == null)
                    Main.tile[num34, num35] = new Tile();

                if (!WorldGen.IsRope(num34, num35))
                    player.pulley = false;

                if (player.gravDir != 1f)
                    player.pulley = false;

                if (player.frozen || player.webbed || player.stoned)
                    player.pulley = false;

                if (!player.pulley)
                    player.velocity.Y -= player.gravity;

                if (player.controlJump) {
                    player.pulley = false;
                    player.jump = Player.jumpHeight;
                    player.velocity.Y = 0f - Player.jumpSpeed;
                }
            }
            #endregion
            #region 如果 grapCount > 0, 则取消滑索状态
            if (player.grapCount > 0)
                player.pulley = false;
            #endregion
            #region 重力之脑影响重力
            if (NPC.brainOfGravity >= 0 && NPC.brainOfGravity < 200 && Vector2.Distance(player.Center, Main.npc[NPC.brainOfGravity].Center) < 4000f)
                player.forcedGravity = 10;

            if (player.forcedGravity > 0)
                player.gravDir = -1f;
            #endregion
            #region 滑索状态更新其二
            if (player.pulley) {
                player.fallStart = (int)player.position.Y / 16;
                player.wingFrame = 0;
                if (player.wings == 4)
                    player.wingFrame = 3;

                int num39 = (int)(player.position.X + (player.width / 2)) / 16;
                int num40 = (int)(player.position.Y - 16f) / 16;
                int num41 = (int)(player.position.Y - 8f) / 16;
                bool flag18 = true;
                bool flag19 = false;
                if (WorldGen.IsRope(num39, num41 - 1) || WorldGen.IsRope(num39, num41 + 1))
                    flag19 = true;

                if (Main.tile[num39, num40] == null)
                    Main.tile[num39, num40] = new Tile();

                if (!WorldGen.IsRope(num39, num40)) {
                    flag18 = false;
                    if (player.velocity.Y < 0f)
                        player.velocity.Y = 0f;
                }

                if (flag19) {
                    if (player.controlUp && flag18) {
                        float x3 = player.position.X;
                        float y5 = player.position.Y - Math.Abs(player.velocity.Y) - 2f;
                        if (Collision.SolidCollision(new Vector2(x3, y5), player.width, player.height)) {
                            x3 = num39 * 16 + 8 - player.width / 2 + 6;
                            if (!Collision.SolidCollision(new Vector2(x3, y5), player.width, (int)(player.height + Math.Abs(player.velocity.Y) + 2f))) {
                                if (playerWai == Main.myPlayer)
                                    Main.cameraX = Main.cameraX + player.position.X - x3;

                                player.pulleyDir = 2;
                                player.direction = 1;
                                player.position.X = x3;
                                player.velocity.X = 0f;
                            }
                            else {
                                x3 = num39 * 16 + 8 - player.width / 2 + -6;
                                if (!Collision.SolidCollision(new Vector2(x3, y5), player.width, (int)(player.height + Math.Abs(player.velocity.Y) + 2f))) {
                                    if (playerWai == Main.myPlayer)
                                        Main.cameraX = Main.cameraX + player.position.X - x3;

                                    player.pulleyDir = 2;
                                    player.direction = -1;
                                    player.position.X = x3;
                                    player.velocity.X = 0f;
                                }
                            }
                        }

                        if (player.velocity.Y > 0f)
                            player.velocity.Y *= 0.7f;

                        if (player.velocity.Y > -3f)
                            player.velocity.Y -= 0.2f;
                        else
                            player.velocity.Y -= 0.02f;

                        if (player.velocity.Y < -8f)
                            player.velocity.Y = -8f;
                    }
                    else if (player.controlDown) {
                        float x4 = player.position.X;
                        float y6 = player.position.Y;
                        if (Collision.SolidCollision(new Vector2(x4, y6), player.width, (int)(player.height + Math.Abs(player.velocity.Y) + 2f))) {
                            x4 = num39 * 16 + 8 - player.width / 2 + 6;
                            if (!Collision.SolidCollision(new Vector2(x4, y6), player.width, (int)(player.height + Math.Abs(player.velocity.Y) + 2f))) {
                                if (playerWai == Main.myPlayer)
                                    Main.cameraX = Main.cameraX + player.position.X - x4;

                                player.pulleyDir = 2;
                                player.direction = 1;
                                player.position.X = x4;
                                player.velocity.X = 0f;
                            }
                            else {
                                x4 = num39 * 16 + 8 - player.width / 2 + -6;
                                if (!Collision.SolidCollision(new Vector2(x4, y6), player.width, (int)(player.height + Math.Abs(player.velocity.Y) + 2f))) {
                                    if (playerWai == Main.myPlayer)
                                        Main.cameraX = Main.cameraX + player.position.X - x4;

                                    player.pulleyDir = 2;
                                    player.direction = -1;
                                    player.position.X = x4;
                                    player.velocity.X = 0f;
                                }
                            }
                        }

                        if (player.velocity.Y < 0f)
                            player.velocity.Y *= 0.7f;

                        if (player.velocity.Y < 3f)
                            player.velocity.Y += 0.2f;
                        else
                            player.velocity.Y += 0.1f;

                        if (player.velocity.Y > player.maxFallSpeed)
                            player.velocity.Y = player.maxFallSpeed;
                    }
                    else {
                        player.velocity.Y *= 0.7f;
                        if (player.velocity.Y > -0.1 && player.velocity.Y < 0.1)
                            player.velocity.Y = 0f;
                    }
                }
                else if (player.controlDown) {
                    player.ropeCount = 10;
                    player.pulley = false;
                    player.velocity.Y = 1f;
                }
                else {
                    player.velocity.Y = 0f;
                    player.position.Y = num40 * 16 + 22;
                }

                float num42 = num39 * 16 + 8 - player.width / 2;
                if (player.pulleyDir == 1)
                    num42 = num39 * 16 + 8 - player.width / 2;

                if (player.pulleyDir == 2)
                    num42 = num39 * 16 + 8 - player.width / 2 + 6 * player.direction;

                if (playerWai == Main.myPlayer) {
                    Main.cameraX += player.position.X - num42;
                    Main.cameraX = MathHelper.Clamp(Main.cameraX, -32f, 32f);
                }

                player.position.X = num42;
                player.pulleyFrameCounter += Math.Abs(player.velocity.Y * 0.75f);
                if (player.velocity.Y != 0f)
                    player.pulleyFrameCounter += 0.75f;

                if (player.pulleyFrameCounter > 10f) {
                    player.pulleyFrame++;
                    player.pulleyFrameCounter = 0f;
                }

                if (player.pulleyFrame > 1)
                    player.pulleyFrame = 0;

                player.canCarpet = true;
                player.carpetFrame = -1;
                player.wingTime = player.wingTimeMax;
                player.rocketTime = player.rocketTimeMax;
                player.rocketDelay = 0;
                player.rocketFrame = false;
                player.canRocket = false;
                player.rocketRelease = false;
                player.DashMovement();
                player.UpdateControlHolds();
            }
            #endregion
            #region 一般情况下 (非滑索非抓钩状态下)...
            else if (player.grappling[0] == -1 && !player.tongued) {
                #region 更新跑步速度 (翅膀, 一些字段, 多段跳, 坐骑)
                #region 翅膀的空中运动逻辑
                if (player.wingsLogic > 0 && player.velocity.Y != 0f && !player.merman && !player.mount.Active)
                    player.WingAirLogicTweaks();
                #endregion
                #region 根据一些字段设置跑步速度
                if (player.empressBrooch)
                    player.runAcceleration *= 1.75f;

                if (player.hasMagiluminescence && player.velocity.Y == 0f) {
                    player.runAcceleration *= 1.75f;
                    player.maxRunSpeed *= 1.15f;
                    player.accRunSpeed *= 1.15f;
                    player.runSlowdown *= 1.75f;
                }

                if (player.shadowArmor) {
                    player.runAcceleration *= 1.75f;
                    player.maxRunSpeed *= 1.15f;
                    player.accRunSpeed *= 1.15f;
                    player.runSlowdown *= 1.75f;
                }

                if (player.mount.Active && player.mount.Type == 43 && player.velocity.Y != 0f)
                    player.runSlowdown = 0f;

                if (player.sticky) {
                    player.maxRunSpeed *= 0.25f;
                    player.runAcceleration *= 0.25f;
                    player.runSlowdown *= 2f;
                    if (player.velocity.X > player.maxRunSpeed)
                        player.velocity.X = player.maxRunSpeed;

                    if (player.velocity.X < 0f - player.maxRunSpeed)
                        player.velocity.X = 0f - player.maxRunSpeed;
                }
                else if (player.powerrun) {
                    player.maxRunSpeed *= 3.5f;
                    player.runAcceleration *= 1f;
                    player.runSlowdown *= 2f;
                }
                else if (player.runningOnSand && player.desertBoots) {
                    player.maxRunSpeed *= 1.75f;
                    player.accRunSpeed *= 1.75f;
                    player.runAcceleration *= 1.75f;
                    player.runSlowdown *= 1.75f;
                }
                else if (player.slippy2) {
                    player.runAcceleration *= 0.6f;
                    player.runSlowdown = 0f;
                    if (player.iceSkate) {
                        player.runAcceleration *= 3.5f;
                        player.maxRunSpeed *= 1.25f;
                    }
                }
                else if (player.slippy) {
                    player.runAcceleration *= 0.7f;
                    if (player.iceSkate) {
                        player.runAcceleration *= 3.5f;
                        player.maxRunSpeed *= 1.25f;
                    }
                    else {
                        player.runSlowdown *= 0.1f;
                    }
                }

                /*
				if (sandStorm) {
					runAcceleration *= 1.5f;
					maxRunSpeed *= 2f;
				}

				if (isPerformingJump_Blizzard && hasJumpOption_Blizzard) {
					runAcceleration *= 3f;
					maxRunSpeed *= 1.5f;
				}

				if (isPerformingJump_Fart && hasJumpOption_Fart) {
					runAcceleration *= 3f;
					maxRunSpeed *= 1.75f;
				}

				if (isPerformingJump_Unicorn && hasJumpOption_Unicorn) {
					runAcceleration *= 3f;
					maxRunSpeed *= 1.5f;
				}

				if (isPerformingJump_Santank && hasJumpOption_Santank) {
					runAcceleration *= 3f;
					maxRunSpeed *= 1.5f;
				}

				if (isPerformingJump_WallOfFleshGoat && hasJumpOption_WallOfFleshGoat) {
					runAcceleration *= 3f;
					maxRunSpeed *= 1.5f;
				}

				if (isPerformingJump_Basilisk && hasJumpOption_Basilisk) {
					runAcceleration *= 3f;
					maxRunSpeed *= 1.5f;
				}

				if (isPerformingJump_Sail && hasJumpOption_Sail) {
					runAcceleration *= 1.5f;
					maxRunSpeed *= 1.25f;
				}
				*/
                #endregion
                #region 根据多段跳设置跑步速度
                ExtraJumpLoader.UpdateHorizontalSpeeds(player);
                #endregion
                #region 根据 carpetFrame 和变态刀设置跑步速度
                if (player.carpetFrame != -1) {
                    player.runAcceleration *= 1.25f;
                    player.maxRunSpeed *= 1.5f;
                }

                if (player.inventory[player.selectedItem].type == ItemID.PsychoKnife && player.stealth < 1f) {
                    float num44 = player.maxRunSpeed / 2f * (1f - player.stealth);
                    player.maxRunSpeed -= num44;
                    player.accRunSpeed = player.maxRunSpeed;
                }
                #endregion
                #region 坐骑的一些更新
                if (player.mount.Active) {
                    player.rocketBoots = 0;
                    player.vanityRocketBoots = 0;
                    player.wings = 0;
                    player.wingsLogic = 0;
                    player.maxRunSpeed = player.mount.RunSpeed;
                    player.accRunSpeed = player.mount.DashSpeed;
                    player.runAcceleration = player.mount.Acceleration;
                    if (player.mount.Type == 12 && !player.MountFishronSpecial) {
                        player.runAcceleration /= 2f;
                        player.maxRunSpeed /= 2f;
                    }

                    player.mount.AbilityRecovery();
                    if (player.mount.Cart && player.velocity.Y == 0f) {
                        if (!Minecart.OnTrack(player.position, player.width, player.height)) {
                            player.fullRotation = 0f;
                            player.onWrongGround = true;
                            player.runSlowdown = 0.2f;
                            if ((player.controlLeft && player.releaseLeft) || (player.controlRight && player.releaseRight))
                                player.mount.Dismount(player);
                        }
                        else {
                            player.runSlowdown = player.runAcceleration;
                            player.onWrongGround = false;
                        }
                    }

                    if (player.mount.Type == 8)
                        player.mount.UpdateDrill(player, player.controlUp, player.controlDown);
                }
                #endregion
                #endregion
                #region ModPlayer.PostUpdateRunSpeeds
                PlayerLoader.PostUpdateRunSpeeds(player);
                #endregion
                #region 水平移动 (HorizontalMovement)
                player.HorizontalMovement();
                #endregion
                #region 更新重力方向 (gravDir)
                bool mountNotActive = !player.mount.Active; // flag20
                if (player.forcedGravity > 0) {
                    player.gravDir = -1f;
                }
                else if ((player.gravControl || player.gravControl2) && mountNotActive) {
                    if (player.controlUp && player.releaseUp) {
                        player.gravDir *= -1f;
                        player.fallStart = (int)(player.position.Y / 16f);
                        player.jump = 0;
                        SoundEngine.PlaySound(SoundID.Item8, player.position);
                    }
                }
                else {
                    player.gravDir = 1f;
                }
                #endregion
                #region y 速度为 0, 在坐骑状态且 mount.CanHover() 且刚按下 上键 时更新 y 速度
                if (player.velocity.Y == 0f && player.mount.Active && player.mount.CanHover() && player.controlUp && player.releaseUp)
                    player.velocity.Y = 0f - (player.mount.Acceleration + player.gravity + 0.001f);
                #endregion
                #region UpdateControlHolds
                player.UpdateControlHolds();
                #endregion
                #region 重置 sandStorm
                player.sandStorm = false;
                #endregion
                #region 跳跃 (JumpMovement)
                player.JumpMovement();
                #endregion
                #region 没有翅膀则取消翅膀时间, 没有火箭靴则取消火箭时间, 不在跳跃则取消跳跃的视觉效果 (CancelAllJumpVisualEffects)
                if (player.wingsLogic == 0)
                    player.wingTime = 0f;

                if (player.rocketBoots == 0)
                    player.rocketTime = 0;

                if (player.jump == 0)
                    player.CancelAllJumpVisualEffects();
                #endregion
                #region 冲刺 (DashMovement)
                player.DashMovement();
                #endregion
                #region 爬墙 (WallslideMovement)
                player.WallslideMovement();
                #endregion
                #region 飞毯 (CarpetMovement)
                player.CarpetMovement();
                #endregion
                #region 多段跳视效 (DoubleJumpVisuals)
                player.DoubleJumpVisuals();
                #endregion
                #region 如果在用翅膀或用坐骑, 取消沙尘暴
                if (player.wingsLogic > 0 || player.mount.Active)
                    player.sandStorm = false;
                #endregion
                #region 设置是否能用火箭靴 (canRocket)
                if (((player.gravDir == 1f && player.velocity.Y > 0f - Player.jumpSpeed) || (player.gravDir == -1f && player.velocity.Y < Player.jumpSpeed)) && player.velocity.Y != 0f)
                    player.canRocket = true;
                #endregion
                #region 尝试回复翅膀时间到最大
                if (((player.velocity.Y == 0f || player.sliding) && player.releaseJump) || (player.autoJump && player.justJumped)) {
                    player.mount.ResetFlightTime(player.velocity.X);
                    player.wingTime = player.wingTimeMax;
                }
                #endregion
                #region 计算翅膀是否在使用
                bool wingInUse = false; // flag21
                if (player.wingsLogic > 0 && player.controlJump && player.wingTime > 0f && player.jump == 0 && player.velocity.Y != 0f)
                    wingInUse = true;

                if ((player.wingsLogic == 22 || player.wingsLogic == 28 || player.wingsLogic == 30 || player.wingsLogic == 32 || player.wingsLogic == 29 || player.wingsLogic == 33 || player.wingsLogic == 35 || player.wingsLogic == 37 || player.wingsLogic == 45) && player.controlJump && player.TryingToHoverDown && player.wingTime > 0f)
                    wingInUse = true;
                #endregion
                #region 如果被冻住, 网住或石化, 则只执行基本逻辑 (取消坐骑, 按重力下降(不超过最大下降速度), 取消沙尘暴, 取消跳跃视效)
                if (player.frozen || player.webbed || player.stoned) {
                    if (player.mount.Active)
                        player.mount.Dismount(player);

                    player.velocity.Y += player.gravity;
                    if (player.velocity.Y > player.maxFallSpeed)
                        player.velocity.Y = player.maxFallSpeed;

                    player.sandStorm = false;
                    player.CancelAllJumpVisualEffects();
                }
                #endregion
                #region 一般情况下(不是被冻住, 网住或石化)...
                else {
                    #region 翅膀更新与视效
                    bool isCustomWings = ItemLoader.WingUpdate(player, wingInUse);
                    if (wingInUse) {
                        player.WingAirVisuals();
                        player.WingMovement();
                    }
                    player.WingFrame(wingInUse, isCustomWings);
                    #endregion
                    #region 按照火箭靴时间增加翅膀时间
                    if (player.wingsLogic > 0 && player.rocketBoots != 0 && player.velocity.Y != 0f && player.rocketTime != 0) {
                        int num45 = 6;
                        int num46 = player.rocketTime * num45;
                        player.wingTime += num46;
                        if (player.wingTime > player.wingTimeMax + num46)
                            player.wingTime = player.wingTimeMax + num46;

                        player.rocketTime = 0;
                    }
                    #endregion
                    #region 尝试发出翅膀拍击声音
                    if (isCustomWings) { }
                    else
                    if (wingInUse && player.wings != 4 && player.wings != 22 && player.wings != 0 && player.wings != 24 && player.wings != 28 && player.wings != 30 && player.wings != 33 && player.wings != 45) {
                        bool flag22 = player.wingFrame == 3;
                        if (player.wings == 43 || player.wings == 44)
                            flag22 = player.wingFrame == 4;

                        if (flag22) {
                            if (!player.flapSound)
                                SoundEngine.PlaySound(SoundID.Item32, player.position);

                            player.flapSound = true;
                        }
                        else {
                            player.flapSound = false;
                        }
                    }
                    #endregion
                    #region 尝试回复火箭靴时间到最大
                    if (player.velocity.Y == 0f || player.sliding || (player.autoJump && player.justJumped))
                        player.rocketTime = player.rocketTimeMax;

                    if (player.empressBrooch)
                        player.rocketTime = player.rocketTimeMax;
                    #endregion
                    #region 火箭靴相关
                    if ((player.wingTime == 0f || player.wingsLogic == 0) && player.rocketBoots != 0 && player.controlJump && player.rocketDelay == 0 && player.canRocket && player.rocketRelease && !player.AnyExtraJumpUsable()) {
                        if (player.rocketTime > 0) {
                            player.rocketTime--;
                            player.rocketDelay = 10;
                            if (player.rocketDelay2 <= 0) {
                                if (player.rocketBoots == 1)
                                    player.rocketDelay2 = 30;
                                else if (player.rocketBoots == 2 || player.rocketBoots == 5 || player.rocketBoots == 3 || player.rocketBoots == 4)
                                    player.rocketDelay2 = 15;
                            }

                            if (player.rocketSoundDelay <= 0) {
                                if (player.vanityRocketBoots == 1 || player.vanityRocketBoots == 5) {
                                    player.rocketSoundDelay = 30;
                                    SoundEngine.PlaySound(SoundID.Item13, player.position);
                                }
                                else if (player.vanityRocketBoots == 2 || player.vanityRocketBoots == 3 || player.vanityRocketBoots == 4) {
                                    player.rocketSoundDelay = 15;
                                    SoundEngine.PlaySound(SoundID.Item24, player.position);
                                }
                            }
                        }
                        else {
                            player.canRocket = false;
                        }
                    }

                    if (player.rocketSoundDelay > 0)
                        player.rocketSoundDelay--;

                    if (player.rocketDelay2 > 0)
                        player.rocketDelay2--;

                    if (player.rocketDelay == 0)
                        player.rocketFrame = false;

                    if (player.rocketDelay > 0) {
                        player.rocketFrame = true;
                        player.RocketBootVisuals();
                        if (player.rocketDelay == 0)
                            player.releaseJump = true;

                        player.rocketDelay--;
                        player.velocity.Y -= 0.1f * player.gravDir;
                        if (player.gravDir == 1f) {
                            if (player.velocity.Y > 0f)
                                player.velocity.Y -= 0.5f;
                            else if (player.velocity.Y > (double)(0f - Player.jumpSpeed) * 0.5)
                                player.velocity.Y -= 0.1f;

                            if (player.velocity.Y < (0f - Player.jumpSpeed) * 1.5f)
                                player.velocity.Y = (0f - Player.jumpSpeed) * 1.5f;
                        }
                        else {
                            if (player.velocity.Y < 0f)
                                player.velocity.Y += 0.5f;
                            else if (player.velocity.Y < Player.jumpSpeed * 0.5)
                                player.velocity.Y += 0.1f;

                            if (player.velocity.Y > Player.jumpSpeed * 1.5f)
                                player.velocity.Y = Player.jumpSpeed * 1.5f;
                        }
                    }
                    #endregion
                    #region 一般情况下 (没在火箭靴且没在用翅膀)...
                    else if (!wingInUse) {
                        #region 尝试坐骑悬停
                        if (player.mount.CanHover()) {
                            player.mount.Hover(player);
                        }
                        #endregion
                        #region 尝试坐骑飞行
                        else if (player.mount.CanFly() && player.controlJump && player.jump == 0) {
                            if (player.mount.Flight()) {
                                if (player.TryingToHoverDown) {
                                    player.velocity.Y *= 0.9f;
                                    if (player.velocity.Y > -1f && player.velocity.Y < 0.5)
                                        player.velocity.Y = 1E-05f;
                                }
                                else {
                                    float num47 = Player.jumpSpeed;
                                    if (player.mount.Type == 50)
                                        num47 *= 0.5f;

                                    if (player.velocity.Y > 0f)
                                        player.velocity.Y -= 0.5f;
                                    else if (player.velocity.Y > (double)(0f - num47) * 1.5)
                                        player.velocity.Y -= 0.1f;

                                    if (player.velocity.Y < (0f - num47) * 1.5f)
                                        player.velocity.Y = (0f - num47) * 1.5f;
                                }
                            }
                            else {
                                player.velocity.Y += player.gravity / 3f * player.gravDir;
                                if (player.gravDir == 1f) {
                                    if (player.velocity.Y > player.maxFallSpeed / 3f && !player.TryingToHoverDown)
                                        player.velocity.Y = player.maxFallSpeed / 3f;
                                }
                                else if (player.velocity.Y < (0f - player.maxFallSpeed) / 3f && !player.TryingToHoverUp) {
                                    player.velocity.Y = (0f - player.maxFallSpeed) / 3f;
                                }
                            }
                        }
                        #endregion
                        #region 尝试缓降
                        else if (player.slowFall && !player.TryingToHoverDown) {
                            if (player.TryingToHoverUp)
                                player.gravity = player.gravity / 10f * player.gravDir;
                            else
                                player.gravity = player.gravity / 3f * player.gravDir;

                            player.velocity.Y += player.gravity;
                        }
                        #endregion
                        #region 尝试翅膀飞行
                        else if (player.wingsLogic > 0 && player.controlJump && player.velocity.Y > 0f) {
                            bool noLightEmittence = player.wingsLogic != player.wings;
                            player.fallStart = (int)(player.position.Y / 16f);
                            if (player.velocity.Y > 0f) {
                                if (player.wings == 10 && Main.rand.NextBool(3)) {
                                    int num48 = 4;
                                    if (player.direction == 1)
                                        num48 = -40;

                                    int num49 = Dust.NewDust(new Vector2(player.position.X + player.width / 2 + num48, player.position.Y + player.height / 2 - 15f), 30, 30, DustID.Snow, 0f, 0f, 50, default, 0.6f);
                                    Main.dust[num49].fadeIn = 1.1f;
                                    Main.dust[num49].noGravity = true;
                                    Main.dust[num49].noLight = true;
                                    Main.dust[num49].velocity *= 0.3f;
                                    Main.dust[num49].shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
                                }

                                if (player.wings == 34 && player.ShouldDrawWingsThatAreAlwaysAnimated() && Main.rand.NextBool(3)) {
                                    int num50 = 4;
                                    if (player.direction == 1)
                                        num50 = -40;

                                    int num51 = Dust.NewDust(new Vector2(player.position.X + player.width / 2 + num50, player.position.Y + player.height / 2 - 15f), 30, 30, DustID.AncientLight, 0f, 0f, 50, default, 0.6f);
                                    Main.dust[num51].fadeIn = 1.1f;
                                    Main.dust[num51].noGravity = true;
                                    Main.dust[num51].noLight = true;
                                    Main.dust[num51].noLightEmittence = noLightEmittence;
                                    Main.dust[num51].velocity *= 0.3f;
                                    Main.dust[num51].shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
                                }

                                if (player.wings == 40)
                                    player.ShouldDrawWingsThatAreAlwaysAnimated();

                                if (player.wings == 44)
                                    player.ShouldDrawWingsThatAreAlwaysAnimated();

                                if (player.wings == 9 && Main.rand.NextBool(3)) {
                                    int num52 = 8;
                                    if (player.direction == 1)
                                        num52 = -40;

                                    int num53 = Dust.NewDust(new Vector2(player.position.X + player.width / 2 + num52, player.position.Y + player.height / 2 - 15f), 30, 30, DustID.Torch, 0f, 0f, 200, default, 2f);
                                    Main.dust[num53].noGravity = true;
                                    Main.dust[num53].velocity *= 0.3f;
                                    Main.dust[num53].noLightEmittence = noLightEmittence;
                                    Main.dust[num53].shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
                                }

                                if (player.wings == 29 && Main.rand.NextBool(3)) {
                                    int num54 = 8;
                                    if (player.direction == 1)
                                        num54 = -40;

                                    int num55 = Dust.NewDust(new Vector2(player.position.X + player.width / 2 + num54, player.position.Y + player.height / 2 - 15f), 30, 30, DustID.Torch, 0f, 0f, 100, default, 2.4f);
                                    Main.dust[num55].noGravity = true;
                                    Main.dust[num55].velocity *= 0.3f;
                                    Main.dust[num55].noLightEmittence = noLightEmittence;
                                    if (Main.rand.NextBool(10))
                                        Main.dust[num55].fadeIn = 2f;

                                    Main.dust[num55].shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
                                }

                                if (player.wings == 6) {
                                    if (Main.rand.NextBool(10)) {
                                        int num56 = 4;
                                        if (player.direction == 1)
                                            num56 = -40;

                                        int num57 = Dust.NewDust(new Vector2(player.position.X + player.width / 2 + num56, player.position.Y + player.height / 2 - 12f), 30, 20, DustID.Pixie, 0f, 0f, 200);
                                        Main.dust[num57].noLightEmittence = noLightEmittence;
                                        Main.dust[num57].velocity *= 0.3f;
                                        Main.dust[num57].shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
                                    }
                                }
                                else if (player.wings == 5 && Main.rand.NextBool(6)) {
                                    int num58 = 6;
                                    if (player.direction == 1)
                                        num58 = -30;

                                    int num59 = Dust.NewDust(new Vector2(player.position.X + player.width / 2 + num58, player.position.Y), 18, player.height, DustID.Enchanted_Pink, 0f, 0f, 255, default, 1.2f);
                                    Main.dust[num59].velocity *= 0.3f;
                                    Main.dust[num59].noLightEmittence = noLightEmittence;
                                    Main.dust[num59].shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
                                }

                                if (isCustomWings) { }
                                else
                                if (player.wings == 4) {
                                    player.rocketDelay2--;
                                    if (player.rocketDelay2 <= 0) {
                                        SoundEngine.PlaySound(SoundID.Item13, player.position);
                                        player.rocketDelay2 = 60;
                                    }

                                    int type = 6;
                                    float scale = 1.5f;
                                    int alpha = 100;
                                    float x5 = player.position.X + player.width / 2 + 16f;
                                    if (player.direction > 0)
                                        x5 = player.position.X + player.width / 2 - 26f;

                                    float num60 = player.position.Y + player.height - 18f;
                                    if (Main.rand.NextBool()) {
                                        x5 = player.position.X + player.width / 2 + 8f;
                                        if (player.direction > 0)
                                            x5 = player.position.X + player.width / 2 - 20f;

                                        num60 += 6f;
                                    }

                                    int num61 = Dust.NewDust(new Vector2(x5, num60), 8, 8, type, 0f, 0f, alpha, default, scale);
                                    Main.dust[num61].velocity.X *= 0.3f;
                                    Main.dust[num61].velocity.Y += 10f;
                                    Main.dust[num61].noGravity = true;
                                    Main.dust[num61].noLightEmittence = noLightEmittence;
                                    Main.dust[num61].shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
                                    player.wingFrameCounter++;
                                    if (player.wingFrameCounter > 4) {
                                        player.wingFrame++;
                                        player.wingFrameCounter = 0;
                                        if (player.wingFrame >= 3)
                                            player.wingFrame = 0;
                                    }
                                }
                                else if (player.wings != 22 && player.wings != 28) {
                                    if (player.wings == 30) {
                                        player.wingFrameCounter++;
                                        int num62 = 5;
                                        if (player.wingFrameCounter >= num62 * 3)
                                            player.wingFrameCounter = 0;

                                        player.wingFrame = 1 + player.wingFrameCounter / num62;
                                    }
                                    else if (player.wings == 34) {
                                        player.wingFrameCounter++;
                                        int num63 = 7;
                                        if (player.wingFrameCounter >= num63 * 6)
                                            player.wingFrameCounter = 0;

                                        player.wingFrame = player.wingFrameCounter / num63;
                                    }
                                    else if (player.wings != 45) {
                                        if (player.wings == 40) {
                                            player.wingFrame = 0;
                                        }
                                        else if (player.wings == 44) {
                                            player.wingFrame = 2;
                                        }
                                        else if (player.wings == 39) {
                                            player.wingFrameCounter++;
                                            int num64 = 12;
                                            if (player.wingFrameCounter >= num64 * 6)
                                                player.wingFrameCounter = 0;

                                            player.wingFrame = player.wingFrameCounter / num64;
                                        }
                                        else if (player.wings == 26) {
                                            int num65 = 6;
                                            if (player.direction == 1)
                                                num65 = -30;

                                            int num66 = Dust.NewDust(new Vector2(player.position.X + player.width / 2 + num65, player.position.Y), 18, player.height, DustID.FishronWings, 0f, 0f, 100, default, 1.4f);
                                            Main.dust[num66].noGravity = true;
                                            Main.dust[num66].noLight = true;
                                            Main.dust[num66].velocity /= 4f;
                                            Main.dust[num66].velocity -= player.velocity;
                                            Main.dust[num66].shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
                                            if (Main.rand.NextBool()) {
                                                num65 = -24;
                                                if (player.direction == 1)
                                                    num65 = 12;

                                                float num67 = player.position.Y;
                                                if (player.gravDir == -1f)
                                                    num67 += player.height / 2;

                                                num66 = Dust.NewDust(new Vector2(player.position.X + player.width / 2 + num65, num67), 12, player.height / 2, DustID.FishronWings, 0f, 0f, 100, default, 1.4f);
                                                Main.dust[num66].noGravity = true;
                                                Main.dust[num66].noLight = true;
                                                Main.dust[num66].velocity /= 4f;
                                                Main.dust[num66].velocity -= player.velocity;
                                                Main.dust[num66].shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
                                            }

                                            player.wingFrame = 2;
                                        }
                                        else if (player.wings == 37) {
                                            Color color = Color.Lerp(Color.Black, Color.White, Main.rand.NextFloat());
                                            int num68 = 6;
                                            if (player.direction == 1)
                                                num68 = -30;

                                            int num69 = Dust.NewDust(new Vector2(player.position.X + player.width / 2 + num68, player.position.Y), 24, player.height, Utils.SelectRandom<int>(Main.rand, 31, 31, 31), 0f, 0f, 100, default, 0.7f);
                                            Main.dust[num69].noGravity = true;
                                            Main.dust[num69].noLight = true;
                                            Main.dust[num69].velocity /= 4f;
                                            Main.dust[num69].velocity -= player.velocity;
                                            Main.dust[num69].shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
                                            if (Main.dust[num69].type == 55)
                                                Main.dust[num69].color = color;

                                            if (Main.rand.NextBool(3)) {
                                                num68 = -24;
                                                if (player.direction == 1)
                                                    num68 = 12;

                                                float num70 = player.position.Y;
                                                if (player.gravDir == -1f)
                                                    num70 += player.height / 2;

                                                num69 = Dust.NewDust(new Vector2(player.position.X + player.width / 2 + num68, num70), 12, player.height / 2, Utils.SelectRandom<int>(Main.rand, 31, 31, 31), 0f, 0f, 140, default, 0.7f);
                                                Main.dust[num69].noGravity = true;
                                                Main.dust[num69].noLight = true;
                                                Main.dust[num69].velocity /= 4f;
                                                Main.dust[num69].velocity -= player.velocity;
                                                Main.dust[num69].shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
                                                if (Main.dust[num69].type == 55)
                                                    Main.dust[num69].color = color;
                                            }

                                            player.wingFrame = 2;
                                        }
                                        else if (player.wings != 24) {
                                            if (player.wings == 43)
                                                player.wingFrame = 1;
                                            else if (player.wings == 12)
                                                player.wingFrame = 3;
                                            else
                                                player.wingFrame = 2;
                                        }
                                    }
                                }
                            }

                            player.velocity.Y += player.gravity / 3f * player.gravDir;
                            if (player.gravDir == 1f) {
                                if (player.velocity.Y > player.maxFallSpeed / 3f && !player.TryingToHoverDown)
                                    player.velocity.Y = player.maxFallSpeed / 3f;
                            }
                            else if (player.velocity.Y < (0f - player.maxFallSpeed) / 3f && !player.TryingToHoverUp) {
                                player.velocity.Y = (0f - player.maxFallSpeed) / 3f;
                            }
                        }
                        #endregion
                        #region 如果飞毯时间不足, 则按重力下降
                        else if (player.cartRampTime <= 0) {
                            player.velocity.Y += player.gravity * player.gravDir;
                        }
                        else {
                            player.cartRampTime--;
                        }
                        #endregion
                    }
                    #endregion
                    #region 只要不是蜜蜂坐骑 (mount.Type != 5), 则根据 maxFallSpeed 限制最大竖直下落速度
                    if (!player.mount.Active || player.mount.Type != 5) {
                        if (player.gravDir == 1f) {
                            if (player.velocity.Y > player.maxFallSpeed)
                                player.velocity.Y = player.maxFallSpeed;

                            if (player.slowFall && player.velocity.Y > player.maxFallSpeed / 3f && !player.TryingToHoverDown)
                                player.velocity.Y = player.maxFallSpeed / 3f;

                            if (player.slowFall && player.velocity.Y > player.maxFallSpeed / 5f && player.TryingToHoverUp)
                                player.velocity.Y = player.maxFallSpeed / 10f;
                        }
                        else {
                            if (player.velocity.Y < 0f - player.maxFallSpeed)
                                player.velocity.Y = 0f - player.maxFallSpeed;

                            if (player.slowFall && player.velocity.Y < (0f - player.maxFallSpeed) / 3f && !player.TryingToHoverDown)
                                player.velocity.Y = (0f - player.maxFallSpeed) / 3f;

                            if (player.slowFall && player.velocity.Y < (0f - player.maxFallSpeed) / 5f && player.TryingToHoverUp)
                                player.velocity.Y = (0f - player.maxFallSpeed) / 10f;
                        }
                    }
                    #endregion
                }
                #endregion
            }
            #endregion
            #region 抓钩状态下只是 UpdateControlHolds
            else {
                player.UpdateControlHolds();
            }
            #endregion
            #region 如果骑坐骑则将 wingFrame 置 0
            if (player.mount.Active)
                player.wingFrame = 0;
            #endregion
            #region 尝试翅膀悬停
            if ((player.wingsLogic == 22 || player.wingsLogic == 28 || player.wingsLogic == 30 || player.wingsLogic == 31 || player.wingsLogic == 33 || player.wingsLogic == 35 || player.wingsLogic == 37 || player.wingsLogic == 45) && player.TryingToHoverDown && player.controlJump && player.wingTime > 0f && !player.merman) {
                float num71 = 0.9f;
                if (player.wingsLogic == 45)
                    num71 = 0.8f;

                player.velocity.Y *= num71;
                if (player.velocity.Y > -2f && player.velocity.Y < 1f)
                    player.velocity.Y = 1E-05f;
            }

            if (player.wingsLogic == 37 && player.TryingToHoverDown && player.controlJump && player.wingTime > 0f && !player.merman) {
                player.velocity.Y *= 0.92f;
                if (player.velocity.Y > -2f && player.velocity.Y < 1f)
                    player.velocity.Y = 1E-05f;
            }
            #endregion
            #region 抓取物品 (GrabItems)
            player.GrabItems(playerWai);
            #endregion
            #region 查找物块互动 (LookForTileInteractions)
            player.LookForTileInteractions();
            #endregion
            #region 肉山强制移动
            if (player.tongued) {
                player.StopVanityActions();
                bool flag23 = false;
                if (Main.wofNPCIndex >= 0) {
                    NPC nPC = Main.npc[Main.wofNPCIndex];
                    float num72 = nPC.Center.X + nPC.direction * 200;
                    float y7 = nPC.Center.Y;
                    Vector2 center = player.Center;
                    float num73 = num72 - center.X;
                    float num74 = y7 - center.Y;
                    float num75 = (float)Math.Sqrt(num73 * num73 + num74 * num74);
                    float num76 = 11f;
                    if (Main.expertMode) {
                        float value = 22f;
                        float amount = Math.Min(1f, nPC.velocity.Length() / 5f);
                        num76 = MathHelper.Lerp(num76, value, amount);
                    }

                    float num77 = num75;
                    if (num75 > num76) {
                        num77 = num76 / num75;
                    }
                    else {
                        num77 = 1f;
                        flag23 = true;
                    }

                    num73 *= num77;
                    num74 *= num77;
                    player.velocity.X = num73;
                    player.velocity.Y = num74;
                }
                else {
                    flag23 = true;
                }

                if (flag23 && Main.myPlayer == player.whoAmI) {
                    for (int num78 = 0; num78 < Player.maxBuffs; num78++) {
                        if (player.buffType[num78] == 38)
                            player.DelBuff(num78);
                    }
                }
            }
            #endregion

            #region 对于自己的玩家...
            if (Main.myPlayer == player.whoAmI) {
                #region 尝试被肉山拖动
                player.WOFTongue();
                #endregion
                #region 设置钩子的控制字段, 尝试开始抓钩
                if (player.controlHook) {
                    if (player.releaseHook)
                        player.QuickGrapple();

                    player.releaseHook = false;
                }
                else {
                    player.releaseHook = true;
                }
                #endregion
                #region 尝试取消正在对话的 NPC
                if (player.talkNPC >= 0) {
                    Rectangle rectangle = new((int)(player.position.X + player.width / 2 - Player.tileRangeX * 16), (int)(player.position.Y + player.height / 2 - Player.tileRangeY * 16), Player.tileRangeX * 16 * 2, Player.tileRangeY * 16 * 2);
                    Rectangle value2 = new((int)Main.npc[player.talkNPC].position.X, (int)Main.npc[player.talkNPC].position.Y, Main.npc[player.talkNPC].width, Main.npc[player.talkNPC].height);
                    if (!rectangle.Intersects(value2) || player.chest != -1 || !Main.npc[player.talkNPC].active || player.tileEntityAnchor.InUse) {
                        if (player.chest == -1)
                            SoundEngine.PlaySound(SoundID.MenuClose);

                        player.SetTalkNPC(-1);
                        Main.npcChatCornerItem = 0;
                        Main.npcChatText = "";
                    }
                }
                #endregion
                #region 尝试取消设置标牌
                if (player.sign >= 0) {
                    Rectangle value3 = new((int)(player.position.X + player.width / 2 - Player.tileRangeX * 16), (int)(player.position.Y + player.height / 2 - Player.tileRangeY * 16), Player.tileRangeX * 16 * 2, Player.tileRangeY * 16 * 2);
                    try {
                        bool flag24 = false;
                        if (Main.sign[player.sign] == null)
                            flag24 = true;

                        if (!flag24 && !new Rectangle(Main.sign[player.sign].x * 16, Main.sign[player.sign].y * 16, 32, 32).Intersects(value3))
                            flag24 = true;

                        if (flag24) {
                            SoundEngine.PlaySound(SoundID.MenuClose);
                            player.sign = -1;
                            Main.editSign = false;
                            Main.npcChatText = "";
                        }
                    }
                    catch {
                        SoundEngine.PlaySound(SoundID.MenuClose);
                        player.sign = -1;
                        Main.editSign = false;
                        Main.npcChatText = "";
                    }
                }
                #endregion
                #region 设置标牌, 编辑箱子
                if (Main.editSign) {
                    if (player.sign == -1)
                        Main.editSign = false;
                    else
                        Main.InputTextSign();
                }
                else if (Main.editChest) {
                    Main.InputTextChest();
                    if (Main.player[Main.myPlayer].chest == -1)
                        Main.editChest = false;
                }
                #endregion
                #region 矿车创人
                if (player.mount.Active && player.mount.Cart && player.velocity.Length() > 4f) {
                    Rectangle rectangle2 = new((int)player.position.X, (int)player.position.Y, player.width, player.height);
                    if (player.velocity.X < -1f)
                        rectangle2.X -= 15;

                    if (player.velocity.X > 1f)
                        rectangle2.Width += 15;

                    if (player.velocity.X < -10f)
                        rectangle2.X -= 10;

                    if (player.velocity.X > 10f)
                        rectangle2.Width += 10;

                    if (player.velocity.Y < -1f)
                        rectangle2.Y -= 10;

                    if (player.velocity.Y > 1f)
                        rectangle2.Height += 10;

                    for (int num79 = 0; num79 < 200; num79++) {
                        if (Main.npc[num79].active && !Main.npc[num79].dontTakeDamage && !Main.npc[num79].friendly && Main.npc[num79].immune[playerWai] == 0 && player.CanNPCBeHitByPlayerOrPlayerProjectile(Main.npc[num79]) && rectangle2.Intersects(new Rectangle((int)Main.npc[num79].position.X, (int)Main.npc[num79].position.Y, Main.npc[num79].width, Main.npc[num79].height))) {
                            /*
							float num80 = meleeCrit;
							if (num80 < (float)rangedCrit)
								num80 = rangedCrit;

							if (num80 < (float)magicCrit)
								num80 = magicCrit;
							*/
                            //TML: Potentially bad for performance
                            float num80 = DamageClassLoader.DamageClasses.Select(t => player.GetTotalCritChance(t)).Max();

                            bool crit = false;
                            if (Main.rand.Next(1, 101) <= num80)
                                crit = true;

                            float currentSpeed = player.velocity.Length() / player.maxRunSpeed;
                            player.GetMinecartDamage(currentSpeed, out var damage2, out var knockback);
                            int num81 = 1;
                            if (player.velocity.X < 0f)
                                num81 = -1;

                            if (Main.npc[num79].knockBackResist < 1f && Main.npc[num79].knockBackResist > 0f)
                                knockback /= Main.npc[num79].knockBackResist;

                            if (player.whoAmI == Main.myPlayer)
                                player.ApplyDamageToNPC(Main.npc[num79], damage2, knockback, num81, crit);

                            Main.npc[num79].immune[playerWai] = 30;
                            if (!Main.npc[num79].active)
                                AchievementsHelper.HandleSpecialEvent(player, 9);
                        }
                    }
                }
                #endregion
                #region NPC 碰撞
                player.Update_NPCCollision();
                #endregion
                #region 非微光状态下的伤害物块碰撞
                if (!player.shimmering) {
                    Collision.HurtTile hurtTile = player.GetHurtTile();
                    if (hurtTile.type >= 0)
                        player.ApplyTouchDamage(hurtTile.type, hurtTile.x, hurtTile.y);
                }
                #endregion
                #region 尝试取消微光隐身 (TryToShimmerUnstuck)
                player.TryToShimmerUnstuck();
                #endregion
            }
            #endregion

            #region 设置一些控制字段 (一些 release 和 timer 等)
            if (player.controlRight) {
                player.releaseRight = false;
            }
            else {
                player.releaseRight = true;
                player.rightTimer = 7;
            }

            if (player.controlLeft) {
                player.releaseLeft = false;
            }
            else {
                player.releaseLeft = true;
                player.leftTimer = 7;
            }

            player.releaseDown = !player.controlDown;
            if (player.rightTimer > 0)
                player.rightTimer--;
            else if (player.controlRight)
                player.rightTimer = 7;

            if (player.leftTimer > 0)
                player.leftTimer--;
            else if (player.controlLeft)
                player.leftTimer = 7;
            #endregion
            #region 抓钩移动 (GrappleMovement)
            player.GrappleMovement();
            #endregion
            #region 粘性移动 (StickyMovement)
            player.StickyMovement();
            #endregion
            #region 更新溺水相关
            player.CheckDrowning();
            #endregion
            #region 液体相关
            #region 如果反重力, 取消水上行走
            if (player.gravDir == -1f) {
                player.waterWalk = false;
                player.waterWalk2 = false;
            }
            #endregion
            int heightForWaterWalk = player.height; // num82
            if (player.waterWalk)
                heightForWaterWalk -= 6;
            #region 设置 lavaWet, 尝试被岩浆烫伤
            bool lavaWetToSet = !player.shimmering && Collision.LavaCollision(player.position, player.width, heightForWaterWalk); // flag25

            if (lavaWetToSet) {
                if (!player.lavaImmune && Main.myPlayer == playerWai && player.hurtCooldowns[4] <= 0) {
                    if (player.lavaTime > 0) {
                        player.lavaTime--;
                    }
                    else {
                        int num83 = 80;
                        int num84 = 420;
                        if (Main.remixWorld) {
                            num83 = 200;
                            num84 = 630;
                        }

                        if (!player.ashWoodBonus || !player.lavaRose) {
                            if (player.ashWoodBonus) {
                                if (Main.remixWorld)
                                    num83 = 145;

                                num83 /= 2;
                                num84 -= 210;
                            }

                            if (player.lavaRose) {
                                num83 -= 45;
                                num84 -= 210;
                            }

                            if (num83 > 0)
                                player.Hurt(PlayerDeathReason.ByOther(2), num83, 0, pvp: false, quiet: false, Crit: false, 4);

                            if (num84 > 0)
                                player.AddBuff(24, num84);
                        }
                    }
                }

                player.lavaWet = true;
            }
            else {
                player.lavaWet = false;
                if (player.lavaTime < player.lavaMax)
                    player.lavaTime++;
            }

            if (player.lavaTime > player.lavaMax)
                player.lavaTime = player.lavaMax;
            #endregion
            if (player.waterWalk2 && !player.waterWalk)
                heightForWaterWalk -= 6;

            #region 液体碰撞检测
            bool wetCollision = Collision.WetCollision(player.position, player.width, player.height); // num85
            bool honeyWetToSet = Collision.honey; // flag26
            bool shimmerWetToSet = Collision.shimmer;
            #endregion
            #region 尝试微光隐身
            if (shimmerWetToSet) {
                player.shimmerWet = true;
                if (player.whoAmI == Main.myPlayer && !player.shimmerImmune && !player.shimmerUnstuckHelper.ShouldUnstuck) {
                    int num86 = (int)(player.Center.X / 16f);
                    int num87 = (int)((player.position.Y + 1f) / 16f);
                    if (Main.tile[num86, num87] != null && Main.tile[num86, num87].shimmer() && Main.tile[num86, num87].liquid >= 0 && player.position.Y / 16f < Main.UnderworldLayer)
                        player.AddBuff(353, 60);
                }
            }
            #endregion
            #region 若沾有蜂蜜且没在微光, 则添加蜂蜜 buff, 设置 honeyWet
            if (honeyWetToSet && !player.shimmering) {
                player.AddBuff(48, 1800);
                player.honeyWet = true;
            }
            #endregion
            #region 如果有液体碰撞...
            if (wetCollision) {
                #region 如果着火了则灭火 (岩浆除外)
                if ((player.onFire || player.onFire3) && !player.lavaWet) {
                    for (int num88 = 0; num88 < Player.maxBuffs; num88++) {
                        int num89 = player.buffType[num88];
                        if (num89 == 24 || num89 == 323)
                            player.DelBuff(num88);
                    }
                }
                #endregion
                #region 进入液体 (原本不是湿的) 时尝试发出视效和音效, 漂浮时限制竖直速度 (ShouldFloatInWater)
                if (!player.wet) {
                    if (player.wetCount == 0) {
                        player.wetCount = 10;
                        #region 非微光状态时根据进入的液体散发粒子, 发出声音
                        if (!player.shimmering) {
                            if (lavaWetToSet) {
                                for (int num96 = 0; num96 < 20; num96++) {
                                    int num97 = Dust.NewDust(new Vector2(player.position.X - 6f, player.position.Y + player.height / 2 - 8f), player.width + 12, 24, DustID.Lava);
                                    Main.dust[num97].velocity.Y -= 1.5f;
                                    Main.dust[num97].velocity.X *= 2.5f;
                                    Main.dust[num97].scale = 1.3f;
                                    Main.dust[num97].alpha = 100;
                                    Main.dust[num97].noGravity = true;
                                }

                                SoundEngine.PlaySound(SoundID.Splash, (int)player.position.X, (int)player.position.Y);
                            }
                            else if (player.shimmerWet) {
                                for (int num90 = 0; num90 < 50; num90++) {
                                    int num91 = Dust.NewDust(new Vector2(player.position.X - 6f, player.position.Y + player.height / 2), player.width + 12, 24, DustID.ShimmerSplash); // 308
                                    Main.dust[num91].velocity.Y -= 4f;
                                    Main.dust[num91].velocity.X *= 2.5f;
                                    Main.dust[num91].scale = 0.8f;
                                    Main.dust[num91].noGravity = true;
                                    Main.dust[num91].color = Main.rand.Next(6) switch {
                                        0 => new Color(255, 255, 210),
                                        1 => new Color(190, 245, 255),
                                        2 => new Color(255, 150, 255),
                                        _ => new Color(190, 175, 255),
                                    };
                                }

                                SoundEngine.PlaySound(SoundID.Shimmer1, (int)player.position.X, (int)player.position.Y); // 19, 2
                            }
                            else if (player.honeyWet) {
                                for (int num92 = 0; num92 < 20; num92++) {
                                    int num93 = Dust.NewDust(new Vector2(player.position.X - 6f, player.position.Y + player.height / 2 - 8f), player.width + 12, 24, DustID.Honey);
                                    Main.dust[num93].velocity.Y -= 1f;
                                    Main.dust[num93].velocity.X *= 2.5f;
                                    Main.dust[num93].scale = 1.3f;
                                    Main.dust[num93].alpha = 100;
                                    Main.dust[num93].noGravity = true;
                                }

                                SoundEngine.PlaySound(SoundID.Splash, (int)player.position.X, (int)player.position.Y);
                            }
                            else {
                                for (int num94 = 0; num94 < 50; num94++) {
                                    int num95 = Dust.NewDust(new Vector2(player.position.X - 6f, player.position.Y + player.height / 2 - 8f), player.width + 12, 24, Dust.dustWater());
                                    Main.dust[num95].velocity.Y -= 3f;
                                    Main.dust[num95].velocity.X *= 2.5f;
                                    Main.dust[num95].scale = 0.8f;
                                    Main.dust[num95].alpha = 100;
                                    Main.dust[num95].noGravity = true;
                                }

                                SoundEngine.PlaySound(SoundID.Splash, (int)player.position.X, (int)player.position.Y);// 19, 0
                            }
                        }
                        #endregion
                    }

                    player.wet = true;
                    #region 漂浮时限制竖直速度 (ShouldFloatInWater)
                    if (player.ShouldFloatInWater) {
                        player.velocity.Y /= 2f;
                        if (player.velocity.Y > 3f)
                            player.velocity.Y = 3f;
                    }
                    #endregion
                }
                #endregion
            }
            #endregion
            #region 离开液体时 (如果没有液体碰撞但原本有) 尝试限制 jump, 尝试发出视效和音效
            else if (player.wet) {
                player.wet = false;
                if (player.jump > Player.jumpHeight / 5 && player.wetSlime == 0)
                    player.jump = Player.jumpHeight / 5;
                #region 尝试根据液体发出粒子和声音
                if (player.wetCount == 0) {
                    player.wetCount = 10;
                    if (!player.shimmering) {
                        if (!player.lavaWet) {
                            if (player.shimmerWet) {
                                for (int num98 = 0; num98 < 50; num98++) {
                                    int num99 = Dust.NewDust(new Vector2(player.position.X - 6f, player.position.Y + player.height / 2), player.width + 12, 24, 308);
                                    Main.dust[num99].velocity.Y -= 4f;
                                    Main.dust[num99].velocity.X *= 2.5f;
                                    Main.dust[num99].scale = 0.75f;
                                    Main.dust[num99].noGravity = true;
                                    Main.dust[num99].color = Main.rand.Next(6) switch {
                                        0 => new Color(255, 255, 210),
                                        1 => new Color(190, 245, 255),
                                        2 => new Color(255, 150, 255),
                                        _ => new Color(190, 175, 255),
                                    };
                                }

                                SoundEngine.PlaySound(SoundID.Shimmer2, (int)player.position.X, (int)player.position.Y); // 19, 3
                            }
                            else if (player.honeyWet) {
                                for (int num100 = 0; num100 < 20; num100++) {
                                    int num101 = Dust.NewDust(new Vector2(player.position.X - 6f, player.position.Y + player.height / 2 - 8f), player.width + 12, 24, DustID.Honey);
                                    Main.dust[num101].velocity.Y -= 1f;
                                    Main.dust[num101].velocity.X *= 2.5f;
                                    Main.dust[num101].scale = 1.3f;
                                    Main.dust[num101].alpha = 100;
                                    Main.dust[num101].noGravity = true;
                                }

                                SoundEngine.PlaySound(SoundID.Splash, (int)player.position.X, (int)player.position.Y);
                            }
                            else {
                                for (int num102 = 0; num102 < 50; num102++) {
                                    int num103 = Dust.NewDust(new Vector2(player.position.X - 6f, player.position.Y + player.height / 2), player.width + 12, 24, Dust.dustWater());
                                    Main.dust[num103].velocity.Y -= 4f;
                                    Main.dust[num103].velocity.X *= 2.5f;
                                    Main.dust[num103].scale = 0.8f;
                                    Main.dust[num103].alpha = 100;
                                    Main.dust[num103].noGravity = true;
                                }

                                SoundEngine.PlaySound(SoundID.Splash, (int)player.position.X, (int)player.position.Y); // 19, 0
                            }
                        }
                        else {
                            for (int num104 = 0; num104 < 20; num104++) {
                                int num105 = Dust.NewDust(new Vector2(player.position.X - 6f, player.position.Y + player.height / 2 - 8f), player.width + 12, 24, DustID.Lava);
                                Main.dust[num105].velocity.Y -= 1.5f;
                                Main.dust[num105].velocity.X *= 2.5f;
                                Main.dust[num105].scale = 1.3f;
                                Main.dust[num105].alpha = 100;
                                Main.dust[num105].noGravity = true;
                            }

                            SoundEngine.PlaySound(SoundID.Splash, (int)player.position.X, (int)player.position.Y);
                        }
                    }
                }
                #endregion
            }
            #endregion
            #region 根据变量将一些 wet 置为 false
            if (!honeyWetToSet)
                player.honeyWet = false;

            if (!shimmerWetToSet)
                player.shimmerWet = false;

            if (!player.wet) {
                player.lavaWet = false;
                player.honeyWet = false;
                player.shimmerWet = false;
            }
            #endregion
            #region wet 相关的计时器
            if (player.wetCount > 0)
                player.wetCount--;

            if (player.wetSlime > 0)
                player.wetSlime--;
            #endregion
            #region 水中的一些坐骑的处理
            if (player.wet && player.mount.Active) {
                switch (player.mount.Type) {
                case 5:
                case 7:
                    if (player.whoAmI == Main.myPlayer)
                        player.mount.Dismount(player);
                    break;
                case 3:
                case 50:
                    player.wetSlime = 30;
                    if (player.velocity.Y > 2f)
                        player.velocity.Y *= 0.9f;
                    player.velocity.Y -= 0.5f;
                    if (player.velocity.Y < -4f)
                        player.velocity.Y = -4f;
                    break;
                }
            }
            #endregion
            #region 专家模式雪地浸水 debuff
            if (Main.expertMode && player.ZoneSnow && player.wet && !player.lavaWet && !player.honeyWet && !player.arcticDivingGear && player.environmentBuffImmunityTimer == 0)
                player.AddBuff(46, 150);
            #endregion
            #endregion

            #region 设置 gfxOffY
            float num106 = 1f + Math.Abs(player.velocity.X) / 3f;
            if (player.gfxOffY > 0f) {
                player.gfxOffY -= num106 * player.stepSpeed;
                if (player.gfxOffY < 0f)
                    player.gfxOffY = 0f;
            }
            else if (player.gfxOffY < 0f) {
                player.gfxOffY += num106 * player.stepSpeed;
                if (player.gfxOffY > 0f)
                    player.gfxOffY = 0f;
            }

            if (player.gfxOffY > 32f)
                player.gfxOffY = 32f;

            if (player.gfxOffY < -32f)
                player.gfxOffY = -32f;
            #endregion
            #region 对于自己的玩家, 尝试碎冰, 破碎砖
            if (Main.myPlayer == playerWai) {
                if (!player.iceSkate)
                    player.CheckIceBreak();

                player.CheckCrackedBrickBreak();
            }
            #endregion
            #region 非微光状态下的楼梯处理 (SlopeDownMovement)
            if (!player.shimmering) {
                player.SlopeDownMovement();
                bool flag27 = player.mount.Type == 7 || player.mount.Type == 8 || player.mount.Type == 12 || player.mount.Type == 44 || player.mount.Type == 49;
                if (player.velocity.Y == player.gravity && (!player.mount.Active || (!player.mount.Cart && player.mount.Type != 48 && !flag27)))
                    Collision.StepDown(ref player.position, ref player.velocity, player.width, player.height, ref player.stepSpeed, ref player.gfxOffY, (int)player.gravDir, player.waterWalk || player.waterWalk2);

                if (player.gravDir == -1f) {
                    if ((player.carpetFrame != -1 || player.velocity.Y <= player.gravity) && !player.controlUp)
                        Collision.StepUp(ref player.position, ref player.velocity, player.width, player.height, ref player.stepSpeed, ref player.gfxOffY, (int)player.gravDir, player.controlUp);
                }
                else if ((player.carpetFrame != -1 || player.velocity.Y >= player.gravity) && !player.controlDown && !player.mount.Cart && !flag27 && player.grappling[0] == -1) {
                    Collision.StepUp(ref player.position, ref player.velocity, player.width, player.height, ref player.stepSpeed, ref player.gfxOffY, (int)player.gravDir, player.controlUp);
                }
            }
            #endregion
            #region 设置 oldPosition, oldDirection
            player.oldPosition = player.position;
            player.oldDirection = player.direction;
            #endregion
            bool falling = Math.Abs(player.velocity.Y) > player.gravity;

            Vector2 oldVelocity = player.velocity; // vector3
            player.slideDir = 0;
            bool ignorePlats = (player.gravDir == -1f) | (player.mount.Active && (player.mount.Cart || player.mount.Type is 7 or 8 or 12 or 23 or 44 or 48)) | player.GoingDownWithGrapple | player.pulley;
            bool fallThrough = ignorePlats || player.controlDown;
            #region 矿车坐骑的处理
            bool oldOnTrack = player.onTrack; // flag28
            player.onTrack = false;
            bool needSyncPlayerControlsForTrackCollision = false; // flag29
            if (player.mount.Active && player.mount.Cart) {
                player.fartKartCloudDelay = Math.Max(0, player.fartKartCloudDelay - 1);
                float num107 = ((player.ignoreWater || player.merman) ? 1f : (player.shimmerWet ? 0.25f : (player.honeyWet ? 0.25f : ((!player.wet) ? 1f : 0.5f))));
                player.velocity *= num107;
                DelegateMethods.Minecart.rotation = player.fullRotation;
                DelegateMethods.Minecart.rotationOrigin = player.fullRotationOrigin;
                BitsByte bitsByte = Minecart.TrackCollision(player, ref player.position, ref player.velocity, ref player.lastBoost, player.width, player.height, player.controlDown, player.controlUp, player.fallStart2, trackOnly: false, player.mount.Delegations);
                if (bitsByte[0]) {
                    player.onTrack = true;
                    player.gfxOffY = Minecart.TrackRotation(player, ref player.fullRotation, player.position + player.velocity, player.width, player.height, player.controlDown, player.controlUp, player.mount.Delegations);
                    player.fullRotationOrigin = new Vector2(player.width / 2, player.height);
                }

                if (oldOnTrack && !player.onTrack)
                    player.mount.Delegations.MinecartJumpingSound(player, player.position, player.width, player.height);

                if (bitsByte[1]) {
                    if (player.controlLeft || player.controlRight) {
                        if (player.cartFlip)
                            player.cartFlip = false;
                        else
                            player.cartFlip = true;
                    }

                    if (player.velocity.X > 0f)
                        player.direction = 1;
                    else if (player.velocity.X < 0f)
                        player.direction = -1;

                    player.mount.Delegations.MinecartBumperSound(player, player.position, player.width, player.height);
                }

                player.velocity /= num107;
                if (bitsByte[3] && player.whoAmI == Main.myPlayer)
                    needSyncPlayerControlsForTrackCollision = true;

                if (bitsByte[2])
                    player.cartRampTime = (int)(Math.Abs(player.velocity.X) / player.mount.RunSpeed * 20f);

                if (bitsByte[4])
                    player.trackBoost -= 4f;

                if (bitsByte[5])
                    player.trackBoost += 4f;
            }
            #endregion

            bool countRunning = player.whoAmI == Main.myPlayer && !player.mount.Active; // flag30
            Vector2 originPosition = player.position; // vector4
            #region 星璇 debuff 控制竖直速度
            if (player.vortexDebuff)
                player.velocity.Y = player.velocity.Y * 0.8f + (float)Math.Cos(player.Center.X % 120f / 120f * ((float)Math.PI * 2f)) * 5f * 0.2f;
            #endregion

            #region ModPlayer.PreUpdateMovement
            PlayerLoader.PreUpdateMovement(player);
            #endregion
            #region 若被肉山拉扯, 则直接使用速度更新位置
            if (player.tongued) {
                player.position += player.velocity;
                countRunning = false;
            }
            #endregion
            #region 否则若在微光中或微光状态中则按微光处理 (ShimmerCollision)
            else if (player.shimmerWet || player.shimmering) {
                player.ShimmerCollision(fallThrough, ignorePlats, player.shimmering);
            }
            #endregion
            #region 否则若在蜂蜜中则按蜂蜜处理 (HoneyCollision)
            else if (player.honeyWet && !player.ignoreWater) {
                player.HoneyCollision(fallThrough, ignorePlats);
            }
            #endregion
            #region 否则若在水中则按在水中处理 (WaterCollision)
            else if (player.wet && !player.merman && !player.ignoreWater && !player.trident) {
                player.WaterCollision(fallThrough, ignorePlats);
            }
            #endregion
            #region 否则按在干燥时处理 (DryCollision), 对于特定坐骑执行额外的竖直处理
            else {
                ShowPlayerDryCollision(fallThrough, ignorePlats); // player.DryCollision(fallThrough, ignorePlats);
                if (player.mount.Active && player.mount.IsConsideredASlimeMount && player.velocity.Y != 0f && !player.SlimeDontHyperJump) {
                    Vector2 originVelocity = player.velocity; // vector5
                    player.velocity.X = 0f;
                    player.DryCollision(fallThrough, ignorePlats);
                    player.velocity.X = originVelocity.X;
                }

                if (player.mount.Active && player.mount.Type == MountID.PogoStick && player.velocity.Y != 0f) { // 43
                    Vector2 originVelocity = player.velocity; // vector6
                    player.velocity.X = 0f;
                    player.DryCollision(fallThrough, ignorePlats);
                    player.velocity.X = originVelocity.X;
                }
            }
            #endregion

            #region 更新触碰到的物块 (UpdateTouchingTiles)
            player.UpdateTouchingTiles();
            #endregion
            #region 尝试弹跳
            player.TryBouncingBlocks(falling);
            #endregion
            #region 尝试落在引爆器上
            player.TryLandingOnDetonator();
            #endregion

            #region 非微光状态和肉山拖拽章台的楼梯碰撞和传送带碰撞
            if (!player.shimmering && !player.tongued) {
                player.SlopingCollision(fallThrough, ignorePlats);
                if (!player.isLockedToATile)
                    Collision.StepConveyorBelt(player, player.gravDir);
            }
            #endregion
            #region 更新跑步机成就
            if (countRunning && player.velocity.Y == 0f)
                AchievementsHelper.HandleRunning(Math.Abs(player.position.X - originPosition.X));
            #endregion
            #region 根据轨道碰撞同步玩家控制
            if (needSyncPlayerControlsForTrackCollision) {
                NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, player.whoAmI);
                Minecart.HitTrackSwitch(new Vector2(player.position.X, player.position.Y), player.width, player.height);
            }
            #endregion

            #region 设置 slideDir
            if (oldVelocity.X != player.velocity.X) {
                if (oldVelocity.X < 0f)
                    player.slideDir = -1;
                else if (oldVelocity.X > 0f)
                    player.slideDir = 1;
            }
            #endregion
            if (player.gravDir == 1f && Collision.up) {
                player.velocity.Y = 0.01f;
                if (!player.merman)
                    player.jump = 0;
            }
            else if (player.gravDir == -1f && Collision.down) {
                player.velocity.Y = -0.01f;
                if (!player.merman)
                    player.jump = 0;
            }
            #region 踩在地上的视效
            if (player.velocity.Y == 0f && player.grappling[0] == -1)
                player.FloorVisuals(falling);
            #endregion
            #region 对于非微光状态的自己的玩家, 调用 Collision.SwitchTiles
            if (player.whoAmI == Main.myPlayer && !player.shimmering)
                Collision.SwitchTiles(player.position, player.width, player.height, player.oldPosition, 1);
            #endregion

            #region 更新压力板
            PressurePlateHelper.UpdatePlayerPosition(player);
            #endregion
            #region 边界移动 (BordersMovement)
            player.BordersMovement();
            #endregion
            #region 重置召唤物数 (numMinions, slotsMinions)
            player.numMinions = 0;
            player.slotsMinions = 0f;
            #endregion
            #region 尝试右键使用物品
            if (Main.netMode != NetmodeID.Server && player.mount.Type != 8)
                player.ItemCheck_ManageRightClickFeatures();
            #endregion

            #region 使用物品 (ItemCheckWrapped)
            player.ItemCheckWrapped(playerWai);
            #endregion
            #region 帧图更新 (PlayerFrame)
            player.PlayerFrame();
            #endregion
            #region 钻机坐骑
            if (player.mount.Type == 8)
                player.mount.UseDrill(player);
            #endregion
            #region 限制生命和魔力不超过最大值
            if (player.statLife > player.statLifeMax2)
                player.statLife = player.statLifeMax2;

            if (player.statMana > player.statManaMax2)
                player.statMana = player.statManaMax2;
            #endregion
            // More patch context.
            player.grappling[0] = -1;
            player.grapCount = 0;
            player.UpdateReleaseUseTile();
            player.UpdateAdvancedShadows();

            #region ModPlayer.PostUpdate
            PlayerLoader.PostUpdate(player);
            #endregion
        }
        public static void ShowPlayerDryCollision(bool fallThrough, bool ignorePlats) {
            
		    int height = ((!player.onTrack) ? player.height : (player.height - 10)); // num
            #region 若速度大于 16, 则每 16 速处理一次移动
            if (player.velocity.Length() > 16f) {
			    Vector2 vector = Collision.TileCollision(player.position, player.velocity, player.width, height, fallThrough, ignorePlats, (int)player.gravDir);
			    float velocityLength = player.velocity.Length(); // num2
			    Vector2 velocityNormalized = Vector2.Normalize(player.velocity); // vector2
			    if (vector.Y == 0f)
				    velocityNormalized.Y = 0f;

			    Vector2 zero = Vector2.Zero;
			    bool specialMountType = player.mount.Type is 7 or 8 or 12 or 44 or 49; // flag
			    while (velocityLength > 0f) {
				    float step = Math.Min(velocityLength, 16f); // num3

				    velocityLength -= step;
				    player.velocity = velocityNormalized * step;
				    player.SlopeDownMovement();
				    Vector2 velocityStep = player.velocity;
				    if (player.velocity.Y == player.gravity && (!player.mount.Active || (!player.mount.Cart && player.mount.Type != 48 && !specialMountType)))
					    Collision.StepDown(ref player.position, ref velocityStep, player.width, player.height, ref player.stepSpeed, ref player.gfxOffY, (int)player.gravDir, player.waterWalk || player.waterWalk2);

				    if (player.gravDir == -1f) {
					    if ((player.carpetFrame != -1 || player.velocity.Y <= player.gravity) && !player.controlUp)
						    Collision.StepUp(ref player.position, ref velocityStep, player.width, player.height, ref player.stepSpeed, ref player.gfxOffY, (int)player.gravDir, player.controlUp);
				    }
				    else if (specialMountType || ((player.carpetFrame != -1 || player.velocity.Y >= player.gravity) && !player.controlDown && !player.mount.Cart)) {
					    Collision.StepUp(ref player.position, ref velocityStep, player.width, player.height, ref player.stepSpeed, ref player.gfxOffY, (int)player.gravDir, player.controlUp);
				    }

				    velocityStep = Collision.TileCollision(player.position, velocityStep, player.width, height, fallThrough, ignorePlats, (int)player.gravDir); // vector4
				    if (Collision.up && player.gravDir == 1f)
					    player.jump = 0;

				    if (player.waterWalk || player.waterWalk2) {
					    Vector2 tempVelocity = player.velocity; // vector5
					    velocityStep = Collision.WaterCollision(player.position, velocityStep, player.width, player.height, fallThrough, fall2: false, player.waterWalk);
					    if (tempVelocity != player.velocity)
						    player.fallStart = (int)(player.position.Y / 16f);
				    }

				    player.position += velocityStep;
				    bool falling = false;
				    if (velocityStep.Y > player.gravity)
					    falling = true;

				    if (velocityStep.Y < 0f - player.gravity)
					    falling = true;

				    player.velocity = velocityStep;
				    player.UpdateTouchingTiles();
				    player.TryBouncingBlocks(falling);
				    player.TryLandingOnDetonator();
				    player.SlopingCollision(fallThrough, ignorePlats);
				    Collision.StepConveyorBelt(player, player.gravDir);
				    velocityStep = player.velocity;
				    zero += velocityStep;
			    }

			    player.velocity = zero;
			    return;
		    }
            #endregion

            #region 将速度修正为经过物块碰撞后的速度
            player.velocity = Collision.TileCollision(player.position, player.velocity, player.width, height, fallThrough, ignorePlats, (int)player.gravDir);
            #endregion
            #region 当重力方向正常且碰到上面时结束玩家跳跃
            if (Collision.up && player.gravDir == 1f)
			    player.jump = 0;
            #endregion
            #region 如果有水上行走则额外将速度修正为经过液体碰撞后的速度, 若有修正则设置摔落起始位置 (fallStart)
            if (player.waterWalk || player.waterWalk2) {
			    Vector2 tempVelocity = player.velocity; // vector6
			    player.velocity = Collision.WaterCollision(player.position, player.velocity, player.width, player.height, fallThrough, fall2: false, player.waterWalk);
			    if (tempVelocity != player.velocity)
				    player.fallStart = (int)(player.position.Y / 16f);
		    }
            #endregion
            #region 根据速度改变位置
            player.position += player.velocity;
            #endregion
        }
    }
    public class Item_cls {
        public static Item item;
        public const string newItem_func = """
            int NewItem(IEntitySource source, position, type, stack = 1, noBroadcast = false, ...)
            以在世界中新建一个物品, position可以为: Vector2 position; Vector2 pos, Vector2 randomBox, Vector2 pos, int Width, int Height;
            int X, int Y, int Width, int Height 当传入矩形范围时似乎会生成在正中间.
            不应该在多人模式的客户端被调用, 若想要在客户端的代码中生成, 可以用 player.QuickSpawnItem(source, item, stack = 1),
            它会处理多人模式的同步需要.     返回生成的物品在Main.item中的序号
            """;
        public const string constructor = """
            new Item()什么都不会做
            item = new Item(itemId)会设置item的各种基本信息(Item基本的SetDefaults)
            会设置其ModItem, 调用ModItem的AutoDefaults和SetDefaults, 然后是
            关联的GlobalItems的SetDefaults
            """;//构造函数
        public static void ShowItem() {
            #region params
            int x = 0, y = 0, width = 0, height = 0;
            Entity entity = null;
            int itemId = 0;
            Mod mod = null;
            #endregion
            Item.NewItem(new EntitySource_DropAsItem(entity), x, y, width, height, itemId);  //在世界中新建一个物品
            Show(item.type);                //物品id(非原版物品的id会因加载不同的模组产生变化)
            Show(item.ModItem);             //获取对应的ModItem
            Show(item.maxStack);            //最大堆叠数
            item.rare = ItemRarityID.White; //稀有度
            Show(item.value);               //值多少铜币, 为商人的卖价, 由玩家卖出只能获得其 1/5 的钱
            Show(item.Name);                //展示的名字
            Show(item.AffixName());         //带有前缀的名字
            Show(item.HoverName);           //鼠标放在上面显示的名字, 带有前缀和个数
            #region 可使用
            Show(item.useTime);                     //使用时间, 设为item.useAnimation的三分之一可让物品三连发
            Show(item.useAnimation);                //使用动画时间, 一般和使用时间相同(或更大?)
            Show(item.reuseDelay);                  //在useAnimation过完后还需要多久才能重新使用
            Show(item.useTurn);                     //使用时是否可以转身
            item.useStyle = ItemUseStyleID.Swing;   //使用方法
            item.UseSound = SoundID.Item1;          //使用声音
            #endregion
            #region 工具武器
            Show(item.width);                       //碰撞箱宽度
            Show(item.height);                      //碰撞箱高度
            Show(item.damage);                      //伤害
            item.DamageType = DamageClass.Melee;    //伤害类型, 默认DamageClass.Default
            Show(item.knockBack);                   //击退
            Show(item.autoReuse);                   //是否自动连用, 默认false
            Show(item.shoot);                       //射出弹幕的弹幕id
            Show(item.shootSpeed);                  //射出弹幕的速度
            Show(item.ammo);                        //消耗弹药的物品id
            #endregion
            #region 消耗品
            Show(item.consumable);                  //是否可消耗
            Show(item.createTile);                  //放置方块的id
            #endregion
            #region 药水食物
            Show(item.buffType);                    //buff的id
            Show(item.buffTime);                    //buff持续时间
            #endregion
            #region 饰品
            Show(item.accessory);                   //是否是饰品, 影响其是否可装入饰品栏
            Show(item.vanity);                      //是否可放在装饰栏位
            Show(item.defense);                     //增加防御, 装备的防御也是这个
#if Terraria143
            Show(item.canBePlacedInVanityRegardlessOfConditions);   //是否可不计条件的装在装饰栏位
#endif
            #endregion
            #region 商店
            Show(item.buyOnce);             //若为真, 则购买时会消耗商店中物品的stack, 消耗完时就不能买了
            Show(item.shopCustomPrice);     //商店中的自定义价格, 但还会受NPC心情影响
            #endregion
            #region 其他
            Item.buyPrice(0, 0, 50, 0);     //会直接转换为对应的铜币数
            Item.sellPrice(0, 0, 10, 0);    //得到的值为同样参数的buyPrice的五倍
            Show(item.tileWand);                    //作为物品法杖消耗物品的id, 默认-1
            Show(item.IsACoin);                     //是否是钱币
            item.StatsModifiedBy.Add(mod);          //告诉游戏此mod对此物品做出了修改
            ContentSamples.ItemsByType.TryGetValue(itemId, out item);   //当只是要查某个值时用这个而不用new一个item出来
            #endregion
        }
        /// <summary>
        /// 关于用Item.NewItem(...)时的联机同步问题
        /// </summary>
        /// <param name="prefixGiven">当为-1时会自动生成前缀</param>
        /// <param name="noBroadcast">在服务端以外无用, 控制在服务端生成的item是否不通过<see cref="NetMessage.SendData"/>
        /// 和<see cref="MessageID.SyncItem"/>同步. 若为真, 可以在此后调整item了之后再用<see cref="MessageID.SyncItem"/>同步
        /// 在多人联机客户端总是需要手动同步的, 在<see cref="Player.QuickSpawnItem"/>中可以看到例子
        /// Controls whether an item spawned on a server is synced using Terraria.NetMessage.SendData
        /// and Terraria.ID.MessageID.SyncItem. If false, the item will be synced. If true,
        /// the calling code can modify the item instance and then sync the item with Terraria.ID.MessageID.SyncItem.
        /// Has no effect except on server. NewItem running on multiplayer clients always
        /// needs to manually sync, see Terraria.Player.QuickSpawnItem(Terraria.DataStructures.IEntitySource,System.Int32,System.Int32)
        /// source code for an example.
        /// </param>
        public static void NewItemSync(IEntitySource source, Vector2 position, int itemId, int stack = 1, int prefixGiven = 0, bool noBroadcast = false, bool noGrabDelay = true) {
            #region 标准的产生物品并同步
            int itemIndex = Item.NewItem(source, position, itemId, stack, noBroadcast: false, prefixGiven, noGrabDelay);
            //NewItemInner里如果是服务器就会在noBroadcast为false时同步, 在客户端就不会[恼]
            if (Main.netMode == NetmodeID.MultiplayerClient) {
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIndex, noGrabDelay.ToInt());
            }
            #endregion
            #region 在产生物品后做一些修改然后再同步
            int itemIndex2 = Item.NewItem(source, position, itemId, stack, noBroadcast: true, prefixGiven, noGrabDelay);
            Do(Main.item[itemIndex2]);  //在这里可以对此item做一些可以被同步的修改
            NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIndex2, noGrabDelay.ToInt());
            #endregion
            #region 在已有一个Item时让它产生在世界中
            Item item = default;    //假设这是从另外一个地方得到的Item
            int itemIndex3 = Item.NewItem(source, position, item, noBroadcast: false, noGrabDelay);
            if (Main.netMode == NetmodeID.MultiplayerClient) {
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIndex3, noGrabDelay.ToInt());
            }
            //实际上这样产生的物品是由item.Clone()克隆出来的Item
            #endregion

            #region 官方轮子
            //在玩家处产生一个物品并自动同步
            Main.LocalPlayer.QuickSpawnItem(source, itemId, stack);
            #endregion
        }
    }
    public class Projectile_cls {
        public static Projectile projectile;
        public static void ShowProjectile() {
            #region params
            Vector2 worldCoordinates = default, position = default, velocity = default;
            int radius = 0, minI = 0, maxI = 0, minJ = 0, maxJ = 0, damage = 10;
            float knockBack = 3.5f;
            IEntitySource source = null;
            Player player = null;
            #endregion
            Show(projectile.friendly);      //是否能对敌方造成伤害
            Show(projectile.hostile);       //是否能对友善NPC以及玩家造成伤害, 与上一个属性可以共存
            Show(projectile.DamageType);    //伤害类型
            Show(projectile.ignoreWater);   //在水中是否不被减速
            Show(projectile.timeLeft);      //存在时间(以帧计)
            Show(projectile.tileCollide);   //是否不能穿墙
            Show(projectile.aiStyle);       //ai行为, 若自行定义AI()需设置为-1
            Show(projectile.extraUpdates);  //额外更新次数, 弹幕每帧会调用AI()总共 1 + extraUpdates 次
            Projectile.NewProjectile(source, position, velocity, ProjectileID.WoodenArrowFriendly, damage, knockBack, player.whoAmI);
            projectile.ExplodeTiles(worldCoordinates, radius, minI, maxI, minJ, maxJ, projectile.ShouldWallExplode(worldCoordinates, radius, minI, maxI, minJ, maxJ));//包括min和max
        }
    }
    public class NPC_cls {
        public static NPC npc;
        public static void ShowNPC() {
            #region params
            int buffIndex = 0, buffID = 0, time = 0, npcID = 0;
            #endregion
            Show(npc.lifeMax);      //最大生命
            Show(npc.value);        //掉落钱币(?)
            Show(npc.friendly);     //是否友好
            Show(npc.boss);         //是否是boss

            Show(npc.position);     //位置
            #region Buff
            Show(npc.buffTime[buffIndex]);      //某个buff的持续时间
            Show(npc.buffImmune[buffID]);       //是否免疫某buff
            Show(npc.buffType[buffIndex]);      //获得某个buff的ID
            npc.AddBuff(buffID, time);          //增加一个buff, 自动处理免疫, 不同模式下时间增幅, 和重添加逻辑
            npc.DelBuff(buffIndex);             //删除对应下标的buff
            npc.RequestBuffRemoval(buffID);     //删除对应类型的buff
            #endregion
            #region 世界进度
            Show(WorldGen.shadowOrbSmashed);    //是否打碎过暗影珠
            Show(NPC.downedGoblins);            //哥布林
            Show(NPC.downedSlimeKing);          //史王
            Show(NPC.downedBoss1);              //克眼
            Show(NPC.downedBoss2);              //克脑/世吞
            Show(DD2Event.DownedInvasionT1);    //旧日军团T1
            Show(NPC.downedQueenBee);           //蜂后
            Show(NPC.downedBoss3);              //骷髅王
            Show(NPC.downedDeerclops);          //独眼巨鹿
            //Show(Main.hardMode);                //肉后
            Show(NPC.downedPirates);            //海盗
            Show(NPC.downedClown);              //小丑
            Show(DD2Event.DownedInvasionT2);    //旧日军团T2
            Show(NPC.downedQueenSlime);         //史莱姆皇后
            Show(NPC.downedMechBossAny);        //任意新三王
            Show(NPC.downedMechBoss2);          //双子魔眼
            Show(NPC.downedMechBoss1);          //毁灭者
            Show(NPC.downedMechBoss3);          //机械骷髅王
            Show(NPC.downedFishron);            //猪龙鱼公爵
            Show(NPC.downedPlantBoss);          //世花
            Show(DD2Event.DownedInvasionT3);    //旧日军团T3
            Show(NPC.downedEmpressOfLight);     //光之女皇
            Show(NPC.downedGolemBoss);          //石巨人
            Show(NPC.downedMartians);           //火星人
            Show(NPC.downedAncientCultist);     //邪教徒
            Show(NPC.downedFrost);              //冰雪军团
            Show(NPC.downedTowerNebula);        //星云柱
            Show(NPC.downedTowerSolar);         //日耀柱
            Show(NPC.downedTowerStardust);      //星尘柱
            Show(NPC.downedTowerVortex);        //星漩柱
            Show(NPC.downedTowers);             //四柱
            Show(NPC.downedMoonlord);           //月后
            #endregion
            Show(npc.IsShimmerVariant);     //是否为微光转化状态(好像只能给城镇NPC用)
            Show(NPC.AnyNPCs(npcID));       //世界上是否有对应id的npc
            Show(NPC.CountNPCS(npcID));     //世界上有对应id的npc的数量
            Show(NPC.FindFirstNPC(npcID));  //找到NPC在Main.npc中的下标, 若没找到返回-1
        }
    }
    public class Tile_cls {
        public static Tile tile;
        public static void ShowTile() {

        }
    }
    public class Main_cls {
        public static Main main;
        public static void ShowMain() {
            Show(Main.hardMode);                //是否到达肉后
            #region Player
            Show(Main.player);                  //玩家数组
            Show(Main.player[Main.myPlayer]);   //玩家自己
            Show(Main.LocalPlayer);             //玩家自己, 相当于上面那行的简写
            Show(Main.mouseItem);               //鼠标上拿着的物品
            Show(Main.CurrentFrameFlags.SleepingPlayersCount);  //正在睡觉的玩家数量
            Show(Main.CurrentFrameFlags.ActivePlayersCount);    //玩家数量
            #endregion
            #region UI
            Show(Main.playerInventory);         //是否在打开背包的状态
            #endregion
            Show(Main.npc);                     //npc数组
            Show(Main.projectile);              //弹幕数组
            Show(Main.item);                    //在世界中的掉落物数组
            #region 时间
            Show(Main.time);            //自开始一个白天/夜晚经过了多长时间
            Show(Main.dayRate);         //Main.time每帧增加的量
            Show(Main.dayTime);         //是否是白天
            Show(Main.dayLength);       //白天时Main.time的限制值
            Show(Main.nightLength);     //夜晚时Main.time的限制值
            Show(Utils.GetDayTimeAs24FloatStartingFromMidnight());    //获得24小时制时间, 如上午四点半为4.5, 晚上七点半为19.5
            #region 月相
            Show(Main.moonPhase);
            Show(Main.GetMoonPhase());
            Show(MoonPhase.Empty);                  //新月
            Show(MoonPhase.QuarterAtRight);         //峨眉月
            Show(MoonPhase.HalfAtRight);            //上弦月
            Show(MoonPhase.ThreeQuartersAtRight);   //渐盈凸月
            Show(MoonPhase.Full);                   //满月
            Show(MoonPhase.ThreeQuartersAtLeft);    //渐亏凸月
            Show(MoonPhase.HalfAtLeft);             //下弦月
            Show(MoonPhase.QuarterAtLeft);          //残月
            #endregion
            #endregion
            #region 世界模式
            Show(Main.expertMode);              //专家模式, 设置需使用Main.GameModeInfo.IsExpertMode
            Show(Main.masterMode);              //大师模式, 设置需使用Main.GameModeInfo.IsMasterMode
            Show(Main.GameModeInfo.IsJourneyMode);  //旅行模式
            #region Game mode info
            Show(Main.GameModeInfo.DebuffTimeMultiplier);       //debuff时间乘数
            Show(Main.GameModeInfo.EnemyDamageMultiplier);      //敌人伤害乘数
            Show(Main.GameModeInfo.EnemyDefenseMultiplier);     //敌人防御乘数
            Show(Main.GameModeInfo.EnemyMaxLifeMultiplier);     //敌人最大生命乘数
            Show(Main.GameModeInfo.EnemyMoneyDropMultiplier);   //敌人掉钱乘数
            Show(Main.GameModeInfo.KnockbackToEnemiesMultiplier);//对敌人的击退乘数
            Show(Main.GameModeInfo.TownNPCDamageMultiplier);    //城镇NPC伤害乘数
            Show(Main.GameModeInfo.IsJourneyMode);              //旅行模式
            Show(Main.GameModeInfo.IsExpertMode);               //专家模式
            Show(Main.GameModeInfo.IsMasterMode);               //大师模式
            #endregion
            #endregion
            #region tile
            Show(Main.tile);                    //tile二维数组, 用Main.tile[i, j]访问
            Show(Main.tileSolid);               //是否为实心块
            Show(Main.tileSolidTop);            //是否为顶端可站(需对于Main.tileSolid为true?)
            Show(Main.tileMergeDirt);           //是否与土块融合
            Show(Main.tileBlockLight);          //是否发光(?)
            Show(Main.tileLighted);             //是否发光(?)
            Show(Main.tileShine);               //反映微小粒子在物块旁边出现的频率, 此值越大频率越小
            Show(Main.tileFrameImportant);      //是否区分不同帧(?) 作用于用Style的物块(?)
            #endregion
            #region 事件
            Show(Main.invasionType = InvasionID.GoblinArmy);    //哥布林军队
            Show(Main.invasionType = InvasionID.SnowLegion);    //雪人军团
            Show(Main.invasionType = InvasionID.PirateInvasion);//海盗
            Show(Main.invasionType = InvasionID.MartianMadness);//火星人
            Show(Main.raining);                 //下雨
            Show(Main.pumpkinMoon);             //南瓜月
            Show(Main.snowMoon);                //雪月
            Show(Main.slimeRain);               //史莱姆雨
            Show(BirthdayParty.PartyIsUp);      //派对
            Show(BirthdayParty.GenuineParty || BirthdayParty.ManualParty);  //派对
            Show(Sandstorm.Happening);          //沙尘暴
            Show(DD2Event.Ongoing);             //旧日军团
            Show(Main.forceHalloweenForToday);  //强制万圣节
            Show(Main.forceXMasForToday);       //强制圣诞节
            Show(Main.fastForwardTimeToDawn);   //日晷
            Show(Main.sundialCooldown);         //日晷冷却
            Show(Main.fastForwardTimeToDusk);   //月晷
            Show(Main.moondialCooldown);        //月晷冷却
            Show(Main.xMas);                    //圣诞
            Show(Main.halloween);               //万圣
            Show(Main.eclipse);                 //日食
            Show(Main.IsItStorming);            //暴雨(泰拉有这东西? 待测试)(是不是说沙暴啊)
            Show(LanternNight.LanternsUp);      //灯笼夜
            Show(Main.IsItAHappyWindyDay);      //快乐大风天(没见过的东西)
            #endregion
            #region 世界种子
            Show(Main.drunkWorld);              //醉酒
            Show(Main.bottomWorld);             //颠倒
            Show(Main.zenithWorld);             //天顶
            Show(Main.getGoodWorld);            //for the worthy
            Show(Main.tenthAnniversaryWorld);   //十周年
            Show(Main.dontStarveWorld);         //饥荒
            Show(Main.notTheBeesWorld);         //没有蜜蜂
            Show(Main.remixWorld);              //混合
            Show(Main.noTrapsWorld);            //没有陷阱
            #endregion
            #region 关于高度信息
            Show(Main.worldSurface * 0.35); //分隔天空和地上(Sky and Overworld)
            Show(Main.worldSurface);    //地面高度, 分隔地上和地下(Overworld and DirLayer)
            Show(Main.rockLayer);       //分隔泥土层和岩石层(DirtLayer and RockLayer)
            Show(Main.maxTilesY - 200); //分隔岩石层和地狱(RockLayer and Underworld)    (地狱固定200格还是有点欠考虑...不过就这样了)
            #endregion
            #region 其他
            Main.instance.MouseText("some text");   //在鼠标上显示信息(持续一帧)
            #endregion
        }
        public static void ShowMainUpdate() {
            #region params
            //请折叠此段
            Main main = null;
            Game _base = null;
            GameTime gameTime = null;
            bool IsEnginePreloaded = false;
            Action OnEnginePreload = null;
            bool _isDrawingOrUpdating = false;
            bool GameAskedToQuit = false;
            Action Main_OnTickForThirdPartySoftwareOnly = null;
            bool _hasPendingNetmodeChange = false;
            int _targetNetMode = 0;
            float logoRotation = 0f;
            float logoRotationSpeed = 0f;
            float logoScale = 1f;
            Func<bool> Main_CanPauseGame = null;
            Action Main_OnTickForInternalCodeOnly = null;
            double _partialWorldEventUpdates = 0.0;
            Stopwatch _worldUpdateTimeTester = default;
            float cameraLerp = 0f;
            int cameraLerpTimer = 0;
            int cameraLerpTimeToggle = 0;
            #endregion
            //Main.Update(GameTime gameTime)的完整逻辑
            if (!/*main.*/IsEnginePreloaded) {    //Main.IsEnginePreloaded
                IsEnginePreloaded = true;
                /*Main.*/
                OnEnginePreload?.Invoke(); //Main.OnEnginePreload
            }

            if (!/*main.*/_isDrawingOrUpdating) {
                _isDrawingOrUpdating = true;
                Do(/*main.DoUpdate(ref gameTime)*/() => {
                    Main.gameTimeCache = gameTime;
                    if (Main.showSplash) {
                        ShowMainUpdateAudio();
                        Main.GlobalTimeWrappedHourly = (float)(gameTime.TotalGameTime.TotalSeconds % 3600.0);
                        ChromaInitializer.UpdateEvents();
                        Main.Chroma.Update(Main.GlobalTimeWrappedHourly);
                        return;
                    }

                    PartySky.MultipleSkyWorkaroundFix = true;
                    Main.LocalPlayer.cursorItemIconReversed = false;
                    if (!Main.GlobalTimerPaused) {
                        Main.GlobalTimeWrappedHourly = (float)(gameTime.TotalGameTime.TotalSeconds % 3600.0);
                    }
                    Do(/*main.UpdateCreativeGameModeOverride()*/() => {

                    });
                    Do(/*main.UpdateWorldPreparationState()*/() => {

                    });
                    if (Player.BlockInteractionWithProjectiles > 0 && !Main.mouseRight && Main.mouseRightRelease) {
                        Player.BlockInteractionWithProjectiles--;
                    }
                    PlayerInput.SetZoom_UI();
                    for (int num = Main.DelayedProcesses.Count - 1; num >= 0; num--) {
                        IEnumerator enumerator = Main.DelayedProcesses[num];
                        if (!enumerator.MoveNext())
                            Main.DelayedProcesses.Remove(enumerator);
                    }

                    if (!Main.gameMenu || Main.menuMode != MenuID.FancyUI) {
                        Main.MenuUI.SetState(null);
                    }
                    else {
                        Main.InGameUI.SetState(null);
                    }

                    Main.CurrentInputTextTakerOverride = null;
                    if (!Main.dedServ) {
                        Main.AchievementAdvisor.Update();
                    }

                    PlayerInput.SetZoom_Unscaled();
                    Dos(/*main.MouseOversTryToClear()*/);
                    PlayerInput.ResetInputsOnActiveStateChange();
                    if (!Main.dedServ) {
                        Main_OnTickForThirdPartySoftwareOnly?.Invoke(); //Main.OnTickForThirdPartySoftwareOnly
                    }
                    if (/*Main.*/_hasPendingNetmodeChange) {
                        Main.netMode = /*Main.*/_targetNetMode;
                        _hasPendingNetmodeChange = false;
                    }

                    if (CaptureManager.Instance.IsCapturing) {
                        return;
                    }

                    Main.ActivePlayerFileData?.UpdatePlayTimer();

                    Netplay.UpdateInMainThread();
                    Main.gameInactive = !_base.IsActive;
                    if (Main.changeTheTitle) {
                        Main.changeTheTitle = false;
                        Dos(/*main.SetTitle()*/);
                    }

                    Dos(/*_worldUpdateTimeTester.Restart()*/);
                    if (!WorldGen.gen) {
                        WorldGen.destroyObject = false;
                    }

                    if (Main.gameMenu) {
                        Main.mapFullscreen = false;
                    }

                    Dos(/*main.UpdateSettingUnlocks()*/);
                    if (Main.dedServ) {
                        if (Main.dedServFPS) {
                            Main.updatesCountedForFPS++;
                            if (!Main.fpsTimer.IsRunning)
                                Main.fpsTimer.Restart();

                            if (Main.fpsTimer.ElapsedMilliseconds >= 1000) {
                                Main.dedServCount1 += Main.updatesCountedForFPS;
                                Main.dedServCount2++;
                                float num2 = Main.dedServCount1 / (float)Main.dedServCount2;
                                Console.WriteLine(Main.updatesCountedForFPS + "  (" + num2 + ")");
                                Main.updatesCountedForFPS = 0;
                                Main.fpsTimer.Restart();
                            }
                        }
                        else {
                            if (Main.fpsTimer.IsRunning)
                                Main.fpsTimer.Stop();

                            Main.updatesCountedForFPS = 0;
                        }
                    }

                    Dos(/*LocalizationLoader.Update()*/);
                    Dos(/*main.DoUpdate_AutoSave()*/);
                    if (!Main.dedServ) {
                        ChromaInitializer.UpdateEvents();
                        Main.Chroma.Update(Main.GlobalTimeWrappedHourly);
                        if (Main.superFast) {
                            _base.IsFixedTimeStep = false;
                            Main.graphics.SynchronizeWithVerticalRetrace = false;
                        }
                        else {
                            if (Main.FrameSkipMode == FrameSkipMode.Off || Main.FrameSkipMode == FrameSkipMode.Subtle) {
                                if (_base.IsActive)
                                    _base.IsFixedTimeStep = false;
                                else
                                    _base.IsFixedTimeStep = true;
                            }
                            else {
                                _base.IsFixedTimeStep = true;
                                Main.graphics.SynchronizeWithVerticalRetrace = true;
                            }

                            Main.graphics.SynchronizeWithVerticalRetrace = true;
                        }

                        if (Main.showSplash) {
                            return;
                        }

                        Main.updatesCountedForFPS++;
                        if (Main.fpsTimer.ElapsedMilliseconds >= 1000) {
                            if (Main.fpsCount >= 30f + 30f * Main.gfxQuality) {
                                Main.gfxQuality += Main.gfxRate;
                                Main.gfxRate += 0.005f;
                            }
                            else if (Main.fpsCount < 29f + 30f * Main.gfxQuality) {
                                Main.gfxRate = 0.01f;
                                Main.gfxQuality -= 0.1f;
                            }

                            if (Main.gfxQuality < 0f)
                                Main.gfxQuality = 0f;

                            if (Main.gfxQuality > 1f)
                                Main.gfxQuality = 1f;

                            if (Main.maxQ && _base.IsActive) {
                                Main.gfxQuality = 1f;
                                Main.maxQ = false;
                            }

                            Main.updateRate = Main.uCount;
                            Main.frameRate = Main.fpsCount;
                            Main.fpsCount = 0;
                            Main.fpsTimer.Restart();
                            Main.updatesCountedForFPS = 0;
                            Main.drawsCountedForFPS = 0;
                            Main.uCount = 0;
                            if (Main.gfxQuality < 0.8f)
                                Main.mapTimeMax = (int)((1f - Main.gfxQuality) * 60f);
                            else
                                Main.mapTimeMax = 0;
                        }

                        if (Main.FrameSkipMode == FrameSkipMode.Off || Main.FrameSkipMode == FrameSkipMode.Subtle) {
                            Main.UpdateTimeAccumulator += gameTime.ElapsedGameTime.TotalSeconds;
                            if (Main.UpdateTimeAccumulator < 0.01666666753590107 && !Main.superFast) {
                                if (Main.FrameSkipMode == FrameSkipMode.Subtle)
                                    Main.instance.SuppressDraw();

                                return;
                            }

                            gameTime = new GameTime(gameTime.TotalGameTime, new TimeSpan(166666L));
                            Main.UpdateTimeAccumulator -= 0.01666666753590107;
                            Main.UpdateTimeAccumulator = Math.Min(Main.UpdateTimeAccumulator, 0.01666666753590107);
                        }

                        Main.uCount++;
                        Main.drawSkip = false;
                        PlayerInput.AllowExecutionOfGamepadInstructions = true;
                        Dos(/*main.TryPlayingCreditsRoll()*/);
                        PlayerInput.SetZoom_UI();
                        Dos(/*UpdateUIStates(gameTime)*/);
                        PlayerInput.SetZoom_Unscaled();
                        Terraria.Graphics.Effects.Filters.Scene.Update(gameTime);
                        Overlays.Scene.Update(gameTime);
                        LiquidRenderer.Instance.Update(gameTime);
                        ShowMainUpdateAudio();
                        InGameNotificationsTracker.Update();
                        ItemSlot.UpdateInterface();
                        if (Main.teamCooldown > 0)
                            Main.teamCooldown--;

                        Do(/*main.DoUpdate_AnimateBackgrounds()*/() => {

                        });
                        Animation.UpdateAll();
                        if (Main.qaStyle == 1)
                            Main.gfxQuality = 1f;
                        else if (Main.qaStyle == 2)
                            Main.gfxQuality = 0.5f;
                        else if (Main.qaStyle == 3)
                            Main.gfxQuality = 0f;

                        Main.maxDustToDraw = (int)(6000f * (Main.gfxQuality * 0.7f + 0.3f));
                        if (Main.gfxQuality < 0.9)
                            Main.maxDustToDraw = (int)(Main.maxDustToDraw * Main.gfxQuality);

                        if (Main.maxDustToDraw < 1000)
                            Main.maxDustToDraw = 1000;

                        Gore.goreTime = (int)(600f * Main.gfxQuality);
                        if (!WorldGen.gen) {
                            Liquid.cycles = (int)(17f - 10f * Main.gfxQuality);
                            Liquid.curMaxLiquid = (int)(Liquid.maxLiquid * 0.25 + Liquid.maxLiquid * 0.75 * Main.gfxQuality);
                            if (Main.Setting_UseReducedMaxLiquids)
                                Liquid.curMaxLiquid = (int)(2500f + 2500f * Main.gfxQuality);
                        }

                        if (Main.superFast) {
                            Main.graphics.SynchronizeWithVerticalRetrace = false;
                            Main.drawSkip = false;
                        }

                        if (Main.gfxQuality < 0.2)
                            LegacyLighting.RenderPhases = 8;
                        else if (Main.gfxQuality < 0.4)
                            LegacyLighting.RenderPhases = 7;
                        else if (Main.gfxQuality < 0.6)
                            LegacyLighting.RenderPhases = 6;
                        else if (Main.gfxQuality < 0.8)
                            LegacyLighting.RenderPhases = 5;
                        else
                            LegacyLighting.RenderPhases = 4;

                        if (!WorldGen.gen && Liquid.quickSettle) {
                            Liquid.curMaxLiquid = Liquid.maxLiquid;
                            if (Main.Setting_UseReducedMaxLiquids)
                                Liquid.curMaxLiquid = 5000;

                            Liquid.cycles = 1;
                        }

                        if (WorldGen.tenthAnniversaryWorldGen && !Main.gameMenu) {
                            WorldGen.tenthAnniversaryWorldGen = false;
                        }

                        if (WorldGen.drunkWorldGen || WorldGen.remixWorldGen) {
                            if (!Main.gameMenu) {
                                WorldGen.drunkWorldGen = false;
                                WorldGen.remixWorldGen = false;
                                /*main.*/
                                logoRotation = 0f;
                                /*main.*/
                                logoRotationSpeed = 0f;
                                /*main.*/
                                logoScale = 1f;
                            }
                        }
                        else if (Main.gameMenu && Math.Abs(logoRotationSpeed) > 1000f) {
                            logoRotation = 0f;
                            logoRotationSpeed = 0f;
                            logoScale = 1f;
                        }

                        Do(/*main.UpdateOldNPCShop()*/() => {

                        });
                        Main.hasFocus = _base.IsActive;
#if false   // if !NETCORE
                        if (Platform.IsWindows) {
                            Form form = Control.FromHandle(base.Window.Handle) as Form;
                            bool num3 = form.WindowState == FormWindowState.Minimized;
                            bool flag = Form.ActiveForm == form;
                            hasFocus |= flag;
                            if (num3)
                                hasFocus = false;
                        }
#endif

                        if (!Main.hasFocus && Main.netMode == NetmodeID.SinglePlayer) {
                            if (!Platform.IsOSX)
                                _base.IsMouseVisible = true;

                            if (Main.netMode != NetmodeID.Server && Main.myPlayer >= 0)
                                Main.player[Main.myPlayer].delayUseItem = true;

                            Main.mouseLeftRelease = false;
                            Main.mouseRightRelease = false;
                            // Reset TML-introduced extra buttons
                            Main.mouseMiddleRelease = false;
                            Main.mouseXButton1Release = false;
                            Main.mouseXButton2Release = false;

                            if (Main.gameMenu) {
                                Dos(/*main.UpdateMenu()*/);
                            }

                            Main.gamePaused = true;
                            return;
                        }

                        if (!Platform.IsOSX)
                            _base.IsMouseVisible = false;

                        SkyManager.Instance.Update(gameTime);
                        if (!Main.gamePaused)
                            EmoteBubble.UpdateAll();

                        ScreenObstruction.Update();
                        ScreenDarkness.Update();
                        MoonlordDeathDrama.Update();
                        Dos(/*main.DoUpdate_AnimateCursorColors()*/);
                        Dos(/*main.DoUpdate_AnimateTileGlows()*/);
                        Dos(/*main.DoUpdate_AnimateDiscoRGB()*/);
                        Dos(/*main.DoUpdate_AnimateVisualPlayerAura()*/);
                        Dos(/*main.DoUpdate_AnimateWaterfalls()*/);
                        Dos(/*main.DoUpdate_AnimateWalls()*/);
                        Dos(/*main.AnimateTiles()*/);
                        Dos(/*main.DoUpdate_AnimateItemIcons()*/);
                        Dos(/*main.DoUpdate_F10_ToggleFPS()*/);
                        Dos(/*main.DoUpdate_F9_ToggleLighting()*/);
                        Dos(/*main.DoUpdate_F8_ToggleNetDiagnostics()*/);
                        Dos(/*main.DoUpdate_F7_ToggleGraphicsDiagnostics()*/);
                        Dos(/*main.DoUpdate_F11_ToggleUI()*/);
                        Dos(/*main.DoUpdate_AltEnter_ToggleFullscreen()*/);
                        Dos(/*main.DoUpdate_HandleInput()*/);
                        Dos(/*main.DoUpdate_HandleChat()*/);
                        Dos(/*main.DoUpdate_Enter_ToggleChat()*/);
                        if ((Main.timeForVisualEffects += 1.0) >= 216000.0)
                            Main.timeForVisualEffects = 0.0;

                        if (Main.gameMenu) {
                            Dos(/*main.UpdateMenu()*/);
                            if (Main.netMode != NetmodeID.Server)
                                return;

                            Main.gamePaused = false;
                        }

                        if (!Main.CanUpdateGameplay && Main.netMode != NetmodeID.Server)
                            return;

                        Main.CheckInvasionProgressDisplay();
                    }

                    Dos(/*main.UpdateWindyDayState()*/);
                    if (Main.netMode == NetmodeID.Server)
                        Main.cloudAlpha = Main.maxRaining;

                    bool isActive = _base.IsActive;
                    if (Main.netMode == NetmodeID.MultiplayerClient) {
                        Dos(/*main.TrySyncingMyPlayer()*/);
                    }

                    if (Main_CanPauseGame()) {  //Main.CanPauseGame()
                        Dos(/*main.DoUpdate_WhilePaused()*/);
                        PlayerLoader.UpdateAutopause(Main.player[Main.myPlayer]);
                        Main.gamePaused = true;
                        return;
                    }

                    Main.gamePaused = false;
                    Main_OnTickForInternalCodeOnly?.Invoke();   //Main.OnTickForInternalCodeOnly()

                    if ((Main.dedServ || (Main.netMode != NetmodeID.MultiplayerClient && !Main.gameMenu && !Main.gamePaused)) && Main.AmbienceServer != null)
                        Main.AmbienceServer.Update();

                    WorldGen.BackgroundsCache.UpdateFlashValues();
                    Main.LocalGolfState?.Update();

                    if ((isActive || Main.netMode == NetmodeID.MultiplayerClient) && Main.cloudAlpha > 0f)
                        Rain.MakeRain();

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Main.instance.updateCloudLayer();

                    if (!(Main.desiredWorldEventsUpdateRate <= 0.0)) {
                        /*main.*/
                        _partialWorldEventUpdates += Main.desiredWorldEventsUpdateRate;
                        Main.worldEventUpdates = (int)_partialWorldEventUpdates;
                        _partialWorldEventUpdates -= Main.worldEventUpdates;
                        for (int i = 0; i < Main.worldEventUpdates; i++) {
                            Main.instance.UpdateWeather(gameTime, i);
                        }
                    }

                    Dos(/*Main.UnpausedUpdateSeed = Utils.RandomNextSeed(Main.UnpausedUpdateSeed)*/);
                    Main.Ambience();
                    if (Main.netMode != NetmodeID.Server) {
                        try {
                            Main.snowing();
                        }
                        catch {
                            if (!Main.ignoreErrors) {
                                throw;
                            }
                        }

                        Sandstorm.EmitDust();
                    }

                    SystemLoader.PreUpdateEntities();
                    if (Main.netMode != NetmodeID.Server) {
                        if (Main.screenPosition.Y < Main.worldSurface * 16.0 + 16.0) {
                            Star.UpdateStars();
                            Cloud.UpdateClouds();
                        }
                        else if (Main.shimmerAlpha > 0f) {
                            Star.UpdateStars();
                            int num2 = Main.rand.Next(Main.numStars);
                            if (Main.rand.NextBool(90)) {
                                if (Main.star[num2] != null && !Main.star[num2].hidden && !Main.star[num2].falling) {
                                    Main.star[num2].Fall();
                                }

                                for (int j = 0; j < Main.numStars; j++) {
                                    if (Main.star[j].hidden) {
                                        Star.SpawnStars(j);
                                    }
                                }
                            }
                        }
                    }

                    PortalHelper.UpdatePortalPoints();
                    LucyAxeMessage.UpdateMessageCooldowns();
                    if (Main.instance.ShouldUpdateEntities()) {
                        Do(/*main.DoUpdateInWorld(main._worldUpdateTimeTester)*/() => {
                            Dos(/*main.UpdateParticleSystems()*/);
                            Main.tileSolid[379] = false;
                            Dos(/*NPCShopDatabase.Test()*/);
                            #region UpdatePlayers
                            SystemLoader.PreUpdatePlayers();    //ModSystems.PreUpdatePlayers()
                            int num = 0;
                            int num2 = 0;
                            Main.sittingManager.ClearPlayerAnchors();
                            Main.sleepingManager.ClearPlayerAnchors();
                            for (int i = 0; i < 255; i++) {
                                if (!Main.player[i].active) {
                                    continue;
                                }
                                Dos(() => Main.player[i].Update(i), () => {

                                });
                                if (Main.player[i].active) {
                                    num++;
                                    if (Main.player[i].sleeping.FullyFallenAsleep) {
                                        num2++;
                                    }
                                }
                            }

                            Main.CurrentFrameFlags.ActivePlayersCount = num;
                            Main.CurrentFrameFlags.SleepingPlayersCount = num2;
                            if (Main.netMode != NetmodeID.Server) {
                                int num3 = Main.myPlayer;
                                if (Main.player[num3].creativeGodMode) {
                                    Main.player[num3].statLife = Main.player[num3].statLifeMax2;
                                    Main.player[num3].statMana = Main.player[num3].statManaMax2;
                                    Main.player[num3].breath = Main.player[num3].breathMax;
                                }
                            }
                            SystemLoader.PostUpdatePlayers();   //ModSystems.PostUpdatePlayers()
                            #endregion
                            Dos(/*Main._gameUpdateCount++*/);
                            #region UpdateNPCs
                            SystemLoader.PreUpdateNPCs();
                            NPC.RevengeManager.Update();
                            if (Main.netMode != NetmodeID.MultiplayerClient) {
                                if (Main.remixWorld) {
                                    NPC.SetRemixHax();
                                }

                                try {
                                    NPC.SpawnNPC();
                                }
                                catch {
                                    // ignored
                                }

                                if (Main.remixWorld) {
                                    NPC.ResetRemixHax();
                                }
                            }

                            if (Main.netMode != NetmodeID.MultiplayerClient) {
                                PressurePlateHelper.Update();
                            }

                            for (int j = 0; j < 255; j++) {
                                Main.player[j].nearbyActiveNPCs = 0f;
                                Main.player[j].townNPCs = 0f;
                            }
                            Dos(/*Main.CheckBossIndexes()*/);
                            Main.sittingManager.ClearNPCAnchors();
                            Main.sleepingManager.ClearNPCAnchors();
                            NPC.taxCollector = false;
                            NPC.ClearFoundActiveNPCs();
                            NPC.UpdateFoundActiveNPCs();
                            FixExploitManEaters.Update();
                            if (Main.netMode != NetmodeID.MultiplayerClient) {
                                Main.BestiaryTracker.Sights.ScanWorldForFinds();
                            }

                            bool anyActiveBossNPC = false;
                            if (NPC.offSetDelayTime > 0) {
                                NPC.offSetDelayTime--;
                            }

                            if (Main.remixWorld && NPC.empressRageMode && !NPC.AnyNPCs(636)) {
                                NPC.empressRageMode = false;
                            }

                            if (Main.netMode != NetmodeID.MultiplayerClient && Main.afterPartyOfDoom && !BirthdayParty.PartyIsUp) {
                                for (int k = 0; k < 200; k++) {
                                    NPC nPC = Main.npc[k];
                                    if (nPC.active && nPC.townNPC && nPC.type != NPCID.OldMan && nPC.type != NPCID.SkeletonMerchant && nPC.type != NPCID.TravellingMerchant) {
                                        Dos(/*nPC.StrikeNPCNoInteraction(9999, 10f, -nPC.direction)*/);
                                        if (Main.netMode == NetmodeID.Server) {
                                            NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, k, 9999f, 10f, -nPC.direction);
                                        }
                                    }
                                }

                                NPC.unlockedPartyGirlSpawn = false;
                                NPC.unlockedPrincessSpawn = false;
                                NPC.unlockedSlimeRainbowSpawn = false;
                                NPC.unlockedSlimeGreenSpawn = false;
                                Main.afterPartyOfDoom = false;
                            }

                            if (NPC.brainOfGravity >= 0 && NPC.brainOfGravity < 200 && (!Main.npc[NPC.brainOfGravity].active || Main.npc[NPC.brainOfGravity].type != NPCID.BrainofCthulhu)) {
                                NPC.brainOfGravity = -1;
                            }

                            for (int l = 0; l < 200; l++) {
                                if (Main.ignoreErrors) {
                                    try {
                                        Main.npc[l].UpdateNPC(l);
                                        if (Main.npc[l].active && (Main.npc[l].boss || NPCID.Sets.DangerThatPreventsOtherDangers[Main.npc[l].type])) {
                                            anyActiveBossNPC = true;
                                        }
                                    }
                                    catch (Exception) {
                                        Main.npc[l] = new NPC();
                                    }
                                }
                                else {
                                    Main.npc[l].UpdateNPC(l);
                                }
                            }

                            Main.CurrentFrameFlags.AnyActiveBossNPC = anyActiveBossNPC;
                            SystemLoader.PostUpdateNPCs();
                            #endregion
                            #region UpateGores
                            SystemLoader.PreUpdateGores();
                            for (int m = 0; m < 600; m++) {
                                if (Main.ignoreErrors) {
                                    try {
                                        Main.gore[m].Update();
                                    }
                                    catch {
                                        Main.gore[m] = new Gore();
                                    }
                                }
                                else {
                                    Main.gore[m].Update();
                                }
                            }
                            SystemLoader.PostUpdateGores();
                            #endregion
                            #region UpdateProjectiles
                            SystemLoader.PreUpdateProjectiles();
                            LockOnHelper.SetUP();
                            Main.CurrentFrameFlags.HadAnActiveInteractibleProjectile = false;
                            Dos(/*main.PreUpdateAllProjectiles()*/);
                            for (int n = 0; n < 1000; n++) {
                                Main.ProjectileUpdateLoopIndex = n;
                                if (Main.ignoreErrors) {
                                    try {
                                        Main.projectile[n].Update(n);
                                    }
                                    catch {
                                        Main.projectile[n] = new Projectile();
                                    }
                                }
                                else {
                                    Main.projectile[n].Update(n);
                                }
                            }

                            Main.ProjectileUpdateLoopIndex = -1;
                            Dos(/*main.PostUpdateAllProjectiles()*/);
                            LockOnHelper.SetDOWN();
                            SystemLoader.PostUpdateProjectiles();
                            #endregion
                            #region UpdateItems
                            SystemLoader.PreUpdateItems();
                            Item.numberOfNewItems = 0;
                            for (int num4 = 0; num4 < 400; num4++) {
                                if (Main.ignoreErrors) {
                                    try {
                                        Main.item[num4].UpdateItem(num4);
                                    }
                                    catch {
                                        Main.item[num4] = new Item();
                                    }
                                }
                                else {
                                    Main.item[num4].UpdateItem(num4);
                                }
                            }
                            SystemLoader.PostUpdateItems();
                            #endregion
                            #region UpdateDusts
                            SystemLoader.PreUpdateDusts();
                            if (Main.ignoreErrors) {
                                try {
                                    Dust.UpdateDust();
                                }
                                catch {
                                    for (int num5 = 0; num5 < 6000; num5++) {
                                        Main.dust[num5] = new() {
                                            dustIndex = num5
                                        };
                                    }
                                }
                            }
                            else {
                                Dust.UpdateDust();
                            }
                            SystemLoader.PostUpdateDusts();
                            #endregion
                            if (Main.netMode != NetmodeID.Server) {
                                CombatText.UpdateCombatText();
                                PopupText.UpdateItemText();
                            }
                            #region UpdateTime
                            SystemLoader.PreUpdateTime();
                            Dos(/*Main.UpdateTime()*/);
                            SystemLoader.PostUpdateTime();
                            #endregion
                            Main.tileSolid[379] = true;
                            if (Main.gameMenu && Main.netMode != NetmodeID.Server) {
                                return;
                            }

                            if (Main.netMode != NetmodeID.MultiplayerClient) {
                                WorldGen.UpdateWorld();
                                Dos(/*Main.UpdateInvasion()*/);
                            }

                            if (Main.netMode == NetmodeID.Server) {
                                Dos(/*Main.UpdateServer()*/);
                            }

                            if (Main.netMode == NetmodeID.MultiplayerClient) {
                                Dos(/*Main.UpdateClient()*/);
                            }

                            SystemLoader.PostUpdateEverything();
                            Main.chatMonitor.Update();
                            Main.upTimer = (float)_worldUpdateTimeTester.Elapsed.TotalMilliseconds;
                            if (Main.upTimerMaxDelay > 0f) {
                                Main.upTimerMaxDelay -= 1f;
                            }
                            else {
                                Main.upTimerMax = 0f;
                            }

                            if (Main.upTimer > Main.upTimerMax) {
                                Main.upTimerMax = Main.upTimer;
                                Main.upTimerMaxDelay = 400f;
                            }

                            Chest.UpdateChestFrames();
                            Dos(/*main._ambientWindSys.Update()*/);
                            Main.instance.TilesRenderer.Update();
                            Main.instance.WallsRenderer.Update();
                            if (/*Main.*/cameraLerp > 0f) {
                                /*Main.*/
                                cameraLerpTimer++;
                                if (cameraLerpTimer >= /*Main.*/cameraLerpTimeToggle) {
                                    cameraLerp += ((cameraLerpTimer - cameraLerpTimeToggle) / 3 + 1) * 0.001f;
                                }

                                if (cameraLerp > 1f) {
                                    cameraLerp = 1f;
                                }
                            }
                        });
                    }

                    if (Main.netMode != NetmodeID.Server) {
                        Main.ChromaPainter.Update();
                    }
                });//这里会捕捉所有报错并Log出来(Logging.Terraria.Error(e))
                Dos(() => CinematicManager.Instance.Update(gameTime), () => {

                });
                #region 网络发包
                if (Main.netMode == NetmodeID.Server) {
                    for (int i = 0; i < 256; i++) {
                        Netplay.Clients[i].Socket?.SendQueuedPackets();
                    }
                }
                else if (Main.netMode == NetmodeID.MultiplayerClient) {
                    Netplay.Connection.Socket.SendQueuedPackets();
                }
                #endregion
                _isDrawingOrUpdating = false;
            }

            Dos(/*base.Update(gameTime)*/);
            Do(/*main.ConsumeAllMainThreadActions()*/() => {

            });
            if (GameAskedToQuit) {   //Main.GameAskedToQuit
                Dos(/*Main.QuitGame()*/);
            }
        }
        public static void ShowMainUpdateAudio() {

        }
        public static void ShowMainDoUpdateInWorldSimple() {
            #region params
            Stopwatch _worldUpdateTimeTester = default;
            #endregion
            Dos(/*main.UpdateParticleSystems()*/);
            Main.tileSolid[379] = false;
            Dos(/*NPCShopDatabase.Test()*/);
            #region UpdatePlayers
            SystemLoader.PreUpdatePlayers();    //ModSystems.PreUpdatePlayers()
            int num = 0;
            int num2 = 0;
            Main.sittingManager.ClearPlayerAnchors();
            Main.sleepingManager.ClearPlayerAnchors();
            for (int i = 0; i < 255; i++) {
                Player player = Main.player[i];
                if (!player.active) {
                    continue;
                }
                player.Update(i);
                //重置运动相关的参数(runAcceleration, gravity等)
                //设置运动相关的参数(根据shimmerWet, wet, vortexDebuff等值)
                //重置Hitbox
                PlayerLoader.PreUpdate(player);
                player.UpdateBiomes();
                player.UpdateMinionTarget();
            }
            SystemLoader.PostUpdatePlayers();   //ModSystems.PostUpdatePlayers()
            #endregion
            Dos(/*Main._gameUpdateCount++*/);
            #region UpdateNPCs
            SystemLoader.PreUpdateNPCs();
            NPC.RevengeManager.Update();
            if (Main.netMode != NetmodeID.MultiplayerClient) {
                if (Main.remixWorld) {
                    NPC.SetRemixHax();
                }

                try {
                    NPC.SpawnNPC();
                }
                catch {

                }

                if (Main.remixWorld) {
                    NPC.ResetRemixHax();
                }
            }

            if (Main.netMode != NetmodeID.MultiplayerClient) {
                PressurePlateHelper.Update();
            }

            for (int j = 0; j < 255; j++) {
                Main.player[j].nearbyActiveNPCs = 0f;
                Main.player[j].townNPCs = 0f;
            }
            Dos(/*Main.CheckBossIndexes()*/);
            Main.sittingManager.ClearNPCAnchors();
            Main.sleepingManager.ClearNPCAnchors();
            NPC.taxCollector = false;
            NPC.ClearFoundActiveNPCs();
            NPC.UpdateFoundActiveNPCs();
            FixExploitManEaters.Update();
            if (Main.netMode != NetmodeID.MultiplayerClient) {
                Main.BestiaryTracker.Sights.ScanWorldForFinds();
            }

            bool anyActiveBossNPC = false;
            if (NPC.offSetDelayTime > 0) {
                NPC.offSetDelayTime--;
            }

            if (Main.remixWorld && NPC.empressRageMode && !NPC.AnyNPCs(636)) {
                NPC.empressRageMode = false;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient && Main.afterPartyOfDoom && !BirthdayParty.PartyIsUp) {
                for (int k = 0; k < 200; k++) {
                    NPC nPC = Main.npc[k];
                    if (nPC.active && nPC.townNPC && nPC.type != NPCID.OldMan && nPC.type != NPCID.SkeletonMerchant && nPC.type != NPCID.TravellingMerchant) {
                        Dos(/*nPC.StrikeNPCNoInteraction(9999, 10f, -nPC.direction)*/);
                        if (Main.netMode == NetmodeID.Server) {
                            NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, k, 9999f, 10f, -nPC.direction);
                        }
                    }
                }

                NPC.unlockedPartyGirlSpawn = false;
                NPC.unlockedPrincessSpawn = false;
                NPC.unlockedSlimeRainbowSpawn = false;
                NPC.unlockedSlimeGreenSpawn = false;
                Main.afterPartyOfDoom = false;
            }

            if (NPC.brainOfGravity >= 0 && NPC.brainOfGravity < 200 && (!Main.npc[NPC.brainOfGravity].active || Main.npc[NPC.brainOfGravity].type != NPCID.BrainofCthulhu)) {
                NPC.brainOfGravity = -1;
            }

            for (int l = 0; l < 200; l++) {
                if (Main.ignoreErrors) {
                    try {
                        Main.npc[l].UpdateNPC(l);
                        if (Main.npc[l].active && (Main.npc[l].boss || NPCID.Sets.DangerThatPreventsOtherDangers[Main.npc[l].type])) {
                            anyActiveBossNPC = true;
                        }
                    }
                    catch (Exception) {
                        Main.npc[l] = new NPC();
                    }
                }
                else {
                    Main.npc[l].UpdateNPC(l);
                }
            }

            Main.CurrentFrameFlags.AnyActiveBossNPC = anyActiveBossNPC;
            SystemLoader.PostUpdateNPCs();
            #endregion
            #region UpateGores
            SystemLoader.PreUpdateGores();
            for (int m = 0; m < 600; m++) {
                if (Main.ignoreErrors) {
                    try {
                        Main.gore[m].Update();
                    }
                    catch {
                        Main.gore[m] = new Gore();
                    }
                }
                else {
                    Main.gore[m].Update();
                }
            }
            SystemLoader.PostUpdateGores();
            #endregion
            #region UpdateProjectiles
            SystemLoader.PreUpdateProjectiles();
            LockOnHelper.SetUP();
            Main.CurrentFrameFlags.HadAnActiveInteractibleProjectile = false;
            Dos(/*main.PreUpdateAllProjectiles()*/);
            for (int n = 0; n < 1000; n++) {
                Main.ProjectileUpdateLoopIndex = n;
                if (Main.ignoreErrors) {
                    try {
                        Main.projectile[n].Update(n);
                    }
                    catch {
                        Main.projectile[n] = new Projectile();
                    }
                }
                else {
                    Main.projectile[n].Update(n);
                }
            }

            Main.ProjectileUpdateLoopIndex = -1;
            Dos(/*main.PostUpdateAllProjectiles()*/);
            LockOnHelper.SetDOWN();
            SystemLoader.PostUpdateProjectiles();
            #endregion
            #region UpdateItems
            SystemLoader.PreUpdateItems();  //ModSystems.PreUpdateItems()
            Item.numberOfNewItems = 0;
            Main.item.WithIndex().ForeachDo(p => Dos(() => p.Item2.UpdateItem(p.Item1), () => {

            }));
            SystemLoader.PostUpdateItems(); //ModSystems.PostUpdateItems()
            #endregion
            #region UpdateDusts
            SystemLoader.PreUpdateDusts();
            Dust.UpdateDust();
            SystemLoader.PostUpdateDusts();
            #endregion
            if (Main.netMode != NetmodeID.Server) {
                CombatText.UpdateCombatText();
                PopupText.UpdateItemText();
            }
            #region UpdateTime
            SystemLoader.PreUpdateTime();
            Dos(/*Main.UpdateTime()*/);
            SystemLoader.PostUpdateTime();
            #endregion
            Main.tileSolid[379] = true;
            if (Main.gameMenu && Main.netMode != NetmodeID.Server) {
                return;
            }
            #region Update world and invasion
            if (Main.netMode != NetmodeID.MultiplayerClient) {
                WorldGen.UpdateWorld();
                Dos(/*Main.UpdateInvasion()*/);
            }
            #endregion
            #region Update server and client
            if (Main.netMode == NetmodeID.Server) {
                Dos(/*Main.UpdateServer()*/);
            }
            if (Main.netMode == NetmodeID.MultiplayerClient) {
                Dos(/*Main.UpdateClient()*/);
            }
            #endregion
            SystemLoader.PostUpdateEverything();    //ModSystems.PostUpdateEverything()
            Main.chatMonitor.Update();
            #region upate Main.upTimer
            Main.upTimer = (float)_worldUpdateTimeTester.Elapsed.TotalMilliseconds;
            if (Main.upTimerMaxDelay > 0f) {
                Main.upTimerMaxDelay -= 1f;
            }
            else {
                Main.upTimerMax = 0f;
            }
            if (Main.upTimer > Main.upTimerMax) {
                Main.upTimerMax = Main.upTimer;
                Main.upTimerMaxDelay = 400f;
            }
            #endregion

            Chest.UpdateChestFrames();
            Dos(/*main._ambientWindSys.Update()*/);
            Main.instance.TilesRenderer.Update();
            Main.instance.WallsRenderer.Update();
        }
    }
    public class WorldGen_cls {
        public static WorldGen worldGen;
        public static void ShowWorldGen() {
            Show(WorldGen.crimson); //是否是猩红(否则为腐化)
        }
    }
    public class Recipe_cls {
        public static Recipe recipe;
        public static void ShowRecipe() {
            #region params
            ModItem modItem = null;
            string recipeGroupName = null;
            RecipeGroup recipeGroup = null;
            int stack = 1, recipeGroupID = RecipeGroupID.Wood, itemID = 0, amount = 1;
            #endregion
            recipe = Recipe.Create(itemID, amount); //以某一物品为产物的配方, 可以额外传入amount代表产出几个
            recipe = modItem.CreateRecipe(amount); //创建一个以自己为产物的配方, 可以传入amount代表产出几个
            recipe.AddIngredient(ItemID.DirtBlock, 10); //添加一种原料
            recipe.AddTile(TileID.WorkBenches); //添加一种物块作为环境(当然可以添加不止一种)
            recipe.AddCondition(Condition.NearWater);   //添加一个条件(此处为在水附近)
            recipe.AddRecipeGroup(recipeGroupName, stack);  //向合成表添加入一个合成组
            recipe.AddRecipeGroup(recipeGroup, stack);
            recipe.AddRecipeGroup(recipeGroupID, stack);
            Show(recipe.createItem);    //可以直接对此进行修改以修改产物, 实际的产物会从它Clone出来
            Show(recipe.requiredItem);  //在游戏完成添加Recipe前可以自由修改, 之后就不要随便改了
            //此处可以对所消耗物品的数量进行修改(最好是调小)
            recipe.AddConsumeItemCallback((Recipe recipe, int type, ref int amount) => { });
            recipe.AddConsumeItemCallback(Recipe.ConsumptionRules.Alchemy); //让每一种材料的每一个物品都有 1/3 的机会不被消耗
            /*
            在制作时做一些事
            consumedItems: 似乎只是克隆出来的, 修改它没法做到修改消耗物
            item: 产物, 是createItem克隆出来的物品, 可以随意修改, 会反映到结果上, 但是还没制作时看不出来
            destinationStack: 往什么上叠, 一般是Main.mouseItem
            */
            recipe.AddOnCraftCallback((recipe, item, consumedItems, destinationStack) => { });
            recipe.Register();  //使用以在游戏中添加此配方

            Show(recipe.RecipeIndex);     //在Main.recipe中的序号, 在被注册(Register)之前为 -1
            Show(Main.recipe);
            //Show(RecipeLoader.FirstRecipeForItem)  //这么好用的东西竟然是internal的, 可惜
        }
    }
    public class RecipeGroup_cls {
        public static RecipeGroup recipeGroup;
        public static void ShowRecipeGroup() {
            string name = null;
            Show(RecipeGroup.recipeGroupIDs);                      //从RecipeGroup的名字转化为id
            Show(RecipeGroup.recipeGroups); //冲id转化为RecipeGroup
            RecipeGroup.RegisterGroup(name, recipeGroup);                    //加载这个合成组, 返回此合成组id(?)

        }
    }
    public class Lighting_cls {
        public static void ShowLighting() {
            #region params
            int x = 0, y = 0, torchID = 0, i = 0, j = 0;
            float lightAmount = 1, r = 0, g = 0, b = 0;
            Vector2 position = default;
            Vector3 rgb = default;
            #endregion
            Lighting.Brightness(x, y);  //获取某处的亮度, 暂不清楚是世界坐标还是物块坐标
            #region AddLight(position, rgb / torchID)     //添加一个光亮, 似乎是瞬时的
            Lighting.AddLight(position, TorchID.Torch);
            Lighting.AddLight(position, rgb);           //暂不清楚rgb的单位
            Lighting.AddLight(i, j, torchID, lightAmount);
            Lighting.AddLight(position, r, g, b);
            Lighting.AddLight(i, j, r, g, b);
            #endregion
            Lighting.GetColor(x, y);    //获取某处(物块坐标?)的颜色
            Show(Lighting.GlobalBrightness);  //整体亮度
        }
    }
    public class Condition_cls {
        public static Condition condition;
        public static void ShowCondition() {
            #region params
            string LocalizationKey = null;
            int itemID = 0, score = 0, npcID = 0;
            #endregion
            Condition conditionFullMoon = new(LocalizationKey, () => Main.moonPhase == (int)MoonPhase.Full);
            condition.IsMet();  //是否满足条件, 同Predict()
            Show(Condition.NearWater);
            Show(Condition.TimeDay);
            Show(Condition.InDungeon);
            Show(Condition.InClassicMode);
            Show(Condition.DrunkWorld);
            Show(Condition.DownedKingSlime);
            Show(Condition.MoonPhaseFull);
            Show(Condition.PlayerCarriesItem(itemID));
            Show(Condition.GolfScoreOver(score));
            Show(Condition.NpcIsPresent(npcID));
        }
    }
}
