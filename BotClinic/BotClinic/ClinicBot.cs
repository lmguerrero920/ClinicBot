// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using BotClinic.Common.Models;
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

namespace BotClinic
{
    public class ClinicBot<T> : ActivityHandler where T : Dialog

    {

        private readonly BotState _userstate;
        private readonly BotState _conversationState;
        private readonly Dialog _dialog;
        private readonly IDataBaseService _databaseService;

        public ClinicBot(UserState userstate, ConversationState conversationstate,T dialog, IDataBaseService databaseService)

        {
             
            _userstate= userstate;
            _conversationState = conversationstate;
            _dialog = dialog;
            _databaseService = databaseService;

        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Bienvenido al ChatBot ¿en que puedo ayudarle?"), cancellationToken);
                }
            }
        }

        public  async override Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            await base.OnTurnAsync(turnContext, cancellationToken);
            await _userstate.SaveChangesAsync(turnContext, false, cancellationToken);
            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected async override Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            //var userMessage=  turnContext.Activity.Text;
            //await turnContext.SendActivityAsync($"Usuario escribio : {userMessage}", cancellationToken: cancellationToken);

            await SaveUser(turnContext);
            
            await _dialog.RunAsync(
      turnContext,
      _conversationState.CreateProperty<DialogState>(nameof(DialogState)),
      cancellationToken);
        }

        private async  Task SaveUser(ITurnContext<IMessageActivity> turnContext)
        {
            try
            {
                var userModel = new UserModel();
                userModel.id = turnContext.Activity.From.Id;
                userModel.userNamechannel = turnContext.Activity.From.Name;
                userModel.channel = turnContext.Activity.ChannelId;
                userModel.registerDate = DateTime.Now.Date;

                var user = _databaseService.User.FirstOrDefault(x => x.id == turnContext.Activity.From.Id);
                
                if (user == null)
                {
                    await _databaseService.User.AddAsync(userModel);
                    await _databaseService.SaveAsync();
                }
            }
            catch (Exception ex)
            {

                // Qué ha sucedido
                var mensaje = "Error message: " + ex.Message;

                // Información sobre la excepción interna
                if (ex.InnerException != null)
                {
                    mensaje = mensaje + " Inner exception: " + ex.InnerException.Message;
                }

                // Dónde ha sucedido
                mensaje = mensaje + " Stack trace: " + ex.StackTrace;

                Console.WriteLine(mensaje);
            }
            

        }   
    }
}
