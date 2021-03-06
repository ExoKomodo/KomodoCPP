using System.Collections.Generic;
using Komodo.Lib.Math;

using PlayerIndex = Microsoft.Xna.Framework.PlayerIndex;

using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using GamePad = Microsoft.Xna.Framework.Input.GamePad;
using GamePadState = Microsoft.Xna.Framework.Input.GamePadState;
using Keyboard = Microsoft.Xna.Framework.Input.Keyboard;
using KeyboardState = Microsoft.Xna.Framework.Input.KeyboardState;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using Mouse = Microsoft.Xna.Framework.Input.Mouse;
using MouseState = Microsoft.Xna.Framework.Input.MouseState;

namespace Komodo.Core.Engine.Input
{
    /// <summary>
    /// Static class for managing <see cref="Komodo.Core.Engine.Input.Inputs"/>, <see cref="Komodo.Core.Engine.Input.InputInfo"/>, and the state of all input devices.
    /// </summary>
    public static class InputManager
    {
        #region Public

        #region Static Methods
        /// <summary>
        /// Adds an input mapping to the specified player.
        /// </summary>
        /// <param name="action">Identifier for the specific input action.</param>
        /// <param name="input">Identifier for the device input to be associated with the action.</param>
        /// <param name="playerIndex">Identifier for the player to map input for.</param>
        public static void AddInputMapping(string action, Inputs input, int playerIndex = 0)
        {
            if (!IsValidPlayerIndex(playerIndex))
            {
                return;
            }
            var inputMap = _inputMaps[playerIndex];
            if (!inputMap.ContainsKey(action))
            {
                inputMap[action] = new List<Inputs>();
            }
            var inputList = inputMap[action];
            if (!inputList.Contains(input))
            {
                inputList.Add(input);
            }
        }

        /// <summary>
        /// Gets <see cref="Komodo.Core.Engine.Input.InputInfo"/> for the given action and player.
        /// </summary>
        /// <param name="action">Action to query for <see cref="Komodo.Core.Engine.Input.InputInfo"/>.</param>
        /// <param name="playerIndex">Action to query for <see cref="Komodo.Core.Engine.Input.InputInfo"/>.</param>
        /// <returns><see cref="Komodo.Core.Engine.Input.InputInfo"/> for the given action and player.</returns>
        public static List<InputInfo> GetAction(string action, int playerIndex = 0, bool usePrevious = false)
        {
            var result = new List<InputInfo>();
            if (IsValidPlayerIndex(playerIndex))
            {
                var inputMap = _inputMaps[playerIndex];
                if (inputMap.ContainsKey(action))
                {
                    var inputList = inputMap[action];
                    foreach (var input in inputList)
                    {
                        var inputInfo = GetInputInfo(input, playerIndex, usePrevious);
                        result.Add(inputInfo);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets mouse position.
        /// </summary>
        /// <param name="isCurrent">Whether or not to return the current or previous mouse position.</param>
        /// <returns></returns>
        public static Vector2 GetMousePosition(bool isCurrent = true)
        {
            var mouseState = isCurrent ? Mouse.GetState() : _previousMouseState;
            var position = mouseState.Position;
            return new Vector2(position.X, position.Y);
        }

        /// <summary>
        /// Whether or not the input has been held on current frame and frame before.
        /// </summary>
        /// <param name="action">Action to query for <see cref="Komodo.Core.Engine.Input.InputInfo"/>.</param>
        /// <param name="playerIndex">Action to query for <see cref="Komodo.Core.Engine.Input.InputInfo"/>.</param>
        public static bool IsHeld(string action, int playerIndex = 0)
        {
            var previousInputs = GetAction(action, playerIndex, usePrevious: true);
            var currentInputs = GetAction(action, playerIndex, usePrevious: false);

            for (int i = 0; i < currentInputs.Count; i++)
            {
                var previousInput = previousInputs[i];
                var currentInput = currentInputs[i];
                if (previousInput.State == InputState.Down && currentInput.State == InputState.Down)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Whether or not the input was just pressed this frame.
        /// </summary>
        /// <param name="action">Action to query for <see cref="Komodo.Core.Engine.Input.InputInfo"/>.</param>
        /// <param name="playerIndex">Action to query for <see cref="Komodo.Core.Engine.Input.InputInfo"/>.</param>
        public static bool IsJustPressed(string action, int playerIndex = 0)
        {
            var previousInputs = GetAction(action, playerIndex, usePrevious: true);
            var currentInputs = GetAction(action, playerIndex, usePrevious: false);

            for (int i = 0; i < currentInputs.Count; i++)
            {
                var previousInput = previousInputs[i];
                var currentInput = currentInputs[i];
                if (previousInput.State == InputState.Up && currentInput.State == InputState.Down)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Whether or not the input was just released this frame.
        /// </summary>
        /// <param name="action">Action to query for <see cref="Komodo.Core.Engine.Input.InputInfo"/>.</param>
        /// <param name="playerIndex">Action to query for <see cref="Komodo.Core.Engine.Input.InputInfo"/>.</param>
        public static bool IsJustReleased(string action, int playerIndex = 0)
        {
            var previousInputs = GetAction(action, playerIndex, usePrevious: true);
            var currentInputs = GetAction(action, playerIndex, usePrevious: false);

            for (int i = 0; i < currentInputs.Count; i++)
            {
                var previousInput = previousInputs[i];
                var currentInput = currentInputs[i];
                if (previousInput.State == InputState.Down && currentInput.State == InputState.Up)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Whether or not the input is being pressed, including the first frame.
        /// </summary>
        /// <param name="action">Action to query for <see cref="Komodo.Core.Engine.Input.InputInfo"/>.</param>
        /// <param name="playerIndex">Action to query for <see cref="Komodo.Core.Engine.Input.InputInfo"/>.</param>
        public static bool IsPressed(string action, int playerIndex = 0)
        {
            var currentInputs = GetAction(action, playerIndex, usePrevious: false);

            for (int i = 0; i < currentInputs.Count; i++)
            {
                var currentInput = currentInputs[i];
                if (currentInput.State == InputState.Down)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Validates given player index between supported player count.
        /// </summary>
        /// <param name="playerIndex">Player index to validate.</param>
        /// <returns>Whether or no the player index is valid.</returns>
        public static bool IsValidPlayerIndex(int playerIndex)
        {
            return playerIndex >= 0 && playerIndex <= 3;
        }

        /// <summary>
        /// Removes an input from the registered action.
        /// </summary>
        /// <param name="action">Action to remove the <see cref="Komodo.Core.Engine.Input.Inputs"/> from.</param>
        /// <param name="input"><see cref="Komodo.Core.Engine.Input.Inputs"/> to remove from the action.</param>
        /// <param name="playerIndex">Player to remove the input mapping from.</param>
        /// <returns>Whether or not the input mapping was removed. Will return false if input was not mapped to specified action and player.</returns>
        public static bool RemoveInputMapping(string action, Inputs input, int playerIndex = 0)
        {
            if (!IsValidPlayerIndex(playerIndex))
            {
                return false;
            }
            var inputMap = _inputMaps[playerIndex];
            if (inputMap.ContainsKey(action))
            {
                var inputList = inputMap[action];
                return inputList.Remove(input);
            }
            return false;
        }

        /// <summary>
        /// Sets the mouse position to the given coordinates.
        /// </summary>
        /// <remarks>
        /// Will floor each coordinate to an integer.
        /// </remarks>
        /// <param name="newMousePosition">New position for the mouse.</param>
        public static void SetMousePosition(Vector2 newMousePosition)
        {
            SetMousePosition((int) newMousePosition.X, (int) newMousePosition.Y);
        }

        /// <summary>
        /// Sets the mouse position to the given coordinates.
        /// </summary>
        /// <param name="x">New X position for the mouse.</param>
        /// <param name="y">New Y position for the mouse.</param>
        public static void SetMousePosition(int x, int y)
        {
            Mouse.SetPosition(x, y);
        }
        #endregion

        #endregion

        #region Internal

        #region Static Methods
        /// <summary>
        /// Updates current and previous input states across all input devices.
        /// </summary>
        internal static void Update()
        {
            _previousMouseState = _mouseState;
            _previousKeyboardState = _keyboardState;
            _keyboardState = Keyboard.GetState();
            _mouseState = Mouse.GetState();

            for (int i = 0; i < 4; i++)
            {
                var playerIndex = GetPlayerIndex(i);

                _previousGamePadStates[i] = _gamePadStates[i];
                _gamePadStates[i] = GamePad.GetState(playerIndex);
            }
        }
        #endregion

        #endregion

        #region Private

        #region Static Members
        /// <summary>
        /// Current state of all 4 supported gamepads.
        /// </summary>
        private static GamePadState[] _gamePadStates { get; set; }

        /// <summary>
        /// State of input maps for all 4 supported players.
        /// </summary>
        private static Dictionary<string, List<Inputs>>[] _inputMaps { get; }

        /// <summary>
        /// Current state of 1 supported keyboard.
        /// </summary>
        private static KeyboardState _keyboardState { get; set; }

        /// <summary>
        /// Current state of 1 supported mouse.
        /// </summary>
        private static MouseState _mouseState { get; set; }

        /// <summary>
        /// Previous state of all 4 supported gamepads.
        /// </summary>
        private static GamePadState[] _previousGamePadStates { get; set; }

        /// <summary>
        /// Previous state of 1 supported keyboard.
        /// </summary>
        private static KeyboardState _previousKeyboardState { get; set; }

        /// <summary>
        /// Previous state of 1 supported mouse.
        /// </summary>
        private static MouseState _previousMouseState { get; set; }
        #endregion

        #region Static Methods
        /// <summary>
        /// Retrieves <see cref="Komodo.Core.Engine.Input.InputInfo"/> for the given <see cref="Komodo.Core.Engine.Input.Inputs"/>.
        /// </summary>
        /// <param name="input">Identifier for the device input to query.</param>
        /// <param name="playerIndex">Identifier for the player to query.</param>
        /// <returns><see cref="Komodo.Core.Engine.Input.InputInfo"/> for the given <see cref="Komodo.Core.Engine.Input.Inputs"/> and player.</returns>
        private static InputInfo GetInputInfo(Inputs input, int playerIndex = 0, bool usePrevious = false)
        {
            if (!IsValidPlayerIndex(playerIndex))
            {
                return new InputInfo();
            }
            var gamePadStates = _gamePadStates;
            var keyboardState = _keyboardState;
            var mouseState = _mouseState;
            if (usePrevious)
            {
                gamePadStates = _previousGamePadStates;
                keyboardState = _previousKeyboardState;
                mouseState = _previousMouseState;
            }
            var komodoInputState = InputState.Undefined;
            switch(input)
            {
                case Inputs.MouseLeftClick:
                    var buttonState = mouseState.LeftButton;
                    komodoInputState = buttonState == ButtonState.Pressed ? InputState.Down : InputState.Up;
                    return new InputInfo(input, komodoInputState, Vector2.Zero, 0f);
                case Inputs.MouseMiddleClick:
                    buttonState = mouseState.MiddleButton;
                    komodoInputState = buttonState == ButtonState.Pressed ? InputState.Down : InputState.Up;
                    return new InputInfo(input, komodoInputState, Vector2.Zero, 0f);
                case Inputs.MouseRightClick:
                    buttonState = mouseState.RightButton;
                    komodoInputState = buttonState == ButtonState.Pressed ? InputState.Down : InputState.Up;
                    return new InputInfo(input, komodoInputState, Vector2.Zero, 0f);
                case Inputs.ThumbstickLeft:
                    var thumbstickPosition = new Vector2(gamePadStates[playerIndex].ThumbSticks.Left);
                    var strength = thumbstickPosition.Length();
                    var direction = thumbstickPosition != Vector2.Zero ? Vector2.Normalize(thumbstickPosition) : Vector2.Zero;
                    komodoInputState = strength == 0f ? InputState.Up : InputState.Down;
                    return new InputInfo(input, komodoInputState, direction, strength);
                case Inputs.ThumbstickRight:
                    thumbstickPosition = new Vector2(gamePadStates[playerIndex].ThumbSticks.Right);
                    strength = thumbstickPosition.Length();
                    direction = thumbstickPosition != Vector2.Zero ? Vector2.Normalize(thumbstickPosition) : Vector2.Zero;
                    komodoInputState = strength == 0f ? InputState.Down : InputState.Up;
                    return new InputInfo(input, komodoInputState, direction, strength);
                default:
                    var monogameButtonInput = InputMapper.ToMonoGameButton(input);
                    if (monogameButtonInput.HasValue)
                    {
                        var gamePadState = gamePadStates[playerIndex];
                        bool isDown = gamePadState.IsButtonDown(monogameButtonInput.Value);
                        komodoInputState = isDown ? InputState.Down : InputState.Up;
                    }
                    
                    var monogameKeyInput = InputMapper.ToMonoGameKey(input);
                    if (monogameKeyInput != Keys.None)
                    {
                        bool isDown = keyboardState.IsKeyDown(monogameKeyInput);
                        komodoInputState = isDown ? InputState.Down : InputState.Up;
                    }
                    
                    return new InputInfo(input, komodoInputState, Vector2.Zero, 0f);
            }
        }

        /// <summary>
        /// Converts integer to a <see cref="Microsoft.Xna.Framework.PlayerIndex"/>.
        /// </summary>
        /// <param name="playerIndex">Player identifier.</param>
        /// <returns>Converted player identifier.</returns>
        private static PlayerIndex GetPlayerIndex(int playerIndex)
        {
            return playerIndex switch
            {
                0 => PlayerIndex.One,
                1 => PlayerIndex.Two,
                2 => PlayerIndex.Three,
                3 => PlayerIndex.Four,
                _ => PlayerIndex.One,
            };
        }
        #endregion

        #endregion

        #region Static Constructor
        /// <summary>
        /// Sets up the input maps and input device states.
        /// </summary>
        static InputManager()
        {
            _inputMaps = new Dictionary<string, List<Inputs>>[4];
            for (int i = 0; i < _inputMaps.Length; i++)
            {
                _inputMaps[i] = new Dictionary<string, List<Inputs>>();
            }
            _gamePadStates = new GamePadState[4];
            _previousGamePadStates = new GamePadState[4];
            Update();
        }
        #endregion
    }
}