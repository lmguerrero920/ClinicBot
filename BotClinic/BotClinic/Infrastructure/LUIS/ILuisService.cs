using Microsoft.Bot.Builder.AI.Luis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotClinic.Infrastructure.LUIS
{
    interface ILuisService
    {
        LuisRecognizer _luisRecognizer {get; set; }

}
}
