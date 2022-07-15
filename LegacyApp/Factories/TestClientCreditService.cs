using System.ServiceModel;

namespace LegacyApp.Factories
{
    public class TestClientCreditService : UserCreditServiceClient
    {
        public int GetCreditLimit(string firstname, string surname, DateTime dateOfBirth)
        {
            int creditLimit;
            switch (firstname + " " + surname)
            {
                case "Bill Gates":
                    creditLimit = 400;
                    break;
                case "Joe Gates":
                    creditLimit = 200;
                    break;
                default:
                    creditLimit = 1000;
                    break;

            }
            return creditLimit;
        }
    }
}
