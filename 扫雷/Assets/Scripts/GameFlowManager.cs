using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    public TileFliper _TF = null;
    public MineMaker _MM = null;

    public static GameFlowManager _Main = null;

    private void Awake()
    {
        if (_Main != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _Main = this;
        }
    }


    public Vector3Int[] getFlipArea(Vector3Int clickPos)
    {
        List<Vector3Int> vps = new List<Vector3Int>();
        List<Vector3Int> waitList = new List<Vector3Int>();
        
        if (!_MM.isMineByPos(clickPos))
        {
            waitList.Add(clickPos);
            int looptime = 0;
            while (waitList.Count > 0 && looptime < 10000)
            {
                looptime++;
                if (!_MM.isMineByPos(waitList[0]))
                {
                    if (_MM.isEmptyByPos(waitList[0]))
                    {
                        waitList.Add(new Vector3Int(waitList[0].x - 1, waitList[0].y, 0));
                        waitList.Add(new Vector3Int(waitList[0].x + 1, waitList[0].y, 0));
                        waitList.Add(new Vector3Int(waitList[0].x, waitList[0].y - 1, 0));
                        waitList.Add(new Vector3Int(waitList[0].x, waitList[0].y + 1, 0));
                        waitList.Add(new Vector3Int(waitList[0].x + 1, waitList[0].y + 1, 0));
                        waitList.Add(new Vector3Int(waitList[0].x + 1, waitList[0].y - 1, 0));
                        waitList.Add(new Vector3Int(waitList[0].x - 1, waitList[0].y + 1, 0));
                        waitList.Add(new Vector3Int(waitList[0].x - 1, waitList[0].y - 1, 0));
                        if (!vps.Contains(waitList[0]))
                        {
                            vps.Add(waitList[0]);
                        }
                    }
                    else
                    {
                        if (!vps.Contains(waitList[0]))
                        {
                            vps.Add(waitList[0]);
                        }
                    }
                }

                waitList.RemoveAt(0);
            }
        }

        return vps.ToArray();
    }
}
