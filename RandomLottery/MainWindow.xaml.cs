using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace RandomLottery
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected bool mCount = false;   ///判定当前是否处于统计状态
        protected int mLotteryManNum = 0;///设置当前最大的抽奖人数
        //private LotteryScript mSelectItem;
        protected ObservableCollection<string> mprizeItems;       ///奖品的模型数据源
        private ObservableCollection<LotteryScript> mHitPrizeList;///中奖名单的模型数据源
        private ObservableCollection<LiveScript> mCountPrizeList; ///统计所有参与抽奖的模型数据源
        //protected List<LiveScript> mlistScript;
        //protected List<LotteryScript> mlistlotteryScript;
        public static LiveType mLiveType = LiveType.ZhanQi;///表明当前抽奖模式
        private Thread mLotteryThread;               ///表示抽奖线程

        public enum LiveType
        {
            ZhanQi   = 1,
            Bilibili = 2
        }

        public MainWindow()
        {
            InitializeComponent();

            if (this.init())
            {
                this.initView();
                this.initEvent();
            }
        }

        /// <summary>
        /// 初始化截包接口
        /// </summary>
        /// <returns>
        /// 返回true初始化成功，返回false初始化失败
        /// </returns>
        protected bool init()
        {
            return CppInterface.InitDll();
        }

        /// <summary>
        /// 初始化对话框
        /// </summary>
        protected void initView()
        {
            string[] netdevice = CppInterface.LoadNetDevice();
            NetDevice.ItemsSource = netdevice;

            if (File.Exists("config.ini"))
            {
                StreamReader sr = new StreamReader("config.ini", Encoding.Default);
                String selIndex = sr.ReadLine();
                if (selIndex != null)
                {
                    int index = int.Parse(selIndex);
                    if (index >= 0 && index < netdevice.Length)
                    {
                        NetDevice.SelectedIndex = index;
                    }
                }
                sr.Close();
            }

            List<LevelLimit> levelist = new List<LevelLimit>();

            if (mLiveType == LiveType.ZhanQi)
            {
                for (int i = 0; i < 25; i++)
                {
                    LevelLimit levellimit = new LevelLimit();
                    levellimit.Leveltext = i.ToString();
                    levellimit.Levelgif = "res/image/" + i.ToString() + ".gif";

                    levelist.Add(levellimit);
                }
            }
            else
            {
                for (int i = 0; i < 21; i++)
                {
                    LevelLimit levellimit = new LevelLimit();
                    levellimit.Leveltext = i.ToString();
                    levellimit.Levelgif = "res/image/medal_" + i.ToString() + ".gif";

                    levelist.Add(levellimit);
                }
            }

            ComboBoxLevelLimit.ItemsSource = levelist;

            mprizeItems = new ObservableCollection<string>(CppInterface.LoadLottteryIni());
            //foreach (string prize in mprizeItems)
            //{
            //    prizeSet.Items.Add(prize);
            //}
            prizeSet.ItemsSource = mprizeItems;
            prizeSet.SelectedIndex = 0;

            mHitPrizeList = new ObservableCollection<LotteryScript>();
            HitPrizeList.ItemsSource = mHitPrizeList;

            mCountPrizeList = new ObservableCollection<LiveScript>();
            mLotteryThread = new Thread(UpdateLotteryManList);

            version.Text += System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        /// <summary>
        /// 初始化事件，将所有需要在开头进行事件初始化的工作全在这个函数中执行
        /// </summary>
        private void initEvent()
        {
            this.Closed += MainWindow_Closed;
        }

        /// <summary>
        /// 窗口将要关闭时响应的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Closed(object sender, EventArgs e)
        {
            String selIndex = NetDevice.SelectedIndex.ToString();
            FileStream fs = new FileStream("config.ini", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            fs.SetLength(0);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(selIndex);
            sw.Flush();
            sw.Close();
            fs.Close();
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 窗口关闭的时候，将奖品信息存储在本地
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string[] lotteryini = mprizeItems.ToArray();

            CppInterface.SaveLotteryIniInfo(lotteryini, lotteryini.Length);
        }

        /// <summary>
        /// 开始统计人数按钮响应函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CountHandler(object sender, RoutedEventArgs e)
        {
            if (mCount == false)
            {
                int index = NetDevice.SelectedIndex;
                if (CppInterface.OpenNetDevice(index))
                {
                    if (mLotteryThread.IsAlive)
                    {
                        mLotteryThread.Join();
                    }

                    mCountPrizeList.Clear();
                    CountBtn.Content = "停止统计";
                    mCount = true;
                    mLotteryThread.Start();
                }
            }
            else
            {
                CountBtn.Content = "开始统计";
                mCount = false;
            }
        }

        /// <summary>
        /// 人数编辑框失去焦点响应，判定当前输入的文字是否符合数字要求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckTextNum(object sender, RoutedEventArgs e)
        {
            string text = LotteryManNum.Text;

            if (text.Length > 0)
            {
                int a = 0;

                if (int.TryParse(text, out a))
                {
                    mLotteryManNum = a;
                }
                else
                {
                    MessageBox.Show("请正确输入抽奖人数");
                    LotteryManNum.Text = "";
                }
            }
        }

        /// <summary>
        /// 增加奖品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddLottery(object sender, RoutedEventArgs e)
        {
            string prize = prizeSet.Text;
            if (prize.Length > 0)
            {
                mprizeItems.Add(prize);
                //prizeSet.Items.Add(prize);
            }
        }

        /// <summary>
        /// 减少奖品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubLottery(object sender, RoutedEventArgs e)
        {
            string prize = prizeSet.Text;
            if (prize.Length > 0)
            {
                mprizeItems.Remove(prize);
                //prizeSet.Items.Remove(prize);
            }
        }

        /// <summary>
        /// 解析战旗弹幕
        /// </summary>
        /// <param name="content"></param>
        //protected void ParseZqContent(string content)
        //{
        //    int nindex, nIndex;
        //    if ((nindex = content.IndexOf('{')) != -1 && (nIndex = content.IndexOf('}')) != -1)
        //    {
        //        string substr = content.Substring(nindex, nIndex - nindex + 1);

        //        JsonReader reader = new JsonTextReader(new StringReader(substr));


        //    }
        //}

        /// <summary>
        /// 抽奖线程
        /// </summary>
        private void UpdateLotteryManList()
        {
            string level;
            string scriptId;
            string scriptUid;
            string content;
            string scriptName;

            Action update = delegate
            {
                info.Text = "参与抽奖的人数：" + mCountPrizeList.Count;
            };

            while (mCount)
            {
                CppInterface.LoadScriptMsg((int)mLiveType, out level, out scriptId, out scriptUid, out content, out scriptName);

                if (level != null && level.Length > 0)
                {
                    long uid = long.Parse(scriptUid.ToString());
                    bool bFind = mCountPrizeList.Any<LiveScript>(P => P.UID == uid);
                    if (bFind == false)
                    {
                        LiveScript livescript = new LiveScript();
                        livescript.LEVEL      = int.Parse(level.ToString());
                        livescript.ID         = long.Parse(scriptId.ToString());
                        livescript.UID        = long.Parse(scriptUid.ToString());
                        livescript.CONTENT    = content.ToString();
                        livescript.NAME       = scriptName.ToString();

                        mCountPrizeList.Add(livescript);
                        this.Dispatcher.BeginInvoke(update);
                    }
                }
                level = null;
            }
            CppInterface.CloseNetDevice();
        }

        /// <summary>
        /// 抽奖按钮的响应函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LotteryHandle(object sender, RoutedEventArgs e)
        {
            if (prizeSet.SelectedIndex == -1)
            {
                MessageBox.Show("请选择或者添加奖品信息", "随机抽奖机");
                return;
            }

            int listCount = mCountPrizeList.Count;
            int levelLitmit = ComboBoxLevelLimit.SelectedIndex == -1 ? 0 : ComboBoxLevelLimit.SelectedIndex;

            if (listCount > 0 && mLotteryManNum > mHitPrizeList.Count)
            {
                Random random = new Random();

                for (int i = 0; i < listCount; i++)
                {
                    int index = random.Next(listCount);
                    bool bFind = mHitPrizeList.Any<LotteryScript>(P => P.SCRIPT.UID == mCountPrizeList[index].UID);

                    if (bFind == false && mCountPrizeList[index].LEVEL >= levelLitmit)
                    {
                        LotteryScript lotteryscript = new LotteryScript();
                        lotteryscript.SCRIPT = (LiveScript)mCountPrizeList[index].Clone();
                        if (prizeSet.SelectedIndex != -1)
                        {
                            lotteryscript.PRIZE = prizeSet.SelectedItem.ToString();
                        }
                        else
                        {
                            lotteryscript.PRIZE = "";
                        }
                        
                        mHitPrizeList.Add(lotteryscript);

                        break;
                    }
                }
            }
        }

        private void item_select(object sender, SelectionChangedEventArgs e)
        {
            
        }

        /// <summary>
        /// 列表控件删除按钮响应函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void list_key_up(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.IsKeyUp(Key.Delete))
            {
                // MessageBox.Show("删除按键弹起");
                LotteryScript SelectItem = (LotteryScript)(HitPrizeList.SelectedItem as LotteryScript).Clone(); ;
                mHitPrizeList.Remove(SelectItem);

                //for (int i = mHitPrizeList.Count - 1; i >= 0; i--)
                //{
                //    var item = mHitPrizeList[i];
                //    if (item.Equals(SelectItem))
                //    {
                //        mHitPrizeList.RemoveAt(i);
                //    }
                //}
            }
        }
    }
}
