namespace APSYS.Core.MVVM
{
    public class BaseViewModel<TView> : SimpleViewModel
    {
        public TView View { get; set; }
    }
}