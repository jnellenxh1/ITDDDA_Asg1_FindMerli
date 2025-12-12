using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class ImageTracker : MonoBehaviour
{
    [SerializeField]
    private ARTrackedImageManager trackedImageManager;

    [SerializeField]
    private GameObject[] placeablePrefabs;

    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();

    private void Start()
    {
        if (trackedImageManager != null)
        {
            trackedImageManager.trackablesChanged.AddListener(OnImageChanged);
            SetupPrefabs();
        }
    }

    void SetupPrefabs()
    {
        foreach (GameObject prefab in placeablePrefabs)
        {
            GameObject newPrefab = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            newPrefab.name = prefab.name;
            newPrefab.SetActive(false);
            spawnedPrefabs.Add(prefab.name, newPrefab);
        }
    }

    void OnImageChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            UpdateImage(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            UpdateImage(trackedImage);
        }

        foreach (KeyValuePair<TrackableId, ARTrackedImage> lostObj in eventArgs.removed)
        {
            UpdateImage(lostObj.Value);
        }
    }

    void UpdateImage(ARTrackedImage trackedImage)
{
    if (trackedImage != null)
    {
        GameObject prefab = spawnedPrefabs[trackedImage.referenceImage.name];

        if (trackedImage.trackingState == TrackingState.Limited || trackedImage.trackingState == TrackingState.None)
        {
            // When tracking is lost, simply hide it
            prefab.SetActive(false);
        }
        else if (trackedImage.trackingState == TrackingState.Tracking)
        {
            // Get the Animator script attached to the Merlion prefab
            MerlionAnimator animator = prefab.GetComponent<MerlionAnimator>();

            if (animator != null)
            {
                // **NEW LOGIC: Start the animation on the animator script**
                // The animator script handles the position and scale over time.
                animator.StartEntranceAnimation(trackedImage.transform.position);

                // Note: We MUST set the rotation directly so the object faces the right way
                prefab.transform.rotation = trackedImage.transform.rotation;
            }
            else
            {
                // Fallback (if the animator script is missing)
                prefab.transform.position = trackedImage.transform.position;
                prefab.transform.rotation = trackedImage.transform.rotation;
                prefab.SetActive(true);
            }
        }
    }
}
}
