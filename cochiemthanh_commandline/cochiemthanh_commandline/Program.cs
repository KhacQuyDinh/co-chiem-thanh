using System;
using System.Collections.Generic;

namespace cochiemthanh_commandline
{
    public class Program
    {
        bool isNonProtectedHumanWall = false;
        bool isNonProtectedMachineWall = false;

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
        public Dictionary<char, int> mapCharToInt= new Dictionary<char, int>();

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

        //player string desc
        public const string player_human_str = "Người";
        public const string player_machine_str = "Máy";

        //public Piece currentHorse = null;

        //empty means having neither machine's troop or human's troop
        public const string empty_str = "  ";

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
        private int num_human_horse = 0;
        private int num_machine_horse = 0;
    
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

        public string[,] boardgame = new string[max_row, max_col];
        public int[,] EBoardgame = new int[max_row, max_col];

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
        /// <param name="move"></param>
        /// //debug
        public void CalEBoard(bool isMaximizePlayer, string move)
        {
            //reset value to zero
            for (int rw = 0; rw < max_row; rw++)
            {
                for (int cl = 0; cl < max_col; cl++)
                {
                    EBoardgame[rw, cl] = 0;
                }
            }

            Player opponent = currentPlayer == Player.human ? Player.machine : Player.human;
            //int defenceScore = 0;
            //int attackScore = 0;
            int row_1 = mapCharToInt[(char)move[0]];
            int col_1 = mapCharToInt[(char)move[1]];
            int row_2 = mapCharToInt[(char)move[2]];
            int col_2 = mapCharToInt[(char)move[3]];

            ////first move and checkmate opponent
            //if (EvaluateRate.num_move == 1
            //    && IsCheckmated(row_1, col_1, row_2, col_2, opponent)) attackScore += EvaluateRate.st_nonblock_checkmate;
            ////first move and move to opponent plane
            //if (EvaluateRate.num_move == 1
            //    && IsInOpptPlane(row_2, col_2, currentPlayer)) attackScore += EvaluateRate.st_moving_to_plane;
            ////absolute block opponent who is checkmating
            //if (IsCheckmated(currentPlayer) && !IsCheckmated(row_1, col_1, row_2, col_2, currentPlayer)) defenceScore += EvaluateRate.opponent_st_abs_block_checkmate;
            ////reduce block(checkmated) point of opponent but can still be checkmated
            //int reducedCheckmatedAmount = GetNumReduceBlockPoint(row_1, col_1, row_2, col_2, currentPlayer);
            //if (reducedCheckmatedAmount > 0) defenceScore += reducedCheckmatedAmount * EvaluateRate.opponent_st_nonabs_block_checkmate;
            ////block all path move to plane for one step opponent moves
            //if (IsFirstAbsBlockToPlane(row_1, col_1, row_2, col_2, currentPlayer)) defenceScore += EvaluateRate.abs_st_block_moving_to_plane;
            ////reduce path move to plane for one step opponent moves
            //int reducedPlaneAmount = GetNumReduceBlockToPlane(row_1, col_1, row_2, col_2, currentPlayer);
            //if (reducedPlaneAmount > 0) defenceScore += reducedPlaneAmount * EvaluateRate.nonabs_st_block_moving_to_plane;

            //int maxValue = Math.Max(attackScore, defenceScore);                        
            //int eValue = isMaximizePlayer ? maxValue : -maxValue;
            //EBoardgame[row_2, col_2] = eValue;
            Random random = new Random();
            EBoardgame[row_2, col_2] = random.Next();
            Console.WriteLine(EBoardgame[row_2,col_2]);
            //Console.WriteLine(attackScore + "-" + defenceScore);
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

        public bool IsValidMove(Player player, int rw_0, int col_0, int rw_1, int col_1)
        {
            //guarantee position of the piece is not beyound the bound of gameboards
            if (rw_1 < 0 || rw_1 >= max_row || col_1 < 0 || col_1 >= max_col) return false;           

            //guarantee you are moving a horse
            if (GetHorse(rw_0, col_0) == null) return false;

            ////guarantee the position move to is valid place. Cannot move to oppenent horse because it cannot be eaten.
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

            ////guarantee the position move to is valid place. Cannot move to oppenent horse because it cannot be eaten.
            if ((player == Player.human)
                && (rw_1 == wall_human.Row && col_1 == wall_human.Col
                || rw_1 == plane_human.Row && col_1 == plane_human.Col
                || listHumanHorse.Contains(GetHorse(Player.human, rw_1, col_1)))                
                ||
                (player == Player.machine)
                && (rw_1 == wall_machine.Row && col_1 == wall_machine.Col
                || rw_1 == plane_machine.Row && col_1 == plane_machine.Col
                || listMachineHorse.Contains(GetHorse(Player.machine, rw_1, col_1))))
            {
                return false;
            }

            //guarantee not be checkmated after moving a horse to there
            //player == Player.machine to sure not get benefit for opponent but if training for two machine fight each other, we need del it.            
            //if (player == Player.machine)
            //{
            //    bool isCheckMated;
            //    string originLabel = boardgame[rw_0, col_0];
            //    string desLabel = boardgame[rw_1, col_1];
            //    boardgame[rw_1, col_1] = originLabel;
            //    if (rw_0 == plane_human.Row && col_0 == plane_human.Col)
            //    {
            //        boardgame[rw_0, col_0] = plane_human.Label;
            //    }
            //    else
            //    {
            //        boardgame[rw_0, col_0] = empty_str;
            //    }

            //    isCheckMated = IsCheckmated(currentPlayer);

            //    boardgame[rw_1, col_1] = desLabel;
            //    boardgame[rw_0, col_0] = originLabel;

            //    if (isCheckMated) return false;
            //}

            //if the horse from the plane of opponent then we don't need guarantee for 
            //the horse be or not be blocked and unlimit the horse path
            if ((player == Player.human
                && rw_0 == plane_machine.Row
                && col_0 == plane_machine.Col)
                ||
                (player == Player.machine
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
            if (dis_row == 2 && (dis_col == 1 || dis_col == -1)
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
            if (dis_row == -2 && (dis_col == -1 || dis_col == 1)
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
        /// check whether machine or human be checkmated correspond with player
        /// </summary>        
        public bool IsCheckmated(Player player)
        {
            return GetListFirstBlockCheckmate(player).Count != 0;
        }

        /// <summary>
        /// check whether machine or human be checkmated correspond to player
        /// after player move to a specific pos (row,col)
        /// move is valid
        /// </summary>        
        public bool IsCheckmated(int row_1, int col_1, int row_2, int col_2, Player player)
        {
            string des = boardgame[row_2, col_2];
            boardgame[row_2, col_2] = boardgame[row_1, col_1];

            if (player == Player.machine)
            {
                if (row_1 == plane_human.Row && col_1 == plane_human.Col) boardgame[row_1, col_1] = plane_human.Label;
                else boardgame[row_1, col_1] = empty_str;
            }
            else if (player == Player.human)
            {
                if (row_1 == plane_machine.Row && col_1 == plane_machine.Col) boardgame[row_1, col_1] = plane_machine.Label;
                else boardgame[row_1, col_1] = empty_str;
            }

            bool isCheckmated = GetListFirstBlockCheckmate(player).Count != 0;
            boardgame[row_1, col_1] = boardgame[row_2, col_2];
            boardgame[row_2, col_2] = des;

            return isCheckmated;
        }

        /// <summary>
        /// check whether player after move to the pos (row_2, col_2) can block all of opponent moves to its plane
        /// after player move to a specific pos (row,col)
        /// </summary>        
        public bool IsFirstAbsBlockToPlane(int row_1, int col_1, int row_2, int col_2, Player player)
        {
            string des = boardgame[row_2, col_2];
            boardgame[row_2, col_2] = boardgame[row_1, col_1];

            if (player == Player.machine)
            {
                if (row_1 == plane_human.Row && col_1 == plane_human.Col) boardgame[row_1, col_1] = plane_human.Label;
                else boardgame[row_1, col_1] = empty_str;
            }
            else if (player == Player.human)
            {
                if (row_1 == plane_machine.Row && col_1 == plane_machine.Col) boardgame[row_1, col_1] = plane_machine.Label;
                else boardgame[row_1, col_1] = empty_str;
            }

            bool isAbsBlock = GetListFirstBlockToPlane(player).Count == 0;
            boardgame[row_1, col_1] = boardgame[row_2, col_2];
            boardgame[row_2, col_2] = des;

            return isAbsBlock;
        }

        /// <summary>
        /// check whether player after move to the pos (row_2, col_2) can block all of opponent moves to its plane
        /// after player move to a specific pos (row,col)
        /// </summary>        
        public int GetNumReduceBlockToPlane(int row_1, int col_1, int row_2, int col_2, Player player)
        {
            //cal firstNumBlockPlane
            int firstNumBlockPlane = GetListFirstBlockToPlane(player).Count;

            string des = boardgame[row_2, col_2];
            boardgame[row_2, col_2] = boardgame[row_1, col_1];

            if (player == Player.machine)
            {
                if (row_1 == plane_human.Row && col_1 == plane_human.Col) boardgame[row_1, col_1] = plane_human.Label;
                else boardgame[row_1, col_1] = empty_str;
            }
            else if (player == Player.human)
            {
                if (row_1 == plane_machine.Row && col_1 == plane_machine.Col) boardgame[row_1, col_1] = plane_machine.Label;
                else boardgame[row_1, col_1] = empty_str;
            }

            int secondNumBlockPlane = GetListFirstBlockToPlane(player).Count;
            boardgame[row_1, col_1] = boardgame[row_2, col_2];
            boardgame[row_2, col_2] = des;

            return  secondNumBlockPlane - firstNumBlockPlane;
        }

        public int GetNumReduceBlockPoint(int row_1, int col_1, int row_2, int col_2, Player player)
        {
            //cal firstNumCheckmatedPoint
            int firstNumBlockPoint = GetListFirstBlockCheckmate(player).Count;

            string des = boardgame[row_2, col_2];
            boardgame[row_2, col_2] = boardgame[row_1, col_1];

            if (player == Player.machine)
            {
                if (row_1 == plane_human.Row && col_1 == plane_human.Col) boardgame[row_1, col_1] = plane_human.Label;
                else boardgame[row_1, col_1] = empty_str;
            }
            else if (player == Player.human)
            {
                if (row_1 == plane_machine.Row && col_1 == plane_machine.Col) boardgame[row_1, col_1] = plane_machine.Label;
                else boardgame[row_1, col_1] = empty_str;
            }

            //cal secondNumBlockPoint
            int secondNumBlockPoint = GetListFirstBlockCheckmate(player).Count;

            boardgame[row_1, col_1] = boardgame[row_2, col_2];
            boardgame[row_2, col_2] = des;

            return secondNumBlockPoint - firstNumBlockPoint;
        }

        /// <summary>
        /// get list block checkmate for the player
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private List<string> GetListFirstBlockCheckmate(Player player)
        {
            List<string> listStBlockCheckmate = new List<string>();
            if (player == Player.human)
            {
                foreach (Piece horse in listMachineHorse)
                {
                    if (Math.Abs(horse.Row - wall_human.Row) == 1)
                    {
                        if (horse.Col - wall_human.Col == -2
                            && boardgame[wall_human.Row, wall_human.Col - 1].Equals(empty_str)
                            && !listStBlockCheckmate.Contains("" + wall_human.Row + (wall_human.Col - 1))) listStBlockCheckmate.Add("" + wall_human.Row + (wall_human.Col - 1));
                        else if (horse.Col - wall_human.Col == 2
                            && boardgame[wall_human.Row, wall_human.Col + 1].Equals(empty_str)
                            && !listStBlockCheckmate.Contains("" + wall_human.Row + (wall_human.Col + 1))) listStBlockCheckmate.Add("" + wall_human.Row + (wall_human.Col + 1));
                    }
                    else if (Math.Abs(horse.Row - wall_human.Row) == 2
                        && Math.Abs(horse.Col - wall_human.Col) == 1
                        && boardgame[wall_human.Row + 1, wall_human.Col].Equals(empty_str)
                        && !listStBlockCheckmate.Contains("" + (wall_human.Row + 1) + wall_human.Col)) listStBlockCheckmate.Add("" + (wall_human.Row + 1) + wall_human.Col);
                }
            }
            else if (player == Player.machine)
            {
                foreach (Piece horse in listHumanHorse)
                {
                    if (Math.Abs(horse.Row - wall_machine.Row) == 1)
                    {
                        if (horse.Col - wall_machine.Col == -2
                            && boardgame[wall_machine.Row, wall_machine.Col - 1].Equals(empty_str)
                            && !listStBlockCheckmate.Contains("" + wall_machine.Row + (wall_machine.Col - 1))) listStBlockCheckmate.Add("" + wall_machine.Row + (wall_machine.Col - 1));
                        else if (horse.Col - wall_machine.Col == 2
                            && boardgame[wall_machine.Row, wall_machine.Col + 1].Equals(empty_str)
                            && !listStBlockCheckmate.Contains("" + wall_machine.Row + (wall_machine.Col + 1))) listStBlockCheckmate.Add("" + wall_machine.Row + (wall_machine.Col + 1));
                    }
                    else if (Math.Abs(horse.Row - wall_machine.Row) == 2
                        && Math.Abs(horse.Col - wall_machine.Col) == 1
                        && boardgame[wall_machine.Row - 1, wall_machine.Col].Equals(empty_str)
                        && !listStBlockCheckmate.Contains("" + (wall_machine.Row - 1) + wall_machine.Col)) listStBlockCheckmate.Add("" + (wall_machine.Row - 1) + wall_machine.Col);
                }
            }
            return listStBlockCheckmate;
        }

        /// <summary>
        /// get list move to the player plane in one more step
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private List<string> GetListFirstBlockToPlane(Player player)
        {
            List<string> listStBlockToPlane = new List<string>();
            if (player == Player.human)
            {
                foreach (Piece horse in listMachineHorse)
                {
                    if (Math.Abs(horse.Row - plane_human.Row) == 1)
                    {
                        if (horse.Col - plane_human.Col == -2
                            && boardgame[plane_human.Row, plane_human.Col - 1].Equals(empty_str)
                            && !listStBlockToPlane.Contains("" + plane_human.Row + (plane_human.Col - 1))) listStBlockToPlane.Add("" + plane_human.Row + (plane_human.Col - 1));
                        else if (horse.Col - plane_human.Col == 2
                            && boardgame[plane_human.Row, plane_human.Col + 1].Equals(empty_str)
                            && !listStBlockToPlane.Contains("" + plane_human.Row + (plane_human.Col + 1))) listStBlockToPlane.Add("" + plane_human.Row + (plane_human.Col + 1));
                    }
                    else if (Math.Abs(horse.Row - plane_human.Row) == 2
                        && Math.Abs(horse.Col - plane_human.Col) == 1
                        && boardgame[plane_human.Row + 1, plane_human.Col].Equals(empty_str)
                        && !listStBlockToPlane.Contains("" + (plane_human.Row + 1) + plane_human.Col)) listStBlockToPlane.Add("" + (plane_human.Row + 1) + plane_human.Col);
                }
            }
            else if (player == Player.machine)
            {
                foreach (Piece horse in listHumanHorse)
                {
                    if (Math.Abs(horse.Row - plane_machine.Row) == 1)
                    {
                        if (horse.Col - plane_machine.Col == -2
                            && boardgame[plane_machine.Row, plane_machine.Col - 1].Equals(empty_str)
                            && !listStBlockToPlane.Contains("" + plane_machine.Row + (plane_machine.Col - 1))) listStBlockToPlane.Add("" + plane_machine.Row + (plane_machine.Col - 1));
                        else if (horse.Col - plane_machine.Col == 2
                            && boardgame[plane_machine.Row, plane_machine.Col + 1].Equals(empty_str)
                            && !listStBlockToPlane.Contains("" + plane_machine.Row + (plane_machine.Col + 1))) listStBlockToPlane.Add("" + plane_machine.Row + (plane_machine.Col + 1));
                    }
                    else if (Math.Abs(horse.Row - plane_machine.Row) == 2
                        && Math.Abs(horse.Col - plane_machine.Col) == 1
                        && boardgame[plane_machine.Row - 1, plane_machine.Col].Equals(empty_str)
                        && !listStBlockToPlane.Contains("" + (plane_machine.Row - 1) + plane_machine.Col)) listStBlockToPlane.Add("" + (plane_machine.Row - 1) + plane_machine.Col);
                }
            }
            return listStBlockToPlane;
        }


        /// <summary>
        /// check how many opponent's horse will be blocked after player move its horse to a specific position (row,col)
        /// </summary>
        /// <param name = "row" > row </ param >
        /// < param name="col">column</param>
        /// <param name = "player" > player who move the horse</param>
        /// <returns></returns>
        private int CountBlockOpponent(int row, int col, Player player)
        {
            int numBlockedOpptHorse = 0;

            if (player == Player.human)
            {
                foreach (Piece horse in listMachineHorse)
                {
                    if (col == horse.Col)
                    {
                        if (row - horse.Row == 1 && max_row - horse.Row > 2) numBlockedOpptHorse++;
                        else if (row - horse.Row == -1 && horse.Row > 1) numBlockedOpptHorse++;
                    }
                    else if (row == horse.Row)
                    {
                        if (col - horse.Col == 1 && max_col - horse.Col > 2) numBlockedOpptHorse++;
                        else if (col - horse.Col == -1 && horse.Col > 1) numBlockedOpptHorse++;
                    }
                }
            }
            else if (player == Player.machine)
            {
                foreach (Piece horse in listHumanHorse)
                {
                    if (col == horse.Col)
                    {
                        if (row - horse.Row == 1 && max_row - horse.Row > 2) numBlockedOpptHorse++;
                        else if (row - horse.Row == -1 && horse.Row > 1) numBlockedOpptHorse++;
                    }
                    else if (row == horse.Row)
                    {
                        if (col - horse.Col == 1 && max_col - horse.Col > 2) numBlockedOpptHorse++;
                        else if (col - horse.Col == -1 && horse.Col > 1) numBlockedOpptHorse++;
                    }
                }
            }

            return numBlockedOpptHorse;
        }

        /// <summary>
        /// check whether a player move its horse to a specific pos (row,col) could be in opponent plane or not
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool IsInOpptPlane(int row, int col, Player player)
        {
            if (player == Player.machine)
            {
                return row == plane_human.Row && col == plane_human.Col;
            }
            else if (player == Player.human)
            {
                return row == plane_machine.Row && col == plane_machine.Col;
            }

            return false;
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
        //public List<string> GetListSecondBlockCheckmate(int row_1, int col_1, int row_2, int col_2, Player player)
        //{
        //    //simulate the horse moves...
        //    string originLabel = boardgame[row_1, col_1];
        //    string desLabel = boardgame[row_2, col_2];
        //    boardgame[row_2, col_2] = originLabel;
        //    if (row_1 == plane_human.Row && col_1 == plane_human.Col)
        //    {
        //        boardgame[row_1, col_1] = plane_human.Label;
        //    }
        //    else
        //    {
        //        boardgame[row_1, col_1] = empty_str;
        //    }

        //    List<string> listNdBlockCheckmate = GetListFirstBlockCheckmate(player);

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

            if (O_human.Row == wall_machine.Row &&  O_human.Col == wall_machine.Col
               || P_human.Row == wall_machine.Row && P_human.Col == wall_machine.Col
               || Q_human.Row == wall_machine.Row && Q_human.Col == wall_machine.Col
               || num_machine_horse == 0)
            {
                if (currentPlayer == Player.machine) return lose_number;
                if (currentPlayer == Player.human) return win_number;
            }

            if (K_machine.Row == wall_human.Row && K_machine.Col == wall_human.Col
                || L_machine.Row == wall_human.Row && L_machine.Col == wall_human.Col
                || M_machine.Row == wall_human.Row && M_machine.Col == wall_human.Col
                || num_human_horse == 0)
            {
                if (currentPlayer == Player.machine) return win_number;
                if (currentPlayer == Player.human) return lose_number;
            }

            if (GetListPossibleMove(Player.human).Count == 0
                && GetListPossibleMove(Player.machine).Count == 0) return raw_number;

            return live_game_number;
        }

        public string GetStatusMsg(int status)
        {
            string currentPlayerStr = (currentPlayer == Player.human) ? player_human_str : player_machine_str;

            if (status == raw_number) return "Hoà";
            if (status == win_number) return currentPlayerStr + " thắng";
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
        public string GetMachineMove(Player player)
        {
            //determine getMove for which player            
            bool isMaximizePlayer = (player == Player.machine) ? true : false;
            int depth = 0;
            Alpha alpha = new Alpha();
            Beta beta = new Beta();
            string finalMove = raw_number + "" + raw_number + "" + raw_number + "" + raw_number;
            GenerateMove(null, depth, isMaximizePlayer, alpha, beta, ref finalMove);
            return finalMove;
        }

        /// <summary>
        /// use minimax to generateMove.
        /// </summary>
        private int GenerateMove(string node, int depth, bool isMaximizePlayer, Alpha alpha, Beta beta, ref string finalMove)
        {
            //machine is maximize player
            Player player = isMaximizePlayer ? Player.machine : Player.human;
            //check end condition for the resurcive func
            List<string> possibleMoves = GetListPossibleMove(player);
            if (depth == max_depth || possibleMoves.Count == 0)
            {
                //debug
                //Console.WriteLine("For machine: " + EBoardgame[mapCharToInt[(char)node[2]], mapCharToInt[(char)node[3]]]);
                return (depth == 0) ? raw_number : EBoardgame[mapCharToInt[(char)node[2]], mapCharToInt[(char)node[3]]];
            }

            int bestVal, value;

            //machine turn
            if (isMaximizePlayer)
            {
                bestVal = max_inf; //-Int32.MaxValue; 
                foreach (string p in possibleMoves)
                {
                    //CalEBoard  for a specific postion (row,col) after simulating the horse moving to there
                    if (depth == max_depth - 1 || GetListPossibleMove(player, p).Count == 0)
                    {
                        CalEBoard(isMaximizePlayer, p);
                    }

                    //convert char to int for p
                    int p_0 = mapCharToInt[(char)p[0]];
                    int p_1 = mapCharToInt[(char)p[1]];
                    int p_2 = mapCharToInt[(char)p[2]];
                    int p_3 = mapCharToInt[(char)p[3]];

                    //update machine horse position
                    UpdateMachineHorsePos("" + p_0 + p_1 + p_2 + p_3);

                    //check whether (p_2,p_3) in opponent horse
                    bool nextMoveInOppHorse = listHumanHorse.Contains(GetHorse(Player.human, p_2, p_3));

                    //simulate the horse moves
                    string originLabel = boardgame[p_0, p_1];
                    string desLabel = boardgame[p_2, p_3];
                    boardgame[p_2, p_3] = originLabel;
                    if (p_0 == plane_human.Row && p_1 == plane_human.Col)
                    {
                        boardgame[p_0, p_1] = plane_human.Label;
                    }
                    else
                    {
                        boardgame[p_0, p_1] = empty_str;
                    }

                    //case eat opponent horse
                    if (nextMoveInOppHorse) num_human_horse--;

                    //generate next move
                    value = GenerateMove(p, depth + 1, false, alpha, new Beta(), ref finalMove);

                    //revert simulation
                    boardgame[p_2, p_3] = desLabel;
                    boardgame[p_0, p_1] = originLabel;
                    //case eat opponent horse
                    if (nextMoveInOppHorse) num_human_horse++;
                    
                    //update machine horse position
                    UpdateMachineHorsePos("" + p_2 + p_3 + p_0 + p_1);

                    //calculate maximum value for maximize player
                    bestVal = Math.Max(value, bestVal);
                                        
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
                foreach (string p in possibleMoves)
                {
                    //CalEBoard  for a specific postion (row,col) after simulating the horse moves there
                    if (depth == max_depth - 1 || GetListPossibleMove(player, p).Count == 0)
                    {
                        CalEBoard(isMaximizePlayer, p);
                    }

                    //convert char to int for p
                    int p_0 = mapCharToInt[(char)p[0]];
                    int p_1 = mapCharToInt[(char)p[1]];
                    int p_2 = mapCharToInt[(char)p[2]];
                    int p_3 = mapCharToInt[(char)p[3]];

                    //update human horse position
                    UpdateHumanHorsePos("" + p_0 + p_1 + p_2 + p_3);

                    //check whether (p_2,p_3) in opponent horse
                    bool nextMoveInOppHorse = listMachineHorse.Contains(GetHorse(Player.machine, p_2, p_3));

                    //simulate the horse moves
                    string originLabel = boardgame[p_0, p_1];
                    string desLabel = boardgame[p_2, p_3];
                    boardgame[p_2, p_3] = originLabel;
                    if (p_0 == plane_human.Row && p_1 == plane_human.Col)
                    {
                        boardgame[p_0, p_1] = plane_human.Label;
                    }
                    else
                    {
                        boardgame[p_0, p_1] = empty_str;
                    }

                    //case eat opponent horse
                    if (nextMoveInOppHorse) num_machine_horse--;

                    //generate next move
                    value = GenerateMove(p, depth + 1, true, new Alpha(), beta, ref finalMove);

                    //revert simulation
                    boardgame[p_2, p_3] = desLabel;
                    boardgame[p_0, p_1] = originLabel;
                    //case eat opponent horse
                    if (nextMoveInOppHorse) num_machine_horse++;

                    //update human horse position
                    UpdateHumanHorsePos("" + p_2 + p_3 + p_0 + p_1);

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
        private List<string> GetListPossibleMove(Player player)
        {
            List<string> mlistMove = new List<string>();
            
            if (player == Player.human)
            {
                //if wall is eaten or no more horse to move
                if (listMachineHorse.Contains(GetHorse(Player.machine, wall_human.Row, wall_human.Col))
                    || num_human_horse == 0) return mlistMove;

                foreach (Piece p in listHumanHorse)
                {
                    if (IsInOpptPlane(p.Row, p.Col, Player.machine))
                    {
                        for (int i = 0; i < max_row; i++)
                        for (int j = 0; j < max_col; j++)
                        {
                            //empty place or opponent horse place                            
                            if (boardgame[i,j].Equals(empty_str) || listMachineHorse.Contains(GetHorse(Player.machine, i, j)))
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
                    || num_machine_horse == 0) return mlistMove;

                foreach (Piece p in listMachineHorse)
                {
                    if (IsInOpptPlane(p.Row, p.Col, Player.human))
                    {
                        for (int i = 0; i < max_row; i++)
                            for (int j = 0; j < max_col; j++)
                            {
                                //empty place or opponent horse place
                                if (boardgame[i, j].Equals(empty_str) || listHumanHorse.Contains(GetHorse(Player.human, i, j)))
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
        private List<string> GetListPossibleMove(Player player, string p)
        {
            //convert char to int for p
            int p_0 = mapCharToInt[(char)p[0]];
            int p_1 = mapCharToInt[(char)p[1]];
            int p_2 = mapCharToInt[(char)p[2]];
            int p_3 = mapCharToInt[(char)p[3]];
            //simulate the horse moves
            string originLabel = boardgame[p_0, p_1];
            string desLabel = boardgame[p_2, p_3];
            boardgame[p_2, p_3] = originLabel;
            if (player == Player.machine)
            {
                if (p_0 == plane_human.Row && p_1 == plane_human.Col)
                {
                    boardgame[p_0, p_1] = plane_human.Label;
                }
                else
                {
                    boardgame[p_0, p_1] = empty_str;
                }
            } 
            else if (player == Player.human)
            {
                if (p_0 == plane_machine.Row && p_1 == plane_machine.Col)
                {
                    boardgame[p_0, p_1] = plane_machine.Label;
                }
                else
                {
                    boardgame[p_0, p_1] = empty_str;
                }
            }


            //get list posible move
            List<string> listPosibleMove = GetListPossibleMove(player);

            //revert simulation
            boardgame[p_2, p_3] = desLabel;
            boardgame[p_0, p_1] = originLabel;

            return listPosibleMove;
        }
        /// <summary>
        /// opponent move don't need restrict condition isValidMove
        /// </summary>
        /// <returns></returns>
        public string GetHumanMove()
        {
            Console.Write("Wating your move (ex. 2a3h): ");
            string move;
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
            max_depth = 4;
            limit_step = 7;

            num_human_horse = 3;
            num_machine_horse = 3;

            isNonProtectedHumanWall = false;
            isNonProtectedMachineWall = false;

            while (true)
            {
                if (currentPlayer == Player.human)
                {
                    string oldMove = "";
                    string move = "";
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
                        bool isEatOppHorse = listMachineHorse.Contains(GetHorse(Player.machine, move_2, move_3));
                        if (isEatOppHorse) num_machine_horse--;

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
                            return;
                        }
                    }
                    currentPlayer = Player.machine;
                }
                else if (currentPlayer == Player.machine)
                {
                    string oldMove = "";
                    string move = "";
                    int count = 2;
                    while (count-- > 0)
                    {
                        do
                        {
                            move = GetMachineMove(Player.machine);
                        } while (oldMove.Equals(move));
                        //print machine move
                        if (!move.Equals("-1-1-1-1"))
                        {
                            Console.WriteLine("" + mapCharRow[mapCharToInt[move[0]]] + mapCharCol[mapCharToInt[move[1]]] + mapCharRow[mapCharToInt[move[2]]] + mapCharCol[mapCharToInt[move[3]]]);
                        }                       
                        UpdateMachineHorsePos(move);
                        oldMove = move;                        

                        //covert char to int
                        int move_0 = mapCharToInt[(char)move[0]];
                        int move_1 = mapCharToInt[(char)move[1]];
                        int move_2 = mapCharToInt[(char)move[2]];
                        int move_3 = mapCharToInt[(char)move[3]];

                        //check whether eat human(opponent) horse
                        bool isEatOppHorse = listHumanHorse.Contains(GetHorse(Player.human, move_2, move_3));
                        if (isEatOppHorse) num_human_horse--;

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
                            return;
                        }
                    }
                    currentPlayer = Player.human;
                }
                num_step++;
                if (num_step == limit_step)
                {
                    limit_step += 7;                    
                    max_depth++;
                }
            }
            Console.ReadLine();
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

        private void UpdateMachineHorsePos(string move)
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

        private void UpdateHumanHorsePos(string move)
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

            Console.WriteLine("\nMay danh truoc (nhan 1) hoac nguoi danh truoc nhan (2)");
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

                Console.Write("Error command! Please try again: ");
            }

            if (firstPlayerId == (int)Player.machine)
            {
                //boardgame[max_row / 2, max_col / 2] = player_machine_str;
                currentPlayer = Player.human;
            }
        }

        public static void Main(string[] args)
        {
            Program cochiemthanh = new Program();                        
            cochiemthanh.GameBoard();
        }
    }
}
