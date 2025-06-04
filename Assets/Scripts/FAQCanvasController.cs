using UnityEngine;
using UnityEngine.UI;

public class FAQCanvasController : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject faqItemPrefab; // ������ � 2 ��������: ������ + �����
    public Transform contentParent;  // ��������� (��������, Content � ScrollView)

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

            // ����� �� �����, ���� ���������� ����� ���
            if (string.IsNullOrEmpty(question) || string.IsNullOrEmpty(answer))
                break;

            GameObject item = Instantiate(faqItemPrefab, contentParent);

            // ����� ������� ������ �������
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
