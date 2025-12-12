using UnityEngine;

public class MerlionAnimator : MonoBehaviour
{
    // --- Public Properties ---
    public float entranceDuration = 2.0f; // Time in seconds for the animation

    // --- Private Variables ---
    private float elapsedTime = 0f;
    private Vector3 initialPosition; // Where the Merlion is placed by the tracker
    private Vector3 startPosition; // Slightly below initialPosition
    private Vector3 startScale = Vector3.zero;
    private Vector3 endScale = Vector3.one; // (1, 1, 1) - full size

    // A flag to ensure the entrance animation only runs once
    private bool isAnimatingEntrance = false;
    private bool isEntranceComplete = false;
    
    // Y-offset for the "swimming up" illusion
    private const float RiseOffset = 0.2f; 
    
    // --- Public Methods to Call ---

    // This function will be called by the ImageTracker when the image is found
    public void StartEntranceAnimation(Vector3 targetPosition)
    {
        if (isEntranceComplete) return; // Prevent re-animating if already done

        // 1. Set the target properties based on the tracker's position
        initialPosition = targetPosition; 
        startPosition = initialPosition - new Vector3(0, RiseOffset, 0); // Start below target

        // 2. Initial Setup: Place and scale the object to its starting point
        transform.position = startPosition;
        transform.localScale = startScale;
        
        // 3. Start the process
        elapsedTime = 0f;
        isAnimatingEntrance = true;
        gameObject.SetActive(true); // Ensure the object is visible for animation
    }

    // --- Animation Loop ---
    void Update()
    {
        if (isAnimatingEntrance)
        {
            // 1. Increment timer and calculate normalized time (t)
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / entranceDuration);
            
            // Optional: Add easing for a smoother effect (like EaseOutQuad)
            // float smooth_t = 1f - (1f - t) * (1f - t); 
            // Use 't' or 'smooth_t' in the Lerp functions below.

            // 2. Lerp the SCALE (Growing)
            transform.localScale = Vector3.Lerp(startScale, endScale, t);

            // 3. Lerp the POSITION (Rising)
            transform.position = Vector3.Lerp(startPosition, initialPosition, t);

            // 4. Check for Completion
            if (t >= 1.0f)
            {
                isAnimatingEntrance = false;
                isEntranceComplete = true;
                // Optionally start the Bobbing Idle animation here
                // StartBobbingAnimation(); 
            }
        }
    }
}