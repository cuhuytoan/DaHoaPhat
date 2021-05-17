using CMS.Data.ModelEntity;
using CMS.Services.RepositoriesBase;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Website.NotiHub
{
    public class NotificationHubs : Hub
    {
        private RepositoryWrapper Repository;
        private static List<ConnectedUser> connectedUsers = new List<ConnectedUser>();
        public NotificationHubs(RepositoryWrapper _repository)
        {
            Repository = _repository;
        }
        public async Task SendAllNotification(string userId, string subject)
        {
            var profile = await Repository.AspNetUsers.AspNetUserProfilesGetByUserId(userId);
            if (profile != null)
            {
                if ((bool)profile.AllowNotifyApp)
                {
                    await Clients.All.SendAsync("ReceiveMessage", userId, subject);
                }
                if ((bool)profile.AllowNotifyEmail)
                {
                    await Repository.Setting.SendMail("Thông báo từ hệ thống", profile.Email, profile.FullName, subject, subject);
                }
                if ((bool)profile.AllowNotifySms)
                {

                }
            }

        }

        public async Task SendNotification(string userId, string subject, string content, string url, string imageUrl)
        {
            var userIdentifier = (from _connectedUser in connectedUsers
                                  where _connectedUser.Name == userId
                                  select _connectedUser.UserIdentifier).FirstOrDefault();

            var profile = await Repository.AspNetUsers.AspNetUserProfilesGetByUserId(userId);
            if (profile != null)
            {
                //save in user noti
                var model = new UserNotify();
                model.AspNetUsersId = userId;
                model.Subject = subject;
                model.Content = content;
                model.Url = url;
                model.ImageUrl = imageUrl;
                await Repository.UserNoti.UserNotiCreateNew(model);
                if ((bool)profile.AllowNotifyApp)
                {
                    try
                    {
                        await Clients.User(userIdentifier).SendAsync("ReceiveMessage", userId, subject, content, url, imageUrl);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                if ((bool)profile.AllowNotifyEmail)
                {
                    await Repository.Setting.SendMail("Thông báo từ hệ thống", profile.Email, profile.FullName, subject, content);
                }
                if ((bool)profile.AllowNotifySms)
                {

                }
            }

        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {

            var user = connectedUsers.Where(cu => cu.UserIdentifier == Context.UserIdentifier).FirstOrDefault();

            var connection = user.Connections.Where(c => c.ConnectionID == Context.ConnectionId).FirstOrDefault();
            var count = user.Connections.Count;

            if (count == 1) // A single connection: remove user
            {
                connectedUsers.Remove(user);

            }
            if (count > 1) // Multiple connection: Remove current connection
            {
                user.Connections.Remove(connection);
            }

            var list = (from _user in connectedUsers
                        select new { _user.Name }).ToList();

            //await Clients.All.SendAsync("ReceiveInitializeUserList", list);

            //await Clients.All.SendAsync("MessageBoard",
            //           $"{Context.User.Identity.Name}  has left");



        }
        public async Task SendGroupNotification(string userId, string subject)
        {
            var profile = await Repository.AspNetUsers.AspNetUserProfilesGetByUserId(userId);
            if (profile != null)
            {
                if ((bool)profile.AllowNotifyApp)
                {
                    await Clients.Group("SignalR Users").SendAsync("ReceiveMessage", userId, subject);
                }
                if ((bool)profile.AllowNotifyEmail)
                {
                    await Repository.Setting.SendMail("Thông báo từ hệ thống", profile.Email, profile.FullName, subject, subject);
                }
                if ((bool)profile.AllowNotifySms)
                {

                }
            }

        }
        public override async Task OnConnectedAsync()
        {
            var user = connectedUsers.Where(cu => cu.UserIdentifier == Context.UserIdentifier).FirstOrDefault();

            if (user == null) // User does not exist
            {
                ConnectedUser connectedUser = new ConnectedUser
                {
                    UserIdentifier = Context.UserIdentifier,
                    Name = Context.User.Identity.Name,
                    Connections = new List<Connection> { new Connection { ConnectionID = Context.ConnectionId } }
                };

                connectedUsers.Add(connectedUser);
            }
            else
            {
                user.Connections.Add(new Connection
                {
                    ConnectionID = Context.ConnectionId
                });
            }


        }
    }


    public class ConnectedUser
    {
        public string Name { get; set; }
        public string UserIdentifier { get; set; }

        public List<Connection> Connections { get; set; }
    }
    public class Connection
    {
        public string ConnectionID { get; set; }

    }
}

