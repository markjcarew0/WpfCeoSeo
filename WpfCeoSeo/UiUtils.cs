// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UiUtils.cs" company="MarkJC">
//   Author Mark Carew
// </copyright>
// <summary>
//various utility methods to allow setting focus to datagrid rows
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WpfCeoSeo
{
    using System;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;

    public static class UiUtils
    {
        /// <summary>
        /// this should be in a datagrid helper class
        /// </summary>
        /// <param name="cellInfo"></param>
        /// <returns></returns>
        /// <summary>
        ///     The set focus to data grid.
        /// </summary>
        /// <param name="grid">
        ///     The grid.
        /// </param>
        public static void SetFocusToDataGrid(DataGrid grid)
        {
            if (grid.Items.Count > 0)
            {
                // If nothing is selected, then select the first prescriber
                if (grid.SelectedIndex == -1)
                {
                    grid.SelectedIndex = 0;
                }

                grid.ScrollIntoView(grid.SelectedItem);
                var selectedRow = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(grid.SelectedIndex);
                if (selectedRow != null)
                {
                    FocusManager.SetIsFocusScope(selectedRow, true);
                    FocusManager.SetFocusedElement(selectedRow, selectedRow);

                    selectedRow.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    Keyboard.Focus(selectedRow);
                }
            }

            // Set focus
            if (!grid.IsKeyboardFocusWithin)
            {
                grid.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        /// <summary>
        /// The set focus to data grid first row.
        /// </summary>
        /// <param name="grid">
        /// The grid.
        /// </param>
        public static void SetFocusToDataGridFirstRow(DataGrid grid)
        {
            if (grid.Items.Count > 0)
            {
                grid.SelectedIndex = 0;

                // Required for row to scroll into view, must happen before focus is changed
                grid.ScrollIntoView(grid.SelectedItem);

                var selectedRow = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(grid.SelectedIndex);
                if (selectedRow != null)
                {
                    FocusManager.SetIsFocusScope(selectedRow, true);
                    FocusManager.SetFocusedElement(selectedRow, selectedRow);

                    // .First is required so that the selection scrolls into view
                    selectedRow.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));

                    // As we have had hard to replicate focus issues, both forms of focus have been used to help ensure that no focus ambiguity can occur
                    selectedRow.Focus();
                }
            }

            // Set focus, fail over if (grid.Items.Count > 0) unable to set focus 
            if (!grid.IsKeyboardFocusWithin)
            {
                grid.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            }
        }

        /// <summary>
        /// The select row by index.
        /// </summary>
        /// <param name="dataGrid">
        /// The data grid.
        /// </param>
        /// <param name="rowIndex">
        /// The row index.
        /// </param>
        public static void SelectRowByIndex(DataGrid dataGrid, int rowIndex)
        {
            if (!dataGrid.SelectionUnit.Equals(DataGridSelectionUnit.FullRow))
            {
                throw new ArgumentException("The SelectionUnit of the DataGrid must be set to FullRow.");
            }

            if (rowIndex < 0 || rowIndex > (dataGrid.Items.Count - 1))
            {
                throw new ArgumentException($"{rowIndex} is an invalid row index.");
            }

            /* set the SelectedItem property */
            object item = dataGrid.Items[rowIndex]; // = Product X
            dataGrid.SelectedItem = item;

            DataGridRow row = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
            if (row == null)
            {
                /* bring the data item (Product object) into view
                 * in case it has been virtualized away */
                dataGrid.ScrollIntoView(item);
                row = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
            }

            if (row != null)
            {
                DataGridCell cell = GetCell(dataGrid, row, 0);
                cell?.Focus();
            }
        }

        /// <summary>
        /// The get cell.
        /// </summary>
        /// <param name="dataGrid">
        /// The data grid.
        /// </param>
        /// <param name="rowContainer">
        /// The row container.
        /// </param>
        /// <param name="column">
        /// The column.
        /// </param>
        /// <returns>
        /// The <see cref="DataGridCell"/>.
        /// </returns>
        public static DataGridCell GetCell(DataGrid dataGrid, DataGridRow rowContainer, int column)
        {
            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
                if (presenter == null)
                {
                    /* if the row has been virtualized away, call its ApplyTemplate() method
                     * to build its visual tree in order for the DataGridCellsPresenter
                     * and the DataGridCells to be created */
                    rowContainer.ApplyTemplate();
                    presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
                }

                if (presenter != null)
                {
                    DataGridCell cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
                    if (cell == null)
                    {
                        /* bring the column into view
                         * in case it has been virtualized away */
                        dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[column]);
                        cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
                    }

                    return cell;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the visual child.
        /// </summary>
        /// <typeparam name="T">
        /// The type of visual to find.
        /// </typeparam>
        /// <param name="parent">
        /// The parent.
        /// </param>
        /// <returns>
        /// The first child of Type T.
        /// </returns>
        internal static T FindVisualChild<T>(Visual parent) where T : Visual
        {
            var child = default(T);
            var numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < numVisuals; i++)
            {
                var v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T ?? FindVisualChild<T>(v);
                if (child != null)
                {
                    break;
                }
            }

            return child;
        }
    }
}
