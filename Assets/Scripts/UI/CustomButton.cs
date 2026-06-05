using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CustomButton : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler
{

    public enum ButtonUIType
    {
        Overlay,
        SpriteChange
    }

    public enum ButtonType
    {
        Toggle,
        Button
    }
    
    [Header("Button Mode")]
    [SerializeField] public ButtonUIType buttonUIType = ButtonUIType.Overlay;
    [SerializeField] public ButtonType buttonType = ButtonType.Button;
    
    [Header("Function Calls")] 
    [SerializeField] public UnityEvent<GameObject> onClickFunctions;
    
    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoveredColor = new Color(0.871f, 0.871f, 0.871f, 1);
    [SerializeField] private Color pressedColor = new Color(0.78f, 0.78f, 0.78f, 1);
    [SerializeField] private Color selectedColor = new Color(0.729f, 0.729f, 0.729f, 1);
    [SerializeField] private Color disabledColor = new Color(0.35f, 0.35f, 0.35f, 1);
    
    [Header("Tint overrides")]
    [SerializeField] private bool isColorTint;
    [SerializeField] private bool tintClickOverride; // disable tint only for clicks 
    
    [Header("Sprites")]
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite hoveredSprite;
    [SerializeField] private Sprite pressedSprite;
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Sprite disabledSprite;

    
    // use color instead of sprite and vice versa
    [Header("Button Overrides")] 
    [SerializeField] private bool hoverOverride;
    [SerializeField] private bool selectedOverride;
    [SerializeField] private bool isDisabled;
    [SerializeField] private bool toggleReleaseOverride; // dont execute click functions on toggle release
    
    [Header("Fade")] 
    [SerializeField] private float duration = 0.1f;

    private Image _targetMain;

    private List<UnityAction<GameObject>> _cachedActions = new();
    
    private bool _isSelected;
    private bool _isHovered;
    private bool _isPressed;
    private bool _deselectedThisPress;

    private Color _initialColorCopy;

    public enum ButtonState
    {
        Normal,
        Hovered,
        Pressed,
        Selected,
        Disabled
    }

    private Coroutine _currentCoroutine;

    public event Action<CustomButton> OnClickEvent;
    
    public void Awake()
    {
        _targetMain = GetComponent<Image>();
        
        normalColor = _targetMain.color;
        _initialColorCopy = new Color(normalColor.r, normalColor.g, normalColor.b);
        
        Debug.Log("am rulat");
        RefreshVisual();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDisabled) return;
        
        _isHovered = true;
        RefreshVisual();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isDisabled) return;
        
        _isHovered = false;
        _deselectedThisPress = true;
        RefreshVisual();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isDisabled) return;
        
        _isPressed = true;
        RefreshVisual();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isDisabled) return;
        
        _isPressed = false;
        _isHovered = false;
        
        // check if its still hovering over the button
        if (eventData.pointerCurrentRaycast.gameObject == gameObject)
        {
            if (buttonType == ButtonType.Toggle)
                _isSelected = !_isSelected;
            else if (buttonType == ButtonType.Button)
                _isSelected = true;
        }
        
        RefreshVisual();
        ExecuteClickFunctions();
        AudioEvents.RequestSFX(AudioManager.Sounds.buttonClick);

        if (buttonType == ButtonType.Button)
        {        
            _isSelected = false;
            _deselectedThisPress = true;
            RefreshVisual();
        }
    }
    
    public ButtonState GetButtonState()
    {
        if (isDisabled)
            return ButtonState.Disabled;
        
        // logic for selecting
        if (_isSelected && _isHovered && !_isPressed)
        {
            return ButtonState.Hovered;
        }
        
        if (_isSelected && !_isHovered && !_isPressed)
        {
            return ButtonState.Selected;
        }
        
        if (_isPressed)
        {
            return ButtonState.Pressed;
        }
        if (_isHovered)
        {
            return ButtonState.Hovered;
        }

        return ButtonState.Normal;
    }

    private void RefreshVisual()
    {
        var state = GetButtonState();
        if (isDisabled)
        {
            if (buttonUIType == ButtonUIType.SpriteChange && !disabledSprite) return;
            
            Color newColor = GetColor(state);
            ChangeButtonColor(newColor);

            return;
        }

        switch (buttonUIType)
        {
            case ButtonUIType.Overlay:
            {
                Color newColor = GetColor(state);
                ChangeButtonColor(newColor);
                
                break;
            }
            case ButtonUIType.SpriteChange:
            {
                // Debug.Log(state + " " + _deselectedThisPress + " " + buttonType);
                if ((hoverOverride && (state == ButtonState.Hovered || _deselectedThisPress)) || (selectedOverride && state == ButtonState.Selected))
                {
                    Color newColor = GetColor(state);
                    ChangeButtonColor(newColor);
                    
                    break;
                }
                
                Sprite newSprite = GetSprite(state);
                ChangeButtonSprite(newSprite);
                
                break;
            }
        }

        _deselectedThisPress = false;
    }

    private Sprite GetSprite(ButtonState state)
    {
        return state switch
        {
            ButtonState.Normal   => normalSprite,
            ButtonState.Hovered  => hoveredSprite,
            ButtonState.Pressed  => pressedSprite,
            ButtonState.Selected => selectedSprite,
            ButtonState.Disabled => disabledSprite,
            _ => normalSprite
        };
    }
    
    private Color GetColor(ButtonState state)
    {
        return state switch
        {
            ButtonState.Normal   => normalColor,
            ButtonState.Hovered  => hoveredColor,
            ButtonState.Pressed  => pressedColor,
            ButtonState.Selected => selectedColor,
            ButtonState.Disabled => disabledColor,
            _ => normalColor
        };
    }
    
    private void ChangeButtonColor(Color newColor)
    {
        if (_currentCoroutine != null)
            StopCoroutine(_currentCoroutine);

        var state = GetButtonState();
        
        // override any behaviour for hover with this
        if (isColorTint && state == ButtonState.Hovered)
        {
            _currentCoroutine = FadeAnimation.FadeTint(_targetMain, newColor, duration, this);
        }
        
        else if (!isColorTint || (tintClickOverride && state == ButtonState.Selected))
        {
            _currentCoroutine = FadeAnimation.FadeColorUI2(_targetMain, _targetMain.color, newColor, duration, this,
                () => { _currentCoroutine = null; });
        }
        
        else
        {
            if (GetButtonState() == ButtonState.Normal)
            {
                _currentCoroutine = FadeAnimation.FadeTintOut(_targetMain, newColor, duration, this);
                
            }
            else
            {
                Debug.Log(GetButtonState());
                _currentCoroutine = FadeAnimation.FadeTint(_targetMain, newColor, duration, this);
            }
        }
    }

    public void ChangeButtonSprite(Sprite newSprite)
    {
        _targetMain.sprite = newSprite;
    }

    private void ExecuteClickFunctions()
    {
        if ((!_isSelected || isDisabled) && buttonType != ButtonType.Toggle) return;
        if (buttonType == ButtonType.Toggle && GetButtonState() == ButtonState.Normal && toggleReleaseOverride) return;
        
        onClickFunctions.Invoke(gameObject);
        OnClickEvent?.Invoke(this);
    }

    public void AddEventListeners(params UnityAction<GameObject>[] actions)
    {
        foreach (var action in actions)
        {
            onClickFunctions.AddListener(action);
            _cachedActions.Add(action);
        }
    }

    public void RemoveAllEventListeners()
    {
        onClickFunctions.RemoveAllListeners();
        _cachedActions.Clear();
    }

    public void EnableButton()
    {
        isDisabled = false;
        RefreshVisual();
    }

    public void DisableButton()
    {
        isDisabled = true;
        RefreshVisual();
    }
    
    public void SelectButton()
    {
        _isSelected = true;
        RefreshVisual();
    }

    public void DeselectButton()
    {
        _isSelected = false;
        RefreshVisual();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(CustomButton))]
public class CustomButtonTestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var typeProp = serializedObject.FindProperty("buttonUIType");
        var type = (CustomButton.ButtonUIType)typeProp.enumValueIndex;
        var prop = serializedObject.GetIterator();
        
        prop.NextVisible(true); // skip "m_Script"
        while (prop.NextVisible(false))
        {
            // Conditional skip based on type
            if (type == CustomButton.ButtonUIType.Overlay &&
                (prop.name == "normalSprite" || prop.name == "hoveredSprite" || prop.name == "pressedSprite" || prop.name == "selectedSprite" || prop.name == "disabledSprite")) continue;
    
            if (type == CustomButton.ButtonUIType.SpriteChange &&
                (prop.name is "normalColor" or "hoveredColor" || prop.name == "pressedColor" || prop.name == "selectedColor" || prop.name == "disabledColor")) continue;

            EditorGUILayout.PropertyField(prop, true);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif