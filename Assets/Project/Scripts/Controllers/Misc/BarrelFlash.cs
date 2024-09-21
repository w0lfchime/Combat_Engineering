using System.Collections;
using UnityEngine;

public class GunFire : MonoBehaviour
{

    public ParticleSystem muzzleFlash; // Drag the particle system here
    public Light flashLight;           // Drag the light here

    public void Fire()
    {
        // Play the muzzle flash particle system
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }
        // Flash the light briefly
        StartCoroutine(FlashLightEffect());
    }
    public void OnAim()
    {
        flashLight.enabled = false;
    }

    IEnumerator FlashLightEffect()
    {
        flashLight.enabled = true;  // Turn the light on
        yield return new WaitForSeconds(0.05f);  // Keep it on for 0.05 seconds
        flashLight.enabled = false; // Turn the light off
    }
}
