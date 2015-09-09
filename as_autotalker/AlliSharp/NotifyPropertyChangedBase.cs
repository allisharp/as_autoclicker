using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlliSharp
{
    [Serializable()]
    public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        [NonSerialized]
        private Dictionary<string, PropertyChangedEventArgs> _argsCache =
        new Dictionary<string, PropertyChangedEventArgs>();

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyChange(string propertyName)
        {
            //i guess when it gets serialized argscache doesnt get reinitialized it is null?
            //was having errors with notifychange
            if (_argsCache == null) _argsCache = new Dictionary<string, PropertyChangedEventArgs>();
            if (_argsCache != null)
            {
                if (!_argsCache.ContainsKey(propertyName))
                    _argsCache[propertyName] = new PropertyChangedEventArgs(propertyName);

                NotifyChange(_argsCache[propertyName]);
            }            
        }
        private void NotifyChange(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
    }
}
