using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BotClinic.Dialogs.CreateAppointment
{
    public class CreateAppointmentDialog : ComponentDialog
    {

        public CreateAppointmentDialog()
        {

            var waterfallStep = new WaterfallStep[]
            {
                SetPhone,
                SetFullName,
                SetEmail,
                SetDate,
                SetTime,
                Confirmation,
                FinalProcess
            };
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallStep));
            AddDialog(new TextPrompt(nameof(TextPrompt)));


        }

        private async Task<DialogTurnResult> FinalProcess(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            try
            {
                var userConfirmation = stepContext.Context.Activity.Text;

                if (userConfirmation.ToLower().Equals("si"))
                {
                    // SAVE DATABASE

                    await stepContext.Context.SendActivityAsync("Excelente , ya esta registrado.", cancellationToken: cancellationToken);

                }
                else
                {
                    await stepContext.Context.SendActivityAsync("No hay problema, la próxima será.", cancellationToken: cancellationToken);
                }
            }
            catch (Exception e)
            {
                await stepContext.Context.SendActivityAsync(e.InnerException.ToString(), cancellationToken: cancellationToken);
            }

            return await stepContext.ContinueDialogAsync(cancellationToken: cancellationToken);
        }

        private async  Task<DialogTurnResult> SetTime(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var medicalDate = stepContext.Context.Activity.Text;

            return await stepContext.PromptAsync(
                nameof(TextPrompt),
                new PromptOptions
                {
                    Prompt = CreateButtonsTime()
                },
                cancellationToken

                );


        }

        private Activity CreateButtonsTime()
        {
            var reply = MessageFactory.Text("Ahora selecciona la hora:");

            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction(){Title = "9", Value = "9", Type = ActionTypes.ImBack},
                    new CardAction(){Title = "10", Value = "10", Type = ActionTypes.ImBack},
                    new CardAction(){Title = "11", Value = "11", Type = ActionTypes.ImBack},
                    new CardAction(){Title = "15", Value = "15", Type = ActionTypes.ImBack},
                    new CardAction(){Title = "16", Value = "16", Type = ActionTypes.ImBack},
                    new CardAction(){Title = "17", Value = "17", Type = ActionTypes.ImBack},
                    new CardAction(){Title = "18", Value = "18", Type = ActionTypes.ImBack}
                }
            };
            return reply as Activity;
        }

        private async Task<DialogTurnResult> Confirmation(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var medicalTime = stepContext.Context.Activity.Text;
            //medicalAppointment.time = int.Parse(medicalTime);

            return await stepContext.PromptAsync(
              nameof(TextPrompt),
              new PromptOptions { Prompt = CreateButtonsConfirmation() },
              cancellationToken
            );

        }

        private Activity CreateButtonsConfirmation()
        {

            var reply = MessageFactory.Text("¿Confirmas la creación de esta cita médica?");

            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction(){Title = "Si", Value = "Si", Type = ActionTypes.ImBack},
                    new CardAction(){Title = "No", Value = "No", Type = ActionTypes.ImBack}
                }
            };
            return reply as Activity;
        }

        private async Task<DialogTurnResult> SetDate(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userEmail = stepContext.Context.Activity.Text;

            string text = $"Ahora ingresa la fecha de la cita medica" +
                $" en el siguien formato {Environment.NewLine}" +
                $"dd/mm/yyyy";


            return await stepContext.PromptAsync(
                nameof(TextPrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text
                ("Ahora ingresa tu fecha tentativa")
                },
                cancellationToken

                );
        }

        private  async Task<DialogTurnResult> SetEmail(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var userEmail = stepContext.Context.Activity.Text;

            return await stepContext.PromptAsync(
                nameof(TextPrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text
                ("Ahora ingresa tu   correo")
                },
                cancellationToken

                );

        }

        private async Task<DialogTurnResult> SetFullName(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var fullNameUser = stepContext.Context.Activity.Text;

            return await stepContext.PromptAsync(
                nameof(TextPrompt),
                new PromptOptions { Prompt= MessageFactory.Text
                ("Ahora ingresa tu nombre completo")},
                cancellationToken
                
                );

        }

        private async Task<DialogTurnResult> SetPhone(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            return await stepContext.PromptAsync(
                nameof(TextPrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.
                Text("Por favor ingresa tu numero de celular ")
                },
                cancellationToken

                );

        }
    }
}
