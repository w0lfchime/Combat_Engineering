using UnityEngine;

public class InputManager : MonoBehaviour
{
    public KeyCode gameMenuPause = KeyCode.Escape;

    void Update()
    {
        if (Input.GetKeyDown(gameMenuPause))
        {
            GameManager.Instance.QuitGame();
        }
    }
}
