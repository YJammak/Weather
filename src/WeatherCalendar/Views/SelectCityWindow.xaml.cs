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
/// SelectCityWindow.xaml 的交互逻辑
/// </summary>
public partial class SelectCityWindow
{
    public SelectCityWindow()
    {
        InitializeComponent();

        ViewModel = new SelectCityWindowViewModel();

        this.WhenActivated(WhenActivated);
    }

    private void WhenActivated(CompositeDisposable disposable)
    {
        this.OneWayBind(
                ViewModel,
                model => model.Provinces,
                view => view.ProvincesComboBox.ItemsSource)
            .DisposeWith(disposable);

        this.OneWayBind(
                ViewModel,
                model => model.Districts,
                view => view.DistrictsComboBox.ItemsSource)
            .DisposeWith(disposable);

        this.OneWayBind(
                ViewModel,
                model => model.Cities,
                view => view.CitiesComboBox.ItemsSource)
            .DisposeWith(disposable);

        this.Bind(
                ViewModel,
                model => model.SelectedProvince,
                view => view.ProvincesComboBox.SelectedValue)
            .DisposeWith(disposable);

        this.Bind(
                ViewModel,
                model => model.SelectedDistrict,
                view => view.DistrictsComboBox.SelectedValue)
            .DisposeWith(disposable);

        this.Bind(
                ViewModel,
                model => model.SelectedCity,
                view => view.CitiesComboBox.SelectedValue)
            .DisposeWith(disposable);

        this.OneWayBind(
                ViewModel,
                model => model.SelectedCityInfo,
                view => view.ErrorTextBlock.Visibility,
                city => city == null ? Visibility.Visible : Visibility.Hidden)
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

        this.BindCommand(
            ViewModel!,
            mode => mode.UpdateWeatherCommand,
            view => view.OkButton);

        this.ViewModel
            .UpdateSuccessInteraction
            .RegisterHandler(interaction =>
            {
                Close();
                interaction.SetOutput(Unit.Default);
            })
            .DisposeWith(disposable);
    }
}
