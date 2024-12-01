using UnityEngine;
using UnityEngine.UI;

public class ButtonActionHandler : MonoBehaviour
{
    public Sprite newButtonSprite; // Assign the new sprite in the Inspector
    public GameObject animationObject; // Assign the GameObject with the animation
    public Image panelBackground; // Assign the Panel’s Image Component

    private Button button;

    private void Start()
    {
        // Get the button component
        button = GetComponent<Button>();

        // Add the click event listener
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }

    private void OnButtonClick()
    {
        // Change the Button Sprite to the Glowing one
        if (newButtonSprite != null)
        {
            button.image.sprite = newButtonSprite;
        }

        // Play the Animation
        if (animationObject != null)
        {
            Animator animator = animationObject.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("TRStartAnimation"); // Trigger the animation
            }
        }

        // Remove the Panel Background
        if (panelBackground != null)
        {
            panelBackground.enabled = false;
        }
    }
}