using BotClinic.Common.Models;
using BotClinic.Common.Models.MedicalAppointment;
using BotClinic.Infrastructure.Data;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BotClinic.Dialogs.CreateAppointment
{
    public class CreateAppointmentDialog : ComponentDialog
    {
        private readonly IDataBaseService  _dataBaseService;

        public static UserModel newUserModel = new UserModel();

        public static MedicalAppointmentModel medicalAppointmentModel =
            new MedicalAppointmentModel();

        public CreateAppointmentDialog(IDataBaseService dataBaseService)
        {
            _dataBaseService = dataBaseService;

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

                    string userId = stepContext.Context.Activity.From.Id;
                    var userModel = await _dataBaseService.User.FirstOrDefaultAsync(
                        x => x.id == userId);

                    //UPDATE USER
                    userModel.phone = newUserModel.phone;
                    userModel.fullName = newUserModel.fullName;
                    userModel.email = newUserModel.email;

                    _dataBaseService.User.Update(userModel);

                    await _dataBaseService.SaveAsync();

                    //save medical appoirntment
                    medicalAppointmentModel.id = Guid.NewGuid().ToString();
                    medicalAppointmentModel.idUser = userId;

                    await _dataBaseService.MedicalAppointment.AddAsync(medicalAppointmentModel);
                    await _dataBaseService.SaveAsync();



                    await stepContext.Context.SendActivityAsync("Excelente , " +
                        "ya esta registrada su cita.", cancellationToken: cancellationToken);


                    //SHOW SUMMARY
                    string summaryMedical = $"Para :{userModel.fullName} "+
                        $"{Environment.NewLine}⌛ Telefono : {userModel.phone}"+
                        $"{Environment.NewLine} ⌛ Email : {userModel.email}+" +
                        $"{Environment.NewLine} ⌛ Fecha : {medicalAppointmentModel.date}"
                        + $"{Environment.NewLine} ⌛ Hora : {medicalAppointmentModel.time}";

                    await stepContext.Context.SendActivityAsync(summaryMedical, cancellationToken
                        : cancellationToken);
                    await Task.Delay(1000);

                    await stepContext.Context.SendActivityAsync("¿En que mas puedo " +
                        "ayudarte?" , cancellationToken: cancellationToken);

                    medicalAppointmentModel = new MedicalAppointmentModel();

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
             
            medicalAppointmentModel.date = Convert.ToDateTime(medicalDate);

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
            medicalAppointmentModel.time = int.Parse(medicalTime);

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

            newUserModel.email = userEmail;

            string text = $"Ahora ingresa la fecha de la cita medica en el siguien formato {Environment.NewLine} dd/mm/yyyy";


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

            var fullNameUser = stepContext.Context.Activity.Text;
            newUserModel.fullName = fullNameUser;


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
            var userPhone = stepContext.Context.Activity.Text;

            newUserModel.phone = userPhone;

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
