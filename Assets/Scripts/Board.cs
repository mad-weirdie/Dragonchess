using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
	public class Board
	{
		public static int width = 12;
		public static int height = 8;

		public static float base_height = 5;
		public static float layer_spacing = 6;

		public int layer_int_val;
		public Layer layer;

		Square[,] m_squares;

		public Board(int l, Layer lay)
		{
			m_squares = new Square[height, width];
			layer = lay;
			layer_int_val = l;

			for (int r = 0; r < Board.height; r++)
			{
				for (int c = 0; c < Board.width; c++)
				{
					this.squares[r,c] = new Square(r, c, this);
				}
			}
		}

		public Square[,] squares
		{
			get
			{
				return m_squares;
			}
		}
	}
}
