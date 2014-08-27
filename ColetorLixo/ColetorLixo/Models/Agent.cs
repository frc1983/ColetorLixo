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
                    return "Content/Images/garbage.png";

                return null;
            }

        }

        private string TrashIconUrl(Models.Agent agent)
        {
            Trash garbage = this as Trash;
            if (garbage.GarbageType.Equals(EnumGarbageType.Metal))
                return "Content/Images/trash_open_metal.png";
            else if (garbage.GarbageType.Equals(EnumGarbageType.Papel))
                return "Content/Images/trash_open_paper.png";
            else if (garbage.GarbageType.Equals(EnumGarbageType.Plastico))
                return "Content/Images/trash_open_plastic.png";
            else if (garbage.GarbageType.Equals(EnumGarbageType.Vidro))
                return "Content/Images/trash_open_glass.png";

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