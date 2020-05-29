using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleDockManagerViews.ViewModel
{
    public class ToolViewModel : DummyViewModel
    {
        public override string LongTitle
        {
            get
            {
                return Title;
            }
        }
    }
}
