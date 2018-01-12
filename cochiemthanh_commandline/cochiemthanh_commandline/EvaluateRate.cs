using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cochiemthanh_commandline
{
    public class EvaluateRate
    {
        public const int st_nonblock_checkmate = 250;        
        public const int opponent_st_abs_block_checkmate = 200;
        public const int opponent_st_nonabs_block_checkmate = 50;        
        public const int st_moving_to_plane = 185;                        
        public const int abs_st_block_moving_to_plane = 160;
        public const int eat_opp_horse = 155;
        public const int nonabs_st_block_moving_to_plane = 40;                
        public const int bonus_checkmate = 15;
        public const int bonus_blocking = 3;
        public const int bonus_not_be_blocked = 3;
        public const int normal_pos = 8;
        public static int num_move = 0;
    }
}
