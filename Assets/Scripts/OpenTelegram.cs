using UnityEngine;
using UnityEngine.EventSystems; // Не забудьте добавить это пространство имён

public class OpenTelegram : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    // Telegram Username
    public string telegramUsername = "";

    // URL для веб-версии Telegram
    private string telegramWebUrl = "https://t.me/";

    // Размеры объекта
    private Vector3 originalScale;
    public Vector3 pressedScale = new Vector3(0.9f, 0.9f, 0.9f); // Уменьшенный размер

    private void Start()
    {
        originalScale = transform.localScale; // Сохраняем оригинальный размер
    }

    public void OnClick()
    {
        string urlToOpen;

#if UNITY_EDITOR
        // В редакторе Unity используем только веб-версию
        urlToOpen = telegramWebUrl + telegramUsername;
#elif UNITY_ANDROID
        // URL для приложения Telegram (используется только на Android)
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
        // Для других платформ используем веб-версию
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
        // Уменьшаем размер объекта
        transform.localScale = pressedScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Возвращаем размер объекта обратно
        transform.localScale = originalScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Если указатель выходит за пределы объекта, возвращаем размер
        transform.localScale = originalScale;
    }
}