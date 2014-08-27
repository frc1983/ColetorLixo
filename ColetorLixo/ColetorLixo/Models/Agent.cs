using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ColetorLixo.Models
{
    public class Agent : Cell
    {
        public EnumAgentType AgentType { get; set; }
        public int GarbageCapacity { get; set; }
        public int GarbageLoad { get; set; }
        public Boolean FullLoad { get; set; }

        public String ImagePath
        {
            get
            {
                if (this.AgentType.Equals(EnumAgentType.TRASH))
                    return TrashIconUrl(this);
                else if (this.AgentType.Equals(EnumAgentType.CHARGER))
                    return "Content/Images/charger.png";
                else if (this.AgentType.Equals(EnumAgentType.COLLECTOR))
                    return "Content/Images/collector.png";
                else if (this.AgentType.Equals(EnumAgentType.GARBAGE))
                    return GarbageIconUrl(this);

                return null;
            }

        }

        private string TrashIconUrl(Models.Agent agent)
        {
            Trash garbage = this as Trash;
            if (garbage.GarbageType.Equals(EnumGarbageType.Metal))
                return "Content/Images/trash_open_metal.png";
            else if (garbage.GarbageType.Equals(EnumGarbageType.Paper))
                return "Content/Images/trash_open_paper.png";
            else if (garbage.GarbageType.Equals(EnumGarbageType.Plastic))
                return "Content/Images/trash_open_plastic.png";
            else if (garbage.GarbageType.Equals(EnumGarbageType.Glass))
                return "Content/Images/trash_open_glass.png";

            return null;
        }

        private string GarbageIconUrl(Models.Agent agent)
        {
            Garbage garbage = this as Garbage;
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

        public Agent(int x, int y, EnumAgentType type, int garbageCapacity, int garbageLoad = 0) : base(x, y)
        {
            this.AgentType = type;
            this.GarbageCapacity = garbageCapacity;
            this.GarbageLoad = garbageLoad;
        }
    }
}