using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiTrainingSuite.Model
{
    public partial class CBListModel : INotifyPropertyChanged, INotifyPropertyChanging
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        public CBListModel()
        {
            ISTICKED = false;
        }

        partial void OnIDChanging(int value);
        partial void OnIDChanged();
        partial void OnLABELChanging(string value);
        partial void OnLABELChanged();
        partial void OnISTICKEDChanging(bool value);
        partial void OnISTICKEDChanged();

        private int _ID;
        public int ID
        {
            get { return _ID; }
            set
            {
                if (_ID != value)
                {
                    this.OnIDChanging(value);
                    this.SendPropertyChanging();
                    this._ID = value;
                    this.SendPropertyChanged("ID");
                    this.OnIDChanged();
                }
            }
        }

        private string _LABEL;
        public string LABEL
        {
            get { return _LABEL; }
            set
            {
                if (_LABEL != value)
                {
                    this.OnLABELChanging(value);
                    this.SendPropertyChanging();
                    this._LABEL = value;
                    this.SendPropertyChanged("LABEL");
                    this.OnLABELChanged();
                }
            }
        }

        private bool _ISTICKED;
        public bool ISTICKED
        {
            get { return _ISTICKED; }
            set
            {
                if (_ISTICKED != value)
                {
                    this.OnISTICKEDChanging(value);
                    this.SendPropertyChanging();
                    this._ISTICKED = value;
                    this.SendPropertyChanged("ISTICKED");
                    this.OnISTICKEDChanged();
                }
            }
        }

        public event PropertyChangingEventHandler PropertyChanging;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void SendPropertyChanging()
        {
            if ((this.PropertyChanging != null))
            {
                this.PropertyChanging(this, emptyChangingEventArgs);
            }
        }

        protected virtual void SendPropertyChanged(String propertyName)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
