using System;
using UnityEngine;

public class CameraFade : MonoBehaviour
{
    private float _alpha = 1;
    private Texture2D _texture;
    private bool _done;
    private float _time;

    private int _direction = 1;

    private bool _fade;

    public Action OnFadeEnd;

    public void Reset()
    {
        _fade = false;
        _done = false;
        _alpha = 1;
        _time = 0;
    }

    public void FadeIn()
    {
        Reset();
        _direction = 1;
        _fade = true;
        _alpha = 0;
    }

    public void FadeOut()
    {
        Reset();
        _direction = -1;
        _fade = true;
        _alpha = 1;
    }

    public void OnGUI()
    {
        if (_fade)
        {
            if (_done) return;
            if (_texture == null) _texture = new Texture2D(1, 1);

            _texture.SetPixel(0, 0, new Color(0, 0, 0, _alpha));
            _texture.Apply();

            if (_direction == 1)
            {
                _alpha += 1/60f;
            }
            else
            {
                _alpha -= 1 / 60f;
            }
            if (_alpha >= 1 && _direction == 1)
            {
                _done = true;
                _fade = false;
                OnFadeEnd?.Invoke();
            }
            else if(_alpha <= 0 && _direction == -1)
            {
                _done = true;
                _fade = false;
                OnFadeEnd?.Invoke();
            }

        }

        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _texture);

    }
}