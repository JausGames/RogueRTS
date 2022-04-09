using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Army : MonoBehaviour
{
    Player owner;
    [SerializeField] List<Minion> minions = new List<Minion>();
    [SerializeField] float sideOffset = 0.35f;
    [SerializeField] private float upwardOffset = 0.35f;
    [SerializeField]  private int nbByLine;
    [SerializeField]  private AnimationCurve nbByLineCurve;

    private void Start()
    {
        owner = GetComponent<Player>();
        foreach(Minion min in minions)
        {
            min.dieEvent.AddListener(delegate { minions.Remove(min); });
        }
    }

    public void SetMinionsPosition(Vector3 position, Vector3 direction)
    {
        Debug.Log("Army, SetMinionsPositions : direction = " + direction);
        var count = minions.Count;
        for (int i = 0; i < count; i++)
        {
            if(!minions[i].Fighting)
            {
                minions[i].SetPosition(GetLinePosition(i, NbMinionByLine(minions.Count)) * owner.transform.right + upwardOffset * Mathf.Ceil(1 + (i / nbByLine)) * owner.transform.forward + position);
                minions[i].SetRotation(direction);
            }
        }
    }

    float GetLinePosition(int nb, int nbByLine)
    {
        var sign = Mathf.FloorToInt((float)nb / (float)nbByLine) % 2 == 1 ? -1 : 1;
            
        var leftOrRight = nb % nbByLine == 0 && nbByLine % 2 == 1 ? 0 : (nb % 2 == 0 ? -1f : 1f) * sign;
        var posInLine = sideOffset * ((Mathf.Floor(nb / 2)) % (Mathf.Floor((float)nbByLine / 2f)) + 1f);
        posInLine = nbByLine % 2 == 0 ? posInLine - 0.5f * sideOffset : posInLine;

        return posInLine * leftOrRight;

    }

    internal void AddMinion(Minion minion)
    {
        minions.Add(minion);
        minion.dieEvent.AddListener(delegate { minions.Remove(minion); });
    }

    int NbMinionByLine(int nbTotal)
    {
        nbByLine = (int)Mathf.Floor(nbByLineCurve.Evaluate(nbTotal));
        //nbByLine = Mathf.Clamp(3, (int)Mathf.Ceil(nbTotal / 4), 9);

        return nbByLine;
    }
}

