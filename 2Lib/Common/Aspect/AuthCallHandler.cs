using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;

namespace Wanda.BusinessIndicators.Common
{

    public class AuthCallHandler : ICallHandler
    {


        public AuthCallHandler() { }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {

            string methodName = input.MethodBase.Name;
            if (methodName == "Test")
            {
                throw new UnAuthException();
            }
            else
            {
                IMethodReturn result = getNext()(input, getNext);

                return result;
            }
        }

        public int Order { get; set; }
    }


    public class AuthHandlerAttribute : HandlerAttribute
    {

        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            return new AuthCallHandler() { Order = this.Order };
        }
    }

    public class UnAuthException : ApplicationException
    {
        public UnAuthException()
            : base("unauthed！")
        {

        }
    }

}
