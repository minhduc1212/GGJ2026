using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Image characterImage;
    
    [Header("Choice UI")]
    public Transform choiceContainer;   // Kéo cái ChoiceContainer vào đây
    public GameObject choiceButtonPrefab; // Kéo cái Button Prefab vào đây

    [Header("Data")]
    public Conversation currentConversation; // Kéo đoạn hội thoại BẮT ĐẦU vào đây (VD: Intro)
    
    // Biến lưu trạng thái
    private int index = 0;
    private bool isTyping = false;
    private int currentTrust = 50; // Giả sử Trust bắt đầu là 50

    void Start()
    {
        StartDialogue(currentConversation);
    }

    public void StartDialogue(Conversation conversation)
    {
        currentConversation = conversation;
        index = 0;
        DisplayNextSentence();
    }

    // Hàm này được gọi khi bấm chuột/Space
    public void OnClickNext()
    {
        // Nếu đang hiện lựa chọn thì KHÔNG cho bấm next để qua câu
        if (choiceContainer.childCount > 0) return; 

        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = currentConversation.lines[index - 1].sentence;
            isTyping = false;
        }
        else
        {
            DisplayNextSentence();
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            OnClickNext();
        }
    }

    public void DisplayNextSentence()
    {
        // Nếu hết câu thoại trong đoạn này -> Kết thúc hoặc làm gì đó
        if (index >= currentConversation.lines.Count)
        {
            Debug.Log("Hết đoạn hội thoại này.");
            return;
        }

        DialogueLine currentLine = currentConversation.lines[index];

        // 1. Hiển thị UI cơ bản
        nameText.text = currentLine.characterName;
        if(currentLine.characterSprite != null) characterImage.sprite = currentLine.characterSprite;
        
        // Glitch Logic (Giữ nguyên của bạn)
        nameText.color = currentLine.isGlitch ? Color.red : Color.white;

        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentLine.sentence));

        // 2. KIỂM TRA LỰA CHỌN
        // Nếu câu thoại này có các lựa chọn đi kèm -> Hiển thị nút
        if (currentLine.choices != null && currentLine.choices.Count > 0)
        {
            ShowChoices(currentLine.choices);
        }

        index++;
    }

    void ShowChoices(List<Choice> choices)
    {
        // Xóa các nút cũ nếu còn sót
        foreach (Transform child in choiceContainer) Destroy(child.gameObject);

        // Tạo nút mới
        foreach (Choice choice in choices)
        {
            GameObject btn = Instantiate(choiceButtonPrefab, choiceContainer);
            // Lấy text trong button để sửa nội dung
            btn.GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceText;
            
            // Gán sự kiện OnClick cho nút
            btn.GetComponent<Button>().onClick.AddListener(() => OnChoiceSelected(choice));
        }
    }

    void OnChoiceSelected(Choice choice)
    {
        // 1. Cập nhật Trust
        currentTrust += choice.trustChange;
        Debug.Log("Trust hiện tại: " + currentTrust);

        // 2. Xóa các nút
        foreach (Transform child in choiceContainer) Destroy(child.gameObject);

        // 3. Chuyển sang đoạn hội thoại tiếp theo
        if (choice.nextConversation != null)
        {
            StartDialogue(choice.nextConversation);
        }
        else
        {
            // Nếu không có đoạn tiếp theo -> Đóng hội thoại hoặc Game Over
            Debug.Log("Kết thúc game/phân cảnh");
        }
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null; // Tốc độ nhanh nhất (mỗi frame 1 chữ)
        }
        isTyping = false;
    }
}