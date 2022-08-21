using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnnotationManager : MonoBehaviour
{
    public TextMeshProUGUI TitleTextMesh;
    public TextMeshProUGUI ContentTextMesh;

    public void SetTitle(string title)
    {
        TitleTextMesh.text = title;
    }

    public string GetTitle()
    {
        return TitleTextMesh.text;
    }

    public void SetContent(string content)
    {
        ContentTextMesh.text = content;
    }

    public string GetContent()
    {
        return ContentTextMesh.text;
    }
}
