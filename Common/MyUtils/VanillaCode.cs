using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Achievements;
using Terraria.Graphics;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace TigerLearning.Common.MyUtils;

public class VanillaCode {
    #region Player.cs
    #region ItemCheck_Shoot
    public static void ItemCheck_Shoot(Player player, int i, Item sItem, int weaponDamage) {
        DebugPrint("ItemCheck_Shoot0");
        if(!CombinedHooks.CanShoot(player, sItem))
            return;
        DebugPrint("ItemCheck_Shoot1");

        int projToShoot = sItem.shoot;
        float speed = sItem.shootSpeed;
        int damage = sItem.damage;
        if(!sItem.noMelee && !ProjectileID.Sets.NoMeleeSpeedVelocityScaling[projToShoot])
            speed /= 1 / player.GetTotalAttackSpeed(DamageClass.Melee);

        // Copied as-is from 1.3
        if(sItem.CountsAsClass(DamageClass.Throwing) && speed < 16f) {
            speed *= player.ThrownVelocity;
            if(speed > 16f)
                speed = 16f;
        }

        bool canShoot = false;
        int Damage = weaponDamage;
        float KnockBack = sItem.knockBack;
        int usedAmmoItemId = 0;
        if(sItem.useAmmo > 0)
            canShoot = player.PickAmmo(sItem, out projToShoot, out speed, out Damage, out KnockBack, out usedAmmoItemId, ItemID.Sets.gunProj[sItem.type]);
        else
            canShoot = true;

        if(ItemID.Sets.gunProj[sItem.type]) {
            KnockBack = sItem.knockBack;
            Damage = weaponDamage;
            speed = sItem.shootSpeed;
        }

        if(sItem.IsACoin)
            canShoot = false;
        DebugPrint($"ItemCheck_Shoot2, canShoot is {canShoot}");

        if(sItem.type == 1254 && projToShoot == 14)
            projToShoot = 242;

        if(sItem.type == 1255 && projToShoot == 14)
            projToShoot = 242;

        if(sItem.type == 1265 && projToShoot == 14)
            projToShoot = 242;

        if(sItem.type == 3542) {
            if(Main.rand.Next(100) < 20) {
                projToShoot++;
                Damage *= 3;
            }
            else {
                speed -= 1f;
            }
        }

        if(sItem.type == 1928)
            Damage = (int)((float)Damage * 1f);

        if(sItem.type == 3063)
            Damage = (int)((float)Damage * 1.25f);

        if(sItem.type == 1306)
            Damage = (int)((double)Damage * 0.67);

        if(sItem.type == 1227)
            Damage = (int)((double)Damage * 0.7);

        if(!canShoot)
            return;
        DebugPrint("ItemCheck_Shoot3");

        // Added by TML. #ItemTimeOnAllClients
        if(player.whoAmI != Main.myPlayer) {
            player.ApplyItemTime(sItem);
            return;
        }
        DebugPrint("ItemCheck_Shoot4");

        KnockBack = player.GetWeaponKnockback(sItem, KnockBack);
        IEntitySource projectileSource_Item_WithPotentialAmmo = player.GetSource_ItemUse_WithPotentialAmmo(sItem, usedAmmoItemId);
        if(projToShoot == 228)
            KnockBack = 0f;

        if(projToShoot == 1 && sItem.type == 120)
            projToShoot = 2;

        if(sItem.type == 682)
            projToShoot = 117;

        if(sItem.type == 725)
            projToShoot = 120;

        if(sItem.type == 2796)
            projToShoot = 442;

        if(sItem.type == 2223)
            projToShoot = 357;

        if(sItem.type == 5117)
            projToShoot = 968;

        if(sItem.fishingPole > 0 && player.overrideFishingBobber > -1)
            projToShoot = player.overrideFishingBobber;

        player.ApplyItemTime(sItem);
        Vector2 pointPoisition = player.RotatedRelativePoint(player.MountedCenter);
        bool flag = true;
        int type = sItem.type;
        if(type == 723 || type == 3611)
            flag = false;

        Vector2 value = Vector2.UnitX.RotatedBy(player.fullRotation);
        Vector2 vector = Main.MouseWorld - pointPoisition;
        Vector2 v = player.itemRotation.ToRotationVector2() * player.direction;
        if(sItem.type == 3852 && !player.ItemAnimationJustStarted)
            vector = (v.ToRotation() + player.fullRotation).ToRotationVector2();

        if(vector != Vector2.Zero)
            vector.Normalize();

        float num = Vector2.Dot(value, vector);
        if(flag) {
            if(num > 0f)
                player.ChangeDir(1);
            else
                player.ChangeDir(-1);
        }

        if(sItem.type == 3094 || sItem.type == 3378 || sItem.type == 3543)
            pointPoisition.Y = player.position.Y + (float)(player.height / 3);

        if(sItem.type == 5117)
            pointPoisition.Y = player.position.Y + (float)(player.height / 3);

        if(sItem.type == 517) {
            pointPoisition.X += (float)Main.rand.Next(-3, 4) * 3.5f;
            pointPoisition.Y += (float)Main.rand.Next(-3, 4) * 3.5f;
        }

        if(sItem.type == 2611) {
            Vector2 vector2 = vector;
            if(vector2 != Vector2.Zero)
                vector2.Normalize();

            pointPoisition += vector2;
        }

        if(sItem.type == 3827)
            pointPoisition += vector.SafeNormalize(Vector2.Zero).RotatedBy((float)player.direction * (-(float)Math.PI / 2f)) * 24f;

        if(projToShoot == 9) {
            pointPoisition = new Vector2(player.position.X + (float)player.width * 0.5f + (float)(Main.rand.Next(201) * -player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
            KnockBack = 0f;
            Damage = (int)((float)Damage * 1.5f);
        }

        if(sItem.type == 986 || sItem.type == 281) {
            pointPoisition.X += 6 * player.direction;
            pointPoisition.Y -= 6f * player.gravDir;
        }

        if(sItem.type == 3007) {
            pointPoisition.X -= 4 * player.direction;
            pointPoisition.Y -= 2f * player.gravDir;
        }

        float num2 = (float)Main.mouseX + Main.screenPosition.X - pointPoisition.X;
        float num3 = (float)Main.mouseY + Main.screenPosition.Y - pointPoisition.Y;
        if(sItem.type == 3852 && !player.ItemAnimationJustStarted) {
            Vector2 vector3 = vector;
            num2 = vector3.X;
            num3 = vector3.Y;
        }

        if(player.gravDir == -1f)
            num3 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - pointPoisition.Y;

        float num4 = (float)Math.Sqrt(num2 * num2 + num3 * num3);
        float num5 = num4;
        if((float.IsNaN(num2) && float.IsNaN(num3)) || (num2 == 0f && num3 == 0f)) {
            num2 = player.direction;
            num3 = 0f;
            num4 = speed;
        }
        else {
            num4 = speed / num4;
        }

        if(sItem.type == 1929 || sItem.type == 2270) {
            num2 += (float)Main.rand.Next(-50, 51) * 0.03f / num4;
            num3 += (float)Main.rand.Next(-50, 51) * 0.03f / num4;
        }

        num2 *= num4;
        num3 *= num4;
        if(projToShoot == 250) {
            for(int j = 0; j < 1000; j++) {
                if(Main.projectile[j].active && Main.projectile[j].owner == player.whoAmI && (Main.projectile[j].type == 250 || Main.projectile[j].type == 251))
                    Main.projectile[j].Kill();
            }
        }

        if(projToShoot == 12 && Collision.CanHitLine(player.Center, 0, 0, pointPoisition + new Vector2(num2, num3) * 4f, 0, 0))
            pointPoisition += new Vector2(num2, num3) * 3f;

        if(projToShoot == 728 && !Collision.CanHitLine(player.Center, 0, 0, pointPoisition + new Vector2(num2, num3) * 2f, 0, 0)) {
            Vector2 vector4 = new Vector2(num2, num3) * 0.25f;
            pointPoisition = player.Center - vector4;
        }

        if(projToShoot == 85) {
            pointPoisition += new Vector2(0f, -6f * (float)player.direction * player.Directions.Y).RotatedBy(vector.ToRotation());
            if(Collision.CanHitLine(pointPoisition, 0, 0, pointPoisition + new Vector2(num2, num3) * 5f, 0, 0))
                pointPoisition += new Vector2(num2, num3) * 4f;
        }

        if(projToShoot == 802 || projToShoot == 842) {
            Vector2 v2 = new Vector2(num2, num3);
            float num6 = (float)Math.PI / 4f;
            Vector2 vector5 = v2.SafeNormalize(Vector2.Zero).RotatedBy(num6 * (Main.rand.NextFloat() - 0.5f)) * (v2.Length() - Main.rand.NextFloatDirection() * 0.7f);
            num2 = vector5.X;
            num3 = vector5.Y;
        }
        DebugPrint("ItemCheck_Shoot5");

        goto DirtBallShoot;

    ShootHook:
        Vector2 velocity = new Vector2(num2, num3);

        CombinedHooks.ModifyShootStats(player, sItem, ref pointPoisition, ref velocity, ref projToShoot, ref Damage, ref KnockBack);

        num2 = velocity.X;
        num3 = velocity.Y;

        if(sItem.useStyle == ItemUseStyleID.Shoot) {
            if(sItem.type == 3029) {
                Vector2 vector6 = new Vector2(num2, num3);
                vector6.X = (float)Main.mouseX + Main.screenPosition.X - pointPoisition.X;
                vector6.Y = (float)Main.mouseY + Main.screenPosition.Y - pointPoisition.Y - 1000f;
                player.itemRotation = (float)Math.Atan2(vector6.Y * (float)player.direction, vector6.X * (float)player.direction);
                NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, player.whoAmI);
                NetMessage.SendData(MessageID.ShotAnimationAndSound, -1, -1, null, player.whoAmI);
            }
            else if(sItem.type == 4381) {
                Vector2 vector7 = new Vector2(num2, num3);
                vector7.X = (float)Main.mouseX + Main.screenPosition.X - pointPoisition.X;
                vector7.Y = (float)Main.mouseY + Main.screenPosition.Y - pointPoisition.Y - 1000f;
                player.itemRotation = (float)Math.Atan2(vector7.Y * (float)player.direction, vector7.X * (float)player.direction);
                NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, player.whoAmI);
                NetMessage.SendData(MessageID.ShotAnimationAndSound, -1, -1, null, player.whoAmI);
            }
            else if(sItem.type == 3779) {
                player.itemRotation = 0f;
                NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, player.whoAmI);
                NetMessage.SendData(MessageID.ShotAnimationAndSound, -1, -1, null, player.whoAmI);
            }
            else {
                player.itemRotation = (float)Math.Atan2(num3 * (float)player.direction, num2 * (float)player.direction) - player.fullRotation;
                NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, player.whoAmI);
                NetMessage.SendData(MessageID.ShotAnimationAndSound, -1, -1, null, player.whoAmI);
                DebugPrint("ItemCheck_Shoot7");
            }
        }

        if(sItem.useStyle == ItemUseStyleID.Rapier) {
            player.itemRotation = (float)Math.Atan2(num3 * (float)player.direction, num2 * (float)player.direction) - player.fullRotation;
            NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, player.whoAmI);
            NetMessage.SendData(MessageID.ShotAnimationAndSound, -1, -1, null, player.whoAmI);
        }

        if(!CombinedHooks.Shoot(player, sItem, (EntitySource_ItemUse_WithAmmo)projectileSource_Item_WithPotentialAmmo, pointPoisition, velocity, projToShoot, Damage, KnockBack))
            return;

        DebugPrint("ItemCheck_Shoot8");
        goto ShootProj;

    DirtBallShoot:
        if(projToShoot == 17) {
            pointPoisition.X = (float)Main.mouseX + Main.screenPosition.X;
            pointPoisition.Y = (float)Main.mouseY + Main.screenPosition.Y;
            if(player.gravDir == -1f)
                pointPoisition.Y = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY;

            player.LimitPointToPlayerReachableArea(ref pointPoisition);
        }

        DebugPrint("ItemCheck_Shoot6");
        goto ShootHook;

    ShootProj:
        if(projToShoot == 76) {
            projToShoot += Main.rand.Next(3);
            float num7 = (float)Main.screenHeight / Main.GameViewMatrix.Zoom.Y;
            num5 /= num7 / 2f;
            if(num5 > 1f)
                num5 = 1f;

            float num8 = num2 + (float)Main.rand.Next(-40, 41) * 0.01f;
            float num9 = num3 + (float)Main.rand.Next(-40, 41) * 0.01f;
            num8 *= num5 + 0.25f;
            num9 *= num5 + 0.25f;
            int num10 = Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num8, num9, projToShoot, Damage, KnockBack, i);
            Main.projectile[num10].ai[1] = 1f;
            num5 = num5 * 2f - 1f;
            if(num5 < -1f)
                num5 = -1f;

            if(num5 > 1f)
                num5 = 1f;

            num5 = (float)Math.Round(num5 * (float)Player.musicNotes);
            num5 /= (float)Player.musicNotes;
            Main.projectile[num10].ai[0] = num5;
            NetMessage.SendData(27, -1, -1, null, num10);
        }
        else if(sItem.type == ItemID.DaedalusStormbow) {
            int num11 = 3;
            if(projToShoot == 91 || projToShoot == 4 || projToShoot == 5 || projToShoot == 41) {
                if(Main.rand.NextBool(3))
                    num11--;
            }
            else if(Main.rand.NextBool(3)) {
                num11++;
            }

            for(int k = 0; k < num11; k++) {
                pointPoisition = new Vector2(player.position.X + (float)player.width * 0.5f + (float)(Main.rand.Next(201) * -player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
                pointPoisition.X = (pointPoisition.X * 10f + player.Center.X) / 11f + (float)Main.rand.Next(-100, 101);
                pointPoisition.Y -= 150 * k;
                num2 = (float)Main.mouseX + Main.screenPosition.X - pointPoisition.X;
                num3 = (float)Main.mouseY + Main.screenPosition.Y - pointPoisition.Y;
                if(num3 < 0f)
                    num3 *= -1f;

                if(num3 < 20f)
                    num3 = 20f;

                num4 = (float)Math.Sqrt(num2 * num2 + num3 * num3);
                num4 = speed / num4;
                num2 *= num4;
                num3 *= num4;
                float num12 = num2 + (float)Main.rand.Next(-40, 41) * 0.03f;
                float speedY = num3 + (float)Main.rand.Next(-40, 41) * 0.03f;
                num12 *= (float)Main.rand.Next(75, 150) * 0.01f;
                pointPoisition.X += Main.rand.Next(-50, 51);
                int num13 = Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num12, speedY, projToShoot, Damage, KnockBack, i);
                Main.projectile[num13].noDropItem = true;
            }
        }
        else if(sItem.type == 4381) {
            int num14 = Main.rand.Next(1, 3);
            if(Main.rand.Next(3) == 0)
                num14++;

            for(int l = 0; l < num14; l++) {
                pointPoisition = new Vector2(player.position.X + (float)player.width * 0.5f + (float)(Main.rand.Next(61) * -player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
                pointPoisition.X = (pointPoisition.X * 10f + player.Center.X) / 11f + (float)Main.rand.Next(-30, 31);
                pointPoisition.Y -= 150f * Main.rand.NextFloat();
                num2 = (float)Main.mouseX + Main.screenPosition.X - pointPoisition.X;
                num3 = (float)Main.mouseY + Main.screenPosition.Y - pointPoisition.Y;
                if(num3 < 0f)
                    num3 *= -1f;

                if(num3 < 20f)
                    num3 = 20f;

                num4 = (float)Math.Sqrt(num2 * num2 + num3 * num3);
                num4 = speed / num4;
                num2 *= num4;
                num3 *= num4;
                float num15 = num2 + (float)Main.rand.Next(-20, 21) * 0.03f;
                float speedY2 = num3 + (float)Main.rand.Next(-40, 41) * 0.03f;
                num15 *= (float)Main.rand.Next(55, 80) * 0.01f;
                pointPoisition.X += Main.rand.Next(-50, 51);
                int num16 = Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num15, speedY2, projToShoot, Damage, KnockBack, i);
                Main.projectile[num16].noDropItem = true;
            }
        }
        else if(sItem.type == 98 || sItem.type == 533) {
            float speedX = num2 + (float)Main.rand.Next(-40, 41) * 0.01f;
            float speedY3 = num3 + (float)Main.rand.Next(-40, 41) * 0.01f;
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, speedX, speedY3, projToShoot, Damage, KnockBack, i);
        }
        else if(sItem.type == 1319) {
            float speedX2 = num2 + (float)Main.rand.Next(-40, 41) * 0.02f;
            float speedY4 = num3 + (float)Main.rand.Next(-40, 41) * 0.02f;
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, speedX2, speedY4, projToShoot, Damage, KnockBack, i);
        }
        else if(sItem.type == 3107) {
            float speedX3 = num2 + (float)Main.rand.Next(-40, 41) * 0.02f;
            float speedY5 = num3 + (float)Main.rand.Next(-40, 41) * 0.02f;
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, speedX3, speedY5, projToShoot, Damage, KnockBack, i);
        }
        else if(ProjectileID.Sets.IsAGolfBall[projToShoot]) {
            Vector2 vector8 = new Vector2((float)Main.mouseX + Main.screenPosition.X, (float)Main.mouseY + Main.screenPosition.Y);
            Vector2 vector9 = vector8 - player.Center;
            bool flag2 = false;
            if(vector9.Length() < 100f)
                flag2 = player.TryPlacingAGolfBallNearANearbyTee(vector8);

            if(!flag2) {
                if(vector9.Length() > 100f || !Collision.CanHit(player.Center, 1, 1, vector8, 1, 1))
                    Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num2, num3, projToShoot, Damage, KnockBack, i);
                else
                    Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, vector8.X, vector8.Y, 0f, 0f, projToShoot, Damage, KnockBack, i);
            }
        }
        else if(sItem.type == 3053) {
            bool flag3 = false;
            if(player.itemAnimation <= sItem.useTime + 1)
                flag3 = true;

            Vector2 vector10 = new Vector2(num2, num3);
            vector10.Normalize();
            vector10 *= 4f;
            if(!flag3) {
                Vector2 vector11 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                vector11.Normalize();
                vector10 += vector11;
            }

            vector10.Normalize();
            vector10 *= sItem.shootSpeed;
            float num17 = (float)Main.rand.Next(10, 80) * 0.001f;
            if(Main.rand.Next(2) == 0)
                num17 *= -1f;

            float num18 = (float)Main.rand.Next(10, 80) * 0.001f;
            if(Main.rand.Next(2) == 0)
                num18 *= -1f;

            if(flag3)
                num18 = (num17 = 0f);

            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, vector10.X, vector10.Y, projToShoot, Damage, KnockBack, i, num18, num17);
        }
        else if(sItem.type == 3019) {
            Vector2 vector12 = new Vector2(num2, num3);
            float num19 = vector12.Length();
            vector12.X += (float)Main.rand.Next(-100, 101) * 0.01f * num19 * 0.15f;
            vector12.Y += (float)Main.rand.Next(-100, 101) * 0.01f * num19 * 0.15f;
            float num20 = num2 + (float)Main.rand.Next(-40, 41) * 0.03f;
            float num21 = num3 + (float)Main.rand.Next(-40, 41) * 0.03f;
            vector12.Normalize();
            vector12 *= num19;
            num20 *= (float)Main.rand.Next(50, 150) * 0.01f;
            num21 *= (float)Main.rand.Next(50, 150) * 0.01f;
            Vector2 vector13 = new Vector2(num20, num21);
            vector13.X += (float)Main.rand.Next(-100, 101) * 0.025f;
            vector13.Y += (float)Main.rand.Next(-100, 101) * 0.025f;
            vector13.Normalize();
            vector13 *= num19;
            num20 = vector13.X;
            num21 = vector13.Y;
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num20, num21, projToShoot, Damage, KnockBack, i, vector12.X, vector12.Y);
        }
        else if(sItem.type == 2797) {
            Vector2 vector14 = Vector2.Normalize(new Vector2(num2, num3)) * 40f * sItem.scale;
            if(Collision.CanHit(pointPoisition, 0, 0, pointPoisition + vector14, 0, 0))
                pointPoisition += vector14;

            float ai = new Vector2(num2, num3).ToRotation();
            float num22 = (float)Math.PI * 2f / 3f;
            int num23 = Main.rand.Next(4, 5);
            if(Main.rand.Next(4) == 0)
                num23++;

            for(int m = 0; m < num23; m++) {
                float num24 = (float)Main.rand.NextDouble() * 0.2f + 0.05f;
                Vector2 vector15 = new Vector2(num2, num3).RotatedBy(num22 * (float)Main.rand.NextDouble() - num22 / 2f) * num24;
                int num25 = Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, vector15.X, vector15.Y, 444, Damage, KnockBack, i, ai);
                Main.projectile[num25].localAI[0] = projToShoot;
                Main.projectile[num25].localAI[1] = speed;
            }
        }
        else if(sItem.type == 2270) {
            float num26 = num2 + (float)Main.rand.Next(-40, 41) * 0.05f;
            float num27 = num3 + (float)Main.rand.Next(-40, 41) * 0.05f;
            if(Main.rand.Next(3) == 0) {
                num26 *= 1f + (float)Main.rand.Next(-30, 31) * 0.02f;
                num27 *= 1f + (float)Main.rand.Next(-30, 31) * 0.02f;
            }

            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num26, num27, projToShoot, Damage, KnockBack, i);
        }
        else if(sItem.type == 5117) {
            float speedX4 = num2 + (float)Main.rand.Next(-15, 16) * 0.075f;
            float speedY6 = num3 + (float)Main.rand.Next(-15, 16) * 0.075f;
            int num28 = Main.rand.Next(Main.projFrames[sItem.shoot]);
            int damage2 = Damage;
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, speedX4, speedY6, projToShoot, damage2, KnockBack, i, 0f, num28);
        }
        else if(sItem.type == 1930) {
            int num29 = 2 + Main.rand.Next(3);
            for(int n = 0; n < num29; n++) {
                float num30 = num2;
                float num31 = num3;
                float num32 = 0.025f * (float)n;
                num30 += (float)Main.rand.Next(-35, 36) * num32;
                num31 += (float)Main.rand.Next(-35, 36) * num32;
                num4 = (float)Math.Sqrt(num30 * num30 + num31 * num31);
                num4 = speed / num4;
                num30 *= num4;
                num31 *= num4;
                float x = pointPoisition.X + num2 * (float)(num29 - n) * 1.75f;
                float y = pointPoisition.Y + num3 * (float)(num29 - n) * 1.75f;
                Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, x, y, num30, num31, projToShoot, Damage, KnockBack, i, Main.rand.Next(0, 10 * (n + 1)));
            }
        }
        else if(sItem.type == 1931) {
            int num33 = 2;
            for(int num34 = 0; num34 < num33; num34++) {
                pointPoisition = new Vector2(player.position.X + (float)player.width * 0.5f + (float)(Main.rand.Next(201) * -player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
                pointPoisition.X = (pointPoisition.X + player.Center.X) / 2f + (float)Main.rand.Next(-200, 201);
                pointPoisition.Y -= 100 * num34;
                num2 = (float)Main.mouseX + Main.screenPosition.X - pointPoisition.X;
                num3 = (float)Main.mouseY + Main.screenPosition.Y - pointPoisition.Y;
                if(player.gravDir == -1f)
                    num3 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - pointPoisition.Y;

                if(num3 < 0f)
                    num3 *= -1f;

                if(num3 < 20f)
                    num3 = 20f;

                num4 = (float)Math.Sqrt(num2 * num2 + num3 * num3);
                num4 = speed / num4;
                num2 *= num4;
                num3 *= num4;
                float speedX5 = num2 + (float)Main.rand.Next(-40, 41) * 0.02f;
                float speedY7 = num3 + (float)Main.rand.Next(-40, 41) * 0.02f;
                Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, speedX5, speedY7, projToShoot, Damage, KnockBack, i, 0f, Main.rand.Next(5));
            }
        }
        else if(sItem.type == 2750) {
            int num35 = 1;
            for(int num36 = 0; num36 < num35; num36++) {
                pointPoisition = new Vector2(player.position.X + (float)player.width * 0.5f + (float)(Main.rand.Next(201) * -player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
                pointPoisition.X = (pointPoisition.X + player.Center.X) / 2f + (float)Main.rand.Next(-200, 201);
                pointPoisition.Y -= 100 * num36;
                num2 = (float)Main.mouseX + Main.screenPosition.X - pointPoisition.X + (float)Main.rand.Next(-40, 41) * 0.03f;
                num3 = (float)Main.mouseY + Main.screenPosition.Y - pointPoisition.Y;
                if(player.gravDir == -1f)
                    num3 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - pointPoisition.Y;

                if(num3 < 0f)
                    num3 *= -1f;

                if(num3 < 20f)
                    num3 = 20f;

                num4 = (float)Math.Sqrt(num2 * num2 + num3 * num3);
                num4 = speed / num4;
                num2 *= num4;
                num3 *= num4;
                float num37 = num2;
                float num38 = num3 + (float)Main.rand.Next(-40, 41) * 0.02f;
                Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num37 * 0.75f, num38 * 0.75f, projToShoot + Main.rand.Next(3), Damage, KnockBack, i, 0f, 0.5f + (float)Main.rand.NextDouble() * 0.3f);
            }
        }
        else if(sItem.type == 3570) {
            int num39 = 3;
            for(int num40 = 0; num40 < num39; num40++) {
                pointPoisition = new Vector2(player.position.X + (float)player.width * 0.5f + (float)(Main.rand.Next(201) * -player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
                pointPoisition.X = (pointPoisition.X + player.Center.X) / 2f + (float)Main.rand.Next(-200, 201);
                pointPoisition.Y -= 100 * num40;
                num2 = (float)Main.mouseX + Main.screenPosition.X - pointPoisition.X;
                num3 = (float)Main.mouseY + Main.screenPosition.Y - pointPoisition.Y;
                float ai2 = num3 + pointPoisition.Y;
                if(num3 < 0f)
                    num3 *= -1f;

                if(num3 < 20f)
                    num3 = 20f;

                num4 = (float)Math.Sqrt(num2 * num2 + num3 * num3);
                num4 = speed / num4;
                num2 *= num4;
                num3 *= num4;
                Vector2 vector16 = new Vector2(num2, num3) / 2f;
                Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, vector16.X, vector16.Y, projToShoot, Damage, KnockBack, i, 0f, ai2);
            }
        }
        else if(sItem.type == 5065) {
            Vector2 farthestSpawnPositionOnLine = player.GetFarthestSpawnPositionOnLine(pointPoisition, num2, num3);
            Vector2 zero = Vector2.Zero;
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, farthestSpawnPositionOnLine, zero, projToShoot, Damage, KnockBack, i);
        }
        else if(sItem.type == 3065) {
            Vector2 vector17 = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY);
            float num41 = vector17.Y;
            if(num41 > player.Center.Y - 200f)
                num41 = player.Center.Y - 200f;

            for(int num42 = 0; num42 < 3; num42++) {
                pointPoisition = player.Center + new Vector2(-Main.rand.Next(0, 401) * player.direction, -600f);
                pointPoisition.Y -= 100 * num42;
                Vector2 vector18 = vector17 - pointPoisition;
                if(vector18.Y < 0f)
                    vector18.Y *= -1f;

                if(vector18.Y < 20f)
                    vector18.Y = 20f;

                vector18.Normalize();
                vector18 *= speed;
                num2 = vector18.X;
                num3 = vector18.Y;
                float speedX6 = num2;
                float speedY8 = num3 + (float)Main.rand.Next(-40, 41) * 0.02f;
                Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, speedX6, speedY8, projToShoot, Damage, KnockBack, i, 0f, num41);
            }
        }
        else if(sItem.type == 2624) {
            float num43 = (float)Math.PI / 10f;
            int num44 = 5;
            Vector2 vector19 = new Vector2(num2, num3);
            vector19.Normalize();
            vector19 *= 40f;
            bool flag4 = Collision.CanHit(pointPoisition, 0, 0, pointPoisition + vector19, 0, 0);
            for(int num45 = 0; num45 < num44; num45++) {
                float num46 = (float)num45 - ((float)num44 - 1f) / 2f;
                Vector2 vector20 = vector19.RotatedBy(num43 * num46);
                if(!flag4)
                    vector20 -= vector19;

                int num47 = Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X + vector20.X, pointPoisition.Y + vector20.Y, num2, num3, projToShoot, Damage, KnockBack, i);
                Main.projectile[num47].noDropItem = true;
            }
        }
        else if(sItem.type == 1929) {
            float speedX7 = num2 + (float)Main.rand.Next(-40, 41) * 0.03f;
            float speedY9 = num3 + (float)Main.rand.Next(-40, 41) * 0.03f;
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, speedX7, speedY9, projToShoot, Damage, KnockBack, i);
        }
        else if(sItem.type == 1553) {
            float speedX8 = num2 + (float)Main.rand.Next(-40, 41) * 0.005f;
            float speedY10 = num3 + (float)Main.rand.Next(-40, 41) * 0.005f;
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, speedX8, speedY10, projToShoot, Damage, KnockBack, i);
        }
        else if(sItem.type == 518) {
            float num48 = num2;
            float num49 = num3;
            num48 += (float)Main.rand.Next(-40, 41) * 0.04f;
            num49 += (float)Main.rand.Next(-40, 41) * 0.04f;
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num48, num49, projToShoot, Damage, KnockBack, i);
        }
        else if(sItem.type == 1265) {
            float num50 = num2;
            float num51 = num3;
            num50 += (float)Main.rand.Next(-30, 31) * 0.03f;
            num51 += (float)Main.rand.Next(-30, 31) * 0.03f;
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num50, num51, projToShoot, Damage, KnockBack, i);
        }
        else if(sItem.type == 4262) {
            float num52 = 2.6666667f;
            _ = player.Bottom;
            _ = (int)player.Bottom.X / 16;
            int num53 = 4;
            float num54 = Math.Abs((float)Main.mouseX + Main.screenPosition.X - player.position.X) / 16f;
            if(player.direction < 0)
                num54 += 1f;

            num53 = (int)num54;
            if(num53 > 15)
                num53 = 15;

            Point point = player.Center.ToTileCoordinates();
            int maxDistance = 31;
            for(int num55 = num53; num55 >= 0; num55--) {
                if(Collision.CanHitLine(player.Center, 1, 1, player.Center + new Vector2(16 * num55 * player.direction, 0f), 1, 1) && WorldUtils.Find(new Point(point.X + player.direction * num55, point.Y), Searches.Chain(new Searches.Down(maxDistance), new Conditions.MysticSnake()), out var result)) {
                    int num56 = result.Y;
                    while(Main.tile[result.X, num56 - 1].IsActuated) {      //!!active -> IsActuated
                        num56--;
                        if(Main.tile[result.X, num56 - 1] == null || num56 < 10 || result.Y - num56 > 7) {
                            num56 = -1;
                            break;
                        }
                    }

                    if(num56 >= 10) {
                        result.Y = num56;
                        for(int num57 = 0; num57 < 1000; num57++) {
                            Projectile projectile = Main.projectile[num57];
                            if(projectile.active && projectile.owner == player.whoAmI && projectile.type == projToShoot) {
                                if(projectile.ai[1] == 2f)
                                    projectile.timeLeft = 4;
                                else
                                    projectile.Kill();
                            }
                        }

                        Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, result.X * 16 + 8, result.Y * 16 + 8 - 16, 0f, 0f - num52, projToShoot, Damage, KnockBack, i, result.Y * 16 + 8 - 16);
                        break;
                    }
                }
            }
        }
        else if(sItem.type == 4952) {
            Vector2 vector21 = Main.rand.NextVector2Circular(1f, 1f) + Main.rand.NextVector2CircularEdge(3f, 3f);
            if(vector21.Y > 0f)
                vector21.Y *= -1f;

            float num58 = (float)player.itemAnimation / (float)player.itemAnimationMax * 0.66f + player.miscCounterNormalized;
            pointPoisition = player.MountedCenter + new Vector2(player.direction * 15, player.gravDir * 3f);
            Point point2 = pointPoisition.ToTileCoordinates();
            Tile tile = Main.tile[point2.X, point2.Y];
            if(tile != null && !tile.IsActuated && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType] && !TileID.Sets.Platforms[tile.TileType])
                pointPoisition = player.MountedCenter;

            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, vector21.X, vector21.Y, projToShoot, Damage, KnockBack, i, -1f, num58 % 1f);
        }
        else if(sItem.type == 4953) {
            float num59 = (float)Math.PI / 10f;
            int num60 = 5;
            Vector2 vector22 = new Vector2(num2, num3);
            vector22.Normalize();
            vector22 *= 40f;
            bool num61 = Collision.CanHit(pointPoisition, 0, 0, pointPoisition + vector22, 0, 0);
            int num62 = (player.itemAnimationMax - player.itemAnimation) / 2;
            int num63 = num62;
            if(player.direction == 1)
                num63 = 4 - num62;

            float num64 = (float)num63 - ((float)num60 - 1f) / 2f;
            Vector2 vector23 = vector22.RotatedBy(num59 * num64);
            if(!num61)
                vector23 -= vector22;

            Vector2 mouseWorld = Main.MouseWorld;
            Vector2 origin = pointPoisition + vector23;
            Vector2 vector24 = origin.DirectionTo(mouseWorld).SafeNormalize(-Vector2.UnitY);
            Vector2 value2 = player.Center.DirectionTo(player.Center + new Vector2(num2, num3)).SafeNormalize(-Vector2.UnitY);
            float lerpValue = Utils.GetLerpValue(100f, 40f, mouseWorld.Distance(player.Center), clamped: true);
            if(lerpValue > 0f)
                vector24 = Vector2.Lerp(vector24, value2, lerpValue).SafeNormalize(new Vector2(num2, num3).SafeNormalize(-Vector2.UnitY));

            Vector2 v3 = vector24 * speed;
            if(num62 == 2) {
                projToShoot = 932;
                Damage *= 2;
            }

            if(projToShoot == 932) {
                float ai3 = player.miscCounterNormalized * 12f % 1f;
                v3 = v3.SafeNormalize(Vector2.Zero) * (speed * 2f);
                Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, origin, v3, projToShoot, Damage, KnockBack, i, 0f, ai3);
            }
            else {
                int num65 = Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, origin, v3, projToShoot, Damage, KnockBack, i);
                Main.projectile[num65].noDropItem = true;
            }
        }
        else if(sItem.type == 534) {
            int num66 = Main.rand.Next(4, 6);
            for(int num67 = 0; num67 < num66; num67++) {
                float num68 = num2;
                float num69 = num3;
                num68 += (float)Main.rand.Next(-40, 41) * 0.05f;
                num69 += (float)Main.rand.Next(-40, 41) * 0.05f;
                Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num68, num69, projToShoot, Damage, KnockBack, i);
            }
        }
        else if(sItem.type == 4703) {
            float num70 = (float)Math.PI / 2f;
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num2, num3, projToShoot, Damage, KnockBack, i);
            for(int num71 = 0; num71 < 7; num71++) {
                Vector2 v4 = new Vector2(num2, num3);
                float num72 = v4.Length();
                v4 += v4.SafeNormalize(Vector2.Zero).RotatedBy(num70 * Main.rand.NextFloat()) * Main.rand.NextFloatDirection() * 5f;
                v4 = v4.SafeNormalize(Vector2.Zero) * num72;
                float x2 = v4.X;
                float y2 = v4.Y;
                x2 += (float)Main.rand.Next(-40, 41) * 0.05f;
                y2 += (float)Main.rand.Next(-40, 41) * 0.05f;
                Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, x2, y2, projToShoot, Damage, KnockBack, i);
            }
        }
        else if(sItem.type == 4270) {
            Vector2 pointPoisition2 = Main.MouseWorld;
            player.LimitPointToPlayerReachableArea(ref pointPoisition2);
            Vector2 vector25 = pointPoisition2 + Main.rand.NextVector2Circular(8f, 8f);
            Vector2 vector26 = FindSharpTearsSpot(player, vector25).ToWorldCoordinates(Main.rand.Next(17), Main.rand.Next(17));
            Vector2 vector27 = (vector25 - vector26).SafeNormalize(-Vector2.UnitY) * 16f;
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, vector26.X, vector26.Y, vector27.X, vector27.Y, projToShoot, Damage, KnockBack, i, 0f, Main.rand.NextFloat() * 0.5f + 0.6f);
        }
        else if(sItem.type == 4715) {
            Vector2 vector28 = Main.MouseWorld;
            List<NPC> validTargets;
            bool sparkleGuitarTarget = GetSparkleGuitarTarget(player, out validTargets);
            if(sparkleGuitarTarget) {
                NPC nPC = validTargets[Main.rand.Next(validTargets.Count)];
                vector28 = nPC.Center + nPC.velocity * 20f;
            }

            Vector2 vector29 = vector28 - player.Center;
            if(!sparkleGuitarTarget) {
                vector28 += Main.rand.NextVector2Circular(24f, 24f);
                if(vector29.Length() > 700f) {
                    vector29 *= 700f / vector29.Length();
                    vector28 = player.Center + vector29;
                }
            }

            Vector2 vector30 = Main.rand.NextVector2CircularEdge(1f, 1f);
            if(vector30.Y > 0f)
                vector30 *= -1f;

            if(Math.Abs(vector30.Y) < 0.5f)
                vector30.Y = (0f - Main.rand.NextFloat()) * 0.5f - 0.5f;

            vector30 *= vector29.Length() * 2f;
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, vector30.X, vector30.Y, projToShoot, Damage, KnockBack, i, vector28.X, vector28.Y);
        }
        else if(sItem.type == 4722) {
            Vector2 vector31 = Main.MouseWorld;
            List<NPC> validTargets2;
            bool sparkleGuitarTarget2 = GetSparkleGuitarTarget(player, out validTargets2);
            if(sparkleGuitarTarget2) {
                NPC nPC2 = validTargets2[Main.rand.Next(validTargets2.Count)];
                vector31 = nPC2.Center + nPC2.velocity * 20f;
            }

            Vector2 vector32 = vector31 - player.Center;
            Vector2 vector33 = Main.rand.NextVector2CircularEdge(1f, 1f);
            float num73 = 1f;
            int num74 = 1;
            for(int num75 = 0; num75 < num74; num75++) {
                if(!sparkleGuitarTarget2) {
                    vector31 += Main.rand.NextVector2Circular(24f, 24f);
                    if(vector32.Length() > 700f) {
                        vector32 *= 700f / vector32.Length();
                        vector31 = player.Center + vector32;
                    }

                    float num76 = Utils.GetLerpValue(0f, 6f, velocity.Length(), clamped: true) * 0.8f;
                    vector33 *= 1f - num76;
                    vector33 += velocity * num76;
                    vector33 = vector33.SafeNormalize(Vector2.UnitX);
                }

                float num77 = 60f;
                float num78 = Main.rand.NextFloatDirection() * (float)Math.PI * (1f / num77) * 0.5f * num73;
                float num79 = num77 / 2f;
                float num80 = 12f + Main.rand.NextFloat() * 2f;
                Vector2 vector34 = vector33 * num80;
                Vector2 vector35 = new Vector2(0f, 0f);
                Vector2 vector36 = vector34;
                for(int num81 = 0; (float)num81 < num79; num81++) {
                    vector35 += vector36;
                    vector36 = vector36.RotatedBy(num78);
                }

                Vector2 vector37 = -vector35;
                Vector2 vector38 = vector31 + vector37;
                float lerpValue2 = Utils.GetLerpValue(player.itemAnimationMax, 0f, player.itemAnimation, clamped: true);
                Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, vector38, vector34, projToShoot, Damage, KnockBack, i, num78, lerpValue2);
            }
        }
        else if(sItem.type == 4607) {
            player.SpawnMinionOnCursor(projectileSource_Item_WithPotentialAmmo, i, projToShoot, damage, KnockBack);
        }
        else if(sItem.type == 5069) {
            player.SpawnMinionOnCursor(projectileSource_Item_WithPotentialAmmo, i, projToShoot, damage, KnockBack);
        }
        else if(sItem.type == 5114) {
            player.SpawnMinionOnCursor(projectileSource_Item_WithPotentialAmmo, i, projToShoot, damage, KnockBack);
        }
        else if(sItem.type == 2188) {
            int num82 = 4;
            if(Main.rand.Next(3) == 0)
                num82++;

            if(Main.rand.Next(4) == 0)
                num82++;

            if(Main.rand.Next(5) == 0)
                num82++;

            for(int num83 = 0; num83 < num82; num83++) {
                float num84 = num2;
                float num85 = num3;
                float num86 = 0.05f * (float)num83;
                num84 += (float)Main.rand.Next(-35, 36) * num86;
                num85 += (float)Main.rand.Next(-35, 36) * num86;
                num4 = (float)Math.Sqrt(num84 * num84 + num85 * num85);
                num4 = speed / num4;
                num84 *= num4;
                num85 *= num4;
                float x3 = pointPoisition.X;
                float y3 = pointPoisition.Y;
                Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, x3, y3, num84, num85, projToShoot, Damage, KnockBack, i);
            }
        }
        else if(sItem.type == 1308) {
            int num87 = 3;
            if(Main.rand.Next(3) == 0)
                num87++;

            for(int num88 = 0; num88 < num87; num88++) {
                float num89 = num2;
                float num90 = num3;
                float num91 = 0.05f * (float)num88;
                num89 += (float)Main.rand.Next(-35, 36) * num91;
                num90 += (float)Main.rand.Next(-35, 36) * num91;
                num4 = (float)Math.Sqrt(num89 * num89 + num90 * num90);
                num4 = speed / num4;
                num89 *= num4;
                num90 *= num4;
                float x4 = pointPoisition.X;
                float y4 = pointPoisition.Y;
                Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, x4, y4, num89, num90, projToShoot, Damage, KnockBack, i);
            }
        }
        else if(sItem.type == 1258) {
            float num92 = num2;
            float num93 = num3;
            num92 += (float)Main.rand.Next(-40, 41) * 0.01f;
            num93 += (float)Main.rand.Next(-40, 41) * 0.01f;
            pointPoisition.X += (float)Main.rand.Next(-40, 41) * 0.05f;
            pointPoisition.Y += (float)Main.rand.Next(-45, 36) * 0.05f;
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num92, num93, projToShoot, Damage, KnockBack, i);
        }
        else if(sItem.type == 964) {
            int num94 = Main.rand.Next(3, 5);
            for(int num95 = 0; num95 < num94; num95++) {
                float num96 = num2;
                float num97 = num3;
                num96 += (float)Main.rand.Next(-35, 36) * 0.04f;
                num97 += (float)Main.rand.Next(-35, 36) * 0.04f;
                Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num96, num97, projToShoot, Damage, KnockBack, i);
            }
        }
        else if(sItem.type == 1569) {
            int num98 = 4;
            if(Main.rand.Next(2) == 0)
                num98++;

            if(Main.rand.Next(4) == 0)
                num98++;

            if(Main.rand.Next(8) == 0)
                num98++;

            if(Main.rand.Next(16) == 0)
                num98++;

            for(int num99 = 0; num99 < num98; num99++) {
                float num100 = num2;
                float num101 = num3;
                float num102 = 0.05f * (float)num99;
                num100 += (float)Main.rand.Next(-35, 36) * num102;
                num101 += (float)Main.rand.Next(-35, 36) * num102;
                num4 = (float)Math.Sqrt(num100 * num100 + num101 * num101);
                num4 = speed / num4;
                num100 *= num4;
                num101 *= num4;
                float x5 = pointPoisition.X;
                float y5 = pointPoisition.Y;
                Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, x5, y5, num100, num101, projToShoot, Damage, KnockBack, i);
            }
        }
        else if(sItem.type == 1572 || sItem.type == 2366 || sItem.type == 3571 || sItem.type == 3569 || sItem.type == 5119) {
            bool num103 = sItem.type == 3571 || sItem.type == 3569;
            int num104 = (int)((float)Main.mouseX + Main.screenPosition.X) / 16;
            int num105 = (int)((float)Main.mouseY + Main.screenPosition.Y) / 16;
            if(player.gravDir == -1f)
                num105 = (int)(Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY) / 16;

            if(!num103) {
                for(; num105 < Main.maxTilesY - 10 && Main.tile[num104, num105] != null && !WorldGen.SolidTile2(num104, num105) && Main.tile[num104 - 1, num105] != null && !WorldGen.SolidTile2(num104 - 1, num105) && Main.tile[num104 + 1, num105] != null && !WorldGen.SolidTile2(num104 + 1, num105); num105++) {
                }

                num105--;
            }

            int num106 = 0;
            switch(sItem.type) {
            case 1572:
                num106 = 60;
                break;
            case 5119:
                num106 = 90;
                break;
            }

            int num107 = Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, (float)Main.mouseX + Main.screenPosition.X, num105 * 16 - 24, 0f, 15f, projToShoot, Damage, KnockBack, i, num106);
            Main.projectile[num107].originalDamage = damage;
            player.UpdateMaxTurrets();
        }
        else if(sItem.type == 1244 || sItem.type == 1256) {
            int num108 = Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num2, num3, projToShoot, Damage, KnockBack, i);
            Main.projectile[num108].ai[0] = (float)Main.mouseX + Main.screenPosition.X;
            Main.projectile[num108].ai[1] = (float)Main.mouseY + Main.screenPosition.Y;
        }
        else if(sItem.type == 1229) {
            int num109 = 2;
            if(Main.rand.Next(3) == 0)
                num109++;

            for(int num110 = 0; num110 < num109; num110++) {
                float num111 = num2;
                float num112 = num3;
                if(num110 > 0) {
                    num111 += (float)Main.rand.Next(-35, 36) * 0.04f;
                    num112 += (float)Main.rand.Next(-35, 36) * 0.04f;
                }

                if(num110 > 1) {
                    num111 += (float)Main.rand.Next(-35, 36) * 0.04f;
                    num112 += (float)Main.rand.Next(-35, 36) * 0.04f;
                }

                if(num110 > 2) {
                    num111 += (float)Main.rand.Next(-35, 36) * 0.04f;
                    num112 += (float)Main.rand.Next(-35, 36) * 0.04f;
                }

                int num113 = Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num111, num112, projToShoot, Damage, KnockBack, i);
                Main.projectile[num113].noDropItem = true;
            }
        }
        else if(sItem.type == 1121) {
            int num114 = Main.rand.Next(1, 4);
            if(Main.rand.Next(6) == 0)
                num114++;

            if(Main.rand.Next(6) == 0)
                num114++;

            if(player.strongBees && Main.rand.Next(3) == 0)
                num114++;

            for(int num115 = 0; num115 < num114; num115++) {
                float num116 = num2;
                float num117 = num3;
                num116 += (float)Main.rand.Next(-35, 36) * 0.02f;
                num117 += (float)Main.rand.Next(-35, 36) * 0.02f;
                int num118 = Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num116, num117, player.beeType(), player.beeDamage(Damage), player.beeKB(KnockBack), i);
                Main.projectile[num118].DamageType = DamageClass.Magic;
            }
        }
        else if(sItem.type == 1155) {
            int num119 = Main.rand.Next(2, 5);
            for(int num120 = 0; num120 < num119; num120++) {
                float num121 = num2;
                float num122 = num3;
                num121 += (float)Main.rand.Next(-35, 36) * 0.02f;
                num122 += (float)Main.rand.Next(-35, 36) * 0.02f;
                Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num121, num122, projToShoot, Damage, KnockBack, i);
            }
        }
        else if(sItem.type == 1801) {
            int num123 = Main.rand.Next(2, 4);
            for(int num124 = 0; num124 < num123; num124++) {
                float num125 = num2;
                float num126 = num3;
                num125 += (float)Main.rand.Next(-35, 36) * 0.05f;
                num126 += (float)Main.rand.Next(-35, 36) * 0.05f;
                Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num125, num126, projToShoot, Damage, KnockBack, i);
            }
        }
        else if(sItem.type == 679) {
            for(int num127 = 0; num127 < 6; num127++) {
                float num128 = num2;
                float num129 = num3;
                num128 += (float)Main.rand.Next(-40, 41) * 0.05f;
                num129 += (float)Main.rand.Next(-40, 41) * 0.05f;
                Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num128, num129, projToShoot, Damage, KnockBack, i);
            }
        }
        else if(sItem.type == 1156) {
            for(int num130 = 0; num130 < 3; num130++) {
                float num131 = num2;
                float num132 = num3;
                num131 += (float)Main.rand.Next(-40, 41) * 0.05f;
                num132 += (float)Main.rand.Next(-40, 41) * 0.05f;
                Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num131, num132, projToShoot, Damage, KnockBack, i);
            }
        }
        else if(sItem.type == 4682) {
            for(int num133 = 0; num133 < 3; num133++) {
                float num134 = num2;
                float num135 = num3;
                num134 += (float)Main.rand.Next(-20, 21) * 0.1f;
                num135 += (float)Main.rand.Next(-20, 21) * 0.1f;
                Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num134, num135, projToShoot, Damage, KnockBack, i);
            }
        }
        else if(sItem.type == 2623) {
            for(int num136 = 0; num136 < 3; num136++) {
                float num137 = num2;
                float num138 = num3;
                num137 += (float)Main.rand.Next(-40, 41) * 0.1f;
                num138 += (float)Main.rand.Next(-40, 41) * 0.1f;
                Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num137, num138, projToShoot, Damage, KnockBack, i);
            }
        }
        else if(sItem.type == 3210) {
            Vector2 vector39 = new Vector2(num2, num3);
            vector39.X += (float)Main.rand.Next(-30, 31) * 0.04f;
            vector39.Y += (float)Main.rand.Next(-30, 31) * 0.03f;
            vector39.Normalize();
            vector39 *= (float)Main.rand.Next(70, 91) * 0.1f;
            vector39.X += (float)Main.rand.Next(-30, 31) * 0.04f;
            vector39.Y += (float)Main.rand.Next(-30, 31) * 0.03f;
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, vector39.X, vector39.Y, projToShoot, Damage, KnockBack, i, Main.rand.Next(20));
        }
        else if(sItem.type == 434) {
            float num139 = num2;
            float num140 = num3;
            if(player.itemAnimation < 5) {
                num139 += (float)Main.rand.Next(-40, 41) * 0.01f;
                num140 += (float)Main.rand.Next(-40, 41) * 0.01f;
                num139 *= 1.1f;
                num140 *= 1.1f;
            }
            else if(player.itemAnimation < 10) {
                num139 += (float)Main.rand.Next(-20, 21) * 0.01f;
                num140 += (float)Main.rand.Next(-20, 21) * 0.01f;
                num139 *= 1.05f;
                num140 *= 1.05f;
            }

            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num139, num140, projToShoot, Damage, KnockBack, i);
        }
        else if(sItem.type == 1157) {
            projToShoot = Main.rand.Next(191, 195);
            int num141 = player.SpawnMinionOnCursor(projectileSource_Item_WithPotentialAmmo, i, projToShoot, damage, KnockBack);
            Main.projectile[num141].localAI[0] = 30f;
        }
        else if(sItem.type == 1802) {
            player.SpawnMinionOnCursor(projectileSource_Item_WithPotentialAmmo, i, projToShoot, damage, KnockBack);
        }
        else if(sItem.type == 2364 || sItem.type == 2365) {
            player.SpawnMinionOnCursor(projectileSource_Item_WithPotentialAmmo, i, projToShoot, damage, KnockBack);
        }
        else if(sItem.type == 2535) {
            num2 = 0f;
            num3 = 0f;
            Vector2 spinningpoint = new Vector2(num2, num3);
            spinningpoint = spinningpoint.RotatedBy(1.5707963705062866);
            player.SpawnMinionOnCursor(projectileSource_Item_WithPotentialAmmo, i, projToShoot, damage, KnockBack, spinningpoint, spinningpoint);
            spinningpoint = spinningpoint.RotatedBy(-3.1415927410125732);
            player.SpawnMinionOnCursor(projectileSource_Item_WithPotentialAmmo, i, projToShoot + 1, damage, KnockBack, spinningpoint, spinningpoint);
        }
        else if(sItem.type == 2551) {
            player.SpawnMinionOnCursor(projectileSource_Item_WithPotentialAmmo, i, projToShoot + player.nextCycledSpiderMinionType, damage, KnockBack);
            player.nextCycledSpiderMinionType++;
            player.nextCycledSpiderMinionType %= 3;
        }
        else if(sItem.type == 2584) {
            player.SpawnMinionOnCursor(projectileSource_Item_WithPotentialAmmo, i, projToShoot + Main.rand.Next(3), damage, KnockBack);
        }
        else if(sItem.type == 2621) {
            player.SpawnMinionOnCursor(projectileSource_Item_WithPotentialAmmo, i, projToShoot, damage, KnockBack);
        }
        else if(sItem.type == 2749 || sItem.type == 3249 || sItem.type == 3474 || sItem.type == 4273 || sItem.type == 4281) {
            player.SpawnMinionOnCursor(projectileSource_Item_WithPotentialAmmo, i, projToShoot, damage, KnockBack);
        }
        else if(sItem.type == 3531) {
            int num142 = -1;
            int num143 = -1;
            for(int num144 = 0; num144 < 1000; num144++) {
                if(Main.projectile[num144].active && Main.projectile[num144].owner == Main.myPlayer) {
                    if(num142 == -1 && Main.projectile[num144].type == 625)
                        num142 = num144;

                    if(num143 == -1 && Main.projectile[num144].type == 628)
                        num143 = num144;

                    if(num142 != -1 && num143 != -1)
                        break;
                }
            }

            if(num142 == -1 && num143 == -1) {
                num2 = 0f;
                num3 = 0f;
                pointPoisition.X = (float)Main.mouseX + Main.screenPosition.X;
                pointPoisition.Y = (float)Main.mouseY + Main.screenPosition.Y;
                int num145 = Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num2, num3, projToShoot, Damage, KnockBack, i);
                int num146 = Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num2, num3, projToShoot + 1, Damage, KnockBack, i, num145);
                int num147 = Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num2, num3, projToShoot + 2, Damage, KnockBack, i, num146);
                int num148 = Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num2, num3, projToShoot + 3, Damage, KnockBack, i, num147);
                Main.projectile[num146].localAI[1] = num147;
                Main.projectile[num147].localAI[1] = num148;
                Main.projectile[num145].originalDamage = damage;
                Main.projectile[num146].originalDamage = damage;
                Main.projectile[num147].originalDamage = damage;
                Main.projectile[num148].originalDamage = damage;
            }
            else if(num142 != -1 && num143 != -1) {
                int num149 = (int)Main.projectile[num143].ai[0];
                int num150 = Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num2, num3, projToShoot + 1, Damage, KnockBack, i, num149);
                int num151 = Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num2, num3, projToShoot + 2, Damage, KnockBack, i, num150);
                Main.projectile[num150].localAI[1] = num151;
                Main.projectile[num150].netUpdate = true;
                Main.projectile[num150].ai[1] = 1f;
                Main.projectile[num151].localAI[1] = num143;
                Main.projectile[num151].netUpdate = true;
                Main.projectile[num151].ai[1] = 1f;
                Main.projectile[num143].ai[0] = num151;
                Main.projectile[num143].netUpdate = true;
                Main.projectile[num143].ai[1] = 1f;
                Main.projectile[num150].originalDamage = damage;
                Main.projectile[num151].originalDamage = damage;
                Main.projectile[num143].originalDamage = damage;
            }
        }
        else if(sItem.type == 1309 || sItem.type == 4758 || sItem.type == 4269 || sItem.type == 5005) {
            player.SpawnMinionOnCursor(projectileSource_Item_WithPotentialAmmo, i, projToShoot, damage, KnockBack);
        }
        else if(sItem.shoot > 0 && (Main.projPet[sItem.shoot] || sItem.shoot == 72 || sItem.shoot == 18 || sItem.shoot == 500 || sItem.shoot == 650) && sItem.DamageType != DamageClass.Summon) {
            for(int num152 = 0; num152 < 1000; num152++) {
                Projectile projectile2 = Main.projectile[num152];
                if(projectile2.active && projectile2.owner == player.whoAmI) {
                    if(sItem.shoot == 72 && (projectile2.type == 72 || projectile2.type == 86 || projectile2.type == 87))
                        projectile2.Kill();
                    else if(sItem.type == 5131 && (projectile2.type == 881 || projectile2.type == 934))
                        projectile2.Kill();
                    else if(sItem.shoot == projectile2.type)
                        projectile2.Kill();
                }
            }

            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num2, num3, projToShoot, 0, 0f, i);
        }
        else if(sItem.type == 3006) {
            pointPoisition = player.GetFarthestSpawnPositionOnLine(pointPoisition, num2, num3);
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, 0f, 0f, projToShoot, Damage, KnockBack, i);
        }
        else if(sItem.type == 3014) {
            Vector2 pointPoisition3 = default(Vector2);
            pointPoisition3.X = Main.MouseWorld.X;
            pointPoisition3.Y = Main.MouseWorld.Y;
            player.LimitPointToPlayerReachableArea(ref pointPoisition3);
            while(Collision.CanHitLine(player.position, player.width, player.height, pointPoisition, 1, 1)) {
                pointPoisition.X += num2;
                pointPoisition.Y += num3;
                if((pointPoisition - pointPoisition3).Length() < 20f + Math.Abs(num2) + Math.Abs(num3)) {
                    pointPoisition = pointPoisition3;
                    break;
                }
            }

            bool flag5 = false;
            int num153 = (int)pointPoisition.Y / 16;
            int num154 = (int)pointPoisition.X / 16;
            int num155;
            for(num155 = num153; num153 < Main.maxTilesY - 10 && num153 - num155 < 30 && !WorldGen.SolidTile(num154, num153) && !TileID.Sets.Platforms[Main.tile[num154, num153].TileType]; num153++) {
            }

            if(!WorldGen.SolidTile(num154, num153) && !TileID.Sets.Platforms[Main.tile[num154, num153].TileType])
                flag5 = true;

            float num156 = num153 * 16;
            num153 = num155;
            while(num153 > 10 && num155 - num153 < 30 && !WorldGen.SolidTile(num154, num153)) {
                num153--;
            }

            float num157 = num153 * 16 + 16;
            float num158 = num156 - num157;
            int num159 = 15;
            if(num158 > (float)(16 * num159))
                num158 = 16 * num159;

            num157 = num156 - num158;
            pointPoisition.X = (int)(pointPoisition.X / 16f) * 16;
            if(!flag5)
                Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, 0f, 0f, projToShoot, Damage, KnockBack, i, num157, num158);
        }
        else if(sItem.type == 3384) {
            int num160 = ((player.altFunctionUse == 2) ? 1 : 0);
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num2, num3, projToShoot, Damage, KnockBack, i, 0f, num160);
        }
        else if(sItem.type == 3473) {
            float ai4 = (Main.rand.NextFloat() - 0.5f) * ((float)Math.PI / 4f);
            Vector2 vector40 = new Vector2(num2, num3);
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, vector40.X, vector40.Y, projToShoot, Damage, KnockBack, i, 0f, ai4);
        }
        else if(sItem.type == ItemID.Zenith) {
            int num161 = (player.itemAnimationMax - player.itemAnimation) / player.itemTime;
            Vector2 vector41 = new Vector2(num2, num3);
            int num162 = FinalFractalHelper.GetRandomProfileIndex();
            if(num161 == 0)
                num162 = 4956;

            Vector2 pointPoisition4 = Main.MouseWorld;
            player.LimitPointToPlayerReachableArea(ref pointPoisition4);
            Vector2 vector42 = pointPoisition4 - player.MountedCenter;
            if(num161 == 1 || num161 == 2) {
                int npcTargetIndex;
                bool zenithTarget = GetZenithTarget(player, pointPoisition4, 400f, out npcTargetIndex);
                if(zenithTarget)
                    vector42 = Main.npc[npcTargetIndex].Center - player.MountedCenter;

                bool flag6 = num161 == 2;
                if(num161 == 1 && !zenithTarget)
                    flag6 = true;

                if(flag6)
                    vector42 += Main.rand.NextVector2Circular(150f, 150f);
            }

            vector41 = vector42 / 2f;
            float ai5 = Main.rand.Next(-100, 101);
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition, vector41, projToShoot, Damage, KnockBack, i, ai5, num162);
        }
        else if(sItem.type == 3836) {
            float ai6 = Main.rand.NextFloat() * speed * 0.75f * (float)player.direction;
            Projectile.NewProjectile(velocity: new Vector2(num2, num3), spawnSource: projectileSource_Item_WithPotentialAmmo, position: pointPoisition, Type: projToShoot, Damage: Damage, KnockBack: KnockBack, Owner: i, ai0: ai6);
        }
        else if(sItem.type == 3858) {
            bool num163 = player.altFunctionUse == 2;
            Vector2 vector43 = new Vector2(num2, num3);
            if(num163) {
                vector43 *= 1.5f;
                float ai7 = (0.3f + 0.7f * Main.rand.NextFloat()) * speed * 1.75f * (float)player.direction;
                Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition, vector43, 708, (int)((float)Damage * 0.5f), KnockBack + 4f, i, ai7);
            }
            else {
                Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition, vector43, projToShoot, Damage, KnockBack, i);
            }
        }
        else if(sItem.type == 3859) {
            Vector2 vector44 = new Vector2(num2, num3);
            projToShoot = 710;
            vector44 *= 0.8f;
            Vector2 vector45 = vector44.SafeNormalize(-Vector2.UnitY);
            float num164 = (float)Math.PI / 180f * (float)(-player.direction);
            for(float num165 = -2.5f; num165 < 3f; num165 += 1f) {
                Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition, (vector44 + vector45 * num165 * 0.5f).RotatedBy(num165 * num164), projToShoot, Damage, KnockBack, i);
            }
        }
        else if(sItem.type == 3870) {
            Vector2 vector46 = Vector2.Normalize(new Vector2(num2, num3)) * 40f * sItem.scale;
            if(Collision.CanHit(pointPoisition, 0, 0, pointPoisition + vector46, 0, 0))
                pointPoisition += vector46;

            Vector2 vector47 = new Vector2(num2, num3);
            vector47 *= 0.8f;
            Vector2 vector48 = vector47.SafeNormalize(-Vector2.UnitY);
            float num166 = (float)Math.PI / 180f * (float)(-player.direction);
            for(int num167 = 0; num167 <= 2; num167++) {
                Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition, (vector47 + vector48 * num167 * 1f).RotatedBy((float)num167 * num166), projToShoot, Damage, KnockBack, i);
            }
        }
        else if(sItem.type == 3542) {
            float num168 = (Main.rand.NextFloat() - 0.5f) * ((float)Math.PI / 4f) * 0.7f;
            for(int num169 = 0; num169 < 10; num169++) {
                if(Collision.CanHit(pointPoisition, 0, 0, pointPoisition + new Vector2(num2, num3).RotatedBy(num168) * 100f, 0, 0))
                    break;

                num168 = (Main.rand.NextFloat() - 0.5f) * ((float)Math.PI / 4f) * 0.7f;
            }

            Vector2 vector49 = new Vector2(num2, num3).RotatedBy(num168) * (0.95f + Main.rand.NextFloat() * 0.3f);
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, vector49.X, vector49.Y, projToShoot, Damage, KnockBack, i);
        }
        else if(sItem.type == 3779) {
            float num170 = Main.rand.NextFloat() * ((float)Math.PI * 2f);
            for(int num171 = 0; num171 < 10; num171++) {
                if(Collision.CanHit(pointPoisition, 0, 0, pointPoisition + new Vector2(num2, num3).RotatedBy(num170) * 100f, 0, 0))
                    break;

                num170 = Main.rand.NextFloat() * ((float)Math.PI * 2f);
            }

            Vector2 vector50 = new Vector2(num2, num3).RotatedBy(num170) * (0.95f + Main.rand.NextFloat() * 0.3f);
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition + vector50 * 30f, Vector2.Zero, projToShoot, Damage, KnockBack, i, -2f);
        }
        else if(sItem.type == 3787) {
            float f = Main.rand.NextFloat() * ((float)Math.PI * 2f);
            float value3 = 20f;
            float value4 = 60f;
            Vector2 vector51 = pointPoisition + f.ToRotationVector2() * MathHelper.Lerp(value3, value4, Main.rand.NextFloat());
            for(int num172 = 0; num172 < 50; num172++) {
                vector51 = pointPoisition + f.ToRotationVector2() * MathHelper.Lerp(value3, value4, Main.rand.NextFloat());
                if(Collision.CanHit(pointPoisition, 0, 0, vector51 + (vector51 - pointPoisition).SafeNormalize(Vector2.UnitX) * 8f, 0, 0))
                    break;

                f = Main.rand.NextFloat() * ((float)Math.PI * 2f);
            }

            Vector2 v5 = Main.MouseWorld - vector51;
            Vector2 vector52 = new Vector2(num2, num3).SafeNormalize(Vector2.UnitY) * speed;
            v5 = v5.SafeNormalize(vector52) * speed;
            v5 = Vector2.Lerp(v5, vector52, 0.25f);
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, vector51, v5, projToShoot, Damage, KnockBack, i);
        }
        else if(sItem.type == 3788) {
            Vector2 vector53 = new Vector2(num2, num3);
            float num173 = (float)Math.PI / 4f;
            for(int num174 = 0; num174 < 2; num174++) {
                Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition, vector53 + vector53.SafeNormalize(Vector2.Zero).RotatedBy(num173 * (Main.rand.NextFloat() * 0.5f + 0.5f)) * Main.rand.NextFloatDirection() * 2f, projToShoot, Damage, KnockBack, i);
                Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition, vector53 + vector53.SafeNormalize(Vector2.Zero).RotatedBy((0f - num173) * (Main.rand.NextFloat() * 0.5f + 0.5f)) * Main.rand.NextFloatDirection() * 2f, projToShoot, Damage, KnockBack, i);
            }

            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition, vector53.SafeNormalize(Vector2.UnitX * player.direction) * (speed * 1.3f), 661, Damage * 2, KnockBack, i);
        }
        else if(sItem.type == 4463 || sItem.type == 486) {
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition, new Vector2(num2, num3), projToShoot, Damage, KnockBack, i);
        }
        else if(sItem.type == 46) {
            Vector2 vector54 = new Vector2(player.direction, player.gravDir * 4f).SafeNormalize(Vector2.UnitY).RotatedBy((float)Math.PI * 2f * Main.rand.NextFloatDirection() * 0.05f);
            Vector2 searchCenter = player.MountedCenter + new Vector2(70f, -40f) * player.Directions + vector54 * -10f;
            if(GetZenithTarget(player, searchCenter, 50f, out var npcTargetIndex2)) {
                NPC nPC3 = Main.npc[npcTargetIndex2];
                searchCenter = nPC3.Center + Main.rand.NextVector2Circular(nPC3.width / 2, nPC3.height / 2);
            }
            else {
                searchCenter += Main.rand.NextVector2Circular(20f, 20f);
            }

            float ai8 = 1f;
            if(Main.rand.Next(100) < player.GetCritChance(DamageClass.Melee)) {
                ai8 = 2f;
                Damage *= 2;
            }

            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, searchCenter, vector54 * 0.001f, projToShoot, (int)((double)Damage * 0.5), KnockBack, i, ai8);
            NetMessage.SendData(13, -1, -1, null, player.whoAmI);
        }
        else if(sItem.type == 273) {
            float adjustedItemScale = player.GetAdjustedItemScale(sItem);
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, player.MountedCenter, new Vector2(player.direction, 0f), projToShoot, Damage, KnockBack, i, (float)player.direction * player.gravDir, player.itemAnimationMax, adjustedItemScale);
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, player.MountedCenter, new Vector2(num2, num3), projToShoot, Damage, KnockBack, i, (float)player.direction * player.gravDir * 0.1f, 30f, adjustedItemScale);
            NetMessage.SendData(13, -1, -1, null, player.whoAmI);
        }
        else if(sItem.type == 368) {
            float adjustedItemScale2 = player.GetAdjustedItemScale(sItem);
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, player.MountedCenter, new Vector2(player.direction, 0f), projToShoot, Damage, KnockBack, i, (float)player.direction * player.gravDir, player.itemAnimationMax, adjustedItemScale2);
            NetMessage.SendData(13, -1, -1, null, player.whoAmI);
        }
        else if(sItem.type == 1826) {
            float adjustedItemScale3 = player.GetAdjustedItemScale(sItem);
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, player.MountedCenter, new Vector2(player.direction, 0f), projToShoot, Damage, KnockBack, i, (float)player.direction * player.gravDir, player.itemAnimationMax, adjustedItemScale3);
            NetMessage.SendData(13, -1, -1, null, player.whoAmI);
        }
        else if(sItem.type == 675) {
            float adjustedItemScale4 = player.GetAdjustedItemScale(sItem);
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, player.MountedCenter, new Vector2(player.direction, 0f), 972, Damage, KnockBack, i, (float)player.direction * player.gravDir, player.itemAnimationMax, adjustedItemScale4);
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, player.MountedCenter, new Vector2(num2, num3), projToShoot, Damage / 2, KnockBack, i, (float)player.direction * player.gravDir, 32f, adjustedItemScale4);
            NetMessage.SendData(13, -1, -1, null, player.whoAmI);
        }
        else if(sItem.type == 674) {
            float adjustedItemScale5 = player.GetAdjustedItemScale(sItem);
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, player.MountedCenter, new Vector2(player.direction, 0f), projToShoot, Damage, KnockBack, i, (float)player.direction * player.gravDir, player.itemAnimationMax, adjustedItemScale5);
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, player.MountedCenter, new Vector2(player.direction, 0f), 982, 0, KnockBack, i, (float)player.direction * player.gravDir, player.itemAnimationMax, adjustedItemScale5);
            NetMessage.SendData(13, -1, -1, null, player.whoAmI);
        }
        else if(sItem.type == 757) {
            float adjustedItemScale6 = player.GetAdjustedItemScale(sItem);
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, player.MountedCenter, new Vector2(player.direction, 0f), 984, Damage, KnockBack, i, (float)player.direction * player.gravDir, player.itemAnimationMax, adjustedItemScale6);
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, player.MountedCenter, new Vector2(num2, num3) * 5f, projToShoot, Damage, KnockBack, i, (float)player.direction * player.gravDir, 18f, adjustedItemScale6);
            NetMessage.SendData(13, -1, -1, null, player.whoAmI);
        }
        else if(sItem.type == 190) {
            Vector2 vector55 = player.MountedCenter + new Vector2(70f, -40f) * player.Directions;
            int npcTargetIndex3;
            bool zenithTarget2 = GetZenithTarget(player, vector55, 150f, out npcTargetIndex3);
            if(zenithTarget2) {
                NPC nPC4 = Main.npc[npcTargetIndex3];
                vector55 = Main.rand.NextVector2FromRectangle(nPC4.Hitbox);
            }
            else {
                vector55 += Main.rand.NextVector2Circular(20f, 20f);
            }

            Vector2 vector56 = player.Center + new Vector2(Main.rand.NextFloatDirection() * (float)player.width / 2f, player.height / 2) * player.Directions;
            Vector2 v6 = vector55 - vector56;
            float num175 = ((float)Math.PI + (float)Math.PI * 2f * Main.rand.NextFloat() * 1.5f) * ((float)(-player.direction) * player.gravDir);
            int num176 = 60;
            float num177 = num175 / (float)num176;
            float num178 = 16f;
            float num179 = v6.Length();
            if(Math.Abs(num177) >= 0.17f)
                num177 *= 0.7f;

            _ = player.direction;
            _ = player.gravDir;
            Vector2 vector57 = Vector2.UnitX * num178;
            Vector2 v7 = vector57;
            int num180 = 0;
            while(v7.Length() < num179 && num180 < num176) {
                num180++;
                v7 += vector57;
                vector57 = vector57.RotatedBy(num177);
            }

            float num181 = v7.ToRotation();
            Vector2 spinningpoint2 = v6.SafeNormalize(Vector2.UnitY).RotatedBy(0f - num181 - num177) * num178;
            if(num180 == num176)
                spinningpoint2 = new Vector2(player.direction, 0f) * num178;

            if(!zenithTarget2) {
                vector56.Y -= player.gravDir * 24f;
                spinningpoint2 = spinningpoint2.RotatedBy((float)player.direction * player.gravDir * ((float)Math.PI * 2f) * 0.14f);
            }

            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, vector56, spinningpoint2, projToShoot, (int)((double)Damage * 0.25), KnockBack, i, num177, num180);
            NetMessage.SendData(13, -1, -1, null, player.whoAmI);
        }
        else if(sItem.type == 3475) {
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num2, num3, 615, Damage, KnockBack, i, 5 * Main.rand.Next(0, 20));
        }
        else if(sItem.type == 3930) {
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num2, num3, 714, Damage, KnockBack, i, 5 * Main.rand.Next(0, 20));
        }
        else if(sItem.type == 3540) {
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num2, num3, 630, Damage, KnockBack, i);
        }
        else if(sItem.type == 5451) {
            for(int num182 = 0; num182 < 1000; num182++) {
                Projectile projectile3 = Main.projectile[num182];
                if(projectile3.type == projToShoot && projectile3.owner == player.whoAmI)
                    projectile3.Kill();
            }

            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num2, num3, projToShoot, Damage, KnockBack, i);
        }
        else if(sItem.type == 3854) {
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num2, num3, 705, Damage, KnockBack, i);
        }
        else if(sItem.type == 3546) {
            for(int num183 = 0; num183 < 2; num183++) {
                float num184 = num2;
                float num185 = num3;
                num184 += (float)Main.rand.Next(-40, 41) * 0.05f;
                num185 += (float)Main.rand.Next(-40, 41) * 0.05f;
                Vector2 vector58 = pointPoisition + Vector2.Normalize(new Vector2(num184, num185).RotatedBy(-(float)Math.PI / 2f * (float)player.direction)) * 6f;
                Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, vector58.X, vector58.Y, num184, num185, 167 + Main.rand.Next(4), Damage, KnockBack, i, 0f, 1f);
            }
        }
        else if(sItem.type == 3350) {
            float num186 = num2;
            float num187 = num3;
            num186 += (float)Main.rand.Next(-1, 2) * 0.5f;
            num187 += (float)Main.rand.Next(-1, 2) * 0.5f;
            if(Collision.CanHitLine(player.Center, 0, 0, pointPoisition + new Vector2(num186, num187) * 2f, 0, 0))
                pointPoisition += new Vector2(num186, num187);

            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y - player.gravDir * 4f, num186, num187, projToShoot, Damage, KnockBack, i, 0f, (float)Main.rand.Next(12) / 6f);
        }
        else if(sItem.type == 3852) {
            if(player.altFunctionUse == 2)
                Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, player.Bottom.Y - 100f, (float)player.direction * speed, 0f, 704, (int)((float)Damage * 1.75f), KnockBack, i);
            else
                Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num2, num3, projToShoot, Damage, KnockBack, i);
        }
        else if(sItem.type == 3818 || sItem.type == 3819 || sItem.type == 3820 || sItem.type == 3824 || sItem.type == 3825 || sItem.type == 3826 || sItem.type == 3829 || sItem.type == 3830 || sItem.type == 3831 || sItem.type == 3832 || sItem.type == 3833 || sItem.type == 3834) {
            PayDD2CrystalsBeforeUse(player, sItem);
            player.FindSentryRestingSpot(sItem.shoot, out var worldX, out var worldY, out var pushYUp);
            int num188 = 0;
            int num189 = 0;
            int num190 = 0;
            switch(sItem.type) {
            case 3824:
            case 3825:
            case 3826:
                num188 = 1;
                num189 = Projectile.GetBallistraShotDelay(player);
                break;
            case 3832:
            case 3833:
            case 3834:
                num190 = Projectile.GetExplosiveTrapCooldown(player);
                break;
            case 3818:
                num188 = 1;
                num189 = 80;
                break;
            case 3819:
                num188 = 1;
                num189 = 70;
                break;
            case 3820:
                num188 = 1;
                num189 = 60;
                break;
            }

            int num191 = Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, worldX, worldY - pushYUp, 0f, 0f, projToShoot, Damage, KnockBack, i, num188, num189);
            Main.projectile[num191].originalDamage = damage;
            Main.projectile[num191].localAI[0] = num190;
            player.UpdateMaxTurrets();
        }
        else if(sItem.type == 65) {
            Vector2 vector59 = new Vector2(num2, num3);
            new Vector2(100f, 0f);
            Vector2 mouseWorld2 = Main.MouseWorld;
            Vector2 vec = mouseWorld2;
            Vector2 vector60 = (pointPoisition - mouseWorld2).SafeNormalize(new Vector2(0f, -1f));
            while(vec.Y > pointPoisition.Y && WorldGen.SolidTile(vec.ToTileCoordinates())) {
                vec += vector60 * 16f;
            }

            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition, vector59, projToShoot, Damage, KnockBack, i, 0f, vec.Y);
        }
        else if(sItem.type == 4923) {
            float adjustedItemScale7 = player.GetAdjustedItemScale(sItem);
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num2, num3, projToShoot, Damage, KnockBack, i, 0f, adjustedItemScale7);
        }
        else if(sItem.type == 1910) {
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num2, num3, projToShoot, Damage, KnockBack, i, 1f);
        }
        else if(sItem.type == 5134) {
            Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num2, num3, projToShoot, Damage, KnockBack, i, 0f, 1f);
        }
        else {
            DebugPrint("ItemCheck_Shoot9");
            //Ruby Staff Entry
            int num192 = Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num2, num3, projToShoot, Damage, KnockBack, i);
            if(sItem.type == ItemID.FrostStaff)
                Main.projectile[num192].DamageType = DamageClass.Magic;

            if(sItem.type == ItemID.IceBlade || sItem.type == ItemID.Frostbrand)
                Main.projectile[num192].DamageType = DamageClass.Melee;

            if(projToShoot == 80) {
                Main.projectile[num192].ai[0] = Player.tileTargetX;
                Main.projectile[num192].ai[1] = Player.tileTargetY;
            }

            if(sItem.type == ItemID.ProximityMineLauncher)
                DestroyOldestProximityMinesOverMinesCap(player, 20);        //!!!

            if(projToShoot == 442) {
                Main.projectile[num192].ai[0] = Player.tileTargetX;
                Main.projectile[num192].ai[1] = Player.tileTargetY;
            }

            if(projToShoot == 826)
                Main.projectile[num192].ai[1] = Main.rand.Next(3);

            if(sItem.type == 949)
                Main.projectile[num192].ai[1] = 1f;

            if(Main.projectile[num192].aiStyle == 99)
                AchievementsHelper.HandleSpecialEvent(player, 7);

            if(Main.projectile[num192].aiStyle == 160 && Main.IsItAHappyWindyDay)
                AchievementsHelper.HandleSpecialEvent(player, 17);

            NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, player.whoAmI);
        }
        DebugPrint("ItemCheck_Shoot10");
    }

    public static Point FindSharpTearsSpot(Player player, Vector2 targetSpot) {
        Point point = targetSpot.ToTileCoordinates();
        Vector2 center = player.Center;
        Vector2 endPoint = targetSpot;
        int samplesToTake = 3;
        float samplingWidth = 4f;
        Collision.AimingLaserScan(center, endPoint, samplingWidth, samplesToTake, out var vectorTowardsTarget, out var samples);
        float num = float.PositiveInfinity;
        for(int i = 0; i < samples.Length; i++) {
            if(samples[i] < num)
                num = samples[i];
        }

        targetSpot = center + vectorTowardsTarget.SafeNormalize(Vector2.Zero) * num;
        point = targetSpot.ToTileCoordinates();
        Rectangle value = new Rectangle(point.X, point.Y, 1, 1);
        value.Inflate(6, 16);
        Rectangle value2 = new Rectangle(0, 0, Main.maxTilesX, Main.maxTilesY);
        value2.Inflate(-40, -40);
        value = Rectangle.Intersect(value, value2);
        List<Point> list = new List<Point>();
        List<Point> list2 = new List<Point>();
        for(int j = value.Left; j <= value.Right; j++) {
            for(int k = value.Top; k <= value.Bottom; k++) {
                if(!WorldGen.SolidTile2(j, k))
                    continue;

                Vector2 value3 = new Vector2(j * 16 + 8, k * 16 + 8);
                if(!(Vector2.Distance(targetSpot, value3) > 200f)) {
                    if(FindSharpTearsOpening(j, k, j > point.X, j < point.X, k > point.Y, k < point.Y))
                        list.Add(new Point(j, k));
                    else
                        list2.Add(new Point(j, k));
                }
            }
        }

        if(list.Count == 0 && list2.Count == 0)
            list.Add((player.Center.ToTileCoordinates().ToVector2() + Main.rand.NextVector2Square(-2f, 2f)).ToPoint());

        List<Point> list3 = list;
        if(list3.Count == 0)
            list3 = list2;

        int index = Main.rand.Next(list3.Count);
        return list3[index];
    }

    public static bool FindSharpTearsOpening(int x, int y, bool acceptLeft, bool acceptRight, bool acceptUp, bool acceptDown) {
        if(acceptLeft && !WorldGen.SolidTile(x - 1, y))
            return true;

        if(acceptRight && !WorldGen.SolidTile(x + 1, y))
            return true;

        if(acceptUp && !WorldGen.SolidTile(x, y - 1))
            return true;

        if(acceptDown && !WorldGen.SolidTile(x, y + 1))
            return true;

        return false;
    }

    public static bool GetSparkleGuitarTarget(Player player, out List<NPC> validTargets) {
        validTargets = new List<NPC>();
        Rectangle value = Utils.CenteredRectangle(player.Center, new Vector2(1000f, 800f));
        for(int i = 0; i < 200; i++) {
            NPC nPC = Main.npc[i];
            if(nPC.CanBeChasedBy(player) && nPC.Hitbox.Intersects(value))
                validTargets.Add(nPC);
        }

        if(validTargets.Count == 0)
            return false;

        return true;
    }

    public static bool GetZenithTarget(Player player, Vector2 searchCenter, float maxDistance, out int npcTargetIndex) {
        npcTargetIndex = 0;
        int? num = null;
        float num2 = maxDistance;
        for(int i = 0; i < 200; i++) {
            NPC nPC = Main.npc[i];
            if(nPC.CanBeChasedBy(player)) {
                float num3 = searchCenter.Distance(nPC.Center);
                if(!(num2 <= num3)) {
                    num = i;
                    num2 = num3;
                }
            }
        }

        if(!num.HasValue)
            return false;

        npcTargetIndex = num.Value;
        return true;
    }

    public static void PayDD2CrystalsBeforeUse(Player player, Item item) {
        int requiredDD2CrystalsToUse = GetRequiredDD2CrystalsToUse(item);
        for(int i = 0; i < requiredDD2CrystalsToUse; i++) {
            player.ConsumeItem(3822, reverseOrder: true);
        }
    }

    public static int GetRequiredDD2CrystalsToUse(Item item) {
        switch(item.type) {
        case 3818:
        case 3819:
        case 3820:
            return 10;
        case 3824:
        case 3825:
        case 3826:
            return 10;
        case 3832:
        case 3833:
        case 3834:
            return 10;
        case 3829:
        case 3830:
        case 3831:
            return 10;
        default:
            return 0;
        }
    }

    public static void DestroyOldestProximityMinesOverMinesCap(Player player, int minesCap) {
        //_oldestProjCheckList.Clear();
        //for (int i = 0; i < 1000; i++) {
        //	Projectile projectile = Main.projectile[i];
        //	if (projectile.active && projectile.owner == whoAmI) {
        //		switch (projectile.type) {
        //			case 135:
        //			case 138:
        //			case 141:
        //			case 144:
        //			case 778:
        //			case 782:
        //			case 786:
        //			case 789:
        //			case 792:
        //			case 795:
        //			case 798:
        //			case 801:
        //				_oldestProjCheckList.Add(projectile);
        //				break;
        //		}
        //	}
        //}
        //
        //while (_oldestProjCheckList.Count > minesCap) {
        //	Projectile projectile2 = _oldestProjCheckList[0];
        //	for (int j = 1; j < _oldestProjCheckList.Count; j++) {
        //		if (_oldestProjCheckList[j].timeLeft < projectile2.timeLeft)
        //			projectile2 = _oldestProjCheckList[j];
        //	}
        //
        //	projectile2.Kill();
        //	_oldestProjCheckList.Remove(projectile2);
        //}
        //
        //_oldestProjCheckList.Clear();
    }
    #endregion
    #endregion
    #region NPC.cs
    #region AI
    public static void AI_Hornet(NPC npc)   //hornet is 42
    {
        if(npc.target < 0 || npc.target <= 255 || Main.player[npc.target].dead)//迷惑
            npc.TargetClosest();

        NPCAimedTarget targetData = npc.GetTargetData();
        bool targetPlayerDead = false;
        if(targetData.Type == NPCTargetType.Player)
            targetPlayerDead = Main.player[npc.target].dead;

        float speed = 3.5f; //default = 6f;
        float acc = .021f;  //default = 0.05f;
        #region NPC.type == 42
        speed *= 1f - npc.scale;
        acc *= 1f - npc.scale;
        //在地面上时轻微减小竖直速度
        if((double)(npc.position.Y / 16f) < Main.worldSurface) {
            if(Main.player[npc.target].position.Y - npc.position.Y > 300f && npc.velocity.Y < 0f)
                npc.velocity.Y *= 0.97f;

            if(Main.player[npc.target].position.Y - npc.position.Y < 80f && npc.velocity.Y > 0f)
                npc.velocity.Y *= 0.97f;
        }
        #endregion

        #region 计算目标向量
        Vector2 center = npc.Center;
        float targetVecX = targetData.Position.X + (targetData.Width / 2);
        float targetVecY = targetData.Position.Y + (targetData.Height / 2);
        targetVecX = (int)(targetVecX / 8f) * 8;
        targetVecY = (int)(targetVecY / 8f) * 8;
        center.X = (int)(center.X / 8f) * 8;
        center.Y = (int)(center.Y / 8f) * 8;
        targetVecX -= center.X;
        targetVecY -= center.Y;
        float targetDistance = (float)Math.Sqrt(targetVecX * targetVecX + targetVecY * targetVecY);

        if(targetDistance == 0f) {
            targetVecX = npc.velocity.X;
            targetVecY = npc.velocity.Y;
        }
        else {
            targetDistance = speed / targetDistance;
            targetVecX *= targetDistance;
            targetVecY *= targetDistance;
        }
        #endregion

        #region if flag3 ( true for hornet ): 斜45度正方形逆时针小幅加速
        npc.ai[0] += 1f;
        if(npc.ai[0] > 0f)
            npc.velocity.Y += 0.023f;
        else
            npc.velocity.Y -= 0.023f;

        if(npc.ai[0] < -100f || npc.ai[0] > 100f)
            npc.velocity.X += 0.023f;
        else
            npc.velocity.X -= 0.023f;

        if(npc.ai[0] > 200f)
            npc.ai[0] = -200f;
        #endregion

        //若目标玩家死亡则朝斜上退出
        if(targetPlayerDead) {
            targetVecX = npc.direction * speed / 2f;
            targetVecY = (0f - speed) / 2f;
        }

        #region 朝目标小幅加速
        if(npc.velocity.X < targetVecX) {
            npc.velocity.X += acc;
        }
        else if(npc.velocity.X > targetVecX) {
            npc.velocity.X -= acc;
        }

        if(npc.velocity.Y < targetVecY) {
            npc.velocity.Y += acc;
        }
        else if(npc.velocity.Y > targetVecY) {
            npc.velocity.Y -= acc;
        }
        #endregion

        #region type 42: 根据速度方向左右翻转及大小轻微前倾
        if(npc.velocity.X > 0f)
            npc.spriteDirection = 1;

        if(npc.velocity.X < 0f)
            npc.spriteDirection = -1;

        npc.rotation = npc.velocity.X * 0.1f;
        #endregion

        #region type 42: 处理方块碰撞
        if(npc.collideX) {
            npc.netUpdate = true;
            npc.velocity.X = npc.oldVelocity.X * -0.7f;
            if(npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 2f)
                npc.velocity.X = 2f;

            if(npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -2f)
                npc.velocity.X = -2f;
        }

        if(npc.collideY) {
            npc.netUpdate = true;
            npc.velocity.Y = npc.oldVelocity.Y * -0.7f;
            if(npc.velocity.Y > 0f && npc.velocity.Y < 1.5)
                npc.velocity.Y = 2f;

            if(npc.velocity.Y < 0f && npc.velocity.Y > -1.5)
                npc.velocity.Y = -2f;
        }
        #endregion

        #region type 42: 处理目标, 发射尖刺

        if(npc.wet) {
            if(npc.velocity.Y > 0f)
                npc.velocity.Y *= 0.95f;

            npc.velocity.Y -= 0.5f;
            if(npc.velocity.Y < -4f)
                npc.velocity.Y = -4f;

            npc.TargetClosest();
        }

        if(npc.ai[1] == 101f) {
            SoundEngine.PlaySound(SoundID.Item17, npc.position);
            npc.ai[1] = 0f;
        }

        if(Main.netMode != NetmodeID.MultiplayerClient) {
            npc.ai[1] += Main.rand.Next(5, 20) * 0.1f * npc.scale;

            if(Main.getGoodWorld)
                npc.ai[1] += Main.rand.Next(5, 20) * 0.1f * npc.scale;

            if(targetData.Type == NPCTargetType.Player) {
                Player player = Main.player[npc.target];
                if(player != null && player.stealth == 0f && player.itemAnimation == 0)
                    npc.ai[1] = 0f;
            }

            if(npc.ai[1] >= 130f) {
                if(targetData.Type != 0 && Collision.CanHit(npc, targetData)) {
                    Vector2 center2 = npc.Center;
                    float pSpeedX = targetData.Center.X - center2.X + Main.rand.Next(-20, 21);
                    float pSpeedY = targetData.Center.Y - center2.Y + Main.rand.Next(-20, 21);
                    if((pSpeedX < 0f && npc.velocity.X < 0f) || (pSpeedX > 0f && npc.velocity.X > 0f)) {
                        float num23 = (float)Math.Sqrt(pSpeedX * pSpeedX + pSpeedY * pSpeedY);
                        num23 = 8f / num23;
                        pSpeedX *= num23;
                        pSpeedY *= num23;
                        int damage = (int)(10f * npc.scale);
                        int pid = ProjectileID.Stinger;
                        int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), center2.X, center2.Y, pSpeedX, pSpeedY, pid, damage, 0f, Main.myPlayer);
                        Main.projectile[proj].timeLeft = 300;
                        npc.ai[1] = 101f;
                        npc.netUpdate = true;
                    }
                    else {
                        npc.ai[1] = 0f;
                    }
                }
                else {
                    npc.ai[1] = 0f;
                }
            }
        }
        #endregion

        if(targetPlayerDead) {
            npc.velocity.Y -= acc * 2f;
            npc.EncourageDespawn(10);
        }

        if(((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
            npc.netUpdate = true;
    }

    public static void AI_Flying(NPC npc) {
        if(npc.type == NPCID.Bee || npc.type == NPCID.BeeSmall)
            NPCUtils.TargetClosestNonBees(npc);

        else if(npc.target < 0 || npc.target <= 255 || Main.player[npc.target].dead)
            npc.TargetClosest();

        if(npc.type == NPCID.BloodSquid) {
            if(Main.dayTime) {
                npc.velocity.Y -= 0.3f;
                npc.EncourageDespawn(60);
            }

            npc.position += npc.netOffset;
            if(npc.alpha == 255) {
                npc.spriteDirection = npc.direction;
                npc.velocity.Y = -6f;
                for(int i = 0; i < 35; i++) {
                    Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, 5);
                    dust.velocity *= 1f;
                    dust.scale = 1f + Main.rand.NextFloat() * 0.5f;
                    dust.fadeIn = 1.5f + Main.rand.NextFloat() * 0.5f;
                    dust.velocity += npc.velocity * 0.5f;
                }
            }

            npc.alpha -= 15;
            if(npc.alpha < 0)
                npc.alpha = 0;

            if(npc.alpha != 0) {
                for(int j = 0; j < 2; j++) {
                    Dust dust2 = Dust.NewDustDirect(npc.position, npc.width, npc.height, 5);
                    dust2.velocity *= 1f;
                    dust2.scale = 1f + Main.rand.NextFloat() * 0.5f;
                    dust2.fadeIn = 1.5f + Main.rand.NextFloat() * 0.5f;
                    dust2.velocity += npc.velocity * 0.3f;
                }
            }

            npc.position -= npc.netOffset;
        }

        NPCAimedTarget targetData = npc.GetTargetData();
        bool targetPlayerDead = false;
        if(targetData.Type == NPCTargetType.Player)
            targetPlayerDead = Main.player[npc.target].dead;

        float speed = 6f;
        float acc = 0.05f;
        if(npc.type == 6 || npc.type == 173) {
            speed = 4f;
            acc = 0.02f;
            if(npc.type == 6 && Main.expertMode)
                acc = 0.035f;

            if(Main.remixWorld) {
                acc = 0.06f;
                speed = 5f;
            }
        }
        else if(npc.type == 94) {
            speed = 4.2f;
            acc = 0.022f;
        }
        else if(npc.type == 619) {
            speed = 6f;
            acc = 0.1f;
        }
        else if(npc.type == 252) {
            if(targetData.Type != 0 && Collision.CanHit(npc, targetData)) {
                speed = 6f;
                acc = 0.1f;
            }
            else {
                acc = 0.01f;
                speed = 2f;
            }
        }
        else if(npc.type == 42 || (npc.type >= 231 && npc.type <= 235)) {
            speed = 3.5f;
            acc = 0.021f;
            if(npc.type == 231) {
                speed = 3f;
                acc = 0.017f;
            }

            speed *= 1f - npc.scale;
            acc *= 1f - npc.scale;
            if((double)(npc.position.Y / 16f) < Main.worldSurface) {
                if(Main.player[npc.target].position.Y - npc.position.Y > 300f && npc.velocity.Y < 0f)
                    npc.velocity.Y *= 0.97f;

                if(Main.player[npc.target].position.Y - npc.position.Y < 80f && npc.velocity.Y > 0f)
                    npc.velocity.Y *= 0.97f;
            }
        }
        else if(npc.type == 205) {
            speed = 3.25f;
            acc = 0.018f;
        }
        else if(npc.type == 176) {
            speed = 4f;
            acc = 0.017f;
        }
        else if(npc.type == 23) {
            speed = 1f;
            acc = 0.03f;
        }
        else if(npc.type == 5) {
            speed = 5f;
            acc = 0.03f;
        }
        else if(npc.type == 210 || npc.type == 211) {
            npc.ai[1] += 1f;
            float num3 = (npc.ai[1] - 60f) / 60f;
            if(num3 > 1f) {
                num3 = 1f;
            }
            else {
                if(npc.velocity.X > 6f)
                    npc.velocity.X = 6f;

                if(npc.velocity.X < -6f)
                    npc.velocity.X = -6f;

                if(npc.velocity.Y > 6f)
                    npc.velocity.Y = 6f;

                if(npc.velocity.Y < -6f)
                    npc.velocity.Y = -6f;
            }

            speed = 5f;
            acc = 0.1f;
            acc *= num3;
        }
        else if(npc.type == 139 && Main.zenithWorld) {
            speed = 3f;
        }

        Vector2 center = npc.Center;
        float num4 = targetData.Position.X + (targetData.Width / 2);
        float num5 = targetData.Position.Y + (targetData.Height / 2);
        num4 = (int)(num4 / 8f) * 8;
        num5 = (int)(num5 / 8f) * 8;
        center.X = (int)(center.X / 8f) * 8;
        center.Y = (int)(center.Y / 8f) * 8;
        num4 -= center.X;
        num5 -= center.Y;
        float num6 = (float)Math.Sqrt(num4 * num4 + num5 * num5);
        float num7 = num6;
        bool tooFarToTarget = false;
        if(num6 > 600f)
            tooFarToTarget = true;

        if(num6 == 0f) {
            num4 = npc.velocity.X;
            num5 = npc.velocity.Y;
        }
        else {
            num6 = speed / num6;
            num4 *= num6;
            num5 *= num6;
        }

        bool num8 = npc.type == NPCID.EaterofSouls || npc.type == NPCID.Probe || npc.type == NPCID.Crimera || npc.type == NPCID.Moth;
        bool flag3 = npc.type == NPCID.Hornet || npc.type == NPCID.Corruptor || npc.type == NPCID.BloodSquid || npc.type == NPCID.MossHornet || npc.type == NPCID.Bee || npc.type == NPCID.BeeSmall || (npc.type >= NPCID.HornetFatty && npc.type <= NPCID.HornetStingy);
        bool flag4 = npc.type != NPCID.Crimera && npc.type != NPCID.EaterofSouls && npc.type != NPCID.Hornet && (npc.type < NPCID.HornetFatty || npc.type > NPCID.HornetStingy) && npc.type != NPCID.Corruptor && npc.type != NPCID.Probe && npc.type != NPCID.BloodSquid;
        if(num8 || flag3) {
            if(num7 > 100f || flag3) {
                npc.ai[0] += 1f;
                if(npc.ai[0] > 0f)
                    npc.velocity.Y += 0.023f;
                else
                    npc.velocity.Y -= 0.023f;

                if(npc.ai[0] < -100f || npc.ai[0] > 100f)
                    npc.velocity.X += 0.023f;
                else
                    npc.velocity.X -= 0.023f;

                if(npc.ai[0] > 200f)
                    npc.ai[0] = -200f;
            }

            if(num7 < 150f && (npc.type == 6 || npc.type == 94 || npc.type == 173 || npc.type == 619)) {
                npc.velocity.X += num4 * 0.007f;
                npc.velocity.Y += num5 * 0.007f;
            }
        }

        if(targetPlayerDead) {
            num4 = (float)npc.direction * speed / 2f;
            num5 = (0f - speed) / 2f;
        }
        else if(npc.type == 619 && npc.Center.Y > targetData.Center.Y - 200f) {
            npc.velocity.Y -= 0.3f;
        }

        if(npc.type == 139 && npc.ai[3] != 0f) {
            if(NPC.IsMechQueenUp) {
                NPC nPC = Main.npc[NPC.mechQueen];
                Vector2 vector2 = new Vector2(26f * npc.ai[3], 0f);
                int num9 = (int)npc.ai[2];
                if(num9 < 0 || num9 >= 200) {
                    num9 = NPC.FindFirstNPC(134);
                    npc.ai[2] = num9;
                    npc.netUpdate = true;
                }

                if(num9 > -1) {
                    NPC nPC2 = Main.npc[num9];
                    if(!nPC2.active || nPC2.type != 134) {
                        npc.dontTakeDamage = false;
                        if(npc.ai[3] > 0f)
                            npc.netUpdate = true;

                        npc.ai[3] = 0f;
                    }
                    else {
                        Vector2 spinningpoint = nPC2.Center + vector2;
                        spinningpoint = spinningpoint.RotatedBy(nPC2.rotation, nPC2.Center);
                        npc.Center = spinningpoint;
                        npc.velocity = nPC.velocity;
                        npc.dontTakeDamage = true;
                    }
                }
                else {
                    npc.dontTakeDamage = false;
                    if(npc.ai[3] > 0f)
                        npc.netUpdate = true;

                    npc.ai[3] = 0f;
                }
            }
            else {
                npc.dontTakeDamage = false;
                if(npc.ai[3] > 0f)
                    npc.netUpdate = true;

                npc.ai[3] = 0f;
            }
        }
        else {
            if(npc.type == 139)
                npc.dontTakeDamage = false;

            if(npc.velocity.X < num4) {
                npc.velocity.X += acc;
                if(flag4 && npc.velocity.X < 0f && num4 > 0f)
                    npc.velocity.X += acc;
            }
            else if(npc.velocity.X > num4) {
                npc.velocity.X -= acc;
                if(flag4 && npc.velocity.X > 0f && num4 < 0f)
                    npc.velocity.X -= acc;
            }

            if(npc.velocity.Y < num5) {
                npc.velocity.Y += acc;
                if(flag4 && npc.velocity.Y < 0f && num5 > 0f)
                    npc.velocity.Y += acc;
            }
            else if(npc.velocity.Y > num5) {
                npc.velocity.Y -= acc;
                if(flag4 && npc.velocity.Y > 0f && num5 < 0f)
                    npc.velocity.Y -= acc;
            }
        }

        if(npc.type == 23) {
            if(num4 > 0f) {
                npc.spriteDirection = 1;
                npc.rotation = (float)Math.Atan2(num5, num4);
            }
            else if(num4 < 0f) {
                npc.spriteDirection = -1;
                npc.rotation = (float)Math.Atan2(num5, num4) + 3.14f;
            }
        }
        else if(npc.type == 139) {
            npc.localAI[0] += 1f;
            if(npc.ai[3] != 0f)
                npc.localAI[0] += 2f;

            if(npc.justHit)
                npc.localAI[0] = 0f;

            float num10 = 120f;
            if(NPC.IsMechQueenUp)
                num10 = 360f;

            if(Main.netMode != 1 && npc.localAI[0] >= num10) {
                npc.localAI[0] = 0f;
                if(targetData.Type != 0 && Collision.CanHit(npc, targetData)) {
                    int attackDamage_ForProjectiles = npc.GetAttackDamage_ForProjectiles(25f, 22f);
                    int num11 = 84;
                    Vector2 vector3 = new Vector2(num4, num5);
                    if(NPC.IsMechQueenUp) {
                        Vector2 v = targetData.Center - npc.Center - targetData.Velocity * 20f;
                        float num12 = 8f;
                        vector3 = v.SafeNormalize(Vector2.UnitY) * num12;
                    }

                    Projectile.NewProjectile(npc.GetSource_FromAI(), center.X, center.Y, vector3.X, vector3.Y, num11, attackDamage_ForProjectiles, 0f, Main.myPlayer);
                }
            }

            int num13 = (int)npc.position.X + npc.width / 2;
            int num14 = (int)npc.position.Y + npc.height / 2;
            num13 /= 16;
            num14 /= 16;
            if(WorldGen.InWorld(num13, num14) && !WorldGen.SolidTile(num13, num14))
                Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.3f, 0.1f, 0.05f);

            if(num4 > 0f) {
                npc.spriteDirection = 1;
                npc.rotation = (float)Math.Atan2(num5, num4);
            }

            if(num4 < 0f) {
                npc.spriteDirection = -1;
                npc.rotation = (float)Math.Atan2(num5, num4) + 3.14f;
            }
        }
        else if(npc.type == 6 || npc.type == 94 || npc.type == 173 || npc.type == 619) {
            npc.rotation = (float)Math.Atan2(num5, num4) - 1.57f;
        }
        else if(npc.type == 42 || npc.type == 176 || npc.type == 205 || (npc.type >= 231 && npc.type <= 235)) {
            if(npc.velocity.X > 0f)
                npc.spriteDirection = 1;

            if(npc.velocity.X < 0f)
                npc.spriteDirection = -1;

            npc.rotation = npc.velocity.X * 0.1f;
        }
        else {
            npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) - 1.57f;
        }

        if(npc.type == 6 || npc.type == 619 || npc.type == 23 || npc.type == 42 || npc.type == 94 || npc.type == 139 || npc.type == 173 || npc.type == 176 || npc.type == 205 || npc.type == 210 || npc.type == 211 || (npc.type >= 231 && npc.type <= 235)) {
            float num15 = 0.7f;
            if(npc.type == 6 || npc.type == 173)
                num15 = 0.4f;

            if(npc.collideX) {
                npc.netUpdate = true;
                npc.velocity.X = npc.oldVelocity.X * (0f - num15);
                if(npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 2f)
                    npc.velocity.X = 2f;

                if(npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -2f)
                    npc.velocity.X = -2f;
            }

            if(npc.collideY) {
                npc.netUpdate = true;
                npc.velocity.Y = npc.oldVelocity.Y * (0f - num15);
                if(npc.velocity.Y > 0f && (double)npc.velocity.Y < 1.5)
                    npc.velocity.Y = 2f;

                if(npc.velocity.Y < 0f && (double)npc.velocity.Y > -1.5)
                    npc.velocity.Y = -2f;
            }

            npc.position += npc.netOffset;
            if(npc.type == 619) {
                int num16 = Dust.NewDust(npc.position, npc.width, npc.height, 5, npc.velocity.X * 0.2f, npc.velocity.Y * 0.2f, 100);
                Main.dust[num16].velocity *= 0.5f;
            }
            else if(npc.type != 42 && npc.type != 139 && npc.type != 176 && npc.type != 205 && npc.type != 210 && npc.type != 211 && npc.type != 252 && (npc.type < 231 || npc.type > 235) && Main.rand.Next(20) == 0) {
                int num17 = 18;
                if(npc.type == 173)
                    num17 = 5;

                int num18 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + (float)npc.height * 0.25f), npc.width, (int)((float)npc.height * 0.5f), num17, npc.velocity.X, 2f, 75, npc.color, npc.scale);
                Main.dust[num18].velocity.X *= 0.5f;
                Main.dust[num18].velocity.Y *= 0.1f;
            }

            npc.position -= npc.netOffset;
        }
        else if(npc.type != 252 && Main.rand.Next(40) == 0) {
            int num19 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + (float)npc.height * 0.25f), npc.width, (int)((float)npc.height * 0.5f), 5, npc.velocity.X, 2f);
            Main.dust[num19].velocity.X *= 0.5f;
            Main.dust[num19].velocity.Y *= 0.1f;
        }

        if((npc.type == 6 || npc.type == 94 || npc.type == 173 || npc.type == 619) && npc.wet) {
            if(npc.velocity.Y > 0f)
                npc.velocity.Y *= 0.95f;

            npc.velocity.Y -= 0.3f;
            if(npc.velocity.Y < -2f)
                npc.velocity.Y = -2f;
        }

        if(npc.type == 205 && npc.wet) {
            if(npc.velocity.Y > 0f)
                npc.velocity.Y *= 0.95f;

            npc.velocity.Y -= 0.5f;
            if(npc.velocity.Y < -4f)
                npc.velocity.Y = -4f;

            npc.TargetClosest();
        }

        if(npc.type == 42 || npc.type == 176 || (npc.type >= 231 && npc.type <= 235)) {
            if(npc.wet) {
                if(npc.velocity.Y > 0f)
                    npc.velocity.Y *= 0.95f;

                npc.velocity.Y -= 0.5f;
                if(npc.velocity.Y < -4f)
                    npc.velocity.Y = -4f;

                npc.TargetClosest();
            }

            if(npc.ai[1] == 101f) {
                SoundEngine.PlaySound(SoundID.Item17, npc.position);
                npc.ai[1] = 0f;
            }

            if(Main.netMode != 1) {
                npc.ai[1] += (float)Main.rand.Next(5, 20) * 0.1f * npc.scale;
                if(npc.type == 176)
                    npc.ai[1] += (float)Main.rand.Next(5, 20) * 0.1f * npc.scale;

                if(Main.getGoodWorld)
                    npc.ai[1] += (float)Main.rand.Next(5, 20) * 0.1f * npc.scale;

                if(targetData.Type == NPCTargetType.Player) {
                    Player player = Main.player[npc.target];
                    if(player != null && player.stealth == 0f && player.itemAnimation == 0)
                        npc.ai[1] = 0f;
                }

                if(npc.ai[1] >= 130f) {
                    if(targetData.Type != 0 && Collision.CanHit(npc, targetData)) {
                        float num20 = 8f;
                        Vector2 vector4 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)(npc.height / 2));
                        float num21 = targetData.Center.X - vector4.X + (float)Main.rand.Next(-20, 21);
                        float num22 = targetData.Center.Y - vector4.Y + (float)Main.rand.Next(-20, 21);
                        if((num21 < 0f && npc.velocity.X < 0f) || (num21 > 0f && npc.velocity.X > 0f)) {
                            float num23 = (float)Math.Sqrt(num21 * num21 + num22 * num22);
                            num23 = num20 / num23;
                            num21 *= num23;
                            num22 *= num23;
                            int num24 = (int)(10f * npc.scale);
                            if(npc.type == 176)
                                num24 = (int)(30f * npc.scale);

                            int num25 = 55;
                            int num26 = Projectile.NewProjectile(npc.GetSource_FromAI(), vector4.X, vector4.Y, num21, num22, num25, num24, 0f, Main.myPlayer);
                            Main.projectile[num26].timeLeft = 300;
                            npc.ai[1] = 101f;
                            npc.netUpdate = true;
                        }
                        else {
                            npc.ai[1] = 0f;
                        }
                    }
                    else {
                        npc.ai[1] = 0f;
                    }
                }
            }
        }

        if(npc.type == 139 && tooFarToTarget) {
            if((npc.velocity.X > 0f && num4 > 0f) || (npc.velocity.X < 0f && num4 < 0f)) {
                int num27 = 12;
                if(NPC.IsMechQueenUp)
                    num27 = 5;

                if(Math.Abs(npc.velocity.X) < (float)num27)
                    npc.velocity.X *= 1.05f;
            }
            else {
                npc.velocity.X *= 0.9f;
            }
        }

        if(npc.type == 139 && NPC.IsMechQueenUp && npc.ai[2] == 0f) {
            Vector2 targetCenter = npc.GetTargetData().Center;
            Vector2 v2 = targetCenter - npc.Center;
            int num28 = 120;
            if(v2.Length() < num28)
                npc.Center = targetCenter - v2.SafeNormalize(Vector2.UnitY) * num28;
        }

        if(Main.netMode != 1) {
            if(Main.getGoodWorld && npc.type == 6 && NPC.AnyNPCs(13)) {
                if(npc.justHit)
                    npc.localAI[0] = 0f;

                npc.localAI[0] += 1f;
                if(npc.localAI[0] == 60f) {
                    if(targetData.Type != 0 && Collision.CanHit(npc, targetData))
                        NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.position.X + (float)(npc.width / 2) + npc.velocity.X), (int)(npc.position.Y + (float)(npc.height / 2) + npc.velocity.Y), 666);

                    npc.localAI[0] = 0f;
                }
            }

            if(npc.type == 94 && !targetPlayerDead) {
                if(npc.justHit)
                    npc.localAI[0] = 0f;

                npc.localAI[0] += 1f;
                if(npc.localAI[0] == 180f) {
                    if(targetData.Type != 0 && Collision.CanHit(npc, targetData))
                        NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.position.X + (float)(npc.width / 2) + npc.velocity.X), (int)(npc.position.Y + (float)(npc.height / 2) + npc.velocity.Y), 112);

                    npc.localAI[0] = 0f;
                }
            }

            if(npc.type == 619 && !targetPlayerDead) {
                if(npc.justHit)
                    npc.localAI[0] += 10f;

                npc.localAI[0] += 1f;
                if(npc.localAI[0] >= 120f) {
                    if(targetData.Type != 0 && Collision.CanHit(npc, targetData)) {
                        if((npc.Center - targetData.Center).Length() < 400f) {
                            Vector2 vector5 = npc.DirectionTo(new Vector2(targetData.Center.X, targetData.Position.Y));
                            npc.velocity = -vector5 * 5f;
                            npc.netUpdate = true;
                            npc.localAI[0] = 0f;
                            vector5 = npc.DirectionTo(new Vector2(targetData.Center.X + (float)Main.rand.Next(-100, 101), targetData.Position.Y + (float)Main.rand.Next(-100, 101)));
                            Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, vector5 * 15f, 811, 35, 1f, Main.myPlayer);
                        }
                        else {
                            npc.localAI[0] = 50f;
                        }
                    }
                    else {
                        npc.localAI[0] = 50f;
                    }
                }
            }
        }

        if((Main.IsItDay() && npc.type != 173 && npc.type != 619 && npc.type != 6 && npc.type != 23 && npc.type != 42 && npc.type != 94 && npc.type != 176 && npc.type != 205 && npc.type != 210 && npc.type != 211 && npc.type != 252 && (npc.type < 231 || npc.type > 235)) || targetPlayerDead) {
            npc.velocity.Y -= acc * 2f;
            npc.EncourageDespawn(10);
        }

        if(((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
            npc.netUpdate = true;
    }
    #endregion
    #endregion
}
