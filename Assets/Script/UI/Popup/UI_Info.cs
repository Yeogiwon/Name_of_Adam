using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Info : UI_Scene
{
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _stat;

    [SerializeField] private UI_HPBar _hpBar;
    [SerializeField] private UI_HoverImageBlock _stigma;
    [SerializeField] private Transform _stigmaGrid;

    [SerializeField] private Transform _rangeGrid;
    [SerializeField] private GameObject _squarePrefab;

    [SerializeField] private Transform _unitInfoFallGrid;
    [SerializeField] private GameObject _fallGaugePrefab;

    [SerializeField] private Transform _stigmaDescriptionGrid;
    [SerializeField] private GameObject _stigmaDescriptionPrefab;

    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private Image _unitImage;



    //색상은 UI에서 정해주는대로
    readonly Color goodColor = Color.yellow;
    readonly Color badColor = Color.red;
    readonly Color textColor = new Color(195f, 195f, 195f);
    readonly string goodColorStr = "yellow";
    readonly string badColorStr = "red";
    readonly string normalColorStr = "white";
    
    

    public void SetInfo(BattleUnit battleUnit)
    {
        //배틀 유닛의 경우
        DeckUnit unit = battleUnit.DeckUnit;

        _name.text = unit.Data.Name;
        Team team = battleUnit.Team;


        //"<color=\"black\"></color>"

        _unitImage.sprite = battleUnit.Data.Image;

        if (battleUnit.Team == Team.Player)
        {
            _unitImage.GetComponent<Image>().sprite = GameManager.Resource.Load<Sprite>($"Arts/Units/Unit_Dia_Portrait/" + battleUnit.DeckUnit.Data.Name + "_타락");
        }
        else
        {
            _unitImage.GetComponent<Image>().sprite = GameManager.Resource.Load<Sprite>($"Arts/Units/Unit_Dia_Portrait/" + battleUnit.DeckUnit.Data.Name);
        }

        string statText = "ATK: " + "<color=\"AttackColor\">" + battleUnit.BattleUnitTotalStat.ATK.ToString() + "</color>" + "\n\n" +
                                "SPD:  " + "<color=\"SpeedColor\">" + battleUnit.BattleUnitTotalStat.SPD.ToString() + "</color>" + "\n\n" +
                                "COST:  " + "<color=\"CostColor\">" + battleUnit.BattleUnitTotalStat.ManaCost.ToString() + "</color>";
                                
        string hpText = battleUnit.BattleUnitTotalStat.CurrentHP.ToString() + "/" + battleUnit.BattleUnitTotalStat.MaxHP.ToString();

        if (unit.DeckUnitChangedStat.ATK > 0 || battleUnit.BattleUnitChangedStat.ATK > 0)
            statText = statText.Replace("AttackColor", goodColorStr);
        else if (unit.DeckUnitChangedStat.ATK < 0 || battleUnit.BattleUnitChangedStat.ATK < 0)
            statText = statText.Replace("AttackColor", badColorStr);
        else
            statText = statText.Replace("AttackColor", normalColorStr);

        if (unit.DeckUnitChangedStat.SPD > 0 || battleUnit.BattleUnitChangedStat.SPD > 0)
            statText = statText.Replace("SpeedColor", goodColorStr);
        else if (unit.DeckUnitChangedStat.SPD < 0 || battleUnit.BattleUnitChangedStat.SPD < 0)
            statText = statText.Replace("SpeedColor", badColorStr);
        else
            statText = statText.Replace("SpeedColor", normalColorStr);

        if (unit.DeckUnitChangedStat.ManaCost > 0 || battleUnit.BattleUnitChangedStat.ManaCost > 0)
            statText = statText.Replace("CostColor", goodColorStr);
        else if (unit.DeckUnitChangedStat.ManaCost < 0 || battleUnit.BattleUnitChangedStat.ManaCost < 0)
            statText = statText.Replace("CostColor", badColorStr);
        else
            statText = statText.Replace("CostColor", normalColorStr);



        _stat.text = statText;

        //_fallText.text = fallText;
        _hpText.text = hpText;

        _hpBar.SetHPBar(team);
        _hpBar.SetFallBar(unit);

        _hpBar.RefreshHPBar((float)battleUnit.BattleUnitTotalStat.CurrentHP / (float)battleUnit.BattleUnitTotalStat.MaxHP);
        _hpBar.RefreshFallGauge(battleUnit.BattleUnitTotalStat.FallCurrentCount);

        //_stigmaDescriptionPrefab.SetStigma(battleUnit);

        foreach(Stigma stigma in battleUnit.StigmaList)
        {
            UI_StigmaDescription sd = GameObject.Instantiate(_stigmaDescriptionPrefab, _stigmaDescriptionGrid).GetComponent<UI_StigmaDescription>();
            sd.SetStigma(stigma);
        }
        


        Sprite attackType;

        if (unit.Data.BehaviorType == BehaviorType.근거리)
            attackType = GameManager.Resource.Load<Sprite>($"Arts/UI/Battle_UI/근거리_아이콘");
        else
            attackType = GameManager.Resource.Load<Sprite>($"Arts/UI/Battle_UI/원거리_아이콘");

        foreach (bool range in unit.Data.AttackRange)
        {
            Image block = GameObject.Instantiate(_squarePrefab, _rangeGrid).GetComponent<Image>();
            if (range)
                block.color = Color.red;
            else
                block.color = Color.grey;
        }
    }

    public void SetInfo(DeckUnit unit, Team team)
    {
        //덱 유닛의 경우
        _name.text = unit.Data.Name;

        _unitImage.sprite = unit.Data.Image;

        if (team == Team.Player)
        {
            _unitImage.GetComponent<Image>().sprite = GameManager.Resource.Load<Sprite>($"Arts/Units/Unit_Dia_Portrait/" + unit.Data.Name + "_타락");
        }
        else
        {
            _unitImage.GetComponent<Image>().sprite = GameManager.Resource.Load<Sprite>($"Arts/Units/Unit_Dia_Portrait/" + unit.Data.Name);
        }

        string statText = "ATK: " + "<color=\"AttackColor\">" + unit.DeckUnitTotalStat.ATK.ToString() + "</color>" + "\n\n" +
                                "SPD:  " + "<color=\"SpeedColor\">" + unit.DeckUnitTotalStat.SPD.ToString() + "</color>" + "\n\n" +
                                "COST:  " + "<color=\"CostColor\">" + unit.DeckUnitTotalStat.ManaCost.ToString() + "</color>";

        string hpText = unit.DeckUnitTotalStat.CurrentHP.ToString() + "/" + unit.DeckUnitTotalStat.MaxHP.ToString();

        if (unit.DeckUnitChangedStat.ATK > 0)
            statText = statText.Replace("AttackColor", goodColorStr);
        else if (unit.DeckUnitChangedStat.ATK < 0)
            statText = statText.Replace("AttackColor", badColorStr);
        else
            statText = statText.Replace("AttackColor", normalColorStr);

        if (unit.DeckUnitChangedStat.SPD > 0)
            statText = statText.Replace("SpeedColor", goodColorStr);
        else if (unit.DeckUnitChangedStat.SPD < 0)
            statText = statText.Replace("SpeedColor", badColorStr);
        else
            statText = statText.Replace("SpeedColor", normalColorStr);

        if (unit.DeckUnitChangedStat.ManaCost > 0)
            statText = statText.Replace("CostColor", goodColorStr);
        else if (unit.DeckUnitChangedStat.ManaCost < 0)
            statText = statText.Replace("CostColor", badColorStr);
        else
            statText = statText.Replace("CostColor", normalColorStr);

        //fallText = fallText.Replace("textColor" , )

        _stat.text = statText;
        _hpText.text = hpText;

        _hpBar.SetHPBar(team);
        _hpBar.SetFallBar(unit);

        _hpBar.RefreshHPBar((float)unit.DeckUnitTotalStat.CurrentHP / (float)unit.DeckUnitTotalStat.MaxHP);
        _hpBar.RefreshFallGauge(unit.DeckUnitTotalStat.FallCurrentCount);

        //_stigmaDescriptionPrefab.SetStigma(unit);


        foreach(Stigma stigma in unit.GetStigma())
        {
            UI_StigmaDescription sd = GameObject.Instantiate(_stigmaDescriptionPrefab, _stigmaDescriptionGrid).GetComponent<UI_StigmaDescription>();
            sd.SetStigma(stigma);
        }


        Sprite attackType;

        if (unit.Data.BehaviorType == BehaviorType.근거리)
            attackType = GameManager.Resource.Load<Sprite>($"Arts/UI/Battle_UI/근거리_아이콘");
        else
            attackType = GameManager.Resource.Load<Sprite>($"Arts/UI/Battle_UI/원거리_아이콘");

        //_SkillImage.Set(attackType, unit.Data.Description.Replace("(ATK)", unit.DeckUnitTotalStat.ATK.ToString()));

        foreach (bool range in unit.Data.AttackRange)
        {
            Image block = GameObject.Instantiate(_squarePrefab, _rangeGrid).GetComponent<Image>();
            if (range)
                block.color = Color.red;
            else
                block.color = Color.grey;
        }
    }

    public void InfoDestroy()
    {
        GameManager.Resource.Destroy(this.gameObject);
    }
}
