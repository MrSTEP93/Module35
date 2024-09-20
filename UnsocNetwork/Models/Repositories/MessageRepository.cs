using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UnsocNetwork.Models.Repositories
{
    public class MessageRepository : Repository<Message>
    {
        public MessageRepository(AppDbContext db) : base(db)
        {

        }

        public void SendMessage(User sender, User recipient, string text)
        {
            SendMessage(sender.Id, recipient.Id, text);
        }

        public void SendMessage(string senderId, string recipientId, string text)
        {
            var newMessage = new Message()
            {
                SenderId = senderId,
                RecipientId = recipientId,
                Text = text,
                Date = DateTime.Now
            };

            Create(newMessage);
        }

        public List<Message> GetMessages(User sender, User recipient)
        {
            return GetMessages(sender.Id, recipient.Id);
        }

        public List<Message> GetMessages(string senderId, string recipientId)
        {
            Set.Include(x => x.Recipient);
            Set.Include(x => x.Sender);

            var from = Set.AsEnumerable().Where(x => x.SenderId == senderId && x.RecipientId == recipientId).ToList();
            var to = Set.AsEnumerable().Where(x => x.SenderId == recipientId && x.RecipientId == senderId).ToList();

            var resultList = new List<Message>();
            resultList.AddRange(from);
            resultList.AddRange(to);
            resultList = resultList.OrderBy(x => x.Date).ToList();
            /*
            foreach (var message in resultList)
            {
                if (message.Date == DateTime.Now.Date)
                {
                    message.Date = DateTime.Now.TimeOfDay;
                }
            }
            */
            return resultList;
        }
    }
}
