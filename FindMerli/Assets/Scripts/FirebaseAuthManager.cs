using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class FirebaseAuthManager : MonoBehaviour
{
    [Header("Firebase")]
    private FirebaseAuth auth;
    private FirebaseUser user;
    private DatabaseReference database;

    [Header("UI References")]
    public TMP_InputField usernameInput;
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;

    private void Start()
    {
        // Initialize Firebase
        StartCoroutine(InitializeFirebase());
    }

    private IEnumerator InitializeFirebase()
    {
        // Check if Firebase is ready
        var dependencyTask = FirebaseApp.CheckAndFixDependenciesAsync();
        
        yield return new WaitUntil(() => dependencyTask.IsCompleted);

        if (dependencyTask.Result == DependencyStatus.Available)
        {
            // Firebase is ready
            auth = FirebaseAuth.DefaultInstance;
            database = FirebaseDatabase.DefaultInstance.RootReference;
            Debug.Log("Firebase initialized successfully");
        }
        else
        {
            Debug.LogError("Could not resolve Firebase dependencies: " + dependencyTask.Result);
        }
    }

    public void OnSignUpButtonClicked()
    {
        // Get input values
        string username = usernameInput.text.Trim();
        string email = emailInput.text.Trim();
        string password = passwordInput.text;

        // Validate inputs
        if (string.IsNullOrEmpty(username))
        {
            Debug.Log("Please enter a username");
            return;
        }

        if (username.Length < 3)
        {
            Debug.Log("Username must be at least 3 characters");
            return;
        }

        if (string.IsNullOrEmpty(email))
        {
            Debug.Log("Please enter an email");
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            Debug.Log("Please enter a password");
            return;
        }

        if (password.Length < 6)
        {
            Debug.Log("Password must be at least 6 characters");
            return;
        }

        // Start sign-up process
        StartCoroutine(SignUp(username, email, password));
    }

    private IEnumerator SignUp(string username, string email, string password)
    {
        Debug.Log("Creating account...");

        // Create user with email and password
        var signUpTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => signUpTask.IsCompleted);

        if (signUpTask.Exception != null)
        {
            // Handle errors
            Debug.LogError("Sign up failed: " + signUpTask.Exception.Message);
        }
        else
        {
            // Sign-up successful
            user = signUpTask.Result.User;
            Debug.Log($"User created successfully: {user.Email}");

            // Update display name and save to database
            StartCoroutine(UpdateUsernameAndSaveToDatabase(username));
        }
    }

private IEnumerator UpdateUsernameAndSaveToDatabase(string username)
{
    // Update display name in Firebase Auth
    UserProfile profile = new UserProfile
    {
        DisplayName = username
    };

    var profileTask = user.UpdateUserProfileAsync(profile);

    yield return new WaitUntil(() => profileTask.IsCompleted);

    if (profileTask.Exception != null)
    {
        Debug.LogError("Failed to set username: " + profileTask.Exception.Message);
    }
    else
    {
        Debug.Log($"Username set to: {username}");
    }

    // Save user data to Realtime Database
    yield return StartCoroutine(SaveUserToDatabase(username));
}

private IEnumerator SaveUserToDatabase(string username)
{
    Debug.Log("Saving user to Realtime Database...");

    // Save each field individually using Child().SetValueAsync()
    var usernameTask = database.Child("users").Child(user.UserId).Child("username").SetValueAsync(username);
    yield return new WaitUntil(() => usernameTask.IsCompleted);

    var emailTask = database.Child("users").Child(user.UserId).Child("email").SetValueAsync(user.Email);
    yield return new WaitUntil(() => emailTask.IsCompleted);

    var userIdTask = database.Child("users").Child(user.UserId).Child("userId").SetValueAsync(user.UserId);
    yield return new WaitUntil(() => userIdTask.IsCompleted);

    var createdAtTask = database.Child("users").Child(user.UserId).Child("createdAt").SetValueAsync(System.DateTime.UtcNow.ToString("o"));
    yield return new WaitUntil(() => createdAtTask.IsCompleted);

    var pointsTask = database.Child("users").Child(user.UserId).Child("knowledgePoints").SetValueAsync(0);
    yield return new WaitUntil(() => pointsTask.IsCompleted);

    // Check for errors
    if (usernameTask.Exception != null || emailTask.Exception != null || 
        userIdTask.Exception != null || createdAtTask.Exception != null || 
        pointsTask.Exception != null)
    {
        Debug.LogError("Failed to save user to database");
    }
    else
    {
        Debug.Log("User data saved to Realtime Database successfully!");
        
        // Initialize stamps after user data is saved
        yield return StartCoroutine(InitializeUserStamps());
    }

    // Wait 2 seconds then load homepage scene
    yield return new WaitForSeconds(2f);
    SceneManager.LoadScene("Home Page"); 
}

private IEnumerator InitializeUserStamps()
{
    Debug.Log("Initializing stamps for new user...");

    // Define all stamps
    string[] stampIds = new string[]
    {
        "stamp_marina_bay",
        "stamp_chinatown",
        "stamp_little_india",
        "stamp_kampong_glam",
        "stamp_sentosa"
    };

    string[] locationNames = new string[]
    {
        "Marina Bay",
        "Chinatown",
        "Little India",
        "Kampong Glam",
        "Sentosa"
    };

    // Save each stamp individually
    for (int i = 0; i < stampIds.Length; i++)
    {
        string stampId = stampIds[i];
        string locationName = locationNames[i];

        // Set collected to false
        var collectedTask = database.Child("users").Child(user.UserId)
            .Child("stamps").Child(stampId).Child("collected").SetValueAsync(false);
        yield return new WaitUntil(() => collectedTask.IsCompleted);

        // Set timestamp to empty string
        var timestampTask = database.Child("users").Child(user.UserId)
            .Child("stamps").Child(stampId).Child("timestamp").SetValueAsync("");
        yield return new WaitUntil(() => timestampTask.IsCompleted);

        // Set location name
        var locationTask = database.Child("users").Child(user.UserId)
            .Child("stamps").Child(stampId).Child("locationName").SetValueAsync(locationName);
        yield return new WaitUntil(() => locationTask.IsCompleted);

        if (collectedTask.Exception != null || timestampTask.Exception != null || locationTask.Exception != null)
        {
            Debug.LogError($"Failed to initialize stamp: {stampId}");
        }
        else
        {
            Debug.Log($"Stamp initialized: {stampId}");
        }
    }

    Debug.Log("All stamps initialized successfully!");
    {

        // Wait 2 seconds then load homepage scene
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Home Page"); 
    }
    }
}