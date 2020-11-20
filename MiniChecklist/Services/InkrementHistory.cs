using Prism.Mvvm;
using System.Collections.Generic;

namespace MiniChecklist.Services
{
    public class InkrementHistory : BindableBase
    {
        private bool _canPop;
        public bool CanPop
        {
            get { return _canPop; }
            set { SetProperty(ref _canPop, value); }
        }

        class Inkrement : List<string> { public Inkrement(List<string> tasks) : base(tasks) { } }
        Stack<Inkrement> _history = new Stack<Inkrement>();

        public InkrementHistory()
        {
            CanPop = _history.Count > 0;
        }

        public List<string> Pop()
        {
            var inkrement = _history.Pop();
            CanPop = _history.Count > 0;
            return (List<string>)inkrement;
        }

        public void Push(List<string> inkrement)
        {
            _history.Push(new Inkrement(inkrement));
            CanPop = true;
        }

        public void Clear()
        {
            _history.Clear();
            CanPop = _history.Count > 0;
        }
    }
}
