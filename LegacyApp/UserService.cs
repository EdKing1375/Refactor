using System;
using LegacyApp.Factories;
using LegacyApp.Interfaces;
namespace LegacyApp
{
    public class UserServicet
    {
        private readonly IClientRepository _clientRepository;
        private readonly bool _isTest;

        public UserService()
        {
            _clientRepository = new ClientRepository();
            _isTest = false;
        }

        public UserService(IClientRepository clientRepository, bool isTest)
        {
            _clientRepository = clientRepository;
            _isTest = isTest;
        }
        // making public so I can test I would prefer to pull this out
        public bool IsUserValid(string firname, string surname, string email, DateTime dateOfBirth, DateTime now)
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

            if (!IsUserValid(firname, surname, email, dateOfBirth, DateTime.Now))
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

            UserCreditCheck(client, user);

            if (user.HasCreditLimit && user.CreditLimit < 500)
            {
                return false;
            }

            // have figure out how to abstract away static class. This so for now I am putting bool for testing sincerio 
            // f I had more time I might try a wrapper function around it with an interface but I may be over engineering it
            if (!_isTest)
            {
                UserDataAccess.AddUser(user);
            }

            return true;
        }

        private void UserCreditCheck(Client client, User user)
        {
            if (client.Name == "VeryImportantClient")
            {
                // Skip credit chek
                user.HasCreditLimit = false;
            }
            else
            {
                // Do credit check and double credit limit
                // the using statement will cause issues using di pattern because of scoping of the using statement 
                // here is where I would add factory  pattern one with the concrete class and the other with a test class
                // I want elavate some of the logic out if the if else because of dry
                // could not get the factory pattern to work here
                user.HasCreditLimit = true;
                var factory = new UserCreditServiceFactory();
                using var userCreditService = factory.GetUserCreditServiceClient(_isTest);
                var creditLimit = userCreditService.GetCreditLimit(user.Firstname, user.Surname, user.DateOfBirth);
                creditLimit = client.Name == "ImportantClient" ? creditLimit * 2 : creditLimit;
                user.CreditLimit = creditLimit;
            }
        }
    }
}
