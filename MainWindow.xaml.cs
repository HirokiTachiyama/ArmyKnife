using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
            DW_PageContentTextBox.Text = DW_dpm.GetPageStr(conf.todoPage);
            DW_status = "Loaded.";
            UpdateStatusLabel();
        }

        private void DW_SaveButton_Clicked(object sender, RoutedEventArgs e)
        {
            bool ret = DW_dpm.PutPage(DW_PageContentTextBox.Text, conf.todoPage);

            if (ret)
            {
                DW_status = "put success.";
            }
            else
            {
                DW_status = "put failed.";
            }
            UpdateStatusLabel();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (this.AK_TabControl.SelectedIndex)
            {
                case 0: // DokuWiki
                    DW_TabItem_Selected();
                    break;
                case 1: // Syogi
                    SY_TabItem_Selected();
                    break;
                case 2: // LogViewer
                    LV_TabItem_Selected();
                    break;
                case 3: // RedMine
                    RM_TabItem_Selected();
                    break;
                default:
                    break;
            }

        }

        private void LV_TabItem_Selected()
        {
            Console.WriteLine("LogViewer");
            UpdateStatusLabel();
        }

        private void SY_TabItem_Selected()
        {
            Console.WriteLine("Syogi");
            UpdateStatusLabel();
        }

        private void DW_TabItem_Selected()
        {
            Console.WriteLine("DokuWiki");
            UpdateStatusLabel();
        }

        private void RM_TabItem_Selected()
        {
            Console.WriteLine("Redmine");



            UpdateStatusLabel();
        }


        // ステータステキストのアップデート
        private void UpdateStatusLabel()
        {
            // 現在のタブに応じて分岐
            switch (AK_TabControl.SelectedIndex)
            {
                case 0: // DokuWiki
                    AK_StatusLabel.Content = DW_status;
                    break;
                case 1: // Syogi
                    AK_StatusLabel.Content = SY_status;
                    break;
                case 2: // LogViewer
                    AK_StatusLabel.Content = LV_status;
                    break;
                case 3: // Redmine
                    AK_StatusLabel.Content = RM_status;
                    break;
                default:
                    break;
            }

        }

        private void testTB_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private async void shogibutton_ClickAsync(object sender, RoutedEventArgs e)
        {

            string text;
            WebClient wc = new WebClient();
            try
            {
                text = wc.DownloadString(shogitb.Text);
                testTB.Text = text.Substring(text.IndexOf("var data")/*, text.IndexOf("</script>")*/).ToString();
                
//                testTB.Text = text[text.IndexOf("var data")..].ToString();

            }
            catch (WebException we)
            {
                testTB.Text = we.ToString();
            }
            _ = Task.Run(async () =>
                {
                    try
                    {


                        // 株価を取得したいサイトのURL
                        var code = "eff8410f11971491eef56d0d2f506c2114ee5fd1";
                        var urlstring = $"https://shogidb2.com/games/{code}";

                        // 指定したサイトのHTMLをストリームで取得する
                        var doc = default(IHtmlDocument);
                        using (var client = new HttpClient())
                        using (var stream = await client.GetStreamAsync(new Uri(urlstring)))
                        {
                            // AngleSharp.Html.Parser.HtmlParserオブジェクトにHTMLをパースさせる
                            var parser = new HtmlParser();
                            //doc = await parser.ParseDocumentAsync(stream);


                            var document = await parser.ParseDocumentAsync(wc.DownloadString(shogitb.Text));
                            Debug.WriteLine(document.ToString());
                            Console.WriteLine(document.ToString());

                        }



                        // クエリーセレクタを指定し株価部分を取得する
                        var priceElement = doc.QuerySelector("#main td[class=stoksPrice]");
                        var hoge = doc.QuerySelector("<script>");

                        Debug.WriteLine(hoge.TextContent.ToString());

                        // 取得した株価がstring型なのでint型にパースする
                        //int.TryParse(priceElement.TextContent, NumberStyles.AllowThousands, null, out var price);
                        //Debug.WriteLine("コクヨ(7984.T)の株価: {0}円", price);

                    }catch (Exception e)
                    {
                        Debug.WriteLine(e);
                    }
                });

//            testMethodAsync();
            
        }

        private async Task testMethodAsync()
        {
            // 株価を取得したいサイトのURL
            var code = "eff8410f11971491eef56d0d2f506c2114ee5fd1";
            var urlstring = $"https://shogidb2.com/games/{code}";

            // 指定したサイトのHTMLをストリームで取得する
            var doc = default(IHtmlDocument);
            using (var client = new HttpClient())
            using (var stream = await client.GetStreamAsync(new Uri(urlstring)))
            {
                // AngleSharp.Html.Parser.HtmlParserオブジェクトにHTMLをパースさせる
                var parser = new HtmlParser();
                doc = await parser.ParseDocumentAsync(stream);
            }

            // クエリーセレクタを指定し株価部分を取得する
            var priceElement = doc.QuerySelector("#main td[class=stoksPrice]");

            var hoge = doc.QuerySelector("<script>");

            Debug.WriteLine(hoge.TextContent.ToString());

            // 取得した株価がstring型なのでint型にパースする
            int.TryParse(priceElement.TextContent, NumberStyles.AllowThousands, null, out var price);

            Debug.WriteLine("コクヨ(7984.T)の株価: {0}円", price);

                

        }

        private void SY_TabItem_KeyDown(object sender, KeyEventArgs e)
        {
            // 歩FU 香KY 桂KE 銀GI 金KI 角KA 飛HI 王OU と金TO 成香NY 成桂NK 成銀NG 馬UM 龍RY
            string tmp = string.Empty;
            switch (e.Key)
            {
                case Key.S: // 先手番
                    tmp = "▲"; break;
                case Key.D: // 後手番
                    tmp = "△"; break;
                case Key.D0 or Key.D1 or Key.D2 or Key.D3 or Key.D4: // "D0", "D1", ...
                case Key.D5 or Key.D6 or Key.D7 or Key.D8 or Key.D9:
                    tmp = e.Key.ToString().Substring(1); break;
                case Key.F or Key.U or Key.K or Key.E or Key.G or Key.I:
                case Key.O or Key.M or Key.R or Key.Y or Key.T or Key.H:
                    tmp = e.Key.ToString();
                    break;
                case Key.Enter:
                    //do_sashite();
                    make_goban(SY_CommandLabel.Content.ToString()[0]);
                    break;
                default:
                    SY_CommandLabel.Content = "";
                    break;
            }
            SY_CommandLabel.Content = SY_CommandLabel.Content.ToString() + tmp;

            convert_sashite();
        }

        void make_goban(char _teban)
        {
            LV_Grid.Children.Clear();
            RowDefinition[] dan = new RowDefinition[9]; // 行, 段, 横
            ColumnDefinition[] suji = new ColumnDefinition[9]; // 列, 筋, 縦

            // 段, 筋の初期化
            for(int i=0; i<9; i++)
            {
                dan[i] = new RowDefinition();
                LV_Grid.RowDefinitions.Add(dan[i]);
                dan[i].Height = new GridLength(25);

                suji[i] = new ColumnDefinition();
                LV_Grid.ColumnDefinitions.Add(suji[i]);
                suji[i].Width = new GridLength(30);
            }

            Label[,] gobanLabel = new Label[10, 10];


            if (_teban == '▲')
            {
                // 先手番の盤の向き
                for (int i = 8; i >= 0; i--)
                {
                    for (int j = 8; j >= 0; j--)
                    {
                        gobanLabel[i + 1, j + 1] = new Label();
                        gobanLabel[i + 1, j + 1].Content = $"{i + 1}, {j + 1}";
                        Grid.SetRow(gobanLabel[i + 1, j + 1], i);
                        Grid.SetColumn(gobanLabel[i + 1, j + 1], 8 - j);
                        LV_Grid.Children.Add(gobanLabel[i + 1, j + 1]);
                    }
                }
            }
            else if (_teban == '△')
            {

                // 後手番の盤の向き
                for (int i = 0; i <= 8; i++)
                {
                    for (int j = 0; j <= 8; j++)
                    {
                        gobanLabel[i + 1, j + 1] = new Label();
                        gobanLabel[i + 1, j + 1].Content = $"{i + 1}, {j + 1}";
                        Grid.SetRow(gobanLabel[i + 1, j + 1], i);
                        Grid.SetColumn(gobanLabel[i + 1, j + 1], 8 - j);
                        LV_Grid.Children.Add(gobanLabel[i + 1, j + 1]);
                    }
                }

            }


            gobanLabel[5, 5].Content = "歩";
            gobanLabel[1, 1].Content = "香";


        }


        void convert_sashite()
        {
            string tmp = SY_CommandLabel.Content.ToString();
            int length = tmp.Length;
            string kanji = string.Empty;
            
            if(Regex.IsMatch(tmp, @"[1-9]{2,2}")) // 2桁の数字が登場したら、一の位を漢数字に変える
            {
                switch (tmp.Substring(length - 1))
                {
                    case "1": kanji = "一"; break;
                    case "2": kanji = "二"; break;
                    case "3": kanji = "三"; break;
                    case "4": kanji = "四"; break;
                    case "5": kanji = "五"; break;
                    case "6": kanji = "六"; break;
                    case "7": kanji = "七"; break;
                    case "8": kanji = "八"; break;
                    case "9": kanji = "九"; break;
                    default: break;
                }
                tmp = tmp.Substring(0, length - 1) + kanji;
                SY_CommandLabel.Content = tmp;
            } else if(Regex.IsMatch(tmp, @"[A-Y]{2,2}")){ // 2文字のアルファベットが登場したら漢字に変える
                switch (tmp.Substring(length - 2)) {
                    // 歩FU 香KY 桂KE 銀GI 金KI 角KA 飛HI 王OU と金TO 成香NY 成桂NK 成銀NG 馬UM 龍RY
                    case "FU": kanji = "歩"; break;
                    case "KY": kanji = "香"; break;
                    case "KE": kanji = "桂"; break;
                    case "GI": kanji = "銀"; break;
                    case "KI": kanji = "金"; break;
                    case "KA": kanji = "角"; break;
                    case "HI": kanji = "飛"; break;
                    case "OU": kanji = "王"; break;
                    case "TO": kanji = "と"; break;
                    case "NY": kanji = "成香"; break;
                    case "NG": kanji = "成銀"; break;
                    case "UM": kanji = "馬"; break;
                    case "RY": kanji = "龍"; break;
                    default: break;
                }
                tmp = tmp.Substring(0, length - 2) + kanji;
                SY_CommandLabel.Content = tmp;
            }
        }

        void do_sashite()
        {

            try
            {
                string tmp = SY_CommandLabel.Content.ToString();

                char teban = tmp[0];
                char suji = tmp[1];
                char dan = tmp[2];
                SY_CommandLabel.Content = teban + "HH" + suji + "HH" + dan + "HH";

                string labelName = "SY_GobanGrid" + suji + change_dan(dan);

                SolidColorBrush mySolidColorBrush = new SolidColorBrush(Colors.SteelBlue);
                // アルファ値で、透過するようにしている。255は完全な不透明 0 は完全な透明
                SY_GobanGrid77.Background = mySolidColorBrush;

                Debug.WriteLine(labelName);

                var property = typeof(Label).GetProperty(labelName);
                //var beforeBackground = property.GetValue(labelName);
                //property.SetValue(labelName, mySolidColorBrush);
                //property.SetValue("Background", mySolidColorBrush);

                // SY_CommandLabel.Content = string.Empty;

            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }


        }

        string change_suji(char _suji)
        {
            switch (_suji)
            {
                case '1': return "8";
                case '2': return "7";
                case '3': return "6";
                case '4': return "5";
                case '5': return "4";
                case '6': return "3";
                case '7': return "2";
                case '8': return "1";
                case '9': return "0";
                default: return  "-1";
            }
        }

        string change_dan(char _dan)
        {
            switch (_dan)
            {
                case '一': return "1";
                case '二': return "2";
                case '三': return "3";
                case '四': return "4";
                case '五': return "5";
                case '六': return "6";
                case '七': return "7";
                case '八': return "8";
                case '九': return "9";
                default: return "-1";
            }
        }
    }
}
