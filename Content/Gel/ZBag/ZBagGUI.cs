using CalamityRangerExpansion.Content.Gel.ZBag;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace CalamityRangerExpansion.Content.Gel.ZBag
{
    public class ZBagGUI : UIState
    {
        public static ZBagGUI Instance;

        private UIElement mainPanel;
        private UIPanel containerPanel;
        private IItemContainer activeContainer;

        public override void OnInitialize()
        {
            Instance = this;

            mainPanel = new UIElement();
            mainPanel.Width.Set(400f, 0f);
            mainPanel.Height.Set(300f, 0f);
            mainPanel.HAlign = 0.5f;
            mainPanel.VAlign = 0.5f;

            containerPanel = new UIPanel();
            containerPanel.Width.Set(350f, 0f);
            containerPanel.Height.Set(250f, 0f);
            containerPanel.HAlign = 0.5f;
            containerPanel.VAlign = 0.5f;

            mainPanel.Append(containerPanel);
            Append(mainPanel);
        }

        public void Open(IItemContainer container)
        {
            activeContainer = container;

            if (!Main.playerInventory)
            {
                Append(mainPanel);
            }

            Main.playerInventory = true;
        }

        public void Close()
        {
            activeContainer = null;

            if (mainPanel.Parent != null)
            {
                RemoveChild(mainPanel);
            }

            Main.playerInventory = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!Main.playerInventory)
            {
                Close();
            }
        }
    }

}
