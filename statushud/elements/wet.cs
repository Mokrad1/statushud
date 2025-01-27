using System;
using Vintagestory.API.Client;

namespace StatusHud
{
    public class StatusHudWetElement : StatusHudElement
    {
        public new const string name = "wet";
        protected const string textKey = "shud-wet";

        public override string ElementName => name;

        public bool active;

        protected StatusHudWetRenderer renderer;

        public StatusHudWetElement(StatusHudSystem system, StatusHudConfig config) : base(system)
        {
            renderer = new StatusHudWetRenderer(system, this, config);
            this.system.capi.Event.RegisterRenderer(renderer, EnumRenderStage.Ortho);

            active = false;
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
            float wetness = system.capi.World.Player.Entity.WatchedAttributes.GetFloat("wetness");

            if (wetness > 0)
            {
                renderer.SetText((int)Math.Round(wetness * 100f, 0) + "%");

                active = true;
            }
            else
            {
                if (active)
                {
                    // Only set text once.
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

    public class StatusHudWetRenderer : StatusHudRenderer
    {
        protected StatusHudWetElement element;

        protected StatusHudText text;

        public StatusHudWetRenderer(StatusHudSystem system, StatusHudWetElement element, StatusHudConfig config) : base(system)
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
                    this.RenderHidden(system.textures.texturesDict["wet"].TextureId);
                }
                return;
            }

            system.capi.Render.RenderTexture(system.textures.texturesDict["wet"].TextureId, x, y, w, h);
        }

        public override void Dispose()
        {
            base.Dispose();
            text.Dispose();
        }
    }
}