using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace CoChiemThanh
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //status gameboard desc
        public const int bad_move_number = -1;
        public const int raw_number = -1;
        public const int win_number = 0;
        public const int lose_number = 1;
        public const int live_game_number = 3;

        public Dictionary<char, int> mapId = new Dictionary<char, int>();

        public void initmapId()
        {
            mapId.Add('a', 0);
            mapId.Add('b', 1);
            mapId.Add('c', 2);
            mapId.Add('d', 3);
            mapId.Add('e', 4);
            mapId.Add('f', 5);
            mapId.Add('g', 6);
            mapId.Add('h', 7);
            mapId.Add('8', 7);
            mapId.Add('7', 6);
            mapId.Add('6', 5);
            mapId.Add('5', 4);
            mapId.Add('4', 3);
            mapId.Add('3', 2);
            mapId.Add('2', 1);
            mapId.Add('1', 0);
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
        public Piece O_human = new Piece(0, 0, " O");
        public Piece P_human = new Piece(0, 4, " P");
        public Piece Q_human = new Piece(0, 7, " Q");

        //machine's horses        
        public Piece K_machine = new Piece(7, 0, " K");
        public Piece L_machine = new Piece(7, 3, " L");
        public Piece M_machine = new Piece(7, 7, " M");

        public List<Piece> listMachineHorse = new List<Piece>();
        public List<Piece> listHumanHorse = new List<Piece>();

        public void initListHorses()
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
                    boardgame[rw, cl] = "  ";
                }

            //set pieces in the board            
            boardgame[wall_human.Col, wall_human.Row] = wall_human.Label;
            boardgame[wall_machine.Col, wall_machine.Row] = wall_machine.Label;

            boardgame[plane_human.Col, plane_human.Row] = plane_human.Label;
            boardgame[O_human.Col, O_human.Row] = O_human.Label;
            boardgame[P_human.Col, P_human.Row] = P_human.Label;
            boardgame[Q_human.Col, Q_human.Row] = Q_human.Label;

            boardgame[K_machine.Col, K_machine.Row] = K_machine.Label;
            boardgame[L_machine.Col, L_machine.Row] = L_machine.Label;
            boardgame[plane_machine.Col, plane_machine.Row] = plane_machine.Label;
            boardgame[M_machine.Col, M_machine.Row] = M_machine.Label;

        }

        //calculate eboard using current player other funcs
        public void CalEBoard()
        {
            //reset value to zero
            for (int rw = 0; rw < max_row; rw++)
            {
                for (int cl = 0; cl < max_col; cl++)
                {
                    EBoardgame[rw, cl] = 0;
                }
            }

            //...
            Player opponent = currentPlayer == Player.human ? Player.machine : Player.human;
            List<string> posibleMoves = getPossibleMoves(currentPlayer);
            foreach (string move in posibleMoves)
            {
                int defenceScore = 0;
                int attackScore = 0;
                int row_1 = mapId[move[0]];
                int col_1 = mapId[move[1]];
                int row_2 = mapId[move[2]];
                int col_2 = mapId[move[3]];

                //first move and checkmate opponent
                if (EvaluateRate.num_move == 1
                    && IsCheckmated(row_1, col_1, row_2, col_2, opponent)) attackScore += EvaluateRate.st_nonblock_checkmate;
                //first move and move to opponent plane
                if (EvaluateRate.num_move == 1
                    && IsInOpptPlane(row_2, col_2, currentPlayer)) attackScore += EvaluateRate.st_moving_to_plane;
                //absolute block opponent who is checkmating
                if (IsCheckmated(currentPlayer) && !IsCheckmated(row_1, col_1, row_2, col_2, currentPlayer)) defenceScore += EvaluateRate.opponent_st_abs_block_checkmate;
                //reduce block(checkmated) point of opponent but can still be checkmated
                int reducedCheckmatedAmount = GetNumReduceBlockPoint(row_1, col_1, row_2, col_2, currentPlayer);
                if (reducedCheckmatedAmount > 0) defenceScore += reducedCheckmatedAmount * EvaluateRate.opponent_st_nonabs_block_checkmate;
                //block all path move to plane for one step opponent moves
                if (IsFirstAbsBlockToPlane(row_1, col_1, row_2, col_2, currentPlayer)) defenceScore += EvaluateRate.abs_st_block_moving_to_plane;
                //reduce path move to plane for one step opponent moves
                int reducedPlaneAmount = GetNumReduceBlockToPlane(row_1, col_1, row_2, col_2, currentPlayer);
                if (reducedPlaneAmount > 0) defenceScore += reducedPlaneAmount * EvaluateRate.nonabs_st_block_moving_to_plane;
                //opponent will checkmate after one move.
                //...
            }
        }

        public void PrintBoard()
        {
            for (int cl = 0; cl < max_col; cl++) Console.Write((char)(cl + 'a') + "   ");

            for (int rw = 0; rw < max_row; rw++)
            {
                Console.WriteLine("--------------------------------------------");
                Console.Write(max_row - rw);
                for (int cl = 0; cl < max_col; cl++)
                {
                    Console.Write(" | " + boardgame[rw, cl] + " ");
                }
                Console.Write("|");
            }
        }

        public bool IsValidMove(int rw_0, int col_0, int rw_1, int col_1)
        {
            //guarantee position of the piece is not beyound the bound of gameboards
            if (rw_1 < 0 || rw_1 >= max_row || col_1 < 0 || col_1 >= max_col) return false;

            //guarantee the position move to is valid place. Cannot move to oppenent horse because it cannot be eaten.
            if (((currentPlayer == Player.human)
                && !boardgame[rw_1, col_1].Equals(plane_machine.Label)
                && !boardgame[rw_1, col_1].Equals(wall_machine.Label)
                && !boardgame[rw_1, col_1].Equals(empty_str))
                ||
                ((currentPlayer == Player.machine)
                && !boardgame[rw_1, col_1].Equals(plane_human.Label)
                && !boardgame[rw_1, col_1].Equals(wall_human.Label))
                && !boardgame[rw_1, col_1].Equals(empty_str))
            {
                return false;
            }

            //guarantee not be checkmated after moving a horse to there
            //currentPlayer == Player.machine to sure not get benefit for opponent but if training for two machine fight each other, we need del it.            
            if (currentPlayer == Player.machine)
            {
                bool isCheckMated;
                string originLabel = boardgame[rw_0, col_0];
                string desLabel = boardgame[rw_1, col_1];
                boardgame[rw_1, col_1] = originLabel;
                if (rw_0 == plane_human.Row && col_0 == plane_human.Col)
                {
                    boardgame[rw_0, col_0] = plane_human.Label;
                }
                else
                {
                    boardgame[rw_0, col_0] = empty_str;
                }

                isCheckMated = IsCheckmated(currentPlayer);

                boardgame[rw_1, col_1] = desLabel;
                boardgame[rw_0, col_0] = originLabel;

                if (isCheckMated) return false;
            }

            //if the horse from the plane of opponent then we don't need guarantee for 
            //the horse be or not be blocked
            if ((currentPlayer == Player.human
                && rw_0 == plane_machine.Row
                && col_0 == plane_machine.Col)
                ||
                (currentPlayer == Player.machine
                && rw_0 == plane_human.Row
                && col_0 == plane_human.Col))
            {
                return true;
            }

            //guarantee the moving in not be blocked by another horse and not be checkmated for 8 piece pos but group into 4
            int dis_row = rw_1 - rw_0;
            int dis_col = col_1 - col_0;
            //position #1 : 30 and -30 degree
            if ((dis_row == 1 || dis_row == -1) && dis_col == 2
                && boardgame[rw_0, col_0 + 1].Equals(empty_str))
            {
                return true;
            }
            //position #2 : 60 and 120 degree
            if (dis_row == 2 && (dis_col == 1 || dis_col == -1)
                && boardgame[rw_0 - 1, col_0].Equals(empty_str))
            {
                return true;
            }
            //position #3 : 150 and 210 degree
            if ((dis_row == 1 || dis_row == -1) && dis_col == -2
                && boardgame[rw_0, col_0 - 1].Equals(empty_str))
            {
                return true;
            }
            //position #4 : 240 and 300 degree
            if (dis_row == -2 && (dis_col == -1 || dis_col == 1)
                && boardgame[rw_0, col_0 - 1].Equals(empty_str))
            {
                return true;
            }

            return false;
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

            return firstNumBlockPlane - secondNumBlockPlane;
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

            return firstNumBlockPoint - secondNumBlockPoint;
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
                            && boardgame[wall_human.Row, wall_human.Col - 1].Equals(empty_str)) listStBlockCheckmate.Add("" + wall_human.Row + (wall_human.Col - 1));
                        else if (horse.Col - wall_human.Col == 2
                            && boardgame[wall_human.Row, wall_human.Col + 1].Equals(empty_str)) listStBlockCheckmate.Add("" + wall_human.Row + (wall_human.Col + 1));
                    } 
                    else if (Math.Abs(horse.Row - wall_human.Row) == 2
                        && Math.Abs(horse.Col - wall_human.Col) == 1
                        && boardgame[wall_human.Row + 1, wall_human.Col].Equals(empty_str)) listStBlockCheckmate.Add("" + (wall_human.Row + 1) + wall_human.Col);                   
                }
            }
            else if (player == Player.machine)
            {
                foreach (Piece horse in listHumanHorse)
                {
                    if (Math.Abs(horse.Row - wall_machine.Row) == 1)
                    {
                        if (horse.Col - wall_human.Col == -2
                            && boardgame[wall_human.Row, wall_human.Col - 1].Equals(empty_str)) listStBlockCheckmate.Add("" + wall_human.Row + (wall_human.Col - 1));
                        else if (horse.Col - wall_human.Col == 2
                            && boardgame[wall_human.Row, wall_human.Col + 1].Equals(empty_str)) listStBlockCheckmate.Add("" + wall_human.Row + (wall_human.Col + 1));
                    }
                    else if (Math.Abs(horse.Row - wall_human.Row) == 2
                        && Math.Abs(horse.Col - wall_human.Col) == 1
                        && boardgame[wall_human.Row - 1, wall_human.Col].Equals(empty_str)) listStBlockCheckmate.Add("" + (wall_human.Row - 1) + wall_human.Col);
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
                            && boardgame[plane_human.Row, plane_human.Col - 1].Equals(empty_str)) listStBlockToPlane.Add("" + plane_human.Row + (plane_human.Col - 1));
                        else if (horse.Col - plane_human.Col == 2
                            && boardgame[plane_human.Row, plane_human.Col + 1].Equals(empty_str)) listStBlockToPlane.Add("" + plane_human.Row + (plane_human.Col + 1));
                    }
                    else if (Math.Abs(horse.Row - plane_human.Row) == 2
                        && Math.Abs(horse.Col - plane_human.Col) == 1
                        && boardgame[plane_human.Row + 1, plane_human.Col].Equals(empty_str)) listStBlockToPlane.Add("" + (plane_human.Row + 1) + plane_human.Col);
                }
            }
            else if (player == Player.machine)
            {
                foreach (Piece horse in listHumanHorse)
                {
                    if (Math.Abs(horse.Row - plane_machine.Row) == 1)
                    {
                        if (horse.Col - plane_human.Col == -2
                            && boardgame[plane_human.Row, plane_human.Col - 1].Equals(empty_str)) listStBlockToPlane.Add("" + plane_human.Row + (plane_human.Col - 1));
                        else if (horse.Col - plane_human.Col == 2
                            && boardgame[plane_human.Row, plane_human.Col + 1].Equals(empty_str)) listStBlockToPlane.Add("" + plane_human.Row + (plane_human.Col + 1));
                    }
                    else if (Math.Abs(horse.Row - plane_human.Row) == 2
                        && Math.Abs(horse.Col - plane_human.Col) == 1
                        && boardgame[plane_human.Row - 1, plane_human.Col].Equals(empty_str)) listStBlockToPlane.Add("" + (plane_human.Row - 1) + plane_human.Col);
                }
            }
            return listStBlockToPlane;
        }


        /// <summary>
        /// get list block checkmate for the player if the opponent checkmate in one more move
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        //public List<string> GetListSecondBlockCheckmate(Player player)
        //{
        //    List<string> listSecondCheckmate = new List<string>();
        //    if (player == Player.human)
        //    {
        //        foreach (Piece horse in listMachineHorse)
        //        {
        //            if (Math.Abs(horse.Row - wall_human.Row) == 1)
        //            {
        //                if (horse.Col - wall_human.Col == -2) listStBlockCheckmate.Add("" + wall_human.Row + (wall_human.Col - 1));
        //                else if (horse.Col - wall_human.Col == 2) listStBlockCheckmate.Add("" + wall_human.Row + (wall_human.Col + 1));
        //            }
        //            else if (horse.Row - wall_human.Row == 2
        //                && Math.Abs(horse.Col - wall_human.Col) == 1) listStBlockCheckmate.Add("" + (wall_human.Row + 1) + wall_human.Col);
        //        }
        //    }
        //    else if (player == Player.machine)
        //    {
        //        foreach (Piece horse in listHumanHorse)
        //        {
        //            if (Math.Abs(horse.Row - wall_machine.Row) == 1)
        //            {
        //                if (horse.Col - wall_machine.Col == -2) listStBlockCheckmate.Add("" + wall_machine.Row + (wall_machine.Col - 1));
        //                else if (horse.Col - wall_machine.Col == 2) listStBlockCheckmate.Add("" + wall_machine.Row + (wall_machine.Col + 1));
        //            }
        //            else if (Math.Abs(horse.Row - wall_machine.Row) == 2
        //                && Math.Abs(horse.Col - wall_machine.Col) == 1) listStBlockCheckmate.Add("" + (wall_machine.Row - 1) + wall_machine.Col);
        //        }
        //    }
        //    return listStBlockCheckmate;
        //}

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
        
        /// <summary>
        /// check status of current gameboard: win - lose - raw
        /// </summary>        
        public int CheckGameBoard()
        {
            if (O_human.Col == wall_machine.Col && O_human.Row == wall_machine.Row
               || P_human.Col == wall_machine.Col && P_human.Row == wall_machine.Row
               || Q_human.Col == wall_machine.Col && Q_human.Row == wall_machine.Row)
            {
                if (currentPlayer == Player.machine) return lose_number;
                if (currentPlayer == Player.human) return win_number;
            }

            if (K_machine.Col == wall_human.Col && K_machine.Row == wall_human.Row
                || M_machine.Col == wall_human.Col && M_machine.Row == wall_human.Row
                || L_machine.Col == wall_human.Col && L_machine.Row == wall_human.Row)
            {
                if (currentPlayer == Player.machine) return win_number;
                if (currentPlayer == Player.human) return lose_number;
            }

            if (getPossibleMoves(Player.human).Count == 0
                && getPossibleMoves(Player.machine).Count == 0) return raw_number;

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
        public const int max_depth = 4;

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
            Player player = isMaximizePlayer ? Player.machine : Player.human;
            List<string> possibleMoves = getPossibleMoves(player);
            if (depth == max_depth || possibleMoves.Count == 0)
            {
                return (depth == 0) ? raw_number : EBoardgame[mapId[node[2]], mapId[node[3]]];
            }

            int bestVal, value;

            //machine turn
            if (isMaximizePlayer)
            {
                bestVal = max_inf;
                foreach (string p in possibleMoves)
                {
                    string originLabel = boardgame[mapId[p[0]], mapId[p[1]]];
                    string desLabel = boardgame[mapId[p[2]], mapId[p[3]]];
                    boardgame[mapId[p[2]], mapId[p[3]]] = originLabel;
                    if (mapId[p[0]] == plane_human.Row && mapId[p[1]] == plane_human.Col)
                    {
                        boardgame[mapId[p[0]], mapId[p[1]]] = plane_human.Label;
                    }
                    else
                    {
                        boardgame[mapId[p[0]], mapId[p[1]]] = empty_str;
                    }
                    value = GenerateMove(p, depth + 1, false, alpha, beta, ref finalMove);
                    boardgame[mapId[p[2]], mapId[p[3]]] = desLabel;
                    boardgame[mapId[p[0]], mapId[p[1]]] = originLabel;
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
            else
            {
                bestVal = min_inf;
                foreach (string p in possibleMoves)
                {
                    string originLabel = boardgame[mapId[p[0]], mapId[p[1]]];
                    string desLabel = boardgame[mapId[p[2]], mapId[p[3]]];
                    boardgame[mapId[p[2]], mapId[p[3]]] = originLabel;
                    if (mapId[p[0]] == plane_machine.Row && mapId[p[1]] == plane_machine.Col)
                    {
                        boardgame[mapId[p[0]], mapId[p[1]]] = plane_machine.Label;
                    }
                    else
                    {
                        boardgame[mapId[p[0]], mapId[p[1]]] = empty_str;
                    }
                    value = GenerateMove(p, depth + 1, true, alpha, beta, ref finalMove);
                    boardgame[mapId[p[2]], mapId[p[3]]] = desLabel;
                    boardgame[mapId[p[0]], mapId[p[1]]] = originLabel;
                    bestVal = Math.Min(value, min_inf);
                    //update final move when alpha changed
                    if (beta.min < bestVal)
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
        private List<string> getPossibleMoves(Player player)
        {
            List<string> mlistMove = new List<string>();
            if (player == Player.human)
            {
                List<Piece> humanHorses = new List<Piece>() { O_human, P_human, Q_human };
                foreach (Piece p in humanHorses)
                {
                    if (IsValidMove(p.Row, p.Col, p.Row - 1, p.Col + 2)) mlistMove.Add("" + p.Row + p.Col + (p.Row - 1) + (p.Col + 2));
                    if (IsValidMove(p.Row, p.Col, p.Row - 2, p.Col + 1)) mlistMove.Add("" + p.Row + p.Col + (p.Row - 2) + (p.Col + 1));
                    if (IsValidMove(p.Row, p.Col, p.Row - 2, p.Col - 1)) mlistMove.Add("" + p.Row + p.Col + (p.Row - 2) + (p.Col - 1));
                    if (IsValidMove(p.Row, p.Col, p.Row - 1, p.Col - 2)) mlistMove.Add("" + p.Row + p.Col + (p.Row - 1) + (p.Col - 2));
                    if (IsValidMove(p.Row, p.Col, p.Row + 1, p.Col - 2)) mlistMove.Add("" + p.Row + p.Col + (p.Row + 1) + (p.Col - 2));
                    if (IsValidMove(p.Row, p.Col, p.Row + 2, p.Col - 1)) mlistMove.Add("" + p.Row + p.Col + (p.Row + 2) + (p.Col - 1));
                    if (IsValidMove(p.Row, p.Col, p.Row + 2, p.Col + 1)) mlistMove.Add("" + p.Row + p.Col + (p.Row + 2) + (p.Col + 1));
                    if (IsValidMove(p.Row, p.Col, p.Row + 1, p.Col + 2)) mlistMove.Add("" + p.Row + p.Col + (p.Row + 1) + (p.Col + 2));
                }
            }
            else if (player == Player.machine)
            {
                List<Piece> machineHorses = new List<Piece>() { K_machine, L_machine, M_machine };
                foreach (Piece p in machineHorses)
                {
                    if (IsValidMove(p.Row, p.Col, p.Row - 1, p.Col + 2)) mlistMove.Add("" + p.Row + p.Col + (p.Row - 1) + (p.Col + 2));
                    if (IsValidMove(p.Row, p.Col, p.Row - 2, p.Col + 1)) mlistMove.Add("" + p.Row + p.Col + (p.Row - 2) + (p.Col + 1));
                    if (IsValidMove(p.Row, p.Col, p.Row - 2, p.Col - 1)) mlistMove.Add("" + p.Row + p.Col + (p.Row - 2) + (p.Col - 1));
                    if (IsValidMove(p.Row, p.Col, p.Row - 1, p.Col - 2)) mlistMove.Add("" + p.Row + p.Col + (p.Row - 1) + (p.Col - 2));
                    if (IsValidMove(p.Row, p.Col, p.Row + 1, p.Col - 2)) mlistMove.Add("" + p.Row + p.Col + (p.Row + 1) + (p.Col - 2));
                    if (IsValidMove(p.Row, p.Col, p.Row + 2, p.Col - 1)) mlistMove.Add("" + p.Row + p.Col + (p.Row + 2) + (p.Col - 1));
                    if (IsValidMove(p.Row, p.Col, p.Row + 2, p.Col + 1)) mlistMove.Add("" + p.Row + p.Col + (p.Row + 2) + (p.Col + 1));
                    if (IsValidMove(p.Row, p.Col, p.Row + 1, p.Col + 2)) mlistMove.Add("" + p.Row + p.Col + (p.Row + 1) + (p.Col + 2));
                }
            }
            return mlistMove;
        }

        /// <summary>
        /// opponent move don't need restrict condition isValidMove
        /// </summary>
        /// <returns></returns>
        public string getHumanMove()
        {
            Console.Write("Wating your move: ");
            string move;
            while (true)
            {
                move = Console.ReadLine();
                if (move.Length < 1 || move.Length > 4
                    || !isAlphabet(move[0]) || !isDigit(move[1])
                    || !isAlphabet(move[2]) || !isDigit(move[2])
                    || !IsValidMove(mapId[move[0]], mapId[move[1]], mapId[move[2]], mapId[move[3]]))
                {
                    Console.Write("Error command! Please try again: ");
                    continue;
                }
                else
                {
                    break;
                }
            }
            return move;
        }

        private bool isDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private bool isAlphabet(char c)
        {
            return c >= 'a' && c <= 'h';
        }

        /// <summary>
        /// game loop
        /// </summary>        
        public void GameBoard()
        {
            InitFirstStep();

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
                            move = getHumanMove();
                        } while (oldMove.Equals(move));
                        oldMove = move;
                        boardgame[mapId[move[2]], mapId[move[3]]] = boardgame[mapId[move[0]], mapId[move[1]]];
                        if (mapId[move[0]] == plane_machine.Row && mapId[move[1]] == plane_machine.Col)
                        {
                            boardgame[mapId[move[0]], mapId[move[1]]] = plane_machine.Label;
                        }
                        else if (mapId[move[0]] == wall_machine.Row && mapId[move[1]] == wall_machine.Col)
                        {
                            boardgame[mapId[move[0]], mapId[move[1]]] = wall_machine.Label;
                        }
                        else
                        {
                            boardgame[mapId[move[0]], mapId[move[1]]] = empty_str;
                        }
                        //check gameboard finish or not
                        int statusGameBoard = CheckGameBoard();
                        if (statusGameBoard != live_game_number)
                        {
                            Console.WriteLine(GetStatusMsg(statusGameBoard));
                            break;
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
                        oldMove = move;
                        boardgame[mapId[move[2]], mapId[move[3]]] = boardgame[mapId[move[0]], mapId[move[1]]];
                        if (mapId[move[0]] == plane_human.Row && mapId[move[1]] == plane_human.Col)
                        {
                            boardgame[mapId[move[0]], mapId[move[1]]] = plane_human.Label;
                        }
                        else if (mapId[move[0]] == wall_human.Row && mapId[move[1]] == wall_human.Col)
                        {
                            boardgame[mapId[move[0]], mapId[move[1]]] = wall_human.Label;
                        }
                        else
                        {
                            boardgame[mapId[move[0]], mapId[move[1]]] = empty_str;
                        }
                        //check gameboard finish or not
                        int statusGameBoard = CheckGameBoard();
                        if (statusGameBoard != live_game_number)
                        {
                            Console.WriteLine(GetStatusMsg(statusGameBoard));
                            break;
                        }
                    }
                    currentPlayer = Player.human;
                }

                //print boardgame
                PrintBoard();
            }
        }

        //consider...
        private void InitFirstStep()
        {
            Console.WriteLine("\nMay danh truoc (nhan 1) hoac nguoi danh truoc nhan (2)");
            Console.Write("Wating your command: ");
            int firstPlayerId;
            while (true)
            {
                try
                {
                    firstPlayerId = Int32.Parse(Console.ReadLine());
                    if (firstPlayerId > 0 || firstPlayerId < 3) return;
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

        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
