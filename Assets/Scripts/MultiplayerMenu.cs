
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerMenu : MonoBehaviour
{
    public GameObject panel;
    Image image;

    public void blurPanel(){
        image = panel.GetComponent<Image>();
        image.color = new Color32(255,255,225,100);
    }
}
