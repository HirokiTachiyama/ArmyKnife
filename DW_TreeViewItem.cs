using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Documents;

namespace ArmyKnife
{
    class DW_TreeViewItem
    {
        public DW_TreeViewItem(string _name)
        {
            Name = _name;
        }

        public string Name { get; set; }
        public List DW_TreeViewItems { get; set; } = new List();
    }
}
