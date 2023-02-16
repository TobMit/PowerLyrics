using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfChat.MVVM.Model;

namespace WpfChat.MVVM.ViewModel
{
    internal class MainViewModel
    {
        public ObservableCollection<MessageModel> Messages { get; set; }
        public ObservableCollection<ContactModel> Contacts { get; set; }

        public MainViewModel()
        {
            Messages = new ObservableCollection<MessageModel>();
            Contacts = new ObservableCollection<ContactModel>();

            //ideme naplnit kontajnery len pre viditelnost

            Messages.Add(new MessageModel
            {
                Username = "Alison",
                UsernameColor = "#409aff",
                ImageSource = "https://images.alphacoders.com/475/475526.jpg",
                Message = "Test n: 0",
                Time = DateTime.Now,
                IsNativeOrigin = false,
                FirstMessage = true
            });

            for (int i = 0; i < 3; i++)
            {
                Messages.Add(new MessageModel
                {
                    Username = "Alison",
                    UsernameColor = "#409aff",
                    ImageSource = "https://images.alphacoders.com/475/475526.jpg",
                    Message = "Test n: " + i + 1,
                    Time = DateTime.Now,
                    IsNativeOrigin = false,
                    FirstMessage = false
                });
            }

            for (int i = 0; i < 4; i++)
            {
                Messages.Add(new MessageModel
                {
                    Username = "Bunny",
                    UsernameColor = "#409aff",
                    ImageSource = "https://images.alphacoders.com/475/475526.jpg",
                    Message = "Test n: " + i + 1,
                    Time = DateTime.Now,
                    IsNativeOrigin = true,
                });
            }

            Messages.Add(new MessageModel
            {
                Username = "Bunny",
                UsernameColor = "#409aff",
                ImageSource = "https://images.alphacoders.com/475/475526.jpg",
                Message = "Last",
                Time = DateTime.Now,
                IsNativeOrigin = true,
            });

            for (int i = 0; i < 5; i++)
            {
                Contacts.Add(new ContactModel
                {
                    // toto je string interpolation
                    Username = $"Allison {i}",
                    ImageSource = "https://rare-gallery.com/uploads/posts/536732-anonymous-wallpaper.jpg",
                    Messages = Messages
                });
            }
        }
    }
}
