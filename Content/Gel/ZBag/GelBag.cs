//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Terraria;
//using Terraria.ID;
//using Terraria.ModLoader;
//using Terraria.UI;
//using CalamityRangerExpansion.Content.Gel.ZBag; // 引入同目录的其他文件

//namespace CalamityRangerExpansion.Content.Gel.ZBag
//{
//    public class GelBag : ModItem, IItemContainer
//    {
//        public string ContainerName => "ZBag";

//        // 容器存储物品的列表
//        public List<Item> ItemContainer { get; private set; } = new List<Item>();

//        // 是否开启自动存储
//        public bool AutoStorage { get; set; }
//        // 是否开启自动排序
//        public bool AutoSort { get; set; }

//        public override void SetStaticDefaults()
//        {
//        }

//        public override void SetDefaults()
//        {
//            Item.width = 34; // 宽度
//            Item.height = 34; // 高度
//            Item.rare = ItemRarityID.Purple; // 稀有度
//            Item.maxStack = 9999; // 不可堆叠
//            Item.consumable = false;
//            Item.value = Item.buyPrice(gold: 10); // 设置价值
//        }

//        // 判断是否可以右键打开
//        public override bool CanRightClick() => true;

//        // 右键点击逻辑
//        public override void RightClick(Player player)
//        {
//            // 如果 UI 未正确初始化，提示错误日志
//            if (ZBagGUI.Instance == null)
//            {
//                Main.NewText("UI 未初始化！", Microsoft.Xna.Framework.Color.Red);
//                return;
//            }

//            // 打开 UI
//            ZBagGUI.Instance.Open(this);

//            // 明确不消费物品
//            player.itemAnimation = 0; // 防止使用动画
//        }


//        // 判断物品是否符合存入条件
//        public bool MeetEntryCriteria(Item item)
//        {
//            // 示例条件：只能存入凝胶类物品
//            return item.type == ItemID.Gel;
//        }

//        // 将物品存入容器
//        public void ItemIntoContainer(Item item)
//        {
//            // 如果符合条件，将物品加入容器
//            if (MeetEntryCriteria(item))
//            {
//                ItemContainer.Add(item.Clone());
//                item.TurnToAir();
//            }
//        }

//        // 排序功能
//        public void SortContainer()
//        {
//            // 按照物品类型和堆叠数量排序
//            ItemContainer.Sort((a, b) => a.type.CompareTo(b.type) + a.stack.CompareTo(b.stack) * 10);
//        }
//    }
//}
