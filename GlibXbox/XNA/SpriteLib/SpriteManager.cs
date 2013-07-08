﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Glib.XNA.SpriteLib
{
    /// <summary>
    /// Manages multiple <seealso cref="Sprite"/> objects on the same SpriteBatch.
    /// </summary>
    public class SpriteManager : ISprite, ISpriteBatchManagerSprite, ITimerSprite
    {
        /// <summary>
        /// The list of sprites managed by this SpriteManager.
        /// </summary>
        public List<Sprite> Sprites = new List<Sprite>();
        private SpriteBatch _sb = null;

        /// <summary>
        /// Gets the SpriteBatch drawn to.
        /// </summary>
        public SpriteBatch SpriteBatch
        {
            get
            {
                return _sb;
            }
        }

        private int _i = 0;
        
        /// <summary>
        /// Get or set the sprite with the specified index in the Sprites list.
        /// </summary>
        /// <remarks>
        /// Returns null if the index is out of bounds of the array.
        /// </remarks>
        /// <param name="index">The index in the Sprites list.</param>
        /// <returns>The sprite with the specified index in the Sprites list.</returns>
        public Sprite this[int index]
        {
            get
            {
                if (index < Sprites.Count)
                {
                    return Sprites[index];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                Sprites[index] = value;
            }
        }

        /// <summary>
        /// Add a new sprite with the specified position and texture.
        /// </summary>
        /// <param name="position">The position of the new Sprite.</param>
        /// <param name="texture">The texture of the new sprite.</param>
        public void AddNewSprite(Microsoft.Xna.Framework.Vector2 position, Texture2D texture)
        {
            Add(new Sprite(texture, position, _sb));
        }

        /// <summary>
        /// Remove a sprite from this SpriteManager.
        /// Safe to call during Update() or Draw() (or from their corresponding events).
        /// </summary>
        /// <param name="spr">The sprite to remove</param>
        internal void RemoveSelf(Sprite spr)
        {
            Remove(spr);
            _i--;
        }

        /// <summary>
        /// Add a sprite to this SpriteManager.
        /// </summary>
        /// <param name="spr">The sprite to add</param>
        public void Add(Sprite spr)
        {
            spr.SpriteManager = this;
            Sprites.Add(spr);
        }

        /// <summary>
        /// Remove a given sprite, that is NOT the sprite being updated.
        /// </summary>
        /// <param name="spr">The sprite to remove</param>
        public void Remove(Sprite spr)
        {
            Sprites.Remove(spr);
        }

        /// <summary>
        /// Construct a new SpriteManager.
        /// </summary>
        /// <param name="sb">The SpriteBatch to use.</param>
        /// <param name="sprites">The sprites to add to the SpriteManager.</param>
        public SpriteManager(SpriteBatch sb, params Sprite[] sprites)
        {
            _sb = sb;
            foreach (Sprite s in sprites)
            {
                s.SpriteBatch = sb;
                s.SpriteManager = this;
                Sprites.Add(s);
            }
        }

        /// <summary>
        /// Draw all sprites managed by this SpriteManager.
        /// </summary>
        public void Draw()
        {
            _sb.Begin();
            DrawNonAuto();
            _sb.End();
        }

        /// <summary>
        /// Draw all sprites managed by this SpriteManager, without opening or closing the SpriteBatch.
        /// </summary>
        public void DrawNonAuto()
        {
            for (_i = 0; _i < Sprites.Count; _i++)
            {
                Sprites[_i].DrawNonAuto();
            }
        }

        /// <summary>
        /// Update all sprites managed by this SpriteManager.
        /// </summary>
        /// <remarks>
        /// Does not call Update(GameTime) on subclasses of Sprite implementing ITimerSprite.
        /// </remarks>
        public void Update()
        {
            for (_i = 0; _i < Sprites.Count; _i++ )
            {
                
                    this[_i].Update();
            }
        }

        /// <summary>
        /// Update all Sprites managed by this SpriteManager, calling Update(GameTime) on ITimerSprites where neccesary.
        /// THIS WILL NOT CALL KEYBOARD OR MOSUE EVENTS IF gameTime.IsRunningSlowly IS TRUE.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            for (_i = 0; _i < Sprites.Count; _i++)
            {
                if (this[_i].GetType().GetInterfaces().Contains(typeof(ITimerSprite)))
                {
                    ((ITimerSprite)this[_i]).Update(gameTime);
                }
                else
                {
                    this[_i].Update();
                }
            }
        }
    }
}