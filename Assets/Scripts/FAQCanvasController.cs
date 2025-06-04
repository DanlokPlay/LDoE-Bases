using UnityEngine;
using UnityEngine.UI;

public class FAQCanvasController : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject faqItemPrefab; // Префаб с 2 текстами: вопрос + ответ
    public Transform contentParent;  // Контейнер (например, Content в ScrollView)

    void Start()
    {
        LoadFAQItems();
    }

    void LoadFAQItems()
    {
        int index = 1;

        while (true)
        {
            string questionKey = $"faq_question_{index}";
            string answerKey = $"faq_answer_{index}";

            string question = LocalizationManager.GetText(questionKey);
            string answer = LocalizationManager.GetText(answerKey);

            // Выход из цикла, если следующего блока нет
            if (string.IsNullOrEmpty(question) || string.IsNullOrEmpty(answer))
                break;

            GameObject item = Instantiate(faqItemPrefab, contentParent);

            // Поиск текстов внутри префаба
            Text[] texts = item.GetComponentsInChildren<Text>();
            if (texts.Length >= 2)
            {
                texts[0].text = $"<b>{question}</b>";
                texts[1].text = answer;
            }

            index++;
        }
    }
}
