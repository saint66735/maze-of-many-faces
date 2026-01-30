using UnityEngine;
using UnityEngine.Rendering;

public class MaskController : MonoBehaviour
{
    [SerializeField] GameObject displayObject;
    [SerializeField] GameObject[] hideObjects;
    [SerializeField] VolumeProfile profile;
    int maskType = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    public void OnInteract(Volume volume)
    {
        if (volume != null && profile != null)
        {
            volume.profile = profile;
        }
        if (displayObject == null || hideObjects.Length == 0)
        {
            return;
        }
        displayObject.SetActive(true);
        foreach (GameObject gameObject in hideObjects)
        {
            gameObject.SetActive(false);
        }

    }
}
