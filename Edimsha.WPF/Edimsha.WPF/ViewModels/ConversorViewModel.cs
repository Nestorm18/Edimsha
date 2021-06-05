using System;
using Edimsha.Core.Logging.Implementation;

namespace Edimsha.WPF.ViewModels
{
    public class ConversorViewModel : ViewModelBase
    {
        public ConversorViewModel()
        {
            Logger.Log("Constructor");
            Console.WriteLine("Test CONVERSOR-VM");
        }
    }
}