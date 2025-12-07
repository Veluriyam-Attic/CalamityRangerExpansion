//using CalamityMod.Projectiles.BaseProjectiles;
//using Microsoft.Xna.Framework;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Terraria.Audio;
//using Terraria.ID;
//using Terraria;
//using CalamityMod;
//using CalamityMod.Items.Weapons.Ranged;
//using Terraria.ModLoader;

//namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.SG225IE
//{
//    internal class SG225IEHoldOut : BaseGunHoldoutProjectile
//    {
//        public override string Texture => "CalamityRangerExpansion/Content/DeveloperItems/Weapon/SG225IE/SG225IE";
//        public override int AssociatedItemID => ModContent.ItemType<SG225IE>();

//        public override float MaxOffsetLengthFromArm => 36f;
//        public override float RecoilResolveSpeed => 0.15f;
//        public override float OffsetXUpwards => -8f;
//        public override float OffsetXDownwards => 2f;
//        public override float BaseOffsetY => -10f;
//        public override float OffsetYUpwards => 12f;
//        public override float OffsetYDownwards => 6f;

//        public ref float shootTimer => ref Projectile.ai[0];
//        public const int ShootInterval = 22;

//        public override void HoldoutAI()
//        {
//            Vector2 muzzleDirection = Projectile.velocity.SafeNormalize(Vector2.UnitX);
//            shootTimer++;

//            if (shootTimer >= ShootInterval)
//            {
//                shootTimer = 0;

//                if (Owner.PickAmmo(Owner.ActiveItem(), out int pickedProjType, out float shootSpeed, out int damage, out float knockback, out int ammoItemType))
//                {
//                    Vector2 muzzle = GunTipPosition;
//                    int projectileCount = Main.rand.Next(8, 13); // 8~12发散射

//                    for (int i = 0; i < projectileCount; i++)
//                    {
//                        float spread = MathHelper.ToRadians(Main.rand.NextFloat(-10f, 10f));
//                        float speedMod = Main.rand.NextFloat(0.85f, 1.15f);
//                        float damageMod = Main.rand.NextFloat(0.75f, 1.1f);
//                        Vector2 perturbedVelocity = muzzleDirection.RotatedBy(spread) * shootSpeed * speedMod;

//                        int projToShoot = pickedProjType;

//                        // 🔥如果使用的是普通火枪子弹（即物品是 MusketBall），则转为我们自定义火弹
//                        if (ammoItemType == ItemID.MusketBall)
//                            projToShoot = ModContent.ProjectileType<SG225IEProj>();

//                        Projectile.NewProjectile(
//                            Projectile.GetSource_FromThis(),
//                            muzzle,
//                            perturbedVelocity,
//                            projToShoot,
//                            (int)(damage * damageMod),
//                            knockback,
//                            Projectile.owner
//                        );
//                    }

//                    // 💥后坐力
//                    Owner.velocity += -muzzleDirection * 2f;

//                    // 📸震动效果
//                    if (Main.myPlayer == Owner.whoAmI)
//                    {
//                        float shakePower = 2.5f;
//                        float distFactor = Utils.GetLerpValue(1000f, 0f, Projectile.Distance(Main.LocalPlayer.Center), true);
//                        Main.LocalPlayer.Calamity().GeneralScreenShakePower = Math.Max(Main.LocalPlayer.Calamity().GeneralScreenShakePower, shakePower * distFactor);
//                    }

//                    // 🔊音效
//                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/FlakKrakenShoot") with { Pitch = 0.5f, Volume = 0.6f }, muzzle);
//                }
//            }
//        }














//    }
//}