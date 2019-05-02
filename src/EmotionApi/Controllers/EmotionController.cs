using EmotionApi.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace EmotionApi.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/emotion")]
    [Produces("application/json")]
    public class EmotionController : Controller
    {
        private IConfiguration _conf;
        public EmotionController(IConfiguration conf)
        {
            _conf = conf;
        }


        [HttpPost]
        public async Task<IActionResult> PostAsync(int id, IFormFile file)
        {
            if (file.Length > 0)
            {
                var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                var bytes = stream.ToArray();

                HttpClient client = new HttpClient();
                HttpContent content = new ByteArrayContent(bytes);
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _conf["OcpApim:SubscriptionKey"]);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                var res = client.PostAsync(new Uri(@"https://westus.api.cognitive.microsoft.com/emotion/v1.0/recognize", UriKind.Absolute), content).Result;
                string jsonResult = res.Content.ReadAsStringAsync().Result.ToString();
                if (res.IsSuccessStatusCode)
                {
                    Emotion[] emotion = JsonConvert.DeserializeObject<Emotion[]>(jsonResult);
                    var result = ListEmotionResult(emotion[0]);
                    if (result != null)
                    {
                        return Ok(result);
                    }
                    else
                    {
                        return BadRequest("Erro Interno...");
                    }
                }
                else
                {
                    return BadRequest(res.Content.ReadAsStringAsync().Result);
                }
            }
            else
            {
                return NoContent();
            }
        }
        public Dictionary<string, string> ListEmotionResult(Emotion emotion)
        {
            if (emotion != null)
            {
                EmotionResultDisplay[] resultDisplay = new EmotionResultDisplay[8];
                resultDisplay[0] = new EmotionResultDisplay { EmotionString = "Bravo", Score = emotion.Scores.Anger * 100 };
                resultDisplay[1] = new EmotionResultDisplay { EmotionString = "Desprezo", Score = emotion.Scores.Contempt * 100 };
                resultDisplay[2] = new EmotionResultDisplay { EmotionString = "Nojo", Score = emotion.Scores.Disgust * 100 };
                resultDisplay[3] = new EmotionResultDisplay { EmotionString = "Medo", Score = emotion.Scores.Fear * 100 };
                resultDisplay[4] = new EmotionResultDisplay { EmotionString = "Felicidade", Score = emotion.Scores.Happiness * 100 };
                resultDisplay[5] = new EmotionResultDisplay { EmotionString = "Neutro", Score = emotion.Scores.Neutral * 100 };
                resultDisplay[6] = new EmotionResultDisplay { EmotionString = "Tristeza", Score = emotion.Scores.Sadness * 100 };
                resultDisplay[7] = new EmotionResultDisplay { EmotionString = "Surpresa", Score = emotion.Scores.Surprise * 100 };

                Array.Sort(resultDisplay, CompareDisplayResults);

                Dictionary<string, string> res = new Dictionary<string, string>();
                res.Add(resultDisplay[0].EmotionString, resultDisplay[0].Score.ToString("##0.000") + "%");
                /*
                for (int j = 0; j < 3; j++)
                {
                    if (!resultDisplay[j].Score.ToString("##0.000").Equals("0,000") && !resultDisplay[j].Score.ToString("##0.000").Equals("0.000"))
                    {
                        res.Add(resultDisplay[j].EmotionString, resultDisplay[j].Score.ToString("##0.000") + "%");
                    }
                }
                */
                return res;

            }
            return null;
        }

        private int CompareDisplayResults(EmotionResultDisplay result1, EmotionResultDisplay result2)
        {
            return ((result1.Score == result2.Score) ? 0 : ((result1.Score < result2.Score) ? 1 : -1));
        }

    }
}
