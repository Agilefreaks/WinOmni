namespace OmniSync
{
    using System.Threading.Tasks;

    public class OmniSyncService
    {
        //public static async Task<RegisterResult> Register(string channel)
        //{
        //    var registerResult = await Task<RegisterResult>.Factory.StartNew(() => new RegisterResult());
            
        //    return registerResult;
        //}
    }

    public class RegisterResult
    {
        object State { get; set; }

        object Data { get; set; }
    }
}
