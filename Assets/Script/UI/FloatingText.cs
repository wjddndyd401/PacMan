using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
    [SerializeField] float floatingSpeed = 5f;
    [SerializeField] float duration = 3f;
    Text text;

    public void Print(string _text)
    {
        text = GetComponent<Text>();
        text.text = _text;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = transform.position;
        position.y += floatingSpeed * Time.deltaTime;
        transform.position = position;
        duration -= Time.deltaTime;
        if(duration <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
