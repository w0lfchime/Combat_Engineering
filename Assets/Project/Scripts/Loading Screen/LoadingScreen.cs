using System.Collections;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(WaitAndLoadTitleScreen());
    }

    private IEnumerator WaitAndLoadTitleScreen()
    {
        // Wait for 5 seconds
        yield return new WaitForSeconds(5f);

        // Load the Title screen
        GameManager2.Instance.LoadScene("TitleScreen");
    }
}
