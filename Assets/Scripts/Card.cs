using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public GameObject front;       
    public GameObject back;      
    public Image frontImage;     
    public GameManager manager;  

    private bool isFlipped = false; 
    public bool isMatched = false; 

    public void SetCardImage(Sprite sprite)
    {
        frontImage.sprite = sprite;
    }

    public Sprite GetCardImage()
    {
        return frontImage.sprite;
    }

    public void OnClick()
    {
        if (isFlipped || isMatched) return;

        Flip();
        manager.OnCardClicked(this);
    }

    public void Flip()
    {
        isFlipped = true;
        front.SetActive(true);
        back.SetActive(false);
    }

    public void FlipBack()
    {
        isFlipped = false;
        front.SetActive(false);
        back.SetActive(true);
    }

    public void ResetCardVisual()
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 1;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }

        isMatched = false;
        front.SetActive(false);
        back.SetActive(true);
    }
}
