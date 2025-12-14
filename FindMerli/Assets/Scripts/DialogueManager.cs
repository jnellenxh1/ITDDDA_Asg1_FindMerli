using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public Button nextLineButton;
    
    public GameObject quizPanel;
    public TMP_Text questionText;
    public Button[] answerButtons;
    public GameObject correctStampImage;

    private PosterContent currentPosterContent;
    private int currentDialogueIndex = 0;

    void Start()
    {
        dialoguePanel.SetActive(false);
        quizPanel.SetActive(false);
        nextLineButton.onClick.AddListener(DisplayNextLine);

        // Add listeners to the answer buttons
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i; // Local copy for the lambda
            answerButtons[i].onClick.AddListener(() => CheckAnswer(index));
        }
    }

    public void OnAnswerSelected(int answerIndex)
{
    int correctIndex = currentPosterContent.correctAnswerIndex; 

    // --- The primary logic change goes here ---
    if (answerIndex == correctIndex)
    {
        // 1. Show Success Feedback
        questionText.text = "Correct! History unlocked!"; 

        correctStampImage.SetActive(true);
        
        correctStampImage.SetActive(true);
        // 2. Disable all buttons (optional, but good practice)
        SetAnswerButtonsInteractable(false); 
        
        // 3. Start the Coroutine to wait and then close the quiz
        StartCoroutine(CloseQuizAfterDelay(2.0f)); 
    }
    else
    {
        // INCORRECT ANSWER: Show feedback and prompt the user to try again
        questionText.text = "That's not quite right. Please try another answer!";
        
        // OPTIONAL: Make the incorrect button non-interactable to guide the user
        answerButtons[answerIndex].interactable = false; 
    }
}

// Add this new method immediately after the OnAnswerSelected method:

private IEnumerator CloseQuizAfterDelay(float delay)
{
    yield return new WaitForSeconds(delay); 
    
    quizPanel.SetActive(false); 
    
    SetAnswerButtonsInteractable(true); 
}

private void SetAnswerButtonsInteractable(bool state)
{
    foreach (var button in answerButtons)
    {
        button.interactable = state;
    }
}

    // --- Public Method Called by ImageTracker ---
    public void StartDialogue(PosterContent content)
    {
        currentPosterContent = content;
        currentDialogueIndex = 0;
        
        // Ensure the Merlion 3D model is active when this is called by the ImageTracker
        // (The ImageTracker handles the Merlion's visibility, but keep this in mind)

        quizPanel.SetActive(false);
        dialoguePanel.SetActive(true);
        DisplayNextLine(); // Start with the first line
    }

    // --- Dialogue Flow ---
    public void DisplayNextLine()
    {
        if (currentPosterContent == null) return;

        if (currentDialogueIndex < currentPosterContent.dialogueLines.Length)
        {
            // Display the current line
            dialogueText.text = currentPosterContent.dialogueLines[currentDialogueIndex];
            currentDialogueIndex++;
            // The button will be visible and the user clicks it to call this method again
        }
        else
        {
            // Dialogue is finished, start the quiz
            dialoguePanel.SetActive(false);
            StartQuiz();
        }
    }

    // --- Quiz Flow ---
    void StartQuiz()
    {
        quizPanel.SetActive(true);
        questionText.text = currentPosterContent.quizQuestion;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < currentPosterContent.answerOptions.Length)
            {
                answerButtons[i].gameObject.SetActive(true);
                answerButtons[i].GetComponentInChildren<TMP_Text>().text = currentPosterContent.answerOptions[i];
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false); // Hide unused buttons
            }
        }
    }

    void CheckAnswer(int selectedAnswerIndex)
    {
        // Disable all buttons to prevent double-clicking or changing answer
        foreach (Button btn in answerButtons)
        {
            btn.interactable = false;
        }

        if (selectedAnswerIndex == currentPosterContent.correctAnswerIndex)
        {
            questionText.text = "Correct, well done! You earned a stamp!";
            // Optionally, change the button color to green
        }
        else
        {
            questionText.text = "Incorrect. Try again.";
            // Optionally, change the button color to red
        }

        // TODO: Add a button/delay to hide the quiz panel after a few seconds
    }
}