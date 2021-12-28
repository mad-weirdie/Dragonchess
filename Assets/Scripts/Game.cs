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
        public GameObject black_text;
        public GameObject white_text;

        void Start()
        {
            turn = Color.White;
        }

        public void SwitchTurn()
        {
            if (turn == Color.White)
            {
                white_text.SetActive(false);
                black_text.SetActive(true);
                turn = Color.Black;
            }
            else
            {
                black_text.SetActive(false);
                white_text.SetActive(true);
                turn = Color.White;
            }
        }
    }
}
