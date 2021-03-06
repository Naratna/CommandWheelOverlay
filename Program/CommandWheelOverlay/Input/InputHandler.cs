﻿using CommandWheelOverlay.Settings;
using CommandWheelOverlay.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CommandWheelOverlay.Input
{
    public class InputHandler : IInputHandler
    {
        public IOverlayView View { get; set; }
        public IList<int> ShowHotkey { get; set; }
        public IList<int> MoveLeftHotkey { get; set; }
        public IList<int> MoveRightHotkey { get; set; }
        public bool PauseOutput { get; set; }

        private bool showPressed = false;
        private bool moveLeftPressed = false;
        private bool moveRightPressed = false;

        private HashSet<int> pressedKeys = new HashSet<int>();
        private static readonly MouseButton[] downs = { MouseButton.Button4Down, MouseButton.Button5Down, MouseButton.MiddleButtonDown, MouseButton.LeftButtonDown, MouseButton.RightButtonDown };
        private static readonly MouseButton[] ups = { MouseButton.Button4Up, MouseButton.Button5Up, MouseButton.MiddleButtonUp, MouseButton.LeftButtonUp, MouseButton.RightButtonUp };

        private bool recording = false;
        private Dictionary<int, int> recorded;
        private CancellationToken cancellationToken;
        private CancellationTokenRegistration registration;
        private Action<int> keyRecorded;
        private Action<IList<int>> finishedRecording;

        public void Handle(MouseInput input)
        {
            if (recording)
            {
                Record(input);
                return;
            }
            if (PauseOutput) return;
            if ((input.LastX != 0 || input.LastY != 0) && showPressed)
                View.SendMouseMovement(new[] {input.LastX, input.LastY });
            Check(input.Buttons, downs, pressedKeys.Add);
            Check(input.Buttons, ups, pressedKeys.Remove);
            UpdatePressed();
        }

        private void Check(MouseButton input, MouseButton[] expected, Func<int, bool> function)
        {
            foreach (MouseButton item in expected)
            {
                if ((input & item) > 0)
                {
                    if (expected == ups)
                    {
                        input = (MouseButton)((int)input / 2);
                    }
                    function(-(int)input);
                }
            }
        }

        public void Handle(KeyboardInput input)
        {
            if (recording)
            {
                Record(input);
                return;
            }
            if (PauseOutput) return;
            if ((input.Flags & KeyboardFlags.Up) > 0)
            {
                pressedKeys.Remove(input.Key);
            }
            else
            {
                pressedKeys.Add(input.Key);
            }
            UpdatePressed();
        }

        public void RecordHotkey(Action<int> keyRecordedCallback, Action<IList<int>> finishedRecordingCallback, CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;
            recorded = new Dictionary<int, int>();

            registration = cancellationToken.Register(() =>
            {
                recorded.Clear();
                StopRecording();
            });

            keyRecorded = keyRecordedCallback;
            finishedRecording = finishedRecordingCallback;
            recording = true;

            pressedKeys.Clear();
            showPressed = false;
            moveLeftPressed = false;
            moveRightPressed = false;
        }

        private void UpdatePressed()
        {
            if (ShowHotkey != null)
            {
                if (showPressed)
                {
                    if (!pressedKeys.IsSupersetOf(ShowHotkey))
                    {
                        showPressed = false;
                        View.Hide();
                    }
                }
                else
                {
                    if (pressedKeys.IsSupersetOf(ShowHotkey))
                    {
                        showPressed = true;
                        View.Show();
                    }
                } 
            }
            if (!showPressed) return;
            if (MoveLeftHotkey != null)
            {
                if (moveLeftPressed)
                {
                    if (!pressedKeys.IsSupersetOf(MoveLeftHotkey))
                    {
                        moveLeftPressed = false;
                    }
                }
                else
                {
                    if (pressedKeys.IsSupersetOf(MoveLeftHotkey))
                    {
                        moveLeftPressed = true;
                        View.MoveLeft();
                    }
                } 
            }
            if (MoveRightHotkey != null)
            {
                if (moveRightPressed)
                {
                    if (!pressedKeys.IsSupersetOf(MoveRightHotkey))
                    {
                        moveRightPressed = false;
                    }
                }
                else
                {
                    if (pressedKeys.IsSupersetOf(MoveRightHotkey))
                    {
                        moveRightPressed = true;
                        View.MoveRight();
                    }
                } 
            }
        }

        private void StopRecording()
        {
            recording = false;
            registration.Dispose();

            List<int> result = new List<int>(new int[recorded.Count]);
            foreach (var pair in recorded)
            {
                result[pair.Value] = pair.Key;
            }

            finishedRecording(result);

            cancellationToken = default;
            keyRecorded = null;
            finishedRecording = null;
        }
        private void Record(KeyboardInput input)
        {
            if ((input.Flags & KeyboardFlags.Up) > 0)
            {
                if (recorded.ContainsKey(input.Key))
                {
                    StopRecording();
                }
            }
            else if (!recorded.ContainsKey(input.Key))
            {
                recorded[input.Key] = recorded.Count;
                keyRecorded(input.Key);
            }
        }
        private void Record(MouseInput input)
        {
            Check(input.Buttons, downs, key =>
            {
                if (recorded.ContainsKey(key)) return false;
                recorded[key] = recorded.Count;
                keyRecorded(key);
                return true;
            });
            Check(input.Buttons, ups, key =>
            {
                if (recorded.ContainsKey(key))
                {
                    StopRecording();
                    return true;
                }
                return false;
            });
        }

        public void LoadHotkeys(IUserSettings userSettings)
        {
            ShowHotkey = userSettings.ShowHotkey;
            MoveLeftHotkey = userSettings.MoveLeftHotkey;
            MoveRightHotkey = userSettings.MoveRightHotkey;
        }
    }
}
