using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    [SerializeField] private Button m_QuitButton;

    public Action OnQuitButtonClicked;
    public Action OnOutSideClicked;

    private int xmin, xmax, ymin, ymax;

    private void Start()
    {
        m_QuitButton.onClick.AddListener(() =>
        {
            OnQuitButtonClicked?.Invoke();
        });

        // get the object size
        var rectTransform = GetComponent<RectTransform>();
        var sizeDelta = rectTransform.sizeDelta;
        var width = sizeDelta.x;
        var height = sizeDelta.y;

        var position = rectTransform.position;
        var x = position.x;
        var y = position.y;

        // calculate the min and max position
        xmin = (int)(x - width / 2);
        xmax = (int)(x + width / 2);
        ymin = (int)(y - height / 2);
        ymax = (int)(y + height / 2);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mousePosition = Input.mousePosition;
            if (mousePosition.x < xmin || mousePosition.x > xmax || mousePosition.y < ymin || mousePosition.y > ymax)
            {
                OnOutSideClicked?.Invoke();
            }
        }
    }
}
