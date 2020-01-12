using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FileSystem : MonoBehaviour
{
    [SerializeField] Text headLine = null;
    [SerializeField] FileSystemElement mapListElementPrefab = null;
    [SerializeField] GameObject scrollContent = null;
    [SerializeField] Button okButton = null;
    [SerializeField] Text okButtonText = null;
    [SerializeField] InputField mapNameField = null;
    MapEditor mapEditor;
    FileSystemElement[] mapListElements;
    string selectedMapName;

    public void Open(MapEditor parent, FileSystemMode mode)
    {
        gameObject.SetActive(true);
        mapEditor = parent;

        okButton.onClick.RemoveAllListeners();
        if (mode == FileSystemMode.Save)
        {
            headLine.text = "Save";
            okButtonText.text = "Save";
            okButton.onClick.AddListener(() => Save(mapNameField));
        }
        if (mode == FileSystemMode.Load)
        {
            headLine.text = "Load";
            okButtonText.text = "Load";
            okButton.onClick.AddListener(() => Load(mapNameField));
        }

        if(mapListElements != null)
        {
            for (int i = 0; i < mapListElements.Length; i++)
            {
                Destroy(mapListElements[i].gameObject);
            }
        }

        Object[] mapObjects = Resources.LoadAll("Map");
        mapListElements = new FileSystemElement[mapObjects.Length];
        for (int i = 0; i < mapObjects.Length; i++)
        {
            if ((TextAsset)mapObjects[i] != null)
            {
                mapListElements[i] = Instantiate(mapListElementPrefab);
                mapListElements[i].transform.SetParent(scrollContent.transform);
                mapListElements[i].Init(this, mapObjects[i].name);
            }
            else
            {
                mapListElements[i] = null;
            }
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void Save(InputField fileNameField)
    {
        //if (Resources.Load("Map/" + fileNameField.text) == null)
        {
            SaveFile(fileNameField.text); 
            Close();
        }
    }

    void SaveFile(string fileName)
    {
        FileStream f = new FileStream("Assets/Resources/Map/" + fileName + ".json", FileMode.Create, FileAccess.Write);   
        StreamWriter writer = new StreamWriter(f, System.Text.Encoding.Unicode);
        writer.WriteLine(mapEditor.GetJson());
        writer.Close();
    }

    public void Load(InputField fileNameField)
    {
        TextAsset mapAsset = Resources.Load("Map/" + fileNameField.text) as TextAsset;
        if (mapAsset != null) {
            mapEditor.LoadNewMap(mapAsset.text);
            Close();
        }
    }

    public void Select(string mapName)
    {
        for (int i = 0; i < mapListElements.Length; i++)
        {
            mapListElements[i].DeSelect();
        }
        selectedMapName = mapName;
        mapNameField.text = mapName;
    }
}
