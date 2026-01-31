using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MaskController : MonoBehaviour
{
    [SerializeField] GameObject displayObject;
    [SerializeField] GameObject[] hideObjects;
    [SerializeField] VolumeProfile profile;
    [SerializeField] Texture overlay;
    int maskType = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    public void OnInteract(Volume volume, out Texture image)
    {
        image = overlay;
        if (displayObject == null || hideObjects.Length == 0)
        {
            return;
        }
        if (volume != null && profile != null)
        {
            volume.profile = profile;
        }

        displayObject.SetActive(true);
        foreach (GameObject gameObject in hideObjects)
        {
            gameObject.SetActive(false);
        }

    }
}
