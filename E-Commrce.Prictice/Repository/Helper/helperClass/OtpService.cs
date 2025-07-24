using E_Commrce.Prictice.Repository.Helper.helperInterface;

namespace E_Commrce.Prictice.Repository.Helper.helperClass
{
    public class OtpService : IOtpService
    {
        public string GenerateOtp()
        {
           Random rnd = new Random();
            return rnd.Next(1000,9999).ToString();
        }
    }
}
