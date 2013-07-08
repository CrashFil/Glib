﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Glib.XNA.SpriteLib
{
    /// <summary>
    /// A Sprite which accepts Text as input.
    /// </summary>
    public class TextBoxSprite : Sprite, ITimerSprite
    {
        private EventHandler movementInternal;

        /// <summary>
        /// An EventHandler called after the successful movement of this TextBoxSprite.
        /// </summary>
        /// <remarks>
        /// The superclass implementation is hidden to prevent removal of internally used event handlers.
        /// </remarks>
        public new event EventHandler Moved
        {
            add
            {
                base.Moved += value;
                base.Moved -= movementInternal;
                base.Moved += movementInternal;
            }

            remove
            {
                base.Moved -= value;
                base.Moved -= movementInternal;
                base.Moved += movementInternal;
            }
        }

        /// <summary>
        /// The color of the text.
        /// </summary>
        public Color TextColor
        {
            get
            {
                return _textView.Color;
            }
            set
            {
                _textView.Color = value;
            }
        }

        void TextBoxSprite_Moved(object source, EventArgs e)
        {
            _textView.Position = Position + new Vector2(1);
        }

        private TextSprite _textView;

        /// <summary>
        /// The displayed text of the underlying TextSprite.
        /// </summary>
        public string Text
        {
            get
            {
                return _textView.Text;
            }
            set
            {
                _textView.Text = value;
            }
        }

        /// <summary>
        /// The delay between allowed key presses.
        /// </summary>
        public TimeSpan KeyPressDelay = new TimeSpan(0, 0, 0, 0, 75);

        /// <summary>
        /// Whether or not to reset the Text of this TextBoxSprite when the text is submitted.
        /// </summary>
        public bool ResetOnSubmit = true;

        private TimeSpan _elapsedKeyPressTime = new TimeSpan();

        private int _firstVisible = 0;

        private string _realTxt = "";

        /// <summary>
        /// The full text typed into this TextBoxSprite.
        /// </summary>
        public string RealText
        {
            get
            {
                return _realTxt;
            }
            set
            {
                _realTxt = value;
            }
        }

        private bool _focused = true;

        /// <summary>
        /// Whether or not this is a password text field.
        /// </summary>
        public bool IsPassword
        {
            get;
            set;
        }

        /// <summary>
        /// Whether or not this TextBoxSprite is focused.
        /// </summary>
        public bool Focused
        {
            get
            {
                return _focused;
            }
            set
        {
            Focused = _focused;
        }
        }

        /// <summary>
        /// An event fired when text is submitted from this TextBoxSprite.
        /// </summary>
        /// <remarks>
        /// The text is not cleared until after the event is called, so you can use the Text property of the source to get the text submitted.
        /// </remarks>
        public event EventHandler TextSubmitted = null;

        /// <summary>
        /// Use the center as the origin.
        /// Always false for this Sprite implementation.
        /// </summary>
        public new bool UseCenterAsOrigin{
         get{
             base.UseCenterAsOrigin = false;
             return base.UseCenterAsOrigin;
         }
        }

        /// <summary>
        /// Create a new TextBoxSprite.
        /// </summary>
        public TextBoxSprite(Texture2D texture, Vector2 pos, SpriteBatch sb, SpriteFont font): this(texture, pos, Color.White, sb, new UpdateParamaters(), font)
        {
        }

        /// <summary>
        /// Create a new TextBoxSprite using a white background.
        /// </summary>
        /// <param name="pos">The position of the TextBoxSprite.</param>
        /// <param name="font">The SpriteFont to draw the text with.</param>
        /// <param name="sb">The SpriteBatch to draw to.</param>
        public TextBoxSprite(Vector2 pos, SpriteBatch sb, SpriteFont font)
            : this(new PlainTexture2D(sb.GraphicsDevice), pos, Color.White, sb, new UpdateParamaters(), font)
        {
        }



        /// <summary>
        /// Create a new TextBoxSprite.
        /// </summary>
        public TextBoxSprite(Texture2D texture, Vector2 pos, Color color, SpriteBatch sb, SpriteFont font)
            : this(texture, pos, color, sb, new UpdateParamaters(), font)
        {
        }

        /// <summary>
        /// Create a new TextBoxSprite.
        /// </summary>
        /// <param name="texture">The texture to show beneath the text (must be scalable).</param>
        /// <param name="color">The color to tint the texture under the text.</param>
        /// <param name="pos">The position of the TextBoxSprite.</param>
        /// <param name="font">The SpriteFont to draw the text with.</param>
        /// <param name="sb">The SpriteBatch to draw to.</param>
        /// <param name="up">The UpdateParameters to use when updating this Sprite.</param>
        public TextBoxSprite(Texture2D texture, Vector2 pos, Color color, SpriteBatch sb, UpdateParamaters up, SpriteFont font)
            : base(texture, pos, color, sb, up)
        {
            _textView = new TextSprite(SpriteBatch, font, "");
            _textView.Position = Position + new Vector2(1);
            Height = font.GetCharSize().Y + 2;
            Scale.X = 0;
            movementInternal = new EventHandler(this.TextBoxSprite_Moved);
            Moved += movementInternal;
            base.UseCenterAsOrigin = false;
        }

        /// <summary>
        /// Create a new TextBoxSprite.
        /// </summary>
        public TextBoxSprite(Texture2D texture, Vector2 pos, SpriteBatch sb, UpdateParamaters up, SpriteFont font)
            : this(texture, pos, Color.White, sb, up, font)
        {
        }

        private char _pwdChar = '*';

        /// <summary>
        /// The character to display for passwords.
        /// </summary>
        public char PasswordCharacter
        {
            get { return _pwdChar; }
            set { _pwdChar = value; }
        }
        

        /// <summary>
        /// Draws the sprite.
        /// Requires you to begin the SpriteBatch before you draw the sprite, and to end the SpriteBatch after you draw the sprite.
        /// </summary>
        public override void DrawNonAuto()
        {
            base.DrawNonAuto();
            _textView.Draw();
        }

        /// <summary>
        /// All keys ignored for input.
        /// </summary>
        public static readonly Keys[] IgnoredKeys = new Keys[] { Keys.CapsLock, Keys.Up, Keys.Down, Keys.Left, Keys.Right, Keys.LeftWindows, Keys.RightWindows, Keys.LeftControl, Keys.RightControl, Keys.RightAlt, Keys.LeftAlt };

        /// <summary>
        /// Update this TextBoxSprite.
        /// This includes processing keypresses and calling events.
        /// </summary>
        /// <remarks>
        /// Doesn't tick elaspedKeyPressTime.
        /// Called internally by Update(GameTime) after incrementing elaspedKeyPressTime.
        /// </remarks>
        public override void Update()
        {
            base.Update();
            if (_elapsedKeyPressTime > KeyPressDelay)
            {
                _elapsedKeyPressTime = new TimeSpan();
                if (Focused)
                {
                    Keys[] pressed = Keyboard.GetState().GetPressedKeys();
                    bool shift = pressed.Contains(Keys.LeftShift) || pressed.Contains(Keys.RightShift);
                    foreach (Keys k in pressed)
                    {
                        if (IgnoredKeys.Contains(k))
                        {
                            // Do nothing with ignored keys
                        }
                        else if (k == Keys.Enter)
                        {
                            if (TextSubmitted != null)
                            {
                                TextSubmitted(this, new EventArgs());
                            }
                            if (ResetOnSubmit)
                            {
                                _realTxt = "";
                            }
                        }
                        else if (k == Keys.Back)
                        {
                            _realTxt = _realTxt.Substring(0, _realTxt.Length <= 0 ? 0 : _realTxt.Length - 1);
                        }
                        else if (k == Keys.OemPeriod)
                        {
                            _realTxt += shift ? ">" : ".";
                        }
                        else if (k == Keys.OemMinus)
                        {
                            _realTxt += shift ? "_" : "-";
                        }
                        else if (k == Keys.D1 || k == Keys.NumPad1)
                        {
                            _realTxt += shift ? "!" : "1";
                        }
                        else if (k == Keys.D2 || k == Keys.NumPad2)
                        {
                            _realTxt += shift ? "@" : "2";
                        }
                        else if (k == Keys.D3 || k == Keys.NumPad3)
                        {
                            _realTxt += shift ? "#" : "3";
                        }
                        else if (k == Keys.D4 || k == Keys.NumPad4)
                        {
                            _realTxt += shift ? "$" : "4";
                        }
                        else if (k == Keys.D5 || k == Keys.NumPad5)
                        {
                            _realTxt += shift ? "%" : "5";
                        }
                        else if (k == Keys.D6 || k == Keys.NumPad6)
                        {
                            _realTxt += shift ? "^" : "6";
                        }
                        else if (k == Keys.D7 || k == Keys.NumPad7)
                        {
                            _realTxt += shift ? "&" : "7";
                        }
                        else if (k == Keys.D8 || k == Keys.NumPad8)
                        {
                            _realTxt += shift ? "*" : "8";

                        }
                        else if (k == Keys.D9 || k == Keys.NumPad9)
                        {
                            _realTxt += shift ? "(" : "9";

                        }
                        else if (k == Keys.D0 || k == Keys.NumPad0)
                        {
                            _realTxt += shift ? ")" : "0";

                        }
                        else if (k != Keys.LeftShift && k != Keys.RightShift)
                        {
                            string keyStr = k.ToString();
                            if (shift)
                            {
                                keyStr = keyStr.ToUpper();
                            }
                            else
                            {
                                keyStr = keyStr.ToLower();
                            }
                            _realTxt += k.ToString();
                        }
                    }
                }

                if (_realTxt.Length > 0)
                {
                    _firstVisible = 0;
                    string truePwdFieldTxt = null;
                    if (IsPassword)
                    {
                        truePwdFieldTxt = new string(_realTxt.ToCharArray());
                        _realTxt = new string(_pwdChar, _realTxt.Length);
                    }
                    if (_textView.Font.MeasureString(_realTxt).X > Width)
                    {
                        while (_textView.Font.MeasureString(_realTxt.Substring(_firstVisible)).X > Width)
                        {
                            _firstVisible++;
                        }
                    }
                    Text = _realTxt.Substring(_firstVisible);
                    if (IsPassword)
                    {
                        _realTxt = truePwdFieldTxt;
                    }
                }
                else
                {
                    Text = "";
                }
                base.UseCenterAsOrigin = false;
            }
        }

        /// <summary>
        /// Update this TextBoxSprite, ticking the keypress delay.
        /// </summary>
        /// <param name="gt">The current GameTime, passed to the Game.</param>
        public void Update(GameTime gt)
        {
            _elapsedKeyPressTime += gt.ElapsedGameTime;
            Update();
        }
    }
}