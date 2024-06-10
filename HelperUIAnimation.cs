using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperUIAnimation : MonoBehaviour
{
    //Effect Types Select from Dropdown in Inspector
    public enum SlideDirection { None, FromTop, FromBottom, FromLeft, FromRight, ToTop, ToBottom, ToLeft, ToRight }//Slide Type
    public enum FadeEffect { None, FadeIn, FadeOut } // Fade Type
    public enum Affecting { Text, Image }//Select in Inspector which type is  the parent of this script
    
    //Assign in Inspector aka choose the effects
    public SlideDirection slideDirection = SlideDirection.None; //Select in Inspector which if you want slide
    public FadeEffect fadeEffect = FadeEffect.None; //Select in Inspector if you want fade
    public Affecting affecting = Affecting.Text; //Select in Inspector which type is  the parent of this script

    //Effects Time and wait
    public float slideDuration = 1.0f; //How long it takes for the slide to complete
    public float fadeDuration = 1.0f; //How long it takes for fade to complete
    public float waitSlide = 0.0f; //How long since active to activate slide
    public float waitFade = 0.0f;  //How long since ACTIVE to activate fade

    private RectTransform rectTransform;// Reference to the component attributed on Start
    private Vector2 originalPosition; // Reference to the original position of the UI

    void Start()
    {
        //Get Component and  Original Position
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;

        //Coroutine animation so Yields work smoothly
        StartCoroutine(AnimateUI()); // Start Animating
    }

    IEnumerator AnimateUI()
    {        
        // if Slide is selected from Dropdown
        if (slideDirection != SlideDirection.None)
        {
            yield return new WaitForSeconds(waitSlide); // wait to animate
            Vector2 offscreenPosition = CalculateOffscreenPosition(slideDirection);
            float elapsedTime = 0f;

            while (elapsedTime < slideDuration)
            {
                rectTransform.anchoredPosition = Vector2.Lerp(offscreenPosition, originalPosition, elapsedTime / slideDuration);
                elapsedTime += Time.deltaTime;
                yield return null; // wait for next frame
            }

            rectTransform.anchoredPosition = originalPosition; // Ensure precise final position
        }

        // if  fade is selected from Dropdown
        if (fadeEffect != FadeEffect.None)
        {
            // you may  want to change logic here  to handle alphas and set absolute values
            float startAlpha = (fadeEffect == FadeEffect.FadeIn) ? 0f : 1f;
            float targetAlpha = 1f - startAlpha;  // Set Alpha to 0 if its 
            float elapsedTime = 0f;
            //Logic to Handle Fade using Lerp to go from Start to target Alphas taking into account the duratiion
            while (elapsedTime < fadeDuration)
            {
                yield return new WaitForSeconds(waitFade); // wait to animate
                float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
                
                if (affecting == Affecting.Text) // it parent is Text  i think it only works with legacy tho
                {
                    GetComponent<UnityEngine.UI.Text>().color = new Color(1f, 1f, 1f, newAlpha); 
                }
                else if (affecting == Affecting.Image) // if parent is Ui,image
                {
                    GetComponent<UnityEngine.UI.Image>().color = new Color(1f, 1f, 1f, newAlpha);
                }

                elapsedTime += Time.deltaTime;// Updating Time passed since the  Lerp Started
                yield return null;
            }
        }
    }

    // Calculating Screen Offsets (pretty much self Explanatory)
    Vector2 CalculateOffscreenPosition(SlideDirection direction)
    {
        switch (direction)
        {
            case SlideDirection.FromTop:
            case SlideDirection.ToTop:
                return new Vector2(originalPosition.x, originalPosition.y + rectTransform.rect.height);
            case SlideDirection.FromBottom:
            case SlideDirection.ToBottom:
                return new Vector2(originalPosition.x, originalPosition.y - rectTransform.rect.height);
            case SlideDirection.FromLeft:
            case SlideDirection.ToLeft:
                return new Vector2(originalPosition.x - rectTransform.rect.width, originalPosition.y);
            case SlideDirection.FromRight:
            case SlideDirection.ToRight:
                return new Vector2(originalPosition.x + rectTransform.rect.width, originalPosition.y);
            default:
                return originalPosition; 
        }
    }
}
