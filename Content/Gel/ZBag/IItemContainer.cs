using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.IO;
using System.Collections.Generic;
using Terraria;

namespace CalamityRangerExpansion.Content.Gel.ZBag
{
    public interface IItemContainer
    {
        string Name { get; }
        List<Item> ItemContainer { get; }
        bool AutoStorage { get; set; }
        bool AutoSort { get; set; }
        void SortContainer();
        void ItemIntoContainer(Item item);
        bool MeetEntryCriteria(Item item);
    }
}
