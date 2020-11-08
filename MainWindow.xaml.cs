using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
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
using System.Windows.Threading;

namespace ArmyKnife
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        // 全体
        private string configFile = "../../../config.json";
        private Config conf;
        private DispatcherTimer timer;

        // DokuWikiタブ 関連変数
        private string DW_status = "DokuWiki";
        DokuwikiPageManager DW_dpm;
        DW_TreeViewItem DW_TreeViewRoot;
        // private var _DW_TreeViewRoot = new ObservableCollection();


        // LogViewerタブ 関連変数
        private string LV_logFile;
        private string LV_formatFile;
        private string LV_status = "Format: NONE, LogFile: NONE";

        // 将棋タブ 関連変数
        private string SY_status = "棋譜";

        // Redmineタブ 関連変数
        private string RM_status = "Redmine";


        public class Product
        {
            public string Name { get; set; }
            public int Price { get; set; }
            public double Tax { get; set; }
        }

        public MainWindow()
        {
            InitializeComponent();


            // Config読み込み
            string configFileStr = System.IO.File.ReadAllText(configFile);
            conf = JsonSerializer.Deserialize<Config>(configFileStr);

            timer = new DispatcherTimer();

            // デフォルトでチェックしておく
            //Radio_LogKind_FormPDFMaker.Checked = true;

            // DokuWiki設定
            PrepareDokuWiki();

            // SY_DataGrid.ItemsSource = new ObservableCollection<Product>
            //{
            //     new Product { Name="化粧品", Price=1900, Tax=10 },
            //   new Product { Name="洗剤", Price=500, Tax=10 },
            //    new Product { Name="パン", Price=800, Tax=8 },
            // new Product { Name="牛乳", Price=800, Tax=8 }
            //};

            // Timer_AK 始動
            this.timer.Start();

            // ログの表示
            ShowLog("../../logs/FormPDFMaker_Excel.log");
        }

        private void PrepareDokuWiki()
        {
            // DokuWikiへのログイン
            DW_dpm = new DokuwikiPageManager(conf.todoUrl, conf.todoUser, conf.todoPass);

            DW_UrlLabel.Content = "URL : " + conf.todoUrl + conf.todoPage;
            DW_UserPassLabel.Content = "User : " + conf.todoUser + ", Pass : ************";

            // DokuWikiのテキスト読み込み
            DW_PageContentTextBox.Text = DW_dpm.GetPageStr(conf.todoPage);
            DW_status = $"{conf.todoUrl}/{conf.todoPage} loaded.";

            DW_TreeViewRoot = new DW_TreeViewItem("root");

           

            //DW_TreeViewRoot.DW_TreeViewItems = new List<DW_TreeViewItem>
            //DW_TreeViewRoot.DW_TreeViewItems.Add(new DW_TreeViewItem("item1"));

            //var 


            //var dto1 = new Dto("Name1");
            //dto1.Dtos.Add(new Dto("Name1-1"));
            //dto1.Dtos.Add(new Dto("Name1-2"));
            //_dtos.Add(dto1);

            //CTreeView.ItemsSource = _dtos;


        }

        private void ShowLog(String _fileName)
        {
            // クリア
            //logListBox.Items.Clear();
            //string[] logs = System.IO.File.ReadAllLines(_fileName);

            //foreach (string log in logs)
            //{
            //    var item = new ListViewItem();
            //    item.Text = log;
            //    logListBox.Items.Add(item);
            //}

        }



        private void DW_LoadButton_Clicked(object sender, RoutedEventArgs e)
        {

        }

        private void DW_SaveButton_Clicked(object sender, RoutedEventArgs e)
        {

        }
    }
}
