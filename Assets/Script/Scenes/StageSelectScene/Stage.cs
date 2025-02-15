using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public enum StageType
{
    Store,
    Event,
    Battle,
    Tutorial,
    BattleTest
}

[Serializable]
public class StageData
{
    public int ID;
    public StageType Type;
    public StageName Name;
    public int StageLevel;
    public int StageID;
}

public class Stage : MonoBehaviour
{
    private SpriteRenderer _renderer;

    [SerializeField] StageNodeBackLight BackLight;
    [Space(10f)]
    [SerializeField] public List<Stage> NextStage;
    [Space(10f)]
    [Header("StageInfo")]
    [SerializeField] public StageData Datas;
    private bool _isClear = false;
    private bool _isNextStage = false;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        StageManager.Instance.InputStageList(this);
    }
    private void Start()
    {
        SetSprite();

        if (GameManager.Data.Map.ClearTileID == null)
            GameManager.Data.Map.ClearTileID = new();

        if (GameManager.Data.Map.ClearTileID.Contains(Datas.ID))
        {
            GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
            _isClear = true;
        }

        foreach (Stage st in NextStage)
        {
            if (Datas.ID == 0)
                return;

            StageLine line = GameManager.Resource.Instantiate("Stage/Line", transform).GetComponent<StageLine>();
            line.DrawLine(st);
        }
    }

    public void SetSprite()
    {
        if (Datas.Name == StageName.none)
            _renderer.color = new Color(1, 1, 1, 0);

        string name = Datas.Name.ToString();

        if (Datas.Name == StageName.EliteBattle || Datas.Name == StageName.BossBattle)
            name += "_" + Datas.StageID;

        _renderer.sprite = GameManager.Resource.Load<Sprite>($"Arts/StageSelect/Node/{name}");
        BackLight.SetSprite(name);
    }

    public IEnumerator Fade()
    {
        float FadeTime = 1;
        float time = 0;

        while(time < FadeTime)
        {
            time += Time.deltaTime;
            float t = time / FadeTime;

            _renderer.color = new Color(1 - t, 1 - t, 1 - t);

            yield return null;
        }
    }

    public void SetNextStage()
    {
        _isNextStage = true;
        BackLight.SetVisible();
    }

    public void OnMouseUp()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            StageManager.Instance.StageMove(Datas.ID);
        }
    }
    
    public void OnMouseEnter()
    {
        if (_isNextStage)
            return;
        if (!_isClear)
            BackLight.FadeIn();
    }

    public void OnMouseExit()
    {
        if (_isNextStage)
            return;
        if (!_isClear)
            BackLight.FadeOut();
    }
    
    public StageData SetBattleStage(int a, int b)
    {
        Datas.StageLevel = a;
        Datas.StageID = b;

        return Datas;
    }
}