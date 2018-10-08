using HtmlAgilityPack;
using Microsoft.Data.Sqlite;
using Windows.System.Threading;
using Sunghoon_App.Helper;
using Sunghoon_App.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.Web.Http;
using Windows.UI.Xaml;
using Windows.UI.Popups;
using System.Threading;
using Windows.ApplicationModel.DataTransfer;
using System.ComponentModel;
using System.Collections.Specialized;

namespace Sunghoon_App.ViewModel
{
    public class MainPageViewModel : Observable
    {
        public string urlAddress { get; set; } = "https://finance.naver.com/sise/sise_trans_style.nhn";
        public string urldefault { get;} = "https://finance.naver.com";
        public bool Progress = false;
        public bool scheduled = false;
        private string timeUrlAddress;
        //private string dayUrlAddress;

        public string data { get; set; } // for test
        ThreadPoolTimer PeriodicTimer;

        public SiseDataList dataList { get; set; } = new SiseDataList();
        public SiseDataList tmp_dataList { get; set; } = new SiseDataList();

        public MainPageViewModel()
        {
            startButtonClick = new RelayCommand(Regist);
            quitButtonClick = new RelayCommand(Quit);
            copyButtonClick = new RelayCommand(Copy);
            dataList.CollectionChanged += ContentCollectionChanged;
        }

        private void ContentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (SiseData item in e.OldItems)
                {
                    item.PropertyChanged -= EntityViewModelPropertyChanged;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (SiseData item in e.NewItems)
                {
                    item.PropertyChanged += EntityViewModelPropertyChanged;
                }
            }
        }

        private void EntityViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("dataList");
        }

        public async void Regist()
        {
            scheduled = true;
            OnPropertyChanged("scheduled");
            TimeSpan period = TimeSpan.FromSeconds(20);
             PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer((source) =>
            {
                Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.High,
                () =>
                {
                    Click();
                });

            }, period);
        }
        public void Quit()
        {
            if (PeriodicTimer != null) PeriodicTimer.Cancel();
            scheduled = false;
            OnPropertyChanged("scheduled");
        }

        public void Copy()
        {
            DataPackage dataPackage = new DataPackage();
            dataPackage.RequestedOperation = DataPackageOperation.Copy;
            if (data == null) return;
            dataPackage.SetText(data);
            Clipboard.SetContent(dataPackage);
        }

        public async void Click()
        {
            Progress = true;
            OnPropertyChanged("Progress");
            HttpClient httpClient = new HttpClient();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var result = await httpClient.GetStringAsync(new Uri(urlAddress));
            data = result;

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(result);
            var node = htmlDoc.DocumentNode.SelectSingleNode("//iframe[@name='time']");
            var src = node.Attributes["src"].Value;

            timeUrlAddress = urldefault + src;
            result = await httpClient.GetStringAsync(new Uri(timeUrlAddress));
            htmlDoc.LoadHtml(result);
            var nodes = htmlDoc.DocumentNode.SelectNodes("//tr[count(td)=11]");

           // using (SqliteConnection db = new SqliteConnection("Filename=" + DBAccessor.dbFileName))
            //     db.Open();
            List<SiseData> tmp_list = new List<SiseData>();
        
            foreach (var i in nodes)
            {
                var childs = i.ChildNodes.Where(j => j.Name.Equals("td"));
                int cnt = -1;
                SiseData Sise = new SiseData();

                foreach (var c in childs)
                {
                    if (cnt == -1)
                    {
                        Sise.date = DateTime.Parse(childs.ElementAt(cnt + 1).InnerText);
                    }
                    else
                    {
                        string tmp = childs.ElementAt(cnt + 1).InnerText.Replace(",", string.Empty);
                        Sise.data_arr[cnt] = Int32.Parse(tmp);
                    }
                    cnt++;
                } // end of iterating td tags

                tmp_list.Add(Sise);
            } // tr iterating


            tmp_list.Reverse();
            while (dataList.Count != 0)
            {
                tmp_dataList.Add(dataList.First());
                dataList.RemoveAt(0);
            }

            // 다 돌았으니까, 리스트 두개를 비교해서 원래 리스트에 넣어줄게
            foreach (var i in tmp_list) 
            // tmp_list 의 개별적 원소가 dataList.Last보다 시간상 더 최근거면 푸쉬
            {
                if (tmp_dataList.Count == 0)
                {
                    tmp_dataList.Add(i);
                }
                else if (tmp_dataList.Last().date < i.date)
                {
                    tmp_dataList.Add(i);
                }
            }

            for (int i=1; i<tmp_dataList.Count; i++) // gap 구하기
            {
                for (int j = 0; j < 10; j++)
                {
                    tmp_dataList.ElementAt(i).gap_data[j] = tmp_dataList.ElementAt(i).data_arr[j] - tmp_dataList.ElementAt(i - 1).data_arr[j];
                }
            }
            while (tmp_dataList.Count != 0)
            {
                dataList.Add(tmp_dataList.First());
                tmp_dataList.RemoveAt(0);
            }

            // debugging 용
            data = "";

            foreach (var i in dataList)
            {

                data += i.date.ToString("yymmdd HH:mm") + "\t" + 
                    i.data_arr[ (int) SiseData.data.개인 ].ToString() + "\t" + 
                    i.gap_data[ (int) SiseData.data.개인 ].ToString() + "\t" + 
                    i.data_arr[ (int) SiseData.data.외국인 ] .ToString() + "\t"  + 
                    i.gap_data[ (int) SiseData.data.외국인 ] .ToString() + "\t"  + 
                    i.data_arr[ (int) SiseData.data.기관계 ].ToString()  + "\t" +
                    i.gap_data[ (int) SiseData.data.기관계 ].ToString()  + "\t" +
                    i.data_arr[ 3].ToString()  + "\t" +
                    i.gap_data[ 3].ToString()  + "\t" +
                    i.data_arr[ 5].ToString()  + "\t" +
                    i.gap_data[ 5].ToString()  + "\t" +
                    i.data_arr[ 7].ToString()  + "\t" +
                    i.gap_data[ 7].ToString()  + "\t" + Environment.NewLine;
            }

            OnPropertyChanged("data");
            OnPropertyChanged("dataList");
            Progress = false;
            OnPropertyChanged("Progress");
        }
        public RelayCommand startButtonClick { get; set; }
        public RelayCommand quitButtonClick { get; set; }
        public RelayCommand copyButtonClick { get; set; }
    }
}
