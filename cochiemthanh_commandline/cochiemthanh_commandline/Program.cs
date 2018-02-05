using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace cochiemthanh_commandline
{
    public class Program
    {
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
            mapCharCol.Add(6, 'g');
            mapCharCol.Add(5, 'f');
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

        //empty means having neither machine's troop or human's troop
        public const String empty_str = "  ";

        public const int max_row = 8;
        public const int max_col = 8;

        public Piece wall_human = null;
        public Piece wall_machine = null;
        public Piece plane_human = null;
        public Piece plane_machine = null;

        //human's horses        
        public Piece O_human = null;
        public Piece P_human = null;
        public Piece Q_human = null;

        //machine's horses        
        public Piece K_machine = null;
        public Piece L_machine = null;
        public Piece M_machine = null;

        public List<Piece> listMachineHorse = new List<Piece>();
        public List<Piece> listHumanHorse = new List<Piece>();

        public void InitListHorses(bool isMachineFirst)
        {
            if (isMachineFirst)
            {
                wall_human = new Piece(7, 3, "TH");
                wall_machine = new Piece(0, 4, "TH");

                plane_human = new Piece(7, 4, "MB");
                plane_machine = new Piece(0, 3, "MB");

                O_human = new Piece(7, 0, "O ");
                P_human = new Piece(7, 3, "P ");
                Q_human = new Piece(7, 7, "Q ");

                K_machine = new Piece(0, 0, "K ");
                L_machine = new Piece(0, 4, "L ");
                M_machine = new Piece(0, 7, "M ");
            }
            else
            {
                wall_human = new Piece(0, 4, "TH");
                wall_machine = new Piece(7, 3, "TH");

                plane_human = new Piece(0, 3, "MB");
                plane_machine = new Piece(7, 4, "MB");

                O_human = new Piece(0, 0, "O ");
                P_human = new Piece(0, 4, "P ");
                Q_human = new Piece(0, 7, "Q ");

                K_machine = new Piece(7, 0, "K ");
                L_machine = new Piece(7, 3, "L ");
                M_machine = new Piece(7, 7, "M ");
            }

            //list human horses
            listHumanHorse.Add(O_human);
            listHumanHorse.Add(P_human);
            listHumanHorse.Add(Q_human);

            //list machine horses
            listMachineHorse.Add(K_machine);
            listMachineHorse.Add(L_machine);
            listMachineHorse.Add(M_machine);

            ResetBoardGame();
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

            if (IsInPlane(row_1, col_1, Player.human) || IsInPlane(row_1, col_1, Player.machine))
            {
                string plane_signal = plane_machine.Label;
                boardgame[row_1, col_1] = plane_signal;
            }
            else if (IsInWall(row_1, col_1, Player.human) || IsInWall(row_1, col_1, Player.machine))
            {
                string wall_signal = wall_machine.Label;
                boardgame[row_1, col_1] = wall_signal;
            }
            else
            {
                boardgame[row_1, col_1] = empty_str;
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
            if (IsInPlane(row_2, col_2, opponent))
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
                defenceScore += EvaluateRate.abs_block_move_to_our_plane;
            }
            else
            {
                //reduce path move to plane for one step opponent moves
                int decreasePathToPlaneAmount
                    = GetNumOfDecreasePathToPlane(row_1, col_1, row_2, col_2, mine, opponent);
                if (decreasePathToPlaneAmount > 0)
                {
                    defenceScore += decreasePathToPlaneAmount 
                        * EvaluateRate.bonus_block_move_to_plane;
                }
            }

            //move to path of oppt plane
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
            int num_can_cplane_1 = NumPointOpptCanCheckPlane(mine, opponent);
            int num_can_cplane_2 = NumPointOpptCanCheckPlane(mine, opponent, move);
            bool isCanCheckedPlaneBeforeMove = num_can_cplane_1 > 0;
            bool isCanCheckedPlaneAfterMove = num_can_cplane_2 > 0;
            if (isCanCheckedPlaneBeforeMove && !isCanCheckedPlaneAfterMove)
            {
                defenceScore += EvaluateRate.prevent_oppt_check_plane;
            }
            else if (!isCanCheckedPlaneBeforeMove && isCanCheckedPlaneAfterMove)
            {
                minusScore += EvaluateRate.fail_block_oppt_check_plane;
            }
            else if (num_can_cplane_1 - num_can_cplane_2 > 0)
            {
                defenceScore += EvaluateRate.bonus_block_path_to_check_plane;
            }
            else if (num_can_cplane_1 - num_can_cplane_2 < 0)
            {
                minusScore += EvaluateRate.minus_block_path_to_plane;
            }

            // oppt checkmate our wall
            int num_can_checkmate_1 = NumPointOpptCanCheckWall(mine, opponent);
            int num_can_checkmate_2 = NumPointOpptCanCheckWall(mine, opponent, move);
            bool isCanCheckedWallBeforeMove = num_can_checkmate_1 > 0;
            bool isCanCheckedWallAfterMove = num_can_checkmate_2 > 0;
            if (isCanCheckedWallBeforeMove && !isCanCheckedWallAfterMove)
            {
                defenceScore += EvaluateRate.prevent_oppt_check_wall;
            }
            else if (!isCanCheckedWallBeforeMove && isCanCheckedWallAfterMove)
            {
                minusScore += EvaluateRate.fail_block_oppt_check_wall;
            }
            else if (num_can_checkmate_1 - num_can_checkmate_2 > 0)
            {
                defenceScore += EvaluateRate.bonus_block_path_to_checkmate;
            }
            else if (num_can_checkmate_1 - num_can_checkmate_2 < 0)
            {
                minusScore += EvaluateRate.minus_block_path_to_checkmate;
            }

            if (IsInWall(row_1, col_1, mine))
            {
                minusScore += EvaluateRate.auto_decrease_defence;
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
        private int NumPointOpptCanCheckWall(Player ourPlayer, Player oppt)
        {
            int result = 0;

            List<Piece> listOurHorse = (ourPlayer == Player.human) ? listHumanHorse : listMachineHorse;
            List<String> listOpptMove = GetListPossibleMove(oppt, ourPlayer);
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
                
                result += listPoint.Count;                                    
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
        private int NumPointOpptCanCheckWall(Player ourPlayer, Player oppt, String our_move)
        {
            int result = 0;

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

            List<String> listOpptMove = GetListPossibleMove(oppt, ourPlayer);
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

                result += list.Count;
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
        private int NumPointOpptCanCheckPlane(Player ourPlayer, Player oppt)
        {
            int result = 0;

            List<Piece> listOurHorse = (ourPlayer == Player.human) ? listHumanHorse : listMachineHorse;
            List<Piece> listOpptHorse = (oppt == Player.human) ? listHumanHorse : listMachineHorse;
            List<String> listOpptMove = GetListPossibleMove(oppt, ourPlayer);
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

                result += list.Count;
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
        private int NumPointOpptCanCheckPlane(Player ourPlayer, Player oppt, String our_move)
        {
            int result = 0;

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

            List<String> listOpptMove = GetListPossibleMove(oppt, ourPlayer);
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

                result += list.Count;
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
            Player ourPlayer = (oppt == Player.machine) ? Player.human : Player.machine;
            List<String> listMoveOpp = GetListPossibleMove(oppt, ourPlayer);
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
            //prevent the player from moving the prev horse at a time
            if (prevHorse != null && rw_0 == prevHorse.Row && col_0 == prevHorse.Col) return false;

            //guarantee position of the piece is not beyound the bound of gameboards
            if (rw_1 < 0 || rw_1 >= max_row || col_1 < 0 || col_1 >= max_col) return false;

            //guarantee you are moving a horse
            if (GetHorse(ourPlayer, rw_0, col_0) == null) return false;

            //guarantee the position move to is valid place. 
            //Cannot move to player's horses.
            List<Piece> listOurHorse = (ourPlayer == Player.human) ? listHumanHorse : listMachineHorse;
            if (listOurHorse.Contains(GetHorse(ourPlayer, rw_1, col_1)))
            {
                return false;
            }

            bool isInOpptPlane = false;
            //if the horse from the plane of opponent:
            //The horse cannot be blocked by oppt horses
            //And broken the origin horse move 
            //Cannot move to oppt_wall and oppt_horse and oppt_plane and our horses(self satisfy)
            //cannot checkmate oppt_wall
            Piece opptMachine = (ourPlayer == Player.human) ? plane_machine : plane_human;
            Player oppt = (ourPlayer == Player.human) ? Player.machine : Player.human;
            Piece oppt_wall = (ourPlayer == Player.human) ? wall_machine : wall_human;
            List<Piece> listOpptHorse = (ourPlayer == Player.human) ? listMachineHorse : listHumanHorse;
            if (rw_0 == opptMachine.Row && col_0 == opptMachine.Col)
            {
                //The horse cannot be blocked by oppt horses
                //And broken the origin horse move 
                isInOpptPlane = true;

                //Cannot move to oppt_wall and oppt_horse and oppt_plane and our horses(self satisfy)
                if (IsInWall(rw_1, col_1, oppt)
                    || IsInPlane(rw_1, col_1, oppt)
                    || listOpptHorse.Contains(GetHorse(oppt, rw_1, col_1)))
                {
                    return false;
                }

                //cannot checkmate oppt_wall
                if (GetListToWall(oppt_wall)
                    .Contains("" + rw_1 + col_1 + oppt_wall.Row + oppt_wall.Col))
                {
                    return false;
                }

            }

            if (!isInOpptPlane)
            {
                //the path does not belong to the horse
                if (!((Math.Abs(rw_1 - rw_0) == 1 && Math.Abs(col_1 - col_0) == 2)
                    || (Math.Abs(rw_1 - rw_0) == 2 && Math.Abs(col_1 - col_0) == 1))) return false;


                //guarantee the moving in not be blocked by another horse
                //and not be checkmated for 8 piece pos but group into 4

                int dis_row = rw_1 - rw_0;
                int dis_col = col_1 - col_0;
                //position #1 : 30 and -30 degree
                if ((dis_row == 1 || dis_row == -1) && dis_col == 2
                    && !boardgame[rw_0, col_0 + 1].Equals(empty_str))
                {
                    return false;
                }
                //position #2 : 60 and 120 degree
                if (dis_row == -2 && (dis_col == 1 || dis_col == -1)
                    && !boardgame[rw_0 - 1, col_0].Equals(empty_str))
                {
                    return false;
                }
                //position #3 : 150 and 210 degree
                if ((dis_row == 1 || dis_row == -1) && dis_col == -2
                    && !boardgame[rw_0, col_0 - 1].Equals(empty_str))
                {
                    return false;
                }
                //position #4 : 240 and 300 degree
                if (dis_row == 2 && (dis_col == -1 || dis_col == 1)
                    && !boardgame[rw_0 + 1, col_0].Equals(empty_str))
                {
                    return false;
                }
            }

            return true;
        }

        private List<String> GetListToWall(Piece wall)
        {
            List<String> list = new List<String>();
            if (IsInBound(wall.Row - 1, wall.Col - 2))
            {
                list.Add("" + (wall.Row - 1) + (wall.Col - 2) + wall.Row + wall.Col);
            }

            if (IsInBound(wall.Row - 1, wall.Col + 2))
            {
                list.Add("" + (wall.Row - 1) + (wall.Col + 2) + wall.Row + wall.Col);
            }

            if (IsInBound(wall.Row - 2, wall.Col - 1))
            {
                list.Add("" + (wall.Row - 2) + (wall.Col - 1) + wall.Row + wall.Col);
            }

            if (IsInBound(wall.Row - 2, wall.Col + 1))
            {
                list.Add("" + (wall.Row - 2) + (wall.Col + 1) + wall.Row + wall.Col);
            }

            if (IsInBound(wall.Row + 1, wall.Col - 2))
            {
                list.Add("" + (wall.Row + 1) + (wall.Col - 2) + wall.Row + wall.Col);
            }

            if (IsInBound(wall.Row + 1, wall.Col + 2))
            {
                list.Add("" + (wall.Row + 1) + (wall.Col + 2) + wall.Row + wall.Col);
            }

            if (IsInBound(wall.Row + 2, wall.Col - 1))
            {
                list.Add("" + (wall.Row + 2) + (wall.Col - 1) + wall.Row + wall.Col);
            }

            if (IsInBound(wall.Row + 2, wall.Col + 1))
            {
                list.Add("" + (wall.Row + 2) + (wall.Col + 1) + wall.Row + wall.Col);
            }

            return list;
        }

        private bool IsInBound(int rw_1, int col_1)
        {
            if (rw_1 < 0 || rw_1 >= max_row || col_1 < 0 || col_1 >= max_col) return false;
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
        private Piece GetHorse(Player ourPlayer, int row, int col)
        {
            List<Piece> listOurHorse = (ourPlayer == Player.machine) ? listMachineHorse : listHumanHorse;
            foreach (Piece p in listOurHorse)
            {
                if (p.Row == row && p.Col == col)
                {
                    return p;
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
        /// check status of current gameboard: win - lose - raw
        /// </summary>        
        public int CheckGameBoard()
        {
            if (O_human.Row == wall_machine.Row && O_human.Col == wall_machine.Col
               || P_human.Row == wall_machine.Row && P_human.Col == wall_machine.Col
               || Q_human.Row == wall_machine.Row && Q_human.Col == wall_machine.Col
               //rule one horse - you lose
               || listMachineHorse.Count == 1)
            {
                if (currentPlayer == Player.machine) return lose_number;
                if (currentPlayer == Player.human) return win_number;
            }

            if (K_machine.Row == wall_human.Row && K_machine.Col == wall_human.Col
                || L_machine.Row == wall_human.Row && L_machine.Col == wall_human.Col
                || M_machine.Row == wall_human.Row && M_machine.Col == wall_human.Col
                //rule one horse - you lose
                || listHumanHorse.Count == 1)
            {
                if (currentPlayer == Player.machine) return win_number;
                if (currentPlayer == Player.human) return lose_number;
            }

            if (GetListPossibleMove(Player.human, Player.machine).Count == 0
                && GetListPossibleMove(Player.machine, Player.human).Count == 0) return raw_number;

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


        private Piece prevHorse = null;
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
            List<String> possibleMoves = GetListPossibleMove(ourPlayer, oppt);
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

                    List<Object> memorySimulation = null;
                    if (isSimulated)
                    {
                        List<Object> paramSimulation = new List<Object>() { null, null, null, null };
                        paramSimulation[move_index] = p;
                        paramSimulation[our_player_index] = ourPlayer;
                        paramSimulation[oppt_index] = oppt;
                        List<Piece> listOpptHorse = (oppt == Player.human) ? listHumanHorse : listMachineHorse;
                        paramSimulation[list_oppt_horse_index] = listOpptHorse;
                        //simulate
                        memorySimulation = SimulateMove(paramSimulation);
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
                        UnSimulateMove(memorySimulation);
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

                    List<Object> memorySimulation = null;
                    if (isSimulated)
                    {
                        List<Object> paramSimulation = new List<Object>() { null, null, null, null };
                        paramSimulation[move_index] = p;
                        paramSimulation[our_player_index] = ourPlayer;
                        paramSimulation[oppt_index] = oppt;
                        List<Piece> listOpptHorse = (oppt == Player.human) ? listHumanHorse : listMachineHorse;
                        paramSimulation[list_oppt_horse_index] = listOpptHorse;
                        //simulate
                        memorySimulation = SimulateMove(paramSimulation);
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
                        UnSimulateMove(memorySimulation);
                    }

                    //calculate minimum value for minimize player
                    bestVal = Math.Min(value, min_inf);

                    //update final move when beta changed
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

        private bool IsInPlane(int row, int col, Player player)
        {
            Piece playerPlane = (player == Player.human) ? plane_human : plane_machine;
            return (row == playerPlane.Row) && (col == playerPlane.Col);
        }

        private bool IsInWall(int row, int col, Player player)
        {
            Piece playerWall = (player == Player.human) ? wall_human : wall_machine;
            return (row == playerWall.Row) && (col == playerWall.Col);
        }

        /// <summary>
        /// get all possible moves with condition checkmate
        /// </summary>
        /// <param name="Player">list posible moves of this player</param>
        /// <returns></returns>
        /// 
        ///Can optimize this function by saperate cases
        private List<String> GetListPossibleMove(Player ourPlayer, Player oppt)
        {
            Piece our_wall = (ourPlayer == Player.machine) ? wall_machine : wall_human;
            List<Piece> listOpptHorse = (ourPlayer == Player.machine) ? listHumanHorse : listMachineHorse;
            List<Piece> listOurHorse = (ourPlayer == Player.machine) ? listMachineHorse : listHumanHorse;
            List<String> mlistMove = new List<String>();

            //if wall is eaten by any oppt horse or no more horse to move
            if (listOpptHorse.Contains(GetHorse(oppt, our_wall.Row, our_wall.Col))
                || listOurHorse.Count == 0) return mlistMove;

            foreach (Piece p in listOurHorse)
            {
                //transfer all            
                for (int i = 0; i < max_row; i++)
                    for (int j = 0; j < max_col; j++)
                    {
                        if (IsValidMove(ourPlayer, p.Row, p.Col, i, j))
                        {
                            mlistMove.Add("" + p.Row + p.Col + i + j);
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
            List<Object> paramSimulation = new List<Object>() { null, null, null, null };
            paramSimulation[move_index] = p;
            paramSimulation[our_player_index] = ourPlayer;
            paramSimulation[oppt_index] = oppt;
            List<Piece> listOpptHorse = (oppt == Player.human) ? listHumanHorse : listMachineHorse;
            paramSimulation[list_oppt_horse_index] = listOpptHorse;
            //simulate
            List<Object> memorySimulation = SimulateMove(paramSimulation);

            //get list posible move
            List<String> listPosibleMove = GetListPossibleMove(ourPlayer, oppt);

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
            Console.WriteLine("Nguoi (O-P-Q)");
            Console.WriteLine("May (K-L-M)");
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
            InitmapId();
            InitmapCharCol();
            InitmapCharRow();
            InitmapCharToInt();
            InitFirstStep();
            max_depth = 1;
            //limit_step = 20;

            while (true)
            {
                if (currentPlayer == Player.human)
                {
                    String oldHorse = "--";
                    String move = "";
                    int count = 2;
                    while (count-- > 0)
                    {
                        do
                        {
                            move = GetHumanMove();
                        } while (oldHorse.Equals("" + move[0] + move[1]));
                        UpdateHumanHorsePos(move);
                        oldHorse = "" + move[2] + move[3];

                        //covert char to int
                        int move_0 = mapCharToInt[(char)move[0]];
                        int move_1 = mapCharToInt[(char)move[1]];
                        int move_2 = mapCharToInt[(char)move[2]];
                        int move_3 = mapCharToInt[(char)move[3]];

                        //check whether eat machine(opponent) horse
                        Piece oppMachineHorse = GetHorse(Player.machine, move_2, move_3);
                        listMachineHorse.Remove(oppMachineHorse);

                        boardgame[move_2, move_3] = boardgame[move_0, move_1];

                        if (IsInPlane(move_0, move_1, Player.human)
                            || IsInPlane(move_0, move_1, Player.machine))
                        {
                            string plane_signal = plane_machine.Label;
                            boardgame[move_0, move_1] = plane_signal;
                        }
                        else if (IsInWall(move_0, move_1, Player.human)
                            || IsInWall(move_0, move_1, Player.machine))
                        {
                            string wall_signal = wall_machine.Label;
                            boardgame[move_0, move_1] = wall_signal;
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
                    String oldHorse = "--";
                    String move = "";
                    int count = 2;
                    while (count-- > 0)
                    {
                        do
                        {
                            move = GetMachineMove(Player.machine);
                        } while (oldHorse.Equals("" + move[0] + move[1]));
                        UpdateMachineHorsePos(move);
                        oldHorse = "" + move[2] + move[3];
                        if (count == 0)
                        {
                            prevHorse = null;
                        }
                        else
                        {
                            prevHorse = GetHorse(mapCharToInt[oldHorse[0]], mapCharToInt[oldHorse[1]]);
                        }

                        //covert char to int
                        int move_0 = mapCharToInt[(char)move[0]];
                        int move_1 = mapCharToInt[(char)move[1]];
                        int move_2 = mapCharToInt[(char)move[2]];
                        int move_3 = mapCharToInt[(char)move[3]];

                        //check whether eat human(opponent) horse
                        Piece oppHumanHorse = GetHorse(Player.human, move_2, move_3);
                        listHumanHorse.Remove(oppHumanHorse);

                        boardgame[move_2, move_3] = boardgame[move_0, move_1];

                        if (IsInPlane(move_0, move_1, Player.human) || IsInPlane(move_0, move_1, Player.machine))
                        {
                            string plane_signal = plane_machine.Label;
                            boardgame[move_0, move_1] = plane_signal;
                        }
                        else if (IsInWall(move_0, move_1, Player.human) || IsInWall(move_0, move_1, Player.machine))
                        {
                            string wall_signal = wall_machine.Label;
                            boardgame[move_0, move_1] = wall_signal;
                        }
                        else
                        {
                            boardgame[move_0, move_1] = empty_str;
                        }

                        //print boardgame
                        PrintBoard();
                        Console.WriteLine("" + boardgame[move_2, move_3] + "(" + mapCharRow[move_0] + mapCharCol[move_1] + mapCharRow[move_2] + mapCharCol[move_3] + ")");

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

                        Console.Write("Enter:");
                        Console.ReadLine();
                    }
                    currentPlayer = Player.human;
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
            Console.WriteLine("\nNguoi danh truoc (O-P-Q) (nhan 1) hoac May danh truoc (K-L-M) (nhan 2)");
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

            //init horse and board depends on who firstPlayer is
            if (firstPlayerId == (int)Player.machine)
            {
                currentPlayer = Player.machine;
                InitListHorses(true);
            }
            else
            {
                currentPlayer = Player.human;
                InitListHorses(false);
            }

            PrintBoard();
        }

        public static void Main(String[] args)
        {
            Program cochiemthanh = new Program();
            cochiemthanh.GameBoard();
        }
    }
}
