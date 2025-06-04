using UnityEngine;
using UnityEngine.UI;

public class AboutCanvasController : MonoBehaviour
{
    public Text botInfo;
    public Text chat;
    public Text site;
    public Text donationText;

    void OnEnable()
    {
        botInfo.text = LocalizationManager.GetText("botInfo");
        chat.text = LocalizationManager.GetText("chat");
        site.text = LocalizationManager.GetText("site");
        donationText.text = LocalizationManager.GetText("donationText");
    }
}
