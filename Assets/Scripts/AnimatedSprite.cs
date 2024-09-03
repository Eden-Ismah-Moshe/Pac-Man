using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(SpriteRenderer))]
public class AnimatedSprite : MonoBehaviour
{
    public Sprite[] sprites = new Sprite[0];
    public float animationTime = 0.25f;
    public bool loop = true;
    
    private SpriteRenderer _spriteRenderer;
    private int _animationFrame;
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        InvokeRepeating(nameof(Advance), animationTime, animationTime);
    }
    
    private void OnEnable()
    {
        _spriteRenderer.enabled = true;
    }

    private void OnDisable()
    {
        _spriteRenderer.enabled = false;
    }
    
    private void Advance()
    {
        if (!_spriteRenderer.enabled)
        {
            return;
        }
        
        _animationFrame++;
        
        if (_animationFrame >= sprites.Length && loop)
        { 
            _animationFrame = 0;
        }
        
        if (_animationFrame >= 0 && _animationFrame < sprites.Length)
        { 
            _spriteRenderer.sprite = sprites[_animationFrame];
        }
    }
    
    public void Restart()
    {
        _animationFrame = -1;
        
        Advance();
        
    }
}
