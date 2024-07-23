using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDataHub.Modules
{
    public class HistoryManager
    {
        private List<string> _history = new List<string>();

        public List<string> History
        {
            get { return _history; }
        }

        public void AddToHistory(string value)
        {
            if (!_history.Contains(value))
            {
                _history.Insert(0, value);
            }
        }
    }
}
