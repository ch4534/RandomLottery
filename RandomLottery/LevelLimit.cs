using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomLottery
{
    /// <summary>
    /// 等级数组缓存类
    /// </summary>
    public class LevelLimit
    {
        protected String mLevelPath;//图片的地址
        protected String mLevelText;//等级字符串

        public String Levelgif
        {
            get { return mLevelPath; }
            set { mLevelPath = value; }
        }

        public String Leveltext
        {
            get { return mLevelText; }
            set { mLevelText = value; }
        }
    }
}
