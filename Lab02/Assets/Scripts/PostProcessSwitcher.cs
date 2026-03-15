using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessSwitcher : MonoBehaviour
{
    [Header("Volume that holds the active profile")]
    public PostProcessVolume volume;

    [Header("Profiles to cycle through")]
    public PostProcessProfile[] profiles;

    [Header("Starting index")]
    public int currentIndex = 0;

    void Start()
    {
        if (profiles.Length > 0 && volume != null)
        {
            currentIndex = Mathf.Clamp(currentIndex, 0, profiles.Length - 1);
            volume.profile = profiles[currentIndex];
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            NextProfile();
        }
    }

    void NextProfile()
    {
        if (profiles.Length == 0 || volume == null)
            return;

        currentIndex++;

        if (currentIndex >= profiles.Length)
            currentIndex = 0;

        volume.profile = profiles[currentIndex];
    }
}
