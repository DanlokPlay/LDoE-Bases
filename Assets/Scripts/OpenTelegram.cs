using UnityEngine;
using UnityEngine.EventSystems; // �� �������� �������� ��� ������������ ���

public class OpenTelegram : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    // Telegram Username
    public string telegramUsername = "";

    // URL ��� ���-������ Telegram
    private string telegramWebUrl = "https://t.me/";

    // ������� �������
    private Vector3 originalScale;
    public Vector3 pressedScale = new Vector3(0.9f, 0.9f, 0.9f); // ����������� ������

    private void Start()
    {
        originalScale = transform.localScale; // ��������� ������������ ������
    }

    public void OnClick()
    {
        string urlToOpen;

#if UNITY_EDITOR
        // � ��������� Unity ���������� ������ ���-������
        urlToOpen = telegramWebUrl + telegramUsername;
#elif UNITY_ANDROID
        // URL ��� ���������� Telegram (������������ ������ �� Android)
        string telegramAppUrl = "tg://resolve?domain=";
        if (IsAppInstalled("org.telegram.messenger"))
        {
            urlToOpen = telegramAppUrl + telegramUsername;
        }
        else
        {
            urlToOpen = telegramWebUrl + telegramUsername;
        }
#else
        // ��� ������ �������� ���������� ���-������
        urlToOpen = telegramWebUrl + telegramUsername;
#endif

        Application.OpenURL(urlToOpen);
    }

    private bool IsAppInstalled(string bundleId)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject packageManager = currentActivity.Call<AndroidJavaObject>("getPackageManager");

        try
        {
            packageManager.Call<AndroidJavaObject>("getPackageInfo", bundleId, 0);
            return true;
        }
        catch (AndroidJavaException)
        {
            return false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // ��������� ������ �������
        transform.localScale = pressedScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // ���������� ������ ������� �������
        transform.localScale = originalScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // ���� ��������� ������� �� ������� �������, ���������� ������
        transform.localScale = originalScale;
    }
}