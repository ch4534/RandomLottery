using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomLottery
{
    /// <summary>
    /// 直播脚本类型，实现了拷贝接口
    /// </summary>
    class LiveScript : ICloneable
    {
        protected int mLevel = 0;    //脚本等级
        protected Int64 mScriptid;   //脚本ID
        protected Int64 mScriptuid;  //脚本UID
        protected string mContent;   //弹幕的内容
        protected string mScriptName;//脚本的名字

        public int LEVEL
        {
            get
            {
                return mLevel;
            }
            
            set
            {
                mLevel = value;
            }
        }

        public Int64 ID
        {
            get
            {
                return mScriptid;
            }

            set
            {
                mScriptid = value;
            }
        }

        public Int64 UID
        {
            get
            {
                return mScriptuid;
            }

            set
            {
                mScriptuid = value;
            }
        }

        public string CONTENT
        {
            get
            {
                return mContent;
            }

            set
            {
                mContent = value;
            }
        }

        public string NAME
        {
            get
            {
                return mScriptName;
            }

            set
            {
                mScriptName = value;
            }
        }

        public override bool Equals(object obj)
        {
            if (this.mScriptuid == ((LiveScript)obj).mScriptuid)
            {
                return true;
            }
            else
            {
                return false;
            }
            //return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.mScriptuid.GetHashCode();
        }

        public override string ToString()
        {
            return mScriptName + "(" + "Level:" + mLevel + ", Scriptid:" + mScriptid + ", Scriptuid:" + mScriptuid + "):" + mContent;
        }

        public object Clone()
        {
            LiveScript liveScript = new LiveScript();
            liveScript.LEVEL   = mLevel;
            liveScript.ID      = mScriptid;
            liveScript.UID     = mScriptuid;
            liveScript.CONTENT = mContent;
            liveScript.NAME    = mScriptName;

            return liveScript;
        }
    }
}
