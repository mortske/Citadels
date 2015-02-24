// -----------------------------------------------------------------------
// <copyright file="DemoPlayer.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.LoadBalancing;

using UnityEngine;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class DemoPlayer : Player
{
    private int posX;
    private int posY;

    protected internal DemoPlayer(string name, int actorID, bool isLocal, Hashtable actorProperties) : base(name, actorID, isLocal, actorProperties)
    {
    }

    public override string ToString()
    {
        return base.ToString() + " pos: " + posX;
    }
}