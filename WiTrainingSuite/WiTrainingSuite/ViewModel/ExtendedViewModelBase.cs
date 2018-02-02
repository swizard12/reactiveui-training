using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiTrainingSuite.ViewModel
{
    public class ExtendedViewModelBase : ReactiveObject
    {
        public ExtendedViewModelBase()
        {
            ColumnHeaders = new Dictionary<string, string>();

            SortList = new ObservableCollection<fsItem>();

            AddCommand = ReactiveCommand.Create(() =>
            {
                SortList.Add(new fsItem());
            });

            DelCommand = ReactiveCommand.Create(() =>
            {
                SortList.Remove(SelectedSortItem);
            },
            this.WhenAnyValue(
                x => x.SortList.Count, (c) => c > 0));

            MoveUpCommand = ReactiveCommand.Create(() =>
            {
                SortList.Move(SortIdx, SortIdx - 1);
            },
            this.WhenAnyValue(
                x => x.SortIdx, (i) => i != 0 && SortList.Count > 1));

            MoveDownCommand = ReactiveCommand.Create(() =>
            {
                SortList.Move(SortIdx, SortIdx + 1);
            },
            this.WhenAnyValue(
                x => x.SortIdx,
                (i) => i != SortList.Count - 1 && SortList.Count > 1));
        }

        private bool _IsBtmOpen;
        public bool IsBtmOpen
        {
            get { return _IsBtmOpen; }
            set { this.RaiseAndSetIfChanged(ref _IsBtmOpen, value); }
        }

        public ReactiveCommand AddCommand { get; protected set; }
        public ReactiveCommand DelCommand { get; protected set; }
        public ReactiveCommand MoveUpCommand { get; protected set; }
        public ReactiveCommand MoveDownCommand { get; protected set; }
        public ReactiveCommand CloseDrawerCommand { get; protected set; }

        private ObservableCollection<fsItem> _sortList;
        public ObservableCollection<fsItem> SortList
        {
            get { return _sortList; }
            set { this.RaiseAndSetIfChanged(ref _sortList, value); }
        }

        public Dictionary<string, string> ColumnHeaders { get; set; }

        private fsItem _SelectedSortItem;
        public fsItem SelectedSortItem
        {
            get { return _SelectedSortItem; }
            set { this.RaiseAndSetIfChanged(ref _SelectedSortItem, value); }
        }

        private int _SortIdx;
        public int SortIdx
        {
            get { return _SortIdx; }
            set { this.RaiseAndSetIfChanged(ref _SortIdx, value); }
        }

        public string BuildFilterString(ObservableCollection<fsItem> SList)
        {
            string f = string.Empty;
            if (SList.Count > 0)
            {
                for (int i = 0; i < SList.Count; i++)
                {
                    if (i == 0)
                    {
                        f = f + SList[i].ColumnHeader;
                        if (SList[i].SortDesc == true)
                            f = f + " DESC";
                    }
                    else
                    {
                        f = f + ",";
                        f = f + SList[i].ColumnHeader;
                        if (SList[i].SortDesc == true)
                            f = f + " DESC";
                    }
                }
            }
            return f;
        }
    }
}
