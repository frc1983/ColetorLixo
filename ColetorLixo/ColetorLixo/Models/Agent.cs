using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ColetorLixo.Models
{
    public class Agent : Cell
    {
        #region Properties

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

                return null;
            }

        }

        #endregion

        #region Constructor

        public Agent(int x, int y, EnumAgentType type, int garbageCapacity, int garbageLoad = 0) : base(x, y)
        {
            this.AgentType = type;
            this.GarbageCapacity = garbageCapacity;
            this.GarbageLoad = garbageLoad;
        }

        #endregion

        #region Methods

        private string TrashIconUrl(Models.Agent agent)
        {
            Trash garbage = this as Trash;
            if (garbage.GarbageType.Equals(EnumGarbageType.Metal))
            {
                if(garbage.FullLoad)
                    return "Content/Images/trash_closed_metal.png";
                return "Content/Images/trash_open_metal.png";
            }
            else if (garbage.GarbageType.Equals(EnumGarbageType.Paper))
            {
                if (garbage.FullLoad)
                    return "Content/Images/trash_closed_paper.png";
                return "Content/Images/trash_open_paper.png";
            }
            else if (garbage.GarbageType.Equals(EnumGarbageType.Plastic))
            {
                if (garbage.FullLoad)
                    return "Content/Images/trash_closed_plastic.png";
                return "Content/Images/trash_open_plastic.png";
            }
            else if (garbage.GarbageType.Equals(EnumGarbageType.Glass))
            {
                if (garbage.FullLoad)
                    return "Content/Images/trash_closed_glass.png";
                return "Content/Images/trash_open_glass.png";
            }

            return null;
        }

        #endregion
    }
}