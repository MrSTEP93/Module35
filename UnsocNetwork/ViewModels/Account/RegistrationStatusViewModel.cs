namespace UnsocNetwork.ViewModels.Account
{
    public class RegistrationStatusViewModel
    {
        private bool _isRegistered { get; set; }

        private string UserName { get; set; }

        public string[] WelcomeMessage { get; set; }

        public RegistrationStatusViewModel(bool isRegistered = false) : this("undef", isRegistered) { }

        public RegistrationStatusViewModel(string userName, bool isRegistered)
        {
            _isRegistered = isRegistered;
            UserName = userName;
            WelcomeMessage = new string[2];
            if (_isRegistered)
            {
                WelcomeMessage[0] = $"Welcome to our site, {UserName}!";
                WelcomeMessage[1] = $"You can log in with your email and password";
            }
            else
            {
                WelcomeMessage[0] = $"Lets join!!!";
                WelcomeMessage[1] = "To our amazing site (jeje)";
            }

        }
    }
}
