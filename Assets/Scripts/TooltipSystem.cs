using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    public static TooltipSystem current;

    public Tooltip tooltip;

    // Start is called before the first frame update
    void Awake()
    {
        current = this;
    }

    public static void Show(string content,string header = "",string cost = "")
    {
        current.tooltip.SetText(content, header,cost);
        current.tooltip.gameObject.SetActive(true);
    }

    public static void Hide()
    {
        current.tooltip.gameObject.SetActive(false);
    }
}
