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

        // Create user data object
        UserData userData = new UserData(username, user.Email, user.UserId);

        // Convert to JSON
        string json = JsonUtility.ToJson(userData);

        Debug.Log($"User data JSON: {json}");

        // Save to Realtime Database under "users/{userId}"
        var saveTask = database.Child("users").Child(user.UserId).SetRawJsonValueAsync(json);

        yield return new WaitUntil(() => saveTask.IsCompleted);

        if (saveTask.Exception != null)
        {
            Debug.LogError("Failed to save user to database: " + saveTask.Exception.Message);
        }
        else
        {
            Debug.Log("User data saved to Realtime Database successfully!");
        }

        // Wait 2 seconds then load homepage scene
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Home Page"); 
    }

}