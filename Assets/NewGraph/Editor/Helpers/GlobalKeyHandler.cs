using System;
using UnityEditor;
using UnityEngine;

namespace NewGraph
{
    public static class GlobalKeyEventHandler
    {
        public static event Action<Event> OnKeyEvent;
        public static bool RegistrationSucceeded = false;
        private const string globalEventHandler = nameof(globalEventHandler);

        static GlobalKeyEventHandler()
        {
            RegistrationSucceeded = false;
            var msg = "";
            try
            {
                var info = typeof(EditorApplication).GetField(globalEventHandler,
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
                if (info != null)
                {
                    var value = (EditorApplication.CallbackFunction)info.GetValue(null);

                    value -= OnKeyPressed;
                    value += OnKeyPressed;

                    info.SetValue(null, value);

                    RegistrationSucceeded = true;
                }
                else
                {
                    msg = $"{globalEventHandler} not found.";
                }
            }
            catch (Exception e)
            {
                msg = e.Message;
            }
            finally
            {
                if (!RegistrationSucceeded)
                {
                    Debug.LogWarning(
                        $"{nameof(GlobalKeyEventHandler)}: error while registering for {globalEventHandler}: " + msg);
                }
            }
        }

        private static void OnKeyPressed()
        {
            OnKeyEvent?.Invoke(Event.current);
        }
    }
}