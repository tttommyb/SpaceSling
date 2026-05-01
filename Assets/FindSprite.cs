using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Note: Use UnityEngine.UI for Image components, not UIElements

public class FindSprite : MonoBehaviour
{
    // 1. Create a serializable helper class
    [Serializable]
    public struct IconMapping
    {
        public string name;
        public Sprite sprite;
        public Material material;
    }

    // 2. Use a List in the inspector instead of a Dictionary
    [SerializeField] private List<IconMapping> iconList = new List<IconMapping>();

    private Image iconImage;
    private bool spriteSet = false;

    void OnEnable()
    {
        iconImage = GetComponent<Image>();

        // 3. Set the sprite immediately in Start instead of checking every frame in Update
        ApplySpriteByTag();
    }

    void ApplySpriteByTag()
    {
        if (spriteSet || iconImage == null) return;

        foreach (var mapping in iconList)
        {
            if (mapping.name == tag)
            {
                iconImage.sprite = mapping.sprite;
                spriteSet = true;
                iconImage.material = mapping.material;
                
                break;
            }
        }

       
    }
}