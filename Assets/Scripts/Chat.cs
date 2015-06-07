using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Chat : Photon.MonoBehaviour {
    public GameObject ChatBox;
    public GameObject TextLine;

    public List<ChatMessage> Messages = new List<ChatMessage>();

    private int displayCount = 5;
    private int YSpacing = 25;
    
    private bool ChatboxNeedsUpdate = true;


    void Awake()
    {
        //AddMessage(new ChatMessage("TEST MESSAGE", 0, ChatMessage.MessageType.Info));
    }

    public void AddMessage(ChatMessage msg)
    {
        Messages.Add(msg);
        UpdateChatBox();
    }

    public void UpdateChatBox()
    {
        int j = 0;
        for (int i = Messages.Count - 1; i >= (Messages.Count - (1 + displayCount)); i--)
        {
            GameObject line = Instantiate(TextLine) as GameObject;
            line.GetComponent<Text>().text = Messages[i].Text;
            line.GetComponent<Text>().color = Color.red;
            line.GetComponent<RectTransform>().SetParent(ChatBox.transform, false);
            line.GetComponent<RectTransform>().anchoredPosition = new Vector3(8f, -6f - YSpacing * j, 0f);
            j++;
            
        }
    }

    [RPC]
    public void AddNetworkMessage(string text, int id, ChatMessage.MessageType type)
    {
        AddMessage(new ChatMessage(text, id, type));
    }

    

}
