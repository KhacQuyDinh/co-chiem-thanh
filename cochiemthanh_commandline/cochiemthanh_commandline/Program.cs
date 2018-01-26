using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace cochiemthanh_commandline
{
    public class Program
    {
        //to calculate the present number of step
        int num_step = 0;
        int limit_step = 0;

        //status gameboard desc
        public const int bad_move_number = -1;
        public const int raw_number = -1;
        public const int win_number = 0;
        public const int lose_number = 1;
        public const int live_game_number = 3;

        public Dictionary<char, int> mapId = new Dictionary<char, int>();
        public Dictionary<int, char> mapCharCol = new Dictionary<int, char>();
        public Dictionary<int, char> mapCharRow = new Dictionary<int, char>();
        public Dictionary<char, int> mapCharToInt = new Dictionary<char, int>();

        public void InitmapId()
        {
            mapId.Add('a', 0);
            mapId.Add('b', 1);
            mapId.Add('c', 2);
            mapId.Add('d', 3);
            mapId.Add('e', 4);
            mapId.Add('f', 5);
            mapId.Add('g', 6);
            mapId.Add('h', 7);
            mapId.Add('8', 0);
            mapId.Add('7', 1);
            mapId.Add('6', 2);
            mapId.Add('5', 3);
            mapId.Add('4', 4);
            mapId.Add('3', 5);
            mapId.Add('2', 6);
            mapId.Add('1', 7);
        }

        public void InitmapCharCol()
        {
            mapCharCol.Add(0, 'a');
            mapCharCol.Add(1, 'b');
            mapCharCol.Add(2, 'c');
            mapCharCol.Add(3, 'd');
            mapCharCol.Add(4, 'e');
            mapCharCol.Add(5, 'f');
            mapCharCol.Add(6, 'g');
            mapCharCol.Add(7, 'h');
        }

        public void InitmapCharRow()
        {
            mapCharRow.Add(0, '8');
            mapCharRow.Add(1, '7');
            mapCharRow.Add(2, '6');
            mapCharRow.Add(3, '5');
            mapCharRow.Add(4, '4');
            mapCharRow.Add(5, '3');
            mapCharRow.Add(6, '2');
            mapCharRow.Add(7, '1');
        }

        public void InitmapCharToInt()
        {
            mapCharToInt.Add('0', 0);
            mapCharToInt.Add('1', 1);
            mapCharToInt.Add('2', 2);
            mapCharToInt.Add('3', 3);
            mapCharToInt.Add('4', 4);
            mapCharToInt.Add('5', 5);
            mapCharToInt.Add('6', 6);
            mapCharToInt.Add('7', 7);
        }

        //determine current turn
        public Player currentPlayer = Player.none;
        public enum Player : int
        {
            none = -1,
            human = 1,
            machine = 2
        }

        //player String desc
        public const String player_human_str = "Nguoi";
        public const String player_machine_str = "May";

        //public Piece currentHorse = null;

        //empty means having neither machine's troop or human's troop
        public const String empty_str = "  ";

        public const int max_row = 8;
        public const int max_col = 8;

        //public int[] attackRate = new int[] { 10, 20, 30, 40, 50 };
        //public int[] defenceRate = new int[] { 10, 20, 30, 40, 50 };

        public Piece wall_human = new Piece(0, 4, "TH");
        public Piece wall_machine = new Piece(7, 3, "TH");
        public Piece plane_human = new Piece(0, 3, "MB");
        public Piece plane_machine = new Piece(7, 4, "MB");

        //human's horses        
        public Piece O_human = new Piece(0, 0, "O ");
        public Piece P_human = new Piece(0, 4, "P ");
        public Piece Q_human = new Piece(0, 7, "Q ");

        //machine's horses        
        public Piece K_machine = new Piece(7, 0, "K ");
        public Piece L_machine = new Piece(7, 3, "L ");
        public Piece M_machine = new Piece(7, 7, "M ");

        public List<Piece> listMachineHorse = new List<Piece>();
        public List<Piece> listHumanHorse = new List<Piece>();
        //private int num_human_horse = 0;
        //private int num_machine_horse = 0;

        public void InitListHorses()
        {
            //list human horses
            listHumanHorse.Add(O_human);
            listHumanHorse.Add(P_human);
            listHumanHorse.Add(Q_human);

            //list machine horses
            listMachineHorse.Add(K_machine);
            listMachineHorse.Add(L_machine);
            listMachineHorse.Add(M_machine);
        }

        public String[,] boardgame = new String[max_row, max_col];
        public int[,] EBoardgame = new int[max_row, max_col];

        //for list param simulation
        private const int our_player_index = 0;
        private const int oppt_index = 1;
        private const int move_index = 2;
        private const int list_oppt_horse_index = 3;

        //for list source simulation
        private const int des_src_index = 0;
        private const int move_src_index = 1;
        private const int oppt_horse_src_index = 2;
        private const int next_move_to_oppt_src_index = 3;
        private const int list_oppt_horse_src_index = 4;
        private const int our_player_src_index = 5;

        private List<Object> SimulateMove(List<Object> list)
        {
            Player ourPlayer = (Player)list[our_player_index];
            Player oppt = (Player)list[oppt_index];
            String move = (String)list[move_index];
            List<Piece> listOpptHorse = (List<Piece>)list[list_oppt_horse_index];

            int row_1 = mapCharToInt[move[0]];
            int col_1 = mapCharToInt[move[1]];
            int row_2 = mapCharToInt[move[2]];
            int col_2 = mapCharToInt[move[3]];

            //simulate first move = actual move

            //board
            String des = boardgame[row_2, col_2];
            boardgame[row_2, col_2] = boardgame[row_1, col_1];

            //update our_player horse position
            UpdatePlayerHorsePos(ourPlayer, "" + row_1 + col_1 + row_2 + col_2);

            //check whether (p_2,p_3) in opponent horse
            Piece opptHorse = GetHorse(oppt, row_2, col_2);
            bool nextMoveInOppHorse = listOpptHorse.Contains(opptHorse);

            //case eat opponent horse
            if (nextMoveInOppHorse) listOpptHorse.Remove(opptHorse);

            if (ourPlayer == Player.machine)
            {
                if (row_1 == plane_human.Row && col_1 == plane_human.Col) boardgame[row_1, col_1] = plane_human.Label;
                else boardgame[row_1, col_1] = empty_str;
            }
            else if (ourPlayer == Player.human)
            {
                if (row_1 == plane_machine.Row && col_1 == plane_machine.Col) boardgame[row_1, col_1] = plane_machine.Label;
                else boardgame[row_1, col_1] = empty_str;
            }

            List<Object> result = new List<Object>() { null, null, null, null, null, null };
            result[des_src_index] = des;
            result[move_src_index] = move;
            result[oppt_horse_src_index] = opptHorse;
            result[next_move_to_oppt_src_index] = nextMoveInOppHorse;
            result[list_oppt_horse_src_index] = listOpptHorse;
            result[our_player_src_index] = ourPlayer;

            return result;
        }

        private void UnSimulateMove(List<Object> list)
        {
            String move = (String)list[move_src_index];
            String des = (String)list[des_src_index];
            bool nextMoveInOppHorse = (Boolean)list[next_move_to_oppt_src_index];
            Piece opptHorse = (Piece)list[oppt_horse_src_index];
            List<Piece> listOpptHorse = (List<Piece>)list[list_oppt_horse_src_index];
            Player ourPlayer = (Player)list[our_player_src_index];

            int row_1 = mapCharToInt[move[0]];
            int col_1 = mapCharToInt[move[1]];
            int row_2 = mapCharToInt[move[2]];
            int col_2 = mapCharToInt[move[3]];
            //redo simulating

            //board
            boardgame[row_1, col_1] = boardgame[row_2, col_2];
            boardgame[row_2, col_2] = des;

            //revert case eat opponent(human) horse
            if (nextMoveInOppHorse) listOpptHorse.Add(opptHorse);

            //update machine horse position
            UpdatePlayerHorsePos(ourPlayer, "" + row_2 + col_2 + row_1 + col_1);
        }

        //reset boardgame
        public void ResetBoardGame()
        {
            for (int rw = 0; rw < max_row; rw++)
                for (int cl = 0; cl < max_col; cl++)
                {
                    boardgame[rw, cl] = empty_str;
                }

            //set pieces in the board            
            boardgame[wall_human.Row, wall_human.Col] = wall_human.Label;
            boardgame[wall_machine.Row, wall_machine.Col] = wall_machine.Label;

            boardgame[plane_human.Row, plane_human.Col] = plane_human.Label;
            boardgame[O_human.Row, O_human.Col] = O_human.Label;
            boardgame[P_human.Row, P_human.Col] = P_human.Label;
            boardgame[Q_human.Row, Q_human.Col] = Q_human.Label;

            boardgame[K_machine.Row, K_machine.Col] = K_machine.Label;
            boardgame[L_machine.Row, L_machine.Col] = L_machine.Label;
            boardgame[plane_machine.Row, plane_machine.Col] = plane_machine.Label;
            boardgame[M_machine.Row, M_machine.Col] = M_machine.Label;
        }

        /// <summary>
        /// calculate eboard using current player other funcs
        /// calculate heristic just for the specific move to reduce a big amount of time
        /// </summary>                
        public void CalEBoard(bool isMaximizePlayer, String move)
        {
            //reset value to zero
            for (int rw = 0; rw < max_row; rw++)
            {
                for (int cl = 0; cl < max_col; cl++)
                {
                    EBoardgame[rw, cl] = 0;
                }
            }

            Player opponent = isMaximizePlayer ? Player.human : Player.machine;
            Player mine = isMaximizePlayer ? Player.machine : Player.human;

            int defenceScore = 0;
            int attackScore = 0;
            int minusScore = 0;

            //cordinate of the move
            int row_1 = mapCharToInt[(char)move[0]];
            int col_1 = mapCharToInt[(char)move[1]];
            int row_2 = mapCharToInt[(char)move[2]];
            int col_2 = mapCharToInt[(char)move[3]];

            //eat wall opponent
            if (IsDefeatOppt(row_2, col_2, mine, opponent))
            {
                attackScore += EvaluateRate.defeat_oppt;
            }

            //move to opponent plane
            if (IsInOpptPlane(row_2, col_2, opponent))
            {
                attackScore += EvaluateRate.moving_to_oppt_plane;
            }

            //absolute block opponent who is checkmating
            if (IsBeCheckmated(mine, opponent)
                && !IsBeCheckmated(row_1, col_1, row_2, col_2, mine, opponent))
            {
                defenceScore += EvaluateRate.completely_block_oppt_checkmate;
            }
            else
            {
                //reduce be checkmated point but player can still be checkmated
                int decreaseCheckmatedAmount
                    = GetNumOfDecreaseBeCheckmatedPoint(row_1, col_1, row_2, col_2, mine, opponent);
                if (decreaseCheckmatedAmount > 0)
                {
                    defenceScore += decreaseCheckmatedAmount * EvaluateRate.bonus_block_checkmate;
                }

            }

            //block all path move to plane
            if (IsAbsBlockToPlane(row_1, col_1, row_2, col_2, mine, opponent))
            {
                defenceScore += EvaluateRate.abs_block_move_to_player_plane;
            }
            else
            {
                //reduce path move to plane for one step opponent moves
                int decreasePathToPlaneAmount
                    = GetNumOfDecreasePathToPlane(row_1, col_1, row_2, col_2, mine, opponent);
                if (decreasePathToPlaneAmount > 0)
                {
                    defenceScore += decreasePathToPlaneAmount * EvaluateRate.bonus_block_move_to_plane;
                }
            }

            //move to path to oppt plane
            List<String> listPathToOpptPlane = GetListPathToOpptPlane(mine, opponent);
            if (listPathToOpptPlane.Contains("" + row_2 + col_2))
            {
                attackScore += EvaluateRate.path_to_oppt_plane;
            }

            //to checkmate oppt
            List<String> listPathToOpptWall = GetListPathToOpptWall(mine, opponent);
            if (listPathToOpptWall.Contains("" + row_2 + col_2))
            {
                attackScore += EvaluateRate.path_to_oppt_wall;
            }

            //eat opp horses
            if (IsEatenOppHorses(row_2, col_2, opponent))
            {
                attackScore += EvaluateRate.eat_opp_horse;
            }

            //if lost any horses
            if (IsLostHorse(row_2, col_2, opponent))
            {
                minusScore += EvaluateRate.lost_our_horse;
            }

            // oppt checkmate our plane            
            bool isBeCheckedPlaneBeforeMove = IsOpptCheckPlane(mine, opponent);
            bool isBeCheckedPlaneAfterMove = IsOpptCheckPlane(mine, opponent, move);
            if (isBeCheckedPlaneBeforeMove && !isBeCheckedPlaneAfterMove)
            {
                defenceScore += EvaluateRate.prevent_oppt_check_plane;
            }
            else if (!isBeCheckedPlaneBeforeMove && isBeCheckedPlaneAfterMove)
            {
                minusScore += EvaluateRate.fail_oppt_check_plane;
            }

            // oppt checkmate our wall
            bool isBeCheckedWallBeforeMove = IsOpptCheckWall(mine, opponent);
            bool isBlockCheckedWallAfterMove = IsOpptCheckWall(mine, opponent, move);
            if (isBeCheckedWallBeforeMove && !isBlockCheckedWallAfterMove)
            {
                defenceScore += EvaluateRate.prevent_oppt_check_wall;
            }
            else if (!isBeCheckedWallBeforeMove && isBlockCheckedWallAfterMove)
            {
                minusScore += EvaluateRate.fail_oppt_check_wall;
            }

            //conclusion
            int abs_eValue = attackScore + defenceScore - minusScore;
            int eValue = isMaximizePlayer ? abs_eValue : -abs_eValue;
            EBoardgame[row_2, col_2] = eValue;
        }

        /// <summary>
        /// before the ourPlayer moves
        /// </summary>
        /// <param name="ourPlayer"></param>
        /// <param name="oppt"></param>
        /// <returns></returns>
        private bool IsOpptCheckWall(Player ourPlayer, Player oppt)
        {
            bool result = false;

            List<Piece> listOurHorse = (ourPlayer == Player.human) ? listHumanHorse : listMachineHorse;
            List<String> listOpptMove = GetListPossibleMove(oppt);
            foreach (String move in listOpptMove)
            {
                //prepare params
                List<Object> paramSimulation = new List<Object>() { null, null, null, null };
                paramSimulation[move_index] = move;
                paramSimulation[our_player_index] = oppt;
                paramSimulation[oppt_index] = ourPlayer;
                paramSimulation[list_oppt_horse_index] = listOurHorse;
                //simulate
                List<Object> memorySimulation = SimulateMove(paramSimulation);

                //get the point to block checkmate
                List<String> listPoint = GetListNeedPointBlockCheckmate(ourPlayer, oppt);

                //unsimulate
                UnSimulateMove(memorySimulation);

                if (listPoint.Count > 0)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        private void UpdatePlayerHorsePos(Player ourPlayer, String move)
        {                        
            if (ourPlayer == Player.human) UpdateHumanHorsePos(move);
            else UpdateMachineHorsePos(move);
        }

        /// <summary>
        /// after the ourPlayer moved
        /// </summary>
        /// <param name="ourPlayer"></param>
        /// <param name="oppt"></param>
        /// <param name="our_move"></param>
        /// <returns></returns>
        private bool IsOpptCheckWall(Player ourPlayer, Player oppt, String our_move)
        {
            bool result = false;

            List<Piece> listOpptHorse = (oppt == Player.human) ? listHumanHorse : listMachineHorse;
            List<Piece> listOurHorse = (ourPlayer == Player.human) ? listHumanHorse : listMachineHorse;
            //prepare params           
            List<Object> paramSimulationOurPlayer = new List<Object>() { null, null, null, null };
            paramSimulationOurPlayer[move_index] = our_move;
            paramSimulationOurPlayer[our_player_index] = ourPlayer;
            paramSimulationOurPlayer[oppt_index] = oppt;
            paramSimulationOurPlayer[list_oppt_horse_index] = listOpptHorse;
            //simulate
            List<Object> memorySimulationOurPlayer = SimulateMove(paramSimulationOurPlayer);

            List<String> listOpptMove = GetListPossibleMove(oppt);
            foreach (String move in listOpptMove)
            {
                //prepare params
                List<Object> paramSimulation = new List<Object>() { null, null, null, null };
                paramSimulation[move_index] = move;
                paramSimulation[our_player_index] = oppt;
                paramSimulation[oppt_index] = ourPlayer;
                paramSimulation[list_oppt_horse_index] = listOurHorse;
                //simulate
                List<Object> memorySimulation = SimulateMove(paramSimulation);

                //get the point to block checkmate
                List<String> list = GetListNeedPointBlockCheckmate(ourPlayer, oppt);

                //unsimulate
                UnSimulateMove(memorySimulation);

                if (list.Count > 0)
                {
                    result = true;
                    break;
                }
            }

            //unsimulate
            UnSimulateMove(memorySimulationOurPlayer);

            return result;
        }

        /// <summary>
        /// before the ourPlayer moves
        /// </summary>
        /// <param name="ourPlayer"></param>
        /// <param name="oppt"></param>
        /// <returns></returns>
        private bool IsOpptCheckPlane(Player ourPlayer, Player oppt)
        {
            bool result = false;

            List<Piece> listOurHorse = (ourPlayer == Player.human) ? listHumanHorse : listMachineHorse;
            List<Piece> listOpptHorse = (oppt == Player.human) ? listHumanHorse : listMachineHorse;
            List<String> listOpptMove = GetListPossibleMove(oppt);
            foreach (String move in listOpptMove)
            {
                //prepare params
                List<Object> paramSimulation = new List<Object>() { null, null, null, null };
                paramSimulation[move_index] = move;
                paramSimulation[our_player_index] = oppt;
                paramSimulation[oppt_index] = ourPlayer;
                paramSimulation[list_oppt_horse_index] = listOurHorse;
                //simulate
                List<Object> memorySimulation = SimulateMove(paramSimulation);

                //get the point to block check plane                
                List<String> list = GetListNeedPointBlockToPlane(ourPlayer, oppt);

                //unsimulate
                UnSimulateMove(memorySimulation);

                if (list.Count > 0)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// after the ourPlayer moved
        /// </summary>
        /// <param name="ourPlayer"></param>
        /// <param name="oppt"></param>
        /// <param name="our_move"></param>
        /// <returns></returns>
        private bool IsOpptCheckPlane(Player ourPlayer, Player oppt, String our_move)
        {
            bool result = false;

            List<Piece> listOurHorse = (ourPlayer == Player.human) ? listHumanHorse : listMachineHorse;
            List<Piece> listOpptHorse = (oppt == Player.human) ? listHumanHorse : listMachineHorse;
            //prepare params           
            List<Object> paramSimulationOurPlayer = new List<Object>() { null, null, null, null };
            paramSimulationOurPlayer[move_index] = our_move;
            paramSimulationOurPlayer[our_player_index] = ourPlayer;
            paramSimulationOurPlayer[oppt_index] = oppt;
            paramSimulationOurPlayer[list_oppt_horse_index] = listOpptHorse;
            //simulate
            List<Object> memorySimulationOurPlayer = SimulateMove(paramSimulationOurPlayer);

            List<String> listOpptMove = GetListPossibleMove(oppt);
            foreach (String move in listOpptMove)
            {
                //prepare params
                List<Object> paramSimulation = new List<Object>() { null, null, null, null };
                paramSimulation[move_index] = move;
                paramSimulation[our_player_index] = oppt;
                paramSimulation[oppt_index] = ourPlayer;
                paramSimulation[list_oppt_horse_index] = listOurHorse;
                //simulate
                List<Object> memorySimulation = SimulateMove(paramSimulation);

                //get the point to block check plane                
                List<String> list = GetListNeedPointBlockToPlane(ourPlayer, oppt);

                //unsimulate
                UnSimulateMove(memorySimulation);

                if (list.Count > 0)
                {
                    result = true;
                    break;
                }
            }

            //unsimulate
            UnSimulateMove(memorySimulationOurPlayer);

            return result;
        }

        private List<String> GetListPathToOpptPlane(Player ourPlayer, Player oppt)
        {
            Piece opptPlane = (oppt == Player.human) ? plane_human : plane_machine;
            return GetListSrcToDes(ourPlayer, opptPlane.Row, opptPlane.Col);
        }

        private List<String> GetListPathToOpptWall(Player ourPlayer, Player oppt)
        {
            Piece opptWall = (oppt == Player.human) ? wall_human : wall_machine;
            return GetListSrcToDes(ourPlayer, opptWall.Row, opptWall.Col);
        }

        private List<String> GetListSrcToDes(Player ourPlayer, int oppt_des_row, int oppt_des_col)
        {
            List<String> list = new List<String>();
            int row;
            int col;

            row = oppt_des_row - 1;
            col = oppt_des_col + 2;
            if (IsValidMove(ourPlayer, row, col, oppt_des_row, oppt_des_col))
            {
                list.Add("" + row + col);
            }

            row = oppt_des_row - 2;
            col = oppt_des_col + 1;
            if (IsValidMove(ourPlayer, row, col, oppt_des_row, oppt_des_col))
            {
                list.Add("" + row + col);
            }

            row = oppt_des_row - 2;
            col = oppt_des_col - 1;
            if (IsValidMove(ourPlayer, row, col, oppt_des_row, oppt_des_col))
            {
                list.Add("" + row + col);
            }

            row = oppt_des_row - 1;
            col = oppt_des_col - 2;
            if (IsValidMove(ourPlayer, row, col, oppt_des_row, oppt_des_col))
            {
                list.Add("" + row + col);
            }

            row = oppt_des_row + 1;
            col = oppt_des_col - 2;
            if (IsValidMove(ourPlayer, row, col, oppt_des_row, oppt_des_col))
            {
                list.Add("" + row + col);
            }

            row = oppt_des_row + 2;
            col = oppt_des_col - 1;
            if (IsValidMove(ourPlayer, row, col, oppt_des_row, oppt_des_col))
            {
                list.Add("" + row + col);
            }

            row = oppt_des_row + 2;
            col = oppt_des_col + 1;
            if (IsValidMove(ourPlayer, row, col, oppt_des_row, oppt_des_col))
            {
                list.Add("" + row + col);
            }

            row = oppt_des_row + 1;
            col = oppt_des_col + 2;
            if (IsValidMove(ourPlayer, row, col, oppt_des_row, oppt_des_col))
            {
                list.Add("" + row + col);
            }

            return list;
        }

        private bool IsLostHorse(int row_2, int col_2, Player oppt)
        {
            List<String> listMoveOpp = GetListPossibleMove(oppt);
            foreach (String moveOpp in listMoveOpp)
            {
                if (mapCharToInt[moveOpp[2]] == row_2 && mapCharToInt[moveOpp[3]] == col_2)
                {
                    return true;
                }
            }

            return false;
        }

        public void PrintBoard()
        {
            Console.Write("    ");
            for (int cl = 0; cl < max_col; cl++) Console.Write((char)(cl + 'a') + "    ");
            Console.WriteLine("\n  -----------------------------------------");
            for (int rw = 0; rw < max_row; rw++)
            {
                Console.Write((max_row - rw) + " ");
                for (int cl = 0; cl < max_col; cl++)
                {
                    Console.Write("| " + boardgame[rw, cl] + " ");
                }
                Console.Write("|");
                Console.WriteLine("\n  -----------------------------------------");
            }
        }

        public bool IsValidMove(Player ourPlayer, int rw_0, int col_0, int rw_1, int col_1)
        {
            //guarantee position of the piece is not beyound the bound of gameboards
            if (rw_1 < 0 || rw_1 >= max_row || col_1 < 0 || col_1 >= max_col) return false;

            //guarantee you are moving a horse
            if (GetHorse(ourPlayer, rw_0, col_0) == null) return false;

            ////guarantee the position move to is valid place that means cannot move to oppenent horse because it cannot be eaten.
            //if (((player == Player.human)
            //    && !boardgame[rw_1, col_1].Equals(plane_machine.Label)
            //    && !boardgame[rw_1, col_1].Equals(wall_machine.Label)
            //    && !boardgame[rw_1, col_1].Equals(empty_str))
            //    ||
            //    ((player == Player.machine)
            //    && !boardgame[rw_1, col_1].Equals(plane_human.Label)
            //    && !boardgame[rw_1, col_1].Equals(wall_human.Label))
            //    && !boardgame[rw_1, col_1].Equals(empty_str))
            //{
            //    return false;
            //}

            ////guarantee the position move to is valid place. 
            //Cannot move to the player's wall, the plane or the horses.
            if ((ourPlayer == Player.human)
                && (rw_1 == wall_human.Row && col_1 == wall_human.Col
                || rw_1 == plane_human.Row && col_1 == plane_human.Col
                || listHumanHorse.Contains(GetHorse(Player.human, rw_1, col_1))
                || IsProtectedMachineWall() && (rw_1 == wall_machine.Row) && (col_1 == wall_machine.Col))
                ||
                (ourPlayer == Player.machine)
                && (rw_1 == wall_machine.Row && col_1 == wall_machine.Col
                || rw_1 == plane_machine.Row && col_1 == plane_machine.Col
                || IsProtectedHumanWall() && (rw_1 == wall_human.Row) && (col_1 == wall_human.Col)
                || listMachineHorse.Contains(GetHorse(Player.machine, rw_1, col_1))))
            {
                return false;
            }

            //guarantee not be checkmated after moving a horse to there
            //player == Player.machine to sure not get benefit for opponent but if training for two machine fight each other, we need del it.            
            //if (player == Player.machine)
            //{
            //    bool isCheckMated;
            //    String originLabel = boardgame[rw_0, col_0];
            //    String desLabel = boardgame[rw_1, col_1];
            //    boardgame[rw_1, col_1] = originLabel;
            //    if (rw_0 == plane_human.Row && col_0 == plane_human.Col)
            //    {
            //        boardgame[rw_0, col_0] = plane_human.Label;
            //    }
            //    else
            //    {
            //        boardgame[rw_0, col_0] = empty_str;
            //    }

            //    isCheckMated = IsDefeatOppt(currentPlayer);

            //    boardgame[rw_1, col_1] = desLabel;
            //    boardgame[rw_0, col_0] = originLabel;

            //    if (isCheckMated) return false;
            //}

            //if the horse from the plane of opponent then we don't need guarantee for 
            //the horse be or not be blocked and unlimit the horse path
            if ((ourPlayer == Player.human
                && rw_0 == plane_machine.Row
                && col_0 == plane_machine.Col)
                ||
                (ourPlayer == Player.machine
                && rw_0 == plane_human.Row
                && col_0 == plane_human.Col))
            {
                return true;
            }

            //the path does not belong to the horse
            if (!((Math.Abs(rw_1 - rw_0) == 1 && Math.Abs(col_1 - col_0) == 2)
                || (Math.Abs(rw_1 - rw_0) == 2 && Math.Abs(col_1 - col_0) == 1))) return false;

            //guarantee the moving in not be blocked by another horse and not be checkmated for 8 piece pos but group into 4
            int dis_row = rw_1 - rw_0;
            int dis_col = col_1 - col_0;
            //position #1 : 30 and -30 degree
            if ((dis_row == 1 || dis_row == -1) && dis_col == 2
                && (col_0 + 1) < max_col
                && !boardgame[rw_0, col_0 + 1].Equals(empty_str))
            {
                return false;
            }
            //position #2 : 60 and 120 degree
            if (dis_row == -2 && (dis_col == 1 || dis_col == -1)
                && (rw_0 - 1) > -1
                && !boardgame[rw_0 - 1, col_0].Equals(empty_str))
            {
                return true;
            }
            //position #3 : 150 and 210 degree
            if ((dis_row == 1 || dis_row == -1) && dis_col == -2
                && (col_0 - 1) > -1
                && !boardgame[rw_0, col_0 - 1].Equals(empty_str))
            {
                return false;
            }
            //position #4 : 240 and 300 degree
            if (dis_row == 2 && (dis_col == -1 || dis_col == 1)
                && (rw_0 + 1) < max_row
                && !boardgame[rw_0 + 1, col_0].Equals(empty_str))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// get a horse at the pos(row,col)
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private Piece GetHorse(int row, int col)
        {
            foreach (Piece p in listMachineHorse)
            {
                if (p.Row == row && p.Col == col)
                {
                    return p;
                }
            }

            foreach (Piece p in listHumanHorse)
            {
                if (p.Row == row && p.Col == col)
                {
                    return p;
                }
            }

            return null;
        }

        /// <summary>
        /// get a horse at the pos(row,col) of player
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private Piece GetHorse(Player player, int row, int col)
        {
            if (player == Player.machine)
            {
                foreach (Piece p in listMachineHorse)
                {
                    if (p.Row == row && p.Col == col)
                    {
                        return p;
                    }
                }
            }
            else
            {
                foreach (Piece p in listHumanHorse)
                {
                    if (p.Row == row && p.Col == col)
                    {
                        return p;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// li
        /// </summary>
        /// <param name="row_2"></param>
        /// <param name="col_2"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool IsEatenOppHorses(int row_2, int col_2, Player oppt)
        {
            List<Piece> listOppHorse = (oppt == Player.human) ? listHumanHorse : listMachineHorse;
            if (listOppHorse.Contains(GetHorse(row_2, col_2)))
            {
                return true;
            }

            return false;
        }

        public bool IsBeCheckmated(Player ourPlayer, Player oppt)
        {
            return GetListNeedPointBlockCheckmate(ourPlayer, oppt).Count != 0;
        }

        public bool IsBeCheckmated(int row_1, int col_1, int row_2, int col_2
            , Player ourPlayer, Player oppt)
        {
            //prepare params
            List<Object> paramSimulation = new List<Object>() { null, null, null, null };
            paramSimulation[move_index] = "" + row_1 + col_1 + row_2 + col_2;
            paramSimulation[our_player_index] = ourPlayer;
            paramSimulation[oppt_index] = oppt;
            List<Piece> listOpptHorse = (oppt == Player.human) ? listHumanHorse : listMachineHorse;
            paramSimulation[list_oppt_horse_index] = listOpptHorse;
            //simulate
            List<Object> memorySimulation = SimulateMove(paramSimulation);

            bool result = GetListNeedPointBlockCheckmate(ourPlayer, oppt).Count != 0;

            //unsimulate
            UnSimulateMove(memorySimulation);

            return result;
        }

        /// <summary>
        /// when ourPlayer move to row_2,col_2 then checkmate opptPlayer
        /// </summary>        
        public bool IsDefeatOppt(int row_2, int col_2
            , Player ourPlayer, Player opptPlayer)
        {
            Piece opptwall = (opptPlayer == Player.human) ? wall_human : wall_machine;
            return (row_2 == opptwall.Row) && (col_2 == opptwall.Col);
        }

        /// <summary>
        /// check whether player after move to the pos (row_2, col_2) can block all of opponent moves to its plane
        /// after player move to a specific pos (row,col)
        /// </summary>        
        public bool IsAbsBlockToPlane(int row_1, int col_1, int row_2, int col_2
            , Player ourPlayer, Player oppt)
        {
            int num_1 = GetListNeedPointBlockToPlane(ourPlayer, oppt).Count;

            //prepare params
            List<Object> paramSimulation = new List<Object>() { null, null, null, null };
            paramSimulation[move_index] = "" + row_1 + col_1 + row_2 + col_2;
            paramSimulation[our_player_index] = ourPlayer;
            paramSimulation[oppt_index] = oppt;
            List<Piece> listOpptHorse = (oppt == Player.human) ? listHumanHorse : listMachineHorse;
            paramSimulation[list_oppt_horse_index] = listOpptHorse;
            //simulate
            List<Object> memorySimulation = SimulateMove(paramSimulation);

            int num_2 = GetListNeedPointBlockToPlane(ourPlayer, oppt).Count;

            //unsimulate
            UnSimulateMove(memorySimulation);

            return (num_1 != 0) && (num_2 == 0);
        }

        /// <summary>
        /// check whether player after move to the pos (row_2, col_2) can block all of opponent moves to its plane
        /// after player move to a specific pos (row,col)
        /// </summary>        
        public int GetNumOfDecreasePathToPlane(int row_1, int col_1, int row_2, int col_2
            , Player ourPlayer, Player oppt)
        {
            //cal firstNumBlockPlane
            int firstNumBlockPlane = GetListNeedPointBlockToPlane(ourPlayer, oppt).Count;

            //prepare params
            List<Object> paramSimulation = new List<Object>() { null, null, null, null };
            paramSimulation[move_index] = "" + row_1 + col_1 + row_2 + col_2;
            paramSimulation[our_player_index] = ourPlayer;
            paramSimulation[oppt_index] = oppt;
            List<Piece> listOpptHorse = (oppt == Player.human) ? listHumanHorse : listMachineHorse;
            paramSimulation[list_oppt_horse_index] = listOpptHorse;
            //simulate
            List<Object> memorySimulation = SimulateMove(paramSimulation);

            int secondNumBlockPlane = GetListNeedPointBlockToPlane(ourPlayer, oppt).Count;

            //unsimulate
            UnSimulateMove(memorySimulation);

            return (firstNumBlockPlane - secondNumBlockPlane > 0)
                ? (firstNumBlockPlane - secondNumBlockPlane) : 0;
        }

        public int GetNumOfDecreaseBeCheckmatedPoint(int row_1, int col_1, int row_2, int col_2
            , Player ourPlayer, Player oppt)
        {
            //cal firstNumCheckmatedPoint
            int firstNumBlockPoint = GetListNeedPointBlockCheckmate(ourPlayer, oppt).Count;

            //prepare params
            List<Object> paramSimulation = new List<Object>() { null, null, null, null };
            paramSimulation[move_index] = "" + row_1 + col_1 + row_2 + col_2;
            paramSimulation[our_player_index] = ourPlayer;
            paramSimulation[oppt_index] = oppt;
            List<Piece> listOpptHorse = (oppt == Player.human) ? listHumanHorse : listMachineHorse;
            paramSimulation[list_oppt_horse_index] = listOpptHorse;
            //simulate
            List<Object> memorySimulation = SimulateMove(paramSimulation);

            //cal secondNumBlockPoint
            int secondNumBlockPoint = GetListNeedPointBlockCheckmate(ourPlayer, oppt).Count;

            //unsimulate
            UnSimulateMove(memorySimulation);

            return (firstNumBlockPoint - secondNumBlockPoint > 0)
                ? (firstNumBlockPoint - secondNumBlockPoint) : 0;
        }

        /// <summary>
        /// get list need block be checkmated point for the player
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private List<String> GetListNeedPointBlockCheckmate(Player ourPlayer, Player oppt)
        {
            Piece ourWall = (ourPlayer == Player.human) ? wall_human : wall_machine;
            List<String> listSrcToOurWall
                = GetListSrcToDes(oppt, ourWall.Row, ourWall.Col);

            List<String> listNeedPointBlock = new List<String>();

            foreach (String src in listSrcToOurWall)
            {
                int row_src = src[0] - '0';
                int col_src = src[1] - '0';
                if (ourWall.Row - row_src == 2)
                {
                    listNeedPointBlock.Add("" + (row_src + 1) + col_src);
                }
                else if (ourWall.Row - row_src == -2)
                {
                    listNeedPointBlock.Add("" + (row_src - 1) + col_src);
                }
                else if (ourWall.Col - col_src == 2)
                {
                    listNeedPointBlock.Add("" + row_src + (col_src + 1));
                }
                else if (ourWall.Col - col_src == -2)
                {
                    listNeedPointBlock.Add("" + row_src + (col_src - 1));
                }
            }

            return listNeedPointBlock;
        }
        /// get list move to the player plane in one more step
        /// </summary>

        /// <summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private List<String> GetListNeedPointBlockToPlane(Player ourPlayer, Player oppt)
        {
            Piece ourPlane = (ourPlayer == Player.human) ? plane_human : plane_machine;
            List<String> listSrcToOurPlane
                = GetListSrcToDes(oppt, ourPlane.Row, ourPlane.Col);

            List<String> listNeedPointBlock = new List<String>();

            foreach (String src in listSrcToOurPlane)
            {
                int row_src = src[0] - '0';
                int col_src = src[1] - '0';
                if (ourPlane.Row - row_src == 2)
                {
                    listNeedPointBlock.Add("" + (row_src + 1) + col_src);
                }
                else if (ourPlane.Row - row_src == -2)
                {
                    listNeedPointBlock.Add("" + (row_src - 1) + col_src);
                }
                else if (ourPlane.Col - col_src == 2)
                {
                    listNeedPointBlock.Add("" + row_src + (col_src + 1));
                }
                else if (ourPlane.Col - col_src == -2)
                {
                    listNeedPointBlock.Add("" + row_src + (col_src - 1));
                }
            }

            return listNeedPointBlock;
        }


        /// <summary>
        /// check how many opponent's horse will be blocked after player move its horse to a specific position (row,col)
        /// </summary>
        /// <param name = "row" > row </ param >
        /// < param name="col">column</param>
        /// <param name = "player" > player who move the horse</param>
        /// <returns></returns>
        //private int CountBlockOpponent(int row, int col, Player player)
        //{
        //    int numBlockedOpptHorse = 0;

        //    if (player == Player.human)
        //    {
        //        foreach (Piece horse in listMachineHorse)
        //        {
        //            if (col == horse.Col)
        //            {
        //                if (row - horse.Row == 1 && max_row - horse.Row > 2) numBlockedOpptHorse++;
        //                else if (row - horse.Row == -1 && horse.Row > 1) numBlockedOpptHorse++;
        //            }
        //            else if (row == horse.Row)
        //            {
        //                if (col - horse.Col == 1 && max_col - horse.Col > 2) numBlockedOpptHorse++;
        //                else if (col - horse.Col == -1 && horse.Col > 1) numBlockedOpptHorse++;
        //            }
        //        }
        //    }
        //    else if (player == Player.machine)
        //    {
        //        foreach (Piece horse in listHumanHorse)
        //        {
        //            if (col == horse.Col)
        //            {
        //                if (row - horse.Row == 1 && max_row - horse.Row > 2) numBlockedOpptHorse++;
        //                else if (row - horse.Row == -1 && horse.Row > 1) numBlockedOpptHorse++;
        //            }
        //            else if (row == horse.Row)
        //            {
        //                if (col - horse.Col == 1 && max_col - horse.Col > 2) numBlockedOpptHorse++;
        //                else if (col - horse.Col == -1 && horse.Col > 1) numBlockedOpptHorse++;
        //            }
        //        }
        //    }

        //    return numBlockedOpptHorse;
        //}

        /// <summary>
        /// check whether a player move its horse to a specific pos (row,col) could be in opponent plane or not
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool IsInOpptPlane(int row, int col, Player oppt)
        {
            if (oppt == Player.human)
            {
                return row == plane_human.Row && col == plane_human.Col;
            }
            else
            {
                return row == plane_machine.Row && col == plane_machine.Col;
            }
        }


        ///// <summary>
        ///// get list block checkmate for the player after one moving from (row_1,col_1) to (row_2,col_2)
        ///// </summary>
        ///// <param name="row_1"></param>
        ///// <param name="col_1"></param>
        ///// <param name="row_2"></param>
        ///// <param name="col_2"></param>
        ///// <param name="player"></param>
        ///// <returns></returns>
        //public List<String> GetListSecondBlockCheckmate(int row_1, int col_1, int row_2, int col_2, Player player)
        //{
        //    //simulate the horse moves...
        //    String originLabel = boardgame[row_1, col_1];
        //    String desLabel = boardgame[row_2, col_2];
        //    boardgame[row_2, col_2] = originLabel;
        //    if (row_1 == plane_human.Row && col_1 == plane_human.Col)
        //    {
        //        boardgame[row_1, col_1] = plane_human.Label;
        //    }
        //    else
        //    {
        //        boardgame[row_1, col_1] = empty_str;
        //    }

        //    List<String> listNdBlockCheckmate = GetListFirstBlockCheckmate(player);

        //    boardgame[row_2, col_2] = desLabel;
        //    boardgame[row_1, col_1] = originLabel;

        //    return listNdBlockCheckmate;
        //}

        /// <summary>
        /// check status of current gameboard: win - lose - raw
        /// </summary>        
        public int CheckGameBoard()
        {
            //if (boardgame[O_human.Row, O_human.Col].Equals(wall_machine.Label)
            //   || boardgame[P_human.Row, P_human.Col].Equals(wall_machine.Label)
            //   || boardgame[Q_human.Row, Q_human.Col].Equals(wall_machine.Label)
            //   || num_machine_horse == 0)
            //{
            //    if (currentPlayer == Player.machine) return lose_number;
            //    if (currentPlayer == Player.human) return win_number;
            //}

            //if (boardgame[K_machine.Row, K_machine.Col].Equals(wall_human.Label)
            //    || boardgame[L_machine.Row, L_machine.Col].Equals(wall_human.Label)
            //    || boardgame[M_machine.Row, M_machine.Col].Equals(wall_human.Label)
            //    || num_human_horse == 0)
            //{
            //    if (currentPlayer == Player.machine) return win_number;
            //    if (currentPlayer == Player.human) return lose_number;
            //}

            if (O_human.Row == wall_machine.Row && O_human.Col == wall_machine.Col
               || P_human.Row == wall_machine.Row && P_human.Col == wall_machine.Col
               || Q_human.Row == wall_machine.Row && Q_human.Col == wall_machine.Col
               || listMachineHorse.Count == 0)
            {
                if (currentPlayer == Player.machine) return lose_number;
                if (currentPlayer == Player.human) return win_number;
            }

            if (K_machine.Row == wall_human.Row && K_machine.Col == wall_human.Col
                || L_machine.Row == wall_human.Row && L_machine.Col == wall_human.Col
                || M_machine.Row == wall_human.Row && M_machine.Col == wall_human.Col
                || listHumanHorse.Count == 0)
            {
                if (currentPlayer == Player.machine) return win_number;
                if (currentPlayer == Player.human) return lose_number;
            }

            if (GetListPossibleMove(Player.human).Count == 0
                && GetListPossibleMove(Player.machine).Count == 0) return raw_number;

            return live_game_number;
        }

        public String GetStatusMsg(int status)
        {
            String currentPlayerStr = (currentPlayer == Player.human) ? player_human_str : player_machine_str;

            if (status == raw_number) return "Hoa";
            if (status == win_number) return currentPlayerStr + " thang";
            if (status == lose_number) return currentPlayerStr + " thua";
            return "";
        }

        //max_inf start from -inf for maximize player (machine)
        public const int max_inf = -Int32.MaxValue;
        //min_inf start from inf for minimize player (human)
        public const int min_inf = Int32.MaxValue;
        //max depth
        public int max_depth = 0;

        /// <summary>
        /// alpha for maximize player (machine)
        /// </summary>
        private class Alpha
        {
            public int max = max_inf;
        }

        /// <summary>
        /// beta for minimize player (human)
        /// </summary>
        private class Beta
        {
            public int min = min_inf;
        }

        /// <summary>
        /// prepare for generateMove and getFinalMove
        /// <param name="Player"></param>
        /// </summary>        
        public String GetMachineMove(Player player)
        {
            //determine getMove for which player            
            bool isMaximizePlayer = (player == Player.machine) ? true : false;
            int depth = 0;
            String finalMove = raw_number + "" + raw_number + "" + raw_number + "" + raw_number;
            GenerateMove(null, depth, isMaximizePlayer, new Alpha(), new Beta(), ref finalMove);
            return finalMove;
        }

        /// <summary>
        /// use minimax to generateMove.
        /// </summary>
        private int GenerateMove(String node, int depth, bool isMaximizePlayer,
            Alpha alpha, Beta beta, ref String finalMove)
        {
            //machine is maximize player
            Player ourPlayer = isMaximizePlayer ? Player.machine : Player.human;
            Player oppt = isMaximizePlayer ? Player.human : Player.machine;

            //check end condition for the resurcive func
            List<String> possibleMoves = GetListPossibleMove(ourPlayer);
            if (depth == max_depth || possibleMoves.Count == 0)
            {
                //CalEBoard for a specific postion (row,col) after simulate the horse moving to there               
                CalEBoard(isMaximizePlayer, node);

                //when all horses were eaten by the opponent.                
                return (depth == 0) ? lose_number : EBoardgame[mapCharToInt[(char)node[2]], mapCharToInt[(char)node[3]]];
            }

            int bestVal, value;

            //machine turn
            if (isMaximizePlayer)
            {
                bool isSimulated = true;
                bestVal = max_inf; //-Int32.MaxValue; 
                foreach (String p in possibleMoves)
                {
                    //if the node is leave, don't need to simulate
                    if (depth == max_depth - 1
                        || GetListPossibleMove(ourPlayer, oppt, p).Count == 0)
                    {
                        isSimulated = false;
                    }

                    String originLabel = "";
                    String desLabel = "";
                    int p_0 = -1;
                    int p_1 = -1;
                    int p_2 = -1;
                    int p_3 = -1;
                    bool nextMoveInOppHorse = false;
                    Piece opptHorse = null;

                    if (isSimulated)
                    {
                        //convert char to int for p
                        p_0 = mapCharToInt[(char)p[0]];
                        p_1 = mapCharToInt[(char)p[1]];
                        p_2 = mapCharToInt[(char)p[2]];
                        p_3 = mapCharToInt[(char)p[3]];

                        //update machine horse position
                        UpdateMachineHorsePos("" + p_0 + p_1 + p_2 + p_3);

                        //check whether (p_2,p_3) in opponent(human) horse
                        opptHorse = GetHorse(Player.human, p_2, p_3);
                        nextMoveInOppHorse = listHumanHorse.Contains(opptHorse);

                        //case eat opponent(human) horse
                        if (nextMoveInOppHorse) listHumanHorse.Remove(opptHorse);

                        //simulate the horse moves
                        originLabel = boardgame[p_0, p_1];
                        desLabel = boardgame[p_2, p_3];
                        boardgame[p_2, p_3] = originLabel;
                        if (p_0 == plane_human.Row && p_1 == plane_human.Col)
                        {
                            boardgame[p_0, p_1] = plane_human.Label;
                        }
                        else
                        {
                            boardgame[p_0, p_1] = empty_str;
                        }
                    }

                    //generate next move
                    if (isSimulated)
                    {
                        value = GenerateMove(p, depth + 1, false, alpha, new Beta(), ref finalMove);
                    }
                    else
                    {
                        value = GenerateMove(p, depth + 1, true, alpha, new Beta(), ref finalMove);
                    }

                    if (isSimulated)
                    {
                        //revert simulation
                        boardgame[p_2, p_3] = desLabel;
                        boardgame[p_0, p_1] = originLabel;
                        //revert case eat opponent(human) horse
                        if (nextMoveInOppHorse) listHumanHorse.Add(opptHorse);

                        //update machine horse position
                        UpdateMachineHorsePos("" + p_2 + p_3 + p_0 + p_1);
                    }

                    //calculate maximum value for maximize player
                    bestVal = Math.Max(value, bestVal);
                    if (depth == 0)
                    {
                        Debug.WriteLine(alpha.max < bestVal);
                    }
                    //update final move when alpha changed
                    if (alpha.max < bestVal)
                    {
                        alpha.max = bestVal;
                        if (depth == 0)
                        {
                            finalMove = p[0] + "" + p[1] + "" + p[2] + "" + p[3];
                        }
                    }
                    if (beta.min <= alpha.max) break;
                }
                return bestVal;
            }
            //human turn
            else
            {
                bestVal = min_inf;
                bool isSimulated = true;

                foreach (String p in possibleMoves)
                {
                    //if the node is leave, don't need to simulate
                    if (depth == max_depth - 1
                        || GetListPossibleMove(ourPlayer, oppt, p).Count == 0)
                    {
                        isSimulated = false;
                    }

                    String originLabel = "";
                    String desLabel = "";
                    int p_0 = -1;
                    int p_1 = -1;
                    int p_2 = -1;
                    int p_3 = -1;
                    bool nextMoveInOppHorse = false;
                    Piece oppHorse = null;

                    if (isSimulated)
                    {
                        //convert char to int for p
                        p_0 = mapCharToInt[(char)p[0]];
                        p_1 = mapCharToInt[(char)p[1]];
                        p_2 = mapCharToInt[(char)p[2]];
                        p_3 = mapCharToInt[(char)p[3]];

                        //update human horse position
                        UpdateHumanHorsePos("" + p_0 + p_1 + p_2 + p_3);

                        //check whether (p_2,p_3) in opponent(machine) horse
                        oppHorse = GetHorse(Player.machine, p_2, p_3);
                        nextMoveInOppHorse = listMachineHorse.Contains(oppHorse);

                        //case eat opponent(machine) horse
                        if (nextMoveInOppHorse) listMachineHorse.Remove(oppHorse);

                        //simulate the horse moves
                        originLabel = boardgame[p_0, p_1];
                        desLabel = boardgame[p_2, p_3];
                        boardgame[p_2, p_3] = originLabel;
                        if (p_0 == plane_machine.Row && p_1 == plane_machine.Col)
                        {
                            boardgame[p_0, p_1] = plane_machine.Label;
                        }
                        else
                        {
                            boardgame[p_0, p_1] = empty_str;
                        }
                    }

                    //generate next move
                    if (isSimulated)
                    {
                        value = GenerateMove(p, depth + 1, true, new Alpha(), beta, ref finalMove);
                    }
                    else
                    {
                        value = GenerateMove(p, depth + 1, false, new Alpha(), beta, ref finalMove);
                    }

                    if (isSimulated)
                    {
                        //revert simulation
                        boardgame[p_2, p_3] = desLabel;
                        boardgame[p_0, p_1] = originLabel;

                        //revert case eat opponent horse
                        if (nextMoveInOppHorse) listMachineHorse.Add(oppHorse);

                        //update human horse position
                        UpdateHumanHorsePos("" + p_2 + p_3 + p_0 + p_1);
                    }

                    //calculate minimum value for minimize player
                    bestVal = Math.Min(value, min_inf);

                    //update final move when alpha changed
                    if (beta.min > bestVal)
                    {
                        beta.min = bestVal;
                        if (depth == 0)
                        {
                            finalMove = p[0] + "" + p[1] + "" + p[2] + "" + p[3];
                        }
                    }
                    if (beta.min <= alpha.max) break;
                }
                return bestVal;
            }
        }

        /// <summary>
        /// get all possible moves with condition checkmate
        /// </summary>
        /// <param name="Player">list posible moves of this player</param>
        /// <returns></returns>
        private List<String> GetListPossibleMove(Player player)
        {
            List<String> mlistMove = new List<String>();

            if (player == Player.human)
            {
                //if wall is eaten or no more horse to move
                if (listMachineHorse.Contains(GetHorse(Player.machine, wall_human.Row, wall_human.Col))
                    || listHumanHorse.Count == 0) return mlistMove;

                foreach (Piece p in listHumanHorse)
                {
                    if (IsInOpptPlane(p.Row, p.Col, Player.machine))
                    {
                        for (int i = 0; i < max_row; i++)
                            for (int j = 0; j < max_col; j++)
                            {
                                //empty place or opponent horse place                            
                                if (boardgame[i, j].Equals(empty_str) 
                                    ||
                                    !(wall_machine.Row == i && wall_machine.Col == j)
                                    &&  listMachineHorse.Contains(GetHorse(Player.machine, i, j)))
                                {
                                    mlistMove.Add("" + p.Row + p.Col + i + j);
                                }
                            }
                    }
                    else
                    {
                        if (IsValidMove(player, p.Row, p.Col, p.Row - 1, p.Col + 2)) mlistMove.Add("" + p.Row + p.Col + (p.Row - 1) + (p.Col + 2));
                        if (IsValidMove(player, p.Row, p.Col, p.Row - 2, p.Col + 1)) mlistMove.Add("" + p.Row + p.Col + (p.Row - 2) + (p.Col + 1));
                        if (IsValidMove(player, p.Row, p.Col, p.Row - 2, p.Col - 1)) mlistMove.Add("" + p.Row + p.Col + (p.Row - 2) + (p.Col - 1));
                        if (IsValidMove(player, p.Row, p.Col, p.Row - 1, p.Col - 2)) mlistMove.Add("" + p.Row + p.Col + (p.Row - 1) + (p.Col - 2));
                        if (IsValidMove(player, p.Row, p.Col, p.Row + 1, p.Col - 2)) mlistMove.Add("" + p.Row + p.Col + (p.Row + 1) + (p.Col - 2));
                        if (IsValidMove(player, p.Row, p.Col, p.Row + 2, p.Col - 1)) mlistMove.Add("" + p.Row + p.Col + (p.Row + 2) + (p.Col - 1));
                        if (IsValidMove(player, p.Row, p.Col, p.Row + 2, p.Col + 1)) mlistMove.Add("" + p.Row + p.Col + (p.Row + 2) + (p.Col + 1));
                        if (IsValidMove(player, p.Row, p.Col, p.Row + 1, p.Col + 2)) mlistMove.Add("" + p.Row + p.Col + (p.Row + 1) + (p.Col + 2));
                    }
                }
            }
            else if (player == Player.machine)
            {
                //if wall is eaten or no more horse to move
                if (listHumanHorse.Contains(GetHorse(Player.human, wall_machine.Row, wall_machine.Col))
                    || listMachineHorse.Count == 0) return mlistMove;

                foreach (Piece p in listMachineHorse)
                {
                    if (IsInOpptPlane(p.Row, p.Col, Player.human))
                    {
                        for (int i = 0; i < max_row; i++)
                            for (int j = 0; j < max_col; j++)
                            {
                                //empty place or opponent horse place
                                if (boardgame[i, j].Equals(empty_str) 
                                    ||
                                    !(wall_human.Row == i && wall_human.Col == j)
                                    && listHumanHorse.Contains(GetHorse(Player.human, i, j)))
                                {
                                    mlistMove.Add("" + p.Row + p.Col + i + j);
                                }
                            }
                    }
                    else
                    {
                        if (IsValidMove(player, p.Row, p.Col, p.Row - 1, p.Col + 2)) mlistMove.Add("" + p.Row + p.Col + (p.Row - 1) + (p.Col + 2));
                        if (IsValidMove(player, p.Row, p.Col, p.Row - 2, p.Col + 1)) mlistMove.Add("" + p.Row + p.Col + (p.Row - 2) + (p.Col + 1));
                        if (IsValidMove(player, p.Row, p.Col, p.Row - 2, p.Col - 1)) mlistMove.Add("" + p.Row + p.Col + (p.Row - 2) + (p.Col - 1));
                        if (IsValidMove(player, p.Row, p.Col, p.Row - 1, p.Col - 2)) mlistMove.Add("" + p.Row + p.Col + (p.Row - 1) + (p.Col - 2));
                        if (IsValidMove(player, p.Row, p.Col, p.Row + 1, p.Col - 2)) mlistMove.Add("" + p.Row + p.Col + (p.Row + 1) + (p.Col - 2));
                        if (IsValidMove(player, p.Row, p.Col, p.Row + 2, p.Col - 1)) mlistMove.Add("" + p.Row + p.Col + (p.Row + 2) + (p.Col - 1));
                        if (IsValidMove(player, p.Row, p.Col, p.Row + 2, p.Col + 1)) mlistMove.Add("" + p.Row + p.Col + (p.Row + 2) + (p.Col + 1));
                        if (IsValidMove(player, p.Row, p.Col, p.Row + 1, p.Col + 2)) mlistMove.Add("" + p.Row + p.Col + (p.Row + 1) + (p.Col + 2));
                    }
                }
            }
            return mlistMove;
        }

        /// <summary>
        /// get List posible move when simulate move a horse to a position
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private List<String> GetListPossibleMove(Player ourPlayer, Player oppt, String p)
        {
            //prepare params
            //prepare params
            List<Object> paramSimulation = new List<Object>() { null, null, null, null };
            paramSimulation[move_index] = p;
            paramSimulation[our_player_index] = ourPlayer;
            paramSimulation[oppt_index] = oppt;
            List<Piece> listOpptHorse = (oppt == Player.human) ? listHumanHorse : listMachineHorse;
            paramSimulation[list_oppt_horse_index] = listOpptHorse;
            //simulate
            List<Object> memorySimulation = SimulateMove(paramSimulation);

            //get list posible move
            List<String> listPosibleMove = GetListPossibleMove(ourPlayer);

            //unsimulate
            UnSimulateMove(memorySimulation);

            return listPosibleMove;
        }

        /// <summary>
        /// opponent move don't need restrict condition isValidMove
        /// </summary>
        /// <returns></returns>
        public String GetHumanMove()
        {
            Console.Write("Wating your move (ex. 2a3h): ");
            String move;
            while (true)
            {
                move = Console.ReadLine();
                if (move.Length != 4
                    || !IsDigit(move[0]) || !IsAlphabet(move[1])
                    || !IsDigit(move[2]) || !IsAlphabet(move[3])
                    || !IsValidMove(Player.human, mapId[move[0]], mapId[move[1]], mapId[move[2]], mapId[move[3]]))
                {
                    Console.Write("Error command! Please try again: ");
                }
                else
                {
                    break;
                }
            }

            //value of row and col in integer
            return "" + mapId[move[0]] + mapId[move[1]] + mapId[move[2]] + mapId[move[3]];
        }

        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private bool IsAlphabet(char c)
        {
            return c >= 'a' && c <= 'h';
        }

        /// <summary>
        /// game loop
        /// </summary>        
        public void GameBoard()
        {
            ResetBoardGame();
            InitListHorses();
            InitmapId();
            InitmapCharCol();
            InitmapCharRow();
            InitmapCharToInt();
            InitFirstStep();
            max_depth = 1;
            limit_step = 20;

            while (true)
            {
                if (currentPlayer == Player.human)
                {
                    String oldMove = "";
                    String move = "";
                    int count = 2;
                    while (count-- > 0)
                    {
                        do
                        {
                            move = GetHumanMove();
                        } while (oldMove.Equals(move));
                        UpdateHumanHorsePos(move);
                        oldMove = move;

                        //covert char to int
                        int move_0 = mapCharToInt[(char)move[0]];
                        int move_1 = mapCharToInt[(char)move[1]];
                        int move_2 = mapCharToInt[(char)move[2]];
                        int move_3 = mapCharToInt[(char)move[3]];

                        //check whether eat machine(opponent) horse
                        Piece oppMachineHorse = GetHorse(Player.machine, move_2, move_3);
                        //bool isAteOppHorse = listMachineHorse.Contains(oppMachineHorse);
                        //if (isAteOppHorse) listMachineHorse.Remove(oppMachineHorse);
                        listMachineHorse.Remove(oppMachineHorse);

                        boardgame[move_2, move_3] = boardgame[move_0, move_1];
                        if (move_0 == plane_machine.Row && move_1 == plane_machine.Col)
                        {
                            boardgame[move_0, move_1] = plane_machine.Label;
                        }
                        else if (move_0 == wall_human.Row && move_1 == wall_human.Col)
                        {
                            boardgame[move_0, move_1] = wall_human.Label;
                        }
                        else
                        {
                            boardgame[move_0, move_1] = empty_str;
                        }

                        //print boardgame
                        PrintBoard();

                        //debug
                        PrintPosAllHorses();

                        //check gameboard finish or not
                        int statusGameBoard = CheckGameBoard();
                        if (statusGameBoard != live_game_number)
                        {
                            Console.WriteLine(GetStatusMsg(statusGameBoard));
                            Console.ReadLine();
                            return;
                        }
                    }
                    currentPlayer = Player.machine;
                }
                else if (currentPlayer == Player.machine)
                {
                    String oldMove = "";
                    String move = "";
                    int count = 2;
                    while (count-- > 0)
                    {
                        do
                        {
                            move = GetMachineMove(Player.machine);
                        } while (oldMove.Equals(move));                        
                        UpdateMachineHorsePos(move);
                        oldMove = move;

                        //covert char to int
                        int move_0 = mapCharToInt[(char)move[0]];
                        int move_1 = mapCharToInt[(char)move[1]];
                        int move_2 = mapCharToInt[(char)move[2]];
                        int move_3 = mapCharToInt[(char)move[3]];

                        //check whether eat human(opponent) horse
                        Piece oppHumanHorse = GetHorse(Player.human, move_2, move_3);
                        //bool isAteOppHorse = listHumanHorse.Contains(oppHumanHorse);
                        //if (isAteOppHorse) listHumanHorse.Remove(oppHumanHorse);
                        listHumanHorse.Remove(oppHumanHorse);

                        boardgame[move_2, move_3] = boardgame[move_0, move_1];
                        if (move_0 == plane_human.Row && move_1 == plane_human.Col)
                        {
                            boardgame[move_0, move_1] = plane_human.Label;
                        }
                        else if (move_0 == wall_machine.Row && move_1 == wall_machine.Col)
                        {
                            boardgame[move_0, move_1] = wall_machine.Label;
                        }
                        else
                        {
                            boardgame[move_0, move_1] = empty_str;
                        }

                        //print boardgame
                        PrintBoard();

                        //debug
                        PrintPosAllHorses();

                        //check gameboard finish or not
                        int statusGameBoard = CheckGameBoard();
                        if (statusGameBoard != live_game_number)
                        {
                            Console.WriteLine(GetStatusMsg(statusGameBoard));
                            Console.ReadLine();
                            return;
                        }
                    }
                    currentPlayer = Player.human;
                }

                num_step = (num_step + 1) % 11;
                if (num_step == limit_step)
                {
                    max_depth = (max_depth + 2) % 6;
                }
            }
        }

        private bool IsProtectedMachineWall()
        {
            foreach (Piece horse in listMachineHorse)
            {
                if (horse.Col == wall_machine.Col && horse.Row == wall_machine.Row)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsProtectedHumanWall()
        {
            foreach (Piece horse in listHumanHorse)
            {
                if (horse.Col == wall_human.Col && horse.Row == wall_human.Row)
                {
                    return true;
                }
            }
            return false;
        }

        //debug
        private void PrintPosAllHorses()
        {
            foreach (Piece p in listHumanHorse)
            {
                Console.WriteLine(p.Label + " = (" + p.Row + "," + p.Col + ")");
            }
            foreach (Piece p in listMachineHorse)
            {
                Console.WriteLine(p.Label + " = (" + p.Row + "," + p.Col + ")");
            }
        }

        private void UpdateMachineHorsePos(String move)
        {
            foreach (Piece p in listMachineHorse)
            {
                if (p.Row == mapCharToInt[(char)move[0]] && p.Col == mapCharToInt[(char)move[1]])
                {
                    p.Row = mapCharToInt[(char)move[2]];
                    p.Col = mapCharToInt[(char)move[3]];
                    break;
                }
            }
        }

        private void UpdateHumanHorsePos(String move)
        {
            foreach (Piece p in listHumanHorse)
            {
                if (p.Row == mapCharToInt[(char)move[0]] && p.Col == mapCharToInt[(char)move[1]])
                {
                    p.Row = mapCharToInt[(char)move[2]];
                    p.Col = mapCharToInt[(char)move[3]];
                    break;
                }
            }
        }

        //consider...
        private void InitFirstStep()
        {
            PrintBoard();

            Console.WriteLine("\nNguoi danh truoc (nhan 1) hoac May danh truoc (nhan 2)");
            Console.Write("Wating your command: ");
            int firstPlayerId;
            while (true)
            {
                try
                {
                    firstPlayerId = Convert.ToInt32(Console.ReadLine());
                    if (firstPlayerId > 0 || firstPlayerId < 3) break;
                }
                catch (Exception ex) { }

                Console.Write("[Error] command! Please try again: ");
            }

            currentPlayer = (firstPlayerId == (int)Player.machine) ? Player.machine : Player.human;
        }

        public static void Main(String[] args)
        {
            Program cochiemthanh = new Program();
            cochiemthanh.GameBoard();
        }
    }
}
