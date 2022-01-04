using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    public class Human : Player
    {
        // Initialize player with color c
        public Human(Color c, PlayerType t) : base(c, PlayerType.Human) { }

        override public Move GetMove()
		{
            MonoBehaviour.print("calling human getmove");
            return null;
		}
    }
}
