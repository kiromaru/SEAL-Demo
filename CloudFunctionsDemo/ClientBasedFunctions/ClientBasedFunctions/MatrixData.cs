// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Data;

namespace SEALAzureFuncClient
{
    /// <summary>
    /// Class used as the Data Context of a Matrix Editor.
    /// </summary>
    public class MatrixData
    {
        private DataTable dt_ = null;
        private DataView dv_ = null;

        /// <summary>
        /// Build an instance of MatrixData
        /// </summary>
        public MatrixData()
        {
            dt_ = new DataTable();
            dt_.ColumnChanging += OnColumnChanging;
            dv_ = dt_.DefaultView;
        }

        /// <summary>
        /// Handle event when a column is changing. Will use to validate that the value entered are
        /// between [-128, 127].
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments.</param>
        private void OnColumnChanging(object sender, DataColumnChangeEventArgs e)
        {
            int propVal = (int)e.ProposedValue;
            if (propVal < -128)
            {
                e.ProposedValue = -128;
            }
            else if (propVal > 127)
            {
                e.ProposedValue = 127;
            }
        }

        /// <summary>
        /// DataTable that contains the Matrix Data
        /// </summary>
        internal DataTable DataTable
        {
            get
            {
                return dt_;
            }
        }

        /// <summary>
        /// View to the Data Table
        /// </summary>
        public DataView DataView
        {
            get
            {
                return dv_;
            }
        }
    }
}
