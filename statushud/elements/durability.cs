using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace StatusHud
{
    public class StatusHudDurabilityElement : StatusHudElement
    {
        public new const string name = "durability";
        protected const string textKey = "shud-durability";

        public override string ElementName => name;

        public bool active;

        protected StatusHudDurabilityRenderer renderer;

        public StatusHudDurabilityElement(StatusHudSystem system, StatusHudConfig config) : base(system, true)
        {
            renderer = new StatusHudDurabilityRenderer(this.system, this, config);
            this.system.capi.Event.RegisterRenderer(renderer, EnumRenderStage.Ortho);
        }

        public override StatusHudRenderer GetRenderer()
        {
            return renderer;
        }

        public virtual string GetTextKey()
        {
            return textKey;
        }

        public override void Tick()
        {
            CollectibleObject item = system.capi.World.Player.InventoryManager.ActiveHotbarSlot?.Itemstack?.Collectible;

            if (item != null && item.Durability != 0)
            {
                renderer.SetText(item.GetRemainingDurability(system.capi.World.Player.InventoryManager.ActiveHotbarSlot.Itemstack).ToString());
                active = true;
            }
            else
            {
                if (active)
                {
                    renderer.SetText("");
                }
                active = false;
            }
        }

        public override void Dispose()
        {
            renderer.Dispose();
            system.capi.Event.UnregisterRenderer(renderer, EnumRenderStage.Ortho);
        }
    }

    public class StatusHudDurabilityRenderer : StatusHudRenderer
    {
        protected StatusHudDurabilityElement element;

        protected StatusHudText text;

        public StatusHudDurabilityRenderer(StatusHudSystem system, StatusHudDurabilityElement element, StatusHudConfig config) : base(system)
        {
            this.element = element;

            text = new StatusHudText(this.system.capi, this.element.GetTextKey(), config);
        }

        public override void Reload()
        {
            text.ReloadText(pos);
        }

        public void SetText(string value)
        {
            text.Set(value);
        }

        protected override void Update()
        {
            base.Update();
            text.Pos(pos);
        }

        protected override void Render()
        {
            if (!element.active)
            {
                if (system.ShowHidden)
                {
                    RenderHidden(system.textures.texturesDict["durability"].TextureId);
                }
                return;
            }

            system.capi.Render.RenderTexture(system.textures.texturesDict["durability"].TextureId, x, y, w, h);
        }

        public override void Dispose()
        {
            base.Dispose();
            text.Dispose();
        }
    }
}