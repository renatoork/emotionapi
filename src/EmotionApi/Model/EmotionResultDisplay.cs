using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmotionApi.Model
{
    public class EmotionResultDisplay
    {
        public string EmotionString
        {
            get;
            set;
        }
        public float Score
        {
            get;
            set;
        }

        public int OriginalIndex
        {
            get;
            set;
        }
    }
}
