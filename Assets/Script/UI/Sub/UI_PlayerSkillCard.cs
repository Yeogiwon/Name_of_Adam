using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UI_PlayerSkillCard : UI_Base, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private GameObject _highlight;
    [SerializeField] private GameObject _inactive;
    [SerializeField] private GameObject _skillCard;
    [SerializeField] private TextMeshProUGUI _ManaCost;
    [SerializeField] private TextMeshProUGUI _essenceCost;
    [SerializeField] private AudioSource _audioSource;
    //[SerializeField] private TextMeshProUGUI _text;

    private UI_PlayerSkill _playerSkill;
    public PlayerSkill _skill;
    public bool IsSelected = false;

    private void Start()
    {
        _highlight.SetActive(false);
        _inactive.SetActive(false);
    }

    public void Set(UI_PlayerSkill ps, PlayerSkill skill)
    {
        _playerSkill = ps;
        _skill = skill;
        //_text.text = skill.GetName();
        GetComponent<Image>().sprite = skill.GetSkillImage();
        _ManaCost.text = skill.GetManaCost().ToString();
        _essenceCost.text = skill.GetDarkEssenceCost().ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _skillCard.transform.localScale = new Vector3(1.15f, 1.15f, 1.15f);
        GameManager.UI.ShowHover<UI_SkillHover>().SetSkillHover(_skill.GetName(), _skill.GetManaCost(), _skill.GetDarkEssenceCost(), _skill.GetDescription(), eventData.position);
        _highlight.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.UI.CloseHover();
        if (IsSelected)
            return;
        _skillCard.transform.localScale = new Vector3(1f, 1f, 1f);
        _highlight.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(BattleManager.BattleUI.UI_hands._selectedHand != null)
        {
            return;
        }
        else if(eventData.button == PointerEventData.InputButton.Left)
        {
            _audioSource.Play();
            _playerSkill.OnClickHand(this);
        }
    }

    public void ChangeSelectState(bool b)
    {
        IsSelected = b;
        _highlight.SetActive(b);

        if (!IsSelected)
        {
            _skillCard.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    public void ChangeInable(bool b)
    {
        _inactive.SetActive(b);
    }

    public PlayerSkill GetSkill() => _skill;
}
