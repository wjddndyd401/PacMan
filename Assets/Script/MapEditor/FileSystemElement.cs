using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FileSystemElement : MonoBehaviour, IPointerClickHandler
{
    Text text;
    FileSystem fileSystem;
    Color originalColor;

    public void Init(FileSystem system, string mapName)
    {
        fileSystem = system;
        text = GetComponent<Text>();
        text.text = mapName;
        originalColor = text.color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        fileSystem.Select(text.text);
        if (text != null)
        {
            text.color = Color.blue;
        }
    }

    public void DeSelect()
    {
        if (text != null)
        {
            text.color = originalColor;
        }
    }
}
