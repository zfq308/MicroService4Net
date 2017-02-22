using MicroService4Net.ServiceInternals;

namespace MicroService4Net.QA
{
    class Program
    {
        static void Main(string[] args)
        {
            Test();

            // default port is 8080, if you want to change the port, change the port value.
            // Please note that if you want this to run on port 80, you should run the program as administrator.
            var microService = new MicroService(port: 8080, serviceDisplayName:"[Data Production/API micro-service for QA team]");
            microService.Run(args);



        }


        static void Test()
        {
          
        }
    }




    // if you want to run the service as window servers, you need to add these two empty class
    public class MicroServiceInstaller : ProjectInstaller { }
    public class MicroServiceService : InternalService { }
}
