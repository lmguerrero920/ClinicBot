using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotClinic.Infrastructure.LUIS
{
    public class LuisService : ILuisService
    {
        public LuisRecognizer _luisRecognizer { get; set; }



        public LuisService(IConfiguration configuration)
        {
            var luisApplication = new LuisApplication(
                configuration["LuisAppId"],
                configuration["LuisApikey"],
                configuration["LuisHostName"]);

            var recognizeroptions = new LuisRecognizerOptionsV3(luisApplication)
            {
                PredictionOptions = new Microsoft.Bot.Builder.AI.LuisV3.LuisPredictionOptions()
                {
                     IncludeInstanceData = true
                }
            };
            _luisRecognizer = new LuisRecognizer(recognizeroptions);


        }
    }


}
