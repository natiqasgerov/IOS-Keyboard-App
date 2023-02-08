using Dapper;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Media;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
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
using T9Project.Models;

namespace T9Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window,IDatabaseService
    {
        public string LastItem { get; set; } = String.Empty;
        private DispatcherTimer timer { get; set; }
        public bool CapsTemp { get; set; } = false;
        public List<string> WordsCollection { get; set; }
        public bool Temp { get; set; } = false;
        public MainWindow()
        {
            InitializeComponent();
            
            timer = new DispatcherTimer(); 
            timer.Interval = new TimeSpan(0, 0, 0, 0,100);
            timer.Tick += new EventHandler(RefreshMainTime);
            timer.Start();
            Temp = true;           
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            var elem = e.Key;
            if (elem == Key.Capital)
            {
                PlayPrivateSound(DeviceSounds.Space);
                caps_Click(sender, e);
            }
            

        }
        static string getAbbreviatedName(int month)
        {
            DateTime date = new DateTime(2020, month, 1);

            return date.ToString("MMM");
        }
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void RefreshMainTime(object sender, EventArgs e)
        {
            var item = DateTime.Now;
            clockMain.Text = item.ToShortTimeString();
            dateMain.Text = item.DayOfWeek.ToString() + "," + item.Day.ToString() + " " + getAbbreviatedName(item.Month);
        }
        private void RefreshWindowTime(object sender,EventArgs e)
        {
            var item = DateTime.Now;      
            time.Text = item.ToShortTimeString();
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var list = message.Text.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToList();

            PressedButton();

            if (list.Count != 0)
            {
                LastItem = list[list.Count - 1];
                SearchText(LastItem);
            }
            else
            {
                w1.Text = String.Empty;
                w2.Text = String.Empty;
                w3.Text = String.Empty; 
            }
        }
        private void CursorRefresh()
        {
            message.Focusable = true;
            message.Focus();
            message.Select(message.Text.Length, 0);
        }
        public static string ReplaceLastOccurrence(string Source, string Find, string Replace)
        {
            int place = Source.LastIndexOf(Find);

            if (place == -1)
                return Source;

            return Source.Remove(place, Find.Length).Insert(place, Replace);
        }
        public async Task SearchText(string value)
        {
            WordsCollection = new List<string>();

            WordsCollection = Database.db.QueryAsync<string>("SELECT Text FROM Words WHERE Text like @val ORDER BY Count DESC LIMIT 3;",
                new
                {
                    @val = value + "%",
                }).Result.ToList();

            await Task.Delay(100);

            if(WordsCollection.Count == 0)
            {
                w1.Text = String.Empty;
                w2.Text = LastItem;
                w3.Text = String.Empty;
            }

            if(WordsCollection.Count != 0)
            {
                switch (WordsCollection.Count)
                    {
                    case 3:
                        w1.Text = WordsCollection[0];
                        w2.Text = WordsCollection[1];
                        w3.Text = WordsCollection[2];
                        if (WordsCollection[0].Length > 11)
                        {
                            w1.FontSize = 14;
                            w1.Padding = new Thickness(7);
                        }
                        else if (WordsCollection[1].Length > 11)
                        {
                            w2.FontSize = 14;
                            w2.Padding = new Thickness(7);
                        }
                        else if (WordsCollection[2].Length > 11)
                        {
                            w3.FontSize = 14;
                            w3.Padding = new Thickness(7);
                        }
                        else
                        {
                            w1.FontSize = 16;
                            w2.FontSize = 16;
                            w3.FontSize = 16;
                            w1.Padding = new Thickness(5);
                            w2.Padding = new Thickness(5);
                            w3.Padding = new Thickness(5);
                        };
                        break;
                    case 2:
                        w1.Text = WordsCollection[0];
                        w2.Text = WordsCollection[1];
                        w3.Text = String.Empty;
                        if (WordsCollection[0].Length > 11)
                        {
                            w1.FontSize = 14;
                            w1.Padding = new Thickness(7);
                        }
                        else if (WordsCollection[1].Length > 11)
                        {
                            w2.FontSize = 14;
                            w2.Padding = new Thickness(7);
                        }                     
                        else
                        {
                            w1.FontSize = 16;
                            w2.FontSize = 16;      
                            w1.Padding = new Thickness(5);
                            w2.Padding = new Thickness(5);                         
                        }
                        break;
                    case 1:
                        w1.Text = WordsCollection[0];
                        w2.Text = String.Empty;
                        w3.Text = String.Empty;
                        if (WordsCollection[0].Length > 11)
                        {
                            w1.FontSize = 14;
                            w1.Padding = new Thickness(7);
                        }
                        else
                        {
                            w1.FontSize = 16;
                            w1.Padding = new Thickness(5);
                            
                        }
                        break;
                    case 0:
                        w1.Text = String.Empty;
                        w2.Text = String.Empty;
                        w3.Text = String.Empty;
                        break;
                    default:                           
                        break;
                    }
            }
        }        
        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            Border br = (Border)sender;

            switch (br.Name)
            {
                case "border1": br.Background = new SolidColorBrush(Colors.Transparent); break;
                case "border2": br.Background = new SolidColorBrush(Colors.Transparent); break;
                case "border3": br.Background = new SolidColorBrush(Colors.Transparent); break;
                default:
                    break;
            }

        }
        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            Border br = (Border)sender;
            
            switch (br.Name)
            {
                case "border1": br.Background = new SolidColorBrush(Colors.White); break;
                case "border2": br.Background = new SolidColorBrush(Colors.White); break;
                case "border3": br.Background = new SolidColorBrush(Colors.White); break;
                default:
                    break;
            }
        }
        private void TextBlock_MouseDown(object sender, MouseEventArgs e)
        {
            TextBlock textBlock = (TextBlock)sender;
            PlayPrivateSound(DeviceSounds.Standart);
            var list = message.Text.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToList();

            var text = message.Text;
            if (textBlock.Text != String.Empty)
            {
                if(message.Text.EndsWith(' '))
                {
                    message.Text += textBlock.Text;
                    message.Select(message.Text.Length, 0);
                }
                else
                {
                    var newText = ReplaceLastOccurrence(text, LastItem, textBlock.Text);
                    message.Text = newText;
                    message.Select(message.Text.Length, 0);
                }


                if (w1.Text == String.Empty && w2.Text != String.Empty && w3.Text == String.Empty)
                {
                    Database.db.Execute("INSERT INTO Words(Text) VALUES(@text)", new
                    {
                        @text = w2.Text
                    });

                }
                else
                {
                    var textCount = Database.db.Query<int>("SELECT Count FROM Words WHERE Text = @text", new
                    {
                        @text = textBlock.Text
                    }).FirstOrDefault();

                    textCount++;

                    Database.db.Execute("UPDATE Words SET Count = @c WHERE Text = @text", new
                    {
                        @c = textCount,
                        @text = textBlock.Text
                    });
                }
            }





        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            message.Text += button.Content;
            CursorRefresh();
            PlayPrivateSound(DeviceSounds.Standart);
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var text = message.Text;
            if(text != String.Empty)
                message.Text = text.Remove(text.Length - 1, 1);
            CursorRefresh();
            PlayPrivateSound(DeviceSounds.Delete);
        }
        private void caps_Click(object sender, RoutedEventArgs e)
        {
            
            List<StackPanel> panels = new List<StackPanel>() { firstPanel,secondPanel,thirdPanel};

            foreach (var item in panels)
            {
                foreach (var child in item.Children)
                {
                    Button btn = (Button)child;

                    if (btn.Content.GetType().ToString() == "System.String")
                    {
                        if (CapsTemp.Equals(false))
                            btn.Content = btn.Content?.ToString()?.ToUpper();
                        else
                            btn.Content = btn.Content?.ToString()?.ToLower();
                    }                   
                }
            }

            if (CapsTemp.Equals(false))
                CapsTemp = true;
            else
                CapsTemp = false;

            CursorRefresh();
            PlayPrivateSound(DeviceSounds.Standart);
        }
        private void return_btn_Click(object sender, RoutedEventArgs e)
        {
            message.Text += "\n";
            CursorRefresh();
            PlayPrivateSound(DeviceSounds.Return);
        }
        private void space_btn_Click(object sender, RoutedEventArgs e)
        {
            message.Text += " ";
            CursorRefresh();
            PlayPrivateSound(DeviceSounds.Space);
        }
        private void btn_123_Click(object sender, RoutedEventArgs e)
        {
            if(btn_123.Content.ToString() == "123")
            {
                firstPanel.Visibility = Visibility.Hidden;
                secondPanel.Visibility = Visibility.Hidden;
                thirdPanel.Visibility = Visibility.Hidden;
                numberFirstPanel.Visibility = Visibility.Visible;
                numberSecondPanel.Visibility = Visibility.Visible;
                numberThirdPanel.Visibility = Visibility.Visible;
                btn_123.Content = "ABC";
            }
            else if (btn_123.Content.ToString() == "ABC")
            {
                numberFirstPanel.Visibility = Visibility.Hidden;
                numberSecondPanel.Visibility = Visibility.Hidden;
                numberThirdPanel.Visibility = Visibility.Hidden;
                symbolFirstPanel.Visibility = Visibility.Hidden;
                symbolSecondPanel.Visibility = Visibility.Hidden;
                symbolThirdPanel.Visibility = Visibility.Hidden;
                firstPanel.Visibility = Visibility.Visible;
                secondPanel.Visibility = Visibility.Visible;
                thirdPanel.Visibility = Visibility.Visible;
                btn_123.Content = "123";
            }
            CursorRefresh();
            PlayPrivateSound(DeviceSounds.Standart);
        }
        private void btn_symbol_Click(object sender, RoutedEventArgs e)
        {        
            numberFirstPanel.Visibility = Visibility.Hidden;
            numberSecondPanel.Visibility = Visibility.Hidden;
            numberThirdPanel.Visibility = Visibility.Hidden;
            symbolFirstPanel.Visibility = Visibility.Visible;
            symbolSecondPanel.Visibility = Visibility.Visible;
            symbolThirdPanel.Visibility = Visibility.Visible;
            CursorRefresh();
            PlayPrivateSound(DeviceSounds.Standart);
        }
        private void endSymbol_btn_Click(object sender, RoutedEventArgs e)
        {
            symbolFirstPanel.Visibility = Visibility.Hidden;
            symbolSecondPanel.Visibility = Visibility.Hidden;
            symbolThirdPanel.Visibility = Visibility.Hidden;
            numberFirstPanel.Visibility = Visibility.Visible;
            numberSecondPanel.Visibility = Visibility.Visible;
            numberThirdPanel.Visibility = Visibility.Visible;
            CursorRefresh();
            PlayPrivateSound(DeviceSounds.Standart);
        }
        private void back_MouseEnter(object sender, MouseEventArgs e)
        {
            backBorder.Background = new SolidColorBrush(Colors.LightGray);
            
        }
        private void back_MouseLeave(object sender, MouseEventArgs e)
        {
            backBorder.Background = new SolidColorBrush(Colors.Transparent);
        }
        private void TextBlock_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            PlayPrivateSound(DeviceSounds.UnLock);
            Thread.Sleep(150);
            this.Close();
        }
        private void mainGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            PlayPrivateSound(DeviceSounds.UnLock);
            timer.Stop();
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Tick += new EventHandler(RefreshWindowTime);
            timer.Start();
            time.Visibility = Visibility.Visible;
            clockMain.Visibility = Visibility.Hidden;
            dateMain.Visibility = Visibility.Hidden;
            mainGrid.Visibility = Visibility.Hidden;
            noteGrid.Visibility = Visibility.Visible;
            backStackpanel.Visibility = Visibility.Visible;
            doneStackpanel.Visibility = Visibility.Visible;
            keyboardBorder.Visibility = Visibility.Visible;
            CursorRefresh();
        }
        private void PlayPrivateSound(string path)
        {
            SoundPlayer sound = new SoundPlayer(path);
            sound.Play();
        }
        private void PressedButton()
        {
            if (Keyboard.IsKeyDown(Key.Space))
                PlayPrivateSound(DeviceSounds.Space);
            else if (Keyboard.IsKeyDown(Key.Back))
                PlayPrivateSound(DeviceSounds.Delete);
            else if (Keyboard.IsKeyDown(Key.Return))
                PlayPrivateSound(DeviceSounds.Return);      
            else if(Temp == true)
                PlayPrivateSound(DeviceSounds.Standart);
        }
    }


    
}
