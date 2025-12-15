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

    [Header("Dialogue and Quiz System")]
    [SerializeField]
    private DialogueManager dialogueManager; // Assign your DialogueManager here

    [SerializeField]
    private PosterContent[] allPosterContent; // Assign all 10 ScriptableObjects here

    private Dictionary<string, PosterContent> contentMap = new Dictionary<string, PosterContent>();
    
    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();
    private Dictionary<GameObject, GameObject> spawnedObjects = new Dictionary<GameObject, GameObject>();

    private void Start()
    {
        if (trackedImageManager != null)
        {
            trackedImageManager.trackablesChanged.AddListener(OnImageChanged);
            SetupPrefabs();
            SetupContentMap(); // NEW call
        }
    }
    
    // --- NEW METHOD ---
    void SetupContentMap()
    {
        foreach (PosterContent content in allPosterContent)
        {
            // Key = the name of the reference image in the AR Reference Image Library
            contentMap.Add(content.imageName, content); 
        }
    }
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

                // --- NEW LOGIC: START DIALOGUE ---
                if (contentMap.ContainsKey(imageName))
                {
                    Debug.Log("Tracking: " + imageName + ". Starting dialogue.");
                    dialogueManager.StartDialogue(contentMap[imageName]);
                }
                // ----------------------------------
            }
        }
        else // Limited or None tracking state
        {
            // Your existing code to disable the content
            spawnedPrefabs[imageName].transform.SetParent(null);
            spawnedPrefabs[imageName].SetActive(false);
            
            // --- NEW LOGIC: HIDE UI ---
            if (dialogueManager.dialoguePanel.activeSelf || dialogueManager.quizPanel.activeSelf)
            {
                 dialogueManager.dialoguePanel.SetActive(false);
                 dialogueManager.quizPanel.SetActive(false);
            }
            // --------------------------
        }
    }
}