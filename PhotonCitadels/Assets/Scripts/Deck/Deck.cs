using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Deck : CardCollection 
{
    public void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public int[] GetIDArray()
    {
        List<Card> cards = Collection;
        int[] tmp = new int[cards.Count];
        for (int i = 0; i < cards.Count; i++)
        {
            tmp[i] = cards[i].id;
        }
        return tmp;
    }

    public void SetOrderWithName(int[] ids)
    {
        List<Card> newOrder = new List<Card>();

        for (int i = 0; i < ids.Length; i++)
        {
            foreach (Card card in Collection)
            {
                if (card.id == ids[i])
                {
                    newOrder.Add(card);
                }
            }
        }
        Collection = newOrder;
    }

    public Card RemoveTopCard()
    {
        Card tmp = Collection[Collection.Count - 1];
        Collection.RemoveAt(Collection.Count - 1);
        return tmp;
    }
}
