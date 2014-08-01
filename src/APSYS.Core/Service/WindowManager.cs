﻿namespace APSYS.Core.Service
{
    using System.Windows;
    using System.Windows.Controls;
    using Autofac;

    public class WindowManager
    {
        private readonly IContainer _container;

        public WindowManager(ApsysContainer apsysContainer)
        {
            _container = apsysContainer.Container;
        }

        public void ShowDialogWindow<TView, TViewModel>()
            where TView : BaseView
            where TViewModel : BaseViewModel<TView>
        {
            var view = BuildView<TView, TViewModel>();
            view.ShowDialog();
        }

        public void ShowWindow<TView, TViewModel>()
            where TView : BaseView
            where TViewModel : BaseViewModel<TView>
        {
            var view = BuildView<TView, TViewModel>();
            view.Show();
        }

        public TView OpenControl<TView, TViewModel>()
            where TView : BaseControlView
            where TViewModel : BaseViewModel<TView>
        {
            var view = _container.Resolve<TView>();
            var viewModel = _container.Resolve<TViewModel>();
            view.DataContext = viewModel;
            viewModel.View = view;
            view.Initialize();
            viewModel.Initialize();
            return view;
        }

        private TView BuildView<TView, TViewModel>()
            where TView : BaseView
            where TViewModel : BaseViewModel<TView>
        {
            var view = _container.Resolve<TView>();
            var viewModel = _container.Resolve<TViewModel>();
            view.DataContext = viewModel;
            viewModel.View = view;
            view.Initialize();
            viewModel.Initialize();
            return view;
        }

        public BaseControlView OpenControl(string viewName, string viewModelName)
        {
            var view = _container.ResolveNamed<BaseControlView>(viewName);
            var viewModel = (BaseViewModel)_container.ResolveNamed(viewModelName, typeof(BaseViewModel<>));
            view.DataContext = viewModel;
            view.Initialize();
            viewModel.Initialize();
            return view;
        }

        public BaseControlView OpenControl(BaseControlView view, BaseViewModel model)
        {
            view.DataContext = model;
            view.Initialize();
            model.Initialize();
            return view;
        }
    }

    public class BaseControlView : UserControl
    {
        public virtual void Initialize()
        {
        }
    }

    public class BaseView : Window
    {
        public virtual void Initialize()
        {
        }
    }
}