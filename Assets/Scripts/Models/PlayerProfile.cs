using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite4Unity3d;

namespace Assets.Scripts.Models
{
    public class PlayerProfile
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return $"[Score: Name={Name}";
        }
    }
}
