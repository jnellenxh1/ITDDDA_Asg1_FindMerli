using UnityEngine;

// This allows us to create data assets in the Unity Editor:
// Right-click in Project window -> Create -> Poster Content
[CreateAssetMenu(fileName = "NewPosterContent", menuName = "AR/Poster Content")]
public class PosterContent : ScriptableObject
{
    // The name of the image in the AR Reference Image Library
    public string imageName;

    // The Merlion's dialogue lines
    [Header("Dialogue")]
    [TextArea(3, 10)] // Makes the input field tall in the inspector
    public string[] dialogueLines;

    // The Quiz
    [Header("Quiz")]
    [TextArea(3, 10)]
    public string quizQuestion;
    
    public string[] answerOptions; // e.g., "Option A", "Option B", "Option C"
    
    // Index of the correct answer (0, 1, or 2 based on your answerOptions array)
    [Tooltip("The index of the correct answer in the Answer Options array (starts at 0)")]
    public int correctAnswerIndex;
}