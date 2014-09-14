using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ColetorLixo.Models
{
    public class Garbage : Cell
    {
        public EnumGarbageType GarbageType { get; set; }
        public int GarbageSize { get; set; }

        public Garbage(int x, int y, EnumGarbageType type, int load = 1)
            : base(x, y)
        {
            this.GarbageType = type;
            GarbageSize = load;
        }

        public static string GarbageIconUrl(Models.Garbage garbage)
        {
            if (garbage.GarbageType.Equals(EnumGarbageType.Metal))
                return "Content/Images/garbage_metal.png";
            else if (garbage.GarbageType.Equals(EnumGarbageType.Paper))
                return "Content/Images/garbage_paper.png";
            else if (garbage.GarbageType.Equals(EnumGarbageType.Plastic))
                return "Content/Images/garbage_plastic.png";
            else if (garbage.GarbageType.Equals(EnumGarbageType.Glass))
                return "Content/Images/garbage_glass.png";

            return null;
        }
    }
}