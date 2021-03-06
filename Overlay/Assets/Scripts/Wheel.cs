﻿using CommandWheelOverlay.View;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[ExecuteInEditMode()]
public class Wheel : MonoBehaviour
{
    public Color bgColor = new Color(25, 140, 210, 128);
    public Color accentColor = Color.white;

    public int buttons;
    public float radious;
    public float innerRadious;
    public RectTransform separator;
    public WheelSegment segment;
    private WheelSegment[] childSegments;

    public SimplifiedWheel? Template { get; set; } = null;
    public SimplifiedWheelButton[] ButtonTemplates { get; set; } = null;

    public int Highlighted { get => highlighted; set => SetHighlighted(value); }
    public Vector2 StartVector { get; private set; }
    public float ActualInnerRadious { get; private set; }
    public float ActualRadious { get; private set; }
    private int highlighted = -1;

    // Start is called before the first frame update
    void Start()
    {
        bool fromTemplate = false;
        if (Template.HasValue && ButtonTemplates != null)
        {
            fromTemplate = true;
            buttons = Template.Value.ButtonIndices.Length;
        }

        var mask = (RectTransform)transform.Find("Mask");
        var innerSize = new Vector2(innerRadious * 2, innerRadious * 2);
        ((RectTransform)transform).sizeDelta = innerSize;
        mask.sizeDelta = innerSize;
        //transform.localScale = new Vector2(innerRadious * 2, innerRadious * 2);
        float angleDiff = 360f / buttons;
        float offset = angleDiff / 2;
        childSegments = new WheelSegment[buttons];
        for (int i = 0; i < buttons; i++)
        {
            float angle = angleDiff * i + offset;
            var seg = Instantiate(segment, transform.position, Quaternion.Euler(0, 0, angle), mask);
            childSegments[i] = seg;
            seg.degrees = angleDiff;
            seg.index = i;
            seg.radious = radious;
            seg.innerRadious = innerRadious;
            if (fromTemplate)
            {
                int buttonIndex = Template.Value.ButtonIndices[i];
                seg.ButtonTemplate = ButtonTemplates[buttonIndex];
                seg.ButtonIndex = buttonIndex;
                gameObject.SetActive(false);
                var template = Template.Value;
                bgColor = colorFromArray(template.BgColor);
                accentColor = colorFromArray(template.AccentColor);
            }
            seg.bgColor = bgColor;
            seg.accentColor = accentColor;
        }
        for (int i = 0; i < buttons; i++)
        {
            float angle = angleDiff * i - offset;
            var sep = Instantiate(separator, transform.position, Quaternion.Euler(0, 0, angle), mask);
            var sepTransform = ((RectTransform)sep.transform);
            var sepSize = sepTransform.sizeDelta;
            sepSize.y = radious;
            sepTransform.sizeDelta = sepSize;
            sep.transform.localPosition += sep.up * (sep.rect.height / 2);

            sep.GetComponent<Image>().color = accentColor;
        }
        StartVector = childSegments[buttons - 1].transform.up;
        ActualInnerRadious = ((RectTransform)transform).sizeDelta.y * innerRadious;
        ActualRadious =  ((RectTransform)transform).sizeDelta.y * radious;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Color colorFromArray(int[] array)
    {
        return new Color(array[0] / 255f, array[1] / 255f, array[2] / 255f, array[3] / 255f);
    }

    private void SetHighlighted(int value)
    {
        if (value == highlighted) return;
        if (highlighted >= 0)
        {
            childSegments[highlighted].Highlighted = false;
        }
        if (value >= 0)
        {
            childSegments[value].Highlighted = true; ;
        }
        highlighted = value;
    }

    public int GetHighlightedButton()
    {
        return childSegments[highlighted].ButtonIndex;
    }

    public void ForceUnhighlightAll()
    {
        highlighted = -1;
        foreach (WheelSegment segment in childSegments)
        {
            segment.ForceUnhighlight();
        }
    }

    public void FadeIn(CursorHighlight cursorHighlight)
    {
        gameObject.SetActive(true);

        var group = GetComponent<CanvasGroup>();
        gameObject.LeanCancel();
        gameObject.LeanMoveLocalX(0, 0.2f);
        group.LeanAlpha(1, 0.2f).setOnComplete(() => cursorHighlight.wheel = this);
    }

    public void FadeOut(int direction, CursorHighlight cursorHighlight)
    {
        direction = Mathf.Clamp(direction, -1, 1);

        var group = GetComponent<CanvasGroup>();
        gameObject.LeanCancel();
        gameObject.LeanMoveLocalX(50 * direction, 0.2f);
        group.LeanAlpha(0, 0.2f).setOnComplete(() => gameObject.SetActive(false));

        cursorHighlight.wheel = null;
    }

    public void Show()
    {
        SetAlpha(1);
    }

    public void Hide()
    {
        SetAlpha(0);
    }

    private void SetAlpha(float alpha)
    {
        gameObject.LeanCancel();
        GetComponent<CanvasGroup>().alpha = alpha;
        gameObject.SetActive(alpha > 0);
    }
}
