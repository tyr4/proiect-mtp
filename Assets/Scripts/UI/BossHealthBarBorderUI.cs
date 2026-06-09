using UnityEngine;
using UnityEngine.UI;

public class BossHealthBarBorderUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private float speed = 5f;

    private float _hue;

    private void Update()
    {
        _hue += Time.deltaTime * speed;
        if (_hue > 1f) _hue -= 1f;
        
        Color c = Color.HSVToRGB(_hue, 1f, 1f);
        image.color = c;
    }
}
