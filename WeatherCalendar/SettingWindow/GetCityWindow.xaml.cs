using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using NPinyin;
using Weather;
using YUI.Controls;

namespace WeatherCalendar
{
    /// <summary>
    /// GetCityWindow.xaml 的交互逻辑
    /// </summary>
    public partial class GetCityWindow : Window
    {
        public event Action<CityKeyInfo> CityChanged;

        public string Text
        {
            get => yAutoCompleteTextbox.Text;
            set => yAutoCompleteTextbox.Text = value;
        }

        public CityKeyInfo City
        {
            get
            {
                if (yAutoCompleteTextbox.SelectedEntry != null &&
                    yAutoCompleteTextbox.SelectedEntry.ToString() == Text)
                {
                    return yAutoCompleteTextbox.SelectedEntry.Content as CityKeyInfo;
                }

                return null;
            }
        }

        public GetCityWindow()
        {
            InitializeComponent();
        }

        public void Initial()
        {
            yAutoCompleteTextbox.Entrys.Clear();
            foreach (var city in WeatherHelper.Instance.CityKeyInfos)
            {
                var keys = new List<string>();

                var pinyinName = Pinyin.GetPinyin(city.City, Encoding.UTF8).Trim().Replace(" ", "");
                var pinyinSName = Pinyin.GetInitials(city.City, Encoding.UTF8).Trim();

                keys.Add(pinyinSName);
                keys.Add(pinyinName);

                pinyinName = Pinyin.GetPinyin(city.Area, Encoding.UTF8).Trim().Replace(" ", "");
                pinyinSName = Pinyin.GetInitials(city.Area, Encoding.UTF8).Trim();

                keys.Add(pinyinSName);
                keys.Add(pinyinName);

                pinyinName = Pinyin.GetPinyin(city.Province, Encoding.UTF8).Trim().Replace(" ", "");
                pinyinSName = Pinyin.GetInitials(city.Province, Encoding.UTF8).Trim();

                keys.Add(pinyinSName);
                keys.Add(pinyinName);

                keys.Add(city.CityKey);

                yAutoCompleteTextbox.Entrys.Add(new YAutoCompleteEntry() { Content = city, Keywords = keys.ToArray() });
            }
        }

        private void CloseWindowCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CloseWindowCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Hide();
        }

        private void GetCityWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ButtonOK_OnClick(object sender, RoutedEventArgs e)
        {

            if (City == null)
            {
                yAutoCompleteTextbox.ShowPopUpOnControl("请选择城市！", 3000);
                return;
            }

            CityChanged?.Invoke(City);
            this.Hide();
        }

        private void GetCityWindow_OnActivated(object sender, EventArgs e)
        {
            yAutoCompleteTextbox.Focus();
        }

        private void YAutoCompleteTextbox_OnSelectedEntryChanged(object arg1, YAutoCompleteEntry arg2)
        {

        }

        private void GetCityWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Initial();
        }
    }
}
