//****************************
//*
//* Created By Lochlan Kennedy - Outback Games
//* https://outbackgames.com.au | https://github.com/Outback-Games/Outback-Games-Simple-UI-Tools-for-Unity
//*
//****************************
using System.Collections;
using UnityEngine;
namespace OutbackGames.NecoreTowerRedux.UI.Tools
{
    public class CursorManager : MonoBehaviour
    {
        [SerializeField, Tooltip("Cursor For Overworld")]
        Texture2D overworldCursor;

        [SerializeField, Tooltip("Default Cursor")]
        Texture2D defaultCursor;

        [SerializeField, Tooltip("Objects To Cause Default Change")]
        GameObject[] objectsToDefault;

        [SerializeField, Tooltip("Current Default Cursor for the Scene")]
        Texture2D currentCursor;

        [SerializeField, Tooltip("Cursor Hotspot")]
        Vector2 cursorHotspot = Vector2.zero;

        [SerializeField, Tooltip("Delay Between Change Checks\nUse this if you want a shorter or longer wait between cursor changes"), Range(0.01f, 2.5f)]
        float unscaledChangeInterval = 0.25f;
        int objOnCount = 0;

        // Start is called before the first frame update
        void Start() {
            SetInitialCursor();
            StartCoroutine(CursorChangeLoop());
        }

        IEnumerator CursorChangeLoop() {
            ChangeCursor();
            yield return new WaitForSecondsRealtime(unscaledChangeInterval);
            StartCoroutine(CursorChangeLoop()); // restart the loop.
        }

        void SetInitialCursor() {
            Cursor.SetCursor(currentCursor, cursorHotspot, CursorMode.Auto);
        }

        protected virtual void ChangeCursor() {
            if (objectsToDefault == null) {
                return;
            }

            foreach (GameObject selected in objectsToDefault) {
                if (selected.activeInHierarchy) {
                    objOnCount++;
                    break;
                }
            }

            //if there's an object turned on from the list, then set it to default cursor, else change back to overworld cursor.
            if (objOnCount > 0) {
                currentCursor = defaultCursor;
                Cursor.SetCursor(defaultCursor, cursorHotspot, CursorMode.Auto);
                objOnCount = 0;
            }
            else if (objOnCount <= 0) {
                currentCursor = overworldCursor;
                Cursor.SetCursor(overworldCursor, cursorHotspot, CursorMode.Auto);
            }

        }


        private void OnDestroy() {
            StopAllCoroutines();
        }
    }

}
