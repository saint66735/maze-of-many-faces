using UnityEngine;

public class MaskController : MonoBehaviour
{
    [SerializeField] GameObject displayObject;
    [SerializeField] GameObject[] hideObjects;
    int maskType = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnInteract()
    {
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
