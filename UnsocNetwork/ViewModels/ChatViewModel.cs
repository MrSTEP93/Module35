using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UnsocNetwork.Models;

namespace UnsocNetwork.ViewModels
{
    public class ChatViewModel
    {
        public List<Message> Messages { get; set; }

        public string RecepientId { get; set; }

        public string Text { get; set; }

    }
}
