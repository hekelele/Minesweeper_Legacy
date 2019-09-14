using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public enum TileType
{
    Mine,
    Empty,
    Number,
    NONE
}

public class MineFieldTilemap : MonoBehaviour
{
    public Tile[] _SafeTileTypes;
    public Tile _MineTile;
    private Tilemap _BackMap;

    private void Awake()
    {
        _BackMap = GetComponent<Tilemap>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    public TileType getMineTileByPos(Vector3Int pos)
    {
        TileBase tb = _BackMap.GetTile(pos);
        if (tb != null)
        {
            if (tb == _MineTile)
            {
                return TileType.Mine;
            }
            else if (tb == _SafeTileTypes[0])
            {
                return TileType.Empty;
            }
            else
            {
                return TileType.Number;
            }
        }

        return TileType.NONE;
    }
}
