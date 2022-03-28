using BotClinic.Common.Cards;
using BotClinic.Infrastructure.LUIS;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BotClinic.Dialogs
{
    public class RootDialog : ComponentDialog
    {
        private readonly ILuisService _luisService;

        public RootDialog(ILuisService luisService)
        {
            _luisService = luisService;

            var waterfallsteps = new WaterfallStep[]
            {

             InitialProcess,
             FinalProcess
            };
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallsteps));
            InitialDialogId = nameof(WaterfallDialog);


        }

      
        private async Task<DialogTurnResult> ManageIntentions(WaterfallStepContext stepContext, Microsoft.Bot.Builder.RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            var topIntent = luisResult.GetTopScoringIntent();
            switch (topIntent.intent)
            {
                case "Saludar":
                    await IntentSaludar(stepContext, luisResult, cancellationToken);
                    break;
                case "Agradecer":
                    await IntentAgradecer(stepContext, luisResult, cancellationToken);
                    break;
                case "Despedir":
                    await IntentDespedir(stepContext, luisResult, cancellationToken);
                    break;
                case "verOpciones":
                    await IntentVerOpciones(stepContext, luisResult, cancellationToken);
                    break;
                case "verCentroContacto":
                    await IntentVerCentroContacto(stepContext, luisResult, cancellationToken);
                    break;
                case "Calificar":
                    return await IntentCalificar(stepContext, luisResult, cancellationToken);
                case "None":
                    await IntentNone(stepContext, luisResult, cancellationToken);
                    break;
                case "CrearCita":
                    return await IntentCrearCita(stepContext, luisResult, cancellationToken);
                case "VerCita":
                    await IntentVerCita(stepContext, luisResult, cancellationToken);
                    break;
                default:
                    await IntentNone(stepContext, luisResult, cancellationToken);
                    break;
            }
            return await stepContext.NextAsync(null, cancellationToken);

        }

        private async Task<DialogTurnResult> InitialProcess(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var luisResult = await _luisService._luisRecognizer.RecognizeAsync(stepContext.Context, cancellationToken);
            return await ManageIntentions(stepContext, luisResult, cancellationToken);

        }
        private async Task<DialogTurnResult> FinalProcess(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken); 
        }


        private Task IntentVerCita(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private Task<DialogTurnResult> IntentCrearCita(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private async Task IntentNone(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("No entiendo lo que dice", cancellationToken: cancellationToken);

        }

        private Task<DialogTurnResult> IntentCalificar(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private async  Task IntentVerCentroContacto(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            string phoneDetail = $"Nuestros numeros de atencion son los siguientes" +
                $":{Environment.NewLine} 👌 3197210595 {Environment.NewLine}" +
                $"3197210101";


            string addressDetail = $"😚 Estamos Ubicados en {Environment.NewLine}" +
                $"Cl 39 # 14-85 ";

            await stepContext.Context.SendActivityAsync(phoneDetail,cancellationToken: cancellationToken);
            await Task.Delay(1000);
            await stepContext.Context.SendActivityAsync(addressDetail,cancellationToken: cancellationToken);
        }

        private  async Task IntentVerOpciones(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            //await stepContext.Context.SendActivityAsync("Aqui tengo mis opciones", cancellationToken: cancellationToken);

            await stepContext.Context.SendActivityAsync("Aquí tengo mis opciones", cancellationToken: cancellationToken);
            await MainOptionsCard.Toshow(stepContext, cancellationToken);
        }

        private  async Task IntentDespedir(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("espero verte pronto", cancellationToken: cancellationToken);

        }

        private async Task IntentAgradecer(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("No hay de que", cancellationToken: cancellationToken);

        }

        private  async Task IntentSaludar(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Hola que gusto verte, cómo te puedo ayudar?", cancellationToken: cancellationToken);

        }

       
    }
}
