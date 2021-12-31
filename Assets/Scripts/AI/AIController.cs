using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess {
    public class AIController : MonoBehaviour
    {
        public MoveController moveController;
        public bool waiting = true;

        public Move GetNextMove(Player player)
        {
            return DumbRandom(player);
        }

        public Move DumbRandom(Player player)
        {
            List<Move> moves = new List<Move>();
            Piece piece = null;
            int p, move = 0;
            
            while (moves.Count == 0)
            {
                p = Random.Range(0, player.pieces.Count);
                piece = player.pieces[p];
                moves = piece.GetMoves();
            }

            move = Random.Range(0, moves.Count);
            Move AIMove = moves[move];
            print("The AI has chosen to move the " + piece.type + " at " + piece.pos.SquareName() + " to " + AIMove.end.SquareName() + ". MoveType: " + AIMove.type);
            return moves[move];
        }
    }
}
