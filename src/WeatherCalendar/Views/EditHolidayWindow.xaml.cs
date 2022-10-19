using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using WeatherCalendar.ViewModels;

namespace WeatherCalendar.Views;

/// <summary>
/// EditHolidayWindow.xaml 的交互逻辑
/// </summary>
public partial class EditHolidayWindow
{
    public EditHolidayWindow()
    {
        InitializeComponent();

        ViewModel = new EditHolidayWindowViewModel();

        this.WhenActivated(WhenActivated);
    }

    private void WhenActivated(CompositeDisposable disposable)
    {
        this.Bind(
                ViewModel,
                model => model.HolidayName,
                view => view.HolidayNameTextBox.Text)
            .DisposeWith(disposable);

        this.Bind(
                ViewModel,
                model => model.IsRestDay,
                view => view.RestRadioButton.IsChecked)
            .DisposeWith(disposable);

        this.OneWayBind(
                ViewModel,
                model => model.HolidayName,
                view => view.ErrorTextBlock.Visibility,
                name => string.IsNullOrWhiteSpace(name) ? Visibility.Visible : Visibility.Hidden)
            .DisposeWith(disposable);

        this.BindCommand(
                ViewModel!,
                model => model.ConfirmCommand,
                view => view.OkButton)
            .DisposeWith(disposable);

        this.TitleBorder
            .Events()
            .MouseLeftButtonDown
            .Do(_ => DragMove())
            .Subscribe()
            .DisposeWith(disposable);

        this.CloseButton
            .Events()
            .Click
            .Do(_ => Close())
            .Subscribe()
            .DisposeWith(disposable);

        this.CancelButton
            .Events()
            .Click
            .Do(_ => Close())
            .Subscribe()
            .DisposeWith(disposable);

        this.ViewModel!
            .ConfirmedInteraction
            .RegisterHandler(interaction =>
            {
                interaction.SetOutput(Unit.Default);
                Close();
            })
            .DisposeWith(disposable);
    }
}
