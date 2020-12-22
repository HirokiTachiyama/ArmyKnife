using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ArmyKnife
{
    public partial class MainWindow
    {

        void make_goban(char _teban)
        {
            // 将棋盤を綺麗にする
            SY_GobanGrid.Children.Clear();
            SY_GobanGrid.ShowGridLines = true;

            SY_KifuTextBox.Text = string.Empty;

            suji_before = dan_before = 5; // 5五にしておけばとりあえず問題無さそう

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
            for (int i = 0; i < 10; i++)
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
                        Grid.SetRow(gobanLabel[j + 1, i + 1], i + 1);
                        Grid.SetColumn(gobanLabel[j + 1, i + 1], 8 - j);
                        SY_GobanGrid.Children.Add(gobanLabel[j + 1, i + 1]);
                    }
                }

                // 段の目盛
                for (int i = 0; i < 9; i++)
                {
                    gobanLabel[i, 10] = new Label
                    {
                        Content = suji2kanji(i + 1),
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                        FontFamily = font,
                        FontSize = fontSize,
                        FontWeight = FontWeights.Bold
                    };
                    Grid.SetRow(gobanLabel[i, 10], i + 1);
                    Grid.SetColumn(gobanLabel[i, 10], 9);
                    SY_GobanGrid.Children.Add(gobanLabel[i, 10]);
                }

                // 敵陣の向きを変更
                for (int i = 1; i <= 9; i++)
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
                    gobanLabel[0, 9 - i] = new Label
                    {
                        Content = i + 1,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                        FontSize = fontSize,
                        FontWeight = FontWeights.Bold
                    };
                    Grid.SetRow(gobanLabel[0, 9 - i], 0);
                    Grid.SetColumn(gobanLabel[0, 9 - i], i);
                    SY_GobanGrid.Children.Add(gobanLabel[0, 9 - i]);

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

        private void SY_TabItem_KeyDown(object sender, KeyEventArgs e)
        {
            // 歩FU 香KY 桂KE 銀GI 金KI 角KA 飛HI 王OU と金TO 成香NY 成桂NK 成銀NG 馬UM 龍RY
            string tmp = string.Empty;
            switch (e.Key)
            {
                case Key.S: // 先手番
                    tmp = "▲";
                    break;
                case Key.D0 or Key.D1 or Key.D2 or Key.D3 or Key.D4 or // "D0", "D1", ...
                     Key.D5 or Key.D6 or Key.D7 or Key.D8 or Key.D9:
                    tmp = e.Key.ToString().Substring(1);
                    break;
                case Key.F or Key.U or Key.K or Key.E or Key.I or Key.G or
                     Key.O or Key.M or Key.R or Key.Y or Key.T or Key.H or Key.A:
                    // 入力無しかつ押下がGなら 後手△ を入れる
                    if (SY_CommandLabel.Content.ToString() == string.Empty && e.Key == Key.G)
                    {
                        tmp = "△";
                    }
                    // 何か入ってたらそのまま入れる
                    else if (SY_CommandLabel.Content.ToString() != string.Empty)
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




        void convert_sashite()
        {
            string tmp = SY_CommandLabel.Content.ToString();
            int length = tmp.Length;
            string kanji = string.Empty;

            if (Regex.IsMatch(tmp, @"[1-9]{2,2}")) // 2桁の数字が登場したら、一の位を漢数字に変える
            {

                kanji = suji2kanji(int.Parse(tmp.Substring(length - 1)));
                tmp = tmp.Substring(0, length - 1) + kanji;
                SY_CommandLabel.Content = tmp;
            }
            else if (Regex.IsMatch(tmp, @"[A-Y]{2,2}"))
            { // 2文字のアルファベットが登場したら漢字に変える
                switch (tmp.Substring(length - 2))
                {
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
            if (gobanLabel[suji, dan].Content.ToString() != string.Empty)
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


            // TODO 自分の駒は取れないようにすること。
            // 開発中は取れた方がでばぐしやすいので、そのうちでよい。
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
                    // TODO 先手番の時に香車の動きがおかしい
                    // 敵側では試していないので、そちらも要確認
                    if (teban == '▲')
                    {
                        for (int i = 1; i <= 9; i++)
                        {
                            int currentDan = dan - i;
                            if (1 <= currentDan && currentDan <= 9)
                            {
                                if (gobanLabel[suji, currentDan].Content.ToString() == "香")
                                {
                                    gobanLabel[suji, currentDan].Content = string.Empty;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }

                    }
                    else if (teban == '△')
                    {
                        for (int i = 1; i <= 9; i++)
                        {
                            int currentDan = dan + i;
                            if (1 <= currentDan && currentDan <= 9)
                            {
                                if (gobanLabel[suji, currentDan].Content.ToString() == "香")
                                {
                                    gobanLabel[suji, currentDan].Content = string.Empty;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
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

            // 駒音を響かせる
            mediaPlayer.controls.play();


            // 打った座標を記録。次の手を打った際に背景を元に戻す際に利用
            suji_before = (short)suji;
            dan_before = (short)dan;

            // 手数を増やしておく
            tesu++;

            // 棋譜エリアに追記
            SY_KifuTextBox.Text = $"{SY_KifuTextBox.Text} \n{tesu} {SY_CommandLabel.Content}";
            // コマンドエリアを空に
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
                case 1: return "一";
                case 2: return "二";
                case 3: return "三";
                case 4: return "四";
                case 5: return "五";
                case 6: return "六";
                case 7: return "七";
                case 8: return "八";
                case 9: return "九";
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
                default: return -1;
            }
        }


    }
}
