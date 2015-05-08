using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class Context : MonoBehaviour
{
    public CanvasRenderer loginPanel, popupPanel;

    public static void ShowPopup(string title, string message, string left, string right, UnityAction leftAction, UnityAction rightAction)
    {
        var popup = Camera.main.GetComponent<Context>().popupPanel.GetComponent<Popup>();
        popup.textTitle.text = title;
        popup.textMessage.text = message;
        popup.textLeft.text = left;
        popup.textRight.text = right;

        popup.buttonLeft.gameObject.SetActive(leftAction != null);
        popup.buttonLeft.onClick.RemoveAllListeners();
        popup.buttonLeft.onClick.AddListener(leftAction);

        popup.buttonRight.gameObject.SetActive(rightAction != null);
        popup.buttonRight.onClick.RemoveAllListeners();
        popup.buttonRight.onClick.AddListener(rightAction);

        popup.gameObject.SetActive(true);
    }

    internal static void ClosePopup()
    {
        var popup = Camera.main.GetComponent<Context>().popupPanel.GetComponent<Popup>();

        popup.gameObject.SetActive(false);
    }
}
