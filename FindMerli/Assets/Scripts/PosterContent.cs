using UnityEngine;

[CreateAssetMenu(fileName = "NewPosterContent", menuName = "AR/Poster Content")]
public class PosterContent : ScriptableObject
{   
    public string imageName;
    [Header("Dialogue")]
    public string[] dialogueLines;
    [Header("Quiz")]
    public string quizQuestion;
    [Header("Answer Options")]
    public string[] answerOptions;
    [Header("Correct Answer Index")]
    public int correctAnswerIndex;
}