using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;
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
        private string LV_status = "";

        // 将棋タブ 関連変数
        // 接頭辞s:先手, g:後手
        private string SY_status = "棋譜";
        Label[,] gobanLabel;
        RowDefinition[] dan = new RowDefinition[10]; // 行, 段, 横
        ColumnDefinition[] suji = new ColumnDefinition[10]; // 列, 筋, 縦

        Dictionary<char, short> sKomadai, gKomadai;

        GridLength gridWidth = new GridLength(35); // 1マスの幅
        GridLength gridheight = new GridLength(45);  // 1マスの高さ

        //FontFamily font = new FontFamily("藍原筆文字楷書");
        FontFamily font = new FontFamily("HG正楷書体-PRO");
        
        int fontSize = 28;

        Transform transformTekijin = new RotateTransform(180, 19, 20);
        Transform transformJijin = new RotateTransform(0, 0, 0);

        SolidColorBrush mySolidColorBrushCurrent = new SolidColorBrush(Colors.Azure);
        SolidColorBrush mySolidColorBrushOriginal = new SolidColorBrush(Colors.WhiteSmoke);
        int suji_before=0, dan_before=0;




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
           // PrepareDokuWiki();

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
                case Key.D0 or Key.D1 or Key.D2 or Key.D3 or Key.D4 or // "D0", "D1", ...
                     Key.D5 or Key.D6 or Key.D7 or Key.D8 or Key.D9:
                    tmp = e.Key.ToString().Substring(1); break;
                case Key.F or Key.U or Key.K or Key.E or Key.I or Key.G or
                     Key.O or Key.M or Key.R or Key.Y or Key.T or Key.H:
                    // 入力無しかつ押下がGなら 後手△ を入れる
                    if (SY_CommandLabel.Content.ToString() == string.Empty && e.Key == Key.G)
                    {
                        tmp = "△";
                    }
                    // 何か入ってたらそのまま入れる
                    else if(SY_CommandLabel.Content.ToString() != string.Empty)
                    {
                        tmp = e.Key.ToString();
                    }
                    break;
                case Key.Escape:
                    //do_sashite();
                    // ▲, △で碁盤の向きを変更
                    try
                    {
                        char teban = SY_CommandLabel.Content.ToString()[0];
                        if (teban == '▲' || teban == '△')
                        {
                            make_goban(SY_CommandLabel.Content.ToString()[0]);
                        }
                    }
                    catch (Exception) { }

                    break;
                case Key.Enter:
                    do_sashite(SY_CommandLabel.Content.ToString());
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
            SY_GobanGrid.Children.Clear();
            SY_GobanGrid.ShowGridLines = true;

            sKomadai = new Dictionary<char, short>();
            gKomadai = new Dictionary<char, short>();

            sKomadai.Add('歩', 0); gKomadai.Add('歩', 0);
            sKomadai.Add('桂', 0); gKomadai.Add('桂', 0);
            sKomadai.Add('香', 0); gKomadai.Add('香', 0);
            sKomadai.Add('銀', 0); gKomadai.Add('銀', 0);
            sKomadai.Add('金', 0); gKomadai.Add('金', 0);
            sKomadai.Add('角', 0); gKomadai.Add('角', 0);
            sKomadai.Add('飛', 0); gKomadai.Add('飛', 0);

            // 段, 筋の初期化
            for (int i=0; i<10; i++)
            {
                dan[i] = new RowDefinition
                {
                    Height = gridheight
                };
                SY_GobanGrid.RowDefinitions.Add(dan[i]);

                suji[i] = new ColumnDefinition
                {
                    Width = gridWidth
                };
                SY_GobanGrid.ColumnDefinitions.Add(suji[i]);
            }

            gobanLabel = new Label[11, 11];

            if (_teban == '▲')
            {
                // 先手番の盤の向き
                for (int i = 8; i >= 0; i--)
                {
                    // 筋の目盛
                    gobanLabel[0, i] = new Label
                    {
                        Content = i + 1,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                        FontWeight = FontWeights.Bold,
                        FontSize = fontSize
                    };
                    Grid.SetRow(gobanLabel[0, i], 0);
                    Grid.SetColumn(gobanLabel[0, i], 8 - i);
                    SY_GobanGrid.Children.Add(gobanLabel[0, i]);

                    // 駒エリア
                    for (int j = 8; j >= 0; j--)
                    {
                        gobanLabel[j + 1, i + 1] = new Label
                        {
                            Content = string.Empty,
                            HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                            FontFamily = font,
                            FontWeight = FontWeights.Bold,
                            FontSize = fontSize
                        };
                        Grid.SetRow(gobanLabel[j + 1, i + 1], i+1);
                        Grid.SetColumn(gobanLabel[j + 1, i + 1], 8 - j);
                        SY_GobanGrid.Children.Add(gobanLabel[j + 1, i + 1]);
                    }
                }

                // 段の目盛
                for(int i=0; i<9; i++)
                {
                    gobanLabel[i, 10] = new Label
                    {
                        Content = suji2kanji(i + 1),
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                        FontFamily = font,
                        FontSize = fontSize,
                        FontWeight = FontWeights.Bold
                    };
                    Grid.SetRow(gobanLabel[i, 10], i+1);
                    Grid.SetColumn(gobanLabel[i, 10], 9);
                    SY_GobanGrid.Children.Add(gobanLabel[i, 10]);
                }

                // 敵陣の向きを変更
                for (int i=1; i<=9; i++)
                {
                    gobanLabel[i, 1].RenderTransform =
                        gobanLabel[i, 2].RenderTransform =
                        gobanLabel[i, 3].RenderTransform = transformTekijin;
                   // gobanLabel[i, 1].Foreground =
                   //     gobanLabel[i, 2].Foreground =
                   //     gobanLabel[i, 3].Foreground =
                   //     new SolidColorBrush(Colors.ForestGreen);
                }

                // 王・玉
                gobanLabel[5, 1].Content = "玉";
                gobanLabel[5, 9].Content = "王";


            }
            else if (_teban == '△')
            {
                // 後手番の盤の向き
                for (int i = 0; i <= 8; i++)
                {

                    // 筋の目盛
                    gobanLabel[0, 9-i] = new Label
                    {
                        Content = i + 1,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                        FontSize = fontSize,
                        FontWeight = FontWeights.Bold
                    };
                    Grid.SetRow(gobanLabel[0, 9-i], 0);
                    Grid.SetColumn(gobanLabel[0, 9-i], i);
                    SY_GobanGrid.Children.Add(gobanLabel[0, 9-i]);

                    // 駒エリア
                    for (int j = 8; j >= 0; j--)
                    {
                        gobanLabel[i + 1, j + 1] = new Label
                        {
                            Content = string.Empty,
                            FontFamily = font,
                            FontSize = fontSize,
                            FontWeight = FontWeights.Bold,
                            HorizontalAlignment = System.Windows.HorizontalAlignment.Center
                        };
                        Grid.SetRow(gobanLabel[i + 1, j + 1], 9 - j);
                        Grid.SetColumn(gobanLabel[i + 1, j + 1], i);
                        SY_GobanGrid.Children.Add(gobanLabel[i + 1, j + 1]);
                    }
                }

                // 段の目盛
                for (int i = 0; i < 9; i++)
                {
                    gobanLabel[9 - i, 10] = new Label
                    {
                        Content = suji2kanji(i + 1),
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                        FontFamily = font,
                        FontSize = fontSize,
                        FontWeight = FontWeights.Bold
                    };
                    Grid.SetRow(gobanLabel[9 - i, 10], 9 - i);
                    Grid.SetColumn(gobanLabel[9 - i, 10], 9);
                    SY_GobanGrid.Children.Add(gobanLabel[9 - i, 10]);
                }

                // 敵陣の向きを変更
                for (int i = 1; i <= 9; i++)
                {
                    gobanLabel[i, 7].RenderTransform =
                        gobanLabel[i, 8].RenderTransform =
                        gobanLabel[i, 9].RenderTransform = transformTekijin;
                    //gobanLabel[i, 7].Foreground =
                    //    gobanLabel[i, 8].Foreground =
                    //    gobanLabel[i, 9].Foreground =
                    //    new SolidColorBrush(Colors.ForestGreen);
                }

                // 王・玉
                gobanLabel[5, 9].Content = "玉";
                gobanLabel[5, 1].Content = "王";
            }

            // 駒並べ
            for (int i = 1; i <= 9; i++)
            {
                gobanLabel[i, 3].Content = gobanLabel[i, 7].Content = "歩";
            }

            gobanLabel[1, 1].Content = gobanLabel[9, 1].Content =
                gobanLabel[1, 9].Content = gobanLabel[9, 9].Content = "香";

            gobanLabel[2, 1].Content = gobanLabel[8, 1].Content =
                gobanLabel[2, 9].Content = gobanLabel[8, 9].Content = "桂";

            gobanLabel[3, 1].Content = gobanLabel[7, 1].Content =
                gobanLabel[3, 9].Content = gobanLabel[7, 9].Content = "銀";

            gobanLabel[4, 1].Content = gobanLabel[6, 1].Content =
                gobanLabel[4, 9].Content = gobanLabel[6, 9].Content = "金";

            gobanLabel[2, 2].Content = gobanLabel[8, 8].Content = "角";
            gobanLabel[2, 8].Content = gobanLabel[8, 2].Content = "飛";


        }


        void convert_sashite()
        {
            string tmp = SY_CommandLabel.Content.ToString();
            int length = tmp.Length;
            string kanji = string.Empty;
            
            if(Regex.IsMatch(tmp, @"[1-9]{2,2}")) // 2桁の数字が登場したら、一の位を漢数字に変える
            {

                kanji = suji2kanji(int.Parse(tmp.Substring(length - 1)));
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

        void do_sashite(string _te)
        {

            char teban = _te[0];
            int suji = int.Parse(_te[1].ToString());
            int dan = change_dan(_te[2]);
            char koma = _te[3];
            // SY_CommandLabel.Content = teban + "HH" + suji + "HH" + dan + "HH";

            // 指す場所に駒が既にあるなら、その駒を取る
            if(gobanLabel[suji, dan].Content.ToString() != string.Empty)
            {
                char c = gobanLabel[suji, dan].Content.ToString()[0];
                if (teban == '▲')
                {
                    short si = (short)sKomadai[c];
                    sKomadai[c] = (short)++si;
                    SY_sLabel.Content = "先手 " + GetKomaOnKomadai(sKomadai);
                }
                else if (teban == '△')
                {
                    short si = (short)gKomadai[c];
                    gKomadai[c] = (short)++si;
                    SY_gLabel.Content = "後手" + GetKomaOnKomadai(gKomadai);
                }
            }

            gobanLabel[suji, dan].Content = koma.ToString();
            gobanLabel[suji, dan].Background = mySolidColorBrushCurrent;
            gobanLabel[suji_before, dan_before].Background = mySolidColorBrushOriginal;

            if (teban == '▲')
            {
                gobanLabel[suji, dan].RenderTransform = transformJijin;
            }
            else if (teban == '△')
            {
                gobanLabel[suji, dan].RenderTransform = transformTekijin;
            }

            switch (koma)
            {
                case '歩':
                    if (gobanLabel[suji, dan + 1].Content.ToString() == "歩")
                    {
                        gobanLabel[suji, dan + 1].Content = string.Empty;
                    }
                    else if (gobanLabel[suji, dan - 1].Content.ToString() == "歩")
                    {
                        gobanLabel[suji, dan - 1].Content = string.Empty;
                    }
                    break;

                case '桂':
                    if (gobanLabel[suji - 1, dan + 2].Content.ToString() == "桂")
                    {
                        gobanLabel[suji - 1, dan + 2].Content = string.Empty;
                    }
                    else if (gobanLabel[suji + 1, dan + 2].Content.ToString() == "桂")
                    {
                        gobanLabel[suji + 1, dan + 2].Content = string.Empty;
                    }
                    else if (gobanLabel[suji - 1, dan - 2].Content.ToString() == "桂")
                    {
                        gobanLabel[suji - 1, dan - 2].Content = string.Empty;
                    }
                    else if (gobanLabel[suji + 1, dan - 2].Content.ToString() == "桂")
                    {
                        gobanLabel[suji + 1, dan - 2].Content = string.Empty;
                    }
                    break;

                case '金':
                    if (gobanLabel[suji - 1, dan].Content.ToString() == "金")
                    {
                        gobanLabel[suji - 1, dan].Content = string.Empty;
                    }
                    else if (gobanLabel[suji + 1, dan].Content.ToString() == "金")
                    {
                        gobanLabel[suji + 1, dan].Content = string.Empty;
                    }
                    else if (gobanLabel[suji, dan - 1].Content.ToString() == "金")
                    {
                        gobanLabel[suji, dan - 1].Content = string.Empty;
                    }
                    else if (gobanLabel[suji, dan + 1].Content.ToString() == "金")
                    {
                        gobanLabel[suji, dan + 1].Content = string.Empty;
                    }
                    else if (gobanLabel[suji - 1, dan - 1].Content.ToString() == "金")
                    {
                        gobanLabel[suji - 1, dan - 1].Content = string.Empty;
                    }
                    else if (gobanLabel[suji - 1, dan + 1].Content.ToString() == "金")
                    {
                        gobanLabel[suji - 1, dan + 1].Content = string.Empty;
                    }
                    else if (gobanLabel[suji + 1, dan - 1].Content.ToString() == "金")
                    {
                        gobanLabel[suji + 1, dan - 1].Content = string.Empty;
                    }
                    else if (gobanLabel[suji + 1, dan + 1].Content.ToString() == "金")
                    {
                        gobanLabel[suji + 1, dan + 1].Content = string.Empty;
                    }
                    break;

                case '銀':
                    if (gobanLabel[suji - 1, dan - 1].Content.ToString() == "銀")
                    {
                        gobanLabel[suji - 1, dan - 1].Content = string.Empty;
                    }
                    else if (gobanLabel[suji - 1, dan + 1].Content.ToString() == "銀")
                    {
                        gobanLabel[suji - 1, dan + 1].Content = string.Empty;
                    }
		            else if (gobanLabel[suji + 1, dan - 1].Content.ToString() == "銀")
                    {
                        gobanLabel[suji + 1, dan - 1].Content = string.Empty;
                    }
		            else if (gobanLabel[suji + 1, dan + 1].Content.ToString() == "銀")
                    {
                        gobanLabel[suji + 1, dan + 1].Content = string.Empty;
                    }
		            else if (gobanLabel[suji + 1, dan].Content.ToString() == "銀")
                    {
                        gobanLabel[suji + 1, dan].Content = string.Empty;
                    }
		            else if (gobanLabel[suji - 1, dan].Content.ToString() == "銀")
                    {
                        gobanLabel[suji - 1, dan].Content = string.Empty;
                    }
                    else if (gobanLabel[suji + 1, dan].Content.ToString() == "銀")
                    {
                        gobanLabel[suji + 1, dan].Content = string.Empty;
                    }
                    else if (gobanLabel[suji, dan - 1].Content.ToString() == "銀")
                    {
                        gobanLabel[suji, dan - 1].Content = string.Empty;
                    }
                    else if (gobanLabel[suji, dan + 1].Content.ToString() == "銀")
                    {
                        gobanLabel[suji, dan + 1].Content = string.Empty;
                    }
                    break;
                case '角':
                    break;
                case '飛':
                    break;
                case '香':
                    break;
                case '王' or '玉':
                    break;
                case '馬':
                    break;
                case '龍':
                    break;
                case 'と':
                    break;
//                case '成桂': // TODO 2文字以上の駒
//                    break;
            }

            suji_before = suji;
            dan_before = dan;
            SY_CommandLabel.Content = string.Empty;

        }

        string GetKomaOnKomadai(Dictionary<char, short> komadai)
        {
            return "歩" + komadai['歩'].ToString() +
                   "香" + komadai['香'].ToString() +
                   "桂" + komadai['桂'].ToString() +
                   "銀" + komadai['銀'].ToString() +
                   "金" + komadai['金'].ToString() +
                   "角" + komadai['角'].ToString() +
                   "飛" + komadai['飛'].ToString();
        }


        string suji2kanji(int _suji)
        {
            switch (_suji)
            {
                case 1:  return "一";
                case 2:  return "二";
                case 3:  return "三";
                case 4:  return "四";
                case 5:  return "五";
                case 6:  return "六";
                case 7:  return "七";
                case 8:  return "八";
                case 9:  return "九";
                default: return string.Empty;
            }
        }

        int change_dan(char _dan)
        {
            switch (_dan)
            {
                case '一': return 1;
                case '二': return 2;
                case '三': return 3;
                case '四': return 4;
                case '五': return 5;
                case '六': return 6;
                case '七': return 7;
                case '八': return 8;
                case '九': return 9;
                default: return  -1;
            }
        }
    }
}
