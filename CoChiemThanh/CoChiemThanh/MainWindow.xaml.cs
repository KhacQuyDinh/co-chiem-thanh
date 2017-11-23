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
        public PlayerTurn currentPlayerTurn = PlayerTurn.none;
        public enum PlayerTurn : int
        {
            none = -1,
            human_turn = 1,
            machine_turn = 2
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

        public string[,] boardgame = new string[max_row, max_col];
        public int[,] eBoardgame = new int[max_row, max_col];

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

        //calculate eboard...
        public void CalEBoard()
        {
            //reset value to zero
            for (int rw = 0; rw < max_row; rw++)
            {
                for (int cl = 0; cl < max_col; cl++)
                {
                    eBoardgame[rw, cl] = 0;
                }
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
            if (((currentPlayerTurn == PlayerTurn.human_turn)
                && !boardgame[rw_1, col_1].Equals(plane_machine.Label)
                && !boardgame[rw_1, col_1].Equals(wall_machine.Label)
                && !boardgame[rw_1, col_1].Equals(empty_str))
                ||
                ((currentPlayerTurn == PlayerTurn.machine_turn)
                && !boardgame[rw_1, col_1].Equals(plane_human.Label)
                && !boardgame[rw_1, col_1].Equals(wall_human.Label))
                && !boardgame[rw_1, col_1].Equals(empty_str))
            {
                return false;
            }

            //guarantee not be checkmated after moving a horse to there
            //currentPlayerTurn == PlayerTurn.machine_turn to sure not get benefit for opponent but if training for two machine fight each other, we need del it.            
            if (currentPlayerTurn == PlayerTurn.machine_turn)
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

                isCheckMated = IsCheckmated(currentPlayerTurn);

                boardgame[rw_1, col_1] = desLabel;
                boardgame[rw_0, col_0] = originLabel;

                if (isCheckMated) return false;
            }

            //if the horse from the plane of opponent then we don't need guarantee for 
            //the horse be or not be blocked
            if ((currentPlayerTurn == PlayerTurn.human_turn
                && rw_0 == plane_machine.Row
                && col_0 == plane_machine.Col)
                ||
                (currentPlayerTurn == PlayerTurn.machine_turn
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

        private bool IsCheckmated(PlayerTurn PlayerTurn)
        {
            if (PlayerTurn == PlayerTurn.human_turn)
            {
                if (Math.Abs(wall_machine.Row - K_machine.Row) == 1 && Math.Abs(wall_machine.Col - K_machine.Col) == 2
                    || Math.Abs(wall_machine.Row - K_machine.Row) == 2 && Math.Abs(wall_machine.Col - K_machine.Col) == 1
                    || Math.Abs(wall_machine.Row - L_machine.Row) == 1 && Math.Abs(wall_machine.Col - L_machine.Col) == 2
                    || Math.Abs(wall_machine.Row - L_machine.Row) == 2 && Math.Abs(wall_machine.Col - L_machine.Col) == 1
                    || Math.Abs(wall_machine.Row - M_machine.Row) == 1 && Math.Abs(wall_machine.Col - M_machine.Col) == 2
                    || Math.Abs(wall_machine.Row - M_machine.Row) == 2 && Math.Abs(wall_machine.Col - M_machine.Col) == 1)
                {
                    return true;
                }
            }
            else if (PlayerTurn == PlayerTurn.machine_turn)
            {
                if (Math.Abs(wall_human.Row - O_human.Row) == 1 && Math.Abs(wall_human.Col - O_human.Col) == 2
                    || Math.Abs(wall_human.Row - O_human.Row) == 2 && Math.Abs(wall_human.Col - O_human.Col) == 1
                    || Math.Abs(wall_human.Row - P_human.Row) == 1 && Math.Abs(wall_human.Col - P_human.Col) == 2
                    || Math.Abs(wall_human.Row - P_human.Row) == 2 && Math.Abs(wall_human.Col - P_human.Col) == 1
                    || Math.Abs(wall_human.Row - Q_human.Row) == 1 && Math.Abs(wall_human.Col - Q_human.Col) == 2
                    || Math.Abs(wall_human.Row - Q_human.Row) == 2 && Math.Abs(wall_human.Col - Q_human.Col) == 1)
                {
                    return true;
                }
            }
            return false;
        }

        //check status of current gameboard: win - lose - raw
        public int CheckGameBoard()
        {
            if (O_human.Col == wall_machine.Col && O_human.Row == wall_machine.Row
               || P_human.Col == wall_machine.Col && P_human.Row == wall_machine.Row
               || Q_human.Col == wall_machine.Col && Q_human.Row == wall_machine.Row)
            {
                if (currentPlayerTurn == PlayerTurn.machine_turn) return lose_number;
                if (currentPlayerTurn == PlayerTurn.human_turn) return win_number;
            }

            if (K_machine.Col == wall_human.Col && K_machine.Row == wall_human.Row
                || M_machine.Col == wall_human.Col && M_machine.Row == wall_human.Row
                || L_machine.Col == wall_human.Col && L_machine.Row == wall_human.Row)
            {
                if (currentPlayerTurn == PlayerTurn.machine_turn) return win_number;
                if (currentPlayerTurn == PlayerTurn.human_turn) return lose_number;
            }

            if (getPossibleMoves(PlayerTurn.human_turn).Count == 0
                && getPossibleMoves(PlayerTurn.machine_turn).Count == 0) return raw_number;

            return live_game_number;
        }

        public string GetStatusMsg(int status)
        {
            string currentPlayerStr = (currentPlayerTurn == PlayerTurn.human_turn) ? player_human_str : player_machine_str;

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
        /// <param name="PlayerTurn"></param>
        /// </summary>        
        public string GetMachineMove(PlayerTurn PlayerTurn)
        {
            //determine getMove for which player            
            bool isMaximizePlayer = (PlayerTurn == PlayerTurn.machine_turn) ? true : false;
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
            PlayerTurn PlayerTurn = isMaximizePlayer ? PlayerTurn.machine_turn : PlayerTurn.human_turn;
            List<string> possibleMoves = getPossibleMoves(PlayerTurn);
            if (depth == max_depth || possibleMoves.Count == 0)
            {
                return (depth == 0) ? raw_number : eBoardgame[mapId[node[2]], mapId[node[3]]];
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
        /// <param name="PlayerTurn">id of the player</param>
        /// <returns></returns>
        private List<string> getPossibleMoves(PlayerTurn PlayerTurn)
        {
            List<string> mlistMove = new List<string>();
            if (PlayerTurn == PlayerTurn.human_turn)
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
            else if (PlayerTurn == PlayerTurn.machine_turn)
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
                if (currentPlayerTurn == PlayerTurn.human_turn)
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
                    currentPlayerTurn = PlayerTurn.machine_turn;
                }
                else if (currentPlayerTurn == PlayerTurn.machine_turn)
                {
                    string oldMove = "";
                    string move = "";
                    int count = 2;
                    while (count-- > 0)
                    {
                        do
                        {
                            move = GetMachineMove(PlayerTurn.machine_turn);
                        } while (oldMove.Equals(move));
                        oldMove = move;
                        boardgame[mapId[move[2]], mapId[move[3]]] = boardgame[mapId[move[0]], mapId[move[1]]];
                        if (mapId[move[0]] == plane_human.Row && mapId[move[1]] == plane_human.Col)
                        {
                            boardgame[mapId[move[0]], mapId[move[1]]] = plane_human.Label;
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
                    currentPlayerTurn = PlayerTurn.human_turn;
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

            if (firstPlayerId == (int)PlayerTurn.machine_turn)
            {
                //boardgame[max_row / 2, max_col / 2] = player_machine_str;
                currentPlayerTurn = PlayerTurn.human_turn;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
