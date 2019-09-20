using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapPlus : MonoBehaviour
{
    public int _Xcount = 9, _YCount = 9;
    protected Tilemap _BaseMap;
    public Tile _BasicTile;

    private void Awake()
    {
        _BaseMap = GetComponent<Tilemap>();
    }

    public void setSize(int xx, int yy)
    {
        _Xcount = xx;
        _YCount = yy;
    }

    public bool isPosValid(Vector3Int vp)
    {
        if (vp.x >= 0 && vp.y >= 0 && vp.x < _Xcount && vp.y < _YCount)
        {
            return true;
        }
        return false;
    }


    public void clearMineFiled()
    {
        _BaseMap.ClearAllTiles();
        for (int m = 0; m < _Xcount; m++)
        {
            for (int n = 0; n < _YCount; n++)
            {
                _BaseMap.SetTile(new Vector3Int(m, n, 0), _BasicTile);
            }
        }
    }

   
    public Vector3Int[] getNeighbours(Vector3Int vp, bool containSelf = false)
    {
        Vector3Int[] vps = new Vector3Int[8]
                {
                    new Vector3Int(vp.x-1,vp.y-1,0),new Vector3Int(vp.x,vp.y-1,0),new Vector3Int(vp.x+1,vp.y-1,0),new Vector3Int(vp.x-1,vp.y,0),
                    new Vector3Int(vp.x+1,vp.y,0),new Vector3Int(vp.x-1,vp.y+1,0),new Vector3Int(vp.x,vp.y+1,0),new Vector3Int(vp.x+1,vp.y+1,0)
                };

        if (!containSelf)
        {
            return vps;
        }

        List<Vector3Int> vpsl = new List<Vector3Int>(vps) {
            vp
        };

        return vpsl.ToArray();
    }

    public Vector3Int WorldToCell(Vector3 vp)
    {
        return _BaseMap.WorldToCell(vp);
    }

    public TileBase GetTile(Vector3Int vp)
    {
        return _BaseMap.GetTile(vp);
    }

    public void SetTile(Vector3Int vp, Tile t)
    {
        _BaseMap.SetTile(vp, t);
    }

    public int getTileNum()
    {
        int count = 0;
        for (int m = 0; m < _Xcount; m++)
        {
            for (int n = 0; n < _YCount; n++)
            {
                count += _BaseMap.GetTile(new Vector3Int(m, n, 0)) == null ? 0 : 1;
            }
        }
        return count;
    }

    public int getTileTypeNum(Tile t)
    {
        int count = 0;
        for (int m = 0; m < _Xcount; m++)
        {
            for (int n = 0; n < _YCount; n++)
            {
                count += _BaseMap.GetTile(new Vector3Int(m, n, 0)) == t ? 1 : 0;
            }
        }
        return count;
    }
}
