using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomLottery
{
    /// <summary>
    /// 中奖脚本存储类
    /// </summary>
    class LotteryScript : ICloneable
    {
        protected LiveScript mLiveScript = new LiveScript();
        protected string mPrizeStr;

        public LiveScript SCRIPT
        {
            get
            {
                return mLiveScript;
            }

            set
            {
                mLiveScript = value;
            }
        }

        public string NAME
        {
            get
            {
                return mLiveScript.NAME;
            }
        }

        public string PRIZE
        {
            get
            {
                return mPrizeStr;
            }

            set
            {
                mPrizeStr = value;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is LotteryScript)
            {
                return mLiveScript.Equals(((LotteryScript)obj).mLiveScript);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return mLiveScript.GetHashCode();
        }

        public object Clone()
        {
            LotteryScript clone = new LotteryScript();
            clone.mLiveScript = (LiveScript)this.mLiveScript.Clone();
            clone.mPrizeStr = (string)this.mPrizeStr.Clone();
            //throw new NotImplementedException();
            return clone;
        }
    }
}
