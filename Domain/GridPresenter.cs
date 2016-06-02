﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Domain.Annotations;
using MColor = System.Windows.Media.Color;
using DColor = System.Drawing.Color;

namespace Domain {
    public class GridPresenter : Notifiable {
        private int _demoSelection;
        private PicrossGrid _grid;
        private PicrossSolver _solver;

        private List<RowPresenter> _rows;

        public List<RowPresenter> Rows {
            get { return _rows; }
            set { SetNotify(() => Rows, ref _rows, value); }
        }
        private List<ClassifierPresenter> _rowClassifiers;
        public List<ClassifierPresenter> RowClassifiers {
            get { return _rowClassifiers; }
            set { SetNotify(() => RowClassifiers, ref _rowClassifiers, value); }
        }

        private List<ClassifierPresenter> _columnClassifiers;
        public List<ClassifierPresenter> ColumnClassifiers {
            get { return _columnClassifiers; }
            set { SetNotify(() => ColumnClassifiers, ref _columnClassifiers, value); }
        }

        public void LoadDemoData() {
            GridHelpers.ResetCache();
            _grid = new PicrossGrid();
            switch (_demoSelection) {
                case 0: _grid.InitFromImg(@"LevelImages\EasyGallery2\Small01.png");break;
                case 1: _grid.InitFromImg(@"LevelImages\EasyGallery2\Medium01.png"); break;
                case 2: _grid.InitFromImg(@"LevelImages\EasyGallery2\Large01.png"); break;
                case 3: _grid.InitFromImg(@"LevelImages\EasyGallery4\XLarge01.png"); break;
                default:throw new Exception("Unknown _demoSelection");
            }
            _demoSelection++;
            if (_demoSelection == 4) _demoSelection = 0;
            _solver = new PicrossSolver(_grid.RowCount, _grid.ColumnCount, _grid.Rows, _grid.Columns);
            _solver.Solve();
            Rows = new List<RowPresenter>();
            for (int i = 0; i < _grid.RowCount; i++) {
                Rows.Add(new RowPresenter(_solver.WorkingGrid.GetRow(i)));
            }
            Notify(() => Rows);
            RowClassifiers = _solver.Rows.Select(kvp => new ClassifierPresenter(kvp.Value)).ToList();
            ColumnClassifiers = _solver.Columns.Select(kvp => new ClassifierPresenter(kvp.Value)).ToList();
        }
    }

    public abstract class RowPresenterBase : Notifiable {
        private List<CellPresenter> _cells;
        public List<CellPresenter> Cells {
            get { return _cells; }
            set { SetNotify(() => Cells, ref _cells, value); }
        }
    }

    public class RowPresenter : RowPresenterBase {
        public RowPresenter(DColor[] row) {
            Cells = row.Select(c => new CellPresenter(c)).ToList();
        }
    }

    public class ClassifierPresenter : RowPresenterBase {
        public ClassifierPresenter(Classifier row) {
            Cells = row.Colors.Select(kvp => new CellPresenter(kvp.Value)).ToList();
        }
    }

    public class CellPresenter : Notifiable {
        private SolidColorBrush _myColor = Brushes.Transparent;

        public SolidColorBrush MyColor {
            get { return _myColor; }
            set { SetNotify(() => MyColor, ref _myColor, value); }
        }

        private string _count = string.Empty;
        public string Count {
            get { return _count; }
            set { SetNotify(() => Count, ref _count, value); }
        }

        private bool _isConnected;
        public bool IsConnected {
            get { return _isConnected; }
            set { SetNotify(() => IsConnected, ref _isConnected, value); }
        }

        public CellPresenter(DColor color) {
            MyColor = new SolidColorBrush(color.ToMediaColor());
        }

        public CellPresenter(ColorClassifier value) {
            MyColor = new SolidColorBrush(value.MyColor.ToMediaColor());
            Count = value.Count.ToString();
            IsConnected = value.IsConnected;
        }
    }

    public static class ColorExtensions {
        public static MColor ToMediaColor(this DColor color) {
            return MColor.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}