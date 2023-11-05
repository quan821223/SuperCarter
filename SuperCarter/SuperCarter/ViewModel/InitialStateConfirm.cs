using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.TextFormatting;

namespace SuperCarter.ViewModel
{
    public  class InitialStateConfirm : CustomscriptExecution
    {
     
        public InitialStateConfirm() {
           
        }
        ~InitialStateConfirm() { }
        private ICommand _testrun;
        public ICommand testrun
        {
            get
            {
                _testrun = new RelayCommand(param => ExecuteScriptInterface());
                return _testrun;
            }
        }

        public async Task ExecuteScriptInterface()
        {
            await base.evt_test_updateUI();
        }



    }
}
