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
        StartCoroutine(InitializeFirebase());
    }

    private IEnumerator InitializeFirebase()
    {
        var dependencyTask = FirebaseApp.CheckAndFixDependenciesAsync();
        
        yield return new WaitUntil(() => dependencyTask.IsCompleted);

        if (dependencyTask.Result == DependencyStatus.Available)
        {
            auth = FirebaseAuth.DefaultInstance;
            database = FirebaseDatabase.DefaultInstance.RootReference;
            Debug.Log("Firebase initialized successfully for Login");
        }
        else
        {
            Debug.LogError("Could not resolve Firebase dependencies: " + dependencyTask.Result);
        }
    }

    /// <summary>
    /// Called when the Login button is clicked
    /// </summary>
    public void OnLoginButtonClicked()
    {
        string email = emailInput.text.Trim();
        string password = passwordInput.text;

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

        StartCoroutine(Login(email, password));
    }

    /// <summary>
    /// Handles user login with firebase authentication
    /// </summary>
    private IEnumerator Login(string email, string password)
    {
        var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
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
            user = loginTask.Result.User;
            Debug.Log($"User logged in successfully: {user.Email}");
            Debug.Log($"User ID: {user.UserId}");
            
            if (!string.IsNullOrEmpty(user.DisplayName))
            {
                Debug.Log($"Welcome back, {user.DisplayName}!");
            }

            yield return StartCoroutine(LoadUserData());
        }
    }

    /// <summary>
    /// Loads user data from the firebase realtime database
    /// </summary>
    private IEnumerator LoadUserData()
    {
        Debug.Log("Loading user data from database...");

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
            
            PlayerPrefs.SetString("CurrentUsername", username);
            PlayerPrefs.SetString("CurrentEmail", email);
            PlayerPrefs.SetString("CurrentUserId", user.UserId);
            PlayerPrefs.Save();
        }

    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Home Page");
    }
    }
}

