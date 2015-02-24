using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CardCollection : MonoBehaviour 
{
    public List<Card> collection;
    public List<Card> Collection
    {
        get { return collection; }
        set { collection = value; }
    }

    public void DestroyCardWithID(int id)
    {
        for (int i = 0; i < Collection.Count; i++)
        {
            if (Collection[i].id == id)
            {
                Collection.RemoveAt(i);
                return;
            }
        }
    }

    public Card RemoveCardWithID(int id)
    {
        Card tmp = null;
        for (int i = 0; i < Collection.Count; i++)
        {
            if (Collection[i].id == id)
            {
                tmp = Collection[i];
                Collection.RemoveAt(i);
            }
        }
        if (tmp == null)
            Debug.LogError("Could not find a card with ID: " + id);
        return tmp;
    }

    public virtual void AddCard(Card card)
    {
        collection.Add(card);
    }

    public Card GetCardAt(int index)
    {
        return Collection[index];
    }

    public Card RemoveCardAt(int index)
    {
        Card tmp = Collection[index];
        Collection.RemoveAt(index);
        return tmp;
    }
}
