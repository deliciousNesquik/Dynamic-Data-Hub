using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DynamicDataHub.Interfaces
{
    public interface IDataTableControl
    {
        event EventHandler<DataGridCellEditEndingEventArgs> CellEditEnding;
        event EventHandler<DataGridPreparingCellForEditEventArgs> PreparingCellForEdit;
    }
}
