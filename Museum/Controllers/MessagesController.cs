using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Text.RegularExpressions;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Museum.Models;
using Museum.Services;

namespace Museum
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private readonly IFirebaseService _service;

        public MessagesController()
        {
            _service = new FirebaseService(Environment.GetEnvironmentVariable("DatabaseEndpoint", EnvironmentVariableTarget.User));
        }

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            if (activity.Type == ActivityTypes.Message)
            {
                var msg = activity.Text;

                if(Regex.IsMatch(msg, "(?<=have we been to )(?<place>[^?]+)", RegexOptions.IgnoreCase))
                {
                    var place = Regex.Match(msg, @"(?<=have we been to )(?<place>[^?]+)", RegexOptions.IgnoreCase)?.Groups["place"]?.Value ?? "";

                    if (!string.IsNullOrWhiteSpace(place))
                    {
                        var visitedPlaces = await _service.GetAllVisitedLocationsAsync();
                        var visitedPlace = visitedPlaces.FirstOrDefault(r => string.Equals(r.Location, place, StringComparison.OrdinalIgnoreCase));

                        if(visitedPlace != null)
                        {
                            await 
                        }
                    }
                }
                // If the user mentions anything related to the list, give it to them
                else if (Regex.IsMatch(msg, "show|all|list all", RegexOptions.IgnoreCase))
                {
                    await ReplyWithPlaceListAsync(activity, connector);
                }
                else if (Regex.IsMatch(msg, "who's next|who is next|whose (pick|turn) is it", RegexOptions.IgnoreCase))
                {
                    await ReplyWithNextMemberToChoose(activity, connector);
                }
                else
                {
                    await ReplyWithDefaultMessageAsync(activity, connector);
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private async Task<ResourceResponse> ReplyWithNextMemberToChoose(Activity activity, ConnectorClient connector)
        {
            try
            {
                var lastPlaceVisited = await _service.GetLastVisitedLocationAsync();
                var members = await _service.GetAllMembersAsync();
                var currentMember = Array.IndexOf(members, lastPlaceVisited?.PickedBy ?? "");

                var nextMember = members[(currentMember + 1) % members.Length];
                var nextMonth = lastPlaceVisited?.Date.AddMonths(1) ?? DateTime.Now.AddMonths(1);

                var replyMessage = string.Format(Messages.NextChooserFormattingMessage, nextMember, nextMonth.ToString("MMMM"));
                var reply = activity.CreateReply(replyMessage);

                return await connector.Conversations.ReplyToActivityAsync(reply);
            }
            catch
            {
                var reply = activity.CreateReply("I am not sure who's next to pick. Try again later.");
                return await connector.Conversations.ReplyToActivityAsync(reply);
            }
        }

        private async Task<ResourceResponse> ReplyWithVisitedPlaceAsync(string place, Activity activity, ConnectorClient connector)
        {

        }

        private async Task<ResourceResponse> ReplyWithPlaceListAsync(Activity activity, ConnectorClient connector)
        {
            var replyMessage = await _service.GetPreviouslyVisitedLocationsMessageAsync();
            var reply = activity.CreateReply(replyMessage);

            return await connector.Conversations.ReplyToActivityAsync(reply);
        }

        private async Task<ResourceResponse> ReplyWithDefaultMessageAsync(Activity activity, ConnectorClient connector)
        {
            var reply = activity.CreateReply(Messages.DefaultResponseMessage);
            return await connector.Conversations.ReplyToActivityAsync(reply);
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}