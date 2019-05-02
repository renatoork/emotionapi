using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmotionApi.Model
{
    public class Emotion
    {
        public Emotion()
        {

        }

        public Rectangle FaceRectangle { get; set; }
        public Score Scores { get; set; }
    }
}
