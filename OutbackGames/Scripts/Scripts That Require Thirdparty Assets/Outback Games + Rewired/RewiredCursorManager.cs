//****************************
//*
//* Created By Lochlan Kennedy - Outback Games
//* https://outbackgames.com.au | https://github.com/Outback-Games/Outback-Games-Simple-UI-Tools-for-Unity
//*
//****************************

using System.Collections;
using UnityEngine;
using Rewired;
namespace OutbackGames.UI.Tools
{
    public class RewiredCursorManager : MonoBehaviour
    {
        [Header("Cursor Image Settings")]
        [SerializeField, Tooltip("Cursor For Overworld")]
        Texture2D overworldCursor;

        [SerializeField, Tooltip("Ui Cursor\nLeave empty to use system default.")]
        Texture2D uiCursor;
        
        [SerializeField, Tooltip("Default Cursor for the Scene\nLeave empty to use system default.")]
        Texture2D defaultCursorImage;

        [SerializeField, Tooltip("Cursor Hotspot")]
        Vector2 cursorHotspot = Vector2.zero;

        [Header("Change Cursor Image Conditions")]
        [SerializeField, Tooltip("Objects To Cause Default Change")]
        GameObject[] changeToUiCursorObjects;

        [Header("Polling Settings")]
        [SerializeField, Tooltip("Delay Between Change Checks\nUse this if you want a shorter or longer wait between cursor changes"), Range(0.01f, 2.5f)]
        float unscaledChangeInterval = 0.25f;
        int objOnCount = 0;
        [SerializeField,Tooltip("The amount of time to wait before unlocking the mouse while loading in.")]
        float timeToInitialize = 2f;
        
        [Header("Rewired Gamepad Settings")]
        [SerializeField]
        string playerID = "DefaultPlayer";
        [SerializeField]
        bool isGamepadConnected;
        Player rewiredPlayer;

        private void Start()
        {
            rewiredPlayer = ReInput.players.GetPlayer(playerID);
            
#if UNITY_STANDALONE || UNITY_EDITOR
            StartCoroutine(Initialize());
#endif
            //if it is on any other device we'll just leave the cursor off.
            Cursor.visible = false;
        }

        IEnumerator Initialize()
        {
            LockMouseState();
            yield return new WaitForSecondsRealtime(timeToInitialize);
            StartCoroutine(CursorChangeLoop());
        }

        IEnumerator CursorChangeLoop() {
            CheckGamepad();
            ChangeCursor();
            yield return new WaitForSecondsRealtime(unscaledChangeInterval);
            StartCoroutine(CursorChangeLoop());
        }

        void CheckGamepad()
        {
            Controller controller = rewiredPlayer.controllers.GetLastActiveController();
            if (controller != null)
            {
                switch (controller.type)
                {
                    case ControllerType.Keyboard:
                        // Do something for keyboard
                        isGamepadConnected = false;
                        break;
                    case ControllerType.Joystick:
                        // Do something for joystick
                        Cursor.visible = false;
                        isGamepadConnected = true;
                        break;
                    case ControllerType.Mouse:
                        // Do something for mouse
                        isGamepadConnected = false;
                        break;
                    case ControllerType.Custom:
                        // Do something custom controller
                        break;
                }
            }
        }

        protected virtual void ChangeCursor() {

            if (isGamepadConnected) { return; }
            UnlockMouse();

            if (changeToUiCursorObjects == null)
            {
                return;
            }

            foreach (GameObject selected in changeToUiCursorObjects)
            {
                if (selected.activeInHierarchy)
                {
                    objOnCount++;
                    break;
                }
            }

            //if there's an object turned on from the list, then set it to default cursor, else change back to overworld cursor.
            if (objOnCount > 0)
            {
                defaultCursorImage = uiCursor;
                Cursor.SetCursor(uiCursor, cursorHotspot, CursorMode.Auto);
                objOnCount = 0;
            }
            else if (objOnCount <= 0)
            {
                defaultCursorImage = overworldCursor;
                Cursor.SetCursor(overworldCursor, cursorHotspot, CursorMode.Auto);
            }

        }

        public void ChangeToDefault()
        {
            UnlockMouse();
            Cursor.SetCursor(defaultCursorImage, cursorHotspot, CursorMode.Auto);
        }

        public void ChangeToUICursor()
        {
            UnlockMouse();
            Cursor.SetCursor(uiCursor, cursorHotspot, CursorMode.Auto);
        }

        public void ChangeToOverworldCursor()
        {
            UnlockMouse();
            Cursor.SetCursor(overworldCursor, cursorHotspot, CursorMode.Auto);
        }

        public void UnlockMouse()
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            if (Cursor.visible == false)
            {
                Cursor.visible = true;
            }
        }

        public void LockMouseState()
        {
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        private void OnApplicationQuit()
        {
            UnlockMouse();
            StopAllCoroutines();
        }

        private void OnDestroy() {
            //UnlockMouse(); //--Enable this if you're having problems when the this object is destroyed.
            StopAllCoroutines();
        }
    }

}
