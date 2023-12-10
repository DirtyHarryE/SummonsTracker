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
                SetProperties(_rectTransform.sizeDelta.x, GetDelegate(false));
                break;
            case AspectMode.HeightControlsWidth:
                SetProperties(_rectTransform.sizeDelta.y, GetDelegate(true));
                break;
        }
    }

    private void SetProperties(float currentExtent, Action<float> setExtent)
    {
        setExtent(currentExtent * _aspectRatio);
    }

    private Action<float> GetDelegate(bool width)
    {
        return delegate (float val)
        {
            if (width)
            {
                if (_applyToMin)
                {
                    _layoutElement.minWidth = val;
                }
                if (_applyToPreferred)
                {
                    _layoutElement.preferredWidth = val;
                }
                if (_applyToFlexible)
                {
                    _layoutElement.flexibleWidth = val;
                }
            }
            else
            {
                if (_applyToMin)
                {
                    _layoutElement.minHeight = val;
                }
                if (_applyToPreferred)
                {
                    _layoutElement.preferredHeight = val;
                }
                if (_applyToFlexible)
                {
                    _layoutElement.flexibleHeight = val;
                }
            }
        };
    }

    [SerializeField]
    private AspectMode _aspectMode;
    [SerializeField]
    private float _aspectRatio;
    [SerializeField]
    private bool _applyToMin = false;
    [SerializeField]
    private bool _applyToPreferred = true;
    [SerializeField]
    private bool _applyToFlexible = false;

    private RectTransform _rectTransform;
    private LayoutElement _layoutElement;
}
