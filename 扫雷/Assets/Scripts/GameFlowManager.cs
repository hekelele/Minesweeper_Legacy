using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameFlowManager : MonoBehaviour
{
    // TileMap
    public TileFliper _TF = null;
    public MineMaker _MM = null;

    //TileMap底部背景
    public Transform _GridTransform;
    public Transform _BorderTransform;
    public Transform _BackgroundTransform;

    // Managers
    public SEPlayer _SE;                            // 音效
    public GameTimer _Timer;                        // 定时器

    // 游戏状态
    public enum GameState
    {
        WAIT,
        RUNNING,
        END,
        WIN,
        CHANGE
    }
    private GameState _State = GameState.WAIT;  // 当前游戏状态


    // UI元素
    public Text _MineNumText;           // 地雷数字UI
    public GameObject _DifficultyGO;    // 难度选单
    public Transform _SpriteFather;     // 结算时 标记的根节点

    // 三种标记Prefab
    public GameObject _MineClickPrefab;     // 失败时：死亡格
    public GameObject _MineMarkedPrefab;    // 失败时：猜了的地雷
    public GameObject _MineCorrectPrefab;   // 成功时：所有地雷

    // 雷场基础数值
    public int _XCount = 9, _YCount = 9;
    public int _MineCount = 10;
    private int _MineCountMark = 0;     // 玩家标记出的地雷数量（>=0，可以比设定的地雷数目多）
    
    // 缓存数据
    public  Vector3 _FinalClickTilePos;     // 记录最后一次点击的位置（用于失败结局）
    private GameState _StateCache;          // 记录上一个状态（以判断之前是WAIT还是RUNNING）
    private bool _CoolDown = true;          // 用来吃掉转换难度按钮抬起后的屏幕点击

    public float _TriggerIntervalPerUnit = 0.3f;    // 单位距离引爆时间差
    public float _TriggerIntervalPadding = 0.1f;
    public float _StereoPanMult = 0.2f;
    private IEnumerator _ExplodeLink;

    // 开始游戏
    private void Start()
    {
        _ExplodeLink = triggerAllMines();
        newGame(0);
    }

    private void Update()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (_State == GameState.WAIT)
        {
            // 如果在等候状态，需要吃掉改变难度后的一次抬起
            if (!_CoolDown)
            {
                _CoolDown = true;
                return;
            }
            if (Input.GetMouseButtonUp(0))
            {
                // 左键踩地块 第一次点击生成地雷(如果在格子中才生成地雷)
                if (_MM.isPosInCells(pos))
                {
                    _MM.makeMines(pos, _XCount, _YCount, _MineCount);
                    clickTile(pos);      
                    _Timer.startTimer();       //定时器开始工作
                }
                else
                {
                    _SE.playSE("click");
                }
            }
            else if (Input.GetMouseButtonUp(1))
            {
                // 右键标记
                changeMark(pos);
            }
        }
        else if (_State == GameState.RUNNING)
        {
            if (Input.GetMouseButtonUp(0))
            {
                clickTile(pos);
            }
            else if (Input.GetMouseButtonUp(1))
            {
                changeMark(pos);
            }

            if (Input.GetKeyUp(KeyCode.R))
            {
                resetGame();
            }
        }
        // 改变难度时
        else if (_State == GameState.CHANGE)
        {
            // 单击右键 取消改变难度
            if (Input.GetMouseButtonUp(1))
            {
                _DifficultyGO.SetActive(false);
                _State = _StateCache;
            }
        }
        else if (_State == GameState.END)
        {
            if (Input.GetKeyUp(KeyCode.R))
            {
                resetGame();
            }
        }
    }


    private void clickTile(Vector3 pos)
    {
        _SE.playSE("click");
        Vector3Int vp = _TF.WorldToCell(pos);
        _FinalClickTilePos = _TF.CellToWorld(vp);
        if (_TF.clickTile(pos))
        {
            Vector3Int[] vpis = getFlipArea(vp);
            for (int i = 0; i < vpis.Length; i++)
            {
                _TF.setTileByPosUncheck(vpis[i], null);
            }

            // 检查是否死亡
            if (_MM.isMineByPos(vp))
            {
                endGame();
            }
            // 检查是否胜利
            else if (_TF.getTileNum() <= _MineCount)
            {
                winGame();
            }
        }
        _State = GameState.RUNNING;

        
    }

    private void changeMark(Vector3 pos)
    {
        _SE.playSE("mark");
        _TF.changeMarking(pos);
        _MineCountMark = _TF.getMineMarkNum();
        _MineNumText.text = "" + (_MineCount - _MineCountMark);
    }
    
    // 游戏胜利
    private void winGame()
    {
        _State = GameState.WIN;


        // 标记所有地雷
        Vector3[] vps = _MM.getAllMineWorldPos();
        for (int i = 0; i < vps.Length; i++)
        {
            respawnSpriteAt(vps[i], _MineMarkedPrefab);
        }

        _MineNumText.text = "0";
        _TF.clearAllTiles();       // 清空表面Tile
        _SE.playSE("win");
        _Timer.stopTimer();        // 定时器停止
    }

    // 结束游戏
    private void endGame()
    {
        _State = GameState.END;
        // 标记死亡地雷 和 猜了的地雷
        respawnSpriteAt(_FinalClickTilePos, _MineClickPrefab);
        Vector3Int[] vpis = _TF.getAllMarkedPos();
        for (int i = 0; i < vpis.Length; i++)
        {
            respawnSpriteAt(_TF.CellToWorld(vpis[i]), _MineMarkedPrefab);
        }
        
        _TF.clearAllTiles();       // 清空表面Tile
        _Timer.stopTimer();        // 定时器停止
        _ExplodeLink = triggerAllMines();
        StartCoroutine(_ExplodeLink);
    }

    // 准备开始新游戏
    private void makeNewGame(int x, int y, int m)
    {
        // 基础数值设定
        _XCount = x;
        _YCount = y;
        _MineCount = m;
        _DifficultyGO.SetActive(false);
        _CoolDown = false;

        clearAllSprites();          // 清空结算Sprite
        _SE.playSE("reset");

        // 雷区初始化
        _TF.reset(_XCount, _YCount);
        _MM.reset(_XCount, _YCount);

        // 计算网格及背景尺寸
        float scale;
        scale = 9f / _YCount;       // 9单位为标准大小
        Vector3 vp = _GridTransform.transform.position;
        vp.x = _XCount * scale / -2f;
        _GridTransform.transform.position = vp;
        Vector3 vp2 = _BorderTransform.localScale;
        Vector3 vp3 = _BackgroundTransform.localScale;
        vp2.x = vp.x * -2f + 0.5f;
        vp3.x = vp.x * -2f - 0.5f * scale;
        _BorderTransform.localScale = vp2;
        _BackgroundTransform.localScale = vp3;

        // UI元素改变
        _MineCountMark = 0;
        _MineNumText.text = "" + (_MineCount - _MineCountMark);
        _GridTransform.localScale = new Vector3(scale, scale, 1);       //改变格子大小
        _State = GameState.WAIT;
        _Timer.clear();        //计时器清空
    }

    // ------------------------Manager调用函数------------------------
    // 获取翻开区域 （坐标）
    public Vector3Int[] getFlipArea(Vector3Int clickPos)
    {
        List<Vector3Int> vps = new List<Vector3Int>();              // 被翻开列表
        List<Vector3Int> waitList = new List<Vector3Int>();         // 等候列表
        List<Vector3Int> checkedList = new List<Vector3Int>();      // 检查过的列别

        // 不是地雷的话
        if (!_MM.isMineByPos(clickPos))
        {
            // 位置不空的话
            if (_MM.isEmptyByPos(clickPos))
            {
                waitList.Add(clickPos);
                int loopNum = 0;        //保险

                // 检查所有临接地块 和 临接临接地块的地块， 列表为空时结束
                while (waitList.Count > 0 || loopNum > 5000)
                {
                    loopNum++;
                    vps.Add(waitList[0]);
                    // 查看周围八个格子
                    Vector3Int[] neighbours = new Vector3Int[8] {
                        new Vector3Int(waitList[0].x+1,waitList[0].y,0), new Vector3Int(waitList[0].x-1, waitList[0].y, 0),
                        new Vector3Int(waitList[0].x, waitList[0].y+1, 0), new Vector3Int(waitList[0].x, waitList[0].y-1, 0),
                        new Vector3Int(waitList[0].x-1,waitList[0].y-1,0), new Vector3Int(waitList[0].x-1, waitList[0].y+1, 0),
                        new Vector3Int(waitList[0].x+1, waitList[0].y-1, 0), new Vector3Int(waitList[0].x+1, waitList[0].y+1, 0)};
                    for (int i = 0; i < neighbours.Length; i++)
                    {
                        // 已经看过了 跳过；没看过的 进去
                        if (!checkedList.Contains(neighbours[i]))
                        {
                            // 这个标记为看过
                            checkedList.Add(neighbours[i]);
                            // 空格的周围格子需要检查，加入列表
                            if (_MM.isEmptyByPos(neighbours[i]))
                            {
                                waitList.Add(neighbours[i]);
                                vps.Add(neighbours[i]);         // 空格本身加入被翻开列表
                            }
                            // 数字格只把自己加入被翻开列表
                            else if (!_MM.isMineByPos(neighbours[i]))
                            {
                                vps.Add(neighbours[i]);
                            }
                        }
                    }
                    waitList.RemoveAt(0);
                }
            }
        }

        // 返回被翻开的列表
        return vps.ToArray();
    }

    // ------------------------UI调用函数------------------------
    // 改变难度
    public void changeDifficluty()
    {
        _StateCache = _State;           //改变游戏状态（来防止误触）
        _DifficultyGO.SetActive(true);
        _State = GameState.CHANGE;
        _SE.playSE("click");
    }

    // 开始新游戏
    public void newGame(int difficulty)
    {
        switch (difficulty)
        {
            case 0: makeNewGame(9, 9, 10); break;
            case 1: makeNewGame(16, 16, 40); break;
            case 2: makeNewGame(32, 16, 99); break;
            default: newGameRand(); break;
        }
    }

    // 随即开始新游戏
    public void newGameRand()
    {
        _DifficultyGO.SetActive(false);
        _YCount = Random.Range(9, 16);
        _XCount = Random.Range(_YCount/2,_YCount*2);
        _MineCount = Random.Range(1, Mathf.Min((_XCount * _YCount) / 2, _XCount * _YCount - 9));
        makeNewGame(_XCount,_YCount,_MineCount);
    }

    // 重新以当前难度开始游戏
    public void resetGame()
    {
        StopCoroutine(_ExplodeLink);
        makeNewGame(_XCount, _YCount, _MineCount);
    }

    // ------------------------UIGO功能相关私有函数------------------------
    // 生成一个Sprite
    private void respawnSpriteAt(Vector3 pos, GameObject spritePrefab)
    {
        GameObject go = Instantiate(spritePrefab, pos,Quaternion.identity,_SpriteFather);
        go.transform.localScale = _GridTransform.localScale;
        go.transform.position += _GridTransform.localScale/2f;
    }

    // 清除所有Sprite
    private void clearAllSprites()
    {
        int cn = _SpriteFather.childCount;
        _SpriteFather.gameObject.SetActive(false);
        for (int i=0; i<cn; i++)
        {
            Destroy(_SpriteFather.GetChild(cn-i-1).gameObject);     // 这里倒着删是为了解决Unity对象计数问题
        }
        _SpriteFather.gameObject.SetActive(true);
    }

    private IEnumerator triggerAllMines()
    {
        Vector3 origin = _FinalClickTilePos;
        Vector3[] vps = _MM.getAllMineWorldPos();

        for(int i=0; i<vps.Length; i++)
        {
            for (int j = 0; j < vps.Length - i - 1; j++)
            {
                if (Vector3.Distance(origin,vps[j]) > Vector3.Distance(origin, vps[j+1]))
                {
                    (vps[j], vps[j + 1]) = (vps[j + 1], vps[j]);
                }

            }
        }

        float lastDistence = 0;
        for(int k = 0; k<vps.Length; k++)
        {
            float newDistence = _TriggerIntervalPerUnit * Vector3.Distance(origin, vps[k]);
            float pan = vps[k].x * _StereoPanMult;
            yield return new WaitForSeconds(newDistence-lastDistence + _TriggerIntervalPadding);
            lastDistence = newDistence;
            int num = Random.Range(1,5);
            _SE.playSE("explode"+num,pan);
            respawnSpriteAt(vps[k], _MineClickPrefab);
        }


        yield return new WaitForEndOfFrame();
    }
}
