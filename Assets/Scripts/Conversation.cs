using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Choice // Định nghĩa một lựa chọn
{
    public string choiceText;       // Nội dung nút bấm (VD: "Cô có ổn không?")
    public Conversation nextConversation; // Nếu chọn cái này thì nhảy sang đoạn hội thoại nào?
    public int trustChange;         // Thay đổi chỉ số Trust (VD: +10 hoặc -10)
}

[System.Serializable]
public class DialogueLine // Định nghĩa một câu thoại
{
    public string characterName;
    [TextArea(3, 10)] public string sentence;
    public Sprite characterSprite;
    public bool isGlitch;
    
    public List<Choice> choices; 
}

[CreateAssetMenu(fileName = "NewConversation", menuName = "Dialogue/Conversation")]
public class Conversation : ScriptableObject
{
    public List<DialogueLine> lines; // Danh sách các câu thoại trong đoạn này
}