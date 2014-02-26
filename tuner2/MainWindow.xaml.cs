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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace tuner2 {
    using System.Net;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        WebClient wget = new WebClient();
        public MainWindow() {
            InitializeComponent();

            var showJson = wget.DownloadString("http://eric-pc/tvtuner/myapi/getshows");
            var showData = JsonConvert.DeserializeObject<ShowsRootObject>(showJson);

        }
    }

    public class ShowCategory {
        public string Key { get; set; }
        public List<ShowInfo> Value { get; set; }
    }

    public class ShowsRootObject {
        public List<ShowCategory> showCategories { get; set; }
    }

    public class ShowInfo {
        public int SeriesId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
