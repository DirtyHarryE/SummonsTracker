using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LayoutElement))]
[ExecuteInEditMode]
public class LayoutAspectRatioFitter : MonoBehaviour
{
    private enum AspectMode
    {
        None,
        WidthControlsHeight,
        HeightControlsWidth,
    }
    private void Awake()
    {
        _rectTransform = this.GetComponent<RectTransform>();
        _layoutElement = this.GetComponent<LayoutElement>();
    }

    private void Update()
    {
        switch (_aspectMode)
        {
            case AspectMode.None:
                break;
            case AspectMode.WidthControlsHeight:
                SetProperties(_rectTransform.sizeDelta.x, y => _layoutElement.preferredHeight = y);
                break;
            case AspectMode.HeightControlsWidth:
                SetProperties(_rectTransform.sizeDelta.y, x => _layoutElement.preferredWidth = x);
                break;
        }
    }

    private void SetProperties(float currentExtent, Action<float> setExtent)
    {
        setExtent(currentExtent * _aspectRatio);
    }

    [SerializeField]
    private AspectMode _aspectMode;
    [SerializeField]
    private float _aspectRatio;

    private RectTransform _rectTransform;
    private LayoutElement _layoutElement;
}
