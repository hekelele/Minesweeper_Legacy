using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FrontFieldTilemap : TilemapPlus
{
    public int _MineNumber = 10;
    public Tile[] _MarkTypes;
    public MineFieldTilemap _MineField;

    public bool flipTile(Vector3Int vp, bool first = false)
    {
        TileType tp = _MineField.getTileTypeByPos(vp);

        switch (tp)
        {
            case TileType.Empty:
                _BaseMap.SetTile(vp, null);
                Vector3Int[] vps = getNeighbours(vp, false);

                for(int i=0; i<vps.Length; i++)
                {
                    if (isPosValid(vps[i]))
                    {
                        if (_BaseMap.GetTile(vps[i]) != null)
                        {
                            flipTile(vps[i]);
                        }
                    }
                }

                //flipTile
                break;
            case TileType.Mine:
                if (first)
                {
                    _BaseMap.SetTile(vp, null);
                    return false;
                }
                break;
            case TileType.Number:
                _BaseMap.SetTile(vp, null);
                break;
            default:
                break;
        }

        return true;
    }

}
