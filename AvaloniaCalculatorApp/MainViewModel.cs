using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaCalculatorApp
{
    internal class MainViewModel : ReactiveObject
    {
        private string mainTextBox;

        public string MainTextBox
        {
            get => mainTextBox;
            set => this.RaiseAndSetIfChanged(ref mainTextBox, value);
        }
    }
}
