using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperCarter.Model
{
    public class SequencesUpdatedEventArgs : EventArgs
    {
        public ObservableCollection<ScriptItemtype> Sequences { get; }

        public SequencesUpdatedEventArgs(ObservableCollection<ScriptItemtype> sequences)
        {
            Sequences = sequences;
        }
    }
}
