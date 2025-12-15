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

    [SerializeField]
    private DialogueManager dialogueManager;

    [SerializeField]
    private PosterContent[] allPosterContent;

    private Dictionary<string, PosterContent> contentMap = new Dictionary<string, PosterContent>();
    
    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();
    private Dictionary<GameObject, GameObject> spawnedObjects = new Dictionary<GameObject, GameObject>();

    /// <summary>
    /// Called when the script instance is being loaded
    /// Initializes the image tracking and sets up prefabs and content mapping
    /// </summary>
    private void Start()
    {
        if (trackedImageManager != null)
        {
            trackedImageManager.trackablesChanged.AddListener(OnImageChanged);
            SetupPrefabs();
            SetupContentMap();
        }
    }

    /// <summary>
    /// Sets up the mapping between image names and their corresponding poster content
    /// </summary>
    void SetupContentMap()
    {
        foreach (PosterContent content in allPosterContent)
        {
            contentMap.Add(content.imageName, content); 
        }
    }

    /// <summary>
    /// Sets up the prefabs for image tracking
    /// </summary>
    void SetupPrefabs()
    {
        foreach (GameObject prefab in placeablePrefabs)
        {
            GameObject newPrefab = Instantiate(prefab);
            newPrefab.name = prefab.name;
            newPrefab.SetActive(false);
            spawnedPrefabs.Add(prefab.name, newPrefab);
            spawnedObjects.Add(newPrefab, prefab);
        }
    }

    /// <summary>
    /// Called when the tracked images change (added, updated, removed)
    /// </summary>
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

    /// <summary>
    /// Updates the image tracking state and manages the associated prefab and dialogue
    /// </summary>
    void UpdateImage(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;

        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            // Your existing code to position and enable the Merlion prefab
            if(spawnedPrefabs[imageName].transform.parent != trackedImage.transform)
            {
                spawnedPrefabs[imageName].transform.SetParent(trackedImage.transform);
                spawnedPrefabs[imageName].transform.localPosition = spawnedObjects[spawnedPrefabs[imageName]].transform.localPosition;
                spawnedPrefabs[imageName].transform.localRotation = spawnedObjects[spawnedPrefabs[imageName]].transform.localRotation;
                spawnedPrefabs[imageName].SetActive(true);

                if (contentMap.ContainsKey(imageName))
                {
                    Debug.Log("Tracking: " + imageName + ". Starting dialogue.");
                    dialogueManager.StartDialogue(contentMap[imageName]);
                }
            }
        }
        else
        {
            spawnedPrefabs[imageName].transform.SetParent(null);
            spawnedPrefabs[imageName].SetActive(false);

            if (dialogueManager.dialoguePanel.activeSelf || dialogueManager.quizPanel.activeSelf)
            {
                 dialogueManager.dialoguePanel.SetActive(false);
                 dialogueManager.quizPanel.SetActive(false);
            }
        }
    }
}