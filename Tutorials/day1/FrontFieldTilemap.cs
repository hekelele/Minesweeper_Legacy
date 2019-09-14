using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FrontFieldTilemap : MonoBehaviour
{
    private Tilemap _FrontMap;
    public int _Xcount = 9, _YCount = 9;
    public Tile[] _MarkTypes;
    public MineFieldTilemap _MineField;

    private void Awake()
    {
        _FrontMap = GetComponent<Tilemap>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3Int vp = getClickPos();
            if (isPosValid(vp))
            {
                flipTile(vp, true);
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Vector3Int vp = getClickPos();
            if (isPosValid(vp))
            {
                TileBase tb = _FrontMap.GetTile(vp);
                if (tb != null)
                {
                    if (tb == _MarkTypes[0])
                    {
                        _FrontMap.SetTile(vp, _MarkTypes[1]);
                    }
                    else if (tb == _MarkTypes[1])
                    {
                        _FrontMap.SetTile(vp, _MarkTypes[2]);
                    }
                    else
                    {
                        _FrontMap.SetTile(vp, _MarkTypes[0]);
                    }
                }
            }
        }
    }

    private Vector3Int getClickPos()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D[] co2ds = Physics2D.OverlapPointAll(mousePos);
        for(int i=0; i<co2ds.Length; i++)
        {
            if(co2ds[i].tag == "Front")
            {
                Vector3Int vp = _FrontMap.WorldToCell(mousePos);
                return vp;
            }
        }
        return new Vector3Int(-1,-1,0);
    }

    public bool isPosValid(Vector3Int vp)
    {
        if(vp.x>=0 && vp.y>=0 && vp.x<_Xcount && vp.y < _YCount)
        {
            return true;
        }
        return false;
    }


    private void flipTile(Vector3Int vp, bool first = false)
    {
        TileType tp = _MineField.getMineTileByPos(vp);

        switch (tp)
        {
            case TileType.Empty:
                _FrontMap.SetTile(vp, null);
                Vector3Int[] vps = new Vector3Int[8]
                {
                    new Vector3Int(vp.x-1,vp.y-1,0),new Vector3Int(vp.x,vp.y-1,0),new Vector3Int(vp.x+1,vp.y-1,0),new Vector3Int(vp.x-1,vp.y,0),
                    new Vector3Int(vp.x+1,vp.y,0),new Vector3Int(vp.x-1,vp.y+1,0),new Vector3Int(vp.x,vp.y+1,0),new Vector3Int(vp.x+1,vp.y+1,0)
                };

                for(int i=0; i<vps.Length; i++)
                {
                    if (isPosValid(vps[i]))
                    {
                        if (_FrontMap.GetTile(vps[i]) != null)
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
                    _FrontMap.SetTile(vp, null);
                    Debug.Log("you lose!");
                }
                break;
            case TileType.Number:
                _FrontMap.SetTile(vp, null);
                break;
            default:
                break;
        }
    }
}
