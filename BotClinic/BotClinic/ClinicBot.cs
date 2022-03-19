// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BotClinic
{
    public class ClinicBot<T> : ActivityHandler where T : Dialog

    {

        private readonly BotState _userstate;
        private readonly BotState _conversationState;
        private readonly Dialog _dialog;

        public ClinicBot(UserState userstate, ConversationState conversationstate,T dialog)

        {
             
            _userstate= userstate;
            _conversationState = conversationstate;
            _dialog = dialog;

        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Hello world!"), cancellationToken);
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
            await _dialog.RunAsync(
      turnContext,
      _conversationState.CreateProperty<DialogState>(nameof(DialogState)),
      cancellationToken);
        }
    }
}
