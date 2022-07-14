using System;
using LegacyApp.Interfaces;
namespace LegacyApp
{
    public class UserServiceRefactor
    {
        private readonly IClientRepository _clientRepository;
        private readonly IUserCreditServiceChannel _userCreditService;

        public UserServiceRefactor()
        {
            _clientRepository = new ClientRepository();
        }

        public UserServiceRefactor(IClientRepository clientRepository, IUserCreditServiceChannel userCreditService)
        {
            _clientRepository = clientRepository;
            _userCreditService = userCreditService;
        }

        public static bool IsUserValid(string firname, string surname, string email, DateTime dateOfBirth, DateTime now)
        {
            bool isValidUser = true;
            if (string.IsNullOrEmpty(firname) || string.IsNullOrEmpty(surname))
            {
                return false;
            }

            if (!email.Contains("@") && !email.Contains("."))
            {
                return false;
            }

            int age = now.Year - dateOfBirth.Year;

            if (now.Month < dateOfBirth.Month || (now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day)) age--;

            if (age < 21)
            {
                return false;
            }

            return isValidUser;
        }

        public bool AddUser(string firname, string surname, string email, DateTime dateOfBirth, int clientId)
        {
          
            if(!IsUserValid(firname, surname, email, dateOfBirth, DateTime.Now))
            {
                return false;
            }

            var client = _clientRepository.GetById(clientId);

            var user = new User
            {
                Client = client,
                DateOfBirth = dateOfBirth,
                EmailAddress = email,
                Firstname = firname,
                Surname = surname
            };

            if (client.Name == "VeryImportantClient")
            {
                // Skip credit chek
                user.HasCreditLimit = false;
            }
            else if (client.Name == "ImportantClient")
            {
                // Do credit check and double credit limit
                // the using statement will cause issues using di pattern because of scoping of the using statement 
                // here is where I would add factory  pattern one with the concrete class and the other with a test class
                user.HasCreditLimit = true;
                using (var userCreditService = new UserCreditServiceClient())
                {
                    var creditLimit = userCreditService.GetCreditLimit(user.Firstname, user.Surname, user.DateOfBirth);
                    creditLimit = creditLimit * 2;
                    user.CreditLimit = creditLimit;
                }
            }
            else
            {
                // Do credit check
                user.HasCreditLimit = true;
                using (var userCreditService = new UserCreditServiceClient())
                {
                    var creditLimit = userCreditService.GetCreditLimit(user.Firstname, user.Surname, user.DateOfBirth);
                    user.CreditLimit = creditLimit;
                }
            }

            if (user.HasCreditLimit && user.CreditLimit < 500)
            {
                return false;
            }

            UserDataAccess.AddUser(user);

            return true;
        }
    }
}
