using Komodo.Core.ECS.Components;
using System;

using Color = Microsoft.Xna.Framework.Color;
using GameTime = Microsoft.Xna.Framework.GameTime;

namespace Common.Behaviors
{
    public class FPSCounterBehavior : BehaviorComponent
    {
        #region Constructors
        #endregion Constructors

        #region Members

        #region Public Members
        public TextComponent CounterText { get; protected set;  }
        #endregion Public Members

        #region Protected Members
        #endregion Protected Members

        #region Private Members
        #endregion Private Members

        #endregion Members

        #region Member Methods

        #region Public Member Methods
        public override void Initialize()
        {
            base.Initialize();

            CounterText = new TextComponent("fonts/font", Color.Black, Game.DefaultSpriteShader, "")
            {
                Position = Komodo.Lib.Math.Vector3.Zero
            };
            Parent.AddComponent(CounterText);
        }
        public override void Update(GameTime gameTime)
        {
            CounterText.Text = $"{Math.Round(Game.FramesPerSecond)} FPS";
        }
        #endregion Public Member Methods

        #region Protected Member Methods
        #endregion Protected Member Methods

        #region Private Member Methods
        #endregion Private Member Methods

        #endregion Member Methods
    }
}