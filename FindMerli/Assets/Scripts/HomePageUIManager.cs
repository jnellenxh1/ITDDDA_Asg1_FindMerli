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

    /// <summary>
    /// Initializes Firebase and loads user data
    /// </summary>
    private IEnumerator InitializeAndLoadData()
    {
        yield return new WaitForSeconds(0.5f);

        databaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;

        if (auth.CurrentUser != null)
        {
            userId = auth.CurrentUser.UserId;
            username = auth.CurrentUser.DisplayName;

            if (string.IsNullOrEmpty(username))
            {
                username = PlayerPrefs.GetString("CurrentUsername", "User");
            }
            
            Debug.Log($"Home page loaded for user: {username}");

            if (welcomeText != null)
            {
                welcomeText.text = $"Welcome, {username}!";
            }          
        }
        else
        {
            Debug.LogError("No user logged in!");
        }
    }
}
