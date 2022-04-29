using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusFactory : Factory
{
    [SerializeField] List<Bonus> bonusList = new List<Bonus>();
    [SerializeField] Bonus bonus;
    [SerializeField] SpriteRenderer rd;
    [SerializeField] Text bonusName;
    [SerializeField] bool open = false;

    public bool Open { get => open; set => open = value; }

    override public void OnInteract(Hitable player)
    {
        if (!open) OpenChest();
        else player.AddBonus(bonus);
    }
    private void OpenChest()
    {
        var rnd = Random.Range(0, bonusList.Count);
        bonusName.text = bonusList[rnd].name;

        bonus = Instantiate(bonusList[rnd], transform);
        bonus.name = bonusName.text;

        rd.sprite = bonus.Sprite;
        open = true;
    }

}
