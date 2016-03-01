using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour
{
    public SpriteRenderer lowerTileRenderer;
    public SpriteRenderer middleTileRenderer;

    public void SetLowerTile(Sprite tileSprite)
    {
        lowerTileRenderer.sprite = tileSprite;
    }

    public void SetMiddleTile(Sprite tileSprite)
    {
        middleTileRenderer.sprite = tileSprite;
    }
}
