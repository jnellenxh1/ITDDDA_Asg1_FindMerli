using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Database;
using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;

public class StampCollectionUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform stampContainer; // Parent for stamp icons (use GridLayoutGroup)
    [SerializeField] private GameObject stampIconPrefab; // Prefab for each stamp
    [SerializeField] private TextMeshProUGUI stampCountText;
    
    [Header("Firebase")]
    private DatabaseReference databaseRef;
    private FirebaseAuth auth;
    private string userId;
    
    // Define all stamps that exist in your game
    private List<StampInfo> allStamps = new List<StampInfo>
    {
        new StampInfo("stamp_marina_bay", "Marina Bay"),
        new StampInfo("stamp_chinatown", "Chinatown"),
        new StampInfo("stamp_little_india", "Little India"),
        new StampInfo("stamp_kampong_glam", "Kampong Glam"),
        new StampInfo("stamp_sentosa", "Sentosa")
    };

    void Start()
    {
        StartCoroutine(InitializeAndLoadStamps());
    }

    private IEnumerator InitializeAndLoadStamps()
    {
        // Wait for Firebase
        yield return new WaitForSeconds(0.5f);
        
        databaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;
        
        if (auth.CurrentUser != null)
        {
            userId = auth.CurrentUser.UserId;
            yield return StartCoroutine(LoadAndDisplayStamps());
        }
        else
        {
            Debug.LogError("No user logged in!");
        }
    }

    private IEnumerator LoadAndDisplayStamps()
    {
        Debug.Log("Loading stamps...");

        // Clear existing stamp icons
        foreach (Transform child in stampContainer)
        {
            Destroy(child.gameObject);
        }

        // Get all stamps from database
        var stampsTask = databaseRef
            .Child("users")
            .Child(userId)
            .Child("stamps")
            .GetValueAsync();

        yield return new WaitUntil(() => stampsTask.IsCompleted);

        if (stampsTask.Exception != null)
        {
            Debug.LogError("Failed to load stamps: " + stampsTask.Exception.Message);
            yield break;
        }

        DataSnapshot stampsSnapshot = stampsTask.Result;
        int collectedCount = 0;

        // Create UI for each stamp
        foreach (StampInfo stampInfo in allStamps)
        {
            bool isCollected = false;

            // Check if stamp is collected
            if (stampsSnapshot.Exists && stampsSnapshot.HasChild(stampInfo.stampId))
            {
                DataSnapshot stampData = stampsSnapshot.Child(stampInfo.stampId);
                isCollected = stampData.Child("collected").Value.ToString() == "True";
                
                if (isCollected)
                {
                    collectedCount++;
                }
            }

            // Create stamp icon
            GameObject stampIcon = Instantiate(stampIconPrefab, stampContainer);
            
            // Get components
            Image stampImage = stampIcon.GetComponent<Image>();
            TextMeshProUGUI stampText = stampIcon.GetComponentInChildren<TextMeshProUGUI>();
            
            // Set appearance based on collected status
            if (stampImage != null)
            {
                if (isCollected)
                {
                    // Full color for collected stamps
                    stampImage.color = Color.white;
                }
                else
                {
                    // Greyed out for uncollected stamps
                    stampImage.color = new Color(0.3f, 0.3f, 0.3f, 0.5f);
                }
            }
            
            // Set location name
            if (stampText != null)
            {
                stampText.text = stampInfo.locationName;
            }
        }

        // Update count text
        if (stampCountText != null)
        {
            stampCountText.text = $"Stamps Collected: {collectedCount}/{allStamps.Count}";
        }

        Debug.Log($"Displayed {allStamps.Count} stamps ({collectedCount} collected)");
    }

    // Call this to refresh the stamp display
    public void RefreshStampDisplay()
    {
        StartCoroutine(LoadAndDisplayStamps());
    }
}

// Helper class to store stamp information
[System.Serializable]
public class StampInfo
{
    public string stampId;
    public string locationName;

    public StampInfo(string stampId, string locationName)
    {
        this.stampId = stampId;
        this.locationName = locationName;
    }
}
