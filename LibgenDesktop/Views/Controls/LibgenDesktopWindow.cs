using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using LibgenDesktop.Infrastructure;

namespace LibgenDesktop.Views.Controls
{
    public class LibgenDesktopWindow : Window
    {
        public static readonly DependencyProperty ShowIconProperty = DependencyProperty.Register("ShowIcon", typeof(bool), typeof(LibgenDesktopWindow),
            new PropertyMetadata(true));

        public static readonly DependencyProperty ShowMinimizeButtonProperty = DependencyProperty.Register("ShowMinimizeButton", typeof(bool),
            typeof(LibgenDesktopWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty ShowMaximizeButtonProperty = DependencyProperty.Register("ShowMaximizeButton", typeof(bool),
            typeof(LibgenDesktopWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty ShowCloseButtonProperty = DependencyProperty.Register("ShowCloseButton", typeof(bool),
            typeof(LibgenDesktopWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty ClosingCommandProperty = DependencyProperty.Register("ClosingCommand", typeof(FuncCommand<bool?, bool>),
            typeof(LibgenDesktopWindow));

        public static readonly DependencyProperty ClosedCommandProperty = DependencyProperty.Register("ClosedCommand", typeof(ICommand),
            typeof(LibgenDesktopWindow));

        public bool ShowIcon
        {
            get
            {
                return (bool)GetValue(ShowIconProperty);
            }
            set
            {
                SetValue(ShowIconProperty, value);
            }
        }

        public bool ShowMinimizeButton
        {
            get
            {
                return (bool)GetValue(ShowMinimizeButtonProperty);
            }
            set
            {
                SetValue(ShowMinimizeButtonProperty, value);
            }
        }

        public bool ShowMaximizeButton
        {
            get
            {
                return (bool)GetValue(ShowMaximizeButtonProperty);
            }
            set
            {
                SetValue(ShowMaximizeButtonProperty, value);
            }
        }

        public bool ShowCloseButton
        {
            get
            {
                return (bool)GetValue(ShowCloseButtonProperty);
            }
            set
            {
                SetValue(ShowCloseButtonProperty, value);
            }
        }

        public FuncCommand<bool?, bool> ClosingCommand
        {
            get
            {
                return (FuncCommand<bool?, bool>)GetValue(ClosingCommandProperty);
            }
            set
            {
                SetValue(ClosingCommandProperty, value);
            }
        }

        public ICommand ClosedCommand
        {
            get
            {
                return (ICommand)GetValue(ClosedCommandProperty);
            }
            set
            {
                SetValue(ClosedCommandProperty, value);
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            if (!ShowIcon)
            {
                this.RemoveWindowIcon();
            }
            if (!ShowMinimizeButton)
            {
                this.RemoveWindowMinimizeButton();
            }
            if (!ShowMaximizeButton)
            {
                this.RemoveWindowMaximizeButton();
            }
            if (!ShowCloseButton)
            {
                this.RemoveWindowCloseButton();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            FuncCommand<bool?, bool> closingCommand = ClosingCommand;
            if (closingCommand != null)
            {
                e.Cancel = !closingCommand.ExecuteWithTypedParameter(DialogResult);
            }
            if (!e.Cancel)
            {
                base.OnClosing(e);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            ICommand closedCommand = ClosedCommand;
            if (closedCommand != null)
            {
                closedCommand.Execute(null);
            }
            base.OnClosed(e);
        }
    }
}
