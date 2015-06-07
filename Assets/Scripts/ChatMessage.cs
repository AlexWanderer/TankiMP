using UnityEngine;
using System.Collections;

public class ChatMessage  {
    public string Text = "";
    public int PlayerID = 0;
    public enum MessageType
    {
        Chatter,
        Info,
        Frag,
        System,
        Other
    }

    public MessageType Type = MessageType.Other;

    public float Timestamp;

    public ChatMessage(string text, int id, MessageType type)
    {
        Text = text;
        PlayerID = id;
        Type = type;
        Timestamp = Time.time;
    }

}
