namespace LegacyApp.Factories
{
    public class UserCreditServiceFactory
    {
        public  UserCreditServiceClient GetUserCreditServiceClient (bool isTest)
        {
            if (isTest)
            {
                return new TestClientCreditService();
            };
            return new UserCreditServiceClient();
        }
    }
}
