using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banner : MonoBehaviour
{
    public List<SpriteRenderer> bannerSpriteRendererList;
    public float space = 2f;
    public Vector3 offset;

    public Sprite sprite;

    //private void Start()
    //{
    //    RefreshLayout(sprite);
    //}

    public void RefreshLayout(Sprite sprite)
    {
        Debug.Log("refresh layout");
        for(int i = 0; i < bannerSpriteRendererList.Count; i++)
        {
            bannerSpriteRendererList[i].sprite = sprite;
            bannerSpriteRendererList[i].transform.localPosition = offset - new Vector3(i * ((sprite.textureRect.width / sprite.pixelsPerUnit) + space), 0, 0);
        }
    }
}
