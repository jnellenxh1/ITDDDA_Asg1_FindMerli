using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Database;
using Firebase.Auth;
using System.Collections;

public class HomePageUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI knowledgePointsText;
    [SerializeField] private TextMeshProUGUI welcomeText;
    
    [Header("Firebase")]
    private DatabaseReference databaseRef;
    private FirebaseAuth auth;
    private string userId;
    private string username;

    void Start()
    {
        StartCoroutine(InitializeAndLoadData());
    }

    private IEnumerator InitializeAndLoadData()
    {
        // Wait a moment for Firebase to be ready
        yield return new WaitForSeconds(0.5f);
        
        // Initialize Firebase
        databaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;
        
        // Get user info
        if (auth.CurrentUser != null)
        {
            userId = auth.CurrentUser.UserId;
            username = auth.CurrentUser.DisplayName;
            
            // Also try from PlayerPrefs as backup
            if (string.IsNullOrEmpty(username))
            {
                username = PlayerPrefs.GetString("CurrentUsername", "User");
            }
            
            Debug.Log($"Home page loaded for user: {username}");
            
            // Set welcome message
            if (welcomeText != null)
            {
                welcomeText.text = $"Welcome, {username}!";
            }
            
            // Load knowledge points
            yield return StartCoroutine(LoadKnowledgePoints());
        }
        else
        {
            Debug.LogError("No user logged in!");
        }
    }

    private IEnumerator LoadKnowledgePoints()
    {
        Debug.Log("Loading knowledge points...");

        var pointsTask = databaseRef
            .Child("users")
            .Child(userId)
            .Child("knowledgePoints")
            .GetValueAsync();

        yield return new WaitUntil(() => pointsTask.IsCompleted);

        if (pointsTask.Exception != null)
        {
            Debug.LogError("Failed to load points: " + pointsTask.Exception.Message);
            SetKnowledgePoints(0);
        }
        else if (pointsTask.Result.Exists)
        {
            int points = int.Parse(pointsTask.Result.Value.ToString());
            SetKnowledgePoints(points);
            Debug.Log($"Knowledge points loaded: {points}");
        }
        else
        {
            Debug.LogWarning("No knowledge points found, setting to 0");
            SetKnowledgePoints(0);
        }
    }

    public void SetKnowledgePoints(int points)
    {
        if (knowledgePointsText != null)
        {
            knowledgePointsText.text = $"Knowledge Points: {points}";
        }
    }

    // Call this method to refresh the display (e.g., when returning to home page)
    public void RefreshDisplay()
    {
        StartCoroutine(LoadKnowledgePoints());
    }
}
