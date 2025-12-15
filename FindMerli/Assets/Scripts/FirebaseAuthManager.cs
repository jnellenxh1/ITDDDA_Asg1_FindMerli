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
        StartCoroutine(InitializeFirebase());
    }

    /// <summary>
    /// Initializes Firebase
    /// </summary>
    private IEnumerator InitializeFirebase()
    {
        var dependencyTask = FirebaseApp.CheckAndFixDependenciesAsync();
        
        yield return new WaitUntil(() => dependencyTask.IsCompleted);

        if (dependencyTask.Result == DependencyStatus.Available)
        {
            auth = FirebaseAuth.DefaultInstance;
            database = FirebaseDatabase.DefaultInstance.RootReference;
        }
    }

    /// <summary>
    /// Called when the Sign Up button is clicked
    /// Validates input and creates a new user account
    /// </summary>
    public void OnSignUpButtonClicked()
    {
        string username = usernameInput.text.Trim();
        string email = emailInput.text.Trim();
        string password = passwordInput.text;

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
        StartCoroutine(SignUp(username, email, password));
    }

    /// <summary>
    /// Handles user sign up with firebase authentication
    /// </summary>
    private IEnumerator SignUp(string username, string email, string password)
    {
        Debug.Log("Creating account...");

        var signUpTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => signUpTask.IsCompleted);

        if (signUpTask.Exception != null)
        {
            Debug.LogError("Sign up failed: " + signUpTask.Exception.Message);
        }
        else
        {
            user = signUpTask.Result.User;
            Debug.Log($"User created successfully: {user.Email}");

            StartCoroutine(UpdateUsernameAndSaveToDatabase(username));
        }
    }

    /// <summary>
    /// Updates the user's display name and saves user data to Realtime Database
    /// </summary>
    private IEnumerator UpdateUsernameAndSaveToDatabase(string username)
    {
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

        yield return StartCoroutine(SaveUserToDatabase(username));
    }

    /// <summary>
    /// Saves the new user's data to Realtime Database
    /// </summary>
    private IEnumerator SaveUserToDatabase(string username)
    {
        Debug.Log("Saving user to Realtime Database...");

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

        if (usernameTask.Exception != null || emailTask.Exception != null || 
            userIdTask.Exception != null || createdAtTask.Exception != null || 
            pointsTask.Exception != null)
        {
            Debug.LogError("Failed to save user to database");
        }
        else
        {
            Debug.Log("User data saved to Realtime Database successfully!");
            yield return StartCoroutine(InitializeUserStamps());
        }

        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Home Page"); 
    }

    /// <summary>
    /// Initializes the stamp collection for the new user
    /// </summary>
    private IEnumerator InitializeUserStamps()
    {
        Debug.Log("Initializing stamps for new user...");


        string[] stampIds = new string[]
        {
            "stamp_siloso",
            "stamp_fullerton",
            "stamp_istana",
            "stamp_lps",
            "stamp_nmos",
            "stamp_ohsps",
            "stamp_sac",
            "stamp_sri",
            "stamp_sultan",
            "stamp_thk"
        };

        string[] locationNames = new string[]
        {
            "Fort Siloso",
            "Fullerton Hotel",
            "Istana",
            "Lau Pa Sat",
            "National Museum of Singapore",
            "Old Hill Street Police Station",
            "Saint Andrew's Cathedral",
            "Sri Mariamman Temple",
            "Sultan Mosque",
            "Tian Hock Keng Temple"
        };


        /// <summary>
        /// Initializes the stamp collection for the new user
        /// Each stamp is marked as not collected initially
        /// Corresponding location names are also saved
        /// </summary>
        for (int i = 0; i < stampIds.Length; i++)
        {
            string stampId = stampIds[i];
            string locationName = locationNames[i];

            var collectedTask = database.Child("users").Child(user.UserId)
                .Child("stamps").Child(stampId).Child("collected").SetValueAsync(false);
            yield return new WaitUntil(() => collectedTask.IsCompleted);

            var timestampTask = database.Child("users").Child(user.UserId)
                .Child("stamps").Child(stampId).Child("timestamp").SetValueAsync("");
            yield return new WaitUntil(() => timestampTask.IsCompleted);

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
            yield return new WaitForSeconds(2f);
            SceneManager.LoadScene("Home Page"); 
        }
    }
}