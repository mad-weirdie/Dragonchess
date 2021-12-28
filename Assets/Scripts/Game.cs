using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Dragonchess
{
    public enum Color { White, Black };
    public enum Layer { Upper, Middle, Lower };

    public class Game : MonoBehaviour
    {
        public Color turn;

        void Start()
        {
            turn = Color.White;
        }

        public void SwitchTurn()
        {
            if (turn == Color.White)
                turn = Color.Black;
            else
                turn = Color.White;
        }
    }
}
