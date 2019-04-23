using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite4Unity3d;

namespace Assets.Scripts.Leaderboard
{
    public class Leaderboard
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public int PlayerProfileID { get; set; }

        public string Song { get; set; }

        public float Score { get; set; }

        public override string ToString()
        {
            return $"[Score: Name={PlayerProfileID},  Song={Song}, Score={Score}";
        }
    }
}
