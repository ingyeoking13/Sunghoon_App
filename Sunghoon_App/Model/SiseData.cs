using Sunghoon_App.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunghoon_App.Model
{
    public class SiseData : Observable
    {
        public DateTime date { get; set; }
        public enum data
        {
            개인, 외국인, 기관계, 금융투자, 보험, 투신, 은행, 기타금융, 연기금등, 기타법인
        }
        public int[] data_arr { get; set; } = new int[10];
        public int[] gap_data { get; set; } = new int[10];

        public SiseData()
        {
            date = DateTime.Now.ToLocalTime();
        }

    }

    public class SiseDataList : ObservableCollection<SiseData>
    {
    }

}
