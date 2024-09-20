using UnityEngine;

public class CursorControl : MonoBehaviour {
    private void Start() {
        // Turn on the cursor for Main Menu and Death Scene
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
