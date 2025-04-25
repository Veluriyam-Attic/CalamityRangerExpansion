//using Microsoft.Build.ObjectModelRemoting;
//using Microsoft.Build.Utilities;
//using Microsoft.Xna.Framework;
//using System;
//using Terraria;

//namespace CalamityRangerExpansion.Content.Ammunition
//{
//    /// <summary>
//    /// 本文件别英文化
//    /// </summary>
//    public class 弹幕测试_Proje : 子弹弹幕
//    {
//        public override void SetDefaults()
//        {
//            Projectile.penetrate = -1;
//            Projectile.maxPenetrate = int.MaxValue;
//            Projectile.MaxUpdates = 2;
//            base.SetDefaults();
//        }
//        Vector2 Cent = Vector2.Zero;
//        Player player = null;
//        Vector2 projev2 = Vector2.Zero;
//        Vector2 npcv2 = Vector2.Zero;
//        Vector2 newV2 = Vector2.Zero;
//        NPC npc = null;
//        int num = 0;
//        int num2 = 0;
//        int time = 0;
//        public override void AI()
//        {
//            #region 相关判断
//            /*
//             * X与Y轴的距离计算公式 -> sqrt((x1-x2)平方)
//             * X更大 就是在右边 反之 左边
//             * Y更大 就是在下边 反之 上边
//             * 
//             * npc.friend -> 是否友好
//             * npc.active -> 是否活着
//             */
//            #endregion

//            #region 给出两点 角度计算
//            //Vector2 proje = Projectile.Center;
//            //Vector2 npc_v2 = npc.Center;
//            //float x = Math.Abs(proje.X - npc_v2.X);
//            //float y = Math.Abs(proje.Y - npc_v2.Y);
//            //double z = Math.Sqrt(x * x + y * y);
//            //Math.Round(Math.Asin(y / z) / Math.PI * 180);
//            #endregion

//            #region 跟踪
//            /*
//            if (num2 == 0)
//            {
//                player = Main.player[Projectile.owner];
//                Cent = Vector2.Normalize(Main.MouseWorld - player.Center) * 10;
//                num2++;
//            }
//            //if (time > 60)
//            //{
//                foreach (NPC npc in Main.npc)
//                {
//                Vector2 proje = Projectile.Center;
//                Vector2 npc_v2 = npc.Center;
//                float x = Math.Abs(proje.X - npc_v2.X);
//                float y = Math.Abs(proje.Y - npc_v2.Y);
//                double z = Math.Sqrt(x * x + y * y);
                
//                    if (!npc.friendly && npc.active)
//                    {
//                        Main.NewText(Math.Round(Math.Asin(y / z) / Math.PI * 180));
//                    }
//                    //npc.active = 活着
//                    if (!npc.friendly && npc.active &&
//                        Vector2.Distance(Projectile.Center, npc.Center) < 100 &&
//                        Math.Round(Math.Asin(y / z) / Math.PI * 180) > 30 &&
//                        Math.Round(Math.Asin(y / z) / Math.PI * 180) < 40)
//                    {
                        
//                        newV2 = Vector2.Normalize(npc.Center - Projectile.Center) * 10f;
//                        this.npc = npc;
//                        num++;
//                        break;
//                    }
//                }
//            //}
//            if (npc != null)
//            {
//                Projectile.velocity.X += (Projectile.velocity.X < newV2.X ? 1 : -1) * 2f;
//                Projectile.velocity.Y += (Projectile.velocity.Y < newV2.Y ? 1 : -1) * 1.1f;
//            }
//            else
//            {
//                Projectile.velocity = Cent;
//            }
//            Projectile.rotation = (float)Projectile.velocity.ToRotation() + (float)Math.PI/2;
//            time++;
//            if (time > 300) Projectile.Kill();
//            */
//            #endregion

//            #region 吸附
//            //泰拉坐标系 Y轴是反的 向下是正 向上是负
//            bool 右边 = false;
//            bool 左边 = false;
//            bool 上边 = false;
//            bool 下边 = false;
//            foreach (NPC npc in Main.npc)
//            {
//                if (npc.active)
//                {
//                    Vector2 npcv2 = npc.Center;Vector2 projve = Projectile.Center;
//                    //float xj = Vector2.Distance(npcv2,projve);
                    
//                    float xj = projve.X - npcv2.X;
//                    xj = (float)Math.Sqrt(xj * xj);
//                    float Y = (float)Math.Sqrt((projve.Y - npcv2.Y) * (projve.Y - npcv2.Y));
//                    if (projve.X > npcv2.X){Main.NewText(xj + "->" + "在弹幕右边");右边 = true;左边 = false; }
//                    else { Main.NewText(xj + "->" + "在弹幕左边"); 左边 = true;右边 = false; }

//                    if (projve.Y < npcv2.Y){Main.NewText(Y + "->" + "在弹幕下面");上边 = false; 下边 = true; }
//                    else { Main.NewText(Y + "->" + "在弹幕上面"); 上边 = true;下边 = false; }
//                    npc.velocity = Vector2.Zero;
//                    if (左边)npc.velocity.X -= 60F;
//                    if (右边) npc.velocity.X += 60F;
//                    if (上边) npc.velocity.Y += 60F;
//                    if (下边) npc.velocity.Y -= 60F;
//                }
//            }
//            #endregion
//            base.AI();
//        }
//    }
//}
