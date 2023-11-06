using SuperCarter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.TextFormatting;

namespace SuperCarter.ViewModel
{
    public  class InitialStateConfirm : CustomScriptEditor
    {
     
        public InitialStateConfirm() {
           
        }
        ~InitialStateConfirm() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
      


    }
}
