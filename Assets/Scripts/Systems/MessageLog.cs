using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MessageLog
{
    private readonly int maxLines = 15;
    private readonly Queue<string> lines;
    private ScrollRect scroller;
    private Transform content;

    public MessageLog()
    {
        lines = new Queue<string>();
    }

    public void AddLog(string message)
    {
        lines.Enqueue(message);

        if (lines.Count > maxLines)
            lines.Dequeue();

        if(scroller != null)
            scroller.verticalNormalizedPosition = 0;
    }

    public void ClearLog()
    {
        lines.Clear();

        for (int t = 0; t < content.childCount; t++)
        {
            GameObject.Destroy(content.GetChild(t).gameObject);
        }
    }

    public void Draw(Transform logContent, ScrollRect scroll)
    {
        if(content == null)
            content = logContent;

        scroller = scroll;

        for (int t = 0; t < logContent.childCount; t++)
        {
            GameObject.Destroy(logContent.GetChild(t).gameObject);
        }

        string[] texts = lines.ToArray();

        for (int i = 0; i < texts.Length; i++)
        {
            GameObject newMessage = GameObject.Instantiate(Res.Instance.MessageLogText);
            newMessage.transform.SetParent(logContent);
            newMessage.GetComponent<Text>().text = texts[i];
            scroller.verticalNormalizedPosition = 0;

        }


    }
}

