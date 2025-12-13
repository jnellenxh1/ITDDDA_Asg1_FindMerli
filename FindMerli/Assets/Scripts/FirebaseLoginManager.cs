using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class FirebaseLoginManager : MonoBehaviour
{
    [Header("Firebase")]
    private FirebaseAuth auth;
    private FirebaseUser user;
    private DatabaseReference database;

    [Header("UI References")]
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
            Debug.Log("Firebase initialized successfully for Login");
        }
        else
        {
            Debug.LogError("Could not resolve Firebase dependencies: " + dependencyTask.Result);
        }
    }

    public void OnLoginButtonClicked()
    {
        // Get input values
        string email = emailInput.text.Trim();
        string password = passwordInput.text;

        // Validate inputs
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

        // Start login process
        StartCoroutine(Login(email, password));
    }

    private IEnumerator Login(string email, string password)
    {
        Debug.Log("Attempting to log in...");

        // Sign in with email and password
        var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            // Login failed - handle errors
            Debug.LogError("Login failed: " + loginTask.Exception.Message);
            
            FirebaseException firebaseEx = loginTask.Exception.GetBaseException() as FirebaseException;
            if (firebaseEx != null)
            {
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                
                switch (errorCode)
                {
                    case AuthError.UserNotFound:
                        Debug.Log("User not found - please sign up first");
                        break;
                    case AuthError.WrongPassword:
                        Debug.Log("Wrong password - please try again");
                        break;
                    case AuthError.InvalidEmail:
                        Debug.Log("Invalid email format");
                        break;
                    case AuthError.UserDisabled:
                        Debug.Log("This account has been disabled");
                        break;
                    default:
                        Debug.Log("Login error: " + errorCode);
                        break;
                }
            }
        }
        else
        {
            // Login successful
            user = loginTask.Result.User;
            Debug.Log($"User logged in successfully: {user.Email}");
            Debug.Log($"User ID: {user.UserId}");
            
            // Get username from Auth profile
            if (!string.IsNullOrEmpty(user.DisplayName))
            {
                Debug.Log($"Welcome back, {user.DisplayName}!");
            }

            // Retrieve user data from database
            yield return StartCoroutine(LoadUserData());
        }
    }

    private IEnumerator LoadUserData()
    {
        Debug.Log("Loading user data from database...");

        // Get user data from Realtime Database
        var dataTask = database.Child("users").Child(user.UserId).GetValueAsync();

        yield return new WaitUntil(() => dataTask.IsCompleted);

        if (dataTask.Exception != null)
        {
            Debug.LogError("Failed to load user data: " + dataTask.Exception.Message);
        }
        else if (dataTask.Result.Value == null)
        {
            Debug.LogWarning("No user data found in database");
        }
        else
        {
            DataSnapshot snapshot = dataTask.Result;
            
            string username = snapshot.Child("username").Value.ToString();
            string email = snapshot.Child("email").Value.ToString();
            string createdAt = snapshot.Child("createdAt").Value.ToString();
            
            Debug.Log($"User data loaded:");
            Debug.Log($"  Username: {username}");
            Debug.Log($"  Email: {email}");
            Debug.Log($"  Account created: {createdAt}");
            
            // You can store this data for use in your app
            PlayerPrefs.SetString("CurrentUsername", username);
            PlayerPrefs.SetString("CurrentEmail", email);
            PlayerPrefs.SetString("CurrentUserId", user.UserId);
            PlayerPrefs.Save();
        }

    { // Wait 1 second then load homepage scene
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Home Page");
    }
    }
}

