using UnityEngine;

public class OnlyOneActiveToggle : MonoBehaviour
{
    private CustomButton _previousToggle;
    private CustomButton _currentToggle;

    public void OnClick(CustomButton current)
    {
        if (_previousToggle != current && _previousToggle != null)
        {
            _previousToggle.DeselectButton();
        }
        
        _previousToggle = current;
    }
}
