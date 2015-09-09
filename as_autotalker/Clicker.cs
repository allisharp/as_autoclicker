using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlliSharp
{
    [Serializable()]
    public class Clicker : NotifyPropertyChangedBase
    {
        private int interval = 0;
        private int mousedown = 0;
        private int fkeystart = 0;
        private int fkeystop = 0;
        private bool rightclick = false;

        [NonSerialized]
        public int hotKeyIdStart = 0;
        [NonSerialized]
        public int hotKeyIdStop = 0;
        [NonSerialized]
        private bool started = false;

        public Clicker(int interval, int mousedown, int fkeystart, int fkeystop, bool rightclick)
        {
            this.interval = interval;
            this.mousedown = mousedown;
            this.fkeystart = fkeystart;
            this.fkeystop = fkeystop;
            this.rightclick = rightclick;
        }

        public int Interval
        {
            get
            {
                return interval;
            }
            set
            {
                interval = value;
                NotifyChange("Interval");
            }
        }

        public int MouseDownInterval
        {
            get
            {
                return mousedown;
            }
            set
            {
                mousedown = value;
                NotifyChange("MouseDownInterval");
            }
        }

        public int FKeyStart
        {
            get
            {
                return fkeystart;
            }
            set
            {
                fkeystart = value;
                NotifyChange("FKeyStart");
            }
        }

        public int FKeyStop
        {
            get
            {
                return fkeystop;
            }
            set
            {
                fkeystop = value;
                NotifyChange("FKeyStop");
            }
        }
        public bool IsRightClick
        {
            get
            {
                return rightclick;
            }
            set
            {
                rightclick = value;
                NotifyChange("IsRightClick");
            }
        }
        public bool Started
        {
            get
            {
                return started;
            }
            set
            {
                started = value;
                NotifyChange("Started");
            }
        }
    }


}
