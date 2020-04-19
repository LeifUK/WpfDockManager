using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfListboxDemo
{
    class TabHeaderItem
    {
        public string Label { get; set; }
        public int ID { get; set; }

        public string HeaderText 
        {
            get 
            { 
                return Label + " : " + ID;
            }
        }
    }
}
