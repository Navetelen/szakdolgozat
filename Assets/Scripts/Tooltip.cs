using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tooltip : MonoBehaviour
{

    public TextMeshProUGUI headerField,contentField,costField;

    public LayoutElement layoutElement;

    public RectTransform rectTransform;

    private void Awake(){
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetText(string content, string header = "", string cost = ""){
        if(string.IsNullOrEmpty(header) || string.IsNullOrEmpty(cost)){
            headerField.gameObject.SetActive(false);
            costField.gameObject.SetActive(false);
        }else{
            headerField.gameObject.SetActive(true);
            headerField.text = header;

            costField.gameObject.SetActive(true);
            costField.text = "Költség: " + cost;
        }

        contentField.text = content;
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        Vector2 position = Input.mousePosition;

        float pivotX = position.x / Screen.width;
        float pivotY = position.y / Screen.height;


        rectTransform.pivot = new Vector2(pivotX, pivotY);
        transform.position = position;
    }
}
