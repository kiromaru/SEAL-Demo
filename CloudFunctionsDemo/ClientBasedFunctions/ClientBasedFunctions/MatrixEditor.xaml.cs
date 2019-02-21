// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace SEALAzureFuncClient
{
    /// <summary>
    /// Interaction logic for MatrixEditor.xaml
    /// </summary>
    public partial class MatrixEditor : UserControl
    {
        /// <summary>
        /// Build an instance of MatrixEditor
        /// </summary>
        public MatrixEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handle selection change in the ComboBox that specifies the Matrix size.
        /// </summary>
        /// <param name="sender">ComboBox containing matrix sizes</param>
        /// <param name="e">Selection changed event arguments</param>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int rows = Rows.SelectedIndex + 1;
            int cols = Columns.SelectedIndex + 1;
            if (rows > 0 && cols > 0)
            {
                InitMatrix(new int[rows, cols]);
            }
        }

        /// <summary>
        /// Initialize a MatrixData instance of the appropriate size
        /// </summary>
        /// <param name="rows">Rows in MatrixData</param>
        /// <param name="columns">Columns in MatrixData</param>
        /// <returns>Initialized MatrixData</returns>
        private static MatrixData InitMatrixData(int[,] matrix)
        {
            MatrixData data = new MatrixData();
            int rows = matrix.GetLength(dimension: 0);
            int cols = matrix.GetLength(dimension: 1);

            for (int c = 0; c < cols; c++)
            {
                data.DataTable.Columns.Add((c + 1).ToString());
                data.DataTable.Columns[c].DataType = typeof(int);
                data.DataTable.Columns[c].AllowDBNull = false;
            }

            for (int r = 0; r < rows; r++)
            {
                object[] rowVals = new object[cols];
                for (int c = 0; c < cols; c++)
                {
                    rowVals[c] = matrix[r, c];
                }

                data.DataTable.Rows.Add(rowVals);
            }

            return data;
        }

        /// <summary>
        /// Initialize data grid with the given matrix
        /// </summary>
        /// <param name="matrix"></param>
        public void InitMatrix(int[,] matrix)
        {
            MatrixData data = InitMatrixData(matrix);
            MatrixData oldData = DataContext as MatrixData;

            DataContext = data;

            bool matrixChanged = (oldData == null) || (null != oldData &&
                (oldData.DataTable.Rows.Count != data.DataTable.Rows.Count ||
                 oldData.DataTable.Columns.Count != data.DataTable.Columns.Count));

            if (matrixChanged)
            {
                MatrixSizeChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Clear data grid
        /// </summary>
        public void ClearMatrix()
        {
            DataContext = null;
        }

        /// <summary>
        /// Get the current size of the Matrix.
        /// </summary>
        public Tuple<int, int> MatrixSize
        {
            get
            {
                return new Tuple<int, int>(Rows.SelectedIndex + 1, Columns.SelectedIndex + 1);
            }
        }

        private static DependencyProperty MatrixTitleProperty = DependencyProperty.Register("MatrixTitle", typeof(string), typeof(MatrixEditor), new PropertyMetadata(defaultValue: null));

        /// <summary>
        /// Get/Set the title of the Matrix
        /// </summary>
        public string MatrixTitle
        {
            get
            {
                return (string)GetValue(MatrixTitleProperty);
            }
            set
            {
                SetValue(MatrixTitleProperty, value);
            }
        }

        private static DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(MatrixEditor), new PropertyMetadata(defaultValue: false));

        /// <summary>
        /// Determines whether the Matrix Editor is read only
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return (bool)GetValue(IsReadOnlyProperty);
            }
            set
            {
                SetValue(IsReadOnlyProperty, value);
            }
        }

        /// <summary>
        /// Event that is fired when the size of the Matrix changes
        /// </summary>
        public event EventHandler MatrixSizeChanged;
    }
}
