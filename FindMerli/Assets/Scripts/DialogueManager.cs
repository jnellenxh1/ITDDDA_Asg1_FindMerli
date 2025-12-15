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

    /// <summary>
    /// Called when the script instance is being loaded
    /// </summary>
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
    /// <summary>
    /// Handles answer selection in the quiz
    /// Stamps are given for correct answers
    /// </summary>
    public void OnAnswerSelected(int answerIndex)
    {
        int correctIndex = currentPosterContent.correctAnswerIndex; 

        if (answerIndex == correctIndex)
        {

            correctStampImage.SetActive(true);
        
            correctStampImage.SetActive(true);
        
            SetAnswerButtonsInteractable(false); 

            StartCoroutine(CloseQuizAfterDelay(2.0f)); 
        }
        else
        {
            answerButtons[answerIndex].interactable = false; 
        }
    }

    /// <summary>
    /// Closes the quiz panel after a delay
    /// </summary>
    private IEnumerator CloseQuizAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); 
    
        quizPanel.SetActive(false); 
    
        SetAnswerButtonsInteractable(true); 
    }

    /// <summary>
    /// Sets the interactable state of answer buttons
    /// </summary>
    private void SetAnswerButtonsInteractable(bool state)
    {
        foreach (var button in answerButtons)
        {
            button.interactable = state;
        }
    }

    /// <summary>
    /// Starts the dialogue for the given poster content
    /// </summary>
    public void StartDialogue(PosterContent content)
    {
        currentPosterContent = content;
        currentDialogueIndex = 0;

        quizPanel.SetActive(false);
        dialoguePanel.SetActive(true);
        DisplayNextLine();
    }

    /// <summary>
    /// Displays the next line of dialogue
    /// </summary>
    public void DisplayNextLine()
    {
        if (currentPosterContent == null) return;

        if (currentDialogueIndex < currentPosterContent.dialogueLines.Length)
        {
            dialogueText.text = currentPosterContent.dialogueLines[currentDialogueIndex];
            currentDialogueIndex++;
        }
        else
        {
            dialoguePanel.SetActive(false);
            StartQuiz();
        }
    }

    /// <summary>
    /// Starts the quiz associated with the current poster content
    /// </summary>
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
                answerButtons[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Checks the selected answer and provides feedback
    /// </summary>
    void CheckAnswer(int selectedAnswerIndex)
    {
        foreach (Button btn in answerButtons)
        {
            btn.interactable = false;
        }

        if (selectedAnswerIndex == currentPosterContent.correctAnswerIndex)
        {
            questionText.text = "Correct, well done! You earned a stamp!";
        }
        else
        {
            questionText.text = "Incorrect. Try again.";
        }
    }
}